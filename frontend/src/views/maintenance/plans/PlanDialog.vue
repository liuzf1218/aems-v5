<template>
  <el-dialog
    :title="title"
    v-model="dialogVisible"
    width="600px"
    :close-on-click-modal="false"
    @closed="resetForm"
  >
    <el-form
      ref="formRef"
      :model="form"
      :rules="rules"
      label-width="100px"
      :disabled="readonly"
    >
      <el-form-item label="计划编号" prop="planNo">
        <el-input v-model="form.planNo" placeholder="请输入计划编号" />
      </el-form-item>
      <el-form-item label="计划名称" prop="name">
        <el-input v-model="form.name" placeholder="请输入计划名称" />
      </el-form-item>
      <el-form-item label="关联设备" prop="equipmentId">
        <el-select v-model="form.equipmentId" placeholder="请选择关联设备" style="width:100%" @change="onEquipmentChange">
          <el-option v-for="eq in equipmentList" :key="eq.id" :label="eq.name" :value="eq.id" />
        </el-select>
      </el-form-item>
      <el-form-item label="所属系统">
        <el-input v-model="form.systemName" placeholder="选择设备后自动显示" disabled />
      </el-form-item>
      <el-form-item label="计划类型" prop="planType">
        <el-select v-model="form.planType" placeholder="请选择" style="width:100%">
          <el-option label="日常保养" :value="1" />
          <el-option label="定期检修" :value="2" />
          <el-option label="年度大修" :value="3" />
        </el-select>
      </el-form-item>
      <el-form-item label="周期(天)" prop="cycleDays">
        <el-input-number v-model="form.cycleDays" :min="1" style="width:100%" />
      </el-form-item>
      <el-form-item label="开始日期" prop="startDate">
        <el-date-picker v-model="form.startDate" type="date" placeholder="选择日期" style="width:100%" value-format="YYYY-MM-DD" />
      </el-form-item>
      <el-form-item label="状态" prop="status">
        <el-select v-model="form.status" placeholder="请选择" style="width:100%">
          <el-option label="启用" :value="1" />
          <el-option label="停用" :value="0" />
        </el-select>
      </el-form-item>
      <el-form-item label="负责人" prop="responsiblePerson">
        <el-input v-model="form.responsiblePerson" placeholder="请输入负责人" />
      </el-form-item>
      <el-form-item label="维护内容" prop="content">
        <el-input v-model="form.content" type="textarea" :rows="3" placeholder="请输入维护内容" />
      </el-form-item>
    </el-form>
    <template #footer>
      <el-button @click="dialogVisible = false">关闭</el-button>
      <el-button v-if="!readonly" type="primary" @click="handleSubmit" :loading="submitting">保存</el-button>
    </template>
  </el-dialog>
</template>

<script setup>
import { ref, computed, watch, onMounted } from 'vue'
import { ElMessage } from 'element-plus'
import { maintenanceApi, equipmentApi } from '@/api'

const props = defineProps({
  visible: Boolean,
  data: Object,
  readonly: { type: Boolean, default: false }
})

const emit = defineEmits(['update:visible', 'success'])

const dialogVisible = computed({
  get: () => props.visible,
  set: (val) => emit('update:visible', val)
})

const title = computed(() => {
  if (props.readonly) return '查看维护计划'
  if (props.data?.id) return '编辑维护计划'
  return '新增维护计划'
})

const formRef = ref(null)
const submitting = ref(false)
const equipmentList = ref([])

const form = ref({
  planNo: '',
  name: '',
  equipmentId: null,
  systemName: '',
  planType: 1,
  cycleDays: 30,
  startDate: '',
  status: 1,
  responsiblePerson: '',
  content: ''
})

const rules = {
  planNo: [{ required: true, message: '请输入计划编号', trigger: 'blur' }],
  name: [{ required: true, message: '请输入计划名称', trigger: 'blur' }],
  planType: [{ required: true, message: '请选择计划类型', trigger: 'change' }],
  cycleDays: [{ required: true, message: '请输入周期天数', trigger: 'blur' }]
}

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

watch(() => props.data, (val) => {
  if (val) {
    form.value = { ...val }
  } else {
    resetForm()
  }
}, { immediate: true })

function resetForm() {
  form.value = {
    planNo: '',
    name: '',
    equipmentId: null,
    systemName: '',
    planType: 1,
    cycleDays: 30,
    startDate: '',
    status: 1,
    responsiblePerson: '',
    content: ''
  }
  formRef.value?.resetFields()
}

async function handleSubmit() {
  const valid = await formRef.value?.validate().catch(() => false)
  if (!valid) return

  submitting.value = true
  try {
    if (props.data?.id) {
      await maintenanceApi.updatePlan(props.data.id, form.value)
      ElMessage.success('更新成功')
    } else {
      await maintenanceApi.createPlan(form.value)
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

onMounted(loadEquipment)
</script>
