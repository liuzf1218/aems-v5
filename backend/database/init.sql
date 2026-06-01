-- ================================================================
-- AEMS V5 数据库初始化脚本
-- 数据库: PostgreSQL
-- 创建时间: 2026-03-31
-- ================================================================

-- 删除已存在的表（按依赖关系逆序）
DROP TABLE IF EXISTS document_version CASCADE;
DROP TABLE IF EXISTS document CASCADE;
DROP TABLE IF EXISTS stock_out_record CASCADE;
DROP TABLE IF EXISTS stock_in_record CASCADE;
DROP TABLE IF EXISTS sparepart CASCADE;
DROP TABLE IF EXISTS maintenance_item CASCADE;
DROP TABLE IF EXISTS maintenance_task CASCADE;
DROP TABLE IF EXISTS maintenance_plan CASCADE;
DROP TABLE IF EXISTS workorder_log CASCADE;
DROP TABLE IF EXISTS workorder CASCADE;
DROP TABLE IF EXISTS fault_type CASCADE;
DROP TABLE IF EXISTS software_instance CASCADE;
DROP TABLE IF EXISTS software_version CASCADE;
DROP TABLE IF EXISTS software CASCADE;
DROP TABLE IF EXISTS subsystem CASCADE;
DROP TABLE IF EXISTS equipment CASCADE;
DROP TABLE IF EXISTS equipment_type CASCADE;
DROP TABLE IF EXISTS cabinet CASCADE;
DROP TABLE IF EXISTS room CASCADE;
DROP TABLE IF EXISTS sys_dict CASCADE;
DROP TABLE IF EXISTS sys_log CASCADE;
DROP TABLE IF EXISTS sys_user CASCADE;
DROP TABLE IF EXISTS sys_role CASCADE;

-- ================================================================
-- 系统管理模块
-- ================================================================

-- 角色表
CREATE TABLE sys_role (
    id SERIAL PRIMARY KEY,
    name VARCHAR(50) NOT NULL,
    code VARCHAR(50) NOT NULL UNIQUE,
    remark VARCHAR(255),
    is_deleted BOOLEAN DEFAULT FALSE,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);
COMMENT ON TABLE sys_role IS '系统角色表';
COMMENT ON COLUMN sys_role.name IS '角色名称';
COMMENT ON COLUMN sys_role.code IS '角色编码';

-- 用户表
CREATE TABLE sys_user (
    id SERIAL PRIMARY KEY,
    username VARCHAR(50) NOT NULL UNIQUE,
    password_hash VARCHAR(255) NOT NULL,
    real_name VARCHAR(50),
    phone VARCHAR(20),
    email VARCHAR(100),
    role_id INTEGER REFERENCES sys_role(id) ON DELETE SET NULL,
    is_active BOOLEAN DEFAULT TRUE,
    is_deleted BOOLEAN DEFAULT FALSE,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);
COMMENT ON TABLE sys_user IS '系统用户表';
COMMENT ON COLUMN sys_user.username IS '用户名';
COMMENT ON COLUMN sys_user.password_hash IS '密码哈希';
COMMENT ON COLUMN sys_user.real_name IS '真实姓名';

-- 系统日志表
CREATE TABLE sys_log (
    id SERIAL PRIMARY KEY,
    user_id INTEGER,
    action VARCHAR(50),
    content VARCHAR(500),
    ip_address VARCHAR(50),
    method VARCHAR(10),
    path VARCHAR(255),
    status_code INTEGER,
    elapsed_ms BIGINT,
    is_deleted BOOLEAN DEFAULT FALSE,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);
COMMENT ON TABLE sys_log IS '系统日志表';

-- 数据字典表
CREATE TABLE sys_dict (
    id SERIAL PRIMARY KEY,
    dict_type VARCHAR(50) NOT NULL,
    label VARCHAR(100) NOT NULL,
    value VARCHAR(100) NOT NULL,
    sort_order INTEGER DEFAULT 0,
    is_active BOOLEAN DEFAULT TRUE,
    remark VARCHAR(255),
    is_deleted BOOLEAN DEFAULT FALSE,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);
COMMENT ON TABLE sys_dict IS '数据字典表';
COMMENT ON COLUMN sys_dict.dict_type IS '字典类型编码';
COMMENT ON COLUMN sys_dict.label IS '字典项名称';
COMMENT ON COLUMN sys_dict.value IS '字典项值';

-- ================================================================
-- 机房管理模块
-- ================================================================

-- 机房表
CREATE TABLE room (
    id SERIAL PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    code VARCHAR(50) NOT NULL UNIQUE,
    location VARCHAR(200),
    area DECIMAL(10,2),
    temp_upper DECIMAL(5,1),
    humidity_upper DECIMAL(5,1),
    manager VARCHAR(50),
    remark VARCHAR(500),
    is_deleted BOOLEAN DEFAULT FALSE,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);
COMMENT ON TABLE room IS '机房表';
COMMENT ON COLUMN room.code IS '机房编码';
COMMENT ON COLUMN room.area IS '面积（平方米）';
COMMENT ON COLUMN room.temp_upper IS '温度上限';

-- 机柜表
CREATE TABLE cabinet (
    id SERIAL PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    code VARCHAR(50) NOT NULL UNIQUE,
    room_id INTEGER NOT NULL REFERENCES room(id) ON DELETE CASCADE,
    total_units INTEGER DEFAULT 42,
    used_units INTEGER DEFAULT 0,
    power_limit DECIMAL(10,2),
    remark VARCHAR(500),
    is_deleted BOOLEAN DEFAULT FALSE,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);
COMMENT ON TABLE cabinet IS '机柜表';
COMMENT ON COLUMN cabinet.total_units IS 'U位总数';
COMMENT ON COLUMN cabinet.used_units IS '已用U位';
COMMENT ON COLUMN cabinet.power_limit IS '功率上限（kW）';

-- ================================================================
-- 设备管理模块
-- ================================================================

-- 设备类型表
CREATE TABLE equipment_type (
    id SERIAL PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    code VARCHAR(50) NOT NULL UNIQUE,
    remark VARCHAR(500),
    is_deleted BOOLEAN DEFAULT FALSE,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);
COMMENT ON TABLE equipment_type IS '设备类型表';

-- 设备表
CREATE TABLE equipment (
    id SERIAL PRIMARY KEY,
    name VARCHAR(200) NOT NULL,
    code VARCHAR(50) NOT NULL UNIQUE,
    equipment_type_id INTEGER REFERENCES equipment_type(id) ON DELETE SET NULL,
    cabinet_id INTEGER REFERENCES cabinet(id) ON DELETE SET NULL,
    position VARCHAR(50),
    ip_address VARCHAR(50),
    mac_address VARCHAR(50),
    serial_number VARCHAR(100),
    manufacturer VARCHAR(100),
    status INTEGER DEFAULT 1,
    purchase_date DATE,
    warranty_date DATE,
    remark VARCHAR(500),
    is_deleted BOOLEAN DEFAULT FALSE,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);
COMMENT ON TABLE equipment IS '设备表';
COMMENT ON COLUMN equipment.status IS '设备状态：0-故障 1-正常 2-维护中';
COMMENT ON COLUMN equipment.position IS '位置/U位';

-- 子系统表
CREATE TABLE subsystem (
    id SERIAL PRIMARY KEY,
    name VARCHAR(200) NOT NULL,
    code VARCHAR(50) NOT NULL,
    equipment_id INTEGER NOT NULL REFERENCES equipment(id) ON DELETE CASCADE,
    subsystem_type VARCHAR(50),
    status INTEGER DEFAULT 1,
    remark VARCHAR(500),
    is_deleted BOOLEAN DEFAULT FALSE,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);
COMMENT ON TABLE subsystem IS '子系统表';
COMMENT ON COLUMN subsystem.status IS '状态：0-故障 1-正常';

-- ================================================================
-- 软件管理模块
-- ================================================================

-- 软件表
CREATE TABLE software (
    id SERIAL PRIMARY KEY,
    name VARCHAR(200) NOT NULL,
    code VARCHAR(50) NOT NULL UNIQUE,
    vendor VARCHAR(100),
    software_type VARCHAR(50),
    license_type VARCHAR(50),
    remark VARCHAR(500),
    is_deleted BOOLEAN DEFAULT FALSE,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);
COMMENT ON TABLE software IS '软件表';

-- 软件版本表
CREATE TABLE software_version (
    id SERIAL PRIMARY KEY,
    software_id INTEGER NOT NULL REFERENCES software(id) ON DELETE CASCADE,
    version VARCHAR(50) NOT NULL,
    release_date DATE,
    change_log TEXT,
    package_path VARCHAR(500),
    is_deleted BOOLEAN DEFAULT FALSE,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);
COMMENT ON TABLE software_version IS '软件版本表';

-- 软件实例表
CREATE TABLE software_instance (
    id SERIAL PRIMARY KEY,
    software_version_id INTEGER NOT NULL REFERENCES software_version(id) ON DELETE CASCADE,
    equipment_id INTEGER NOT NULL REFERENCES equipment(id) ON DELETE CASCADE,
    install_path VARCHAR(500),
    install_date DATE,
    status INTEGER DEFAULT 1,
    remark VARCHAR(500),
    is_deleted BOOLEAN DEFAULT FALSE,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);
COMMENT ON TABLE software_instance IS '软件实例表';
COMMENT ON COLUMN software_instance.status IS '运行状态：0-停止 1-运行中 2-异常';

-- ================================================================
-- 工单管理模块
-- ================================================================

-- 故障类型表
CREATE TABLE fault_type (
    id SERIAL PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    code VARCHAR(50) NOT NULL UNIQUE,
    remark VARCHAR(500),
    is_deleted BOOLEAN DEFAULT FALSE,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);
COMMENT ON TABLE fault_type IS '故障类型表';

-- 工单表
CREATE TABLE workorder (
    id SERIAL PRIMARY KEY,
    workorder_no VARCHAR(50) NOT NULL UNIQUE,
    title VARCHAR(200) NOT NULL,
    description TEXT,
    fault_type_id INTEGER REFERENCES fault_type(id) ON DELETE SET NULL,
    equipment_id INTEGER REFERENCES equipment(id) ON DELETE SET NULL,
    priority INTEGER DEFAULT 2,
    status INTEGER DEFAULT 0,
    creator_id INTEGER REFERENCES sys_user(id) ON DELETE SET NULL,
    handler_id INTEGER REFERENCES sys_user(id) ON DELETE SET NULL,
    plan_finish_time TIMESTAMP,
    actual_finish_time TIMESTAMP,
    is_deleted BOOLEAN DEFAULT FALSE,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);
COMMENT ON TABLE workorder IS '工单表';
COMMENT ON COLUMN workorder.priority IS '优先级：1-低 2-中 3-高 4-紧急';
COMMENT ON COLUMN workorder.status IS '状态：0-待处理 1-处理中 2-已完成 3-已关闭';

-- 工单日志表
CREATE TABLE workorder_log (
    id SERIAL PRIMARY KEY,
    workorder_id INTEGER NOT NULL REFERENCES workorder(id) ON DELETE CASCADE,
    operator_id INTEGER,
    action_type VARCHAR(50),
    content VARCHAR(1000),
    is_deleted BOOLEAN DEFAULT FALSE,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);
COMMENT ON TABLE workorder_log IS '工单日志表';

-- ================================================================
-- 维护管理模块
-- ================================================================

-- 维护计划表
CREATE TABLE maintenance_plan (
    id SERIAL PRIMARY KEY,
    name VARCHAR(200) NOT NULL,
    plan_no VARCHAR(50) NOT NULL UNIQUE,
    plan_type INTEGER DEFAULT 1,
    cycle_days INTEGER,
    start_date DATE,
    end_date DATE,
    owner_id INTEGER,
    status INTEGER DEFAULT 0,
    remark VARCHAR(500),
    is_deleted BOOLEAN DEFAULT FALSE,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);
COMMENT ON TABLE maintenance_plan IS '维护计划表';
COMMENT ON COLUMN maintenance_plan.plan_type IS '维护类型：1-日常维护 2-定期维护 3-专项维护';
COMMENT ON COLUMN maintenance_plan.status IS '状态：0-未开始 1-进行中 2-已完成 3-已取消';

-- 维护任务表
CREATE TABLE maintenance_task (
    id SERIAL PRIMARY KEY,
    plan_id INTEGER NOT NULL REFERENCES maintenance_plan(id) ON DELETE CASCADE,
    name VARCHAR(200) NOT NULL,
    plan_time TIMESTAMP,
    actual_time TIMESTAMP,
    executor_id INTEGER,
    status INTEGER DEFAULT 0,
    remark VARCHAR(500),
    is_deleted BOOLEAN DEFAULT FALSE,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);
COMMENT ON TABLE maintenance_task IS '维护任务表';
COMMENT ON COLUMN maintenance_task.status IS '状态：0-待执行 1-执行中 2-已完成';

-- 维护项表
CREATE TABLE maintenance_item (
    id SERIAL PRIMARY KEY,
    task_id INTEGER NOT NULL REFERENCES maintenance_task(id) ON DELETE CASCADE,
    name VARCHAR(200) NOT NULL,
    description VARCHAR(500),
    result INTEGER DEFAULT 0,
    abnormal_note VARCHAR(500),
    is_deleted BOOLEAN DEFAULT FALSE,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);
COMMENT ON TABLE maintenance_item IS '维护项（检查点）表';
COMMENT ON COLUMN maintenance_item.result IS '检查结果：0-未检查 1-正常 2-异常';

-- ================================================================
-- 备件管理模块
-- ================================================================

-- 备件表
CREATE TABLE sparepart (
    id SERIAL PRIMARY KEY,
    name VARCHAR(200) NOT NULL,
    code VARCHAR(50) NOT NULL UNIQUE,
    specification VARCHAR(100),
    unit VARCHAR(20),
    stock_quantity INTEGER DEFAULT 0,
    min_stock INTEGER DEFAULT 0,
    price DECIMAL(18,2),
    location VARCHAR(100),
    remark VARCHAR(500),
    is_deleted BOOLEAN DEFAULT FALSE,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);
COMMENT ON TABLE sparepart IS '备件表';
COMMENT ON COLUMN sparepart.stock_quantity IS '当前库存数量';
COMMENT ON COLUMN sparepart.min_stock IS '最低库存预警';

-- 入库记录表
CREATE TABLE stock_in_record (
    id SERIAL PRIMARY KEY,
    sparepart_id INTEGER NOT NULL REFERENCES sparepart(id) ON DELETE CASCADE,
    quantity INTEGER NOT NULL,
    unit_price DECIMAL(18,2),
    supplier VARCHAR(100),
    in_date DATE,
    operator_id INTEGER,
    remark VARCHAR(500),
    is_deleted BOOLEAN DEFAULT FALSE,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);
COMMENT ON TABLE stock_in_record IS '入库记录表';

-- 出库记录表
CREATE TABLE stock_out_record (
    id SERIAL PRIMARY KEY,
    sparepart_id INTEGER NOT NULL REFERENCES sparepart(id) ON DELETE CASCADE,
    quantity INTEGER NOT NULL,
    department VARCHAR(100),
    recipient VARCHAR(50),
    out_date DATE,
    operator_id INTEGER,
    purpose VARCHAR(500),
    is_deleted BOOLEAN DEFAULT FALSE,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);
COMMENT ON TABLE stock_out_record IS '出库记录表';

-- ================================================================
-- 文档管理模块
-- ================================================================

-- 文档表
CREATE TABLE document (
    id SERIAL PRIMARY KEY,
    name VARCHAR(200) NOT NULL,
    doc_no VARCHAR(50) NOT NULL UNIQUE,
    category VARCHAR(50),
    equipment_id INTEGER REFERENCES equipment(id) ON DELETE SET NULL,
    current_version VARCHAR(20),
    uploader_id INTEGER,
    remark VARCHAR(500),
    is_deleted BOOLEAN DEFAULT FALSE,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);
COMMENT ON TABLE document IS '文档表';

-- 文档版本表
CREATE TABLE document_version (
    id SERIAL PRIMARY KEY,
    document_id INTEGER NOT NULL REFERENCES document(id) ON DELETE CASCADE,
    version VARCHAR(20) NOT NULL,
    file_path VARCHAR(500) NOT NULL,
    original_file_name VARCHAR(200),
    file_size BIGINT,
    file_type VARCHAR(50),
    uploader_id INTEGER,
    change_note VARCHAR(500),
    is_deleted BOOLEAN DEFAULT FALSE,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);
COMMENT ON TABLE document_version IS '文档版本表';

-- ================================================================
-- 索引
-- ================================================================
CREATE INDEX idx_equipment_cabinet ON equipment(cabinet_id);
CREATE INDEX idx_equipment_type ON equipment(equipment_type_id);
CREATE INDEX idx_subsystem_equipment ON subsystem(equipment_id);
CREATE INDEX idx_workorder_status ON workorder(status);
CREATE INDEX idx_workorder_equipment ON workorder(equipment_id);
CREATE INDEX idx_maintenance_plan_status ON maintenance_plan(status);
CREATE INDEX idx_sparepart_stock ON sparepart(stock_quantity);
CREATE INDEX idx_sys_log_user ON sys_log(user_id);
CREATE INDEX idx_sys_log_created ON sys_log(created_at);

-- ================================================================
-- 初始数据
-- ================================================================

-- 默认角色
INSERT INTO sys_role (name, code, remark) VALUES
('管理员', 'admin', '系统管理员，拥有全部权限'),
('运维人员', 'operator', '运维人员，负责设备维护'),
('普通用户', 'user', '普通用户，查看权限');

-- 默认管理员用户（密码：changeme，请修改）
INSERT INTO sys_user (username, password_hash, real_name, role_id) VALUES
('admin', 'changeme', '系统管理员', 1);

-- 默认字典数据
INSERT INTO sys_dict (dict_type, label, value, sort_order) VALUES
('equipment_status', '故障', '0', 1),
('equipment_status', '正常', '1', 2),
('equipment_status', '维护中', '2', 3),
('workorder_priority', '低', '1', 1),
('workorder_priority', '中', '2', 2),
('workorder_priority', '高', '3', 3),
('workorder_priority', '紧急', '4', 4),
('workorder_status', '待处理', '0', 1),
('workorder_status', '处理中', '1', 2),
('workorder_status', '已完成', '2', 3),
('workorder_status', '已关闭', '3', 4);
