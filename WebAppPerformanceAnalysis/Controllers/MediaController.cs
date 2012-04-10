using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Collections;

namespace WebAppPerformanceAnalysis.Controllers
{
    public class MediaController : AsyncController
    {
        //
        // GET: /Media/

        public ActionResult Index()
        {
            ViewBag.Title = "Media Tests";

            return View();
        }

        public ActionResult LoadMediaSynchronous()
        {
            System.Reflection.Assembly a = System.Reflection.Assembly.GetExecutingAssembly();
            DirectoryInfo d = Directory.GetParent(Directory.GetParent(a.CodeBase.ToString().Substring(8)).ToString());
            string[] fileEntries = Directory.GetFiles(d.ToString() + @"/Content", "*.jpg");
            var files = from f in fileEntries
                        where f.EndsWith(".jpg") || f.EndsWith(".png")
                        select f;
            ViewBag.Images = new ArrayList();
            for (int j = 0; j < 5; j++)
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

        public ActionResult MediaCompleted()
        {
            ViewBag.Title = "Async Media Tests";
            return View("Media");
        }

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
            for (int j = 0; j < 5; j++)
            {
                foreach (var fileName in files)
                {

                    AsyncManager.OutstandingOperations.Increment();
                    AsyncWorker l = new AsyncWorker();
                    l.LoadImageCompleted += (sender, e) =>
                    {
                        // Place image contents in an array of images in viewbag
                        ViewBag.Images.Add("data:image/jpeg;base64," + @e.ImageContents);

                        //Response.Write("<img alt=\"\" src=\"data:image/jpeg;base64," + @e.ImageContents + "\" />");
                        //Response.Write("<img src=\"../.." + fileName.Substring(fileName.LastIndexOf("/")) + "\" alt=\"\" />");
                        //Response.Flush();
                        AsyncManager.OutstandingOperations.Decrement();
                    };

                    l.LoadImageAsync((string)fileName);
                }
            }
        }
    }

    public delegate void LoadImageCompletedEventHandler(object sender, LoadImageCompletedEventArgs e);
}
