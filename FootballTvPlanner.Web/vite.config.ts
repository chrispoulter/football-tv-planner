import { defineConfig, loadEnv } from 'vite';
import path from 'node:path';
import react from '@vitejs/plugin-react';
import tailwindcss from '@tailwindcss/vite';

// https://vitejs.dev/config/
export default defineConfig(({ mode }) => {
    const env = loadEnv(mode, process.cwd(), '');

    return {
        plugins: [react(), tailwindcss()],
        define: {
            'import.meta.env.VITE_APP_VERSION': JSON.stringify(
                env.npm_package_version
            ),
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
                    rewrite: (path) => path.replace(/^\/api/, ''),
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
                            proxyReq.setHeader(
                                'X-FootballTvPlanner-Prefix',
                                '/api'
                            );
                        });
                    },
                },
            },
        },
    };
});
