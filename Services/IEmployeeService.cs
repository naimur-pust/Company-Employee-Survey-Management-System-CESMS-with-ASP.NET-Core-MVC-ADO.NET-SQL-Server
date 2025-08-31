using SmartHRIS.Models;

namespace SmartHRIS.Services
{
    public interface IEmployeeService
    {
        Task<int> CreateAsync(Employee emp);
        Task<Employee?> GetByPhoneAsync(string phone);
        Task<int> UpdateByPhoneAsync(Employee emp);
        Task<int> DeleteByPhoneAsync(string phone);
        Task<List<Employee>> GetAllAsync();
    }
}
