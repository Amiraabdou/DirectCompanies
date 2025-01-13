using CsvHelper.Configuration.Attributes;
using DirectCompanies.Dtos;
using System.ComponentModel.DataAnnotations;

namespace DirectCompanies.Models
{
    public class Employee
    {
        public Employee()
        {
            
        }
        public Employee(EmployeeDto EmployeeDto)
        {
            Id = EmployeeDto.Id;
            Name = EmployeeDto.Name;
            IDCardNo = EmployeeDto.IDCardNo;
            PhoneNumber = EmployeeDto.PhoneNumber;
            Address = EmployeeDto.Address;
            BeneficiaryTypeId =(decimal)EmployeeDto.BeneficiaryTypeId;
            MedicalContractClassId =(decimal) EmployeeDto.MedicalContractClassId;
            IsPermanentSuspension= EmployeeDto.IsPermanentSuspension;
            IsTemporarySuspension= EmployeeDto.IsTemporarySuspension;
            SuspendFromDate = EmployeeDto.SuspendFromDate;
            SuspendToDate = EmployeeDto.SuspendToDate;
          
        }

        [Key]
        public decimal Id { get; set; }
        [Required]

        public string Name { get; set; }
        public string? Address { get; set; }
        public string PhoneNumber { get; set; }
        [Required]
        public string IDCardNo { get; set; }
        public decimal BeneficiaryTypeId { get; set; }
        public decimal MedicalContractClassId { get; set; }

        public MedicalContractClass MedicalContractClass { get; set; }
        public BeneficiaryType BeneficiaryType { get; set; }
        public ApplicationUser ApplicationUser { get; set; }

        public decimal MedicalCustomerId { get; set; }
        public bool IsPermanentSuspension { get; set; }
        public bool IsTemporarySuspension { get; set; }
        public DateTime? SuspendFromDate { get; set; }
        public DateTime? SuspendToDate { get; set; }
        





    }
}
