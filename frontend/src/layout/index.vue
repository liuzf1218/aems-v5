<template>
  <el-container class="layout-container">
    <!-- 侧边栏 -->
    <el-aside :width="isCollapsed ? '64px' : '210px'" class="layout-aside">
      <div class="logo-container">
        <el-icon :size="28" color="#409EFF"><Tools /></el-icon>
        <transition name="fade">
          <span v-if="!isCollapsed" class="logo-text">设备管理系统</span>
        </transition>
      </div>

      <el-menu
        :default-active="activeMenu"
        :collapse="isCollapsed"
        :router="true"
        background-color="#001529"
        text-color="#ffffffb3"
        active-text-color="#409EFF"
        class="sidebar-menu"
      >
        <el-menu-item index="/dashboard">
          <el-icon><Odometer /></el-icon>
          <template #title>首页驾驶舱</template>
        </el-menu-item>

        <el-sub-menu index="equipment">
          <template #title>
            <el-icon><Monitor /></el-icon>
            <span>设备管理</span>
          </template>
          <el-menu-item index="/equipment/list">设备台账</el-menu-item>
          <el-menu-item index="/equipment/tree">设备分类</el-menu-item>
          <el-menu-item index="/equipment/health">设备健康</el-menu-item>
        </el-sub-menu>

        <el-sub-menu index="workorder">
          <template #title>
            <el-icon><Document /></el-icon>
            <span>工单管理</span>
          </template>
          <el-menu-item index="/workorder/list">全部工单</el-menu-item>
          <el-menu-item index="/workorder/inspection">巡检工单</el-menu-item>
          <el-menu-item index="/workorder/fault">故障工单</el-menu-item>
        </el-sub-menu>

        <el-sub-menu index="maintenance">
          <template #title>
            <el-icon><Setting /></el-icon>
            <span>维保管理</span>
          </template>
          <el-menu-item index="/maintenance/plans">维护计划</el-menu-item>
          <el-menu-item index="/maintenance/tasks">维护任务</el-menu-item>
        </el-sub-menu>

        <el-sub-menu index="room">
          <template #title>
            <el-icon><OfficeBuilding /></el-icon>
            <span>机房管理</span>
          </template>
          <el-menu-item index="/room/tree">机房分类</el-menu-item>
        </el-sub-menu>

        <el-sub-menu index="software">
          <template #title>
            <el-icon><Monitor /></el-icon>
            <span>软件管理</span>
          </template>
          <el-menu-item index="/software/list">软件列表</el-menu-item>
          <el-menu-item index="/software/instances">软件实例</el-menu-item>
        </el-sub-menu>

        <el-sub-menu index="sparepart">
          <template #title>
            <el-icon><Box /></el-icon>
            <span>备件管理</span>
          </template>
          <el-menu-item index="/sparepart/list">备件列表</el-menu-item>
          <el-menu-item index="/sparepart/stock">出入库管理</el-menu-item>
          <el-menu-item index="/sparepart/warnings">库存预警</el-menu-item>
        </el-sub-menu>

        <el-sub-menu index="document">
          <template #title>
            <el-icon><Files /></el-icon>
            <span>文档管理</span>
          </template>
          <el-menu-item index="/document/list">文档列表</el-menu-item>
        </el-sub-menu>

        <el-sub-menu index="statistics">
          <template #title>
            <el-icon><DataAnalysis /></el-icon>
            <span>统计分析</span>
          </template>
          <el-menu-item index="/statistics">综合报表</el-menu-item>
        </el-sub-menu>

        <el-sub-menu index="system">
          <template #title>
            <el-icon><Tools /></el-icon>
            <span>系统管理</span>
          </template>
          <el-menu-item index="/system/user">用户管理</el-menu-item>
          <el-menu-item index="/system/log">操作日志</el-menu-item>
          <el-menu-item index="/system/settings">系统设置</el-menu-item>
        </el-sub-menu>
      </el-menu>
    </el-aside>

    <!-- 主体 -->
    <el-container class="layout-main">
      <el-header class="layout-header">
        <div class="header-left">
          <el-icon class="collapse-btn" @click="toggleCollapse">
            <Fold v-if="!isCollapsed" />
            <Expand v-else />
          </el-icon>
          <el-breadcrumb separator="/">
            <el-breadcrumb-item :to="{ path: '/' }">首页</el-breadcrumb-item>
            <el-breadcrumb-item v-if="currentRoute.meta?.title">
              {{ currentRoute.meta.title }}
            </el-breadcrumb-item>
          </el-breadcrumb>
        </div>

        <div class="header-right">
          <el-input
            v-model="searchKeyword"
            placeholder="搜索设备/工单..."
            :prefix-icon="Search"
            class="global-search"
            @keyup.enter="handleGlobalSearch"
          />
          <el-badge :value="3" class="notification-badge">
            <el-icon :size="20"><Bell /></el-icon>
          </el-badge>
          <el-dropdown @command="handleUserCommand">
            <div class="user-info">
              <el-avatar :size="32" :icon="UserFilled" />
              <span class="username">{{ userStore.username || '管理员' }}</span>
              <el-icon><ArrowDown /></el-icon>
            </div>
            <template #dropdown>
              <el-dropdown-menu>
                <el-dropdown-item command="profile">
                  <el-icon><User /></el-icon>个人中心
                </el-dropdown-item>
                <el-dropdown-item command="settings">
                  <el-icon><Setting /></el-icon>系统设置
                </el-dropdown-item>
                <el-dropdown-item divided command="logout">
                  <el-icon><SwitchButton /></el-icon>退出登录
                </el-dropdown-item>
              </el-dropdown-menu>
            </template>
          </el-dropdown>
        </div>
      </el-header>

      <el-main class="layout-content">
        <router-view v-slot="{ Component }">
          <transition name="fade-transform" mode="out-in">
            <component :is="Component" />
          </transition>
        </router-view>
      </el-main>
    </el-container>
  </el-container>
</template>

<script setup>
import { ref, computed, onMounted, onUnmounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { Search, UserFilled, Odometer, Monitor, Document, Setting, Box, Files, DataAnalysis, Tools, Fold, Expand, Bell, User, SwitchButton, ArrowDown, OfficeBuilding } from '@element-plus/icons-vue'
import { useUserStore } from '@/store/user'

const route = useRoute()
const router = useRouter()
const userStore = useUserStore()

const isCollapsed = ref(false)
const searchKeyword = ref('')

const currentRoute = computed(() => route)
const activeMenu = computed(() => {
  const path = route.path
  if (path.startsWith('/equipment')) return path
  if (path.startsWith('/workorder')) return path
  if (path.startsWith('/maintenance')) return path
  if (path.startsWith('/room')) return path
  if (path.startsWith('/software')) return path
  if (path.startsWith('/sparepart')) return path
  if (path.startsWith('/document')) return path
  if (path.startsWith('/system')) return path
  if (path.startsWith('/statistics')) return '/statistics'
  return '/dashboard'
})

const toggleCollapse = () => {
  isCollapsed.value = !isCollapsed.value
}

const handleGlobalSearch = () => {
  if (searchKeyword.value.trim()) {
    router.push({ path: '/equipment/list', query: { keyword: searchKeyword.value } })
  }
}

const handleUserCommand = (command) => {
  switch (command) {
    case 'profile': break
    case 'settings': router.push('/system/settings'); break
    case 'logout': userStore.logout(); router.push('/login'); break
  }
}

const handleKeydown = (e) => {
  if ((e.ctrlKey || e.metaKey) && e.key === 'k') {
    e.preventDefault()
    document.querySelector('.global-search input')?.focus()
  }
}

onMounted(() => { document.addEventListener('keydown', handleKeydown) })
onUnmounted(() => { document.removeEventListener('keydown', handleKeydown) })
</script>

<style scoped lang="scss">
.layout-container { width: 100%; height: 100vh; }
.layout-aside { background: #001529; transition: width 0.3s; overflow: hidden;
  .logo-container { height: 60px; display: flex; align-items: center; justify-content: center; gap: 8px; border-bottom: 1px solid #ffffff1a;
    .logo-icon { font-size: 24px; }
    .logo-text { font-size: 16px; font-weight: 700; color: #fff; white-space: nowrap; }
  }
  .sidebar-menu { border-right: none; &:not(.el-menu--collapse) { width: 210px; } }
}
.layout-main { display: flex; flex-direction: column; }
.layout-header { height: 60px; background: #fff; box-shadow: 0 1px 4px rgba(0,0,0,.08); display: flex; align-items: center; justify-content: space-between; padding: 0 20px;
  .header-left { display: flex; align-items: center; gap: 16px;
    .collapse-btn { font-size: 20px; cursor: pointer; color: #606266; &:hover { color: #409EFF; } }
  }
  .header-right { display: flex; align-items: center; gap: 20px;
    .global-search { width: 260px; }
    .notification-badge { cursor: pointer; color: #606266; &:hover { color: #409EFF; } }
    .user-info { display: flex; align-items: center; gap: 8px; cursor: pointer; padding: 4px 8px; border-radius: 4px; &:hover { background: #f5f7fa; }
      .username { font-size: 14px; color: #303133; }
    }
  }
}
.layout-content { background: #f0f2f5; padding: 16px; overflow-y: auto; flex: 1; }
.fade-transform-enter-active, .fade-transform-leave-active { transition: all 0.2s; }
.fade-transform-enter-from { opacity: 0; transform: translateX(-10px); }
.fade-transform-leave-to { opacity: 0; transform: translateX(10px); }
.fade-enter-active, .fade-leave-active { transition: opacity 0.3s; }
.fade-enter-from, .fade-leave-to { opacity: 0; }
</style>
