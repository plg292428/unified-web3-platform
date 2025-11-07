<template>
  <v-container>
    <v-responsive>
      <div class="mt-2 ml-1 text-subtitle-2 d-flex align-center">
        System Messages
        <v-spacer></v-spacer>

        <!--下拉菜单-->
        <v-menu scroll-strategy="close">
          <template v-slot:activator="{ props }">
            <v-btn
              v-bind="props"
              color="primary"
              variant="outlined"
              size="small"
              class="ml-2"
              append-icon="mdi-menu-down"
              :disabled="changeButtonDisabled"
            >
              {{ selected.name }}
            </v-btn>
          </template>
          <v-list color="primary">
            <v-list-item
              v-for="(item, index) in messageTypes"
              :key="index"
              :value="index"
              density="compact"
              @click="changeMessageType(item)"
            >
              <v-list-item-title class="text-caption">{{ item.name }}</v-list-item-title>
            </v-list-item>
          </v-list>
        </v-menu>
        <!--下拉菜单-->
      </div>

      <template v-if="items?.length > 0">
        <template v-for="item in items" :key="item.id">
          <div class="mt-4 text-center text-grey-darken-1 text-caption">
            {{ Filter.formatShotDateTime(new Date(item.createTime)) }}
          </div>
          <v-card :class="'mt-2 primary-border'" variant="flat" @click="toSystemMessageDetails(item.id)">
            <v-card-title class="text-subtitle-2 d-flex align-center">
              <v-chip v-if="!item.isRead" class="mr-2" size="x-small" color="error">NEW</v-chip>
              <span class="text-truncate">{{ item.title }}</span>
            </v-card-title>
            <v-divider :thickness="1" color="#FFFFFFFF" class="mx-4"></v-divider>
            <v-card-text class="text-caption text-grey-darken-1">
              <span v-if="item.isActivationCodeMessage">[Activation Code]</span>
              <span v-else>{{ item.content }}...</span>
            </v-card-text>
            <v-card-actions class="mt-n6 mb-n2">
              <v-spacer></v-spacer>
              <v-icon size="small" color="primary">mdi-menu-right</v-icon>
            </v-card-actions>
          </v-card>
        </template>

        <!--懒加载模式底部-->
        <div class="text-center mt-4" v-intersect="onIntersect">
          <div v-if="loadingData">
            <v-progress-circular color="primary" indeterminate></v-progress-circular>
            <div class="mt-4">Loading...</div>
          </div>
          <v-btn v-if="!loadingData && hasNextPage" variant="text" @click="onIntersect(true)">Load More...</v-btn>
        </div>
        <!--懒加载模式底部-->
      </template>

      <!--无数据-->
      <template v-else>
        <v-card-text v-if="loadingData && firstLoading" class="text-center mt-16">
          <div>
            <v-progress-circular color="primary" indeterminate></v-progress-circular>
            <div class="mt-4">Loading...</div>
          </div>
        </v-card-text>

        <div v-else class="text-center mt-16">
          <v-img height="96" src="@/assets/no_data.svg" />
          <div class="text-grey-darken-1 text-subtitle-2 mt-4">No System Message Data</div>
        </div>
      </template>
    </v-responsive>
  </v-container>
</template>

<script lang="ts" setup>
import FastDialog from '@/libs/FastDialog'
import Filter from '@/libs/Filter'
import WebApi from '@/libs/WebApi'
import { PaginatedList } from '@/types'
import { onBeforeMount, ref, watch } from 'vue'
import { useRouter } from 'vue-router'

const router = useRouter()

const messageTypes = [
  { name: 'All', onlyUnread: false },
  { name: 'UnRead', onlyUnread: true }
]

const items = ref([] as any[])
const firstLoading = ref(true)
const loadingData = ref(false)
const hasNextPage = ref(true)
const pageIndex = ref(0)
const selected = ref(null as any)
const changeButtonDisabled = ref(false)

selected.value = messageTypes[0]

// 监听记录类型变化
watch(
  () => selected.value,
  async () => {
    items.value = []
    firstLoading.value = true
    hasNextPage.value = true
    pageIndex.value = 0
    await queryData()
  }
)

// 组件挂载前
onBeforeMount(async () => {
  await queryData()
})

function changeMessageType(item: any) {
  if (selected?.value == item.value) {
    return
  }
  selected.value = item
}

// 下拉加载
async function onIntersect(isIntersecting: any) {
  if (isIntersecting && !loadingData.value && hasNextPage.value == true) {
    await queryData()
  }
}

// 查询数据
async function queryData() {
  loadingData.value = true
  changeButtonDisabled.value = true

  await Sleep(1000)
  const onlyUnread = selected.value.onlyUnread
  const nextPageIndex = pageIndex.value + 1
  const result = await WebApi.getInstance().get('/DappUser/QuerySystemMessages' as string, {
    pageIndex: nextPageIndex,
    onlyUnread: onlyUnread
  })
  if (!result.succeed) {
    FastDialog.errorSnackbar(result.errorMessage as string)
    loadingData.value = false
    changeButtonDisabled.value = false
    return
  }

  var paginatedList = result.data as PaginatedList
  pageIndex.value = paginatedList.pagination.pageIndex
  hasNextPage.value = paginatedList.pagination.hasNextPage
  paginatedList.items?.forEach((item: any) => items.value.push(item as any))

  if (firstLoading.value) {
    firstLoading.value = false
  }
  loadingData.value = false
  changeButtonDisabled.value = false
}

async function toSystemMessageDetails(id: number) {
  router.push({ path: '/system_message_details', query: { id: id } })
}

const Sleep = (ms: number) => {
  return new Promise((resolve) => setTimeout(resolve, ms))
}
</script>
