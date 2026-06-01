using System.Security.Claims;
using AEMS.Core.DTOs;
using AEMS.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AEMS.Api.Controllers;

/// <summary>
/// 认证控制器
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// 用户登录
    /// </summary>
    /// <param name="request">登录请求</param>
    /// <returns>登录响应（包含JWT令牌）</returns>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ApiResponse<LoginResponse>> Login([FromBody] LoginRequest request)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage);
            return ApiResponse<LoginResponse>.Fail(400, string.Join("; ", errors));
        }

        try
        {
            var result = await _authService.LoginAsync(request);
            return ApiResponse<LoginResponse>.Success(result, "登录成功");
        }
        catch (UnauthorizedAccessException ex)
        {
            return ApiResponse<LoginResponse>.Fail(401, ex.Message);
        }
    }

    /// <summary>
    /// 用户登出
    /// </summary>
    /// <returns>操作结果</returns>
    [HttpPost("logout")]
    [Authorize]
    public async Task<ApiResponse> Logout()
    {
        var token = HttpContext.Request.Headers["Authorization"]
            .ToString().Replace("Bearer ", "");

        await _authService.LogoutAsync(token);
        return ApiResponse.Success("登出成功");
    }

    /// <summary>
    /// 获取当前登录用户信息
    /// </summary>
    /// <returns>用户信息</returns>
    [HttpGet("current")]
    [Authorize]
    public async Task<ApiResponse<UserInfo>> GetCurrentUser()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
        {
            return ApiResponse<UserInfo>.Fail(401, "用户未登录");
        }

        var userInfo = await _authService.GetCurrentUserAsync(userId);
        if (userInfo == null)
        {
            return ApiResponse<UserInfo>.Fail(401, "用户不存在或已禁用");
        }

        return ApiResponse<UserInfo>.Success(userInfo);
    }
}
