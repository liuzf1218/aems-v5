using Microsoft.AspNetCore.Http;

namespace AEMS.Core.DTOs;

/// <summary>
/// 文档列表请求参数
/// </summary>
public class DocumentListRequest : PagedRequest
{
    /// <summary>
    /// 文档名称（模糊搜索）
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// 文档编号
    /// </summary>
    public string? DocNo { get; set; }

    /// <summary>
    /// 文档分类
    /// </summary>
    public string? Category { get; set; }

    /// <summary>
    /// 关联设备ID
    /// </summary>
    public int? EquipmentId { get; set; }

    /// <summary>
    /// 所属系统ID
    /// </summary>
    public int? SubsystemId { get; set; }
}

/// <summary>
/// 文档列表响应
/// </summary>
public class DocumentListItemDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string DocNo { get; set; } = string.Empty;
    public string? Category { get; set; }
    public int? EquipmentId { get; set; }
    public string? EquipmentName { get; set; }
    public string? SystemName { get; set; }
    public string? CurrentVersion { get; set; }
    public int? UploaderId { get; set; }
    public string? Remark { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// 文档详情响应
/// </summary>
public class DocumentDetailDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string DocNo { get; set; } = string.Empty;
    public string? Category { get; set; }
    public int? EquipmentId { get; set; }
    public string? EquipmentName { get; set; }
    public string? SystemName { get; set; }
    public string? CurrentVersion { get; set; }
    public int? UploaderId { get; set; }
    public string? Remark { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DocumentVersionDto? LatestVersion { get; set; }
}

/// <summary>
/// 创建文档请求（包含文件上传）
/// </summary>
public class CreateDocumentRequest
{
    /// <summary>
    /// 文档名称
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 文档编号
    /// </summary>
    public string DocNo { get; set; } = string.Empty;

    /// <summary>
    /// 文档分类
    /// </summary>
    public string? Category { get; set; }

    /// <summary>
    /// 关联设备ID
    /// </summary>
    public int? EquipmentId { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    public string? Remark { get; set; }

    /// <summary>
    /// 上传的文件（与请求合并，避免 Swagger 生成冲突）
    /// </summary>
    public IFormFile File { get; set; } = null!;
}

/// <summary>
/// 文档版本响应
/// </summary>
public class DocumentVersionDto
{
    public int Id { get; set; }
    public int DocumentId { get; set; }
    public string Version { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public string? OriginalFileName { get; set; }
    public long FileSize { get; set; }
    public string? FileType { get; set; }
    public int? UploaderId { get; set; }
    public string? ChangeNote { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// 上传新版本请求
/// </summary>
public class UploadVersionRequest
{
    /// <summary>
    /// 版本号
    /// </summary>
    public string Version { get; set; } = string.Empty;

    /// <summary>
    /// 变更说明
    /// </summary>
    public string? ChangeNote { get; set; }
}