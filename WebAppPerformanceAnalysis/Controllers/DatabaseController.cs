using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading;
using System.Threading.Tasks;
using System.ComponentModel;
using System.IO;

using System.Data.Sql;
using System.Data.SqlClient;

namespace WebAppPerformanceAnalysis.Controllers
{
    public class DatabaseController : AsyncController
    {
        private static SqlConnection conn;

        //
        // GET: /Database/
        public ActionResult Index()
        {
            ViewBag.Title = "Database Tests";
            return View();
        }

        public ActionResult QueryDatabase()
        {
            return View("RegularQueries");
        }

        public ActionResult DatabaseCompleted()
        {
            ViewBag.Title = "Async Database Tests";
            return View("Blank");
        }

        public void DatabaseAsync()
        {
            AsyncManager.Timeout = 10000;

            for (int j = 0; j < 5; j++)
            {
                
                //AsyncManager.OutstandingOperations.Increment();
                ////var task = Task.Factory.StartNew(() => loadImage((string)fileName));
                ////Task t = new Task(() => loadImage((string)fileName));
                ////t.Start();
                //MediaLoader l = new MediaLoader();
                //l.LoadImageCompleted += (sender, e) =>
                //{
                //    Response.Write("<img alt=\"\" src=\"data:image/jpeg;base64," + @e.ImageContents + "\" />");
                //    Response.Flush();
                //    AsyncManager.OutstandingOperations.Decrement();
                //};

                
            }

            DatabaseCompleted();
        }

        public static void connectDB()
        {
            conn = new SqlConnection();
            conn.ConnectionString =
             "Persist Security Info=False;Integrated Security=false;Connect Timeout=30";
            conn.Open();
        }

        private int queryDB(string query)
        {
            SqlCommand cmd = conn.CreateCommand();
            cmd.Connection = conn;
            try
            {
                cmd.CommandText = query;
                object result = cmd.ExecuteScalar();
                if (result != null)
                {
                    int id = Int32.Parse(result.ToString());
                    return id;
                }
                return 0;
            }
            catch (SqlException ex)
            {
                return 0;
            }
        }
    }
}
