-- sys_role: 已有1条，再插9条
INSERT INTO sys_role (Name, Code, Remark, CreatedAt, UpdatedAt, IsDeleted) VALUES
(N'操作员', 'operator', N'普通操作员', GETDATE(), GETDATE(), 0),
(N'审核员', 'auditor', N'数据审核员', GETDATE(), GETDATE(), 0),
(N'维修员', 'repair', N'设备维修员', GETDATE(), GETDATE(), 0),
(N'库管员', 'warehouse', N'仓库管理员', GETDATE(), GETDATE(), 0),
(N'工程师', 'engineer', N'系统工程师', GETDATE(), GETDATE(), 0),
(N'访客', 'guest', N'只读访客', GETDATE(), GETDATE(), 0),
(N'主管', 'manager', N'部门主管', GETDATE(), GETDATE(), 0),
(N'质检员', 'qc', N'质量检查员', GETDATE(), GETDATE(), 0),
(N'调度员', 'dispatcher', N'任务调度员', GETDATE(), GETDATE(), 0);

-- sys_user: 已有1条，再插9条
INSERT INTO sys_user (Username, PasswordHash, RealName, Phone, Email, RoleId, IsActive, CreatedAt, UpdatedAt, IsDeleted) VALUES
('operator1', '123456', N'Operator A', '13800000001', 'op1@example.com', 2, 1, GETDATE(), GETDATE(), 0),
('auditor1', '123456', N'Auditor A', '13800000002', 'aud1@example.com', 3, 1, GETDATE(), GETDATE(), 0),
('repair1', '123456', N'Repairer A', '13800000003', 'rep1@example.com', 4, 1, GETDATE(), GETDATE(), 0),
('warehouse1', '123456', N'Warehouse A', '13800000004', 'wh1@example.com', 5, 1, GETDATE(), GETDATE(), 0),
('engineer1', '123456', N'Engineer A', '13800000005', 'eng1@example.com', 6, 1, GETDATE(), GETDATE(), 0),
('guest1', '123456', N'Guest A', '13800000006', 'guest1@example.com', 7, 1, GETDATE(), GETDATE(), 0),
('manager1', '123456', N'Manager A', '13800000007', 'mgr1@example.com', 8, 1, GETDATE(), GETDATE(), 0),
('qc1', '123456', N'QC A', '13800000008', 'qc1@example.com', 9, 1, GETDATE(), GETDATE(), 0),
('dispatcher1', '123456', N'Dispatcher A', '13800000009', 'disp1@example.com', 10, 1, GETDATE(), GETDATE(), 0);

-- room: 10条
INSERT INTO room (Name, Code, Location, Area, TempUpper, HumidityUpper, Manager, Remark, CreatedAt, UpdatedAt, IsDeleted) VALUES
(N'机房A', 'ROOM-A001', N'一楼东侧', 120.5, 26.0, 60.0, N'Manager A', N'主机房', GETDATE(), GETDATE(), 0),
(N'机房B', 'ROOM-B001', N'一楼西侧', 98.0, 25.0, 55.0, N'Manager B', N'备用机房', GETDATE(), GETDATE(), 0),
(N'机房C', 'ROOM-C001', N'二楼东侧', 150.0, 24.0, 50.0, N'Manager C', N'数据中心', GETDATE(), GETDATE(), 0),
(N'机房D', 'ROOM-D001', N'二楼西侧', 80.0, 26.5, 65.0, N'Manager D', N'测试机房', GETDATE(), GETDATE(), 0),
(N'机房E', 'ROOM-E001', N'三楼东侧', 200.0, 23.0, 45.0, N'Manager E', N'核心机房', GETDATE(), GETDATE(), 0),
(N'库房A', 'ROOM-W001', N'地下一层', 300.0, 20.0, 40.0, N'Manager F', N'备件库房', GETDATE(), GETDATE(), 0),
(N'办公室', 'ROOM-O001', N'三楼西侧', 60.0, 25.0, 55.0, N'Manager G', N'运维办公室', GETDATE(), GETDATE(), 0),
(N'监控室', 'ROOM-M001', N'一楼中部', 45.0, 24.0, 50.0, N'Manager H', N'监控中心', GETDATE(), GETDATE(), 0),
(N'配电室', 'ROOM-P001', N'地下二层', 70.0, 22.0, 40.0, N'Manager I', N'配电设备间', GETDATE(), GETDATE(), 0),
(N'UPS室', 'ROOM-U001', N'地下一层', 55.0, 21.0, 35.0, N'Manager J', N'UPS电池间', GETDATE(), GETDATE(), 0);

-- equipment_type: 10条
INSERT INTO equipment_type (Name, Code, Remark, CreatedAt, UpdatedAt, IsDeleted) VALUES
(N'服务器', 'ET-SERVER', N'通用服务器', GETDATE(), GETDATE(), 0),
(N'交换机', 'ET-SWITCH', N'网络交换机', GETDATE(), GETDATE(), 0),
(N'路由器', 'ET-ROUTER', N'核心路由器', GETDATE(), GETDATE(), 0),
(N'防火墙', 'ET-FIREWALL', N'安全防火墙', GETDATE(), GETDATE(), 0),
(N'存储设备', 'ET-STORAGE', N'磁盘阵列', GETDATE(), GETDATE(), 0),
(N'UPS电源', 'ET-UPS', N'不间断电源', GETDATE(), GETDATE(), 0),
(N'空调', 'ET-AC', N'精密空调', GETDATE(), GETDATE(), 0),
(N'监控摄像头', 'ET-CAMERA', N'视频监控', GETDATE(), GETDATE(), 0),
(N'门禁设备', 'ET-ACCESS', N'门禁控制器', GETDATE(), GETDATE(), 0),
(N'传感器', 'ET-SENSOR', N'环境传感器', GETDATE(), GETDATE(), 0);

-- fault_type: 10条
INSERT INTO fault_type (Name, Code, Remark, CreatedAt, UpdatedAt, IsDeleted) VALUES
(N'硬件故障', 'FT-HARDWARE', N'物理硬件损坏', GETDATE(), GETDATE(), 0),
(N'软件故障', 'FT-SOFTWARE', N'系统软件异常', GETDATE(), GETDATE(), 0),
(N'网络故障', 'FT-NETWORK', N'网络连接问题', GETDATE(), GETDATE(), 0),
(N'电源故障', 'FT-POWER', N'供电异常', GETDATE(), GETDATE(), 0),
(N'环境故障', 'FT-ENV', N'温湿度异常', GETDATE(), GETDATE(), 0),
(N'人为故障', 'FT-HUMAN', N'操作失误', GETDATE(), GETDATE(), 0),
(N'老化故障', 'FT-AGING', N'设备老化', GETDATE(), GETDATE(), 0),
(N'配置故障', 'FT-CONFIG', N'配置错误', GETDATE(), GETDATE(), 0),
(N'病毒攻击', 'FT-VIRUS', N'恶意软件', GETDATE(), GETDATE(), 0),
(N'数据丢失', 'FT-DATA', N'数据损坏或丢失', GETDATE(), GETDATE(), 0);

-- software: 10条
INSERT INTO software (Name, Code, Vendor, SoftwareType, LicenseType, Remark, CreatedAt, UpdatedAt, IsDeleted) VALUES
(N'Windows Server 2022', 'SW-WS2022', N'Microsoft', N'操作系统', N'批量许可', N'服务器操作系统', GETDATE(), GETDATE(), 0),
(N'Red Hat Linux', 'SW-RHEL9', N'Red Hat', N'操作系统', N'订阅', N'企业级Linux', GETDATE(), GETDATE(), 0),
(N'VMware vSphere', 'SW-VSPHERE', N'VMware', N'虚拟化', N'永久许可', N'虚拟化平台', GETDATE(), GETDATE(), 0),
(N'Nagios', 'SW-NAGIOS', N'Nagios', N'监控', N'开源', N'系统监控', GETDATE(), GETDATE(), 0),
(N'SQL Server 2022', 'SW-SQL2022', N'Microsoft', N'数据库', N'核心许可', N'关系型数据库', GETDATE(), GETDATE(), 0),
(N'Oracle DB', 'SW-ORACLE', N'Oracle', N'数据库', N'处理器许可', N'企业数据库', GETDATE(), GETDATE(), 0),
(N'Docker', 'SW-DOCKER', N'Docker Inc', N'容器', N'开源', N'容器运行时', GETDATE(), GETDATE(), 0),
(N'Kubernetes', 'SW-K8S', N'CNCF', N'编排', N'开源', N'容器编排', GETDATE(), GETDATE(), 0),
(N'Zabbix', 'SW-ZABBIX', N'Zabbix', N'监控', N'开源', N'网络监控', GETDATE(), GETDATE(), 0),
(N'Prometheus', 'SW-PROM', N'CNCF', N'监控', N'开源', N'时序监控', GETDATE(), GETDATE(), 0);

-- sparepart: 10条
INSERT INTO sparepart (Name, Code, Specification, Unit, StockQuantity, MinStock, Price, Location, Remark, CreatedAt, UpdatedAt, IsDeleted) VALUES
(N'服务器电源', 'SP-PSU-01', N'800W冗余电源', N'个', 15, 5, 1200.00, N'库房A-A01', N'戴尔兼容', GETDATE(), GETDATE(), 0),
(N'内存条 DDR4', 'SP-RAM-01', N'32GB DDR4 ECC', N'条', 30, 10, 800.00, N'库房A-A02', N'服务器内存', GETDATE(), GETDATE(), 0),
(N'固态硬盘', 'SP-SSD-01', N'2TB NVMe SSD', N'个', 20, 8, 1500.00, N'库房A-A03', N'企业级SSD', GETDATE(), GETDATE(), 0),
(N'网卡', 'SP-NIC-01', N'万兆双口网卡', N'个', 12, 4, 600.00, N'库房A-A04', N'Intel网卡', GETDATE(), GETDATE(), 0),
(N'交换机模块', 'SP-SWM-01', N'48口千兆模块', N'个', 8, 3, 3000.00, N'库房A-B01', N'核心交换模块', GETDATE(), GETDATE(), 0),
(N'光纤跳线', 'SP-FIBER-01', N'LC-LC单模3米', N'根', 100, 30, 50.00, N'库房A-C01', N'OS2单模', GETDATE(), GETDATE(), 0),
(N'UPS电池', 'SP-BAT-01', N'12V100AH', N'组', 10, 4, 2500.00, N'UPS室', N'铅酸蓄电池', GETDATE(), GETDATE(), 0),
(N'空调滤网', 'SP-FILTER-01', N'精密空调滤网', N'片', 50, 15, 80.00, N'库房A-D01', N'可清洗滤网', GETDATE(), GETDATE(), 0),
(N'硬盘托架', 'SP-TRAY-01', N'3.5寸热插拔托架', N'个', 25, 8, 120.00, N'库房A-A05', N'通用托架', GETDATE(), GETDATE(), 0),
(N'CPU散热器', 'SP-COOLER-01', N'2U服务器散热器', N'个', 10, 3, 400.00, N'库房A-A06', N'铜底散热器', GETDATE(), GETDATE(), 0);

-- sys_dict: 10条
INSERT INTO sys_dict (DictType, Label, Value, SortOrder, IsActive, Remark, CreatedAt, UpdatedAt, IsDeleted) VALUES
('equipment_status', N'运行中', 'running', 1, 1, N'设备状态', GETDATE(), GETDATE(), 0),
('equipment_status', N'停机', 'stopped', 2, 1, N'设备状态', GETDATE(), GETDATE(), 0),
('equipment_status', N'故障', 'fault', 3, 1, N'设备状态', GETDATE(), GETDATE(), 0),
('equipment_status', N'维护中', 'maintenance', 4, 1, N'设备状态', GETDATE(), GETDATE(), 0),
('priority', N'紧急', 'urgent', 1, 1, N'优先级', GETDATE(), GETDATE(), 0),
('priority', N'高', 'high', 2, 1, N'优先级', GETDATE(), GETDATE(), 0),
('priority', N'中', 'medium', 3, 1, N'优先级', GETDATE(), GETDATE(), 0),
('priority', N'低', 'low', 4, 1, N'优先级', GETDATE(), GETDATE(), 0),
('workorder_status', N'待处理', 'pending', 1, 1, N'工单状态', GETDATE(), GETDATE(), 0),
('workorder_status', N'已完成', 'completed', 2, 1, N'工单状态', GETDATE(), GETDATE(), 0);

-- cabinet: 10条 (依赖 room 1-10)
INSERT INTO cabinet (Name, Code, RoomId, TotalUnits, UsedUnits, PowerLimit, Remark, CreatedAt, UpdatedAt, IsDeleted) VALUES
(N'A01机柜', 'CAB-A01', 1, 42, 20, 5000.0, N'主服务器机柜', GETDATE(), GETDATE(), 0),
(N'A02机柜', 'CAB-A02', 1, 42, 15, 5000.0, N'网络设备机柜', GETDATE(), GETDATE(), 0),
(N'B01机柜', 'CAB-B01', 2, 42, 10, 4500.0, N'存储设备机柜', GETDATE(), GETDATE(), 0),
(N'C01机柜', 'CAB-C01', 3, 47, 30, 6000.0, N'核心交换区', GETDATE(), GETDATE(), 0),
(N'D01机柜', 'CAB-D01', 4, 42, 5, 4000.0, N'测试设备机柜', GETDATE(), GETDATE(), 0),
(N'E01机柜', 'CAB-E01', 5, 52, 40, 8000.0, N'高密度计算区', GETDATE(), GETDATE(), 0),
(N'W01货架', 'CAB-W01', 6, 0, 0, 0.0, N'备件存放区', GETDATE(), GETDATE(), 0),
(N'O01机柜', 'CAB-O01', 7, 24, 8, 2000.0, N'办公设备机柜', GETDATE(), GETDATE(), 0),
(N'M01机柜', 'CAB-M01', 8, 24, 12, 2500.0, N'监控设备机柜', GETDATE(), GETDATE(), 0),
(N'P01机柜', 'CAB-P01', 9, 36, 18, 3500.0, N'配电设备机柜', GETDATE(), GETDATE(), 0);

-- maintenance_plan: 10条 (依赖 sys_user 1-10)
INSERT INTO maintenance_plan (Name, PlanNo, PlanType, CycleDays, StartDate, EndDate, OwnerId, Status, Remark, CreatedAt, UpdatedAt, IsDeleted) VALUES
(N'服务器巡检', 'MP-2026-001', 1, 7, '2026-01-01', '2026-12-31', 1, 1, N'每周巡检服务器状态', GETDATE(), GETDATE(), 0),
(N'网络设备检查', 'MP-2026-002', 1, 14, '2026-01-01', '2026-12-31', 2, 1, N'双周检查网络设备', GETDATE(), GETDATE(), 0),
(N'UPS维护', 'MP-2026-003', 2, 30, '2026-01-01', '2026-12-31', 3, 1, N'月度UPS维护', GETDATE(), GETDATE(), 0),
(N'空调保养', 'MP-2026-004', 2, 30, '2026-01-01', '2026-12-31', 4, 1, N'月度空调保养', GETDATE(), GETDATE(), 0),
(N'存储扩容检查', 'MP-2026-005', 3, 90, '2026-01-01', '2026-12-31', 5, 1, N'季度存储检查', GETDATE(), GETDATE(), 0),
(N'安全巡检', 'MP-2026-006', 1, 7, '2026-01-01', '2026-12-31', 1, 1, N'每周安全巡检', GETDATE(), GETDATE(), 0),
(N'备份验证', 'MP-2026-007', 3, 30, '2026-01-01', '2026-12-31', 6, 1, N'月度备份验证', GETDATE(), GETDATE(), 0),
(N'消防检查', 'MP-2026-008', 2, 30, '2026-01-01', '2026-12-31', 7, 1, N'月度消防检查', GETDATE(), GETDATE(), 0),
(N'环境监控', 'MP-2026-009', 1, 1, '2026-01-01', '2026-12-31', 8, 1, N'每日环境监控', GETDATE(), GETDATE(), 0),
(N'软件更新', 'MP-2026-010', 3, 90, '2026-01-01', '2026-12-31', 9, 1, N'季度软件更新检查', GETDATE(), GETDATE(), 0);

-- equipment: 10条 (依赖 equipment_type 1-10, cabinet 1-10)
INSERT INTO equipment (Name, Code, EquipmentTypeId, CabinetId, Position, IpAddress, MacAddress, SerialNumber, Manufacturer, Status, PurchaseDate, WarrantyDate, Remark, CreatedAt, UpdatedAt, IsDeleted) VALUES
(N'Web服务器01', 'EQ-WEB-01', 1, 1, N'U01-U04', '192.168.1.101', '00:11:22:33:44:51', 'SN2026001', N'Dell', 0, '2024-01-15', '2027-01-15', N'主Web服务器', GETDATE(), GETDATE(), 0),
(N'核心交换机', 'EQ-SW-01', 2, 2, N'U10-U12', '192.168.1.201', '00:11:22:33:44:52', 'SN2026002', N'Cisco', 0, '2024-02-20', '2027-02-20', N'核心三层交换', GETDATE(), GETDATE(), 0),
(N'边界路由器', 'EQ-RT-01', 3, 2, N'U14-U15', '192.168.1.202', '00:11:22:33:44:53', 'SN2026003', N'Huawei', 0, '2024-03-10', '2027-03-10', N'互联网出口路由', GETDATE(), GETDATE(), 0),
(N'防火墙主', 'EQ-FW-01', 4, 2, N'U20-U21', '192.168.1.203', '00:11:22:33:44:54', 'SN2026004', N'Fortinet', 0, '2024-04-05', '2027-04-05', N'主防火墙', GETDATE(), GETDATE(), 0),
(N'存储阵列', 'EQ-ST-01', 5, 3, N'U01-U08', '192.168.1.301', '00:11:22:33:44:55', 'SN2026005', N'EMC', 0, '2024-05-12', '2027-05-12', N'全闪存阵列', GETDATE(), GETDATE(), 0),
(N'UPS主机', 'EQ-UPS-01', 6, 10, N'落地', '192.168.1.401', '00:11:22:33:44:56', 'SN2026006', N'APC', 0, '2024-06-18', '2027-06-18', N'60KVA UPS', GETDATE(), GETDATE(), 0),
(N'精密空调A', 'EQ-AC-01', 7, 1, N'落地', '192.168.1.501', '00:11:22:33:44:57', 'SN2026007', N'Emerson', 0, '2024-07-22', '2027-07-22', N'行间空调', GETDATE(), GETDATE(), 0),
(N'监控主机', 'EQ-CAM-01', 8, 9, N'U05-U06', '192.168.1.601', '00:11:22:33:44:58', 'SN2026008', N'Hikvision', 0, '2024-08-30', '2027-08-30', N'NVR录像机', GETDATE(), GETDATE(), 0),
(N'门禁主控', 'EQ-AC-02', 9, 9, N'U08', '192.168.1.602', '00:11:22:33:44:59', 'SN2026009', N'Dahua', 0, '2024-09-15', '2027-09-15', N'门禁控制器', GETDATE(), GETDATE(), 0),
(N'温湿度传感器', 'EQ-SEN-01', 10, 1, N'U42', '192.168.1.701', '00:11:22:33:44:60', 'SN2026010', N'Sensirion', 0, '2024-10-01', '2027-10-01', N'环境传感器', GETDATE(), GETDATE(), 0);

-- maintenance_task: 10条 (依赖 maintenance_plan 1-10, sys_user ExecutorId 1-10)
INSERT INTO maintenance_task (PlanId, Name, PlanTime, ActualTime, ExecutorId, Status, Remark, CreatedAt, UpdatedAt, IsDeleted) VALUES
(1, N'服务器巡检-第1周', '2026-04-28 09:00', NULL, 2, 0, N'按计划执行', GETDATE(), GETDATE(), 0),
(2, N'网络设备检查-第1轮', '2026-04-29 10:00', NULL, 3, 0, N'按计划执行', GETDATE(), GETDATE(), 0),
(3, N'UPS月度维护', '2026-04-30 14:00', NULL, 3, 0, N'按计划执行', GETDATE(), GETDATE(), 0),
(4, N'空调月度保养', '2026-05-01 09:00', NULL, 4, 0, N'按计划执行', GETDATE(), GETDATE(), 0),
(5, N'存储季度检查', '2026-05-05 10:00', NULL, 5, 0, N'按计划执行', GETDATE(), GETDATE(), 0),
(6, N'安全巡检-本周', '2026-04-28 16:00', '2026-04-28 17:30', 1, 2, N'已完成', GETDATE(), GETDATE(), 0),
(7, N'备份验证-本月', '2026-04-25 02:00', '2026-04-25 03:00', 6, 2, N'备份正常', GETDATE(), GETDATE(), 0),
(8, N'消防月度检查', '2026-04-20 09:00', '2026-04-20 10:00', 7, 2, N'消防设备正常', GETDATE(), GETDATE(), 0),
(9, N'环境监控-今日', '2026-04-27 08:00', '2026-04-27 08:30', 8, 2, N'温湿度正常', GETDATE(), GETDATE(), 0),
(10, N'软件更新检查', '2026-04-15 14:00', '2026-04-15 16:00', 9, 2, N'已更新补丁', GETDATE(), GETDATE(), 0);

-- document: 10条 (依赖 equipment 1-10, sys_user UploaderId 1-10)
INSERT INTO document (Name, DocNo, Category, EquipmentId, CurrentVersion, UploaderId, Remark, CreatedAt, UpdatedAt, IsDeleted) VALUES
(N'Web服务器操作手册', 'DOC-001', N'操作手册', 1, N'v1.0', 1, N'服务器运维手册', GETDATE(), GETDATE(), 0),
(N'交换机配置指南', 'DOC-002', N'配置文档', 2, N'v1.0', 2, N'网络配置说明', GETDATE(), GETDATE(), 0),
(N'路由器维护手册', 'DOC-003', N'维护手册', 3, N'v1.0', 3, N'路由维护指南', GETDATE(), GETDATE(), 0),
(N'防火墙策略文档', 'DOC-004', N'策略文档', 4, N'v1.0', 4, N'安全策略配置', GETDATE(), GETDATE(), 0),
(N'存储阵列规格书', 'DOC-005', N'技术规格', 5, N'v1.0', 5, N'存储技术参数', GETDATE(), GETDATE(), 0),
(N'UPS维护记录', 'DOC-006', N'维护记录', 6, N'v1.0', 6, N'UPS维护日志', GETDATE(), GETDATE(), 0),
(N'空调保养手册', 'DOC-007', N'保养手册', 7, N'v1.0', 7, N'空调保养说明', GETDATE(), GETDATE(), 0),
(N'监控系统手册', 'DOC-008', N'操作手册', 8, N'v1.0', 8, N'监控操作指南', GETDATE(), GETDATE(), 0),
(N'门禁系统配置', 'DOC-009', N'配置文档', 9, N'v1.0', 9, N'门禁配置说明', GETDATE(), GETDATE(), 0),
(N'传感器安装指南', 'DOC-010', N'安装手册', 10, N'v1.0', 10, N'传感器安装说明', GETDATE(), GETDATE(), 0);

-- document_version: 10条 (依赖 document 1-10, sys_user UploaderId 1-10)
INSERT INTO document_version (DocumentId, Version, FilePath, OriginalFileName, FileSize, FileType, UploaderId, ChangeNote, CreatedAt, UpdatedAt, IsDeleted) VALUES
(1, 'v1.0', '/uploads/doc1_v1.pdf', 'web_server_manual.pdf', 2048000, 'pdf', 1, N'初始版本', GETDATE(), GETDATE(), 0),
(2, 'v1.0', '/uploads/doc2_v1.pdf', 'switch_config.pdf', 1536000, 'pdf', 2, N'初始版本', GETDATE(), GETDATE(), 0),
(3, 'v1.0', '/uploads/doc3_v1.pdf', 'router_manual.pdf', 1843200, 'pdf', 3, N'初始版本', GETDATE(), GETDATE(), 0),
(4, 'v1.0', '/uploads/doc4_v1.pdf', 'firewall_policy.pdf', 2560000, 'pdf', 4, N'初始版本', GETDATE(), GETDATE(), 0),
(5, 'v1.0', '/uploads/doc5_v1.pdf', 'storage_spec.pdf', 3072000, 'pdf', 5, N'初始版本', GETDATE(), GETDATE(), 0),
(6, 'v1.0', '/uploads/doc6_v1.pdf', 'ups_record.pdf', 1024000, 'pdf', 6, N'初始版本', GETDATE(), GETDATE(), 0),
(7, 'v1.0', '/uploads/doc7_v1.pdf', 'ac_manual.pdf', 1792000, 'pdf', 7, N'初始版本', GETDATE(), GETDATE(), 0),
(8, 'v1.0', '/uploads/doc8_v1.pdf', 'camera_manual.pdf', 1280000, 'pdf', 8, N'初始版本', GETDATE(), GETDATE(), 0),
(9, 'v1.0', '/uploads/doc9_v1.pdf', 'access_config.pdf', 2048000, 'pdf', 9, N'初始版本', GETDATE(), GETDATE(), 0),
(10, 'v1.0', '/uploads/doc10_v1.pdf', 'sensor_guide.pdf', 1536000, 'pdf', 10, N'初始版本', GETDATE(), GETDATE(), 0);

-- software_version: 10条 (依赖 software 1-10)
INSERT INTO software_version (SoftwareId, Version, ReleaseDate, ChangeLog, PackagePath, CreatedAt, UpdatedAt, IsDeleted) VALUES
(1, '2022-21H2', '2024-01-15', N'Windows Server 2022 21H2', '/packages/ws2022.iso', GETDATE(), GETDATE(), 0),
(2, '9.2', '2024-02-20', N'RHEL 9.2', '/packages/rhel92.iso', GETDATE(), GETDATE(), 0),
(3, '8.0', '2024-03-10', N'vSphere 8.0', '/packages/vsphere8.iso', GETDATE(), GETDATE(), 0),
(4, '4.4.14', '2024-04-05', N'Nagios Core 4.4.14', '/packages/nagios.tar.gz', GETDATE(), GETDATE(), 0),
(5, '2022-CU10', '2024-05-12', N'SQL Server 2022 CU10', '/packages/sql2022cu10.iso', GETDATE(), GETDATE(), 0),
(6, '19c', '2024-06-18', N'Oracle 19c', '/packages/oracle19c.zip', GETDATE(), GETDATE(), 0),
(7, '24.0.7', '2024-07-22', N'Docker CE 24.0.7', '/packages/docker.tgz', GETDATE(), GETDATE(), 0),
(8, '1.29', '2024-08-30', N'Kubernetes 1.29', '/packages/k8s.tar.gz', GETDATE(), GETDATE(), 0),
(9, '6.4', '2024-09-15', N'Zabbix 6.4', '/packages/zabbix64.tar.gz', GETDATE(), GETDATE(), 0),
(10, '2.48', '2024-10-01', N'Prometheus 2.48', '/packages/prometheus.tar.gz', GETDATE(), GETDATE(), 0);

-- software_instance: 10条 (依赖 software_version 1-10, equipment 1-10)
INSERT INTO software_instance (SoftwareVersionId, EquipmentId, InstallPath, InstallDate, Status, Remark, CreatedAt, UpdatedAt, IsDeleted) VALUES
(1, 1, N'C:\\Windows', '2024-01-20', 0, N'Web服务器系统', GETDATE(), GETDATE(), 0),
(2, 1, N'/opt/rhel', '2024-02-25', 0, N'虚拟机宿主机', GETDATE(), GETDATE(), 0),
(3, 1, N'/vmfs/volumes', '2024-03-15', 0, N'虚拟化层', GETDATE(), GETDATE(), 0),
(4, 2, N'/opt/nagios', '2024-04-10', 0, N'监控系统', GETDATE(), GETDATE(), 0),
(5, 1, N'C:\\Program Files\\Microsoft SQL Server', '2024-05-18', 0, N'数据库服务', GETDATE(), GETDATE(), 0),
(6, 5, N'/opt/oracle', '2024-06-22', 0, N'备用数据库', GETDATE(), GETDATE(), 0),
(7, 1, N'/usr/bin/docker', '2024-07-25', 0, N'容器平台', GETDATE(), GETDATE(), 0),
(8, 1, N'/usr/local/bin/kubectl', '2024-09-02', 0, N'容器编排', GETDATE(), GETDATE(), 0),
(9, 2, N'/opt/zabbix', '2024-09-20', 0, N'网络监控', GETDATE(), GETDATE(), 0),
(10, 2, N'/opt/prometheus', '2024-10-05', 0, N'时序监控', GETDATE(), GETDATE(), 0);

-- subsystem: 10条 (依赖 equipment 1-10)
INSERT INTO subsystem (Name, Code, CategoryId, Status, CreatedAt, UpdatedAt, IsDeleted) VALUES
(N'电源模块', 'SUB-PSU-01', 1, 0, GETDATE(), GETDATE(), 0),
(N'风扇模块', 'SUB-FAN-01', 1, 0, GETDATE(), GETDATE(), 0),
(N'管理口', 'SUB-MGMT-01', 2, 0, GETDATE(), GETDATE(), 0),
(N'业务口', 'SUB-BUS-01', 2, 0, GETDATE(), GETDATE(), 0),
(N'控制平面', 'SUB-CTRL-01', 3, 0, GETDATE(), GETDATE(), 0),
(N'转发平面', 'SUB-FWD-01', 3, 0, GETDATE(), GETDATE(), 0),
(N'控制器', 'SUB-CTRL-02', 5, 0, GETDATE(), GETDATE(), 0),
(N'控制器B', 'SUB-CTRL-03', 5, 0, GETDATE(), GETDATE(), 0),
(N'电池组', 'SUB-BAT-01', 6, 0, GETDATE(), GETDATE(), 0),
(N'压缩机', 'SUB-COMP-01', 7, 0, GETDATE(), GETDATE(), 0);

-- maintenance_item: 10条 (依赖 maintenance_task 1-10)
INSERT INTO maintenance_item (TaskId, Name, Description, Result, AbnormalNote, CreatedAt, UpdatedAt, IsDeleted) VALUES
(1, N'CPU检查', N'检查CPU使用率', 0, NULL, GETDATE(), GETDATE(), 0),
(1, N'内存检查', N'检查内存使用率', 0, NULL, GETDATE(), GETDATE(), 0),
(2, N'端口检查', N'检查交换机端口状态', 0, NULL, GETDATE(), GETDATE(), 0),
(2, N'VLAN检查', N'检查VLAN配置', 0, NULL, GETDATE(), GETDATE(), 0),
(3, N'电池电压', N'测量电池组电压', 0, NULL, GETDATE(), GETDATE(), 0),
(4, N'滤网清洁', N'清洁空调滤网', 0, NULL, GETDATE(), GETDATE(), 0),
(5, N'容量检查', N'检查存储容量', 0, NULL, GETDATE(), GETDATE(), 0),
(6, N'日志审计', N'审计安全日志', 0, NULL, GETDATE(), GETDATE(), 0),
(7, N'备份完整性', N'验证备份完整性', 0, NULL, GETDATE(), GETDATE(), 0),
(8, N'灭火器检查', N'检查灭火器压力', 0, NULL, GETDATE(), GETDATE(), 0);

-- stock_in_record: 10条 (依赖 sparepart 1-10, sys_user OperatorId=5)
INSERT INTO stock_in_record (SparepartId, Quantity, UnitPrice, Supplier, InDate, OperatorId, Remark, CreatedAt, UpdatedAt, IsDeleted) VALUES
(1, 10, 1200.00, N'Dell官方', '2026-01-10', 5, N'首批采购', GETDATE(), GETDATE(), 0),
(2, 20, 800.00, N'京东企业购', '2026-01-15', 5, N'批量采购', GETDATE(), GETDATE(), 0),
(3, 15, 1500.00, N'Intel官方', '2026-02-01', 5, N'紧急采购', GETDATE(), GETDATE(), 0),
(4, 8, 600.00, N'TP-LINK', '2026-02-10', 5, N'常规采购', GETDATE(), GETDATE(), 0),
(5, 5, 3000.00, N'H3C官方', '2026-03-01', 5, N'项目采购', GETDATE(), GETDATE(), 0),
(6, 50, 50.00, N'光纤之家', '2026-03-05', 5, N'批量采购', GETDATE(), GETDATE(), 0),
(7, 6, 2500.00, N'APC官方', '2026-03-10', 5, N'维护采购', GETDATE(), GETDATE(), 0),
(8, 30, 80.00, N'艾默生', '2026-03-15', 5, N'常规采购', GETDATE(), GETDATE(), 0),
(9, 15, 120.00, N'戴尔配件', '2026-03-20', 5, N'补充库存', GETDATE(), GETDATE(), 0),
(10, 8, 400.00, N'超微', '2026-03-25', 5, N'项目采购', GETDATE(), GETDATE(), 0);

-- stock_out_record: 10条 (依赖 sparepart 1-10, sys_user OperatorId=5)
INSERT INTO stock_out_record (SparepartId, Quantity, Department, Recipient, OutDate, OperatorId, Purpose, CreatedAt, UpdatedAt, IsDeleted) VALUES
(1, 2, N'运维部', N'Manager A', '2026-04-01', 5, N'服务器电源更换', GETDATE(), GETDATE(), 0),
(2, 4, N'运维部', N'Manager B', '2026-04-02', 5, N'内存扩容', GETDATE(), GETDATE(), 0),
(3, 2, N'运维部', N'Manager C', '2026-04-03', 5, N'硬盘更换', GETDATE(), GETDATE(), 0),
(4, 1, N'网络部', N'Manager D', '2026-04-05', 5, N'网卡升级', GETDATE(), GETDATE(), 0),
(5, 1, N'网络部', N'Manager E', '2026-04-06', 5, N'交换模块更换', GETDATE(), GETDATE(), 0),
(6, 10, N'工程部', N'Manager F', '2026-04-08', 5, N'光纤布线', GETDATE(), GETDATE(), 0),
(7, 1, N'运维部', N'Manager G', '2026-04-10', 5, N'UPS电池更换', GETDATE(), GETDATE(), 0),
(8, 5, N'运维部', N'Manager H', '2026-04-12', 5, N'滤网更换', GETDATE(), GETDATE(), 0),
(9, 3, N'运维部', N'Manager I', '2026-04-15', 5, N'托架更换', GETDATE(), GETDATE(), 0),
(10, 1, N'运维部', N'Manager J', '2026-04-18', 5, N'散热器更换', GETDATE(), GETDATE(), 0);

-- workorder: 10条 (依赖 fault_type 1-10, equipment 1-10, sys_user CreatorId/HandlerId 1-10)
INSERT INTO workorder (WorkorderNo, Title, Description, FaultTypeId, EquipmentId, Priority, Status, ResponseDeadlineMinutes, FixDeadlineMinutes, ActualResponseTime, CreatorId, HandlerId, PlanFinishTime, ActualFinishTime, CreatedAt, UpdatedAt, IsDeleted) VALUES
('WO-2026-001', N'Web服务器宕机', N'Web服务器无法访问，需要紧急处理', 1, 1, 0, 1, 30, 240, '2026-04-27 10:05', 1, 3, '2026-04-27 14:00', NULL, GETDATE(), GETDATE(), 0),
('WO-2026-002', N'交换机端口故障', N'核心交换机端口Down', 3, 2, 1, 1, 60, 480, '2026-04-27 11:10', 2, 3, '2026-04-27 19:00', NULL, GETDATE(), GETDATE(), 0),
('WO-2026-003', N'UPS报警', N'UPS电池电压低报警', 4, 6, 1, 2, 60, 360, '2026-04-26 09:15', 3, 3, '2026-04-26 15:00', '2026-04-26 14:30', GETDATE(), GETDATE(), 0),
('WO-2026-004', N'空调制冷不足', N'机房温度升高', 5, 7, 2, 2, 120, 720, '2026-04-25 14:20', 4, 4, '2026-04-26 14:00', '2026-04-26 10:00', GETDATE(), GETDATE(), 0),
('WO-2026-005', N'存储扩容', N'存储容量不足，需要扩容', 8, 5, 2, 3, 240, 1440, NULL, 5, 5, '2026-05-10 18:00', NULL, GETDATE(), GETDATE(), 0),
('WO-2026-006', N'防火墙规则更新', N'更新安全策略', 9, 4, 3, 3, 480, 2880, NULL, 6, 6, '2026-05-15 18:00', NULL, GETDATE(), GETDATE(), 0),
('WO-2026-007', N'监控摄像头离线', N'部分摄像头无法连接', 3, 8, 2, 2, 120, 720, '2026-04-24 08:30', 7, 7, '2026-04-24 20:00', '2026-04-24 16:00', GETDATE(), GETDATE(), 0),
('WO-2026-008', N'门禁失效', N'门禁系统无法刷卡', 2, 9, 1, 2, 60, 360, '2026-04-23 09:00', 8, 8, '2026-04-23 15:00', '2026-04-23 12:00', GETDATE(), GETDATE(), 0),
('WO-2026-009', N'传感器误报', N'温湿度传感器频繁报警', 5, 10, 3, 2, 240, 1440, '2026-04-22 10:00', 9, 9, '2026-04-23 10:00', '2026-04-22 18:00', GETDATE(), GETDATE(), 0),
('WO-2026-010', N'路由器配置备份', N'定期备份路由器配置', 8, 3, 3, 0, 480, 2880, NULL, 10, 10, '2026-04-30 18:00', NULL, GETDATE(), GETDATE(), 0);

-- workorder_log: 10条 (依赖 workorder 1-10, sys_user OperatorId 1-10)
INSERT INTO workorder_log (WorkorderId, OperatorId, ActionType, Content, CreatedAt, UpdatedAt, IsDeleted) VALUES
(1, 1, N'创建', N'创建工单：Web服务器宕机', GETDATE(), GETDATE(), 0),
(2, 2, N'创建', N'创建工单：交换机端口故障', GETDATE(), GETDATE(), 0),
(3, 3, N'处理', N'已更换UPS电池', GETDATE(), GETDATE(), 0),
(4, 4, N'处理', N'已补充制冷剂', GETDATE(), GETDATE(), 0),
(5, 5, N'创建', N'创建工单：存储扩容', GETDATE(), GETDATE(), 0),
(6, 6, N'创建', N'创建工单：防火墙规则更新', GETDATE(), GETDATE(), 0),
(7, 7, N'处理', N'已修复网络连接', GETDATE(), GETDATE(), 0),
(8, 8, N'处理', N'已重启门禁服务', GETDATE(), GETDATE(), 0),
(9, 9, N'处理', N'已校准传感器', GETDATE(), GETDATE(), 0),
(10, 10, N'创建', N'创建工单：路由器配置备份', GETDATE(), GETDATE(), 0);

-- sys_log: 10条 (依赖 sys_user UserId 1-10)
INSERT INTO sys_log (UserId, Action, Content, IpAddress, Method, Path, StatusCode, ElapsedMs, CreatedAt, UpdatedAt, IsDeleted) VALUES
(1, N'登录', N'用户登录成功', '192.168.1.100', 'POST', '/api/Auth/login', 200, 150, GETDATE(), GETDATE(), 0),
(2, N'查询', N'查询设备列表', '192.168.1.101', 'GET', '/api/Equipment', 200, 80, GETDATE(), GETDATE(), 0),
(3, N'查询', N'查询机柜列表', '192.168.1.102', 'GET', '/api/Cabinet', 200, 120, GETDATE(), GETDATE(), 0),
(4, N'创建', N'创建维护计划', '192.168.1.103', 'POST', '/api/maintenance/plans', 200, 200, GETDATE(), GETDATE(), 0),
(5, N'更新', N'更新备件库存', '192.168.1.104', 'PUT', '/api/sparepart/1', 200, 180, GETDATE(), GETDATE(), 0),
(6, N'删除', N'删除文档', '192.168.1.105', 'DELETE', '/api/Document/1', 200, 100, GETDATE(), GETDATE(), 0),
(7, N'查询', N'查询工单列表', '192.168.1.106', 'GET', '/api/workorder', 200, 250, GETDATE(), GETDATE(), 0),
(8, N'创建', N'创建工单', '192.168.1.107', 'POST', '/api/workorder', 200, 300, GETDATE(), GETDATE(), 0),
(9, N'导出', N'导出日志', '192.168.1.108', 'GET', '/api/Log/export', 200, 500, GETDATE(), GETDATE(), 0),
(10, N'登录', N'用户登录成功', '192.168.1.109', 'POST', '/api/Auth/login', 200, 130, GETDATE(), GETDATE(), 0);
