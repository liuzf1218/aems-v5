-- AEMS V5 数据库初始化脚本
-- PostgreSQL 14+

-- 创建数据库
-- CREATE DATABASE aems_v5;

-- 设备分类
CREATE TABLE IF NOT EXISTS equipment_type (
    id SERIAL PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    code VARCHAR(50) NOT NULL UNIQUE,
    parent_id INTEGER REFERENCES equipment_type(id),
    level INTEGER NOT NULL DEFAULT 1,
    sort_order INTEGER DEFAULT 0,
    description TEXT
);

-- 机房
CREATE TABLE IF NOT EXISTS room (
    id SERIAL PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    code VARCHAR(50) NOT NULL UNIQUE,
    location VARCHAR(200),
    area VARCHAR(50),
    manager VARCHAR(50),
    phone VARCHAR(20),
    description TEXT,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- 设备
CREATE TABLE IF NOT EXISTS equipment (
    id SERIAL PRIMARY KEY,
    code VARCHAR(50) NOT NULL UNIQUE,
    name VARCHAR(200) NOT NULL,
    type VARCHAR(100),
    system VARCHAR(100),
    category VARCHAR(100),
    room_id INTEGER REFERENCES room(id),
    position VARCHAR(200),
    description TEXT,
    asset_code VARCHAR(50),
    vendor VARCHAR(100),
    supplier VARCHAR(100),
    model VARCHAR(100),
    serial_no VARCHAR(100),
    purchase_date DATE,
    warranty_expiry DATE,
    contract_no VARCHAR(50),
    status INTEGER DEFAULT 1,
    commission_date DATE,
    design_life INTEGER,
    running_hours DOUBLE PRECISION DEFAULT 0,
    fault_count INTEGER DEFAULT 0,
    last_maintenance TIMESTAMP,
    next_maintenance TIMESTAMP,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- 软件
CREATE TABLE IF NOT EXISTS software (
    id SERIAL PRIMARY KEY,
    name VARCHAR(200) NOT NULL,
    code VARCHAR(50) NOT NULL UNIQUE,
    vendor VARCHAR(100),
    category VARCHAR(100),
    description TEXT,
    current_version VARCHAR(50),
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- 软件版本
CREATE TABLE IF NOT EXISTS software_version (
    id SERIAL PRIMARY KEY,
    software_id INTEGER NOT NULL REFERENCES software(id),
    version VARCHAR(50) NOT NULL,
    release_notes TEXT,
    release_date DATE NOT NULL,
    file_path VARCHAR(500),
    file_size BIGINT
);

-- 软件实例
CREATE TABLE IF NOT EXISTS software_instance (
    id SERIAL PRIMARY KEY,
    software_id INTEGER NOT NULL REFERENCES software(id),
    software_version_id INTEGER NOT NULL REFERENCES software_version(id),
    equipment_id INTEGER NOT NULL REFERENCES equipment(id),
    config TEXT,
    deploy_date DATE NOT NULL,
    status VARCHAR(20) DEFAULT 'running'
);

-- 故障类型
CREATE TABLE IF NOT EXISTS fault_type (
    id SERIAL PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    description TEXT,
    parent_id INTEGER REFERENCES fault_type(id)
);

-- 工单
CREATE TABLE IF NOT EXISTS work_order (
    id SERIAL PRIMARY KEY,
    code VARCHAR(50) NOT NULL UNIQUE,
    title VARCHAR(200) NOT NULL,
    description TEXT,
    equipment_id INTEGER REFERENCES equipment(id),
    room_id INTEGER REFERENCES room(id),
    fault_type VARCHAR(100),
    priority INTEGER DEFAULT 2,
    status INTEGER DEFAULT 1,
    reporter_id INTEGER,
    assignee_id INTEGER,
    accepted_at TIMESTAMP,
    completed_at TIMESTAMP,
    sla_response_minutes INTEGER,
    sla_resolve_minutes INTEGER,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- 工单操作日志
CREATE TABLE IF NOT EXISTS work_order_log (
    id SERIAL PRIMARY KEY,
    work_order_id INTEGER NOT NULL REFERENCES work_order(id),
    action VARCHAR(50) NOT NULL,
    remark TEXT,
    operator_id INTEGER,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- 维护计划
CREATE TABLE IF NOT EXISTS maintenance_plan (
    id SERIAL PRIMARY KEY,
    name VARCHAR(200) NOT NULL,
    description TEXT,
    equipment_id INTEGER REFERENCES equipment(id),
    cycle_type VARCHAR(20) NOT NULL DEFAULT 'daily',
    cycle_value INTEGER NOT NULL DEFAULT 1,
    next_execute_date DATE,
    is_active BOOLEAN DEFAULT true,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- 维护任务
CREATE TABLE IF NOT EXISTS maintenance_task (
    id SERIAL PRIMARY KEY,
    plan_id INTEGER REFERENCES maintenance_plan(id),
    name VARCHAR(200) NOT NULL,
    description TEXT,
    equipment_id INTEGER REFERENCES equipment(id),
    status INTEGER DEFAULT 1,
    assignee_id INTEGER,
    planned_date DATE,
    executed_at TIMESTAMP,
    result TEXT,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- 备件
CREATE TABLE IF NOT EXISTS spare_part (
    id SERIAL PRIMARY KEY,
    code VARCHAR(50) NOT NULL UNIQUE,
    name VARCHAR(200) NOT NULL,
    category VARCHAR(100),
    specification VARCHAR(200),
    unit VARCHAR(20),
    stock_quantity INTEGER DEFAULT 0,
    min_stock INTEGER DEFAULT 0,
    max_stock INTEGER DEFAULT 0,
    unit_price DECIMAL(10, 2),
    vendor VARCHAR(100),
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- 入库记录
CREATE TABLE IF NOT EXISTS stock_in_record (
    id SERIAL PRIMARY KEY,
    spare_part_id INTEGER NOT NULL REFERENCES spare_part(id),
    quantity INTEGER NOT NULL,
    unit_price DECIMAL(10, 2),
    supplier VARCHAR(100),
    remark TEXT,
    operator_id INTEGER,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- 出库记录
CREATE TABLE IF NOT EXISTS stock_out_record (
    id SERIAL PRIMARY KEY,
    spare_part_id INTEGER NOT NULL REFERENCES spare_part(id),
    quantity INTEGER NOT NULL,
    purpose VARCHAR(200),
    work_order_id INTEGER REFERENCES work_order(id),
    equipment_id INTEGER REFERENCES equipment(id),
    remark TEXT,
    operator_id INTEGER,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- 机柜
CREATE TABLE IF NOT EXISTS cabinet (
    id SERIAL PRIMARY KEY,
    room_id INTEGER NOT NULL REFERENCES room(id),
    name VARCHAR(100) NOT NULL,
    code VARCHAR(50) NOT NULL,
    total_units INTEGER DEFAULT 42,
    used_units INTEGER DEFAULT 0,
    description TEXT
);

-- 文档
CREATE TABLE IF NOT EXISTS document (
    id SERIAL PRIMARY KEY,
    name VARCHAR(200) NOT NULL,
    category VARCHAR(100),
    equipment_id INTEGER REFERENCES equipment(id),
    description TEXT,
    current_version VARCHAR(50),
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- 文档版本
CREATE TABLE IF NOT EXISTS document_version (
    id SERIAL PRIMARY KEY,
    document_id INTEGER NOT NULL REFERENCES document(id),
    version VARCHAR(50) NOT NULL,
    file_name VARCHAR(300),
    file_path VARCHAR(500),
    file_size BIGINT,
    remark TEXT,
    uploader_id INTEGER,
    is_current BOOLEAN DEFAULT false,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- 系统用户
CREATE TABLE IF NOT EXISTS sys_user (
    id SERIAL PRIMARY KEY,
    username VARCHAR(50) NOT NULL UNIQUE,
    password_hash VARCHAR(200) NOT NULL,
    real_name VARCHAR(50),
    email VARCHAR(100),
    phone VARCHAR(20),
    role_id INTEGER,
    is_active BOOLEAN DEFAULT true,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- 角色
CREATE TABLE IF NOT EXISTS sys_role (
    id SERIAL PRIMARY KEY,
    name VARCHAR(50) NOT NULL,
    code VARCHAR(50) UNIQUE,
    description TEXT,
    permissions TEXT,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- 操作日志
CREATE TABLE IF NOT EXISTS sys_log (
    id SERIAL PRIMARY KEY,
    user_id INTEGER,
    username VARCHAR(50),
    module VARCHAR(50) NOT NULL,
    action VARCHAR(50) NOT NULL,
    description TEXT,
    ip_address VARCHAR(50),
    request_url VARCHAR(500),
    request_body TEXT,
    response_body TEXT,
    duration INTEGER,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);
CREATE INDEX idx_sys_log_created_at ON sys_log(created_at);

-- 字典
CREATE TABLE IF NOT EXISTS sys_dict (
    id SERIAL PRIMARY KEY,
    type VARCHAR(50) NOT NULL,
    label VARCHAR(100) NOT NULL,
    value VARCHAR(100) NOT NULL,
    sort_order INTEGER DEFAULT 0,
    is_active BOOLEAN DEFAULT true,
    UNIQUE(type, value)
);

-- 插入初始数据
-- 管理员用户 (密码: changeme，实际使用BCrypt哈希，请修改)
INSERT INTO sys_user (username, password_hash, real_name, email, is_active)
VALUES ('admin', 'changeme', '系统管理员', 'admin@example.com', true)
ON CONFLICT (username) DO NOTHING;

-- 基础角色
INSERT INTO sys_role (name, code, description) VALUES
    ('超级管理员', 'SUPER_ADMIN', '拥有所有权限'),
    ('管理员', 'ADMIN', '系统管理员'),
    ('工程师', 'ENGINEER', '设备维护工程师'),
    ('操作员', 'OPERATOR', '日常操作员')
ON CONFLICT (code) DO NOTHING;

-- 设备分类基础数据
INSERT INTO equipment_type (name, code, level, sort_order) VALUES
    ('通信导航', 'COMM', 1, 1),
    ('监视雷达', 'RADAR', 1, 2),
    ('气象设备', 'WX', 1, 3),
    ('自动化系统', 'AUTO', 1, 4),
    ('供电系统', 'POWER', 1, 5)
ON CONFLICT (code) DO NOTHING;

-- 故障类型
INSERT INTO fault_type (name, description) VALUES
    ('硬件故障', '设备硬件损坏'),
    ('软件故障', '软件异常或崩溃'),
    ('网络故障', '网络连接问题'),
    ('性能异常', '性能下降或不稳定'),
    ('其他', '其他类型故障')
ON CONFLICT DO NOTHING;

-- 字典数据
INSERT INTO sys_dict (type, label, value, sort_order) VALUES
    ('equipment_status', '在用', '1', 1),
    ('equipment_status', '备用', '2', 2),
    ('equipment_status', '故障', '3', 3),
    ('equipment_status', '维修中', '4', 4),
    ('equipment_status', '报废', '5', 5),
    ('workorder_priority', '低', '1', 1),
    ('workorder_priority', '中', '2', 2),
    ('workorder_priority', '高', '3', 3),
    ('workorder_priority', '紧急', '4', 4)
ON CONFLICT (type, value) DO NOTHING;
