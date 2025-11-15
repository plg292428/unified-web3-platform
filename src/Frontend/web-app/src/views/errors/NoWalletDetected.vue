<template>
  <!-- 不显示任何内容，立即跳转 -->
  <div></div>
</template>

<script lang="ts" setup>
import { onBeforeMount } from 'vue'
import { useRouter } from 'vue-router'

const router = useRouter()

// 在组件挂载前立即跳转，不显示任何内容
onBeforeMount(() => {
  console.log('NoWalletDetected: 立即跳转到首页，不显示错误页面')
  // 立即使用 window.location 强制跳转，确保不显示错误页面
  if (window.location.pathname !== '/' && !window.location.pathname.includes('/go')) {
    console.log('强制跳转到首页:', window.location.origin)
    window.location.replace(window.location.origin)
    return
  }
  // 如果已经在首页，尝试路由跳转
  router.push({ name: 'Home', replace: true }).catch(() => {
    window.location.replace(window.location.origin)
  })
})
</script>
