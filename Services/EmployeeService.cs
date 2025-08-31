using System.Data;
using SmartHRIS.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;

namespace SmartHRIS.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly string _conn;

        public EmployeeService(IConfiguration configuration)
        {
            _conn = configuration.GetConnectionString("DefaultConnection")
                    ?? throw new InvalidOperationException("Connection string not found.");
        }

        public async Task<int> CreateAsync(Employee emp)
        {
            const string sql = @"
                                INSERT INTO Employees (Name, Email, PhoneNumber, City, Country, ZipCode)
                                VALUES (@Name, @Email, @PhoneNumber, @City, @Country, @ZipCode);
                                SELECT CAST(SCOPE_IDENTITY() AS INT);";

            using var conn = new SqlConnection(_conn);
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Name", emp.Name);
            cmd.Parameters.AddWithValue("@Email", emp.Email);
            cmd.Parameters.AddWithValue("@PhoneNumber", emp.PhoneNumber);
            cmd.Parameters.AddWithValue("@City", (object?)emp.City ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Country", (object?)emp.Country ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ZipCode", (object?)emp.ZipCode ?? DBNull.Value);

            await conn.OpenAsync();
            var result = await cmd.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }

        public async Task<Employee?> GetByPhoneAsync(string phone)
        {
            const string sql = @"SELECT TOP 1 * FROM Employees WHERE PhoneNumber=@PhoneNumber;";

            using var conn = new SqlConnection(_conn);
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@PhoneNumber", phone);

            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SingleRow);
            if (await reader.ReadAsync())
            {
                return new Employee
                {
                    EmployeeId = reader.GetInt32(reader.GetOrdinal("EmployeeId")),
                    Name = reader.GetString(reader.GetOrdinal("Name")),
                    Email = reader.GetString(reader.GetOrdinal("Email")),
                    PhoneNumber = reader.GetString(reader.GetOrdinal("PhoneNumber")),
                    City = reader.IsDBNull(reader.GetOrdinal("City")) ? null : reader.GetString(reader.GetOrdinal("City")),
                    Country = reader.IsDBNull(reader.GetOrdinal("Country")) ? null : reader.GetString(reader.GetOrdinal("Country")),
                    ZipCode = reader.IsDBNull(reader.GetOrdinal("ZipCode")) ? null : reader.GetString(reader.GetOrdinal("ZipCode"))
                };
            }
            return null;
        }

        public async Task<int> UpdateByPhoneAsync(Employee emp)
        {
            const string sql = @"
UPDATE Employees
   SET Name=@Name, Email=@Email, City=@City, Country=@Country, ZipCode=@ZipCode
 WHERE PhoneNumber=@PhoneNumber;";

            using var conn = new SqlConnection(_conn);
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Name", emp.Name);
            cmd.Parameters.AddWithValue("@Email", emp.Email);
            cmd.Parameters.AddWithValue("@City", (object?)emp.City ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Country", (object?)emp.Country ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ZipCode", (object?)emp.ZipCode ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@PhoneNumber", emp.PhoneNumber);

            await conn.OpenAsync();
            return await cmd.ExecuteNonQueryAsync();
        }

        public async Task<int> DeleteByPhoneAsync(string phone)
        {
            const string sql = @"DELETE FROM Employees WHERE PhoneNumber=@PhoneNumber;";

            using var conn = new SqlConnection(_conn);
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@PhoneNumber", phone);

            await conn.OpenAsync();
            return await cmd.ExecuteNonQueryAsync();
        }

        public async Task<List<Employee>> GetAllAsync()
        {
            const string sql = @"SELECT * FROM Employees ORDER BY EmployeeId DESC;";
            var list = new List<Employee>();

            using var conn = new SqlConnection(_conn);
            using var cmd = new SqlCommand(sql, conn);
            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(new Employee
                {
                    EmployeeId = reader.GetInt32(reader.GetOrdinal("EmployeeId")),
                    Name = reader.GetString(reader.GetOrdinal("Name")),
                    Email = reader.GetString(reader.GetOrdinal("Email")),
                    PhoneNumber = reader.GetString(reader.GetOrdinal("PhoneNumber")),
                    City = reader.IsDBNull(reader.GetOrdinal("City")) ? null : reader.GetString(reader.GetOrdinal("City")),
                    Country = reader.IsDBNull(reader.GetOrdinal("Country")) ? null : reader.GetString(reader.GetOrdinal("Country")),
                    ZipCode = reader.IsDBNull(reader.GetOrdinal("ZipCode")) ? null : reader.GetString(reader.GetOrdinal("ZipCode"))
                });
            }
            return list;
        }
    }
}
