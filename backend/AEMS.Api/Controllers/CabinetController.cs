using AEMS.Core.DTOs;
using AEMS.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AEMS.Api.Controllers;

/// <summary>
/// 机柜管理控制器
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CabinetController : ControllerBase
{
    private readonly ICabinetService _cabinetService;

    public CabinetController(ICabinetService cabinetService)
    {
        _cabinetService = cabinetService;
    }

    /// <summary>
    /// 获取机柜列表（分页+筛选）
    /// </summary>
    [HttpGet]
    public async Task<ApiResponse<PagedResult<CabinetListItemDto>>> GetList([FromQuery] CabinetQueryRequest request)
    {
        var result = await _cabinetService.GetCabinetListAsync(request);
        return ApiResponse<PagedResult<CabinetListItemDto>>.Success(result);
    }

    /// <summary>
    /// 获取机柜详情
    /// </summary>
    /// <param name="id">机柜ID</param>
    [HttpGet("{id}")]
    public async Task<ApiResponse<CabinetDetailDto>> GetById(int id)
    {
        var result = await _cabinetService.GetCabinetDetailAsync(id);
        if (result == null)
        {
            return ApiResponse<CabinetDetailDto>.Fail(404, "机柜不存在");
        }
        return ApiResponse<CabinetDetailDto>.Success(result);
    }

    /// <summary>
    /// 新增机柜
    /// </summary>
    [HttpPost]
    public async Task<ApiResponse<CabinetDetailDto>> Create([FromBody] CabinetRequest request)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage);
            return ApiResponse<CabinetDetailDto>.Fail(400, string.Join("; ", errors));
        }

        try
        {
            var result = await _cabinetService.CreateCabinetAsync(request);
            return ApiResponse<CabinetDetailDto>.Success(result, "创建成功");
        }
        catch (InvalidOperationException ex)
        {
            return ApiResponse<CabinetDetailDto>.Fail(400, ex.Message);
        }
    }

    /// <summary>
    /// 更新机柜
    /// </summary>
    /// <param name="id">机柜ID</param>
    [HttpPut("{id}")]
    public async Task<ApiResponse<CabinetDetailDto>> Update(int id, [FromBody] CabinetRequest request)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage);
            return ApiResponse<CabinetDetailDto>.Fail(400, string.Join("; ", errors));
        }

        try
        {
            var result = await _cabinetService.UpdateCabinetAsync(id, request);
            if (result == null)
            {
                return ApiResponse<CabinetDetailDto>.Fail(404, "机柜不存在");
            }
            return ApiResponse<CabinetDetailDto>.Success(result, "更新成功");
        }
        catch (InvalidOperationException ex)
        {
            return ApiResponse<CabinetDetailDto>.Fail(400, ex.Message);
        }
    }

    /// <summary>
    /// 删除机柜
    /// </summary>
    /// <param name="id">机柜ID</param>
    [HttpDelete("{id}")]
    public async Task<ApiResponse> Delete(int id)
    {
        try
        {
            var result = await _cabinetService.DeleteCabinetAsync(id);
            if (!result)
            {
                return ApiResponse.Fail(404, "机柜不存在");
            }
            return ApiResponse.Success("删除成功");
        }
        catch (InvalidOperationException ex)
        {
            return ApiResponse.Fail(400, ex.Message);
        }
    }
}
