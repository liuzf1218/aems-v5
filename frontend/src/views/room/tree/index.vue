<template>
  <div class="page-container">
    <el-row :gutter="16">
      <el-col :span="6">
        <el-card shadow="never">
          <template #header>
            <div class="card-header">
              <span>机房分类</span>
              <el-button type="primary" link size="small" @click="loadTree">刷新</el-button>
            </div>
          </template>
          <el-input v-model="filterText" placeholder="搜索楼宇/机房" clearable style="margin-bottom:8px" />
          <el-tree
            ref="treeRef"
            :data="treeData"
            :props="{ label: 'name', children: 'children' }"
            :filter-node-method="filterNode"
            highlight-current
            default-expand-all
            @node-click="handleNodeClick"
          />
        </el-card>
      </el-col>
      <el-col :span="18">
        <!-- 点击楼宇：显示机房卡片列表 -->
        <template v-if="selectedNode?.nodeType === 'building'">
          <el-card shadow="never">
            <template #header>
              <div class="card-header">
                <span>{{ selectedNode?.name }} - 机房列表</span>
                <el-button type="primary" size="small" @click="handleAddRoom">新增机房</el-button>
              </div>
            </template>
            <el-row :gutter="16">
              <el-col :span="8" v-for="room in roomList" :key="room.id">
                <el-card shadow="hover" class="room-card" @click="selectRoom(room)">
                  <div class="room-header">
                    <span class="room-name">{{ room.name }}</span>
                    <el-tag :type="room.status === '正常' ? 'success' : 'danger'" size="small">{{ room.status || '正常' }}</el-tag>
                  </div>
                  <div class="room-info">
                    <div class="info-item"><span class="label">设备数</span><span class="value">{{ room.deviceCount || 0 }}</span></div>
                    <div class="info-item"><span class="label">温度</span><span class="value">{{ room.temperature || '-' }}°C</span></div>
                    <div class="info-item"><span class="label">湿度</span><span class="value">{{ room.humidity || '-' }}%</span></div>
                  </div>
                </el-card>
              </el-col>
            </el-row>
            <el-empty v-if="!roomList.length" description="暂无机房" />
          </el-card>
        </template>

        <!-- 点击机房：显示机房详情 -->
        <template v-else-if="selectedNode?.nodeType === 'room'">
          <RoomDetail :room-id="selectedNode.id" />
        </template>

        <!-- 默认 -->
        <template v-else>
          <el-card shadow="never">
            <el-empty description="请选择左侧楼宇或机房" />
          </el-card>
        </template>
      </el-col>
    </el-row>
  </div>
</template>

<script setup>
import { ref, onMounted, watch } from 'vue'
import { buildingApi } from '@/api'
import RoomDetail from '../detail/index.vue'

const treeRef = ref(null)
const treeData = ref([])
const filterText = ref('')
const selectedNode = ref(null)
const roomList = ref([])

const loadTree = async () => {
  try {
    const res = await buildingApi.getTree()
    treeData.value = Array.isArray(res) ? res : (res || [])
  } catch (e) {
    console.error('Load tree error:', e)
  }
}

onMounted(loadTree)

watch(filterText, (val) => {
  treeRef.value?.filter(val)
})

function filterNode(value, data) {
  if (!value) return true
  return data.name && data.name.includes(value)
}

function handleNodeClick(node) {
  selectedNode.value = node
  if (node.nodeType === 'building') {
    roomList.value = Array.isArray(node.children) ? node.children : []
  }
}

function selectRoom(room) {
  selectedNode.value = { ...room, nodeType: 'room' }
}

function handleAddRoom() {
  // TODO: 新增机房
}
</script>

<style scoped>
.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}
.room-card {
  cursor: pointer;
  transition: all .2s;
  margin-bottom: 16px;
}
.room-card:hover {
  transform: translateY(-2px);
}
.room-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 12px;
}
.room-name {
  font-size: 16px;
  font-weight: 600;
}
.room-info {
  display: grid;
  grid-template-columns: repeat(3, 1fr);
  gap: 8px;
}
.info-item {
  text-align: center;
}
.label {
  display: block;
  font-size: 12px;
  color: #909399;
}
.value {
  font-size: 18px;
  font-weight: 600;
}
</style>
