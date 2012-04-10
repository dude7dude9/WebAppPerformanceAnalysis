using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading;
using System.Threading.Tasks;
using System.ComponentModel;
using System.IO;
using System.Collections;

using System.Data.Sql;
using System.Data.SqlClient;

namespace WebAppPerformanceAnalysis.Controllers
{
    public class DatabaseController : AsyncController
    {

        //
        // GET: /Database/
        public ActionResult Index()
        {
            ViewBag.Title = "Database Tests";
            return View();
        }

        public ActionResult QueryDatabase()
        {
            DatabaseWorker dw = new DatabaseWorker();
            DatabaseWorker.connectDB();
            ViewBag.Images = new ArrayList();
            for (int j = 0; j < 5; j++)
            {
                int count = dw.intQueryDB("select count(*) from Images");

                for (int i = 1; i <= count; i++)
                {
                    byte[] contents = dw.binaryQueryDB("select image from Images where imageID = " + i);

                    // Place image contents in an array of images in viewbag
                    ViewBag.Images.Add("data:image/jpeg;base64," + Convert.ToBase64String(contents));
                    
                }
            }
            DatabaseWorker.disconnectDB();
            ViewBag.Title = "Synchronous Tests Loading Media From Database";
            return View("Blank");
        }

        public ActionResult DatabaseCompleted()
        {
            ViewBag.Title = "Asynchronous Tests Loading Media From Database";
            return View("Blank");
        }

        public void DatabaseAsync()
        {
            AsyncManager.Timeout = 10000;
            
            ViewBag.Images = new ArrayList();
            DatabaseWorker.connectDB();
            DatabaseWorker dw = new DatabaseWorker();
            for (int j = 0; j < 5; j++)
            {
                int count = dw.intQueryDB("select count(*) from Images");
                
                for (int i = 1; i <= count; i++)
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

                    l.LoadDBImageAsync("select image from Images where imageID = " + i);
                                        
                }
            }
            DatabaseWorker.disconnectDB();
        }

        

        
    }
}
