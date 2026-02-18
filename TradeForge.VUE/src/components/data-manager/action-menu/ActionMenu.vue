<template>
  <v-menu
    v-model="isOpen"
    :style="{ left: menuX + 'px', top: menuY + 'px' }"
    :absolute="absolute"
    offset-y
  >
    <v-list density="compact">
      <template v-for="item in items" :key="item.key">
        <v-divider v-if="item.type === 'divider'" />
        <v-list-item
          v-else
          @click="handleAction(item)"
          :disabled="props.disabledButtons[item.key]"
        >
          <template v-slot:prepend v-if="item.icon">
            <v-icon :icon="item.icon" size="small"/>
          </template>
          <v-list-item-title>{{ item.label }}</v-list-item-title>
        </v-list-item>
      </template>
    </v-list>
  </v-menu>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import type { ActionButton, ActionMenuItem } from '@/types/ActionMenuItem.interface.ts';

interface Props {
  items: ActionMenuItem[];
  absolute?: boolean;
  disabledButtons?: Record<string, boolean>;
}

const props = withDefaults(defineProps<Props>(), {
  disabledButtons: () => ({})
})

const isOpen = ref(false);
const menuX = ref(0);
const menuY = ref(0);

const show = (x: number, y: number) => {
  menuX.value = x;
  menuY.value = y;
  isOpen.value = true;
};

const handleAction = (item: ActionMenuItem) => {
  if (item.type === 'button') {
    (item as ActionButton).action();
  }
  isOpen.value = false;
};

defineExpose({ show });
</script>
