using System.ComponentModel.DataAnnotations;

namespace AEMS.Core.DTOs;

/// <summary>
/// 维护计划查询参数
/// </summary>
public class MaintenancePlanQueryRequest : PagedRequest
{
    /// <summary>
    /// 关键词搜索（名称/编号）
    /// </summary>
    public string? Keyword { get; set; }

    /// <summary>
    /// 计划类型：1-日常维护 2-定期维护 3-专项维护
    /// </summary>
    public int? PlanType { get; set; }

    /// <summary>
    /// 状态：0-未开始 1-进行中 2-已完成 3-已取消
    /// </summary>
    public int? Status { get; set; }
}

/// <summary>
/// 维护计划创建/更新请求
/// </summary>
public class MaintenancePlanRequest
{
    [Required(ErrorMessage = "计划名称不能为空")]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "计划编号不能为空")]
    [MaxLength(50)]
    public string PlanNo { get; set; } = string.Empty;

    /// <summary>
    /// 维护类型：1-日常维护 2-定期维护 3-专项维护
    /// </summary>
    public int PlanType { get; set; } = 1;

    /// <summary>
    /// 执行周期（天）
    /// </summary>
    public int? CycleDays { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public int? OwnerId { get; set; }

    public int Status { get; set; } = 0;

    [MaxLength(500)]
    public string? Remark { get; set; }

    /// <summary>
    /// 关联设备ID
    /// </summary>
    public int? EquipmentId { get; set; }
}

/// <summary>
/// 维护计划列表响应DTO
/// </summary>
public class MaintenancePlanDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string PlanNo { get; set; } = string.Empty;
    public int PlanType { get; set; }
    public string PlanTypeName { get; set; } = string.Empty;
    public int? CycleDays { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int? OwnerId { get; set; }
    public int Status { get; set; }
    public string StatusName { get; set; } = string.Empty;
    public string? Remark { get; set; }
    public string? EquipmentName { get; set; }
    public string? SystemName { get; set; }
    public int TaskCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// 维护任务查询参数
/// </summary>
public class MaintenanceTaskQueryRequest : PagedRequest
{
    /// <summary>
    /// 关键词搜索
    /// </summary>
    public string? Keyword { get; set; }

    /// <summary>
    /// 计划ID
    /// </summary>
    public int? PlanId { get; set; }

    /// <summary>
    /// 状态：0-待执行 1-执行中 2-已完成 3-已审核
    /// </summary>
    public int? Status { get; set; }

    /// <summary>
    /// 执行人ID
    /// </summary>
    public int? ExecutorId { get; set; }
}

/// <summary>
/// 维护任务创建请求
/// </summary>
public class MaintenanceTaskRequest
{
    public int? PlanId { get; set; }

    [Required(ErrorMessage = "任务名称不能为空")]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    public DateTime? PlanTime { get; set; }

    public int? ExecutorId { get; set; }

    [MaxLength(500)]
    public string? Remark { get; set; }
}

/// <summary>
/// 维护任务响应DTO
/// </summary>
public class MaintenanceTaskDto
{
    public int Id { get; set; }
    public int PlanId { get; set; }
    public string PlanName { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public DateTime? PlanTime { get; set; }
    public DateTime? ActualTime { get; set; }
    public int? ExecutorId { get; set; }
    public int Status { get; set; }
    public string StatusName { get; set; } = string.Empty;
    public string? Remark { get; set; }
    public string? ExecuteRemark { get; set; }
    public string? ReviewRemark { get; set; }
    public string? EquipmentName { get; set; }
    public string? ExecutorName { get; set; }
    public string? Content { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// 派单请求
/// </summary>
public class DispatchRequest
{
    [Required(ErrorMessage = "执行人不能为空")]
    public int ExecutorId { get; set; }

    public DateTime? PlanTime { get; set; }
}

/// <summary>
/// 执行请求
/// </summary>
public class ExecuteRequest
{
    [MaxLength(500)]
    public string? ExecuteRemark { get; set; }
}

/// <summary>
/// 审核请求
/// </summary>
public class ReviewRequest
{
    /// <summary>
    /// 审核结果：1-通过 2-驳回
    /// </summary>
    [Required(ErrorMessage = "请选择审核结果")]
    public int Result { get; set; }

    [MaxLength(500)]
    public string? ReviewRemark { get; set; }
}

/// <summary>
/// 维护任务统计响应
/// </summary>
public class MaintenanceTaskStatsDto
{
    /// <summary>
    /// 待执行
    /// </summary>
    public int PendingCount { get; set; }

    /// <summary>
    /// 执行中
    /// </summary>
    public int ExecutingCount { get; set; }

    /// <summary>
    /// 待审核
    /// </summary>
    public int ReviewingCount { get; set; }

    /// <summary>
    /// 已完成
    /// </summary>
    public int CompletedCount { get; set; }

    /// <summary>
    /// 本月完成
    /// </summary>
    public int MonthCompleted { get; set; }

    /// <summary>
    /// 总任务数
    /// </summary>
    public int TotalCount { get; set; }
}
