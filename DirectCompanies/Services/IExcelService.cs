namespace DirectCompanies.Services
{
    public interface IExcelService
    {
        List<Dictionary<string, object>> SheetToList(string stream, string sheetName = null);
    }
}
