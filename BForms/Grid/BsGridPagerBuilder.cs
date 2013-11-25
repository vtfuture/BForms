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
        internal BsTheme theme = BsTheme.Default;

        public BsGridPagerBuilder(BsPagerModel pager, BsPagerSettings settings, BsGridBaseRepositorySettings baseSettings)
        {
            this.renderer = new BsPagerBaseRenderer(this);
            this.pager = pager;
            this.settings = settings;
            this.baseSettings = baseSettings;
        }
        #endregion

        #region Config
        public BsGridPagerBuilder Theme(BsTheme theme)
        {
            this.theme = theme;

            return this;
        }
        #endregion
    }
}
