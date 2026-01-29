<template>
  <!-- Top Bar with Filters -->
  <v-card class="ma-2">
    <v-card-text class="pa-4">
      <v-row align="center" dense>
        <!-- Category Filter -->
        <v-col cols="12" sm="4" md="3">
          <v-combobox
            v-model="filterCategory"
            :items="categoryItems"
            label="Category"
            variant="solo"
            density="comfortable"
            hide-details
          />
        </v-col>

        <!-- Type Filter -->
        <v-col cols="12" sm="4" md="3">
          <v-combobox
            v-model="filterType"
            :items="typeItems"
            label="Type"
            variant="solo"
            density="comfortable"
            hide-details
          />
        </v-col>

        <!-- Spacer for larger screens -->
        <v-col cols="0" md="3" class="d-none d-md-block"/>



        <v-col cols="12" sm="4" md="3">
          <v-menu>
            <template v-slot:activator="{ props }">
              <v-btn
                v-bind="props"
                color="primary"
                size="large"
                block
                append-icon="mdi-chevron-down"
              >
                Actions
              </v-btn>
            </template>
            <v-list>
              <v-list-item
                to="/history/create"
                prepend-icon="mdi-plus"
              >
                <v-list-item-title>Create</v-list-item-title>
              </v-list-item>

              <v-list-item
                to="/history/import"
                prepend-icon="mdi-import"
              >
                <v-list-item-title>Import</v-list-item-title>
              </v-list-item>

              <v-list-item

                prepend-icon="mdi-export"
              >
                <v-list-item-title>Export</v-list-item-title>
              </v-list-item>

              <v-divider/>

              <v-list-item

                prepend-icon="mdi-delete"

              >
                <v-list-item-title>Delete Selected</v-list-item-title>
              </v-list-item>
            </v-list>
          </v-menu>
        </v-col>
      </v-row>
    </v-card-text>
  </v-card>

  <div class="d-flex justify-center my-4" v-if="loading">
    <v-progress-circular
      indeterminate
    />
  </div>
  <v-table density="comfortable" striped="odd" class="ma-2" v-else >
    <thead>
    <tr>
      <th
        v-for="(col, index) in columns"
        :key="col.id"
        :style="{ cursor: index === 0 ? 'default' : 'pointer', userSelect: 'none' }"
        :class="{
              'd-none d-md-table-cell': ['#', 'description', 'dateFrom', 'dateTo'].includes(col.key)
            }"
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
        @contextmenu.prevent="openMenu($event, item)"
    >
      <td>{{ idx + 1 }}</td>
      <td>{{ item.ticker }}</td>
      <td>{{ item.category }}</td>
      <td>{{ item.name }}</td>
      <td>{{ item.type }}</td>
      <td class="d-none d-md-table-cell">{{ item.description }}</td>
      <td class="d-none d-md-table-cell">{{ item.dateFrom }}</td>
      <td class="d-none d-md-table-cell">{{ item.dateTo }}</td>
      <td :class="item.days === 0 ? 'text-warning' : ''">
        {{ item.days }}
      </td>
    </tr>

    <tr v-if="!items.length">
      <td colspan="9" class="text-center">No data</td>
    </tr>
    </tbody>
  </v-table>


  <ContextMenu ref="contextMenu">
    <template #default="{ data, close }">
      <v-list density="compact">
        <v-list-item @click="editItem(data, close)">
          <v-list-item-title>Edit</v-list-item-title>
        </v-list-item>

        <v-list-item @click="viewData(data, close)">
          <v-list-item-title>
            View Data
          </v-list-item-title>
        </v-list-item>

        <v-list-item :disabled="data.days === 0" @click="clearItemHistory(data, close)">
          <v-list-item-title :class="data.days === 0 ? 'text-error' : '' ">
            Clear History
          </v-list-item-title>
        </v-list-item>

        <v-list-item @click="deleteItem(data, close)">
          <v-list-item-title class="text-error">
            Delete
          </v-list-item-title>
        </v-list-item>
      </v-list>
    </template>

  </ContextMenu>

  <HistoryDeleteDialog
    v-model="deletingDialog"
    :item-data="deleteData"
    :on-confirm="handleDelete"
    @cancel="handleDeleteCancel">
  </HistoryDeleteDialog>

  <HistoryClearDialog
    v-model="clearingDialog"
    :item-data="clearData"
    :on-confirm="handleClear"
    @cancel="handleClearCancel">
  </HistoryClearDialog>


</template>

<script src="./_historyBrowse.js" lang="js"/>
<style scoped>

</style>
