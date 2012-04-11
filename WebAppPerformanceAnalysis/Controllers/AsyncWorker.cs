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
    public class AsyncWorker : Component
    {
        public event LoadCompletedEventHandler LoadDataCompleted;

        private SendOrPostCallback onCompletedDelegate;

        public AsyncWorker()
        {
            InitializeDelegates();
        }

        protected virtual void InitializeDelegates()
        {
            onCompletedDelegate = new SendOrPostCallback(LoadCompleted);
        }

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

        public string LoadDBImageAsync(string query)
        {
            DatabaseWorker dw = new DatabaseWorker();
            
            byte[] contents = dw.binaryQueryDB(query);
            
            string base64string = Convert.ToBase64String(contents);
            LoadCompleted(new LoadCompletedEventArgs(base64string, null, false, null));
            return base64string;
        }

        public DataTable LoadDBTextAsync(string query)
        {
            DatabaseWorker dw = new DatabaseWorker();
            DataTable t = dw.queryDBRows(query);
            LoadCompleted(new LoadCompletedEventArgs(t, null, false, null));
            return t;
        }

        private void LoadCompleted(object operationState)
        {
            LoadCompletedEventArgs e = operationState as LoadCompletedEventArgs;

            OnLoadCompleted(e);
        }

        protected void OnLoadCompleted(LoadCompletedEventArgs e)
        {
            if (LoadDataCompleted != null)
            {
                LoadDataCompleted(this, e);
            }
        }

        private void CompletionMethod(string file, Exception exception, bool canceled, AsyncOperation asyncOp)
        {
            LoadCompletedEventArgs e = new LoadCompletedEventArgs(file, exception, canceled, asyncOp);

            asyncOp.PostOperationCompleted(onCompletedDelegate, e);
        }


    }
}