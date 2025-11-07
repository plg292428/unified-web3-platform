<template>
  <v-container class="fill-height">
    <v-responsive class="align-center text-center fill-height">
      <h1 class="text-h5 font-weight-bold text-primary">Error</h1>

      <div class="text-h4 text-grey-darken-1">No wallet detected.</div>

      <div class="py-4"></div>

      <v-img height="256" src="@/assets/error_no_wallet.svg" />

      <div class="py-4"></div>

      <div class="text-body-1">
        This application relies on smart contract function, please install a
        wallet or browse this website using the wallet's built-in browser.
      </div>
    </v-responsive>
  </v-container>
</template>

<script lang="ts" setup></script>
