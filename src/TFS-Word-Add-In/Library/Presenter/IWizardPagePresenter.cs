//---------------------------------------------------------------------
// <copyright file="IWizardPagePresenter.cs" company="Microsoft">
//    Copyright (c) Microsoft. All rights reserved.
//    This code is licensed under the MIT License.
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF 
//    ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
//    TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
//    PARTICULAR PURPOSE AND NONINFRINGEMENT.
// </copyright>
// <summary>The IWizardPagePresenter type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.Word4Tfs.Library.Presenter
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Defines the operations on the presenter for an individual wizard page.
    /// </summary>
    public interface IWizardPagePresenter
    {
        /// <summary>
        /// Raised when the validity of the page has changed.
        /// </summary>
        event EventHandler ValidityChanged;

        /// <summary>
        /// Gets the title of the page.
        /// </summary>
        string Title { get; }

        /// <summary>
        /// Gets a value indicating whether the page is valid and navigation can proceed to the next page, or finish.
        /// </summary>
        /// <remarks>
        /// Must be callable immediately after construction and give a valid answer.
        /// </remarks>
        bool IsValid { get; }

        /// <summary>
        /// Initialises the wizard page presenter.
        /// </summary>
        /// <remarks>
        /// This is where the presenter adds the page view to the list of pages managed by the overall wizard view.
        /// </remarks>
        void Initialise();

        /// <summary>
        /// Makes the wizard page presenter instruct the view to start, for example by loading data.
        /// </summary>
        void Start();

        /// <summary>
        /// Activates the wizard page presenter so that the page is the active one.
        /// </summary>
        /// <remarks>This does not deactivate any other pages.</remarks>
        void Activate();

        /// <summary>
        /// Deactivates the wizard page presenter so that the page is no longer the active one.
        /// </summary>
        /// <remarks>This does not activate any other pages.</remarks>
        void Deactivate();

        /// <summary>
        /// Commits the changes made by the user.
        /// </summary>
        void Commit();

        /// <summary>
        /// Cancels the page.
        /// </summary>
        /// <remarks>
        /// Used particularly to cancel any asynchronous operations.
        /// </remarks>
        void Cancel();
    }
}
