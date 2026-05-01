import { sveltekit } from '@sveltejs/kit/vite';
import { defineConfig } from 'vite';
import tailwindcss from '@tailwindcss/vite'

export default defineConfig({
    plugins: [sveltekit(), tailwindcss()],
    ssr: {
        noExternal: ['@lucide/svelte']
    },
    server: {
        proxy: {
            '/api': {
                target: 'http://localhost:5201',
                changeOrigin: true,
                secure: false
            }
        }
    }
});