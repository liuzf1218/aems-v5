import { defineStore } from 'pinia'
import { ref } from 'vue'
import { sparepartApi, stockApi } from '@/api'

export const useSparepartStore = defineStore('sparepart', () => {
  const list = ref([])
  const currentSparepart = ref(null)
  const warnings = ref([])
  const stockInRecords = ref([])
  const stockOutRecords = ref([])
  const loading = ref(false)

  const fetchList = async (params = {}) => {
    loading.value = true
    try {
      const res = await sparepartApi.getList(params)
      return res || { items: [], total: 0 }
    } catch (e) {
      console.error('Fetch sparepart list error:', e)
      return { items: [], total: 0 }
    } finally {
      loading.value = false
    }
  }

  const fetchWarnings = async () => {
    loading.value = true
    try {
      const res = await sparepartApi.getWarnings()
      warnings.value = res?.list || []
      return res
    } catch (e) {
      console.error('Fetch warnings error:', e)
      warnings.value = []
      return { list: [], stats: {} }
    } finally {
      loading.value = false
    }
  }

  const fetchStockIn = async (params = {}) => {
    loading.value = true
    try {
      const res = await stockApi.getInList(params)
      return res || { items: [], total: 0 }
    } catch (e) {
      console.error('Fetch stock in error:', e)
      return { items: [], total: 0 }
    } finally {
      loading.value = false
    }
  }

  const fetchStockOut = async (params = {}) => {
    loading.value = true
    try {
      const res = await stockApi.getOutList(params)
      return res || { items: [], total: 0 }
    } catch (e) {
      console.error('Fetch stock out error:', e)
      return { items: [], total: 0 }
    } finally {
      loading.value = false
    }
  }

  return {
    list, currentSparepart, warnings, stockInRecords, stockOutRecords,
    loading, fetchList, fetchWarnings, fetchStockIn, fetchStockOut
  }
})
