<template>
  <v-dialog v-model="dialog" class="delete-dialog" max-width="500" persistent>
    <v-card>
      <v-card-title class="text-h5">
        Are you sure?
      </v-card-title>

      <v-card-text>
        <div class="text-body-1 mb-2">
          Are you sure you want to delete ticker <strong>{{ currentTicker?.symbol }}</strong>?
        </div>
        <div class="text-medium-emphasis">
          This will permanently delete the ticker and all its historical data. This action cannot be
          undone.
        </div>
      </v-card-text>

      <v-card-actions>
        <v-spacer/>
        <v-btn
          variant="text"
          @click="handleCancel"
          :disabled="isLoading"
        >
          No
        </v-btn>
        <v-btn
          color="error"
          variant="flat"
          @click="handleDelete"
          :loading="isLoading"
          :disabled="isLoading"
        >
          Delete
        </v-btn>
      </v-card-actions>

      <v-progress-linear
        v-if="isLoading"
        indeterminate
        color="error"
      />
    </v-card>
  </v-dialog>
</template>
<style scoped src="./DeleteTickerModal.css"/>

<script setup lang="ts">
import {ref} from 'vue';
import type {Ticker} from '@/types/Ticker';

interface Props {

}

defineProps<Props>();

const currentTicker = ref<Ticker | null>(null);

const emit = defineEmits<{
  delete: []
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

const handleDelete = () => {
  emit('delete');
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
