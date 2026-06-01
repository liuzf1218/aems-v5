using AEMS.Core.DTOs;
using AEMS.Core.Entities;
using AEMS.Core.Interfaces;
using AEMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AEMS.Infrastructure.Repositories;

/// <summary>
/// 机房服务实现
/// </summary>
public class RoomService : IRoomService
{
    private readonly AemsDbContext _context;

    public RoomService(AemsDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// 获取机房列表（分页+筛选）
    /// </summary>
    public async Task<PagedResult<RoomListItemDto>> GetRoomListAsync(RoomQueryRequest request)
    {
        var query = _context.Rooms
            .Where(r => !r.IsDeleted)
            .AsQueryable();

        // 关键词搜索
        if (!string.IsNullOrWhiteSpace(request.Keyword))
        {
            var keyword = request.Keyword.Trim().ToLower();
            query = query.Where(r =>
                r.Name.ToLower().Contains(keyword) ||
                r.Code.ToLower().Contains(keyword));
        }

        // 负责人筛选
        if (!string.IsNullOrWhiteSpace(request.Manager))
        {
            query = query.Where(r => r.Manager == request.Manager);
        }

        // 排序
        query = request.SortBy?.ToLower() switch
        {
            "name" => request.Desc ? query.OrderByDescending(r => r.Name) : query.OrderBy(r => r.Name),
            "code" => request.Desc ? query.OrderByDescending(r => r.Code) : query.OrderBy(r => r.Code),
            "createdat" => request.Desc ? query.OrderByDescending(r => r.CreatedAt) : query.OrderBy(r => r.CreatedAt),
            _ => query.OrderByDescending(r => r.CreatedAt)
        };

        var total = await query.CountAsync();

        var rooms = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync();

        // 批量查询机柜数和设备数
        var roomIds = rooms.Select(r => r.Id).ToList();
        var cabinetCounts = await _context.Cabinets
            .Where(c => roomIds.Contains(c.RoomId) && !c.IsDeleted)
            .GroupBy(c => c.RoomId)
            .Select(g => new { RoomId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.RoomId, x => x.Count);

        var cabinetIds = await _context.Cabinets
            .Where(c => roomIds.Contains(c.RoomId) && !c.IsDeleted)
            .Select(c => new { c.Id, c.RoomId })
            .ToListAsync();

        var cabinetIdList = cabinetIds.Select(c => c.Id).ToList();
        var deviceCountsByCabinet = await _context.Equipments
            .Where(e => e.CabinetId != null && cabinetIdList.Contains(e.CabinetId.Value) && !e.IsDeleted)
            .GroupBy(e => e.CabinetId!.Value)
            .Select(g => new { CabinetId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.CabinetId, x => x.Count);

        var cabinetToRoom = cabinetIds.ToDictionary(c => c.Id, c => c.RoomId);

        // 计算每个机房的设备数
        var deviceCounts = new Dictionary<int, int>();
        foreach (var kvp in deviceCountsByCabinet)
        {
            var roomId = cabinetToRoom[kvp.Key];
            deviceCounts[roomId] = deviceCounts.GetValueOrDefault(roomId) + kvp.Value;
        }

        var items = rooms.Select(r => new RoomListItemDto
        {
            Id = r.Id,
            Name = r.Name,
            Code = r.Code,
            Location = r.Location,
            Area = r.Area,
            Manager = r.Manager,
            CabinetCount = cabinetCounts.GetValueOrDefault(r.Id),
            DeviceCount = deviceCounts.GetValueOrDefault(r.Id),
            CreatedAt = r.CreatedAt
        }).ToList();

        return new PagedResult<RoomListItemDto>
        {
            Items = items,
            Total = total,
            Page = request.Page,
            PageSize = request.PageSize
        };
    }

    /// <summary>
    /// 获取机房详情
    /// </summary>
    public async Task<RoomDetailDto?> GetRoomDetailAsync(int id)
    {
        var room = await _context.Rooms
            .Include(r => r.Building)
            .FirstOrDefaultAsync(r => r.Id == id && !r.IsDeleted);

        if (room == null) return null;

        var cabinetCount = await _context.Cabinets
            .CountAsync(c => c.RoomId == id && !c.IsDeleted);

        var cabinetIds = await _context.Cabinets
            .Where(c => c.RoomId == id && !c.IsDeleted)
            .Select(c => c.Id)
            .ToListAsync();

        var deviceCount = await _context.Equipments
            .CountAsync(e => e.CabinetId != null && cabinetIds.Contains(e.CabinetId.Value) && !e.IsDeleted);

        return new RoomDetailDto
        {
            Id = room.Id,
            Name = room.Name,
            Code = room.Code,
            Location = room.Location,
            Area = room.Area,
            TempUpper = room.TempUpper,
            HumidityUpper = room.HumidityUpper,
            Manager = room.Manager,
            Remark = room.Remark,
            BuildingId = room.BuildingId,
            BuildingName = room.Building?.Name,
            Floor = room.Floor,
            CabinetCount = cabinetCount,
            DeviceCount = deviceCount,
            CreatedAt = room.CreatedAt,
            UpdatedAt = room.UpdatedAt
        };
    }

    /// <summary>
    /// 新增机房
    /// </summary>
    public async Task<RoomDetailDto> CreateRoomAsync(RoomRequest request)
    {
        // 检查编码唯一性
        var exists = await _context.Rooms
            .AnyAsync(r => r.Code == request.Code && !r.IsDeleted);
        if (exists)
        {
            throw new InvalidOperationException($"机房编码 '{request.Code}' 已存在");
        }

        var room = new Room
        {
            Name = request.Name,
            Code = request.Code,
            Location = request.Location,
            Area = request.Area,
            TempUpper = request.TempUpper,
            HumidityUpper = request.HumidityUpper,
            Manager = request.Manager,
            Remark = request.Remark
        };

        _context.Rooms.Add(room);
        await _context.SaveChangesAsync();

        return new RoomDetailDto
        {
            Id = room.Id,
            Name = room.Name,
            Code = room.Code,
            Location = room.Location,
            Area = room.Area,
            TempUpper = room.TempUpper,
            HumidityUpper = room.HumidityUpper,
            Manager = room.Manager,
            Remark = room.Remark,
            CabinetCount = 0,
            DeviceCount = 0,
            CreatedAt = room.CreatedAt,
            UpdatedAt = room.UpdatedAt
        };
    }

    /// <summary>
    /// 更新机房
    /// </summary>
    public async Task<RoomDetailDto?> UpdateRoomAsync(int id, RoomRequest request)
    {
        var room = await _context.Rooms
            .FirstOrDefaultAsync(r => r.Id == id && !r.IsDeleted);

        if (room == null) return null;

        // 检查编码唯一性（排除自身）
        var codeExists = await _context.Rooms
            .AnyAsync(r => r.Code == request.Code && r.Id != id && !r.IsDeleted);
        if (codeExists)
        {
            throw new InvalidOperationException($"机房编码 '{request.Code}' 已存在");
        }

        room.Name = request.Name;
        room.Code = request.Code;
        room.Location = request.Location;
        room.Area = request.Area;
        room.TempUpper = request.TempUpper;
        room.HumidityUpper = request.HumidityUpper;
        room.Manager = request.Manager;
        room.Remark = request.Remark;

        await _context.SaveChangesAsync();

        return await GetRoomDetailAsync(id);
    }

    /// <summary>
    /// 删除机房
    /// </summary>
    public async Task<bool> DeleteRoomAsync(int id)
    {
        var room = await _context.Rooms
            .FirstOrDefaultAsync(r => r.Id == id && !r.IsDeleted);

        if (room == null) return false;

        // 检查是否有机柜
        var hasCabinets = await _context.Cabinets
            .AnyAsync(c => c.RoomId == id && !c.IsDeleted);
        if (hasCabinets)
        {
            throw new InvalidOperationException("该机房下存在机柜，请先删除机柜");
        }

        room.IsDeleted = true;
        await _context.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// 获取机房下的机柜列表
    /// </summary>
    public async Task<List<CabinetListItemDto>> GetRoomCabinetsAsync(int roomId)
    {
        var cabinets = await _context.Cabinets
            .Where(c => c.RoomId == roomId && !c.IsDeleted)
            .OrderBy(c => c.Code)
            .ToListAsync();

        var cabinetIds = cabinets.Select(c => c.Id).ToList();
        var deviceCounts = await _context.Equipments
            .Where(e => e.CabinetId != null && cabinetIds.Contains(e.CabinetId.Value) && !e.IsDeleted)
            .GroupBy(e => e.CabinetId!.Value)
            .Select(g => new { CabinetId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.CabinetId, x => x.Count);

        return cabinets.Select(c => new CabinetListItemDto
        {
            Id = c.Id,
            Name = c.Name,
            Code = c.Code,
            RoomId = c.RoomId,
            TotalUnits = c.TotalUnits,
            UsedUnits = c.UsedUnits,
            PowerLimit = c.PowerLimit,
            DeviceCount = deviceCounts.GetValueOrDefault(c.Id),
            CreatedAt = c.CreatedAt
        }).ToList();
    }

    /// <summary>
    /// 获取机房内的设备列表
    /// </summary>
    public async Task<List<RoomDeviceDto>> GetRoomDevicesAsync(int roomId)
    {
        var cabinetIds = await _context.Cabinets
            .Where(c => c.RoomId == roomId && !c.IsDeleted)
            .Select(c => c.Id)
            .ToListAsync();

        var devices = await _context.Equipments
            .Where(e => e.CabinetId != null && cabinetIds.Contains(e.CabinetId.Value) && !e.IsDeleted)
            .Include(e => e.EquipmentType)
            .Include(e => e.Cabinet)
            .OrderBy(e => e.Cabinet!.Code)
            .ThenBy(e => e.Position)
            .ToListAsync();

        return devices.Select(e => new RoomDeviceDto
        {
            Id = e.Id,
            Name = e.Name,
            Code = e.Code,
            TypeName = e.EquipmentType?.Name,
            CabinetName = e.Cabinet?.Name,
            Position = e.Position,
            IpAddress = e.IpAddress,
            Status = e.Status,
            Manufacturer = e.Manufacturer,
            Model = e.Model
        }).ToList();
    }
}

/// <summary>
/// 机柜服务实现
/// </summary>
public class CabinetService : ICabinetService
{
    private readonly AemsDbContext _context;

    public CabinetService(AemsDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// 获取机柜列表（分页+筛选）
    /// </summary>
    public async Task<PagedResult<CabinetListItemDto>> GetCabinetListAsync(CabinetQueryRequest request)
    {
        var query = _context.Cabinets
            .Where(c => !c.IsDeleted)
            .Include(c => c.Room)
            .AsQueryable();

        // 关键词搜索
        if (!string.IsNullOrWhiteSpace(request.Keyword))
        {
            var keyword = request.Keyword.Trim().ToLower();
            query = query.Where(c =>
                c.Name.ToLower().Contains(keyword) ||
                c.Code.ToLower().Contains(keyword));
        }

        // 机房筛选
        if (request.RoomId.HasValue)
        {
            query = query.Where(c => c.RoomId == request.RoomId.Value);
        }

        // 排序
        query = request.SortBy?.ToLower() switch
        {
            "name" => request.Desc ? query.OrderByDescending(c => c.Name) : query.OrderBy(c => c.Name),
            "code" => request.Desc ? query.OrderByDescending(c => c.Code) : query.OrderBy(c => c.Code),
            "roomid" => request.Desc ? query.OrderByDescending(c => c.RoomId) : query.OrderBy(c => c.RoomId),
            _ => query.OrderByDescending(c => c.CreatedAt)
        };

        var total = await query.CountAsync();

        var cabinets = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync();

        // 批量查询设备数
        var cabinetIds = cabinets.Select(c => c.Id).ToList();
        var deviceCounts = await _context.Equipments
            .Where(e => e.CabinetId != null && cabinetIds.Contains(e.CabinetId.Value) && !e.IsDeleted)
            .GroupBy(e => e.CabinetId!.Value)
            .Select(g => new { CabinetId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.CabinetId, x => x.Count);

        var items = cabinets.Select(c => new CabinetListItemDto
        {
            Id = c.Id,
            Name = c.Name,
            Code = c.Code,
            RoomId = c.RoomId,
            RoomName = c.Room?.Name,
            TotalUnits = c.TotalUnits,
            UsedUnits = c.UsedUnits,
            PowerLimit = c.PowerLimit,
            DeviceCount = deviceCounts.GetValueOrDefault(c.Id),
            CreatedAt = c.CreatedAt
        }).ToList();

        return new PagedResult<CabinetListItemDto>
        {
            Items = items,
            Total = total,
            Page = request.Page,
            PageSize = request.PageSize
        };
    }

    /// <summary>
    /// 获取机柜详情
    /// </summary>
    public async Task<CabinetDetailDto?> GetCabinetDetailAsync(int id)
    {
        var cabinet = await _context.Cabinets
            .Include(c => c.Room)
            .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);

        if (cabinet == null) return null;

        var deviceCount = await _context.Equipments
            .CountAsync(e => e.CabinetId == id && !e.IsDeleted);

        return new CabinetDetailDto
        {
            Id = cabinet.Id,
            Name = cabinet.Name,
            Code = cabinet.Code,
            RoomId = cabinet.RoomId,
            RoomName = cabinet.Room?.Name,
            TotalUnits = cabinet.TotalUnits,
            UsedUnits = cabinet.UsedUnits,
            PowerLimit = cabinet.PowerLimit,
            Remark = cabinet.Remark,
            DeviceCount = deviceCount,
            CreatedAt = cabinet.CreatedAt,
            UpdatedAt = cabinet.UpdatedAt
        };
    }

    /// <summary>
    /// 新增机柜
    /// </summary>
    public async Task<CabinetDetailDto> CreateCabinetAsync(CabinetRequest request)
    {
        // 检查机房是否存在
        var roomExists = await _context.Rooms
            .AnyAsync(r => r.Id == request.RoomId && !r.IsDeleted);
        if (!roomExists)
        {
            throw new InvalidOperationException("指定的机房不存在");
        }

        // 检查编码唯一性
        var exists = await _context.Cabinets
            .AnyAsync(c => c.Code == request.Code && !c.IsDeleted);
        if (exists)
        {
            throw new InvalidOperationException($"机柜编码 '{request.Code}' 已存在");
        }

        var cabinet = new Cabinet
        {
            Name = request.Name,
            Code = request.Code,
            RoomId = request.RoomId,
            TotalUnits = request.TotalUnits,
            PowerLimit = request.PowerLimit,
            Remark = request.Remark
        };

        _context.Cabinets.Add(cabinet);
        await _context.SaveChangesAsync();

        return await GetCabinetDetailAsync(cabinet.Id)
            ?? throw new InvalidOperationException("创建机柜失败");
    }

    /// <summary>
    /// 更新机柜
    /// </summary>
    public async Task<CabinetDetailDto?> UpdateCabinetAsync(int id, CabinetRequest request)
    {
        var cabinet = await _context.Cabinets
            .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);

        if (cabinet == null) return null;

        // 检查机房是否存在
        var roomExists = await _context.Rooms
            .AnyAsync(r => r.Id == request.RoomId && !r.IsDeleted);
        if (!roomExists)
        {
            throw new InvalidOperationException("指定的机房不存在");
        }

        // 检查编码唯一性（排除自身）
        var codeExists = await _context.Cabinets
            .AnyAsync(c => c.Code == request.Code && c.Id != id && !c.IsDeleted);
        if (codeExists)
        {
            throw new InvalidOperationException($"机柜编码 '{request.Code}' 已存在");
        }

        cabinet.Name = request.Name;
        cabinet.Code = request.Code;
        cabinet.RoomId = request.RoomId;
        cabinet.TotalUnits = request.TotalUnits;
        cabinet.PowerLimit = request.PowerLimit;
        cabinet.Remark = request.Remark;

        await _context.SaveChangesAsync();

        return await GetCabinetDetailAsync(id);
    }

    /// <summary>
    /// 删除机柜
    /// </summary>
    public async Task<bool> DeleteCabinetAsync(int id)
    {
        var cabinet = await _context.Cabinets
            .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);

        if (cabinet == null) return false;

        // 检查是否有设备
        var hasDevices = await _context.Equipments
            .AnyAsync(e => e.CabinetId == id && !e.IsDeleted);
        if (hasDevices)
        {
            throw new InvalidOperationException("该机柜下存在设备，请先移除设备");
        }

        cabinet.IsDeleted = true;
        await _context.SaveChangesAsync();
        return true;
    }
}
