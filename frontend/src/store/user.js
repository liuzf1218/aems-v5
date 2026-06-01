import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import { authApi } from '@/api/index.js'

export const useUserStore = defineStore('user', () => {
  const token = ref(localStorage.getItem('aems_token') || '')
  const userInfo = ref(JSON.parse(localStorage.getItem('aems_user') || '{}'))
  const permissions = ref(JSON.parse(localStorage.getItem('aems_permissions') || '[]'))

  const isLoggedIn = computed(() => !!token.value)
  const username = computed(() => userInfo.value.username || '')
  const role = computed(() => userInfo.value.role || 'user')
  const department = computed(() => userInfo.value.department || '')

  const login = async (loginData) => {
    const res = await authApi.login(loginData)
    const realToken = res?.token || res
    const user = res?.userInfo || { username: loginData.username, role: 'admin' }

    token.value = realToken
    userInfo.value = user
    permissions.value = [{ resource: '*', action: '*', conditions: {} }]

    localStorage.setItem('aems_token', realToken)
    localStorage.setItem('aems_user', JSON.stringify(user))
    localStorage.setItem('aems_permissions', JSON.stringify([{ resource: '*', action: '*', conditions: {} }]))
  }

  const logout = () => {
    token.value = ''
    userInfo.value = {}
    permissions.value = []
    localStorage.removeItem('aems_token')
    localStorage.removeItem('aems_user')
    localStorage.removeItem('aems_permissions')
  }

  const hasPermission = (resource, action) => {
    return permissions.value.some(p => {
      if (p.resource === '*') return true
      if (p.resource === resource && (p.action === action || p.action === '*')) return true
      return false
    })
  }

  return {
    token,
    userInfo,
    permissions,
    isLoggedIn,
    username,
    role,
    department,
    login,
    logout,
    hasPermission
  }
})
