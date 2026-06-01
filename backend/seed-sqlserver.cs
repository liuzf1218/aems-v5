using System.Data.SqlClient;

var conn = new SqlConnection("Server=localhost,1433;Database=AEMS;User Id=sa;Password=YourStrongPassword;TrustServerCertificate=true");
conn.Open();
var cmd = conn.CreateCommand();
cmd.CommandText = @"
SET IDENTITY_INSERT sys_role ON;
IF NOT EXISTS (SELECT 1 FROM sys_role WHERE Id = 1)
    INSERT INTO sys_role (Id, Name, Code, Remark, CreatedAt, UpdatedAt, IsDeleted) VALUES (1, N'管理员', 'admin', N'系统管理员角色', GETDATE(), GETDATE(), 0);
SET IDENTITY_INSERT sys_role OFF;

SET IDENTITY_INSERT sys_user ON;
IF NOT EXISTS (SELECT 1 FROM sys_user WHERE Id = 1)
    INSERT INTO sys_user (Id, Username, PasswordHash, RealName, Phone, Email, RoleId, IsActive, CreatedAt, UpdatedAt, IsDeleted) VALUES (1, 'admin', 'changeme', N'系统管理员', '13800000000', 'admin@example.com', 1, 1, GETDATE(), GETDATE(), 0);
SET IDENTITY_INSERT sys_user OFF;
";
var rows = cmd.ExecuteNonQuery();
Console.WriteLine($"Done. Affected rows: {rows}");
conn.Close();
