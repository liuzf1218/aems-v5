<template>
  <div class="page-container">
    <el-card shadow="never">
      <template #header>
        <div class="card-header">
          <span>文档版本 - {{ $route.params.id }}</span>
          <el-button @click="$router.back()">返回</el-button>
        </div>
      </template>
      <el-timeline>
        <el-timeline-item timestamp="2026-03-15" placement="top">
          <el-card shadow="never">
            <h4>v2.0</h4>
            <p>更新操作流程，增加故障处理章节</p>
          </el-card>
        </el-timeline-item>
        <el-timeline-item timestamp="2026-01-10" placement="top">
          <el-card shadow="never">
            <h4>v1.5</h4>
            <p>修订维护周期，增加安全注意事项</p>
          </el-card>
        </el-timeline-item>
        <el-timeline-item timestamp="2025-10-20" placement="top">
          <el-card shadow="never">
            <h4>v1.0</h4>
            <p>初始版本</p>
          </el-card>
        </el-timeline-item>
      </el-timeline>
    </el-card>
  </div>
</template>

<style scoped>
.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}
</style>
