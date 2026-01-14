<template>
  <v-container>
    <v-form ref="form" v-model="valid" eager>
      <v-row>
        <!-- left: generic fields -->
        <v-col cols="12" md="6">
          <v-text-field
            v-model="model.ticker"
            label="Ticker *"
            density="compact"
            :rules="[rl.required]"
            @blur="form?.validate()"
          />

          <v-select
            v-model="model.type"
            label="Type *"
            density="compact"
            :items="typeItems"
            :rules="[rl.required]"
            @blur="form?.validate()"
          />

          <v-select
            v-model="model.category"
            label="Category *"
            density="compact"
            :items="categoryItems"
            :rules="[rl.required]"
            @blur="form?.validate()"
          />

          <v-textarea
            v-model="rl.description"
            label="Description"
            density="compact"
            rows="3"
            auto-grow
            placeholder="Some description of your asset"
          />
        </v-col>

        <!-- right: contract specs -->
        <v-col cols="12" md="6">
          <v-text-field
            density="compact"
            v-model.number="model.contractSize"
            label="Contract size *"
            type="number"
            step="1"
            :rules="[rl.required, rl.positive]"
            @blur="form?.validate()"
          />

          <v-text-field
            v-model="model.units"
            density="compact"
            label="Units *"
            :rules="[rl.required]"
            @blur="form?.validate()"
          />

          <v-text-field
            v-model.number="model.volumeStep"
            label="Volume step *"
            density="compact"
            type="number"
            step="0.01"
            :rules="[rl.required, rl.positive]"
            @blur="form?.validate()"
          />

          <v-text-field
            v-model.number="model.defaultLeverage"
            label="Default leverage *"
            density="compact"
            type="number"
            step="0.01"
            :rules="[rl.required, rl.positive]"
            @blur="form?.validate()"
          />

          <v-text-field
            v-model.number="model.minVolume"
            label="Min volume *"
            density="compact"
            type="number"
            step="0.01"
            :rules="[rl.required, rl.positive]"
            @blur="form?.validate()"
          />

          <v-text-field
            v-model.number="model.maxVolume"
            label="Max volume *"
            density="compact"
            type="number"
            step="0.01"
            :rules="[rl.required, gtMinRule]"
            @blur="form?.validate()"
          />

          <v-text-field
            v-model.number="model.minTick"
            label="Min tick *"
            density="compact"
            type="number"
            step="0.00001"
            :rules="[rl.required, rl.positive]"
            @blur="form?.validate()"
          />
        </v-col>
      </v-row>
    </v-form>
  </v-container>
</template>

<script setup>

import {required, positive} from '@/utils/rules'

import {
  defineProps,
  defineExpose,
  defineEmits,
  computed,
  ref,
  onMounted,
  reactive,
  readonly,
  watch
} from "vue";

const props = defineProps({
  model: Object,
});

const valid = ref(false)
const form = ref(null)

defineExpose({
  isValid: () => valid.value
})

const TYPE_ITEMS = Object.freeze(['CFD', 'Spot', 'Future', 'Option', 'ETF'])
const CATEGORY_ITEMS = Object.freeze(['Stock', 'Forex', 'Crypto', 'Equity', 'Index', 'Commodity'])

const typeItems = TYPE_ITEMS;
const categoryItems = CATEGORY_ITEMS;
const rl = {required, positive};

const gtMinRule = computed(() => {
  const min = Number(props.model.minVolume)
  return v => Number(v) >= min || 'Must be ≥ Min volume'
})


</script>

