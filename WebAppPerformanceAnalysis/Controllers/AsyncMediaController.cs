using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Timers;
using System.Threading;
using System.Threading.Tasks;
using System.IO;

namespace WebAppPerformanceAnalysis.Controllers
{
    public class AsyncMediaController : AsyncController
    {
        //
        // GET: /Media/

        public ActionResult Index()
        {
            ViewBag.Title = "Async Media Tests";
            return View();
        }

        public ActionResult UnicornsCompleted()
        {
            return View("AsyncMediaView");
        }

        public void UnicornsAsync()
        {
            string pwd = @"C:\Users\BB\WebAppPerformanceAnalysis\WebAppPerformanceAnalysis\Controllers"; // Need to get the path to the "Content" folder??
            pwd = pwd.Remove(pwd.LastIndexOf('\\')) + "\\Content";
            Console.WriteLine(pwd);
            string[] fileEntries = Directory.GetFiles(pwd);
            var files = from f in fileEntries 
                        where f.EndsWith(".jpg") || f.EndsWith(".png")
                        select f;
            AsyncManager.Timeout = 10000;
            
            for (int j = 0; j < 1; j++)
            {
                foreach (var fileName in files)
                {
                    
                    AsyncManager.OutstandingOperations.Increment();
                    var task = Task.Factory.StartNew(() => loadImage((string)fileName));
                    
                }
            }
            UnicornsCompleted();
        }

        private void loadImage(string file)
        {
            // Render image...
            FileStream fileStream = new FileStream(file, FileMode.Open, FileAccess.Read);
            byte[] buffer = new byte[fileStream.Length];
            fileStream.Read(buffer, 0, (int)fileStream.Length);
            fileStream.Close();
            //Response.WriteFile(file);
            //Response.ContentType = "image/jpg";
            string base64string = Convert.ToBase64String(buffer);
            
            Response.Write("<img alt=\"\" src=\"data:image/jpeg;base64,"+@base64string+"\" />");
            Response.Flush();
            AsyncManager.OutstandingOperations.Decrement();
        }
    }

    public class MediaLoader
    {
        private string file;
        public MediaLoader(string filepath)
        {
            file = filepath;
        }
        
        public void LoadAsync()
        {
            // Load image file
        }
    }
}