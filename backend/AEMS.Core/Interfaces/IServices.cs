using AEMS.Core.DTOs;
using AEMS.Core.Entities;

namespace AEMS.Core.Interfaces;

/// <summary>
/// 用户服务接口
/// </summary>
public interface IUserService
{
    /// <summary>
    /// 获取用户列表（分页+筛选）
    /// </summary>
    Task<PagedResult<UserDto>> GetUserListAsync(UserQueryRequest query);

    /// <summary>
    /// 根据ID获取用户
    /// </summary>
    Task<UserDto?> GetUserByIdAsync(int id);

    /// <summary>
    /// 创建用户
    /// </summary>
    Task<UserDto> CreateUserAsync(UserRequest request);

    /// <summary>
    /// 更新用户
    /// </summary>
    Task<UserDto> UpdateUserAsync(int id, UserRequest request);

    /// <summary>
    /// 删除用户（软删除）
    /// </summary>
    Task DeleteUserAsync(int id);

    /// <summary>
    /// 启用/禁用用户
    /// </summary>
    Task ToggleUserAsync(int id);
}

/// <summary>
/// 操作日志服务接口
/// </summary>
public interface ILogService
{
    /// <summary>
    /// 获取日志列表（分页+筛选）
    /// </summary>
    Task<PagedResult<LogDto>> GetLogListAsync(LogQueryRequest query);

    /// <summary>
    /// 根据ID获取日志详情
    /// </summary>
    Task<LogDto?> GetLogByIdAsync(int id);

    /// <summary>
    /// 写入操作日志
    /// </summary>
    Task AddLogAsync(SysLog log);
}

/// <summary>
/// 角色服务接口
/// </summary>
public interface IRoleService
{
    /// <summary>
    /// 获取所有角色
    /// </summary>
    Task<IEnumerable<SysRole>> GetAllRolesAsync();

    /// <summary>
    /// 根据ID获取角色
    /// </summary>
    Task<SysRole?> GetRoleByIdAsync(int id);

    /// <summary>
    /// 创建角色
    /// </summary>
    Task<SysRole> CreateRoleAsync(SysRole role);

    /// <summary>
    /// 更新角色
    /// </summary>
    Task<SysRole> UpdateRoleAsync(int id, SysRole role);

    /// <summary>
    /// 删除角色
    /// </summary>
    Task DeleteRoleAsync(int id);
}
