using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebAppPerformanceAnalysis.Controllers
{
    public class ComputationController : Controller
    {
        //
        // GET: /Computation/

        public ActionResult Index()
        {
            ViewBag.Title = "High Computation Tests";
            return View();
        }

    }
}
