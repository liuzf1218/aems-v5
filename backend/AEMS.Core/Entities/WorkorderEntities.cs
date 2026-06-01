using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AEMS.Core.Entities;

/// <summary>
/// 故障类型
/// </summary>
[Table("fault_type")]
public class FaultType : BaseEntity
{
    /// <summary>
    /// 故障类型名称
    /// </summary>
    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 故障类型编码
    /// </summary>
    [Required, MaxLength(50)]
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// 备注
    /// </summary>
    [MaxLength(500)]
    public string? Remark { get; set; }
}

/// <summary>
/// 工单
/// </summary>
[Table("workorder")]
public class Workorder : BaseEntity
{
    /// <summary>
    /// 工单编号
    /// </summary>
    [Required, MaxLength(50)]
    public string WorkorderNo { get; set; } = string.Empty;

    /// <summary>
    /// 工单标题
    /// </summary>
    [Required, MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// 工单描述
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 故障类型ID
    /// </summary>
    public int? FaultTypeId { get; set; }

    /// <summary>
    /// 设备ID
    /// </summary>
    public int? EquipmentId { get; set; }

    /// <summary>
    /// 优先级：1-低 2-中 3-高 4-紧急
    /// </summary>
    public int Priority { get; set; } = 2;

    /// <summary>
    /// 工单状态：0-待受理 1-处理中 2-待验收 3-已完成 4-已归档
    /// </summary>
    public int Status { get; set; } = 0;

    /// <summary>
    /// SLA响应时限（分钟），根据优先级自动计算
    /// </summary>
    public int? ResponseDeadlineMinutes { get; set; }

    /// <summary>
    /// SLA修复时限（分钟），根据优先级自动计算
    /// </summary>
    public int? FixDeadlineMinutes { get; set; }

    /// <summary>
    /// 故障现象
    /// </summary>
    [MaxLength(500)]
    public string? Symptom { get; set; }

    /// <summary>
    /// 解决方案
    /// </summary>
    [MaxLength(500)]
    public string? Solution { get; set; }

    /// <summary>
    /// 实际响应时间（首次受理时间）
    /// </summary>
    public DateTime? ActualResponseTime { get; set; }

    /// <summary>
    /// 创建人ID
    /// </summary>
    public int? CreatorId { get; set; }

    /// <summary>
    /// 处理人ID
    /// </summary>
    public int? HandlerId { get; set; }

    /// <summary>
    /// 计划完成时间
    /// </summary>
    public DateTime? PlanFinishTime { get; set; }

    /// <summary>
    /// 实际完成时间
    /// </summary>
    public DateTime? ActualFinishTime { get; set; }

    /// <summary>
    /// 导航属性 - 故障类型
    /// </summary>
    public FaultType? FaultType { get; set; }

    /// <summary>
    /// 导航属性 - 设备
    /// </summary>
    public Equipment? Equipment { get; set; }

    /// <summary>
    /// 导航属性 - 工单日志
    /// </summary>
    public ICollection<WorkorderLog> Logs { get; set; } = new List<WorkorderLog>();
}

/// <summary>
/// 工单日志
/// </summary>
[Table("workorder_log")]
public class WorkorderLog : BaseEntity
{
    /// <summary>
    /// 工单ID
    /// </summary>
    public int WorkorderId { get; set; }

    /// <summary>
    /// 操作人ID
    /// </summary>
    public int? OperatorId { get; set; }

    /// <summary>
    /// 操作类型
    /// </summary>
    [MaxLength(50)]
    public string? ActionType { get; set; }

    /// <summary>
    /// 操作内容
    /// </summary>
    [MaxLength(1000)]
    public string? Content { get; set; }

    /// <summary>
    /// 导航属性 - 工单
    /// </summary>
    public Workorder Workorder { get; set; } = null!;
}
