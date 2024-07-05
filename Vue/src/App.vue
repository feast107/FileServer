<script setup lang="ts">
import Upload from './components/Upload.vue'
import FileTree from "./components/FileTree.vue";
</script>

<template>
	<div style="float: left;width: 100%;
	display: flex;
	height: 100%;
	justify-content: center;
	align-items: stretch">
		<Upload style="width: calc(50% - 32px)"/>
		<el-divider direction="vertical" />
		<FileTree style="width: calc(50% - 32px)"/>
	</div>
</template>

<style scoped>
html, body {
	height: 100%;
	width: 100%;
	margin: 0;
	padding: 0;
}

.el-row {
	margin: 16px;
	height: 100%;
}

.el-divider {
	height: calc(100% - 32px);
	margin-top: 16px;
	margin-bottom: 16px;
}

.logo {
	height: 6em;
	padding: 1.5em;
	will-change: filter;
	transition: filter 300ms;
}

.logo:hover {
	filter: drop-shadow(0 0 2em #646cffaa);
}

.logo.vue:hover {
	filter: drop-shadow(0 0 2em #42b883aa);
}
</style>
