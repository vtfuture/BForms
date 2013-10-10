﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BootstrapForms.Models
{
    public class BsGridModel<T>
    {
        public IEnumerable<T> Items { get; set; }

        public BsPagerModel Pager { get; set; }
    }
}
