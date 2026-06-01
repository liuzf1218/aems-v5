<template>
  <div class="device-container" style="padding: 20px;">
    <!-- ===================== 1. 查询区域 ===================== -->
    <div class="search-box" style="margin-bottom: 20px;">
      <el-input
        v-model="searchForm.deviceName"
        placeholder="按设备名称搜索"
        style="width: 220px; margin-right: 10px;"
      />

      <!-- 查询按钮 + 新增按钮 -->
      <el-button type="primary" @click="getDeviceList">查询</el-button>
      <el-button type="success" @click="addDevice">新增设备</el-button>
    </div>

    <!-- ===================== 2. 设备表格 ===================== -->
    <el-table
      :data="deviceList"
      border
      stripe
      style="width: 100%; margin-bottom: 20px;"
    >
      <el-table-column label="设备编号" prop="deviceCode" />
      <el-table-column label="设备名称" prop="deviceName" />
      <el-table-column label="使用部门" prop="deptName" />
      
      <!-- 设备状态列 -->
      <el-table-column label="设备状态" prop="status">
        <template #default="scope">
          <el-tag :type="scope.row.status === '正常' ? 'success' : 'warning'">
            {{ scope.row.status }}
          </el-tag>
        </template>
      </el-table-column>

      <!-- 操作列：编辑、删除 -->
      <el-table-column label="操作" width="160">
        <template #default="scope">
          <el-button type="primary" size="small" @click="editDevice(scope.row)">
            编辑
          </el-button>
          <el-button type="danger" size="small" @click="deleteDevice(scope.row.id)">
            删除
          </el-button>
        </template>
      </el-table-column>
    </el-table>

    <!-- ===================== 3. 分页 ===================== -->
    <el-pagination
      v-model:current-page="pageNum"
      v-model:page-size="pageSize"
      :total="total"
      layout="total, sizes, prev, pager, next, jumper"
      @size-change="getDeviceList"
      @current-change="getDeviceList"
    />

    <!-- ===================== 4. 新增/编辑弹窗 ===================== -->
    <el-dialog v-model="dialogVisible" title="设备信息" width="500px">
      <!-- 弹窗内表单 -->
      <el-form :model="form" label-width="100px">
        <el-form-item label="设备编号">
          <el-input v-model="form.deviceCode" placeholder="请输入设备编号" />
        </el-form-item>

        <el-form-item label="设备名称">
          <el-input v-model="form.deviceName" placeholder="请输入设备名称" />
        </el-form-item>

        <el-form-item label="使用部门">
          <el-input v-model="form.deptName" />
        </el-form-item>

        <!-- 下拉选择设备状态 -->
        <el-form-item label="设备状态">
          <el-select v-model="form.status" placeholder="请选择状态">
            <el-option label="正常" value="正常" />
            <el-option label="维修中" value="维修中" />
            <el-option label="待报废" value="待报废" />
          </el-select>
        </el-form-item>
      </el-form>

      <!-- 弹窗底部按钮 -->
      <template #footer>
        <el-button @click="dialogVisible = false">取消</el-button>
        <el-button type="primary" @click="submitForm">保存提交</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script setup>
import { ref, reactive } from 'vue'
// 这里后面替换成你自己的后端API接口
// import { getDeviceListApi, addDeviceApi, updateDeviceApi, deleteDeviceApi } from '@/api/device'

// ===================== 查询条件 =====================
const searchForm = reactive({
  deviceName: ''
})

// ===================== 分页参数 =====================
const pageNum = ref(1)
const pageSize = ref(10)
const total = ref(0)

// ===================== 表格数据 =====================
const deviceList = ref([
  {
    id: 1,
    deviceCode: 'SB-001',
    deviceName: '甚高频导航设备',
    deptName: '机场运维部',
    status: '正常'
  },
  {
    id: 2,
    deviceCode: 'SB-002',
    deviceName: '雷达监控主机',
    deptName: '通信保障部',
    status: '维修中'
  }
])

// ===================== 弹窗 & 表单 =====================
const dialogVisible = ref(false)
const form = ref({
  id: '',
  deviceCode: '',
  deviceName: '',
  deptName: '',
  status: '正常'
})

// ===================== 获取设备列表 =====================
const getDeviceList = () => {
  // 这里以后调用你后端的 Web API
  // getDeviceListApi({
  //   pageNum: pageNum.value,
  //   pageSize: pageSize.value,
  //   deviceName: searchForm.deviceName
  // }).then(res => {
  //   deviceList.value = res.data.records
  //   total.value = res.data.total
  // })

  console.log('查询设备列表，页码：', pageNum.value)
}

// ===================== 新增设备 =====================
const addDevice = () => {
  // 清空表单
  form.value = {
    id: '',
    deviceCode: '',
    deviceName: '',
    deptName: '',
    status: '正常'
  }
  dialogVisible.value = true
}

// ===================== 编辑设备 =====================
const editDevice = (row) => {
  // 把当前行数据回填到表单
  form.value = { ...row }
  dialogVisible.value = true
}

// ===================== 提交保存 =====================
const submitForm = () => {
  // 根据有没有id判断是新增还是编辑
  if (form.value.id) {
    console.log('调用编辑接口', form.value)
    // updateDeviceApi(form.value).then(() => {
    //   ElMessage.success('编辑成功')
    //   dialogVisible.value = false
    //   getDeviceList()
    // })
  } else {
    console.log('调用新增接口', form.value)
    // addDeviceApi(form.value).then(() => {
    //   ElMessage.success('新增成功')
    //   dialogVisible.value = false
    //   getDeviceList()
    // })
  }
}

// ===================== 删除设备 =====================
const deleteDevice = (id) => {
  ElMessageBox.confirm('确定要删除该设备吗？', '提示', {
    type: 'warning'
  }).then(() => {
    console.log('调用删除接口，id：', id)
    // deleteDeviceApi(id).then(() => {
    //   ElMessage.success('删除成功')
    //   getDeviceList()
    // })
  })
}

// 页面一加载就获取列表
getDeviceList()
</script>

<style scoped>
.device-container {
  background: #fff;
  border-radius: 4px;
}
</style>