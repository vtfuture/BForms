using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using BForms.Grid;
using BForms.Models;
using BForms.Mvc;
using DocumentFormat.OpenXml.Office2010.PowerPoint;

namespace BForms.Panels.ControlPanel
{
    public interface IBsControlPanelCustomTab<TBuilder> where TBuilder : BsBaseComponent
    {
        TBuilder GetBuilder();
        string Render();
    }

    public class BsControlPanelGridTab<TModel, TRow> : IBsControlPanelCustomTab<BsGridHtmlBuilder<TModel, TRow>> where TRow: BsItemModel, new()
    {
        private BsGridHtmlBuilder<TModel, TRow> _builder;

        public BsControlPanelGridTab()
        {
            
        }

        public BsGridHtmlBuilder<TModel, TRow> GetBuilder()
        {
            return _builder;
        }

        public string Render()
        {
            return _builder.ToString();
        }
    }
}
