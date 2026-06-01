# AEMS 前端模块化开发指南

## 快速启动

```bash
# 全模块模式（默认 5173 端口）
npm run dev

# 单模块独立调试（各自独立端口，可同时启动多个）
npm run dev:equipment     # → http://localhost:5174
npm run dev:workorder     # → http://localhost:5175
npm run dev:dashboard     # → http://localhost:5176
npm run dev:maintenance   # → http://localhost:5177
npm run dev:software      # → http://localhost:5178
npm run dev:sparepart     # → http://localhost:5179
npm run dev:document      # → http://localhost:5180
npm run dev:room          # → http://localhost:5181
npm run dev:statistics    # → http://localhost:5182
npm run dev:system        # → http://localhost:5183
```

## 后端启动（VS2026）

```bash
cd AEMS-Backend
# 用 VS2026 打开 AEMS.sln
# 或命令行：
dotnet restore
dotnet build
dotnet run --project AEMS.Api   # → http://localhost:5000
# Swagger: http://localhost:5000/swagger
```

## 前后端对接

| 前端模块 | 前端端口 | 后端 Controller | API 前缀 |
|---------|---------|----------------|---------|
| 登录 | 5173 | AuthController | /api/auth |
| 驾驶舱 | 5176 | DashboardController | /api/dashboard |
| 设备 | 5174 | EquipmentController | /api/equipment |
| 工单 | 5175 | WorkOrderController | /api/workorder |
| 维保 | 5177 | MaintenanceController | /api/maintenance |
| 软件 | 5178 | SoftwareController | /api/software |
| 备件 | 5179 | SparePartController | /api/sparepart |
| 文档 | 5180 | DocumentController | /api/document |
| 机房 | 5181 | RoomController | /api/room |
| 统计 | 5182 | StatisticController | /api/statistic |
| 系统 | 5183 | SystemController | /api/system |

## 项目结构

```
AEMS-Backend/                 ← 后端（VS2026 打开 AEMS.sln）
├── AEMS.sln
├── AEMS.Api/                 ← Web API 入口
│   ├── Controllers/          ← 10个模块控制器
│   ├── Program.cs
│   └── appsettings.json
├── AEMS.Domain/              ← 领域层
│   ├── Entities/             ← 10个实体
│   ├── Enums/
│   └── Interfaces/
├── AEMS.Application/         ← 业务层
│   ├── Services/             ← 10个服务接口+实现
│   ├── DTOs/
│   └── Common/
├── AEMS.Infrastructure/      ← 基础设施层
│   ├── Data/                 ← EF Core DbContext
│   ├── Repositories/
│   └── Extensions/
└── AEMS.Tests/               ← 单元测试

aems-frontend/                ← 前端（VSCode 打开）
├── .vscode/                  ← 调试配置
├── .env*                     ← 各模块环境变量
├── src/
│   ├── api/index.js          ← API 统一入口（对接后端）
│   ├── store/                ← Pinia Store（已对接API）
│   ├── views/                ← 各模块页面
│   ├── composables/          ← 复用逻辑
│   ├── utils/request.js      ← Axios 封装（已配置代理）
│   └── router/               ← 路由配置
└── vite.config.js            ← 多端口代理配置
```

## VSCode 调试

按 F5 → 选择启动配置：
- AEMS-全模块启动（5173）
- AEMS-设备模块（5174）
- AEMS-工单模块（5175）
- AEMS-驾驶舱（5176）

## 同时调试多个模块

终端1: `npm run dev:equipment`  → 5174
终端2: `npm run dev:workorder`  → 5175
终端3: `npm run dev:dashboard`  → 5176

每个端口独立运行，互不干扰。
