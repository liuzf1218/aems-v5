<template>
  <el-dialog
    :title="dialogTitle"
    :model-value="visible"
    @update:model-value="$emit('update:visible', $event)"
    width="600px"
    :close-on-click-modal="false"
  >
    <el-form
      ref="formRef"
      :model="form"
      label-width="100px"
      :disabled="readonly"
    >
      <el-form-item label="编号" prop="code">
        <el-input v-model="form.code" placeholder="请输入编号" />
      </el-form-item>
      <el-form-item label="软件名称" prop="name">
        <el-input v-model="form.name" placeholder="请输入软件名称" />
      </el-form-item>
      <el-form-item label="部署设备" prop="equipmentId">
        <el-select v-model="form.equipmentId" placeholder="请选择部署设备" style="width: 100%" @change="onEquipmentChange">
          <el-option v-for="eq in equipmentList" :key="eq.id" :label="eq.name" :value="eq.id" />
        </el-select>
      </el-form-item>
      <el-form-item label="所属系统">
        <el-input v-model="form.systemName" placeholder="选择设备后自动显示" disabled />
      </el-form-item>
      <el-form-item label="类型" prop="softwareType">
        <el-select v-model="form.softwareType" placeholder="请选择类型" style="width: 100%">
          <el-option label="操作系统" value="操作系统" />
          <el-option label="数据库" value="数据库" />
          <el-option label="中间件" value="中间件" />
          <el-option label="应用软件" value="应用软件" />
          <el-option label="工具软件" value="工具软件" />
        </el-select>
      </el-form-item>
      <el-form-item label="厂商" prop="vendor">
        <el-input v-model="form.vendor" placeholder="请输入厂商" />
      </el-form-item>
      <el-form-item label="授权类型" prop="licenseType">
        <el-select v-model="form.licenseType" placeholder="请选择授权类型" style="width: 100%">
          <el-option label="永久授权" value="永久授权" />
          <el-option label="年度订阅" value="年度订阅" />
          <el-option label="免费开源" value="免费开源" />
        </el-select>
      </el-form-item>
      <el-form-item label="版本" prop="version">
        <el-input v-model="form.version" placeholder="请输入版本" />
      </el-form-item>
      <el-form-item label="备注" prop="remark">
        <el-input v-model="form.remark" type="textarea" :rows="3" placeholder="请输入备注" />
      </el-form-item>
    </el-form>
    <template #footer>
      <el-button @click="$emit('update:visible', false)">关闭</el-button>
      <el-button v-if="!readonly" type="primary" :loading="submitting" @click="handleSubmit">保存</el-button>
    </template>
  </el-dialog>
</template>

<script setup>
import { ref, watch, computed, onMounted } from 'vue'
import { ElMessage } from 'element-plus'
import { softwareApi, equipmentApi } from '@/api'

const props = defineProps({
  visible: {
    type: Boolean,
    default: false
  },
  data: {
    type: Object,
    default: null
  },
  readonly: {
    type: Boolean,
    default: false
  }
})

const emit = defineEmits(['update:visible', 'success'])

const formRef = ref(null)
const submitting = ref(false)
const equipmentList = ref([])

const defaultForm = {
  code: '',
  name: '',
  equipmentId: null,
  systemName: '',
  softwareType: '',
  vendor: '',
  licenseType: '',
  version: '',
  remark: ''
}

const form = ref({ ...defaultForm })

const dialogTitle = computed(() => {
  if (props.readonly) return '查看软件'
  return props.data?.id ? '编辑软件' : '新增软件'
})

const loadEquipment = async () => {
  try {
    const res = await equipmentApi.getList({ page: 1, pageSize: 999 })
    equipmentList.value = res.items || []
  } catch (e) {
    console.error('Load equipment error:', e)
  }
}

const onEquipmentChange = (id) => {
  const eq = equipmentList.value.find(item => item.id === id)
  form.value.systemName = eq?.systemName || ''
}

watch(() => props.visible, (val) => {
  if (val) {
    form.value = props.data ? { ...defaultForm, ...props.data } : { ...defaultForm }
  }
})

const handleSubmit = async () => {
  try {
    submitting.value = true
    if (props.data?.id) {
      await softwareApi.update(props.data.id, form.value)
      ElMessage.success('更新成功')
    } else {
      await softwareApi.create(form.value)
      ElMessage.success('创建成功')
    }
    emit('success')
    emit('update:visible', false)
  } catch (e) {
    console.error('Submit software error:', e)
    ElMessage.error(e?.response?.data?.message || '操作失败')
  } finally {
    submitting.value = false
  }
}

onMounted(loadEquipment)
</script>
