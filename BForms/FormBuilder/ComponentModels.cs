using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using BForms.Models;
using BForms.Utilities;

namespace BForms.FormBuilder
{
    public class BsComponentModel
    {
        protected string TagName { get; set; }
        protected BsComponentType Type { get; set; }

        public string Text { get; set; }
        public Glyphicon? Glyphicon { get; set; }
        public BsComponentStatus Status { get; set; }

        public BsComponentModel(string text, BsComponentStatus status = BsComponentStatus.None, Glyphicon? glyphicon = null)
        {
            Text = text;
            Status = status;
            Glyphicon = glyphicon;
        }

        public override string ToString()
        {
            var tagBuilder = new TagBuilder(TagName);

            var componentClass = Type.GetDescription();
            var componentStatus = Status.GetDescription();

            var cssClass = componentClass + " " + componentClass + "-" + componentStatus;

            tagBuilder.AddCssClass(cssClass);

            var innerHtml = String.Empty;

            if (Glyphicon != null)
            {
                var glyphiconBuilder = new TagBuilder("span");

                glyphiconBuilder.AddCssClass("glyphicon " + Glyphicon.Value.GetDescription());

                innerHtml += glyphiconBuilder.ToString();
            }

            if (!String.IsNullOrEmpty(Text))
            {
                innerHtml += " " + Text;
            }

            tagBuilder.InnerHtml = innerHtml;

            return tagBuilder.ToString();
        }
    }

    public class BsButtonModel : BsComponentModel
    {
        public BsButtonModel(string text, BsComponentStatus status = BsComponentStatus.None, Glyphicon? glyphicon = null)
            : base(text, status, glyphicon)
        {
            TagName = "button";
            Type = BsComponentType.Button;
        }
    }
}
