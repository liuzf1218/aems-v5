using AEMS.Core.DTOs;
using AEMS.Core.Entities;
using AEMS.Core.Interfaces;
using AEMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AEMS.Infrastructure.Repositories;

/// <summary>
/// 用户服务实现
/// </summary>
public class UserService : IUserService
{
    private readonly AemsDbContext _context;

    public UserService(AemsDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<UserDto>> GetUserListAsync(UserQueryRequest query)
    {
        var queryable = _context.SysUsers
            .Include(u => u.Role)
            .Where(u => !u.IsDeleted)
            .AsQueryable();

        // 关键字筛选：用户名、真实姓名
        if (!string.IsNullOrEmpty(query.Keyword))
        {
            var keyword = query.Keyword.Trim();
            queryable = queryable.Where(u =>
                u.Username.Contains(keyword) ||
                (u.RealName != null && u.RealName.Contains(keyword)));
        }

        // 角色筛选
        if (query.RoleId.HasValue)
        {
            queryable = queryable.Where(u => u.RoleId == query.RoleId.Value);
        }

        // 状态筛选
        if (query.IsActive.HasValue)
        {
            queryable = queryable.Where(u => u.IsActive == query.IsActive.Value);
        }

        var total = await queryable.CountAsync();
        var items = await queryable
            .OrderByDescending(u => u.CreatedAt)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(u => MapToDto(u))
            .ToListAsync();

        return new PagedResult<UserDto>
        {
            Items = items,
            Total = total,
            Page = query.Page,
            PageSize = query.PageSize
        };
    }

    public async Task<UserDto?> GetUserByIdAsync(int id)
    {
        var user = await _context.SysUsers
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Id == id && !u.IsDeleted);

        return user == null ? null : MapToDto(user);
    }

    public async Task<UserDto> CreateUserAsync(UserRequest request)
    {
        // 检查用户名是否已存在
        var exists = await _context.SysUsers
            .AnyAsync(u => u.Username == request.Username && !u.IsDeleted);
        if (exists)
        {
            throw new ArgumentException("用户名已存在");
        }

        var user = new SysUser
        {
            Username = request.Username,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password ?? "123456"),
            RealName = request.RealName,
            Phone = request.Phone,
            Email = request.Email,
            RoleId = request.RoleId,
            IsActive = request.IsActive
        };

        _context.SysUsers.Add(user);
        await _context.SaveChangesAsync();

        // 重新查询获取关联数据
        var created = await _context.SysUsers
            .Include(u => u.Role)
            .FirstAsync(u => u.Id == user.Id);

        return MapToDto(created);
    }

    public async Task<UserDto> UpdateUserAsync(int id, UserRequest request)
    {
        var user = await _context.SysUsers
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Id == id && !u.IsDeleted);

        if (user == null)
        {
            throw new KeyNotFoundException("用户不存在");
        }

        // 检查用户名是否与其他用户冲突
        var nameExists = await _context.SysUsers
            .AnyAsync(u => u.Username == request.Username && u.Id != id && !u.IsDeleted);
        if (nameExists)
        {
            throw new ArgumentException("用户名已存在");
        }

        user.Username = request.Username;
        user.RealName = request.RealName;
        user.Phone = request.Phone;
        user.Email = request.Email;
        user.RoleId = request.RoleId;
        user.IsActive = request.IsActive;

        // 如果提供了新密码，则更新密码
        if (!string.IsNullOrEmpty(request.Password))
        {
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
        }

        await _context.SaveChangesAsync();

        // 重新查询获取最新数据
        var updated = await _context.SysUsers
            .Include(u => u.Role)
            .FirstAsync(u => u.Id == id);

        return MapToDto(updated);
    }

    public async Task DeleteUserAsync(int id)
    {
        var user = await _context.SysUsers.FindAsync(id);
        if (user == null)
        {
            throw new KeyNotFoundException("用户不存在");
        }

        user.IsDeleted = true;
        await _context.SaveChangesAsync();
    }

    public async Task ToggleUserAsync(int id)
    {
        var user = await _context.SysUsers.FindAsync(id);
        if (user == null)
        {
            throw new KeyNotFoundException("用户不存在");
        }

        user.IsActive = !user.IsActive;
        await _context.SaveChangesAsync();
    }

    private static UserDto MapToDto(SysUser user)
    {
        return new UserDto
        {
            Id = user.Id,
            Username = user.Username,
            RealName = user.RealName,
            Phone = user.Phone,
            Email = user.Email,
            RoleId = user.RoleId,
            RoleName = user.Role?.Name,
            IsActive = user.IsActive,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt
        };
    }
}

/// <summary>
/// 操作日志服务实现
/// </summary>
public class LogService : ILogService
{
    private readonly AemsDbContext _context;

    public LogService(AemsDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<LogDto>> GetLogListAsync(LogQueryRequest query)
    {
        var queryable = _context.SysLogs.AsQueryable();

        // 用户名筛选
        if (!string.IsNullOrEmpty(query.Username))
        {
            queryable = queryable.Where(l => l.UserId.ToString()!.Contains(query.Username));
        }

        // 操作类型筛选
        if (!string.IsNullOrEmpty(query.Action))
        {
            queryable = queryable.Where(l => l.Action == query.Action);
        }

        // 时间范围筛选
        if (query.StartTime.HasValue)
        {
            queryable = queryable.Where(l => l.CreatedAt >= query.StartTime.Value);
        }

        if (query.EndTime.HasValue)
        {
            queryable = queryable.Where(l => l.CreatedAt <= query.EndTime.Value);
        }

        var total = await queryable.CountAsync();
        var items = await queryable
            .OrderByDescending(l => l.CreatedAt)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(l => new LogDto
            {
                Id = l.Id,
                UserId = l.UserId,
                Action = l.Action,
                Content = l.Content,
                IpAddress = l.IpAddress,
                Method = l.Method,
                Path = l.Path,
                StatusCode = l.StatusCode,
                ElapsedMs = l.ElapsedMs,
                CreatedAt = l.CreatedAt
            })
            .ToListAsync();

        return new PagedResult<LogDto>
        {
            Items = items,
            Total = total,
            Page = query.Page,
            PageSize = query.PageSize
        };
    }

    public async Task<LogDto?> GetLogByIdAsync(int id)
    {
        var log = await _context.SysLogs.FindAsync(id);
        if (log == null) return null;

        return new LogDto
        {
            Id = log.Id,
            UserId = log.UserId,
            Action = log.Action,
            Content = log.Content,
            IpAddress = log.IpAddress,
            Method = log.Method,
            Path = log.Path,
            StatusCode = log.StatusCode,
            ElapsedMs = log.ElapsedMs,
            CreatedAt = log.CreatedAt
        };
    }

    public async Task AddLogAsync(SysLog log)
    {
        _context.SysLogs.Add(log);
        await _context.SaveChangesAsync();
    }
}

/// <summary>
/// 角色服务实现
/// </summary>
public class RoleService : IRoleService
{
    private readonly AemsDbContext _context;

    public RoleService(AemsDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<SysRole>> GetAllRolesAsync()
    {
        return await _context.SysRoles
            .Where(r => !r.IsDeleted)
            .OrderBy(r => r.Name)
            .ToListAsync();
    }

    public async Task<SysRole?> GetRoleByIdAsync(int id)
    {
        return await _context.SysRoles
            .FirstOrDefaultAsync(r => r.Id == id && !r.IsDeleted);
    }

    public async Task<SysRole> CreateRoleAsync(SysRole role)
    {
        // 检查编码是否已存在
        var exists = await _context.SysRoles
            .AnyAsync(r => r.Code == role.Code && !r.IsDeleted);
        if (exists)
        {
            throw new ArgumentException("角色编码已存在");
        }

        _context.SysRoles.Add(role);
        await _context.SaveChangesAsync();
        return role;
    }

    public async Task<SysRole> UpdateRoleAsync(int id, SysRole role)
    {
        var existing = await _context.SysRoles
            .FirstOrDefaultAsync(r => r.Id == id && !r.IsDeleted);

        if (existing == null)
        {
            throw new KeyNotFoundException("角色不存在");
        }

        // 检查编码是否与其他角色冲突
        var codeExists = await _context.SysRoles
            .AnyAsync(r => r.Code == role.Code && r.Id != id && !r.IsDeleted);
        if (codeExists)
        {
            throw new ArgumentException("角色编码已存在");
        }

        existing.Name = role.Name;
        existing.Code = role.Code;
        existing.Remark = role.Remark;

        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task DeleteRoleAsync(int id)
    {
        var role = await _context.SysRoles.FindAsync(id);
        if (role == null)
        {
            throw new KeyNotFoundException("角色不存在");
        }

        // 检查是否有用户关联此角色
        var hasUsers = await _context.SysUsers
            .AnyAsync(u => u.RoleId == id && !u.IsDeleted);
        if (hasUsers)
        {
            throw new ArgumentException("该角色下仍有用户，无法删除");
        }

        role.IsDeleted = true;
        await _context.SaveChangesAsync();
    }
}
