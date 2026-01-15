<template>
  <v-dialog
    v-model="isOpen"
    :persistent="true"
    max-width="500"
  >
    <v-card>
      <v-card-title class="text-h5">
        Are you sure you want to clear history data for <span
        class="text-weight-bold text-warning">{{ itemData.ticker }}</span>?
      </v-card-title>

      <v-card-text>
        <span>
          This will <span
          class="text-weight-bold text-uppercase text-error">permanently delete</span> all ticks/candles data for <span
          class="text-weight-bold text-accent">'{{ itemData.ticker }}'</span> ({{ itemData.type }}, ID: {{
            itemData.id
          }})
        </span>
      </v-card-text>

      <v-card-actions>
        <v-spacer></v-spacer>

        <v-btn
          color="grey"
          text
          @click="handleCancel"
          :disabled="isClearing"
        >
          No
        </v-btn>

        <v-btn
          color="error"
          text
          @click="handleConfirm"
          :disabled="isClearing"
          :loading="isClearing"
        >
          <v-progress-circular
            v-if="isClearing"
            indeterminate
            size="20"
            width="2"
          ></v-progress-circular>
          <span v-else>Yes</span>
        </v-btn>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>

<script setup>
import {ref, computed, defineProps, defineEmits} from 'vue'

const props = defineProps({
  modelValue: {
    type: Boolean,
    default: false
  },
  itemData: {
    type: Object,
    default: () => ({
      ticker: '',
      type: '',
      id: null
    })
  },
  onConfirm: Function
})

const emit = defineEmits(['update:modelValue', 'confirm', 'cancel'])

const isClearing = ref(false)

const isOpen = computed({
  get() {
    return props.modelValue
  },
  set(value) {
    emit('update:modelValue', value)
  }
})

const handleCancel = () => {
  if (!isClearing.value) {
    isOpen.value = false
    emit('cancel')
  }
}

const handleConfirm = async () => {
  isClearing.value = true

  try {
    // Call the parent's function directly (if passed as prop)
    if (props.onConfirm) {
      await props.onConfirm(props.itemData)
    }
  } finally {
    isClearing.value = false
    isOpen.value = false
  }
}
</script>
