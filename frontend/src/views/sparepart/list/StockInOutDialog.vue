<template>
  <el-dialog
    :title="title"
    v-model="dialogVisible"
    width="480px"
    :close-on-click-modal="false"
  >
    <el-form ref="formRef" :model="form" :rules="rules" label-width="90px">
      <el-form-item label="备件">
        <el-input :model-value="sparepart?.name" disabled />
      </el-form-item>
      <el-form-item label="编号">
        <el-input :model-value="sparepart?.code" disabled />
      </el-form-item>
      <el-form-item label="所属系统">
        <el-input :model-value="sparepart?.systemName || '-'" disabled />
      </el-form-item>
      <el-form-item label="存放位置">
        <el-input :model-value="sparepart?.location || '-'" disabled />
      </el-form-item>
      <el-form-item label="当前库存">
        <el-input :model-value="sparepart?.stockQuantity || sparepart?.stock" disabled />
      </el-form-item>
      <el-form-item label="编号">
        <el-input :model-value="sparepart?.code" disabled />
      </el-form-item>
      <el-form-item label="所属系统">
        <el-input :model-value="sparepart?.systemName" disabled />
      </el-form-item>
      <el-form-item label="存放位置">
        <el-input :model-value="sparepart?.location" disabled />
      </el-form-item>
      <el-form-item label="操作类型" prop="type">
        <el-radio-group v-model="form.type">
          <el-radio label="in">入库</el-radio>
          <el-radio label="out">出库</el-radio>
        </el-radio-group>
      </el-form-item>
      <el-form-item label="数量" prop="quantity">
        <el-input-number v-model="form.quantity" :min="1" style="width:100%" />
      </el-form-item>
      <el-form-item label="经办人" prop="operatorName">
        <el-input v-model="form.operatorName" placeholder="请输入经办人" />
      </el-form-item>
      <el-form-item label="备注" prop="remark">
        <el-input v-model="form.remark" type="textarea" :rows="2" placeholder="请输入备注" />
      </el-form-item>
    </el-form>
    <template #footer>
      <el-button @click="dialogVisible = false">关闭</el-button>
      <el-button type="primary" @click="handleSubmit" :loading="submitting">确认</el-button>
    </template>
  </el-dialog>
</template>

<script setup>
import { ref, computed } from 'vue'
import { ElMessage } from 'element-plus'
import { stockApi } from '@/api'

const props = defineProps({
  visible: Boolean,
  sparepart: Object
})
const emit = defineEmits(['update:visible', 'success'])

const dialogVisible = computed({
  get: () => props.visible,
  set: (val) => emit('update:visible', val)
})

const title = computed(() => (form.value.type === 'in' ? '备件入库' : '备件出库'))

const formRef = ref(null)
const submitting = ref(false)
const form = ref({ type: 'in', quantity: 1, operatorName: '', remark: '' })

const rules = {
  type: [{ required: true, message: '请选择操作类型', trigger: 'change' }],
  quantity: [{ required: true, message: '请输入数量', trigger: 'blur' }],
  operatorName: [{ required: true, message: '请输入经办人', trigger: 'blur' }]
}

async function handleSubmit() {
  const valid = await formRef.value?.validate().catch(() => false)
  if (!valid) return
  submitting.value = true
  try {
    const payload = {
      sparepartId: props.sparepart?.id,
      quantity: form.value.quantity,
      operatorName: form.value.operatorName,
      remark: form.value.remark
    }
    if (form.value.type === 'in') {
      await stockApi.createIn(payload)
      ElMessage.success('入库成功')
    } else {
      await stockApi.createOut(payload)
      ElMessage.success('出库成功')
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
