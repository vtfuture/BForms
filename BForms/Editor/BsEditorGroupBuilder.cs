using BForms.Models;
using BForms.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using BForms.Utilities;
using BForms.Renderers;

namespace BForms.Editor
{
    public abstract class BsEditorGroupBuilder : BsBaseComponent
    {
        protected object uid { get; set; }
        protected string name { get; set; }
        protected string text { get; set; }
        protected string propertyName { get; set; }

        internal string PropertyName
        {
            get
            {
                return this.propertyName;
            }

        }
        internal object Uid
        {
            get
            {
                return this.uid;
            }
            
        }

        internal string Name
        {
            get
            {
                return this.name;
            }
        }

        internal string Text
        {
            get
            {
                return this.text;
            }
        }
    }

    public class BsEditorGroupBuilder<TModel> : BsEditorGroupBuilder where TModel : IBsEditorGroupModel
    {
        protected TModel model { get; set; }
        protected Dictionary<string, BsEditorFormBuilder> forms { get; set; }
        protected object[] editableTabIds { get; set; }

        protected bool isReadonly { get; set; }

        internal object[] EditableTabIds
        {
            get
            {
                return this.editableTabIds;
            }
        }

        internal Dictionary<string, BsEditorFormBuilder> RowForms
        {
            get
            {
                return this.forms;
            }
        }

        internal bool IsReadonly
        {
            get
            {
                return this.isReadonly;
            }
        }

        internal TModel Model
        {
            get
            {
                return this.model;
            }
        }

        public BsEditorGroupBuilder(ViewContext viewContext)
        {
            this.viewContext = viewContext;
        }

        public BsEditorGroupBuilder(IBsEditorGroupModel model, ViewContext viewContext, object[] editableTabIds)
        {
            this.editableTabIds = editableTabIds;
            this.forms = new Dictionary<string, BsEditorFormBuilder>();
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

        public BsEditorGroupBuilder<TModel> Readonly(bool isReadonly)
        {
            this.isReadonly = isReadonly;

            return this;

        }

        internal BsEditorGroupBuilder<TModel> SetPropertyName(string propertyName)
        {
            this.propertyName = propertyName;

            return this;
        }

        public BsEditorGroupBuilder<TModel> Template<TValue>(Expression<Func<TModel, TValue>> expression, string template) where TValue : class
        {
            var key = this.model.GetPropertyName(expression);

            if (key == "Items")
            {
                this.template = template;
            }
            else
            {
                if (this.model == null)
                {
                    throw new Exception("Trying to set property " + key + " from group model " + this.uid + ". Model is null");
                }

                var model = (TValue)expression.GetPropertyInfo().GetValue(this.model);

                if (model == null)
                {
                    throw new Exception("Property " + key + " is null" + " from group model " + this.uid);
                }

                var formBuilder = new BsEditorFormBuilder<TValue>(model, key, this.viewContext).Template(template);

                this.forms.Add(key, formBuilder);
            }

            return this;
        }

        internal void RegisterRenderer<TRow, TForm>() where TRow : BsEditorGroupItemModel<TForm>, new()
        {
            this.renderer = new BsEditorGroupRenderer<TModel, TRow, TForm>(this);
        }

        
    }
}
