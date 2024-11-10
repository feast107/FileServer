<script setup lang="ts">
import Upload from './components/Upload.vue'
import FileTree from "./components/FileTree.vue";
import {onMounted, provide, reactive} from "vue";
import {Fold} from "@element-plus/icons-vue";
import Animation from "./components/Animation.vue";
import Terminal from "./components/Terminal.vue";
import {TerminalSession} from "./models/TerminalSession.ts";

function collapseChange() {
	/*if (!state.collapse) {
		state.menuStyle = ''
	} else {
		state.menuStyle = `width:${state.menuWidth}px`
	}*/
	state.collapse = !state.collapse
}

export type TabNames = 'fs' | 'tr' | 'up'

export type GlobalState = {
	collapse : boolean,
	main : TabNames,
	menuWidth : number,
	menuIndex : string,
	menuStyle : string,
	openTerminal(path : string) : void,
	terminals : TerminalSession[],
	lastTerminal? : string | null
}

const state = reactive<GlobalState>(
	{
		collapse : false,
		main     : 'fs' as TabNames,
		menuIndex: '0',
		menuWidth: 0,
		menuStyle: '',
		openTerminal(path : string) {
			const id = new Date().getTime().toString()
			this.main = 'tr'
			const session = reactive(new TerminalSession({id, path}));
			session.open.then(() =>
			                  {
								  session.ready = true
				                  session.send(path)
			                  })
			session.onMessage((s) => {
				console.log(s)
				session.payloads.push(s)
			})
			this.terminals.push(session as TerminalSession)
			this.lastTerminal = id
			this.menuIndex = '3'
		},
		terminals   : [],
		lastTerminal: null
	})

onMounted(() => {
	state.menuWidth = document.getElementById('menu')?.clientWidth ?? 120
})

provide("state", state)
</script>

<template>
	<el-container style="height: 100%;background-color: white">
		<el-menu ref="menu" id="menu" :style="state.menuStyle" v-model="state.menuIndex"
		         class="el-menu-vertical-demo"
		         :collapse="state.collapse">
			<el-menu-item index="1" @click="collapseChange">
				<el-icon>
					<Expand v-if="state.collapse" />
					<Fold v-else />
				</el-icon>
				<template #title>
					<span>{{ (state.collapse ? '展开' : '收起') }}</span>
				</template>
			</el-menu-item>
			<el-menu-item index="2" @click="() => state.main = 'fs'">
				<el-icon>
					<FolderOpened />
				</el-icon>
				<template #title>文件系统</template>
			</el-menu-item>
			<el-menu-item index="3" @click="() => state.main = 'up'">
				<el-icon>
					<UploadFilled />
				</el-icon>
				<template #title>上传</template>
			</el-menu-item>
			<el-menu-item index="4" @click="() => state.main = 'tr'">
				<el-icon>
					<Monitor />
				</el-icon>
				<template #title>终端</template>
			</el-menu-item>
		</el-menu>
		<el-main style="height: 100%;width: 100%;">
			<Animation v-if="state.main == 'up'">
				<Upload style="height: 100%;width: 100%;" />
			</Animation>
			<Animation v-if="state.main == 'fs'">
				<FileTree style="height: 100%;width: 100%;" />
			</Animation>
			<Animation v-if="state.main == 'tr'">
				<Terminal style="height: 100%;width: 100%;" />
			</Animation>
		</el-main>
	</el-container>


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
