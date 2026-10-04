import { defineConfig, loadEnv } from 'vite';
import path from 'node:path';
import react from '@vitejs/plugin-react';
import tailwindcss from '@tailwindcss/vite';

const gitCommitSha = process.env.GIT_COMMIT_SHA || undefined;

const version = gitCommitSha?.slice(0, 7) ?? process.env.npm_package_version;

// https://vitejs.dev/config/
export default defineConfig(({ mode }) => {
    const env = loadEnv(mode, process.cwd(), '');

    return {
        plugins: [react(), tailwindcss()],
        define: {
            'import.meta.env.VITE_APP_VERSION': JSON.stringify(version),
        },
        resolve: {
            alias: {
                '@': path.resolve(__dirname, './src'),
            },
        },
        server: {
            proxy: {
                '/api': {
                    target: env.API_URL,
                    changeOrigin: true,
                    secure: false,
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
