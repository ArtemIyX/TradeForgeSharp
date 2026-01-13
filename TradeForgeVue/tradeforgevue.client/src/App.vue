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

  </v-app>
</template>

<script setup>
const menuItems = [
  {title: 'Dashboard', path: '/'},
  {title: 'History Manager', path: '/history/browse'},
  {title: 'Backtester', path: '/backtester'}
]
</script>

<style scoped>

</style>
