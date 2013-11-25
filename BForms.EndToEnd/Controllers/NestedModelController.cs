using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using BForms.EndToEnd.Models;

namespace BForms.EndToEnd.Controllers
{
    public class NestedModelController : Controller
    {
        //
        // GET: /NestedModel/
        public ActionResult Index()
        {
            var model = new Car();
            model.Tires = new List<Tire>();
            model.Tires.Add(new Tire{ Manufacturer = "Goodyear", ConstructionType = ConstructionType.Airless});
            model.Tires.Add(new Tire { Manufacturer = "Lego Group", ConstructionType = ConstructionType.BeltedBias });
            model.Tires.Add(new Tire { Manufacturer = "Bridgestone", ConstructionType = ConstructionType.Bias });
            return View(model);
        }
	}
}