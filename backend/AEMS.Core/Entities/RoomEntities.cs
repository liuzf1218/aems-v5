using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AEMS.Core.Entities;

/// <summary>
/// 机房
/// </summary>
[Table("room")]
public class Room : BaseEntity
{
    /// <summary>
    /// 机房名称
    /// </summary>
    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 机房编码
    /// </summary>
    [Required, MaxLength(50)]
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// 位置/地址
    /// </summary>
    [MaxLength(200)]
    public string? Location { get; set; }

    /// <summary>
    /// 面积（平方米）
    /// </summary>
    [Column(TypeName = "decimal(10,2)")]
    public decimal? Area { get; set; }

    /// <summary>
    /// 温度上限
    /// </summary>
    [Column(TypeName = "decimal(5,1)")]
    public decimal? TempUpper { get; set; }

    /// <summary>
    /// 湿度上限
    /// </summary>
    [Column(TypeName = "decimal(5,1)")]
    public decimal? HumidityUpper { get; set; }

    /// <summary>
    /// 负责人
    /// </summary>
    [MaxLength(50)]
    public string? Manager { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    [MaxLength(500)]
    public string? Remark { get; set; }

    /// <summary>
    /// 所属楼宇ID
    /// </summary>
    public int? BuildingId { get; set; }

    /// <summary>
    /// 楼层
    /// </summary>
    [MaxLength(50)]
    public string? Floor { get; set; }

    /// <summary>
    /// 导航属性 - 所属楼宇
    /// </summary>
    public Building? Building { get; set; }

    /// <summary>
    /// 导航属性 - 机柜列表
    /// </summary>
    public ICollection<Cabinet> Cabinets { get; set; } = new List<Cabinet>();

    /// <summary>
    /// 导航属性 - 设备列表（含附属设施）
    /// </summary>
    public ICollection<Equipment> Equipments { get; set; } = new List<Equipment>();
}

/// <summary>
/// 机柜
/// </summary>
[Table("cabinet")]
public class Cabinet : BaseEntity
{
    /// <summary>
    /// 机柜名称
    /// </summary>
    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 机柜编号
    /// </summary>
    [Required, MaxLength(50)]
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// 机房ID
    /// </summary>
    public int RoomId { get; set; }

    /// <summary>
    /// U位总数
    /// </summary>
    public int TotalUnits { get; set; } = 42;

    /// <summary>
    /// 已用U位
    /// </summary>
    public int UsedUnits { get; set; } = 0;

    /// <summary>
    /// 功率上限（kW）
    /// </summary>
    [Column(TypeName = "decimal(10,2)")]
    public decimal? PowerLimit { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    [MaxLength(500)]
    public string? Remark { get; set; }

    /// <summary>
    /// 导航属性 - 机房
    /// </summary>
    public Room Room { get; set; } = null!;

    /// <summary>
    /// 导航属性 - 设备列表
    /// </summary>
    public ICollection<Equipment> Equipments { get; set; } = new List<Equipment>();
}
