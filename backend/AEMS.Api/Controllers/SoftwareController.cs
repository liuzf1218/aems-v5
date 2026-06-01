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
public class SoftwareController : ControllerBase
{
    private readonly AemsDbContext _context;
    public SoftwareController(AemsDbContext context) { _context = context; }

    [HttpGet]
    public async Task<IActionResult> GetList([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? keyword = null)
    {
        var query = _context.Softwares.Where(s => !s.IsDeleted).AsQueryable();
        if (!string.IsNullOrEmpty(keyword))
            query = query.Where(s => s.Name.Contains(keyword) || s.Code.Contains(keyword));
        var total = await query.CountAsync();
        var items = await query
            .OrderByDescending(s => s.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Include(s => s.Equipment)
            .ThenInclude(e => e!.Subsystem)
            .Select(s => new
            {
                s.Id,
                s.Name,
                s.Code,
                s.Vendor,
                s.SoftwareType,
                s.LicenseType,
                s.Remark,
                s.EquipmentId,
                SystemName = s.Equipment != null && s.Equipment.Subsystem != null ? s.Equipment.Subsystem.Name : null,
                s.CreatedAt,
                s.UpdatedAt
            })
            .ToListAsync();
        return Ok(ApiResponse<object>.Success(new { items, total, page, pageSize }));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var sw = await _context.Softwares
            .Include(s => s.Equipment)
            .ThenInclude(e => e!.Subsystem)
            .FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted);
        if (sw == null) return NotFound(ApiResponse.Fail(404, "Not Found"));
        return Ok(ApiResponse<object>.Success(new
        {
            sw.Id,
            sw.Name,
            sw.Code,
            sw.Vendor,
            sw.SoftwareType,
            sw.LicenseType,
            sw.Remark,
            sw.EquipmentId,
            SystemName = sw.Equipment != null && sw.Equipment.Subsystem != null ? sw.Equipment.Subsystem.Name : null,
            sw.CreatedAt,
            sw.UpdatedAt
        }));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Software software)
    {
        software.CreatedAt = DateTime.Now;
        software.UpdatedAt = DateTime.Now;
        _context.Softwares.Add(software);
        await _context.SaveChangesAsync();
        return Ok(ApiResponse<object>.Success(software, "Created"));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] Software software)
    {
        var existing = await _context.Softwares.FindAsync(id);
        if (existing == null) return NotFound(ApiResponse.Fail(404, "Not Found"));
        existing.Name = software.Name;
        existing.Vendor = software.Vendor;
        existing.SoftwareType = software.SoftwareType;
        existing.EquipmentId = software.EquipmentId;
        existing.UpdatedAt = DateTime.Now;
        await _context.SaveChangesAsync();
        return Ok(ApiResponse<object>.Success(existing, "Updated"));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var sw = await _context.Softwares.FindAsync(id);
        if (sw == null) return NotFound(ApiResponse.Fail(404, "Not Found"));
        sw.IsDeleted = true;
        await _context.SaveChangesAsync();
        return Ok(ApiResponse.Success("Deleted"));
    }

    [HttpGet("{id}/versions")]
    public async Task<IActionResult> GetVersions(int id)
    {
        var versions = await _context.SoftwareVersions.Where(v => v.SoftwareId == id).OrderByDescending(v => v.ReleaseDate).ToListAsync();
        return Ok(ApiResponse<object>.Success(versions));
    }

    [HttpGet("instances")]
    public async Task<IActionResult> GetInstances([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var total = await _context.SoftwareInstances.CountAsync();
        var items = await _context.SoftwareInstances
            .OrderByDescending(i => i.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Include(i => i.Equipment)
            .ThenInclude(e => e!.Subsystem)
            .Select(i => new
            {
                i.Id,
                i.SoftwareVersionId,
                i.EquipmentId,
                i.InstallPath,
                i.InstallDate,
                i.Status,
                i.Remark,
                EquipmentName = i.Equipment != null ? i.Equipment.Name : null,
                SystemName = i.Equipment != null && i.Equipment.Subsystem != null ? i.Equipment.Subsystem.Name : null,
                i.CreatedAt,
                i.UpdatedAt
            })
            .ToListAsync();
        return Ok(ApiResponse<object>.Success(new { items, total, page, pageSize }));
    }
}
