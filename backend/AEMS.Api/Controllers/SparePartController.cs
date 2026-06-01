using AEMS.Core.DTOs;
using AEMS.Core.Entities;
using AEMS.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AEMS.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SparePartController : ControllerBase
{
    private readonly AemsDbContext _context;

    public SparePartController(AemsDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// 获取备件列表（分页+筛选）
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetList([FromQuery] SparePartQueryRequest query)
    {
        var queryable = _context.Spareparts
            .Include(x => x.Subsystem)
            .Where(x => !x.IsDeleted).AsQueryable();

        if (!string.IsNullOrEmpty(query.Keyword))
        {
            queryable = queryable.Where(x =>
                x.Name.Contains(query.Keyword) ||
                x.Code.Contains(query.Keyword));
        }

        if (!string.IsNullOrEmpty(query.Location))
        {
            queryable = queryable.Where(x => x.Location == query.Location);
        }

        if (query.SubsystemId.HasValue)
        {
            queryable = queryable.Where(x => x.SubsystemId == query.SubsystemId.Value);
        }

        var total = await queryable.CountAsync();
        var items = await queryable
            .OrderByDescending(x => x.CreatedAt)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(x => new
            {
                x.Id,
                x.Name,
                x.Code,
                x.Specification,
                x.Unit,
                x.StockQuantity,
                x.MinStock,
                x.Price,
                x.Location,
                x.SubsystemId,
                SystemName = x.Subsystem != null ? x.Subsystem.Name : "-",
                x.Remark,
                x.CreatedAt,
                x.UpdatedAt
            })
            .ToListAsync();

        return Ok(ApiResponse<object>.Success(new { items, total, page = query.Page, pageSize = query.PageSize }));
    }

    /// <summary>
    /// 获取备件详情
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var sparepart = await _context.Spareparts
            .Include(x => x.Subsystem)
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
        if (sparepart == null) return NotFound(ApiResponse.Fail(404, "备件不存在"));
        return Ok(ApiResponse<object>.Success(sparepart));
    }

    /// <summary>
    /// 新增备件
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Sparepart sparepart)
    {
        // 检查编号唯一
        var exists = await _context.Spareparts.AnyAsync(x => x.Code == sparepart.Code && !x.IsDeleted);
        if (exists) return Ok(ApiResponse.Fail(400, "备件编号已存在"));

        sparepart.CreatedAt = DateTime.Now;
        sparepart.UpdatedAt = DateTime.Now;
        _context.Spareparts.Add(sparepart);
        await _context.SaveChangesAsync();
        return Ok(ApiResponse<object>.Success(sparepart, "创建成功"));
    }

    /// <summary>
    /// 编辑备件
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] Sparepart sparepart)
    {
        var existing = await _context.Spareparts.FindAsync(id);
        if (existing == null || existing.IsDeleted) return NotFound(ApiResponse.Fail(404, "备件不存在"));

        existing.Name = sparepart.Name;
        existing.Code = sparepart.Code;
        existing.Specification = sparepart.Specification;
        existing.Unit = sparepart.Unit;
        existing.MinStock = sparepart.MinStock;
        existing.Price = sparepart.Price;
        existing.Location = sparepart.Location;
        existing.SubsystemId = sparepart.SubsystemId;
        existing.Remark = sparepart.Remark;
        existing.UpdatedAt = DateTime.Now;

        await _context.SaveChangesAsync();
        return Ok(ApiResponse<object>.Success(existing, "更新成功"));
    }

    /// <summary>
    /// 删除备件（软删除）
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var sparepart = await _context.Spareparts.FindAsync(id);
        if (sparepart == null || sparepart.IsDeleted) return NotFound(ApiResponse.Fail(404, "备件不存在"));

        sparepart.IsDeleted = true;
        sparepart.UpdatedAt = DateTime.Now;
        await _context.SaveChangesAsync();
        return Ok(ApiResponse.Success("删除成功"));
    }

    /// <summary>
    /// 获取存放位置列表（用于筛选）
    /// </summary>
    [HttpGet("locations")]
    public async Task<IActionResult> GetLocations()
    {
        var locations = await _context.Spareparts
            .Where(x => !x.IsDeleted && x.Location != null && x.Location != "")
            .Select(x => x.Location!)
            .Distinct()
            .OrderBy(x => x)
            .ToListAsync();
        return Ok(ApiResponse<object>.Success(locations));
    }
}

public class SparePartQueryRequest : PagedRequest
{
    public string? Keyword { get; set; }
    public string? Location { get; set; }
    public int? SubsystemId { get; set; }
}
