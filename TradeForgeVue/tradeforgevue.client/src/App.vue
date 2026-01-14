<template>
  <v-app>
    <v-navigation-drawer permanent>
      <v-list-item title="Trade Forge" subtitle="v1.0.0.0"/>
      <v-divider/>
      <v-list-item v-for="item in menuItems" :key="item.path" :to="item.path" link
                   :title="item.title"/>
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
import {useSnackbar} from '@/composables/useSnackbar'
import {useErrorSnackbar} from '@/composables/useErrorSnackbar';


const menuItems = [
  {title: 'Dashboard', path: '/'},
  {title: 'History Manager', path: '/history/browse'},
  {title: 'Backtester', path: '/backtester'}
]

const {messages} = useSnackbar()
const {errors} = useErrorSnackbar();

</script>

<style scoped>

</style>
