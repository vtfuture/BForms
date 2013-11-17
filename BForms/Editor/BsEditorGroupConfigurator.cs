using BForms.Models;
using BForms.Mvc;
using BForms.Renderers;
using BForms.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace BForms.Editor
{
    public class BsEditorGroupConfigurator<TModel> : BsBaseConfigurator
    {
        #region Properties and Constructor
        internal Dictionary<object, BsEditorGroupBuilder> Groups { get; set; }

        public BsEditorGroupConfigurator(ViewContext viewContext) : base(viewContext)
        {
            this.Groups = new Dictionary<object, BsEditorGroupBuilder>();
        }
        #endregion

        #region Public Methods
        public BsEditorGroupBuilder<TEditor> For<TEditor>(Expression<Func<TModel, TEditor>> expression)
            where TEditor : IBsEditorGroupModel
        {
            var builder = this.GetGroup(expression);

            return builder as BsEditorGroupBuilder<TEditor>;
        }
        #endregion

        #region Helpers
        private BsEditorGroupBuilder GetGroup<TValue>(Expression<Func<TModel, TValue>> expression)
        {
            var prop = expression.GetPropertyInfo<TModel, TValue>();

            BsEditorGroupAttribute attr = null;

            if (ReflectionHelpers.TryGetAttribute(prop, out attr))
            {
                var id = attr.Id;

                return this.Groups[id];
            }

            throw new Exception("Property " + prop.Name + " has no BsGroupEditorAttribute");
        }

        private void Add<TEditor, TRow>(BsEditorGroupAttribute attr, IBsEditorGroupModel model)
            where TEditor : IBsEditorGroupModel
            where TRow : new()
        {
            var group = new BsEditorGroupBuilder<TEditor>(model, this.viewContext)
                       .Id(attr.Id);

            group.renderer = new BsEditorGroupRenderer<TEditor, TRow>(group);

            InsertGroup<TEditor, TRow>(attr.Id, group);
        }

        private void InsertGroup<TEditor, TRow>(object id, BsEditorGroupBuilder<TEditor> tabBuilder)
            where TEditor : IBsEditorGroupModel
            where TRow : new()
        {
            this.Groups.Add(id, tabBuilder);
        }
        #endregion
    }
}
