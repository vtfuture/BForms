using BForms.Models;
using BForms.Mvc;
using BForms.Renderers;
using BForms.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace BForms.Editor
{
    public class BsEditorHtmlBuilder<TModel> : BsBaseComponent<BsEditorHtmlBuilder<TModel>>
    {
        #region Properties and Constructor
        protected BsEditorTabConfigurator<TModel> tabConfigurator;
        protected BsEditorGroupConfigurator<TModel> groupConfigurator;

        internal BsEditorTabConfigurator<TModel> TabConfigurator
        {
            get
            {
                return this.tabConfigurator;
            }
        }

        internal BsEditorGroupConfigurator<TModel> GroupConfigurator
        {
            get
            {
                return this.groupConfigurator;
            }
        }

        internal string saveUrl { get; set; }

        public BsEditorHtmlBuilder(TModel model)
        {
            this.renderer = new BsEditorRenderer<TModel>(this);
        }

        public BsEditorHtmlBuilder(TModel model, ViewContext viewContext)
            : base(viewContext)
        {
            this.viewContext = viewContext;

            this.renderer = new BsEditorRenderer<TModel>(this);

            this.tabConfigurator = new BsEditorTabConfigurator<TModel>(viewContext);

            this.groupConfigurator = new BsEditorGroupConfigurator<TModel>(viewContext);

            var type = typeof(TModel);

            var props = type.GetProperties();

            var editableTabIds = new List<object>();

            // find out what tabs are editable to send to group configurator TODO refactor somehow (3 foreach ..)
            foreach (var prop in props)
            {
                BsEditorTabAttribute tabAttr = null;

                if (ReflectionHelpers.TryGetAttribute(prop, out tabAttr))
                {
                    if (tabAttr.Editable)
                    {
                        editableTabIds.Add(tabAttr.Id);
                    }
                }
            }

            if (!this.IsAjaxRequest()) // we don't care about groups
            {
                foreach (var prop in props)
                {
                    BsEditorGroupAttribute groupAttr = null;

                    if (ReflectionHelpers.TryGetAttribute(prop, out groupAttr))
                    {
                        var value = prop.GetValue(model);

                        InvokeAddGroupConfig(value, prop, groupAttr, editableTabIds); // TODO send editableTabIds
                    }
                }
            }

            foreach (var prop in props)
            {
                BsEditorTabAttribute tabAttr = null;

                if (ReflectionHelpers.TryGetAttribute(prop, out tabAttr))
                {
                    tabConfigurator.AddNavTab(tabAttr);

                    var value = prop.GetValue(model);

                    InvokeAddTabConfig(value, prop, tabAttr); // this has to happen after group configuration
                }
            }
        }
        #endregion

        #region Public Methods
        public BsEditorHtmlBuilder<TModel> ConfigureTabs(Action<BsEditorTabConfigurator<TModel>> config)
        {
            config(this.tabConfigurator);

            return this;
        }

        public BsEditorHtmlBuilder<TModel> ConfigureGroups(Action<BsEditorGroupConfigurator<TModel>> config)
        {
            if (!this.IsAjaxRequest())
            {
                config(this.groupConfigurator);
            }

            return this;
        }

        public BsEditorHtmlBuilder<TModel> SaveUrl(string saveUrl)
        {
            this.saveUrl = saveUrl;

            return this;
        }
        #endregion

        #region Helpers
        private void InvokeAddGroupConfig(object value, PropertyInfo prop, BsEditorGroupAttribute attr, List<object> editableTabIds)
        {
            var propertyType = prop.PropertyType;
            var propertyName = prop.Name;

            if (!propertyType.GetInterfaces().Contains(typeof(IBsEditorGroupModel)))
                throw new Exception("The model with BsEditorGroupAttribute must inherit BsEditorGroupModel");

            var genericArgs = propertyType.GetGenericArguments();

            var count = genericArgs.Count();

            if (count == 0)
            {
                var baseType = propertyType.BaseType;

                genericArgs = baseType.GetGenericArguments();

                count = genericArgs.Count();
            }

            if (count > 0)
            {
                MethodInfo method = null, generic = null;

                Type rowType = genericArgs[0];

                method = typeof(BsEditorGroupConfigurator<TModel>).GetMethod("Add", this.Bindings());
                generic = method.MakeGenericMethod(propertyType, rowType);

                generic.Invoke(this.groupConfigurator, new object[] { attr, value, editableTabIds.ToArray(), propertyName });
            }
        }

        private void InvokeAddTabConfig(object value, PropertyInfo prop, BsEditorTabAttribute attr)
        {
            var propertyType = prop.PropertyType;

            if (!propertyType.GetInterfaces().Contains(typeof(IBsEditorTabModel)))
                throw new Exception("The model with BsEditorTabAttribute must inherit BsEditorTabModel");

            var genericArgs = propertyType.GetGenericArguments();

            var count = genericArgs.Count();
            if (count == 0)
            {
                var baseType = propertyType.BaseType;

                genericArgs = baseType.GetGenericArguments();

                count = genericArgs.Count();
            }

            if (count > 0)
            {
                MethodInfo method = null, generic = null;

                Type rowType = genericArgs[0];

                method = typeof(BsEditorTabConfigurator<TModel>).GetMethod("Add", this.Bindings());
                generic = method.MakeGenericMethod(propertyType, rowType);

                generic.Invoke(this.tabConfigurator, new object[] { attr, value, this.groupConfigurator.Connections, this.groupConfigurator.GetGroupIds() });
            }
        }

        private BindingFlags Bindings()
        {
            return BindingFlags.Default | BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.NonPublic;
        }
        #endregion
    }
}
