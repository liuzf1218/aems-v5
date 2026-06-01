<template>
  <div class="page-container">
    <el-card shadow="never">
      <template #header><span>系统设置</span></template>
      <el-tabs v-model="activeTab">
        <el-tab-pane label="基本设置" name="basic">
          <el-form :model="basicSettings" label-width="120px" style="max-width:600px">
            <el-form-item label="系统名称"><el-input v-model="basicSettings.name" /></el-form-item>
            <el-form-item label="系统Logo"><el-upload action="#" :auto-upload="false"><el-button>选择图片</el-button></el-upload></el-form-item>
            <el-form-item label="登录超时"><el-input-number v-model="basicSettings.timeout" :min="5" :max="120" /> 分钟</el-form-item>
            <el-form-item><el-button type="primary" :loading="saving" @click="saveSettings('basic')">保存</el-button></el-form-item>
          </el-form>
        </el-tab-pane>
        <el-tab-pane label="通知设置" name="notify">
          <el-form :model="notifySettings" label-width="120px" style="max-width:600px">
            <el-form-item label="邮件通知"><el-switch v-model="notifySettings.email" /></el-form-item>
            <el-form-item label="短信通知"><el-switch v-model="notifySettings.sms" /></el-form-item>
            <el-form-item label="工单超时提醒"><el-switch v-model="notifySettings.workorderTimeout" /></el-form-item>
            <el-form-item label="库存预警通知"><el-switch v-model="notifySettings.stockWarning" /></el-form-item>
            <el-form-item><el-button type="primary" :loading="saving" @click="saveSettings('notify')">保存</el-button></el-form-item>
          </el-form>
        </el-tab-pane>
        <el-tab-pane label="工单设置" name="workorder">
          <el-form :model="workorderSettings" label-width="120px" style="max-width:600px">
            <el-form-item label="工单号前缀"><el-input v-model="workorderSettings.prefix" /></el-form-item>
            <el-form-item label="自动关闭天数"><el-input-number v-model="workorderSettings.autoCloseDays" :min="1" :max="30" /> 天</el-form-item>
            <el-form-item label="SLA超时时间"><el-input-number v-model="workorderSettings.slaHours" :min="1" :max="72" /> 小时</el-form-item>
            <el-form-item><el-button type="primary" :loading="saving" @click="saveSettings('workorder')">保存</el-button></el-form-item>
          </el-form>
        </el-tab-pane>
      </el-tabs>
    </el-card>
  </div>
</template>

<script setup>
import { ref, reactive, onMounted } from 'vue'
import { ElMessage } from 'element-plus'
import { settingsApi } from '@/api'

const activeTab = ref('basic')
const saving = ref(false)
const loading = ref(false)

const basicSettings = reactive({ name: '', timeout: 30 })
const notifySettings = reactive({ email: false, sms: false, workorderTimeout: false, stockWarning: false })
const workorderSettings = reactive({ prefix: '', autoCloseDays: 7, slaHours: 24 })

const categoryMap = { basic: basicSettings, notify: notifySettings, workorder: workorderSettings }

const loadSettings = async () => {
  loading.value = true
  try {
    for (const cat of ['basic', 'notify', 'workorder']) {
      const res = await settingsApi.get(cat)
      if (res) {
        Object.assign(categoryMap[cat], res)
      }
    }
  } catch (e) {
    console.error('Load settings error:', e)
    ElMessage.warning('加载设置失败，使用默认值')
  } finally {
    loading.value = false
  }
}

const saveSettings = async (category) => {
  saving.value = true
  try {
    await settingsApi.save({ category, data: categoryMap[category] })
    ElMessage.success('设置保存成功')
  } catch (e) {
    console.error('Save settings error:', e)
    ElMessage.error('保存失败')
  } finally {
    saving.value = false
  }
}

onMounted(loadSettings)
</script>
