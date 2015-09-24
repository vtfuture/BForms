using BForms.Models;
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
        #region AreEqual
        /// <summary>
        /// Test if two BsGridExcelStyles are equal (based on fontId and fillId reference)
        /// </summary>
        /// <param name="excelStyle"></param>
        /// <param name="otherExcelStyle"></param>
        /// <returns></returns>
        public static bool AreEqual(this BsGridExcelStyle excelStyle, BsGridExcelStyle otherExcelStyle)
        {
            return excelStyle.FontId.HasValue && 
                excelStyle.FillId.HasValue &&
                excelStyle.FontId == otherExcelStyle.FontId &&
                excelStyle.FillId == otherExcelStyle.FillId;
        }
        #endregion

        #region Concat
        /// <summary>
        /// Concatenate styles properties without overriding them
        /// </summary>
        public static Func<BsGridExcelStyle, BsGridExcelStyle, BsGridExcelStyle> Concat = (style1, style2) =>
        {
            if (style2 != null && style1 != null)
            {
                if (!style1.FillColor.HasValue)
                {
                    style1.FillColor = style2.FillColor;
                }

                if (!style1.Font.Bold.HasValue)
                {
                    style1.Font.Bold = style2.Font.Bold;
                }

                if (!style1.Font.Italic.HasValue)
                {
                    style1.Font.Italic = style2.Font.Italic;
                }

                if (string.IsNullOrEmpty(style1.Font.Family))
                {
                    style1.Font.Family = style2.Font.Family;
                }

                if (!style1.Font.Size.HasValue)
                {
                    style1.Font.Size = style2.Font.Size;
                }

                if (!style1.Font.Color.HasValue)
                {
                    style1.Font.Color = style2.Font.Color;
                }
            }

            return style1;
        };
        #endregion
    }
    #endregion

    #region BsGridExcelStyle
    /// <summary>
    /// Class representing a cell style
    /// </summary>
    public class BsGridExcelStyle
    {
        public BsGridExcelStyle()
        {
            Font = new BsGridExcelFont();
        }
        public BsGridExcelFont Font { get; set; }
        public BsGridExcelColor? FillColor { get; set; }
        internal PatternValues? FillPattern { get; set; }
        internal int? FontId { get; set; }
        internal int? FillId { get; set; }
    }

    #region BsGridExcelFont
    /// <summary>
    /// Class representing a font style
    /// </summary>
    public class BsGridExcelFont
    {
        public bool? Bold { get; set; }
        public bool? Italic { get; set; }
        public string Family { get; set; }
        public int? Size { get; set; }
        public BsGridExcelColor? Color { get; set; }
    }
    #endregion

    #endregion

    #region BsGridExcelStyleSheetBuilder
    public class BsGridExcelStyleSheetBuilder
    {
        #region Ctor and Props
        public Stylesheet Stylesheet;
        protected List<BsGridExcelStyle> fonts = new List<BsGridExcelStyle>();
        protected List<BsGridExcelStyle> fills = new List<BsGridExcelStyle>();
        protected List<BsGridExcelStyle> cellFormats = new List<BsGridExcelStyle>();

        public BsGridExcelStyleSheetBuilder()
        {
            GenerateDefaultStylesheet();
        }
        #endregion

        #region Style

        /// <summary>
        /// Creates a basic stylesheet
        /// Can be overriden for custom cell styles
        /// </summary>
        /// <returns></returns>
        private void GenerateDefaultStylesheet()
        {
            Stylesheet = new Stylesheet() { MCAttributes = new MarkupCompatibilityAttributes() { Ignorable = "x14ac" } };
            Stylesheet.AddNamespaceDeclaration("mc", "http://schemas.openxmlformats.org/markup-compatibility/2006");
            Stylesheet.AddNamespaceDeclaration("x14ac", "http://schemas.microsoft.com/office/spreadsheetml/2009/9/ac");

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
            var noneFill = GetFill(new BsGridExcelStyle()
            {
                FillPattern = PatternValues.None
            }, out fillId); //needed, reserved by excel
            var greyFill = GetFill(new BsGridExcelStyle()
            {
                FillPattern = PatternValues.Gray125
            }, out fillId); //needed, reserved by excel
            fills.Append(noneFill);
            fills.Append(greyFill);

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
            Stylesheet.Append(fonts);
            Stylesheet.Append(fills);
            Stylesheet.Append(borders);
            Stylesheet.Append(cellStyleFormats);
            Stylesheet.Append(cellFormats);
            Stylesheet.Append(cellStyles);
        }

        internal Font GetFont(BsGridExcelStyle style, out int fontId)
        {
            foreach (var item in fonts)
            {
                if (item.Font.Bold == style.Font.Bold &&
                    item.Font.Italic == style.Font.Italic &&
                    item.Font.Color == style.Font.Color &&
                    item.Font.Size == style.Font.Size &&
                    string.Compare(item.Font.Family, style.Font.Family) == 0)
                {
                    fontId = fonts.IndexOf(item);
                    return null;
                }
            }
            var color = GetColor(style.Font.Color);
            var font = ExcelHelpers.CreateFont(
                style.Font.Bold ?? false,
                style.Font.Italic ?? false,
                style.Font.Family,
                style.Font.Size,
                color);

            fonts.Add(style);

            fontId = fonts.IndexOf(style);

            return font;
        }

        internal Fill GetFill(BsGridExcelStyle style, out int fillId)
        {
            foreach (var item in fills)
            {
                if ((item.FillColor == style.FillColor) && (item.FillPattern == style.FillPattern))
                {
                    fillId = fills.IndexOf(item);
                    return null;
                }
            }

            var fillColor = GetColor(style.FillColor);

            var fill = ExcelHelpers.CreateFill(fillColor, style.FillPattern);

            fills.Add(style);

            fillId = fills.IndexOf(style);

            return fill;
        }

        internal CellFormat GetCellFormat(BsGridExcelStyle style, out int formatId)
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
                    Stylesheet.Fonts.Append(font);
                }

                int fillId;
                var fill = GetFill(style, out fillId);
                if (fill != null)
                {
                    Stylesheet.Fills.Append(fill);
                }

                var formatStyle = new BsGridExcelStyle()
                {
                    FontId = fontId,
                    FillId = fillId
                };

                return GetCellFormat(formatStyle, out formatId);
            }
        }

        internal string GetColor(BsGridExcelColor? fillColor)
        {
            return fillColor.HasValue ? ReflectionHelpers.EnumDescription(typeof(BsGridExcelColor), fillColor.Value) : null;
        }

        #endregion

        internal virtual void AttachToWorksheet(Worksheet worksheet) { }
    }
    #endregion

    #region BsGridExcelSheetBuilder
    public abstract class BsGridExcelSheetBuilder
    {
        internal virtual void AttachToWorksheet(Worksheet worksheet)
        {
        }
    }

    public class BsGridExcelSheetBuilder<T> : BsGridExcelSheetBuilder where T : class
    {
        private Action<T, BsGridExcelStyle> aConfig;
        private Func<T, BsGridExcelStyle> fConfig;
        private List<BsGridExcelCell<T>> dataCells;
        private List<BsGridExcelCell<T>> headerCells;
        private BsGridExcelStyle headerStyle;
        private Dictionary<string, int> order;
        private IEnumerable<T> items;
        private BsGridExcelStyleSheetBuilder styleBuilder;
        private int widthUnit = 10;

        public BsGridExcelSheetBuilder(
            BsGridExcelStyleSheetBuilder builder,
            IEnumerable<T> items)
        {
            this.styleBuilder = builder;
            this.items = items;
        }

        #region Config
        public BsGridExcelSheetBuilder<T> ConfigureRows(Action<T, BsGridExcelStyle> config)
        {
            this.aConfig = config;

            return this;
        }

        public BsGridExcelSheetBuilder<T> ConfigureRows(Func<T, BsGridExcelStyle> config)
        {
            this.fConfig = config;

            return this;
        }

        public BsGridExcelSheetBuilder<T> ConfigureHeader(Action<BsGridExcelCellFactory<T>> config)
        {
            var factory = new BsGridExcelCellFactory<T>();

            config(factory);

            this.headerCells = factory.Cells;

            this.headerStyle = factory.Style;

            this.order = factory.Positions;

            return this;
        }

        public BsGridExcelSheetBuilder<T> ConfigureColumns(Action<BsGridExcelColumnFactory<T>> config)
        {
            var factory = new BsGridExcelColumnFactory<T>();

            config(factory);

            this.dataCells = factory.Cells;

            return this;
        }
        #endregion

        /// <summary>
        /// Processes the list of items, adding them to the worksheet
        /// </summary>
        /// <param name="worksheet"></param>
        private void AddRows(Worksheet worksheet)
        {
            var sheetData = new SheetData();

            var columns = new List<string>();

            Columns exColumns = new Columns();

            var headerRow = new Row();

            var type = typeof(T);

            #region Header
            var index = 0;
            var properties = type.GetProperties();

            #region Order Columns
            if (this.order != null) // the column order from settings has priority
            {
                properties = properties.OrderBy(x =>
                {
                    if (this.order.ContainsKey(x.Name))
                    {
                        return this.order.FirstOrDefault(y => y.Key == x.Name).Value;
                    }
                    else
                    {
                        return Int32.MaxValue;
                    }

                }).ToArray();
            }
            else if (this.dataCells != null && this.dataCells.Any())
            {
                properties = properties.OrderBy(x =>
                {
                    var cell = this.dataCells.FirstOrDefault(y => y.PropName == x.Name);

                    if (cell != null)
                    {
                        return cell.Position ?? Int32.MaxValue;
                    }
                    else
                    {
                        return Int32.MaxValue;
                    }
                }).ToArray();
            }
            #endregion

            // create header based on DisplayAttribute and BsGridColumnAttribute
            foreach (var property in properties)
            {
                BsGridColumnAttribute columnAttr = null;

                if (ReflectionHelpers.TryGetAttribute(property, out columnAttr))
                {
                    if (columnAttr.Usage != Models.BsGridColumnUsage.Html)
                    {
                        index++;

                        #region Value
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
                        #endregion

                        #region Style
                        var width = columnAttr.Width;

                        BsGridExcelStyle style = null;
                        if (headerCells != null)
                        {
                            var headerCell = headerCells.FirstOrDefault(x => string.Compare(x.PropName, property.Name) == 0);

                            if (headerCell != null)
                            {
                                displayName = headerCell.Name;
                                style = BsGridExcelHelpers.Concat(headerCell.CellStyle, this.headerStyle);
                            }
                        }

                        columns.Add(property.Name);

                        exColumns.Append(ExcelHelpers.CreateColumn((UInt32)index, (UInt32)index, width * widthUnit));

                        int formatId;

                        if (style == null)
                        {
                            style = this.headerStyle ?? new BsGridExcelStyle
                            {
                                Font = new BsGridExcelFont
                                {
                                    Bold = true
                                }
                            };
                        }
                        var cellFormat = styleBuilder.GetCellFormat(style, out formatId);

                        if (cellFormat != null)
                        {
                            styleBuilder.Stylesheet.CellFormats.Append(cellFormat);
                        }
                        #endregion

                        headerRow.AppendChild(ExcelHelpers.CreateTextCell(displayName, (UInt32)formatId));
                    }
                }
            }

            sheetData.AppendChild(headerRow);
            #endregion

            #region Rows
            // create data table
            foreach (var item in items)
            {
                var row = new Row();

                foreach (var column in columns)
                {
                    var property = type.GetProperty(column);

                    object value;

                    BsGridExcelCell<T> cell = null;

                    if (this.dataCells != null && this.dataCells.Any())
                    {
                        cell = this.dataCells.FirstOrDefault(x => string.Compare(x.PropName, column) == 0);
                    }

                    #region Style
                    BsGridExcelStyle style = new BsGridExcelStyle();

                    if (aConfig != null)
                    {
                        aConfig(item, style);
                    }
                    else if (fConfig != null)
                    {
                        style = fConfig(item);
                    }

                    if (cell != null)
                    {
                        if (cell.CellStyle != null)
                        {
                            style = BsGridExcelHelpers.Concat(style, cell.CellStyle);
                        }
                        if (cell.StyleFunc != null)
                        {
                            var style2 = new BsGridExcelStyle();
                            cell.StyleFunc(item, style2);
                            style = BsGridExcelHelpers.Concat(style, style2);
                        }
                    }

                    int formatId;
                    var cellFormat = styleBuilder.GetCellFormat(style, out formatId);

                    if (cellFormat != null)
                    {
                        styleBuilder.Stylesheet.CellFormats.Append(cellFormat);
                    }
                    #endregion

                    #region Value
                    if (cell != null && cell.NameFunc != null)
                    {
                        value = cell.NameFunc(item);
                    }
                    else
                    {
                        value = property.GetValue(item);
                    }

                    if (value != null)
                    {
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
                    else
                    {
                        row.AppendChild(ExcelHelpers.CreateTextCell(string.Empty, (UInt32)formatId));
                    }
                    #endregion
                }

                sheetData.AppendChild(row);
            }
            #endregion

            worksheet.Append(exColumns);
            worksheet.Append(sheetData);
        }

        internal override void AttachToWorksheet(Worksheet worksheet)
        {
            base.AttachToWorksheet(worksheet);

            AddRows(worksheet);
        }
    }
    #endregion

    #region BsGridExcelBuilder
    public class BsGridExcelBuilder
    {
        #region Constructor and Properties
        protected Dictionary<string, BsGridExcelSheetBuilder> sheetBuilders;
        protected BsGridExcelStyleSheetBuilder styleBuilder;

        public BsGridExcelBuilder()
        {
            this.styleBuilder = new BsGridExcelStyleSheetBuilder();
            this.sheetBuilders = new Dictionary<string, BsGridExcelSheetBuilder>();
        }
        #endregion

        #region Config
        public BsGridExcelSheetBuilder<Z> AddSheet<Z>(IEnumerable<Z> items, string name) where Z : class
        {
            if (this.sheetBuilders.ContainsKey(name)) throw new Exception("Excel sheets must have unique names");

            var builder = new BsGridExcelSheetBuilder<Z>(
                this.styleBuilder,
                items);

            this.sheetBuilders.Add(name, builder);

            return builder;
        }
        #endregion

        #region Helpers
        /// <summary>
        /// Generate basic excel document, filling it with rows representation of the items
        /// </summary>
        /// <param name="spreadsheetDocument"></param>
        private void Generate(SpreadsheetDocument spreadsheetDocument)
        {
            // add a WorkbookPart to the document
            WorkbookPart workbookpart = spreadsheetDocument.AddWorkbookPart();
            workbookpart.Workbook = new Workbook();

            // add Sheets to the Workbook.
            Sheets sheets = spreadsheetDocument.WorkbookPart.Workbook.AppendChild<Sheets>(new Sheets());

            UInt32Value sheetId = 1;
            foreach (var sheetName in this.sheetBuilders.Keys)
            {
                // add a WorksheetPart to the WorkbookPart
                WorksheetPart worksheetPart = workbookpart.AddNewPart<WorksheetPart>();

                // add worksheet
                var ws = new Worksheet();

                var sheetBuilder = this.sheetBuilders[sheetName];

                sheetBuilder.AttachToWorksheet(ws);

                worksheetPart.Worksheet = ws;
                worksheetPart.Worksheet.Save();

                // append a new worksheet and associate it with the workbook.
                Sheet sheet = new Sheet() { Id = spreadsheetDocument.WorkbookPart.GetIdOfPart(worksheetPart), SheetId = sheetId, Name = sheetName };
                sheets.Append(sheet);
                sheetId++;
            }

            // add Stylesheet
            WorkbookStylesPart workbookStylesPart = spreadsheetDocument.WorkbookPart.AddNewPart<WorkbookStylesPart>();
            workbookStylesPart.Stylesheet = this.styleBuilder.Stylesheet;

            workbookpart.Workbook.Save();
        }
        #endregion

        #region Serialize
        /// <summary>
        /// Excel format memory stream representation of the resulting excel file
        /// </summary>
        /// <returns></returns>
        public MemoryStream ToStream()
        {
            MemoryStream memoryStream = new MemoryStream();

            // Create the spreadsheet on the MemoryStream
            SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Create(memoryStream, SpreadsheetDocumentType.Workbook);

            Generate(spreadsheetDocument);

            // Close the document.
            spreadsheetDocument.Close();

            return memoryStream;
        }

        /// <summary>
        /// Excel format byte array representation of the resulting excel file
        /// </summary>
        /// <returns></returns>
        public byte[] ToByteArray()
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

    public class BsGridExcelBuilder<T> : BsGridExcelBuilder where T : class
    {
        #region Constructor and Properties
        public BsGridExcelBuilder() : base() { }
        #endregion

        #region Config
        public BsGridExcelSheetBuilder<T> AddSheet(IEnumerable<T> items, string name)
        {
            return base.AddSheet<T>(items, name);
        }
        #endregion
    }
    #endregion
}

