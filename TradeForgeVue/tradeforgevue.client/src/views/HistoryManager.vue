<template>
  <router-view class="ma-2"/>

  <v-footer app class="compact-footer">
    <v-container fluid>
      <v-row align="center" justify="space-between" no-gutters>


        <!-- Open location button -->
        <v-col cols="12" sm="auto" class="text-center">
          <v-btn
            size="small"
            prepend-icon="mdi-folder-open"
            @click="openHistoryLocation"
          >
            Open History Location
          </v-btn>
        </v-col>

        <!-- Last updated -->
        <v-col cols="12" sm="auto" class="text-center">
          <div class="text-body-2 text-medium-emphasis">
            Last updated: {{ formattedLastUpdated }}
          </div>
        </v-col>
      </v-row>
    </v-container>
  </v-footer>
</template>

<script setup>
import { ref, computed } from 'vue'

const lastUpdated = ref(new Date().getTime())

const formattedLastUpdated = computed(() => {
  const date = new Date(lastUpdated.value)
  return date.toLocaleString('en-US', {
    year: 'numeric',
    month: 'short',
    day: 'numeric',
    hour: '2-digit',
    minute: '2-digit'
  })
})

const openHistoryLocation = () => {
  console.log("Open history folder location")
}

</script>
<style scoped>


</style>
