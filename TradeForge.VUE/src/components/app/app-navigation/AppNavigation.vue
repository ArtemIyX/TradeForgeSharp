<template>
  <v-navigation-drawer
    permanent
    :rail="isMobile"
    :width="240"
    class="app-navigation"
  >
    <!-- App Header -->
    <div class="app-header">
      <div class="app-logo">
        <v-icon size="large" color="primary">mdi-chart-line</v-icon>
      </div>
      <div v-if="!isMobile" class="app-info">
        <h2 class="app-name">TradeForge</h2>
        <span class="app-version">v{{ appVersion }}</span>
      </div>
    </div>

    <v-divider class="my-4"></v-divider>

    <!-- Navigation Items -->
    <v-list nav density="comfortable">
      <v-list-item
        v-for="item in navigationItems"
        :key="item.path"
        :to="item.path"
        :prepend-icon="item.icon"
        :title="item.title"
        active-class="nav-active"
      ></v-list-item>
    </v-list>
  </v-navigation-drawer>
</template>

<script setup lang="ts">
import {ref, computed} from 'vue';
import {useDisplay} from 'vuetify';

interface NavigationItem {
  title: string;
  icon: string;
  path: string;
}

const {mobile} = useDisplay();
const isMobile = computed(() => mobile.value);

const appVersion = ref('26.1.1');

const navigationItems = ref<NavigationItem[]>([
  {
    title: 'Home',
    icon: 'mdi-home',
    path: '/',
  },
  {
    title: 'Data Manager',
    icon: 'mdi-database',
    path: '/data-manager',
  },
  {
    title: 'Back Test',
    icon: 'mdi-chart-timeline-variant',
    path: '/backtest',
  },
]);
</script>

<style scoped src="./AppNavigation.css">

</style>
