using AEMS.Core.DTOs;

namespace AEMS.Core.Interfaces;

public interface IWorkOrderService
{
    Task<PagedResult<WorkOrderListItemDto>> GetListAsync(WorkOrderQueryRequest query);
    Task<WorkOrderDetailDto?> GetByIdAsync(int id);
    Task<WorkOrderDetailDto> CreateAsync(CreateWorkOrderRequest request, int creatorId);
    Task<bool> AcceptAsync(int id, int operatorId);
    Task<bool> AssignAsync(int id, AssignWorkOrderRequest request, int operatorId);
    Task<bool> CompleteAsync(int id, CompleteWorkOrderRequest request, int operatorId);
    Task<bool> ProcessAsync(int id, int operatorId);
    Task<bool> CancelAsync(int id, int operatorId);
    Task<List<WorkOrderLogDto>> GetLogsAsync(int id);
    Task<SlaInfoDto?> GetSlaInfoAsync(int id);
    Task<List<OptionDto>> GetSystemsAsync();
    Task<List<OptionDto>> GetDevicesAsync(string? keyword = null);
}
