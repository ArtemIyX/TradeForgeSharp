<template>
  <v-navigation-drawer
    permanent
    :width="240"
    class="app-navigation"
  >
    <!-- App Header -->
    <div class="app-header">
      <div class="app-logo">
        <v-icon size="large" color="primary">mdi-chart-line</v-icon>
      </div>
      <div class="app-info">
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
import { ref } from 'vue';

interface NavigationItem {
  title: string;
  icon: string;
  path: string;
}

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
]);
</script>

<style scoped>
.app-navigation {
  border-right: thin solid rgba(var(--v-border-color), var(--v-border-opacity));
}

.app-header {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  padding: 1.5rem 1rem;
}

.app-logo {
  display: flex;
  align-items: center;
  justify-content: center;
}

.app-info {
  display: flex;
  flex-direction: column;
  gap: 0.125rem;
}

.app-name {
  font-size: 1.25rem;
  font-weight: 600;
  line-height: 1.2;
  margin: 0;
}

.app-version {
  font-size: 0.75rem;
  opacity: 0.6;
  font-weight: 500;
}

.nav-active {
  background-color: rgba(var(--v-theme-primary), 0.12);
}
</style>
