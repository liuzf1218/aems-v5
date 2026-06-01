using AEMS.Core.DTOs;

namespace AEMS.Core.Interfaces;

/// <summary>
/// 通用仓储接口
/// </summary>
/// <typeparam name="T">实体类型</typeparam>
public interface IRepository<T> where T : class
{
    /// <summary>
    /// 获取所有记录
    /// </summary>
    Task<IEnumerable<T>> GetAllAsync();

    /// <summary>
    /// 根据ID获取记录
    /// </summary>
    Task<T?> GetByIdAsync(int id);

    /// <summary>
    /// 新增记录
    /// </summary>
    Task<T> AddAsync(T entity);

    /// <summary>
    /// 更新记录
    /// </summary>
    Task UpdateAsync(T entity);

    /// <summary>
    /// 删除记录（软删除）
    /// </summary>
    Task DeleteAsync(int id);

    /// <summary>
    /// 分页查询
    /// </summary>
    Task<PagedResult<T>> GetPagedAsync(PagedRequest request);
}
