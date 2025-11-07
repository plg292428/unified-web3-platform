<template>
  <v-bottom-sheet persistent v-model="localModel">
    <v-card class="text-center" color="grey-darken-4" :disabled="controlsLoading">
      <v-card-title class="text-subtitle-2 mb-n2"> Create Transaction</v-card-title>
      <v-card-text>
        <v-img height="96" src="@/assets/online_transactions.svg" />
        <div class="mt-6 text-h5 text-amount">
          {{ Filter.formatToken(aiTradingState.requiresOnChainAssets) }}
        </div>
        <div class="text-caption text-grey-darken-1">
          Available On-Chain Assets {{ Filter.formatToken(userStore?.state?.userInfo?.asset.onChainAssets ?? 0) }}
        </div>

        <v-select
          class="mt-4"
          v-model="selectedObj"
          color="primary"
          density="compact"
          variant="filled"
          label="Please select aggregation mode"
          :items="aggregationModes"
          item-title="name"
          single-line
          center-affix
          hide-details
          return-object
          :loading="controlsLoading"
          :disabled="controlsLoading"
        >
          <template v-slot:item="{ props, item }">
            <v-list-item v-bind="props">
              <template v-slot:prepend>
                <v-icon>{{ item.raw.icon }}</v-icon>
              </template>
            </v-list-item>
          </template>

          <template v-slot:selection="{ item }">
            <div class="d-flex align-center">
              <v-icon>{{ item.raw.icon }}</v-icon>
              <span class="ml-2">{{ item.raw.name }}</span>
            </div>
          </template>
        </v-select>

        <div class="text-caption mt-2 text-success">
          Estimated Income
          {{ Filter.formatToken(aiTradingState.minEstimatedIncome) }} -
          {{ Filter.formatToken(aiTradingState.maxEstimatedIncome) }}
        </div>

        <v-alert v-model="alert" variant="flat" closable type="error" class="mt-4 text-body-2">
          {{ alertContent }}
          <template v-slot:close>
            <v-btn icon="$close" color="white" @click="alertClose"></v-btn>
          </template>
        </v-alert>

        <div class="my-4">
          <v-btn variant="tonal" @click="closeSheet"> Cancel </v-btn>
          <v-btn class="ml-12" variant="flat" @click="create" :loading="controlsLoading"> Create </v-btn>
        </div>
      </v-card-text>
    </v-card>
  </v-bottom-sheet>
</template>

<script lang="ts" setup>
import FastDialog from '@/libs/FastDialog'
import Filter from '@/libs/Filter'
import WebApi from '@/libs/WebApi'
import { useUserStore } from '@/store/user'
import { computed } from 'vue'
import { ref } from 'vue'

const props = defineProps({
  modelValue: {
    type: Boolean,
    default: true
  },
  aiTradingState: {
    type: Object,
    default: null
  }
})

const emits = defineEmits(['update:modelValue', 'onTransactionCreated'])
const localModel = computed({
  get: () => props.modelValue,
  set: (val) => {
    emits('update:modelValue', val)
  }
})

const aggregationModes = [
  { name: 'Centralized Exchanges', icon: 'mdi-alpha-c-box', key: 1 },
  { name: 'Decentralized Exchanges', icon: 'mdi-alpha-d-box', key: 2 }
]
const userStore = useUserStore()

const alert = ref(false)
const alertContent = ref('')
const controlsLoading = ref(false)
const selectedObj = ref(null)

async function alertShow(content: string) {
  alertContent.value = content
  alert.value = true
}

async function alertClose() {
  alertContent.value = ''
  alert.value = false
}

// 关闭 Sheet
async function closeSheet() {
  if (!localModel.value) {
    return
  }
  alertClose()
  controlsLoading.value = false
  selectedObj.value = null
  localModel.value = false
}

// 创建交易
async function create() {
  alertClose()
  if (!selectedObj.value) {
    alertShow('Please select aggregation mode.')
    return
  }

  controlsLoading.value = true

  const webApi: WebApi = WebApi.getInstance()
  const result = await webApi.post('/DappUser/CreateAiTradingOrder')
  if (!result.succeed) {
    FastDialog.errorSnackbar(result.errorMessage as string)
    controlsLoading.value = false
    return
  }
  emits('onTransactionCreated')
  FastDialog.successSnackbar('Transaction created successfully!')
  closeSheet()
}
</script>
