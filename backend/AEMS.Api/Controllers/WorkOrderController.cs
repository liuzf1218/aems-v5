using System.Security.Claims;
using AEMS.Core.DTOs;
using AEMS.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AEMS.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class WorkOrderController : ControllerBase
{
    private readonly IWorkOrderService _workOrderService;

    public WorkOrderController(IWorkOrderService workOrderService)
    {
        _workOrderService = workOrderService;
    }

    /// <summary>
    /// 获取工单列表（分页+筛选）
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetList([FromQuery] WorkOrderQueryRequest query)
    {
        var result = await _workOrderService.GetListAsync(query);
        return Ok(ApiResponse<object>.Success(result));
    }

    /// <summary>
    /// 获取工单详情
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _workOrderService.GetByIdAsync(id);
        if (result == null) return NotFound(ApiResponse.Fail(404, "工单不存在"));
        return Ok(ApiResponse<object>.Success(result));
    }

    /// <summary>
    /// 创建工单
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateWorkOrderRequest request)
    {
        var userId = GetCurrentUserId();
        var result = await _workOrderService.CreateAsync(request, userId);
        return Ok(ApiResponse<object>.Success(result, "创建成功"));
    }

    /// <summary>
    /// 受理工单
    /// </summary>
    [HttpPut("{id}/accept")]
    public async Task<IActionResult> Accept(int id)
    {
        var userId = GetCurrentUserId();
        var success = await _workOrderService.AcceptAsync(id, userId);
        if (!success) return Ok(ApiResponse.Fail(400, "工单不存在或当前状态不允许受理"));
        return Ok(ApiResponse.Success("受理成功"));
    }

    /// <summary>
    /// 转派工单
    /// </summary>
    [HttpPut("{id}/assign")]
    public async Task<IActionResult> Assign(int id, [FromBody] AssignWorkOrderRequest request)
    {
        var userId = GetCurrentUserId();
        var success = await _workOrderService.AssignAsync(id, request, userId);
        if (!success) return Ok(ApiResponse.Fail(400, "工单不存在或当前状态不允许转派"));
        return Ok(ApiResponse.Success("转派成功"));
    }

    /// <summary>
    /// 提交完成工单
    /// </summary>
    [HttpPut("{id}/complete")]
    public async Task<IActionResult> Complete(int id, [FromBody] CompleteWorkOrderRequest request)
    {
        var userId = GetCurrentUserId();
        var success = await _workOrderService.CompleteAsync(id, request, userId);
        if (!success) return Ok(ApiResponse.Fail(400, "工单不存在或当前状态不允许提交完成"));
        return Ok(ApiResponse.Success("提交成功，等待验收"));
    }

    /// <summary>
    /// 工单状态推进（待受理→处理中→待验收→已完成）
    /// </summary>
    [HttpPost("{id}/process")]
    public async Task<IActionResult> Process(int id)
    {
        var userId = GetCurrentUserId();
        var success = await _workOrderService.ProcessAsync(id, userId);
        if (!success) return Ok(ApiResponse.Fail(400, "工单不存在或当前状态不允许推进"));
        return Ok(ApiResponse.Success("状态推进成功"));
    }

    /// <summary>
    /// 取消工单
    /// </summary>
    [HttpPut("{id}/cancel")]
    public async Task<IActionResult> Cancel(int id)
    {
        var userId = GetCurrentUserId();
        var success = await _workOrderService.CancelAsync(id, userId);
        if (!success) return Ok(ApiResponse.Fail(400, "工单不存在或当前状态不允许取消"));
        return Ok(ApiResponse.Success("取消成功"));
    }

    /// <summary>
    /// 获取工单操作日志时间线
    /// </summary>
    [HttpGet("{id}/logs")]
    public async Task<IActionResult> GetLogs(int id)
    {
        var logs = await _workOrderService.GetLogsAsync(id);
        return Ok(ApiResponse<object>.Success(logs));
    }

    /// <summary>
    /// 获取SLA信息+倒计时
    /// </summary>
    [HttpGet("{id}/sla")]
    public async Task<IActionResult> GetSla(int id)
    {
        var sla = await _workOrderService.GetSlaInfoAsync(id);
        if (sla == null) return NotFound(ApiResponse.Fail(404, "工单不存在"));
        return Ok(ApiResponse<object>.Success(sla));
    }

    /// <summary>
    /// 获取故障类型列表（创建工单时选择）
    /// </summary>
    [HttpGet("systems")]
    public async Task<IActionResult> GetSystems()
    {
        var result = await _workOrderService.GetSystemsAsync();
        return Ok(ApiResponse<object>.Success(result));
    }

    /// <summary>
    /// 获取设备列表（创建工单时选择）
    /// </summary>
    [HttpGet("devices")]
    public async Task<IActionResult> GetDevices([FromQuery] string? keyword)
    {
        var result = await _workOrderService.GetDevicesAsync(keyword);
        return Ok(ApiResponse<object>.Success(result));
    }

    private int GetCurrentUserId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("sub");
        return claim != null ? int.Parse(claim.Value) : 0;
    }
}
