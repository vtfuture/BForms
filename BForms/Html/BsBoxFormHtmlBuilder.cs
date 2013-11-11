using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using BForms.Mvc;

namespace BForms.Html
{
    public class BsBoxFormHtmlBuilder : BaseComponent
    {
        private string _name;
        private bool _isEditable;
        private bool _isExpanded;
        private string _readonlyUrl;
        private string _editableUrl;
        private string _content;

        public BsBoxFormHtmlBuilder Name(string name)
        {
            this._name = name;
            return this;
        }

        public BsBoxFormHtmlBuilder IsEditable(bool isEditable)
        {
            this._isEditable = isEditable;
            return this;
        }

        public BsBoxFormHtmlBuilder IsExpanded(bool isExpanded)
        {
            this._isExpanded = isExpanded;
            return this;
        }

        public BsBoxFormHtmlBuilder SetReadonlyUrl(string url)
        {
            this._readonlyUrl = url;
            return this;
        }

        public BsBoxFormHtmlBuilder SetEditableUrl(string url)
        {
            this._editableUrl = url;
            this._isEditable = true;

            return this;
        }

        public BsBoxFormHtmlBuilder SetContent(string content)
        {
            this._content = content;
            return this;
        }

        public override string Render()
        {
            var container = new TagBuilder("div");
            container.AddCssClass("panel panel-default");

            var headerTag = new TagBuilder("div");
            headerTag.AddCssClass("panel-heading");

            var headerTitleTag = new TagBuilder("h4");
            headerTitleTag.AddCssClass("panel-title");

            var nameTag = new TagBuilder("a");
            nameTag.AddCssClass("bs-toggleBox");
            nameTag.MergeAttribute("href","#");

            var caretTag = new TagBuilder("span");
            caretTag.AddCssClass("caret");

            nameTag.InnerHtml += caretTag.ToString();
            nameTag.InnerHtml += this._name;

            headerTitleTag.InnerHtml += nameTag.ToString();


            if (this._isEditable)
            {
                var editableTag = new TagBuilder("a");
                editableTag.MergeAttribute("href", "#");
                editableTag.AddCssClass("pull-right bs-editBox");

                var glyphTag = new TagBuilder("span");
                glyphTag.AddCssClass("glyphicon glyphicon-pencil");

                editableTag.InnerHtml += glyphTag.ToString();

                headerTitleTag.InnerHtml += editableTag.ToString();
            }


            return string.Empty;
        }
    }
}
