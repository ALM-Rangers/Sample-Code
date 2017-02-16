//---------------------------------------------------------------------
// <copyright file="IWorkItemQueryAndLayoutPickerWizardPresenter.cs" company="Microsoft">
//    Copyright (c) Microsoft. All rights reserved.
//    This code is licensed under the MIT License.
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF 
//    ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
//    TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
//    PARTICULAR PURPOSE AND NONINFRINGEMENT.
// </copyright>
// <summary>The IWorkItemQueryAndLayoutPickerWizardPresenter type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.Word4Tfs.Library.Presenter
{
    using Microsoft.Word4Tfs.Library.Data;
    using Microsoft.Word4Tfs.Library.Model;

    /// <summary>
    /// Defines the operations on the presenter for the wizard that allows the user to choose the work item query and a layout.
    /// </summary>
    public interface IWorkItemQueryAndLayoutPickerWizardPresenter : IWizardPresenterBase
    {
        /// <summary>
        /// Gets the definition of the query and the associated layout to go into the document.
        /// </summary>
        QueryAndLayoutInformation QueryAndLayout { get; }

        /// <summary>
        /// Saves the query and layout in the document.
        /// </summary>
        void SaveQueryAndLayout();
    }
}
