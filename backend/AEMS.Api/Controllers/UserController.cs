using System.Security.Claims;
using AEMS.Core.DTOs;
using AEMS.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AEMS.Api.Controllers;

/// <summary>
/// 用户管理控制器
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    /// <summary>
    /// 获取用户列表（分页+筛选）
    /// </summary>
    [HttpGet]
    public async Task<ApiResponse<PagedResult<UserDto>>> GetList([FromQuery] UserQueryRequest query)
    {
        var result = await _userService.GetUserListAsync(query);
        return ApiResponse<PagedResult<UserDto>>.Success(result);
    }

    /// <summary>
    /// 获取用户详情
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ApiResponse<UserDto>> GetById(int id)
    {
        var user = await _userService.GetUserByIdAsync(id);
        if (user == null)
        {
            return ApiResponse<UserDto>.Fail(404, "用户不存在");
        }
        return ApiResponse<UserDto>.Success(user);
    }

    /// <summary>
    /// 创建用户
    /// </summary>
    [HttpPost]
    public async Task<ApiResponse<UserDto>> Create([FromBody] UserRequest request)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage);
            return ApiResponse<UserDto>.Fail(400, string.Join("; ", errors));
        }

        try
        {
            var user = await _userService.CreateUserAsync(request);
            return ApiResponse<UserDto>.Success(user, "创建成功");
        }
        catch (ArgumentException ex)
        {
            return ApiResponse<UserDto>.Fail(400, ex.Message);
        }
    }

    /// <summary>
    /// 编辑用户
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ApiResponse<UserDto>> Update(int id, [FromBody] UserRequest request)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage);
            return ApiResponse<UserDto>.Fail(400, string.Join("; ", errors));
        }

        try
        {
            var user = await _userService.UpdateUserAsync(id, request);
            return ApiResponse<UserDto>.Success(user, "更新成功");
        }
        catch (KeyNotFoundException ex)
        {
            return ApiResponse<UserDto>.Fail(404, ex.Message);
        }
        catch (ArgumentException ex)
        {
            return ApiResponse<UserDto>.Fail(400, ex.Message);
        }
    }

    /// <summary>
    /// 删除用户（软删除）
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ApiResponse> Delete(int id)
    {
        try
        {
            await _userService.DeleteUserAsync(id);
            return ApiResponse.Success("删除成功");
        }
        catch (KeyNotFoundException ex)
        {
            return ApiResponse.Fail(404, ex.Message);
        }
    }

    /// <summary>
    /// 启用/禁用用户
    /// </summary>
    [HttpPut("{id}/toggle")]
    public async Task<ApiResponse> Toggle(int id)
    {
        try
        {
            await _userService.ToggleUserAsync(id);
            return ApiResponse.Success("操作成功");
        }
        catch (KeyNotFoundException ex)
        {
            return ApiResponse.Fail(404, ex.Message);
        }
    }
}
