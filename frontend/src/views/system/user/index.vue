<template>
  <div class="page-container">
    <el-card shadow="never">
      <template #header>
        <div class="card-header">
          <span>用户管理</span>
          <el-button type="primary" size="small" @click="handleAdd">新增用户</el-button>
        </div>
      </template>
      <el-form inline style="margin-bottom:16px">
        <el-form-item>
          <el-input v-model="filters.keyword" placeholder="搜索用户名" clearable @keyup.enter="fetchData" />
        </el-form-item>
        <el-form-item>
          <el-select v-model="filters.role" placeholder="角色" clearable style="width:120px">
            <el-option label="管理员" value="admin" />
            <el-option label="操作员" value="operator" />
          </el-select>
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="fetchData">搜索</el-button>
          <el-button @click="filters.keyword = ''; filters.role = ''; fetchData()">重置</el-button>
        </el-form-item>
      </el-form>
      <el-table :data="users" stripe v-loading="loading">
        <el-table-column prop="id" label="ID" width="60" />
        <el-table-column prop="username" label="用户名" width="120" />
        <el-table-column prop="realName" label="姓名" width="90" />
        <el-table-column prop="email" label="邮箱" min-width="150" show-overflow-tooltip />
        <el-table-column prop="roleName" label="角色" width="100">
          <template #default="{ row }">
            <el-tag :type="row.roleName === '管理员' ? 'danger' : 'primary'" size="small">{{ row.roleName }}</el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="status" label="状态" width="80" align="center">
          <template #default="{ row }">
            <el-tag :type="row.isActive ? 'success' : 'info'" size="small">{{ row.isActive ? '启用' : '禁用' }}</el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="createdAt" label="创建时间" width="160" />
        <el-table-column label="操作" width="150" align="center">
          <template #default="{ row }">
            <el-button type="primary" link size="small" @click="handleEdit(row)">编辑</el-button>
            <el-button :type="row.isActive ? 'danger' : 'success'" link size="small" @click="handleToggle(row)">
              {{ row.isActive ? '禁用' : '启用' }}
            </el-button>
          </template>
        </el-table-column>
      </el-table>
    </el-card>

    <UserDialog v-model:visible="dialogVisible" :data="currentUser" @success="fetchData" />
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { userApi } from '@/api'
import UserDialog from './UserDialog.vue'

const users = ref([])
const loading = ref(false)
const filters = ref({ keyword: '', role: '' })

const dialogVisible = ref(false)
const currentUser = ref(null)

const fetchData = async () => {
  loading.value = true
  try {
    const res = await userApi.getList(filters.value)
    users.value = res?.items || []
  } catch (e) {
    console.error('Fetch user list error:', e)
  } finally {
    loading.value = false
  }
}

onMounted(fetchData)

const handleAdd = () => {
  currentUser.value = null
  dialogVisible.value = true
}

const handleEdit = (row) => {
  currentUser.value = row
  dialogVisible.value = true
}

const handleToggle = async (row) => {
  const action = row.isActive ? '禁用' : '启用'
  try {
    await ElMessageBox.confirm(`确定要${action}用户 "${row.username}" 吗？`, '提示', { type: 'warning' })
    await userApi.toggle(row.id)
    ElMessage.success(`${action}成功`)
    fetchData()
  } catch (e) {
    if (e !== 'cancel') {
      ElMessage.error(e.message || '操作失败')
    }
  }
}
</script>

<style scoped>
.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}
</style>
