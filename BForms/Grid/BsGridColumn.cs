using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using BForms.Mvc;
using BForms.Models;

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
        internal string PrivateName { get; set; }

        internal Dictionary<string, object> HtmlAttr { get; set; }

        internal bool HasDetails { get; set; }

        internal bool HideDetails { get; set; }

        public PropertyInfo Property { get; set; }

        public bool IsSortable { get; set; }

        public bool IsEditable { get; set; }

        public string EditableContent { get; set; }

        public string DisplayName { get; set; }

        public int? Order { get; set; }

        private List<BsColumnWidth> widthSizes = new List<BsColumnWidth>() 
        { 
            new BsColumnWidth
            {
                ScreenType = BsScreenType.Large,
                Size = 1
            },
            new BsColumnWidth
            {
                ScreenType = BsScreenType.Medium,
                Size = 1
            },
            new BsColumnWidth
            {
                ScreenType = BsScreenType.Small,
                Size = 1
            }
        };

        public List<BsColumnWidth> WidthSizes
        {
            get
            {
                return this.widthSizes;
            }
        }

        internal Func<TRow, object> CellText { get; set; }

        public BsGridColumn(ViewContext viewContext) : base(viewContext) { }

        public BsGridColumn(string name, ViewContext viewContext)
            : base(viewContext)
        {
            this.PrivateName = name;
        }

        public BsGridColumn(PropertyInfo property, ViewContext viewContext)
            : base(viewContext)
        {
            this.Property = property;
            this.PrivateName = this.Property.Name;
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

        public BsGridColumn<TRow> HtmlAttributes(Func<TRow, Dictionary<string, object>> configurator)
        {
            this.HtmlAttr = configurator(new TRow());

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
            this.widthSizes = new List<BsColumnWidth>();
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

        public BsGridColumn<TRow> SetOrder(int order)
        {
            this.Order = order;
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

            if (this.HasDetails)
            {
                var detailsBuilder = new TagBuilder("a");
                detailsBuilder.MergeAttribute("class", "expand bs-toggleExpand");

                if (this.HideDetails)
                {
                    detailsBuilder.MergeAttribute("style", "display:none");
                }

                detailsBuilder.MergeAttribute("href", "#");
                detailsBuilder.InnerHtml += "&nbsp;";

                columnBuilder.InnerHtml += detailsBuilder.ToString();
            }

            if (this.Property != null && this.IsSortable)
            {
                var linkBuilder = new TagBuilder("a");
                linkBuilder.MergeAttribute("href", "#");
                linkBuilder.MergeAttribute("class", "bs-orderColumn");
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
            columnBuilder.MergeAttribute("data-name", this.PrivateName);

            return columnBuilder.ToString();
        }
    }
}