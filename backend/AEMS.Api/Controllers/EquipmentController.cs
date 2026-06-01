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
public class EquipmentController : ControllerBase
{
    private readonly AemsDbContext _context;
    public EquipmentController(AemsDbContext context) { _context = context; }

    [HttpGet]
    public async Task<IActionResult> GetList([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? keyword = null, [FromQuery] int? subsystemId = null, [FromQuery] int? categoryId = null)
    {
        var query = _context.Equipments
            .Include(e => e.EquipmentType)
            .Include(e => e.Subsystem)
            .Where(e => !e.IsDeleted)
            .AsQueryable();
        if (!string.IsNullOrEmpty(keyword))
            query = query.Where(e => e.Name.Contains(keyword) || e.Code.Contains(keyword));
        if (subsystemId.HasValue)
            query = query.Where(e => e.SubsystemId == subsystemId.Value);
        if (categoryId.HasValue)
            query = query.Where(e => e.Subsystem != null && e.Subsystem.CategoryId == categoryId.Value);
        var total = await query.CountAsync();
        var items = await query
            .OrderByDescending(e => e.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(e => new
            {
                id = e.Id,
                code = e.Code,
                name = e.Name,
                model = e.Model ?? "-",
                categoryName = e.EquipmentType != null ? e.EquipmentType.Name : "-",
                systemName = e.Subsystem != null ? e.Subsystem.Name : "-",
                location = e.Position ?? "-",
                status = e.Status,
                criticality = e.Criticality,
                manufacturer = e.Manufacturer ?? "-",
                runtimeHours = e.RuntimeHours,
                lastMaintenanceDate = e.LastMaintenanceDate != null ? e.LastMaintenanceDate.Value.ToString("yyyy-MM-dd") : "-",
                nextMaintenanceDate = e.NextMaintenanceDate != null ? e.NextMaintenanceDate.Value.ToString("yyyy-MM-dd") : "-",
                failureCount = e.FailureCount,
                ipAddress = e.IpAddress ?? "-",
                serialNumber = e.SerialNumber ?? "-",
                purchaseDate = e.PurchaseDate,
                warrantyDate = e.WarrantyDate,
                equipmentTypeId = e.EquipmentTypeId,
                subsystemId = e.SubsystemId
            })
            .ToListAsync();
        return Ok(ApiResponse<object>.Success(new { items, total, page, pageSize }));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var eq = await _context.Equipments
            .Include(e => e.EquipmentType)
            .Include(e => e.Subsystem)
            .Include(e => e.Cabinet)
            .FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted);
        if (eq == null) return NotFound(ApiResponse.Fail(404, "Not Found"));
        return Ok(ApiResponse<object>.Success(new
        {
            id = eq.Id,
            code = eq.Code,
            name = eq.Name,
            model = eq.Model ?? "-",
            categoryName = eq.EquipmentType?.Name ?? "-",
            systemName = eq.Subsystem?.Name ?? "-",
            location = eq.Position ?? "-",
            status = eq.Status,
            criticality = eq.Criticality,
            manufacturer = eq.Manufacturer ?? "-",
            serialNumber = eq.SerialNumber ?? "-",
            installDate = eq.InstallDate?.ToString("yyyy-MM-dd") ?? "-",
            warrantyExpiry = eq.WarrantyDate?.ToString("yyyy-MM-dd") ?? "-",
            runtimeHours = eq.RuntimeHours,
            failureCount = eq.FailureCount,
            lastMaintenanceDate = eq.LastMaintenanceDate?.ToString("yyyy-MM-dd") ?? "-",
            nextMaintenanceDate = eq.NextMaintenanceDate?.ToString("yyyy-MM-dd") ?? "-",
            ipAddress = eq.IpAddress ?? "-",
            macAddress = eq.MacAddress ?? "-",
            purchaseDate = eq.PurchaseDate?.ToString("yyyy-MM-dd") ?? "-",
            remark = eq.Remark ?? "-",
            cabinetName = eq.Cabinet?.Name ?? "-",
            equipmentTypeId = eq.EquipmentTypeId,
            subsystemId = eq.SubsystemId
        }));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Equipment equipment)
    {
        equipment.CreatedAt = DateTime.Now;
        equipment.UpdatedAt = DateTime.Now;
        _context.Equipments.Add(equipment);
        await _context.SaveChangesAsync();
        return Ok(ApiResponse<object>.Success(equipment, "Created"));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] Equipment equipment)
    {
        var existing = await _context.Equipments.FindAsync(id);
        if (existing == null) return NotFound(ApiResponse.Fail(404, "Not Found"));
        existing.Name = equipment.Name;
        existing.Code = equipment.Code;
        existing.EquipmentTypeId = equipment.EquipmentTypeId;
        existing.SubsystemId = equipment.SubsystemId;
        existing.CabinetId = equipment.CabinetId;
        existing.RoomId = equipment.RoomId;
        existing.Model = equipment.Model;
        existing.Status = equipment.Status;
        existing.Criticality = equipment.Criticality;
        existing.Manufacturer = equipment.Manufacturer;
        existing.SerialNumber = equipment.SerialNumber;
        existing.Position = equipment.Position;
        existing.IpAddress = equipment.IpAddress;
        existing.MacAddress = equipment.MacAddress;
        existing.WarrantyDate = equipment.WarrantyDate;
        existing.PurchaseDate = equipment.PurchaseDate;
        existing.Remark = equipment.Remark;
        existing.UpdatedAt = DateTime.Now;
        await _context.SaveChangesAsync();
        return Ok(ApiResponse<object>.Success(existing, "Updated"));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var eq = await _context.Equipments.FindAsync(id);
        if (eq == null) return NotFound(ApiResponse.Fail(404, "Not Found"));
        eq.IsDeleted = true;
        await _context.SaveChangesAsync();
        return Ok(ApiResponse.Success("Deleted"));
    }

    [HttpGet("tree")]
    public async Task<IActionResult> GetTree()
    {
        var allTypes = await _context.EquipmentTypes.Where(x => !x.IsDeleted).ToListAsync();
        var systems = await _context.Subsystems.Where(x => !x.IsDeleted).ToListAsync();
        var rootTypes = allTypes.Where(x => x.ParentId == null).OrderBy(x => x.Id).ToList();
        var result = rootTypes.Select(t => BuildSystemTreeNode(t, systems)).ToList();
        return Ok(ApiResponse<object>.Success(result));
    }

    private static object BuildSystemTreeNode(EquipmentType type, List<Subsystem> systems)
    {
        var typeSystems = systems.Where(s => s.CategoryId == type.Id).OrderBy(s => s.Id).ToList();
        return new
        {
            id = type.Id,
            name = type.Name,
            code = type.Code,
            remark = type.Remark,
            nodeType = "category",
            children = typeSystems.Select(s => new
            {
                id = s.Id,
                name = s.Name,
                code = s.Code,
                nodeType = "system",
                categoryId = s.CategoryId,
                children = new List<object>()
            }).ToList()
        };
    }

    [HttpGet("health")]
    public async Task<IActionResult> GetHealth()
    {
        var devices = await _context.Equipments.Where(e => !e.IsDeleted).ToListAsync();
        return Ok(ApiResponse<object>.Success(devices));
    }
}
