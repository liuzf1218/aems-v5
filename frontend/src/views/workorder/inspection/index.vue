<template>
  <div class="page-container">
    <el-card shadow="never">
      <template #header>
        <div class="card-header">
          <span>巡检工单</span>
          <el-button type="primary" size="small" @click="$router.push('/workorder/create')">创建巡检</el-button>
        </div>
      </template>
      <el-form :model="filters" inline class="search-form" style="margin-bottom:16px">
        <el-form-item label="系统">
          <el-select v-model="filters.systemId" placeholder="全部" clearable style="width:160px" @change="handleSearch">
            <el-option v-for="sys in systemOptions" :key="sys.id" :label="sys.name" :value="sys.id" />
          </el-select>
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="handleSearch">搜索</el-button>
          <el-button @click="handleReset">重置</el-button>
        </el-form-item>
      </el-form>
      <el-table :data="data" stripe v-loading="loading">
        <el-table-column prop="id" label="工单号" width="90" />
        <el-table-column prop="title" label="巡检项目" min-width="150" />
        <el-table-column prop="equipmentName" label="设备" width="120" />
        <el-table-column prop="systemName" label="所属系统" width="110" />
        <el-table-column prop="priority" label="优先级" width="80" align="center">
          <template #default="{ row }">
            <el-tag :type="getPriorityType(row.priority)" size="small">{{ getPriorityLabel(row.priority) }}</el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="status" label="状态" width="85" align="center">
          <template #default="{ row }">
            <el-tag :type="getStatusType(row.status)" size="small">{{ getStatusLabel(row.status) }}</el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="handlerName" label="巡检人" width="90" />
        <el-table-column prop="createdAt" label="创建时间" width="105">
          <template #default="{ row }">{{ row.createdAt?.substring(0, 10) }}</template>
        </el-table-column>
        <el-table-column prop="planFinishTime" label="计划完成" width="105">
          <template #default="{ row }">{{ row.planFinishTime?.substring(0, 10) }}</template>
        </el-table-column>
        <el-table-column label="操作" width="100" align="center">
          <template #default="{ row }">
            <el-button type="primary" link size="small" @click="$router.push(`/workorder/detail/${row.id}`)">查看</el-button>
          </template>
        </el-table-column>
      </el-table>
      <div class="pagination-wrapper">
        <el-pagination v-model:current-page="pagination.page" v-model:page-size="pagination.pageSize" :total="pagination.total" layout="total, prev, pager, next" @current-change="handlePageChange" />
      </div>
    </el-card>
  </div>
</template>

<script setup>
import { reactive, onMounted, ref } from 'vue'
import { useWorkorderStore } from '@/store/workorder'
import { useTable } from '@/composables/useTable'
import { mapValue, WORKORDER_STATUS_MAP, PRIORITY_MAP } from '@/utils/statusMap'
import { equipmentApi } from '@/api'

const workorderStore = useWorkorderStore()
const systemOptions = ref([])

const filters = reactive({ systemId: '' })

const getStatusType = (v) => mapValue(WORKORDER_STATUS_MAP, v, 'type', 'info')
const getStatusLabel = (v) => mapValue(WORKORDER_STATUS_MAP, v, 'label', v)
const getPriorityType = (v) => mapValue(PRIORITY_MAP, v, 'type', 'info')
const getPriorityLabel = (v) => mapValue(PRIORITY_MAP, v, 'label', v)

const { loading, data, pagination, fetchData, handlePageChange, handleSearch, handleReset } = useTable(
  (params) => workorderStore.fetchList({ ...params, type: 1 }),
  { pageSize: 20, initialFilters: filters }
)

function extractSystems(nodes) {
  const systems = []
  const walk = (arr) => {
    arr.forEach(node => {
      if (node.nodeType === 'system') systems.push({ id: node.id, name: node.name })
      if (node.children) walk(node.children)
    })
  }
  walk(nodes)
  return systems
}

onMounted(async () => {
  fetchData()
  try {
    const tree = await equipmentApi.getTree()
    systemOptions.value = extractSystems(Array.isArray(tree) ? tree : (tree || []))
  } catch (e) {
    console.error('Load systems error:', e)
  }
})
</script>

<style scoped>
.card-header { display: flex; justify-content: space-between; align-items: center; }
.pagination-wrapper { display: flex; justify-content: flex-end; margin-top: 16px; }
.search-form .el-form-item { margin-bottom: 0; }
</style>
