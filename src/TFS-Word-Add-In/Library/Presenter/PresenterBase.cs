//---------------------------------------------------------------------
// <copyright file="PresenterBase.cs" company="Microsoft">
//    Copyright © ALM | DevOps Ranger Contributors. All rights reserved.
//    This code is licensed under the MIT License.
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF 
//    ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
//    TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
//    PARTICULAR PURPOSE AND NONINFRINGEMENT.
// </copyright>
// <summary>The PresenterBase type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.Word4Tfs.Library.Presenter
{
    using System;
    using Microsoft.Word4Tfs.Library.View;

    /// <summary>
    /// The base class for presenters. Provides some common functionality.
    /// </summary>
    public abstract class PresenterBase
    {
        /// <summary>
        /// Gets the title to be used when displaying errors.
        /// </summary>
        protected abstract string ErrorTitle { get; }

        /// <summary>
        /// Displays a message.
        /// </summary>
        /// <param name="messageType">The type of message to display.</param>
        /// <param name="view">The view to be used to display the error.</param>
        /// <param name="message">The message to display.</param>
        /// <param name="details">Optional extra details to display.</param>
        protected void DisplayError(UIMessageType messageType, IViewBase view, string message, string details)
        {
            if (view == null)
            {
                throw new ArgumentNullException("view");
            }

            view.DisplayMessage(messageType, this.ErrorTitle, message, details);
        }

        /// <summary>
        /// Displays an error message due to an exception.
        /// </summary>
        /// <param name="view">The view to be used to display the error.</param>
        /// <param name="ex">The exception to display.</param>
        protected void DisplayError(IViewBase view, Exception ex)
        {
            if (ex == null)
            {
                throw new ArgumentNullException("ex");
            }

            this.DisplayError(UIMessageType.Error, view, ex.Message, Utilities.FormatException(ex));
        }
    }
}
