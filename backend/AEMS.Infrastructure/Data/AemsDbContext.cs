using AEMS.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace AEMS.Infrastructure.Data;

/// <summary>
/// AEMS数据库上下文
/// </summary>
public class AemsDbContext : DbContext
{
    public AemsDbContext(DbContextOptions<AemsDbContext> options) : base(options) { }

    #region 系统管理
    public DbSet<SysUser> SysUsers => Set<SysUser>();
    public DbSet<SysRole> SysRoles => Set<SysRole>();
    public DbSet<SysLog> SysLogs => Set<SysLog>();
    public DbSet<SysDict> SysDicts => Set<SysDict>();
    #endregion

    #region 设备管理
    public DbSet<Equipment> Equipments => Set<Equipment>();
    public DbSet<EquipmentType> EquipmentTypes => Set<EquipmentType>();
    public DbSet<Subsystem> Subsystems => Set<Subsystem>();
    #endregion

    #region 软件管理
    public DbSet<Software> Softwares => Set<Software>();
    public DbSet<SoftwareVersion> SoftwareVersions => Set<SoftwareVersion>();
    public DbSet<SoftwareInstance> SoftwareInstances => Set<SoftwareInstance>();
    #endregion

    #region 工单管理
    public DbSet<Workorder> Workorders => Set<Workorder>();
    public DbSet<WorkorderLog> WorkorderLogs => Set<WorkorderLog>();
    public DbSet<FaultType> FaultTypes => Set<FaultType>();
    #endregion

    #region 维护管理
    public DbSet<MaintenancePlan> MaintenancePlans => Set<MaintenancePlan>();
    public DbSet<MaintenanceTask> MaintenanceTasks => Set<MaintenanceTask>();
    public DbSet<MaintenanceItem> MaintenanceItems => Set<MaintenanceItem>();
    #endregion

    #region 备件管理
    public DbSet<Sparepart> Spareparts => Set<Sparepart>();
    public DbSet<StockInRecord> StockInRecords => Set<StockInRecord>();
    public DbSet<StockOutRecord> StockOutRecords => Set<StockOutRecord>();
    #endregion

    #region 机房管理
    public DbSet<Building> Buildings => Set<Building>();
    public DbSet<Room> Rooms => Set<Room>();
    public DbSet<Cabinet> Cabinets => Set<Cabinet>();
    #endregion

    #region 文档管理
    public DbSet<Document> Documents => Set<Document>();
    public DbSet<DocumentVersion> DocumentVersions => Set<DocumentVersion>();
    #endregion

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // 软删除过滤器通过实体上的 IsDeleted 属性手动实现

        // SysUser 唯一索引
        modelBuilder.Entity<SysUser>()
            .HasIndex(u => u.Username)
            .IsUnique();

        // SysRole 唯一索引
        modelBuilder.Entity<SysRole>()
            .HasIndex(r => r.Code)
            .IsUnique();

        // Building 唯一索引
        modelBuilder.Entity<Building>()
            .HasIndex(b => b.Code)
            .IsUnique();

        // Room -> Building 外键
        modelBuilder.Entity<Room>()
            .HasOne(r => r.Building)
            .WithMany(b => b.Rooms)
            .HasForeignKey(r => r.BuildingId)
            .OnDelete(DeleteBehavior.SetNull);

        // Equipment -> Room 外键
        modelBuilder.Entity<Equipment>()
            .HasOne(e => e.Room)
            .WithMany(r => r.Equipments)
            .HasForeignKey(e => e.RoomId)
            .OnDelete(DeleteBehavior.SetNull);

        // MaintenancePlan -> Equipment 外键
        modelBuilder.Entity<MaintenancePlan>()
            .HasOne(mp => mp.Equipment)
            .WithMany()
            .HasForeignKey(mp => mp.EquipmentId)
            .OnDelete(DeleteBehavior.SetNull);

        // Software -> Equipment 外键
        modelBuilder.Entity<Software>()
            .HasOne(s => s.Equipment)
            .WithMany()
            .HasForeignKey(s => s.EquipmentId)
            .OnDelete(DeleteBehavior.SetNull);

        // Sparepart -> Subsystem 外键
        modelBuilder.Entity<Sparepart>()
            .HasOne(sp => sp.Subsystem)
            .WithMany()
            .HasForeignKey(sp => sp.SubsystemId)
            .OnDelete(DeleteBehavior.SetNull);

        // Equipment 唯一索引
        modelBuilder.Entity<Equipment>()
            .HasIndex(e => e.Code)
            .IsUnique();

        // Workorder 唯一索引
        modelBuilder.Entity<Workorder>()
            .HasIndex(w => w.WorkorderNo)
            .IsUnique();

        // Sparepart 唯一索引
        modelBuilder.Entity<Sparepart>()
            .HasIndex(s => s.Code)
            .IsUnique();

        // EquipmentType 自引用关系（树形结构）
        modelBuilder.Entity<EquipmentType>()
            .HasOne(et => et.Parent)
            .WithMany(et => et.Children)
            .HasForeignKey(et => et.ParentId)
            .OnDelete(DeleteBehavior.Restrict);

        // Subsystem -> EquipmentType（分类）
        modelBuilder.Entity<Subsystem>()
            .HasOne(s => s.Category)
            .WithMany(et => et.Subsystems)
            .HasForeignKey(s => s.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        // Equipment -> Subsystem（系统）
        modelBuilder.Entity<Equipment>()
            .HasOne(e => e.Subsystem)
            .WithMany(s => s.Equipments)
            .HasForeignKey(e => e.SubsystemId)
            .OnDelete(DeleteBehavior.Restrict);

        // SysUser -> SysRole 关系
        modelBuilder.Entity<SysUser>()
            .HasOne(u => u.Role)
            .WithMany(r => r.Users)
            .HasForeignKey(u => u.RoleId)
            .OnDelete(DeleteBehavior.SetNull);
    }

    /// <summary>
    /// 重写SaveChanges，自动填充更新时间
    /// </summary>
    public override int SaveChanges()
    {
        UpdateTimestamps();
        return base.SaveChanges();
    }

    /// <summary>
    /// 重写SaveChangesAsync，自动填充更新时间
    /// </summary>
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateTimestamps();
        return base.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// 更新时间戳
    /// </summary>
    private void UpdateTimestamps()
    {
        var entries = ChangeTracker.Entries<BaseEntity>();
        foreach (var entry in entries)
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = DateTime.Now;
                    entry.Entity.UpdatedAt = DateTime.Now;
                    break;
                case EntityState.Modified:
                    entry.Entity.UpdatedAt = DateTime.Now;
                    break;
            }
        }
    }
}
