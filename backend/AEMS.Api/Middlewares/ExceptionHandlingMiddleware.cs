using System.Text.Json;
using AEMS.Core.DTOs;

namespace AEMS.Api.Middlewares;

/// <summary>
/// 全局异常处理中间件
/// </summary>
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (UnauthorizedAccessException ex)
        {
            // 未授权异常
            _logger.LogWarning(ex, "未授权访问: {Message}", ex.Message);
            await WriteResponseAsync(context, 401, ex.Message);
        }
        catch (ArgumentException ex)
        {
            // 参数错误
            _logger.LogWarning(ex, "参数错误: {Message}", ex.Message);
            await WriteResponseAsync(context, 400, ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            // 业务逻辑错误
            _logger.LogWarning(ex, "业务错误: {Message}", ex.Message);
            await WriteResponseAsync(context, 400, ex.Message);
        }
        catch (KeyNotFoundException ex)
        {
            // 资源不存在
            _logger.LogWarning(ex, "资源不存在: {Message}", ex.Message);
            await WriteResponseAsync(context, 404, ex.Message);
        }
        catch (Exception ex)
            {
            // 未知异常
            _logger.LogError(ex, "服务器内部错误: {Message}", ex.Message);
            await WriteResponseAsync(context, 500, "服务器内部错误");
        }
    }

    /// <summary>
    /// 写入错误响应
    /// </summary>
    private static async Task WriteResponseAsync(HttpContext context, int code, string message)
    {
        context.Response.ContentType = "application/json; charset=utf-8";
        context.Response.StatusCode = 200; // HTTP状态码统一返回200，通过业务码区分

        var response = ApiResponse.Fail(code, message);
        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(json);
    }
}
