<template>
  <div class="page-container" v-loading="loading">
    <el-card shadow="never" class="detail-card">
      <template #header>
        <div class="detail-header">
          <div class="header-left">
            <el-button @click="$router.back()" :icon="ArrowLeft">返回</el-button>
            <span class="sparepart-name">{{ sparepart?.name || '备件详情' }}</span>
            <el-tag v-if="sparepart" :type="sparepart.stockQuantity > sparepart.minStock ? 'success' : 'danger'" effect="dark">
              {{ sparepart.stockQuantity > sparepart.minStock ? '库存正常' : '库存不足' }}
            </el-tag>
          </div>
        </div>
      </template>

      <el-tabs v-model="activeTab">
        <el-tab-pane label="基本信息" name="info">
          <el-descriptions :column="3" border>
            <el-descriptions-item label="编号">{{ sparepart?.code }}</el-descriptions-item>
            <el-descriptions-item label="备件名称">{{ sparepart?.name }}</el-descriptions-item>
            <el-descriptions-item label="分类">{{ sparepart?.category }}</el-descriptions-item>
            <el-descriptions-item label="规格型号">{{ sparepart?.specification || '-' }}</el-descriptions-item>
            <el-descriptions-item label="所属系统">{{ sparepart?.systemName || '-' }}</el-descriptions-item>
            <el-descriptions-item label="单位">{{ sparepart?.unit || '-' }}</el-descriptions-item>
            <el-descriptions-item label="当前库存">{{ sparepart?.stockQuantity }}</el-descriptions-item>
            <el-descriptions-item label="最低库存">{{ sparepart?.minStock }}</el-descriptions-item>
            <el-descriptions-item label="单价">¥{{ sparepart?.price }}</el-descriptions-item>
            <el-descriptions-item label="存放位置">{{ sparepart?.location || '-' }}</el-descriptions-item>
            <el-descriptions-item label="备注" :span="2">{{ sparepart?.remark || '-' }}</el-descriptions-item>
          </el-descriptions>
        </el-tab-pane>

        <el-tab-pane label="入库记录" name="in">
          <el-table :data="inRecords" stripe v-loading="recordsLoading">
            <el-table-column prop="id" label="记录号" width="80" />
            <el-table-column prop="quantity" label="数量" width="80" align="center" />
            <el-table-column prop="unitPrice" label="单价" width="100" align="right">
              <template #default="{ row }">¥{{ row.unitPrice }}</template>
            </el-table-column>
            <el-table-column prop="supplier" label="供应商" width="150" />
            <el-table-column prop="inDate" label="入库日期" width="120">
              <template #default="{ row }">{{ row.inDate?.substring(0, 10) }}</template>
            </el-table-column>
            <el-table-column prop="remark" label="备注" min-width="150" />
          </el-table>
          <el-empty v-if="!inRecords.length" description="暂无入库记录" />
        </el-tab-pane>

        <el-tab-pane label="出库记录" name="out">
          <el-table :data="outRecords" stripe v-loading="recordsLoading">
            <el-table-column prop="id" label="记录号" width="80" />
            <el-table-column prop="quantity" label="数量" width="80" align="center" />
            <el-table-column prop="department" label="领用部门" width="120" />
            <el-table-column prop="recipient" label="领用人" width="100" />
            <el-table-column prop="outDate" label="出库日期" width="120">
              <template #default="{ row }">{{ row.outDate?.substring(0, 10) }}</template>
            </el-table-column>
            <el-table-column prop="purpose" label="用途" min-width="150" />
          </el-table>
          <el-empty v-if="!outRecords.length" description="暂无出库记录" />
        </el-tab-pane>
      </el-tabs>
    </el-card>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useRoute } from 'vue-router'
import { ArrowLeft } from '@element-plus/icons-vue'
import { sparepartApi, stockApi } from '@/api'

const route = useRoute()
const loading = ref(true)
const recordsLoading = ref(false)
const sparepart = ref(null)
const activeTab = ref('info')
const inRecords = ref([])
const outRecords = ref([])

onMounted(async () => {
  const id = route.params.id
  loading.value = true
  try {
    sparepart.value = await sparepartApi.getById(id)
  } catch (e) {
    console.error('Fetch sparepart error:', e)
  } finally {
    loading.value = false
  }

  recordsLoading.value = true
  try {
    const [inRes, outRes] = await Promise.all([
      stockApi.getInList({ page: 1, pageSize: 999 }),
      stockApi.getOutList({ page: 1, pageSize: 999 })
    ])
    const allIn = inRes.items || inRes.data?.items || inRes.data || []
    const allOut = outRes.items || outRes.data?.items || outRes.data || []
    inRecords.value = allIn.filter(r => r.sparepartId == id || r.sparePartId == id)
    outRecords.value = allOut.filter(r => r.sparepartId == id || r.sparePartId == id)
  } catch (e) {
    console.error('Fetch stock records error:', e)
  } finally {
    recordsLoading.value = false
  }
})
</script>

<style scoped lang="scss">
.detail-header {
  display: flex; justify-content: space-between; align-items: center;
  .header-left { display: flex; align-items: center; gap: 12px; .sparepart-name { font-size: 18px; font-weight: 600; } }
}
</style>
