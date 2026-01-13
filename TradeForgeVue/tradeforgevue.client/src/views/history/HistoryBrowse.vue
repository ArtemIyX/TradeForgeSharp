<template>
  <div class="mt-2 mr-2">
    <!-- Loading bar -->
    <div class="d-flex justify-center my-4" v-if="loading">
      <v-progress-circular
        indeterminate
      />
    </div>

    <v-table density="comfortable" striped="odd" v-else>
      <thead>
      <tr>
        <th
          v-for="(col, index) in columns"
          :key="col.id"
          :style="{ cursor: index === 0 ? 'default' : 'pointer', userSelect: 'none' }"
          @click="index !== 0 && sortBy(col.key)"
        >
          <span>{{ col.title }}</span>
          <v-icon v-if="index !== 0"
                  size="14"
                  class="ml-1"
                  :icon="
              sortKey === col.key
                ? sortDesc
                  ? 'mdi-menu-down'
                  : 'mdi-menu-up'
                : 'mdi-unfold-more-horizontal'
            "/>
        </th>
      </tr>
      </thead>

      <tbody>
      <tr v-for="(item, idx) in sortedItems" :key="item.id"
          :class="item.days === 0 ? 'bg-error' : ''">
        <td>{{ idx + 1 }}</td>
        <td>{{ item.ticker }}</td>
        <td>{{ item.category }}</td>
        <td>{{ item.name }}</td>
        <td>{{ item.type }}</td>
        <td>{{ item.description }}</td>
        <td>{{ item.dateFrom }}</td>
        <td>{{ item.dateTo }}</td>
        <td>
          {{ item.days }}
        </td>
      </tr>

      <tr v-if="!items.length">
        <td colspan="9" class="text-center">No data</td>
      </tr>
      </tbody>
    </v-table>
  </div>

</template>

<script>
export default {
  name: "HistoryBrowseTable",

  data() {
    return {
      loading: false,
      items: [],
      sortKey: '',   // current column key
      sortDesc: false, // false = ASC, true = DESC
      columns: [
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

  computed: {
    sortedItems() {
      if (!this.sortKey) return this.items

      const copy = [...this.items]
      const key = this.sortKey
      const desc = this.sortDesc

      copy.sort((a, b) => {
        let va = a[key]
        let vb = b[key]

        // numeric
        if (key === 'days') {
          va = Number(va)
          vb = Number(vb)
        }
        // date
        if (key === 'dateFrom' || key === 'dateTo') {
          va = new Date(va)
          vb = new Date(vb)
        }

        if (va < vb) return desc ? 1 : -1
        if (va > vb) return desc ? -1 : 1
        return 0
      })
      return copy
    }
  },

  methods: {
    sortBy(key) {
      if (this.sortKey === key) {
        this.sortDesc = !this.sortDesc
      } else {
        this.sortKey = key
        this.sortDesc = false
      }
    },

    async fetchData() {
      this.loading = true
      try {
        const res = await fetch('/dummy/tickers.json')
        this.items = await res.json()
      } catch (e) {
        console.error('Failed to load local JSON:', e)
        this.items = []
      } finally {
        this.loading = false
      }
    }
  }
}
</script>
<style scoped>

</style>
