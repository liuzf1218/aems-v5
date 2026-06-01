import axios from 'axios'
import { ElMessage } from 'element-plus'

const service = axios.create({
  baseURL: import.meta.env.VITE_API_BASE || '/api',
  timeout: 15000,
  headers: { 'Content-Type': 'application/json' }
})

// 请求拦截 - 自动附加 JWT Token
service.interceptors.request.use(
  config => {
    const token = localStorage.getItem('aems_token')
    if (token) {
      config.headers['Authorization'] = `Bearer ${token}`
    }
    // 开发环境打印请求
    if (import.meta.env.DEV) {
      console.log(`[API] ${config.method?.toUpperCase()} ${config.url}`, config.params || config.data || '')
    }
    return config
  },
  error => {
    console.error('Request error:', error)
    return Promise.reject(error)
  }
)

// 响应拦截 - 统一错误处理
service.interceptors.response.use(
  response => {
    const res = response.data
    if (res.code && res.code !== 200) {
      ElMessage.error(res.message || '请求失败')
      if (res.code === 401) {
        localStorage.removeItem('aems_token')
        window.location.href = '/login'
      }
      return Promise.reject(new Error(res.message || 'Error'))
    }
    return res.data !== undefined ? res.data : res
  },
  error => {
    const { response } = error
    if (response) {
      const msg = {
        401: '未授权，请重新登录',
        403: '没有权限访问',
        404: '请求资源不存在',
        500: '服务器错误'
      }[response.status] || response.data?.message || '请求失败'
      ElMessage.error(msg)
      if (response.status === 401) {
        localStorage.removeItem('aems_token')
        window.location.href = '/login'
      }
    } else {
      ElMessage.error('网络连接失败，请检查后端是否启动')
    }
    return Promise.reject(error)
  }
)

export default service


