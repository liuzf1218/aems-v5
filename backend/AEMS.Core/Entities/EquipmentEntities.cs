using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AEMS.Core.Entities;

/// <summary>
/// 设备类型
/// </summary>
[Table("equipment_type")]
public class EquipmentType : BaseEntity
{
    /// <summary>
    /// 类型名称
    /// </summary>
    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 类型编码
    /// </summary>
    [Required, MaxLength(50)]
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// 备注
    /// </summary>
    [MaxLength(500)]
    public string? Remark { get; set; }

    /// <summary>
    /// 父类型ID
    /// </summary>
    public int? ParentId { get; set; }

    /// <summary>
    /// 导航属性 - 父类型
    /// </summary>
    [ForeignKey("ParentId")]
    public EquipmentType? Parent { get; set; }

    /// <summary>
    /// 导航属性 - 子类型列表
    /// </summary>
    public ICollection<EquipmentType> Children { get; set; } = new List<EquipmentType>();

    /// <summary>
    /// 导航属性 - 设备列表
    /// </summary>
    public ICollection<Equipment> Equipments { get; set; } = new List<Equipment>();

    /// <summary>
    /// 导航属性 - 系统列表
    /// </summary>
    public ICollection<Subsystem> Subsystems { get; set; } = new List<Subsystem>();
}

/// <summary>
/// 设备
/// </summary>
[Table("equipment")]
public class Equipment : BaseEntity
{
    /// <summary>
    /// 设备名称
    /// </summary>
    [Required, MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 设备编号
    /// </summary>
    [Required, MaxLength(50)]
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// 设备类型ID（技术类型：服务器/交换机/路由器等）
    /// </summary>
    public int? EquipmentTypeId { get; set; }

    /// <summary>
    /// 所属系统ID（业务系统：ILS/VOR/VHF/雷达等）
    /// </summary>
    public int? SubsystemId { get; set; }

    /// <summary>
    /// 机柜ID
    /// </summary>
    public int? CabinetId { get; set; }

    /// <summary>
    /// 位置/U位
    /// </summary>
    [MaxLength(50)]
    public string? Position { get; set; }

    /// <summary>
    /// IP地址
    /// </summary>
    [MaxLength(50)]
    public string? IpAddress { get; set; }

    /// <summary>
    /// MAC地址
    /// </summary>
    [MaxLength(50)]
    public string? MacAddress { get; set; }

    /// <summary>
    /// 序列号
    /// </summary>
    [MaxLength(100)]
    public string? SerialNumber { get; set; }

    /// <summary>
    /// 厂商
    /// </summary>
    [MaxLength(100)]
    public string? Manufacturer { get; set; }

    /// <summary>
    /// 设备型号
    /// </summary>
    [MaxLength(100)]
    public string? Model { get; set; }

    /// <summary>
    /// 设备状态：0-在用/正常 1-故障 2-维护中
    /// </summary>
    public int Status { get; set; } = 0;

    /// <summary>
    /// 重要性：1=A级 2=B级 3=C级
    /// </summary>
    public int Criticality { get; set; } = 2;

    /// <summary>
    /// 运行时长（小时）
    /// </summary>
    public int RuntimeHours { get; set; } = 0;

    /// <summary>
    /// 上次维保日期
    /// </summary>
    public DateTime? LastMaintenanceDate { get; set; }

    /// <summary>
    /// 下次维保日期
    /// </summary>
    public DateTime? NextMaintenanceDate { get; set; }

    /// <summary>
    /// 故障次数
    /// </summary>
    public int FailureCount { get; set; } = 0;

    /// <summary>
    /// 安装日期
    /// </summary>
    public DateTime? InstallDate { get; set; }

    /// <summary>
    /// 购买日期
    /// </summary>
    public DateTime? PurchaseDate { get; set; }

    /// <summary>
    /// 保修到期日期
    /// </summary>
    public DateTime? WarrantyDate { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    [MaxLength(500)]
    public string? Remark { get; set; }

    /// <summary>
    /// 所属机房ID（用于无机柜的设备/附属设施直接关联机房）
    /// </summary>
    public int? RoomId { get; set; }

    /// <summary>
    /// 导航属性 - 设备类型
    /// </summary>
    public EquipmentType? EquipmentType { get; set; }

    /// <summary>
    /// 导航属性 - 所属系统
    /// </summary>
    public Subsystem? Subsystem { get; set; }

    /// <summary>
    /// 导航属性 - 机柜
    /// </summary>
    public Cabinet? Cabinet { get; set; }

    /// <summary>
    /// 导航属性 - 机房
    /// </summary>
    public Room? Room { get; set; }
}

/// <summary>
/// 业务系统/子系统（ILS/VOR/VHF/雷达等）
/// </summary>
[Table("subsystem")]
public class Subsystem : BaseEntity
{
    /// <summary>
    /// 系统名称
    /// </summary>
    [Required, MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 系统编码
    /// </summary>
    [Required, MaxLength(50)]
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// 所属设备分类ID（导航/通信/监视/气象/信息化）
    /// </summary>
    public int CategoryId { get; set; }

    /// <summary>
    /// 状态：0-停用 1-正常
    /// </summary>
    public int Status { get; set; } = 1;

    /// <summary>
    /// 导航属性 - 所属分类
    /// </summary>
    public EquipmentType? Category { get; set; }

    /// <summary>
    /// 导航属性 - 设备列表
    /// </summary>
    public ICollection<Equipment> Equipments { get; set; } = new List<Equipment>();
}
