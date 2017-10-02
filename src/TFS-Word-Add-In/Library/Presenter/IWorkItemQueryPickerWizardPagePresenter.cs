//---------------------------------------------------------------------
// <copyright file="IWorkItemQueryPickerWizardPagePresenter.cs" company="Microsoft">
//    Copyright © ALM | DevOps Ranger Contributors. All rights reserved.
//    This code is licensed under the MIT License.
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF 
//    ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
//    TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
//    PARTICULAR PURPOSE AND NONINFRINGEMENT.
// </copyright>
// <summary>The IWorkItemQueryPickerWizardPagePresenter type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.Word4Tfs.Library.Presenter
{
    using Microsoft.TeamFoundation.WorkItemTracking.Client;

    /// <summary>
    /// Defines the operations on the presenter for the work item query picker view.
    /// </summary>
    public interface IWorkItemQueryPickerWizardPagePresenter : IWizardPagePresenter
    {
        /// <summary>
        /// Gets the definition of the selected query. Only valid when <see cref="IsValid"/> is true.
        /// </summary>
        QueryDefinition SelectedQuery { get; }
    }
}
