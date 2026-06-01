<template>
  <div style="padding: 30px; max-width: 1200px; margin: 0 auto">
    <h1>🏠 设备数据看板</h1>

    <div style="display: grid; grid-template-columns: repeat(3, 1fr); gap: 20px; margin: 20px 0">
      <el-card
        ><h3>总设备</h3>
        <h2 style="color: #409eff">{{ total }}</h2></el-card
      >
      <el-card
        ><h3>运行中</h3>
        <h2 style="color: #67c23a">{{ online }}</h2></el-card
      >
      <el-card
        ><h3>离线</h3>
        <h2 style="color: #f56c6c">{{ offline }}</h2></el-card
      >
    </div>

    <div style="display: grid; grid-template-columns: 1fr 1fr; gap: 20px">
      <el-card
        ><template #header>状态分布</template>
        <div id="pie" style="width: 100%; height: 350px"></div
      ></el-card>
      <el-card
        ><template #header>位置分布</template>
        <div id="bar" style="width: 100%; height: 350px"></div
      ></el-card>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import * as echarts from 'echarts'

// 测试数据（和 Device.vue 保持一致）
const devices = ref([
  { id: 1, name: '空调设备', status: '运行中', place: '一楼大厅' },
  { id: 2, name: '监控摄像头', status: '运行中', place: '二楼走廊' },
  { id: 3, name: '门禁设备', status: '离线', place: '大门入口' },
  { id: 4, name: '消防报警器', status: '运行中', place: '地下室' },
  { id: 5, name: '电梯控制', status: '离线', place: '一号楼' },
  { id: 6, name: 'LED大屏', status: '运行中', place: '前广场' },
  { id: 7, name: 'WiFi基站', status: '运行中', place: '三楼机房' },
  { id: 8, name: '烟雾传感器', status: '离线', place: '负一楼' },
  { id: 9, name: '停车场道闸', status: '运行中', place: '车库入口' },
  { id: 10, name: '环境监测仪', status: '运行中', place: '楼顶' },
])

const total = computed(() => devices.value.length)
const online = computed(() => devices.value.filter((i) => i.status === '运行中').length)
const offline = computed(() => devices.value.filter((i) => i.status === '离线').length)

let pie, bar

onMounted(() => {
  pie = echarts.init(document.getElementById('pie'))
  bar = echarts.init(document.getElementById('bar'))
  refresh()
})

const refresh = () => {
  pie.setOption({
    tooltip: { trigger: 'item' },
    series: [
      {
        type: 'pie',
        radius: ['40%', '70%'],
        data: [
          { value: online.value, name: '运行中', itemStyle: { color: '#67C23A' } },
          { value: offline.value, name: '离线', itemStyle: { color: '#F56C6C' } },
        ],
      },
    ],
  })

  const placeMap = {}
  devices.value.forEach((item) => {
    placeMap[item.place] = (placeMap[item.place] || 0) + 1
  })

  bar.setOption({
    xAxis: { type: 'category', data: Object.keys(placeMap) },
    yAxis: { type: 'value' },
    series: [{ type: 'bar', data: Object.values(placeMap) }],
  })
}

window.onresize = () => {
  pie?.resize()
  bar?.resize()
}
</script>
