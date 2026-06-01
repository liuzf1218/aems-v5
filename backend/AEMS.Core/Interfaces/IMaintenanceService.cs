using AEMS.Core.DTOs;

namespace AEMS.Core.Interfaces;

/// <summary>
/// 维护计划服务接口
/// </summary>
public interface IMaintenancePlanService
{
    /// <summary>
    /// 获取计划列表（分页+筛选）
    /// </summary>
    Task<PagedResult<MaintenancePlanDto>> GetPlanListAsync(MaintenancePlanQueryRequest query);

    /// <summary>
    /// 获取计划详情
    /// </summary>
    Task<MaintenancePlanDto?> GetPlanByIdAsync(int id);

    /// <summary>
    /// 创建计划
    /// </summary>
    Task<MaintenancePlanDto> CreatePlanAsync(MaintenancePlanRequest request);

    /// <summary>
    /// 更新计划
    /// </summary>
    Task<MaintenancePlanDto?> UpdatePlanAsync(int id, MaintenancePlanRequest request);

    /// <summary>
    /// 删除计划（软删除）
    /// </summary>
    Task<bool> DeletePlanAsync(int id);

    /// <summary>
    /// 启用/停用计划
    /// </summary>
    Task<bool> TogglePlanAsync(int id);
}

/// <summary>
/// 维护任务服务接口
/// </summary>
public interface IMaintenanceTaskService
{
    /// <summary>
    /// 获取任务列表（分页+筛选）
    /// </summary>
    Task<PagedResult<MaintenanceTaskDto>> GetTaskListAsync(MaintenanceTaskQueryRequest query);

    /// <summary>
    /// 获取任务详情
    /// </summary>
    Task<MaintenanceTaskDto?> GetTaskByIdAsync(int id);

    /// <summary>
    /// 手动创建任务
    /// </summary>
    Task<MaintenanceTaskDto> CreateTaskAsync(MaintenanceTaskRequest request);

    /// <summary>
    /// 派单
    /// </summary>
    Task<bool> DispatchTaskAsync(int id, DispatchRequest request);

    /// <summary>
    /// 执行任务
    /// </summary>
    Task<bool> ExecuteTaskAsync(int id, ExecuteRequest request);

    /// <summary>
    /// 审核任务
    /// </summary>
    Task<bool> ReviewTaskAsync(int id, ReviewRequest request);

    /// <summary>
    /// 获取统计数据
    /// </summary>
    Task<MaintenanceTaskStatsDto> GetTaskStatsAsync();
}
