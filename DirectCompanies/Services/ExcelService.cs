
using ClosedXML.Excel;

namespace DirectCompanies.Services
{
    public class ExcelService : IExcelService
    {
        public List<Dictionary<string, object>> SheetToList(string File, string sheetName = null)
        {
            var bytes = Convert.FromBase64String(File);
            var stream = new StreamContent(new MemoryStream(bytes));

            using var package = new XLWorkbook(stream.ReadAsStream());
            var worksheet = string.IsNullOrEmpty(sheetName)
                ? package.Worksheet(1)
                : package.Worksheet(sheetName);
            var columnNames = new List<string>();

            var FirstRow = true;
            //Range for reading the cells based on the last cell used.  
            var readRange = "1:1";
            var result = new List<Dictionary<string, object>>();
            foreach (var row in worksheet.RowsUsed())
                //If Reading the First Row (used) then add them as column name  
                if (FirstRow)
                {
                    //Checking the Last cellused for column generation in datatable  
                    readRange = string.Format("{0}:{1}", 1, row.LastCellUsed().Address.ColumnNumber);
                    foreach (var cell in row.Cells(readRange)) columnNames.Add(cell.Value.ToString());
                    FirstRow = false;
                }
                else
                {
                    var dic = new Dictionary<string, object>();
                    var cellIndex = 0;
                    foreach (var cell in row.Cells(readRange))
                    {
                        var ColumnName = columnNames.ElementAt(cellIndex);
                        dic.Add(ColumnName, cell.Value.ToString());
                        cellIndex++;
                    }

                    if (dic.Values.Any(c => !string.IsNullOrEmpty(c?.ToString())))
                        result.Add(dic);
                }

            return result.ToList();
        }
    }
}
