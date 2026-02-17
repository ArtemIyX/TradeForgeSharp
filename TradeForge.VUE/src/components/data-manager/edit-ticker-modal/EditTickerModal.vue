<template>
  <v-dialog v-model="dialog" max-width="640" scrollable>
    <v-card>
      <v-card-title class="d-flex align-center justify-space-between">
        <span>Ticker Details</span>
        <v-btn icon="mdi-close" variant="text" :disabled="isEditing" @click="dialog = false"/>
      </v-card-title>

      <v-divider/>

      <v-card-text class="pa-4">
        <!-- Loading -->
        <div v-if="loading" class="d-flex justify-center align-center" style="height: 16rem">
          <v-progress-circular indeterminate/>
        </div>

        <!-- Editor -->
        <TickerDetailsEditor
          v-else-if="ticker"
          ref="editorRef"
          v-model="ticker"
          @save="onSave"
          @editing="isEditing = $event"
        />
      </v-card-text>
    </v-card>
  </v-dialog>
</template>

<script setup lang="ts">
import {ref} from 'vue';
import TickerDetailsEditor from "@/components/data-manager/ticker-details/TickerDetailsEditor.vue";
import {type TickerDetails} from "@/types/Ticker.ts";

const dialog = ref(false);
const loading = ref(false);
const isEditing = ref(false);
const ticker = ref<TickerDetails | null>(null);
const editorRef = ref<InstanceType<typeof TickerDetailsEditor> | null>(null);

// ── Fake fetch ────────────────────────────────────────────────────────────────

const fakeFetch = async (id: string): Promise<TickerDetails> => {
  await new Promise(r => setTimeout(r, 600));

  return {
    id: id,
    symbol: `TKR${id}`,
    instrument: `Instrument #${id}`,
    category: 'Forex',
    contractSize: 100000,
    units: 'Lots',
    minVolume: 0.01,
    maxVolume: 100,
    volumeStep: 0.01,
    minTick: 0.00001,
    leverage: 0.25,
  };
};

// ── Public API ────────────────────────────────────────────────────────────────

const open = async (id: string) => {
  ticker.value = null;
  isEditing.value = false;
  dialog.value = true;
  loading.value = true;

  try {
    ticker.value = await fakeFetch(id);
  } finally {
    loading.value = false;
  }
};

// ── Save ──────────────────────────────────────────────────────────────────────

const emit = defineEmits<{
  saved: [value: TickerDetails];
}>();

const onSave = async (data: TickerDetails) => {
  editorRef.value?.setSaving(true);

  try {
    // TODO: replace with real API call
    await new Promise(r => setTimeout(r, 800));
    ticker.value = data;
    emit('saved', data);
    editorRef.value?.setSaving(false);
  } catch {
    editorRef.value?.setSaving(false);
  }
};

defineExpose({open});
</script>
