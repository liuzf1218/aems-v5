import { createRouter, createWebHistory } from 'vue-router'

const routes = [
  {
    path: '/login',
    name: 'Login',
    component: () => import('@/views/login/index.vue'),
    meta: { title: '登录', public: true }
  },
  {
    path: '/',
    component: () => import('@/layout/index.vue'),
    redirect: '/dashboard',
    children: [
      {
        path: 'dashboard',
        name: 'Dashboard',
        component: () => import('@/views/dashboard/index.vue'),
        meta: { title: '首页驾驶舱', icon: 'Odometer' }
      }
    ]
  },
  {
    path: '/equipment',
    component: () => import('@/layout/index.vue'),
    children: [
      { path: 'list', name: 'EquipmentList', component: () => import('@/views/equipment/list/index.vue'), meta: { title: '设备台账' } },
      { path: 'tree', name: 'EquipmentTree', component: () => import('@/views/equipment/tree/index.vue'), meta: { title: '设备分类' } },
      { path: 'health', name: 'EquipmentHealth', component: () => import('@/views/equipment/health/index.vue'), meta: { title: '设备健康' } },
      { path: 'detail/:id', name: 'EquipmentDetail', component: () => import('@/views/equipment/detail/index.vue'), meta: { title: '设备详情', hidden: true } }
    ]
  },
  {
    path: '/workorder',
    component: () => import('@/layout/index.vue'),
    children: [
      { path: 'list', name: 'WorkorderList', component: () => import('@/views/workorder/list/index.vue'), meta: { title: '全部工单' } },
      { path: 'create', name: 'WorkorderCreate', component: () => import('@/views/workorder/create/index.vue'), meta: { title: '创建工单' } },
      { path: 'detail/:id', name: 'WorkorderDetail', component: () => import('@/views/workorder/detail/index.vue'), meta: { title: '工单详情', hidden: true } },
      { path: 'inspection', name: 'Inspection', component: () => import('@/views/workorder/inspection/index.vue'), meta: { title: '巡检工单' } },
      { path: 'fault', name: 'Fault', component: () => import('@/views/workorder/fault/index.vue'), meta: { title: '故障工单' } }
    ]
  },
  {
    path: '/maintenance',
    component: () => import('@/layout/index.vue'),
    children: [
      { path: 'plans', name: 'MaintenancePlans', component: () => import('@/views/maintenance/plans/index.vue'), meta: { title: '维护计划' } },
      { path: 'tasks', name: 'MaintenanceTasks', component: () => import('@/views/maintenance/tasks/index.vue'), meta: { title: '维护任务' } }
    ]
  },
  {
    path: '/software',
    component: () => import('@/layout/index.vue'),
    children: [
      { path: 'list', name: 'SoftwareList', component: () => import('@/views/software/list/index.vue'), meta: { title: '软件列表' } },
      { path: 'instances', name: 'SoftwareInstances', component: () => import('@/views/software/instances/index.vue'), meta: { title: '软件实例' } },
      { path: 'detail/:id', name: 'SoftwareDetail', component: () => import('@/views/software/detail/index.vue'), meta: { title: '软件详情', hidden: true } }
    ]
  },
  {
    path: '/sparepart',
    component: () => import('@/layout/index.vue'),
    children: [
      { path: 'list', name: 'SparePartList', component: () => import('@/views/sparepart/list/index.vue'), meta: { title: '备件列表' } },
      { path: 'detail/:id', name: 'SparePartDetail', component: () => import('@/views/sparepart/detail/index.vue'), meta: { title: '备件详情', hidden: true } },
      { path: 'stock', name: 'SparePartStock', component: () => import('@/views/sparepart/stock/index.vue'), meta: { title: '出入库管理' } },
      { path: 'warnings', name: 'SparePartWarnings', component: () => import('@/views/sparepart/warnings/index.vue'), meta: { title: '库存预警' } }
    ]
  },
  {
    path: '/document',
    component: () => import('@/layout/index.vue'),
    children: [
      { path: 'list', name: 'DocumentList', component: () => import('@/views/document/list/index.vue'), meta: { title: '文档列表' } },
      { path: 'versions/:id', name: 'DocumentVersions', component: () => import('@/views/document/versions/index.vue'), meta: { title: '版本管理', hidden: true } }
    ]
  },
  {
    path: '/room',
    component: () => import('@/layout/index.vue'),
    children: [
      { path: 'list', name: 'RoomList', component: () => import('@/views/room/list/index.vue'), meta: { title: '机房管理' } },
      { path: 'tree', name: 'RoomTree', component: () => import('@/views/room/tree/index.vue'), meta: { title: '机房分类' } },
      { path: 'detail/:id', name: 'RoomDetail', component: () => import('@/views/room/detail/index.vue'), meta: { title: '机房详情', hidden: true } }
    ]
  },
  {
    path: '/statistics',
    component: () => import('@/layout/index.vue'),
    children: [
      { path: '', name: 'Statistics', component: () => import('@/views/statistics/index.vue'), meta: { title: '统计分析' } }
    ]
  },
  {
    path: '/system',
    component: () => import('@/layout/index.vue'),
    children: [
      { path: 'user', name: 'SystemUser', component: () => import('@/views/system/user/index.vue'), meta: { title: '用户管理' } },
      { path: 'log', name: 'SystemLog', component: () => import('@/views/system/log/index.vue'), meta: { title: '操作日志' } },
      { path: 'settings', name: 'SystemSettings', component: () => import('@/views/system/settings/index.vue'), meta: { title: '系统设置' } }
    ]
  },
  {
    path: '/:pathMatch(.*)*',
    redirect: '/dashboard'
  }
]

const router = createRouter({
  history: createWebHistory(),
  routes,
  scrollBehavior: () => ({ top: 0 })
})

router.beforeEach((to, from, next) => {
  document.title = (to.meta.title || '设备管理系统') + ' - 设备管理系统'

  if (to.meta.public) {
    next()
    return
  }

  const token = localStorage.getItem('aems_token')
  if (!token) {
    next({ path: '/login', query: { redirect: to.fullPath } })
  } else {
    next()
  }
})

export default router
