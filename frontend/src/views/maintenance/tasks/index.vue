<template>
  <div class="page-container">
    <el-card shadow="never">
      <template #header>
        <div class="card-header"><span>维护任务</span></div>
      </template>
      <el-form inline style="margin-bottom:16px">
        <el-form-item style="margin-bottom: 0">
          <el-input v-model="filters.keyword" placeholder="搜索任务名称/编号" clearable @keyup.enter="fetchData" style="width:200px" />
        </el-form-item>
        <el-form-item style="margin-bottom: 0">
          <el-select v-model="filters.status" placeholder="状态" clearable style="width:120px">
            <el-option label="待执行" :value="0" />
            <el-option label="执行中" :value="1" />
            <el-option label="已完成" :value="2" />
            <el-option label="已审核" :value="3" />
          </el-select>
        </el-form-item>
        <el-form-item style="margin-bottom: 0">
          <el-button type="primary" @click="fetchData">搜索</el-button>
          <el-button @click="filters.keyword = ''; filters.status = ''; fetchData()">重置</el-button>
        </el-form-item>
      </el-form>
      <el-table :data="tasks" stripe v-loading="loading">
        <el-table-column prop="id" label="任务号" width="100" />
        <el-table-column prop="name" label="任务名称" min-width="150" />
        <el-table-column prop="equipmentName" label="关联设备" width="130">
          <template #default="{ row }">{{ row.equipmentName || row.planName || '-' }}</template>
        </el-table-column>
        <el-table-column prop="planName" label="所属计划" width="130" />
        <el-table-column prop="planTime" label="计划完成时间" width="110">
          <template #default="{ row }">
            {{ row.planTime ? row.planTime.substring(0, 16) : '-' }}
          </template>
        </el-table-column>
        <el-table-column prop="actualTime" label="实际完成时间" width="110">
          <template #default="{ row }">
            {{ row.actualTime ? row.actualTime.substring(0, 16) : '-' }}
          </template>
        </el-table-column>
        <el-table-column prop="executorName" label="执行人" width="90">
          <template #default="{ row }">{{ row.executorName || (row.executorId ? '用户' + row.executorId : '-') }}</template>
        </el-table-column>
        <el-table-column prop="statusName" label="状态" width="85" align="center">
          <template #default="{ row }">
            <el-tag
              :type="row.status === 3 ? 'info' : (row.status === 2 ? 'success' : (row.status === 1 ? 'warning' : ''))"
              size="small"
            >
              {{ row.statusName }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column label="操作" width="100" align="center">
          <template #default="{ row }">
            <el-button type="primary" link size="small" @click="handleView(row)">查看</el-button>
          </template>
        </el-table-column>
      </el-table>
    </el-card>

    <TaskDialog v-model:visible="dialogVisible" :data="currentTask" />
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useMaintenanceStore } from '@/store/maintenance'
import TaskDialog from './TaskDialog.vue'

const maintenanceStore = useMaintenanceStore()
const tasks = ref([])
const loading = ref(false)
const filters = ref({ keyword: '', status: '' })

const dialogVisible = ref(false)
const currentTask = ref(null)

const fetchData = async () => {
  loading.value = true
  const params = {}
  if (filters.value.keyword) params.keyword = filters.value.keyword
  if (filters.value.status !== '') params.status = filters.value.status
  const res = await maintenanceStore.fetchTasks(params)
  tasks.value = res.items || []
  loading.value = false
}

onMounted(() => {
  fetchData()
})

const handleView = (row) => {
  currentTask.value = row
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
