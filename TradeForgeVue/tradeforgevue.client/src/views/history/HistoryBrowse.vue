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
            :class="item.days === 0 ? 'bg-error' : ''"
            @contextmenu.prevent="openMenu($event, item)">
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

      <!--      <v-menu
              v-model="menu"
              :target="[menuX, menuY]"
              location="bottom start"
              scroll-strategy="close">-->
      <ContextMenu ref="contextMenu">
        <template #default="{ data, close }">
          <v-list density="compact">
            <v-list-item @click="editItem(data, close)">
              <v-list-item-title>Edit</v-list-item-title>
            </v-list-item>

            <v-list-item @click="deleteItem(data, close)">
              <v-list-item-title class="text-error">
                Delete
              </v-list-item-title>
            </v-list-item>
          </v-list>
        </template>

      </ContextMenu>
      <v-dialog
        v-model="deletingDialog"
        :persistent="true"
        max-width="500"
      >
        <v-card>
          <v-card-title class="text-h5">
            Are you sure you want to delete <span
            class="text-weight-bold text-warning">{{ deleteData.ticker }}</span>?
          </v-card-title>

          <v-card-text>
    <span>
      This will <span class="text-weight-bold text-uppercase text-error">permanently delete</span> the symbol <span
      class="text-weight-bold text-accent">'{{ deleteData.ticker }}'</span> ({{ deleteData.type }}, ID: {{
        deleteData.id
      }})
    </span>
          </v-card-text>

          <v-card-actions>
            <v-spacer></v-spacer>

            <v-btn
              color="grey"
              text
              @click="deletingDialog = false"
              :disabled="isDeleting"
            >
              No
            </v-btn>

            <v-btn
              color="error"
              text
              @click="handleDelete"
              :disabled="isDeleting"
              :loading="isDeleting"
            >
              <v-progress-circular
                v-if="isDeleting"
                indeterminate
                size="20"
                width="2"
              ></v-progress-circular>
              <span v-else>Yes</span>
            </v-btn>
          </v-card-actions>
        </v-card>
      </v-dialog>
    </div>
  </div>

</template>

<script setup>
import {ref, computed, onMounted} from 'vue'

import ContextMenu from "@/components/ContextMenu.vue";

const loading = ref(false)
const items = ref([])
const sortKey = ref('')
const sortDesc = ref(false)
const filterCategory = ref(null)
const filterType = ref(null)

const deletingDialog = ref(false)
const isDeleting = ref(false)

const deleteData = ref({});

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

const contextMenu = ref(null)

const openMenu = (event, item) => {
  contextMenu.value.open(event, item)
}

const editItem = (item, close) => {
  console.log('Edit', item.id);
  close();
}

const deleteItem = (item, close) => {
  console.log('Delete', item.id)
  close();
  openDeleteDialog(item)
}

const openDeleteDialog = (item) => {
  deleteData.value = item
  deletingDialog.value = true
}

const handleDelete = () => {
  isDeleting.value = true

  // Simulate server action with timeout
  setTimeout(() => {
    console.log('Item deleted:', deleteData.value)

    // Perform your actual delete logic here
    // For example: store.dispatch('deleteItem', deleteData.value)

    isDeleting.value = false
    deletingDialog.value = false

    // Optional: Reset delete data
    deleteData.value = {
      ticker: '',
      type: '',
      id: null
    }
  }, 300) // 2 second timeout - adjust as needed
}

</script>
<style scoped>

</style>
