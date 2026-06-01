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
public class StockController : ControllerBase
{
    private readonly AemsDbContext _context;

    public StockController(AemsDbContext context)
    {
        _context = context;
    }

    // ==================== 入库 ====================

    /// <summary>
    /// 获取入库记录（分页+筛选）
    /// </summary>
    [HttpGet("in")]
    public async Task<IActionResult> GetStockInList([FromQuery] StockQueryRequest query)
    {
        var queryable = _context.StockInRecords
            .Include(x => x.Sparepart)
                .ThenInclude(s => s!.Subsystem)
            .AsQueryable();

        if (query.SparepartId.HasValue)
        {
            queryable = queryable.Where(x => x.SparepartId == query.SparepartId.Value);
        }

        if (query.StartDate.HasValue)
        {
            queryable = queryable.Where(x => x.InDate >= query.StartDate.Value);
        }

        if (query.EndDate.HasValue)
        {
            queryable = queryable.Where(x => x.InDate <= query.EndDate.Value);
        }

        var total = await queryable.CountAsync();
        var items = await queryable
            .OrderByDescending(x => x.InDate)
            .ThenByDescending(x => x.CreatedAt)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(x => new
            {
                x.Id,
                x.SparepartId,
                SparepartName = x.Sparepart != null ? x.Sparepart.Name : "-",
                SparepartCode = x.Sparepart != null ? x.Sparepart.Code : "-",
                SystemName = x.Sparepart != null && x.Sparepart.Subsystem != null ? x.Sparepart.Subsystem.Name : "-",
                x.Quantity,
                x.UnitPrice,
                x.Supplier,
                x.InDate,
                x.OperatorId,
                x.Remark,
                x.CreatedAt
            })
            .ToListAsync();

        return Ok(ApiResponse<object>.Success(new { items, total, page = query.Page, pageSize = query.PageSize }));
    }

    /// <summary>
    /// 新增入库记录
    /// </summary>
    [HttpPost("in")]
    public async Task<IActionResult> CreateStockIn([FromBody] StockInCreateRequest request)
    {
        var sparepart = await _context.Spareparts.FindAsync(request.SparepartId);
        if (sparepart == null || sparepart.IsDeleted) return Ok(ApiResponse.Fail(400, "备件不存在"));

        if (request.Quantity <= 0) return Ok(ApiResponse.Fail(400, "入库数量必须大于0"));

        // 使用事务确保库存一致性
        await using var transaction = await _context.Database.BeginTransactionAsync();

        var record = new StockInRecord
        {
            SparepartId = request.SparepartId,
            Quantity = request.Quantity,
            UnitPrice = request.UnitPrice,
            Supplier = request.Supplier,
            InDate = request.InDate ?? DateTime.Now,
            OperatorId = request.OperatorId,
            Remark = request.Remark,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };

        _context.StockInRecords.Add(record);

        // 更新库存数量
        sparepart.StockQuantity += request.Quantity;
        sparepart.UpdatedAt = DateTime.Now;

        await _context.SaveChangesAsync();
        await transaction.CommitAsync();

        return Ok(ApiResponse<object>.Success(record, "入库成功"));
    }

    // ==================== 出库 ====================

    /// <summary>
    /// 获取出库记录（分页+筛选）
    /// </summary>
    [HttpGet("out")]
    public async Task<IActionResult> GetStockOutList([FromQuery] StockQueryRequest query)
    {
        var queryable = _context.StockOutRecords
            .Include(x => x.Sparepart)
                .ThenInclude(s => s!.Subsystem)
            .AsQueryable();

        if (query.SparepartId.HasValue)
        {
            queryable = queryable.Where(x => x.SparepartId == query.SparepartId.Value);
        }

        if (query.StartDate.HasValue)
        {
            queryable = queryable.Where(x => x.OutDate >= query.StartDate.Value);
        }

        if (query.EndDate.HasValue)
        {
            queryable = queryable.Where(x => x.OutDate <= query.EndDate.Value);
        }

        var total = await queryable.CountAsync();
        var items = await queryable
            .OrderByDescending(x => x.OutDate)
            .ThenByDescending(x => x.CreatedAt)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(x => new
            {
                x.Id,
                x.SparepartId,
                SparepartName = x.Sparepart != null ? x.Sparepart.Name : "-",
                SparepartCode = x.Sparepart != null ? x.Sparepart.Code : "-",
                SystemName = x.Sparepart != null && x.Sparepart.Subsystem != null ? x.Sparepart.Subsystem.Name : "-",
                x.Quantity,
                x.Department,
                x.Recipient,
                x.OutDate,
                x.OperatorId,
                x.Purpose,
                x.CreatedAt
            })
            .ToListAsync();

        return Ok(ApiResponse<object>.Success(new { items, total, page = query.Page, pageSize = query.PageSize }));
    }

    /// <summary>
    /// 新增出库记录
    /// </summary>
    [HttpPost("out")]
    public async Task<IActionResult> CreateStockOut([FromBody] StockOutCreateRequest request)
    {
        var sparepart = await _context.Spareparts.FindAsync(request.SparepartId);
        if (sparepart == null || sparepart.IsDeleted) return Ok(ApiResponse.Fail(400, "备件不存在"));

        if (request.Quantity <= 0) return Ok(ApiResponse.Fail(400, "出库数量必须大于0"));

        if (sparepart.StockQuantity < request.Quantity)
            return Ok(ApiResponse.Fail(400, $"库存不足，当前库存：{sparepart.StockQuantity}"));

        // 使用事务确保库存一致性
        await using var transaction = await _context.Database.BeginTransactionAsync();

        var record = new StockOutRecord
        {
            SparepartId = request.SparepartId,
            Quantity = request.Quantity,
            Department = request.Department,
            Recipient = request.Recipient,
            OutDate = request.OutDate ?? DateTime.Now,
            OperatorId = request.OperatorId,
            Purpose = request.Purpose,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };

        _context.StockOutRecords.Add(record);

        // 更新库存数量
        sparepart.StockQuantity -= request.Quantity;
        sparepart.UpdatedAt = DateTime.Now;

        await _context.SaveChangesAsync();
        await transaction.CommitAsync();

        return Ok(ApiResponse<object>.Success(record, "出库成功"));
    }
}

/// <summary>
/// 库存查询请求参数
/// </summary>
public class StockQueryRequest : PagedRequest
{
    public int? SparepartId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}

/// <summary>
/// 入库创建请求
/// </summary>
public class StockInCreateRequest
{
    public int SparepartId { get; set; }
    public int Quantity { get; set; }
    public decimal? UnitPrice { get; set; }
    public string? Supplier { get; set; }
    public DateTime? InDate { get; set; }
    public int? OperatorId { get; set; }
    public string? Remark { get; set; }
}

/// <summary>
/// 出库创建请求
/// </summary>
public class StockOutCreateRequest
{
    public int SparepartId { get; set; }
    public int Quantity { get; set; }
    public string? Department { get; set; }
    public string? Recipient { get; set; }
    public DateTime? OutDate { get; set; }
    public int? OperatorId { get; set; }
    public string? Purpose { get; set; }
}
