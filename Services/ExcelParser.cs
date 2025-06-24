using Aspose.Cells;
using System.Collections.Generic;

namespace ApiTask.Services
{
    public class ExcelParser
    {
        public List<string> GetDataFromFile(string filename)
        {
            WorksheetCollection collection = GetWorksheets(filename);
            return GetDataFromCells(collection);
        }

        private WorksheetCollection GetWorksheets(string filename)
        {
            Workbook workbook = new Workbook(filename);
            WorksheetCollection collection = workbook.Worksheets;
            return collection;
        }

        private List<string> GetDataFromCells(WorksheetCollection collection)
        {
            List<string> data = new List<string>();

            for (int worksheetIndex = 0; worksheetIndex < collection.Count; worksheetIndex++)
            {
                Worksheet worksheet = collection[worksheetIndex];
                GetDataFromSheet(data, worksheet);
            }

            return data;
        }

        private void GetDataFromSheet(List<string> data, Worksheet worksheet)
        {
            int rows = worksheet.Cells.MaxDataRow + 1;
            int columns = worksheet.Cells.MaxDataColumn + 1;

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    data.Add(worksheet.Cells[i, j].Value.ToString());
                }
            }
        }
    }
}
