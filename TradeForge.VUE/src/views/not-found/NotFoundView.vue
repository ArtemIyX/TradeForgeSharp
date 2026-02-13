<script setup lang="ts">

</script>

<template>
  <v-container class="fill-height">
    <v-row align="center" justify="center">
      <v-col cols="12" md="6" class="text-center">
        <h1 class="text-h2 mb-4">404</h1>
        <p class="text-h6 mb-4">Page not found</p>
        <v-btn color="primary" :to="{ name: 'home' }">
          Go Home
        </v-btn>
      </v-col>
    </v-row>
  </v-container>
</template>

<style scoped>

</style>
