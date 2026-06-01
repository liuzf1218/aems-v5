using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AEMS.Core.DTOs;
using AEMS.Core.Entities;
using AEMS.Core.Interfaces;
using AEMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace AEMS.Infrastructure.Repositories;

/// <summary>
/// 认证服务实现
/// </summary>
public class AuthService : IAuthService
{
    private readonly AemsDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthService(AemsDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    /// <summary>
    /// 用户登录
    /// </summary>
    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        // 查找用户
        var user = await _context.SysUsers
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Username == request.Username && u.IsActive);

        if (user == null)
        {
            throw new UnauthorizedAccessException("用户名或密码错误");
        }

        // 验证密码（这里使用简单比较，生产环境应使用BCrypt等哈希算法）
        if (!VerifyPassword(request.Password, user.PasswordHash))
        {
            throw new UnauthorizedAccessException("用户名或密码错误");
        }

        // 生成JWT令牌
        var token = GenerateJwtToken(user);
        var expireMinutes = int.Parse(_configuration["Jwt:ExpireMinutes"] ?? "120");

        return new LoginResponse
        {
            Token = token,
            ExpireAt = DateTime.Now.AddMinutes(expireMinutes),
            UserInfo = new UserInfo
            {
                Id = user.Id,
                Username = user.Username,
                RealName = user.RealName ?? string.Empty,
                Role = user.Role?.Code ?? string.Empty
            }
        };
    }

    /// <summary>
    /// 用户登出
    /// </summary>
    public Task LogoutAsync(string token)
    {
        // JWT是无状态的，客户端删除token即可
        // 如需服务端控制，可维护黑名单（Redis等）
        return Task.CompletedTask;
    }

    /// <summary>
    /// 获取当前用户信息
    /// </summary>
    public async Task<UserInfo?> GetCurrentUserAsync(int userId)
    {
        var user = await _context.SysUsers
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Id == userId && u.IsActive);

        if (user == null) return null;

        return new UserInfo
        {
            Id = user.Id,
            Username = user.Username,
            RealName = user.RealName ?? string.Empty,
            Role = user.Role?.Code ?? string.Empty
        };
    }

    /// <summary>
    /// 验证令牌是否有效
    /// </summary>
    public Task<bool> ValidateTokenAsync(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"] ?? "AEMS_Default_Secret_Key_At_Least_32_Characters!");
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _configuration["Jwt:Issuer"] ?? "AEMS",
                ValidateAudience = true,
                ValidAudience = _configuration["Jwt:Audience"] ?? "AEMS",
                ClockSkew = TimeSpan.Zero
            }, out _);
            return Task.FromResult(true);
        }
        catch
        {
            return Task.FromResult(false);
        }
    }

    /// <summary>
    /// 生成JWT令牌
    /// </summary>
    private string GenerateJwtToken(SysUser user)
    {
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"] ?? "AEMS_Default_Secret_Key_At_Least_32_Characters!"));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role?.Code ?? "user")
        };

        var expireMinutes = int.Parse(_configuration["Jwt:ExpireMinutes"] ?? "120");
        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"] ?? "AEMS",
            audience: _configuration["Jwt:Audience"] ?? "AEMS",
            claims: claims,
            expires: DateTime.Now.AddMinutes(expireMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    /// <summary>
    /// 验证密码（使用 BCrypt）
    /// </summary>
    private static bool VerifyPassword(string password, string passwordHash)
    {
        return BCrypt.Net.BCrypt.Verify(password, passwordHash);
    }
}
