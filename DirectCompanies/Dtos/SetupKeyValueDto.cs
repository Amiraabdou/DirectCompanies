using DirectCompanies.Enums;

namespace DirectCompanies.Dtos
{
    public class SetupKeyValueDto
    {
        public decimal ErpKey { get; set; }
        public EventType EventType { get; set; }
        public Document DocumentType { get; set; }
        public string? ValueAr { get; set; }
        public string? ValueEn { get; set; }

    }
}
