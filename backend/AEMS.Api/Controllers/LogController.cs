using AEMS.Core.DTOs;
using AEMS.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AEMS.Api.Controllers;

/// <summary>
/// 操作日志控制器
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class LogController : ControllerBase
{
    private readonly ILogService _logService;

    public LogController(ILogService logService)
    {
        _logService = logService;
    }

    /// <summary>
    /// 获取日志列表（分页+筛选）
    /// </summary>
    [HttpGet]
    public async Task<ApiResponse<PagedResult<LogDto>>> GetList([FromQuery] LogQueryRequest query)
    {
        var result = await _logService.GetLogListAsync(query);
        return ApiResponse<PagedResult<LogDto>>.Success(result);
    }

    /// <summary>
    /// 获取日志详情
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ApiResponse<LogDto>> GetById(int id)
    {
        var log = await _logService.GetLogByIdAsync(id);
        if (log == null)
        {
            return ApiResponse<LogDto>.Fail(404, "日志不存在");
        }
        return ApiResponse<LogDto>.Success(log);
    }
}
