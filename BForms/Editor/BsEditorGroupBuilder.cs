using BForms.Models;
using BForms.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace BForms.Editor
{
    public abstract class BsEditorGroupBuilder : BsBaseComponent
    {
        internal object uid { get; set; }
        internal string name { get; set; }
        internal string text { get; set; }
    }

    public class BsEditorGroupBuilder<TModel> : BsEditorGroupBuilder where TModel : IBsEditorGroupModel
    {
        internal TModel model {get; set;}

        public BsEditorGroupBuilder(IBsEditorGroupModel model, ViewContext viewContext)
        {
            this.viewContext = viewContext;
            this.model = (TModel)model;
        }

        public BsEditorGroupBuilder<TModel> Id(object id)
        {
            this.uid = id;

            return this;
        }

        public BsEditorGroupBuilder<TModel> DisplayText(string text)
        {
            this.text = text;

            return this;
        }

        public BsEditorGroupBuilder<TModel> DisplayName(string name)
        {
            this.name = name;

            return this;
        }
    }
}
