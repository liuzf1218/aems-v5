<template>
  <div class="page-container">
    <el-card shadow="never">
      <template #header><span>软件实例</span></template>
      <el-table :data="instances" stripe>
        <el-table-column prop="id" label="实例ID" width="120" />
        <el-table-column prop="software" label="软件" min-width="150" />
        <el-table-column prop="server" label="服务器" width="130" />
        <el-table-column prop="ip" label="IP地址" width="130" />
        <el-table-column prop="version" label="版本" width="80" />
        <el-table-column prop="status" label="状态" width="90" align="center">
          <template #default="{ row }"><el-tag :type="row.status === '运行中' ? 'success' : 'danger'" size="small">{{ row.status }}</el-tag></template>
        </el-table-column>
        <el-table-column prop="uptime" label="运行时长" width="100" />
      </el-table>
    </el-card>
  </div>
</template>
<script setup>
import { ref } from 'vue'
const instances = ref([
  { id: 'INS-001', software: '空管自动化系统', server: 'ATC-SRV-01', ip: '10.0.1.10', version: 'v3.2', status: '运行中', uptime: '45天' },
  { id: 'INS-002', software: '空管自动化系统', server: 'ATC-SRV-02', ip: '10.0.1.11', version: 'v3.2', status: '运行中', uptime: '45天' },
  { id: 'INS-003', software: '气象数据处理系统', server: 'MET-SRV-01', ip: '10.0.2.10', version: 'v2.1', status: '运行中', uptime: '30天' }
])
</script>
