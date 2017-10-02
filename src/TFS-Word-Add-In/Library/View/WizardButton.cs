//---------------------------------------------------------------------
// <copyright file="WizardButton.cs" company="Microsoft">
//    Copyright © ALM | DevOps Ranger Contributors. All rights reserved.
//    This code is licensed under the MIT License.
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF 
//    ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
//    TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
//    PARTICULAR PURPOSE AND NONINFRINGEMENT.
// </copyright>
// <summary>The WizardButton type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.Word4Tfs.Library.View
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// The buttons on a wizard.
    /// </summary>
    public enum WizardButton
    {
        /// <summary>
        /// The button that moves to the next page.
        /// </summary>
        Next,

        /// <summary>
        /// The button that moves to the previous page.
        /// </summary>
        Previous,

        /// <summary>
        /// The button that causes the wizard to complete.
        /// </summary>
        Finish,

        /// <summary>
        /// The button to cancel the wizard.
        /// </summary>
        Cancel
    }
}
