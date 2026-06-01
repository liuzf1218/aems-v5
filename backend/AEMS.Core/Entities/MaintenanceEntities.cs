using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AEMS.Core.Entities;

/// <summary>
/// 维护计划
/// </summary>
[Table("maintenance_plan")]
public class MaintenancePlan : BaseEntity
{
    /// <summary>
    /// 计划名称
    /// </summary>
    [Required, MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 计划编号
    /// </summary>
    [Required, MaxLength(50)]
    public string PlanNo { get; set; } = string.Empty;

    /// <summary>
    /// 维护类型：1-日常维护 2-定期维护 3-专项维护
    /// </summary>
    public int PlanType { get; set; } = 1;

    /// <summary>
    /// 执行周期（天）
    /// </summary>
    public int? CycleDays { get; set; }

    /// <summary>
    /// 开始日期
    /// </summary>
    public DateTime? StartDate { get; set; }

    /// <summary>
    /// 结束日期
    /// </summary>
    public DateTime? EndDate { get; set; }

    /// <summary>
    /// 负责人ID
    /// </summary>
    public int? OwnerId { get; set; }

    /// <summary>
    /// 状态：0-未开始 1-进行中 2-已完成 3-已取消
    /// </summary>
    public int Status { get; set; } = 0;

    /// <summary>
    /// 备注
    /// </summary>
    [MaxLength(500)]
    public string? Remark { get; set; }

    /// <summary>
    /// 关联设备ID
    /// </summary>
    public int? EquipmentId { get; set; }

    /// <summary>
    /// 导航属性 - 设备
    /// </summary>
    public Equipment? Equipment { get; set; }

    /// <summary>
    /// 导航属性 - 维护任务
    /// </summary>
    public ICollection<MaintenanceTask> Tasks { get; set; } = new List<MaintenanceTask>();
}

/// <summary>
/// 维护任务
/// </summary>
[Table("maintenance_task")]
public class MaintenanceTask : BaseEntity
{
    /// <summary>
    /// 维护计划ID
    /// </summary>
    public int PlanId { get; set; }

    /// <summary>
    /// 任务名称
    /// </summary>
    [Required, MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 计划执行时间
    /// </summary>
    public DateTime? PlanTime { get; set; }

    /// <summary>
    /// 实际执行时间
    /// </summary>
    public DateTime? ActualTime { get; set; }

    /// <summary>
    /// 执行人ID
    /// </summary>
    public int? ExecutorId { get; set; }

    /// <summary>
    /// 状态：0-待执行 1-执行中 2-已完成
    /// </summary>
    public int Status { get; set; } = 0;

    /// <summary>
    /// 维护内容
    /// </summary>
    [MaxLength(500)]
    public string? Content { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    [MaxLength(500)]
    public string? Remark { get; set; }

    /// <summary>
    /// 导航属性 - 维护计划
    /// </summary>
    public MaintenancePlan Plan { get; set; } = null!;

    /// <summary>
    /// 导航属性 - 维护项
    /// </summary>
    public ICollection<MaintenanceItem> Items { get; set; } = new List<MaintenanceItem>();
}

/// <summary>
/// 维护项（检查点）
/// </summary>
[Table("maintenance_item")]
public class MaintenanceItem : BaseEntity
{
    /// <summary>
    /// 维护任务ID
    /// </summary>
    public int TaskId { get; set; }

    /// <summary>
    /// 检查项名称
    /// </summary>
    [Required, MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 检查项描述
    /// </summary>
    [MaxLength(500)]
    public string? Description { get; set; }

    /// <summary>
    /// 检查结果：0-未检查 1-正常 2-异常
    /// </summary>
    public int Result { get; set; } = 0;

    /// <summary>
    /// 异常说明
    /// </summary>
    [MaxLength(500)]
    public string? AbnormalNote { get; set; }

    /// <summary>
    /// 导航属性 - 维护任务
    /// </summary>
    public MaintenanceTask Task { get; set; } = null!;
}
