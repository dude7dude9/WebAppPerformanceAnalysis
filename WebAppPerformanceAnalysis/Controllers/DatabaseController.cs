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
using System.Data;

namespace WebAppPerformanceAnalysis.Controllers
{
    /// <summary>
    /// Handles both asynchronous and synchronous requests from the client.
    /// </summary>
    public class DatabaseController : AsyncController
    {

        public ActionResult Index()
        {
            ViewBag.Title = "Database Tests";
            return View();
        }

        /// <summary>
        /// Loads images from database synchronously.
        /// </summary>
        /// <returns>View of images</returns>
        public ActionResult GetImagesSynchronous()
        {
            DatabaseWorker dw = new DatabaseWorker();
            DatabaseWorker.connectDB();
            ViewBag.Images = new ArrayList();
            ViewBag.Tables = new ArrayList();
            for (int j = 0; j < 10; j++)
            {
                int count = dw.intQueryDB("select count(*) from Images");

                for (int i = 1; i <= count; i++)
                {
                    byte[] contents = dw.binaryQueryDB("select image from Images where imageID = " + i + " and exists (select * from ArticleAttachments where ImageID = " + i + ")");

                    // Place image contents in an array of images in viewbag
                    ViewBag.Images.Add("data:image/jpeg;base64," + Convert.ToBase64String(contents));
                    
                }
            }
            DatabaseWorker.disconnectDB();
            ViewBag.Title = "Synchronous Test Loading Media From Database";
            return View("RegularQueries");
        }

        /// <summary>
        /// Loads images from database synchronously.
        /// </summary>
        /// <returns>View of textual tables</returns>
        public ActionResult GetTextSynchronous()
        {
            DatabaseWorker dw = new DatabaseWorker();
            DatabaseWorker.connectDB();
            ViewBag.Tables = new ArrayList();
            ViewBag.Images = new ArrayList();
            String[] tableNames = { "Technology", "Music", "Nature", "Health", "Sports", "Business" };
            for (int j = 0; j < tableNames.Length; j++)
            {
                AsyncWorker aw = new AsyncWorker();
                DataTable table = aw.LoadDBTextAsync("select DocumentTitle as Title, Username, FirstName, LastName " +
                    "from Articles, Groups, SiteUsers where Articles.GroupID=Groups.GroupID and SiteUsers.UserID=Articles.UserID "
                    + "and GroupName = '" + tableNames[j] + "' order by FirstName, LastName ASC");
                table.TableName = tableNames[j] + " Articles:";
                ViewBag.Tables.Add(table);
                
            }
            DatabaseWorker.disconnectDB();
            ViewBag.Title = "Synchronous Test Loading Text From Database";
            return View("RegularQueries");
        }

        /// <summary>
        /// Callback method for GetImagesAsync().
        /// </summary>
        /// <returns>View of images</returns>
        public ActionResult GetImagesCompleted()
        {
            ViewBag.Title = "Asynchronous Test Loading Media From Database";
            return View("RegularQueries");
        }

        /// <summary>
        /// Loads images from database using asynchronous request processing.
        /// </summary>
        public void GetImagesAsync()
        {
            AsyncManager.Timeout = 10000;
            
            ViewBag.Images = new ArrayList();
            ViewBag.Tables = new ArrayList();
            DatabaseWorker.connectDB();
            DatabaseWorker dw = new DatabaseWorker();
            for (int j = 0; j < 10; j++)
            {
                int count = dw.intQueryDB("select count(*) from Images");
                
                for (int i = 1; i <= count; i++)
                {
                    AsyncManager.OutstandingOperations.Increment();
                    AsyncWorker l = new AsyncWorker();
                    // Register the event handler to be notified when the async operation finishes
                    l.LoadDataCompleted += (sender, e) =>
                    {
                        // Place image contents in an array of images in viewbag
                        ViewBag.Images.Add("data:image/jpeg;base64," + @e.ImageContents);

                        AsyncManager.OutstandingOperations.Decrement();
                    };

                    // Execute async method (automatically on separate thread)
                    l.LoadDBImageAsync("select image from Images where imageID = " + i + " and exists (select * from ArticleAttachments where ImageID = " + i + ")");
                                        
                }
            }
            DatabaseWorker.disconnectDB();
        }

        /// <summary>
        /// Callback method for GetTextAsync().
        /// </summary>
        /// <returns>View of textual data in tabular form</returns>
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

                l.LoadDBTextAsync("select DocumentTitle as Title, Username, FirstName, LastName from Articles, Groups, SiteUsers " +
                    "where Articles.GroupID=Groups.GroupID and SiteUsers.UserID=Articles.UserID and GroupName = '"+tableNames[j]+"'" + 
                    " order by FirstName, LastName ASC");
            }
            DatabaseWorker.disconnectDB();
        }

        /// <summary>
        /// Processes request for media from database using caching.
        /// </summary>
        /// <returns>View of media from database</returns>
        [OutputCache(Duration = 60, VaryByParam = "none")]
        public ActionResult GetMediaCache()
        {
            DatabaseWorker dw = new DatabaseWorker();
            DatabaseWorker.connectDB();
            ViewBag.Tables = new ArrayList();
            ViewBag.Images = new ArrayList();
            for (int j = 0; j < 10; j++)
            {
                int count = dw.intQueryDB("select count(*) from Images");

                for (int i = 1; i <= count; i++)
                {
                    byte[] contents;
                    object cached = HttpRuntime.Cache.Get("DB.Images." + i);
                    if (cached == null)
                    {
                        contents = dw.binaryQueryDB("select image from Images where imageID = " + i + " and exists (select * from ArticleAttachments where ImageID = " + i + ")");
                        HttpRuntime.Cache.Insert("DB.Images." + i, contents, null, System.Web.Caching.Cache.NoAbsoluteExpiration, new TimeSpan(0, 1, 0));
                    }
                    else
                    {
                        contents = (byte[])cached;
                    }

                    ViewBag.Images.Add("data:image/jpeg;base64," + Convert.ToBase64String(contents));

                }
            }
                                
            DatabaseWorker.disconnectDB();
            ViewBag.Title = "Caching Test Loading Media From Database";
            return View("RegularQueries");
        }

        /// <summary>
        /// Processes request for text from database using caching.
        /// </summary>
        /// <returns>View of textual data in tabular form</returns>
        [OutputCache(Duration = 60, VaryByParam = "none")]
        public ActionResult GetTextCache()
        {
            DatabaseWorker dw = new DatabaseWorker();
            DatabaseWorker.connectDB();
            ViewBag.Tables = new ArrayList();
            ViewBag.Images = new ArrayList();
            String[] tableNames = { "Technology", "Music", "Nature", "Health", "Sports", "Business" };
            for (int j = 0; j < tableNames.Length; j++)
            {
                AsyncWorker aw = new AsyncWorker();
                DataTable table;
                object t = HttpRuntime.Cache.Get(tableNames[j]);
                if (t == null)
                {
                    table = aw.LoadDBTextAsync("select DocumentTitle as Title, Username, FirstName, LastName from Articles, Groups, SiteUsers " +
                    "where Articles.GroupID=Groups.GroupID and SiteUsers.UserID=Articles.UserID and GroupName = '"+tableNames[j]+"'" +
                    " order by FirstName, LastName ASC");
                    table.TableName = tableNames[j] + " Articles:";
                    HttpRuntime.Cache.Insert(tableNames[j], table, null, System.Web.Caching.Cache.NoAbsoluteExpiration, new TimeSpan(0, 1, 0));
                }
                else
                {
                    table = (DataTable)t;
                }
                ViewBag.Tables.Add(table);

            }
            DatabaseWorker.disconnectDB();
            ViewBag.Title = "Caching Test Loading Text From Database";
            return View("RegularQueries");
        }
    }
}
