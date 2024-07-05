import {defineConfig} from 'vite'
import vue from '@vitejs/plugin-vue'

export default defineConfig(
	{
		base : 'view',
		plugins: [vue()],
		server : {
			proxy: {
				'/tree': {
					target: "http://localhost:9567/tree",
				},
				'/upload': {
					target: "http://localhost:9567/upload",
				}
			},
		}
	})
