using AEMS.Core.DTOs;
using AEMS.Core.Interfaces;
using AEMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AEMS.Infrastructure.Repositories;

/// <summary>
/// 统计报表服务实现
/// </summary>
public class StatisticsService : IStatisticsService
{
    private readonly AemsDbContext _context;

    public StatisticsService(AemsDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// 获取仪表盘数据
    /// </summary>
    public async Task<DashboardDto> GetDashboardAsync()
    {
        var now = DateTime.Now;
        var thirtyDaysAgo = now.AddDays(-30);

        // 设备总数（未删除）
        var equipmentTotal = await _context.Equipments.CountAsync(x => !x.IsDeleted);

        // 正常设备数（状态=1）
        var normalCount = await _context.Equipments.CountAsync(x => !x.IsDeleted && x.Status == 0);

        // 在线率
        var onlineRate = equipmentTotal > 0
            ? Math.Round((decimal)normalCount / equipmentTotal * 100, 1)
            : 0;

        // 待处理工单数（状态=0）
        var pendingWorkorders = await _context.Workorders.CountAsync(x => !x.IsDeleted && x.Status == 0);

        // 活跃告警数 = 故障设备数 + 库存预警数
        var faultEquipment = await _context.Equipments.CountAsync(x => !x.IsDeleted && x.Status == 1);
        var stockWarnings = await _context.Spareparts.CountAsync(x => !x.IsDeleted && x.StockQuantity < x.MinStock);
        var activeAlerts = faultEquipment + stockWarnings;

        // 系统状态（按Room统计）
        var rooms = await _context.Rooms.Where(x => !x.IsDeleted).ToListAsync();
        var systemStatus = new List<SystemStatusDto>();
        foreach (var room in rooms)
        {
            var roomEquipmentCount = await _context.Equipments
                .CountAsync(x => !x.IsDeleted && x.Cabinet != null && x.Cabinet.RoomId == room.Id);
            var roomFaultCount = await _context.Equipments
                .CountAsync(x => !x.IsDeleted && x.Status == 1 && x.Cabinet != null && x.Cabinet.RoomId == room.Id);

            string status = "normal";
            string label = "正常";
            if (roomFaultCount > 0)
            {
                status = "error";
                label = $"故障 {roomFaultCount} 台";
            }

            systemStatus.Add(new SystemStatusDto
            {
                Name = room.Name,
                Status = status,
                Label = label
            });
        }

        // 最新告警（故障设备 + 库存预警，取最近的）
        var recentAlerts = new List<AlertDto>();

        var faultEquipments = await _context.Equipments
            .Where(x => !x.IsDeleted && x.Status == 1)
            .OrderByDescending(x => x.UpdatedAt)
            .Take(5)
            .Select(x => new AlertDto
            {
                Level = "严重",
                Message = $"设备「{x.Name}({x.Code})」故障",
                Time = x.UpdatedAt.ToString("yyyy-MM-dd HH:mm")
            })
            .ToListAsync();
        recentAlerts.AddRange(faultEquipments);

        var warnSpareparts = await _context.Spareparts
            .Where(x => !x.IsDeleted && x.StockQuantity < x.MinStock)
            .OrderBy(x => x.StockQuantity)
            .Take(5)
            .Select(x => new AlertDto
            {
                Level = "警告",
                Message = $"备件「{x.Name}」库存不足（当前{x.StockQuantity}，最低{x.MinStock}）",
                Time = x.UpdatedAt.ToString("yyyy-MM-dd HH:mm")
            })
            .ToListAsync();
        recentAlerts.AddRange(warnSpareparts);

        // 设备状态分布
        var statusDist = await _context.Equipments
            .Where(x => !x.IsDeleted)
            .GroupBy(x => x.Status)
            .Select(g => new { Status = g.Key, Count = g.Count() })
            .ToListAsync();

        var statusNameMap = new Dictionary<int, string> { { 0, "在用" }, { 1, "故障" }, { 2, "维护中" } };
        var equipmentStatusDistribution = statusDist.Select(x => new NameValueDto
        {
            Name = statusNameMap.GetValueOrDefault(x.Status, "未知"),
            Value = x.Count
        }).ToList();

        // 近30天工单趋势
        var workorderTrend = await _context.Workorders
            .Where(x => !x.IsDeleted && x.CreatedAt >= thirtyDaysAgo)
            .GroupBy(x => x.CreatedAt.Date)
            .Select(g => new { Date = g.Key, Count = g.Count() })
            .OrderBy(x => x.Date)
            .ToListAsync();

        var trendList = new List<TrendDto>();
        for (var d = thirtyDaysAgo.Date; d <= now.Date; d = d.AddDays(1))
        {
            var dayCount = workorderTrend.FirstOrDefault(x => x.Date == d)?.Count ?? 0;
            trendList.Add(new TrendDto { Date = d.ToString("MM-dd"), Count = dayCount });
        }

        // 系统设备数分布（按Room）
        var systemEquipmentCount = new List<NameValueDto>();
        foreach (var room in rooms)
        {
            var count = await _context.Equipments
                .CountAsync(x => !x.IsDeleted && x.Cabinet != null && x.Cabinet.RoomId == room.Id);
            if (count > 0)
            {
                systemEquipmentCount.Add(new NameValueDto { Name = room.Name, Value = count });
            }
        }

        return new DashboardDto
        {
            EquipmentTotal = equipmentTotal,
            OnlineRate = onlineRate,
            PendingWorkorders = pendingWorkorders,
            ActiveAlerts = activeAlerts,
            SystemStatus = systemStatus,
            RecentAlerts = recentAlerts.Take(10).ToList(),
            EquipmentStatusDistribution = equipmentStatusDistribution,
            WorkorderTrend = trendList,
            SystemEquipmentCount = systemEquipmentCount
        };
    }

    /// <summary>
    /// 获取设备统计数据
    /// </summary>
    public async Task<DeviceStatisticsDto> GetDeviceStatisticsAsync()
    {
        var total = await _context.Equipments.CountAsync(x => !x.IsDeleted);

        // 类型分布
        var typeDistribution = await _context.Equipments
            .Where(x => !x.IsDeleted)
            .GroupBy(x => x.EquipmentTypeId)
            .Select(g => new { TypeId = g.Key, Count = g.Count() })
            .ToListAsync();

        var typeIds = typeDistribution.Where(x => x.TypeId.HasValue).Select(x => x.TypeId!.Value).ToList();
        var typeNames = await _context.EquipmentTypes
            .Where(x => typeIds.Contains(x.Id))
            .ToDictionaryAsync(x => x.Id, x => x.Name);

        var typeDistList = typeDistribution.Select(x => new NameValueDto
        {
            Name = x.TypeId.HasValue ? typeNames.GetValueOrDefault(x.TypeId.Value, "未分类") : "未分类",
            Value = x.Count
        }).ToList();

        // 状态分布
        var statusNameMap = new Dictionary<int, string> { { 0, "在用" }, { 1, "故障" }, { 2, "维护中" } };
        var statusDistribution = await _context.Equipments
            .Where(x => !x.IsDeleted)
            .GroupBy(x => x.Status)
            .Select(g => new NameValueDto
            {
                Name = statusNameMap.ContainsKey(g.Key) ? statusNameMap[g.Key] : "未知",
                Value = g.Count()
            })
            .ToListAsync();

        // 各系统设备数（按Room）
        var rooms = await _context.Rooms.Where(x => !x.IsDeleted).ToListAsync();
        var systemEquipmentCount = new List<NameValueDto>();
        foreach (var room in rooms)
        {
            var count = await _context.Equipments
                .CountAsync(x => !x.IsDeleted && x.Cabinet != null && x.Cabinet.RoomId == room.Id);
            if (count > 0)
            {
                systemEquipmentCount.Add(new NameValueDto { Name = room.Name, Value = count });
            }
        }

        return new DeviceStatisticsDto
        {
            Total = total,
            TypeDistribution = typeDistList,
            StatusDistribution = statusDistribution,
            SystemEquipmentCount = systemEquipmentCount
        };
    }

    /// <summary>
    /// 获取工单统计数据
    /// </summary>
    public async Task<WorkorderStatisticsDto> GetWorkorderStatisticsAsync()
    {
        var total = await _context.Workorders.CountAsync(x => !x.IsDeleted);
        var now = DateTime.Now;
        var twelveMonthsAgo = new DateTime(now.Year, now.Month, 1).AddMonths(-11);

        // 月度趋势（近12个月）
        var monthlyData = await _context.Workorders
            .Where(x => !x.IsDeleted && x.CreatedAt >= twelveMonthsAgo)
            .GroupBy(x => new { x.CreatedAt.Year, x.CreatedAt.Month })
            .Select(g => new { g.Key.Year, g.Key.Month, Count = g.Count() })
            .OrderBy(x => x.Year).ThenBy(x => x.Month)
            .ToListAsync();

        var monthlyTrend = new List<TrendDto>();
        for (var d = twelveMonthsAgo; d <= now; d = d.AddMonths(1))
        {
            var monthData = monthlyData.FirstOrDefault(x => x.Year == d.Year && x.Month == d.Month);
            monthlyTrend.Add(new TrendDto
            {
                Date = d.ToString("yyyy-MM"),
                Count = monthData?.Count ?? 0
            });
        }

        // 类型分布（按故障类型）
        var typeData = await _context.Workorders
            .Where(x => !x.IsDeleted && x.FaultTypeId != null)
            .GroupBy(x => x.FaultTypeId)
            .Select(g => new { TypeId = g.Key, Count = g.Count() })
            .ToListAsync();

        var typeIds = typeData.Where(x => x.TypeId.HasValue).Select(x => x.TypeId!.Value).ToList();
        var faultTypeNames = await _context.FaultTypes
            .Where(x => typeIds.Contains(x.Id))
            .ToDictionaryAsync(x => x.Id, x => x.Name);

        var typeDistribution = typeData.Select(x => new NameValueDto
        {
            Name = x.TypeId.HasValue ? faultTypeNames.GetValueOrDefault(x.TypeId.Value, "未分类") : "未分类",
            Value = x.Count
        }).ToList();

        // 故障等级分布
        var priorityNameMap = new Dictionary<int, string>
        {
            { 1, "低" }, { 2, "中" }, { 3, "高" }, { 4, "紧急" }
        };
        var priorityDistribution = await _context.Workorders
            .Where(x => !x.IsDeleted)
            .GroupBy(x => x.Priority)
            .Select(g => new NameValueDto
            {
                Name = priorityNameMap.ContainsKey(g.Key) ? priorityNameMap[g.Key] : "未知",
                Value = g.Count()
            })
            .ToListAsync();

        // 故障频次TOP5设备
        var faultTop5 = await _context.Workorders
            .Where(x => !x.IsDeleted && x.EquipmentId != null)
            .GroupBy(x => x.EquipmentId)
            .Select(g => new { EquipmentId = g.Key, Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .Take(5)
            .ToListAsync();

        var eqIds = faultTop5.Where(x => x.EquipmentId.HasValue).Select(x => x.EquipmentId!.Value).ToList();
        var eqNames = await _context.Equipments
            .Where(x => eqIds.Contains(x.Id))
            .ToDictionaryAsync(x => x.Id, x => $"{x.Name}({x.Code})");

        var faultTop5List = faultTop5.Select(x => new NameValueDto
        {
            Name = x.EquipmentId.HasValue ? eqNames.GetValueOrDefault(x.EquipmentId.Value, "未知设备") : "未知设备",
            Value = x.Count
        }).ToList();

        return new WorkorderStatisticsDto
        {
            Total = total,
            MonthlyTrend = monthlyTrend,
            TypeDistribution = typeDistribution,
            PriorityDistribution = priorityDistribution,
            FaultTop5 = faultTop5List
        };
    }

    /// <summary>
    /// 获取设备增长趋势（按月，近12个月）
    /// </summary>
    public async Task<List<TrendDto>> GetDeviceTrendAsync()
    {
        var now = DateTime.Now;
        var twelveMonthsAgo = new DateTime(now.Year, now.Month, 1).AddMonths(-11);

        var monthlyData = await _context.Equipments
            .Where(x => !x.IsDeleted && x.CreatedAt >= twelveMonthsAgo)
            .GroupBy(x => new { x.CreatedAt.Year, x.CreatedAt.Month })
            .Select(g => new { g.Key.Year, g.Key.Month, Count = g.Count() })
            .OrderBy(x => x.Year).ThenBy(x => x.Month)
            .ToListAsync();

        var result = new List<TrendDto>();
        for (var d = twelveMonthsAgo; d <= now; d = d.AddMonths(1))
        {
            var monthData = monthlyData.FirstOrDefault(x => x.Year == d.Year && x.Month == d.Month);
            result.Add(new TrendDto
            {
                Date = d.ToString("yyyy-MM"),
                Count = monthData?.Count ?? 0
            });
        }

        return result;
    }

    /// <summary>
    /// 获取各系统故障频次TOP5
    /// </summary>
    public async Task<List<NameValueDto>> GetFaultTop5Async()
    {
        // 按Room统计故障设备数
        var rooms = await _context.Rooms.Where(x => !x.IsDeleted).ToListAsync();
        var result = new List<NameValueDto>();

        foreach (var room in rooms)
        {
            var faultCount = await _context.Equipments
                .CountAsync(x => !x.IsDeleted && x.Status == 1 && x.Cabinet != null && x.Cabinet.RoomId == room.Id);
            if (faultCount > 0)
            {
                result.Add(new NameValueDto { Name = room.Name, Value = faultCount });
            }
        }

        return result.OrderByDescending(x => x.Value).Take(5).ToList();
    }
}
