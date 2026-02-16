<template>
  <v-snackbar
    v-model="snackbarStore.visible"
    :color="snackbarStore.current?.color"
    :timeout="snackbarStore.current?.timeout"
    location="top right"
    @update:model-value="onUpdate"
  >
    {{ snackbarStore.current?.message }}

    <template v-slot:actions>
      <v-btn
        variant="text"
        icon="mdi-close"
        @click="snackbarStore.onClose()"
      />
    </template>
  </v-snackbar>
</template>

<script setup lang="ts">
import {useSnackbarStore} from '@/stores/Snackbar.store';

const snackbarStore = useSnackbarStore();

const onUpdate = (value: boolean) => {
  if (!value) {
    snackbarStore.onClose();
  }
};
</script>
