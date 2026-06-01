import { defineStore } from 'pinia'
import { ref } from 'vue'

export const useAppStore = defineStore('app', () => {
  const sidebarCollapsed = ref(false)
  const theme = ref('light')
  const language = ref('zh-cn')
  const loading = ref(false)

  const toggleSidebar = () => {
    sidebarCollapsed.value = !sidebarCollapsed.value
  }

  const setLoading = (val) => {
    loading.value = val
  }

  return {
    sidebarCollapsed,
    theme,
    language,
    loading,
    toggleSidebar,
    setLoading
  }
})
