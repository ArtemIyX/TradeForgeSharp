

<template>
  <div class="data-manager">


    <div class="content-container">
      <DataManagerToolbar
        @create="handleCreate"
        @edit="handleEdit"
        @delete="handleDelete"
        @view-data="handleViewData"
        @import="handleImport"
        @export="handleExport"
        @clear-data="handleClearData"
      />

      <div class="table-placeholder">
        Table will be here
      </div>
    </div>
  </div>
</template>

<style scoped>
.data-manager {
  height: 100%;
  display: flex;
  flex-direction: column;
}

.content-container {
  display: flex;
  flex-direction: column;
  flex: 1;
  border: 0.0625rem solid rgba(var(--v-border-color), var(--v-border-opacity));
  border-radius: 0.25rem;
  overflow: hidden;
}

.table-placeholder {
  flex: 1;
  background-color: rgb(var(--v-theme-surface-variant));
  border-top: 0.0625rem solid rgba(var(--v-border-color), var(--v-border-opacity));
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 1.25rem;
  color: rgba(var(--v-theme-on-surface), 0.6);
}
</style>

<script setup lang="ts">
import DataManagerToolbar
  from "@/components/data-manager/data-manager-toolbar/DataManagerToolbar.vue";


const handleCreate = () => console.log('Create')
const handleEdit = () => console.log('Edit')
const handleDelete = () => console.log('Delete')
const handleViewData = () => console.log('View Data')
const handleImport = () => console.log('Import')
const handleExport = () => console.log('Export')
const handleClearData = () => console.log('Clear Data')
</script>
