<style scoped src="./OhlcTable.css"/>

<template>
  <!-- Delete confirmation dialog -->
  <v-dialog v-model="deleteDialog" max-width="420" persistent>
    <v-card>
      <v-card-title class="text-h6">Delete record?</v-card-title>
      <v-card-text>
        Remove OHLC entry for
        <strong>{{ formatDateTime(pendingDelete?.timestamp ?? null) }}</strong>?
        This action cannot be undone.
      </v-card-text>
      <v-card-actions>
        <v-spacer/>
        <v-btn variant="text" @click="deleteDialog = false" :disabled="deleteLoading">Cancel</v-btn>
        <v-btn color="error" variant="flat" @click="confirmDelete" :loading="deleteLoading">Delete</v-btn>
      </v-card-actions>
    </v-card>
  </v-dialog>

  <!-- Insert / Edit dialog -->
  <v-dialog v-model="formDialog" max-width="480" persistent>
    <v-card>
      <v-card-title class="text-h6">{{ editingItem ? 'Edit Record' : 'Insert Record' }}</v-card-title>
      <v-card-text>
        <v-form ref="formRef" @submit.prevent="submitForm">
          <v-row dense>
            <v-col cols="12">
              <v-text-field
                v-model="form.timestamp"
                label="Timestamp"
                type="datetime-local"
                density="compact"
                :rules="[required]"
                hide-details="auto"
              />
            </v-col>
            <v-col cols="6">
              <v-text-field
                v-model.number="form.open"
                label="Open"
                type="number"
                step="any"
                density="compact"
                :rules="[required, numeric]"
                hide-details="auto"
              />
            </v-col>
            <v-col cols="6">
              <v-text-field
                v-model.number="form.high"
                label="High"
                type="number"
                step="any"
                density="compact"
                :rules="[required, numeric]"
                hide-details="auto"
              />
            </v-col>
            <v-col cols="6">
              <v-text-field
                v-model.number="form.low"
                label="Low"
                type="number"
                step="any"
                density="compact"
                :rules="[required, numeric]"
                hide-details="auto"
              />
            </v-col>
            <v-col cols="6">
              <v-text-field
                v-model.number="form.close"
                label="Close"
                type="number"
                step="any"
                density="compact"
                :rules="[required, numeric]"
                hide-details="auto"
              />
            </v-col>
            <v-col cols="12">
              <v-text-field
                v-model.number="form.volume"
                label="Volume (optional)"
                type="number"
                step="any"
                density="compact"
                hide-details="auto"
              />
            </v-col>
          </v-row>
        </v-form>
      </v-card-text>
      <v-card-actions>
        <v-spacer/>
        <v-btn variant="text" @click="closeForm" :disabled="formLoading">Cancel</v-btn>
        <v-btn color="primary" variant="flat" @click="submitForm" :loading="formLoading">
          {{ editingItem ? 'Save' : 'Insert' }}
        </v-btn>
      </v-card-actions>
    </v-card>
  </v-dialog>

  <!-- Main table -->
  <div class="table-container">

    <div class="pagination-header">
      <span class="pagination-info">
        {{ paginationInfo }}
      </span>
      <div class="d-flex align-center gap-2">
        <v-select
          v-model="pageSize"
          :items="pageSizeOptions"
          density="compact"
          hide-details
          variant="outlined"
          style="width: 5rem"
        />
        <v-btn icon="mdi-chevron-left" size="small" variant="text" :disabled="currentPage === 1" @click="currentPage--"/>
        <span class="pagination-info">{{ currentPage }} / {{ totalPages || 1 }}</span>
        <v-btn icon="mdi-chevron-right" size="small" variant="text" :disabled="currentPage >= totalPages" @click="currentPage++"/>
        <v-btn
          size="small"
          variant="text"
          prepend-icon="mdi-plus"
          color="primary"
          @click="openInsert"
        >
          Insert
        </v-btn>
      </div>
    </div>

    <div class="table-wrapper">
      <table class="custom-table">
        <thead class="table-head">
        <tr>
          <th>Timestamp</th>
          <th>Open</th>
          <th>High</th>
          <th>Low</th>
          <th>Close</th>
          <th>Volume</th>
          <th class="actions-column">Actions</th>
        </tr>
        </thead>
        <tbody>
        <tr v-if="loading">
          <td colspan="7" class="text-center" style="padding: 3rem 1rem">
            <v-progress-circular indeterminate size="24"/>
          </td>
        </tr>
        <tr v-else-if="!pagedItems.length">
          <td colspan="7" class="empty-state">No OHLC data available</td>
        </tr>
        <tr
          v-else
          v-for="item in pagedItems"
          :key="String(item.timestamp)"
          class="data-row"
        >
          <td>{{ formatDateTime(item.timestamp) }}</td>
          <td>{{ item.open }}</td>
          <td>{{ item.high }}</td>
          <td :class="item.close >= item.open ? 'bullish' : 'bearish'">{{ item.low }}</td>
          <td :class="item.close >= item.open ? 'bullish' : 'bearish'">{{ item.close }}</td>
          <td>{{ item.volume != null ? item.volume.toLocaleString() : '—' }}</td>
          <td class="actions-cell">
            <v-btn icon="mdi-pencil" size="x-small" variant="text" @click.stop="openEdit(item)"/>
            <v-btn icon="mdi-delete" size="x-small" variant="text" color="error" @click.stop="openDelete(item)"/>
          </td>
        </tr>
        </tbody>
      </table>
    </div>
  </div>
</template>

<script setup lang="ts">
import {ref, computed, watch} from 'vue';
import type {OhlcData} from '@/types/OhlcData.ts';

interface Props {
  items: OhlcData[];
  loading?: boolean;
}

const props = withDefaults(defineProps<Props>(), {loading: false});

const emit = defineEmits<{
  insert: [item: Omit<OhlcData, 'timestamp'> & { timestamp: Date }]
  update: [original: OhlcData, updated: Omit<OhlcData, 'timestamp'> & { timestamp: Date }]
  remove: [item: OhlcData]
}>();

// ── Pagination ────────────────────────────────────────────────────────────────
const pageSizeOptions = [25, 50, 100, 250];
const pageSize = ref(50);
const currentPage = ref(1);

const totalPages = computed(() => Math.ceil(props.items.length / pageSize.value));

const paginationInfo = computed(() => {
  const total = props.items.length;
  const from = total === 0 ? 0 : (currentPage.value - 1) * pageSize.value + 1;
  const to = Math.min(currentPage.value * pageSize.value, total);
  return `${from}–${to} of ${total.toLocaleString()} records`;
});

const pagedItems = computed(() => {
  const start = (currentPage.value - 1) * pageSize.value;
  return props.items.slice(start, start + pageSize.value);
});

// Reset to page 1 when data or page size changes
watch(() => props.items.length, () => { currentPage.value = 1; });
watch(pageSize, () => { currentPage.value = 1; });

// ── Delete ────────────────────────────────────────────────────────────────────
const deleteDialog = ref(false);
const deleteLoading = ref(false);
const pendingDelete = ref<OhlcData | null>(null);

const openDelete = (item: OhlcData) => {
  pendingDelete.value = item;
  deleteDialog.value = true;
};

const confirmDelete = () => {
  if (!pendingDelete.value) return;
  deleteLoading.value = true;
  emit('remove', pendingDelete.value);
};

// Call after parent finishes the delete operation
const stopDeleteLoading = () => {
  deleteLoading.value = false;
  deleteDialog.value = false;
  pendingDelete.value = null;
};

// ── Insert / Edit ─────────────────────────────────────────────────────────────
const formDialog = ref(false);
const formLoading = ref(false);
const formRef = ref<any>(null);
const editingItem = ref<OhlcData | null>(null);

const emptyForm = () => ({
  timestamp: '',
  open: null as number | null,
  high: null as number | null,
  low: null as number | null,
  close: null as number | null,
  volume: null as number | null,
});

const form = ref(emptyForm());

const toDatetimeLocal = (d: Date | number) => {
  const date = d instanceof Date ? d : new Date(d);
  // format: YYYY-MM-DDTHH:mm
  const pad = (n: number) => String(n).padStart(2, '0');
  return `${date.getFullYear()}-${pad(date.getMonth() + 1)}-${pad(date.getDate())}T${pad(date.getHours())}:${pad(date.getMinutes())}`;
};

const openInsert = () => {
  editingItem.value = null;
  form.value = emptyForm();
  formDialog.value = true;
};

const openEdit = (item: OhlcData) => {
  editingItem.value = item;
  form.value = {
    timestamp: toDatetimeLocal(item.timestamp instanceof Date ? item.timestamp : new Date(item.timestamp)),
    open: item.open,
    high: item.high,
    low: item.low,
    close: item.close,
    volume: item.volume ?? null,
  };
  formDialog.value = true;
};

const closeForm = () => {
  formDialog.value = false;
  editingItem.value = null;
};

const submitForm = async () => {
  const {valid} = await formRef.value.validate();
  if (!valid) return;

  const payload = {
    timestamp: new Date(form.value.timestamp),
    open: form.value.open!,
    high: form.value.high!,
    low: form.value.low!,
    close: form.value.close!,
    volume: form.value.volume ?? undefined,
  };

  formLoading.value = true;
  if (editingItem.value) {
    emit('update', editingItem.value, payload);
  } else {
    emit('insert', payload);
  }
};

const stopFormLoading = () => {
  formLoading.value = false;
  formDialog.value = false;
  editingItem.value = null;
};

// ── Helpers ───────────────────────────────────────────────────────────────────
const formatDateTime = (value: Date | number | null) => {
  if (value == null) return '—';
  return new Date(value).toLocaleString();
};

// ── Validation ────────────────────────────────────────────────────────────────
const required = (v: any) => (v !== null && v !== '' && v !== undefined) || 'Required';
const numeric = (v: any) => (!isNaN(Number(v))) || 'Must be a number';

// ── Expose for parent control ─────────────────────────────────────────────────
defineExpose({stopDeleteLoading, stopFormLoading});
</script>
