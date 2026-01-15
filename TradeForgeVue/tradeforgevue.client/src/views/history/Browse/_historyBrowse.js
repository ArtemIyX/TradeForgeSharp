import {ref, computed, onMounted} from 'vue'

import ContextMenu from "@/components/ContextMenu.vue";
import HistoryDeleteDialog
  from "@/components/history/dialogs/historyDeleteDialog.vue";
import {useSnackbar} from "@/composables/useSnackbar.js";
import HistoryClearDialog from "@/components/history/dialogs/historyClearDialog.vue";

export default {
  components: {HistoryClearDialog, ContextMenu, HistoryDeleteDialog},
  setup() {
    const {showSuccess, showError, showInfo, showWarning} = useSnackbar()

    const loading = ref(false)
    const items = ref([])
    const sortKey = ref('')
    const sortDesc = ref(false)
    const filterCategory = ref(null)
    const filterType = ref(null)

    const deletingDialog = ref(false)
    const isDeleting = ref(false)
    const deleteData = ref({});

    const clearingDialog = ref(false)
    const isClearing = ref(false)
    const clearData = ref({});

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
      const fetchPromise = fetchData()

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
      return new Promise((resolve) => {
        isDeleting.value = true

        // Simulate server action with timeout
        setTimeout(() => {
          console.log('Item deleted:', deleteData.value)

          // Perform your actual delete logic here
          // For example: store.dispatch('deleteItem', deleteData.value)

          isDeleting.value = false
          deletingDialog.value = false

          showWarning('You have deleted ' + deleteData.value.ticker);

          // Optional: Reset delete data
          deleteData.value = {
            ticker: '',
            type: '',
            id: null
          }

          resolve() // Resolve the promise when done
        }, 750)
      })
    }

    const handleDeleteCancel = () => {
      console.log('Delete cancelled')
      showInfo('You have cancelled deleting ' + deleteData.value.ticker);
      deleteData.value = {
        ticker: '',
        type: '',
        id: null
      }
    }

    const clearItemHistory = (data, close) => {
      console.log('Clear History:', data)
      close();
      openClearDialog(data);
    };

    const openClearDialog = (item) => {
      clearData.value = item
      clearingDialog.value = true
    }

    const handleClear = () => {
      return new Promise((resolve) => {
        isDeleting.value = true

        setTimeout(() => {
          
          isClearing.value = false
          clearingDialog.value = false

          showWarning('You have cleared ' + clearData.value.ticker);

          // Optional: Reset delete data
          clearData.value = {
            ticker: '',
            type: '',
            id: null
          }

          resolve() // Resolve the promise when done
        }, 750)
      })
    }

    const handleClearCancel = () => {
      clearData.value = {
        ticker: '',
        type: '',
        id: null
      }
    }

    return {
      loading,
      items,
      sortKey,
      sortDesc,
      filterCategory,
      filterType,
      deletingDialog,
      isDeleting,
      deleteData,
      columns,
      categoryItems,
      typeItems,
      sortedItems,
      sortBy,
      fetchData,
      contextMenu,
      openMenu,
      editItem,
      deleteItem,
      openDeleteDialog,
      handleDelete,
      handleDeleteCancel,
      clearItemHistory,
      clearingDialog,
      isClearing,
      clearData,
      handleClear,
      handleClearCancel
    }
  }
}


