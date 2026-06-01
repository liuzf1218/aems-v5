<template>
  <el-dialog
    title="任务详情"
    v-model="dialogVisible"
    width="600px"
    :close-on-click-modal="false"
  >
    <el-descriptions :column="2" border>
      <el-descriptions-item label="任务编号">{{ data?.id || '-' }}</el-descriptions-item>
      <el-descriptions-item label="任务名称">{{ data?.name || '-' }}</el-descriptions-item>
      <el-descriptions-item label="关联设备">{{ data?.equipmentName || '-' }}</el-descriptions-item>
      <el-descriptions-item label="执行人">{{ data?.executorName || '-' }}</el-descriptions-item>
      <el-descriptions-item label="计划完成日期">{{ data?.planTime ? data.planTime.substring(0, 16) : '-' }}</el-descriptions-item>
      <el-descriptions-item label="状态">
        <el-tag :type="statusType" size="small">{{ data?.statusName || (data?.status === 0 ? '待执行' : data?.status === 1 ? '执行中' : data?.status === 2 ? '已完成' : '-') }}</el-tag>
      </el-descriptions-item>
      <el-descriptions-item label="维护内容" :span="2">{{ data?.content || '-' }}</el-descriptions-item>
      <el-descriptions-item label="备注" :span="2">{{ data?.remark || '-' }}</el-descriptions-item>
    </el-descriptions>
    <template #footer>
      <el-button @click="dialogVisible = false">关闭</el-button>
    </template>
  </el-dialog>
</template>

<script setup>
import { computed } from 'vue'

const props = defineProps({
  visible: Boolean,
  data: Object
})

const emit = defineEmits(['update:visible'])

const dialogVisible = computed({
  get: () => props.visible,
  set: (val) => emit('update:visible', val)
})

const statusType = computed(() => {
  const s = props.data?.status
  if (s === 2 || s === '已完成') return 'success'
  if (s === 0 || s === '待执行') return 'warning'
  if (s === 1 || s === '执行中') return 'primary'
  return 'info'
})
</script>
