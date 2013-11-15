using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using BForms.Models;
using BForms.Renderers;

namespace BForms.Mvc
{
    public abstract class BsBaseComponent : IHtmlString
    {
        #region Properties and Constructor
        internal string template;
        internal ViewContext viewContext;
        internal BsBaseRenderer renderer;
        internal IDictionary<string, object> htmlAttributes;

        public BsBaseComponent(){}

        public BsBaseComponent(ViewContext viewContext)
        {
            this.viewContext = viewContext;
        }
        #endregion

        #region Config
        public BsBaseComponent HtmlAttributes(IDictionary<string, object> htmlAttributes)
        {
            this.htmlAttributes = htmlAttributes;

            return this;
        }
        #endregion

        #region Helpers
        internal bool IsAjaxRequest()
        {
            return this.viewContext.RequestContext.HttpContext.Request.IsAjaxRequest();
        }

        public T GetInstance<T>() where T : BsBaseComponent
        {
            return (T)this;
        }
        #endregion

        #region ToString ToHtmlString
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
        #endregion
    }
}