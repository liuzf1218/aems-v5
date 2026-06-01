<template>
  <div class="page-container">
    <el-card shadow="never">
      <template #header><span>库存预警</span></template>
      <el-alert title="以下备件库存低于最低库存阈值，请及时补充" type="warning" show-icon :closable="false" style="margin-bottom:16px" />
      <el-table :data="warnings" stripe v-loading="loading">
        <el-table-column prop="code" label="编号" width="100" />
        <el-table-column prop="name" label="备件名称" min-width="150" />
        <el-table-column prop="stockQuantity" label="当前库存" width="100" align="center">
          <template #default="{ row }"><span style="color:#f56c6c;font-weight:600">{{ row.stockQuantity }}</span></template>
        </el-table-column>
        <el-table-column prop="minStock" label="最低库存" width="100" align="center" />
        <el-table-column prop="shortage" label="缺口" width="80" align="center">
          <template #default="{ row }"><span style="color:#f56c6c">{{ row.minStock - row.stockQuantity }}</span></template>
        </el-table-column>
      </el-table>
    </el-card>
  </div>
</template>
<script setup>
import { ref, onMounted, computed } from 'vue'
import { useSparepartStore } from '@/store/sparepart'

const sparepartStore = useSparepartStore()
const loading = ref(false)

const warnings = computed(() => sparepartStore.warnings)

onMounted(async () => {
  loading.value = true
  await sparepartStore.fetchWarnings()
  loading.value = false
})
</script>
