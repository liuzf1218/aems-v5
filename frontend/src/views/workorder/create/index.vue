<template>
  <div class="workorder-create">
    <el-card shadow="never">
      <template #header>
        <div class="card-header">
          <span>创建工单</span>
          <el-button @click="$router.back()">返回</el-button>
        </div>
      </template>

      <el-steps :active="currentStep" finish-status="success" align-center class="steps">
        <el-step title="基本信息" />
        <el-step title="详细描述" />
        <el-step title="确认提交" />
      </el-steps>

      <el-form ref="formRef" :model="formData" :rules="formRules" label-width="100px" class="create-form">
        <!-- 步骤1：基本信息 -->
        <div v-show="currentStep === 0">
          <el-row :gutter="24">
            <el-col :span="12">
              <el-form-item label="工单类型" prop="type">
                <el-select v-model="formData.type" style="width:100%">
                  <el-option label="巡检" :value="1" />
                  <el-option label="故障维修" :value="2" />
                  <el-option label="维护" :value="3" />
                  <el-option label="校准" :value="4" />
                </el-select>
              </el-form-item>
            </el-col>
            <el-col :span="12">
              <el-form-item label="优先级" prop="priority">
                <el-radio-group v-model="formData.priority">
                  <el-radio-button :value="1"><span style="color:#f56c6c">紧急</span></el-radio-button>
                  <el-radio-button :value="2"><span style="color:#e6a23c">高</span></el-radio-button>
                  <el-radio-button :value="3">中</el-radio-button>
                  <el-radio-button :value="4">低</el-radio-button>
                </el-radio-group>
              </el-form-item>
            </el-col>
            <el-col :span="12">
              <el-form-item label="关联设备" prop="equipmentId">
                <el-select
                  v-model="formData.equipmentId"
                  filterable
                  remote
                  :remote-method="searchEquipment"
                  placeholder="搜索设备编号或名称"
                  style="width:100%"
                  @change="onEquipmentChange"
                >
                  <el-option
                    v-for="eq in equipmentOptions"
                    :key="eq.id"
                    :label="`${eq.code} - ${eq.name}`"
                    :value="eq.id"
                  />
                </el-select>
              </el-form-item>
            </el-col>
            <el-col :span="12">
              <el-form-item label="所属系统">
                <el-input v-model="formData.systemName" readonly placeholder="选择设备后自动显示" style="width:100%" />
              </el-form-item>
            </el-col>
            <el-col :span="12">
              <el-form-item label="处理人" prop="assignee">
                <el-select v-model="formData.assignee" style="width:100%">
                  <el-option label="张三" value="张三" />
                  <el-option label="李四" value="李四" />
                  <el-option label="王五" value="王五" />
                  <el-option label="赵六" value="赵六" />
                </el-select>
              </el-form-item>
            </el-col>
            <el-col :span="12">
              <el-form-item label="截止时间" prop="deadline">
                <el-date-picker v-model="formData.deadline" type="datetime" style="width:100%" placeholder="选择截止时间" />
              </el-form-item>
            </el-col>
          </el-row>
        </div>

        <!-- 步骤2：详细描述 -->
        <div v-show="currentStep === 1">
          <el-form-item label="工单标题" prop="title">
            <el-input v-model="formData.title" placeholder="请输入工单标题，如：导航设备NAV-001温度异常" />
          </el-form-item>
          <el-form-item label="故障现象" prop="symptom">
            <el-input v-model="formData.symptom" type="textarea" :rows="3" placeholder="描述故障现象或巡检要求" />
          </el-form-item>
          <el-form-item label="处理方案">
            <el-input v-model="formData.solution" type="textarea" :rows="3" placeholder="初步处理方案（选填）" />
          </el-form-item>
          <el-form-item label="附件">
            <el-upload action="#" :auto-upload="false" :on-change="handleFileChange" multiple>
              <el-button type="primary"><el-icon><Upload /></el-icon>上传附件</el-button>
            </el-upload>
          </el-form-item>
        </div>

        <!-- 步骤3：确认 -->
        <div v-show="currentStep === 2">
          <el-descriptions :column="2" border>
            <el-descriptions-item label="工单类型">{{ getTypeLabel(formData.type) }}</el-descriptions-item>
            <el-descriptions-item label="优先级">
              <el-tag :type="getPriorityType(formData.priority)" size="small">{{ formData.priority }}</el-tag>
            </el-descriptions-item>
            <el-descriptions-item label="关联设备">{{ formData.equipmentName || '未选择' }}</el-descriptions-item>
            <el-descriptions-item label="所属系统">{{ formData.systemName || '未选择' }}</el-descriptions-item>
            <el-descriptions-item label="处理人">{{ formData.assignee }}</el-descriptions-item>
            <el-descriptions-item label="截止时间">{{ formData.deadline }}</el-descriptions-item>
            <el-descriptions-item label="工单标题">{{ formData.title }}</el-descriptions-item>
            <el-descriptions-item label="故障现象" :span="2">{{ formData.symptom }}</el-descriptions-item>
            <el-descriptions-item label="处理方案" :span="2">{{ formData.solution || '无' }}</el-descriptions-item>
          </el-descriptions>
        </div>

        <div class="step-actions">
          <el-button v-if="currentStep > 0" @click="currentStep--">上一步</el-button>
          <el-button v-if="currentStep < 2" type="primary" @click="nextStep">下一步</el-button>
          <el-button v-if="currentStep === 2" type="primary" :loading="submitting" @click="handleSubmit">提交工单</el-button>
        </div>
      </el-form>
    </el-card>
  </div>
</template>

<script setup>
import { ref, reactive, onMounted } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { ElMessage } from 'element-plus'
import { Upload } from '@element-plus/icons-vue'
import { useEquipmentStore } from '@/store/equipment'
import { useWorkorderStore } from '@/store/workorder'
import { mapValue, WORKORDER_TYPE_MAP, PRIORITY_MAP } from '@/utils/statusMap'

const router = useRouter()
const route = useRoute()
const equipmentStore = useEquipmentStore()
const workorderStore = useWorkorderStore()
const formRef = ref(null)
const currentStep = ref(0)
const submitting = ref(false)
const equipmentOptions = ref([])

const formData = reactive({
  type: 2,
  priority: 3,
  equipmentId: route.query.equipmentId || '',
  equipmentName: '',
  systemName: '',
  assignee: '',
  deadline: '',
  title: '',
  symptom: '',
  solution: '',
  attachments: []
})

const formRules = {
  type: [{ required: true, message: '请选择工单类型', trigger: 'change' }],
  priority: [{ required: true, message: '请选择优先级', trigger: 'change' }],
  equipmentId: [{ required: true, message: '请选择关联设备', trigger: 'change' }],
  assignee: [{ required: true, message: '请选择处理人', trigger: 'change' }],
  deadline: [{ required: true, message: '请选择截止时间', trigger: 'change' }],
  title: [{ required: true, message: '请输入工单标题', trigger: 'blur' }],
  symptom: [{ required: true, message: '请输入故障现象', trigger: 'blur' }]
}

const getTypeLabel = (v) => mapValue(WORKORDER_TYPE_MAP, v, 'label', v)
const getPriorityType = (v) => mapValue(PRIORITY_MAP, v, 'type', 'info')

const searchEquipment = async (query) => {
  if (!query) return
  const res = await equipmentStore.fetchList({ keyword: query, pageSize: 10 })
  equipmentOptions.value = res.list || []
}

const onEquipmentChange = (id) => {
  const eq = equipmentOptions.value.find(e => e.id === id)
  if (eq) {
    formData.equipmentName = eq.name
    formData.systemName = eq.systemName || ''
  }
}

const handleFileChange = (file) => {
  formData.attachments.push(file)
}

const nextStep = async () => {
  // 验证当前步骤
  const fields = [
    ['type', 'priority', 'equipmentId', 'assignee', 'deadline'],
    ['title', 'symptom']
  ]
  const valid = await formRef.value?.validateField(fields[currentStep.value]).catch(() => false)
  if (valid !== false) currentStep.value++
}

const handleSubmit = async () => {
  submitting.value = true
  try {
    await workorderStore.create({
      title: formData.title,
      type: formData.type,
      priority: formData.priority,
      equipmentId: formData.equipmentId,
      assigneeName: formData.assignee,
      deadline: formData.deadline,
      description: formData.description || formData.symptom,
      symptom: formData.symptom,
      solution: formData.solution
    })
    ElMessage.success('工单创建成功！')
    router.push('/workorder/list')
  } catch (e) {
    ElMessage.error('创建失败')
  } finally {
    submitting.value = false
  }
}

onMounted(() => {
  // 如果有预选设备，加载信息
  if (formData.equipmentId) {
    equipmentStore.getById(formData.equipmentId).then(eq => {
      if (eq) {
        formData.equipmentName = eq.name
        formData.systemName = eq.systemName || ''
        equipmentOptions.value = [eq]
      }
    })
  }
})
</script>

<style scoped lang="scss">
.workorder-create {
  max-width: 900px;
  margin: 0 auto;
}

.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.steps {
  margin-bottom: 32px;
}

.create-form {
  padding: 16px 0;
}

.step-actions {
  display: flex;
  justify-content: center;
  gap: 16px;
  margin-top: 32px;
  padding-top: 24px;
  border-top: 1px solid #f0f0f0;
}
</style>
