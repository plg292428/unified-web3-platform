<template>
  <!-- 完全不渲染任何内容，立即跳转 -->
</template>

<script lang="ts" setup>
// 立即跳转，不等待任何生命周期
console.log('NoWalletDetected: 立即强制跳转到首页')
// 使用 replace 而不是 href，避免在历史记录中留下错误页面
if (typeof window !== 'undefined') {
  window.location.replace(window.location.origin)
}
</script>
