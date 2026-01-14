import { ref } from 'vue'

const messages = ref([])

export function useSnackbar() {
  const showMessage = (text, color = 'success') => {
    messages.value.push({ text, color, id: Date.now() })
  }

  const showSuccess = (text) => showMessage(text, 'success')
  const showError = (text) => showMessage(text, 'error')
  const showInfo = (text) => showMessage(text, 'info')
  const showWarning = (text) => showMessage(text, 'warning')

  return {
    messages,
    showMessage,
    showSuccess,
    showError,
    showInfo,
    showWarning
  }
}
