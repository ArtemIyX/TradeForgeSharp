<template>


</template>

<script setup lang="ts">
import {useRouter} from 'vue-router'

const router = useRouter()

const goHome = () => {
  router.push({name: 'home'})
}
</script>

<style scoped>

</style>
