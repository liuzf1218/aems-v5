namespace AEMS.Core.DTOs;

/// <summary>
/// 工单查询请求
/// </summary>
public class WorkOrderQueryRequest : PagedRequest
{
    /// <summary>
    /// 工单编号（模糊）
    /// </summary>
    public string? WorkorderNo { get; set; }

    /// <summary>
    /// 标题（模糊）
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// 故障类型ID
    /// </summary>
    public int? FaultTypeId { get; set; }

    /// <summary>
    /// 优先级：1-低 2-中 3-高 4-紧急
    /// </summary>
    public int? Priority { get; set; }

    /// <summary>
    /// 状态：0-待受理 1-处理中 2-待验收 3-已完成 4-已归档
    /// </summary>
    public int? Status { get; set; }

    /// <summary>
    /// 所属系统ID
    /// </summary>
    public int? SubsystemId { get; set; }
}

/// <summary>
/// 创建工单请求
/// </summary>
public class CreateWorkOrderRequest
{
    /// <summary>
    /// 工单标题
    /// </summary>
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
}

/// <summary>
/// 转派工单请求
/// </summary>
public class AssignWorkOrderRequest
{
    /// <summary>
    /// 处理人ID
    /// </summary>
    public int HandlerId { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    public string? Remark { get; set; }
}

/// <summary>
/// 完成工单请求
/// </summary>
public class CompleteWorkOrderRequest
{
    /// <summary>
    /// 完成备注/处理结果
    /// </summary>
    public string? Result { get; set; }
}

/// <summary>
/// 工单列表响应（含关联信息）
/// </summary>
public class WorkOrderListItemDto
{
    public int Id { get; set; }
    public string WorkorderNo { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? FaultTypeName { get; set; }
    public string? EquipmentName { get; set; }
    public int Priority { get; set; }
    public int Status { get; set; }
    public string? CreatorName { get; set; }
    public string? HandlerName { get; set; }
    public string? SystemName { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? PlanFinishTime { get; set; }
    public DateTime? ActualFinishTime { get; set; }
}

/// <summary>
/// 工单详情响应
/// </summary>
public class WorkOrderDetailDto
{
    public int Id { get; set; }
    public string WorkorderNo { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? FaultTypeId { get; set; }
    public string? FaultTypeName { get; set; }
    public int? EquipmentId { get; set; }
    public string? EquipmentName { get; set; }
    public string? EquipmentCode { get; set; }
    public int Priority { get; set; }
    public int Status { get; set; }
    public int? CreatorId { get; set; }
    public string? CreatorName { get; set; }
    public int? HandlerId { get; set; }
    public string? HandlerName { get; set; }
    public DateTime? PlanFinishTime { get; set; }
    public DateTime? ActualFinishTime { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string? SystemName { get; set; }
    public string? Symptom { get; set; }
    public string? Solution { get; set; }
    // SLA info
    public int? ResponseDeadlineMinutes { get; set; }
    public int? FixDeadlineMinutes { get; set; }
    public DateTime? ResponseDeadlineTime { get; set; }
    public DateTime? FixDeadlineTime { get; set; }
    public bool IsResponseOverdue { get; set; }
    public bool IsFixOverdue { get; set; }
    public int? ResponseRemainingMinutes { get; set; }
    public int? FixRemainingMinutes { get; set; }
}

/// <summary>
/// 工单操作日志响应
/// </summary>
public class WorkOrderLogDto
{
    public int Id { get; set; }
    public string? ActionType { get; set; }
    public string? Content { get; set; }
    public string? OperatorName { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// SLA信息响应
/// </summary>
public class SlaInfoDto
{
    public int Priority { get; set; }
    public string PriorityText { get; set; } = string.Empty;
    public int ResponseDeadlineMinutes { get; set; }
    public int FixDeadlineMinutes { get; set; }
    public DateTime? ResponseDeadlineTime { get; set; }
    public DateTime? FixDeadlineTime { get; set; }
    public bool IsResponseOverdue { get; set; }
    public bool IsFixOverdue { get; set; }
    public int? ResponseRemainingMinutes { get; set; }
    public int? FixRemainingMinutes { get; set; }
    public string ResponseRemainingText { get; set; } = string.Empty;
    public string FixRemainingText { get; set; } = string.Empty;
}

/// <summary>
/// 简单选项响应（用于下拉选择）
/// </summary>
public class OptionDto
{
    public int Id { get; set; }
    public string Label { get; set; } = string.Empty;
    public string? Code { get; set; }
}
