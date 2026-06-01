import { defineStore } from 'pinia'
import { ref } from 'vue'
import { softwareApi } from '@/api'

export const useSoftwareStore = defineStore('software', () => {
  const list = ref([])
  const currentSoftware = ref(null)
  const loading = ref(false)

  const fetchList = async (params = {}) => {
    loading.value = true
    try {
      const res = await softwareApi.getList(params)
      return res || { items: [], total: 0 }
    } catch (e) {
      console.error('Fetch software list error:', e)
      return { items: [], total: 0 }
    } finally {
      loading.value = false
    }
  }

  const getById = async (id) => {
    loading.value = true
    try {
      currentSoftware.value = await softwareApi.getById(id)
      return currentSoftware.value
    } catch (e) {
      console.error('Fetch software error:', e)
      return null
    } finally {
      loading.value = false
    }
  }

  return { list, currentSoftware, loading, fetchList, getById }
})
