using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using BForms.Models;

namespace BForms.Grid
{
    public class BsToolbarAction<TToolbar> : BaseComponent
    {
        #region Properties and constructors
        private string descriptorClass;

        private string styleClasses;

        private string title;

        private string text;

        private Glyphicon? glyphIcon;

        private string href;

        private Func<TToolbar, MvcHtmlString> tabDelegate;
        public Func<TToolbar, MvcHtmlString> TabDelegate
        {
            get
            {
                return this.tabDelegate;
            }
        }

        public BsToolbarAction(ViewContext viewContext)
            :base(viewContext) { }

        public BsToolbarAction(BsToolbarActionType type, ViewContext viewContext)
            : base(viewContext)
        {
            switch (type)
            {
                case BsToolbarActionType.Add:
                    {
                        this.descriptorClass = "btn-add";
                        this.glyphIcon = Glyphicon.Plus;
                        this.text = "Add";
                        break;
                    }
                case BsToolbarActionType.Refresh:
                    {
                        this.descriptorClass = "btn-refresh";
                        this.title = "Refresh";
                        this.glyphIcon = Glyphicon.Refresh;
                        break;
                    }
                case BsToolbarActionType.AdvancedSearch:
                    {
                        this.descriptorClass = "btn-search btn_advanced_search";
                        this.title = "Advanced Search";
                        this.glyphIcon = Glyphicon.Filter;
                        break;
                    }
            }
        }

        public BsToolbarAction(string descriptorClass, ViewContext viewContext)
            : base(viewContext)
        {
            this.descriptorClass = descriptorClass;
        }
        #endregion

        public BsToolbarAction<TToolbar> DescriptorClass(string descriptorClass)
        {
            this.descriptorClass = descriptorClass;
            return this;
        }

        public BsToolbarAction<TToolbar> StyleClasses(string styleClasses)
        {
            this.styleClasses = styleClasses;
            return this;
        }

        public BsToolbarAction<TToolbar> Title(string title)
        {
            this.title = title;
            return this;
        }

        public BsToolbarAction<TToolbar> Text(string text)
        {
            this.text = text;
            return this;
        }

        public BsToolbarAction<TToolbar> GlyphIcon(Glyphicon icon)
        {
            this.glyphIcon = icon;
            return this;
        }

        public BsToolbarAction<TToolbar> Tab(Func<TToolbar, MvcHtmlString> tabDelegate)
        {
            this.tabDelegate = tabDelegate;
            return this;
        }

        public BsToolbarAction<TToolbar> Action(string action)
        {
            this.href = action;
            return this;
        }

        public override string Render()
        {
            var actionBuilder = new TagBuilder("a");
            actionBuilder.AddCssClass(descriptorClass);
            actionBuilder.AddCssClass(this.styleClasses);
            actionBuilder.MergeAttribute("href", this.href ?? "#");

            if (!string.IsNullOrEmpty(this.title))
            {
                actionBuilder.MergeAttribute("title", this.title);
            }

            actionBuilder.InnerHtml += (this.glyphIcon.HasValue ? GetGlyphcon(this.glyphIcon.Value) + " " : "") + this.text;

            return actionBuilder.ToString();
        }
    }
}