//---------------------------------------------------------------------
// <copyright file="IWizardPageView.cs" company="Microsoft">
//    Copyright © ALM | DevOps Ranger Contributors. All rights reserved.
//    This code is licensed under the MIT License.
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF 
//    ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
//    TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
//    PARTICULAR PURPOSE AND NONINFRINGEMENT.
// </copyright>
// <summary>The IWizardPageView type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.Word4Tfs.Library.View
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// The operations on a wizard page view.
    /// </summary>
    public interface IWizardPageView
    {
        /// <summary>
        /// Causes the view to show itself.
        /// </summary>
        void Show();

        /// <summary>
        /// Causes the view to hide itself.
        /// </summary>
        void Hide();
    }
}
