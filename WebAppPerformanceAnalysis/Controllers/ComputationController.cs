using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebAppPerformanceAnalysis.Controllers.ComputationLogic;
using WebAppPerformanceAnalysis.Models;
using System.Web.Script.Serialization;
using System.Diagnostics;

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


        public ActionResult RayTraceSync()
        {
            ViewBag.Title = "Ray Tracing Synchronous";

            RayTracer rt = new RayTracer();

            int[][] pixelArray = rt.RayTraceScene(false, false);

            ComputationModels model = new ComputationModels();

            int len = pixelArray.Length;

            model.rgba0 = pixelArray[0];
            model.rgba1 = pixelArray[1];
            model.rgba2 = pixelArray[2];
            model.rgba3 = pixelArray[3];
            model.rgba4 = pixelArray[4];
            model.rgba5 = pixelArray[5];
            model.rgba6 = pixelArray[6];
            model.rgba7 = pixelArray[7];
            model.rgba8 = pixelArray[8];
            model.rgba9 = pixelArray[9];

            return View(model);
        }


        public ActionResult RayTraceAsync()
        {
            ViewBag.Title = "Ray Tracing Asynchronous";

            RayTracer rt = new RayTracer();

            int[][] pixelArray = rt.RayTraceScene(true, false);

            ComputationModels model = new ComputationModels();

            int len = pixelArray.Length;

            model.rgba0 = pixelArray[0];
            model.rgba1 = pixelArray[1];
            model.rgba2 = pixelArray[2];
            model.rgba3 = pixelArray[3];
            model.rgba4 = pixelArray[4];
            model.rgba5 = pixelArray[5];
            model.rgba6 = pixelArray[6];
            model.rgba7 = pixelArray[7];
            model.rgba8 = pixelArray[8];
            model.rgba9 = pixelArray[9];

            return View("RayTraceSync", model);
        }

        /// <summary>
        /// Implementation of ray tracer using caching - caches the entire pixel array to be used for subsequent 
        /// identical requests to the server and caches the colour intensities for each pixel.
        /// </summary>
        /// <returns>View of ray trace scene</returns>
        public ActionResult RayTraceCache()
        {
            ViewBag.Title = "Ray Tracing with Caching";
            int[][] cachedPixelArray = (int[][])HttpRuntime.Cache.Get("RayTraceScene");
            int[][] pixelArray;
            if (cachedPixelArray == null)
            {
                RayTracer rt = new RayTracer();

                pixelArray = rt.RayTraceScene(false, true);

                HttpRuntime.Cache.Insert("RayTraceScene", pixelArray);
                Debug.WriteLine("Added to cache");
            }
            else
            {
                pixelArray = cachedPixelArray;
                Debug.WriteLine("Taken from cache");
            }

            ComputationModels model = new ComputationModels();

            int len = pixelArray.Length;

            model.rgba0 = pixelArray[0];
            model.rgba1 = pixelArray[1];
            model.rgba2 = pixelArray[2];
            model.rgba3 = pixelArray[3];
            model.rgba4 = pixelArray[4];
            model.rgba5 = pixelArray[5];
            model.rgba6 = pixelArray[6];
            model.rgba7 = pixelArray[7];
            model.rgba8 = pixelArray[8];
            model.rgba9 = pixelArray[9];

            return View("RayTraceSync", model);
        }
    }
}
