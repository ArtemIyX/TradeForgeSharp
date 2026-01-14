<template>

  <v-container>
    <h1>Create new history item</h1>
    <SymbolEditor :model="symbol"
                  ref="editor"


    />

    <v-row>
      <v-col class="d-flex justify-end">
        <v-btn color="primary" type="submit" @click="submit"
               :disabled="!editor?.isValid()">Save
        </v-btn>
        <v-btn class="ml-2" @click="$router.back()">Cancel</v-btn>
      </v-col>
    </v-row>

  </v-container>
</template>

<script setup>
import {ref, reactive, computed, onMounted} from 'vue'
import SymbolEditor from '@/components/SymbolEditor.vue'


const symbol = reactive({
  ticker: 'gg',
  type: 'CFD',
  category: 'Stock',
  description: '',

  contractSize: 1,
  units: 'Share(s)',
  volumeStep: 0.01,
  defaultLeverage: 0.05,
  minVolume: 0.01,
  maxVolume: 100_000_000,
  minTick: 0.00001
});

const editor = ref(null)

/* ---- methods ---- */
async function submit() {


  console.log('Our object:', symbol)
  console.log(editor.value);

}
</script>

<style scoped>

</style>
