using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AEMS.Core.Entities;

/// <summary>
/// 软件
/// </summary>
[Table("software")]
public class Software : BaseEntity
{
    /// <summary>
    /// 软件名称
    /// </summary>
    [Required, MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 软件编码
    /// </summary>
    [Required, MaxLength(50)]
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// 厂商
    /// </summary>
    [MaxLength(100)]
    public string? Vendor { get; set; }

    /// <summary>
    /// 软件类型
    /// </summary>
    [MaxLength(50)]
    public string? SoftwareType { get; set; }

    /// <summary>
    /// 授权类型
    /// </summary>
    [MaxLength(50)]
    public string? LicenseType { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    [MaxLength(500)]
    public string? Remark { get; set; }

    /// <summary>
    /// 部署设备ID
    /// </summary>
    public int? EquipmentId { get; set; }

    /// <summary>
    /// 导航属性 - 设备
    /// </summary>
    public Equipment? Equipment { get; set; }

    /// <summary>
    /// 导航属性 - 版本列表
    /// </summary>
    public ICollection<SoftwareVersion> Versions { get; set; } = new List<SoftwareVersion>();
}

/// <summary>
/// 软件版本
/// </summary>
[Table("software_version")]
public class SoftwareVersion : BaseEntity
{
    /// <summary>
    /// 软件ID
    /// </summary>
    public int SoftwareId { get; set; }

    /// <summary>
    /// 版本号
    /// </summary>
    [Required, MaxLength(50)]
    public string Version { get; set; } = string.Empty;

    /// <summary>
    /// 发布日期
    /// </summary>
    public DateTime? ReleaseDate { get; set; }

    /// <summary>
    /// 更新说明
    /// </summary>
    [MaxLength(1000)]
    public string? ChangeLog { get; set; }

    /// <summary>
    /// 安装包路径
    /// </summary>
    [MaxLength(500)]
    public string? PackagePath { get; set; }

    /// <summary>
    /// 导航属性 - 软件
    /// </summary>
    public Software Software { get; set; } = null!;
}

/// <summary>
/// 软件实例（部署在设备上的软件）
/// </summary>
[Table("software_instance")]
public class SoftwareInstance : BaseEntity
{
    /// <summary>
    /// 软件版本ID
    /// </summary>
    public int SoftwareVersionId { get; set; }

    /// <summary>
    /// 设备ID
    /// </summary>
    public int EquipmentId { get; set; }

    /// <summary>
    /// 安装路径
    /// </summary>
    [MaxLength(500)]
    public string? InstallPath { get; set; }

    /// <summary>
    /// 安装日期
    /// </summary>
    public DateTime? InstallDate { get; set; }

    /// <summary>
    /// 运行状态：0-停止 1-运行中 2-异常
    /// </summary>
    public int Status { get; set; } = 1;

    /// <summary>
    /// 备注
    /// </summary>
    [MaxLength(500)]
    public string? Remark { get; set; }

    /// <summary>
    /// 导航属性 - 软件版本
    /// </summary>
    public SoftwareVersion SoftwareVersion { get; set; } = null!;

    /// <summary>
    /// 导航属性 - 设备
    /// </summary>
    public Equipment Equipment { get; set; } = null!;
}
