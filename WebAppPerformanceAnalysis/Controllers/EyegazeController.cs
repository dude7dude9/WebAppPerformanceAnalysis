using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebAppPerformanceAnalysis.Controllers
{
    /// <summary>
    /// Can be ignored for SOFTENG751 project.
    /// </summary>
    public class EyegazeController : Controller
    {
        //
        // GET: /EyeGaze/

        public ActionResult Index()
        {
            ViewBag.Message = "Welcome to ASP.NET MVC!";
            return View();
        }

        public ActionResult TechSearchArticle()
        {
            return View();
        }

        public ActionResult StandardSearchArticle()
        {
            return View();
        }

        public ActionResult TechReadArticle()
        {
            return View();
        }

        public ActionResult StandardReadArticle()
        {
            return View();
        }
    }
}
