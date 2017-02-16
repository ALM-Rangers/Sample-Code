//---------------------------------------------------------------------
// <copyright file="MessageDialogue.cs" company="Microsoft">
//    Copyright (c) Microsoft. All rights reserved.
//    This code is licensed under the MIT License.
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF 
//    ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
//    TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
//    PARTICULAR PURPOSE AND NONINFRINGEMENT.
// </copyright>
// <summary>The MessageDialogue type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.Word4Tfs.WordAddIn
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;
    using Microsoft.Word4Tfs.Library;

    /// <summary>
    /// Dialogue for showing errors, warnings and other messages, with optional detail.
    /// </summary>
    public partial class MessageDialogue : Form
    {
        /// <summary>
        /// The size of the dialog when detail is not shown.
        /// </summary>
        private Size noDetailsSize;

        /// <summary>
        /// The size of the dialogue when detail is shown.
        /// </summary>
        private Size withDetailsSize;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageDialogue"/> class.
        /// </summary>
        public MessageDialogue()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Sets up the display of a message.
        /// </summary>
        /// <param name="messageType">The type of message to display.</param>
        /// <param name="title">The title to display.</param>
        /// <param name="message">The message to display.</param>
        /// <param name="details">The details to display (optional)</param>
        public void DisplayMessage(UIMessageType messageType, string title, string message, string details)
        {
            Icon icon;
            switch (messageType)
            {
                case UIMessageType.Warning:
                    {
                        icon = SystemIcons.Warning;
                        break;
                    }

                case UIMessageType.Error:
                    {
                        icon = SystemIcons.Error;
                        break;
                    }

                default:
                    {
                        icon = SystemIcons.Error;
                        break;
                    }
            }

            using (Icon errorIcon = new Icon(icon, 32, 32))
            {
                this.pictureBoxIcon.Image = errorIcon.ToBitmap();
            }

            this.Text = title;
            this.textBoxMessage.Text = message;
            if (!string.IsNullOrEmpty(details))
            {
                this.buttonDetails.Visible = true;
                this.textBoxDetails.Text = details;
                this.textBoxDetails.Visible = false;
            }
            else
            {
                this.buttonDetails.Visible = false;
            }

            this.buttonDetails.Text = DialogueResources.ShowDetails;
        }
        
        /// <summary>
        /// Handles the <see cref="Load"/> event.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event args.</param>
        private void ErrorDialogue_Load(object sender, EventArgs e)
        {
            this.noDetailsSize = new Size(682, 240);
            this.withDetailsSize = new Size(682, 240 + 175 + 20);
        }

        /// <summary>
        /// Handles the <see cref="Click"/> event for the OK button.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event args.</param>
        private void ButtonOk_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Causes the details to be shown.
        /// </summary>
        private void ShowDetails()
        {
            this.Size = this.withDetailsSize;
            this.buttonDetails.Text = DialogueResources.HideDetails;
        }

        /// <summary>
        /// Causes the details to be hidden.
        /// </summary>
        private void HideDetails()
        {
            this.Size = this.noDetailsSize;
            this.buttonDetails.Text = DialogueResources.ShowDetails;
        }

        /// <summary>
        /// Handles the <see cref="Click"/> event for the details button.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event args.</param>
        private void ButtonDetails_Click(object sender, EventArgs e)
        {
            this.textBoxDetails.Visible = !this.textBoxDetails.Visible;
            if (this.textBoxDetails.Visible)
            {
                this.ShowDetails();
            }
            else
            {
                this.HideDetails();
            }
        }
    }
}
