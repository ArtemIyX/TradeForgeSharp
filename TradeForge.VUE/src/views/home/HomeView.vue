<style scoped>


</style>
<template>
  <h1>home</h1>
  <CandlestickChart :data="ohlcData"/>
</template>
<script setup lang="ts">

import CandlestickChart from "@/components/shared/charts/CandlestickChart.vue";
import {
  convertToOhlc,
  convertToOhlcArray,
  type OhlcData,
  type OhlcResponseModel
} from "@/types/OhlcData.ts";

import data from './ohlc-dummy.json';

const ohlcDataResponse = data as OhlcResponseModel[];

const ohlcData = convertToOhlcArray(ohlcDataResponse);
</script>

