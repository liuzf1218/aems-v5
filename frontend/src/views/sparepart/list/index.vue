<template>
  <div class="page-container">
    <el-card shadow="never">
      <template #header>
        <div class="card-header">
          <span>备件列表</span>
          <el-button type="primary" size="small" @click="handleAdd">新增备件</el-button>
        </div>
      </template>
      <el-form inline style="margin-bottom:16px">
        <el-form-item style="margin-bottom: 0">
          <el-input v-model="filters.keyword" placeholder="搜索备件名称" clearable @keyup.enter="fetchData" />
        </el-form-item>
        <el-form-item style="margin-bottom: 0">
          <el-select v-model="filters.category" placeholder="分类" clearable style="width:120px">
            <el-option label="电子元件" value="电子元件" />
            <el-option label="机械部件" value="机械部件" />
            <el-option label="电源" value="电源" />
            <el-option label="网络" value="网络" />
            <el-option label="耗材" value="耗材" />
          </el-select>
        </el-form-item>
        <el-form-item style="margin-bottom: 0">
          <el-select v-model="filters.systemId" placeholder="系统" clearable style="width:180px">
            <el-option v-for="sys in systems" :key="sys.id" :label="sys.name" :value="sys.id" />
          </el-select>
        </el-form-item>
        <el-form-item style="margin-bottom: 0">
          <el-button type="primary" @click="fetchData">搜索</el-button>
          <el-button @click="filters.keyword = ''; filters.category = ''; filters.systemId = ''; fetchData()">重置</el-button>
        </el-form-item>
      </el-form>
      <el-table :data="parts" stripe v-loading="loading">
        <el-table-column prop="code" label="编号" width="100" />
        <el-table-column prop="name" label="备件名称" min-width="140" />
        <el-table-column prop="specification" label="规格型号" width="120" />
        <el-table-column prop="category" label="分类" width="90" />
        <el-table-column prop="systemName" label="所属系统" width="120" />
        <el-table-column prop="stockQuantity" label="库存" width="80" align="center">
          <template #default="{ row }">
            <span :style="{ color: (row.stockQuantity || row.stock || 0) <= (row.minStockQuantity || row.minStock || 0) ? '#f56c6c' : '#67c23a' }">
              {{ row.stockQuantity || row.stock }}
            </span>
          </template>
        </el-table-column>
        <el-table-column prop="minStock" label="最低库存" width="85" align="center">
          <template #default="{ row }">{{ row.minStockQuantity || row.minStock }}</template>
        </el-table-column>
        <el-table-column prop="price" label="单价" width="85" align="right">
          <template #default="{ row }">¥{{ row.price }}</template>
        </el-table-column>
        <el-table-column prop="location" label="库位" width="100" />
        <el-table-column label="操作" width="140" align="center">
          <template #default="{ row }">
            <el-button type="primary" link size="small" @click="handleView(row)">查看</el-button>
          </template>
        </el-table-column>
      </el-table>
    </el-card>

    <SparepartDialog
      v-model:visible="spDialogVisible"
      :data="currentPart"
      :readonly="spReadonly"
      @success="fetchData"
    />

    <StockInOutDialog
      v-model:visible="ioDialogVisible"
      :sparepart="currentPart"
      @success="fetchData"
    />
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { sparepartApi, equipmentApi } from '@/api'
import SparepartDialog from './SparepartDialog.vue'
import StockInOutDialog from './StockInOutDialog.vue'

const parts = ref([])
const router = useRouter()
const loading = ref(false)
const systems = ref([])
const filters = ref({ keyword: '', category: '', systemId: '' })

const spDialogVisible = ref(false)
const spReadonly = ref(false)
const currentPart = ref(null)

const ioDialogVisible = ref(false)

const fetchData = async () => {
  loading.value = true
  try {
    const params = { ...filters.value }
    const res = await sparepartApi.getList(params)
    parts.value = res.items || []
  } catch (e) {
    console.error('Fetch sparepart list error:', e)
  } finally {
    loading.value = false
  }
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
  currentPart.value = null
  spReadonly.value = false
  spDialogVisible.value = true
}

const handleView = (row) => {
  router.push('/sparepart/detail/' + row.id)
}

const handleStock = (row) => {
  currentPart.value = row
  ioDialogVisible.value = true
}
</script>

<style scoped>
.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}
</style>
