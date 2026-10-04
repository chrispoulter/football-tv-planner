import { defineConfig, loadEnv } from 'vite';
import react from '@vitejs/plugin-react';
import tailwindcss from '@tailwindcss/vite';
import path from 'node:path';

const gitCommitSha = process.env.GIT_COMMIT_SHA || undefined;

const version = gitCommitSha?.slice(0, 7) ?? process.env.npm_package_version;

// https://vite.dev/config/
export default defineConfig(({ mode }) => {
    const env = loadEnv(mode, process.cwd(), '');

    return {
        define: {
            'import.meta.env.VITE_APP_VERSION': JSON.stringify(version),
        },
        plugins: [react(), tailwindcss()],
        resolve: {
            alias: {
                '@': path.resolve(import.meta.dirname, './src'),
            },
        },
        server: {
            proxy: {
                '/api': {
                    target: env.API_URL,
                    changeOrigin: true,
                    configure: (proxy) => {
                        proxy.on('proxyReq', (proxyReq, req) => {
                            proxyReq.setHeader(
                                'X-FootballTvPlanner-Host',
                                req.headers.host ?? ''
                            );
                            proxyReq.setHeader(
                                'X-FootballTvPlanner-Proto',
                                'http'
                            );
                        });
                    },
                },
            },
        },
    };
});
