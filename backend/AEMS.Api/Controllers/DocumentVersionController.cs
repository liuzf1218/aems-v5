using AEMS.Core.DTOs;
using AEMS.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AEMS.Api.Controllers;

/// <summary>
/// 文档版本管理控制器
/// </summary>
[ApiController]
[Route("api/document/{documentId:int}/versions")]
[Authorize]
public class DocumentVersionController : ControllerBase
{
    private readonly IDocumentService _documentService;

    public DocumentVersionController(IDocumentService documentService)
    {
        _documentService = documentService;
    }

    /// <summary>
    /// 获取文档版本列表
    /// </summary>
    [HttpGet]
    public async Task<ApiResponse<IEnumerable<DocumentVersionDto>>> GetList(int documentId)
    {
        var result = await _documentService.GetDocumentVersionsAsync(documentId);
        return ApiResponse<IEnumerable<DocumentVersionDto>>.Success(result);
    }

    /// <summary>
    /// 上传新版本
    /// </summary>
    [HttpPost]
    [Consumes("multipart/form-data")]
    public async Task<ApiResponse<DocumentVersionDto>> Upload(
        int documentId,
        [FromForm] string version,
        [FromForm] string? changeNote,
        [FromForm] IFormFile file)
    {
        if (file == null || file.Length == 0)
            return ApiResponse<DocumentVersionDto>.Fail(400, "请选择要上传的文件");

        if (string.IsNullOrWhiteSpace(version))
            return ApiResponse<DocumentVersionDto>.Fail(400, "版本号不能为空");

        var userId = GetCurrentUserId();
        var result = await _documentService.UploadVersionAsync(documentId, file, version, changeNote, userId);
        return ApiResponse<DocumentVersionDto>.Success(result, "版本上传成功");
    }

    /// <summary>
    /// 设置当前版本
    /// </summary>
    [HttpPut("{versionId:int}/current")]
    public async Task<ApiResponse> SetCurrent(int documentId, int versionId)
    {
        await _documentService.SetCurrentVersionAsync(documentId, versionId);
        return ApiResponse.Success("设置成功");
    }

    /// <summary>
    /// 获取当前用户ID
    /// </summary>
    private int? GetCurrentUserId()
    {
        var claim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        if (claim != null && int.TryParse(claim.Value, out var id))
            return id;
        return null;
    }
}
