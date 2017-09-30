//---------------------------------------------------------------------
// <copyright file="Logger.cs" company="Microsoft">
//    Copyright © ALM | DevOps Ranger Contributors. All rights reserved.
//    This code is licensed under the MIT License.
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF 
//    ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
//    TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
//    PARTICULAR PURPOSE AND NONINFRINGEMENT.
// </copyright>
// <summary>The Logger type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.Word4Tfs.Library
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.Text;

    /// <summary>
    /// Provides logging.
    /// </summary>
    public class Logger : ILogger
    {
        /// <summary>
        /// Used to synchronise logging so that background thread logging does not get mixed up with main thread logging.
        /// </summary>
        private static object synch = new object();

        #region ILog Members

        /// <summary>
        /// Logs a message.
        /// </summary>
        /// <param name="eventType">The event type being logged.</param>
        /// <param name="format">The format of the message to be logged.</param>
        /// <param name="args">The arguments to be inserted in the format.</param>
        public void Log(TraceEventType eventType, string format, params object[] args)
        {
            lock (synch)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("WORD4TFS ");
                sb.Append(DateTime.Now.ToString("HH:mm:ss.fff", CultureInfo.InvariantCulture));
                sb.Append(" ");
                sb.AppendFormat(CultureInfo.InvariantCulture, format, args);
                Trace.WriteLine(sb.ToString());
            }
        }

        #endregion
    }
}
