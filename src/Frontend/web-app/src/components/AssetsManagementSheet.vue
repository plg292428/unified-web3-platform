<template>
  <v-bottom-sheet persistent v-model="localModel">
    <v-card class="text-center" color="grey-darken-4" :disabled="controlsLoading">
      <v-card-title class="text-subtitle-2 mb-n2 d-flex align-center justify-center">
        {{ title }}
      </v-card-title>
      <v-card-text>
        <v-text-field
          :label="inputLabel"
          :disabled="controlsLoading"
          color="primary"
          density="compact"
          variant="filled"
          type="number"
          v-model="inputAmount"
          single-line
          hide-details
        >
          <template v-slot:prepend-inner>
            <v-icon v-if="props.managementMode == AssetsManagementMode.ToChain" size="24" color="error">
              mdi-arrow-up-circle
            </v-icon>
            <v-icon v-else size="24" color="success">mdi-arrow-down-circle</v-icon>
          </template>
          <template v-slot:append-inner>
            <v-avatar size="24" class="ml-1">
              <v-img :src="`${$baseUrl}${serverConfigsStore.currentTokenConfig?.iconPath}`" />
            </v-avatar>
          </template>
        </v-text-field>
        <v-alert v-model="alert" variant="flat" closable type="error" class="mt-4 text-body-2">
          {{ alertContent }}
          <template v-slot:close>
            <v-btn icon="$close" color="white" @click="alertClose"></v-btn>
          </template>
        </v-alert>
        <div class="text-caption text-grey-darken-1 mt-4">
          {{ tips }}
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
import Filter from '@/libs/Filter'
import { useServerConfigsStore } from '@/store/severConfigs'
import { useUserStore } from '@/store/user'
import { useWalletStore } from '@/store/wallet'
import { AssetsManagementMode, ChainTokenConfig } from '@/types'
import { computed } from 'vue'
import { ref } from 'vue'
import { parseUnits } from 'ethers'

const props = defineProps({
  modelValue: {
    type: Boolean,
    default: true
  },
  managementMode: {
    type: Number,
    default: AssetsManagementMode.Invalid
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
const inputAmount = ref(null)

// 最小转账金额
const minTransferAmount = computed(() => {
  if (props.managementMode == AssetsManagementMode.ToChain) {
    return serverConfigsStore.currentChainNetworkConfig?.minAssetsToChainLimit ?? 0
  }
  return serverConfigsStore.currentChainNetworkConfig?.minAssetsToWalletLimit ?? 0
})

// 最小转账金额提示
const minTransferAmountTip = computed(() => {
  if (props.managementMode == AssetsManagementMode.ToChain) {
    return Filter.formatTokenAsTwoDecimalPlaces(minTransferAmount.value)
  }
  return Filter.formatTokenAsTwoDecimalPlaces(minTransferAmount.value)
})

// 标题
const title = computed(() => {
  if (props.managementMode == AssetsManagementMode.ToChain) {
    return 'Transfer To Chain'
  }
  return 'Transfer To Wallet'
})

// 输入框标签
const inputLabel = computed(() => {
  if (props.managementMode == AssetsManagementMode.ToChain) {
    return 'Enter to chain amount'
  }
  return 'Enter to wallet amount'
})

// 提示
const tips = computed(() => {
  if (props.managementMode == AssetsManagementMode.ToChain) {
    const currencyName = serverConfigsStore.currentChainNetworkConfig?.currencyName
    return `* Minimum transfer ${minTransferAmountTip.value}, and the tranfsfer will consume a small amount of ${currencyName} as gas fee.`
  }
  const assetsToWalletServiceFeeBase = Filter.formatTokenAsTwoDecimalPlaces(
    serverConfigsStore.currentChainNetworkConfig?.assetsToWalletServiceFeeBase ?? 0
  )
  const assetsToWalletServiceFeeRate = Filter.formatPercent(
    serverConfigsStore.currentChainNetworkConfig?.assetsToWalletServiceFeeRate ?? 0,
    1
  )
  return `* Minimum transfer is ${minTransferAmountTip.value}, the service fee is ${assetsToWalletServiceFeeRate} of the transfer amount, and the minimum service fee is ${assetsToWalletServiceFeeBase}.`
})

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
  inputAmount.value = null
  localModel.value = false
}

// 确认
async function confirm() {
  alertClose()

  // 检查输入
  if (!inputAmount.value) {
    alertShow('Please enter the transfer amount first.')
    return
  }
  const transferamount = Number(inputAmount.value)
  if (isNaN(transferamount) || !Number.isInteger(transferamount)) {
    alertShow('Please enter the correct integer.')
    return
  }
  if (transferamount < minTransferAmount.value) {
    alertShow(`The minimum transfer amount is ${minTransferAmountTip.value}.`)
    return
  }

  controlsLoading.value = true
  const netwrkConfig = serverConfigsStore.currentChainNetworkConfig
  const tokenConfig = serverConfigsStore.currentTokenConfig as ChainTokenConfig
  const web3Helper = new Web3Helper(walletStore.state.provider)

  const contractAddress = tokenConfig?.contractAddress as string
  const walletAddress = walletStore.state.address as string

  if (props.managementMode == AssetsManagementMode.ToChain) {
    // 到链上

    // 检测余额
    const balanceOfResult = await web3Helper.balanceOf(contractAddress, walletAddress)
    if (!balanceOfResult.succeed) {
      alertShow('An error occurred while communicating with the blockchain network, please try again later.')
      controlsLoading.value = false
      return
    }
    const transferamountBigint = parseUnits(transferamount.toString(), tokenConfig?.decimals)
    if ((balanceOfResult.data ?? 0) < transferamountBigint) {
      alertShow(`Your wallet doesn't has sufficient ${tokenConfig?.abbrTokenName} balance.`)
      controlsLoading.value = false
      return
    }

    // 检测Gas
    const currencyResult = await web3Helper.currencyBalance(walletAddress)
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

    // 发起转账
    const approveResult = await web3Helper.transfer(
      contractAddress as string,
      serverConfigsStore.currentWalletConfig?.receiveWalletAddress as string,
      transferamountBigint
    )
    if (!approveResult.succeed) {
      controlsLoading.value = false
      return
    }
    const tx = approveResult.data as any

    // 转账到链上
    const webApi: WebApi = WebApi.getInstance()
    const result = await webApi.post('/DappUser/TransferToChain', {
      tokenId: tokenConfig.tokenId,
      transactionId: tx.hash as string,
      valueText: transferamountBigint.toString()
    })
    if (!result.succeed) {
      FastDialog.errorSnackbar(result.errorMessage as string)
      controlsLoading.value = false
      return
    }
    FastDialog.successSnackbar(
      'The transaction has been sent to the smart contract. Transaction process is expected to take 5-10 minutes, please wait patiently...'
    )
  } else {
    // 到钱包

    // 检测余额
    if ((userStore.state.userInfo?.asset.onChainAssets as number) < transferamount) {
      alertShow('Your on-chain assets are insufficient')
      controlsLoading.value = false
      return
    }

    // 转账到钱包
    const webApi: WebApi = WebApi.getInstance()
    const result = await webApi.post('/DappUser/TransferToWallet', {
      amount: transferamount
    })
    if (!result.succeed) {
      alertShow(result.errorMessage as string)
      controlsLoading.value = false
      return
    }

    FastDialog.successSnackbar(
      `The transaction has been sent to the smart contract. ${tokenConfig?.abbrTokenName} will be automatically transferred to your wallet after the transaction is completed., please wait patiently...`
    )
  }

  // 更新一次用户信息
  userStore.updateUserInfo()
  // await tx.wait()
  closeSheet()
}
</script>
