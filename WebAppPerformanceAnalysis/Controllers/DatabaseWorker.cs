using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Data.SqlClient;
using System.IO;
using System.Collections;
using System.Data;

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
            conn.ConnectionString = @"Data Source=.\SQLEXPRESS;AttachDbFilename=" + dbfile + ";Integrated Security=True;User Instance=True;MultipleActiveResultSets=True";

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

        public DataTable queryDBRows(string query)
        {
            SqlCommand cmd = conn.CreateCommand();
            cmd.Connection = conn;
            try
            {
                cmd.CommandText = query;
                DataTable dt = new DataTable();
                SqlDataReader reader = cmd.ExecuteReader();
                int cols = reader.FieldCount;
                for (int i = 0; i < cols; i++) 
                {
                    dt.Columns.Add(new DataColumn(reader.GetName(i)));
                }

                while (reader.Read())
                {
                    DataRow row = dt.NewRow();
                    for (int i = 0; i < cols; i++)
                    {
                        row[reader.GetName(i)] = reader.GetValue(i);
                    }
                    dt.Rows.Add(row);
                    
                }
                return dt;
            }
            catch (SqlException ex)
            {
                return null;
            }
        }
    }
}