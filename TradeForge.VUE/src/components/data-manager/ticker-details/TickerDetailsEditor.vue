<template>
  <v-card class="ticker-editor" :class="{ 'is-editing': isEditing }">

    <!-- Header -->
    <v-card-title class="d-flex align-center justify-space-between pa-4">
      <div class="d-flex align-center gap-2">
        <v-icon :icon="isEditing ? 'mdi-pencil' : 'mdi-tag-outline'" size="small" />
        <span class="ticker-symbol">{{ modelValue.symbol }}</span>
        <v-chip :color="categoryColor(modelValue.category)" size="x-small" variant="tonal">
          {{ modelValue.category }}
        </v-chip>
      </div>

      <div class="d-flex align-center gap-2" v-if="!readonly">
        <v-btn
          v-if="!isEditing"
          size="small"
          variant="tonal"
          prepend-icon="mdi-pencil"
          @click="startEditing"
        >
          Edit
        </v-btn>

        <template v-else>
          <v-btn
            size="small"
            variant="text"
            prepend-icon="mdi-close"
            :disabled="isSaving"
            @click="cancel"
          >
            Cancel
          </v-btn>
          <v-btn
            size="small"
            color="primary"
            variant="flat"
            :loading="isSaving"
            :disabled="isSaving || isFormValid === false"
            :prepend-icon="isSaving ? undefined : 'mdi-content-save'"
            @click="save"
          >
            {{ isSaving ? 'Saving…' : 'Save' }}
          </v-btn>
        </template>
      </div>
    </v-card-title>

    <v-divider />

    <v-card-text class="pa-4">
      <v-form ref="formRef" v-model="isFormValid">
        <v-row dense>

          <!-- ID (always readonly) -->
          <v-col cols="12" sm="6">
            <v-text-field
              :model-value="draft.id"
              label="ID"
              density="compact"
              variant="underlined"
              readonly
              hide-details
            />
          </v-col>

          <!-- Symbol -->
          <v-col cols="12" sm="6">
            <v-text-field
              v-model="draft.symbol"
              label="Symbol"
              density="compact"
              :variant="isEditing ? 'outlined' : 'underlined'"
              :readonly="!isEditing"
              :rules="isEditing ? [ruleNotEmpty] : []"
              hide-details="auto"
            />
          </v-col>

          <!-- Instrument -->
          <v-col cols="12">
            <v-text-field
              v-model="draft.instrument"
              label="Instrument"
              density="compact"
              :variant="isEditing ? 'outlined' : 'underlined'"
              :readonly="!isEditing"
              :rules="isEditing ? [ruleNotEmpty] : []"
              hide-details="auto"
            />
          </v-col>

          <!-- Category -->
          <v-col cols="12" sm="6">
            <v-text-field
              v-model="draft.category"
              label="Category"
              density="compact"
              :variant="isEditing ? 'outlined' : 'underlined'"
              :readonly="!isEditing"
              :rules="isEditing ? [ruleNotEmpty] : []"
              hide-details="auto"
            />
          </v-col>

          <!-- Units -->
          <v-col cols="12" sm="6">
            <v-text-field
              v-model="draft.units"
              label="Units"
              density="compact"
              :variant="isEditing ? 'outlined' : 'underlined'"
              :readonly="!isEditing"
              :rules="isEditing ? [ruleNotEmpty] : []"
              hide-details="auto"
            />
          </v-col>

          <v-col cols="12"><v-divider class="my-1" /></v-col>

          <!-- Contract Size -->
          <v-col cols="12" sm="6" md="4">
            <v-text-field
              v-model.number="draft.contractSize"
              label="Contract Size"
              type="number"
              step="any"
              density="compact"
              :variant="isEditing ? 'outlined' : 'underlined'"
              :readonly="!isEditing"
              :rules="isEditing ? [rulePositive] : []"
              hide-details="auto"
            />
          </v-col>

          <!-- Leverage -->
          <v-col cols="12" sm="6" md="4">
            <v-text-field
              v-model.number="draft.leverage"
              label="Leverage"
              type="number"
              step="any"
              density="compact"
              :variant="isEditing ? 'outlined' : 'underlined'"
              :readonly="!isEditing"
              :rules="isEditing ? [rulePositive] : []"
              hide-details="auto"
            />
          </v-col>

          <!-- Min Tick -->
          <v-col cols="12" sm="6" md="4">
            <v-text-field
              v-model.number="draft.minTick"
              label="Min Tick"
              type="number"
              step="any"
              density="compact"
              :variant="isEditing ? 'outlined' : 'underlined'"
              :readonly="!isEditing"
              :rules="isEditing ? [rulePositive] : []"
              hide-details="auto"
            />
          </v-col>

          <!-- Min Volume -->
          <v-col cols="12" sm="6" md="4">
            <v-text-field
              v-model.number="draft.minVolume"
              label="Min Volume"
              type="number"
              step="any"
              density="compact"
              :variant="isEditing ? 'outlined' : 'underlined'"
              :readonly="!isEditing"
              :rules="isEditing ? [rulePositive] : []"
              hide-details="auto"
            />
          </v-col>

          <!-- Max Volume -->
          <v-col cols="12" sm="6" md="4">
            <v-text-field
              v-model.number="draft.maxVolume"
              label="Max Volume"
              type="number"
              step="any"
              density="compact"
              :variant="isEditing ? 'outlined' : 'underlined'"
              :readonly="!isEditing"
              :rules="isEditing ? [rulePositive] : []"
              hide-details="auto"
            />
          </v-col>

          <!-- Volume Step -->
          <v-col cols="12" sm="6" md="4">
            <v-text-field
              v-model.number="draft.volumeStep"
              label="Volume Step"
              type="number"
              step="any"
              density="compact"
              :variant="isEditing ? 'outlined' : 'underlined'"
              :readonly="!isEditing"
              :rules="isEditing ? [rulePositive] : []"
              hide-details="auto"
            />
          </v-col>

        </v-row>
      </v-form>
    </v-card-text>

  </v-card>
</template>

<script setup lang="ts">
import { ref, watch } from 'vue';

export interface TickerDetails {
  id: string;
  symbol: string;
  instrument: string;
  category: string;
  contractSize: number;
  units: string;
  minVolume: number;
  maxVolume: number;
  volumeStep: number;
  minTick: number;
  leverage: number;
}

interface Props {
  modelValue: TickerDetails;
  readonly?: boolean;
}

const props = withDefaults(defineProps<Props>(), { readonly: false });

const emit = defineEmits<{
  'update:modelValue': [value: TickerDetails];
  'save': [value: TickerDetails];
  'editing': [value: boolean];
}>();

// ── State ─────────────────────────────────────────────────────────────────────

const isEditing = ref(false);
const isSaving = ref(false);
const isFormValid = ref(true);
const formRef = ref<any>(null);
const draft = ref<TickerDetails>({ ...props.modelValue });

watch(() => props.modelValue, (val) => {
  if (!isEditing.value) draft.value = { ...val };
}, { deep: true });

// ── Edit lifecycle ────────────────────────────────────────────────────────────

const startEditing = () => {
  draft.value = { ...props.modelValue };
  isEditing.value = true;
  emit('editing', true);
};

const cancel = () => {
  draft.value = { ...props.modelValue };
  isEditing.value = false;
  formRef.value?.resetValidation();
  emit('editing', false);
};

const save = async () => {
  const { valid } = await formRef.value.validate();
  if (!valid) return;
  isSaving.value = true;
  emit('save', { ...draft.value });
};

const setSaving = (val: boolean) => {
  isSaving.value = val;
  if (!val) {
    isEditing.value = false;
    emit('editing', false);
  }
};

defineExpose({ cancel, setSaving });

// ── Validation ────────────────────────────────────────────────────────────────

const ruleNotEmpty = (v: any) =>
  (v !== null && v !== undefined && String(v).trim() !== '') || 'Cannot be empty';

const rulePositive = (v: any) =>
  (v !== null && v !== undefined && Number(v) > 0) || 'Must be > 0';

// ── Helpers ───────────────────────────────────────────────────────────────────

const categoryColor = (cat: string) => {
  const map: Record<string, string> = {
    'Forex': 'blue',
    'Exotic Forex': 'blue-darken-2',
    'Metals': 'amber',
    'Energy': 'orange',
    'Indices': 'purple',
    'Stocks': 'green',
    'Crypto': 'cyan',
    'Agriculture': 'lime',
    'Bonds': 'teal',
  };
  return map[cat] ?? 'grey';
};
</script>

<style scoped>
.ticker-editor {
  transition: box-shadow 0.2s;
}

.ticker-editor.is-editing {
  box-shadow: 0 0 0 0.125rem rgb(var(--v-theme-primary)) !important;
}

.ticker-symbol {
  font-weight: 700;
  font-size: 1rem;
  letter-spacing: 0.05em;
}
</style>
