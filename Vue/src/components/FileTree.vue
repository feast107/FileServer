<script setup lang="ts">
import {onMounted, reactive, watch} from "vue";
import request from "../utils/request.ts";
import {FileSystemInfo} from "../models/FileSystemInfo.ts";

const data = reactive(
	{
		deep: [] as FileSystemInfo[],
		tree: [] as FileSystemInfo[]
	})
watch(() => data.deep, (value, oldValue, _) => {
	console.log(value)
	console.log(oldValue)
})

async function init() {
	data.deep = [];
	data.tree = await request.tree();
}

function back(index : number) : void {
	const item = data.deep[index];
	while (data.deep.length > index) {
		data.deep.pop()
	}
	enter(item)
}

async function enter(item : FileSystemInfo) {
	if (item.isFile) {
		window.open(request.url(item.path))
	} else {
		data.tree = await request.tree(item.path)
		data.deep.push(item)
	}
}

onMounted(async () => {
	await init()
})
</script>

<template>
	<el-card style="margin: 16px;">
		<template #header>
			<el-scrollbar>
				<el-breadcrumb separator="/" style="display: flex;width: fit-content">
					<el-breadcrumb-item>
						<el-button @click="init" :text="true" :type="'primary'">此电脑</el-button>
					</el-breadcrumb-item>
					<el-breadcrumb-item v-for="(item,index) in data.deep">
						<el-button :text="true" :type="'primary'" @click="()=> back(index)">{{ item.display }}
						</el-button>
					</el-breadcrumb-item>
				</el-breadcrumb>
			</el-scrollbar>
		</template>
		<el-table :data="data.tree" style="height: 100%;" empty-text="Empty">
			<el-table-column label="名称" fixed>
				<template #default="item : { row : FileSystemInfo }">
					<el-button @click="()=> enter(item.row)" :text="true" :type="item.row.isFile ? 'primary' : ''">
						{{ item.row.display }}
					</el-button>
				</template>
			</el-table-column>
			<el-table-column label="类型">
				<template #default="item">
					<span v-if="item.row.isFile">{{ item.row.extension }}</span>
				</template>
			</el-table-column>
		</el-table>
	</el-card>
</template>

<style>
.el-card__body {
	height: calc(100% - 100px);
}

.el-scrollbar__view {
	height: 100%;
	width: fit-content;
}
</style>
