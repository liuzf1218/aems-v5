<template>
  <div class="page-container">
    <el-card shadow="never">
      <template #header><div class="card-header"><span>机房管理</span><el-button type="primary" size="small">新增机房</el-button></div></template>
      <el-form inline style="margin-bottom:16px">
        <el-form-item>
          <el-input v-model="filters.keyword" placeholder="搜索机房名称/编码" clearable @keyup.enter="handleSearch" />
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="handleSearch">搜索</el-button>
          <el-button @click="filters.keyword = ''; handleSearch()">重置</el-button>
        </el-form-item>
      </el-form>
      <el-row :gutter="16">
        <el-col :span="8" v-for="room in rooms" :key="room.id">
          <el-card shadow="hover" class="room-card" @click="$router.push(`/room/detail/${room.id}`)">
            <div class="room-header">
              <span class="room-name">{{ room.name }}</span>
              <el-tag :type="room.status === '正常' ? 'success' : 'danger'" size="small">{{ room.status }}</el-tag>
            </div>
            <div class="room-info">
              <div class="info-item"><span class="label">设备数</span><span class="value">{{ room.deviceCount || 0 }}</span></div>
              <div class="info-item"><span class="label">温度</span><span class="value">{{ room.temperature || '-' }}°C</span></div>
              <div class="info-item"><span class="label">湿度</span><span class="value">{{ room.humidity || '-' }}%</span></div>
            </div>
          </el-card>
        </el-col>
      </el-row>
    </el-card>
  </div>
</template>
<script setup>
import { ref, onMounted } from 'vue'
import { useRoomStore } from '@/store/room'

const roomStore = useRoomStore()
const rooms = ref([])
const allRooms = ref([])
const filters = ref({ keyword: '' })

const handleSearch = () => {
  const kw = filters.value.keyword.trim().toLowerCase()
  if (!kw) {
    rooms.value = allRooms.value
    return
  }
  rooms.value = allRooms.value.filter(r =>
    (r.name && r.name.toLowerCase().includes(kw)) ||
    (r.code && r.code.toLowerCase().includes(kw))
  )
}

onMounted(async () => {
  const res = await roomStore.fetchList()
  allRooms.value = res.items || []
  rooms.value = allRooms.value
})
</script>
<style scoped lang="scss">
.card-header{display:flex;justify-content:space-between;align-items:center}
.room-card{cursor:pointer;transition:all .2s;&:hover{transform:translateY(-2px)}}
.room-header{display:flex;justify-content:space-between;align-items:center;margin-bottom:12px;.room-name{font-size:16px;font-weight:600}}
.room-info{display:grid;grid-template-columns:repeat(3,1fr);gap:8px;.info-item{text-align:center;.label{display:block;font-size:12px;color:#909399}.value{font-size:18px;font-weight:600}}}
</style>
