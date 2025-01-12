using DirectCompanies.Dtos;
using DirectCompanies.Enums;

namespace DirectCompanies.Services
{
    public interface IOutBoxEventService
    {
        Task AddOutboxEvent(int EntityType,decimal EntityID, int EventType ,string EventData);
        Task<string> GetPendingOutboxEvents(int EntityType);
        public Task ChangeOutBoxIsSent(List<int> ids);


    }
}
