<script setup lang="ts">
import {onMounted, reactive} from "vue";

const style = reactive(
	{
		opacity: 0
	})

onMounted(() =>
          {
	          setTimeout(() =>
	                     {
		                     style.opacity = 1
	                     }, 1)
          })
</script>

<template>
	<div class="animation" :style="`opacity:${style.opacity}`">
		<slot></slot>
	</div>
</template>

<style scoped>
.animation {
	transition-duration: 0.65s;
	display: flex;
	height: 100%;
	width: 100%;
}
</style>
