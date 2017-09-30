//---------------------------------------------------------------------
// <copyright file="IViewBase.cs" company="Microsoft">
//    Copyright © ALM | DevOps Ranger Contributors. All rights reserved.
//    This code is licensed under the MIT License.
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF 
//    ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
//    TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
//    PARTICULAR PURPOSE AND NONINFRINGEMENT.
// </copyright>
// <summary>The IViewBase type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.Word4Tfs.Library.View
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Base interface common to all views.
    /// </summary>
    public interface IViewBase
    {
        /// <summary>
        /// Sets the button to be enabled or not
        /// </summary>
        /// <param name="button">The button for which the status is to be set.</param>
        /// <param name="enabled">The enabled/disabled status to set.</param>
        void SetButtonState(Enum button, bool enabled);

        /// <summary>
        /// Displays a message.
        /// </summary>
        /// <param name="messageType">The type of message to display.</param>
        /// <param name="title">The title to be given to the message box.</param>
        /// <param name="message">The message to display.</param>
        /// <param name="details">Optional details to display.</param>
        void DisplayMessage(UIMessageType messageType, string title, string message, string details);
    }
}
