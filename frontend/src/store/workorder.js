import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import { workorderApi } from '@/api'

export const useWorkorderStore = defineStore('workorder', () => {
  const list = ref([])
  const currentWorkorder = ref(null)
  const loading = ref(false)

  const pendingCount = computed(() => list.value.filter(w => w.status === 'PENDING').length)
  const inProgressCount = computed(() => list.value.filter(w => w.status === 'IN_PROGRESS').length)
  const overdueCount = computed(() => list.value.filter(w => w.isOverdue).length)

  const statusMap = {
    DRAFT: { label: '草稿', color: '#909399' },
    PENDING: { label: '待处理', color: '#409EFF' },
    IN_PROGRESS: { label: '处理中', color: '#e6a23c' },
    ON_HOLD: { label: '挂起', color: '#909399' },
    COMPLETED: { label: '已完成', color: '#67c23a' },
    CLOSED: { label: '已关闭', color: '#67c23a' },
    CANCELLED: { label: '已取消', color: '#c0c4cc' }
  }

  const priorityMap = {
    P1: { label: '紧急', color: '#f56c6c' },
    P2: { label: '重要', color: '#e6a23c' },
    P3: { label: '一般', color: '#409EFF' },
    P4: { label: '低', color: '#909399' }
  }

  const fetchList = async (params = {}) => {
    loading.value = true
    try {
      const res = await workorderApi.getList(params)
      return res || { list: [], total: 0 }
    } catch (e) {
      console.error('Fetch workorder list error:', e)
      return { list: [], total: 0 }
    } finally {
      loading.value = false
    }
  }

  const getById = async (id) => {
    loading.value = true
    try {
      currentWorkorder.value = await workorderApi.getById(id)
      return currentWorkorder.value
    } catch (e) {
      console.error('Fetch workorder error:', e)
      return null
    } finally {
      loading.value = false
    }
  }

  const create = async (data) => {
    loading.value = true
    try {
      return await workorderApi.create(data)
    } catch (e) {
      console.error('Create workorder error:', e)
      return null
    } finally {
      loading.value = false
    }
  }

  const transitionStatus = async (id, action) => {
    return await workorderApi.transition(id, action)
  }

  const process = async (id) => {
    return await workorderApi.process(id)
  }

  return {
    list, currentWorkorder, loading,
    pendingCount, inProgressCount, overdueCount,
    statusMap, priorityMap,
    fetchList, getById, create, transitionStatus, process
  }
})
