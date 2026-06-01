<template>
  <div>
    <div style="padding: 20px; background: #f5f5f5">
      <router-link to="/" style="margin-right: 20px">首页</router-link>
      <router-link to="/device" style="margin-right: 20px">设备管理</router-link>
      <router-link to="/swagger">API文档</router-link>
    </div>
    <router-view />
  </div>
</template>
