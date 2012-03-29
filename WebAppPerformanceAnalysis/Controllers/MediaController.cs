using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Timers;

namespace WebAppPerformanceAnalysis.Controllers
{
    public class MediaController : Controller
    {
        //
        // GET: /Media/

        public ActionResult Index()
        {
            ViewBag.Title = "Media Tests";
            return View();
        }

        public ActionResult LoadUnicorns()
        {
            return View("Unicorns");
        }


    }
}
