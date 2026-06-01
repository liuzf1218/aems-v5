<template>
  <div class="page-container">
    <el-card shadow="never">
      <template #header>
        <div class="card-header">
          <span>软件列表</span>
          <el-button type="primary" size="small" @click="handleAdd">新增软件</el-button>
        </div>
      </template>
      <el-form inline style="margin-bottom:16px">
        <el-form-item style="margin-bottom: 0">
          <el-input v-model="filters.keyword" placeholder="搜索软件名称/编号" clearable @keyup.enter="fetchData" style="width:200px" />
        </el-form-item>
        <el-form-item style="margin-bottom: 0">
          <el-select v-model="filters.systemId" placeholder="系统" clearable style="width:180px">
            <el-option v-for="sys in systems" :key="sys.id" :label="sys.name" :value="sys.id" />
          </el-select>
        </el-form-item>
        <el-form-item style="margin-bottom: 0">
          <el-button type="primary" @click="fetchData">搜索</el-button>
          <el-button @click="filters.keyword = ''; filters.systemId = ''; fetchData()">重置</el-button>
        </el-form-item>
      </el-form>
      <el-table :data="softwares" stripe v-loading="loading">
        <el-table-column prop="code" label="编号" width="100" />
        <el-table-column prop="name" label="软件名称" min-width="150" />
        <el-table-column prop="softwareType" label="类型" width="90" />
        <el-table-column prop="systemName" label="所属系统" width="120" />
        <el-table-column prop="vendor" label="厂商" width="110" />
        <el-table-column prop="licenseType" label="授权类型" width="100" />
        <el-table-column prop="remark" label="备注" min-width="120" show-overflow-tooltip />
        <el-table-column label="操作" width="120" align="center">
          <template #default="{ row }">
            <el-button type="primary" link size="small" @click="handleView(row)">查看</el-button>
            <el-button type="success" link size="small" @click="handleInstance(row)">实例</el-button>
          </template>
        </el-table-column>
      </el-table>
    </el-card>

    <SoftwareDialog
      v-model:visible="softwareDialogVisible"
      :data="currentSoftware"
      :readonly="softwareDialogReadonly"
      @success="handleSuccess"
    />

    <InstanceDialog
      v-model:visible="instanceDialogVisible"
      :software-id="currentSoftwareId"
    />
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useSoftwareStore } from '@/store/software'
import { equipmentApi } from '@/api'
import SoftwareDialog from './SoftwareDialog.vue'
import InstanceDialog from './InstanceDialog.vue'

const softwareStore = useSoftwareStore()
const softwares = ref([])
const router = useRouter()
const loading = ref(false)
const systems = ref([])
const filters = ref({ keyword: '', systemId: '' })

const softwareDialogVisible = ref(false)
const softwareDialogReadonly = ref(false)
const currentSoftware = ref(null)

const instanceDialogVisible = ref(false)
const currentSoftwareId = ref(null)

const fetchData = async () => {
  loading.value = true
  const params = {}
  if (filters.value.keyword) params.keyword = filters.value.keyword
  if (filters.value.systemId) params.systemId = filters.value.systemId
  const res = await softwareStore.fetchList(params)
  softwares.value = res.items || []
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
  fetchData()
  loadSystems()
})

const handleAdd = () => {
  currentSoftware.value = null
  softwareDialogReadonly.value = false
  softwareDialogVisible.value = true
}

const handleView = (row) => {
  router.push('/software/detail/' + row.id)
}

const handleInstance = (row) => {
  currentSoftwareId.value = row.id
  instanceDialogVisible.value = true
}

const handleSuccess = () => {
  fetchData()
}
</script>

<style scoped>
.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}
</style>
