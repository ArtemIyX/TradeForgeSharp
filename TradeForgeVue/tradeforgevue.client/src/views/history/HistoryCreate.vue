<template>

  <v-container>
    <h1>Create new history item</h1>
    <SymbolEditor :value="symbol"/>

    <v-row>
      <v-col class="d-flex justify-end">
        <v-btn color="primary" type="submit" :disabled="!valid">Save</v-btn>
        <v-btn class="ml-2" @click="$router.back()">Cancel</v-btn>
      </v-col>
    </v-row>

  </v-container>
</template>

<script>


import SymbolEditor from "@/components/SymbolEditor.vue";

export default {
  name: "CreateSymbol",
  components: {SymbolEditor},
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


  }),

  methods: {

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
