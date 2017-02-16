//---------------------------------------------------------------------
// <copyright file="ILayoutPickerWizardPagePresenter.cs" company="Microsoft">
//    Copyright (c) Microsoft. All rights reserved.
//    This code is licensed under the MIT License.
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF 
//    ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
//    TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
//    PARTICULAR PURPOSE AND NONINFRINGEMENT.
// </copyright>
// <summary>The ILayoutPickerWizardPagePresenter type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.Word4Tfs.Library.Presenter
{
    using Microsoft.Word4Tfs.Library.Data;

    /// <summary>
    /// Defines the operations on the presenter for the layout picker view. Only valid when <see cref="IsValid"/> is true.
    /// </summary>
    public interface ILayoutPickerWizardPagePresenter : IWizardPagePresenter
    {
        /// <summary>
        /// Gets the selected layout.
        /// </summary>
        LayoutInformation SelectedLayout { get; }
    }
}
