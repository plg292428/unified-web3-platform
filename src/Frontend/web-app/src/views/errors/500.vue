<template>
  <v-container class="fill-height">
    <v-responsive class="align-center text-center fill-height">
      <h1 class="text-h5 font-weight-bold text-primary">Error</h1>

      <div class="text-h4 text-grey-darken-1">Internal Server Error.</div>

      <div class="py-4"></div>

      <v-img height="256" src="@/assets/error_500.svg" />

      <div class="py-4"></div>

      <div class="text-body-1 mb-10">Unable to connect to the server, please try again later.</div>
    </v-responsive>
  </v-container>
</template>

<script lang="ts" setup></script>
