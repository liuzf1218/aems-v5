import { ref, reactive, computed } from 'vue'

/**
 * 表格逻辑复用 Composable
 */
export function useTable(fetchFn, options = {}) {
  const loading = ref(false)
  const data = ref([])
  const pagination = reactive({
    page: 1,
    pageSize: options.pageSize || 20,
    total: 0
  })
  const filters = reactive(options.initialFilters || {})

  const hasData = computed(() => data.value.length > 0)

  const fetchData = async () => {
    loading.value = true
    try {
      const res = await fetchFn({
        page: pagination.page,
        pageSize: pagination.pageSize,
        ...filters
      })
      data.value = res.data || res.list || res.items || []
      pagination.total = res.total || res.totalCount || 0
    } catch (err) {
      console.error('Fetch data error:', err)
    } finally {
      loading.value = false
    }
  }

  const handlePageChange = (page) => {
    pagination.page = page
    fetchData()
  }

  const handleSizeChange = (size) => {
    pagination.pageSize = size
    pagination.page = 1
    fetchData()
  }

  const handleSearch = (params = {}) => {
    Object.assign(filters, params)
    pagination.page = 1
    fetchData()
  }

  const handleReset = () => {
    Object.keys(filters).forEach(key => {
      filters[key] = options.initialFilters?.[key] ?? ''
    })
    pagination.page = 1
    fetchData()
  }

  const refresh = () => fetchData()

  return {
    loading,
    data,
    pagination,
    filters,
    hasData,
    fetchData,
    handlePageChange,
    handleSizeChange,
    handleSearch,
    handleReset,
    refresh
  }
}
