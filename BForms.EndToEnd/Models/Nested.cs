using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using BForms.Models;
using BForms.Mvc;

namespace BForms.EndToEnd.Models
{
    public class Car
    {
        public List<Tire> Tires { get; set; } 
    }

    public class Tire
    {
        public string Manufacturer { get; set; }
        [BsControl(BsControlType.RadioButtonList)]
        public BsSelectList<ConstructionType> ConstructionType { get; set; }
    }

    public enum ConstructionType
    {
        Bias,
        BeltedBias,
        Radial,
        Solid,
        Semipneumatic,
        Airless
    }
}