# AEMS V5 - 机场设备管理系统 / Airport Equipment Management System

<p align="center">
  <b>面向企业设备管理部门的全生命周期管理系统</b><br>
  <b>Full Lifecycle Equipment Management System for Enterprise</b>
</p>

---

## 📋 简介 / Introduction

**中文**：AEMS (Airport Equipment Management System) V5 是一个完整的设备管理解决方案，覆盖设备台账、软件管理、工单处理、维护计划、备件库存、机房管理、文档管理和统计报表等核心业务。

**English**: AEMS V5 is a comprehensive equipment management solution covering equipment ledger, software management, work order processing, maintenance planning, spare parts inventory, server room management, document management, and statistical reporting.

---

## ✨ 核心功能 / Core Features

| 模块 Module | 功能 Features |
|------------|--------------|
| 📊 设备管理 / Equipment | 设备分类树（15种类型）、台账列表、健康度评分 |
| 💿 软件管理 / Software | 软件台账、版本历史、实例部署管理 |
| 📝 工单管理 / Work Order | 4步向导式创建、SLA监控、处理流程跟踪 |
| 🔧 维护管理 / Maintenance | 维护计划、维护任务、执行跟踪 |
| 📦 备件管理 / Spare Parts | 库存预警、出入库管理、三级预警机制 |
| 🏢 机房管理 / Server Room | 机房信息、机柜管理、设备上架 |
| 📄 文档管理 / Document | 文档分类、版本控制、在线预览 |
| 📈 统计报表 / Statistics | 设备分布、故障趋势、工单效率分析 |

---

## 🏗️ 技术栈 / Tech Stack

### 后端 / Backend
| 技术 / Technology | 版本 / Version | 用途 / Purpose |
|------------------|---------------|---------------|
| .NET | 8.0 | Web API 框架 / Framework |
| Entity Framework Core | 8.0 | ORM 数据访问 / Data Access |
| SQL Server | 2019+ | 主数据库 / Primary Database |
| JWT Bearer | - | 认证与授权 / Authentication |
| Swagger / OpenAPI | - | API 文档 / API Documentation |

### 前端 / Frontend
| 技术 / Technology | 版本 / Version | 用途 / Purpose |
|------------------|---------------|---------------|
| Vue | 3.x | 核心框架 / Core Framework |
| Vite | 5.x | 构建工具 / Build Tool |
| Element Plus | 最新 / Latest | UI 组件库 / UI Component Library |
| Vue Router | 4.x | 路由管理 / Routing |
| Pinia | 最新 / Latest | 状态管理 / State Management |
| ECharts | 5.x | 图表可视化 / Charts |
| Axios | - | HTTP 客户端 / HTTP Client |

---

## 📁 项目结构 / Project Structure

```
aems-v5/
├── backend/                          # .NET 后端 / Backend
│   ├── AEMS.Api/                     # API 入口 / API Entry
│   │   ├── Controllers/              # API 控制器 / Controllers
│   │   ├── Middlewares/              # 中间件 / Middlewares
│   │   └── Program.cs                # 程序入口 / Entry Point
│   ├── AEMS.Core/                    # 实体与 DTO / Entities & DTOs
│   │   ├── Entities/                 # 数据库实体 / Entities
│   │   ├── DTOs/                     # 数据传输对象 / DTOs
│   │   └── Interfaces/               # 服务接口 / Interfaces
│   ├── AEMS.Infrastructure/          # 数据访问层 / Data Access
│   │   ├── Data/                     # DbContext
│   │   ├── Migrations/               # 数据库迁移 / Migrations
│   │   └── Repositories/             # 服务实现 / Service Implementations
│   ├── database/                     # 数据库脚本 / DB Scripts
│   └── scripts/                      # 工具脚本 / Utility Scripts
│
├── frontend/                         # Vue 前端 / Frontend
│   ├── src/
│   │   ├── views/                    # 页面组件 / Page Components
│   │   ├── api/                      # API 调用 / API Calls
│   │   ├── router/                   # 路由配置 / Router Config
│   │   ├── store/                    # Pinia 状态 / State Stores
│   │   ├── components/               # 公共组件 / Shared Components
│   │   └── utils/                    # 工具函数 / Utilities
│   └── public/                       # 静态资源 / Static Assets
│
└── docs/                             # 项目文档 / Documentation
    └── AEMS-V5-System-Design.md      # V5 系统设计方案 / System Design
```

---

## 🚀 快速开始 / Quick Start

### 1. 克隆仓库 / Clone Repository

```bash
git clone <repository-url>
cd aems-v5
```

### 2. 后端配置 / Backend Setup

```bash
cd backend

# 1. 复制配置文件模板 / Copy config template
cp AEMS.Api/appsettings.example.json AEMS.Api/appsettings.json
cp AEMS.Api/appsettings.example.json AEMS.Api/appsettings.Development.json

# 2. 编辑 appsettings.json，配置数据库连接字符串 / Edit connection string
# 3. 还原依赖 / Restore dependencies
dotnet restore

# 4. 应用数据库迁移 / Apply migrations
cd AEMS.Api
dotnet ef database update

# 5. 运行 / Run
dotnet run
```

后端服务将在 `http://localhost:5289` 启动  
API 文档：`http://localhost:5289/swagger`

### 3. 前端配置 / Frontend Setup

```bash
cd frontend

# 1. 安装依赖 / Install dependencies
npm install

# 2. 开发模式运行 / Run in development mode
npm run dev
```

前端将在 `http://localhost:5173` 启动

### 4. 默认登录账号 / Default Login

- **用户名 / Username**: `admin`
- **密码 / Password**: `changeme`（请在首次登录后修改）

---

## 🧪 数据库初始化 / Database Initialization

如需使用种子数据初始化数据库：

```bash
cd backend/AEMS.Api
dotnet run --seed
# 或使用 SQL 脚本 / Or use SQL script
sqlcmd -S localhost -U sa -P YourPassword -d AEMS -i ../seed-data.sql
```

---

## 🔧 常用命令 / Common Commands

### 后端 / Backend
```bash
# 构建 / Build
dotnet build

# 创建迁移 / Create migration
dotnet ef migrations add MigrationName

# 运行测试 / Run tests
dotnet test
```

### 前端 / Frontend
```bash
# 开发模式 / Development
npm run dev

# 生产构建 / Production build
npm run build

# 预览构建结果 / Preview build
npm run preview
```

---

## 📄 许可证 / License

本项目为内部项目，仅供学习交流使用。  
This project is for internal use and educational purposes only.

---

<p align="center">
  <b>版本 / Version</b>: v5.0  <br>
  <b>作者 / Author</b>: AEMS Development Team
</p>
