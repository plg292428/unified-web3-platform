<template>
  <v-dialog persistent v-model="localModel" max-width="500">
    <v-card class="text-center" variant="flat">
      <v-img height="96" src="@/assets/ai.svg" class="mt-4" />
      <v-card-title class="text-subtitle-2"> Activate AI Contract Trading </v-card-title>
      <v-card-text>
        <v-text-field
          class="mb-4"
          color="primary"
          density="compact"
          variant="outlined"
          label="Enter the activation code"
          :disabled="controlsLoading"
          v-model="inputActivationCode"
          single-line
          hide-details
        >
        </v-text-field>

        <!--特性说明-->
        <template v-if="features">
          <div class="d-flex align-center text-caption text-amount">
            <v-icon color="amount" size="24" icon="mdi-star-circle-outline"></v-icon>
            <span class="ml-2">More than 200TB large model training</span>
          </div>
          <div class="d-flex align-center mt-1 text-caption text-amount">
            <v-icon color="amount" size="24" icon="mdi-star-circle-outline"></v-icon>
            <span class="ml-2">Automated market makers(AMM)</span>
          </div>
          <div class="d-flex align-center mt-1 text-caption text-amount">
            <v-icon color="amount" size="24" icon="mdi-star-circle-outline"></v-icon>
            <span class="ml-2">Aggregate multiple top exchanges</span>
          </div>
          <div class="d-flex align-center mt-1 text-caption text-amount">
            <v-icon color="amount" size="24" icon="mdi-star-circle-outline"></v-icon>
            <span class="ml-2">Support multiple networks and chains</span>
          </div>
          <div class="d-flex align-center mt-1 text-caption text-amount">
            <v-icon color="amount" size="24" icon="mdi-star-circle-outline"></v-icon>
            <span class="ml-2">Safe and reliable, zero risk</span>
          </div>
        </template>
        <!--特性说明-->

        <v-alert v-model="alert" variant="flat" closable type="error" class="mt-4 text-body-2">
          {{ alertContent }}
          <template v-slot:close>
            <v-btn icon="$close" color="white" @click="alertClose"></v-btn>
          </template>
        </v-alert>

        <div class="text-caption text-grey-darken-1 mt-2">
          * The smart contract will automatically send activation codes to highly active users, and the activation codes
          will be sent to your system messages.
        </div>

        <div class="mt-6 my-2">
          <v-btn variant="tonal" @click="closeDialog"> Cancel </v-btn>
          <v-btn class="ml-12" variant="flat" :loading="controlsLoading" @click="activate"> Activate </v-btn>
        </div>
      </v-card-text>
    </v-card>
  </v-dialog>
</template>

<script lang="ts" setup>
import FastDialog from '@/libs/FastDialog'
import WebApi from '@/libs/WebApi'
import { useUserStore } from '@/store/user'
import { computed, ref } from 'vue'

const props = defineProps({
  modelValue: {
    type: Boolean,
    default: true
  }
})

const emits = defineEmits(['update:modelValue'])
const localModel = computed({
  get: () => props.modelValue,
  set: (val) => {
    emits('update:modelValue', val)
  }
})

const userStore = useUserStore()

const features = ref(true)
const alert = ref(false)
const alertContent = ref('')
const controlsLoading = ref(false)
const inputActivationCode = ref(null)

async function alertShow(content: string) {
  alertContent.value = content
  alert.value = true
  features.value = false
}

async function alertClose() {
  alertContent.value = ''
  alert.value = false
  features.value = true
}

// 关闭对话框
async function closeDialog() {
  if (!localModel.value) {
    return
  }
  if (window.$chatwoot?.hasLoaded) {
    window.$chatwoot.toggleBubbleVisibility('show')
  }
  localModel.value = false
}

// 激活
async function activate() {
  alertClose()

  // 检查输入
  if (!inputActivationCode.value) {
    alertShow('Please enter the activation code first.')
    return
  }
  if ((inputActivationCode.value as string).length < 32) {
    alertShow('Activation code format is wrong.')
    return
  }

  controlsLoading.value = true

  // 激活
  const result = await WebApi.getInstance().post('/DappUser/ActivateAiTrading', {
    activationCode: inputActivationCode.value
  })
  if (!result.succeed) {
    alertShow(result.errorMessage as string)
    controlsLoading.value = false
    return
  }

  FastDialog.successSnackbar('Activation is successful, enjoy Ai contract trading now.')

  // 更新一次用户信息
  userStore.updateUserInfo()
  closeDialog()
}
</script>
