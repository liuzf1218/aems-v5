using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AEMS.Core.Entities;

/// <summary>
/// 备件
/// </summary>
[Table("sparepart")]
public class Sparepart : BaseEntity
{
    /// <summary>
    /// 备件名称
    /// </summary>
    [Required, MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 备件编号
    /// </summary>
    [Required, MaxLength(50)]
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// 规格型号
    /// </summary>
    [MaxLength(100)]
    public string? Specification { get; set; }

    /// <summary>
    /// 单位
    /// </summary>
    [MaxLength(20)]
    public string? Unit { get; set; }

    /// <summary>
    /// 当前库存数量
    /// </summary>
    public int StockQuantity { get; set; } = 0;

    /// <summary>
    /// 最低库存预警
    /// </summary>
    public int MinStock { get; set; } = 0;

    /// <summary>
    /// 单价
    /// </summary>
    [Column(TypeName = "decimal(18,2)")]
    public decimal? Price { get; set; }

    /// <summary>
    /// 存放位置
    /// </summary>
    [MaxLength(100)]
    public string? Location { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    [MaxLength(500)]
    public string? Remark { get; set; }

    /// <summary>
    /// 所属系统ID
    /// </summary>
    public int? SubsystemId { get; set; }

    /// <summary>
    /// 导航属性 - 所属系统
    /// </summary>
    public Subsystem? Subsystem { get; set; }

    /// <summary>
    /// 导航属性 - 入库记录
    /// </summary>
    public ICollection<StockInRecord> StockInRecords { get; set; } = new List<StockInRecord>();

    /// <summary>
    /// 导航属性 - 出库记录
    /// </summary>
    public ICollection<StockOutRecord> StockOutRecords { get; set; } = new List<StockOutRecord>();
}

/// <summary>
/// 入库记录
/// </summary>
[Table("stock_in_record")]
public class StockInRecord : BaseEntity
{
    /// <summary>
    /// 备件ID
    /// </summary>
    public int SparepartId { get; set; }

    /// <summary>
    /// 入库数量
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// 单价
    /// </summary>
    [Column(TypeName = "decimal(18,2)")]
    public decimal? UnitPrice { get; set; }

    /// <summary>
    /// 供应商
    /// </summary>
    [MaxLength(100)]
    public string? Supplier { get; set; }

    /// <summary>
    /// 入库日期
    /// </summary>
    public DateTime? InDate { get; set; }

    /// <summary>
    /// 经手人ID
    /// </summary>
    public int? OperatorId { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    [MaxLength(500)]
    public string? Remark { get; set; }

    /// <summary>
    /// 导航属性 - 备件
    /// </summary>
    public Sparepart Sparepart { get; set; } = null!;
}

/// <summary>
/// 出库记录
/// </summary>
[Table("stock_out_record")]
public class StockOutRecord : BaseEntity
{
    /// <summary>
    /// 备件ID
    /// </summary>
    public int SparepartId { get; set; }

    /// <summary>
    /// 出库数量
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// 领用部门
    /// </summary>
    [MaxLength(100)]
    public string? Department { get; set; }

    /// <summary>
    /// 领用人
    /// </summary>
    [MaxLength(50)]
    public string? Recipient { get; set; }

    /// <summary>
    /// 出库日期
    /// </summary>
    public DateTime? OutDate { get; set; }

    /// <summary>
    /// 经手人ID
    /// </summary>
    public int? OperatorId { get; set; }

    /// <summary>
    /// 用途
    /// </summary>
    [MaxLength(500)]
    public string? Purpose { get; set; }

    /// <summary>
    /// 导航属性 - 备件
    /// </summary>
    public Sparepart Sparepart { get; set; } = null!;
}
