using BForms.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace BForms.Renderers
{
    public abstract class BsBaseRenderer
    {
        public abstract string GetGlyphcon(Glyphicon icon, bool forInput = false);

        public abstract string Render();

        public abstract string RenderAjax();
    }

    public abstract class BsBaseRenderer<TBuilder> : BsBaseRenderer
    {
        private TBuilder _builder;

        public TBuilder Builder
        {
            get
            {
                return this._builder;
            }
        }

        public BsBaseRenderer()
        {

        }

        public BsBaseRenderer(TBuilder builder)
        {
            this._builder = builder;
        }

        public virtual BsBaseRenderer<TBuilder> Register(TBuilder builder)
        {
            this._builder = builder;

            return this;
        }

        public override string GetGlyphcon(Glyphicon icon, bool forInput = false)
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

        public override string Render()
        {
            throw new NotImplementedException();
        }

        public override string RenderAjax()
        {
            throw new NotImplementedException();
        }
    }
}
