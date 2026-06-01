using AEMS.Core.DTOs;

namespace AEMS.Core.Interfaces;

/// <summary>
/// 认证服务接口
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// 用户登录
    /// </summary>
    Task<LoginResponse> LoginAsync(LoginRequest request);

    /// <summary>
    /// 用户登出
    /// </summary>
    Task LogoutAsync(string token);

    /// <summary>
    /// 获取当前用户信息
    /// </summary>
    Task<UserInfo?> GetCurrentUserAsync(int userId);

    /// <summary>
    /// 验证令牌是否有效
    /// </summary>
    Task<bool> ValidateTokenAsync(string token);
}
