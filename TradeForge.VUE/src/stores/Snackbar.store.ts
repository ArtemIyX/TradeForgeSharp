import { defineStore } from 'pinia';
import { ref } from 'vue';

export interface SnackbarOptions {
  message: string;
  color?: string;
  timeout?: number;
}

interface SnackbarItem extends Required<SnackbarOptions> {
  id: number;
}

export const useSnackbarStore = defineStore('snackbar', () => {
  const queue = ref<SnackbarItem[]>([]);
  const visible = ref(false);
  const current = ref<SnackbarItem | null>(null);
  let nextId = 0;

  const show = (options: SnackbarOptions) => {
    const item: SnackbarItem = {
      id: nextId++,
      message: options.message,
      color: options.color || 'info',
      timeout: options.timeout || 3000
    };

    queue.value.push(item);

    if (!visible.value) {
      showNext();
    }
  };

  const showNext = () => {
    if (queue.value.length === 0) {
      visible.value = false;
      current.value = null;
      return;
    }

    current.value = queue.value.shift()!;
    visible.value = true;
  };

  const onClose = () => {
    visible.value = false;
    // Show next after a small delay
    setTimeout(() => {
      showNext();
    }, 200);
  };

  return {
    visible,
    current,
    queue,
    show,
    onClose
  };
});
