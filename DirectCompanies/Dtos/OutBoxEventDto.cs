namespace DirectCompanies.Dtos
{
    public class OutBoxEventDto
    {
        public decimal OutBoxEventId { get; set; }

        public decimal EntityId { get; set; }
        public int EventType { get; set; }
        public int EntityType { get; set; }
        public string? EventData { get; set; }
    }
}
