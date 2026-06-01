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
public class BuildingController : ControllerBase
{
    private readonly AemsDbContext _context;
    public BuildingController(AemsDbContext context) { _context = context; }

    [HttpGet]
    public async Task<IActionResult> GetList([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? keyword = null)
    {
        var query = _context.Buildings.Where(b => !b.IsDeleted).AsQueryable();
        if (!string.IsNullOrEmpty(keyword))
            query = query.Where(b => b.Name.Contains(keyword) || b.Code.Contains(keyword));
        var total = await query.CountAsync();
        var items = await query.OrderByDescending(b => b.CreatedAt).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        return Ok(ApiResponse<object>.Success(new { items, total, page, pageSize }));
    }

    [HttpGet("tree")]
    public async Task<IActionResult> GetTree()
    {
        var buildings = await _context.Buildings
            .Where(b => !b.IsDeleted)
            .Include(b => b.Rooms)
            .ToListAsync();

        var tree = buildings.Select(b => new
        {
            id = b.Id,
            name = b.Name,
            code = b.Code,
            nodeType = "building",
            children = b.Rooms
                .Where(r => !r.IsDeleted)
                .Select(r => new
                {
                    id = r.Id,
                    name = r.Name,
                    code = r.Code,
                    nodeType = "room",
                    children = new List<object>()
                })
                .ToList()
        }).ToList();

        return Ok(ApiResponse<object>.Success(tree));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Building building)
    {
        building.CreatedAt = DateTime.Now;
        building.UpdatedAt = DateTime.Now;
        _context.Buildings.Add(building);
        await _context.SaveChangesAsync();
        return Ok(ApiResponse<object>.Success(building, "Created"));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] Building building)
    {
        var existing = await _context.Buildings.FindAsync(id);
        if (existing == null) return NotFound(ApiResponse.Fail(404, "Not Found"));
        existing.Name = building.Name;
        existing.Code = building.Code;
        existing.Location = building.Location;
        existing.Remark = building.Remark;
        existing.UpdatedAt = DateTime.Now;
        await _context.SaveChangesAsync();
        return Ok(ApiResponse<object>.Success(existing, "Updated"));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var building = await _context.Buildings
            .Include(b => b.Rooms)
            .FirstOrDefaultAsync(b => b.Id == id);
        if (building == null) return NotFound(ApiResponse.Fail(404, "Not Found"));
        if (building.Rooms.Any(r => !r.IsDeleted))
            return BadRequest(ApiResponse.Fail(400, "该楼宇下存在关联机房，无法删除"));
        building.IsDeleted = true;
        await _context.SaveChangesAsync();
        return Ok(ApiResponse.Success("Deleted"));
    }
}
