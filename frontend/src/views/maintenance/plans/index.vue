<template>
  <div class="page-container">
    <el-card shadow="never">
      <template #header>
        <div class="card-header">
          <span>维护计划</span>
          <el-button type="primary" size="small" @click="handleAdd">新建计划</el-button>
        </div>
      </template>
      <el-form inline style="margin-bottom:16px">
        <el-form-item style="margin-bottom: 0">
          <el-input v-model="filters.keyword" placeholder="搜索计划名称/编号" clearable @keyup.enter="refresh" style="width:200px" />
        </el-form-item>
        <el-form-item style="margin-bottom: 0">
          <el-select v-model="filters.planType" placeholder="计划类型" clearable style="width:140px">
            <el-option label="日常维护" :value="1" />
            <el-option label="定期维护" :value="2" />
            <el-option label="专项维护" :value="3" />
          </el-select>
        </el-form-item>
        <el-form-item style="margin-bottom: 0">
          <el-select v-model="filters.systemId" placeholder="系统" clearable style="width:180px">
            <el-option v-for="sys in systems" :key="sys.id" :label="sys.name" :value="sys.id" />
          </el-select>
        </el-form-item>
        <el-form-item style="margin-bottom: 0">
          <el-button type="primary" @click="refresh">搜索</el-button>
          <el-button @click="filters.keyword = ''; filters.planType = ''; filters.systemId = ''; refresh()">重置</el-button>
        </el-form-item>
      </el-form>
      <el-table :data="plans" stripe v-loading="loading">
        <el-table-column prop="planNo" label="计划编号" width="120" />
        <el-table-column prop="name" label="计划名称" min-width="160" />
        <el-table-column prop="planTypeName" label="类型" width="90" />
        <el-table-column prop="equipmentName" label="关联设备" width="130" />
        <el-table-column prop="systemName" label="所属系统" width="120" />
        <el-table-column prop="cycleDays" label="周期" width="85">
          <template #default="{ row }">{{ row.cycleDays }}天</template>
        </el-table-column>
        <el-table-column prop="startDate" label="开始日期" width="105" />
        <el-table-column prop="statusName" label="状态" width="80" align="center">
          <template #default="{ row }">
            <el-tag :type="row.status === 1 ? 'success' : 'info'" size="small">
              {{ row.statusName }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="taskCount" label="任务数" width="80" align="center" />
        <el-table-column label="操作" width="130" align="center">
          <template #default="{ row }">
            <el-button type="primary" link size="small" @click="handleView(row)">查看</el-button>
            <el-button type="success" link size="small" @click="handleEdit(row)">编辑</el-button>
          </template>
        </el-table-column>
      </el-table>
    </el-card>

    <PlanDialog
      v-model:visible="dialogVisible"
      :data="currentPlan"
      :readonly="readonly"
      @success="refresh"
    />
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useMaintenanceStore } from '@/store/maintenance'
import { equipmentApi } from '@/api'
import PlanDialog from './PlanDialog.vue'

const maintenanceStore = useMaintenanceStore()
const plans = ref([])
const loading = ref(false)
const systems = ref([])
const filters = ref({ keyword: '', planType: '', systemId: '' })

const dialogVisible = ref(false)
const readonly = ref(false)
const currentPlan = ref(null)

const refresh = async () => {
  loading.value = true
  const params = {}
  if (filters.value.keyword) params.keyword = filters.value.keyword
  if (filters.value.planType) params.planType = filters.value.planType
  if (filters.value.systemId) params.systemId = filters.value.systemId
  const res = await maintenanceStore.fetchPlans(params)
  plans.value = res.items || []
  loading.value = false
}

const loadSystems = async () => {
  try {
    const res = await equipmentApi.getTree()
    const tree = Array.isArray(res) ? res : (res || [])
    const extract = (nodes) => {
      const result = []
      for (const node of nodes || []) {
        if (node.nodeType === 'system') result.push(node)
        if (node.children?.length) result.push(...extract(node.children))
      }
      return result
    }
    systems.value = extract(tree)
  } catch (e) {
    console.error('Load systems error:', e)
  }
}

onMounted(() => {
  refresh()
  loadSystems()
})

const handleAdd = () => {
  currentPlan.value = null
  readonly.value = false
  dialogVisible.value = true
}

const handleView = (row) => {
  currentPlan.value = row
  readonly.value = true
  dialogVisible.value = true
}

const handleEdit = (row) => {
  currentPlan.value = row
  readonly.value = false
  dialogVisible.value = true
}
</script>

<style scoped>
.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}
</style>
