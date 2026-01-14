import { ref } from 'vue'

const errors = ref([])

export function useErrorSnackbar() {
  const showGlobalError = (text, color = 'error') => {
    errors.value.push({ text, color, id: Date.now() })
  }


  return {
    errors,
    showGlobalError,
  }
}
