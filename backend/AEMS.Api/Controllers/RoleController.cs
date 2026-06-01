using AEMS.Core.DTOs;
using AEMS.Core.Entities;
using AEMS.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AEMS.Api.Controllers;

/// <summary>
/// 角色管理控制器
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RoleController : ControllerBase
{
    private readonly IRoleService _roleService;

    public RoleController(IRoleService roleService)
    {
        _roleService = roleService;
    }

    /// <summary>
    /// 获取所有角色
    /// </summary>
    [HttpGet]
    public async Task<ApiResponse<IEnumerable<SysRole>>> GetAll()
    {
        var roles = await _roleService.GetAllRolesAsync();
        return ApiResponse<IEnumerable<SysRole>>.Success(roles);
    }

    /// <summary>
    /// 获取角色详情
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ApiResponse<SysRole>> GetById(int id)
    {
        var role = await _roleService.GetRoleByIdAsync(id);
        if (role == null)
        {
            return ApiResponse<SysRole>.Fail(404, "角色不存在");
        }
        return ApiResponse<SysRole>.Success(role);
    }

    /// <summary>
    /// 创建角色
    /// </summary>
    [HttpPost]
    public async Task<ApiResponse<SysRole>> Create([FromBody] SysRole role)
    {
        try
        {
            var created = await _roleService.CreateRoleAsync(role);
            return ApiResponse<SysRole>.Success(created, "创建成功");
        }
        catch (ArgumentException ex)
        {
            return ApiResponse<SysRole>.Fail(400, ex.Message);
        }
    }

    /// <summary>
    /// 编辑角色
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ApiResponse<SysRole>> Update(int id, [FromBody] SysRole role)
    {
        try
        {
            var updated = await _roleService.UpdateRoleAsync(id, role);
            return ApiResponse<SysRole>.Success(updated, "更新成功");
        }
        catch (KeyNotFoundException ex)
        {
            return ApiResponse<SysRole>.Fail(404, ex.Message);
        }
        catch (ArgumentException ex)
        {
            return ApiResponse<SysRole>.Fail(400, ex.Message);
        }
    }

    /// <summary>
    /// 删除角色
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ApiResponse> Delete(int id)
    {
        try
        {
            await _roleService.DeleteRoleAsync(id);
            return ApiResponse.Success("删除成功");
        }
        catch (KeyNotFoundException ex)
        {
            return ApiResponse.Fail(404, ex.Message);
        }
        catch (ArgumentException ex)
        {
            return ApiResponse.Fail(400, ex.Message);
        }
    }
}
