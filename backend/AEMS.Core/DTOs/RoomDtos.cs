using System.ComponentModel.DataAnnotations;

namespace AEMS.Core.DTOs;

/// <summary>
/// 机房列表查询参数
/// </summary>
public class RoomQueryRequest : PagedRequest
{
    /// <summary>
    /// 关键词搜索（名称/编码）
    /// </summary>
    public string? Keyword { get; set; }

    /// <summary>
    /// 负责人
    /// </summary>
    public string? Manager { get; set; }
}

/// <summary>
/// 机房创建/更新请求
/// </summary>
public class RoomRequest
{
    [Required(ErrorMessage = "机房名称不能为空")]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "机房编码不能为空")]
    [MaxLength(50)]
    public string Code { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? Location { get; set; }

    public decimal? Area { get; set; }

    public decimal? TempUpper { get; set; }

    public decimal? HumidityUpper { get; set; }

    [MaxLength(50)]
    public string? Manager { get; set; }

    [MaxLength(500)]
    public string? Remark { get; set; }
}

/// <summary>
/// 机房列表响应DTO
/// </summary>
public class RoomListItemDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string? Location { get; set; }
    public decimal? Area { get; set; }
    public string? Manager { get; set; }
    public int CabinetCount { get; set; }
    public int DeviceCount { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// 机房详情响应DTO
/// </summary>
public class RoomDetailDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string? Location { get; set; }
    public decimal? Area { get; set; }
    public decimal? TempUpper { get; set; }
    public decimal? HumidityUpper { get; set; }
    public string? Manager { get; set; }
    public string? Remark { get; set; }
    public int? BuildingId { get; set; }
    public string? BuildingName { get; set; }
    public string? Floor { get; set; }
    public int CabinetCount { get; set; }
    public int DeviceCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// 机柜创建/更新请求
/// </summary>
public class CabinetRequest
{
    [Required(ErrorMessage = "机柜名称不能为空")]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "机柜编号不能为空")]
    [MaxLength(50)]
    public string Code { get; set; } = string.Empty;

    [Required(ErrorMessage = "机房ID不能为空")]
    public int RoomId { get; set; }

    public int TotalUnits { get; set; } = 42;

    public decimal? PowerLimit { get; set; }

    [MaxLength(500)]
    public string? Remark { get; set; }
}

/// <summary>
/// 机柜列表响应DTO
/// </summary>
public class CabinetListItemDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public int RoomId { get; set; }
    public string? RoomName { get; set; }
    public int TotalUnits { get; set; }
    public int UsedUnits { get; set; }
    public decimal? PowerLimit { get; set; }
    public int DeviceCount { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// 机柜详情响应DTO
/// </summary>
public class CabinetDetailDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public int RoomId { get; set; }
    public string? RoomName { get; set; }
    public int TotalUnits { get; set; }
    public int UsedUnits { get; set; }
    public decimal? PowerLimit { get; set; }
    public string? Remark { get; set; }
    public int DeviceCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// 机房内设备列表响应DTO
/// </summary>
public class RoomDeviceDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string? TypeName { get; set; }
    public string? CabinetName { get; set; }
    public string? Position { get; set; }
    public string? IpAddress { get; set; }
    public int Status { get; set; }
    public string? Manufacturer { get; set; }
    public string? Model { get; set; }
}

/// <summary>
/// 机柜查询参数
/// </summary>
public class CabinetQueryRequest : PagedRequest
{
    /// <summary>
    /// 关键词搜索
    /// </summary>
    public string? Keyword { get; set; }

    /// <summary>
    /// 机房ID筛选
    /// </summary>
    public int? RoomId { get; set; }
}
