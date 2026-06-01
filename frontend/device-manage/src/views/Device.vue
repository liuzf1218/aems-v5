<template>
  <div style="padding: 20px">
    <h1>📱 设备管理（前端测试数据）</h1>

    <div style="margin-bottom: 20px">
      <el-input v-model="search" placeholder="搜索" style="width: 300px" />
      <el-button type="primary" style="margin-left: 10px" @click="openAdd">新增</el-button>
    </div>

    <el-table :data="list" border style="width: 100%">
      <el-table-column label="ID" prop="id" />
      <el-table-column label="名称" prop="name" />
      <el-table-column label="位置" prop="place" />
      <el-table-column label="状态">
        <template #default="s">
          <el-tag :type="s.row.status === '运行中' ? 'success' : 'danger'">
            {{ s.row.status }}
          </el-tag>
        </template>
      </el-table-column>
      <el-table-column label="操作">
        <template #default="s">
          <el-button size="small" type="primary" @click="openEdit(s.row)">编辑</el-button>
          <el-button size="small" type="success" @click="toggle(s.row)">切换</el-button>
          <el-button size="small" type="danger" @click="del(s.row.id)">删除</el-button>
        </template>
      </el-table-column>
    </el-table>

    <el-dialog v-model="addShow" title="新增">
      <el-form label-width="80px">
        <el-form-item label="名称"><el-input v-model="addForm.name" /></el-form-item>
        <el-form-item label="位置"><el-input v-model="addForm.place" /></el-form-item>
        <el-form-item label="状态">
          <el-select v-model="addForm.status" style="width: 100%">
            <el-option label="运行中" value="运行中" />
            <el-option label="离线" value="离线" />
          </el-select>
        </el-form-item>
      </el-form>
      <template #footer>
        <el-button @click="addShow = false">取消</el-button>
        <el-button type="primary" @click="saveAdd">保存</el-button>
      </template>
    </el-dialog>

    <el-dialog v-model="editShow" title="编辑">
      <el-form label-width="80px">
        <el-form-item label="名称"><el-input v-model="editForm.name" /></el-form-item>
        <el-form-item label="位置"><el-input v-model="editForm.place" /></el-form-item>
        <el-form-item label="状态">
          <el-select v-model="editForm.status" style="width: 100%">
            <el-option label="运行中" value="运行中" />
            <el-option label="离线" value="离线" />
          </el-select>
        </el-form-item>
      </el-form>
      <template #footer>
        <el-button @click="editShow = false">取消</el-button>
        <el-button type="primary" @click="saveEdit">保存</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script setup>
import { ref, computed } from 'vue'
import { ElMessage } from 'element-plus'

// 前端测试数据
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

const search = ref('')

const list = computed(() => devices.value.filter((item) => item.name.includes(search.value)))

// 新增
const addShow = ref(false)
const addForm = ref({ name: '', status: '运行中', place: '' })

const openAdd = () => {
  addShow.value = true
  addForm.value = { name: '', status: '运行中', place: '' }
}

const saveAdd = () => {
  const newId = Date.now()
  devices.value.push({ ...addForm.value, id: newId })
  ElMessage.success('新增成功')
  addShow.value = false
}

// 编辑
const editShow = ref(false)
const editForm = ref({})

const openEdit = (row) => {
  editShow.value = true
  editForm.value = { ...row }
}

const saveEdit = () => {
  const index = devices.value.findIndex((x) => x.id === editForm.value.id)
  devices.value[index] = { ...editForm.value }
  ElMessage.success('保存成功')
  editShow.value = false
}

// 切换状态
const toggle = (row) => {
  row.status = row.status === '运行中' ? '离线' : '运行中'
}

// 删除
const del = (id) => {
  devices.value = devices.value.filter((x) => x.id !== id)
  ElMessage.success('删除成功')
}
</script>
