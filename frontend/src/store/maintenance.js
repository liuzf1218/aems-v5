import { defineStore } from 'pinia'
import { ref } from 'vue'
import { maintenanceApi } from '@/api'

export const useMaintenanceStore = defineStore('maintenance', () => {
  const plans = ref([])
  const tasks = ref([])
  const currentPlan = ref(null)
  const currentTask = ref(null)
  const loading = ref(false)

  const fetchPlans = async (params = {}) => {
    loading.value = true
    try {
      const res = await maintenanceApi.getPlans(params)
      return res || { items: [], total: 0 }
    } catch (e) {
      console.error('Fetch maintenance plans error:', e)
      return { items: [], total: 0 }
    } finally {
      loading.value = false
    }
  }

  const fetchTasks = async (params = {}) => {
    loading.value = true
    try {
      const res = await maintenanceApi.getTasks(params)
      return res || { items: [], total: 0 }
    } catch (e) {
      console.error('Fetch maintenance tasks error:', e)
      return { items: [], total: 0 }
    } finally {
      loading.value = false
    }
  }

  return { plans, tasks, currentPlan, currentTask, loading, fetchPlans, fetchTasks }
})
