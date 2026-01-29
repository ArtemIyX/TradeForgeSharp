<template>
  <v-card>

    <div class="d-flex justify-center my-4" v-if="loading">
      <v-progress-circular
        indeterminate
      />
    </div>
    <div v-else>
      <v-card-title>
        <BackButton/>
        <span class="text-accent">Details of    </span>
        <v-chip>
          <span class="text-subtitle-2 text-medium-emphasis">{{ symbol.ticker }}</span>
        </v-chip>

      </v-card-title>
      <SymbolEditor :model="symbol" ref="editor"/>

      <v-card-actions class="pa-4">
        <v-spacer></v-spacer>
        <v-btn

          @click="save()"
          :disabled="!editor?.isValid()"
          :loading="isSaving"
        >
          Save
        </v-btn>
        <v-btn @click="$router.back()">Cancel</v-btn>
      </v-card-actions>
    </div>
  </v-card>
</template>

<script setup>

import {ref, onMounted, computed} from "vue";
import {useRoute} from 'vue-router';
import SymbolEditor from "@/components/SymbolEditor.vue";
import BackButton from "@/components/BackButton.vue";

import {useSnackbar} from "@/composables/useSnackbar.js";

const snackbar = useSnackbar();
const route = useRoute();
const editor = ref({});
const isSaving = ref(false);

const symbol = ref({
  id: 999
});

const loading = ref(false)

onMounted(() => {
  symbol.id = route.params.id;
  fetchData();
})

const fetchData = async () => {
  loading.value = true
  try {
    const res = await fetch('/dummy/symbol.json')
    await new Promise(resolve => setTimeout(resolve, 150))
    symbol.value = await res.json()
  } catch (e) {
    console.error('Failed to load local JSON:', e)
    symbol.value = {}
  } finally {
    loading.value = false
  }
}

const save = async () => {
  isSaving.value = true;
  try {
    await new Promise(resolve => setTimeout(resolve, 500))
    snackbar.showInfo("Successfully Saved!");
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
