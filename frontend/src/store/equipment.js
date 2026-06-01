import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import { equipmentApi } from '@/api'

export const useEquipmentStore = defineStore('equipment', () => {
  const list = ref([])
  const currentEquipment = ref(null)
  const loading = ref(false)

  const total = computed(() => list.value.length)
  const activeCount = computed(() => list.value.filter(e => e.status === 0 || e.status === 'ACTIVE').length)
  const faultCount = computed(() => list.value.filter(e => e.status === 1 || e.status === 'FAULT').length)
  const healthRate = computed(() => {
    if (total.value === 0) return 0
    return ((activeCount.value / total.value) * 100).toFixed(1)
  })

  const fetchList = async (params = {}) => {
    loading.value = true
    try {
      const res = await equipmentApi.getList(params)
      return res || { list: [], total: 0 }
    } catch (e) {
      console.error('Fetch equipment list error:', e)
      return { list: [], total: 0 }
    } finally {
      loading.value = false
    }
  }

  const getById = async (id) => {
    loading.value = true
    try {
      currentEquipment.value = await equipmentApi.getById(id)
      return currentEquipment.value
    } catch (e) {
      console.error('Fetch equipment error:', e)
      return null
    } finally {
      loading.value = false
    }
  }

  const create = async (data) => {
    loading.value = true
    try {
      return await equipmentApi.create(data)
    } catch (e) {
      console.error('Create equipment error:', e)
      return null
    } finally {
      loading.value = false
    }
  }

  const update = async (id, data) => {
    loading.value = true
    try {
      return await equipmentApi.update(id, data)
    } catch (e) {
      console.error('Update equipment error:', e)
      return null
    } finally {
      loading.value = false
    }
  }

  const remove = async (id) => {
    loading.value = true
    try {
      return await equipmentApi.delete(id)
    } catch (e) {
      console.error('Delete equipment error:', e)
      return null
    } finally {
      loading.value = false
    }
  }

  return {
    list, currentEquipment, loading,
    total, activeCount, faultCount, healthRate,
    fetchList, getById, create, update, remove
  }
})
