<template>
  <div class="dashboard">
    <!-- 顶部概览卡片 -->
    <el-row :gutter="16" class="overview-row" style="margin-bottom:4px">
      <el-col :xs="12" :sm="8" :md="4.8" v-for="card in overviewCards" :key="card.key">
        <el-card shadow="hover" class="stat-card" @click="handleCardClick(card)">
          <div class="stat-icon" :style="{ background: card.bgColor }">
            <el-icon :size="24" :color="'#fff'">
              <component :is="icons[card.icon]" />
            </el-icon>
          </div>
          <div class="stat-info">
            <div class="stat-value">{{ card.value }}</div>
            <div class="stat-label">{{ card.label }}</div>
          </div>
          <div class="stat-trend" :class="card.trendClass">
            <el-icon v-if="card.trendClass === 'up'"><Top /></el-icon>
            <el-icon v-else><Bottom /></el-icon>
            {{ card.trend }}
          </div>
        </el-card>
      </el-col>
    </el-row>

    <!-- 图表区 -->
    <el-row :gutter="16" class="chart-row">
      <!-- 设备状态分布 -->
      <el-col :xs="24" :sm="12" :lg="8">
        <el-card shadow="never" class="chart-card">
          <template #header>
            <div class="card-header">
              <span>设备状态分布</span>
              <el-button type="primary" link size="small" @click="$router.push('/equipment/list')">查看详情</el-button>
            </div>
          </template>
          <div ref="deviceStatusChart" class="chart-container"></div>
        </el-card>
      </el-col>

      <!-- 工单趋势 -->
      <el-col :xs="24" :sm="12" :lg="8">
        <el-card shadow="never" class="chart-card">
          <template #header>
            <div class="card-header">
              <span>工单趋势（近30天）</span>
              <el-radio-group v-model="trendType" size="small">
                <el-radio-button value="all">全部</el-radio-button>
                <el-radio-button value="fault">故障</el-radio-button>
                <el-radio-button value="inspect">巡检</el-radio-button>
              </el-radio-group>
            </div>
          </template>
          <div ref="workorderTrendChart" class="chart-container"></div>
        </el-card>
      </el-col>

      <!-- 成本分析 -->
      <el-col :xs="24" :sm="12" :lg="8">
        <el-card shadow="never" class="chart-card">
          <template #header>
            <div class="card-header">
              <span>本月成本构成</span>
              <span class="total-cost">&#165;{{ totalCost.toLocaleString() }}</span>
            </div>
          </template>
          <div ref="costAnalysisChart" class="chart-container"></div>
        </el-card>
      </el-col>
    </el-row>

    <!-- 下半区 -->
    <el-row :gutter="16">
      <!-- 系统健康 -->
      <el-col :xs="24" :sm="12" :lg="12">
        <el-card shadow="never" class="chart-card">
          <template #header>
            <div class="card-header">
              <span>系统状态概览</span>
              <el-tag type="success" effect="dark">运行正常</el-tag>
            </div>
          </template>
          <el-table :data="systemHealth" stripe size="small" :show-header="true">
            <el-table-column prop="name" label="系统名称" min-width="100" />
            <el-table-column prop="totalDevices" label="设备总数" width="80" align="center" />
            <el-table-column prop="activeCount" label="运行中" width="80" align="center">
              <template #default="{ row }">
                <span style="color: #67c23a">{{ row.activeCount }}</span>
              </template>
            </el-table-column>
            <el-table-column prop="faultedCount" label="故障" width="60" align="center">
              <template #default="{ row }">
                <span :style="{ color: row.faultedCount > 0 ? '#f56c6c' : '#67c23a' }">{{ row.faultedCount }}</span>
              </template>
            </el-table-column>
            <el-table-column label="健康率" width="100" align="center">
              <template #default="{ row }">
                <el-progress :percentage="Math.round((row.activeCount / row.totalDevices) * 100)" :stroke-width="8" :color="row.faultedCount === 0 ? '#67c23a' : '#e6a23c'" />
              </template>
            </el-table-column>
            <el-table-column label="状态" width="70" align="center">
              <template #default="{ row }">
                <el-tag :type="row.faultedCount === 0 ? 'success' : 'warning'" size="small" effect="dark">{{ row.faultedCount === 0 ? '正常' : '告警' }}</el-tag>
              </template>
            </el-table-column>
          </el-table>
        </el-card>
      </el-col>

      <!-- 最新告警 -->
      <el-col :xs="24" :sm="12" :lg="12">
        <el-card shadow="never" class="chart-card">
          <template #header>
            <div class="card-header">
              <span>最新告警</span>
              <el-button type="primary" link size="small">查看全部</el-button>
            </div>
          </template>
          <div class="alert-list">
            <div v-for="(alert, index) in alerts" :key="index" class="alert-item" :class="alert.level">
              <div class="alert-icon">
                <el-icon v-if="alert.level === 'critical'" color="#f56c6c"><WarningFilled /></el-icon>
                <el-icon v-else-if="alert.level === 'warning'" color="#e6a23c"><Warning /></el-icon>
                <el-icon v-else color="#67c23a"><CircleCheck /></el-icon>
              </div>
              <div class="alert-content">
                <div class="alert-message">{{ alert.message }}</div>
                <div class="alert-meta">
                  <span>{{ alert.source }}</span>
                  <span>{{ alert.time }}</span>
                </div>
              </div>
              <el-button type="primary" link size="small">处理</el-button>
            </div>
          </div>
        </el-card>
      </el-col>
    </el-row>

    <!-- 快捷操作 -->
    <el-card shadow="never" class="quick-actions">
      <template #header><span>快捷操作</span></template>
      <el-row :gutter="16">
        <el-col :xs="12" :sm="6" :md="3" v-for="action in quickActions" :key="action.path">
          <div class="action-item" @click="$router.push(action.path)">
            <div class="action-icon" :style="{ background: action.color }">
              <el-icon :size="20" color="#fff"><component :is="icons[action.icon]" /></el-icon>
            </div>
            <span>{{ action.label }}</span>
          </div>
        </el-col>
      </el-row>
    </el-card>
  </div>
</template>

<script setup>
import { ref, reactive, onMounted, nextTick } from 'vue'
import { useRouter } from 'vue-router'
import * as echarts from 'echarts'
import { useDashboardStore } from '@/store/dashboard'
import { useEquipmentStore } from '@/store/equipment'
import { useWorkorderStore } from '@/store/workorder'
import {
  Monitor, CircleCheck, WarningFilled, Warning,
  Document, TrendCharts, EditPen, Clock, Box,
  Calendar, DataAnalysis, Files, Setting, Top, Bottom
} from '@element-plus/icons-vue'

const icons = { Monitor, CircleCheck, WarningFilled, Warning, Document, TrendCharts, EditPen, Clock, Box, Calendar, DataAnalysis, Files, Setting, Top, Bottom }

const router = useRouter()
const dashboardStore = useDashboardStore()
const equipmentStore = useEquipmentStore()
const workorderStore = useWorkorderStore()

const deviceStatusChart = ref(null)
const workorderTrendChart = ref(null)
const costAnalysisChart = ref(null)
const trendType = ref('all')
const totalCost = ref(458230)

const overviewCards = reactive([
  { key: 'total', label: '设备总数', value: '0', icon: 'Monitor', bgColor: 'linear-gradient(135deg, #667eea 0%, #764ba2 100%)', trend: '-', trendClass: 'up' },
  { key: 'inUse', label: '在用设备', value: '0', icon: 'CircleCheck', bgColor: 'linear-gradient(135deg, #11998e 0%, #38ef7d 100%)', trend: '-', trendClass: 'up' },
  { key: 'faulted', label: '故障设备', value: '0', icon: 'Warning', bgColor: 'linear-gradient(135deg, #f093fb 0%, #f5576c 100%)', trend: '-', trendClass: 'down' },
  { key: 'pending', label: '待处理工单', value: '0', icon: 'Document', bgColor: 'linear-gradient(135deg, #4facfe 0%, #00f2fe 100%)', trend: '-', trendClass: 'up' },
  { key: 'health', label: '完好率', value: '0%', icon: 'TrendCharts', bgColor: 'linear-gradient(135deg, #43e97b 0%, #38f9d7 100%)', trend: '-', trendClass: 'up' }
])

const systemHealth = ref([])

const alerts = [
  { level: 'critical', time: '05:15', message: '导航设备NAV-001温度异常超过阈值', source: '设备监控' },
  { level: 'critical', time: '04:30', message: '工单WO-20260330-042响应超时', source: 'SLA监控' },
  { level: 'warning', time: '03:45', message: '备件"雷达天线模块"库存不足（剩余2件）', source: '库存预警' },
  { level: 'warning', time: '02:20', message: '二次雷达SRD-003校准即将到期', source: '维护管理' },
  { level: 'success', time: '01:00', message: '工单WO-20260328-038已验收关闭', source: '工单管理' }
]

const quickActions = [
  { label: '创建工单', icon: 'EditPen', path: '/workorder/create', color: '#409EFF' },
  { label: '设备台账', icon: 'Monitor', path: '/equipment/list', color: '#67c23a' },
  { label: '巡检管理', icon: 'Clock', path: '/workorder/inspection', color: '#e6a23c' },
  { label: '库存查询', icon: 'Box', path: '/sparepart/list', color: '#f56c6c' },
  { label: '维保计划', icon: 'Calendar', path: '/maintenance/plans', color: '#909399' },
  { label: '统计报表', icon: 'DataAnalysis', path: '/statistics', color: '#764ba2' },
  { label: '文档管理', icon: 'Files', path: '/document/list', color: '#11998e' },
  { label: '系统设置', icon: 'Setting', path: '/system/settings', color: '#4facfe' }
]

const handleCardClick = (card) => {
  if (card.key === 'total' || card.key === 'inUse' || card.key === 'faulted') {
    router.push('/equipment/list')
  } else if (card.key === 'pending') {
    router.push('/workorder/list')
  }
}

function initDeviceStatusChart(active, fault, maintenance, standby) {
  const chart = echarts.init(deviceStatusChart.value)
  chart.setOption({
    tooltip: { trigger: 'item', formatter: '{b}: {c} ({d}%)' },
    legend: { orient: 'vertical', right: 10, top: 'center', textStyle: { fontSize: 12 } },
    series: [{
      type: 'pie', radius: ['45%', '75%'], center: ['40%', '50%'],
      label: { show: false },
      emphasis: { label: { show: true, fontSize: 14, fontWeight: 'bold' } },
      data: [
        { value: active || 0, name: '\u5728\u7528', itemStyle: { color: '#67c23a' } },
        { value: fault || 0, name: '\u6545\u969c', itemStyle: { color: '#f56c6c' } },
        { value: maintenance || 0, name: '\u7ef4\u4fee\u4e2d', itemStyle: { color: '#e6a23c' } },
        { value: standby || 0, name: '\u5907\u7528', itemStyle: { color: '#409eff' } }
      ]
    }]
  })
  window.addEventListener('resize', () => chart.resize())
}

function initWorkorderTrendChart() {
  const chart = echarts.init(workorderTrendChart.value)
  const days = 30, dates = [], faultData = [], inspectData = []
  for (let i = 0; i < days; i++) {
    const d = new Date(); d.setDate(d.getDate() - days + i)
    dates.push((d.getMonth() + 1) + '/' + d.getDate())
    faultData.push(Math.floor(Math.random() * 5) + 1)
    inspectData.push(Math.floor(Math.random() * 8) + 2)
  }
  chart.setOption({
    tooltip: { trigger: 'axis' },
    legend: { data: ['故障工单', '巡检工单'], bottom: 0, textStyle: { fontSize: 11 } },
    grid: { left: 40, right: 20, top: 10, bottom: 35 },
    xAxis: { type: 'category', data: dates, axisLabel: { fontSize: 10 } },
    yAxis: { type: 'value' },
    series: [
      { name: '故障工单', type: 'line', smooth: true, data: faultData, areaStyle: { color: 'rgba(245,108,108,0.15)' }, lineStyle: { color: '#f56c6c' }, itemStyle: { color: '#f56c6c' } },
      { name: '巡检工单', type: 'line', smooth: true, data: inspectData, areaStyle: { color: 'rgba(64,158,255,0.15)' }, lineStyle: { color: '#409eff' }, itemStyle: { color: '#409eff' } }
    ]
  })
  window.addEventListener('resize', () => chart.resize())
}

function initCostAnalysisChart() {
  const chart = echarts.init(costAnalysisChart.value)
  chart.setOption({
    tooltip: { trigger: 'item', formatter: '{b}: \u00a5{c} ({d}%)' },
    legend: { orient: 'vertical', right: 10, top: 'center', textStyle: { fontSize: 12 } },
    series: [{
      type: 'pie', radius: ['45%', '75%'], center: ['40%', '50%'],
      label: { show: false },
      data: [
        { value: 206204, name: '人工成本', itemStyle: { color: '#409eff' } },
        { value: 160381, name: '备件成本', itemStyle: { color: '#67c23a' } },
        { value: 91646, name: '外协服务', itemStyle: { color: '#e6a23c' } }
      ]
    }]
  })
  window.addEventListener('resize', () => chart.resize())
}

onMounted(async () => {
  try {
    // 获取 Dashboard 统计数据
    const dash = await dashboardStore.fetchDashboard()
    const deviceStats = await dashboardStore.fetchDeviceStats()

    const total = dash.equipmentTotal || 0
    const statusDist = dash.equipmentStatusDistribution || []
    const getStatusCount = (name) => {
      const item = statusDist.find(x => x.name === name || x.Name === name)
      return item ? (item.value || item.Value || 0) : 0
    }
    const active = getStatusCount('在用')
    const fault = getStatusCount('故障')
    const maintenance = getStatusCount('维护中')
    const standby = Math.max(0, total - active - fault - maintenance)
    const healthRate = total > 0 ? ((active / total) * 100).toFixed(1) + '%' : '0%'

    overviewCards[0].value = String(total)
    overviewCards[1].value = String(active)
    overviewCards[2].value = String(fault)
    overviewCards[3].value = String(dash.pendingWorkorders || 0)
    overviewCards[4].value = healthRate

    // 获取设备列表用于系统健康计算
    const eqRes = await equipmentStore.fetchList({ page: 1, pageSize: 200 })
    const allEquip = eqRes.items || []
    const catMap = {}
    allEquip.forEach(eq => {
      const cat = eq.categoryName || eq.softwareType || '\u5176\u4ed6'
      if (!catMap[cat]) catMap[cat] = { name: cat, totalDevices: 0, activeCount: 0, faultedCount: 0 }
      catMap[cat].totalDevices++
      if (eq.status === 0 || eq.status === 'ACTIVE') catMap[cat].activeCount++
      else if (eq.status === 1 || eq.status === 'FAULT') catMap[cat].faultedCount++
    })
    systemHealth.value = Object.values(catMap)

    nextTick(() => {
      initDeviceStatusChart(active, fault, maintenance, standby)
      initWorkorderTrendChart()
      initCostAnalysisChart()
    })
  } catch (e) {
    console.error('Dashboard data fetch error:', e)
    nextTick(() => {
      initDeviceStatusChart(0, 0, 0, 0)
      initWorkorderTrendChart()
      initCostAnalysisChart()
    })
  }
})
</script>

<style scoped lang="scss">
.dashboard { display: flex; flex-direction: column; gap: 16px; }
.overview-row .stat-card {
  display: flex; align-items: center; gap: 12px; cursor: pointer; transition: transform 0.2s;
  &:hover { transform: translateY(-2px); }
  .stat-icon { width: 48px; height: 48px; border-radius: 12px; display: flex; align-items: center; justify-content: center; flex-shrink: 0; }
  .stat-info { flex: 1; .stat-value { font-size: 24px; font-weight: 700; color: #303133; } .stat-label { font-size: 12px; color: #909399; margin-top: 2px; } }
  .stat-trend { font-size: 12px; display: flex; align-items: center; gap: 2px; &.up { color: #67c23a; } &.down { color: #f56c6c; } }
}
.chart-row .chart-card { height: 100%; }
.chart-card .card-header { display: flex; justify-content: space-between; align-items: center; .total-cost { font-size: 18px; font-weight: 700; color: #f56c6c; } }
.chart-container { height: 280px; }
.alert-list { max-height: 300px; overflow-y: auto;
  .alert-item { display: flex; align-items: center; gap: 12px; padding: 10px 0; border-bottom: 1px solid #f0f0f0; &:last-child { border-bottom: none; }
    .alert-icon { width: 32px; height: 32px; border-radius: 50%; display: flex; align-items: center; justify-content: center; flex-shrink: 0; background: #f5f7fa; }
    .alert-content { flex: 1; min-width: 0; .alert-message { font-size: 13px; color: #303133; white-space: nowrap; overflow: hidden; text-overflow: ellipsis; } .alert-meta { font-size: 12px; color: #909399; margin-top: 4px; display: flex; gap: 16px; } }
  }
}
.quick-actions { margin-top: 4px; }
.quick-actions .action-item { display: flex; flex-direction: column; align-items: center; gap: 8px; padding: 16px 8px; cursor: pointer; border-radius: 8px; transition: all 0.2s; &:hover { background: #f5f7fa; transform: translateY(-2px); } .action-icon { width: 40px; height: 40px; border-radius: 10px; display: flex; align-items: center; justify-content: center; } span { font-size: 13px; color: #606266; } }
</style>
