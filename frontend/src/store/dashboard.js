import { defineStore } from 'pinia'
import { ref } from 'vue'
import { statisticsApi, equipmentApi, workorderApi } from '@/api'

export const useDashboardStore = defineStore('dashboard', () => {
  const stats = ref({})
  const deviceStats = ref({})
  const loading = ref(false)

  const fetchDashboard = async () => {
    loading.value = true
    try {
      const res = await statisticsApi.getDashboard()
      stats.value = res || {}
      return res
    } catch (e) {
      console.error('Fetch dashboard error:', e)
      stats.value = {}
      return {}
    } finally {
      loading.value = false
    }
  }

  const fetchDeviceStats = async () => {
    loading.value = true
    try {
      const res = await statisticsApi.getDevice()
      deviceStats.value = res || {}
      return res
    } catch (e) {
      console.error('Fetch device stats error:', e)
      deviceStats.value = {}
      return {}
    } finally {
      loading.value = false
    }
  }

  return { stats, deviceStats, loading, fetchDashboard, fetchDeviceStats }
})
