//---------------------------------------------------------------------
// <copyright file="TeamRibbonView.cs" company="Microsoft">
//    Copyright (c) Microsoft. All rights reserved.
//    This code is licensed under the MIT License.
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF 
//    ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
//    TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
//    PARTICULAR PURPOSE AND NONINFRINGEMENT.
// </copyright>
// <summary>The TeamRibbonView type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.Word4Tfs.WordAddIn
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Threading;
    using Microsoft.Office.Tools.Ribbon;
    using Microsoft.Practices.Unity;
    using Microsoft.Word4Tfs.Library;
    using Microsoft.Word4Tfs.Library.View;

    /// <summary>
    /// This is the Team ribbon used by the addin.
    /// </summary>
    public partial class TeamRibbonView : ITeamRibbonView, IWaitNotifier
    {
        /// <summary>
        /// The form used to display the progress of a cancellable operation and allow it to be cancelled.
        /// </summary>
        private CancellableOperationProgressDialogue cancellableOperationProgress;

        /// <summary>
        /// Used while setting a toggle button so that events are not raised from the button.
        /// </summary>
        private bool dontRaiseToggleEvents = false;

        /// <summary>
        /// Raised when the user requests that work items be imported into the document from a team project.
        /// </summary>
        public event EventHandler Import;

        /// <summary>
        /// Raised when the user requests the refresh of work items that have been previously imported into the document from a team project.
        /// </summary>
        public event EventHandler Refresh;

        /// <summary>
        /// Raised when the user requests to see the layout designer.
        /// </summary>
        public event EventHandler ShowLayoutDesigner;

        /// <summary>
        /// Raised when the user requests to hide the layout designer.
        /// </summary>
        public event EventHandler HideLayoutDesigner;

        /// <summary>
        /// Raised when the user requests to see the bookmarks.
        /// </summary>
        public event EventHandler ShowBookmarks;

        /// <summary>
        /// Raised when the user requests to hide the bookmarks.
        /// </summary>
        public event EventHandler HideBookmarks;

        /// <summary>
        /// Gets or sets the object to be used for logging.
        /// </summary>
        [Dependency]
        public ILogger Logger { get; set; }

        /// <summary>
        /// Notifies the user that a long-running operation has started.
        /// </summary>
        public void StartWait()
        {
            System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.WaitCursor;
        }

        /// <summary>
        /// Notifies the user that a long-running operation has ended.
        /// </summary>
        public void EndWait()
        {
            System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.Default;
        }

        /// <summary>
        /// Sets the button to be enabled or not
        /// </summary>
        /// <param name="button">The button for which the status is to be set.</param>
        /// <param name="enabled">The enabled/disabled status to set.</param>
        public void SetButtonState(Enum button, bool enabled)
        {
            RibbonControl buttonToSet = this.FindRibbonControl(button);
            buttonToSet.Enabled = enabled;
        }

        /// <summary>
        /// Sets the check state of a Team Ribbon toggle button
        /// </summary>
        /// <remarks>
        /// Changing the state of the button through this method does not raise an event.
        /// </remarks>
        /// <param name="button">The button whose check state is to be changed.</param>
        /// <param name="isChecked">The checked state to set.</param>
        public void SetButtonCheckState(Enum button, bool isChecked)
        {
            RibbonControl control = this.FindRibbonControl(button);
            RibbonToggleButton toggleButton = control as RibbonToggleButton;
            RibbonCheckBox checkBox = control as RibbonCheckBox;
            if (toggleButton != null)
            {
                try
                {
                    this.dontRaiseToggleEvents = true;
                    toggleButton.Checked = isChecked;
                }
                finally
                {
                    this.dontRaiseToggleEvents = false;
                }
            }
            else if (checkBox != null)
            {
                try
                {
                    this.dontRaiseToggleEvents = true;
                    checkBox.Checked = isChecked;
                }
                finally
                {
                    this.dontRaiseToggleEvents = false;
                }
            }
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
        /// Displays a message about a cancellable operation and allows the user to cancel the operation.
        /// </summary>
        /// <param name="title">The title to be given to the message box.</param>
        /// <param name="message">The message to display.</param>
        /// <param name="cancellationTokenSource">Used to cancel the operation.</param>
        public void StartCancellableOperation(string title, string message, CancellationTokenSource cancellationTokenSource)
        {
            if (cancellationTokenSource == null)
            {
                throw new ArgumentNullException("cancellationTokenSource");
            }

            this.Logger.Log(TraceEventType.Information, "Starting cancellable operation dialog");
            this.cancellableOperationProgress = new CancellableOperationProgressDialogue();
            this.cancellableOperationProgress.Start(title, message, cancellationTokenSource);
        }

        /// <summary>
        /// Updates the message about a cancellable operation.
        /// </summary>
        /// <param name="message">The message to display</param>
        public void UpdateCancellableOperation(string message)
        {
            this.Logger.Log(TraceEventType.Information, "Updating cancellable operation dialog");
            this.cancellableOperationProgress.SetMessage(message);
        }

        /// <summary>
        /// Removes the dialogue about a cancellable operation.
        /// </summary>
        public void EndCancellableOperation()
        {
            this.Logger.Log(TraceEventType.Information, "Finishing cancellable operation dialog");
            this.cancellableOperationProgress.Finish();
        }

        /// <summary>
        /// Handler for the Load event of the team ribbon
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        private void TeamRibbon_Load(object sender, RibbonUIEventArgs e)
        {
            this.Logger.Log(TraceEventType.Information, "Loading icons");
            try
            {
                this.buttonImport.Image = IconManager.GetImage("NewList.png");
                this.buttonImport.ShowImage = true;
                this.buttonRefresh.Image = IconManager.GetImage("Refresh.png");
                this.buttonRefresh.ShowImage = true;
                this.toggleButtonShowLayoutEditor.Image = IconManager.GetImage("LayoutEditor.png");
                this.toggleButtonShowLayoutEditor.ShowImage = true;
            }
            catch (Win32Exception)
            {
                // Ignore problems loading the icons.
            }
        }

        /// <summary>
        /// Finds the ribbon control tagged with the name of the button.
        /// </summary>
        /// <param name="button">The button to find. The tag on the control is the string of the enumeration value.</param>
        /// <returns>The corresponding ribbon control, <c>null</c> if not found</returns>
        private RibbonControl FindRibbonControl(Enum button)
        {
            string tag = Utilities.GetButtonTag(button);
            foreach (RibbonTab tab in this.Tabs)
            {
                foreach (RibbonGroup group in tab.Groups)
                {
                    foreach (RibbonControl control in group.Items)
                    {
                        if (control.Tag != null && control.Tag.ToString() == tag)
                        {
                            return control;
                        }                    
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Handles the click event of the import button.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event args.</param>
        private void Import_Click(object sender, RibbonControlEventArgs e)
        {
            this.Logger.Log(TraceEventType.Information, "Import button clicked");
            if (this.Import != null)
            {
                this.Import(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Handles the click event of the refresh button.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event args.</param>
        private void Refresh_Click(object sender, RibbonControlEventArgs e)
        {
            this.Logger.Log(TraceEventType.Information, "Refresh button clicked");
            if (this.Refresh != null)
            {
                this.Refresh(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Handles the click event on the layout designer toggle button.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        private void ToggleButtonShowLayoutEditor_Click(object sender, RibbonControlEventArgs e)
        {
            if (!this.dontRaiseToggleEvents)
            {
                if (((RibbonToggleButton)sender).Checked)
                {
                    if (this.ShowLayoutDesigner != null)
                    {
                        this.ShowLayoutDesigner(this, EventArgs.Empty);
                    }
                }
                else
                {
                    if (this.HideLayoutDesigner != null)
                    {
                        this.HideLayoutDesigner(this, EventArgs.Empty);
                    }
                }
            }
        }

        /// <summary>
        /// Handles the click event on the show bookmarks toggle button.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        private void CheckBoxShowBookmarks_Click(object sender, RibbonControlEventArgs e)
        {
            if (!this.dontRaiseToggleEvents)
            {
                if (((RibbonCheckBox)sender).Checked)
                {
                    if (this.ShowBookmarks != null)
                    {
                        this.ShowBookmarks(this, EventArgs.Empty);
                    }
                }
                else
                {
                    if (this.HideBookmarks != null)
                    {
                        this.HideBookmarks(this, EventArgs.Empty);
                    }
                }
            }
        }
    }
}
