<template>
  <div class="sidebar">
    <div class="logo">
      <el-icon :size="24" color="#fff"><Setting /></el-icon>
      <span class="logo-text" v-show="!collapsed">设备管理系统</span>
    </div>
    <el-scrollbar>
      <el-menu
        :default-active="route.path"
        :collapse="collapsed"
        background-color="#1E3A5F"
        text-color="#94B8D0"
        active-text-color="#FFFFFF"
        :router="true"
        :unique-opened="true"
      >
        <el-menu-item index="/dashboard">
          <el-icon><DataBoard /></el-icon>
          <template #title>首页驾驶舱</template>
        </el-menu-item>
        <el-sub-menu index="/equipment">
          <template #title><el-icon><Monitor /></el-icon><span>设备管理</span></template>
          <el-menu-item index="/equipment/list">设备台账</el-menu-item>
          <el-menu-item index="/equipment/tree">设备分类</el-menu-item>
          <el-menu-item index="/equipment/health">设备健康</el-menu-item>
        </el-sub-menu>
        <el-sub-menu index="/software">
          <template #title><el-icon><Connection /></el-icon><span>软件管理</span></template>
          <el-menu-item index="/software/list">软件列表</el-menu-item>
          <el-menu-item index="/software/instances">软件实例</el-menu-item>
        </el-sub-menu>
        <el-sub-menu index="/workorder">
          <template #title><el-icon><Document /></el-icon><span>工单管理</span></template>
          <el-menu-item index="/workorder/inspection">巡检工单</el-menu-item>
          <el-menu-item index="/workorder/fault">故障管理</el-menu-item>
        </el-sub-menu>
        <el-sub-menu index="/maintenance">
          <template #title><el-icon><Calendar /></el-icon><span>维护管理</span></template>
          <el-menu-item index="/maintenance/plans">维护计划</el-menu-item>
          <el-menu-item index="/maintenance/tasks">维护任务</el-menu-item>
        </el-sub-menu>
        <el-sub-menu index="/sparepart">
          <template #title><el-icon><Box /></el-icon><span>备件管理</span></template>
          <el-menu-item index="/sparepart/list">备件列表</el-menu-item>
          <el-menu-item index="/sparepart/stock">出入库管理</el-menu-item>
          <el-menu-item index="/sparepart/warnings">库存预警</el-menu-item>
        </el-sub-menu>
        <el-sub-menu index="/document">
          <template #title><el-icon><Folder /></el-icon><span>文档管理</span></template>
          <el-menu-item index="/document/list">文档列表</el-menu-item>
        </el-sub-menu>
        <el-sub-menu index="/system">
          <template #title><el-icon><Setting /></el-icon><span>系统设置</span></template>
          <el-menu-item index="/system/user">用户管理</el-menu-item>
          <el-menu-item index="/system/log">操作日志</el-menu-item>
        </el-sub-menu>
      </el-menu>
    </el-scrollbar>
  </div>
</template>
<script setup>
import { useRoute } from 'vue-router'
import { DataBoard, Monitor, Connection, Document, Calendar, Box, Folder, Setting } from '@element-plus/icons-vue'
defineProps({ collapsed: Boolean })
const route = useRoute()
</script>
<style scoped>
.sidebar { height: 100%; display: flex; flex-direction: column; }
.logo {
  height: 50px;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 8px;
  background: #152A45;
  color: #fff;
}
.logo-icon { font-size: 24px; }
.logo-text { font-size: 18px; font-weight: bold; letter-spacing: 2px; }
.el-menu { border-right: none; flex: 1; overflow-y: auto; }
.el-menu-item, :deep(.el-sub-menu__title) { height: 44px !important; line-height: 44px !important; }
.el-menu-item.is-active { background: #3B82F6 !important; }
:deep(.el-sub-menu .el-menu) { background: #162D4D !important; }
:deep(.el-sub-menu .el-menu-item) { background: #162D4D !important; }
:deep(.el-sub-menu .el-menu-item.is-active) { background: #3B82F6 !important; color: #FFF !important; }
:deep(.el-sub-menu .el-menu-item:hover) { background: #2A5080 !important; color: #FFF !important; }
:deep(.el-sub-menu__title:hover) { background: #2A5080 !important; }
</style>
