<template>
  <v-container>
    <v-responsive>
      <div class="mt-2 text-center text-subtitle-1">{{ message?.title }}</div>

      <div class="mt-4 text-caption text-center text-grey-darken-1">
        {{ Filter.formatShotDateTime(new Date(message?.createTime)) }}
      </div>

      <div class="mt-4 mx-2 text-caption" v-html="message?.content"></div>

      <v-card v-if="message?.isActivationCodeMessage" class="mt-4 mx-4 primary-border" variant="flat">
        <v-card-title class="text-subtitle-2"> Activation Code </v-card-title>
        <v-card-text>
          <v-text-field
            readonly
            focused
            base-color="primary"
            color="primary"
            density="compact"
            variant="outlined"
            :model-value="message.activationCodeGuid"
            single-line
            hide-details
          >
            <template v-slot:append-inner>
              <v-btn color="primary" size="small" @click="copyContent(message.activationCodeGuid as string | null)">
                Copy
              </v-btn>
            </template>
          </v-text-field>
          <div v-if="message?.activationCodeExpirationTime" class="mt-2 text-caption text-center text-grey-darken-1">
            Expiration Time {{ Filter.formatShotDateTime(new Date(message?.activationCodeExpirationTime)) }}
          </div>
        </v-card-text>
      </v-card>
      <div class="text-right text-caption mt-4">Operations Team</div>
    </v-responsive>
  </v-container>
</template>

<script lang="ts" setup>
import FastDialog from '@/libs/FastDialog'
import Filter from '@/libs/Filter'
import WebApi from '@/libs/WebApi'
import { onBeforeMount, ref } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useClipboard } from '@vueuse/core'

const route = useRoute()
const router = useRouter()
const { copy } = useClipboard()

const messageId = Number(route.query?.id ?? -1)
const message = ref(null as any)

// 组件挂载前
onBeforeMount(async () => {
  await router.isReady()
  await getSystemMessageDetails(messageId)
})

// 查询数据
async function getSystemMessageDetails(messageId: number) {
  const result = await WebApi.getInstance().get('DappUser/GetSystemMessageDetails' as string, { messageId: messageId })
  if (!result.succeed) {
    FastDialog.errorSnackbar(result.errorMessage as string)
    router.push({ name: 'Error404' })
    return
  }
  message.value = result.data
}

// 复制内容
async function copyContent(content: string | null) {
  if (!content) {
    FastDialog.errorSnackbar('Copy failed, your browser does not support copying.')
    return
  }
  try {
    copy(content as string)
    FastDialog.successSnackbar('Copied successfully!')
  } catch (error) {
    FastDialog.errorSnackbar('Copy failed, your browser does not support copying.')
  }
}
</script>
