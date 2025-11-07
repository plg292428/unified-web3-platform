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
            <span class="text-h6 text-primary">Mining</span>
            with <span class="text-h6 text-primary">zero risk</span> and
            <span class="text-h6 text-primary">earning</span> rewards easily.
          </div>
        </v-col>
        <v-col cols="5" class="d-flex align-center justify-center">
          <v-img height="96" src="@/assets/bitcoin_p2p.svg" />
        </v-col>
      </v-row>
      <!--概述-->

      <v-divider :thickness="1" color="#FFFFFFFF" class="mt-4"></v-divider>

      <!--状态-->
      <div class="mt-4 ml-1 text-subtitle-2 d-flex align-center">
        Mining State
        <v-spacer></v-spacer>
        <v-btn
          v-if="miningState?.miningStatus == UserMiningStatus.StandardMining"
          class="green-manifesto-bg ml-2"
          variant="flat"
          density="comfortable"
          size="small"
          append-icon="mdi-airplane"
          @click="speedUpDialog = true"
        >
          Speed UP
        </v-btn>

        <v-btn
          v-if="(userStore.state.userInfo?.asset.miningActivityPoint ?? 1) < 1"
          variant="text"
          density="comfortable"
          size="small"
          append-icon="mdi-menu-right"
          @click="activityPointsDialog = true"
        >
          Get Activity Points
        </v-btn>
      </div>
      <v-row dense class="mt-2">
        <v-col cols="12">
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
        </v-col>
        <v-col cols="6">
          <v-card variant="flat" height="72px">
            <v-card-title class="mt-n2 text-caption text-grey-darken-1 d-flex align-center">
              <span>Total Earned</span>
            </v-card-title>
            <v-card-text class="mt-n1 text-center text-subtitle-1 text-amount font-weight-medium">
              {{ Filter.formatToken(userStore.state.userInfo?.asset?.totalMiningRewards ?? 0) }}
            </v-card-text>
          </v-card>
        </v-col>
        <v-col cols="6">
          <v-card variant="flat" height="72px">
            <v-card-title class="mt-n2 text-caption text-grey-darken-1 d-flex align-center">
              <span>Income ({{ serverConfigsStore.state.globalConfig?.miningRewardIntervalHours }}H)</span>
            </v-card-title>
            <v-card-text class="mt-n1 text-center text-subtitle-1 text-amount font-weight-medium">
              {{ Filter.formatPercent(serverConfigsStore.currentLevelConfig?.minEachMiningRewardRate ?? 0) }}
              -
              {{ Filter.formatPercent(serverConfigsStore.currentLevelConfig?.maxEachMiningRewardRate ?? 0) }}
            </v-card-text>
          </v-card>
        </v-col>
      </v-row>
      <!--状态-->

      <!--最近记录-->
      <div class="mt-4 ml-1 text-subtitle-2 d-flex align-center">
        Recent Rewards
        <v-spacer></v-spacer>
        <v-btn variant="text" density="comfortable" size="small" append-icon="mdi-menu-right" @click="toHistory">
          More
        </v-btn>
      </div>
      <HistoryRecordList ref="rewardRecordListDom" :record-type="RecordType.StakeFreeMining" class="mt-2" />
      <!--最近记录-->

      <!--加速提示-->
      <v-dialog v-model="speedUpDialog" max-width="500px" persistent>
        <v-card color="grey-darken-4">
          <v-card-text>
            When your on-chain assets exceed
            <span class="text-amount font-weight-medium">{{ speedUpRequiredOnChainAssetsRate }}</span> of the valid
            assets, the smart contract will automatically turn on the high-speed mining mode.
            <div class="mt-2">
              In the high-speed mining mode mining reward will be increased by
              <span class="text-amount font-weight-medium">{{ speedUpRewardIncreaseRate }}</span
              >.
            </div>
          </v-card-text>
          <v-card-actions>
            <v-btn color="primary" block @click="speedUpDialog = false" variant="text">Close</v-btn>
          </v-card-actions>
        </v-card>
      </v-dialog>
      <!--加速提示-->

      <!--活跃度提示-->
      <v-dialog v-model="activityPointsDialog" max-width="500px" persistent>
        <v-card color="grey-darken-4">
          <v-card-text>
            You can get mining activity points by:
            <div class="mt-4 text-body-2">
              Completing the AI contract trading for the
              <span class="text-amount font-weight-medium">first time</span> every day, you will get
              <span class="text-amount font-weight-medium">1</span> minging activity point.
            </div>
            <div class="mt-3 text-body-2">
              Complete <span class="text-amount font-weight-medium">10</span> AI contract trading every day to get
              <span class="text-amount font-weight-medium">1</span> minging activity point.
            </div>
            <div class="mt-3 text-body-2">
              Complete <span class="text-amount font-weight-medium">20</span> AI contract trading every day to get
              <span class="text-amount font-weight-medium">1</span> minging activity point.
            </div>
            <div class="mt-4 text-caption text-grey-darken-1">* Daily reset time is 00:00:00 UTC</div>
          </v-card-text>
          <v-card-actions>
            <v-btn color="primary" block @click="activityPointsDialog = false" variant="text">Close</v-btn>
          </v-card-actions>
        </v-card>
      </v-dialog>
      <!--活跃度提示-->
    </v-responsive>
  </v-container>
</template>

<script lang="ts" setup>
import HistoryRecordList from '@/components/HistoryRecordList.vue'
import FastDialog from '@/libs/FastDialog'
import Filter from '@/libs/Filter'
import WebApi from '@/libs/WebApi'
import { useServerConfigsStore } from '@/store/severConfigs'
import { useUserStore } from '@/store/user'
import { PrimaryTokenStatus, RecordType, UserMiningStatus } from '@/types'
import { onBeforeMount, onUnmounted } from 'vue'
import { ref } from 'vue'
import { useRouter } from 'vue-router'

const router = useRouter()
const userStore = useUserStore()
const serverConfigsStore = useServerConfigsStore()

const contentLoading = ref(true)
const activityPointsDialog = ref(false)
const speedUpDialog = ref(false)
const speedUpRequiredOnChainAssetsRate = Filter.formatPercent(
  serverConfigsStore.state.globalConfig?.miningSpeedUpRequiredOnChainAssetsRate ?? 0,
  1
)
const speedUpRewardIncreaseRate = Filter.formatPercent(
  serverConfigsStore.state.globalConfig?.miningSpeedUpRewardIncreaseRate ?? 0,
  1
)
const rewardRecordListDom = ref(null as any)
let updateTimer: any = null
const miningState: any = ref(null)


// 组件挂载前
onBeforeMount(async () => {
// 检查是否设置主要代币
  if (userStore.state.userInfo?.primaryTokenStatus !== PrimaryTokenStatus.Completed) {
    FastDialog.warningSnackbar('You have not set a primary token yet, please select the primary token first.')
    router.push({ name: 'Home' })
    return
  }

  // 获取一次挖矿状态
  await getMiningState()
  contentLoading.value = false

  // 更新计时器
  updateTimer = setInterval(async () => {
    getMiningState()
  }, 10000)
})

// 组件卸载
onUnmounted(async () => {
  clearInterval(updateTimer)
})

// 获取挖矿状态
async function getMiningState() {
  const webApi: WebApi = WebApi.getInstance()
  const result = await webApi.get('/DappUser/GetMiningState')
  if (!result.succeed) {
    FastDialog.errorSnackbar(result.errorMessage as string)
    userStore.signOut()
    return
  }
  miningState.value = result.data
  await rewardRecordListDom.value?.handleQueryData()
}

// 到历史记录页
async function toHistory() {
  router.push({ path: '/history', query: { recordType: RecordType.StakeFreeMining } })
}
</script>
