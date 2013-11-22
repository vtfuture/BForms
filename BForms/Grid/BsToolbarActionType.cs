﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BForms.Grid
{
    /// <summary>
    /// Toolbar built-in grid actions
    /// </summary>
    public enum BsToolbarActionType
    {
        /// <summary>
        /// Triggers the grid current page refresh ajax 
        /// Preserves search filters and current page but deselects all rows
        /// </summary>
        Refresh = 1,
        /// <summary>
        /// Toggles search form 
        /// To be renderd after quick search input
        /// </summary>
        AdvancedSearch = 2,
        /// <summary>
        /// Toggles create form
        /// </summary>
        Add = 3,
        /// <summary>
        /// Toggles order form
        /// </summary>
        Order = 4
    }

    /// <summary>
    /// Button Group built-in actions
    /// </summary>
    public enum BsToolbarItemGroupType
    {
        /// <summary>
        /// Adds a link in button group
        /// </summary>
        ActionLink = 1,
        /// <summary>
        ///  Adds a separator in button group
        /// </summary>
        Separator = 2
    }
}