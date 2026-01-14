<template>
  <v-app>
    <v-app-bar v-if="!$vuetify.display.lgAndUp">
      <v-app-bar-nav-icon @click="drawer = !drawer"/>
      <v-app-bar-title>Trade Forge</v-app-bar-title>
    </v-app-bar>
    <v-navigation-drawer v-model="drawer" :permanent="$vuetify.display.lgAndUp">
      <v-list-item title="Trade Forge" subtitle="v1.0.0.0"/>
      <v-divider/>
      <v-list-item
        v-for="item in menuItems"
        :key="item.path"
        :to="item.path"
        link
        :title="item.title"
        :prepend-icon="item.icon"
      />
    </v-navigation-drawer>
    <v-main>

      <router-view/>
    </v-main>
    <v-snackbar-queue
      location="top"
      v-model="messages"
      :timeout="5000"
    />
    <v-snackbar-queue
      v-model="errors"
      :timeout="5000"
    />
  </v-app>
</template>

<script setup>
import {ref} from 'vue';

import {useSnackbar} from '@/composables/useSnackbar'
import {useErrorSnackbar} from '@/composables/useErrorSnackbar';

const drawer = ref(true)

const menuItems = [
  {path: '/', title: 'Dashboard', icon: 'mdi-view-dashboard'},
  {path: '/history/browse', title: 'History', icon: 'mdi-history'},
  {path: '/backtester', title: 'Backtest', icon: 'mdi-chart-line'}
]

const {messages} = useSnackbar()
const {errors} = useErrorSnackbar();

</script>

<style scoped>

</style>
