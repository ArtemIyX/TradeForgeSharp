<template>
  <v-btn
    icon="mdi-arrow-left"
    variant="text"
    size="small"
    class="mr-2"
    @click="goBack"
  />
</template>

<script setup>
import { useRouter } from 'vue-router'

const router = useRouter()

const goBack = () => {
  router.go(-1)
}

</script>
