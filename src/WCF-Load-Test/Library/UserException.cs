//---------------------------------------------------------------------
// <copyright file="UserException.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
//     THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
//     OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
//     LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR
//     FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
// <summary>The UserException type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.WcfUnit.Library
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// An exception used to convey user errors.
    /// </summary>
    [Serializable]
    public class UserException : Exception
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="UserException"/> class.
        /// </summary>
        public UserException()
            : base(Messages.UnknownError)
        {
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="UserException"/> class.
        /// </summary>
        /// <param name="message">The error message.</param>
        public UserException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="UserException"/> class.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="innerException">The inner exception.</param>
        public UserException(string message, Exception innerException) :
            base(message, innerException)
        {
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="UserException"/> class.
        /// </summary>
        /// <param name="info">The serialization information.</param>
        /// <param name="context">The streaming context.</param>
        protected UserException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
