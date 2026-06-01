import { defineStore } from 'pinia'
import { ref } from 'vue'
import { documentApi } from '@/api'

export const useDocumentStore = defineStore('document', () => {
  const list = ref([])
  const currentDocument = ref(null)
  const loading = ref(false)

  const fetchList = async (params = {}) => {
    loading.value = true
    try {
      const res = await documentApi.getList(params)
      return res || { items: [], total: 0 }
    } catch (e) {
      console.error('Fetch document list error:', e)
      return { items: [], total: 0 }
    } finally {
      loading.value = false
    }
  }

  const getById = async (id) => {
    loading.value = true
    try {
      currentDocument.value = await documentApi.getById(id)
      return currentDocument.value
    } catch (e) {
      console.error('Fetch document error:', e)
      return null
    } finally {
      loading.value = false
    }
  }

  return { list, currentDocument, loading, fetchList, getById }
})
