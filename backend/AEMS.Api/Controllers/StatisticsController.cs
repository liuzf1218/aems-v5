using AEMS.Core.DTOs;
using AEMS.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AEMS.Api.Controllers;

/// <summary>
/// 统计报表控制器
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class StatisticsController : ControllerBase
{
    private readonly IStatisticsService _statisticsService;

    public StatisticsController(IStatisticsService statisticsService)
    {
        _statisticsService = statisticsService;
    }

    /// <summary>
    /// 获取仪表盘数据（4个指标卡 + 系统状态 + 告警列表 + 3个图表数据）
    /// </summary>
    [HttpGet("dashboard")]
    public async Task<IActionResult> GetDashboard()
    {
        var data = await _statisticsService.GetDashboardAsync();
        return Ok(ApiResponse<DashboardDto>.Success(data));
    }

    /// <summary>
    /// 获取设备统计（类型分布 / 状态分布 / 系统设备数）
    /// </summary>
    [HttpGet("device")]
    public async Task<IActionResult> GetDeviceStatistics()
    {
        var data = await _statisticsService.GetDeviceStatisticsAsync();
        return Ok(ApiResponse<DeviceStatisticsDto>.Success(data));
    }

    /// <summary>
    /// 获取工单统计（月度趋势 / 类型分布 / 故障等级 / TOP5）
    /// </summary>
    [HttpGet("workorder")]
    public async Task<IActionResult> GetWorkorderStatistics()
    {
        var data = await _statisticsService.GetWorkorderStatisticsAsync();
        return Ok(ApiResponse<WorkorderStatisticsDto>.Success(data));
    }

    /// <summary>
    /// 获取设备增长趋势（按月）
    /// </summary>
    [HttpGet("device/trend")]
    public async Task<IActionResult> GetDeviceTrend()
    {
        var data = await _statisticsService.GetDeviceTrendAsync();
        return Ok(ApiResponse<List<TrendDto>>.Success(data));
    }

    /// <summary>
    /// 获取各系统故障频次TOP5
    /// </summary>
    [HttpGet("fault/top5")]
    public async Task<IActionResult> GetFaultTop5()
    {
        var data = await _statisticsService.GetFaultTop5Async();
        return Ok(ApiResponse<List<NameValueDto>>.Success(data));
    }
}
