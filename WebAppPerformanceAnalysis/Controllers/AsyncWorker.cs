using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.IO;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;

namespace WebAppPerformanceAnalysis.Controllers
{
    public class AsyncWorker : Component
    {
        public event LoadImageCompletedEventHandler LoadImageCompleted;

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
            LoadCompleted(new LoadImageCompletedEventArgs(base64string, null, false, null));
            return base64string;
        }

        public string LoadDBImageAsync(string query)
        {
            DatabaseWorker dw = new DatabaseWorker();
            
            byte[] contents = dw.binaryQueryDB(query);
            
            string base64string = Convert.ToBase64String(contents);
            LoadCompleted(new LoadImageCompletedEventArgs(base64string, null, false, null));
            return base64string;
        }

        private void LoadCompleted(object operationState)
        {
            LoadImageCompletedEventArgs e = operationState as LoadImageCompletedEventArgs;

            OnLoadImageCompleted(e);
        }

        protected void OnLoadImageCompleted(LoadImageCompletedEventArgs e)
        {
            if (LoadImageCompleted != null)
            {
                LoadImageCompleted(this, e);
            }
        }

        private void CompletionMethod(string file, Exception exception, bool canceled, AsyncOperation asyncOp)
        {
            LoadImageCompletedEventArgs e = new LoadImageCompletedEventArgs(file, exception, canceled, asyncOp);

            asyncOp.PostOperationCompleted(onCompletedDelegate, e);
        }


    }
}