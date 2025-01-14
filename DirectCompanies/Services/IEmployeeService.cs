using DirectCompanies.Dtos;
using DirectCompanies.Models;
using System;
using System.Threading.Tasks;

namespace DirectCompanies.Services
{
    public interface IEmployeeService
    {
       public Task<PagedResult<EmployeeDto>> GetAll(PagedResult<EmployeeDto> pagedResult);
       public Task<EmployeeDto> GetById(decimal? EmployeeId,string? lang);
       public Task<string> Save(EmployeeDto EmployeeDto);
       public Task<string> Delete(decimal EmployeeId);
       public Task<List<string>> Upload(string FileBase64);

    }
}
