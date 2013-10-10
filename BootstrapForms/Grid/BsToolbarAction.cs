﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace BootstrapForms.Grid
{
    public class BsToolbarAction<TToolbar> : BaseComponent
    {
        private string descriptorClass;

        private BsButtonType buttonType;
        private Dictionary<BsButtonType, string> buttonTypes = new Dictionary<BsButtonType, string>() 
        { 
            {BsButtonType.None, "" }, 
            {BsButtonType.WithoutText, "without_text"}, 
            {BsButtonType.WithText, "with_text"}
        };

        private string styleClasses;

        private string title;

        private string text;

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
                        this.buttonType = BsButtonType.WithText;
                        this.text = "Add";

                        break;
                    }
                case BsToolbarActionType.Refresh:
                    {
                        this.descriptorClass = "btn-refresh";
                        this.buttonType = BsButtonType.WithoutText;
                        this.text = this.title = "Refresh";

                        break;
                    }
                case BsToolbarActionType.Search:
                    {
                        this.descriptorClass = "btn-search";
                        this.buttonType = BsButtonType.WithoutText;
                        this.text = this.title = "Search";

                        break;
                    }
                case BsToolbarActionType.Print:
                    {
                        this.descriptorClass = "btn-print";
                        this.buttonType = BsButtonType.WithoutText;
                        this.text = this.title = "Print";

                        break;
                    }
            }
        }

        public BsToolbarAction(string descriptorClass, ViewContext viewContext)
            : base(viewContext)
        {
            this.descriptorClass = descriptorClass;
        }

        public BsToolbarAction<TToolbar> DescriptorClass(string descriptorClass)
        {
            this.descriptorClass = descriptorClass;
            return this;
        }

        public BsToolbarAction<TToolbar> ButtonType(BsButtonType btnType)
        {
            this.buttonType = btnType;
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

            var classes = this.descriptorClass;
            if (this.buttonType != BsButtonType.None)
            {
                classes += " " + this.buttonTypes[this.buttonType];
            }
            if (!string.IsNullOrEmpty(this.styleClasses))
            {
                classes += " " + this.styleClasses;
            }

            actionBuilder.MergeAttribute("class", classes);

            actionBuilder.MergeAttribute("href", this.href ?? "#");

            if (!string.IsNullOrEmpty(this.title))
            {
                actionBuilder.MergeAttribute("title", this.title);
            }

            actionBuilder.InnerHtml += this.text;

            return actionBuilder.ToString();
        }
    }
}