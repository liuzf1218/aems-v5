<template>
  <el-dialog
    :title="title"
    v-model="dialogVisible"
    width="560px"
    :close-on-click-modal="false"
    @closed="resetForm"
  >
    <el-form ref="formRef" :model="form" :rules="rules" label-width="90px" :disabled="readonly">
      <el-form-item label="编号" prop="code">
        <el-input v-model="form.code" placeholder="请输入编号" />
      </el-form-item>
      <el-form-item label="备件名称" prop="name">
        <el-input v-model="form.name" placeholder="请输入备件名称" />
      </el-form-item>
      <el-form-item label="规格型号" prop="model">
        <el-input v-model="form.model" placeholder="请输入规格型号" />
      </el-form-item>
      <el-form-item label="分类" prop="category">
        <el-select v-model="form.category" placeholder="请选择" style="width:100%">
          <el-option label="电子元件" value="电子元件" />
          <el-option label="机械部件" value="机械部件" />
          <el-option label="电源" value="电源" />
          <el-option label="网络" value="网络" />
          <el-option label="耗材" value="耗材" />
        </el-select>
      </el-form-item>
      <el-form-item label="所属系统" prop="systemId">
        <el-select v-model="form.systemId" placeholder="请选择所属系统" style="width:100%">
          <el-option v-for="sys in systems" :key="sys.id" :label="sys.name" :value="sys.id" />
        </el-select>
      </el-form-item>
      <el-row :gutter="16">
        <el-col :span="12">
          <el-form-item label="库存" prop="stock">
            <el-input-number v-model="form.stock" :min="0" style="width:100%" />
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="最低库存" prop="minStock">
            <el-input-number v-model="form.minStock" :min="0" style="width:100%" />
          </el-form-item>
        </el-col>
      </el-row>
      <el-form-item label="单价" prop="price">
        <el-input-number v-model="form.price" :min="0" :precision="2" style="width:100%" />
      </el-form-item>
      <el-form-item label="库位" prop="location">
        <el-input v-model="form.location" placeholder="请输入库位" />
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
import { sparepartApi, equipmentApi } from '@/api'

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
  if (props.readonly) return '查看备件'
  if (props.data?.id) return '编辑备件'
  return '新增备件'
})

const formRef = ref(null)
const submitting = ref(false)
const systems = ref([])

const form = ref({ code: '', name: '', model: '', category: '', systemId: null, stock: 0, minStock: 0, price: 0, location: '' })

const rules = {
  code: [{ required: true, message: '请输入编号', trigger: 'blur' }],
  name: [{ required: true, message: '请输入备件名称', trigger: 'blur' }],
  category: [{ required: true, message: '请选择分类', trigger: 'change' }]
}

const loadSystems = async () => {
  try {
    const res = await equipmentApi.getTree()
    const tree = Array.isArray(res) ? res : (res || [])
    const extract = (nodes) => {
      const result = []
      for (const node of nodes || []) {
        if (node.nodeType === 'system') result.push(node)
        if (node.children?.length) result.push(...extract(node.children))
      }
      return result
    }
    systems.value = extract(tree)
  } catch (e) {
    console.error('Load systems error:', e)
  }
}

watch(() => props.data, (val) => {
  if (val) form.value = { ...val }
  else resetForm()
}, { immediate: true })

function resetForm() {
  form.value = { code: '', name: '', model: '', category: '', systemId: null, stock: 0, minStock: 0, price: 0, location: '' }
  formRef.value?.resetFields()
}

async function handleSubmit() {
  const valid = await formRef.value?.validate().catch(() => false)
  if (!valid) return
  submitting.value = true
  try {
    if (props.data?.id) {
      await sparepartApi.update(props.data.id, form.value)
      ElMessage.success('更新成功')
    } else {
      await sparepartApi.create(form.value)
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

onMounted(loadSystems)
</script>
