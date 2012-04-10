using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Data.SqlClient;
using System.IO;

namespace WebAppPerformanceAnalysis.Controllers
{
    public class DatabaseWorker
    {
        private static SqlConnection conn;

        public static void connectDB()
        {
            conn = new SqlConnection();
            System.Reflection.Assembly a = System.Reflection.Assembly.GetExecutingAssembly();
            DirectoryInfo d = Directory.GetParent(Directory.GetParent(a.CodeBase.ToString().Substring(8)).ToString());
            String dbfile = d.ToString() + @"\App_Data\ASPNETDB.MDF";
            conn.ConnectionString = @"Data Source=.\SQLEXPRESS;AttachDbFilename=" + dbfile + ";Integrated Security=True;User Instance=True";

            conn.Open();
        }

        public static void disconnectDB()
        {
            conn.Close();
        }

        public int intQueryDB(string query)
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

        public byte[] binaryQueryDB(string query)
        {
            SqlCommand cmd = conn.CreateCommand();
            cmd.Connection = conn;
            try
            {
                cmd.CommandText = query;
                object result = cmd.ExecuteScalar();
                if (result != null)
                {
                    byte[] id = (byte[])result;
                    return id;
                }
                return null;
            }
            catch (SqlException ex)
            {
                return null;
            }
        }
    }
}