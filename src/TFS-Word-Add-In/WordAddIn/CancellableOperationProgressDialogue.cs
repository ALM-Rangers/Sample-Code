//---------------------------------------------------------------------
// <copyright file="CancellableOperationProgressDialogue.cs" company="Microsoft">
//    Copyright (c) Microsoft. All rights reserved.
//    This code is licensed under the MIT License.
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF 
//    ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
//    TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
//    PARTICULAR PURPOSE AND NONINFRINGEMENT.
// </copyright>
// <summary>The CancellableOperationProgressDialogue type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.Word4Tfs.WordAddIn
{
    using System;
    using System.Threading;
    using System.Windows.Forms;

    /// <summary>
    /// The dialogue used to display a dialogue to show that a long operation is in progress and which allows the user to cancel.
    /// </summary>
    public partial class CancellableOperationProgressDialogue : Form
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CancellableOperationProgressDialogue"/> class.
        /// </summary>
        public CancellableOperationProgressDialogue()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Starts the dialogue
        /// </summary>
        /// <param name="title">The title of the dialogue.</param>
        /// <param name="message">The message to be displayed.</param>
        /// <param name="cancellationTokenSource">The object used to cancel the operation.</param>
        public void Start(string title, string message, CancellationTokenSource cancellationTokenSource)
        {
            if (this.InvokeRequired)
            {
                this.Invoke((Action)delegate
                {
                    this.Start(title, message, cancellationTokenSource);
                });
            }
            else
            {
                this.Text = title;
                this.SetMessage(message);
                if (this.ShowDialog() == DialogResult.Cancel)
                {
                    cancellationTokenSource.Cancel();
                }
            }
        }

        /// <summary>
        /// Closes the dialogue.
        /// </summary>
        public void Finish()
        {
            if (this.InvokeRequired)
            {
                this.Invoke((Action)delegate
                {
                    this.Finish();
                });
            }
            else
            {
                this.Close();
            }
        }

        /// <summary>
        /// Sets the message on the dialogue.
        /// </summary>
        /// <param name="message">The message to display.</param>
        public void SetMessage(string message)
        {
            if (this.InvokeRequired)
            {
                this.Invoke((Action)delegate
                {
                    this.SetMessage(message);
                });
            }
            else
            {
                this.textBoxMessage.Text = message;
            }
        }
    }
}
