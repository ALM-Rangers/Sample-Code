//---------------------------------------------------------------------
// <copyright file="UserMessageEventArgs.cs" company="Microsoft">
//    Copyright © ALM | DevOps Ranger Contributors. All rights reserved.
//    This code is licensed under the MIT License.
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF 
//    ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
//    TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
//    PARTICULAR PURPOSE AND NONINFRINGEMENT.
// </copyright>
// <summary>The UserMessageEventArgs type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.Word4Tfs.Library.Model
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Event arguments used to pass a message from the model to the UI for display by the UI.
    /// </summary>
    public class UserMessageEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserMessageEventArgs"/> class.
        /// </summary>
        /// <param name="message">The message to be displayed.</param>
        /// <param name="messageType">The type of message to be displayed.</param>
        public UserMessageEventArgs(string message, UIMessageType messageType)
        {
            this.Message = message;
            this.MessageType = messageType;
        }

        /// <summary>
        /// Gets the message to be displayed.
        /// </summary>
        public string Message { get; private set; }

        /// <summary>
        /// Gets the type of message to be displayed.
        /// </summary>
        public UIMessageType MessageType { get; private set; }
    }
}
