import request from '@/utils/request'

// ==================== 认证 ====================
export const authApi = {
  login: (data) => request.post('/auth/login', data),
  logout: () => request.post('/auth/logout'),
  getUserInfo: () => request.get('/auth/current')
}

// ==================== 设备 ====================
export const equipmentApi = {
  getList: (params) => request.get('/equipment', { params }),
  getById: (id) => request.get(`/equipment/${id}`),
  create: (data) => request.post('/equipment', data),
  update: (id, data) => request.put(`/equipment/${id}`, data),
  delete: (id) => request.delete(`/equipment/${id}`),
  getTree: () => request.get('/equipment/tree'),
  getHealth: () => request.get('/equipment/health')
}

// ==================== 工单 ====================
export const workorderApi = {
  getList: (params) => request.get('/workorder', { params }),
  getById: (id) => request.get(`/workorder/${id}`),
  create: (data) => request.post('/workorder', data),
  process: (id) => request.post(`/workorder/${id}/process`),
  accept: (id) => request.put(`/workorder/${id}/accept`),
  assign: (id) => request.put(`/workorder/${id}/assign`),
  complete: (id) => request.put(`/workorder/${id}/complete`),
  cancel: (id) => request.put(`/workorder/${id}/cancel`),
  getDevices: () => request.get('/workorder/devices'),
  getSystems: () => request.get('/workorder/systems')
}

// ==================== 软件 ====================
export const softwareApi = {
  getList: (params) => request.get('/software', { params }),
  getById: (id) => request.get(`/software/${id}`),
  create: (data) => request.post('/software', data),
  update: (id, data) => request.put(`/software/${id}`, data),
  delete: (id) => request.delete(`/software/${id}`),
  getInstances: () => request.get('/software/instances'),
  getVersions: (id) => request.get(`/software/${id}/versions`)
}

// ==================== 备件 ====================
export const sparepartApi = {
  getList: (params) => request.get('/sparepart', { params }),
  getById: (id) => request.get(`/sparepart/${id}`),
  create: (data) => request.post('/sparepart', data),
  update: (id, data) => request.put(`/sparepart/${id}`, data),
  delete: (id) => request.delete(`/sparepart/${id}`),
  getWarnings: () => request.get('/sparepart/warning'),
  getLocations: () => request.get('/sparepart/locations')
}

// ==================== 库存 ====================
export const stockApi = {
  getInList: (params) => request.get('/stock/in', { params }),
  getOutList: (params) => request.get('/stock/out', { params }),
  createIn: (data) => request.post('/stock/in', data),
  createOut: (data) => request.post('/stock/out', data)
}

// ==================== 楼宇 ====================
export const buildingApi = {
  getList: (params) => request.get('/building', { params }),
  getTree: () => request.get('/building/tree'),
  create: (data) => request.post('/building', data),
  update: (id, data) => request.put(`/building/${id}`, data),
  delete: (id) => request.delete(`/building/${id}`)
}

// ==================== 机房 ====================
export const roomApi = {
  getList: (params) => request.get('/room', { params }),
  getById: (id) => request.get(`/room/${id}`),
  create: (data) => request.post('/room', data),
  update: (id, data) => request.put(`/room/${id}`, data),
  delete: (id) => request.delete(`/room/${id}`),
  getDevices: (id) => request.get(`/room/${id}/devices`),
  getCabinets: (id) => request.get(`/room/${id}/cabinets`),
  getFacilities: (id) => request.get(`/room/${id}/facilities`),
  getSystems: (id) => request.get(`/room/${id}/systems`)
}

// ==================== 机柜 ====================
export const cabinetApi = {
  getList: (params) => request.get('/cabinet', { params }),
  getById: (id) => request.get(`/cabinet/${id}`),
  create: (data) => request.post('/cabinet', data),
  update: (id, data) => request.put(`/cabinet/${id}`, data),
  delete: (id) => request.delete(`/cabinet/${id}`)
}

// ==================== 文档 ====================
export const documentApi = {
  getList: (params) => request.get('/document', { params }),
  getById: (id) => request.get(`/document/${id}`),
  create: (data) => request.post('/document', data),
  update: (id, data) => request.put(`/document/${id}`, data),
  delete: (id) => request.delete(`/document/${id}`),
  download: (id) => request.get(`/document/${id}/download`, { responseType: 'blob' }),
  getVersions: (documentId) => request.get(`/document/${documentId}/versions`)
}

// ==================== 维保 ====================
export const maintenanceApi = {
  getPlans: (params) => request.get('/maintenance/plans', { params }),
  getPlanById: (id) => request.get(`/maintenance/plans/${id}`),
  createPlan: (data) => request.post('/maintenance/plans', data),
  updatePlan: (id, data) => request.put(`/maintenance/plans/${id}`, data),
  deletePlan: (id) => request.delete(`/maintenance/plans/${id}`),
  togglePlan: (id) => request.put(`/maintenance/plans/${id}/toggle`),

  getTasks: (params) => request.get('/maintenance/tasks', { params }),
  getTaskById: (id) => request.get(`/maintenance/tasks/${id}`),
  createTask: (data) => request.post('/maintenance/tasks', data),
  dispatchTask: (id) => request.put(`/maintenance/tasks/${id}/dispatch`),
  executeTask: (id) => request.put(`/maintenance/tasks/${id}/execute`),
  reviewTask: (id) => request.put(`/maintenance/tasks/${id}/review`),
  getTaskStats: () => request.get('/maintenance/tasks/stats')
}

// ==================== 用户 ====================
export const userApi = {
  getList: (params) => request.get('/user', { params }),
  getById: (id) => request.get(`/user/${id}`),
  create: (data) => request.post('/user', data),
  update: (id, data) => request.put(`/user/${id}`, data),
  delete: (id) => request.delete(`/user/${id}`),
  toggle: (id) => request.put(`/user/${id}/toggle`)
}

// ==================== 角色 ====================
export const roleApi = {
  getList: () => request.get('/role'),
  getById: (id) => request.get(`/role/${id}`),
  create: (data) => request.post('/role', data),
  update: (id, data) => request.put(`/role/${id}`, data),
  delete: (id) => request.delete(`/role/${id}`)
}

// ==================== 日志 ====================
export const logApi = {
  getList: (params) => request.get('/log', { params })
}

// ==================== 系统设置 ====================
export const settingsApi = {
  get: (category) => request.get('/settings', { params: { category } }),
  save: (data) => request.post('/settings', data)
}

// ==================== 统计 ====================
export const statisticsApi = {
  getDashboard: () => request.get('/statistics/dashboard'),
  getDevice: () => request.get('/statistics/device'),
  getWorkorder: () => request.get('/statistics/workorder'),
  getFaultTop5: () => request.get('/statistics/fault/top5'),
  getDeviceTrend: () => request.get('/statistics/device/trend')
}
