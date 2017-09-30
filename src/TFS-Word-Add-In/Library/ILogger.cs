//---------------------------------------------------------------------
// <copyright file="ILogger.cs" company="Microsoft">
//    Copyright © ALM | DevOps Ranger Contributors. All rights reserved.
//    This code is licensed under the MIT License.
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF 
//    ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
//    TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
//    PARTICULAR PURPOSE AND NONINFRINGEMENT.
// </copyright>
// <summary>The ILogger type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.Word4Tfs.Library
{
    using System.Diagnostics;

    /// <summary>
    /// The logging operations.
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// Logs a message.
        /// </summary>
        /// <param name="eventType">The event type being logged.</param>
        /// <param name="format">The format of the message to be logged.</param>
        /// <param name="args">The arguments to be inserted in the format.</param>
        void Log(TraceEventType eventType, string format, params object[] args);
    }
}
