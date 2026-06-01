<template>
  <div class="page-container" v-loading="loading">
    <el-card shadow="never" class="detail-card">
      <template #header>
        <div class="detail-header">
          <div class="header-left">
            <el-button @click="$router.back()" :icon="ArrowLeft">返回</el-button>
            <span class="software-name">{{ software?.name || '软件详情' }}</span>
          </div>
        </div>
      </template>

      <el-tabs v-model="activeTab">
        <el-tab-pane label="基本信息" name="info">
          <el-descriptions :column="3" border>
            <el-descriptions-item label="编号">{{ software?.code }}</el-descriptions-item>
            <el-descriptions-item label="软件名称">{{ software?.name }}</el-descriptions-item>
            <el-descriptions-item label="类型">{{ software?.softwareType }}</el-descriptions-item>
            <el-descriptions-item label="厂商">{{ software?.vendor || '-' }}</el-descriptions-item>
            <el-descriptions-item label="授权类型">{{ software?.licenseType || '-' }}</el-descriptions-item>
            <el-descriptions-item label="所属系统">{{ software?.systemName || '-' }}</el-descriptions-item>
            <el-descriptions-item label="备注" :span="3">{{ software?.remark || '-' }}</el-descriptions-item>
          </el-descriptions>
        </el-tab-pane>

        <el-tab-pane label="版本记录" name="versions">
          <el-table :data="versions" stripe v-loading="versionsLoading">
            <el-table-column prop="version" label="版本号" width="120" />
            <el-table-column prop="releaseDate" label="发布日期" width="120">
              <template #default="{ row }">{{ row.releaseDate?.substring(0, 10) }}</template>
            </el-table-column>
            <el-table-column prop="changeLog" label="变更说明" min-width="200" />
            <el-table-column prop="packagePath" label="包路径" min-width="150" show-overflow-tooltip />
          </el-table>
          <el-empty v-if="!versions.length" description="暂无版本记录" />
        </el-tab-pane>

        <el-tab-pane label="部署实例" name="instances">
          <el-table :data="instances" stripe v-loading="instancesLoading">
            <el-table-column prop="id" label="实例ID" width="80" />
            <el-table-column prop="equipmentName" label="部署设备" min-width="150" />
            <el-table-column prop="systemName" label="所属系统" width="130" />
            <el-table-column prop="installPath" label="安装路径" min-width="180" show-overflow-tooltip />
            <el-table-column prop="installDate" label="安装日期" width="110">
              <template #default="{ row }">{{ row.installDate?.substring(0, 10) }}</template>
            </el-table-column>
            <el-table-column prop="status" label="状态" width="90" align="center">
              <template #default="{ row }">
                <el-tag :type="row.status === 0 ? 'success' : 'danger'" size="small">
                  {{ row.status === 0 ? '正常' : '异常' }}
                </el-tag>
              </template>
            </el-table-column>
          </el-table>
          <el-empty v-if="!instances.length" description="暂无部署实例" />
        </el-tab-pane>
      </el-tabs>
    </el-card>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useRoute } from 'vue-router'
import { ArrowLeft } from '@element-plus/icons-vue'
import { softwareApi } from '@/api'

const route = useRoute()
const loading = ref(true)
const versionsLoading = ref(false)
const instancesLoading = ref(false)
const software = ref(null)
const activeTab = ref('info')
const versions = ref([])
const instances = ref([])

onMounted(async () => {
  const id = route.params.id
  loading.value = true
  try {
    software.value = await softwareApi.getById(id)
  } catch (e) {
    console.error('Fetch software error:', e)
  } finally {
    loading.value = false
  }

  versionsLoading.value = true
  try {
    const res = await softwareApi.getVersions(id)
    versions.value = res.items || res.data?.items || res.data || []
  } catch (e) {
    console.error('Fetch versions error:', e)
  } finally {
    versionsLoading.value = false
  }

  instancesLoading.value = true
  try {
    const res = await softwareApi.getInstances()
    const all = res.items || res.data?.items || res.data || []
    instances.value = all.filter(i => i.softwareId == id || i.id == id)
  } catch (e) {
    console.error('Fetch instances error:', e)
  } finally {
    instancesLoading.value = false
  }
})
</script>

<style scoped lang="scss">
.detail-header {
  display: flex; justify-content: space-between; align-items: center;
  .header-left { display: flex; align-items: center; gap: 12px; .software-name { font-size: 18px; font-weight: 600; } }
}
</style>
