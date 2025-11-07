<template>
  <v-container class="fill-height">
    <v-responsive class="align-center text-center fill-height">
      <h1 class="text-h5 font-weight-bold text-primary">Error</h1>

      <div class="text-h4 text-grey-darken-1">Page not found.</div>

      <div class="py-4"></div>

      <v-img height="256" src="@/assets/error_404.svg" />

      <div class="py-4"></div>

      <div class="text-body-1 mb-10">We couldn't find the page you're looking for.</div>

      <v-row class="d-flex align-center justify-center">
        <v-col cols="auto">
          <v-btn color="primary" @click="toGoPage"> Go To Home </v-btn>
        </v-col>
      </v-row>
    </v-responsive>
  </v-container>
</template>

<script lang="ts" setup>
import { useRouter } from 'vue-router'

const router = useRouter()

// 返回首页
async function toGoPage() {
  router.push({ name: 'Go' })
}
</script>
