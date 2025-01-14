using DirectCompanies.Models;
using System.ComponentModel.DataAnnotations;
using DirectCompanies.Helper;
using DirectCompanies.Attributes;
using Microsoft.AspNetCore.Components.Forms;
using System.Resources;
using static DirectCompanies.Attributes.LocalizedValidation;

namespace DirectCompanies.Dtos
{
    public class EmployeeDto
    {
       
        public  EmployeeDto(){}
        public EmployeeDto(Employee Employee,string? Lang,string? CompanyName)
        {
            Id = Employee.Id;
            Name= Employee.Name;
            Address = Employee.Address;
            PhoneNumber = Employee.PhoneNumber;
            IDCardNo = Employee.IDCardNo;
            BeneficiaryTypeName=( Lang == null || Lang == "ar"?Employee.BeneficiaryType.ValueAr: Employee.BeneficiaryType.ValueEn)?? Employee.BeneficiaryType.ValueAr;
            BeneficiaryTypeId=Employee.BeneficiaryTypeId;
            MedicalContractClassName=  Employee.MedicalContractClass.ValueAr ;
            MedicalContractClassId=Employee.MedicalContractClassId;
            this.CompanyName=CompanyName;
            IsPermanentSuspension= Employee.IsPermanentSuspension;
            IsTemporarySuspension= Employee.IsTemporarySuspension;
            SuspendFromDate = Employee.SuspendFromDate;
            SuspendToDate = Employee.SuspendToDate;
        }

        public decimal Id { get; set; }
        [CzRequired]
        [CzRegularExpression(@"^(\w+\s){2,}\w+$", "NameMoreThanThreeError")]


        public string Name { get; set; }
        public string? Address { get; set; }
        [CzRegularExpression(@"^01[0-9]{9}$", "PhoneNumberError")]

        [CzRequired]

        public string PhoneNumber { get; set; }
        [CzRegularExpression(@"^\d{14}$", "NationalIDError")]
        [CzRequired]

        public string IDCardNo { get; set; }
        public string CompanyName { get; set; }
        public string BeneficiaryTypeName { get; set; }
        [CzRequired]

        public decimal? BeneficiaryTypeId { get; set; }

        public string MedicalContractClassName { get; set; }
        [CzRequired]

        public decimal? MedicalContractClassId { get; set; }
        public bool IsPermanentSuspension { get; set; }
        public bool IsTemporarySuspension { get; set; }
        [CzRequiredIfSuspended("SuspensionToRequiredErrorMessage")]


        public DateTime? SuspendFromDate { get; set; }
        [CzRequiredIfSuspended("SuspensionToRequiredErrorMessage")]

        public DateTime? SuspendToDate { get; set; }
    }
}
