using BForms.Models;
using BForms.Mvc;
using BForms.Renderers;
using BForms.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace BForms.Grid
{
    public class BsGridPagerBuilder : BsBaseComponent<BsGridPagerBuilder>
    {
        #region Properties and Constructor
        internal BsPagerModel pager { get; set; }
        internal BsPagerSettings settings { get; set; }
        internal BsGridBaseRepositorySettings baseSettings { get; set; }

        public BsGridPagerBuilder(BsPagerModel pager, BsPagerSettings settings, BsGridBaseRepositorySettings baseSettings)
        {
            this.renderer = new BsPagerBaseRenderer(this);
            this.pager = pager;
            this.settings = settings;
            this.baseSettings = baseSettings;
        }

        public BsGridPagerBuilder Settings(BsPagerSettings settings)
        {
            this.settings = settings;
            return this;
        }

        public BsGridPagerBuilder BaseSettings(BsGridBaseRepositorySettings baseSettings)
        {
            this.baseSettings = baseSettings;
            return this;
        }
        #endregion
    }
}
