using DirectCompanies.Enums;
using System.Text.Json.Serialization;

namespace DirectCompanies.Dtos
{
    public class ApplicationUserDto
    {
        [JsonPropertyName("MedicalCustomerID")]

        public decimal UserId { get; set; }
        public EventType EventType { get; set; }
        public string? CompanyName { get; set; }
        public string? UserName { get; set; }
        public string? Password { get; set; }
    }
}
