<script setup lang="ts">
import {UploadFilled} from '@element-plus/icons-vue'
import {UploadRequestOptions} from "element-plus";
import {reactive} from "vue";

const data = reactive(
	{
		target: '/upload/'
	})


function httpRequest(options : UploadRequestOptions) {
	const ret = new XMLHttpRequest()
	const form = new FormData();
	form.append('file', options.file);
	ret.open(options.method, options.action);
	ret.send(form)
	console.log(options)
	return ret;
}
</script>

<template>
	<el-card style="margin: 16px">
		<template #header>
			上传
		</template>
		<el-upload multiple drag :action="data.target" :http-request="httpRequest" name="file">
			<el-icon class="el-icon--upload">
				<upload-filled />
			</el-icon>
			<div class="el-upload__text">
				将文件丢至此处或 <em>点击上传</em>
			</div>
			<template #tip>
				<div class="el-upload__tip">
				</div>
			</template>
		</el-upload>
	</el-card>
</template>

<style scoped>

</style>
