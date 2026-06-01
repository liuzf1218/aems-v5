<template>
  <div class="page-container">
    <el-card shadow="never">
      <template #header><div class="card-header"><span>文档列表</span><el-button type="primary" size="small">上传文档</el-button></div></template>
      <el-form inline style="margin-bottom:16px">
        <el-form-item><el-input placeholder="搜索文档" clearable /></el-form-item>
        <el-form-item>
          <el-select placeholder="分类" clearable style="width:120px">
            <el-option label="技术文档" value="技术" />
            <el-option label="操作手册" value="操作" />
          </el-select>
        </el-form-item>
        <el-form-item>
          <el-select v-model="filters.systemId" placeholder="系统" clearable style="width:180px">
            <el-option v-for="sys in systems" :key="sys.id" :label="sys.name" :value="sys.id" />
          </el-select>
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="fetchData">搜索</el-button>
          <el-button @click="filters.systemId = ''; fetchData()">重置</el-button>
        </el-form-item>
      </el-form>
      <el-table :data="documents" stripe v-loading="loading">
        <el-table-column prop="docNo" label="编号" width="100" />
        <el-table-column prop="name" label="文档名称" min-width="200" />
        <el-table-column prop="category" label="分类" width="100" />
        <el-table-column prop="systemName" label="所属系统" width="140" />
        <el-table-column prop="currentVersion" label="版本" width="80" />
        <el-table-column prop="updatedAt" label="更新时间" width="160" />
        <el-table-column label="操作" width="120" align="center">
          <template #default="{ row }"><el-button type="primary" link size="small" @click="handleView(row)">查看</el-button><el-button type="success" link size="small">下载</el-button></template>
        </el-table-column>
      </el-table>
    </el-card>

    <el-dialog title="文档详情" v-model="detailVisible" width="600px">
      <el-descriptions :column="2" border v-if="currentDoc">
        <el-descriptions-item label="文档编号">{{ currentDoc.docNo }}</el-descriptions-item>
        <el-descriptions-item label="文档名称">{{ currentDoc.name }}</el-descriptions-item>
        <el-descriptions-item label="分类">{{ currentDoc.category }}</el-descriptions-item>
        <el-descriptions-item label="所属系统">{{ currentDoc.systemName || '-' }}</el-descriptions-item>
        <el-descriptions-item label="当前版本">{{ currentDoc.currentVersion }}</el-descriptions-item>
        <el-descriptions-item label="更新时间">{{ currentDoc.updatedAt?.substring(0, 10) }}</el-descriptions-item>
        <el-descriptions-item label="备注" :span="2">{{ currentDoc.remark || '-' }}</el-descriptions-item>
      </el-descriptions>
      <template #footer>
        <el-button @click="detailVisible = false">关闭</el-button>
      </template>
    </el-dialog>
  </div>
</template>
<script setup>
import { ref, onMounted } from 'vue'
import { useDocumentStore } from '@/store/document'
import { equipmentApi } from '@/api'

const currentDoc = ref(null)
const detailVisible = ref(false)

const handleView = (row) => {
  currentDoc.value = row
  detailVisible.value = true
}

const documentStore = useDocumentStore()
const documents = ref([])
const loading = ref(false)
const systems = ref([])
const filters = ref({ systemId: '' })

const fetchData = async () => {
  loading.value = true
  const params = {}
  if (filters.value.systemId) params.systemId = filters.value.systemId
  const res = await documentStore.fetchList(params)
  documents.value = res.items || []
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
</script>
<style scoped>.card-header{display:flex;justify-content:space-between;align-items:center}</style>
