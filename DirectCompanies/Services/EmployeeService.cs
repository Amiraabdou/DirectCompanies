using CsvHelper.Configuration;
using CsvHelper;
using DirectCompanies.Dtos;
using DirectCompanies.Models;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using DocumentFormat.OpenXml.Office2016.Excel;
using DirectCompanies.Localization;
using DirectCompanies.Attributes;
using static DirectCompanies.Attributes.LocalizedValidation;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using DirectCompanies.Helper;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Hosting;
using DirectCompanies.Enums;
using Document = DirectCompanies.Enums.Document;
using DocumentFormat.OpenXml.InkML;

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
        public async Task<PagedResult<EmployeeDto>> GetAllEmployees(string? EmployeeName, string? Lang, int PageNumber, int PageSize)
        {
            var user = await _userService.GetLoggedUser();

            var Query = _context.Employees.Where(c=>c.MedicalCustomerId==user.Id).AsQueryable();          
            if (!string.IsNullOrEmpty(EmployeeName))
                Query = Query.Where(c => c.Name.StartsWith(EmployeeName));
   
            Query = Query.Include(e => e.MedicalContractClass)
                         .Include(e => e.BeneficiaryType);

            var EmployeesDto = await Query
                        .Skip((PageNumber - 1) * PageSize)
                        .Take(PageSize)
                        .Select(e => new EmployeeDto(e, Lang, user.CompanyName))
                         .ToListAsync();
            return new PagedResult<EmployeeDto>
            {
                Items = EmployeesDto,
                PageNumber = PageNumber,
                PageSize = PageSize,
                TotalItems = await Query.CountAsync()
            };
        }

        public async Task SaveEmployee(EmployeeDto EmployeeDto)
        {
            try
            {
                var Employee = new Employee(EmployeeDto);
                var user = await _userService.GetLoggedUser();
                Employee.MedicalCustomerId = user.Id;


                var ExistingEmployee = await _context.Employees
                    .FirstOrDefaultAsync(c => c.Id == EmployeeDto.Id);

                if (ExistingEmployee != null)
                {
                    _context.Entry(ExistingEmployee).CurrentValues.SetValues(Employee);

                }
                else
                {
                     _context.Employees.Add(Employee);
                }

                await _context.SaveChangesAsync();

                 if (ExistingEmployee == null)                
                    await SendToOutBox(Employee, (int)EventType.Added);
                else if (ExistingEmployee != null)
                    await SendToOutBox(Employee, (int)EventType.Modified);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"An error occurred while saving the employee: {ex.Message}");
            }
        }
   
        public async Task DeleteEmployee(EmployeeDto EmployeeDto)
        {
            var ExistingEmployee = await _context.Employees
                .FirstOrDefaultAsync(c => c.Id == EmployeeDto.Id);
            if (ExistingEmployee != null)
            {
                _context.Remove(ExistingEmployee);
             
                await _context.SaveChangesAsync();
               await SendToOutBox(ExistingEmployee, (int)EventType.Deleted);
            }
            else
            {
                Console.Error.WriteLine("Employee not found.");
            }
        }

        public async Task<List<string>> UploadEmployees(string FileBase64)
        {
            var sheetList = _excelService.SheetToList(FileBase64)?.Select((c, i) => new
            {

                Name = c["Name"]?.ToString()?.TrimStart()?.TrimEnd(),
                IDCardNo = c["IDCardNo"]?.ToString()?.TrimStart()?.TrimEnd()?.ToUpper(),
                PhoneNumber = c["PhoneNumber"]?.ToString()?.TrimStart()?.TrimEnd(),
                Address = c["Address"]?.ToString()?.TrimStart()?.TrimEnd(),
                MedicalContractClassName = c["MedicalContractClass"]?.ToString()?.TrimStart()?.TrimEnd(),
                BeneficiaryTypeName = c["BeneficiaryType"]?.ToString()?.TrimStart()?.TrimEnd(),
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
                var itemErrors = new List<string>();
                try {
                    if (string.IsNullOrEmpty(item.Name))
                        itemErrors.Add(
                    $"'{Localizer.GetString("Name")}' {Localizer.GetString("Required")}");

                    else
                    {
                        var regexAttribute = new LocalizedRegularExpressionAttribute(@"^(\w+\s){2,}\w+$", "NameMoreThanThreeError");
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
                        var regexAttribute = new LocalizedRegularExpressionAttribute(@"^\d{14}$", "NationalIDError");
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
                        var regexAttribute = new LocalizedRegularExpressionAttribute(@"^01[0-9]{9}$", "PhoneNumberError");
                        if (!regexAttribute.IsValid(item.PhoneNumber))
                        {
                            itemErrors.Add(
                          $"{Localizer.GetString("PhoneNumber")} {item.PhoneNumber} {Localizer.GetString("PhoneNumberError")}");


                        }

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
                        var Employee = new Employee() {Id=DecimalHelper.NewID(), Name = item.Name, PhoneNumber = item?.PhoneNumber, Address = item.Address, IDCardNo = item.IDCardNo, MedicalContractClassId = MedicalContractClassId, BeneficiaryTypeId = BeneficiaryTypeId, MedicalCustomerId = LoggedUser.Id };
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
                        await SendToOutBox(AddedEmployeeToSendToOutBox, (int)EventType.Added);
                    else if (ModifiedEmployeeToSendToOutBox != null)
                        await SendToOutBox(ModifiedEmployeeToSendToOutBox, (int)EventType.Modified);

                }


            }

              return errors;
            }
      

        public async Task SendToOutBox(Employee Employee,int EventType)
        {
            var EmployeesToSendToErpDto =  new EmployeeToSendToErpDto(Employee);
            var EventData= System.Text.Json.JsonSerializer.Serialize(EmployeesToSendToErpDto);

            await _outBoxEventService.AddOutboxEvent((int)Document.Employee, Employee.Id, EventType, EventData);

        }
    }
}
