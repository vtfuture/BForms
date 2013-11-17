using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using BForms.Models;
using BForms.Renderers;
using System;

namespace BForms.Mvc
{
    public abstract class BsBaseComponent : IHtmlString
    {
        #region Properties and Constructor
        internal string template;
        internal ViewContext viewContext;
        internal BsBaseRenderer renderer;
        internal IDictionary<string, object> htmlAttributes;

        public string TemplatePath
        {
            get
            {
                return this.template;
            }
        }

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
            if (this.viewContext == null)
                throw new Exception("View context is null");

            return this.viewContext.RequestContext.HttpContext.Request.IsAjaxRequest();
        }

        internal string RenderModel<T>(T model)
        {
            if (string.IsNullOrEmpty(this.template))
                throw new Exception("Template path is null or empty");

            if (model == null)
                throw new Exception("Model is null");

            if (this.viewContext == null)
                throw new Exception("View context is null");

            return this.viewContext.Controller.BsRenderPartialView(this.template, model);
        }

        internal T GetInstance<T>() where T : BsBaseComponent
        {
            return (T)this;
        }
        #endregion

        #region ToString ToHtmlString
        public override string ToString()
        {
            if (this.renderer == null)
                throw new Exception("Renderer is null");

            return this.renderer.Render();
        }

        public virtual string ToHtmlString()
        {
            if (this.viewContext == null)
                throw new Exception("View context is null");

            var writer = new System.IO.StringWriter();
            this.viewContext.Writer.Write(this.ToString());
            return writer.ToString();
        }
        #endregion
    }
}