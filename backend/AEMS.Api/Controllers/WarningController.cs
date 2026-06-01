using AEMS.Core.DTOs;
using AEMS.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AEMS.Api.Controllers;

[ApiController]
[Route("api/sparepart/[controller]")]
[Authorize]
public class WarningController : ControllerBase
{
    private readonly AemsDbContext _context;

    public WarningController(AemsDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// 获取库存预警列表
    /// 当前库存 <= 最低库存预警线的备件
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetWarnings()
    {
        var warnings = await _context.Spareparts
            .Where(x => !x.IsDeleted && x.MinStock > 0 && x.StockQuantity <= x.MinStock)
            .OrderBy(x => x.StockQuantity)
            .ThenBy(x => x.Name)
            .ToListAsync();

        // 统计数据
        var totalSparepart = await _context.Spareparts.CountAsync(x => !x.IsDeleted);
        var warningCount = warnings.Count;
        var outOfStockCount = warnings.Count(x => x.StockQuantity == 0);
        var lowStockCount = warnings.Count(x => x.StockQuantity > 0);

        return Ok(ApiResponse<object>.Success(new
        {
            stats = new
            {
                totalSparepart,
                warningCount,
                outOfStockCount,
                lowStockCount
            },
            list = warnings.Select(x => new
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
                Deficit = x.MinStock - x.StockQuantity,
                Status = x.StockQuantity == 0 ? "out_of_stock" : "low_stock"
            })
        }));
    }

    /// <summary>
    /// 生成采购申请
    /// </summary>
    [HttpPost("purchase")]
    public async Task<IActionResult> CreatePurchaseRequest([FromBody] PurchaseRequest request)
    {
        var spareparts = await _context.Spareparts
            .Where(x => request.SparepartIds.Contains(x.Id) && !x.IsDeleted)
            .ToListAsync();

        if (!spareparts.Any()) return Ok(ApiResponse.Fail(400, "未找到选中的备件"));

        // 生成采购申请单号
        var purchaseNo = $"PO{DateTime.Now:yyyyMMddHHmmss}";

        var items = spareparts.Select(x => new
        {
            x.Id,
            x.Name,
            x.Code,
            x.Specification,
            x.Unit,
            CurrentStock = x.StockQuantity,
            x.MinStock,
            RequiredQuantity = x.MinStock - x.StockQuantity,
            x.Price,
            EstimatedCost = x.Price * (x.MinStock - x.StockQuantity)
        }).ToList();

        // TODO: 持久化采购申请到数据库（待采购申请表创建后实现）
        var result = new
        {
            purchaseNo,
            createdAt = DateTime.Now,
            items,
            totalAmount = items.Sum(i => i.EstimatedCost ?? 0)
        };

        return Ok(ApiResponse<object>.Success(result, "采购申请已生成"));
    }
}

/// <summary>
/// 采购申请请求
/// </summary>
public class PurchaseRequest
{
    public List<int> SparepartIds { get; set; } = new();
}
