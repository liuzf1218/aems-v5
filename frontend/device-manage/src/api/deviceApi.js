import request from './request'

export function getDeviceList() {
  return request({ url: '/api/Device', method: 'get' })
}

export function getDeviceById(id) {
  return request({ url: `/api/Device/${id}`, method: 'get' })
}

export function addDevice(data) {
  return request({ url: '/api/Device', method: 'post', data })
}

export function updateDevice(id, data) {
  return request({ url: `/api/Device/${id}`, method: 'put', data })
}

export function deleteDevice(id) {
  return request({ url: `/api/Device/${id}`, method: 'delete' })
}
