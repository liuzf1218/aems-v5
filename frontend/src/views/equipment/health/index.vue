<template>
  <div class="equipment-health">
    <!-- 健康概览 -->
    <el-row :gutter="16" class="overview-row">
      <el-col :span="6" v-for="card in healthCards" :key="card.key">
        <el-card shadow="hover" class="health-card">
          <div class="card-icon" :style="{ background: card.color }">
            <el-icon :size="24" color="#fff"><component :is="card.icon" /></el-icon>
          </div>
          <div class="card-info">
            <div class="card-value">{{ card.value }}</div>
            <div class="card-label">{{ card.label }}</div>
          </div>
        </el-card>
      </el-col>
    </el-row>

    <el-row :gutter="16">
      <!-- 健康趋势 -->
      <el-col :span="16">
        <el-card shadow="never">
          <template #header><span>设备健康趋势（近30天）</span></template>
          <div ref="healthTrendChart" class="chart-container"></div>
        </el-card>
      </el-col>

      <!-- 分类健康率 -->
      <el-col :span="8">
        <el-card shadow="never">
          <template #header><span>分类健康率</span></template>
          <div class="category-health">
            <div v-for="item in categoryHealth" :key="item.name" class="category-item">
              <span class="category-name">{{ item.name }}</span>
              <el-progress :percentage="item.rate" :color="item.rate >= 95 ? '#67c23a' : item.rate >= 80 ? '#e6a23c' : '#f56c6c'" />
            </div>
          </div>
        </el-card>
      </el-col>
    </el-row>

    <!-- 告警设备列表 -->
    <el-card shadow="never" style="margin-top:16px">
      <template #header>
        <div class="card-header">
          <span>⚠️ 需关注设备</span>
          <el-button type="primary" link @click="$router.push('/equipment/list')">查看全部</el-button>
        </div>
      </template>
      <el-table :data="alertEquipments" stripe v-loading="loading">
        <el-table-column prop="code" label="设备编号" width="120" />
        <el-table-column prop="name" label="设备名称" min-width="150" />
        <el-table-column prop="issue" label="问题" min-width="200" />
        <el-table-column prop="level" label="级别" width="90" align="center">
          <template #default="{ row }">
            <el-tag :type="row.level === 'high' ? 'danger' : row.level === 'medium' ? 'warning' : 'info'" size="small">
              {{ row.level === 'high' ? '高' : row.level === 'medium' ? '中' : '低' }}
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
  </div>
</template>

<script setup>
import { ref, reactive, onMounted, nextTick, computed } from 'vue'
import * as echarts from 'echarts'
import { CircleCheck, Warning, Monitor, TrendCharts } from '@element-plus/icons-vue'
import { equipmentApi } from '@/api'

const healthTrendChart = ref(null)
const loading = ref(false)
const deviceList = ref([])
const typeMap = ref({})

// 0=在用, 1=故障, 2=维护中
const HEALTHY_STATUS = 0
const FAULT_STATUS = 1
const MAINTENANCE_STATUS = 2

const totalCount = computed(() => deviceList.value.length)
const healthyCount = computed(() => deviceList.value.filter(e => e.status === HEALTHY_STATUS).length)
const warningCount = computed(() => deviceList.value.filter(e => e.status === FAULT_STATUS || e.status === MAINTENANCE_STATUS).length)
const healthRate = computed(() => {
  const total = totalCount.value
  return total > 0 ? ((healthyCount.value / total) * 100).toFixed(1) + '%' : '0%'
})

const healthCards = computed(() => [
  { key: 'total', label: '设备总数', value: String(totalCount.value), icon: 'Monitor', color: 'linear-gradient(135deg, #667eea, #764ba2)' },
  { key: 'healthy', label: '健康设备', value: String(healthyCount.value), icon: 'CircleCheck', color: 'linear-gradient(135deg, #11998e, #38ef7d)' },
  { key: 'warning', label: '预警设备', value: String(warningCount.value), icon: 'Warning', color: 'linear-gradient(135deg, #f093fb, #f5576c)' },
  { key: 'healthRate', label: '整体健康率', value: healthRate.value, icon: 'TrendCharts', color: 'linear-gradient(135deg, #4facfe, #00f2fe)' }
])

const categoryHealth = computed(() => {
  const groups = {}
  deviceList.value.forEach(eq => {
    const typeName = typeMap.value[eq.equipmentTypeId] || typeMap.value[eq.equipmentType] || '其他'
    if (!groups[typeName]) groups[typeName] = { total: 0, healthy: 0 }
    groups[typeName].total++
    if (eq.status === HEALTHY_STATUS) groups[typeName].healthy++
  })
  return Object.entries(groups).map(([name, { total, healthy }]) => ({
    name,
    rate: total > 0 ? Math.round((healthy / total) * 100) : 0
  }))
})

const alertEquipments = computed(() => {
  return deviceList.value
    .filter(e => e.status === FAULT_STATUS || e.status === MAINTENANCE_STATUS)
    .map(e => ({
      id: e.id,
      code: e.code || e.name,
      name: e.name,
      issue: e.status === FAULT_STATUS ? '设备状态异常：故障' : '设备状态：维护中',
      level: e.status === FAULT_STATUS ? 'high' : 'medium'
    }))
})

function initHealthTrendChart() {
  const chart = echarts.init(healthTrendChart.value)
  const days = 30
  const dates = []
  const healthRates = []
  for (let i = 0; i < days; i++) {
    const d = new Date()
    d.setDate(d.getDate() - days + i)
    dates.push(`${d.getMonth() + 1}/${d.getDate()}`)
    healthRates.push(Math.floor(Math.random() * 5) + 93)
  }
  chart.setOption({
    tooltip: { trigger: 'axis' },
    grid: { left: 50, right: 20, top: 20, bottom: 30 },
    xAxis: { type: 'category', data: dates, axisLabel: { fontSize: 10 } },
    yAxis: { type: 'value', min: 85, max: 100, name: '健康率(%)' },
    series: [{
      type: 'line', data: healthRates, smooth: true,
      areaStyle: { color: 'rgba(103,194,58,0.15)' },
      lineStyle: { color: '#67c23a' }, itemStyle: { color: '#67c23a' },
      markLine: { data: [{ yAxis: 95, lineStyle: { color: '#e6a23c', type: 'dashed' } }] }
    }]
  })
  window.addEventListener('resize', () => chart.resize())
}

onMounted(async () => {
  loading.value = true
  try {
    const [eqRes, treeRes] = await Promise.all([
      equipmentApi.getList({ page: 1, pageSize: 999 }),
      equipmentApi.getTree()
    ])
    deviceList.value = eqRes.items || eqRes.data || eqRes || []
    const types = treeRes.data || treeRes || []
    types.forEach(t => { typeMap.value[t.id] = t.name })
  } catch (e) {
    console.error('Fetch health data error:', e)
  } finally {
    loading.value = false
  }
  nextTick(() => initHealthTrendChart())
})
</script>

<style scoped lang="scss">
.equipment-health {
  display: flex;
  flex-direction: column;
  gap: 16px;
}

.overview-row .health-card {
  display: flex;
  align-items: center;
  gap: 16px;
  .card-icon { width: 48px; height: 48px; border-radius: 12px; display: flex; align-items: center; justify-content: center; }
  .card-info { .card-value { font-size: 24px; font-weight: 700; } .card-label { font-size: 13px; color: #909399; } }
}

.chart-container { height: 300px; }

.category-health {
  .category-item {
    display: flex;
    align-items: center;
    gap: 12px;
    margin-bottom: 16px;
    .category-name { width: 90px; font-size: 13px; color: #606266; }
  }
}

.card-header { display: flex; justify-content: space-between; align-items: center; }
</style>
