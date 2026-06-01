namespace AEMS.Core.DTOs;

/// <summary>
/// 操作日志查询参数
/// </summary>
public class LogQueryRequest : PagedRequest
{
    /// <summary>
    /// 用户名筛选
    /// </summary>
    public string? Username { get; set; }

    /// <summary>
    /// 操作类型筛选
    /// </summary>
    public string? Action { get; set; }

    /// <summary>
    /// 开始时间
    /// </summary>
    public DateTime? StartTime { get; set; }

    /// <summary>
    /// 结束时间
    /// </summary>
    public DateTime? EndTime { get; set; }
}

/// <summary>
/// 操作日志响应DTO
/// </summary>
public class LogDto
{
    public int Id { get; set; }
    public int? UserId { get; set; }
    public string? Action { get; set; }
    public string? Content { get; set; }
    public string? IpAddress { get; set; }
    public string? Method { get; set; }
    public string? Path { get; set; }
    public int? StatusCode { get; set; }
    public long? ElapsedMs { get; set; }
    public DateTime CreatedAt { get; set; }
}
