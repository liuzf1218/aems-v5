using AEMS.Core.DTOs;
using AEMS.Core.Entities;
using AEMS.Core.Interfaces;
using AEMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AEMS.Infrastructure.Repositories;

public class WorkOrderService : IWorkOrderService
{
    private readonly AemsDbContext _context;

    // SLA规则：优先级 → (响应时限分钟, 修复时限分钟)
    private static readonly Dictionary<int, (int ResponseMinutes, int FixMinutes)> SlaRules = new()
    {
        { 1, (480, 2880) },   // 低：8h响应，48h修复
        { 2, (240, 1440) },   // 中：4h响应，24h修复
        { 3, (60, 480) },     // 高：1h响应，8h修复
        { 4, (15, 120) },     // 紧急：15min响应，2h修复
    };

    private static readonly Dictionary<int, string> PriorityTexts = new()
    {
        { 1, "低" }, { 2, "中" }, { 3, "高" }, { 4, "紧急" }
    };

    private static readonly Dictionary<int, string> StatusTexts = new()
    {
        { 0, "待受理" }, { 1, "处理中" }, { 2, "待验收" }, { 3, "已完成" }, { 4, "已归档" }
    };

    public WorkOrderService(AemsDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<WorkOrderListItemDto>> GetListAsync(WorkOrderQueryRequest query)
    {
        var q = _context.Workorders
            .Include(w => w.FaultType)
            .Include(w => w.Equipment).ThenInclude(e => e.Subsystem)
            .Where(w => !w.IsDeleted);

        if (!string.IsNullOrEmpty(query.WorkorderNo))
            q = q.Where(w => w.WorkorderNo.Contains(query.WorkorderNo));
        if (!string.IsNullOrEmpty(query.Title))
            q = q.Where(w => w.Title.Contains(query.Title));
        if (query.FaultTypeId.HasValue)
            q = q.Where(w => w.FaultTypeId == query.FaultTypeId.Value);
        if (query.Priority.HasValue)
            q = q.Where(w => w.Priority == query.Priority.Value);
        if (query.Status.HasValue)
            q = q.Where(w => w.Status == query.Status.Value);
        if (query.SubsystemId.HasValue)
            q = q.Where(w => w.Equipment != null && w.Equipment.SubsystemId == query.SubsystemId.Value);

        var total = await q.CountAsync();
        var items = await q
            .OrderByDescending(w => w.CreatedAt)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync();

        // 获取关联用户
        var userIds = items.Select(w => w.CreatorId)
            .Concat(items.Select(w => w.HandlerId))
            .Where(id => id.HasValue)
            .Select(id => id!.Value)
            .Distinct()
            .ToList();

        var users = await _context.SysUsers
            .Where(u => userIds.Contains(u.Id))
            .ToDictionaryAsync(u => u.Id, u => u.RealName ?? u.Username);

        var result = items.Select(w => new WorkOrderListItemDto
        {
            Id = w.Id,
            WorkorderNo = w.WorkorderNo,
            Title = w.Title,
            FaultTypeName = w.FaultType?.Name,
            EquipmentName = w.Equipment?.Name,
            Priority = w.Priority,
            Status = w.Status,
            CreatorName = w.CreatorId.HasValue && users.ContainsKey(w.CreatorId.Value) ? users[w.CreatorId.Value] : null,
            HandlerName = w.HandlerId.HasValue && users.ContainsKey(w.HandlerId.Value) ? users[w.HandlerId.Value] : null,
            SystemName = w.Equipment?.Subsystem?.Name,
            CreatedAt = w.CreatedAt,
            PlanFinishTime = w.PlanFinishTime,
            ActualFinishTime = w.ActualFinishTime,
        }).ToList();

        return new PagedResult<WorkOrderListItemDto>
        {
            Items = result,
            Total = total,
            Page = query.Page,
            PageSize = query.PageSize
        };
    }

    public async Task<WorkOrderDetailDto?> GetByIdAsync(int id)
    {
        var w = await _context.Workorders
            .Include(w => w.FaultType)
            .Include(w => w.Equipment).ThenInclude(e => e.Subsystem)
            .FirstOrDefaultAsync(w => w.Id == id && !w.IsDeleted);

        if (w == null) return null;

        var creatorName = w.CreatorId.HasValue
            ? await _context.SysUsers.Where(u => u.Id == w.CreatorId.Value).Select(u => u.RealName ?? u.Username).FirstOrDefaultAsync()
            : null;
        var handlerName = w.HandlerId.HasValue
            ? await _context.SysUsers.Where(u => u.Id == w.HandlerId.Value).Select(u => u.RealName ?? u.Username).FirstOrDefaultAsync()
            : null;

        var now = DateTime.Now;
        var responseDeadline = w.ResponseDeadlineMinutes.HasValue ? w.CreatedAt.AddMinutes(w.ResponseDeadlineMinutes.Value) : (DateTime?)null;
        var fixDeadline = w.FixDeadlineMinutes.HasValue ? w.CreatedAt.AddMinutes(w.FixDeadlineMinutes.Value) : (DateTime?)null;

        return new WorkOrderDetailDto
        {
            Id = w.Id,
            WorkorderNo = w.WorkorderNo,
            Title = w.Title,
            Description = w.Description,
            FaultTypeId = w.FaultTypeId,
            FaultTypeName = w.FaultType?.Name,
            EquipmentId = w.EquipmentId,
            EquipmentName = w.Equipment?.Name,
            EquipmentCode = w.Equipment?.Code,
            Priority = w.Priority,
            Status = w.Status,
            CreatorId = w.CreatorId,
            CreatorName = creatorName,
            HandlerId = w.HandlerId,
            HandlerName = handlerName,
            PlanFinishTime = w.PlanFinishTime,
            ActualFinishTime = w.ActualFinishTime,
            SystemName = w.Equipment?.Subsystem?.Name,
            Symptom = w.Symptom,
            Solution = w.Solution,
            CreatedAt = w.CreatedAt,
            UpdatedAt = w.UpdatedAt,
            ResponseDeadlineMinutes = w.ResponseDeadlineMinutes,
            FixDeadlineMinutes = w.FixDeadlineMinutes,
            ResponseDeadlineTime = responseDeadline,
            FixDeadlineTime = fixDeadline,
            IsResponseOverdue = w.Status == 0 && responseDeadline.HasValue && now > responseDeadline.Value,
            IsFixOverdue = w.Status < 3 && fixDeadline.HasValue && now > fixDeadline.Value,
            ResponseRemainingMinutes = responseDeadline.HasValue ? (int)(responseDeadline.Value - now).TotalMinutes : null,
            FixRemainingMinutes = fixDeadline.HasValue ? (int)(fixDeadline.Value - now).TotalMinutes : null,
        };
    }

    public async Task<WorkOrderDetailDto> CreateAsync(CreateWorkOrderRequest request, int creatorId)
    {
        var now = DateTime.Now;
        var sla = SlaRules.GetValueOrDefault(request.Priority, (ResponseMinutes: 240, FixMinutes: 1440));
        var woNo = $"WO{now:yyyyMMdd}{await GetNextSequenceAsync(now):D4}";

        var workorder = new Workorder
        {
            WorkorderNo = woNo,
            Title = request.Title,
            Description = request.Description,
            FaultTypeId = request.FaultTypeId,
            EquipmentId = request.EquipmentId,
            Priority = request.Priority,
            Status = 0,
            CreatorId = creatorId,
            ResponseDeadlineMinutes = sla.ResponseMinutes,
            FixDeadlineMinutes = sla.FixMinutes,
            PlanFinishTime = now.AddMinutes(sla.FixMinutes),
            CreatedAt = now,
            UpdatedAt = now
        };

        _context.Workorders.Add(workorder);
        await _context.SaveChangesAsync();

        // 记录操作日志
        await AddLogAsync(workorder.Id, creatorId, "创建", $"创建工单「{request.Title}」，优先级：{PriorityTexts.GetValueOrDefault(request.Priority, "未知")}");
        await _context.SaveChangesAsync();

        return (await GetByIdAsync(workorder.Id))!;
    }

    public async Task<bool> AcceptAsync(int id, int operatorId)
    {
        var wo = await _context.Workorders.FindAsync(id);
        if (wo == null || wo.IsDeleted || wo.Status != 0) return false;

        wo.Status = 1;
        wo.HandlerId = operatorId;
        wo.ActualResponseTime = DateTime.Now;
        wo.UpdatedAt = DateTime.Now;
        await AddLogAsync(id, operatorId, "受理", "受理工单，开始处理");
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> AssignAsync(int id, AssignWorkOrderRequest request, int operatorId)
    {
        var wo = await _context.Workorders.FindAsync(id);
        if (wo == null || wo.IsDeleted || wo.Status > 2) return false;

        var oldHandler = wo.HandlerId;
        wo.HandlerId = request.HandlerId;
        wo.Status = 1; // 转派后自动进入处理中
        if (!wo.ActualResponseTime.HasValue)
            wo.ActualResponseTime = DateTime.Now;
        wo.UpdatedAt = DateTime.Now;

        var handlerName = await _context.SysUsers
            .Where(u => u.Id == request.HandlerId)
            .Select(u => u.RealName ?? u.Username)
            .FirstOrDefaultAsync() ?? "未知";

        var remark = string.IsNullOrEmpty(request.Remark) ? "" : $"，备注：{request.Remark}";
        await AddLogAsync(id, operatorId, "转派", $"转派给「{handlerName}」{remark}");
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> CompleteAsync(int id, CompleteWorkOrderRequest request, int operatorId)
    {
        var wo = await _context.Workorders.FindAsync(id);
        if (wo == null || wo.IsDeleted || wo.Status != 1) return false;

        wo.Status = 2; // 待验收
        wo.ActualFinishTime = DateTime.Now;
        wo.UpdatedAt = DateTime.Now;

        var result = string.IsNullOrEmpty(request.Result) ? "" : $"，处理结果：{request.Result}";
        await AddLogAsync(id, operatorId, "提交完成", $"提交工单完成{result}");
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ProcessAsync(int id, int operatorId)
    {
        var wo = await _context.Workorders.FindAsync(id);
        if (wo == null || wo.IsDeleted || wo.Status < 0 || wo.Status > 2) return false;

        var now = DateTime.Now;
        var oldStatus = wo.Status;
        wo.Status = oldStatus + 1;
        wo.UpdatedAt = now;

        string actionType;
        string content;

        switch (oldStatus)
        {
            case 0:
                actionType = "受理";
                content = "受理工单，开始处理";
                if (!wo.ActualResponseTime.HasValue)
                    wo.ActualResponseTime = now;
                wo.HandlerId = operatorId;
                break;
            case 1:
                actionType = "提交完成";
                content = "提交工单完成，等待验收";
                break;
            case 2:
                actionType = "验收通过";
                content = "验收通过，工单已完成";
                wo.ActualFinishTime = now;
                break;
            default:
                return false;
        }

        await AddLogAsync(id, operatorId, actionType, content);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> CancelAsync(int id, int operatorId)
    {
        var wo = await _context.Workorders.FindAsync(id);
        if (wo == null || wo.IsDeleted || wo.Status >= 3) return false;

        wo.Status = 4; // 已归档（取消直接归档）
        wo.UpdatedAt = DateTime.Now;
        await AddLogAsync(id, operatorId, "取消", "取消工单");
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<WorkOrderLogDto>> GetLogsAsync(int id)
    {
        var logs = await _context.WorkorderLogs
            .Where(l => l.WorkorderId == id)
            .OrderByDescending(l => l.CreatedAt)
            .ToListAsync();

        var operatorIds = logs.Where(l => l.OperatorId.HasValue)
            .Select(l => l.OperatorId!.Value)
            .Distinct()
            .ToList();

        var users = await _context.SysUsers
            .Where(u => operatorIds.Contains(u.Id))
            .ToDictionaryAsync(u => u.Id, u => u.RealName ?? u.Username);

        return logs.Select(l => new WorkOrderLogDto
        {
            Id = l.Id,
            ActionType = l.ActionType,
            Content = l.Content,
            OperatorName = l.OperatorId.HasValue && users.ContainsKey(l.OperatorId.Value) ? users[l.OperatorId.Value] : null,
            CreatedAt = l.CreatedAt
        }).ToList();
    }

    public async Task<SlaInfoDto?> GetSlaInfoAsync(int id)
    {
        var wo = await _context.Workorders.FindAsync(id);
        if (wo == null || wo.IsDeleted) return null;

        var now = DateTime.Now;
        var sla = SlaRules.GetValueOrDefault(wo.Priority, (ResponseMinutes: 240, FixMinutes: 1440));
        var responseDeadline = wo.CreatedAt.AddMinutes(sla.ResponseMinutes);
        var fixDeadline = wo.CreatedAt.AddMinutes(sla.FixMinutes);

        var responseRemaining = (int)(responseDeadline - now).TotalMinutes;
        var fixRemaining = (int)(fixDeadline - now).TotalMinutes;

        return new SlaInfoDto
        {
            Priority = wo.Priority,
            PriorityText = PriorityTexts.GetValueOrDefault(wo.Priority, "未知"),
            ResponseDeadlineMinutes = sla.ResponseMinutes,
            FixDeadlineMinutes = sla.FixMinutes,
            ResponseDeadlineTime = responseDeadline,
            FixDeadlineTime = fixDeadline,
            IsResponseOverdue = wo.Status == 0 && now > responseDeadline,
            IsFixOverdue = wo.Status < 3 && now > fixDeadline,
            ResponseRemainingMinutes = responseRemaining,
            FixRemainingMinutes = fixRemaining,
            ResponseRemainingText = FormatRemaining(responseRemaining),
            FixRemainingText = FormatRemaining(fixRemaining),
        };
    }

    public async Task<List<OptionDto>> GetSystemsAsync()
    {
        return await _context.FaultTypes
            .Where(f => !f.IsDeleted)
            .Select(f => new OptionDto { Id = f.Id, Label = f.Name, Code = f.Code })
            .ToListAsync();
    }

    public async Task<List<OptionDto>> GetDevicesAsync(string? keyword = null)
    {
        var q = _context.Equipments.Where(e => !e.IsDeleted).AsQueryable();
        if (!string.IsNullOrEmpty(keyword))
            q = q.Where(e => e.Name.Contains(keyword) || e.Code.Contains(keyword));

        return await q
            .Select(e => new OptionDto { Id = e.Id, Label = $"{e.Code} - {e.Name}", Code = e.Code })
            .Take(100)
            .ToListAsync();
    }

    #region Private Methods

    private async Task AddLogAsync(int workorderId, int operatorId, string actionType, string content)
    {
        _context.WorkorderLogs.Add(new WorkorderLog
        {
            WorkorderId = workorderId,
            OperatorId = operatorId,
            ActionType = actionType,
            Content = content,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        });
    }

    private async Task<int> GetNextSequenceAsync(DateTime date)
    {
        var prefix = $"WO{date:yyyyMMdd}";
        var count = await _context.Workorders.CountAsync(w => w.WorkorderNo.StartsWith(prefix));
        return count + 1;
    }

    private static string FormatRemaining(int minutes)
    {
        if (minutes < 0)
        {
            var abs = Math.Abs(minutes);
            if (abs >= 1440) return $"超时 {abs / 1440}天{abs % 1440 / 60}小时";
            if (abs >= 60) return $"超时 {abs / 60}小时{abs % 60}分钟";
            return $"超时 {abs}分钟";
        }
        if (minutes >= 1440) return $"剩余 {minutes / 1440}天{minutes % 1440 / 60}小时";
        if (minutes >= 60) return $"剩余 {minutes / 60}小时{minutes % 60}分钟";
        return $"剩余 {minutes}分钟";
    }

    #endregion
}
