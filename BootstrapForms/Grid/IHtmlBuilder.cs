﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BootstrapForms.Grid
{
    public interface IComponent : IHtmlString
    {
        string Render();
    }

    public abstract class BaseComponent : IComponent
    {
        protected ViewContext viewContext;

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
    }
}