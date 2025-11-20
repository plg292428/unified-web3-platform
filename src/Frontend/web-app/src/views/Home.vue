<template>
  <v-container>
    <v-responsive>
      <!-- Personal Information -->
      <v-row v-if="isSigned" no-gutters justify="center" align="center" align-content="center">
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
      <!-- Personal Information -->

      <!-- Anomaly Notification -->
      <v-card v-if="userStore.state.userInfo?.anomaly" class="mt-4 primary-border" variant="outlined">
        <v-alert type="error">
          <template v-slot:text>
            <p>Your account is at risk!</p>
            <p class="mt-2">To ensure the safety of your funds, some functions have been disabled.</p>
            <p class="mt-2">Please contact us through customer service as soon as possible.</p>
          </template>
        </v-alert>
      </v-card>
      <!-- Anomaly Notification -->

      <WalletStatusCard />

      <!-- Product Display -->
      <div class="mt-6">
        <div class="ml-1 d-flex align-center text-subtitle-2">
          Hot Products
          <v-chip class="ml-2" size="x-small" color="primary" variant="tonal" label>Live</v-chip>
          <v-spacer></v-spacer>
          <v-btn color="primary" variant="tonal" size="small" @click="handleOpenCart">
            <v-icon start size="18">mdi-cart</v-icon>
            Cart ({{ cartTotalQuantity }})
          </v-btn>
        </div>

        <!-- Search and Filter -->
        <v-row dense class="mt-3">
          <v-col cols="12" md="6">
            <v-text-field
              v-model="searchKeyword"
              density="compact"
              variant="outlined"
              placeholder="Search products..."
              prepend-inner-icon="mdi-magnify"
              clearable
              hide-details
              @update:model-value="onSearchKeywordChange"
            ></v-text-field>
          </v-col>
          <v-col cols="6" md="3">
            <v-select
              v-model="sortBy"
              density="compact"
              variant="outlined"
              :items="sortOptions"
              item-title="label"
              item-value="value"
              hide-details
              @update:model-value="onSortChange"
            ></v-select>
          </v-col>
          <v-col cols="6" md="3">
            <v-btn
              color="primary"
              variant="outlined"
              density="compact"
              block
              @click="showFilterDialog = true"
            >
              <v-icon start size="18">mdi-filter</v-icon>
              Filter
            </v-btn>
          </v-col>
        </v-row>

        <v-slide-group class="mt-3" show-arrows="always">
          <v-slide-group-item>
            <v-chip
              class="mr-2 mb-2"
              label
              size="small"
              :color="selectedCategoryId === null ? 'primary' : undefined"
              variant="tonal"
              @click="onSelectCategory(null)"
            >
              All
            </v-chip>
          </v-slide-group-item>
          <v-slide-group-item v-for="category in categoryOptions" :key="category.categoryId">
            <v-chip
              class="mr-2 mb-2"
              label
              size="small"
              :color="selectedCategoryId === category.categoryId ? 'primary' : undefined"
              variant="tonal"
              @click="onSelectCategory(category.categoryId)"
            >
              {{ category.name }}
            </v-chip>
          </v-slide-group-item>
        </v-slide-group>

        <v-row dense class="mt-2">
          <template v-if="productLoading">
            <v-col cols="12" md="4" v-for="n in productPageSize" :key="`product-skel-${n}`">
              <v-skeleton-loader type="image, article, actions" class="primary-border" />
            </v-col>
          </template>
          <template v-else-if="productList.length">
            <v-col cols="12" md="4" v-for="product in productList" :key="product.productId">
              <v-card 
                class="primary-border product-card" 
                variant="outlined" 
                height="100%"
                style="cursor: pointer; transition: all 0.2s;"
                @click="viewProductDetail(product.productId)"
                @mouseenter="(e) => e.currentTarget.style.transform = 'translateY(-4px)'"
                @mouseleave="(e) => e.currentTarget.style.transform = 'translateY(0)'"
              >
                <v-img
                  :src="resolveProductImage(product)"
                  height="160"
                  cover
                  class="product-image"
                  style="cursor: pointer;"
                  @click="viewProductDetail(product.productId)"
                >
                  <template v-slot:placeholder>
                    <div class="d-flex align-center justify-center fill-height bg-grey-darken-3">
                      <v-progress-circular color="primary" indeterminate></v-progress-circular>
                    </div>
                  </template>
                </v-img>
                <v-card-title 
                  class="text-subtitle-1"
                  @click.stop="viewProductDetail(product.productId)"
                >
                  {{ product.name }}
                </v-card-title>
                <v-card-subtitle class="text-caption text-grey-lighten-1">
                  {{ product.subtitle ?? product.categoryName }}
                </v-card-subtitle>
                <v-card-text class="text-body-2 text-grey-lighten-2">
                  Stock: {{ product.inventoryAvailable }}
                </v-card-text>
                <v-divider></v-divider>
                <v-card-actions class="justify-space-between">
                  <div @click.stop>
                    <div class="text-subtitle-1 text-primary font-weight-medium">
                      {{ Filter.formatPrice(product.price) }} {{ product.currency }}
                    </div>
                    <div class="text-caption text-grey-lighten-1">Updated {{ formatDateTime(product.updateTime) }}</div>
                  </div>
                  <v-btn
                    color="primary"
                    append-icon="mdi-cart-plus"
                    :loading="cartProcessing"
                    @click.stop="handleAddToCart(product)"
                  >
                    Add to Cart
                  </v-btn>
                </v-card-actions>
              </v-card>
            </v-col>
          </template>
          <template v-else>
            <v-col cols="12">
              <v-alert type="info" variant="tonal">No products available. Please check back later.</v-alert>
            </v-col>
          </template>
        </v-row>

        <v-pagination
          v-if="productTotal > productPageSize"
          class="mt-3"
          v-model="productPage"
          :length="productTotalPages"
          color="primary"
          density="comfortable"
          @update:modelValue="handlePagination"
        />
      </div>
      <!-- Product Display -->

      <!-- Latest Orders -->
      <div class="mt-6">
        <div class="ml-1 d-flex align-center text-subtitle-2">
          Recent Orders
          <v-spacer></v-spacer>
          <v-btn
            v-if="isSigned"
            color="primary"
            variant="text"
            size="small"
            @click="$router.push({ name: 'Orders' })"
          >
            View All
            <v-icon end size="18">mdi-arrow-right</v-icon>
          </v-btn>
          <span v-else class="text-caption text-grey-lighten-1">Real-time order status sync</span>
        </div>
        <v-card class="primary-border mt-2" variant="outlined">
          <v-data-table
            :headers="orderHeaders"
            :items="orderTableItems"
            density="compact"
            :loading="orderListLoading"
            hide-default-footer
          >
            <template #loading>
              <v-skeleton-loader type="table" class="ma-4" />
            </template>
            <template #item.statusText="{ item }">
              <v-chip size="x-small" :color="item.statusColor" variant="tonal" label>
                {{ item.statusText }}
              </v-chip>
            </template>
            <template #no-data>
              <v-alert type="info" variant="tonal" border="start" class="ma-4">
                No orders yet. Complete a checkout to view order status here.
              </v-alert>
            </template>
          </v-data-table>
        </v-card>
      </div>
      <!-- Latest Orders -->

      <!-- Assets -->
      <v-row v-if="isSigned" dense class="mt-4">
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

        <!-- Token Not Set -->
        <v-col cols="12" v-if="userStore.state.userInfo?.primaryTokenStatus !== PrimaryTokenStatus.Completed">
          <v-card variant="flat">
            <v-card-text class="text-center">
              <v-row no-gutters>
                <!-- Token Not Set -->
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

                <!-- Setting Token -->
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
        <!-- Token Not Set -->

        <!-- Token Set -->
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
        <!-- Token Set -->
      </v-row>
      <!-- Assets -->

      <!-- Bitget DeepLink Payment -->
      <v-row dense class="mt-4">
        <v-col cols="12">
          <v-card class="primary-border" variant="outlined">
            <v-card-title class="text-subtitle-2 d-flex align-center">
              Pay with Bitget Wallet
              <v-chip class="ml-2" size="x-small" color="primary" label variant="tonal">DeepLink</v-chip>
            </v-card-title>
            <v-card-text class="text-caption text-grey-lighten-1">
              Pay directly via Bitget Wallet deep link. Select a chain and enter amount to open Bitget Wallet.
              <v-row class="mt-3" dense>
                <v-col cols="12" sm="4">
                  <v-btn block color="primary" @click="openBitgetPay('tron')">TRON · TYDdA1hvk...</v-btn>
                </v-col>
                <v-col cols="12" sm="4">
                  <v-btn block color="secondary" @click="openBitgetPay('eth')">Ethereum · 0x426d...E3</v-btn>
                </v-col>
                <v-col cols="12" sm="4">
                  <v-btn block color="info" @click="openBitgetPay('btc')">Bitcoin · bc1pd9vh...</v-btn>
                </v-col>
              </v-row>
            </v-card-text>
          </v-card>
        </v-col>
      </v-row>
      <!-- Bitget DeepLink Payment -->

      <v-divider :thickness="1" color="#FFFFFFFF" class="mt-4"></v-divider>

      <!-- Menu -->
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
      <!-- Menu -->

      <!-- Team -->
      <template v-if="isSigned">
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
      </template>
      <!-- Team -->

      <!-- Invite Friends -->
      <template v-if="isSigned">
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
      </template>
      <!-- Invite Friends -->


      <v-dialog v-model="orderSuccessDialog" max-width="520">
        <v-card>
          <v-card-title class="d-flex align-center">
            Order Created Successfully
            <v-spacer></v-spacer>
            <v-btn icon="mdi-close" variant="text" @click="closeOrderSuccessDialog"></v-btn>
          </v-card-title>
          <v-divider></v-divider>
          <v-card-text v-if="orderSuccessDetail">
            <div class="text-body-2">Order Number: {{ orderSuccessDetail.orderNumber }}</div>
            <div class="text-body-2 mt-2">
              Amount: {{ Filter.formatToken(orderSuccessDetail.totalAmount) }} {{ orderSuccessDetail.currency }}
            </div>
            <div class="text-body-2 mt-2">Payment Method: {{ orderSuccessDetail.paymentMethod ?? 'PayFi' }}</div>
            <div class="text-body-2 mt-2">
              Status: {{ getOrderStatusMeta(orderSuccessDetail.status).text }}
            </div>
            <v-divider class="my-3"></v-divider>
            <div class="text-body-2 font-weight-medium mb-2">Product Information</div>
            <v-list density="compact">
              <v-list-item v-for="item in orderSuccessDetail.items" :key="item.orderItemId">
                <v-list-item-title>{{ item.productName }}</v-list-item-title>
                <v-list-item-subtitle>
                  {{ item.quantity }} pcs · {{ Filter.formatToken(item.unitPrice) }} {{ orderSuccessDetail.currency }}
                </v-list-item-subtitle>
              </v-list-item>
            </v-list>
            <div class="text-caption text-grey-lighten-1 mt-3">
              Please refresh the page after payment to view the latest status.
            </div>
          </v-card-text>
          <v-card-actions>
            <v-spacer></v-spacer>
            <v-btn color="primary" variant="tonal" @click="closeOrderSuccessDialog">OK</v-btn>
            <v-btn color="primary" @click="closeOrderSuccessDialog">Continue Shopping</v-btn>
          </v-card-actions>
        </v-card>
      </v-dialog>

      <!--设置代币sheet-->
      <PrimaryTokenSheet v-model="primaryTokenSheet" />
      <!--设置代币sheet-->

      <AssetsManagementSheet v-model="assetsManagementSheet" :managementMode="assetsManagementMode" />

      <ActivateAIContractTradingDialog v-model="activateAIContractTradingDialog" />


      <!-- 订单支付对话框 -->
      <OrderPaymentDialog
        v-model="orderPaymentDialog"
        :order="orderPaymentDetail"
        @payment-success="handlePaymentSuccess"
      />
      <!-- 订单支付对话框 -->

      <!-- 订单成功 -->
      <v-dialog v-model="orderSuccessDialog" max-width="600px">
        <v-card>
          <v-card-title>Order Success</v-card-title>
          <v-card-text>
            <v-list v-if="orderSuccessDetail">
              <v-list-item>
                <v-list-item-title>Order Number: {{ orderSuccessDetail.orderNumber }}</v-list-item-title>
                <v-list-item-subtitle>
                  Total Amount: {{ Filter.formatToken(orderSuccessDetail.totalAmount) }} {{ orderSuccessDetail.currency }}
                </v-list-item-subtitle>
                <v-list-item-subtitle>
                  Payment Channel:
                  {{
                    orderSuccessDetail.paymentProviderName
                      ?? orderSuccessDetail.paymentMethod
                      ?? getPaymentModeLabel(orderSuccessDetail.paymentMode)
                  }}
                </v-list-item-subtitle>
                <v-list-item-subtitle>
                  Payment Status: {{ getPaymentStatusMeta(orderSuccessDetail.paymentStatus).text }}
                </v-list-item-subtitle>
                <v-list-item-subtitle>
                  Order Status: {{ getOrderStatusMeta(orderSuccessDetail.status).text }}
                </v-list-item-subtitle>
                <v-list-item-subtitle v-if="orderSuccessDetail.paymentWalletAddress">
                  Wallet Address: {{ formatWalletAddress(orderSuccessDetail.paymentWalletAddress) }}
                </v-list-item-subtitle>
                <v-list-item-subtitle v-if="orderSuccessDetail.paymentTransactionHash">
                  Transaction Hash: {{ orderSuccessDetail.paymentTransactionHash }}
                </v-list-item-subtitle>
                <v-list-item-subtitle>
                  Created At: {{ formatDateTime(orderSuccessDetail.createTime) }}
                </v-list-item-subtitle>
              </v-list-item>
            </v-list>
            <v-card-actions class="justify-end">
              <v-btn color="primary" @click="closeOrderSuccessDialog">Close</v-btn>
            </v-card-actions>
          </v-card-text>
        </v-card>
      </v-dialog>
      <!-- 订单成功 -->

      <!-- 筛选对话框 -->
      <v-dialog v-model="showFilterDialog" max-width="500">
        <v-card>
          <v-card-title class="d-flex align-center">
            Filter Products
            <v-spacer></v-spacer>
            <v-btn icon="mdi-close" variant="text" @click="showFilterDialog = false"></v-btn>
          </v-card-title>
          <v-card-text>
            <v-text-field
              v-model.number="filterMinPrice"
              label="Min Price"
              type="number"
              density="compact"
              variant="outlined"
              prepend-inner-icon="mdi-currency-usd"
              clearable
              class="mb-3"
            ></v-text-field>
            <v-text-field
              v-model.number="filterMaxPrice"
              label="Max Price"
              type="number"
              density="compact"
              variant="outlined"
              prepend-inner-icon="mdi-currency-usd"
              clearable
              class="mb-3"
            ></v-text-field>
            <v-select
              v-model="filterChainId"
              label="Select Chain"
              density="compact"
              variant="outlined"
              :items="chainOptions"
              item-title="title"
              item-value="value"
              clearable
              prepend-inner-icon="mdi-link-variant"
            ></v-select>
          </v-card-text>
          <v-card-actions>
            <v-spacer></v-spacer>
            <v-btn variant="text" @click="resetFilter">Reset</v-btn>
            <v-btn color="primary" @click="applyFilter">Apply</v-btn>
          </v-card-actions>
        </v-card>
      </v-dialog>
      <!-- 筛选对话框 -->
    </v-responsive>
  </v-container>
</template>

<script lang="ts" setup>
import PrimaryTokenSheet from '@/components/PrimaryTokenSheet.vue'
import AssetsManagementSheet from '@/components/AssetsManagementSheet.vue'
import ActivateAIContractTradingDialog from '@/components/ActivateAIContractTradingDialog.vue'
import WalletStatusCard from '@/components/WalletStatusCard.vue'
import OrderPaymentDialog from '@/components/OrderPaymentDialog.vue'
import Filter from '@/libs/Filter'
import FastDialog from '@/libs/FastDialog'
import WebApi from '@/libs/WebApi'
import productPlaceholderImage from '@/assets/online_transactions.svg?url'
import { useServerConfigsStore } from '@/store/severConfigs'
import { useUserStore } from '@/store/user'
import { useWalletStore } from '@/store/wallet'
import { useCartStore } from '@/store/cart'
import {
  AssetsManagementMode,
  PrimaryTokenStatus,
  RecordType,
  StoreCartItemResult,
  StoreOrderDetailResult,
  StoreOrderStatus,
  StorePaymentMode,
  StorePaymentStatus,
  StoreProductCategoryResult,
  StoreProductSummaryResult,
  StoreOrderSummaryResult
} from '@/types'
import { fetchOrderList, fetchProductCategories, fetchProductList } from '@/services/storeApi'
import { computed, onBeforeMount, ref, watch, nextTick } from 'vue'
import { useRouter } from 'vue-router'
import { useDisplay } from 'vuetify/lib/framework.mjs'
import { useClipboard } from '@vueuse/core'

const router = useRouter()
const { copy } = useClipboard()

const userStore = useUserStore()
const serverConfigsStore = useServerConfigsStore()
const walletStore = useWalletStore()
const cartStore = useCartStore()

const bitgetDeepLinkConfigs = {
  tron: {
    chain: 'tron',
    address: 'TYDdA1hvkotXZxiWm3yVoZqHeoY9DCrd4c',
    symbol: 'TRX',
    chainName: 'TRON'
  },
  eth: {
    chain: 'eth',
    address: '0x426d2aC6f5f486aC79E795f9efBEe609316438E3',
    symbol: 'ETH',
    chainName: 'Ethereum'
  },
  btc: {
    chain: 'btc',
    address: 'bc1pd9vh56wlglp7y3a7qqkmu7asvqn4305q7p0q2g92eevh5qhh276qz68e3k',
    symbol: 'BTC',
    chainName: 'Bitcoin'
  }
} as const

type BitgetDeepLinkChain = keyof typeof bitgetDeepLinkConfigs

// 商品/分类
const productCategories = ref<StoreProductCategoryResult[]>([])
const selectedCategoryId = ref<number | null>(null)
const productList = ref<StoreProductSummaryResult[]>([])
const productLoading = ref(false)
const productPage = ref(1)
const productPageSize = ref(6)
const productTotal = ref(0)

// 搜索和筛选
const searchKeyword = ref<string>('')
const sortBy = ref<string>('time_desc')
const showFilterDialog = ref(false)
const filterMinPrice = ref<number | null>(null)
const filterMaxPrice = ref<number | null>(null)
const filterChainId = ref<number | null>(null)

// 排序选项
const sortOptions = [
  { label: 'Newest', value: 'time_desc' },
  { label: 'Oldest', value: 'time_asc' },
  { label: 'Price: Low to High', value: 'price_asc' },
  { label: 'Price: High to Low', value: 'price_desc' },
  { label: 'Name A-Z', value: 'name_asc' },
  { label: 'Name Z-A', value: 'name_desc' }
]

// 链选项
const chainOptions = computed(() => {
  if (!serverConfigsStore.state.chainNetworkConfigs) {
    return []
  }
  return serverConfigsStore.state.chainNetworkConfigs.map((chain: any) => ({
    title: chain.chainName,
    value: chain.chainId
  }))
})

// 搜索防抖
let searchDebounceTimer: ReturnType<typeof setTimeout> | null = null
function onSearchKeywordChange() {
  if (searchDebounceTimer) {
    clearTimeout(searchDebounceTimer)
  }
  searchDebounceTimer = setTimeout(() => {
    productPage.value = 1
    loadProducts()
  }, 500) // 500ms 防抖
}

function onSortChange() {
  productPage.value = 1
  loadProducts()
}

const apiBaseUrl = WebApi.getInstance().baseUrl ?? ''

function resolveProductImage(product: StoreProductSummaryResult) {
  const url = product.thumbnailUrl
  if (!url) {
    return productPlaceholderImage
  }
  if (url.startsWith('http')) {
    return url
  }
  if (apiBaseUrl) {
    const normalizedBase = apiBaseUrl.endsWith('/') ? apiBaseUrl.slice(0, -1) : apiBaseUrl
    const normalizedPath = url.startsWith('/') ? url : `/${url}`
    return `${normalizedBase}${normalizedPath}`
  }
  return url
}

const productTotalPages = computed(() => {
  const total = Math.ceil(productTotal.value / productPageSize.value)
  return total > 0 ? total : 1
})

const categoryOptions = computed(() => {
  const flat: StoreProductCategoryResult[] = []
  const walk = (source: StoreProductCategoryResult[]) => {
    for (const item of source) {
      flat.push(item)
      if (item.children && item.children.length) {
        walk(item.children)
      }
    }
  }
  walk(productCategories.value)
  return flat
})

// 订单
const orderListLoading = ref(false)
const orderList = ref<StoreOrderSummaryResult[]>([])
const orderPage = ref(1)
const orderPageSize = ref(5)

const orderSuccessDialog = ref(false)
const orderSuccessDetail = ref<StoreOrderDetailResult | null>(null)
const orderPaymentDialog = ref(false)
const orderPaymentDetail = ref<StoreOrderDetailResult | null>(null)

const isSigned = computed(() => userStore.state.signed)

const cartItems = computed(() => cartStore.items)
const cartProcessing = computed(() => cartStore.processing)
const cartTotalAmount = computed(() => cartStore.totalAmount)
const cartTotalQuantity = computed(() => {
  const userCartQuantity = cartStore.totalQuantity
  if (userUid.value) {
    return userCartQuantity
  }
  // If not logged in, include temporary cart quantity
  // Use dynamic import to avoid circular dependencies
  let tempQuantity = 0
  try {
    // We'll calculate this synchronously by reading localStorage directly
    const stored = localStorage.getItem('temporary_cart')
    if (stored) {
      const items = JSON.parse(stored)
      const sevenDaysAgo = Date.now() - 7 * 24 * 60 * 60 * 1000
      const validItems = items.filter((item: any) => item.addedAt > sevenDaysAgo)
      tempQuantity = validItems.reduce((sum: number, item: any) => sum + item.quantity, 0)
    }
  } catch {
    // Ignore errors
  }
  return userCartQuantity + tempQuantity
})
const cartCurrency = computed(() => cartItems.value[0]?.currency ?? 'USDT')

const userUid = computed(() => userStore.state.userInfo?.uid ?? null)

// Chatwoot 气泡处理
userStore.chatWootReadyFunc = async () => {
  if (
    (activateAIContractTradingDialog.value || primaryTokenSheet.value || assetsManagementSheet.value) &&
    window.$chatwoot?.hasLoaded
  ) {
    window.$chatwoot.toggleBubbleVisibility('hide')
  }
}

// 数据加载
async function initData() {
  // 使用 console.error 确保日志能被看到（即使生产环境 terser 移除了 console.log）
  console.error('[Home] 开始加载数据...')
  // 等待 WebApi 初始化完成
  const webApi = WebApi.getInstance()
  if (!webApi.isReady) {
    console.error('[Home] 等待 WebApi 初始化...')
    // 最多等待 5 秒
    let retries = 50
    while (!webApi.isReady && retries > 0) {
      await new Promise(resolve => setTimeout(resolve, 100))
      retries--
    }
    if (!webApi.isReady) {
      console.error('[Home] WebApi 初始化超时，尝试继续加载')
    } else {
      console.error('[Home] WebApi 初始化完成')
    }
  }
  
  try {
    await loadCategories()
    console.error('[Home] 分类加载完成，数量:', productCategories.value.length)
  } catch (error) {
    console.error('[Home] 分类加载失败:', error)
  }
  
  try {
    await loadProducts()
    console.error('[Home] 商品加载完成，数量:', productList.value.length, '总计:', productTotal.value)
  } catch (error) {
    console.error('[Home] 商品加载失败:', error)
  }
}

onBeforeMount(async () => {
  await initData()
  
  // 监听 app-ready 事件，当用户登录完成后重新加载商品
  window.addEventListener('app-ready', async (event: any) => {
    console.log('[Home] 收到 app-ready 事件，重新加载商品')
    await initData()
  })
})

watch([selectedCategoryId, productPage], async () => {
  await loadProducts()
})

watch([filterMinPrice, filterMaxPrice, filterChainId], () => {
  if (showFilterDialog.value) {
    return // 在对话框中修改时不触发搜索
  }
  productPage.value = 1
  loadProducts()
})

watch(userUid, async (uid) => {
  if (uid) {
    try {
      await cartStore.refresh(uid)
    } catch (error) {
      console.warn(error)
    }
    await loadOrders(uid)
  } else {
    cartStore.reset()
    orderList.value = []
  }
}, { immediate: true })

async function loadCategories() {
  try {
    const categories = await fetchProductCategories()
    productCategories.value = categories
  } catch (error) {
    console.warn(error)
    // 访客模式下不显示网络错误提示
    const errorMessage = (error as Error).message ?? 'Failed to load categories'
    const isNetworkError = errorMessage.includes('无法连接到服务器') || 
                          errorMessage.includes('ERR_CONNECTION_REFUSED') ||
                          errorMessage.includes('ERR_NETWORK')
    const isGuestMode = !walletStore.state.provider || !walletStore.state.active
    if (!isGuestMode || !isNetworkError) {
      FastDialog.errorSnackbar(errorMessage)
    }
  }
}

async function loadProducts() {
  productLoading.value = true
  try {
    console.error('[Home] ========== 开始加载商品 ==========')
    
    // 检查 WebApi 状态
    const webApi = WebApi.getInstance()
    console.error('[Home] WebApi 状态:', {
      isReady: webApi.isReady,
      baseUrl: webApi.baseUrl
    })
    
    if (!webApi.isReady) {
      throw new Error('WebApi 未初始化，请刷新页面重试')
    }
    
    console.error('[Home] 调用商品API，参数:', {
      page: productPage.value,
      pageSize: productPageSize.value,
      categoryId: selectedCategoryId.value,
      keyword: searchKeyword.value
    })
    
    const result = await fetchProductList({
      page: productPage.value,
      pageSize: productPageSize.value,
      categoryId: selectedCategoryId.value ?? undefined,
      keyword: searchKeyword.value || undefined,
      sortBy: sortBy.value || undefined,
      minPrice: filterMinPrice.value ?? undefined,
      maxPrice: filterMaxPrice.value ?? undefined,
      chainId: filterChainId.value ?? undefined
    })
    
    console.error('[Home] API返回结果:', {
      itemsCount: result?.items?.length ?? 0,
      totalCount: result?.totalCount ?? 0,
      hasItems: !!(result?.items && result.items.length > 0)
    })
    
    productList.value = result.items || []
    productTotal.value = result.totalCount || 0
    
    // 如果返回空数据，输出警告
    if (!result.items || result.items.length === 0) {
      console.error('[Home] ⚠️ 商品列表为空')
      console.error('[Home] 完整API返回:', JSON.stringify(result, null, 2))
      // 即使数据为空，也清空列表，确保显示"No products available"提示
      productList.value = []
      productTotal.value = 0
    } else {
      console.error('[Home] ✅ 商品加载成功，已设置到 productList，数量:', productList.value.length)
    }
  } catch (error) {
    // 使用 console.error 确保错误能被看到
    console.error('[Home] ❌ 商品加载失败')
    console.error('[Home] 错误对象:', error)
    console.error('[Home] 错误详情:', {
      message: (error as Error).message,
      stack: (error as Error).stack,
      name: (error as Error).name
    })
    
    // 清空列表，确保显示错误提示
    productList.value = []
    productTotal.value = 0
    
    // 显示错误提示
    const errorMessage = (error as Error).message ?? '获取商品失败'
    const isNetworkError = errorMessage.includes('无法连接到服务器') || 
                          errorMessage.includes('ERR_CONNECTION_REFUSED') ||
                          errorMessage.includes('ERR_NETWORK') ||
                          errorMessage.includes('Instance not initialized') ||
                          errorMessage.includes('WebApi 未初始化')
    
    // 访客模式下也显示错误（但网络错误除外，因为可能是后端未启动）
    const isGuestMode = !walletStore.state.provider || !walletStore.state.active
    if (!isGuestMode || !isNetworkError) {
      FastDialog.errorSnackbar(errorMessage)
    } else {
      // 访客模式下的网络错误，只输出日志，不显示提示
      console.error('[Home] 访客模式下网络错误，不显示提示')
    }
  } finally {
    productLoading.value = false
    console.error('[Home] 商品加载完成，最终状态:', {
      loading: productLoading.value,
      count: productList.value.length,
      total: productTotal.value
    })
    console.error('[Home] ========== 商品加载流程结束 ==========')
  }
}

function applyFilter() {
  showFilterDialog.value = false
  productPage.value = 1
  loadProducts()
}

function resetFilter() {
  filterMinPrice.value = null
  filterMaxPrice.value = null
  filterChainId.value = null
  applyFilter()
}

async function loadOrders(uid: number) {
  orderListLoading.value = true
  try {
    const result = await fetchOrderList(uid, orderPage.value, orderPageSize.value)
    orderList.value = result.items
  } catch (error) {
    console.warn(error)
    // 访客模式下不显示网络错误提示
    const errorMessage = (error as Error).message ?? 'Failed to load orders'
    const isNetworkError = errorMessage.includes('无法连接到服务器') || 
                          errorMessage.includes('ERR_CONNECTION_REFUSED') ||
                          errorMessage.includes('ERR_NETWORK')
    const isGuestMode = !walletStore.state.provider || !walletStore.state.active
    if (!isGuestMode || !isNetworkError) {
      FastDialog.errorSnackbar(errorMessage)
    }
  } finally {
    orderListLoading.value = false
  }
}

const orderHeaders = [
  { title: 'Order Number', key: 'orderNumber' },
  { title: 'Amount', key: 'amount' },
  { title: 'Payment Method', key: 'paymentMethod' },
  { title: 'Payment Status', key: 'statusText' },
  { title: 'Created At', key: 'createTime' }
] as const

const orderTableItems = computed(() => {
  return orderList.value.map((order) => {
    const paymentMeta = getPaymentStatusMeta(order.paymentStatus)
    const orderMeta = getOrderStatusMeta(order.status)
    return {
      orderId: order.orderId,
      orderNumber: order.orderNumber,
      amount: `${Filter.formatToken(order.totalAmount)} ${order.currency}`,
      statusText: `${paymentMeta.text} · ${orderMeta.text}`,
      statusColor: paymentMeta.color,
      createTime: formatDateTime(order.createTime),
      paymentMethod:
        order.paymentProviderName ?? order.paymentMethod ?? getPaymentModeLabel(order.paymentMode)
    }
  })
})

function getOrderStatusMeta(status: StoreOrderStatus | number) {
  const mapping: Record<number, { text: string; color: string }> = {
    [StoreOrderStatus.PendingPayment]: { text: 'Pending Payment', color: 'warning' },
    [StoreOrderStatus.Paid]: { text: 'Paid', color: 'primary' },
    [StoreOrderStatus.Cancelled]: { text: 'Cancelled', color: 'grey' },
    [StoreOrderStatus.Completed]: { text: 'Completed', color: 'success' }
  }
  return mapping[Number(status)] ?? { text: 'Unknown', color: 'grey' }
}

function getPaymentStatusMeta(status: StorePaymentStatus | number) {
  const mapping: Record<number, { text: string; color: string }> = {
    [StorePaymentStatus.PendingSignature]: { text: 'Pending Signature', color: 'warning' },
    [StorePaymentStatus.AwaitingOnChainConfirmation]: { text: 'Awaiting Confirmation', color: 'info' },
    [StorePaymentStatus.Confirmed]: { text: 'Confirmed', color: 'success' },
    [StorePaymentStatus.Failed]: { text: 'Failed', color: 'error' },
    [StorePaymentStatus.Cancelled]: { text: 'Cancelled', color: 'grey' }
  }
  return mapping[Number(status)] ?? { text: 'Unknown', color: 'grey' }
}

function getPaymentModeLabel(mode: StorePaymentMode | number) {
  const mapping: Record<number, string> = {
    [StorePaymentMode.Traditional]: 'Traditional Payment',
    [StorePaymentMode.Web3]: 'Web3 Wallet'
  }
  return mapping[Number(mode)] ?? 'Unknown'
}

function formatWalletAddress(address?: string | null) {
  if (!address) {
    return ''
  }
  if (address.length <= 12) {
    return address
  }
  return `${address.slice(0, 6)}...${address.slice(-4)}`
}

function formatDateTime(value?: string | null) {
  if (!value) return '-'
  const date = new Date(value)
  if (Number.isNaN(date.getTime())) return value
  return date.toLocaleString()
}

function onSelectCategory(categoryId: number | null) {
  selectedCategoryId.value = categoryId
  productPage.value = 1
}

function handlePagination(page: number) {
  productPage.value = page
}

async function handleAddToCart(product: StoreProductSummaryResult) {
  const uid = userUid.value
  if (!uid) {
    // 未登录，添加到临时购物车
    const { addToTemporaryCart, getTemporaryCartTotalQuantity } = await import('@/utils/temporaryCart')
    addToTemporaryCart(product.productId, 1)
    const totalQuantity = getTemporaryCartTotalQuantity()
    FastDialog.successSnackbar(`Added to cart (${totalQuantity} items). Connect wallet to checkout.`)
    return
  }
  try {
    await cartStore.addItem(uid, product.productId, 1)
    FastDialog.successSnackbar('Added to cart')
  } catch (error) {
    console.warn(error)
    FastDialog.errorSnackbar((error as Error).message ?? 'Failed to add to cart')
  }
}

// 为添加到购物车功能引导登录
async function handleLoginForAddToCart(): Promise<boolean> {
  // 检查是否有钱包提供方
  if (!walletStore.state.provider) {
    console.log('No wallet provider detected in handleLoginForAddToCart, redirecting to Go page')
    FastDialog.warningSnackbar('No wallet detected. Redirecting to wallet setup page...')
    await nextTick()
    await new Promise(resolve => setTimeout(resolve, 500))
    window.location.replace('/go')
    return false
  }

  // 检查钱包是否已连接账户
  if (!walletStore.state.active || !walletStore.state.address) {
    try {
      FastDialog.infoSnackbar('Connecting wallet...')
      const accounts = await walletStore.connect()
      if (!accounts || accounts.length === 0) {
        FastDialog.errorSnackbar('Failed to connect wallet. Please try again.')
        return false
      }
    } catch (error) {
      console.error('Wallet connection error:', error)
      FastDialog.errorSnackbar('Failed to connect wallet. Please check your wallet and try again.')
      return false
    }
  }

  // 检查是否已登录
  const checkSignedResult = await userStore.checkSigned()
  if (!checkSignedResult.succeed) {
    FastDialog.errorSnackbar(checkSignedResult.errorMessage as string)
    return false
  }

  // 如果已登录，直接返回成功
  if (checkSignedResult.data?.singined) {
    return true
  }

  // 需要签名登录
  try {
    FastDialog.infoSnackbar('Please sign the message in your wallet to login...')
    const from = walletStore.state.address
    const provider = walletStore.state.provider
    const message = `0x${checkSignedResult.data.tokenText}`
    
    const signedText = await provider.request({
      method: 'personal_sign',
      params: [message, from]
    })

    // 执行登录
    const signResult = await userStore.signIn(signedText)
    if (!signResult.succeed) {
      FastDialog.errorSnackbar(signResult.errorMessage as string)
      return false
    }

    // 获取用户信息
    if (!(await userStore.updateUserInfo())) {
      userStore.signOut()
      FastDialog.errorSnackbar('Failed to get user information. Please try again.')
      return false
    }

    FastDialog.successSnackbar('Login successful!')
    return true
  } catch (error) {
    console.error('Login error:', error)
    if ((error as any)?.code === 4001) {
      FastDialog.warningSnackbar('Login cancelled. Please sign the message to add items to cart.')
    } else {
      FastDialog.errorSnackbar('Login failed. Please try again.')
    }
    return false
  }
}


function handlePaymentSuccess(order: StoreOrderDetailResult) {
  orderPaymentDialog.value = false
  orderSuccessDetail.value = order
  orderSuccessDialog.value = true
  const uid = userUid.value
  if (uid) {
    loadOrders(uid)
  }
}

async function handleOpenCart() {
  const uid = userUid.value
  if (!uid) {
    // 未登录，引导用户登录
    await handleLoginForCart()
    return
  }
  // 已登录，跳转到购物车页面
  router.push({ name: 'Cart' })
}

// 为购物车功能引导登录
async function handleLoginForCart() {
  // 检查是否有钱包提供方
  if (!walletStore.state.provider) {
    console.log('No wallet provider detected in handleLoginForCart, redirecting to Go page')
    FastDialog.warningSnackbar('No wallet detected. Redirecting to wallet setup page...')
    await nextTick()
    await new Promise(resolve => setTimeout(resolve, 500))
    window.location.replace('/go')
    return
  }

  // 检查钱包是否已连接账户
  if (!walletStore.state.active || !walletStore.state.address) {
    try {
      FastDialog.infoSnackbar('Connecting wallet...')
      const accounts = await walletStore.connect()
      if (!accounts || accounts.length === 0) {
        FastDialog.errorSnackbar('Failed to connect wallet. Please try again.')
        return
      }
    } catch (error) {
      console.error('Wallet connection error:', error)
      FastDialog.errorSnackbar('Failed to connect wallet. Please check your wallet and try again.')
      return
    }
  }

  // 检查是否已登录
  const checkSignedResult = await userStore.checkSigned()
  if (!checkSignedResult.succeed) {
    FastDialog.errorSnackbar(checkSignedResult.errorMessage as string)
    return
  }

  // 如果已登录，直接跳转到购物车页面
  if (checkSignedResult.data?.singined) {
    router.push({ name: 'Cart' })
    return
  }

  // 需要签名登录
  try {
    FastDialog.infoSnackbar('Please sign the message in your wallet to login...')
    const from = walletStore.state.address
    const provider = walletStore.state.provider
    const message = `0x${checkSignedResult.data.tokenText}`
    
    const signedText = await provider.request({
      method: 'personal_sign',
      params: [message, from]
    })

    // 执行登录
    const signResult = await userStore.signIn(signedText)
    if (!signResult.succeed) {
      FastDialog.errorSnackbar(signResult.errorMessage as string)
      return
    }

    // 获取用户信息
    if (!(await userStore.updateUserInfo())) {
      userStore.signOut()
      FastDialog.errorSnackbar('Failed to get user information. Please try again.')
      return
    }

    // 登录成功，跳转到购物车页面
    FastDialog.successSnackbar('Login successful!')
    router.push({ name: 'Cart' })
  } catch (error) {
    console.error('Login error:', error)
    if ((error as any)?.code === 4001) {
      FastDialog.warningSnackbar('Login cancelled. Please sign the message to access your cart.')
    } else {
      FastDialog.errorSnackbar('Login failed. Please try again.')
    }
  }
}

function closeOrderSuccessDialog() {
  orderSuccessDialog.value = false
  orderSuccessDetail.value = null
}

const display = useDisplay()

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

async function toStakeFreeMining() {
  if (userStore.state.userInfo?.primaryTokenStatus !== PrimaryTokenStatus.Completed) {
    FastDialog.warningSnackbar('You have not set a primary token yet, please select the primary token first.')
    return
  }
  router.push({ name: 'StakeFreeMining' })
}

const assetsManagementSheet = ref(false)
const assetsManagementMode = ref(AssetsManagementMode.Invalid)

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

async function toHistory() {
  router.push({ path: '/history', query: { recordType: RecordType.OnChainAssets } })
}

function openBitgetPay(chain: BitgetDeepLinkChain, defaultAmount?: number) {
  const config = bitgetDeepLinkConfigs[chain]
  if (!config) {
    FastDialog.errorSnackbar('Unsupported chain, please contact support.')
    return
  }

  let amountValue: number
  if (typeof defaultAmount === 'number') {
    amountValue = defaultAmount
  } else {
    const amountInput = window.prompt(
      `Enter ${config.symbol} amount to pay (Chain: ${config.chainName})`,
      '10'
    )
    if (amountInput === null) {
      FastDialog.warningSnackbar('Payment cancelled.')
      return
    }
    amountValue = Number(amountInput)
  }

  if (Number.isNaN(amountValue) || amountValue <= 0) {
    FastDialog.errorSnackbar('Invalid amount, please enter a positive number.')
    return
  }

  const params = new URLSearchParams({
    action: 'transfer',
    chain: config.chain,
    to: config.address,
    amount: amountValue.toString(),
    token: config.symbol,
    redirectUrl: window.location.href
  })

  const deeplink = `https://bkcode.vip/bitkeep?${params.toString()}`
  window.open(deeplink, '_blank', 'noopener')
  FastDialog.successSnackbar('Opening Bitget Wallet. Please complete payment in wallet.')
}

function viewProductDetail(productId: number) {
  router.push({ name: 'ProductDetail', params: { productId } })
}
</script>
