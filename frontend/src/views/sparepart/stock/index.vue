<template>
  <div class="page-container">
    <el-card shadow="never">
      <template #header>
        <div class="card-header">
          <span>出入库管理</span>
          <el-button type="primary" size="small" @click="handleAdd">新建出入库</el-button>
        </div>
      </template>
      <el-table :data="records" stripe v-loading="loading">
        <el-table-column prop="id" label="单号" width="130" />
        <el-table-column prop="type" label="类型" width="80">
          <template #default="{ row }">
            <el-tag :type="row.type === '入库' || row.type === 'in' ? 'success' : 'warning'" size="small">
              {{ row.type === 'in' ? '入库' : row.type }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="sparepartName" label="备件" min-width="150" />
        <el-table-column prop="quantity" label="数量" width="80" align="center" />
        <el-table-column prop="operatorName" label="经办人" width="90" />
        <el-table-column prop="date" label="日期" width="110" />
        <el-table-column prop="remark" label="备注" min-width="120" />
      </el-table>
    </el-card>

    <StockRecordDialog v-model:visible="dialogVisible" @success="refresh" />
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useSparepartStore } from '@/store/sparepart'
import StockRecordDialog from './StockRecordDialog.vue'

const sparepartStore = useSparepartStore()
const records = ref([])
const loading = ref(false)
const dialogVisible = ref(false)

const refresh = async () => {
  loading.value = true
  const [inRes, outRes] = await Promise.all([
    sparepartStore.fetchStockIn(),
    sparepartStore.fetchStockOut()
  ])
  const inList = (inRes.items || []).map(i => ({
    id: i.id,
    type: '入库',
    sparepartName: i.sparepart?.name || i.sparepartId,
    quantity: i.quantity,
    operatorName: i.operatorId,
    date: i.inDate?.substring(0, 10) || '',
    remark: i.remark || ''
  }))
  const outList = (outRes.items || []).map(i => ({
    id: i.id,
    type: '出库',
    sparepartName: i.sparepart?.name || i.sparepartId,
    quantity: i.quantity,
    operatorName: i.operatorId,
    date: i.outDate?.substring(0, 10) || '',
    remark: i.purpose || ''
  }))
  records.value = [...inList, ...outList].sort((a, b) => b.id - a.id)
  loading.value = false
}

onMounted(refresh)

const handleAdd = () => {
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
