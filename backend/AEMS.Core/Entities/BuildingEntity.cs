using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AEMS.Core.Entities;

/// <summary>
/// 楼宇（航站楼/航管楼/综合楼等）
/// </summary>
[Table("building")]
public class Building : BaseEntity
{
    /// <summary>
    /// 楼宇名称
    /// </summary>
    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 楼宇编码
    /// </summary>
    [Required, MaxLength(50)]
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// 位置/地址
    /// </summary>
    [MaxLength(200)]
    public string? Location { get; set; }

    /// <MaxLength(500)]
    public string? Remark { get; set; }

    /// <summary>
    /// 导航属性 - 机房列表
    /// </summary>
    public ICollection<Room> Rooms { get; set; } = new List<Room>();
}
