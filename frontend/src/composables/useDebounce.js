import { ref, watch } from 'vue'

/**
 * 防抖 Composable
 */
export function useDebounce(fn, delay = 300) {
  let timer = null

  const debouncedFn = (...args) => {
    if (timer) clearTimeout(timer)
    timer = setTimeout(() => {
      fn(...args)
    }, delay)
  }

  const cancel = () => {
    if (timer) {
      clearTimeout(timer)
      timer = null
    }
  }

  return { debouncedFn, cancel }
}

/**
 * 防抖值
 */
export function useDebouncedRef(initialValue, delay = 300) {
  const value = ref(initialValue)
  const debouncedValue = ref(initialValue)

  watch(value, (newVal) => {
    const timer = setTimeout(() => {
      debouncedValue.value = newVal
    }, delay)
    return () => clearTimeout(timer)
  })

  return { value, debouncedValue }
}
