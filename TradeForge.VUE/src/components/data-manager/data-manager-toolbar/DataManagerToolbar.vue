<script setup lang="ts">
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

<template>
  <v-toolbar density="compact" color="surface">
    <v-btn
      variant="text"
      prepend-icon="mdi-plus"
      @click="emit('create')"
    >
      Create
    </v-btn>

    <v-btn
      variant="text"
      prepend-icon="mdi-pencil"
      @click="emit('edit')"
    >
      Edit
    </v-btn>

    <v-btn
      variant="text"
      prepend-icon="mdi-delete"
      @click="emit('delete')"
    >
      Delete
    </v-btn>

    <v-btn
      variant="text"
      prepend-icon="mdi-eye"
      @click="emit('viewData')"
    >
      View Data
    </v-btn>

    <v-divider vertical class="mx-2" />

    <v-btn
      variant="text"
      prepend-icon="mdi-import"
      @click="emit('import')"
    >
      Import
    </v-btn>

    <v-btn
      variant="text"
      prepend-icon="mdi-export"
      @click="emit('export')"
    >
      Export
    </v-btn>

    <v-divider vertical class="mx-2" />

    <v-btn
      variant="text"
      prepend-icon="mdi-delete-sweep"
      color="error"
      @click="emit('clearData')"
    >
      Clear Data
    </v-btn>
  </v-toolbar>
</template>

<style scoped>
</style>
