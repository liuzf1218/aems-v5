using Microsoft.Data.Sqlite;
using System.Security.Cryptography;
using System.Text;

var conn = new SqliteConnection("Data Source=aems_v5.db");
conn.Open();

// Create role first
var cmd = conn.CreateCommand();
cmd.CommandText = "INSERT OR IGNORE INTO sys_role (name, code, description, created_at, updated_at) VALUES ('����Ա', 'admin', 'ϵͳ����Ա��ɫ', datetime('now'), datetime('now'))";
cmd.ExecuteNonQuery();

// Get role id
cmd.CommandText = "SELECT id FROM sys_role WHERE code = 'admin'";
var roleId = Convert.ToInt32(cmd.ExecuteScalar());

// Hash password
var password = "changeme";
var salt = "AEMS_Salt";
var hash = SHA256.HashData(Encoding.UTF8.GetBytes(password + salt));
var passwordHash = BitConverter.ToString(hash).Replace("-", "").ToLower();

// Insert admin user
cmd.CommandText = "INSERT OR IGNORE INTO sys_user (username, real_name, password_hash, role_id, phone, email, status, created_at, updated_at) VALUES ('admin', 'ϵͳ����Ա', @pwd, @roleId, '13800000000', 'admin@example.com', 1, datetime('now'), datetime('now'))";
cmd.Parameters.AddWithValue("@pwd", passwordHash);
cmd.Parameters.AddWithValue("@roleId", roleId);
cmd.ExecuteNonQuery();

Console.WriteLine("Admin user created successfully!");
conn.Close();
