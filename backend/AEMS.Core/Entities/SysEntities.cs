using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AEMS.Core.Entities;

/// <summary>
/// 系统用户
/// </summary>
[Table("sys_user")]
public class SysUser : BaseEntity
{
    /// <summary>
    /// 用户名
    /// </summary>
    [Required, MaxLength(50)]
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// 密码哈希
    /// </summary>
    [Required, MaxLength(255)]
    public string PasswordHash { get; set; } = string.Empty;

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
    public string? Email { get; set; }

    /// <summary>
    /// 角色ID
    /// </summary>
    public int? RoleId { get; set; }

    /// <summary>
    /// 是否启用
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// 导航属性 - 角色
    /// </summary>
    public SysRole? Role { get; set; }
}

/// <summary>
/// 系统角色
/// </summary>
[Table("sys_role")]
public class SysRole : BaseEntity
{
    /// <summary>
    /// 角色名称
    /// </summary>
    [Required, MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 角色编码
    /// </summary>
    [Required, MaxLength(50)]
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// 备注
    /// </summary>
    [MaxLength(255)]
    public string? Remark { get; set; }

    /// <summary>
    /// 导航属性 - 用户列表
    /// </summary>
    public ICollection<SysUser> Users { get; set; } = new List<SysUser>();
}

/// <summary>
/// 系统日志
/// </summary>
[Table("sys_log")]
public class SysLog : BaseEntity
{
    /// <summary>
    /// 用户ID
    /// </summary>
    public int? UserId { get; set; }

    /// <summary>
    /// 操作类型
    /// </summary>
    [MaxLength(50)]
    public string? Action { get; set; }

    /// <summary>
    /// 操作内容
    /// </summary>
    [MaxLength(500)]
    public string? Content { get; set; }

    /// <summary>
    /// IP地址
    /// </summary>
    [MaxLength(50)]
    public string? IpAddress { get; set; }

    /// <summary>
    /// 请求方法
    /// </summary>
    [MaxLength(10)]
    public string? Method { get; set; }

    /// <summary>
    /// 请求路径
    /// </summary>
    [MaxLength(255)]
    public string? Path { get; set; }

    /// <summary>
    /// 响应状态码
    /// </summary>
    public int? StatusCode { get; set; }

    /// <summary>
    /// 执行耗时（毫秒）
    /// </summary>
    public long? ElapsedMs { get; set; }
}

/// <summary>
/// 数据字典
/// </summary>
[Table("sys_dict")]
public class SysDict : BaseEntity
{
    /// <summary>
    /// 字典类型编码
    /// </summary>
    [Required, MaxLength(50)]
    public string DictType { get; set; } = string.Empty;

    /// <summary>
    /// 字典项名称
    /// </summary>
    [Required, MaxLength(100)]
    public string Label { get; set; } = string.Empty;

    /// <summary>
    /// 字典项值
    /// </summary>
    [Required, MaxLength(100)]
    public string Value { get; set; } = string.Empty;

    /// <summary>
    /// 排序号
    /// </summary>
    public int SortOrder { get; set; } = 0;

    /// <summary>
    /// 是否启用
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// 备注
    /// </summary>
    [MaxLength(255)]
    public string? Remark { get; set; }
}
