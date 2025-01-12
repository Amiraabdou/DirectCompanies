using DirectCompanies.Dtos;
using DirectCompanies.Shared;
using DirectCompanies.Enums;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;
using System.Reflection;
using DirectCompanies.Models;

namespace DirectCompanies.Services
{
    public class SetupKeyValueService : ISetupKeyValueService
    {
        private Dictionary<Document, Type> _Documents = new Dictionary<Document, Type>
        {
            {Document.MedicalContractClass,typeof(MedicalContractClass)},
            {Document.BeneficiaryType,typeof(BeneficiaryType)},
            
        };
        private readonly ApplicationDbContext _context;

        public SetupKeyValueService(ApplicationDbContext context)
        {
            _context=context;
        }
        public async Task<List<KeyValue>> GetKeyValueList<TSetup>(string lang) where TSetup : SetupKeyValue
        {
            var query = _context.Set<TSetup>().AsQueryable();

            return await query.Select(e => PrepareKeyValue(e, lang)).ToListAsync();
        }
        private static KeyValue PrepareKeyValue(SetupKeyValue setupKeyValue, string lang)
        {
            var keyValue = new KeyValue() { Id = setupKeyValue.Id };
            if (lang == null || lang == "ar")
                keyValue.Name = setupKeyValue?.ValueAr ?? setupKeyValue?.ValueEn ?? "";
            else
                keyValue.Name = setupKeyValue?.ValueEn ?? setupKeyValue?.ValueAr ?? "";
            return keyValue;
        }
        public async Task HandleSetup(List<SetupKeyValueDto> dtos)
        {
            var method = typeof(SetupKeyValueService).GetMethod(nameof(HandleSetup), BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var dto in dtos)
            {
                var setupType = _Documents[dto.DocumentType];
                var handleMethod = method.MakeGenericMethod(setupType);
                await(Task)handleMethod.Invoke(this, [dto]);
            }
            await _context.SaveChangesAsync();
        }
        private async Task HandleSetup<TSetup>(SetupKeyValueDto dto) where TSetup : SetupKeyValue, new()
        {

            var ExistingEntity = await _context.Set<TSetup>().FindAsync(dto.ErpKey);
            try
            {

                if (ExistingEntity == null)
                {
                    var entityToAdd = new TSetup() { Id = dto.ErpKey, ValueAr = dto.ValueAr, ValueEn = dto.ValueEn };
                    _context.Set<TSetup>().Add(entityToAdd);

                }
                else
                {
                    if (dto.EventType == EventType.Modified)
                    {
                        ExistingEntity.ValueAr = dto.ValueAr;
                        ExistingEntity.ValueEn = dto.ValueEn;
                        _context.Set<TSetup>().Update(ExistingEntity);
                    }
                    else if (dto.EventType == EventType.Deleted)
                    {
                        _context.Set<TSetup>().Remove(ExistingEntity);
                    }

                }

            }
            catch (Exception ex)
            {

            }

         }

        
    }
}
