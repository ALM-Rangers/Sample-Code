//---------------------------------------------------------------------
// <copyright file="TimingsFile.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
//     THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
//     OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
//     LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR
//     FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
// <summary>The TimingsFile type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.WcfUnit.Library
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Text;

    /// <summary>
    /// This class is used to read a file of timings and return <see cref="TimedComment"/>s.
    /// </summary>
    public class TimingsFile : IDisposable
    {
        /// <summary>
        /// Indicates whether the instance has already been disposed.
        /// </summary>
        private bool disposed;

        /// <summary>
        /// Reader used to read the timings file.
        /// </summary>
        private StreamReader reader;

        /// <summary>
        /// Timestamp format.
        /// </summary>
        private string timestampFormat = @"yyyy-MM-dd\THH:mm:ss.ffffffK";

        /// <summary>
        /// Initialises a new instance of the <see cref="TimingsFile"/> class.
        /// </summary>
        /// <param name="file">The stream containing the timed comments data to be read.</param>
        public TimingsFile(Stream file)
        {
            this.reader = new StreamReader(file);
        }

        /// <summary>
        /// Finalises an instance of the <see cref="TimingsFile"/> class.
        /// </summary>
        ~TimingsFile()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            this.Dispose(false);
        }

        /// <summary>
        /// Disposes the object.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Gets the next timed comment.
        /// </summary>
        /// <returns>The next timed comment, or <c>null</c> if there are no more.</returns>
        public TimedComment NextTimedComment()
        {
            TimedComment ans = null;

            string line = this.reader.ReadLine();
            if (line != null)
            {
                DateTime timestamp = DateTime.ParseExact(line.Substring(0, 27), this.timestampFormat, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal);
                ans = new TimedComment(timestamp, line.Substring(27));
            }

            return ans;
        }

        /// <summary>
        /// Disposes the object
        /// </summary>
        /// <param name="disposing">Indicates whether the dispose was automatic or not.</param>
        private void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!this.disposed)
            {
                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if (disposing)
                {
                    this.reader.Dispose();
                }

                // Note disposing has been done.
                this.disposed = true;
            }
        }
    }
}
