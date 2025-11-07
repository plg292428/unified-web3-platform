<template>
  <v-container>
    <v-responsive>
      <div class="mt-2 ml-1 text-subtitle-2 d-flex align-center">
        History Records
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
              {{ selected.title }}
            </v-btn>
          </template>
          <v-list color="primary">
            <v-list-item
              v-for="(item, index) in HistoryRecordParams"
              :key="index"
              :value="index"
              density="compact"
              @click="changeRecordType(item)"
            >
              <v-list-item-title class="text-caption">{{ item.title }}</v-list-item-title>
            </v-list-item>
          </v-list>
        </v-menu>
        <!--下拉菜单-->
      </div>

      <HistoryRecordList @on-load-status-change="onLoadStatusChange" :record-type="selected.value" lazy-load class="mt-2" />
    </v-responsive>
  </v-container>
</template>

<script lang="ts" setup>
import HistoryRecordList from '@/components/HistoryRecordList.vue'
import { RecordType } from '@/types'
import { ref } from 'vue'
import { useRoute } from 'vue-router'
import { HistoryRecordParams } from '@/libs/Constants'

const route = useRoute()

const selected = ref(null as any)
const changeButtonDisabled = ref(false)

const recordType = Number(route.query?.recordType) ?? RecordType.OnChainAssets
selected.value = HistoryRecordParams.find((o) => o.value == recordType)

function changeRecordType(item: any) {
  if (selected?.value == item.value) {
    return
  }
  selected.value = item
}

function onLoadStatusChange(val: boolean) {
  changeButtonDisabled.value = val
}
</script>
