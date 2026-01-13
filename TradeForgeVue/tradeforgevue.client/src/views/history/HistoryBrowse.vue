<template>
  <div class="mt-2 mr-2">
    <!-- Loading bar -->
    <div class="d-flex justify-center my-4" v-if="loading">
      <v-progress-circular
        indeterminate
      />
    </div>
    <div v-else>

      <v-row class="mb-3" dense v-if="items.length">
        <v-col cols="12" sm="4">
          <v-combobox
            v-model="filterCategory"
            label="Category"
            :items="categoryItems"
            clearable
            hide-details
          />
        </v-col>
        <v-col cols="12" sm="4">
          <v-combobox
            v-model="filterType"
            label="Type"
            :items="typeItems"
            clearable
            hide-details
          />
        </v-col>
      </v-row>

      <v-table density="comfortable" striped="odd">
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
      ],
      filterCategory: null,
      filterType: null
    }
  },

  mounted() {
    console.log('Component mounted.')
    this.fetchData();
  },

  computed: {
    categoryItems() {
      return [...new Set(this.items.map(i => i.category))]
    },
    typeItems() {
      return [...new Set(this.items.map(i => i.type))]
    },
    sortedItems() {
      let list = this.items

      if (this.filterCategory) {
        list = list.filter(i => i.category === this.filterCategory)
      }
      if (this.filterType) {
        list = list.filter(i => i.type === this.filterType)
      }

      if (!this.sortKey) return list

      return [...list].sort((a, b) => {
        const valA = a[this.sortKey]
        const valB = b[this.sortKey]
        let result = 0
        if (valA > valB) result = 1
        else if (valA < valB) result = -1
        return this.sortDesc ? -result : result
      })
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
