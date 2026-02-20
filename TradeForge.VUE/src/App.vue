<template>
  <v-app>
    <app-snack-bar/>
    <AppNavigation/>

    <v-main>
      <v-container fluid style="height: 100%; padding: 0;">
        <router-view/>
      </v-container>
    </v-main>

    <AppFooter />
  </v-app>
</template>

<script setup lang="ts">
import AppNavigation from "@/components/app/app-navigation/AppNavigation.vue";
import AppSnackBar from "@/components/app/app-snackbar/AppSnackBar.vue";
import AppFooter from "@/components/app/app-footer/AppFooter.vue";

</script>


<style scoped></style>
