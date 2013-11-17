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

namespace BForms.Editor
{
    public class BsEditorTabConfigurator<TModel> : BsBaseConfigurator
    {
        #region Properties and Constructor
        internal Dictionary<object, BsEditorTabBuilder> Tabs { get; set; }
        internal BsEditorNavBuilder navBuilder { get; set; }

        public BsEditorTabConfigurator(ViewContext viewContext) : base(viewContext)
        {
            this.Tabs = new Dictionary<object, BsEditorTabBuilder>();
            this.navBuilder = new BsEditorNavBuilder(viewContext);
            this.viewContext = viewContext;
        }
        #endregion

        #region Public Methods
        public BsEditorTabBuilder<TEditor> For<TEditor>(Expression<Func<TModel, TEditor>> expression)
            where TEditor : IBsEditorTabModel
        {
            var builder = this.GetTab(expression);

            return builder as BsEditorTabBuilder<TEditor>;
        }
        #endregion

        #region Helpers
        private void InsertTab<TEditor, TRow>(object id, BsEditorTabBuilder<TEditor> tabBuilder)
            where TEditor : IBsEditorTabModel
            where TRow : new ()
        {
            if (tabBuilder.IsSelected())
            {
                foreach (var tab in this.Tabs)
                {
                    if (tab.Key == id)
                    {
                        throw new Exception("Duplicate tab id " + id.ToString());
                    }

                    if (tab.Value.IsSelected())
                    {
                        tab.Value.SetSelected(false);
                    }
                }
            }
            this.Tabs.Add(id, tabBuilder);
        }

        private BsEditorTabBuilder GetTab<TValue>(Expression<Func<TModel, TValue>> expression)
        {
            var prop = expression.GetPropertyInfo<TModel, TValue>();

            BsEditorTabAttribute attr = null;

            if (ReflectionHelpers.TryGetAttribute(prop, out attr))
            {
                var id = attr.Id;

                return this.Tabs[id];
            }

            throw new Exception("Property " + prop.Name + " has no BsGroupEditorAttribute");
        }

        internal void AddNavTab(BsEditorTabAttribute attr)
        {
            navBuilder.AddTab(attr);
        }

        private void Add<TEditor, TRow>(BsEditorTabAttribute attr, BsEditorTabModel<TRow> model)
            where TEditor : IBsEditorTabModel
            where TRow : new()
        {
            var tab = new BsEditorTabBuilder<TEditor>(model, this.viewContext)
                       .DisplayName(attr.Name)
                       .Id(attr.Id)
                       .Selected(attr.Selected);

            tab.InitRenderer<TRow>();

            if (attr.Selected)
            {
                navBuilder.Selected(attr.Id);
            }

            InsertTab<TEditor, TRow>(attr.Id, tab);
        }
        #endregion
    }
}
