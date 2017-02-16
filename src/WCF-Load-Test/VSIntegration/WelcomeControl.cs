//---------------------------------------------------------------------
// <copyright file="WelcomeControl.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
//     THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
//     OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
//     LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR
//     FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
// <summary>The WelcomeControl type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.WcfUnit.VSIntegration
{
    using System;
    using System.Windows.Forms;

    /// <summary>
    /// The welcome control for the wizard.
    /// </summary>
    public partial class WelcomeControl : UserControl
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="WelcomeControl"/> class.
        /// </summary>
        public WelcomeControl()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Handler for Load event.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        private void WelcomeControl_Load(object sender, EventArgs e)
        {
            this.textBoxDescription.Text = DialogResources.WelcomeFormDescriptionSummary;
        }
    }
}
