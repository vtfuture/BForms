﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace BootstrapForms.Utilities
{
    internal static class  TagBuilderExtensions
    {
        internal static void MergeClassAttribute(this TagBuilder tagBuilder, string value, Dictionary<string, object> dictAttr)
        {
            var key = "class";

            if (dictAttr != null && dictAttr.ContainsKey(key))
            {
                value += " " + dictAttr[key];
                dictAttr.Remove(key);
            }

            tagBuilder.MergeAttribute(key, value, true);
        }
    }
}
