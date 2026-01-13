<template>
  <v-app-bar>
    <v-app-bar-title>History Manager</v-app-bar-title>

    <v-spacer/>
    <v-btn to="/history/create">Create</v-btn>
    <v-btn to="/history/browse">Browse</v-btn>


  </v-app-bar>
  <router-view class="ml-2"/>
</template>

<script>

export default {}
</script>
<style lang="">

</style>
