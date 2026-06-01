<template>
  <div class="statistics-page">
    <el-row :gutter="16" class="stat-row">
      <el-col :span="6" v-for="stat in statCards" :key="stat.key">
        <el-card shadow="hover" class="stat-card">
          <div class="stat-value">{{ stat.value }}</div>
          <div class="stat-label">{{ stat.label }}</div>
          <div class="stat-trend" :class="stat.trendClass">{{ stat.trend }}</div>
        </el-card>
      </el-col>
    </el-row>

    <el-row :gutter="16">
      <el-col :span="12">
        <el-card shadow="never">
          <template #header><span>工单完成趋势</span></template>
          <div ref="trendChart" class="chart-container"></div>
        </el-card>
      </el-col>
      <el-col :span="12">
        <el-card shadow="never">
          <template #header><span>成本分析</span></template>
          <div ref="costChart" class="chart-container"></div>
        </el-card>
      </el-col>
    </el-row>

    <el-row :gutter="16" style="margin-top:16px">
      <el-col :span="12">
        <el-card shadow="never">
          <template #header><span>设备故障率 Top 5</span></template>
          <el-table :data="faultRank" stripe size="small" v-loading="loading">
            <el-table-column type="index" label="排名" width="60" align="center" />
            <el-table-column prop="name" label="设备" min-width="150" />
            <el-table-column prop="value" label="故障次数" width="100" align="center">
              <template #default="{ row }"><span style="color:#f56c6c">{{ row.value }}</span></template>
            </el-table-column>
          </el-table>
        </el-card>
      </el-col>
      <el-col :span="12">
        <el-card shadow="never">
          <template #header><span>维修效率统计</span></template>
          <div class="efficiency-stats">
            <div class="eff-item"><div class="eff-label">平均响应时间</div><div class="eff-value">12 min</div></div>
            <div class="eff-item"><div class="eff-label">平均修复时间</div><div class="eff-value">2.3 h</div></div>
            <div class="eff-item"><div class="eff-label">超时率</div><div class="eff-value">3.2%</div></div>
            <div class="eff-item"><div class="eff-label">本月完成工单</div><div class="eff-value">{{ monthlyCompleted }}</div></div>
          </div>
        </el-card>
      </el-col>
    </el-row>
  </div>
</template>

<script setup>
import { ref, onMounted, nextTick } from 'vue'
import * as echarts from 'echarts'
import { statisticsApi } from '@/api'

const trendChart = ref(null)
const costChart = ref(null)
const loading = ref(false)
const faultRank = ref([])
const monthlyCompleted = ref(156)

const statCards = ref([
  { key: 'total', label: '设备总数', value: '155', trend: '↑+5', trendClass: 'up' },
  { key: 'healthRate', label: '完好率', value: '95.6%', trend: '↑0.3%', trendClass: 'up' },
  { key: 'completed', label: '本月完成工单', value: '156', trend: '↑+12', trendClass: 'up' },
  { key: 'cost', label: '本月成本', value: '¥458,230', trend: '↑8.5%', trendClass: 'up' }
])

function initTrendChart(data) {
  const chart = echarts.init(trendChart.value)
  const xData = data?.map(d => d.month || d.date) || ['1月', '2月', '3月', '4月', '5月', '6月']
  const yData = data?.map(d => d.count || d.value) || [120, 132, 156, 140, 148, 160]
  chart.setOption({
    tooltip: { trigger: 'axis' },
    grid: { left: 40, right: 20, top: 20, bottom: 30 },
    xAxis: { type: 'category', data: xData },
    yAxis: { type: 'value' },
    series: [
      { type: 'bar', name: '完成工单', data: yData, itemStyle: { color: '#409eff' } },
      { type: 'line', name: '趋势', data: yData, smooth: true, lineStyle: { color: '#67c23a' } }
    ]
  })
  window.addEventListener('resize', () => chart.resize())
}

function initCostChart() {
  const chart = echarts.init(costChart.value)
  chart.setOption({
    tooltip: { trigger: 'item' },
    legend: { orient: 'vertical', right: 10, top: 'center' },
    series: [{
      type: 'pie', radius: ['40%', '70%'],
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
  loading.value = true
  try {
    const [dashRes, woRes, faultRes] = await Promise.all([
      statisticsApi.getDashboard(),
      statisticsApi.getWorkorder(),
      statisticsApi.getFaultTop5()
    ])

    const dash = dashRes || {}
    const wo = woRes || {}

    statCards.value[0].value = String(dash.equipmentTotal || 0)
    statCards.value[1].value = (dash.onlineRate ? (dash.onlineRate * 100).toFixed(1) : 0) + '%'
    statCards.value[2].value = String(wo.total || 0)
    monthlyCompleted.value = wo.total || 0

    faultRank.value = faultRes || []

    nextTick(() => {
      initTrendChart(wo.monthlyTrend)
      initCostChart()
    })
  } catch (e) {
    console.error('Statistics fetch error:', e)
    nextTick(() => {
      initTrendChart()
      initCostChart()
    })
  } finally {
    loading.value = false
  }
})
</script>

<style scoped lang="scss">
.statistics-page { display: flex; flex-direction: column; gap: 16px; }
.stat-row .stat-card { text-align: center; .stat-value { font-size: 28px; font-weight: 700; } .stat-label { font-size: 13px; color: #909399; margin-top: 4px; } .stat-trend { font-size: 12px; margin-top: 4px; &.up { color: #67c23a; } } }
.chart-container { height: 280px; }
.efficiency-stats { display: grid; grid-template-columns: 1fr 1fr; gap: 24px; padding: 20px; .eff-item { text-align: center; .eff-label { color: #909399; font-size: 14px; margin-bottom: 8px; } .eff-value { font-size: 28px; font-weight: bold; color: #303133; } } }
</style>
