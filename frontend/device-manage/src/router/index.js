import { createRouter, createWebHistory } from 'vue-router'
import Home from '../views/Home.vue'
import Device from '../views/Device.vue'
import SwaggerPage from '../views/SwaggerPage.vue'

const routes = [
  { path: '/', component: Home },
  { path: '/device', component: Device },
  { path: '/swagger', component: SwaggerPage },
]

const router = createRouter({
  history: createWebHistory(),
  routes,
})

export default router
