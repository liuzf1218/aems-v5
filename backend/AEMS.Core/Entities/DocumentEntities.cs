using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AEMS.Core.Entities;

/// <summary>
/// 文档
/// </summary>
[Table("document")]
public class Document : BaseEntity
{
    /// <summary>
    /// 文档名称
    /// </summary>
    [Required, MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 文档编号
    /// </summary>
    [Required, MaxLength(50)]
    public string DocNo { get; set; } = string.Empty;

    /// <summary>
    /// 文档分类
    /// </summary>
    [MaxLength(50)]
    public string? Category { get; set; }

    /// <summary>
    /// 关联设备ID
    /// </summary>
    public int? EquipmentId { get; set; }

    /// <summary>
    /// 当前版本号
    /// </summary>
    [MaxLength(20)]
    public string? CurrentVersion { get; set; }

    /// <summary>
    /// 上传人ID
    /// </summary>
    public int? UploaderId { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    [MaxLength(500)]
    public string? Remark { get; set; }

    /// <summary>
    /// 导航属性 - 设备
    /// </summary>
    public Equipment? Equipment { get; set; }

    /// <summary>
    /// 导航属性 - 版本列表
    /// </summary>
    public ICollection<DocumentVersion> Versions { get; set; } = new List<DocumentVersion>();
}

/// <summary>
/// 文档版本
/// </summary>
[Table("document_version")]
public class DocumentVersion : BaseEntity
{
    /// <summary>
    /// 文档ID
    /// </summary>
    public int DocumentId { get; set; }

    /// <summary>
    /// 版本号
    /// </summary>
    [Required, MaxLength(20)]
    public string Version { get; set; } = string.Empty;

    /// <summary>
    /// 文件路径
    /// </summary>
    [Required, MaxLength(500)]
    public string FilePath { get; set; } = string.Empty;

    /// <summary>
    /// 原始文件名
    /// </summary>
    [MaxLength(200)]
    public string? OriginalFileName { get; set; }

    /// <summary>
    /// 文件大小（字节）
    /// </summary>
    public long FileSize { get; set; }

    /// <summary>
    /// 文件类型
    /// </summary>
    [MaxLength(50)]
    public string? FileType { get; set; }

    /// <summary>
    /// 上传人ID
    /// </summary>
    public int? UploaderId { get; set; }

    /// <summary>
    /// 变更说明
    /// </summary>
    [MaxLength(500)]
    public string? ChangeNote { get; set; }

    /// <summary>
    /// 导航属性 - 文档
    /// </summary>
    public Document Document { get; set; } = null!;
}
