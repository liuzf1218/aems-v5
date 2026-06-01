namespace AEMS.Core.DTOs;

/// <summary>
/// 统一API响应格式
/// </summary>
/// <typeparam name="T">响应数据类型</typeparam>
public class ApiResponse<T>
{
    /// <summary>
    /// 状态码：200成功/400参数错误/401未授权/403无权限/500服务器错误
    /// </summary>
    public int Code { get; set; } = 200;

    /// <summary>
    /// 响应消息
    /// </summary>
    public string Message { get; set; } = "success";

    /// <summary>
    /// 响应数据
    /// </summary>
    public T? Data { get; set; }

    /// <summary>
    /// 创建成功响应
    /// </summary>
    public static ApiResponse<T> Success(T data, string message = "success")
    {
        return new ApiResponse<T> { Code = 200, Message = message, Data = data };
    }

    /// <summary>
    /// 创建失败响应
    /// </summary>
    public static ApiResponse<T> Fail(int code, string message)
    {
        return new ApiResponse<T> { Code = code, Message = message, Data = default };
    }
}

/// <summary>
/// 无数据的统一响应
/// </summary>
public class ApiResponse : ApiResponse<object>
{
    /// <summary>
    /// 创建成功响应（无数据）
    /// </summary>
    public static ApiResponse Success(string message = "success")
    {
        return new ApiResponse { Code = 200, Message = message };
    }

    /// <summary>
    /// 创建失败响应
    /// </summary>
    public new static ApiResponse Fail(int code, string message)
    {
        return new ApiResponse { Code = code, Message = message };
    }
}
