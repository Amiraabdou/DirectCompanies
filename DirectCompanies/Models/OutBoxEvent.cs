using DirectCompanies.Enums;

namespace DirectCompanies.Models
{
    public class OutBoxEvent
    {
        public int Id { get; set; }
        public int EntityType { get; set; }
        public decimal EntityId { get; set; }
        public int EventType { get; set; }  
        public string? EventData { get; set; }
        public bool IsSent { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
