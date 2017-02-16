//---------------------------------------------------------------------
// <copyright file="TimedComment.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
//     THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
//     OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
//     LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR
//     FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
// <summary>The TimedComment type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.WcfUnit.Library
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Represents a comment with a particular timestamp.
    /// </summary>
    public class TimedComment
    {
        /// <summary>
        /// The time of the comment.
        /// </summary>
        private DateTime timeStamp;

        /// <summary>
        /// The comment.
        /// </summary>
        private string comment;

        /// <summary>
        /// Initialises a new instance of the <see cref="TimedComment"/> class.
        /// </summary>
        /// <param name="timestamp">The time of the comment.</param>
        /// <param name="comment">The comment itself.</param>
        public TimedComment(DateTime timestamp, string comment)
        {
            this.timeStamp = timestamp;
            this.comment = comment;
        }

        /// <summary>
        /// Gets the time at which the comment was made.
        /// </summary>
        /// <value>The time at which the comment was made.</value>
        public DateTime Timestamp
        {
            get
            {
                return this.timeStamp;
            }
        }

        /// <summary>
        /// Gets the actual comment.
        /// </summary>
        /// <value>The actual comment.</value>
        public string Comment
        {
            get
            {
                return this.comment;
            }
        }
    }
}
