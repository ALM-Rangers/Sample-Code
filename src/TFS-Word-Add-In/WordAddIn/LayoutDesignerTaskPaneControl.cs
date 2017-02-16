//---------------------------------------------------------------------
// <copyright file="LayoutDesignerTaskPaneControl.cs" company="Microsoft">
//    Copyright (c) Microsoft. All rights reserved.
//    This code is licensed under the MIT License.
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF 
//    ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
//    TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
//    PARTICULAR PURPOSE AND NONINFRINGEMENT.
// </copyright>
// <summary>The LayoutDesignerTaskPaneControl type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.Word4Tfs.WordAddIn
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Windows.Forms;
    using Microsoft.Office.Tools;
    using Microsoft.Practices.Unity;
    using Microsoft.Word4Tfs.Library;
    using Microsoft.Word4Tfs.Library.Data;
    using Microsoft.Word4Tfs.Library.Model.TeamFoundation;
    using Microsoft.Word4Tfs.Library.View;

    /// <summary>
    /// The view for the layout designer.
    /// </summary>
    public partial class LayoutDesignerTaskPaneControl : UserControl, ILayoutDesignerView
    {
        /// <summary>
        /// The custom task pane that is populated with this control.
        /// </summary>
        private CustomTaskPane customTaskPane;

        /// <summary>
        /// Timer used to clear a status message.
        /// </summary>
        private Timer statusTimer = new Timer();

        /// <summary>
        /// Initializes a new instance of the <see cref="LayoutDesignerTaskPaneControl"/> class.
        /// </summary>
        public LayoutDesignerTaskPaneControl()
        {
            this.InitializeComponent();
            this.layoutListControl.AllowRename = true;
            this.layoutListControl.AllowDelete = true; 
            this.statusTimer.Tick += new EventHandler(this.HandleStatusTimer);
        }

        /// <summary>
        /// Raised when the user wants the layouts to be saved.
        /// </summary>
        public event EventHandler Save;

        /// <summary>
        /// Raised when the user wants the designer to be connected to TFS.
        /// </summary>
        public event EventHandler Connect;

        /// <summary>
        /// Raised when the view is hidden or closed.
        /// </summary>
        public event EventHandler Hidden;

        /// <summary>
        /// Raised when a field is to be added to the designer document.
        /// </summary>
        public event EventHandler<FieldDefinitionEventArgs> AddField;

        /// <summary>
        /// Raised when a new layout is to be added to the designer document.
        /// </summary>
        public event EventHandler AddNewLayout;

        /// <summary>
        /// Raised when the user selects a layout. Cancellable so that the layout selection is not changed.
        /// </summary>
        public event EventHandler<LayoutItemEventArgs> LayoutSelected;

        /// <summary>
        /// Raised when a layout is to be renamed. Rename can be cancelled.
        /// </summary>
        public event EventHandler<RenameEventArgs> LayoutRename;

        /// <summary>
        /// Raised when the user deletes a layout. Cancellable so that the layout deletion can be stopped.
        /// </summary>
        public event EventHandler<LayoutItemEventArgs> LayoutDelete;

        /// <summary>
        /// Gets or sets the addin that is using this view.
        /// </summary>
        /// <remarks>
        /// Required so that the control can add itself as a custom task pane.
        /// </remarks>
        [Dependency]
        public ThisAddIn AddIn { get; set; }

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
        /// Shows the layout designer.
        /// </summary>
        public void ShowLayoutDesigner()
        {
            this.GetCustomTaskPane().Visible = true;
        }

        /// <summary>
        /// Hides the layout designer.
        /// </summary>
        public void HideLayoutDesigner()
        {
            this.GetCustomTaskPane().Visible = false;
        }

        /// <summary>
        /// Gives the view the list of layouts to display.
        /// </summary>
        /// <param name="layouts">The list of layouts to display.</param>
        public void SetLayoutList(IEnumerable<LayoutInformation> layouts)
        {
            if (layouts == null)
            {
                throw new ArgumentNullException("layouts");
            }

            this.layoutListControl.SetLayoutList(layouts);
        }

        /// <summary>
        /// Causes the given layout to be selected.
        /// </summary>
        /// <remarks>This will <u>not</u> raise the <see cref="LayoutSelected"/> event.</remarks>
        /// <param name="layoutName">The name of the layout to be selected</param>
        public void SelectLayout(string layoutName)
        {
            this.layoutListControl.SelectLayout(layoutName);
        }

        /// <summary>
        /// Causes the given layout name to be set into edit mode.
        /// </summary>
        /// <param name="layoutName">The name of the layout to be edited</param>
        public void StartLayoutNameEdit(string layoutName)
        {
            this.layoutListControl.StartLayoutNameEdit(layoutName);
        }

        /// <summary>
        /// Gives the view the list of fields in the team project that are available for selection.
        /// </summary>
        /// <param name="fields">The fields.</param>
        public void SetFieldList(IEnumerable<ITfsFieldDefinition> fields)
        {
            if (fields == null)
            {
                throw new ArgumentNullException("fields");
            }

            this.listViewFields.Items.Clear();
            foreach (ITfsFieldDefinition field in fields)
            {
                ListViewItem lvi = new ListViewItem();
                lvi.Tag = field;
                lvi.Text = field.FriendlyName;
                lvi.SubItems.Add(field.FriendlyName);
                lvi.ToolTipText = field.ReferenceName;
                this.listViewFields.Items.Add(lvi);
            }
        }

        /// <summary>
        /// Closes the view.
        /// </summary>
        public void Close()
        {
            if (this.customTaskPane != null)
            {
                this.HideLayoutDesigner(); // to ensure event is raised so that presenter can uncheck the ribbon button.
                this.AddIn.CustomTaskPanes.Remove(this.customTaskPane);
                this.customTaskPane = null;
            }
        }

        /// <summary>
        /// Sets the string in the task pane status bar.
        /// </summary>
        /// <param name="status">The status message to be displayed.</param>
        /// <param name="timeout">The number of seconds to display the status message, less than 1 means infinite.</param>
        public void SetStatus(string status, int timeout)
        {
            this.statusTimer.Stop();
            this.textBoxStatus.Text = status;
            if (timeout > 0)
            {
                this.statusTimer.Interval = timeout * 1000;
                this.statusTimer.Start();
            }
        }

        /// <summary>
        /// Handles the status timer tick.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        private void HandleStatusTimer(object sender, EventArgs e)
        {
            this.statusTimer.Stop();
            this.textBoxStatus.Text = string.Empty;
        }

        /// <summary>
        /// Handles the custom task pane's VisibleChanged event.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        private void HandleVisibleChanged(object sender, EventArgs e)
        {
            if (this.customTaskPane != null && !this.customTaskPane.Visible)
            {
                if (this.Hidden != null)
                {
                    this.Hidden(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Gets the custom task pane, setting it up the first time it is accessed.
        /// </summary>
        /// <returns>The custom task pane.</returns>
        private CustomTaskPane GetCustomTaskPane()
        {
            if (this.customTaskPane == null)
            {
                this.customTaskPane = this.AddIn.CustomTaskPanes.Add(this, DialogueResources.LayoutDesignerTitle, this.AddIn.Application.ActiveDocument.Windows[1]);
                this.customTaskPane.VisibleChanged += new EventHandler(this.HandleVisibleChanged);
            }

            return this.customTaskPane;
        }

        /// <summary>
        /// Handles the clicking of the Save button.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        private void ButtonSave_Click(object sender, EventArgs e)
        {
            if (this.Save != null)
            {
                this.Save(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Handles the clicking of the Connect button.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        private void ButtonConnect_Click(object sender, EventArgs e)
        {
            if (this.Connect != null)
            {
                this.Connect(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Handles the <see cref="LayoutListControl.LayoutSelected"/> event.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        private void LayoutListControl_LayoutSelected(object sender, LayoutItemEventArgs e)
        {
            if (this.LayoutSelected != null)
            {
                this.LayoutSelected(this, e);
            }
        }

        /// <summary>
        /// Handles when a field is dragged to the designer document.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        private void ListViewFields_ItemDrag(object sender, ItemDragEventArgs e)
        {
            ////ListViewItem lvi = (ListViewItem)e.Item;
            ////ITfsFieldDefinition field = (ITfsFieldDefinition)lvi.Tag;
            ////DragDropEffects dde = this.DoDragDrop(field.FriendlyName, DragDropEffects.Copy);
            ////if (dde == DragDropEffects.Copy)
            ////{
            ////    // TODO: How to check that this is really the document that is the destination and not something else.
            ////    if (this.AddField != null)
            ////    {
            ////        this.AddField(this, new FieldDefinitionEventArgs(field));
            ////    }
            ////}
        }

        /// <summary>
        /// Handles when the user request a new layout is added.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        private void ButtonAddNew_Click(object sender, EventArgs e)
        {
            if (this.AddNewLayout != null)
            {
                this.AddNewLayout(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Handles when the user request to change a layout name.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        private void LayoutListControl_LayoutRename(object sender, RenameEventArgs e)
        {
            if (this.LayoutRename != null)
            {
                this.LayoutRename(this, e);
            }
        }

        /// <summary>
        /// Handles when the user request to delete a layout.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        private void LayoutListControl_LayoutDelete(object sender, LayoutItemEventArgs e)
        {
            if (this.LayoutDelete != null)
            {
                this.LayoutDelete(this, e);
            }
        }

        /// <summary>
        /// Handles the resize event for the list view so the columns can be set to the right width.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        private void ListViewFields_SizeChanged(object sender, EventArgs e)
        {
            this.listViewFields.Columns[0].AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
        }

        /// <summary>
        /// Handles the double click event for the list view.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        private void ListViewFields_DoubleClick(object sender, EventArgs e)
        {
            this.OnFieldSelectedForAdd();
        }

        /// <summary>
        /// Handles the key down event for the list view.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        private void ListViewFields_KeyDown(object sender, KeyEventArgs e)
        {
            System.Diagnostics.Trace.WriteLine(e.KeyCode);
            if (e.KeyValue == 13 && this.listViewFields.SelectedItems.Count == 1)
            {
                this.OnFieldSelectedForAdd();
            }
        }

        /// <summary>
        /// Raises event for the selected field to be added.
        /// </summary>
        private void OnFieldSelectedForAdd()
        {
            ListViewItem lvi = (ListViewItem)this.listViewFields.SelectedItems[0];
            ITfsFieldDefinition field = (ITfsFieldDefinition)lvi.Tag;

            // TODO: How to check that this is really the document that is the destination and not something else.
            if (this.AddField != null)
            {
                this.AddField(this, new FieldDefinitionEventArgs(field));
            }
        }
    }
}
