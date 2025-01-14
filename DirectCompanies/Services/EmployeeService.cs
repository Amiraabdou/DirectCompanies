
using DirectCompanies.Dtos;
using DirectCompanies.Models;
using Microsoft.EntityFrameworkCore;
using DirectCompanies.Localization;
using static DirectCompanies.Attributes.LocalizedValidation;
using DirectCompanies.Helper;
using DirectCompanies.Enums;
using Document = DirectCompanies.Enums.Document;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DirectCompanies.Services
{
    public class EmployeeService:IEmployeeService
    {
        private readonly ApplicationDbContext _context;
        private readonly IExcelService _excelService;
        private readonly ISetupKeyValueService _setupKeyValueService;
        private readonly IUserService _userService;
        private readonly IOutBoxEventService _outBoxEventService;

        public EmployeeService(ApplicationDbContext context,IExcelService excelService, ISetupKeyValueService setupKeyValueService,IUserService userService,IOutBoxEventService outBoxEventService)
        {
            _context = context;
            _excelService = excelService;
            _setupKeyValueService = setupKeyValueService;
            _userService = userService;
            _outBoxEventService = outBoxEventService;
        }
        public async Task<PagedResult<EmployeeDto>> GetAll(PagedResult<EmployeeDto>  PagedResult)
        {
            var User = await _userService.GetLoggedUser();

            var Employees = _context.Employees.Where(c=>c.MedicalCustomerId== User.Id).AsQueryable();
             if (!string.IsNullOrEmpty(PagedResult.SearchName))
                Employees = Employees.Where(c => c.Name.StartsWith(PagedResult.SearchName));

            Employees = Employees.Include(e => e.MedicalContractClass)
                         .Include(e => e.BeneficiaryType);

            var EmployeesDto = await Employees
                        .Skip((PagedResult.PageNumber - 1) * PagedResult.PageSize)
                        .Take(PagedResult.PageSize)
                        .Select(e => new EmployeeDto(e, PagedResult.Lang, User != null? User.CompanyName:null))
                         .ToListAsync();
            return new PagedResult<EmployeeDto>
            {
                Items = EmployeesDto,
                PageNumber = PagedResult.PageNumber,
                PageSize = PagedResult.PageSize,
                TotalItems = await Employees.CountAsync()
            };
        }

        public async Task<EmployeeDto> GetById(decimal? EmployeeId, string? lang)
        {
            var user = await _userService.GetLoggedUser();

            EmployeeDto EmployeeDto;

            var Employee = await _context.Employees.FirstOrDefaultAsync(c => c.Id == EmployeeId);
            EmployeeDto = new EmployeeDto(Employee, lang, user != null ? user.CompanyName : null);
            return EmployeeDto;

        }

        public async Task<string> Save(EmployeeDto EmployeeDto)
        {
            try
            {
                var Employee = new Employee(EmployeeDto);
                var user = await _userService.GetLoggedUser();
                Employee.MedicalCustomerId = user.Id;


                var ExistingEmployee = await _context.Employees
                    .FirstOrDefaultAsync(c => c.Id == EmployeeDto.Id);

                if (ExistingEmployee != null)
                    _context.Entry(ExistingEmployee).CurrentValues.SetValues(Employee);
                else
                     _context.Employees.Add(Employee);

                await _context.SaveChangesAsync();

                 if (ExistingEmployee == null)                
                    await SendToOutBox(Employee, EventType.Added);
                else if (ExistingEmployee != null)
                    await SendToOutBox(Employee, EventType.Modified);
                return "";
            }
            catch (Exception ex)
            {
              return ($" {Localizer.GetString("ExecutionError")} {ex.Message}");
                
            }
        }
   
        public async Task<string> Delete(decimal EmployeeId)
        {
            try
            {
                var ExistingEmployee = await _context.Employees
                    .FirstOrDefaultAsync(c => c.Id == EmployeeId);
                if (ExistingEmployee != null)
                {
                    _context.Remove(ExistingEmployee);

                    await _context.SaveChangesAsync();
                    await SendToOutBox(ExistingEmployee, EventType.Deleted);
                }
                return "";
            }
            catch (Exception ex)
            {
                return ($" {Localizer.GetString("ExecutionError")} {ex.Message}");
            }

        }

        public async Task<List<string>> Upload(string FileBase64)
        {
            var sheetList = _excelService.SheetToList(FileBase64)?.Select((c, i) => new
            {

                Name = c["Name"]?.ToString()?.TrimStart()?.TrimEnd(),
                IDCardNo = c["IDCardNo"]?.ToString()?.TrimStart()?.TrimEnd()?.ToUpper(),
                PhoneNumber = c["PhoneNumber"]?.ToString()?.TrimStart()?.TrimEnd(),
                Address = c["Address"]?.ToString()?.TrimStart()?.TrimEnd(),
                MedicalContractClassName = c["MedicalContractClass"]?.ToString()?.TrimStart()?.TrimEnd(),
                BeneficiaryTypeName = c["BeneficiaryType"]?.ToString()?.TrimStart()?.TrimEnd(),
                IsTemporarySuspension = c["IsTemporarySuspension"]?.ToString()?.TrimStart()?.TrimEnd(),
                SuspendFromDate = c["SuspendFromDate"]?.ToString()?.TrimStart()?.TrimEnd(),
                SuspendToDate = c["SuspendToDate"]?.ToString()?.TrimStart()?.TrimEnd(),
                Index = i
            })?.Distinct()?.ToList();

           var MedicalContractClasses = await _setupKeyValueService.GetKeyValueList<MedicalContractClass>("ar");
            var BeneficiaryTypes = await _setupKeyValueService.GetKeyValueList<BeneficiaryType>("ar");


            var errors = new List<string>();

            foreach (var item in sheetList)
            {

                var RowNum = item.Index + 2;
                decimal MedicalContractClassId = 0m;
                decimal BeneficiaryTypeId = 0m;
                Employee AddedEmployeeToSendToOutBox=null;
                Employee ModifiedEmployeeToSendToOutBox=null;
                bool IsTemporarySuspension = (string.Equals(item.IsTemporarySuspension, "TRUE", StringComparison.OrdinalIgnoreCase)) ? true : false;
                DateTime? SuspendFromDate=null;
                DateTime? SuspendToDate=null;
                var itemErrors = new List<string>();
                try {
                    if (string.IsNullOrEmpty(item.Name))
                        itemErrors.Add(
                    $"'{Localizer.GetString("Name")}' {Localizer.GetString("Required")}");

                    else
                    {
                        var regexAttribute = new CzRegularExpressionAttribute(@"^(\w+\s){2,}\w+$", "NameMoreThanThreeError");
                        if (!regexAttribute.IsValid(item.Name))
                        {
                            itemErrors.Add(
                          $" {Localizer.GetString("Name")} {item.Name} {Localizer.GetString("NameMoreThanThreeError")}");

                        }

                    }
                    if (string.IsNullOrEmpty(item.IDCardNo))
                        itemErrors.Add(
                        $"{Localizer.GetString("IDCardNo")} {Localizer.GetString("Required")}");

                    else
                    {
                        var regexAttribute = new CzRegularExpressionAttribute(@"^\d{14}$", "NationalIDError");
                        if (!regexAttribute.IsValid(item.IDCardNo))
                        {
                            itemErrors.Add(
                            $"{Localizer.GetString("IDCardNo")} {item.IDCardNo} {Localizer.GetString("NationalIDError")}");


                        }
                    }
                    if (string.IsNullOrEmpty(item.PhoneNumber))
                        itemErrors.Add(
                           $"{Localizer.GetString("PhoneNumber")} {Localizer.GetString("Required")}");
                    else
                    {
                        var regexAttribute = new CzRegularExpressionAttribute(@"^01[0-9]{9}$", "PhoneNumberError");
                        if (!regexAttribute.IsValid(item.PhoneNumber))
                        {
                            itemErrors.Add(
                          $"{Localizer.GetString("PhoneNumber")} {item.PhoneNumber} {Localizer.GetString("PhoneNumberError")}");


                        }

                    }
                   
                    if (IsTemporarySuspension)
                    {
                        if (string.IsNullOrEmpty(item.SuspendFromDate))
                            itemErrors.Add(
                               $"{Localizer.GetString("SuspendFromDate")} {Localizer.GetString("Required")}");
                        if (string.IsNullOrEmpty(item.SuspendToDate))
                            itemErrors.Add(
                               $"{Localizer.GetString("SuspendToDate")} {Localizer.GetString("Required")}");
                        SuspendFromDate= DateTime.ParseExact(item.SuspendFromDate, "M/d/yyyy h:mm:ss tt", null);
                        SuspendToDate= DateTime.ParseExact(item.SuspendToDate, "M/d/yyyy h:mm:ss tt", null);

                    }
                    if (string.IsNullOrEmpty(item.BeneficiaryTypeName))
                        itemErrors.Add(
                           $"{Localizer.GetString("BeneficiaryType")} {Localizer.GetString("Required")}");
                    else
                    {
                        BeneficiaryTypeId = BeneficiaryTypes.Where(c => c.Name == item.BeneficiaryTypeName).Select(c => c.Id).FirstOrDefault();
                        if (BeneficiaryTypeId==0m)
                            itemErrors.Add(
                                $"{Localizer.GetString("BeneficiaryType")} {item.BeneficiaryTypeName} {Localizer.GetString("NotExistInDatabase")}");
                    }
                    if (string.IsNullOrEmpty(item.MedicalContractClassName))
                        itemErrors.Add(
                           $"{Localizer.GetString("MedicalContractClass")} {Localizer.GetString("Required")}");
                    else
                    {
                        MedicalContractClassId = MedicalContractClasses.Where(c => c.Name == item.MedicalContractClassName).Select(c => c.Id).FirstOrDefault();
                        if (MedicalContractClassId == 0m)
                            itemErrors.Add(
                                $"{Localizer.GetString("MedicalContractClass")} {item.MedicalContractClassName} {Localizer.GetString("NotExistInDatabase")}");
                    }

                    var ExistingEmployee = _context.Employees.Where(c => c.IDCardNo == item.IDCardNo).FirstOrDefault();
                    var LoggedUser =await _userService.GetLoggedUser();
                    if (ExistingEmployee == null)
                    {
                        var Employee = new Employee() {Id=DecimalHelper.NewID(), Name = item.Name, PhoneNumber = item?.PhoneNumber, Address = item.Address, IDCardNo = item.IDCardNo, MedicalContractClassId = MedicalContractClassId, BeneficiaryTypeId = BeneficiaryTypeId, MedicalCustomerId = LoggedUser.Id,IsTemporarySuspension=IsTemporarySuspension,SuspendFromDate=SuspendFromDate,SuspendToDate=SuspendToDate };
                        _context.Employees.Add(Employee);
                        AddedEmployeeToSendToOutBox = Employee;
                    }
                    else
                    {
                        ExistingEmployee.Name = item.Name;
                        ExistingEmployee.PhoneNumber = item.PhoneNumber;
                        ExistingEmployee.Address = item.Address;
                        ExistingEmployee.BeneficiaryTypeId = BeneficiaryTypeId;
                        ExistingEmployee.MedicalContractClassId = MedicalContractClassId;
                        ExistingEmployee.IsTemporarySuspension = IsTemporarySuspension;
                        ExistingEmployee.SuspendToDate = SuspendToDate;
                        ExistingEmployee.SuspendFromDate = SuspendFromDate;

                        _context.Employees.Update(ExistingEmployee);
                        ModifiedEmployeeToSendToOutBox = ExistingEmployee;
                    }                    
                }
                catch (Exception ex)
                {
                    itemErrors.Add(ex.Message);
                }

                if (itemErrors.Any())
                {
                    errors.AddRange(itemErrors);
                }
                else
                {
                    _context.SaveChanges();
                    if (AddedEmployeeToSendToOutBox != null)
                        await SendToOutBox(AddedEmployeeToSendToOutBox, EventType.Added);
                    else if (ModifiedEmployeeToSendToOutBox != null)
                        await SendToOutBox(ModifiedEmployeeToSendToOutBox, EventType.Modified);

                }


            }

              return errors;
            }
      

        public async Task SendToOutBox(Employee Employee,EventType EventType)
        {
            var EmployeesToSendToErpDto =  new EmployeeToSendToErpDto(Employee);
            var EventData= System.Text.Json.JsonSerializer.Serialize(EmployeesToSendToErpDto);

            await _outBoxEventService.AddOutboxEvent((int)Document.Employee, Employee.Id, EventType, EventData);

        }

    }
}
