using Microsoft.AspNetCore.Identity;

namespace DirectCompanies.Models
{
    public class ApplicationUser : IdentityUser<decimal>
    {
        public string? CompanyName { get; set; }
        public HashSet<Employee> Employees { get; set; }=new HashSet<Employee>();
    }
}
