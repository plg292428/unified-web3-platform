<template>
  <v-container class="fill-height">
    <v-responsive class="align-center text-center fill-height">
      <v-progress-circular color="primary" indeterminate size="64"></v-progress-circular>
      <div class="py-4"></div>
      <div class="text-body-1">
        Redirecting to home page...
      </div>
    </v-responsive>
  </v-container>
</template>

<script lang="ts" setup>
import { onMounted, onBeforeMount } from 'vue'
import { useRouter } from 'vue-router'

const router = useRouter()

// 在组件挂载前就尝试跳转
onBeforeMount(() => {
  console.log('NoWalletDetected 组件准备挂载，立即跳转到首页')
  // 立即使用 window.location 强制跳转，不等待路由
  if (window.location.pathname !== '/' && !window.location.pathname.includes('/go')) {
    console.log('强制跳转到首页:', window.location.origin)
    window.location.href = window.location.origin
  }
})

onMounted(async () => {
  console.log('NoWalletDetected 组件已挂载，准备跳转到首页')
  // 双重保险：再次尝试跳转
  try {
    // 先尝试路由跳转
    await router.push({ name: 'Home' }).catch(() => {
      // 如果路由跳转失败，使用 window.location 强制跳转
      console.log('路由跳转失败，使用 window.location 强制跳转')
      window.location.href = window.location.origin
    })
    console.log('已跳转到首页')
  } catch (error) {
    console.error('跳转失败:', error)
    // 如果路由跳转失败，使用 window.location 强制跳转
    window.location.href = window.location.origin
  }
})
</script>
