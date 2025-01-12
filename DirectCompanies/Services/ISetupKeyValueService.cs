using DirectCompanies.Shared;
using DirectCompanies.Dtos;
using DirectCompanies.Enums;
using DirectCompanies.Models;

namespace DirectCompanies.Services
{
    public interface ISetupKeyValueService
    {
        Task HandleSetup(List<SetupKeyValueDto> dtos);
        Task<List<KeyValue>> GetKeyValueList<TSetup>(string? lang) where TSetup : SetupKeyValue;
    }
}
