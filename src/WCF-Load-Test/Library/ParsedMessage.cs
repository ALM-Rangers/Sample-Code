//---------------------------------------------------------------------
// <copyright file="ParsedMessage.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
//     THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
//     OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
//     LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR
//     FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
// <summary>The ParsedMessage type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.WcfUnit.Library
{
    using System;
    using System.ServiceModel.Channels;

    /// <summary>
    /// Contains information about a parsed message
    /// </summary>
    public class ParsedMessage
    {
        /// <summary>
        /// The parsed message.
        /// </summary>
        private readonly MessageBuffer message;

        /// <summary>
        /// The SOAP action in the parsed message.
        /// </summary>
        private readonly string soapAction;

        /// <summary>
        /// The timestamp when the message was logged.
        /// </summary>
        private readonly DateTime timestamp;

        /// <summary>
        /// Initialises a new instance of the <see cref="ParsedMessage"/> class.
        /// </summary>
        /// <param name="message">The parsed message.</param>
        /// <param name="soapAction">The SOAP action in the parsed message.</param>
        /// <param name="timestamp">The timestamp when the message was logged.</param>
        public ParsedMessage(MessageBuffer message, string soapAction, DateTime timestamp)
        {
            this.message = message;
            this.soapAction = soapAction;
            this.timestamp = timestamp;
        }

        /// <summary>
        /// Gets the parsed message.
        /// </summary>
        /// <value>The parsed message.</value>
        public MessageBuffer Message
        {
            get { return this.message; }
        }

        /// <summary>
        /// Gets the SOAP action contained in the parsed message.
        /// </summary>
        /// <value>The SOAP action contained in the parsed message.</value>
        public string SoapAction
        {
            get { return this.soapAction; }
        }

        /// <summary>
        /// Gets the time the parsed message was logged.
        /// </summary>
        /// <value>The time the parsed message was logged.</value>
        public DateTime Timestamp
        {
            get { return this.timestamp; }
        }
    }
}
