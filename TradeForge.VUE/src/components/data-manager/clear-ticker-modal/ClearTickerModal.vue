<template>
  <v-dialog v-model="dialog" class="clear-data-dialog" max-width="500" persistent>
    <v-card>
      <v-card-title class="text-h5">
        Clear Historical Data?
      </v-card-title>

      <v-card-text>
        <div class="text-body-1 mb-2">
          Are you sure you want to clear all historical data for <strong>{{ currentTicker?.symbol }}</strong>?
        </div>
        <div class="text-medium-emphasis">
          This will permanently delete all historical price data for this ticker. The ticker configuration will remain intact. This action cannot be undone.
        </div>
      </v-card-text>

      <v-card-actions>
        <v-spacer/>
        <v-btn
          variant="text"
          @click="handleCancel"
          :disabled="isLoading"
        >
          Cancel
        </v-btn>
        <v-btn
          color="warning"
          variant="flat"
          @click="handleClear"
          :loading="isLoading"
          :disabled="isLoading"
        >
          Clear Data
        </v-btn>
      </v-card-actions>

      <v-progress-linear
        v-if="isLoading"
        indeterminate
        color="warning"
      />
    </v-card>
  </v-dialog>
</template>

<style scoped>
.clear-data-dialog {
  z-index: 9999;
}
</style>

<script setup lang="ts">
import {ref} from 'vue';
import type {Ticker} from '@/types/Ticker';

interface Props {

}

defineProps<Props>();

const currentTicker = ref<Ticker | null>(null);

const emit = defineEmits<{
  clear: []
  cancel: []
}>();

const dialog = ref(false);
const isLoading = ref(false);

const show = (ticker: Ticker) => {
  currentTicker.value = ticker;
  dialog.value = true;
  isLoading.value = false;
};

const hide = () => {
  dialog.value = false;
};

const startLoading = () => {
  isLoading.value = true;
};

const stopLoading = () => {
  isLoading.value = false;
};

const handleClear = () => {
  emit('clear');
};

const handleCancel = () => {
  emit('cancel');
  hide();
};

defineExpose({
  show,
  hide,
  startLoading,
  stopLoading
});
</script>
