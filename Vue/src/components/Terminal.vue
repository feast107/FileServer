<script setup lang="ts">
import {inject} from "vue";
import {GlobalState} from "../App.vue";

export type TerminalSession = {
	path : string,
	outputs : string[]
}

const global = inject<GlobalState>('state') ?? {} as GlobalState
</script>

<template>
	<el-tabs type="border-card" v-model="global.lastTerminal">
		<el-tab-pane v-for="item in global.terminals" :name="item.path" :label="item.path">User</el-tab-pane>
	</el-tabs>
</template>

<style scoped>

</style>
