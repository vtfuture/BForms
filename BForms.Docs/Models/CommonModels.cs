using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BForms.Grid;

namespace BForms.Docs.Models
{
    [Serializable]
    public class ThemeSettings
    {
        public bool Open { get; set; }
        public BsTheme Theme { get; set; }
    }
}