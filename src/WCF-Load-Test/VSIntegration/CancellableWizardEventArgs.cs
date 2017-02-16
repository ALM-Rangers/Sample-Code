//---------------------------------------------------------------------
// <copyright file="CancellableWizardEventArgs.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
//     THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
//     OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
//     LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR
//     FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
// <summary>The CancellableWizardEventArgs type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.WcfUnit.VSIntegration
{
    using System.ComponentModel;
    using Microsoft.WcfUnit.Library;

    /// <summary>
    /// Event that can be cancelled that contains <see cref="WizardData"/>.
    /// </summary>
    public class CancellableWizardEventArgs : CancelEventArgs
    {
        /// <summary>
        /// The wizard data.
        /// </summary>
        private WizardData data;

        /// <summary>
        /// Initialises a new instance of the <see cref="CancellableWizardEventArgs"/> class.
        /// </summary>
        /// <param name="data">The wizard data.</param>
        /// <param name="cancel">Value for the <see cref="CancelEventArgs.Cancel">Cancel</see> property.</param>
        public CancellableWizardEventArgs(WizardData data, bool cancel)
            : base(cancel)
        {
            this.data = data;
        }

        /// <summary>
        /// Gets the wizard data.
        /// </summary>
        /// <value>The wizard data.</value>
        public WizardData Data
        {
            get { return this.data; }
        }
    }
}
