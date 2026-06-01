<template>
  <div class="page-container">
    <el-card shadow="never">
      <template #header><span>操作日志</span></template>
      <el-form inline style="margin-bottom:16px">
        <el-form-item><el-input placeholder="搜索操作人/内容" clearable /></el-form-item>
        <el-form-item><el-date-picker type="daterange" range-separator="至" start-placeholder="开始日期" end-placeholder="结束日期" style="width:260px" /></el-form-item>
        <el-form-item><el-button type="primary">搜索</el-button></el-form-item>
      </el-form>
      <el-table :data="logs" stripe>
        <el-table-column prop="time" label="时间" width="160" />
        <el-table-column prop="user" label="操作人" width="90" />
        <el-table-column prop="action" label="操作" width="100" />
        <el-table-column prop="target" label="操作对象" min-width="150" />
        <el-table-column prop="ip" label="IP地址" width="130" />
        <el-table-column prop="result" label="结果" width="80" align="center">
          <template #default="{ row }"><el-tag :type="row.result === '成功' ? 'success' : 'danger'" size="small">{{ row.result }}</el-tag></template>
        </el-table-column>
      </el-table>
    </el-card>
  </div>
</template>
<script setup>
import { ref } from 'vue'
const logs = ref([
  { time: '2026-03-31 05:00', user: 'admin', action: '登录', target: '系统', ip: '10.0.1.1', result: '成功' },
  { time: '2026-03-31 04:30', user: '张三', action: '创建工单', target: 'WO-20260331-001', ip: '10.0.1.5', result: '成功' },
  { time: '2026-03-31 03:00', user: '李四', action: '编辑设备', target: 'NAV-001', ip: '10.0.1.8', result: '成功' }
])
</script>
