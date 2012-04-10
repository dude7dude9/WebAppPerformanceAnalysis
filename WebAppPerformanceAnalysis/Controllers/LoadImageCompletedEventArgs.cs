using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAppPerformanceAnalysis.Controllers
{
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