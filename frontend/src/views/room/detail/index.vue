<template>
  <div class="page-container">
    <el-card shadow="never" v-loading="loading">
      <template #header>
        <div class="card-header">
          <span>机房详情</span>
          <el-button @click="goBack">返回</el-button>
        </div>
      </template>

      <el-tabs v-model="activeTab" type="border-card">
        <!-- 基本信息 -->
        <el-tab-pane label="基本信息" name="basic">
          <el-descriptions :column="3" border>
            <el-descriptions-item label="机房名称">{{ roomInfo.name }}</el-descriptions-item>
            <el-descriptions-item label="机房编码">{{ roomInfo.code }}</el-descriptions-item>
            <el-descriptions-item label="所属楼宇">{{ roomInfo.buildingName || '-' }}</el-descriptions-item>
            <el-descriptions-item label="楼层">{{ roomInfo.floor || '-' }}</el-descriptions-item>
            <el-descriptions-item label="面积">{{ roomInfo.area }} m²</el-descriptions-item>
            <el-descriptions-item label="温度阈值">{{ roomInfo.temperatureMin }}°C ~ {{ roomInfo.temperatureMax }}°C</el-descriptions-item>
            <el-descriptions-item label="湿度阈值">{{ roomInfo.humidityMin }}% ~ {{ roomInfo.humidityMax }}%</el-descriptions-item>
            <el-descriptions-item label="负责人">{{ roomInfo.manager }}</el-descriptions-item>
            <el-descriptions-item label="备注">{{ roomInfo.remark }}</el-descriptions-item>
          </el-descriptions>
        </el-tab-pane>

        <!-- 设备列表 -->
        <el-tab-pane label="设备列表" name="devices">
          <el-table :data="deviceList" stripe>
            <el-table-column prop="code" label="设备编号" width="130" />
            <el-table-column prop="name" label="设备名称" min-width="140" />
            <el-table-column prop="model" label="型号" width="130" />
            <el-table-column prop="cabinetName" label="所属机柜" width="120" />
            <el-table-column prop="systemName" label="所属系统" width="130" />
            <el-table-column prop="uPosition" label="U位" width="80" />
            <el-table-column prop="status" label="状态" width="90" align="center">
              <template #default="{ row }">
                <el-tag :type="row.status === 0 || row.status === 'ACTIVE' ? 'success' : 'danger'" size="small">
                  {{ row.status === 0 || row.status === 'ACTIVE' ? '在用' : '故障' }}
                </el-tag>
              </template>
            </el-table-column>
          </el-table>
          <el-empty v-if="!deviceList.length" description="暂无设备" />
        </el-tab-pane>

        <!-- 机柜管理 -->
        <el-tab-pane label="机柜管理" name="cabinets">
          <el-table :data="cabinetList" stripe>
            <el-table-column prop="name" label="名称" min-width="150" />
            <el-table-column prop="code" label="编号" width="120" />
            <el-table-column prop="uTotal" label="U位总数" width="100" />
            <el-table-column prop="uUsed" label="已用U位" width="100" />
            <el-table-column prop="deviceCount" label="设备数" width="90" />
          </el-table>
          <el-empty v-if="!cabinetList.length" description="暂无机柜" />
        </el-tab-pane>

        <!-- 附属设施 -->
        <el-tab-pane label="附属设施" name="facilities">
          <el-table :data="facilityList" stripe>
            <el-table-column prop="name" label="设施名称" min-width="150" />
            <el-table-column prop="type" label="类型" width="120" />
            <el-table-column prop="status" label="状态" width="100" align="center">
              <template #default="{ row }">
                <el-tag :type="row.status === '正常' ? 'success' : 'danger'" size="small">{{ row.status || '正常' }}</el-tag>
              </template>
            </el-table-column>
            <el-table-column prop="remark" label="备注" min-width="150" />
          </el-table>
          <el-empty v-if="!facilityList.length" description="暂无附属设施" />
        </el-tab-pane>


      </el-tabs>
    </el-card>
  </div>
</template>

<script setup>
import { ref, watch, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { roomApi } from '@/api'

const props = defineProps({
  roomId: {
    type: [String, Number],
    default: null
  }
})

const route = useRoute()
const router = useRouter()

const activeTab = ref('basic')
const loading = ref(false)

const roomInfo = ref({})
const deviceList = ref([])
const cabinetList = ref([])
const facilityList = ref([])


const currentId = ref(null)

const loadAll = async (id) => {
  if (!id) return
  currentId.value = id
  loading.value = true
  try {
    const [roomRes, devicesRes, cabinetsRes, facilitiesRes] = await Promise.all([
      roomApi.getById(id),
      roomApi.getDevices(id),
      roomApi.getCabinets(id),
      roomApi.getFacilities(id)
    ])
    roomInfo.value = roomRes || {}
    deviceList.value = Array.isArray(devicesRes) ? devicesRes : (devicesRes?.items || [])
    cabinetList.value = Array.isArray(cabinetsRes) ? cabinetsRes : (cabinetsRes?.items || [])
    facilityList.value = Array.isArray(facilitiesRes) ? facilitiesRes : (facilitiesRes?.items || [])

  } catch (e) {
    console.error('Load room detail error:', e)
  } finally {
    loading.value = false
  }
}

const goBack = () => {
  if (window.history.length > 1) {
    router.back()
  } else {
    router.push('/room/tree')
  }
}

onMounted(() => {
  const id = props.roomId || route.params.id
  if (id) {
    loadAll(id)
  }
})

watch(() => props.roomId, (val) => {
  if (val) loadAll(val)
})
</script>

<style scoped>
.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}
</style>
