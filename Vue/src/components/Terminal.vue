<script setup lang="ts">
import {inject} from "vue";
import {GlobalState} from "../App.vue";
import {TerminalSession} from "../models/TerminalSession.ts";

const global = inject<GlobalState>('state') ?? {} as GlobalState

function keyup(evt : KeyboardEvent, item : TerminalSession) {
	if ((evt as KeyboardEvent).code == 'Enter') {
		item.send();
		item.input = null
	}
}

function tabRemove(id : string) {
	let index = global.terminals.findIndex(x => x.id == id);
	const session = global.terminals[index];
	session.close()
	global.terminals.splice(index, 1)
	if (session.id != global.lastTerminal) return
	index--;
	if (index < 0) index = 0
	if (global.terminals.length > index) {
		global.lastTerminal = global.terminals[index].id
	}
}
</script>

<template>
	<el-tabs type="border-card" v-model="global.lastTerminal" closable @tab-remove="tabRemove">
		<el-tab-pane class="full" v-for="item in global.terminals" :name="item.id" :label="item.path">
			<el-card class="full" v-loading="!item.ready" style="background-color: #2D3037">
				<el-scrollbar>
					<p class="text" :style="`color:${ payload.type == 'out' ? '#fff6eb' : 'red' }`"
					   v-for="payload in item.payloads">{{ payload.message }}</p>
				</el-scrollbar>
				<template #footer>
					<el-row>
						<el-col :span="20">
							<el-input v-model="item.input" @keyup="(evt : KeyboardEvent)=>keyup(evt,item)"></el-input>
						</el-col>
						<el-col :span="4" style="padding-left: 20px">
							<el-button style="width: 100%;"
							           @click="()=>{ item.send();item.input = null }"
							           type="primary">输入
							</el-button>
						</el-col>
					</el-row>
				</template>
			</el-card>
		</el-tab-pane>
	</el-tabs>
</template>

<style scoped>
.full {
	height: 100%;
	width: 100%;
}
</style>

<style>
.el-tabs__content {
	height: calc(100% - 70px);
}

.text {
	margin: 0;
	color: #fff6eb;
}
</style>
