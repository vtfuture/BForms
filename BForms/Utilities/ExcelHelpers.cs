using BForms.Mvc;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BForms.Utilities
{
    public static class ExcelHelpers
    {
        public static MemoryStream ToExcel<T>(this IEnumerable<T> items, string sheetName) where T : class
        {
            MemoryStream memoryStream = new MemoryStream();

            // Create the spreadsheet on the MemoryStream
            SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Create(memoryStream, SpreadsheetDocumentType.Workbook);

            var sheetData = CreateSpreadsheetWorkbook(spreadsheetDocument, sheetName);

            sheetData.Fill(items);

            // Close the document.
            spreadsheetDocument.Close();

            return memoryStream;
        }

        private static SheetData CreateSpreadsheetWorkbook(SpreadsheetDocument spreadsheetDocument, string sheetName)
        {
            // Add a WorkbookPart to the document.
            WorkbookPart workbookpart = spreadsheetDocument.AddWorkbookPart();
            workbookpart.Workbook = new Workbook();

            // Add a WorksheetPart to the WorkbookPart.
            WorksheetPart worksheetPart = workbookpart.AddNewPart<WorksheetPart>();

            var sheetData = new SheetData();

            worksheetPart.Worksheet = new Worksheet(sheetData);

            // Add Sheets to the Workbook.
            Sheets sheets = spreadsheetDocument.WorkbookPart.Workbook.AppendChild<Sheets>(new Sheets());

            // Append a new worksheet and associate it with the workbook.
            Sheet sheet = new Sheet() { Id = spreadsheetDocument.WorkbookPart.GetIdOfPart(worksheetPart), SheetId = 1, Name = sheetName };
            sheets.Append(sheet);

            workbookpart.Workbook.Save();

            return sheetData;
        }

        private static void Fill<T>(this SheetData sheetData, IEnumerable<T> items) where T : class
        {
            if (items == null || !items.Any()) return;

            var columns = new List<string>();

            var headerRow = new Row();

            var type = typeof(T);

            // Create header based on DisplayAttribute and BsGridColumnAttribute
            foreach (var property in type.GetProperties())
            {
                BsGridColumnAttribute columnAttr = null;

                if (ReflectionHelpers.TryGetAttribute(property, out columnAttr))
                {
                    if (columnAttr.Usage != Models.BsGridColumnUsage.Html)
                    {
                        var width = columnAttr.Width;

                        string displayName = null;
                        DisplayAttribute displayAttribute = null;

                        if (ReflectionHelpers.TryGetAttribute(property, out displayAttribute))
                        {
                            displayName = displayAttribute.GetName();
                        }
                        else
                        {
                            displayName = property.Name;
                        }

                        columns.Add(property.Name);

                        headerRow.AppendChild(CreateCell(displayName));
                    }
                }
            }

            sheetData.AppendChild(headerRow);

            // Create data table
            foreach (var item in items)
            {
                var row = new Row();

                foreach (var column in columns)
                {
                    var property = type.GetProperty(column);

                    var value = property.GetValue(item);

                    var strValue = value as string;

                    if (strValue != null)
                    {
                        row.AppendChild(CreateCell(strValue));
                    }
                    else
                    {
                        throw new Exception(column + " is not of type string");
                    }
                }

                sheetData.AppendChild(row);
            }
        }

        private static Cell CreateCell(string name)
        {
            Cell cell = new Cell();
            cell.DataType = CellValues.String;
            cell.CellValue = new CellValue(name);
            return cell;
        }
    }
}
