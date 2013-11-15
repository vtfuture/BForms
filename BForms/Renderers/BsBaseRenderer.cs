using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BForms.Renderers
{
    public abstract class BsBaseRenderer<TBuilder>
    {
        private TBuilder _builder;

        protected TBuilder Builder
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
    }
}
