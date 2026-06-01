using AEMS.Core.DTOs;
using AEMS.Core.Entities;
using AEMS.Core.Interfaces;
using AEMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AEMS.Infrastructure.Repositories;

/// <summary>
/// 维护计划服务实现
/// </summary>
public class MaintenancePlanService : IMaintenancePlanService
{
    private readonly AemsDbContext _context;

    public MaintenancePlanService(AemsDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<MaintenancePlanDto>> GetPlanListAsync(MaintenancePlanQueryRequest query)
    {
        var queryable = _context.MaintenancePlans
            .Where(p => !p.IsDeleted)
            .AsQueryable();

        if (!string.IsNullOrEmpty(query.Keyword))
        {
            var keyword = query.Keyword.Trim();
            queryable = queryable.Where(p =>
                p.Name.Contains(keyword) ||
                p.PlanNo.Contains(keyword));
        }

        if (query.PlanType.HasValue)
        {
            queryable = queryable.Where(p => p.PlanType == query.PlanType.Value);
        }

        if (query.Status.HasValue)
        {
            queryable = queryable.Where(p => p.Status == query.Status.Value);
        }

        var total = await queryable.CountAsync();
        var items = await queryable
            .Include(p => p.Equipment)
                .ThenInclude(e => e.Subsystem)
            .OrderByDescending(p => p.CreatedAt)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync();

        var dtoList = items.Select(p =>
        {
            var dto = MapToDto(p);
            dto.EquipmentName = p.Equipment?.Name;
            dto.SystemName = p.Equipment?.Subsystem?.Name;
            return dto;
        }).ToList();

        // 补充任务数量
        var planIds = dtoList.Select(i => i.Id).ToList();
        var taskCounts = await _context.MaintenanceTasks
            .Where(t => planIds.Contains(t.PlanId) && !t.IsDeleted)
            .GroupBy(t => t.PlanId)
            .Select(g => new { PlanId = g.Key, Count = g.Count() })
            .ToListAsync();

        foreach (var item in dtoList)
        {
            item.TaskCount = taskCounts.FirstOrDefault(tc => tc.PlanId == item.Id)?.Count ?? 0;
        }

        return new PagedResult<MaintenancePlanDto>
        {
            Items = dtoList,
            Total = total,
            Page = query.Page,
            PageSize = query.PageSize
        };
    }

    public async Task<MaintenancePlanDto?> GetPlanByIdAsync(int id)
    {
        var plan = await _context.MaintenancePlans
            .Include(p => p.Equipment)
                .ThenInclude(e => e.Subsystem)
            .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);

        if (plan == null) return null;

        var dto = MapToDto(plan);
        dto.EquipmentName = plan.Equipment?.Name;
        dto.SystemName = plan.Equipment?.Subsystem?.Name;
        dto.TaskCount = await _context.MaintenanceTasks
            .CountAsync(t => t.PlanId == id && !t.IsDeleted);
        return dto;
    }

    public async Task<MaintenancePlanDto> CreatePlanAsync(MaintenancePlanRequest request)
    {
        var exists = await _context.MaintenancePlans
            .AnyAsync(p => p.PlanNo == request.PlanNo && !p.IsDeleted);
        if (exists)
            throw new ArgumentException("计划编号已存在");

        var plan = new MaintenancePlan
        {
            Name = request.Name,
            PlanNo = request.PlanNo,
            PlanType = request.PlanType,
            CycleDays = request.CycleDays,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            OwnerId = request.OwnerId,
            Status = request.Status,
            Remark = request.Remark
        };

        _context.MaintenancePlans.Add(plan);
        await _context.SaveChangesAsync();

        return MapToDto(plan);
    }

    public async Task<MaintenancePlanDto?> UpdatePlanAsync(int id, MaintenancePlanRequest request)
    {
        var plan = await _context.MaintenancePlans
            .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);

        if (plan == null) return null;

        var nameExists = await _context.MaintenancePlans
            .AnyAsync(p => p.PlanNo == request.PlanNo && p.Id != id && !p.IsDeleted);
        if (nameExists)
            throw new ArgumentException("计划编号已存在");

        plan.Name = request.Name;
        plan.PlanNo = request.PlanNo;
        plan.PlanType = request.PlanType;
        plan.CycleDays = request.CycleDays;
        plan.StartDate = request.StartDate;
        plan.EndDate = request.EndDate;
        plan.OwnerId = request.OwnerId;
        plan.Status = request.Status;
        plan.Remark = request.Remark;

        await _context.SaveChangesAsync();

        return MapToDto(plan);
    }

    public async Task<bool> DeletePlanAsync(int id)
    {
        var plan = await _context.MaintenancePlans.FindAsync(id);
        if (plan == null || plan.IsDeleted) return false;

        plan.IsDeleted = true;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> TogglePlanAsync(int id)
    {
        var plan = await _context.MaintenancePlans.FindAsync(id);
        if (plan == null || plan.IsDeleted) return false;

        // 0-未开始 <-> 3-已取消 之间的切换（启用/停用）
        plan.Status = plan.Status == 3 ? 0 : 3;
        await _context.SaveChangesAsync();
        return true;
    }

    private static MaintenancePlanDto MapToDto(MaintenancePlan plan)
    {
        return new MaintenancePlanDto
        {
            Id = plan.Id,
            Name = plan.Name,
            PlanNo = plan.PlanNo,
            PlanType = plan.PlanType,
            PlanTypeName = plan.PlanType switch
            {
                1 => "日常维护",
                2 => "定期维护",
                3 => "专项维护",
                _ => "未知"
            },
            CycleDays = plan.CycleDays,
            StartDate = plan.StartDate,
            EndDate = plan.EndDate,
            OwnerId = plan.OwnerId,
            Status = plan.Status,
            StatusName = plan.Status switch
            {
                0 => "未开始",
                1 => "进行中",
                2 => "已完成",
                3 => "已取消",
                _ => "未知"
            },
            Remark = plan.Remark,
            CreatedAt = plan.CreatedAt,
            UpdatedAt = plan.UpdatedAt
        };
    }
}

/// <summary>
/// 维护任务服务实现
/// </summary>
public class MaintenanceTaskService : IMaintenanceTaskService
{
    private readonly AemsDbContext _context;

    public MaintenanceTaskService(AemsDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<MaintenanceTaskDto>> GetTaskListAsync(MaintenanceTaskQueryRequest query)
    {
        var queryable = _context.MaintenanceTasks
            .Include(t => t.Plan)
                .ThenInclude(p => p.Equipment)
            .Where(t => !t.IsDeleted)
            .AsQueryable();

        if (!string.IsNullOrEmpty(query.Keyword))
        {
            var keyword = query.Keyword.Trim();
            queryable = queryable.Where(t =>
                t.Name.Contains(keyword));
        }

        if (query.PlanId.HasValue)
        {
            queryable = queryable.Where(t => t.PlanId == query.PlanId.Value);
        }

        if (query.Status.HasValue)
        {
            queryable = queryable.Where(t => t.Status == query.Status.Value);
        }

        if (query.ExecutorId.HasValue)
        {
            queryable = queryable.Where(t => t.ExecutorId == query.ExecutorId.Value);
        }

        var total = await queryable.CountAsync();
        var items = await queryable
            .OrderByDescending(t => t.CreatedAt)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync();

        // 批量补充执行人名称
        var executorIds = items.Select(t => t.ExecutorId).Where(id => id.HasValue).Select(id => id!.Value).Distinct().ToList();
        var executors = await _context.SysUsers
            .Where(u => executorIds.Contains(u.Id))
            .ToDictionaryAsync(u => u.Id, u => u.RealName ?? u.Username);

        var dtoList = items.Select(t =>
        {
            var dto = MapToDto(t);
            if (t.ExecutorId.HasValue && executors.TryGetValue(t.ExecutorId.Value, out var name))
                dto.ExecutorName = name;
            return dto;
        }).ToList();

        return new PagedResult<MaintenanceTaskDto>
        {
            Items = dtoList,
            Total = total,
            Page = query.Page,
            PageSize = query.PageSize
        };
    }

    public async Task<MaintenanceTaskDto?> GetTaskByIdAsync(int id)
    {
        var task = await _context.MaintenanceTasks
            .Include(t => t.Plan)
                .ThenInclude(p => p.Equipment)
            .FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted);

        if (task == null) return null;
        var dto = MapToDto(task);
        if (task.ExecutorId.HasValue)
        {
            var user = await _context.SysUsers
                .Where(u => u.Id == task.ExecutorId.Value)
                .Select(u => u.RealName ?? u.Username)
                .FirstOrDefaultAsync();
            dto.ExecutorName = user ?? "";
        }
        return dto;
    }

    public async Task<MaintenanceTaskDto> CreateTaskAsync(MaintenanceTaskRequest request)
    {
        var task = new MaintenanceTask
        {
            PlanId = request.PlanId ?? 0,
            Name = request.Name,
            PlanTime = request.PlanTime,
            ExecutorId = request.ExecutorId,
            Status = 0, // 待执行
            Remark = request.Remark
        };

        _context.MaintenanceTasks.Add(task);
        await _context.SaveChangesAsync();

        // 重新查询获取关联数据
        var created = await _context.MaintenanceTasks
            .Include(t => t.Plan)
                .ThenInclude(p => p.Equipment)
            .FirstAsync(t => t.Id == task.Id);

        var dto = MapToDto(created);
        if (created.ExecutorId.HasValue)
        {
            var user = await _context.SysUsers
                .Where(u => u.Id == created.ExecutorId.Value)
                .Select(u => u.RealName ?? u.Username)
                .FirstOrDefaultAsync();
            dto.ExecutorName = user ?? "";
        }
        return dto;
    }

    public async Task<bool> DispatchTaskAsync(int id, DispatchRequest request)
    {
        var task = await _context.MaintenanceTasks.FindAsync(id);
        if (task == null || task.IsDeleted) return false;

        // 只有待执行状态的任务可以派单
        if (task.Status != 0)
            throw new InvalidOperationException("只有待执行状态的任务可以派单");

        task.ExecutorId = request.ExecutorId;
        if (request.PlanTime.HasValue)
            task.PlanTime = request.PlanTime.Value;
        task.Status = 0; // 保持待执行
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExecuteTaskAsync(int id, ExecuteRequest request)
    {
        var task = await _context.MaintenanceTasks.FindAsync(id);
        if (task == null || task.IsDeleted) return false;

        // 只有待执行状态的任务可以执行
        if (task.Status != 0)
            throw new InvalidOperationException("只有待执行状态的任务可以执行");

        task.Status = 1; // 执行中
        task.ActualTime = DateTime.Now;
        if (!string.IsNullOrEmpty(request.ExecuteRemark))
            task.Remark = request.ExecuteRemark;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ReviewTaskAsync(int id, ReviewRequest request)
    {
        var task = await _context.MaintenanceTasks.FindAsync(id);
        if (task == null || task.IsDeleted) return false;

        // 只有执行中状态的任务可以审核
        if (task.Status != 1)
            throw new InvalidOperationException("只有执行中状态的任务可以审核");

        task.Status = request.Result == 1 ? 2 : 0; // 1-通过=>已完成，2-驳回=>待执行
        if (!string.IsNullOrEmpty(request.ReviewRemark))
            task.Remark = request.ReviewRemark;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<MaintenanceTaskStatsDto> GetTaskStatsAsync()
    {
        var queryable = _context.MaintenanceTasks.Where(t => !t.IsDeleted);

        var now = DateTime.Now;
        var monthStart = new DateTime(now.Year, now.Month, 1);

        return new MaintenanceTaskStatsDto
        {
            PendingCount = await queryable.CountAsync(t => t.Status == 0),
            ExecutingCount = await queryable.CountAsync(t => t.Status == 1),
            ReviewingCount = await queryable.CountAsync(t => t.Status == 2 && t.ActualTime != null), // 已执行待审核
            CompletedCount = await queryable.CountAsync(t => t.Status == 2),
            MonthCompleted = await queryable.CountAsync(t => t.Status == 2 && t.UpdatedAt >= monthStart),
            TotalCount = await queryable.CountAsync()
        };
    }

    private static MaintenanceTaskDto MapToDto(MaintenanceTask task)
    {
        return new MaintenanceTaskDto
        {
            Id = task.Id,
            PlanId = task.PlanId,
            PlanName = task.Plan?.Name ?? "",
            Name = task.Name,
            PlanTime = task.PlanTime,
            ActualTime = task.ActualTime,
            ExecutorId = task.ExecutorId,
            ExecutorName = "",
            Status = task.Status,
            StatusName = task.Status switch
            {
                0 => "待执行",
                1 => "执行中",
                2 => "已完成",
                _ => "未知"
            },
            EquipmentName = task.Plan?.Equipment?.Name ?? "",
            Content = task.Content,
            Remark = task.Remark,
            CreatedAt = task.CreatedAt,
            UpdatedAt = task.UpdatedAt
        };
    }
}
