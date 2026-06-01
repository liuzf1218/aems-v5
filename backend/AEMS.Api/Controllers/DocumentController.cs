using AEMS.Core.DTOs;
using AEMS.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AEMS.Api.Controllers;

/// <summary>
/// 文档管理控制器
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DocumentController : ControllerBase
{
    private readonly IDocumentService _documentService;

    public DocumentController(IDocumentService documentService)
    {
        _documentService = documentService;
    }

    /// <summary>
    /// 获取文档列表（带筛选和分页）
    /// </summary>
    [HttpGet]
    public async Task<ApiResponse<PagedResult<DocumentListItemDto>>> GetList([FromQuery] DocumentListRequest request)
    {
        var result = await _documentService.GetDocumentListAsync(request);
        return ApiResponse<PagedResult<DocumentListItemDto>>.Success(result);
    }

    /// <summary>
    /// 创建文档（上传文件）
    /// </summary>
    [HttpPost]
    [Consumes("multipart/form-data")]
    public async Task<ApiResponse<DocumentDetailDto>> Create([FromForm] CreateDocumentRequest request)
    {
        if (request.File == null || request.File.Length == 0)
            return ApiResponse<DocumentDetailDto>.Fail(400, "请选择要上传的文件");

        var userId = GetCurrentUserId();
        var result = await _documentService.CreateDocumentAsync(request, request.File, userId);
        return ApiResponse<DocumentDetailDto>.Success(result, "文档创建成功");
    }

    /// <summary>
    /// 获取文档详情
    /// </summary>
    [HttpGet("{id:int}")]
    public async Task<ApiResponse<DocumentDetailDto>> GetById(int id)
    {
        var result = await _documentService.GetDocumentDetailAsync(id);
        if (result == null)
            return ApiResponse<DocumentDetailDto>.Fail(404, "文档不存在");
        return ApiResponse<DocumentDetailDto>.Success(result);
    }

    /// <summary>
    /// 删除文档（软删除）
    /// </summary>
    [HttpDelete("{id:int}")]
    public async Task<ApiResponse> Delete(int id)
    {
        await _documentService.DeleteDocumentAsync(id);
        return ApiResponse.Success("删除成功");
    }

    /// <summary>
    /// 下载文档（当前版本）
    /// </summary>
    [HttpGet("{id:int}/download")]
    [AllowAnonymous]
    public async Task<IActionResult> Download(int id)
    {
        var fileResult = await _documentService.GetFileStreamAsync(id);
        if (fileResult == null)
            return NotFound("文件不存在");

        var (stream, fileName, contentType) = fileResult.Value;
        return File(stream, contentType, fileName);
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