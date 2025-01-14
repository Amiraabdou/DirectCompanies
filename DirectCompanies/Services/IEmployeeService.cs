using DirectCompanies.Dtos;
using DirectCompanies.Models;
using System.Threading.Tasks;

namespace DirectCompanies.Services
{
    public interface IEmployeeService
    {
       public Task<PagedResult<EmployeeDto>> GetAllEmployees(PagedResult<EmployeeDto> pagedResult);
       public Task<EmployeeDto> GetEmployee(decimal? EmployeeId,string? lang);
       public Task<string> SaveEmployee(EmployeeDto EmployeeDto);
       public Task<string> DeleteEmployee(EmployeeDto EmployeeDto);
       public Task<List<string>> UploadEmployees(string FileBase64);

    }
}
