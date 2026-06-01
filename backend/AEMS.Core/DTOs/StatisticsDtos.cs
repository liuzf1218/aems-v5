namespace AEMS.Core.DTOs;

/// <summary>
/// 仪表盘数据
/// </summary>
public class DashboardDto
{
    /// <summary>
    /// 设备总数
    /// </summary>
    public int EquipmentTotal { get; set; }

    /// <summary>
    /// 在线率（百分比）
    /// </summary>
    public decimal OnlineRate { get; set; }

    /// <summary>
    /// 待处理工单数
    /// </summary>
    public int PendingWorkorders { get; set; }

    /// <summary>
    /// 活跃告警数
    /// </summary>
    public int ActiveAlerts { get; set; }

    /// <summary>
    /// 系统状态列表
    /// </summary>
    public List<SystemStatusDto> SystemStatus { get; set; } = new();

    /// <summary>
    /// 最新告警列表
    /// </summary>
    public List<AlertDto> RecentAlerts { get; set; } = new();

    /// <summary>
    /// 设备状态分布（饼图）
    /// </summary>
    public List<NameValueDto> EquipmentStatusDistribution { get; set; } = new();

    /// <summary>
    /// 近30天工单趋势（折线图）
    /// </summary>
    public List<TrendDto> WorkorderTrend { get; set; } = new();

    /// <summary>
    /// 系统设备数分布（柱状图）
    /// </summary>
    public List<NameValueDto> SystemEquipmentCount { get; set; } = new();
}

/// <summary>
/// 系统状态
/// </summary>
public class SystemStatusDto
{
    public string Name { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty; // normal / warning / error
    public string Label { get; set; } = string.Empty;
}

/// <summary>
/// 告警项
/// </summary>
public class AlertDto
{
    public string Level { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Time { get; set; } = string.Empty;
}

/// <summary>
/// 名称-值对
/// </summary>
public class NameValueDto
{
    public string Name { get; set; } = string.Empty;
    public int Value { get; set; }
}

/// <summary>
/// 趋势数据
/// </summary>
public class TrendDto
{
    public string Date { get; set; } = string.Empty;
    public int Count { get; set; }
}

/// <summary>
/// 设备统计数据
/// </summary>
public class DeviceStatisticsDto
{
    /// <summary>
    /// 设备类型分布
    /// </summary>
    public List<NameValueDto> TypeDistribution { get; set; } = new();

    /// <summary>
    /// 设备状态分布
    /// </summary>
    public List<NameValueDto> StatusDistribution { get; set; } = new();

    /// <summary>
    /// 各系统设备数
    /// </summary>
    public List<NameValueDto> SystemEquipmentCount { get; set; } = new();

    /// <summary>
    /// 设备总数
    /// </summary>
    public int Total { get; set; }
}

/// <summary>
/// 工单统计数据
/// </summary>
public class WorkorderStatisticsDto
{
    /// <summary>
    /// 月度趋势
    /// </summary>
    public List<TrendDto> MonthlyTrend { get; set; } = new();

    /// <summary>
    /// 工单类型分布
    /// </summary>
    public List<NameValueDto> TypeDistribution { get; set; } = new();

    /// <summary>
    /// 故障等级分布
    /// </summary>
    public List<NameValueDto> PriorityDistribution { get; set; } = new();

    /// <summary>
    /// 故障频次TOP5设备
    /// </summary>
    public List<NameValueDto> FaultTop5 { get; set; } = new();

    /// <summary>
    /// 工单总数
    /// </summary>
    public int Total { get; set; }
}
