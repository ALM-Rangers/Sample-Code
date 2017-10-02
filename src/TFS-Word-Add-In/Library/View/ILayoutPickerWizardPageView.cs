//---------------------------------------------------------------------
// <copyright file="ILayoutPickerWizardPageView.cs" company="Microsoft">
//    Copyright © ALM | DevOps Ranger Contributors. All rights reserved.
//    This code is licensed under the MIT License.
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF 
//    ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
//    TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
//    PARTICULAR PURPOSE AND NONINFRINGEMENT.
// </copyright>
// <summary>The ILayoutPickerWizardPageView type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.Word4Tfs.Library.View
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Word4Tfs.Library.Data;

    /// <summary>
    /// Defines the operations on the view for the wizard page that allows the user to select a layout.
    /// </summary>
    public interface ILayoutPickerWizardPageView : IWizardPageView
    {
        /// <summary>
        /// Raised when the user selects a layout.
        /// </summary>
        event EventHandler<LayoutItemEventArgs> LayoutSelected;

        /// <summary>
        /// Gets the layout that has been selected.
        /// </summary>
        /// <remarks>
        /// Only valid after <see cref="StartDialog"/> has returned <c>true</c>.
        /// </remarks>
        LayoutInformation SelectedLayout { get; }

        /// <summary>
        /// Loads the list of layouts to display to the user.
        /// </summary>
        /// <param name="layouts">The list of layouts to be displayed.</param>
        void LoadLayouts(IEnumerable<LayoutInformation> layouts);

        /// <summary>
        /// Causes the given layout to be selected.
        /// </summary>
        /// <remarks>This will <u>not</u> raise the <see cref="LayoutSelected"/> event.</remarks>
        /// <param name="layoutName">The name of the layout to be selected</param>
        void SelectLayout(string layoutName);
    }
}
