# AEMS V5 后端

设备管理系统 V5 后端 API 项目

## 技术栈

- ASP.NET Core 8 WebAPI
- Entity Framework Core 8 (ORM)
- PostgreSQL (数据库)
- JWT Bearer (认证)
- Serilog (日志)
- Swagger/OpenAPI (文档)

## 快速开始

```bash
# 还原依赖
dotnet restore

# 构建
dotnet build

# 运行
dotnet run --project src/AEMS.Api

# 数据库迁移
dotnet ef migrations add Init --project src/AEMS.Infrastructure --startup-project src/AEMS.Api
dotnet ef database update --project src/AEMS.Infrastructure --startup-project src/AEMS.Api
```

## 项目结构

```
src/
├── AEMS.Api/              # API层 (Controllers, Middlewares)
│   ├── Controllers/       # API控制器
│   ├── Middlewares/       # 中间件
│   ├── Extensions/        # 扩展方法
│   ├── Program.cs         # 启动配置
│   └── appsettings.json   # 配置文件
├── AEMS.Core/             # 核心层 (Entities, DTOs, Enums, Interfaces)
│   ├── Entities/          # 实体类
│   ├── DTOs/              # 数据传输对象
│   ├── Enums/             # 枚举
│   └── Interfaces/        # 接口定义
└── AEMS.Infrastructure/   # 基础设施层 (Data, Repositories, Services)
    ├── Data/              # DbContext
    ├── Repositories/      # 仓储实现
    └── Services/          # 业务服务
scripts/
└── init-database.sql      # 数据库初始化脚本
```

## API规范

- 基础地址: `http://localhost:5000/api`
- 认证方式: JWT Bearer Token
- 响应格式: `{ code: 200, message: "success", data: {} }`
- 分页格式: `{ items: [], total: 0, page: 1, pageSize: 10 }`
- 时间格式: ISO 8601

## 数据库

- 类型: PostgreSQL 14+
- 默认连接: `Host=localhost;Port=5432;Database=aems_v5;Username=postgres;Password=postgres`
- 初始化脚本: `scripts/init-database.sql`

## 默认账号

- 用户名: `admin`
- 密码: `changeme`（请在首次登录后修改）
