<template>
  <div class="page-container" v-loading="loading">
    <el-card shadow="never">
      <template #header>
        <div style="display:flex;justify-content:space-between;align-items:center">
          <span>工单详情 - {{ workorder?.id }}</span>
          <el-tag :type="getStatusType(workorder?.status)">{{ getStatusLabel(workorder?.status) }}</el-tag>
        </div>
      </template>
      <el-descriptions :column="2" border v-if="workorder">
        <el-descriptions-item label="工单号">{{ workorder.id }}</el-descriptions-item>
        <el-descriptions-item label="状态">
          <el-tag :type="getStatusType(workorder.status)">{{ getStatusLabel(workorder.status) }}</el-tag>
        </el-descriptions-item>
        <el-descriptions-item label="标题">{{ workorder.title }}</el-descriptions-item>
        <el-descriptions-item label="类型">
          <el-tag :type="getTypeType(workorder.type)">{{ getTypeLabel(workorder.type) }}</el-tag>
        </el-descriptions-item>
        <el-descriptions-item label="优先级">
          <el-tag :type="getPriorityType(workorder.priority)">{{ getPriorityLabel(workorder.priority) }}</el-tag>
        </el-descriptions-item>
        <el-descriptions-item label="关联设备">{{ workorder.equipmentName }}</el-descriptions-item>
        <el-descriptions-item label="所属系统">{{ workorder.systemName || '-' }}</el-descriptions-item>
        <el-descriptions-item label="处理人">{{ workorder.handlerName }}</el-descriptions-item>
        <el-descriptions-item label="计划完成">{{ workorder.planFinishTime?.substring(0, 10) }}</el-descriptions-item>
        <el-descriptions-item label="创建时间">{{ workorder.createdAt?.substring(0, 10) }}</el-descriptions-item>
        <el-descriptions-item label="描述" :span="2">{{ workorder.description }}</el-descriptions-item>
        <el-descriptions-item label="故障现象" :span="2">{{ workorder.symptom || '-' }}</el-descriptions-item>
        <el-descriptions-item label="解决方案" :span="2">{{ workorder.solution || '-' }}</el-descriptions-item>
        <el-descriptions-item label="费用" v-if="workorder.cost">&#165;{{ workorder.cost }}</el-descriptions-item>
      </el-descriptions>
      <div style="margin-top:16px">
        <el-button @click="$router.back()">返回</el-button>
        <el-button type="primary" v-if="workorder?.status <= 2" @click="handleProcess">处理工单</el-button>
        <el-button type="success" v-if="workorder?.status === 2">完成工单</el-button>
      </div>
    </el-card>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useRoute } from 'vue-router'
import { ElMessage } from 'element-plus'
import { useWorkorderStore } from '@/store/workorder'
import { workorderApi } from '@/api'
import { mapValue, WORKORDER_STATUS_MAP, WORKORDER_TYPE_MAP, PRIORITY_MAP } from '@/utils/statusMap'

const route = useRoute()
const workorderStore = useWorkorderStore()
const loading = ref(true)
const workorder = ref(null)

const getStatusType = (v) => mapValue(WORKORDER_STATUS_MAP, v, 'type', 'info')
const getStatusLabel = (v) => mapValue(WORKORDER_STATUS_MAP, v, 'label', v)
const getTypeType = (v) => mapValue(WORKORDER_TYPE_MAP, v, 'type', 'info')
const getTypeLabel = (v) => mapValue(WORKORDER_TYPE_MAP, v, 'label', v)
const getPriorityType = (v) => mapValue(PRIORITY_MAP, v, 'type', 'info')
const getPriorityLabel = (v) => mapValue(PRIORITY_MAP, v, 'label', v)

const loadDetail = async () => {
  loading.value = true
  try {
    workorder.value = await workorderStore.getById(route.params.id)
  } finally {
    loading.value = false
  }
}

const handleProcess = async () => {
  try {
    await workorderApi.process(workorder.value.id)
    ElMessage.success('处理成功')
    await loadDetail()
  } catch (e) {
    ElMessage.error('处理失败')
  }
}

onMounted(loadDetail)
</script>
