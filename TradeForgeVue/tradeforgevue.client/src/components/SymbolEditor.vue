<template>
  <v-container>
    <v-form ref="form" v-model="valid">
      <v-row>
        <!-- left: generic fields -->
        <v-col cols="12" md="6">
          <v-text-field
            v-model="local.ticker"
            label="Ticker *"
            :rules="[rules.required]"
          />

          <v-select
            v-model="local.type"
            label="Type *"
            :items="typeItems"
            :rules="[rules.required]"
          />

          <v-select
            v-model="local.category"
            label="Category *"
            :items="categoryItems"
            :rules="[rules.required]"
          />

          <v-textarea
            v-model="local.description"
            label="Description"
            rows="3"
            auto-grow
            placeholder="Some description of your asset"
          />
        </v-col>

        <!-- right: contract specs -->
        <v-col cols="12" md="6">
          <v-text-field
            v-model.number="local.contractSize"
            label="Contract size *"
            type="number"
            step="1"
            :rules="[rules.required, rules.positive]"
          />

          <v-text-field
            v-model="local.units"
            label="Units *"
            :rules="[rules.required]"
          />

          <v-text-field
            v-model.number="local.volumeStep"
            label="Volume step *"
            type="number"
            step="0.01"
            :rules="[rules.required, rules.positive]"
          />

          <v-text-field
            v-model.number="local.defaultLeverage"
            label="Default leverage *"
            type="number"
            step="0.01"
            :rules="[rules.required, rules.positive]"
          />

          <v-text-field
            v-model.number="local.minVolume"
            label="Min volume *"
            type="number"
            step="0.01"
            :rules="[rules.required, rules.positive]"
          />

          <v-text-field
            v-model.number="local.maxVolume"
            label="Max volume *"
            type="number"
            step="0.01"
            :rules="[rules.required, gtMinRule]"
          />

          <v-text-field
            v-model.number="local.minTick"
            label="Min tick *"
            type="number"
            step="0.00001"
            :rules="[rules.required, rules.positive]"
          />
        </v-col>
      </v-row>
    </v-form>
  </v-container>
</template>

<script>
import { required, positive } from '@/utils/rules'

const TYPE_ITEMS = Object.freeze(['CFD', 'Spot', 'Future', 'Option', 'ETF'])
const CATEGORY_ITEMS = Object.freeze(['Stock', 'Forex', 'Crypto', 'Equity', 'Index', 'Commodity'])

export default {
  name: 'SymbolEditor',
  props: {
    value: { type: Object, required: true }
  },
  data() {
    return {
      valid: false,
      local: { ...this.value },
      typeItems: TYPE_ITEMS,
      categoryItems: CATEGORY_ITEMS,
      rules: { required, positive }
    }
  },
  computed: {
    gtMinRule() {
      const min = Number(this.local.minVolume)
      return v => Number(v) >= min || 'Must be ≥ Min volume'
    }
  },
  watch: {
    value: {
      handler(newVal) {
        this.local = { ...newVal }
      },
      deep: true
    },
    local: {
      handler(newVal) {
        this.$emit('input', newVal)
      },
      deep: true
    }
  }
}
</script>

