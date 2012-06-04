using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.IO;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using System.Data;

namespace WebAppPerformanceAnalysis.Controllers
{
    /// <summary>
    /// Instances of AsyncWorker carry out asynchronous methods and notify the event handler when the methods are completed.
    /// </summary>
    public class AsyncWorker : Component
    {
        public delegate void LoadCompletedEventHandler(object sender, LoadCompletedEventArgs e);

        public event LoadCompletedEventHandler LoadDataCompleted;

        private SendOrPostCallback onCompletedDelegate;

        public AsyncWorker()
        {
            InitializeDelegates();
        }

        /// <summary>
        /// Initialises the callback method to be executed after completing the asynchronous task.
        /// </summary>
        protected virtual void InitializeDelegates()
        {
            onCompletedDelegate = new SendOrPostCallback(LoadCompleted);
        }

        /// <summary>
        /// Asynchronously loads data from an image file.
        /// </summary>
        /// <param name="file">Relative path to file</param>
        /// <returns>String representation of the file contents</returns>
        public string LoadImageAsync(string file)
        {
            // Render image...
            FileStream fileStream = new FileStream(file, FileMode.Open, FileAccess.Read);
            byte[] buffer = new byte[fileStream.Length];
            fileStream.Read(buffer, 0, (int)fileStream.Length);
            fileStream.Close();
            string base64string = Convert.ToBase64String(buffer);
            LoadCompleted(new LoadCompletedEventArgs(base64string, null, false, null));
            return base64string;
        }

        /// <summary>
        /// Asynchronously loads binary data of an image from the database using the DatabaseWorker.
        /// </summary>
        /// <param name="query">SQL query to be sent to the database</param>
        /// <returns>String representation of the binary data</returns>
        public string LoadDBImageAsync(string query)
        {
            DatabaseWorker dw = new DatabaseWorker();
            
            byte[] contents = dw.binaryQueryDB(query);
            
            string base64string = Convert.ToBase64String(contents);
            LoadCompleted(new LoadCompletedEventArgs(base64string, null, false, null));
            return base64string;
        }

        /// <summary>
        /// Asynchronously loads textual data from database.
        /// </summary>
        /// <param name="query">SQL query to the database</param>
        /// <returns>Table of results returned from the database</returns>
        public DataTable LoadDBTextAsync(string query)
        {
            DatabaseWorker dw = new DatabaseWorker();
            DataTable t = dw.queryDBRows(query);
            LoadCompleted(new LoadCompletedEventArgs(t, null, false, null));
            return t;
        }

        /// <summary>
        /// Method called whenever an asynchronous task has been completed, returning the data from 
        /// the asynchronous method to the listener(s).
        /// </summary>
        /// <param name="operationState">The operation state (containing data returned from the asynchronous method)</param>
        private void LoadCompleted(object operationState)
        {
            LoadCompletedEventArgs e = operationState as LoadCompletedEventArgs;

            OnLoadCompleted(e);
        }

        /// <summary>
        /// Called with specific instances of an event after an asynchronous task is completed.
        /// </summary>
        /// <param name="e">Event arguments</param>
        protected void OnLoadCompleted(LoadCompletedEventArgs e)
        {
            if (LoadDataCompleted != null)
            {
                LoadDataCompleted(this, e);
            }
        }

    }
}