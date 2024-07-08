import {defineConfig} from 'vite'
import vue from '@vitejs/plugin-vue'

export default defineConfig(
	{
		base : 'view',
		plugins: [vue()],
		server : {
			proxy: {
				'/tree': {
					target: "http://127.0.0.1:5310/",
					rewrite(path){
						console.log(path)
						return path
					}
				},
				'/upload': {
					target: "http://127.0.0.1:5310/",
				}
			},
		}
	})
