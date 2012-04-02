using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Timers;
using System.Threading;
using System.Threading.Tasks;
using System.ComponentModel;
using System.IO;

namespace WebAppPerformanceAnalysis.Controllers
{
    public class AsyncMediaController : AsyncController
    {
        
        // GET: /Media/

        public ActionResult Index()
        {
            ViewBag.Title = "Async Media Tests";
            return View();
        }

        public ActionResult MediaCompleted()
        {
            ViewBag.Title = "Async Media Tests";
            return View("Blank");
        }

        public void MediaAsync()
        {
            System.Reflection.Assembly a = System.Reflection.Assembly.GetExecutingAssembly();
            DirectoryInfo d = Directory.GetParent(Directory.GetParent(a.CodeBase.ToString().Substring(8)).ToString());
            
            string[] fileEntries = Directory.GetFiles(d.ToString()+@"/Content", "*.jpg");
            var files = from f in fileEntries 
                        where f.EndsWith(".jpg") || f.EndsWith(".png")
                        select f;
            AsyncManager.Timeout = 10000;
            
            for (int j = 0; j < 5; j++)
            {
                foreach (var fileName in files)
                {
                    
                    AsyncManager.OutstandingOperations.Increment();
                    //var task = Task.Factory.StartNew(() => loadImage((string)fileName));
                    //Task t = new Task(() => loadImage((string)fileName));
                    //t.Start();
                    MediaLoader l = new MediaLoader();
                    l.LoadImageCompleted += (sender, e) =>
                    {
                        //Response.Write("<img alt=\"\" src=\"data:image/jpeg;base64," + @e.ImageContents + "\" />");
                        Response.Write("<img src=\"../.." + fileName.Substring(fileName.LastIndexOf("/")) + "\" alt=\"\" />");
                        Response.Flush();
                        AsyncManager.OutstandingOperations.Decrement();
                    };

                    l.LoadImageAsync((string)fileName);
                }
            }
            
            MediaCompleted();
        }

        
    }

    public delegate void LoadImageCompletedEventHandler(object sender, LoadImageCompletedEventArgs e);
    
    public class MediaLoader : Component
    {
        public event LoadImageCompletedEventHandler LoadImageCompleted;
        
        private SendOrPostCallback onCompletedDelegate;

        public MediaLoader()
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
            //FileStream fileStream = new FileStream(file, FileMode.Open, FileAccess.Read);
            //byte[] buffer = new byte[fileStream.Length];
            //fileStream.Read(buffer, 0, (int)fileStream.Length);
            //fileStream.Close();
            //string base64string = Convert.ToBase64String(buffer);
            LoadCompleted(new LoadImageCompletedEventArgs(null, null, false, null));
            return null; // base64string;
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
    

    public class LoadImageCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {
        private string imageContents;

        public LoadImageCompletedEventArgs(string imageContents, Exception e, bool canceled, object state)
            : base(e, canceled, state)
        {
            this.imageContents = imageContents;
        }

        public string ImageContents
        {
            get
            {
                RaiseExceptionIfNecessary();

                return imageContents;
            }
        }
    }


}