<template>
  <v-toolbar density="compact" color="surface">
    <template v-for="(item, index) in toolbarItems" :key="index">
      <v-divider
        v-if="item.type === 'divider'"
        vertical
        class="mx-2"
      />
      <v-btn
        v-else
        variant="text"
        :prepend-icon="item.icon"
        @click="item.action()"
      >
        {{ item.label }}
      </v-btn>
    </template>
  </v-toolbar>
</template>

<script setup lang="ts">
import type {ActionMenuItem} from '@/types/DataManagerContextMenuItem.ts';

const toolbarItems: ActionMenuItem[] = [
  {type: 'button', key: 'create', label: 'Create', icon: 'mdi-plus', action: () => emit('create')},
  {type: 'button', key: 'edit', label: 'Edit', icon: 'mdi-pencil', action: () => emit('edit')},
  {
    type: 'button',
    key: 'delete',
    label: 'Delete',
    icon: 'mdi-delete',
    action: () => emit('delete')
  },
  {
    type: 'button',
    key: 'view-data',
    label: 'View Data',
    icon: 'mdi-eye',
    action: () => emit('viewData')
  },
  {type: 'divider', key: 'divider-1'},
  {
    type: 'button',
    key: 'import',
    label: 'Import',
    icon: 'mdi-import',
    action: () => emit('import')
  },
  {
    type: 'button',
    key: 'export',
    label: 'Export',
    icon: 'mdi-export',
    action: () => emit('export')
  },
  {type: 'divider', key: 'divider-2'},
  {
    type: 'button',
    key: 'clear-data',
    label: 'Clear Data',
    icon: 'mdi-delete-sweep',
    action: () => emit('clearData')
  }
]

const emit = defineEmits<{
  create: []
  edit: []
  delete: []
  viewData: []
  import: []
  export: []
  clearData: []
}>()
</script>


<style scoped>
</style>
