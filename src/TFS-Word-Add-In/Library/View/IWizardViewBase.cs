//---------------------------------------------------------------------
// <copyright file="IWizardViewBase.cs" company="Microsoft">
//    Copyright (c) Microsoft. All rights reserved.
//    This code is licensed under the MIT License.
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF 
//    ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
//    TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
//    PARTICULAR PURPOSE AND NONINFRINGEMENT.
// </copyright>
// <summary>The IWizardViewBase type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.Word4Tfs.Library.View
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Defines the operations on a base wizard view.
    /// </summary>
    public interface IWizardViewBase : IViewBase
    {
        /// <summary>
        /// Raised when the user clicks the previous button.
        /// </summary>
        event EventHandler PreviousPage;

        /// <summary>
        /// Raised when the user clicks the next button.
        /// </summary>
        event EventHandler NextPage;

        /// <summary>
        /// Gets or sets the title of the current page.
        /// </summary>
        string Title { get; set; }

        /// <summary>
        /// Initializes the wizard as a whole.
        /// </summary>
        void Initialize();
 
        /// <summary>
        /// Adds a page to the list of pages managed by the wizard.
        /// </summary>
        /// <param name="pageView">The page to be added to the pages managed by the wizard.</param>
        void AddPage(IWizardPageView pageView);

        /// <summary>
        /// Starts showing the wizard dialog.
        /// </summary>
        /// <remarks>
        /// Events giving information about what has been selected are raised while in this call.
        /// </remarks>
        /// <returns><c>true</c> if the user clicked OK, <c>false</c> if the user cancelled the dialog.</returns>
        bool StartDialog();
    }
}
