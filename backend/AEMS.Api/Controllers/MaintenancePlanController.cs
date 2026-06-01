using AEMS.Core.DTOs;
using AEMS.Core.Entities;
using AEMS.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AEMS.Api.Controllers;

/// <summary>
/// 维护计划控制器
/// </summary>
[ApiController]
[Route("api/maintenance/plans")]
[Authorize]
public class MaintenancePlanController : ControllerBase
{
    private readonly AemsDbContext _context;

    public MaintenancePlanController(AemsDbContext context)
    {
        _context = context;
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
            EquipmentName = plan.Equipment?.Name,
            SystemName = plan.Equipment?.Subsystem?.Name,
            CreatedAt = plan.CreatedAt,
            UpdatedAt = plan.UpdatedAt
        };
    }

    /// <summary>
    /// 获取维护计划列表（分页+筛选）
    /// </summary>
    [HttpGet]
    public async Task<ApiResponse<PagedResult<MaintenancePlanDto>>> GetList([FromQuery] MaintenancePlanQueryRequest query)
    {
        var queryable = _context.MaintenancePlans
            .Include(p => p.Equipment)
            .ThenInclude(e => e!.Subsystem)
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
            .OrderByDescending(p => p.CreatedAt)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync();

        var dtos = items.Select(MapToDto).ToList();

        // 补充任务数量
        var planIds = dtos.Select(i => i.Id).ToList();
        var taskCounts = await _context.MaintenanceTasks
            .Where(t => planIds.Contains(t.PlanId) && !t.IsDeleted)
            .GroupBy(t => t.PlanId)
            .Select(g => new { PlanId = g.Key, Count = g.Count() })
            .ToListAsync();

        foreach (var item in dtos)
        {
            item.TaskCount = taskCounts.FirstOrDefault(tc => tc.PlanId == item.Id)?.Count ?? 0;
        }

        return ApiResponse<PagedResult<MaintenancePlanDto>>.Success(new PagedResult<MaintenancePlanDto>
        {
            Items = dtos,
            Total = total,
            Page = query.Page,
            PageSize = query.PageSize
        });
    }

    /// <summary>
    /// 获取维护计划详情
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ApiResponse<MaintenancePlanDto>> GetById(int id)
    {
        var plan = await _context.MaintenancePlans
            .Include(p => p.Equipment)
            .ThenInclude(e => e!.Subsystem)
            .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);

        if (plan == null)
            return ApiResponse<MaintenancePlanDto>.Fail(404, "维护计划不存在");

        var dto = MapToDto(plan);
        dto.TaskCount = await _context.MaintenanceTasks
            .CountAsync(t => t.PlanId == id && !t.IsDeleted);
        return ApiResponse<MaintenancePlanDto>.Success(dto);
    }

    /// <summary>
    /// 新增维护计划
    /// </summary>
    [HttpPost]
    public async Task<ApiResponse<MaintenancePlanDto>> Create([FromBody] MaintenancePlanRequest request)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
            return ApiResponse<MaintenancePlanDto>.Fail(400, string.Join("; ", errors));
        }

        var exists = await _context.MaintenancePlans
            .AnyAsync(p => p.PlanNo == request.PlanNo && !p.IsDeleted);
        if (exists)
            return ApiResponse<MaintenancePlanDto>.Fail(400, "计划编号已存在");

        try
        {
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
                Remark = request.Remark,
                EquipmentId = request.EquipmentId
            };

            _context.MaintenancePlans.Add(plan);
            await _context.SaveChangesAsync();

            // 重新加载导航属性
            var created = await _context.MaintenancePlans
                .Include(p => p.Equipment)
                .ThenInclude(e => e!.Subsystem)
                .FirstAsync(p => p.Id == plan.Id);

            return ApiResponse<MaintenancePlanDto>.Success(MapToDto(created), "创建成功");
        }
        catch (ArgumentException ex)
        {
            return ApiResponse<MaintenancePlanDto>.Fail(400, ex.Message);
        }
    }

    /// <summary>
    /// 编辑维护计划
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ApiResponse<MaintenancePlanDto>> Update(int id, [FromBody] MaintenancePlanRequest request)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
            return ApiResponse<MaintenancePlanDto>.Fail(400, string.Join("; ", errors));
        }

        var plan = await _context.MaintenancePlans
            .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);

        if (plan == null)
            return ApiResponse<MaintenancePlanDto>.Fail(404, "维护计划不存在");

        var nameExists = await _context.MaintenancePlans
            .AnyAsync(p => p.PlanNo == request.PlanNo && p.Id != id && !p.IsDeleted);
        if (nameExists)
            return ApiResponse<MaintenancePlanDto>.Fail(400, "计划编号已存在");

        plan.Name = request.Name;
        plan.PlanNo = request.PlanNo;
        plan.PlanType = request.PlanType;
        plan.CycleDays = request.CycleDays;
        plan.StartDate = request.StartDate;
        plan.EndDate = request.EndDate;
        plan.OwnerId = request.OwnerId;
        plan.Status = request.Status;
        plan.Remark = request.Remark;
        plan.EquipmentId = request.EquipmentId;

        await _context.SaveChangesAsync();

        // 重新加载导航属性
        var updated = await _context.MaintenancePlans
            .Include(p => p.Equipment)
            .ThenInclude(e => e!.Subsystem)
            .FirstAsync(p => p.Id == plan.Id);

        return ApiResponse<MaintenancePlanDto>.Success(MapToDto(updated), "更新成功");
    }

    /// <summary>
    /// 删除维护计划
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ApiResponse> Delete(int id)
    {
        var plan = await _context.MaintenancePlans.FindAsync(id);
        if (plan == null || plan.IsDeleted)
            return ApiResponse.Fail(404, "维护计划不存在");

        plan.IsDeleted = true;
        await _context.SaveChangesAsync();
        return ApiResponse.Success("删除成功");
    }

    /// <summary>
    /// 启用/停用维护计划
    /// </summary>
    [HttpPut("{id}/toggle")]
    public async Task<ApiResponse> Toggle(int id)
    {
        var plan = await _context.MaintenancePlans.FindAsync(id);
        if (plan == null || plan.IsDeleted)
            return ApiResponse.Fail(404, "维护计划不存在");

        // 0-未开始 <-> 3-已取消 之间的切换（启用/停用）
        plan.Status = plan.Status == 3 ? 0 : 3;
        await _context.SaveChangesAsync();
        return ApiResponse.Success("操作成功");
    }
}
