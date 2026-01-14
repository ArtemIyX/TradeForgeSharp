<template>
  <div class="mt-2 mr-2">
    <!-- Loading bar -->
    <div class="d-flex justify-center my-4" v-if="loading">
      <v-progress-circular
        indeterminate
      />
    </div>
    <div v-else>
      <v-row dense v-if="items.length">
        <v-col cols="12" sm="4">
          <v-combobox
            v-model="filterCategory"
            :items="categoryItems"
            label="Category"
            variant="solo"
          />
        </v-col>
        <v-col cols="12" sm="4">
          <v-combobox
            v-model="filterType"
            :items="typeItems"
            label="Type"
            variant="solo"
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

<script setup>
import {ref, computed, onMounted} from 'vue'

const loading = ref(false)
const items = ref([])
const sortKey = ref('')
const sortDesc = ref(false)
const filterCategory = ref(null)
const filterType = ref(null)

const columns = [
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

const categoryItems = computed(() => [...new Set(items.value.map(i => i.category))])
const typeItems = computed(() => [...new Set(items.value.map(i => i.type))])

const sortedItems = computed(() => {
  let list = [...items.value]

  if (filterCategory.value) {
    list = list.filter(i => i.category === filterCategory.value)
  }
  if (filterType.value) {
    list = list.filter(i => i.type === filterType.value)
  }

  if (!sortKey.value) return list

  return list.sort((a, b) => {
    const valA = a[sortKey.value]
    const valB = b[sortKey.value]
    let result = 0
    if (valA > valB) result = 1
    else if (valA < valB) result = -1
    return sortDesc.value ? -result : result
  })
})

const sortBy = (key) => {
  if (sortKey.value === key) {
    sortDesc.value = !sortDesc.value
  } else {
    sortKey.value = key
    sortDesc.value = false
  }
}

const fetchData = async () => {
  loading.value = true
  try {
    const res = await fetch('/dummy/tickers.json')
    items.value = await res.json()
  } catch (e) {
    console.error('Failed to load local JSON:', e)
    items.value = []
  } finally {
    loading.value = false
  }
}

onMounted(() => {
  fetchData()

})
</script>
<style scoped>

</style>
