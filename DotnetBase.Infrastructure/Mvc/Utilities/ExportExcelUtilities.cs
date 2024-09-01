using OfficeOpenXml;
using OfficeOpenXml.Style;
using DotnetBase.Infrastructure.Common.Attributes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace DotnetBase.Infrastructure.Mvc.Utilities
{
    public class ExportExcelUtilities
    {

        public static List<PropertyInfo> GetAllExcelProperty<T>()
        {
            var props = typeof(T).GetProperties().Where(prop => Attribute.IsDefined(prop, typeof(ExportExcelAttribute)))
                .OrderBy(x => x.GetAttributValue((ExportExcelAttribute a) => a.Priority)).ToList();

            return props;
        }
        //Export excel
        public static MemoryStream ExportExcel<T>(List<T> listT)
        {
            var props = GetAllExcelProperty<T>();

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;


            //ExportExcel
            var stream = new MemoryStream();
            var excel = new ExcelPackage(stream);

            var workSheet = excel.Workbook.Worksheets.Add("Sheet1");
            workSheet.TabColor = System.Drawing.Color.Black;
            workSheet.DefaultRowHeight = 20;
            workSheet.DefaultColWidth = 20;
            workSheet.Cells.Style.WrapText = true;

            // add header of table
            workSheet.Row(1).Height = 20;
            workSheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Row(1).Style.Font.Bold = true;

            string displayName = null;
            workSheet.Cells[1, 1].Value = "STT";
            var colIndex = 2;
            foreach (var prop in props)
            {
                displayName = prop.GetAttributValue((ExportExcelAttribute a) => a.DisplayName);
                workSheet.Cells[1, colIndex].Value = displayName;
                colIndex++;
            }

            // Body table
            int rowIndex = 2, stt = 1;
            colIndex = 2;
            string value;
            foreach (var model in listT)
            {
                workSheet.Row(rowIndex).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                workSheet.Cells[rowIndex, 1].Value = stt;
                foreach (var prop in props)
                {
                    if (prop.GetValue(model) == null)
                    {
                        value = string.Empty;
                    }
                    else
                    {
                        value = prop.GetValue(model).ToString();
                    }
                    workSheet.Cells[rowIndex, colIndex].Value = value;
                    colIndex++;
                }
                stt++;
                colIndex = 2;
                rowIndex++;
            }

            excel.Save();
            stream.Position = 0;
            return stream;
        }
    }
}
