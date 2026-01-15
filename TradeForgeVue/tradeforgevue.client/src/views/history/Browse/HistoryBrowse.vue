<template>
  <div class="pa-0">
    <!-- Loading bar -->
    <div class="d-flex justify-center my-4" v-if="loading">
      <v-progress-circular
        indeterminate
      />
    </div>
    <div v-else>
      <!-- Top Bar with Filters -->
      <v-card flat class="mb-4">
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

            <!-- Create Button -->
            <v-col cols="12" sm="4" md="3">
              <v-btn
                to="/history/create"
                color="primary"
                size="large"
                block
                prepend-icon="mdi-plus"
              >
                Create
              </v-btn>
            </v-col>
          </v-row>
        </v-card-text>
      </v-card>


      <v-table density="comfortable" striped="odd">
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
    </div>
  </div>

</template>

<script src="./_historyBrowse.js" lang="js"/>
<style scoped>

</style>
