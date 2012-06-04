using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Collections;

using System.Web.Caching;

namespace WebAppPerformanceAnalysis.Controllers
{
    /// <summary>
    /// Represents the controller for the media pages - loads approx 40MB of images from file.
    /// </summary>
    public class MediaController : AsyncController
    {
        //
        // GET: /Media/
        public ActionResult Index()
        {
            ViewBag.Title = "Media Tests";

            return View();
        }

        /// <summary>
        /// Loads all JPG and PNG images and sends them to the view synchronously.
        /// </summary>
        /// <returns>View of images</returns>
        public ActionResult LoadMediaSynchronous()
        {
            System.Reflection.Assembly a = System.Reflection.Assembly.GetExecutingAssembly();
            DirectoryInfo d = Directory.GetParent(Directory.GetParent(a.CodeBase.ToString().Substring(8)).ToString());
            string[] fileEntries = Directory.GetFiles(d.ToString() + @"/Content", "*.jpg");
            var files = from f in fileEntries
                        where f.EndsWith(".jpg") || f.EndsWith(".png")
                        select f;
            ViewBag.Images = new ArrayList();
            for (int j = 0; j < 10; j++)
            {
                foreach (var fileName in files)
                {
                    AsyncWorker l = new AsyncWorker();
                    String contents = l.LoadImageAsync((string)fileName);
                    // Place image contents in an array of images in viewbag
                    ViewBag.Images.Add("data:image/jpeg;base64," + contents);
                }
            }
            return View("Media");
        }

        /// <summary>
        /// Callback method for MediaAsync().
        /// </summary>
        /// <returns>View of images</returns>
        public ActionResult MediaCompleted()
        {
            ViewBag.Title = "Async Media Tests";
            return View("Media");
        }

        /// <summary>
        /// Loads all images using the AsyncWorker class - will not return until the number of outstanding operations
        /// is zero.
        /// </summary>
        public void MediaAsync()
        {
            System.Reflection.Assembly a = System.Reflection.Assembly.GetExecutingAssembly();
            DirectoryInfo d = Directory.GetParent(Directory.GetParent(a.CodeBase.ToString().Substring(8)).ToString());

            string[] fileEntries = Directory.GetFiles(d.ToString() + @"/Content", "*.jpg");
            var files = from f in fileEntries
                        where f.EndsWith(".jpg") || f.EndsWith(".png")
                        select f;
            AsyncManager.Timeout = 10000;
            ViewBag.Images = new ArrayList();
            for (int j = 0; j < 10; j++)
            {
                foreach (var fileName in files)
                {
                    AsyncManager.OutstandingOperations.Increment();
                    AsyncWorker l = new AsyncWorker();
                    l.LoadDataCompleted += (sender, e) =>
                    {
                        // Place image contents in an array of images in viewbag
                        ViewBag.Images.Add("data:image/jpeg;base64," + @e.ImageContents);

                        AsyncManager.OutstandingOperations.Decrement();
                    };

                    l.LoadImageAsync((string)fileName);
                }
            }
        }

        /// <summary>
        /// Loads the same images from file but caches the images according to their file name.
        /// </summary>
        /// <returns>View of images</returns>
        [OutputCache(Duration = 60, VaryByParam = "none")]
        public ActionResult LoadCache()
        {
            System.Reflection.Assembly a = System.Reflection.Assembly.GetExecutingAssembly();
            DirectoryInfo d = Directory.GetParent(Directory.GetParent(a.CodeBase.ToString().Substring(8)).ToString());
            string[] fileEntries = Directory.GetFiles(d.ToString() + @"/Content", "*.jpg");
            var files = from f in fileEntries
                        where f.EndsWith(".jpg") || f.EndsWith(".png")
                        select f;
            ViewBag.Images = new ArrayList();
            for (int j = 0; j < 10; j++)
            {
                foreach (var fileName in files)
                {
                    AsyncWorker l = new AsyncWorker();
                    String contents = (String)HttpRuntime.Cache.Get(fileName); // Check if file contents exist in cache
                    if (contents == null)
                    {
                        contents = l.LoadImageAsync((string)fileName);
                        // Insert contents into cache, specifying a 1 minute timeout policy
                        HttpRuntime.Cache.Insert(fileName, contents, null, System.Web.Caching.Cache.NoAbsoluteExpiration, new TimeSpan(0, 1, 0));
                    }
                    // Place image contents in an array of images in viewbag
                    ViewBag.Images.Add("data:image/jpeg;base64," + contents);
                }
            }
            return View("Media");
        }
    }

}
