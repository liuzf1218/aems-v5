<template>
  <div class="equipment-list">
    <el-card shadow="never" class="search-card">
      <el-form :model="filters" inline class="search-form">
        <el-form-item label="关键词">
          <el-input v-model="filters.keyword" placeholder="设备名称/编号/型号" :prefix-icon="Search" clearable style="width: 200px" @keyup.enter="handleSearch" />
        </el-form-item>
        <el-form-item label="状态">
          <el-select v-model="filters.status" placeholder="全部" clearable style="width: 120px">
            <el-option label="在用" :value="0" /><el-option label="故障" :value="1" /><el-option label="维修中" :value="2" /><el-option label="备用" :value="3" /><el-option label="退役" :value="4" />
          </el-select>
        </el-form-item>
        <el-form-item label="分类">
          <el-select v-model="filters.category" placeholder="全部" clearable style="width: 140px">
            <el-option label="导航设备" :value="11" /><el-option label="通信设备" :value="12" /><el-option label="气象设备" :value="13" /><el-option label="监视设备" :value="14" /><el-option label="信息化设备" :value="15" />
          </el-select>
        </el-form-item>
        <el-form-item label="重要性">
          <el-select v-model="filters.criticality" placeholder="全部" clearable style="width: 100px">
            <el-option label="A级" :value="1" /><el-option label="B级" :value="2" /><el-option label="C级" :value="3" />
          </el-select>
        </el-form-item>
        <el-form-item label="系统">
          <el-select v-model="filters.subsystemId" placeholder="全部" clearable style="width: 160px">
            <el-option v-for="sys in systemOptions" :key="sys.id" :label="sys.name" :value="sys.id" />
          </el-select>
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="handleSearch"><el-icon><Search /></el-icon>搜索</el-button>
          <el-button @click="handleReset"><el-icon><Refresh /></el-icon>重置</el-button>
        </el-form-item>
      </el-form>
    </el-card>

    <el-card shadow="never" class="toolbar-card">
      <div class="toolbar">
        <div class="toolbar-left">
          <el-button type="primary" @click="handleCreate"><el-icon><Plus /></el-icon>新增设备</el-button>
          <el-button @click="handleExport"><el-icon><Download /></el-icon>导出</el-button>
          <el-button @click="handleImport"><el-icon><Upload /></el-icon>导入</el-button>
        </div>
        <div class="toolbar-right">
          <el-radio-group v-model="viewMode" size="small">
            <el-radio-button value="table"><el-icon><List /></el-icon></el-radio-button>
            <el-radio-button value="card"><el-icon><Grid /></el-icon></el-radio-button>
          </el-radio-group>
          <el-tooltip content="刷新"><el-button :icon="Refresh" circle size="small" @click="refresh" /></el-tooltip>
        </div>
      </div>
    </el-card>

    <el-card v-if="viewMode === 'table'" shadow="never" class="table-card">
      <el-table v-loading="loading" :data="data" stripe border size="default" @sort-change="handleSortChange" @selection-change="handleSelectionChange">
        <el-table-column type="selection" width="45" fixed />
        <el-table-column prop="code" label="设备编号" width="120" fixed sortable="custom">
          <template #default="{ row }"><el-link type="primary" @click="goDetail(row.id)">{{ row.code }}</el-link></template>
        </el-table-column>
        <el-table-column prop="name" label="设备名称" min-width="160" show-overflow-tooltip />
        <el-table-column prop="model" label="型号" width="150" show-overflow-tooltip>
          <template #default="{ row }">{{ row.model || '-' }}</template>
        </el-table-column>
        <el-table-column prop="systemName" label="所属系统" width="130" show-overflow-tooltip>
          <template #default="{ row }">{{ row.systemName || '-' }}</template>
        </el-table-column>
        <el-table-column prop="categoryName" label="分类" width="90">
          <template #default="{ row }"><el-tag size="small" :type="getCategoryType(row.categoryName)">{{ row.categoryName || row.equipmentType?.name || '-' }}</el-tag></template>
        </el-table-column>
        <el-table-column prop="location" label="位置" width="100">
          <template #default="{ row }">{{ row.location || row.position || '-' }}</template>
        </el-table-column>
        <el-table-column prop="status" label="状态" width="90" align="center">
          <template #default="{ row }"><el-tag :type="getStatusType(row.status)" size="small" effect="dark">{{ getStatusLabel(row.status) }}</el-tag></template>
        </el-table-column>
        <el-table-column prop="criticality" label="重要性" width="80" align="center">
          <template #default="{ row }"><el-tag :type="getCriticalityType(row.criticality)" size="small">{{ getCriticalityLabel(row.criticality) }}</el-tag></template>
        </el-table-column>
        <el-table-column prop="manufacturer" label="制造商" width="130" show-overflow-tooltip />
        <el-table-column prop="runtimeHours" label="运行时长(h)" width="110" align="right" sortable="custom">
          <template #default="{ row }">{{ row.runtimeHours?.toLocaleString() || 0 }}</template>
        </el-table-column>
        <el-table-column prop="lastMaintenanceDate" label="上次维保" width="110">
          <template #default="{ row }">{{ row.lastMaintenanceDate || '-' }}</template>
        </el-table-column>
        <el-table-column prop="nextMaintenanceDate" label="下次维保" width="110">
          <template #default="{ row }"><span :class="{ 'text-warning': isMaintenanceDue(row.nextMaintenanceDate) }">{{ row.nextMaintenanceDate || '-' }}</span></template>
        </el-table-column>
        <el-table-column label="操作" width="180" fixed="right" align="center">
          <template #default="{ row }">
            <el-button type="primary" link size="small" @click="goDetail(row.id)">查看</el-button>
            <el-button type="success" link size="small" @click="handleEdit(row)">编辑</el-button>
            <el-button type="warning" link size="small" @click="handleCreateWorkorder(row)">工单</el-button>
            <el-popconfirm title="确定删除此设备？" @confirm="handleDelete(row)"><template #reference><el-button type="danger" link size="small">删除</el-button></template></el-popconfirm>
          </template>
        </el-table-column>
      </el-table>
      <div class="pagination-wrapper">
        <el-pagination v-model:current-page="pagination.page" v-model:page-size="pagination.pageSize" :page-sizes="[20,50,100,200]" :total="pagination.total" layout="total, sizes, prev, pager, next, jumper" @size-change="handleSizeChange" @current-change="handlePageChange" />
      </div>
    </el-card>

    <div v-else class="card-view" v-loading="loading">
      <el-row :gutter="16">
        <el-col v-for="item in data" :key="item.id" :xs="24" :sm="12" :md="8" :lg="6">
          <el-card shadow="hover" class="equipment-card" @click="goDetail(item.id)">
            <div class="card-header">
              <el-tag :type="getStatusType(item.status)" size="small" effect="dark">{{ getStatusLabel(item.status) }}</el-tag>
              <el-tag :type="getCriticalityType(item.criticality)" size="small">{{ getCriticalityLabel(item.criticality) }}</el-tag>
            </div>
            <div class="card-body">
              <div class="equipment-name">{{ item.name }}</div>
              <div class="equipment-code">{{ item.code }}</div>
              <div class="equipment-info"><span>{{ item.location }}</span><span>{{ item.manufacturer }}</span></div>
            </div>
            <div class="card-footer">
              <span>运行: {{ item.runtimeHours }}h</span>
              <span>故障: {{ item.failureCount }}次</span>
            </div>
          </el-card>
        </el-col>
      </el-row>
      <div class="pagination-wrapper"><el-pagination v-model:current-page="pagination.page" v-model:page-size="pagination.pageSize" :total="pagination.total" layout="prev, pager, next" @current-change="handlePageChange" /></div>
    </div>

    <el-dialog v-model="dialogVisible" :title="isEdit ? '编辑设备' : '新增设备'" width="700px" destroy-on-close>
      <el-form ref="formRef" :model="formData" :rules="formRules" label-width="100px">
        <el-row :gutter="20">
          <el-col :span="12"><el-form-item label="设备编号" prop="code"><el-input v-model="formData.code" placeholder="自动生成或手动输入" /></el-form-item></el-col>
          <el-col :span="12"><el-form-item label="设备名称" prop="name"><el-input v-model="formData.name" placeholder="请输入设备名称" /></el-form-item></el-col>
          <el-col :span="12"><el-form-item label="型号" prop="model"><el-input v-model="formData.model" placeholder="请输入设备型号" /></el-form-item></el-col>
          <el-col :span="12"><el-form-item label="分类" prop="category">
            <el-select v-model="formData.category" placeholder="请选择" style="width:100%">
              <el-option label="导航设备" :value="11" /><el-option label="通信设备" :value="12" /><el-option label="气象设备" :value="13" /><el-option label="监视设备" :value="14" /><el-option label="信息化设备" :value="15" />
            </el-select>
          </el-form-item></el-col>
          <el-col :span="12"><el-form-item label="所属系统" prop="subsystemId">
            <el-select v-model="formData.subsystemId" placeholder="请选择" style="width:100%">
              <el-option v-for="sys in systemOptions" :key="sys.id" :label="sys.name" :value="sys.id" />
            </el-select>
          </el-form-item></el-col>
          <el-col :span="12"><el-form-item label="位置" prop="location"><el-input v-model="formData.location" placeholder="请输入安装位置" /></el-form-item></el-col>
          <el-col :span="12"><el-form-item label="重要性" prop="criticality">
            <el-select v-model="formData.criticality" style="width:100%">
              <el-option label="A级 - 关键设备" :value="1" /><el-option label="B级 - 重要设备" :value="2" /><el-option label="C级 - 一般设备" :value="3" />
            </el-select>
          </el-form-item></el-col>
          <el-col :span="12"><el-form-item label="制造商" prop="manufacturer"><el-input v-model="formData.manufacturer" placeholder="请输入制造商" /></el-form-item></el-col>
          <el-col :span="12"><el-form-item label="序列号" prop="serialNumber"><el-input v-model="formData.serialNumber" placeholder="请输入序列号" /></el-form-item></el-col>
          <el-col :span="12"><el-form-item label="安装日期"><el-date-picker v-model="formData.installDate" type="date" style="width:100%" /></el-form-item></el-col>
          <el-col :span="12"><el-form-item label="保修到期"><el-date-picker v-model="formData.warrantyExpiry" type="date" style="width:100%" /></el-form-item></el-col>
        </el-row>
      </el-form>
      <template #footer><el-button @click="dialogVisible = false">取消</el-button><el-button type="primary" @click="handleSubmit">确定</el-button></template>
    </el-dialog>
  </div>
</template>

<script setup>
import { ref, reactive, onMounted } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { ElMessage } from 'element-plus'
import { Search, Refresh, Plus, Download, Upload, List, Grid } from '@element-plus/icons-vue'
import { useEquipmentStore } from '@/store/equipment'
import { useTable } from '@/composables/useTable'
import { mapValue, EQUIPMENT_STATUS_MAP, CATEGORY_MAP, CRITICALITY_MAP } from '@/utils/statusMap'
import { equipmentApi } from '@/api'

const router = useRouter()
const route = useRoute()
const equipmentStore = useEquipmentStore()

const viewMode = ref('table')
const dialogVisible = ref(false)
const isEdit = ref(false)
const formRef = ref(null)
const selectedRows = ref([])

const filters = reactive({ keyword: route.query.keyword || '', status: '', category: '', criticality: '', subsystemId: '' })
const formData = reactive({ id: '', code: '', name: '', model: '', category: '', location: '', criticality: 2, manufacturer: '', serialNumber: '', installDate: '', warrantyExpiry: '', subsystemId: '' })
const systemOptions = ref([])

const formRules = {
  code: [{ required: true, message: '请输入设备编号', trigger: 'blur' }],
  name: [{ required: true, message: '请输入设备名称', trigger: 'blur' }],
  category: [{ required: true, message: '请选择分类', trigger: 'change' }],
  location: [{ required: true, message: '请输入安装位置', trigger: 'blur' }],
  subsystemId: [{ required: true, message: '请选择所属系统', trigger: 'change' }]
}

const { loading, data, pagination, fetchData, handlePageChange, handleSizeChange, handleSearch: search, handleReset: reset, refresh } = useTable(
  (params) => equipmentStore.fetchList(params), { pageSize: 20 }
)

const handleSearch = () => search(filters)
const handleReset = () => { Object.keys(filters).forEach(key => filters[key] = ''); reset() }
const getStatusType = (v) => mapValue(EQUIPMENT_STATUS_MAP, v, 'type', 'info')
const getStatusLabel = (v) => mapValue(EQUIPMENT_STATUS_MAP, v, 'label', v)
const getCategoryType = (v) => mapValue(CATEGORY_MAP, v, 'type', '')
const getCriticalityLabel = (v) => mapValue(CRITICALITY_MAP, v, 'label', v + '\u7ea7')
const getCriticalityType = (v) => mapValue(CRITICALITY_MAP, v, 'type', 'info')
const isMaintenanceDue = (date) => { if (!date) return false; const diff = (new Date(date) - new Date()) / 86400000; return diff <= 7 && diff >= 0 }
const goDetail = (id) => router.push('/equipment/detail/' + id)
const handleCreate = () => { isEdit.value = false; Object.keys(formData).forEach(key => formData[key] = ''); formData.criticality = 2; dialogVisible.value = true }
const loadSystems = async () => {
  try {
    const res = await equipmentApi.getTree()
    const list = Array.isArray(res) ? res : (res || [])
    const systems = []
    const walk = (nodes) => {
      nodes.forEach(node => {
        if (node.nodeType === 'system' || node.nodeType === 'subsystem' || !node.nodeType) {
          systems.push({ id: node.id, name: node.name })
        }
        if (node.children && node.children.length) walk(node.children)
      })
    }
    walk(list)
    systemOptions.value = systems
  } catch (e) {
    console.error('Load systems error:', e)
  }
}
const handleEdit = (row) => { isEdit.value = true; Object.assign(formData, row); dialogVisible.value = true }
const handleSubmit = async () => {
  const valid = await formRef.value?.validate().catch(() => false)
  if (!valid) return
  if (isEdit.value) { await equipmentStore.update(formData.id, { ...formData }) } else { await equipmentStore.create({ ...formData }) }
  ElMessage.success(isEdit.value ? '更新成功' : '创建成功')
  dialogVisible.value = false; refresh()
}
const handleDelete = async (row) => { await equipmentStore.remove(row.id); ElMessage.success('删除成功'); refresh() }
const handleCreateWorkorder = (row) => router.push({ path: '/workorder/create', query: { equipmentId: row.id } })
const handleExport = () => ElMessage.info('导出功能开发中')
const handleImport = () => ElMessage.info('导入功能开发中')
const handleSortChange = ({ prop, order }) => console.log('Sort:', prop, order)
const handleSelectionChange = (rows) => { selectedRows.value = rows }

onMounted(async () => {
  fetchData()
  await loadSystems()
  // 从详情页跳转过来带 editId 时自动打开编辑
  const editId = route.query.editId
  if (editId) {
    try {
      const row = await equipmentStore.getById(editId)
      if (row) handleEdit(row)
    } catch (e) { console.error('Auto edit error:', e) }
  }
})
</script>

<style scoped lang="scss">
.equipment-list { display: flex; flex-direction: column; gap: 16px; }
.search-card .search-form .el-form-item { margin-bottom: 0; }
.toolbar-card .toolbar { display: flex; justify-content: space-between; align-items: center; .toolbar-left { display: flex; gap: 8px; } .toolbar-right { display: flex; align-items: center; gap: 12px; } }
.table-card :deep(.el-table) { font-size: 13px; }
.pagination-wrapper { display: flex; justify-content: flex-end; margin-top: 16px; }
.text-warning { color: #e6a23c; font-weight: 500; }
.card-view .equipment-card {
  margin-bottom: 16px; cursor: pointer; transition: all 0.2s;
  &:hover { transform: translateY(-2px); box-shadow: 0 4px 12px rgba(0,0,0,.12); }
  .card-header { display: flex; justify-content: space-between; margin-bottom: 12px; }
  .card-body { .equipment-name { font-size: 15px; font-weight: 600; color: #303133; margin-bottom: 4px; } .equipment-code { font-size: 12px; color: #909399; margin-bottom: 12px; } .equipment-info { display: flex; flex-direction: column; gap: 4px; font-size: 12px; color: #606266; } }
  .card-footer { margin-top: 12px; padding-top: 12px; border-top: 1px solid #f0f0f0; display: flex; justify-content: space-between; font-size: 12px; color: #909399; }
}
</style>
