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
    /// <summary>
    /// Used for communication with the SQL Server Express database - ASPNETDB.MDF must exist in the App_Data directory of the project.
    /// </summary>
    public class DatabaseWorker
    {
        private static SqlConnection conn;

        public static void connectDB()
        {
            conn = new SqlConnection();
            conn.ConnectionString = @"Data Source=.\SQLEXPRESS;AttachDbFilename=|DataDirectory|\ASPNETDB.MDF;Integrated Security=True;User Instance=True;MultipleActiveResultSets=True";
            conn.Open();
        }

        public static void disconnectDB()
        {
            conn.Close();
        }

        /// <summary>
        /// Queries the database for numerical data.
        /// </summary>
        /// <param name="query">SQL query</param>
        /// <returns>Integer value</returns>
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
                return -1;
            }
            catch (SqlException ex)
            {
                return -1;
            }
        }

        /// <summary>
        /// Queries the database for binary data.
        /// </summary>
        /// <param name="query">SQL query</param>
        /// <returns>Binary data</returns>
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

        /// <summary>
        /// Queries the database for multiple rows/columns of data.
        /// </summary>
        /// <param name="query">SQL query</param>
        /// <returns>Rows of data</returns>
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