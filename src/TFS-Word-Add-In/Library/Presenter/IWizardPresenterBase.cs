//---------------------------------------------------------------------
// <copyright file="IWizardPresenterBase.cs" company="Microsoft">
//    Copyright © ALM | DevOps Ranger Contributors. All rights reserved.
//    This code is licensed under the MIT License.
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF 
//    ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
//    TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
//    PARTICULAR PURPOSE AND NONINFRINGEMENT.
// </copyright>
// <summary>The IWizardPresenterBase type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.Word4Tfs.Library.Presenter
{
    using System;

    // TODO: Is this interface needed?

    /// <summary>
    /// Defines the operations of the wizard presenter base.
    /// </summary>
    public interface IWizardPresenterBase
    {
        /// <summary>
        /// Initialises the wizard presenter and all the page presenters.
        /// </summary>
        void Initialise();

        /// <summary>
        /// Starts the wizard on the first page.
        /// </summary>
        /// <remarks>
        /// <see cref="Initialise"/> must have been called first.
        /// </remarks>
        /// <returns><c>True</c> if the wizard is finished, <c>false</c> if the wizard is cancelled.</returns>
        bool Start();
    }
}
