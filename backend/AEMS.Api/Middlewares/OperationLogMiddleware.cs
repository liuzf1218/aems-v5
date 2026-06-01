using System.Diagnostics;
using System.Security.Claims;
using AEMS.Core.Entities;
using AEMS.Infrastructure.Data;

namespace AEMS.Api.Middlewares;

/// <summary>
/// 操作日志中间件 - 自动记录所有写操作（POST/PUT/DELETE）
/// </summary>
public class OperationLogMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<OperationLogMiddleware> _logger;

    // 不记录日志的路径（登录/登出等由各自控制器处理）
    private static readonly HashSet<string> ExcludePaths = new(StringComparer.OrdinalIgnoreCase)
    {
        "/api/auth/login",
        "/api/auth/logout",
        "/api/health"
    };

    public OperationLogMiddleware(RequestDelegate next, ILogger<OperationLogMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var method = context.Request.Method;
        var path = context.Request.Path.Value ?? "";

        // 只记录写操作（POST/PUT/DELETE），跳过排除路径
        if (!IsWriteOperation(method) || IsExcludedPath(path))
        {
            await _next(context);
            return;
        }

        var stopwatch = Stopwatch.StartNew();

        // 获取用户信息
        int? userId = null;
        var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim != null && int.TryParse(userIdClaim.Value, out var uid))
        {
            userId = uid;
        }

        // 读取请求体（需要启用 buffering）
        string? requestBody = null;
        if (context.Request.ContentLength > 0 && context.Request.ContentLength < 10240)
        {
            context.Request.EnableBuffering();
            context.Request.Body.Position = 0;
            using var reader = new StreamReader(context.Request.Body, leaveOpen: true);
            requestBody = await reader.ReadToEndAsync();
            context.Request.Body.Position = 0;
        }

        // 使用自定义响应流捕获响应体
        var originalBodyStream = context.Response.Body;
        using var responseBodyStream = new MemoryStream();
        context.Response.Body = responseBodyStream;

        try
        {
            await _next(context);
        }
        finally
        {
            stopwatch.Stop();

            // 读取响应体
            responseBodyStream.Position = 0;
            var responseBody = await new StreamReader(responseBodyStream).ReadToEndAsync();
            responseBodyStream.Position = 0;

            // 将响应复制回原始流
            await responseBodyStream.CopyToAsync(originalBodyStream);

            // 异步写入日志（不影响响应速度）
            _ = Task.Run(async () =>
            {
                try
                {
                    // 从作用域获取DbContext（创建新scope避免disposed问题）
                    using var scope = context.RequestServices.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<AemsDbContext>();

                    var log = new SysLog
                    {
                        UserId = userId,
                        Action = GetActionName(method, path),
                        Content = BuildLogContent(method, path, requestBody, responseBody),
                        IpAddress = GetClientIp(context),
                        Method = method,
                        Path = path,
                        StatusCode = context.Response.StatusCode,
                        ElapsedMs = stopwatch.ElapsedMilliseconds
                    };

                    db.SysLogs.Add(log);
                    await db.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "写入操作日志失败: {Path}", path);
                }
            });
        }
    }

    /// <summary>
    /// 判断是否为写操作
    /// </summary>
    private static bool IsWriteOperation(string method)
    {
        return method is "POST" or "PUT" or "DELETE" or "PATCH";
    }

    /// <summary>
    /// 判断是否为排除路径
    /// </summary>
    private static bool IsExcludedPath(string path)
    {
        return ExcludePaths.Any(p => path.StartsWith(p, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// 获取操作名称
    /// </summary>
    private static string GetActionName(string method, string path)
    {
        // 从路径提取模块名
        var segments = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
        var module = segments.Length > 1 ? segments[1] : "unknown";

        return method switch
        {
            "POST" => $"{module}_create",
            "PUT" => $"{module}_update",
            "DELETE" => $"{module}_delete",
            "PATCH" => $"{module}_patch",
            _ => $"{module}_{method.ToLower()}"
        };
    }

    /// <summary>
    /// 构建日志内容
    /// </summary>
    private static string BuildLogContent(string method, string path, string? requestBody, string? responseBody)
    {
        var content = $"{method} {path}";
        if (!string.IsNullOrEmpty(requestBody) && requestBody.Length < 2000)
        {
            content += $" | 请求: {requestBody}";
        }
        return content;
    }

    /// <summary>
    /// 获取客户端IP
    /// </summary>
    private static string? GetClientIp(HttpContext context)
    {
        return context.Request.Headers["X-Forwarded-For"].FirstOrDefault()
            ?? context.Connection.RemoteIpAddress?.ToString();
    }
}
