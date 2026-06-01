<template>
  <div class="equipment-detail" v-loading="loading">
    <el-card shadow="never" class="detail-card">
      <template #header>
        <div class="detail-header">
          <div class="header-left">
            <el-button @click="$router.back()" :icon="ArrowLeft">返回</el-button>
            <span class="equipment-name">{{ equipment?.name || '设备详情' }}</span>
            <el-tag v-if="equipment" :type="getStatusType(equipment.status)" effect="dark">{{ getStatusLabel(equipment.status) }}</el-tag>
          </div>
          <div class="header-right">
            <el-button type="primary" @click="handleEdit"><el-icon><Edit /></el-icon>编辑</el-button>
            <el-button type="warning" @click="handleCreateWorkorder"><el-icon><Document /></el-icon>创建工单</el-button>
          </div>
        </div>
      </template>

      <el-tabs v-model="activeTab">
        <el-tab-pane label="基本信息" name="info">
          <el-descriptions :column="3" border>
            <el-descriptions-item label="设备编号">{{ equipment?.code }}</el-descriptions-item>
            <el-descriptions-item label="设备名称">{{ equipment?.name }}</el-descriptions-item>
            <el-descriptions-item label="型号">{{ equipment?.model }}</el-descriptions-item>
            <el-descriptions-item label="分类">{{ equipment?.categoryName }}</el-descriptions-item>
            <el-descriptions-item label="位置">{{ equipment?.location }}</el-descriptions-item>
            <el-descriptions-item label="重要性">{{ getCriticalityLabel(equipment?.criticality) }}</el-descriptions-item>
            <el-descriptions-item label="制造商">{{ equipment?.manufacturer }}</el-descriptions-item>
            <el-descriptions-item label="序列号">{{ equipment?.serialNumber }}</el-descriptions-item>
            <el-descriptions-item label="安装日期">{{ equipment?.installDate }}</el-descriptions-item>
            <el-descriptions-item label="保修到期">{{ equipment?.warrantyExpiry }}</el-descriptions-item>
            <el-descriptions-item label="运行时长">{{ equipment?.runtimeHours?.toLocaleString() }} 小时</el-descriptions-item>
            <el-descriptions-item label="故障次数">{{ equipment?.failureCount }} 次</el-descriptions-item>
            <el-descriptions-item label="上次维保">{{ equipment?.lastMaintenanceDate }}</el-descriptions-item>
            <el-descriptions-item label="下次维保">{{ equipment?.nextMaintenanceDate }}</el-descriptions-item>
          </el-descriptions>
        </el-tab-pane>

        <el-tab-pane label="维保记录" name="maintenance">
          <el-timeline>
            <el-timeline-item v-for="(record, index) in maintenanceRecords" :key="index" :timestamp="record.date" :type="record.type" placement="top">
              <el-card shadow="never">
                <h4>{{ record.title }}</h4>
                <p>{{ record.description }}</p>
                <div class="record-footer">
                  <span>处理人: {{ record.handler }}</span>
                  <span>费用: &#165;{{ record.cost }}</span>
                </div>
              </el-card>
            </el-timeline-item>
          </el-timeline>
        </el-tab-pane>

        <el-tab-pane label="关联工单" name="workorders">
          <el-table :data="relatedWorkorders" stripe>
            <el-table-column prop="id" label="工单号" width="150">
              <template #default="{ row }">
                <el-link type="primary" @click="$router.push('/workorder/detail/' + row.id)">{{ row.id }}</el-link>
              </template>
            </el-table-column>
            <el-table-column prop="title" label="标题" min-width="180" />
            <el-table-column prop="status" label="状态" width="90" align="center">
              <template #default="{ row }">
                <el-tag :type="getStatusType(row.status)" size="small">{{ getStatusLabel(row.status) }}</el-tag>
              </template>
            </el-table-column>
            <el-table-column prop="createdAt" label="创建时间" width="110" />
          </el-table>
        </el-tab-pane>

        <el-tab-pane label="监控数据" name="monitor">
          <el-row :gutter="16">
            <el-col :span="8">
              <div class="monitor-item">
                <div class="monitor-label">温度</div>
                <div class="monitor-value" :class="{ warning: tempValue > 35 }">{{ tempValue }}°C</div>
                <el-progress :percentage="tempValue" :color="tempValue > 35 ? '#f56c6c' : '#67c23a'" :show-text="false" />
              </div>
            </el-col>
            <el-col :span="8">
              <div class="monitor-item">
                <div class="monitor-label">湿度</div>
                <div class="monitor-value">{{ humidityValue }}%</div>
                <el-progress :percentage="humidityValue" color="#409eff" :show-text="false" />
              </div>
            </el-col>
            <el-col :span="8">
              <div class="monitor-item">
                <div class="monitor-label">负载</div>
                <div class="monitor-value">{{ loadValue }}%</div>
                <el-progress :percentage="loadValue" :color="loadValue > 80 ? '#f56c6c' : '#67c23a'" :show-text="false" />
              </div>
            </el-col>
          </el-row>
        </el-tab-pane>
      </el-tabs>
    </el-card>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { ArrowLeft, Edit, Document } from '@element-plus/icons-vue'
import { useEquipmentStore } from '@/store/equipment'
import { mapValue, EQUIPMENT_STATUS_MAP, CRITICALITY_MAP } from '@/utils/statusMap'
import { ElMessage } from 'element-plus'

const route = useRoute()
const router = useRouter()
const equipmentStore = useEquipmentStore()

const loading = ref(true)
const equipment = ref(null)
const activeTab = ref('info')
const tempValue = ref(32)
const humidityValue = ref(45)
const loadValue = ref(67)

const maintenanceRecords = [
  { date: '2026-03-15', title: '定期巡检', description: '设备运行正常，已完成常规检查', handler: '张三', cost: 0, type: 'success' },
  { date: '2026-02-20', title: '更换滤网', description: '空调滤网积尘严重，已更换', handler: '李四', cost: 200, type: 'warning' },
  { date: '2026-01-10', title: '软件升级', description: '固件升级至v3.2.1', handler: '王五', cost: 0, type: 'primary' }
]

const relatedWorkorders = [
  { id: 'WO-20260320-001', title: '温度传感器校准', status: 4, createdAt: '2026-03-20' },
  { id: 'WO-20260215-003', title: '通信模块故障维修', status: 5, createdAt: '2026-02-15' },
  { id: 'WO-20260110-005', title: '系统升级', status: 4, createdAt: '2026-01-10' }
]

const getStatusType = (v) => mapValue(EQUIPMENT_STATUS_MAP, v, 'type', 'info')
const getStatusLabel = (v) => mapValue(EQUIPMENT_STATUS_MAP, v, 'label', v)
const getCriticalityLabel = (v) => mapValue(CRITICALITY_MAP, v, 'label', v + '级')

const handleEdit = async () => {
  if (!equipment.value) return
  // 复用列表页的编辑逻辑：带数据跳转到列表页编辑
  router.push({ path: '/equipment/list', query: { editId: equipment.value.id } })
}
const handleCreateWorkorder = () => {
  router.push({ path: '/workorder/create', query: { equipmentId: equipment.value?.id } })
}

onMounted(async () => {
  const id = route.params.id
  equipment.value = await equipmentStore.getById(id)
  loading.value = false
})
</script>

<style scoped lang="scss">
.equipment-detail .detail-header {
  display: flex; justify-content: space-between; align-items: center;
  .header-left { display: flex; align-items: center; gap: 12px; .equipment-name { font-size: 18px; font-weight: 600; } }
  .header-right { display: flex; gap: 8px; }
}
.record-footer { display: flex; gap: 24px; font-size: 12px; color: #909399; margin-top: 8px; }
.monitor-item {
  text-align: center; padding: 20px; background: #f5f7fa; border-radius: 8px;
  .monitor-label { font-size: 13px; color: #909399; margin-bottom: 8px; }
  .monitor-value { font-size: 32px; font-weight: 700; margin-bottom: 12px; &.warning { color: #f56c6c; } }
}
</style>
