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

        /// <summary>
        /// Loads images from database using synchronous method calling.
        /// </summary>
        /// <returns></returns>
        public ActionResult GetImagesSynchronous()
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
            ViewBag.Title = "Synchronous Test Loading Media From Database";
            return View("Media");
        }

        public ActionResult GetImagesCompleted()
        {
            ViewBag.Title = "Asynchronous Test Loading Media From Database";
            return View("Media");
        }

        /// <summary>
        /// Loads images from database using asynchronous method calling.
        /// </summary>
        public void GetImagesAsync()
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
                    l.LoadDataCompleted += (sender, e) =>
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


        public ActionResult GetTextCompleted()
        {
            ViewBag.Title = "Asynchronous Test Loading Text From Database";
            return View("RegularQueries");
        }

        /// <summary>
        /// Loads images from database using asynchronous method calling.
        /// </summary>
        public void GetTextAsync()
        {
            AsyncManager.Timeout = 10000;

            ViewBag.Images = new ArrayList();
            DatabaseWorker.connectDB();
            DatabaseWorker dw = new DatabaseWorker();
            ViewBag.Tables = new ArrayList();
            String[] tableNames = { "Technology", "Music", "Nature", "Health", "Sports", "Business" };
            for (int j = 0; j < tableNames.Length; j++)
            {
                AsyncManager.OutstandingOperations.Increment();
                AsyncWorker l = new AsyncWorker();
                l.LoadDataCompleted += (sender, e) =>
                {
                    e.Data.TableName = tableNames[j] +" Articles:";
                    ViewBag.Tables.Add(e.Data);
                    AsyncManager.OutstandingOperations.Decrement();
                };

                l.LoadDBTextAsync("select DocumentTitle as Title, Username, FirstName, LastName from Articles, Groups, SiteUsers where Articles.GroupID=Groups.GroupID and SiteUsers.UserID=Articles.UserID and GroupName = '"+tableNames[j]+"'");
            }
            DatabaseWorker.disconnectDB();
        }

        
    }
}
