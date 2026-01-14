<template>
  <div class="pa-0">
    <!-- Top Bar with Title and Back Button -->
    <v-card flat class="mb-4">
      <v-card-text class="pa-4">
        <v-row align="center" dense>
          <!-- Title Text -->
          <v-col cols="12" sm="8" md="9">
            <h2 class="text-h5 text-md-h4 font-weight-medium">
              Create new history item
            </h2>
          </v-col>

          <!-- Back Button -->
          <v-col cols="12" sm="4" md="3">
            <v-btn
              to="/history/browse"
              size="large"
              block
              prepend-icon="mdi-arrow-left"
              variant="tonal"
            >
              Back
            </v-btn>
          </v-col>
        </v-row>
      </v-card-text>
    </v-card>

    <v-card>
      <v-card-text>
        <SymbolEditor :model="symbol" ref="editor" />
      </v-card-text>
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
  </div>
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
