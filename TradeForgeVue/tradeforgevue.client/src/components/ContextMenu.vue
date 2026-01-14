<template>
  <v-menu
    v-model="isOpen"
    :target="[x, y]"
    location="bottom start"
    scroll-strategy="close"
  >
    <slot
      :data="payload"
      :close="close"
    />
  </v-menu>
</template>

<script setup>
import {ref, defineExpose} from 'vue'

const isOpen = ref(false)
const x = ref(0)
const y = ref(0)
const payload = ref(null)

function open(event, data) {
  x.value = event.clientX
  y.value = event.clientY
  payload.value = data
  isOpen.value = true
}

function close() {
  isOpen.value = false
}

defineExpose({
  open,
  close
})
</script>
