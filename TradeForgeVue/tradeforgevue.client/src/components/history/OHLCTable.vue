<template>
    <!-- OHLC Data Table -->
    <v-table striped="odd">
      <thead>
      <tr>
        <th class="text-left">Timestamp</th>
        <th class="text-right">Open</th>
        <th class="text-right">High</th>
        <th class="text-right">Low</th>
        <th class="text-right">Close</th>
        <th class="text-right">Volume</th>
      </tr>
      </thead>
      <tbody>
      <tr v-for="item in paginatedData" :key="item.timestamp">
        <td>{{ formatTimestamp(item.timestamp) }}</td>
        <td class="text-right">{{ formatPrice(item.open) }}</td>
        <td class="text-right">{{ formatPrice(item.high) }}</td>
        <td class="text-right">{{ formatPrice(item.low) }}</td>
        <td class="text-right">
          {{ formatPrice(item.close) }}
        </td>
        <td class="text-right">{{ formatVolume(item.volume) }}</td>
      </tr>
      </tbody>
    </v-table>

    <!-- Pagination -->
    <v-pagination
      v-model="currentPage"
      :length="totalPages"
      :total-visible="7"
      class="mt-4"
    ></v-pagination>

</template>

<script>
export default {
  name: 'OHLCTable',

  props: {
    // Array of OHLC objects
    ohlcData: {
      type: Array,
      required: true,
      default: () => []
    },

    // Number of rows per page
    rowsPerPage: {
      type: Number,
      default: 10
    },

    // Decimal precision (number of digits after decimal point)
    decimalPrecision: {
      type: Number,
      default: 2
    },

    // Ticker information object
    ticker: {
      type: Object,
      required: true,
      default: () => ({
        id: 0,
        ticker: '',
        category: '',
        name: '',
        type: '',
        description: ''
      })
    }
  },

  data() {
    return {
      currentPage: 1
    };
  },

  computed: {
    // Calculate total number of pages
    totalPages() {
      return Math.ceil(this.ohlcData.length / this.rowsPerPage);
    },

    // Get paginated data for current page
    paginatedData() {
      const start = (this.currentPage - 1) * this.rowsPerPage;
      const end = start + this.rowsPerPage;
      return this.ohlcData.slice(start, end);
    }
  },

  methods: {
    // Format price with specified decimal precision
    formatPrice(value) {
      if (value === null || value === undefined) return '-';
      return Number(value).toFixed(this.decimalPrecision);
    },

    // Format volume with thousand separators
    formatVolume(value) {
      if (value === null || value === undefined) return '-';
      return Number(value).toLocaleString();
    },

    // Format timestamp
    formatTimestamp(timestamp) {
      if (!timestamp) return '-';
      const date = new Date(timestamp);
      return date.toLocaleString();
    },


  }
};
</script>

<style scoped>

</style>
