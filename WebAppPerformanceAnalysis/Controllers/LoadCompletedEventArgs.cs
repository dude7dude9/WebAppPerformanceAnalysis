using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

namespace WebAppPerformanceAnalysis.Controllers
{
    public class LoadCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {
        private string imageContents;
        private DataTable table;

        public LoadCompletedEventArgs(string imageContents, Exception e, bool canceled, object state)
            : base(e, canceled, state)
        {
            this.imageContents = imageContents;
        }

        public LoadCompletedEventArgs(DataTable table, Exception e, bool canceled, object state)
            : base(e, canceled, state)
        {
            this.table = table;
        }

        public DataTable Data
        {
            get
            {
                RaiseExceptionIfNecessary();

                return table;
            }
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