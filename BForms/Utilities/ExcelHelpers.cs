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

            spreadsheetDocument.Fill(items, sheetName);

            // Close the document.
            spreadsheetDocument.Close();

            return memoryStream;
        }

        private static void Fill<T>(this SpreadsheetDocument spreadsheetDocument, IEnumerable<T> items, string sheetName) where T : class
        {
            // Add a WorkbookPart to the document.
            WorkbookPart workbookpart = spreadsheetDocument.AddWorkbookPart();
            workbookpart.Workbook = new Workbook();

            // Add a WorksheetPart to the WorkbookPart.
            WorksheetPart worksheetPart = workbookpart.AddNewPart<WorksheetPart>();

            var ws = new Worksheet();

            ws.Fill(items);

            worksheetPart.Worksheet = ws;
            worksheetPart.Worksheet.Save();

            // Add Sheets to the Workbook.
            Sheets sheets = spreadsheetDocument.WorkbookPart.Workbook.AppendChild<Sheets>(new Sheets());

            // Append a new worksheet and associate it with the workbook.
            Sheet sheet = new Sheet() { Id = spreadsheetDocument.WorkbookPart.GetIdOfPart(worksheetPart), SheetId = 1, Name = sheetName };
            sheets.Append(sheet);

            workbookpart.Workbook.Save();
        }

        private static void Fill<T>(this Worksheet worksheet, IEnumerable<T> items) where T : class
        {
            if (items == null || !items.Any()) return;

            var sheetData = new SheetData();

            var columns = new List<string>();

            Columns exColumns = new Columns();

            var headerRow = new Row();

            var type = typeof(T);

            var index = 0;

            // Create header based on DisplayAttribute and BsGridColumnAttribute
            foreach (var property in type.GetProperties())
            {
                BsGridColumnAttribute columnAttr = null;

                if (ReflectionHelpers.TryGetAttribute(property, out columnAttr))
                {
                    if (columnAttr.Usage != Models.BsGridColumnUsage.Html)
                    {
                        index++;

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

                        exColumns.Append(CreateColumn((UInt32)index, (UInt32)index, width * 10));

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

            worksheet.Append(exColumns);
            worksheet.Append(sheetData);
        }

        private static Run GetBoldStyle()
        {
            Run run = new Run();
            RunProperties runProperties = new RunProperties();
            Bold bold = new Bold();

            runProperties.Append(bold);
            run.Append(runProperties);

            return run;
        }

        private static Cell CreateCell(string name)
        {
            Cell cell = new Cell();
            cell.DataType = CellValues.String;
            cell.CellValue = new CellValue(name);
            //var run = GetBoldStyle();
            //run.Append(new Text(name));
            //var cellValue = new CellValue();
            //cellValue.Append(run);
            //cell.Append(cellValue);
            return cell;
        }

        private static Column CreateColumn(UInt32 startIndex, UInt32 endIndex, double width)
        {
            Column column;
            column = new Column();
            column.Min = startIndex;
            column.Max = endIndex;
            column.Width = width;
            column.CustomWidth = true;
            return column;
        }
    }
}
