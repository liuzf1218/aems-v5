<template>
  <el-dialog
    :title="title"
    v-model="dialogVisible"
    width="500px"
    :close-on-click-modal="false"
    @closed="resetForm"
  >
    <el-form ref="formRef" :model="form" :rules="rules" label-width="80px">
      <el-form-item label="用户名" prop="username">
        <el-input v-model="form.username" placeholder="请输入用户名" :disabled="!!props.data?.id" />
      </el-form-item>
      <el-form-item label="姓名" prop="realName">
        <el-input v-model="form.realName" placeholder="请输入姓名" />
      </el-form-item>
      <el-form-item label="邮箱" prop="email">
        <el-input v-model="form.email" placeholder="请输入邮箱" />
      </el-form-item>
      <el-form-item label="电话" prop="phone">
        <el-input v-model="form.phone" placeholder="请输入电话" />
      </el-form-item>
      <el-form-item label="角色" prop="roleId">
        <el-select v-model="form.roleId" placeholder="请选择角色" style="width:100%">
          <el-option v-for="role in roleList" :key="role.id" :label="role.name" :value="role.id" />
        </el-select>
      </el-form-item>
      <el-form-item label="密码" prop="password" v-if="!props.data?.id">
        <el-input v-model="form.password" type="password" placeholder="请输入密码" show-password />
      </el-form-item>
      <el-form-item label="状态" prop="isActive">
        <el-switch v-model="form.isActive" active-text="启用" inactive-text="禁用" />
      </el-form-item>
    </el-form>
    <template #footer>
      <el-button @click="dialogVisible = false">关闭</el-button>
      <el-button type="primary" @click="handleSubmit" :loading="submitting">保存</el-button>
    </template>
  </el-dialog>
</template>

<script setup>
import { ref, computed, watch, onMounted } from 'vue'
import { ElMessage } from 'element-plus'
import { userApi, roleApi } from '@/api'

const props = defineProps({
  visible: Boolean,
  data: Object
})
const emit = defineEmits(['update:visible', 'success'])

const dialogVisible = computed({
  get: () => props.visible,
  set: (val) => emit('update:visible', val)
})

const title = computed(() => (props.data?.id ? '编辑用户' : '新增用户'))

const formRef = ref(null)
const submitting = ref(false)
const roleList = ref([])

const form = ref({
  username: '',
  realName: '',
  email: '',
  phone: '',
  roleId: null,
  password: '',
  isActive: true
})

const rules = {
  username: [
    { required: true, message: '请输入用户名', trigger: 'blur' },
    { min: 2, max: 20, message: '长度在 2 到 20 个字符', trigger: 'blur' }
  ],
  email: [
    { required: true, message: '请输入邮箱', trigger: 'blur' },
    { type: 'email', message: '请输入正确的邮箱地址', trigger: 'blur' }
  ],
  roleId: [{ required: true, message: '请选择角色', trigger: 'change' }],
  password: [{ required: true, message: '请输入密码', trigger: 'blur' }]
}

onMounted(async () => {
  try {
    const res = await roleApi.getList()
    roleList.value = res.items || res || []
  } catch (e) {
    console.error('Fetch roles error:', e)
  }
})

watch(() => props.data, (val) => {
  if (val) {
    form.value = {
      username: val.username || '',
      realName: val.realName || '',
      email: val.email || '',
      phone: val.phone || '',
      roleId: val.roleId || val.role?.id || null,
      password: '',
      isActive: val.isActive !== false
    }
  } else {
    resetForm()
  }
}, { immediate: true })

function resetForm() {
  form.value = { username: '', realName: '', email: '', phone: '', roleId: null, password: '', isActive: true }
  formRef.value?.resetFields()
}

async function handleSubmit() {
  const valid = await formRef.value?.validate().catch(() => false)
  if (!valid) return
  submitting.value = true
  try {
    const payload = { ...form.value }
    if (props.data?.id) {
      delete payload.password
      delete payload.username
      await userApi.update(props.data.id, payload)
      ElMessage.success('更新成功')
    } else {
      await userApi.create(payload)
      ElMessage.success('创建成功')
    }
    emit('success')
    dialogVisible.value = false
  } catch (e) {
    ElMessage.error(e.message || '操作失败')
  } finally {
    submitting.value = false
  }
}
</script>
