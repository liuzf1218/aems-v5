using AEMS.Core.DTOs;
using AEMS.Core.Entities;
using AEMS.Core.Interfaces;
using AEMS.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AEMS.Api.Controllers;

/// <summary>
/// 机房管理控制器
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RoomController : ControllerBase
{
    private readonly IRoomService _roomService;
    private readonly AemsDbContext _context;

    public RoomController(IRoomService roomService, AemsDbContext context)
    {
        _roomService = roomService;
        _context = context;
    }

    /// <summary>
    /// 获取机房列表（分页+筛选）
    /// </summary>
    [HttpGet]
    public async Task<ApiResponse<PagedResult<RoomListItemDto>>> GetList([FromQuery] RoomQueryRequest request)
    {
        var result = await _roomService.GetRoomListAsync(request);
        return ApiResponse<PagedResult<RoomListItemDto>>.Success(result);
    }

    /// <summary>
    /// 获取机房详情
    /// </summary>
    /// <param name="id">机房ID</param>
    [HttpGet("{id}")]
    public async Task<ApiResponse<RoomDetailDto>> GetById(int id)
    {
        var result = await _roomService.GetRoomDetailAsync(id);
        if (result == null)
        {
            return ApiResponse<RoomDetailDto>.Fail(404, "机房不存在");
        }
        return ApiResponse<RoomDetailDto>.Success(result);
    }

    /// <summary>
    /// 新增机房
    /// </summary>
    [HttpPost]
    public async Task<ApiResponse<RoomDetailDto>> Create([FromBody] RoomRequest request)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage);
            return ApiResponse<RoomDetailDto>.Fail(400, string.Join("; ", errors));
        }

        try
        {
            var result = await _roomService.CreateRoomAsync(request);
            return ApiResponse<RoomDetailDto>.Success(result, "创建成功");
        }
        catch (InvalidOperationException ex)
        {
            return ApiResponse<RoomDetailDto>.Fail(400, ex.Message);
        }
    }

    /// <summary>
    /// 更新机房
    /// </summary>
    /// <param name="id">机房ID</param>
    [HttpPut("{id}")]
    public async Task<ApiResponse<RoomDetailDto>> Update(int id, [FromBody] RoomRequest request)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage);
            return ApiResponse<RoomDetailDto>.Fail(400, string.Join("; ", errors));
        }

        try
        {
            var result = await _roomService.UpdateRoomAsync(id, request);
            if (result == null)
            {
                return ApiResponse<RoomDetailDto>.Fail(404, "机房不存在");
            }
            return ApiResponse<RoomDetailDto>.Success(result, "更新成功");
        }
        catch (InvalidOperationException ex)
        {
            return ApiResponse<RoomDetailDto>.Fail(400, ex.Message);
        }
    }

    /// <summary>
    /// 删除机房
    /// </summary>
    /// <param name="id">机房ID</param>
    [HttpDelete("{id}")]
    public async Task<ApiResponse> Delete(int id)
    {
        try
        {
            var result = await _roomService.DeleteRoomAsync(id);
            if (!result)
            {
                return ApiResponse.Fail(404, "机房不存在");
            }
            return ApiResponse.Success("删除成功");
        }
        catch (InvalidOperationException ex)
        {
            return ApiResponse.Fail(400, ex.Message);
        }
    }

    /// <summary>
    /// 获取机房下的机柜列表
    /// </summary>
    /// <param name="id">机房ID</param>
    [HttpGet("{id}/cabinets")]
    public async Task<ApiResponse<List<CabinetListItemDto>>> GetCabinets(int id)
    {
        // 验证机房是否存在
        var room = await _roomService.GetRoomDetailAsync(id);
        if (room == null)
        {
            return ApiResponse<List<CabinetListItemDto>>.Fail(404, "机房不存在");
        }

        var result = await _roomService.GetRoomCabinetsAsync(id);
        return ApiResponse<List<CabinetListItemDto>>.Success(result);
    }

    /// <summary>
    /// 获取机房内的设备列表
    /// </summary>
    /// <param name="id">机房ID</param>
    [HttpGet("{id}/devices")]
    public async Task<ApiResponse<List<RoomDeviceDto>>> GetDevices(int id)
    {
        // 验证机房是否存在
        var room = await _roomService.GetRoomDetailAsync(id);
        if (room == null)
        {
            return ApiResponse<List<RoomDeviceDto>>.Fail(404, "机房不存在");
        }

        var result = await _roomService.GetRoomDevicesAsync(id);
        return ApiResponse<List<RoomDeviceDto>>.Success(result);
    }

    /// <summary>
    /// 获取机房树形结构（楼宇→机房）
    /// </summary>
    [HttpGet("tree")]
    public async Task<IActionResult> GetTree()
    {
        var buildings = await _context.Buildings
            .Where(b => !b.IsDeleted)
            .Include(b => b.Rooms.Where(r => !r.IsDeleted))
            .OrderBy(b => b.Id)
            .ToListAsync();

        var result = buildings.Select(b => new
        {
            id = b.Id,
            name = b.Name,
            code = b.Code,
            nodeType = "building",
            children = b.Rooms.Select(r => new
            {
                id = r.Id,
                name = r.Name,
                code = r.Code,
                nodeType = "room",
                buildingId = b.Id,
                children = new List<object>()
            }).ToList()
        }).ToList();

        return Ok(ApiResponse<object>.Success(result));
    }

    /// <summary>
    /// 获取机房附属设施列表（无机柜且无系统的Equipment）
    /// </summary>
    [HttpGet("{id}/facilities")]
    public async Task<IActionResult> GetFacilities(int id)
    {
        var room = await _context.Rooms.FindAsync(id);
        if (room == null) return NotFound(ApiResponse.Fail(404, "机房不存在"));

        var facilities = await _context.Equipments
            .Where(e => !e.IsDeleted
                && e.EquipmentTypeId == 16
                && e.RoomId == id)
            .Include(e => e.EquipmentType)
            .Select(e => new
            {
                id = e.Id,
                name = e.Name,
                code = e.Code,
                type = e.EquipmentType != null ? e.EquipmentType.Name : "-",
                status = e.Status,
                manufacturer = e.Manufacturer ?? "-",
                position = e.Position ?? "-",
                remark = e.Remark ?? "-"
            })
            .ToListAsync();

        return Ok(ApiResponse<object>.Success(facilities));
    }

    /// <summary>
    /// 获取机房系统视图（按系统统计设备）
    /// </summary>
    [HttpGet("{id}/systems")]
    public async Task<IActionResult> GetSystemView(int id)
    {
        var room = await _context.Rooms.FindAsync(id);
        if (room == null) return NotFound(ApiResponse.Fail(404, "机房不存在"));

        var systems = await _context.Equipments
            .Where(e => !e.IsDeleted
                && e.SubsystemId != null
                && (e.RoomId == id || (e.Cabinet != null && e.Cabinet.RoomId == id)))
            .Include(e => e.Subsystem)
            .GroupBy(e => new { e.SubsystemId, e.Subsystem!.Name })
            .Select(g => new
            {
                systemId = g.Key.SubsystemId,
                systemName = g.Key.Name,
                deviceCount = g.Count()
            })
            .ToListAsync();

        return Ok(ApiResponse<object>.Success(systems));
    }
}
