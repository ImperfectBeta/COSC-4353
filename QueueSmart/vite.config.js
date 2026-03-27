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
			'/api': 'http://localhost:5201'
		}
	}
});
