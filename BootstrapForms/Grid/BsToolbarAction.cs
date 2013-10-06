using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BootstrapForms.Grid
{
    public class BsToolbarAction : IHtmlBuilder
    {
        private readonly HtmlHelper htmlHelper;
        public HtmlHelper HtmlHelper
        {
            get
            {
                return this.htmlHelper;
            }
        }

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

        private string tabHtml;
        public string TabHtml
        {
            get
            {
                return this.tabHtml;
            }
        }

        public BsToolbarAction() { }

        public BsToolbarAction(BsToolbarActionType type)
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

        public BsToolbarAction(string descriptorClass)
        {
            this.descriptorClass = descriptorClass;
        }

        public BsToolbarAction DescriptorClass(string descriptorClass)
        {
            this.descriptorClass = descriptorClass;
            return this;
        }

        public BsToolbarAction ButtonType(BsButtonType btnType)
        {
            this.buttonType = btnType;
            return this;
        }

        public BsToolbarAction StyleClasses(string styleClasses)
        {
            this.styleClasses = styleClasses;
            return this;
        }

        public BsToolbarAction Title(string title)
        {
            this.title = title;
            return this;
        }

        public BsToolbarAction Text(string text)
        {
            this.text = text;
            return this;
        }

        public BsToolbarAction Tab(string html)
        {
            this.tabHtml = html;
            return this;
        }

        public MvcHtmlString Render()
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

            actionBuilder.MergeAttribute("href", "#");

            if (!string.IsNullOrEmpty(this.title))
            {
                actionBuilder.MergeAttribute("title", this.title);
            }

            actionBuilder.InnerHtml += this.text;

            return new MvcHtmlString(actionBuilder.ToString());
        }
    }
}