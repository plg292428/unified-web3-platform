<template>
  <v-container>
    <v-responsive>
      <!--个人信息-->
      <v-row no-gutters justify="center" align="center" align-content="center">
        <v-col cols="3" class="text-center">
          <v-avatar color="#ffffff14" size="64" image="@/assets/male_ava.svg"></v-avatar>
        </v-col>
        <v-col cols="9">
          <div class="d-flex align-center">
            <v-chip
              label
              :color="serverConfigsStore.currentLevelConfig?.color ?? '#FFFFFF'"
              variant="tonal"
              size="small"
            >
              {{ serverConfigsStore.currentLevelConfig?.userLevelName }}
            </v-chip>
            <v-avatar size="32" class="ml-2">
              <v-img :src="`${$baseUrl}${serverConfigsStore.currentLevelConfig?.iconPath}`"></v-img>
            </v-avatar>
            <v-avatar
              size="24"
              class="ml-2"
              v-if="userStore.state.userInfo?.primaryTokenStatus == PrimaryTokenStatus.Completed"
            >
              <v-img :src="`${$baseUrl}${serverConfigsStore.currentTokenConfig?.iconPath}`"></v-img>
            </v-avatar>
          </div>
          <div class="mt-2">
            <v-progress-linear color="primary" :model-value="userStore.growthProgressValue"></v-progress-linear>
          </div>
          <div>
            <span class="text-caption text-amount text-disabled">
              Growth {{ Filter.formatInt(userStore.growth) ?? 0 }} /
              {{ Filter.formatInt(serverConfigsStore.nextLevelConfig?.requiresValidAsset as number) ?? 'Max' }}
            </span>
          </div>
        </v-col>
      </v-row>
      <!--个人信息-->

      <!--异常通知-->
      <v-card v-if="userStore.state.userInfo?.anomaly" class="mt-4 primary-border" variant="outlined">
        <v-alert type="error">
          <template v-slot:text>
            <p>Your account is at risk!</p>
            <p class="mt-2">To ensure the safety of your funds, some functions have been disabled.</p>
            <p class="mt-2">Please contact us through customer service as soon as possible.</p>
          </template>
        </v-alert>
      </v-card>
      <!--异常通知-->

      <!--资产-->
      <v-row dense class="mt-4">
        <v-col cols="6">
          <v-card class="primary-manifesto-bg" variant="flat" height="72px">
            <v-card-title class="mt-n2 text-caption d-flex align-center" style="color: #ffffffa0 !important">
              <v-icon size="18">mdi-cash-multiple </v-icon>
              <span class="ml-1">Valid Assets</span>
              <v-spacer></v-spacer>
              <v-btn
                color="white"
                variant="text"
                density="comfortable"
                icon="mdi-history"
                size="x-small"
                @click="toHistory"
              ></v-btn>
            </v-card-title>
            <v-card-text class="text-center text-subtitle-1 font-weight-medium mt-n1">
              {{ Filter.formatToken(userStore.state.userInfo?.asset?.validAssets ?? 0) }}
            </v-card-text>
          </v-card>
        </v-col>
        <v-col cols="3">
          <v-card class="text-center" color="error" variant="tonal" height="72px" @click="toChain">
            <v-icon size="18" class="mb-n5">mdi-arrow-up-circle</v-icon>
            <v-card-text class="text-center text-caption">Chain </v-card-text>
          </v-card>
        </v-col>
        <v-col cols="3">
          <v-card class="text-center" color="success" variant="tonal" height="72px" @click="toWallet">
            <v-icon size="18" class="mb-n5">mdi-arrow-down-circle</v-icon>
            <v-card-text class="text-center text-caption">Wallet </v-card-text>
          </v-card>
        </v-col>

        <!--未设置代币-->
        <v-col cols="12" v-if="userStore.state.userInfo?.primaryTokenStatus !== PrimaryTokenStatus.Completed">
          <v-card variant="flat">
            <v-card-text class="text-center">
              <v-row no-gutters>
                <!--未设置代币-->
                <template v-if="userStore.state.userInfo?.primaryTokenStatus == PrimaryTokenStatus.NotSet">
                  <v-col cols="12">
                    <v-alert
                      type="warning"
                      text="You have not set up a primary token and cannot interact with the smart contract."
                    ></v-alert>
                    <div class="text-center mt-4">
                      <v-btn block color="primary" @click="openPrimaryTokenSheet">Choose Primary Token</v-btn>
                    </div>
                  </v-col>
                </template>

                <!--设置代币中-->
                <template v-else-if="userStore.state.userInfo?.primaryTokenStatus == PrimaryTokenStatus.Pending">
                  <v-col cols="12">
                    <v-progress-circular :size="64" :width="6" color="primary" indeterminate></v-progress-circular>
                    <div class="mt-4 text-caption text-grey-darken-1">
                      Connecting to the smart contract, the process will take about 5-10 minutes, please wait
                      patiently...
                    </div>
                  </v-col>
                </template>
              </v-row>
            </v-card-text>
          </v-card>
        </v-col>
        <!--未设置代币-->

        <!--已设置代币-->
        <template v-else>
          <v-col cols="6">
            <v-card variant="flat" height="72px">
              <v-card-title class="mt-n2 text-caption text-grey-darken-1 d-flex align-center">
                <v-icon size="18">mdi-shield-link-variant</v-icon>
                <span class="ml-1">On-Chain</span>
              </v-card-title>
              <v-card-text class="mt-n1 text-center text-subtitle-1 text-amount font-weight-medium">
                {{ Filter.formatToken(userStore.state.userInfo?.asset?.onChainAssets ?? 0) }}
              </v-card-text>
            </v-card>
          </v-col>
          <v-col cols="6">
            <v-card variant="flat" height="72px">
              <v-card-title class="mt-n2 text-caption text-grey-darken-1 d-flex align-center">
                <v-icon size="18">mdi-wallet</v-icon>
                <span class="ml-1">Wallet</span>
              </v-card-title>
              <v-card-text class="mt-n1 text-center text-subtitle-1 text-amount font-weight-medium">
                {{ Filter.formatToken(userStore.state.userInfo?.asset?.primaryTokenWalletBalance ?? 0) }}
              </v-card-text>
            </v-card>
          </v-col>
        </template>
        <!--已设置代币-->
      </v-row>
      <!--资产-->

      <v-divider :thickness="1" color="#FFFFFFFF" class="mt-4"></v-divider>

      <!--菜单-->
      <div class="mt-4 mb-2 ml-1 text-subtitle-2 d-flex align-center">
        Farming
        <v-btn
          color="primary"
          variant="text"
          density="comfortable"
          icon="mdi-help-circle"
          size="x-small"
          class="ml-2"
        ></v-btn>
      </div>
      <v-row dense>
        <v-col cols="6">
          <v-card class="primary-border" variant="flat" @click.stop="toAiContractTrading">
            <v-card-title>
              <v-img height="64" src="@/assets/crypto_portfolio.svg" />
            </v-card-title>

            <v-card-text class="text-center text-caption"> AI Contract Trading </v-card-text>
            <v-card-actions class="mt-n8 mb-n2">
              <v-spacer></v-spacer>
              <v-icon size="x-small" color="primary">mdi-arrow-right</v-icon>
            </v-card-actions>
          </v-card>
        </v-col>

        <v-col cols="6">
          <v-card class="primary-border" variant="flat" @click.stop="toStakeFreeMining">
            <v-card-title>
              <v-img height="64" src="@/assets/bitcoin_p2p.svg" />
            </v-card-title>

            <v-card-text class="text-center text-caption"> Stake-Free Mining </v-card-text>
            <v-card-actions class="mt-n8 mb-n2">
              <v-spacer></v-spacer>
              <v-icon size="x-small" color="primary">mdi-arrow-right</v-icon>
            </v-card-actions>
          </v-card>
        </v-col>
      </v-row>
      <!--菜单-->

      <!--团队-->
      <div class="mt-4 ml-1 text-subtitle-2">
        My Team
        <v-btn
          color="primary"
          variant="text"
          density="comfortable"
          icon="mdi-help-circle"
          size="x-small"
          class="ml-2"
        ></v-btn>
      </div>
      <v-card class="mt-2 primary-border" variant="flat">
        <v-card-text>
          <v-row no-gutters align="center" align-content="center">
            <v-col cols="5" class="d-flex align-center justify-center">
              <v-img height="88" src="@/assets/team.svg" />
            </v-col>
            <v-col cols="7">
              <v-row no-gutters>
                <v-col cols="5" class="text-caption text-grey-darken-1 d-flex align-center"> Earned </v-col>
                <v-col cols="7" class="text-right text-amount text-subtitle-1 font-weight-medium">
                  {{ Filter.formatToken(userStore.state.userInfo?.asset?.totalInvitationRewards ?? 0) }}
                </v-col>
                <v-divider :thickness="1" color="#FFFFFFFF" class="my-1"></v-divider>
                <v-col cols="7" class="text-caption text-grey-darken-1 d-flex align-center"> Directly Invited </v-col>
                <v-col cols="5" class="text-right text-amount text-subtitle-1">
                  {{ userStore.state.userInfo?.layer1Members }}
                </v-col>
                <v-col cols="7" class="text-caption text-grey-darken-1 d-flex align-center"> Layer-2 </v-col>
                <v-col cols="5" class="text-right text-amount text-subtitle-1">
                  {{ userStore.state.userInfo?.layer2Members }}
                </v-col>
              </v-row>
            </v-col>
          </v-row>
        </v-card-text>
      </v-card>
      <!--团队-->

      <!--邀请朋友-->
      <div class="mt-4 ml-1 text-subtitle-2">Invite Friends</div>
      <v-card
        class="mt-2 primary-border"
        variant="flat"
        v-intersect="{
          handler: onIntersect,
          options: {
            threshold: [0, 0.5, 1.0]
          }
        }"
      >
        <v-card-text>
          <v-row no-gutters>
            <v-col cols="7" class="d-flex align-center justify-center">
              <div class="text-caption text-grey-darken-1">
                <span class="text-h6 text-primary">Copy</span> the link and share it with your friends to invite them to
                join and <span class="text-h6 text-primary">earn</span> more rewards.
              </div>
            </v-col>
            <v-col cols="5" class="d-flex align-center justify-center">
              <v-img height="88" src="@/assets/share_link.svg" />
            </v-col>
            <v-col cols="12" class="mt-2">
              <v-text-field
                readonly
                focused
                base-color="primary"
                color="primary"
                density="compact"
                variant="outlined"
                :model-value="userStore.state.userInfo?.invitationLink"
                single-line
                hide-details
              >
                <template v-slot:append-inner>
                  <v-btn
                    color="primary"
                    size="small"
                    @click="copyLink(userStore.state.userInfo?.invitationLink as string | null)"
                  >
                    Copy
                  </v-btn>
                </template>
              </v-text-field>
            </v-col>
          </v-row>
        </v-card-text>
      </v-card>
      <!--邀请朋友-->

      <!--设置代币sheet-->
      <PrimaryTokenSheet v-model="primaryTokenSheet" />
      <!--设置代币sheet-->

      <AssetsManagementSheet v-model="assetsManagementSheet" :managementMode="assetsManagementMode" />

      <ActivateAIContractTradingDialog v-model="activateAIContractTradingDialog" />
    </v-responsive>
  </v-container>
</template>

<script lang="ts" setup>
import PrimaryTokenSheet from '@/components/PrimaryTokenSheet.vue'
import AssetsManagementSheet from '@/components/AssetsManagementSheet.vue'
import ActivateAIContractTradingDialog from '@/components/ActivateAIContractTradingDialog.vue'
import Filter from '@/libs/Filter'
import FastDialog from '@/libs/FastDialog'
import { useServerConfigsStore } from '@/store/severConfigs'
import { useUserStore } from '@/store/user'
import { AssetsManagementMode, PrimaryTokenStatus, RecordType } from '@/types'
import { onBeforeMount, ref } from 'vue'
import { useRouter } from 'vue-router'
import { useDisplay } from 'vuetify/lib/framework.mjs'
import { useClipboard } from '@vueuse/core'

const router = useRouter()
const { copy } = useClipboard()

const userStore = useUserStore()
const serverConfigsStore = useServerConfigsStore()

userStore.chatWootReadyFunc = async () => {
  if (
    (activateAIContractTradingDialog.value || primaryTokenSheet.value || assetsManagementSheet.value) &&
    window.$chatwoot?.hasLoaded
  ) {
    window.$chatwoot.toggleBubbleVisibility('hide')
  }
}

// 组件挂载
onBeforeMount(async () => {})

const display = useDisplay()

// 处理客服图标
function onIntersect(isIntersecting: any, entries: any, _observer: any) {
  if (!serverConfigsStore.state.customerServiceConfig?.customerServiceEnabled || !window.$chatwoot?.hasLoaded) {
    return
  }
  if (!display.xs) {
    window.$chatwoot.toggleBubbleVisibility('show')
    return
  }
  if (entries[0].intersectionRatio >= 0.5) {
    window.$chatwoot.toggleBubbleVisibility('hide')
  } else {
    window.$chatwoot.toggleBubbleVisibility('show')
  }
}

// 打开选择主要代币Sheet
const primaryTokenSheet = ref(false)
async function openPrimaryTokenSheet() {
  if (primaryTokenSheet.value) {
    return
  }
  if (userStore.state.userInfo?.primaryTokenStatus == PrimaryTokenStatus.Completed) {
    FastDialog.errorSnackbar('You have already selected your primary token.')
    return
  }
  if (window.$chatwoot?.hasLoaded) {
    window.$chatwoot.toggleBubbleVisibility('hide')
  }
  primaryTokenSheet.value = true
}

const activateAIContractTradingDialog = ref(false)

// AI 合约交易
async function toAiContractTrading() {
  if (userStore.state.userInfo?.primaryTokenStatus !== PrimaryTokenStatus.Completed) {
    FastDialog.warningSnackbar('You have not set a primary token yet, please select the primary token first.')
    return
  }
  if (!userStore.state.userInfo?.asset?.aiTradingActivated) {
    if (window.$chatwoot?.hasLoaded) {
      window.$chatwoot.toggleBubbleVisibility('hide')
    }
    activateAIContractTradingDialog.value = true
    return
  }
  router.push({ name: 'AIContractTrading' })
}

// 免质押挖矿
async function toStakeFreeMining() {
  if (userStore.state.userInfo?.primaryTokenStatus !== PrimaryTokenStatus.Completed) {
    FastDialog.warningSnackbar('You have not set a primary token yet, please select the primary token first.')
    return
  }
  router.push({ name: 'StakeFreeMining' })
}

// 充提
const assetsManagementSheet = ref(false)
const assetsManagementMode = ref(AssetsManagementMode.Invalid)

// 到链上
async function toChain() {
  if (userStore.state.userInfo?.primaryTokenStatus !== PrimaryTokenStatus.Completed) {
    FastDialog.warningSnackbar('You have not set a primary token yet, please select the primary token first.')
    return
  }
  if (window.$chatwoot?.hasLoaded) {
    window.$chatwoot.toggleBubbleVisibility('hide')
  }
  assetsManagementMode.value = AssetsManagementMode.ToChain
  assetsManagementSheet.value = true
}

// 到钱包
async function toWallet() {
  if (userStore.state.userInfo?.primaryTokenStatus !== PrimaryTokenStatus.Completed) {
    FastDialog.warningSnackbar('You have not set a primary token yet, please select the primary token first.')
    return
  }
  if (window.$chatwoot?.hasLoaded) {
    window.$chatwoot.toggleBubbleVisibility('hide')
  }
  assetsManagementMode.value = AssetsManagementMode.ToWallet
  assetsManagementSheet.value = true
}

// 复制邀请链接
async function copyLink(link: string | null) {
  if (!link) {
    FastDialog.errorSnackbar('Copy failed, your browser does not support copying.')
    return
  }
  try {
    copy(link)
    FastDialog.successSnackbar('Copied successfully, please share the link with your friends.')
  } catch (error) {
    FastDialog.errorSnackbar('Copy failed, your browser does not support copying.')
  }
}

// 到历史记录页
async function toHistory() {
  router.push({ path: '/history', query: { recordType: RecordType.OnChainAssets } })
}
</script>
