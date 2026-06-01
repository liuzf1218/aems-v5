/**
 * 后端枚举值 → 前端展示映射
 * 后端返回数字枚举，前端需要显示中文标签和 Element Plus 类型
 */

// 设备状态: 0=在用, 1=故障, 2=维修中, 3=备用, 4=退役
const EQUIPMENT_STATUS_MAP = {
  0: { label: '在用', type: 'success', code: 'ACTIVE' },
  1: { label: '故障', type: 'danger', code: 'FAULT' },
  2: { label: '维修中', type: 'warning', code: 'MAINTENANCE' },
  3: { label: '备用', type: 'info', code: 'STANDBY' },
  4: { label: '退役', type: 'info', code: 'RETIRED' },
  // 兼容字符串
  ACTIVE: { label: '在用', type: 'success', code: 'ACTIVE' },
  FAULT: { label: '故障', type: 'danger', code: 'FAULT' },
  MAINTENANCE: { label: '维修中', type: 'warning', code: 'MAINTENANCE' },
  STANDBY: { label: '备用', type: 'info', code: 'STANDBY' },
  RETIRED: { label: '退役', type: 'info', code: 'RETIRED' }
}

// 工单状态: 0=待处理, 1=处理中, 2=待验收, 3=已完成, 4=已关闭
const WORKORDER_STATUS_MAP = {
  0: { label: '待处理', type: 'info', code: 'PENDING' },
  1: { label: '处理中', type: 'warning', code: 'IN_PROGRESS' },
  2: { label: '待验收', type: 'primary', code: 'PENDING_REVIEW' },
  3: { label: '已完成', type: 'success', code: 'COMPLETED' },
  4: { label: '已关闭', type: 'info', code: 'CLOSED' },
  PENDING: { label: '待处理', type: 'info' },
  IN_PROGRESS: { label: '处理中', type: 'warning' },
  PENDING_REVIEW: { label: '待验收', type: 'primary' },
  COMPLETED: { label: '已完成', type: 'success' },
  CLOSED: { label: '已关闭', type: 'info' }
}

// 工单类型: 1=巡检, 2=故障, 3=维护, 4=校准
const WORKORDER_TYPE_MAP = {
  1: { label: '巡检', type: 'primary' },
  2: { label: '故障', type: 'danger' },
  3: { label: '维护', type: 'warning' },
  4: { label: '校准', type: 'success' }
}

// 优先级: 1=紧急, 2=高, 3=中, 4=低
const PRIORITY_MAP = {
  1: { label: '紧急', type: 'danger' },
  2: { label: '高', type: 'warning' },
  3: { label: '中', type: 'primary' },
  4: { label: '低', type: 'info' }
}

// 重要性(A/B/C): 1=A, 2=B, 3=C
const CRITICALITY_MAP = {
  1: { label: 'A级', type: 'danger', code: 'A' },
  2: { label: 'B级', type: 'warning', code: 'B' },
  3: { label: 'C级', type: 'info', code: 'C' },
  A: { label: 'A级', type: 'danger', code: 'A' },
  B: { label: 'B级', type: 'warning', code: 'B' },
  C: { label: 'C级', type: 'info', code: 'C' }
}

// 设备分类: 1=导航, 2=通信, 3=气象, 4=监视, 5=信息化
const CATEGORY_MAP = {
  1: { label: '导航', type: '' },
  2: { label: '通信', type: 'success' },
  3: { label: '气象', type: 'warning' },
  4: { label: '监视', type: 'danger' },
  5: { label: '信息化', type: 'info' },
  '导航': { label: '导航', type: '' },
  '通信': { label: '通信', type: 'success' },
  '气象': { label: '气象', type: 'warning' },
  '监视': { label: '监视', type: 'danger' },
  '信息化': { label: '信息化', type: 'info' }
}

/**
 * 通用映射函数
 * @param {Object} map - 映射表
 * @param {*} value - 后端值
 * @param {string} field - 返回字段 (label/type/code)
 * @param {*} fallback - 默认值
 */
function mapValue(map, value, field = 'label', fallback = '') {
  const entry = map[value]
  return entry ? entry[field] : fallback || value
}

export {
  EQUIPMENT_STATUS_MAP,
  WORKORDER_STATUS_MAP,
  WORKORDER_TYPE_MAP,
  PRIORITY_MAP,
  CRITICALITY_MAP,
  CATEGORY_MAP,
  mapValue
}
