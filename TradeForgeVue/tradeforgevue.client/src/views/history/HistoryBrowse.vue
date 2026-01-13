<template>
      <div class="mt-2 mr-2">
        <!-- Loading bar -->
        <div class="d-flex justify-center my-4"  v-if="loading">
          <v-progress-circular
            indeterminate
          />
        </div>

        <v-table v-else density="comfortable" striped="odd">
          <thead>
          <tr>
            <th>#</th>
            <th>Ticker</th>
            <th>Category</th>
            <th>Name</th>
            <th>Type</th>
            <th>Description</th>
            <th>Date From</th>
            <th>Date To</th>
            <th>Days</th>
          </tr>
          </thead>

          <tbody>
          <tr
            v-for="(item, index) in items"
            :key="item.id"
          >
            <td>{{ index + 1 }}</td>
            <td>{{ item.ticker }}</td>
            <td>{{ item.category }}</td>
            <td>{{ item.name }}</td>
            <td>{{ item.type }}</td>
            <td>{{ item.description }}</td>
            <td>{{ item.dateFrom }}</td>
            <td>{{ item.dateTo }}</td>
            <td>{{ item.days }}</td>
          </tr>

          <tr v-if="!items.length">
            <td colspan="9" class="text-center">
              No data
            </td>
          </tr>
          </tbody>
        </v-table>
      </div>

</template>

<script>
export default {

  data() {
    return {
      loading: false,
      items: [],
      headers: [
        {title: '#', key: 'index'},
        {title: 'Ticker', key: 'ticker'},
        {title: 'Category', key: 'category'},
        {title: 'Name', key: 'name'},
        {title: 'Type', key: 'type'},
        {title: 'Description', key: 'description'},
        {title: 'Date From', key: 'dateFrom'},
        {title: 'Date To', key: 'dateTo'},
        {title: 'Days', key: 'days'}
      ]
    }
  },

  mounted() {
    console.log('Component mounted.')
    this.fetchData();
  },

  methods: {
    fetchData() {
      this.loading = true

      // simulate API request
      setTimeout(() => {
        this.items = [
          {
            id: 1,
            ticker: 'EURUSD',
            category: 'Forex',
            name: 'Euro vs Dollar',
            type: 'CFD',
            description: 'Major forex pair',
            dateFrom: '2024-01-01',
            dateTo: '2024-01-31',
            days: 30
          },
          {
            id: 2,
            ticker: 'BTCUSD',
            category: 'Crypto',
            name: 'Bitcoin',
            type: 'Spot',
            description: 'Crypto asset',
            dateFrom: '2024-02-01',
            dateTo: '2024-02-20',
            days: 19
          }
        ]

        this.loading = false
      }, 200)
    }
  }
}
</script>
<style scoped>

</style>
