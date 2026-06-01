using AEMS.Core.DTOs;
using AEMS.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AEMS.Api.Controllers;

/// <summary>
/// 维护任务控制器
/// </summary>
[ApiController]
[Route("api/maintenance/tasks")]
[Authorize]
public class MaintenanceTaskController : ControllerBase
{
    private readonly IMaintenanceTaskService _taskService;

    public MaintenanceTaskController(IMaintenanceTaskService taskService)
    {
        _taskService = taskService;
    }

    /// <summary>
    /// 获取维护任务列表（分页+筛选）
    /// </summary>
    [HttpGet]
    public async Task<ApiResponse<PagedResult<MaintenanceTaskDto>>> GetList([FromQuery] MaintenanceTaskQueryRequest query)
    {
        var result = await _taskService.GetTaskListAsync(query);
        return ApiResponse<PagedResult<MaintenanceTaskDto>>.Success(result);
    }

    /// <summary>
    /// 获取任务统计数据（6个统计卡）
    /// </summary>
    [HttpGet("stats")]
    public async Task<ApiResponse<MaintenanceTaskStatsDto>> GetStats()
    {
        var stats = await _taskService.GetTaskStatsAsync();
        return ApiResponse<MaintenanceTaskStatsDto>.Success(stats);
    }

    /// <summary>
    /// 获取任务详情
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ApiResponse<MaintenanceTaskDto>> GetById(int id)
    {
        var task = await _taskService.GetTaskByIdAsync(id);
        if (task == null)
            return ApiResponse<MaintenanceTaskDto>.Fail(404, "任务不存在");
        return ApiResponse<MaintenanceTaskDto>.Success(task);
    }

    /// <summary>
    /// 手动创建任务
    /// </summary>
    [HttpPost]
    public async Task<ApiResponse<MaintenanceTaskDto>> Create([FromBody] MaintenanceTaskRequest request)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
            return ApiResponse<MaintenanceTaskDto>.Fail(400, string.Join("; ", errors));
        }
        try
        {
            var task = await _taskService.CreateTaskAsync(request);
            return ApiResponse<MaintenanceTaskDto>.Success(task, "创建成功");
        }
        catch (ArgumentException ex)
        {
            return ApiResponse<MaintenanceTaskDto>.Fail(400, ex.Message);
        }
    }

    /// <summary>
    /// 派单
    /// </summary>
    [HttpPut("{id}/dispatch")]
    public async Task<ApiResponse> Dispatch(int id, [FromBody] DispatchRequest request)
    {
        try
        {
            var result = await _taskService.DispatchTaskAsync(id, request);
            if (!result)
                return ApiResponse.Fail(404, "任务不存在");
            return ApiResponse.Success("派单成功");
        }
        catch (InvalidOperationException ex)
        {
            return ApiResponse.Fail(400, ex.Message);
        }
    }

    /// <summary>
    /// 执行任务
    /// </summary>
    [HttpPut("{id}/execute")]
    public async Task<ApiResponse> Execute(int id, [FromBody] ExecuteRequest request)
    {
        try
        {
            var result = await _taskService.ExecuteTaskAsync(id, request);
            if (!result)
                return ApiResponse.Fail(404, "任务不存在");
            return ApiResponse.Success("执行成功");
        }
        catch (InvalidOperationException ex)
        {
            return ApiResponse.Fail(400, ex.Message);
        }
    }

    /// <summary>
    /// 审核任务
    /// </summary>
    [HttpPut("{id}/review")]
    public async Task<ApiResponse> Review(int id, [FromBody] ReviewRequest request)
    {
        try
        {
            var result = await _taskService.ReviewTaskAsync(id, request);
            if (!result)
                return ApiResponse.Fail(404, "任务不存在");
            return ApiResponse.Success("审核成功");
        }
        catch (InvalidOperationException ex)
        {
            return ApiResponse.Fail(400, ex.Message);
        }
    }
}
