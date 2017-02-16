//---------------------------------------------------------------------
// <copyright file="IWizardViewPage.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
//     THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
//     OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
//     LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR
//     FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
// <summary>The IWizardViewPage interface.</summary>
//---------------------------------------------------------------------

namespace Microsoft.WcfUnit.VSIntegration
{
    /// <summary>
    /// Implemented by a control which represents a page on a wizard view
    /// </summary>
    public interface IWizardViewPage
    {
        /// <summary>
        /// Sets the controller to be used by the view.
        /// </summary>
        /// <param name="controller">The controller to be used by the view.</param>
        void SetController(WizardController controller);
    }
}
