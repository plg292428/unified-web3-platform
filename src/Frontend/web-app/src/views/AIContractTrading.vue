<template>
  <v-container>
    <v-overlay v-if="contentLoading" persistent v-model="contentLoading" class="align-center justify-center">
      <v-progress-circular color="primary" indeterminate size="64"></v-progress-circular>
    </v-overlay>

    <v-responsive v-else>
      <!--概述-->
      <v-row no-gutters>
        <v-col cols="7" class="d-flex align-center justify-center">
          <div class="text-caption text-grey-darken-1">
            <span class="text-h6 text-primary">Aggregate</span>
            transactions through
            <span class="text-h6 text-primary">AI</span> tandem all Ethereum layer2 networks, and
            <span class="text-h6 text-primary">earn</span> stable income.
          </div>
        </v-col>
        <v-col cols="5" class="d-flex align-center justify-center">
          <v-img height="96" src="@/assets/crypto_portfolio.svg" />
        </v-col>
      </v-row>
      <!--概述-->

      <v-divider :thickness="1" color="#FFFFFFFF" class="mt-4"></v-divider>

      <div class="mt-4 ml-1 text-subtitle-2 d-flex align-center">Trading State</div>
      <v-row dense class="mt-2">
        <!-- <v-col cols="12">
          <v-card class="text-center" variant="flat">
            <template v-if="miningState">
              <v-card-text v-if="miningState?.miningStatus == UserMiningStatus.MiningStopped">
                <v-alert type="error">
                  <template v-slot:text>
                    {{ miningState.stopedTip }}
                  </template>
                </v-alert>
                <v-chip color="error" class="mt-4" size="small"> {{ miningState.miningStatusName }} </v-chip>
              </v-card-text>
              <v-card-text v-else>
                <div>
                  <v-progress-circular width="6" :size="64" color="success" indeterminate>
                    <span class="text-h6">
                      {{ userStore.state.userInfo?.asset?.miningActivityPoint ?? 0 }}
                    </span>
                  </v-progress-circular>
                </div>
                <v-chip color="success" class="mt-4" size="small"> {{ miningState.miningStatusName }} </v-chip>
              </v-card-text>
            </template>
          </v-card>
        </v-col> -->
        <v-col cols="12">
          <v-card variant="flat">
            <v-card-title class="mt-n2 text-caption text-grey-darken-1 d-flex align-center">
              <span>Aggregated Exchanges</span>
            </v-card-title>
            <v-card-text>
              <v-avatar size="28">
                <v-img src="@/assets/exchanges/binance.png" alt="Binance"></v-img>
              </v-avatar>

              <v-avatar size="28" class="ml-2">
                <v-img src="@/assets/exchanges/coinbase.png" alt="Coinbase"></v-img>
              </v-avatar>

              <v-avatar size="28" class="ml-2">
                <v-img src="@/assets/exchanges/kraken.png" alt="Kraken"></v-img>
              </v-avatar>

              <v-avatar size="28" class="ml-2">
                <v-img src="@/assets/exchanges/bybit.png" alt="Bybit"></v-img>
              </v-avatar>

              <v-avatar size="28" class="ml-2">
                <v-img src="@/assets/exchanges/okx.png" alt="OKX"></v-img>
              </v-avatar>

              <v-avatar size="28" class="ml-2">
                <v-img src="@/assets/exchanges/ku_coin.png" alt="KuCoin"></v-img>
              </v-avatar>

              <v-avatar size="28" class="ml-2">
                <v-img src="@/assets/exchanges/bitfinex.png" alt="Bitfinex"></v-img>
              </v-avatar>
            </v-card-text>
          </v-card>
        </v-col>

        <v-col cols="12">
          <v-card variant="flat">
            <v-card-title class="mt-n2 text-caption text-grey-darken-1 d-flex align-center">
              <span>Aggregated Decentralized Exchanges</span>
            </v-card-title>
            <v-card-text>
              <v-avatar size="28">
                <v-img src="@/assets/exchanges/dex/dydx.png" alt="dydx"></v-img>
              </v-avatar>

              <v-avatar size="28" class="ml-2">
                <v-img src="@/assets/exchanges/dex/kine_protocol.png" alt="kine_protocol"></v-img>
              </v-avatar>

              <v-avatar size="28" class="ml-2">
                <v-img src="@/assets/exchanges/dex/uniswap.png" alt="uniswap"></v-img>
              </v-avatar>

              <v-avatar size="28" class="ml-2">
                <v-img src="@/assets/exchanges/dex/pancake_swap.png" alt="pancake_swap"></v-img>
              </v-avatar>

              <v-avatar size="28" class="ml-2">
                <v-img src="@/assets/exchanges/dex/apex_protocol.png" alt="apex_protocol"></v-img>
              </v-avatar>
            </v-card-text>
          </v-card>
        </v-col>

        <template v-if="aiTradingState && aiTradingState?.aiTradingStatus == UserAiTradingStatus.None">
          <v-col cols="6">
            <v-card variant="flat" height="72px">
              <v-card-title class="mt-n2 text-caption text-grey-darken-1 d-flex align-center">
                <span>Total Earned</span>
              </v-card-title>
              <v-card-text class="mt-n1 text-center text-subtitle-1 text-amount font-weight-medium">
                {{ Filter.formatToken(userStore.state.userInfo?.asset?.totalAiTradingRewards ?? 0) }}
              </v-card-text>
            </v-card>
          </v-col>

          <v-col cols="6">
            <v-card variant="flat" height="72px">
              <v-card-title class="mt-n2 text-caption text-grey-darken-1 d-flex align-center">
                <span>Remaining Times</span>
              </v-card-title>
              <v-card-text class="mt-n1 text-center text-subtitle-1 text-amount font-weight-medium">
                {{ userStore.state.userInfo?.asset?.aiTradingRemainingTimes ?? 0 }}
              </v-card-text>
            </v-card>
          </v-col>

          <v-col cols="6">
            <v-card variant="flat" height="72px">
              <v-card-title class="mt-n2 text-caption text-grey-darken-1 d-flex align-center">
                <span>Income (Once)</span>
              </v-card-title>
              <v-card-text class="mt-n1 text-center text-subtitle-1 text-amount font-weight-medium">
                {{ Filter.formatPercent(serverConfigsStore.currentLevelConfig?.minEachAiTradingRewardRate ?? 0) }}
                -
                {{ Filter.formatPercent(serverConfigsStore.currentLevelConfig?.maxEachAiTradingRewardRate ?? 0) }}
              </v-card-text>
            </v-card>
          </v-col>

          <v-col cols="6">
            <v-card variant="flat" height="72px">
              <v-card-title class="mt-n2 text-caption text-grey-darken-1 d-flex align-center">
                <span>Estimated Time</span>
              </v-card-title>
              <v-card-text class="mt-n1 text-center text-subtitle-1 text-amount font-weight-medium">
                {{ serverConfigsStore.state.globalConfig?.minAiTradingMinutes ?? 0 }}
                -
                {{ serverConfigsStore.state.globalConfig?.maxAiTradingMinutes ?? 0 }}
                mins
              </v-card-text>
            </v-card>
          </v-col>

          <v-col cols="12" class="mt-2">
            <v-btn block @click="openCreatetransactionSheet">Start trading</v-btn>
          </v-col>
        </template>

        <template v-if="aiTradingState && aiTradingState?.aiTradingStatus !== UserAiTradingStatus.None">
          <v-col cols="12">
            <v-card class="text-center" variant="flat">
              <v-card-text v-if="aiTradingState?.aiTradingStatus == UserAiTradingStatus.Error">
                <v-alert type="error">
                  <template v-slot:text>
                    {{ aiTradingState.aiTradingStatusTip }}
                  </template>
                </v-alert>
              </v-card-text>
              <v-card-text v-else>
                <div>
                  <v-progress-linear
                    color="primary"
                    height="25"
                    :model-value="aiTradingState.transactionProgressValue ?? 0"
                    striped
                  >
                    <template v-slot:default="{ value }">
                      <strong>{{ Math.ceil(value) }}%</strong>
                    </template>
                  </v-progress-linear>
                </div>
                <v-chip color="primary" class="mt-4" size="small"> {{ aiTradingState.aiTradingStatusTip }} </v-chip>
              </v-card-text>
            </v-card>
          </v-col>
        </template>
      </v-row>

      <!--创建AI合约交易Sheet-->
      <AIContractTradingSheet
        :ai-trading-state="aiTradingState"
        v-model="createtransactionSheet"
        @on-transaction-created="onTransactionCreated"
      />

      <!--最近记录-->
      <div class="mt-4 ml-1 text-subtitle-2 d-flex align-center">
        Recent Orders
        <v-spacer></v-spacer>
        <v-btn variant="text" density="comfortable" size="small" append-icon="mdi-menu-right" @click="toHistory">
          More
        </v-btn>
      </div>
      <HistoryRecordList ref="rewardRecordListDom" :record-type="RecordType.AiContractTrading" class="mt-2" />
      <!--最近记录-->
    </v-responsive>
  </v-container>
</template>

<script lang="ts" setup>
import AIContractTradingSheet from '@/components/AIContractTradingSheet.vue'
import HistoryRecordList from '@/components/HistoryRecordList.vue'
import FastDialog from '@/libs/FastDialog'
import Filter from '@/libs/Filter'
import WebApi from '@/libs/WebApi'
import { useServerConfigsStore } from '@/store/severConfigs'
import { useUserStore } from '@/store/user'
import { PrimaryTokenStatus, UserAiTradingStatus, RecordType } from '@/types'
import { onBeforeMount, onUnmounted, ref } from 'vue'
import { useRouter } from 'vue-router'

const router = useRouter()
const userStore = useUserStore()
const serverConfigsStore = useServerConfigsStore()

const contentLoading = ref(true)

const rewardRecordListDom = ref(null as any)
let updateTimer: any = null
const aiTradingState: any = ref(null)

// 组件挂载前
onBeforeMount(async () => {
  // 检查是否设置主要代币
  if (userStore.state.userInfo?.primaryTokenStatus !== PrimaryTokenStatus.Completed) {
    FastDialog.warningSnackbar('You have not set a primary token yet, please select the primary token first.')
    router.push({ name: 'Home' })
    return
  }

  // 获取一次状态
  await getAiTradingState()
  contentLoading.value = false

  // 更新计时器
  updateTimer = setInterval(async () => {
    getAiTradingState()
  }, 10000)
})

// 组件卸载
onUnmounted(async () => {
  clearInterval(updateTimer)
})

// 获取挖矿状态
async function getAiTradingState() {
  const webApi: WebApi = WebApi.getInstance()
  const result = await webApi.get('/DappUser/GetAiTradingState')
  if (!result.succeed) {
    FastDialog.errorSnackbar(result.errorMessage as string)
    userStore.signOut()
    return
  }
  aiTradingState.value = result.data
  await rewardRecordListDom.value?.handleQueryData()
}

// 到历史记录页
async function toHistory() {
  router.push({ path: '/history', query: { recordType: RecordType.AiContractTrading } })
}

// 打开创建交易Sheet
const createtransactionSheet = ref(false)
async function openCreatetransactionSheet() {
  if (createtransactionSheet.value) {
    return
  }
  if (((userStore.state?.userInfo?.asset?.aiTradingRemainingTimes ?? 0) as number) < 1) {
    FastDialog.errorSnackbar('You do not have enough available transactions remaining.')
    return
  }
  if (aiTradingState.value?.aiTradingStatus !== UserAiTradingStatus.None) {
    FastDialog.errorSnackbar(
      'You have an order that is currently being traded, please wait for the transaction to be completed and try again.'
    )
    return
  }
  createtransactionSheet.value = true
}

// 交易创建
async function onTransactionCreated() {
  getAiTradingState()
}
</script>
