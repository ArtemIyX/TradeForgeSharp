<template>

  <v-card>
    <v-card-title class="text-h5 pb-2 flex-shrink-0">
      <back-button/>
      <span class="text-accent">Create ticker</span>
<!--      <span class="text-subtitle-2 text-medium-emphasis">(New)</span>-->
    </v-card-title>

    <v-divider class="flex-shrink-0"></v-divider>

    <SymbolEditor :model="symbol" ref="editor" />
    <v-card-actions class="pa-4">
      <v-spacer></v-spacer>
      <v-btn

        @click="submit"
        :disabled="!editor?.isValid()"
      >
        Save
      </v-btn>
      <v-btn @click="$router.back()">Cancel</v-btn>
    </v-card-actions>

  </v-card>

</template>

<script setup>
import {ref, reactive, computed, onMounted} from 'vue'
import SymbolEditor from '@/components/SymbolEditor.vue'
import BackButton from "@/components/BackButton.vue";


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
