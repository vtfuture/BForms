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
    #region ExcelHandler
    /// <summary>
    /// Manages the processing of a list into excel readable content
    /// </summary>
    /// <typeparam name="T">Class with properties marked with BsGridColumnAttribute (supported types: DateTime, string, int, double)</typeparam>
    public class ExcelHandler<T> where T : class
    {
        #region Constructor and Properties
        private SpreadsheetDocument SpreadsheetDocument;
        private Stylesheet Stylesheet;
        private string SheetName;
        public UInt32Value HeaderStyleIndex;
        public UInt32Value DataStyleIndex;
        private int WidthUnit = 10;

        public ExcelHandler(){}
        #endregion

        /// <summary>
        /// Configures the spreadsheet and adds the items in its sheet
        /// </summary>
        /// <param name="items"></param>
        public void Add(SpreadsheetDocument spreadsheetDocument, IEnumerable<T> items, string sheetName)
        {
            SpreadsheetDocument = spreadsheetDocument;
            SheetName = sheetName;

            // add a WorkbookPart to the document
            WorkbookPart workbookpart = SpreadsheetDocument.AddWorkbookPart();
            workbookpart.Workbook = new Workbook();

            // add a WorksheetPart to the WorkbookPart
            WorksheetPart worksheetPart = workbookpart.AddNewPart<WorksheetPart>();

            // add Stylesheet
            WorkbookStylesPart workbookStylesPart = SpreadsheetDocument.WorkbookPart.AddNewPart<WorkbookStylesPart>();
            workbookStylesPart.Stylesheet = Stylesheet ?? CreateStylesheet();

            // add worksheet
            var ws = new Worksheet();

            Process(ws, items);

            worksheetPart.Worksheet = ws;
            worksheetPart.Worksheet.Save();

            // add Sheets to the Workbook.
            Sheets sheets = SpreadsheetDocument.WorkbookPart.Workbook.AppendChild<Sheets>(new Sheets());

            // append a new worksheet and associate it with the workbook.
            Sheet sheet = new Sheet() { Id = SpreadsheetDocument.WorkbookPart.GetIdOfPart(worksheetPart), SheetId = 1, Name = SheetName };
            sheets.Append(sheet);

            workbookpart.Workbook.Save();
        }

        #region Helpers
        /// <summary>
        /// Creates a basic stylesheet
        /// Can be overriden for custom cell styles
        /// </summary>
        /// <returns></returns>
        protected virtual Stylesheet CreateStylesheet()
        {
            Stylesheet stylesheet = new Stylesheet() { MCAttributes = new MarkupCompatibilityAttributes() { Ignorable = "x14ac" } };
            stylesheet.AddNamespaceDeclaration("mc", "http://schemas.openxmlformats.org/markup-compatibility/2006");
            stylesheet.AddNamespaceDeclaration("x14ac", "http://schemas.microsoft.com/office/spreadsheetml/2009/9/ac");

            // create fonts
            Fonts fonts = new Fonts() { Count = (UInt32Value)1U, KnownFonts = true };
            var simpleFont = ExcelHelpers.CreateFont(false, null, null, null);
            var boldFont = ExcelHelpers.CreateFont(true, null, null, null);
            fonts.Append(simpleFont);
            fonts.Append(boldFont);

            // create fills
            Fills fills = new Fills() { Count = (UInt32Value)1U };
            var noneFill = ExcelHelpers.CreateFill(PatternValues.None, null, null);
            fills.Append(noneFill);

            // create borders
            Borders borders = new Borders() { Count = (UInt32Value)1U };
            var border = ExcelHelpers.CreateBorder();
            borders.Append(border);

            // create cell style formats
            CellStyleFormats cellStyleFormats = new CellStyleFormats() { Count = (UInt32Value)1U };
            CellFormat cellFormat = new CellFormat() { NumberFormatId = (UInt32Value)0U, FontId = (UInt32Value)0U, FillId = (UInt32Value)0U, BorderId = (UInt32Value)0U };

            cellStyleFormats.Append(cellFormat);

            CellFormats cellFormats = new CellFormats() { Count = (UInt32Value)2U };
            CellFormat headerCellFormat = ExcelHelpers.CreateCellFormat(1U, 0U);
            CellFormat dataCellFormat = ExcelHelpers.CreateCellFormat(0U, 0U);

            cellFormats.Append(headerCellFormat);
            HeaderStyleIndex = 0;
            cellFormats.Append(dataCellFormat);
            DataStyleIndex = 1;

            CellStyles cellStyles = new CellStyles() { Count = (UInt32Value)1U };
            CellStyle cellStyle = new CellStyle() { Name = "Normal", FormatId = (UInt32Value)0U, BuiltinId = (UInt32Value)0U };
            cellStyles.Append(cellStyle);

            // add to stylesheet
            stylesheet.Append(fonts);
            stylesheet.Append(fills);
            stylesheet.Append(borders);
            stylesheet.Append(cellStyleFormats);
            stylesheet.Append(cellFormats);
            stylesheet.Append(cellStyles);

            return stylesheet;
        }

        /// <summary>
        /// Add a custom stylesheet
        /// Set HeaderStyleIndex and DataStyleIndex to style the cells
        /// </summary>
        /// <param name="stylesheet"></param>
        public void RegisterStylesheet(Stylesheet stylesheet)
        {
            Stylesheet = stylesheet;
        }

        /// <summary>
        /// Processes the list of items, adding them to the worksheet
        /// </summary>
        /// <param name="worksheet"></param>
        /// <param name="items"></param>
        private void Process(Worksheet worksheet, IEnumerable<T> items)
        {
            if (items == null || !items.Any()) return;

            var sheetData = new SheetData();

            var columns = new List<string>();

            Columns exColumns = new Columns();

            var headerRow = new Row();

            var type = typeof(T);

            var index = 0;

            // create header based on DisplayAttribute and BsGridColumnAttribute
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

                        exColumns.Append(ExcelHelpers.CreateColumn((UInt32)index, (UInt32)index, width * WidthUnit));

                        headerRow.AppendChild(ExcelHelpers.CreateTextCell(displayName, HeaderStyleIndex));
                    }
                }
            }

            sheetData.AppendChild(headerRow);

            // create data table
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
                        row.AppendChild(ExcelHelpers.CreateTextCell(strValue, DataStyleIndex));
                    }
                    else
                    {
                        DateTime dateValue;
                        int intValue;
                        long longValue;
                        double doubleValue;

                        if (DateTime.TryParse(value.ToString(), out dateValue))
                        {
                            // ToOADate => excel representation of DateTime - TODO: format date
                            row.AppendChild(ExcelHelpers.CreateTextCell(dateValue.ToShortDateString(), DataStyleIndex));
                        }
                        else if (int.TryParse(value.ToString(), out intValue))
                        {
                            row.AppendChild(ExcelHelpers.CreateValueCell(intValue, DataStyleIndex, CellValues.Number));

                        }
                        else if (long.TryParse(value.ToString(), out longValue))
                        {
                            row.AppendChild(ExcelHelpers.CreateValueCell(longValue, DataStyleIndex, CellValues.Number));
                        }
                        else if (Double.TryParse(value.ToString(), out doubleValue))
                        {
                            row.AppendChild(ExcelHelpers.CreateValueCell(doubleValue, DataStyleIndex, CellValues.Number));
                        }
                        else // not supported type
                        {
                            throw new Exception(column + " is not of type string");
                        }
                    }
                }

                sheetData.AppendChild(row);
            }

            worksheet.Append(exColumns);
            worksheet.Append(sheetData);
        }
        #endregion
    }
    #endregion

    #region ExcelHelpers
    public static class ExcelHelpers
    {
        /// <summary>
        /// Converts a list of items in a memory stream representation of an excel file (using the ExcelHandler class)
        /// The header is generated based on the display attribute of the model
        /// The properties marked with the BsGridColumnAttribute are taken into consideration
        /// </summary>
        /// <typeparam name="T">Class with properties marked with BsGridColumnAttribute (supported property types: DateTime, string, int, double)</typeparam>
        /// <typeparam name="Handler">Class inherited from ExcelHandler</typeparam>
        /// <param name="items">List of items to be converted to excel</param>
        /// <param name="sheetName">The sheetname of the excel file</param>
        /// <returns></returns>
        public static MemoryStream ToExcelMemoryStream<T, Handler>(this IEnumerable<T> items, string sheetName) 
            where T : class
            where Handler : ExcelHandler<T>, new()
        {
            MemoryStream memoryStream = new MemoryStream();

            // Create the spreadsheet on the MemoryStream
            SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Create(memoryStream, SpreadsheetDocumentType.Workbook);

            var handler = new Handler();

            handler.Add(spreadsheetDocument, items, sheetName);

            // Close the document.
            spreadsheetDocument.Close();

            return memoryStream;
        }

        #region Style
        /// <summary>
        /// Create a new cell format
        /// </summary>
        /// <param name="fontId"></param>
        /// <param name="fillId"></param>
        /// <param name="numberFormatId"></param>
        /// <param name="borderId"></param>
        /// <param name="formatId"></param>
        /// <param name="applyFill"></param>
        /// <returns></returns>
        public static CellFormat CreateCellFormat(UInt32 fontId, UInt32 fillId, UInt32 numberFormatId = 0U, UInt32 borderId = 0U, UInt32 formatId = 0U, bool applyFill = true, bool applyNumberFormat = true)
        {
            return new CellFormat()
            {
                NumberFormatId = (UInt32Value)numberFormatId,
                FontId = (UInt32Value)fontId,
                FillId = (UInt32Value)fillId,
                BorderId = (UInt32Value)borderId,
                FormatId = (UInt32Value)formatId,
                ApplyFill = applyFill,
                ApplyNumberFormat = applyNumberFormat
            };
        }

        /// <summary>
        /// Create a new font
        /// </summary>
        /// <param name="isBold">Whether the text should be bold</param>
        /// <param name="fontFamily">The font family eg: "Arial"</param>
        /// <param name="size">The font size</param>
        /// <param name="color">The font color (value of the color theme)</param>
        /// <returns></returns>
        public static Font CreateFont(bool isBold, string fontFamily, System.Double? size, UInt32? color)
        {
            Font font = new Font();

            if (size.HasValue)
            {
                FontSize fontSize = new FontSize() { Val = size };
                font.Append(fontSize);
            }

            if (isBold)
            {
                Bold bold = new Bold();
                font.Append(bold);
            }

            if (color != null)
            {
                Color color1 = new Color() { Theme = (UInt32Value)color };
                font.Append(color1);
            }

            if (!string.IsNullOrEmpty(fontFamily))
            {
                FontName fontName = new FontName() { Val = fontFamily };
                font.Append(fontName);
            }

            return font;
        }

        /// <summary>
        /// Create a new fill
        /// </summary>
        /// <param name="patternType">The pattern type value</param>
        /// <param name="foregroundColor">The foreground color of the fill</param>
        /// <param name="backgroundColor">The background color of the fill</param>
        /// <returns></returns>
        public static Fill CreateFill(PatternValues patternType, string foregroundColor, string backgroundColor)
        {
            Fill fill = new Fill();
            PatternFill patternFill = new PatternFill() { PatternType = patternType };

            if (!string.IsNullOrEmpty(foregroundColor))
            {
                ForegroundColor foreColor = new ForegroundColor() { Rgb = foregroundColor };
                patternFill.Append(foreColor);
            }

            if (!string.IsNullOrEmpty(backgroundColor))
            {
                BackgroundColor backColor = new BackgroundColor() { Rgb = backgroundColor };
                patternFill.Append(backColor);
            }

            fill.Append(patternFill);
            return fill;
        }

        /// <summary>
        /// Create a new border
        /// </summary>
        /// <returns></returns>
        public static Border CreateBorder()
        {
            Border border = new Border();
            LeftBorder leftBorder = new LeftBorder();
            RightBorder rightBorder = new RightBorder();
            TopBorder topBorder = new TopBorder();
            BottomBorder bottomBorder = new BottomBorder();
            DiagonalBorder diagonalBorder = new DiagonalBorder();

            border.Append(leftBorder);
            border.Append(rightBorder);
            border.Append(topBorder);
            border.Append(bottomBorder);
            border.Append(diagonalBorder);

            return border;
        }
        #endregion

        #region Generate Cells/Columns
        /// <summary>
        /// Create simple text cell
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Cell CreateTextCell(string name)
        {
            Cell cell = new Cell();
            cell.DataType = CellValues.String;
            cell.CellValue = new CellValue(name);
            return cell;
        }

        /// <summary>
        /// Create styled text cell at specified position
        /// </summary>
        /// <param name="columnIndex"></param>
        /// <param name="rowIndex"></param>
        /// <param name="cellValue"></param>
        /// <param name="styleIndex"></param>
        /// <returns></returns>
        public static Cell CreateTextCell(int columnIndex, int rowIndex, object cellValue, Nullable<uint> styleIndex)
        {
            Cell cell = CreateTextCell(cellValue, styleIndex);
            cell.CellReference = GetColumnName(columnIndex) + rowIndex;
            return cell;
        }

        /// <summary>
        /// Create styled text cell
        /// </summary>
        /// <param name="cellValue"></param>
        /// <param name="styleIndex"></param>
        /// <returns></returns>
        public static Cell CreateTextCell(object cellValue, Nullable<uint> styleIndex)
        {
            Cell cell = new Cell();
            cell.DataType = CellValues.InlineString;

            if (styleIndex.HasValue)
                cell.StyleIndex = styleIndex.Value;

            InlineString inlineString = new InlineString();
            Text t = new Text();

            t.Text = cellValue.ToString();
            inlineString.AppendChild(t);
            cell.AppendChild(inlineString);

            return cell;
        }

        /// <summary>
        /// Create styled value cell at specified position
        /// </summary>
        /// <param name="columnIndex"></param>
        /// <param name="rowIndex"></param>
        /// <param name="cellValue"></param>
        /// <param name="styleIndex"></param>
        /// <returns></returns>
        public static Cell CreateValueCell(int columnIndex, int rowIndex, object cellValue, Nullable<uint> styleIndex, CellValues? cellType = null)
        {
            Cell cell = CreateValueCell(cellValue, styleIndex, cellType);
            cell.CellReference = GetColumnName(columnIndex) + rowIndex;
            return cell;
        }

        /// <summary>
        /// Create styled value cell
        /// </summary>
        /// <param name="cellValue"></param>
        /// <param name="styleIndex"></param>
        /// <returns></returns>
        public static Cell CreateValueCell(object cellValue, Nullable<uint> styleIndex, CellValues? cellType = null)
        {
            Cell cell = new Cell();

            if (cellType.HasValue)
            {
                cell.DataType = cellType;
            }

            CellValue value = new CellValue();

            value.Text = cellValue.ToString();

            if (styleIndex.HasValue)
                cell.StyleIndex = styleIndex.Value;

            cell.AppendChild(value);

            return cell;
        }

        /// <summary>
        /// Create a custom width column
        /// </summary>
        /// <param name="startIndex"></param>
        /// <param name="endIndex"></param>
        /// <param name="width"></param>
        /// <returns></returns>
        public static Column CreateColumn(UInt32 startIndex, UInt32 endIndex, double width)
        {
            Column column;
            column = new Column();
            column.Min = startIndex;
            column.Max = endIndex;
            column.Width = width;
            column.CustomWidth = true;
            return column;
        }

        /// <summary>
        /// Get the column name based on its index
        /// </summary>
        /// <param name="columnIndex"></param>
        /// <returns></returns>
        private static string GetColumnName(int columnIndex)
        {
            int dividend = columnIndex;
            string columnName = String.Empty;
            int modifier;

            while (dividend > 0)
            {
                modifier = (dividend - 1) % 26;
                columnName =
                    Convert.ToChar(65 + modifier).ToString() + columnName;
                dividend = (int)((dividend - modifier) / 26);
            }

            return columnName;
        }
        #endregion
    }
    #endregion
}
