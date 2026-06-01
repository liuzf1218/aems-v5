import { defineStore } from 'pinia'
import { ref } from 'vue'
import { roomApi } from '@/api'

export const useRoomStore = defineStore('room', () => {
  const list = ref([])
  const currentRoom = ref(null)
  const loading = ref(false)

  const fetchList = async (params = {}) => {
    loading.value = true
    try {
      const res = await roomApi.getList(params)
      return res || { items: [], total: 0 }
    } catch (e) {
      console.error('Fetch room list error:', e)
      return { items: [], total: 0 }
    } finally {
      loading.value = false
    }
  }

  const getById = async (id) => {
    loading.value = true
    try {
      currentRoom.value = await roomApi.getById(id)
      return currentRoom.value
    } catch (e) {
      console.error('Fetch room error:', e)
      return null
    } finally {
      loading.value = false
    }
  }

  return { list, currentRoom, loading, fetchList, getById }
})
