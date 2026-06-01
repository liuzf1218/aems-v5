using AEMS.Core.DTOs;

namespace AEMS.Core.Interfaces;

/// <summary>
/// 统计报表服务接口
/// </summary>
public interface IStatisticsService
{
    /// <summary>
    /// 获取仪表盘数据
    /// </summary>
    Task<DashboardDto> GetDashboardAsync();

    /// <summary>
    /// 获取设备统计数据
    /// </summary>
    Task<DeviceStatisticsDto> GetDeviceStatisticsAsync();

    /// <summary>
    /// 获取工单统计数据
    /// </summary>
    Task<WorkorderStatisticsDto> GetWorkorderStatisticsAsync();

    /// <summary>
    /// 获取设备增长趋势
    /// </summary>
    Task<List<TrendDto>> GetDeviceTrendAsync();

    /// <summary>
    /// 获取各系统故障频次TOP5
    /// </summary>
    Task<List<NameValueDto>> GetFaultTop5Async();
}
