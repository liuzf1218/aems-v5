using AEMS.Core.DTOs;

namespace AEMS.Core.Interfaces;

/// <summary>
/// 机房服务接口
/// </summary>
public interface IRoomService
{
    /// <summary>
    /// 获取机房列表（分页+筛选）
    /// </summary>
    Task<PagedResult<RoomListItemDto>> GetRoomListAsync(RoomQueryRequest request);

    /// <summary>
    /// 获取机房详情
    /// </summary>
    Task<RoomDetailDto?> GetRoomDetailAsync(int id);

    /// <summary>
    /// 新增机房
    /// </summary>
    Task<RoomDetailDto> CreateRoomAsync(RoomRequest request);

    /// <summary>
    /// 更新机房
    /// </summary>
    Task<RoomDetailDto?> UpdateRoomAsync(int id, RoomRequest request);

    /// <summary>
    /// 删除机房
    /// </summary>
    Task<bool> DeleteRoomAsync(int id);

    /// <summary>
    /// 获取机房下的机柜列表
    /// </summary>
    Task<List<CabinetListItemDto>> GetRoomCabinetsAsync(int roomId);

    /// <summary>
    /// 获取机房内的设备列表
    /// </summary>
    Task<List<RoomDeviceDto>> GetRoomDevicesAsync(int roomId);
}

/// <summary>
/// 机柜服务接口
/// </summary>
public interface ICabinetService
{
    /// <summary>
    /// 获取机柜列表（分页+筛选）
    /// </summary>
    Task<PagedResult<CabinetListItemDto>> GetCabinetListAsync(CabinetQueryRequest request);

    /// <summary>
    /// 获取机柜详情
    /// </summary>
    Task<CabinetDetailDto?> GetCabinetDetailAsync(int id);

    /// <summary>
    /// 新增机柜
    /// </summary>
    Task<CabinetDetailDto> CreateCabinetAsync(CabinetRequest request);

    /// <summary>
    /// 更新机柜
    /// </summary>
    Task<CabinetDetailDto?> UpdateCabinetAsync(int id, CabinetRequest request);

    /// <summary>
    /// 删除机柜
    /// </summary>
    Task<bool> DeleteCabinetAsync(int id);
}
