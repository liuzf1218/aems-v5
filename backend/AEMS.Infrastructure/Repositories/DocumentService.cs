using AEMS.Core.DTOs;
using AEMS.Core.Entities;
using AEMS.Core.Interfaces;
using AEMS.Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace AEMS.Infrastructure.Repositories;

/// <summary>
/// 文档管理服务实现
/// </summary>
public class DocumentService : IDocumentService
{
    private readonly AemsDbContext _context;
    private readonly string _uploadRoot;

    public DocumentService(AemsDbContext context, IConfiguration configuration)
    {
        _context = context;
        _uploadRoot = configuration["FileStorage:RootPath"]
            ?? Path.Combine(Directory.GetCurrentDirectory(), "uploads");
        Directory.CreateDirectory(_uploadRoot);
    }

    /// <summary>
    /// 获取文档列表（带筛选和分页）
    /// </summary>
    public async Task<PagedResult<DocumentListItemDto>> GetDocumentListAsync(DocumentListRequest request)
    {
        var query = _context.Documents
            .Include(d => d.Equipment)
                .ThenInclude(e => e!.Subsystem)
            .Where(d => !d.IsDeleted);

        // 筛选条件
        if (!string.IsNullOrWhiteSpace(request.Name))
            query = query.Where(d => d.Name.Contains(request.Name));

        if (!string.IsNullOrWhiteSpace(request.DocNo))
            query = query.Where(d => d.DocNo.Contains(request.DocNo));

        if (!string.IsNullOrWhiteSpace(request.Category))
            query = query.Where(d => d.Category == request.Category);

        if (request.EquipmentId.HasValue)
            query = query.Where(d => d.EquipmentId == request.EquipmentId.Value);

        if (request.SubsystemId.HasValue)
            query = query.Where(d => d.Equipment != null && d.Equipment.SubsystemId == request.SubsystemId.Value);

        // 排序
        query = request.SortBy?.ToLower() switch
        {
            "name" => request.Desc ? query.OrderByDescending(d => d.Name) : query.OrderBy(d => d.Name),
            "docno" => request.Desc ? query.OrderByDescending(d => d.DocNo) : query.OrderBy(d => d.DocNo),
            "updatedat" => request.Desc ? query.OrderByDescending(d => d.UpdatedAt) : query.OrderBy(d => d.UpdatedAt),
            _ => query.OrderByDescending(d => d.UpdatedAt)
        };

        var total = await query.CountAsync();
        var items = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(d => new DocumentListItemDto
            {
                Id = d.Id,
                Name = d.Name,
                DocNo = d.DocNo,
                Category = d.Category,
                EquipmentId = d.EquipmentId,
                EquipmentName = d.Equipment != null ? d.Equipment.Name : "-",
                SystemName = d.Equipment != null && d.Equipment.Subsystem != null ? d.Equipment.Subsystem.Name : "-",
                CurrentVersion = d.CurrentVersion,
                UploaderId = d.UploaderId,
                Remark = d.Remark,
                CreatedAt = d.CreatedAt,
                UpdatedAt = d.UpdatedAt
            })
            .ToListAsync();

        return new PagedResult<DocumentListItemDto>
        {
            Items = items,
            Total = total,
            Page = request.Page,
            PageSize = request.PageSize
        };
    }

    /// <summary>
    /// 获取文档详情
    /// </summary>
    public async Task<DocumentDetailDto?> GetDocumentDetailAsync(int id)
    {
        var doc = await _context.Documents
            .Include(d => d.Equipment)
                .ThenInclude(e => e!.Subsystem)
            .Include(d => d.Versions)
            .FirstOrDefaultAsync(d => d.Id == id && !d.IsDeleted);

        if (doc == null) return null;

        var latestVersion = doc.Versions
            .OrderByDescending(v => v.CreatedAt)
            .FirstOrDefault();

        return new DocumentDetailDto
        {
            Id = doc.Id,
            Name = doc.Name,
            DocNo = doc.DocNo,
            Category = doc.Category,
            EquipmentId = doc.EquipmentId,
            EquipmentName = doc.Equipment != null ? doc.Equipment.Name : "-",
            SystemName = doc.Equipment != null && doc.Equipment.Subsystem != null ? doc.Equipment.Subsystem.Name : "-",
            CurrentVersion = doc.CurrentVersion,
            UploaderId = doc.UploaderId,
            Remark = doc.Remark,
            CreatedAt = doc.CreatedAt,
            UpdatedAt = doc.UpdatedAt,
            LatestVersion = latestVersion != null ? MapToVersionDto(latestVersion) : null
        };
    }

    /// <summary>
    /// 创建文档（含初始版本）
    /// </summary>
    public async Task<DocumentDetailDto> CreateDocumentAsync(CreateDocumentRequest request, IFormFile file, int? uploaderId)
    {
        // 保存文件
        var fileName = file.FileName;
        var ext = Path.GetExtension(fileName);
        var storedName = $"{Guid.NewGuid()}{ext}";
        var relativePath = Path.Combine("documents", storedName);
        var fullPath = Path.Combine(_uploadRoot, relativePath);

        Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);
        using (var stream = new FileStream(fullPath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        var now = DateTime.Now;

        // 创建文档
        var document = new Document
        {
            Name = request.Name,
            DocNo = request.DocNo,
            Category = request.Category,
            EquipmentId = request.EquipmentId,
            CurrentVersion = "V1.0",
            UploaderId = uploaderId,
            Remark = request.Remark,
            CreatedAt = now,
            UpdatedAt = now
        };

        _context.Documents.Add(document);
        await _context.SaveChangesAsync();

        // 创建初始版本
        var version = new DocumentVersion
        {
            DocumentId = document.Id,
            Version = "V1.0",
            FilePath = relativePath,
            OriginalFileName = fileName,
            FileSize = file.Length,
            FileType = ext,
            UploaderId = uploaderId,
            ChangeNote = "初始版本",
            CreatedAt = now,
            UpdatedAt = now
        };

        _context.DocumentVersions.Add(version);
        await _context.SaveChangesAsync();

        return new DocumentDetailDto
        {
            Id = document.Id,
            Name = document.Name,
            DocNo = document.DocNo,
            Category = document.Category,
            EquipmentId = document.EquipmentId,
            CurrentVersion = document.CurrentVersion,
            UploaderId = document.UploaderId,
            Remark = document.Remark,
            CreatedAt = document.CreatedAt,
            UpdatedAt = document.UpdatedAt,
            LatestVersion = MapToVersionDto(version)
        };
    }

    /// <summary>
    /// 删除文档（软删除）
    /// </summary>
    public async Task DeleteDocumentAsync(int id)
    {
        var doc = await _context.Documents
            .FirstOrDefaultAsync(d => d.Id == id && !d.IsDeleted);

        if (doc == null) return;

        doc.IsDeleted = true;
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// 获取文档版本列表
    /// </summary>
    public async Task<IEnumerable<DocumentVersionDto>> GetDocumentVersionsAsync(int documentId)
    {
        var versions = await _context.DocumentVersions
            .Where(v => v.DocumentId == documentId)
            .OrderByDescending(v => v.CreatedAt)
            .ToListAsync();

        return versions.Select(MapToVersionDto);
    }

    /// <summary>
    /// 上传新版本
    /// </summary>
    public async Task<DocumentVersionDto> UploadVersionAsync(
        int documentId, IFormFile file, string version, string? changeNote, int? uploaderId)
    {
        var doc = await _context.Documents
            .FirstOrDefaultAsync(d => d.Id == documentId && !d.IsDeleted);

        if (doc == null)
            throw new InvalidOperationException("文档不存在");

        // 保存文件
        var fileName = file.FileName;
        var ext = Path.GetExtension(fileName);
        var storedName = $"{Guid.NewGuid()}{ext}";
        var relativePath = Path.Combine("documents", storedName);
        var fullPath = Path.Combine(_uploadRoot, relativePath);

        Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);
        using (var stream = new FileStream(fullPath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        var now = DateTime.Now;

        var docVersion = new DocumentVersion
        {
            DocumentId = documentId,
            Version = version,
            FilePath = relativePath,
            OriginalFileName = fileName,
            FileSize = file.Length,
            FileType = ext,
            UploaderId = uploaderId,
            ChangeNote = changeNote,
            CreatedAt = now,
            UpdatedAt = now
        };

        _context.DocumentVersions.Add(docVersion);

        // 更新文档当前版本号
        doc.CurrentVersion = version;
        doc.UpdatedAt = now;

        await _context.SaveChangesAsync();

        return MapToVersionDto(docVersion);
    }

    /// <summary>
    /// 设置当前版本
    /// </summary>
    public async Task SetCurrentVersionAsync(int documentId, int versionId)
    {
        var doc = await _context.Documents
            .FirstOrDefaultAsync(d => d.Id == documentId && !d.IsDeleted);

        if (doc == null)
            throw new InvalidOperationException("文档不存在");

        var version = await _context.DocumentVersions
            .FirstOrDefaultAsync(v => v.Id == versionId && v.DocumentId == documentId);

        if (version == null)
            throw new InvalidOperationException("版本不存在");

        doc.CurrentVersion = version.Version;
        doc.UpdatedAt = DateTime.Now;

        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// 获取文件流（用于下载）
    /// </summary>
    public async Task<(Stream FileStream, string FileName, string ContentType)?> GetFileStreamAsync(int documentId)
    {
        // 查找当前版本
        var doc = await _context.Documents
            .Include(d => d.Versions)
            .FirstOrDefaultAsync(d => d.Id == documentId && !d.IsDeleted);

        if (doc == null) return null;

        var currentVersion = doc.Versions
            .FirstOrDefault(v => v.Version == doc.CurrentVersion)
            ?? doc.Versions.OrderByDescending(v => v.CreatedAt).FirstOrDefault();

        if (currentVersion == null) return null;

        var fullPath = Path.Combine(_uploadRoot, currentVersion.FilePath);
        if (!File.Exists(fullPath)) return null;

        var contentType = GetContentType(currentVersion.FileType);
        var stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read);

        return (stream, currentVersion.OriginalFileName ?? "download", contentType);
    }

    /// <summary>
    /// 映射到DTO
    /// </summary>
    private static DocumentVersionDto MapToVersionDto(DocumentVersion v)
    {
        return new DocumentVersionDto
        {
            Id = v.Id,
            DocumentId = v.DocumentId,
            Version = v.Version,
            FilePath = v.FilePath,
            OriginalFileName = v.OriginalFileName,
            FileSize = v.FileSize,
            FileType = v.FileType,
            UploaderId = v.UploaderId,
            ChangeNote = v.ChangeNote,
            CreatedAt = v.CreatedAt
        };
    }

    /// <summary>
    /// 获取MIME类型
    /// </summary>
    private static string GetContentType(string? ext)
    {
        return ext?.ToLower() switch
        {
            ".pdf" => "application/pdf",
            ".doc" => "application/msword",
            ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            ".xls" => "application/vnd.ms-excel",
            ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            ".ppt" => "application/vnd.ms-powerpoint",
            ".pptx" => "application/vnd.openxmlformats-officedocument.presentationml.presentation",
            ".txt" => "text/plain",
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".zip" => "application/zip",
            ".rar" => "application/x-rar-compressed",
            _ => "application/octet-stream"
        };
    }
}
