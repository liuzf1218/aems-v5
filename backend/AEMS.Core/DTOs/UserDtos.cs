using System.ComponentModel.DataAnnotations;

namespace AEMS.Core.DTOs;

/// <summary>
/// 用户查询参数
/// </summary>
public class UserQueryRequest : PagedRequest
{
    /// <summary>
    /// 用户名/姓名关键字
    /// </summary>
    public string? Keyword { get; set; }

    /// <summary>
    /// 角色ID筛选
    /// </summary>
    public int? RoleId { get; set; }

    /// <summary>
    /// 状态筛选：true=启用，false=禁用，null=全部
    /// </summary>
    public bool? IsActive { get; set; }
}

/// <summary>
/// 创建/编辑用户请求
/// </summary>
public class UserRequest
{
    /// <summary>
    /// 用户名
    /// </summary>
    [Required(ErrorMessage = "用户名不能为空")]
    [MaxLength(50, ErrorMessage = "用户名不能超过50个字符")]
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// 密码（新增时必填，编辑时可选）
    /// </summary>
    [MaxLength(255)]
    public string? Password { get; set; }

    /// <summary>
    /// 真实姓名
    /// </summary>
    [MaxLength(50)]
    public string? RealName { get; set; }

    /// <summary>
    /// 手机号
    /// </summary>
    [MaxLength(20)]
    public string? Phone { get; set; }

    /// <summary>
    /// 邮箱
    /// </summary>
    [MaxLength(100)]
    [EmailAddress(ErrorMessage = "邮箱格式不正确")]
    public string? Email { get; set; }

    /// <summary>
    /// 角色ID
    /// </summary>
    public int? RoleId { get; set; }

    /// <summary>
    /// 是否启用
    /// </summary>
    public bool IsActive { get; set; } = true;
}

/// <summary>
/// 用户列表响应DTO（不含密码）
/// </summary>
public class UserDto
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string? RealName { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public int? RoleId { get; set; }
    public string? RoleName { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
