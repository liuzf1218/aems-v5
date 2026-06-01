<template>
  <div class="page-container">
    <el-row :gutter="16">
      <el-col :span="6">
        <el-card shadow="never">
          <template #header>
            <div class="card-header">
              <span>设备分类</span>
              <el-button type="primary" link size="small" @click="loadTree">刷新</el-button>
            </div>
          </template>
          <el-input v-model="filterText" placeholder="搜索分类/系统" clearable style="margin-bottom:8px" />
          <el-tree
            ref="treeRef"
            :data="treeData"
            :props="{ label: 'name', children: 'children' }"
            :filter-node-method="filterNode"
            highlight-current
            default-expand-all
            @node-click="handleNodeClick"
          />
        </el-card>
      </el-col>
      <el-col :span="18">
        <el-card shadow="never">
          <template #header>
            <div class="card-header">
              <span>{{ selectedNode?.name || '设备列表' }}</span>
              <el-button type="primary" size="small" @click="handleAdd">添加设备</el-button>
            </div>
          </template>
          <el-table :data="deviceList" stripe v-loading="loading">
            <el-table-column prop="code" label="编号" width="110" />
            <el-table-column prop="name" label="设备名称" min-width="140" />
            <el-table-column prop="systemName" label="所属系统" width="120" />
            <el-table-column prop="manufacturer" label="厂商" width="110" />
            <el-table-column prop="ipAddress" label="IP地址" width="120" />
            <el-table-column prop="status" label="状态" width="90" align="center">
              <template #default="{ row }">
                <el-tag :type="row.status === 0 || row.status === 'ACTIVE' ? 'success' : 'danger'" size="small">
                  {{ row.status === 0 || row.status === 'ACTIVE' ? '在用' : '故障' }}
                </el-tag>
              </template>
            </el-table-column>
            <el-table-column label="操作" width="120" align="center">
              <template #default="{ row }">
                <el-button type="primary" link size="small" @click="$router.push(`/equipment/detail/${row.id}`)">查看</el-button>
              </template>
            </el-table-column>
          </el-table>
        </el-card>
      </el-col>
    </el-row>
  </div>
</template>

<script setup>
import { ref, onMounted, watch } from 'vue'
import { useRouter } from 'vue-router'
import { ElMessage } from 'element-plus'
import { equipmentApi } from '@/api'

const router = useRouter()

const treeRef = ref(null)
const treeData = ref([])
const filterText = ref('')
const selectedNode = ref(null)
const deviceList = ref([])
const loading = ref(false)

const loadTree = async () => {
  try {
    const res = await equipmentApi.getTree()
    treeData.value = Array.isArray(res) ? res : (res || [])
  } catch (e) {
    console.error('Load tree error:', e)
  }
}

onMounted(loadTree)

watch(filterText, (val) => {
  treeRef.value?.filter(val)
})

function filterNode(value, data) {
  if (!value) return true
  return data.name.includes(value)
}

async function handleNodeClick(node) {
  selectedNode.value = node
  loading.value = true
  try {
    const params = { page: 1, pageSize: 999 }
    if (node.nodeType === 'system') {
      params.subsystemId = node.id
    } else if (node.nodeType === 'category') {
      params.categoryId = node.id
    }
    const res = await equipmentApi.getList(params)
    // 响应拦截器已解包，直接使用 res
    const data = res.items || []
    deviceList.value = Array.isArray(data) ? data : []
  } catch (e) {
    console.error('Fetch devices error:', e)
  } finally {
    loading.value = false
  }
}

function handleAdd() {
  if (!selectedNode.value) {
    ElMessage.warning('请先选择左侧分类或系统')
    return
  }
  router.push({ path: '/equipment/list', query: { categoryId: selectedNode.value.id } })
}
</script>

<style scoped>
.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}
</style>
