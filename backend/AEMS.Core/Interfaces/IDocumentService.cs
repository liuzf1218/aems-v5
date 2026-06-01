using AEMS.Core.DTOs;
using Microsoft.AspNetCore.Http;

namespace AEMS.Core.Interfaces;

/// <summary>
/// 文档管理服务接口
/// </summary>
public interface IDocumentService
{
    /// <summary>
    /// 获取文档列表（带筛选和分页）
    /// </summary>
    Task<PagedResult<DocumentListItemDto>> GetDocumentListAsync(DocumentListRequest request);

    /// <summary>
    /// 获取文档详情
    /// </summary>
    Task<DocumentDetailDto?> GetDocumentDetailAsync(int id);

    /// <summary>
    /// 创建文档（含初始版本）
    /// </summary>
    Task<DocumentDetailDto> CreateDocumentAsync(CreateDocumentRequest request, IFormFile file, int? uploaderId);

    /// <summary>
    /// 删除文档（软删除）
    /// </summary>
    Task DeleteDocumentAsync(int id);

    /// <summary>
    /// 获取文档版本列表
    /// </summary>
    Task<IEnumerable<DocumentVersionDto>> GetDocumentVersionsAsync(int documentId);

    /// <summary>
    /// 上传新版本
    /// </summary>
    Task<DocumentVersionDto> UploadVersionAsync(int documentId, IFormFile file, string version, string? changeNote, int? uploaderId);

    /// <summary>
    /// 设置当前版本
    /// </summary>
    Task SetCurrentVersionAsync(int documentId, int versionId);

    /// <summary>
    /// 获取文件流（用于下载）
    /// </summary>
    Task<(Stream FileStream, string FileName, string ContentType)?> GetFileStreamAsync(int documentId);
}
