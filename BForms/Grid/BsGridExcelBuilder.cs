using BForms.Mvc;
using BForms.Utilities;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BForms.Grid
{
    #region Helpers
    public static class BsGridExcelHelpers
    {
        public static bool AreEqual(this BsGridExcelStyle excelStyle, BsGridExcelStyle otherExcelStyle)
        {
            return excelStyle.FontId.HasValue && 
                excelStyle.FillId.HasValue &&
                excelStyle.FontId == otherExcelStyle.FontId &&
                excelStyle.FillId == otherExcelStyle.FillId;
        }
    }
    #endregion

    #region BsGridExcelStyle
    public class BsGridExcelStyle
    {
        public BsGridExcelStyle()
        {
            Font = new BsGridExcelFont();
        }
        public BsGridExcelFont Font { get; set; }
        public string FillColor { get; set; }
        public int? FontId { get; set; }
        public int? FillId { get; set; }
    }

    public class BsGridExcelFont
    {
        public bool Bold { get; set; }
        public string Family { get; set; }
        public int? Size { get; set; }
        public string Color { get; set; }
    }
    #endregion

    #region BsGridExcelBuilder
    public class BsGridExcelBuilder<T> where T : class
    {
        #region Constructor and Properties
        private string fileName;
        private Action<T, BsGridExcelStyle> aConfig;
        private Func<T, BsGridExcelStyle> fConfig;
        private List<BsGridExcelCell<T>> dataCells;
        private List<BsGridExcelCell<T>> headerCells;
        private BsGridExcelStyle headerStyle;
        private IEnumerable<T> items;
        private Stylesheet styleSheet;
        private List<BsGridExcelStyle> fonts = new List<BsGridExcelStyle>();
        private List<BsGridExcelStyle> fills = new List<BsGridExcelStyle>();
        private List<BsGridExcelStyle> cellFormats = new List<BsGridExcelStyle>();
        private int widthUnit = 10;

        public BsGridExcelBuilder(string fileName, IEnumerable<T> items)
        {
            this.fileName = fileName;
            this.items = items;
        }
        #endregion

        #region Config
        public BsGridExcelBuilder<T> ConfigureRows(Action<T, BsGridExcelStyle> config)
        {
            this.aConfig = config;

            return this;
        }

        public BsGridExcelBuilder<T> ConfigureRows(Func<T, BsGridExcelStyle> config)
        {
            this.fConfig = config;

            return this;
        }

        public BsGridExcelBuilder<T> ConfigureHeader(Action<BsGridExcelCellFactory<T>> config)
        {
            var factory = new BsGridExcelCellFactory<T>();

            config(factory);

            this.headerCells = factory.Cells;

            this.headerStyle = factory.Style;

            return this;
        }

        public BsGridExcelBuilder<T> ConfigureColumns(Action<BsGridExcelColumnFactory<T>> config)
        {
            var factory = new BsGridExcelColumnFactory<T>();

            config(factory);

            this.dataCells = factory.Cells;

            return this;
        }
        #endregion

        #region Helpers
        private void AddWorkbook(SpreadsheetDocument spreadsheetDocument)
        {
            // add a WorkbookPart to the document
            WorkbookPart workbookpart = spreadsheetDocument.AddWorkbookPart();
            workbookpart.Workbook = new Workbook();

            // add a WorksheetPart to the WorkbookPart
            WorksheetPart worksheetPart = workbookpart.AddNewPart<WorksheetPart>();

            // add Stylesheet
            WorkbookStylesPart workbookStylesPart = spreadsheetDocument.WorkbookPart.AddNewPart<WorkbookStylesPart>();
            styleSheet = AddStylesheet();
            workbookStylesPart.Stylesheet = styleSheet;

            // add worksheet
            var ws = new Worksheet();

            Fill(ws);

            worksheetPart.Worksheet = ws;
            worksheetPart.Worksheet.Save();

            // add Sheets to the Workbook.
            Sheets sheets = spreadsheetDocument.WorkbookPart.Workbook.AppendChild<Sheets>(new Sheets());

            // append a new worksheet and associate it with the workbook.
            Sheet sheet = new Sheet() { Id = spreadsheetDocument.WorkbookPart.GetIdOfPart(worksheetPart), SheetId = 1, Name = fileName };
            sheets.Append(sheet);

            workbookpart.Workbook.Save();
        }

        /// <summary>
        /// Processes the list of items, adding them to the worksheet
        /// </summary>
        /// <param name="worksheet"></param>
        /// <param name="items"></param>
        private void Fill(Worksheet worksheet)
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

                        if (headerCells != null)
                        {
                            var headerCell = headerCells.FirstOrDefault(x => string.Compare(x.PropName, property.Name) == 0);

                            if (headerCell != null)
                            {
                                displayName = headerCell.Name;
                            }
                        }

                        columns.Add(property.Name);

                        exColumns.Append(ExcelHelpers.CreateColumn((UInt32)index, (UInt32)index, width * widthUnit));

                        int formatId;
                        var cellFormat = GetCellFormat(this.headerStyle ?? new BsGridExcelStyle
                        {
                            Font = new BsGridExcelFont
                            {
                                Bold = true
                            }
                        }, out formatId);

                        if (cellFormat != null)
                        {
                            styleSheet.CellFormats.Append(cellFormat);
                        }

                        headerRow.AppendChild(ExcelHelpers.CreateTextCell(displayName, (UInt32)formatId));
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

                    object value;

                    var cell = dataCells.FirstOrDefault(x => string.Compare(x.PropName, column) == 0);

                    if (cell != null && cell.NameFunc != null)
                    {
                        value = cell.NameFunc(item);
                    }
                    else
                    {
                        value = property.GetValue(item);
                    }

                    var style = new BsGridExcelStyle();

                    if (aConfig != null)
                    {
                        aConfig(item, style);
                    }
                    else if (fConfig != null)
                    {
                        style = fConfig(item);
                    }

                    int formatId;
                    var cellFormat = GetCellFormat(style, out formatId);

                    if (cellFormat != null)
                    {
                        styleSheet.CellFormats.Append(cellFormat);
                    }

                    var strValue = value as string;

                    if (strValue != null)
                    {
                        row.AppendChild(ExcelHelpers.CreateTextCell(strValue, (UInt32)formatId));
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
                            row.AppendChild(ExcelHelpers.CreateTextCell(dateValue.ToShortDateString(), (UInt32)formatId));
                        }
                        else if (int.TryParse(value.ToString(), out intValue))
                        {
                            row.AppendChild(ExcelHelpers.CreateValueCell(intValue, (UInt32)formatId, CellValues.Number));

                        }
                        else if (long.TryParse(value.ToString(), out longValue))
                        {
                            row.AppendChild(ExcelHelpers.CreateValueCell(longValue, (UInt32)formatId, CellValues.Number));
                        }
                        else if (Double.TryParse(value.ToString(), out doubleValue))
                        {
                            row.AppendChild(ExcelHelpers.CreateValueCell(doubleValue, (UInt32)formatId, CellValues.Number));
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

        /// <summary>
        /// Creates a basic stylesheet
        /// Can be overriden for custom cell styles
        /// </summary>
        /// <returns></returns>
        private Stylesheet AddStylesheet()
        {
            Stylesheet stylesheet = new Stylesheet() { MCAttributes = new MarkupCompatibilityAttributes() { Ignorable = "x14ac" } };
            stylesheet.AddNamespaceDeclaration("mc", "http://schemas.openxmlformats.org/markup-compatibility/2006");
            stylesheet.AddNamespaceDeclaration("x14ac", "http://schemas.microsoft.com/office/spreadsheetml/2009/9/ac");

            // create fonts
            Fonts fonts = new Fonts() { Count = (UInt32Value)1U, KnownFonts = true };
            int fontId;
            var simpleFont = GetFont(new BsGridExcelStyle(), out fontId);
            var boldFont = GetFont(new BsGridExcelStyle()
            {
                Font = new BsGridExcelFont()
                {
                    Bold = true
                }
            }, out fontId);
            fonts.Append(simpleFont);
            fonts.Append(boldFont);

            // create fills
            Fills fills = new Fills() { Count = (UInt32Value)1U };
            int fillId;
            var noneFill = GetFill(new BsGridExcelStyle(), out fillId);
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
            int formatId;
            CellFormat headerCellFormat = GetCellFormat(new BsGridExcelStyle
            {
                FontId = 1,
                FillId = 0
            }, out formatId);
            CellFormat dataCellFormat = GetCellFormat(new BsGridExcelStyle
            {
                FontId = 0,
                FillId = 0
            }, out formatId);

            cellFormats.Append(headerCellFormat);
            cellFormats.Append(dataCellFormat);

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

        private Font GetFont(BsGridExcelStyle style, out int fontId)
        {
            foreach (var item in fonts)
            {
                if (item.Font.Bold == style.Font.Bold &&
                    string.Compare(item.Font.Color, style.Font.Color) == 0 &&
                    item.Font.Size == style.Font.Size &&
                    string.Compare(item.Font.Family, style.Font.Family) == 0)
                {
                    fontId = fonts.IndexOf(item);
                    return null;
                }
            }
            var font = ExcelHelpers.CreateFont(
                style.Font.Bold, 
                style.Font.Family, 
                style.Font.Size, 
                style.Font.Color);

            fonts.Add(style);

            fontId = fonts.IndexOf(style);

            return font;
        }

        private Fill GetFill(BsGridExcelStyle style, out int fillId)
        {
            foreach (var item in fills)
            {
                if (string.Compare(item.FillColor, style.FillColor) == 0)
                {
                    fillId = fills.IndexOf(item);
                    return null;
                }
            }
            var fill = ExcelHelpers.CreateFill(style.FillColor);

            fills.Add(style);

            fillId = fills.IndexOf(style);

            return fill;
        }

        private CellFormat GetCellFormat(BsGridExcelStyle style, out int formatId)
        {
            if (style.FillId.HasValue && style.FontId.HasValue)
            {
                foreach (var item in cellFormats)
                {
                    if (item.AreEqual(style))
                    {
                        formatId = cellFormats.IndexOf(item);
                        return null;
                    }
                }

                var cellFormat = ExcelHelpers.CreateCellFormat((UInt32)style.FontId, (UInt32)style.FillId);

                cellFormats.Add(style);

                formatId = cellFormats.IndexOf(style);

                return cellFormat;
            }
            else
            {
                int fontId;
                var font = GetFont(style, out fontId);
                if (font != null)
                {
                    styleSheet.Fonts.Append(font);
                }

                int fillId;
                var fill = GetFill(style, out fillId);
                if (fill != null)
                {
                    styleSheet.Fills.Append(fill);
                }

                var formatStyle = new BsGridExcelStyle()
                {
                    FontId = fontId,
                    FillId = fillId
                };

                return GetCellFormat(formatStyle, out formatId);
            }
        }
        #endregion

        #region Serialize
        public MemoryStream ToStream()
        {
            MemoryStream memoryStream = new MemoryStream();

            // Create the spreadsheet on the MemoryStream
            SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Create(memoryStream, SpreadsheetDocumentType.Workbook);

            AddWorkbook(spreadsheetDocument);

            // Close the document.
            spreadsheetDocument.Close();

            return memoryStream;
        }

        public byte[] ToArray()
        {
            var stream = ToStream();

            if (stream != null)
            {
                return ToStream().ToArray();
            }

            return null;
        }
        #endregion
    }
    #endregion
}

