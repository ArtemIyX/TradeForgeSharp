
<template>
  <v-app>
    <AppNavigation />

    <v-main >
      <v-container fluid style="height: 100%; padding: 0;">
        <router-view />
      </v-container>
    </v-main>
  </v-app>
</template>

<script setup lang="ts">
import AppNavigation from "@/components/app-navigation/AppNavigation.vue";
</script>


<style scoped></style>
