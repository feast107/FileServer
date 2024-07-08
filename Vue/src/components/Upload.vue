<script setup lang="ts">
import {UploadFilled} from '@element-plus/icons-vue'
import {UploadFile} from "element-plus";
import {reactive} from "vue";

const data = reactive(
	{
		target: '/upload/'
	})

function onChange(file : UploadFile) {
	data.target = '/upload/' + file.name
}

</script>

<template>
	<el-card style="margin: 16px">
		<template #header>
			上传
		</template>
		<el-upload multiple drag :action="data.target" :on-change="onChange">
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
