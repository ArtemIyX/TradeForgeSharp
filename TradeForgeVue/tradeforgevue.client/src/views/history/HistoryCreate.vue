<template>
  <v-container>
    <v-form ref="form" v-model="valid" @submit.prevent="submit">
      <v-row>
        <!-- left: generic fields -->
        <v-col cols="12" md="6">
          <v-text-field
            v-model="symbol.ticker"
            label="Ticker *"
            :rules="[required]"
          />

          <v-select
            v-model="symbol.type"
            label="Type *"
            :items="typeItems"
            :rules="[required]"
          />

          <v-select
            v-model="symbol.category"
            label="Category *"
            :items="categoryItems"
            :rules="[required]"
          />

          <v-textarea
            v-model="symbol.description"
            label="Description"
            rows="3"
            placeholder="Some description of your asset"
          />
        </v-col>

        <!-- right: contract specs -->
        <v-col cols="12" md="6">
          <v-text-field
            v-model.number="symbol.contractSize"
            label="Contract size *"
            type="number"
            step="1"
            :rules="[required, positive]"
          />

          <v-text-field
            v-model="symbol.units"
            label="Units *"
            :rules="[required]"
          />

          <v-text-field
            v-model.number="symbol.volumeStep"
            label="Volume step *"
            type="number"
            step="0.01"
            :rules="[required, positive]"
          />

          <v-text-field
            v-model.number="symbol.defaultLeverage"
            label="Default leverage *"
            type="number"
            step="0.01"
            :rules="[required, positive]"
          />

          <v-text-field
            v-model.number="symbol.minVolume"
            label="Min volume *"
            type="number"
            step="0.01"
            :rules="[required, positive]"
          />

          <v-text-field
            v-model.number="symbol.maxVolume"
            label="Max volume *"
            type="number"
            :rules="[required, gtMin]"
          />

          <v-text-field
            v-model.number="symbol.minTick"
            label="Min tick *"
            type="number"
            step="0.00001"
            :rules="[required, positive]"
          />
        </v-col>
      </v-row>

      <v-row>
        <v-col class="d-flex justify-end">
          <v-btn color="primary" type="submit" :disabled="!valid">Save</v-btn>
          <v-btn class="ml-2" @click="$router.back()">Cancel</v-btn>
        </v-col>
      </v-row>
    </v-form>
  </v-container>
</template>

<script>


export default {
  name: "CreateSymbol",
  components: {},
  data: () => ({
    valid: false,
    symbol: {
      ticker: '',
      type: 'CFD',
      category: 'Stock',
      description: '',

      contractSize: 1,
      units: 'Share(s)',
      volumeStep: 0.01,
      defaultLeverage: 0.05,
      minVolume: 0.01,
      maxVolume: 100000000,
      minTick: 0.00001
    },

    typeItems: ['CFD', 'Spot', 'Future', 'Option', 'ETF'],
    categoryItems: ['Stock', 'Forex', 'Crypto', 'Equity', 'Index', 'Commodity']
  }),

  methods: {
    required: v => !!v || 'Required',
    positive: v => v > 0 || 'Must be positive',
    gtMin(v) {
      return !this.symbol || v > this.symbol.minVolume || 'Must be > min volume'
    },

    async submit() {
      if (!this.$refs.form.validate()) return

      // TODO: POST to your API or store however you like
      console.log('New symbol payload:', this.symbol)

      // example: post to json-server
      // await this.$http.post('/symbols', this.symbol)


    }
  }
}
</script>

<style scoped>

</style>
