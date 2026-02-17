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
        <!-- View mode: Edit button -->
        <v-btn
          v-if="!isEditing"
          size="small"
          variant="tonal"
          prepend-icon="mdi-pencil"
          @click="startEditing"
        >
          Edit
        </v-btn>

        <!-- Edit mode: Cancel + Save -->
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
      <v-form ref="formRef">
        <v-row dense>

          <!-- ID (always readonly) -->
          <v-col cols="12" sm="6">
            <TickerField
              label="ID"
              :value="draft.id"
              readonly-always
            />
          </v-col>

          <!-- Symbol -->
          <v-col cols="12" sm="6">
            <TickerField
              v-model="draft.symbol"
              label="Symbol"
              :editing="isEditing"
              :rules="[ruleNotEmpty]"
            />
          </v-col>

          <!-- Instrument -->
          <v-col cols="12">
            <TickerField
              v-model="draft.instrument"
              label="Instrument"
              :editing="isEditing"
              :rules="[ruleNotEmpty]"
            />
          </v-col>

          <!-- Category -->
          <v-col cols="12" sm="6">
            <TickerField
              v-model="draft.category"
              label="Category"
              :editing="isEditing"
              :rules="[ruleNotEmpty]"
            />
          </v-col>

          <!-- Units -->
          <v-col cols="12" sm="6">
            <TickerField
              v-model="draft.units"
              label="Units"
              :editing="isEditing"
              :rules="[ruleNotEmpty]"
            />
          </v-col>

          <v-col cols="12"><v-divider class="my-1" /></v-col>

          <!-- Contract Size -->
          <v-col cols="12" sm="6" md="4">
            <TickerField
              v-model.number="draft.contractSize"
              label="Contract Size"
              type="number"
              :editing="isEditing"
              :rules="[rulePositive]"
            />
          </v-col>

          <!-- Leverage -->
          <v-col cols="12" sm="6" md="4">
            <TickerField
              v-model.number="draft.leverage"
              label="Leverage"
              type="number"
              :editing="isEditing"
              :rules="[rulePositive]"
            />
          </v-col>

          <!-- Min Tick -->
          <v-col cols="12" sm="6" md="4">
            <TickerField
              v-model.number="draft.minTick"
              label="Min Tick"
              type="number"
              :editing="isEditing"
              :rules="[rulePositive]"
            />
          </v-col>

          <!-- Min Volume -->
          <v-col cols="12" sm="6" md="4">
            <TickerField
              v-model.number="draft.minVolume"
              label="Min Volume"
              type="number"
              :editing="isEditing"
              :rules="[rulePositive]"
            />
          </v-col>

          <!-- Max Volume -->
          <v-col cols="12" sm="6" md="4">
            <TickerField
              v-model.number="draft.maxVolume"
              label="Max Volume"
              type="number"
              :editing="isEditing"
              :rules="[rulePositive]"
            />
          </v-col>

          <!-- Volume Step -->
          <v-col cols="12" sm="6" md="4">
            <TickerField
              v-model.number="draft.volumeStep"
              label="Volume Step"
              type="number"
              :editing="isEditing"
              :rules="[rulePositive]"
            />
          </v-col>

        </v-row>
      </v-form>
    </v-card-text>

  </v-card>
</template>

<script setup lang="ts">
import { ref, watch } from 'vue';
import type {TickerDetails} from "@/types/Ticker.ts";

// ── Types ─────────────────────────────────────────────────────────────────────

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
const formRef = ref<any>(null);
const draft = ref<TickerDetails>({ ...props.modelValue });

// Keep draft in sync when modelValue changes externally (while not editing)
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
  emit('editing', false);
};

const save = async () => {
  const { valid } = await formRef.value.validate();
  if (!valid) return;

  isSaving.value = true;
  emit('save', { ...draft.value });
};

// Called by parent to signal save is complete
const setSaving = (val: boolean) => {
  isSaving.value = val;
  if (!val) {
    isEditing.value = false;
    emit('editing', false);
  }
};

// ── Expose ────────────────────────────────────────────────────────────────────

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

<!-- ── Sub-component: TickerField ───────────────────────────────────────────── -->
<script lang="ts">
// Inline sub-component so the file stays self-contained
import { defineComponent, h, resolveComponent } from 'vue';

export const TickerField = defineComponent({
  name: 'TickerField',
  props: {
    modelValue: { type: [String, Number], default: '' },
    label: { type: String, required: true },
    editing: { type: Boolean, default: false },
    readonlyAlways: { type: Boolean, default: false },
    type: { type: String, default: 'text' },
    rules: { type: Array, default: () => [] },
  },
  emits: ['update:modelValue'],
  setup(props, { emit }) {
    return () => {
      const VTextField = resolveComponent('v-text-field') as any;

      return h(VTextField, {
        modelValue: props.modelValue,
        label: props.label,
        type: props.type,
        step: props.type === 'number' ? 'any' : undefined,
        density: 'compact',
        variant: (props.readonlyAlways || !props.editing) ? 'underlined' : 'outlined',
        readonly: props.readonlyAlways || !props.editing,
        rules: (!props.readonlyAlways && props.editing) ? props.rules : [],
        hideDetails: 'auto',
        class: (!props.readonlyAlways && !props.editing) ? 'ticker-field-view' : '',
        'onUpdate:modelValue': (v: any) => emit('update:modelValue', v),
      });
    };
  },
});
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

:deep(.ticker-field-view .v-field__input) {
  color: rgba(var(--v-theme-on-surface), 0.87);
}

:deep(.ticker-field-view .v-field__outline) {
  display: none;
}
</style>
