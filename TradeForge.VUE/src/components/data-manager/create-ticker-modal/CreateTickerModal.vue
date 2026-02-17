<template>
  <v-dialog v-model="dialog" max-width="640" scrollable persistent>
    <v-card>
      <v-card-title class="d-flex align-center justify-space-between">
        <span>New Instrument</span>
        <v-btn icon="mdi-close" variant="text" :disabled="isSaving" @click="close"/>
      </v-card-title>

      <v-divider/>

      <v-card-text class="pa-4">
        <v-form ref="formRef" v-model="isFormValid">
          <v-row dense>

            <!-- Symbol -->
            <v-col cols="12" sm="6">
              <v-text-field
                v-model="form.symbol"
                label="Symbol"
                density="compact"
                variant="outlined"
                :rules="[ruleNotEmpty]"
                hide-details="auto"
              />
            </v-col>

            <!-- Instrument -->
            <v-col cols="12" sm="6">
              <v-text-field
                v-model="form.instrument"
                label="Instrument"
                density="compact"
                variant="outlined"
                :rules="[ruleNotEmpty]"
                hide-details="auto"
              />
            </v-col>

            <!-- Category -->
            <v-col cols="12" sm="6">
              <v-text-field
                v-model="form.category"
                label="Category"
                density="compact"
                variant="outlined"
                :rules="[ruleNotEmpty]"
                hide-details="auto"
              />
            </v-col>

            <!-- Units -->
            <v-col cols="12" sm="6">
              <v-text-field
                v-model="form.units"
                label="Units"
                density="compact"
                variant="outlined"
                :rules="[ruleNotEmpty]"
                hide-details="auto"
              />
            </v-col>

            <v-col cols="12">
              <v-divider class="my-1"/>
            </v-col>

            <!-- Contract Size -->
            <v-col cols="12" sm="6" md="4">
              <v-text-field
                v-model.number="form.contractSize"
                label="Contract Size"
                type="number"
                step="any"
                density="compact"
                variant="outlined"
                :rules="[rulePositive]"
                hide-details="auto"
              />
            </v-col>

            <!-- Leverage -->
            <v-col cols="12" sm="6" md="4">
              <v-text-field
                v-model.number="form.leverage"
                label="Leverage"
                type="number"
                step="any"
                density="compact"
                variant="outlined"
                :rules="[rulePositive]"
                hide-details="auto"
              />
            </v-col>

            <!-- Min Tick -->
            <v-col cols="12" sm="6" md="4">
              <v-text-field
                v-model.number="form.minTick"
                label="Min Tick"
                type="number"
                step="any"
                density="compact"
                variant="outlined"
                :rules="[rulePositive]"
                hide-details="auto"
              />
            </v-col>

            <!-- Min Volume -->
            <v-col cols="12" sm="6" md="4">
              <v-text-field
                v-model.number="form.minVolume"
                label="Min Volume"
                type="number"
                step="any"
                density="compact"
                variant="outlined"
                :rules="[rulePositive]"
                hide-details="auto"
              />
            </v-col>

            <!-- Max Volume -->
            <v-col cols="12" sm="6" md="4">
              <v-text-field
                v-model.number="form.maxVolume"
                label="Max Volume"
                type="number"
                step="any"
                density="compact"
                variant="outlined"
                :rules="[rulePositive]"
                hide-details="auto"
              />
            </v-col>

            <!-- Volume Step -->
            <v-col cols="12" sm="6" md="4">
              <v-text-field
                v-model.number="form.volumeStep"
                label="Volume Step"
                type="number"
                step="any"
                density="compact"
                variant="outlined"
                :rules="[rulePositive]"
                hide-details="auto"
              />
            </v-col>

          </v-row>
        </v-form>
      </v-card-text>

      <v-divider/>

      <v-card-actions>
        <v-spacer/>
        <v-btn variant="text" :disabled="isSaving" @click="close">Cancel</v-btn>
        <v-btn
          color="primary"
          variant="flat"
          prepend-icon="mdi-plus"
          :loading="isSaving"
          :disabled="isSaving || isFormValid === false"
          @click="submit"
        >
          {{ isSaving ? 'Creating…' : 'Create' }}
        </v-btn>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>

<script setup lang="ts">
import {ref} from 'vue';
import {type TickerDetails} from '@/types/Ticker.ts';

type CreateTickerForm = Omit<TickerDetails, 'id'>;

const emptyForm = (): CreateTickerForm => ({
  symbol: '',
  instrument: '',
  category: '',
  units: '',
  contractSize: null as any,
  leverage: null as any,
  minTick: null as any,
  minVolume: null as any,
  maxVolume: null as any,
  volumeStep: null as any,
});

const dialog = ref(false);
const isSaving = ref(false);
const isFormValid = ref<boolean | null>(null);
const formRef = ref<any>(null);
const form = ref<CreateTickerForm>(emptyForm());

const emit = defineEmits<{
  created: [value: TickerDetails];
}>();

// ── Public API ────────────────────────────────────────────────────────────────

const open = () => {
  form.value = emptyForm();
  isFormValid.value = null;
  dialog.value = true;
};

const close = () => {
  dialog.value = false;
  formRef.value?.resetValidation();
};

// ── Submit ────────────────────────────────────────────────────────────────────

const submit = async () => {
  const {valid} = await formRef.value.validate();
  if (!valid) return;

  isSaving.value = true;

  try {
    // TODO: replace with real API call
    await new Promise(r => setTimeout(r, 800));

    const created: TickerDetails = {
      id: "0",
      ...form.value,
    };

    emit('created', created);
    close();
  } finally {
    isSaving.value = false;
  }
};

defineExpose({open});

// ── Validation ────────────────────────────────────────────────────────────────

const ruleNotEmpty = (v: any) =>
  (v !== null && v !== undefined && String(v).trim() !== '') || 'Cannot be empty';

const rulePositive = (v: any) =>
  (v !== null && v !== undefined && Number(v) > 0) || 'Must be > 0';
</script>
