import {defineConfig} from 'vite'
import vue from '@vitejs/plugin-vue'

const proxy = {
	target: "http://127.0.0.1:5310/"
}

export default defineConfig(
	{
		base   : 'view',
		plugins: [vue()],
		server : {
			proxy: {
				'/tree'    : proxy,
				'/upload'  : proxy,
				'/terminal': {
					ws    : true,
					target: proxy.target
				}
			},
		}
	})
