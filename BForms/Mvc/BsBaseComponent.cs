using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using BForms.Models;
using BForms.Renderers;

namespace BForms.Mvc
{
    public abstract class BsBaseComponent : IHtmlString
    {
        internal string template;
        internal ViewContext viewContext;
        internal BsBaseRenderer renderer;

        public BsBaseComponent()
        {

        }

        public BsBaseComponent(ViewContext viewContext)
        {
            this.viewContext = viewContext;
        }

        internal bool IsAjaxRequest()
        {
            return this.viewContext.RequestContext.HttpContext.Request.IsAjaxRequest();
        }

        public override string ToString()
        {
            return this.renderer.Render();
        }

        public virtual string ToHtmlString()
        {
            var result = this.renderer.Render();

            if (this.viewContext != null)
            {
                var writer = new System.IO.StringWriter();
                this.viewContext.Writer.Write(result);
                return writer.ToString();
            }

            return result;
        }

        public T GetInstance<T>() where T : BsBaseComponent
        {
            return (T)this;
        }
    }
}