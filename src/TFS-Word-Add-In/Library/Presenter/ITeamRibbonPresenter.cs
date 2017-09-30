//---------------------------------------------------------------------
// <copyright file="ITeamRibbonPresenter.cs" company="Microsoft">
//    Copyright © ALM | DevOps Ranger Contributors. All rights reserved.
//    This code is licensed under the MIT License.
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF 
//    ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
//    TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
//    PARTICULAR PURPOSE AND NONINFRINGEMENT.
// </copyright>
// <summary>The ITeamRibbonPresenter type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.Word4Tfs.Library.Presenter
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Word4Tfs.Library.View;

    /// <summary>
    /// The operations of a team ribbon presenter.
    /// </summary>
    public interface ITeamRibbonPresenter
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
        /// Gets or sets the layout designer view.
        /// </summary>
        /// <remarks>
        /// This view is just used to keep the toggle state of the show/hide layout designer button up to date. When the property is <c>null</c> there is no view.
        /// </remarks>
        ILayoutDesignerView LayoutDesignerView { get; set; }

        /// <summary>
        /// Initialises the presenter.
        /// </summary>
        /// <param name="rebindCallback">The callback to be called when a document rebind is required. The callback must return null to cancel the rebind.</param>
        void Initialise(Func<Uri> rebindCallback);

        /// <summary>
        /// Displays an error message.
        /// </summary>
        /// <param name="message">The message to be displayed.</param>
        /// <param name="details">Optional additional details about the error.</param>
        void DisplayError(string message, string details);

        /// <summary>
        /// Displays a question to the user which has only a Yes or No answer.
        /// </summary>
        /// <param name="message">The text of the question.</param>
        /// <returns><c>True</c> if the user responds with a Yes, <c>false</c> otherwise.</returns>
        bool AskYesNoQuestion(string message);

        /// <summary>
        /// Loads the document state and initialises the ribbon according to the state.
        /// </summary>
        void LoadState();

        /// <summary>
        /// Updates the ribbon based on the current state.
        /// </summary>
        void UpdateState();

        /// <summary>
        /// Advises the user that a long cancellable operation has been started and allows the user to cancel it.
        /// </summary>
        /// <param name="message">A message to tell the user what is happening.</param>
        /// <param name="cancellationTokenSource">Used to cancel the operation.</param>
        void StartCancellableOperation(string message, CancellationTokenSource cancellationTokenSource);

        /// <summary>
        /// Updates the message associated with the long cancellable operation.
        /// </summary>
        /// <param name="message">The new message to be displayed.</param>
        void UpdateCancellableOperation(string message);

        /// <summary>
        /// Removes the advice to the user about a long cancellable operation.
        /// </summary>
        void EndCancellableOperation();
    }
}
