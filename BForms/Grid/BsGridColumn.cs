using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using BForms.Mvc;

namespace BForms.Grid
{
    /// <summary>
    ///  Bootstrap grid system mods
    /// </summary>
    public enum BsScreenType
    {
        /// <summary>
        /// Large devices Desktops and TVs
        /// </summary>
        Large = 1,
        /// <summary>
        /// Medium devices Desktops and Tablets
        /// </summary>
        Medium = 2,
        /// <summary>
        /// Small devices Tablets and Phones
        /// </summary>
        Small = 3
    }

    /// <summary>
    /// Grid column width helper class
    /// </summary>
    public class BsColumnWidth
    {
        public BsScreenType ScreenType { get; set; }
        public int Size { get; set; }
    }

    public class BsGridColumn<TRow> : BaseComponent where TRow : new()
    {
        public PropertyInfo Property { get; set; }

        public bool IsSortable { get; set; }

        public bool IsEditable { get; set; }

        public string EditableContent { get; set; }

        public string DisplayName { get; set; }

        private List<BsColumnWidth> widthSizes = new List<BsColumnWidth>();

        public List<BsColumnWidth> WidthSizes
        {
            get
            {
                return this.widthSizes;
            }
        }

        public Func<TRow, object> CellText { get; set; }

        public BsGridColumn(ViewContext viewContext) : base(viewContext) { }

        public BsGridColumn(PropertyInfo property, ViewContext viewContext)
            : base(viewContext)
        {
            this.Property = property;
        }

        public BsGridColumn<TRow> Name(string name)
        {
            this.DisplayName = name;
            return this;
        }

        public BsGridColumn<TRow> Editable(Func<TRow, string> configurator)
        {
            this.EditableContent = configurator(new TRow());
            this.IsEditable = true;
            return this;
        }

        public BsGridColumn<TRow> Sortable()
        {
            this.IsSortable = true;
            return this;
        }

        public BsGridColumn<TRow> SetWidth(int width)
        {
            return this.SetWidth(width, width);
        }

        public BsGridColumn<TRow> SetWidth(int largeWidth, int normalWidth)
        {
            return this.SetWidth(largeWidth, normalWidth, normalWidth);
        }

        public BsGridColumn<TRow> SetWidth(int largeWidth, int mediumWidth, int smallWidth)
        {
            this.widthSizes.Add(new BsColumnWidth
            {
                ScreenType = BsScreenType.Large,
                Size = largeWidth
            });

            this.widthSizes.Add(new BsColumnWidth
            {
                ScreenType = BsScreenType.Medium,
                Size = mediumWidth
            });

            this.widthSizes.Add(new BsColumnWidth
            {
                ScreenType = BsScreenType.Small,
                Size = smallWidth
            });

            return this;
        }

        public BsGridColumn<TRow> Text(Func<TRow, object> cellText)
        {
            this.CellText = cellText;
            return this;
        }

        public virtual string GetWidthClasses()
        {
            var classes = string.Empty;

            foreach (var item in this.widthSizes)
            {
                var classPrefix = string.Empty;

                switch (item.ScreenType)
                {
                    case BsScreenType.Large:
                        {
                            classPrefix = "col-lg-";
                            break;
                        }
                    case BsScreenType.Medium:
                        {
                            classPrefix = "col-md-";
                            break;
                        }
                    case BsScreenType.Small:
                        {
                            classPrefix = "col-sm-";
                            break;
                        }
                }

                classes += " " + classPrefix + item.Size;
            }

            return classes;
        }

        public override string Render()
        {
            var columnBuilder = new TagBuilder("div");
            if (this.IsSortable)
            {
                var linkBuilder = new TagBuilder("a");
                linkBuilder.MergeAttribute("data-name", this.Property.Name);
                linkBuilder.MergeAttribute("href", "#");
                linkBuilder.InnerHtml = this.DisplayName;

                columnBuilder.InnerHtml += linkBuilder.ToString();
            }
            else
            {
                columnBuilder.InnerHtml += this.DisplayName;
            }

            if (this.IsEditable)
            {
                columnBuilder.InnerHtml += this.EditableContent;
            }

            columnBuilder.AddCssClass(this.GetWidthClasses());

            return columnBuilder.ToString();
        }
    }
}