//---------------------------------------------------------------------
// <copyright file="WorkItemQueryAndLayoutPickerWizardView.cs" company="Microsoft">
//    Copyright © ALM | DevOps Ranger Contributors. All rights reserved.
//    This code is licensed under the MIT License.
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF 
//    ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
//    TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
//    PARTICULAR PURPOSE AND NONINFRINGEMENT.
// </copyright>
// <summary>The WorkItemQueryAndLayoutPickerWizardView type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.Word4Tfs.WordAddIn
{
    using System;
    using System.Windows.Forms;
    using Microsoft.Word4Tfs.Library;
    using Microsoft.Word4Tfs.Library.View;

    /// <summary>
    /// This is the view of the wizard for choosing a query and a layout.
    /// </summary>
    public partial class WorkItemQueryAndLayoutPickerWizardView : Form, IWorkItemQueryAndLayoutPickerWizardView
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WorkItemQueryAndLayoutPickerWizardView"/> class.
        /// </summary>
        public WorkItemQueryAndLayoutPickerWizardView()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Raised when the user clicks the previous button.
        /// </summary>
        public event EventHandler PreviousPage;

        /// <summary>
        /// Raised when the user clicks the next button.
        /// </summary>
        public event EventHandler NextPage;

        /// <summary>
        /// Gets or sets the title of the current page.
        /// </summary>
        public string Title
        {
            get
            {
                return this.textBoxTitle.Text;
            }

            set
            {
                this.textBoxTitle.Text = value;
            }
        }

        /// <summary>
        /// Initializes the wizard as a whole.
        /// </summary>
        public void Initialize()
        {
            this.panelPages.Controls.Clear();
        }

        /// <summary>
        /// Adds a page to the list of pages managed by the wizard.
        /// </summary>
        /// <param name="pageView">The page to be added to the pages managed by the wizard.</param>
        public void AddPage(IWizardPageView pageView)
        {
            this.panelPages.Controls.Add(pageView as Control);
        }

        /// <summary>
        /// Starts showing the wizard dialog.
        /// </summary>
        /// <remarks>
        /// Events giving information about what has been selected are raised while in this call.
        /// </remarks>
        /// <returns><c>true</c> if the user clicked OK, <c>false</c> if the user cancelled the dialog.</returns>
        public bool StartDialog()
        {
            DialogResult result = this.ShowDialog();
            return result == DialogResult.OK;
        }

        /// <summary>
        /// Sets the button to be enabled or not
        /// </summary>
        /// <param name="button">The button for which the status is to be set.</param>
        /// <param name="enabled">The enabled/disabled status to set.</param>
        public void SetButtonState(Enum button, bool enabled)
        {
            Control buttonToSet = Utilities.FindButton(this, button);
            buttonToSet.Enabled = enabled;
        }

        /// <summary>
        /// Displays a message.
        /// </summary>
        /// <param name="messageType">The type of message to display.</param>
        /// <param name="title">The title to be given to the message box.</param>
        /// <param name="message">The message to display.</param>
        /// <param name="details">Optional details to display.</param>
        public void DisplayMessage(UIMessageType messageType, string title, string message, string details)
        {
            Utilities.DisplayMessage(messageType, title, message, details);
        }

        /// <summary>
        /// Handles the Load event.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        private void WorkItemQueryAndLayoutPickerWizardView_Load(object sender, EventArgs e)
        {
            this.pictureBoxTitleIcon.Image = IconManager.GetImage("NewList.png");
        }

        /// <summary>
        /// Handles the clicking of the finish button.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        private void ButtonFinish_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        /// <summary>
        /// Handles the clicking of the cancel button.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        private void ButtonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        /// <summary>
        /// Handles the clicking of the next button.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        private void ButtonNext_Click(object sender, EventArgs e)
        {
            if (this.NextPage != null)
            {
                this.NextPage(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Handles the clicking of the previous button.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        private void ButtonPrevious_Click(object sender, EventArgs e)
        {
            if (this.PreviousPage != null)
            {
                this.PreviousPage(this, EventArgs.Empty);
            }
        }
    }
}
