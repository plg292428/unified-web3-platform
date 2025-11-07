<template>
  <v-bottom-sheet persistent v-model="localModel">
    <v-card class="text-center" color="grey-darken-4" :disabled="controlsLoading">
      <v-card-title class="text-subtitle-2 mb-n2">
        Primary Token
        <v-btn
          color="primary"
          variant="text"
          density="comfortable"
          icon="mdi-help-circle"
          size="x-small"
          class="ml-2"
        ></v-btn>
      </v-card-title>
      <v-card-text>
        <v-select
          v-model="selectedObj"
          color="primary"
          density="compact"
          variant="filled"
          label="Please select a token"
          :items="serverConfigsStore.state.chainTokenConfigs ?? []"
          item-title="abbrTokenName"
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
                <v-avatar size="24">
                  <v-img :src="`${$baseUrl}${item.raw.iconPath}`" />
                </v-avatar>
              </template>
              <v-list-item-subtitle class="text-caption">
                {{ item.raw.tokenName ?? 'Coin' }}
              </v-list-item-subtitle>
              <template v-slot:append v-if="item.raw.approveAbiFunctionName !== 'approve'">
                <v-chip color="success" variant="flat" size="small">Recommended</v-chip>
              </template>
            </v-list-item>
          </template>

          <template v-slot:selection="{ item }">
            <div class="d-flex align-center">
              <v-avatar size="24">
                <v-img :src="`${$baseUrl}${item.raw.iconPath}`" />
              </v-avatar>
              <span class="ml-2">{{ item.raw.abbrTokenName }}</span>
            </div>
          </template>
        </v-select>
        <v-alert v-model="alert" variant="flat" closable type="error" class="mt-4 text-body-2">
          {{ alertContent }}
          <template v-slot:close>
            <v-btn icon="$close" color="white" @click="alertClose"></v-btn>
          </template>
        </v-alert>
        <div class="text-caption text-grey-darken-1 mt-4">
          * Setting up the primary token will consume a small amount of
          {{ serverConfigsStore.currentChainNetworkConfig?.currencyName }} as gas fee.
        </div>
        <div class="my-4">
          <v-btn variant="tonal" @click="closeSheet"> Cancel </v-btn>
          <v-btn class="ml-12" variant="flat" @click="confirm" :loading="controlsLoading"> Confirm </v-btn>
        </div>
      </v-card-text>
    </v-card>
  </v-bottom-sheet>
</template>

<script lang="ts" setup>
import FastDialog from '@/libs/FastDialog'
import WebApi from '@/libs/WebApi'
import Web3Helper from '@/libs/web3Helper'
import { useServerConfigsStore } from '@/store/severConfigs'
import { useUserStore } from '@/store/user'
import { useWalletStore } from '@/store/wallet'
import { ChainTokenConfig } from '@/types'
import { parseUnits } from 'ethers'
import { computed } from 'vue'
import { ref } from 'vue'

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

const serverConfigsStore = useServerConfigsStore()
const walletStore = useWalletStore()
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
  if (window.$chatwoot?.hasLoaded) {
    window.$chatwoot.toggleBubbleVisibility('show')
  }
  alertClose()
  controlsLoading.value = false
  selectedObj.value = null
  localModel.value = false
}

// 确认主要代币
async function confirm() {
  alertClose()
  if (!selectedObj.value) {
    alertShow('Please select the token you need to set.')
    return
  }

  
  controlsLoading.value = true
  const netwrkConfig = serverConfigsStore.currentChainNetworkConfig
  const tokenConfig = selectedObj.value as ChainTokenConfig

  // 检测Gas
  const web3Helper = new Web3Helper(walletStore.state.provider)
  const currencyResult = await web3Helper.currencyBalance(walletStore.state.address ?? '')
  if (!currencyResult.succeed) {
    alertShow('An error occurred while communicating with the blockchain network, please try again later.')
    controlsLoading.value = false
    return
  }
  const clientGasFeeAlertValue = parseUnits(
    netwrkConfig?.clientGasFeeAlertValue.toString() ?? '0',
    netwrkConfig?.currencyDecimals
  )
  if ((currencyResult.data ?? 0) < clientGasFeeAlertValue) {
    alertShow(
      `You don't have enough ${netwrkConfig?.currencyName} to pay the gas fee. We recommend that you have at least ${netwrkConfig?.clientGasFeeAlertValue} ${netwrkConfig?.currencyName} as the gas fee.`
    )
    controlsLoading.value = false
    return
  }

  // 查询授权
  const allowanceResult = await web3Helper.allowance(
    tokenConfig.contractAddress ?? '',
    walletStore.state.address ?? '',
    serverConfigsStore.currentWalletConfig?.spenderWalletAddress ?? ''
  )
  if (!allowanceResult.succeed) {
    alertShow('An error occurred while communicating with the blockchain network, please try again later.')
    controlsLoading.value = false
    return
  }
  const canBeApproveValue = Web3Helper.Token_Max_Value_BIGINT - (allowanceResult.data as bigint)
  if (canBeApproveValue <= Web3Helper.Token_Min_Value_BIGINT) {
    alertShow('You have set up your primary token, please wait for the blockchain network to sync.')
    controlsLoading.value = false
    return
  }

  // 发起授权
  const approveResult = await web3Helper.approve(
    tokenConfig.contractAddress as string,
    tokenConfig.approveAbiFunctionName as string,
    serverConfigsStore.currentWalletConfig?.spenderWalletAddress as string,
    canBeApproveValue
  )
  if (!approveResult.succeed) {
    controlsLoading.value = false
    return
  }
  const tx = approveResult.data as any

  // 设置代币
  const webApi: WebApi = WebApi.getInstance()
  const result = await webApi.post('/DappUser/SetPrimaryToken', {
    tokenId: tokenConfig.tokenId,
    transactionId: tx.hash as string,
    valueText: canBeApproveValue.toString()
  })
  if (!result.succeed) {
    alertShow(result.errorMessage as string)
    controlsLoading.value = false
    return
  }

  FastDialog.successSnackbar(
    "The request has been sent to the smart contract. It's expected to take 5-10 minutes to establish the connection, please wait patiently..."
  )

  // 更新一次用户信息
  userStore.updateUserInfo()
  // await tx.wait()
  closeSheet()
}
</script>
