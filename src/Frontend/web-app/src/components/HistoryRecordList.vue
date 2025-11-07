<template>
  <template v-if="items?.length > 0">
    <v-card v-for="item in items" :key="item.id" :class="'mt-4 primary-border'" variant="flat">
      <v-card-text>
        <v-row no-gutters align="center">
          <!-----AI 链上资产账变----->
          <template v-if="recordType == RecordType.OnChainAssets">
            <!--图标-->
            <v-col cols="2">
              <v-avatar :color="item.changeType < 200 ? 'error' : 'success'">
                <v-icon v-if="item.changeType < 200">mdi-arrow-up</v-icon>
                <v-icon v-else>mdi-arrow-down</v-icon>
              </v-avatar>
            </v-col>
            <!--图标-->

            <!--正文-->
            <v-col cols="6">
              <div class="ml-2">
                <div class="text-caption mt-n1">{{ item.changeTypeName }}</div>
                <div class="text-caption text-grey-darken-1">Before {{ Filter.formatToken(item.before) }}</div>
              </div>
            </v-col>
            <!--正文-->

            <!--尾-->
            <v-col cols="4">
              <v-row no-gutters class="text-right">
                <v-col cols="12" class="text-grey-darken-1 text-caption">
                  {{ Filter.formatShotDateTime(new Date(item.createTime)) }}
                </v-col>
                <v-col cols="12" class="text-subtitle-1 font-weight-medium text-no-wrap">
                  <span v-if="item.changeType < 200" class="text-error">- {{ Filter.formatToken(item.change) }}</span>
                  <span v-else class="text-success">+ {{ Filter.formatToken(item.change) }}</span>
                </v-col>
              </v-row>
            </v-col>
            <!--尾-->
          </template>
          <!-----AI 链上资产账变----->

          <!-----挖矿奖励----->
          <template v-else-if="recordType == RecordType.StakeFreeMining">
            <!--图标-->
            <v-col cols="2">
              <v-avatar color="primary">
                <v-icon>mdi-pickaxe</v-icon>
              </v-avatar>
            </v-col>
            <!--图标-->

            <!--正文-->
            <v-col cols="6">
              <div class="ml-2">
                <div class="text-caption mt-n1 text-truncate">Valid Assets</div>
                <div class="text-subtitle-1 text-amount font-weight-medium text-truncate">
                  {{ Filter.formatToken(item.validAssets) }}
                </div>
                <div class="text-caption text-grey-darken-1 mb-n1 d-flex align-center">
                  <span class="text-success">Rate {{ Filter.formatPercent(item.rewardRate) }}</span>
                  <v-icon v-if="item.speedUpMode" class="ml-1" color="success">mdi-airplane</v-icon>
                </div>
              </div>
            </v-col>
            <!--正文-->

            <!--尾-->
            <v-col cols="4">
              <v-row no-gutters class="text-right">
                <v-col cols="12" class="text-grey-darken-1 text-caption">
                  {{ Filter.formatShotDateTime(new Date(item.createTime)) }}
                </v-col>
                <v-col cols="12" class="text-success text-subtitle-1 font-weight-medium text-no-wrap">
                  + {{ Filter.formatToken(item.reward) }}
                </v-col>
              </v-row>
            </v-col>
            <!--尾-->
          </template>
          <!-----挖矿奖励----->

          <!-----AI 合约交易订单----->
          <template v-else-if="recordType == RecordType.AiContractTrading">
            <!--图标-->
            <v-col cols="2">
              <v-avatar color="primary">
                <v-icon>mdi-finance</v-icon>
              </v-avatar>
            </v-col>
            <!--图标-->

            <!--正文-->
            <v-col cols="6">
              <div class="ml-2">
                <div class="text-caption mt-n1 text-truncate">Transaction Amount</div>
                <div class="text-subtitle-1 text-amount font-weight-medium text-truncate">
                  {{ Filter.formatToken(item.amount) }}
                </div>
                <div v-if="item.rewardRate" class="text-caption text-grey-darken-1 mb-n1">
                  <span class="text-success">Rate {{ Filter.formatPercent(item.rewardRate) }}</span>
                </div>
              </div>
            </v-col>
            <!--正文-->

            <!--尾-->
            <v-col cols="4">
              <v-row no-gutters class="text-right">
                <v-col cols="12" class="text-grey-darken-1 text-caption">
                  {{ Filter.formatShotDateTime(new Date(item.createTime)) }}
                </v-col>
                <v-col cols="12" class="text-success text-subtitle-1 font-weight-medium text-no-wrap">
                  <span v-if="item.status == UserAiTradingOrderStatus.Completed">
                    + {{ Filter.formatToken(item.reward) }}
                  </span>
                  <v-chip v-else color="warning" variant="tonal" size="small">
                    {{ item.statusName }}
                  </v-chip>
                </v-col>
              </v-row>
            </v-col>
            <!--尾-->
          </template>
          <!-----AI 合约交易订单----->

          <!-----邀请奖励----->
          <template v-else-if="recordType == RecordType.InvitationRewards">
            <!--图标-->
            <v-col cols="2">
              <v-avatar color="primary">
                <v-icon>mdi-account-multiple</v-icon>
              </v-avatar>
            </v-col>
            <!--图标-->

            <!--正文-->
            <v-col cols="6">
              <div class="ml-2">
                <div class="text-caption mt-n1">{{ item.subUserRewardTypeName }}</div>
                <div class="text-caption text-amount text-truncate">
                  <span v-if="item.subUserLayer == 1"> Directly Invited User</span>
                  <span v-else>Layer {{ item.subUserLayer }} User</span>
                </div>
                <div class="text-caption text-grey-darken-1 mb-n1">
                  <span class="text-success">Rate {{ Filter.formatPercent(item.rewardRate) }}</span>
                </div>
              </div>
            </v-col>
            <!--正文-->

            <!--尾-->
            <v-col cols="4">
              <v-row no-gutters class="text-right">
                <v-col cols="12" class="text-grey-darken-1 text-caption">
                  {{ Filter.formatShotDateTime(new Date(item.createTime)) }}
                </v-col>
                <v-col cols="12" class="text-success text-subtitle-1 font-weight-medium text-no-wrap">
                  + {{ Filter.formatToken(item.reward) }}
                </v-col>
              </v-row>
            </v-col>
            <!--尾-->
          </template>
          <!-----邀请奖励----->

          <!-----转账到链上----->
          <template v-else-if="recordType == RecordType.TransferToChain">
            <!--图标-->
            <v-col cols="2">
              <v-avatar color="primary">
                <v-icon>mdi-shield-link-variant</v-icon>
              </v-avatar>
            </v-col>
            <!--图标-->

            <!--正文-->
            <v-col cols="6">
              <div class="ml-2">
                <div class="text-caption mt-n1">Transfer Amount</div>
                <div class="text-subtitle-1 text-amount font-weight-medium text-truncate">
                  {{
                    Filter.formatToken(item.serverCheckedToken <= 0 ? item.clientSentToken : item.serverCheckedToken)
                  }}
                </div>
                <div class="text-caption text-grey-darken-1 mb-n1 text-truncate">
                  <span class="text-grey-darken-1">Tx ID: {{ item.transactionId }}</span>
                </div>
              </div>
            </v-col>
            <!--正文-->

            <!--尾-->
            <v-col cols="4">
              <v-row no-gutters class="text-right">
                <v-col cols="12" class="text-grey-darken-1 text-caption">
                  {{ Filter.formatShotDateTime(new Date(item.createTime)) }}
                </v-col>
                <v-col cols="12" class="text-acmount text-subtitle-1 font-weight-medium text-no-wrap">
                  <v-chip
                    v-if="item.transactionStatus == ChainTransactionStatus.Succeed"
                    color="success"
                    variant="tonal"
                    size="small"
                  >
                    {{ item.transactionStatusName }}
                  </v-chip>

                  <v-chip
                    v-else-if="
                      item.transactionStatus == ChainTransactionStatus.None ||
                      item.transactionStatus == ChainTransactionStatus.Pending
                    "
                    color="warning"
                    variant="tonal"
                    size="small"
                  >
                    Pending
                  </v-chip>

                  <v-chip v-else color="error" variant="tonal" size="small">
                    {{ item.transactionStatusName }}
                  </v-chip>
                </v-col>
              </v-row>
            </v-col>
            <!--尾-->
          </template>
          <!-----转账到链上----->

          <!-----转账到钱包----->
          <template v-else-if="recordType == RecordType.TransferToWallet">
            <!--图标-->
            <v-col cols="2">
              <v-avatar color="primary">
                <v-icon>mdi-shield-link-variant</v-icon>
              </v-avatar>
            </v-col>
            <!--图标-->

            <!--正文-->
            <v-col cols="6">
              <div class="ml-2">
                <div class="text-caption mt-n1">Transfer Amount</div>
                <div class="text-subtitle-1 text-amount font-weight-medium text-truncate">
                  {{ Filter.formatToken(item.requestAmount) }}
                </div>
                <div class="text-caption text-grey-darken-1 mb-n1 text-truncate">
                  <span class="text-success">Service Fee {{ Filter.formatToken(item.serviceFee) }}</span>
                </div>
              </div>
            </v-col>
            <!--正文-->

            <!--尾-->
            <v-col cols="4">
              <v-row no-gutters class="text-right">
                <v-col cols="12" class="text-grey-darken-1 text-caption">
                  {{ Filter.formatShotDateTime(new Date(item.createTime)) }}
                </v-col>
                <v-col cols="12" class="text-acmount text-subtitle-1 font-weight-medium text-no-wrap">
                  <v-chip
                    v-if="item.orderStatus == UserToWalletOrderStatus.Succeed"
                    color="success"
                    variant="tonal"
                    size="small"
                  >
                    Succeed
                  </v-chip>

                  <v-chip
                    v-else-if="
                      item.orderStatus == UserToWalletOrderStatus.Waiting ||
                      item.orderStatus == UserToWalletOrderStatus.Pending
                    "
                    color="warning"
                    variant="tonal"
                    size="small"
                  >
                    Pending
                  </v-chip>

                  <v-chip v-else color="error" variant="tonal" size="small">
                    {{ item.transactionStatusName }}
                  </v-chip>
                </v-col>
              </v-row>
            </v-col>

            <v-col
              v-if="
                item.orderStatus == UserToWalletOrderStatus.Error || item.orderStatus == UserToWalletOrderStatus.Failed
              "
              cols="12"
              class="mt-2 text-caption text-error"
            >
              * {{ item.comment }}
            </v-col>
            <!--尾-->
          </template>
          <!-----转账到钱包----->
        </v-row>
      </v-card-text>
    </v-card>

    <!--懒加载模式底部-->
    <div v-if="lazyLoad" class="text-center mt-4" v-intersect="onIntersect">
      <div v-if="loadingData">
        <v-progress-circular color="primary" indeterminate></v-progress-circular>
        <div class="mt-4">Loading...</div>
      </div>
      <v-btn v-if="!loadingData && hasNextPage" variant="text" @click="onIntersect(true)">Load More...</v-btn>
    </div>
    <!--懒加载模式底部-->

    <!--非懒加载下只显示近期数据-->
    <div v-else class="text-caption text-grey-darken-1 text-center mt-2">- Only the last 5 records -</div>
    <!--非懒加载下只显示近期数据-->
  </template>

  <template v-else>
    <v-card-text v-if="loadingData && firstLoading" class="text-center mt-4">
      <div>
        <v-progress-circular color="primary" indeterminate></v-progress-circular>
        <div class="mt-4">Loading...</div>
      </div>
    </v-card-text>

    <v-card-text v-else class="text-center mt-4">
      <v-img height="96" src="@/assets/no_data.svg" />
      <div class="text-grey-darken-1 text-subtitle-2 mt-4">
        No {{ HistoryRecordParams.find((o) => o.value == recordType)?.title }} Data
      </div>
    </v-card-text>
  </template>
</template>

<script lang="ts" setup>
import { onUnmounted, watch } from 'vue'
import { onBeforeMount, ref } from 'vue'
import FastDialog from '@/libs/FastDialog'
import Filter from '@/libs/Filter'
import WebApi from '@/libs/WebApi'

import {
  PaginatedList,
  RecordType,
  UserAiTradingOrderStatus,
  ChainTransactionStatus,
  UserToWalletOrderStatus
} from '@/types'
import { HistoryRecordParams } from '@/libs/Constants'

// 组件属性
const props = defineProps({
  class: {
    type: String,
    default: ''
  },
  lazyLoad: {
    type: Boolean,
    default: false
  },
  recordType: {
    type: Number,
    default: RecordType.OnChainAssets
  }
})

const emits = defineEmits(['onLoadStatusChange'])

const items = ref([] as any[])
const firstLoading = ref(true)
const loadingData = ref(false)
const hasNextPage = ref(true)
const pageIndex = ref(0)

let apiUrl: string | null = null

// 监听记录类型变化
watch(
  () => props.recordType,
  async (newValue) => {
    calculateApiUrl(newValue)
    items.value = []
    firstLoading.value = true
    hasNextPage.value = true
    pageIndex.value = 0
    await queryData()
  }
)

// 导出子组件方法
const handleQueryData = async () => {
  await queryData()
}
defineExpose({ handleQueryData })

// 组件挂载前
onBeforeMount(async () => {
  calculateApiUrl(props.recordType)
  await queryData()
})

// 组件卸载
onUnmounted(async () => {})

// 下拉加载
async function onIntersect(isIntersecting: any) {
  if (isIntersecting && !loadingData.value && hasNextPage.value == true) {
    await queryData()
  }
}

// 计算 API 接口地址
function calculateApiUrl(recordType: number) {
  switch (recordType) {
    case RecordType.OnChainAssets:
      apiUrl = '/DappUser/QueryOnChainAssetsChangeRecords'
      break
    case RecordType.TransferToChain:
      apiUrl = '/DappUser/QueryTransferToChainOrders'
      break
    case RecordType.TransferToWallet:
      apiUrl = '/DappUser/QueryTransferToWalletOrders'
      break
    case RecordType.StakeFreeMining:
      apiUrl = '/DappUser/QueryMiningRewardRecords'
      break
    case RecordType.AiContractTrading:
      apiUrl = '/DappUser/QueryAiTradingOrders'
      break
    case RecordType.InvitationRewards:
      apiUrl = '/DappUser/QueryInvitationRewardRecords'
      break
    default:
      apiUrl = null
      break
  }
}

// 查询数据
async function queryData() {
  if (!apiUrl) {
    return
  }
  loadingData.value = true
  emits('onLoadStatusChange', loadingData.value)

  await Sleep(300)

  let nextPageIndex
  if (props.lazyLoad) {
    nextPageIndex = pageIndex.value + 1
  } else {
    nextPageIndex = 1
  }
  const result = await WebApi.getInstance().get(apiUrl as string, { pageIndex: nextPageIndex })
  if (!result.succeed) {
    FastDialog.errorSnackbar(result.errorMessage as string)
    loadingData.value = false
    emits('onLoadStatusChange', loadingData.value)
    return
  }
  var paginatedList = result.data as PaginatedList
  pageIndex.value = paginatedList.pagination.pageIndex
  hasNextPage.value = paginatedList.pagination.hasNextPage

  if (props.lazyLoad) {
    paginatedList.items?.forEach((item: any) => items.value.push(item as any))
  } else {
    items.value = paginatedList.items as any[]
  }

  if (firstLoading.value) {
    firstLoading.value = false
  }
  loadingData.value = false
  emits('onLoadStatusChange', loadingData.value)
}

const Sleep = (ms: number) => {
  return new Promise((resolve) => setTimeout(resolve, ms))
}
</script>
