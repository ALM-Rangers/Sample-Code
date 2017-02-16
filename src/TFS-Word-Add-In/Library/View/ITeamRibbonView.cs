//---------------------------------------------------------------------
// <copyright file="ITeamRibbonView.cs" company="Microsoft">
//    Copyright (c) Microsoft. All rights reserved.
//    This code is licensed under the MIT License.
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF 
//    ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
//    TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
//    PARTICULAR PURPOSE AND NONINFRINGEMENT.
// </copyright>
// <summary>The ITeamRibbonView type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.Word4Tfs.Library.View
{
    using System;
    using System.Threading;

    /// <summary>
    /// Defines the functions of the Team ribbon view.
    /// </summary>
    public interface ITeamRibbonView : IViewBase
    {
        /// <summary>
        /// Raised when the user requests that work items be imported into the document from a team project.
        /// </summary>
        event EventHandler Import;

        /// <summary>
        /// Raised when the user requests the refresh of work items that have been previously imported into the document from a team project.
        /// </summary>
        event EventHandler Refresh;

        /// <summary>
        /// Raised when the user requests to see the layout designer.
        /// </summary>
        event EventHandler ShowLayoutDesigner;

        /// <summary>
        /// Raised when the user requests to hide the layout designer.
        /// </summary>
        event EventHandler HideLayoutDesigner;

        /// <summary>
        /// Raised when the user requests to see the bookmarks.
        /// </summary>
        event EventHandler ShowBookmarks;

        /// <summary>
        /// Raised when the user requests to hide the bookmarks.
        /// </summary>
        event EventHandler HideBookmarks;

        /// <summary>
        /// Sets the check state of a Team Ribbon toggle button
        /// </summary>
        /// <remarks>
        /// Changing the state of the button through this method does not raise an event.
        /// </remarks>
        /// <param name="button">The button whose check state is to be changed.</param>
        /// <param name="isChecked">The checked state to set.</param>
        void SetButtonCheckState(Enum button, bool isChecked);

        /// <summary>
        /// Displays a message about a cancellable operation and allows the user to cancel the operation.
        /// </summary>
        /// <param name="title">The title to be given to the message box.</param>
        /// <param name="message">The message to display.</param>
        /// <param name="cancellationTokenSource">Used to cancel the operation.</param>
        void StartCancellableOperation(string title, string message, CancellationTokenSource cancellationTokenSource);

        /// <summary>
        /// Updates the message about a cancellable operation.
        /// </summary>
        /// <param name="message">The message to display</param>
        void UpdateCancellableOperation(string message);

        /// <summary>
        /// Removes the dialogue about a cancellable operation.
        /// </summary>
        void EndCancellableOperation();
    }
}
