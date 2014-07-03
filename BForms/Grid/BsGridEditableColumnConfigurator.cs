using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using BForms.FormBuilder;
using BForms.Models;

namespace BForms.Grid
{
    public class BsGridEditableColumnConfigurator
    {
        private BsFormHtmlRenderer renderer;

        private HtmlHelper helper;

        private string updateUrl;

        private string formString;

        public BsGridEditableColumnConfigurator(ViewContext viewContext)
        {
            helper = new HtmlHelper(viewContext, new ViewPage());

            renderer = new BsFormHtmlRenderer(helper);
        }

        #region Fluent API

        public BsGridEditableColumnConfigurator UpdateUrl(string url)
        {
            updateUrl = url;

            return this;
        }

        public BsGridEditableColumnConfigurator FormModel<TFormModel>(TFormModel formModel = null) where TFormModel : class
        {
            formString = renderer.RenderForm(formModel);

            return this;
        }

        #endregion

        #region Public methods

        public string GetRenderedForm()
        {
            return formString;
        }

        #endregion
    }
}
