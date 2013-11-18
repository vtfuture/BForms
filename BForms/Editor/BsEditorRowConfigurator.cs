using BForms.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace BForms.Editor
{
    public interface IBsEditorRowConfigurator
    {

    }

    public class BsEditorRowConfigurator<TRow> : BsBaseConfigurator, IBsEditorRowConfigurator
    {
        internal Expression<Func<TRow, string>> avatarExpression { get; set; }

        internal Expression<Func<TRow, Dictionary<string, object>>> htmlExpression { get; set; }


        public BsEditorRowConfigurator(ViewContext viewContext) : base(viewContext)
        {

        }

        public BsEditorRowConfigurator<TRow> Avatar(Expression<Func<TRow, string>> expression)
        {
            this.avatarExpression = expression;

            return this;
        }

        public BsEditorRowConfigurator<TRow> HtmlAttributes(Expression<Func<TRow, Dictionary<string, object>>> expression)
        {
            this.htmlExpression = expression;

            return this;
        }
    }
}
