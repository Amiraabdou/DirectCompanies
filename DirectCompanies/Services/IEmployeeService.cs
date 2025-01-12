using DirectCompanies.Dtos;
using DirectCompanies.Models;
using System.Threading.Tasks;

namespace DirectCompanies.Services
{
    public interface IEmployeeService
    {
       //public Task<List<EmployeeDto>> GetAllEmployees(string? EmployeeName,string? Lang);
       public Task<PagedResult<EmployeeDto>> GetAllEmployees(string? EmployeeName,string? Lang, int PageNumber, int PageSize);
       public Task SaveEmployee(EmployeeDto EmployeeDto);
       public Task DeleteEmployee(EmployeeDto EmployeeDto);
       public Task<List<string>> UploadEmployees(string FileBase64);

    }
}
