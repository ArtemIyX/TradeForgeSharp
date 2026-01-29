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

        @click="save"
        :disabled="!editor?.isValid()"
        :loading="isSaving"
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
import {useRouter} from 'vue-router';
import {useSnackbar} from "@/composables/useSnackbar.js";
const router = useRouter();

const snackbar = useSnackbar();

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

const editor = ref(null);
const isSaving = ref(false);

const save = async () => {
  isSaving.value = true;
  try {
    await new Promise(resolve => setTimeout(resolve, 500))
    router.push({
      name: 'history-browse'
    });

  }
  catch (e) {

  }
  finally {
    isSaving.value = false;
  }

}
</script>

<style scoped>

</style>
