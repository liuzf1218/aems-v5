import { computed } from 'vue'
import { useUserStore } from '@/store/user'

/**
 * 权限控制 Composable
 */
export function usePermission() {
  const userStore = useUserStore()

  const permissions = computed(() => userStore.permissions || [])

  const check = (resource, action, conditions = {}) => {
    return permissions.value.some(p => {
      if (p.resource !== resource) return false
      if (p.action !== action && p.action !== '*') return false

      if (conditions.ownOnly && !p.conditions?.ownOnly) return false
      if (conditions.department && p.conditions?.department !== conditions.department) return false

      return true
    })
  }

  const can = (permission) => {
    const [resource, action] = permission.split(':')
    return check(resource, action)
  }

  const isAdmin = computed(() => userStore.role === 'admin')

  const hasAnyRole = (...roles) => {
    return roles.includes(userStore.role)
  }

  return {
    permissions,
    check,
    can,
    isAdmin,
    hasAnyRole
  }
}
