using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using BForms.Models;

namespace BForms.Mvc
{
    public abstract class BaseComponent : IComponent
    {
        internal ViewContext viewContext;

        private Dictionary<string, object> htmlAttributes;

        public BaseComponent() { }

        public BaseComponent(ViewContext viewContext)
        {
            this.viewContext = viewContext;
        }

        public abstract string Render();

        public virtual string ToHtmlString()
        {
            var writer = new System.IO.StringWriter();
            this.viewContext.Writer.Write(this.Render());
            return writer.ToString();
        }

        protected string GetGlyphcon(Glyphicon icon, bool forInput = false)
        {
            var spanTag = new TagBuilder("span");
            spanTag.AddCssClass(Utilities.ReflectionHelpers.GetDescription(icon));
            spanTag.AddCssClass("glyphicon");
            if (forInput)
            {
                spanTag.AddCssClass("input-group-addon");
            }
            return spanTag.ToString(TagRenderMode.Normal);
        }
    }

    public interface IComponent : IHtmlString
    {
        string Render();
    }
}