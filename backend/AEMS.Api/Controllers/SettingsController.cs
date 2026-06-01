using AEMS.Core.DTOs;
using AEMS.Core.Entities;
using AEMS.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace AEMS.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SettingsController : ControllerBase
{
    private readonly AemsDbContext _context;
    public SettingsController(AemsDbContext context) { _context = context; }

    /// <summary>
    /// 获取指定类别的系统设置
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] string category)
    {
        var dict = await _context.SysDicts
            .FirstOrDefaultAsync(d => d.DictType == "system_config" && d.Label == category && !d.IsDeleted);
        if (dict == null) return Ok(ApiResponse<object>.Success(new object()));
        try
        {
            var data = JsonSerializer.Deserialize<object>(dict.Value);
            return Ok(ApiResponse<object>.Success(data ?? new object()));
        }
        catch
        {
            return Ok(ApiResponse<object>.Success(new object()));
        }
    }

    /// <summary>
    /// 保存指定类别的系统设置
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Save([FromBody] SaveSettingsRequest request)
    {
        if (string.IsNullOrEmpty(request.Category)) return BadRequest(ApiResponse.Fail(400, "Category is required"));
        var json = JsonSerializer.Serialize(request.Data);
        var dict = await _context.SysDicts
            .FirstOrDefaultAsync(d => d.DictType == "system_config" && d.Label == request.Category);
        if (dict == null)
        {
            dict = new SysDict
            {
                DictType = "system_config",
                Label = request.Category,
                Value = json,
                SortOrder = 0,
                IsActive = true,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                IsDeleted = false
            };
            _context.SysDicts.Add(dict);
        }
        else
        {
            dict.Value = json;
            dict.UpdatedAt = DateTime.Now;
        }
        await _context.SaveChangesAsync();
        return Ok(ApiResponse<object>.Success(null, "设置保存成功"));
    }
}

public class SaveSettingsRequest
{
    public string Category { get; set; } = string.Empty;
    public object Data { get; set; } = new object();
}
