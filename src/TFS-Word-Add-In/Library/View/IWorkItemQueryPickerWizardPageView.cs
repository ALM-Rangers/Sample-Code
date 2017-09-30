//---------------------------------------------------------------------
// <copyright file="IWorkItemQueryPickerWizardPageView.cs" company="Microsoft">
//    Copyright © ALM | DevOps Ranger Contributors. All rights reserved.
//    This code is licensed under the MIT License.
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF 
//    ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
//    TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
//    PARTICULAR PURPOSE AND NONINFRINGEMENT.
// </copyright>
// <summary>The IWorkItemQueryPickerWizardPageView type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.Word4Tfs.Library.View
{
    using System;
    using Microsoft.TeamFoundation.WorkItemTracking.Client;

    /// <summary>
    /// Defines the operations on the view for the wizard page that allows the user to select a work item query.
    /// </summary>
    public interface IWorkItemQueryPickerWizardPageView : IWizardPageView
    {
        /// <summary>
        /// Raised when the user selects a query.
        /// </summary>
        event EventHandler<QueryItemEventArgs> QuerySelected;

        /// <summary>
        /// Gets the query that has been selected.
        /// </summary>
        /// <remarks>
        /// Only valid after <see cref="StartDialog"/> has returned <c>true</c>.
        /// </remarks>
        QueryDefinition SelectedQuery { get; }

        /// <summary>
        /// Loads the query hierarchy to display to the user.
        /// </summary>
        /// <param name="rootQueryFolder">The query hierarchy to be displayed.</param>
        void LoadQueryHierarchy(QueryFolder rootQueryFolder);

        /// <summary>
        /// Displays a warning message.
        /// </summary>
        /// <param name="message">The warning message to display.</param>
        void DisplayWarning(string message);

        /// <summary>
        /// Cancels the view.
        /// </summary>
        /// <remarks>
        /// This view uses asynchronous code and so this must be cancelled if the user cancels the wizard.
        /// </remarks>
        void Cancel();
    }
}
