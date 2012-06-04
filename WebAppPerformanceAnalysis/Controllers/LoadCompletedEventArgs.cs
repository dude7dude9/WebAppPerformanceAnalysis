using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

namespace WebAppPerformanceAnalysis.Controllers
{
    /// <summary>
    /// Represents the event arguments to be returned to the event listener.
    /// The event listener is only interested in the data returned from the asynchronous methods in this case.
    /// </summary>
    public class LoadCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {
        #region Asynchronous method return variables
        private string imageContents;
        private DataTable table;
        #endregion

        /// <summary>
        /// Constructor for asynchronous media methods.
        /// </summary>
        /// <param name="imageContents">Image data</param>
        /// <param name="e"></param>
        /// <param name="canceled"></param>
        /// <param name="state"></param>
        public LoadCompletedEventArgs(string imageContents, Exception e, bool canceled, object state)
            : base(e, canceled, state)
        {
            this.imageContents = imageContents;
        }

        /// <summary>
        /// Constructor for asynchronous database methods.
        /// </summary>
        /// <param name="table">Textual data returned from database</param>
        /// <param name="e"></param>
        /// <param name="canceled"></param>
        /// <param name="state"></param>
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