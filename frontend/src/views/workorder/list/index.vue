<template>
  <div class="workorder-list">
    <el-row :gutter="16" class="stat-row">
      <el-col :xs="12" :sm="6" v-for="stat in stats" :key="stat.key">
        <el-card shadow="hover" class="stat-card" :class="{ active: filters.status === stat.status }" @click="filterByStatus(stat.status)">
          <div class="stat-value" :style="{ color: stat.color }">{{ stat.value }}</div>
          <div class="stat-label">{{ stat.label }}</div>
        </el-card>
      </el-col>
    </el-row>

    <el-card shadow="never" class="search-card">
      <el-form :model="filters" inline class="search-form">
        <el-form-item label="关键词"><el-input v-model="filters.keyword" placeholder="工单号/标题/设备" clearable style="width:200px" @keyup.enter="handleSearch" /></el-form-item>
        <el-form-item label="状态">
          <el-select v-model="filters.status" placeholder="全部" clearable style="width:120px">
            <el-option label="待处理" :value="1" /><el-option label="处理中" :value="2" /><el-option label="待验收" :value="3" /><el-option label="已完成" :value="4" /><el-option label="已关闭" :value="5" />
          </el-select>
        </el-form-item>
        <el-form-item label="优先级">
          <el-select v-model="filters.priority" placeholder="全部" clearable style="width:100px">
            <el-option label="紧急" :value="1" /><el-option label="高" :value="2" /><el-option label="中" :value="3" /><el-option label="低" :value="4" />
          </el-select>
        </el-form-item>

        <el-form-item label="系统">
          <el-select v-model="filters.systemId" placeholder="全部" clearable style="width:160px">
            <el-option v-for="sys in systemOptions" :key="sys.id" :label="sys.name" :value="sys.id" />
          </el-select>
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="handleSearch"><el-icon><Search /></el-icon>搜索</el-button>
          <el-button @click="handleReset"><el-icon><Refresh /></el-icon>重置</el-button>
        </el-form-item>
      </el-form>
    </el-card>

    <el-card shadow="never" class="table-card">
      <template #header>
        <div class="card-header">
          <span>工单列表</span>
          <el-button type="primary" size="small" @click="handleCreate"><el-icon><Plus /></el-icon>创建工单</el-button>
        </div>
      </template>
      <el-table v-loading="loading" :data="data" stripe>
        <el-table-column prop="id" label="工单号" width="100" />
        <el-table-column prop="title" label="标题" min-width="160" show-overflow-tooltip />
        <el-table-column prop="faultTypeName" label="故障类型" width="100" align="center">
          <template #default="{ row }"><el-tag type="info" size="small">{{ row.faultTypeName || '-' }}</el-tag></template>
        </el-table-column>
        <el-table-column prop="equipmentName" label="设备" width="120" show-overflow-tooltip />
        <el-table-column prop="systemName" label="所属系统" width="120" show-overflow-tooltip />
        <el-table-column prop="priority" label="优先级" width="80" align="center">
          <template #default="{ row }"><el-tag :type="getPriorityType(row.priority)" size="small">{{ getPriorityLabel(row.priority) }}</el-tag></template>
        </el-table-column>
        <el-table-column prop="status" label="状态" width="90" align="center">
          <template #default="{ row }"><el-tag :type="getStatusType(row.status)" size="small" effect="dark">{{ getStatusLabel(row.status) }}</el-tag></template>
        </el-table-column>
        <el-table-column prop="handlerName" label="处理人" width="90" />
        <el-table-column prop="createdAt" label="创建时间" width="110">
          <template #default="{ row }">{{ row.createdAt?.substring(0, 10) }}</template>
        </el-table-column>
        <el-table-column prop="planFinishTime" label="计划完成" width="110">
          <template #default="{ row }">{{ row.planFinishTime?.substring(0, 10) }}</template>
        </el-table-column>
        <el-table-column label="操作" width="130" align="center">
          <template #default="{ row }">
            <el-button type="primary" link size="small" @click="goDetail(row.id)">查看</el-button>
            <el-button v-if="row.status <= 2" type="success" link size="small" @click="handleProcess(row)">处理</el-button>
          </template>
        </el-table-column>
      </el-table>
      <div class="pagination-wrapper">
        <el-pagination v-model:current-page="pagination.page" v-model:page-size="pagination.pageSize" :page-sizes="[20,50,100]" :total="pagination.total" layout="total, sizes, prev, pager, next" @size-change="handleSizeChange" @current-change="handlePageChange" />
      </div>
    </el-card>
  </div>
</template>

<script setup>
import { reactive, onMounted, ref } from 'vue'
import { useRouter } from 'vue-router'
import { ElMessage } from 'element-plus'
import { Search, Refresh, Plus } from '@element-plus/icons-vue'
import { useWorkorderStore } from '@/store/workorder'
import { useTable } from '@/composables/useTable'
import { mapValue, WORKORDER_STATUS_MAP, PRIORITY_MAP } from '@/utils/statusMap'
import { workorderApi, equipmentApi } from '@/api'

const router = useRouter()
const workorderStore = useWorkorderStore()

const filters = reactive({ keyword: '', status: '', priority: '', systemId: '' })
const systemOptions = ref([])

const stats = [
  { key: 'pending', label: '待处理', value: 8, color: '#409EFF', status: 1 },
  { key: 'inProgress', label: '处理中', value: 12, color: '#e6a23c', status: 2 },
  { key: 'completed', label: '本月完成', value: 156, color: '#67c23a', status: 4 },
  { key: 'overdue', label: '超期工单', value: 3, color: '#f56c6c', status: '' }
]

const { loading, data, pagination, fetchData, handlePageChange, handleSizeChange, handleSearch: search, handleReset: reset } = useTable(
  (params) => workorderStore.fetchList(params), { pageSize: 20 }
)

const handleSearch = () => search(filters)
const handleReset = () => { Object.keys(filters).forEach(key => filters[key] = ''); reset() }
const filterByStatus = (status) => { filters.status = status; handleSearch() }
const getStatusType = (v) => mapValue(WORKORDER_STATUS_MAP, v, 'type', 'info')
const getStatusLabel = (v) => mapValue(WORKORDER_STATUS_MAP, v, 'label', v)
const getPriorityType = (v) => mapValue(PRIORITY_MAP, v, 'type', 'info')
const getPriorityLabel = (v) => mapValue(PRIORITY_MAP, v, 'label', v)

const goDetail = (id) => router.push('/workorder/detail/' + id)
const handleCreate = () => router.push('/workorder/create')
const handleProcess = async (row) => {
  try {
    await workorderApi.process(row.id)
    ElMessage.success('处理成功')
    fetchData()
  } catch (e) {
    ElMessage.error('处理失败')
  }
}

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

<style scoped lang="scss">
.workorder-list { display: flex; flex-direction: column; gap: 16px; }
.stat-row .stat-card { text-align: center; cursor: pointer; transition: all 0.2s; &:hover { transform: translateY(-2px); } &.active { border-color: #409EFF; } .stat-value { font-size: 28px; font-weight: 700; } .stat-label { font-size: 13px; color: #909399; margin-top: 4px; } }
.search-card .search-form .el-form-item { margin-bottom: 0; }
.table-card .card-header { display: flex; justify-content: space-between; align-items: center; }
.pagination-wrapper { display: flex; justify-content: flex-end; margin-top: 16px; }
.text-danger { color: #f56c6c; }
</style>
