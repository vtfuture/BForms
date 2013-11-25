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
    #region ExcelHelpers
    public static class ExcelHelpers
    {
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
        public static Font CreateFont(bool isBold, bool isItalic, string fontFamily, System.Double? size, string color)
        {
            Font font = new Font();

            if (size.HasValue)
            {
                FontSize fontSize = new FontSize() { Val = size };
                font.Append(fontSize);
            }

            if (isItalic)
            {
                Italic italic = new Italic();
                font.Append(italic);
            }

            if (isBold)
            {
                Bold bold = new Bold();
                font.Append(bold);
            }

            if (color != null)
            {
                Color color1 = new Color() { Rgb = color };
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
        /// <param name="fillColorRgb">The foreground color of the fill</param>
        /// <param name="pattern">The fill pattern</param>
        /// <returns></returns>
        public static Fill CreateFill(string fillColorRgb, PatternValues? pattern = null)
        {
            PatternFill patternFill = new PatternFill();

            if (!string.IsNullOrEmpty(fillColorRgb))
            {
                patternFill.PatternType = pattern ?? PatternValues.Solid;
                ForegroundColor foregroundColor = new ForegroundColor() { Rgb = fillColorRgb };
                BackgroundColor backgroundColor = new BackgroundColor() { Indexed = (UInt32Value)64U };
                patternFill.Append(foregroundColor);
                patternFill.Append(backgroundColor);
            }
            else
            {
                patternFill.PatternType = pattern ?? PatternValues.None;
            }

            return new Fill(patternFill);
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
