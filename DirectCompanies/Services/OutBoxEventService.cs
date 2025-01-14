using DirectCompanies.Dtos;
using DirectCompanies.Enums;
using DirectCompanies.Models;
using Microsoft.EntityFrameworkCore;

namespace DirectCompanies.Services
{
    public class OutBoxEventService : IOutBoxEventService
    {
        private readonly ApplicationDbContext _context;

        public OutBoxEventService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task AddOutboxEvent(int EntityType,decimal EntityId, EventType EventType, string EventData)
        {
            var outboxEvent = new OutBoxEvent
            {
                EntityId = EntityId,
                EventType = (int)EventType,
                EntityType = EntityType,
                EventData = EventData, 
                CreatedAt = DateTime.UtcNow
            };
            await _context.OutBoxEvents.AddAsync(outboxEvent);
            await _context.SaveChangesAsync();
        }

        public async Task<string> GetPendingOutboxEvents(int EntityType)
        {
            var PendingEvents =await _context.OutBoxEvents.Where(c=>c.EntityType==EntityType && !c.IsSent).ToListAsync();

            var OutBoxEventDtos = PendingEvents.Select(e => new OutBoxEventDto
            {  OutBoxEventId=e.Id,
               EntityType= e.EntityType,
                EntityId= e.EntityId,
                EventType = e.EventType,
               EventData= e.EventData,
            }).ToList();
            return System.Text.Json.JsonSerializer.Serialize(OutBoxEventDtos);
        }
        public async Task ChangeOutBoxIsSent(List<int> ids)
        {
            var Employees = await _context.OutBoxEvents.Where(e => ids.Contains(e.Id)).ToListAsync();
            foreach (var Employee in Employees)
            {

                Employee.IsSent = true;
            }
            await _context.SaveChangesAsync();
        }
    }
}
