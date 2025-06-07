using OfficeOpenXml;
using ItemSearchApp.Models;

namespace ItemSearchApp.Helpers
{
    public static class ExcelReader
    {
        public static List<Item> LoadItemsFromExcel(string filePath)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            var items = new List<Item>();

            if (!File.Exists(filePath))
            {
                // ファイルが存在しなければ空のリストを返す
                return items;
            }

            using var package = new ExcelPackage(new FileInfo(filePath));
            var worksheet = package.Workbook.Worksheets.FirstOrDefault();
            if (worksheet == null)
            {
                // ワークシートがない場合も空リストを返す
                return items;
            }

            var rowCount = worksheet.Dimension.Rows;

            for (int row = 2; row <= rowCount; row++)
            {
                var name = worksheet.Cells[row, 1].Text;
                var price = worksheet.Cells[row, 2].Text;

                if (!string.IsNullOrWhiteSpace(name))
                {
                    items.Add(new Item { Name = name, Price = price });
                }
            }

            return items;
        }
    }
}
