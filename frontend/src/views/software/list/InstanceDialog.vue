<template>
  <el-dialog
    title="软件实例"
    :model-value="visible"
    @update:model-value="$emit('update:visible', $event)"
    width="800px"
  >
    <el-table :data="filteredInstances" stripe v-loading="loading">
      <el-table-column prop="id" label="实例编号" width="120" />
      <el-table-column prop="name" label="实例名称" min-width="150" />
      <el-table-column prop="serverName" label="所在服务器" width="150" />
      <el-table-column prop="status" label="状态" width="100" align="center">
        <template #default="{ row }">
          <el-tag :type="row.status === '运行中' ? 'success' : 'info'" size="small">{{ row.status }}</el-tag>
        </template>
      </el-table-column>
      <el-table-column prop="installDate" label="安装日期" width="120" />
    </el-table>
    <template #footer>
      <el-button @click="$emit('update:visible', false)">关闭</el-button>
    </template>
  </el-dialog>
</template>

<script setup>
import { ref, watch, computed } from 'vue'
import { ElMessage } from 'element-plus'
import { softwareApi } from '@/api'

const props = defineProps({
  visible: {
    type: Boolean,
    default: false
  },
  softwareId: {
    type: Number,
    default: null
  }
})

const emit = defineEmits(['update:visible'])

const instances = ref([])
const loading = ref(false)

const filteredInstances = computed(() => {
  if (!props.softwareId) return []
  return instances.value.filter(item => item.softwareId === props.softwareId)
})

const fetchInstances = async () => {
  loading.value = true
  try {
    const res = await softwareApi.getInstances()
    instances.value = res?.items || res || []
  } catch (e) {
    console.error('Fetch instances error:', e)
    ElMessage.error('获取实例列表失败')
    instances.value = []
  } finally {
    loading.value = false
  }
}

watch(() => props.visible, (val) => {
  if (val) {
    fetchInstances()
  }
})
</script>
