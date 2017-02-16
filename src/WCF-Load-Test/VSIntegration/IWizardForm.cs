//---------------------------------------------------------------------
// <copyright file="IWizardForm.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
//     THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
//     OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
//     LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR
//     FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
// <summary>Contains the IWizardForm interface type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.WcfUnit.VSIntegration
{
    using System;
    using Microsoft.WcfUnit.Library;

    /// <summary>
    /// Interface for controlling the wizard form.
    /// </summary>
    public interface IWizardForm : IDisposable
    {
        /// <summary>
        /// Raised when the user has clicked the Finish button and the code generation should run.
        /// </summary>
        /// <remarks>
        /// The event handler can cancel the close if there was an error so that the user has the
        /// opportunity to fix the error.
        /// </remarks>
        event EventHandler<CancellableWizardEventArgs> WizardCompleting;

        /// <summary>
        /// Gets the data gathered by the wizard
        /// </summary>
        WizardData WizardData
        {
            get;
        }

        /// <summary>
        /// Runs the wizard
        /// </summary>
        /// <returns>True if the wizard completed, false if it was cancelled.</returns>
        bool RunWizard();
    }
}
