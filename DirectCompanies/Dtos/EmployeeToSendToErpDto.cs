using DirectCompanies.Models;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using DocumentFormat.OpenXml.Wordprocessing;

namespace DirectCompanies.Dtos
{
    public class EmployeeToSendToErpDto
    {
        public EmployeeToSendToErpDto()
        {
            
        }
        public EmployeeToSendToErpDto(Employee Employee)
        {
            Id = Employee.Id;
            Name = Employee.Name;
            Address = Employee.Address;
            PhoneNumber = Employee.PhoneNumber;
            IDCardNo = Employee.IDCardNo;
            BeneficiaryTypeId =(decimal) Employee.BeneficiaryTypeId;
            MedicalContractClassId =(decimal) Employee.MedicalContractClassId;
            MedicalCustomerId = Employee.MedicalCustomerId;

        }
        public decimal Id { get; set; }
        public string Name { get; set; }
        public string? Address { get; set; }
        public string PhoneNumber { get; set; }
        public string IDCardNo { get; set; }
        public decimal BeneficiaryTypeId { get; set; }
        public decimal MedicalContractClassId { get; set; }
        public decimal MedicalCustomerId { get; set; }



    }
}
