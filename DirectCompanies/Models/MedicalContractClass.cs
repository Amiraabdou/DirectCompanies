using DirectCompanies.Dtos;
using DirectCompanies.Shared;
namespace DirectCompanies.Models
{
    public class MedicalContractClass : SetupKeyValue
    {
        public HashSet<Employee> Employees { get; set; }=new HashSet<Employee>();
    }
}
