//---------------------------------------------------------------------
// <copyright file="LayoutListControl.cs" company="Microsoft">
//    Copyright © ALM | DevOps Ranger Contributors. All rights reserved.
//    This code is licensed under the MIT License.
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF 
//    ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
//    TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
//    PARTICULAR PURPOSE AND NONINFRINGEMENT.
// </copyright>
// <summary>The LayoutListControl type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.Word4Tfs.WordAddIn
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Windows.Forms;
    using Microsoft.Word4Tfs.Library.Data;
    using Microsoft.Word4Tfs.Library.View;

    /// <summary>
    /// Control for presenting a list of layouts.
    /// </summary>
    public partial class LayoutListControl : UserControl
    {
        /// <summary>
        /// Indicates if we are cancelling a selection.
        /// </summary>
        private bool cancellingSelection;

        /// <summary>
        /// The index of the last layout that was selected.
        /// </summary>
        private int lastSelectedIndex = -1;

        /// <summary>
        /// Initializes a new instance of the <see cref="LayoutListControl"/> class.
        /// </summary>
        public LayoutListControl()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Raised when a layout is selected. Selection can be cancelled.
        /// </summary>
        public event EventHandler<LayoutItemEventArgs> LayoutSelected;

        /// <summary>
        /// Raised when a layout is to be renamed. Rename can be cancelled.
        /// </summary>
        public event EventHandler<RenameEventArgs> LayoutRename;

        /// <summary>
        /// Raised when a layout is to be deleted. Deletion can be cancelled.
        /// </summary>
        public event EventHandler<LayoutItemEventArgs> LayoutDelete;

        /// <summary>
        /// Gets or sets a value indicating whether layouts can be renamed.
        /// </summary>
        public bool AllowRename
        {
            get
            {
                return this.listViewLayouts.LabelEdit;
            }

            set
            {
                this.listViewLayouts.LabelEdit = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether layouts can be deleted.
        /// </summary>
        public bool AllowDelete { get; set; }

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

            this.listViewLayouts.Items.Clear();
            foreach (LayoutInformation li in layouts)
            {
                ListViewItem lvi = new ListViewItem();
                lvi.Tag = li;
                lvi.Text = li.Name;
                lvi.SubItems.Add(li.Name);
                lvi.ToolTipText = li.Name;
                this.listViewLayouts.Items.Add(lvi);
            }
        }

        /// <summary>
        /// Causes the given layout to be selected in the list of layouts.
        /// </summary>
        /// <remarks>This will <u>not</u> raise the <see cref="LayoutSelected"/> event.</remarks>
        /// <param name="layoutName">The name of the layout to be selected</param>
        public void SelectLayout(string layoutName)
        {
            foreach (ListViewItem lvi in this.listViewLayouts.Items)
            {
                if (lvi.Text == layoutName)
                {
                    this.listViewLayouts.SelectedIndexChanged -= new System.EventHandler(this.ListViewLayouts_SelectedIndexChanged);
                    lvi.Selected = true;
                    lvi.BackColor = Color.FromKnownColor(KnownColor.Highlight);
                    lvi.ForeColor = Color.FromKnownColor(KnownColor.HighlightText);
                    this.listViewLayouts.SelectedIndexChanged += new System.EventHandler(this.ListViewLayouts_SelectedIndexChanged);
                    this.lastSelectedIndex = lvi.Index;
                    lvi.EnsureVisible();
                }
                else
                {
                    lvi.BackColor = Color.FromKnownColor(KnownColor.Window);
                    lvi.ForeColor = Color.FromKnownColor(KnownColor.WindowText);
                }
            }
        }

        /// <summary>
        /// Causes the given layout name to be set into edit mode.
        /// </summary>
        /// <param name="layoutName">The name of the layout to be edited</param>
        public void StartLayoutNameEdit(string layoutName)
        {
            foreach (ListViewItem lvi in this.listViewLayouts.Items)
            {
                if (lvi.Text == layoutName)
                {
                    lvi.BeginEdit();
                    break;
                }
            }
        }

        /// <summary>
        /// Handles the Load event.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        private void LayoutListControl_Load(object sender, EventArgs e)
        {
            this.listViewLayouts.AfterLabelEdit += new LabelEditEventHandler(this.HandleAfterLabelEdit);
            this.SetToolStripItemVisibility("Rename", this.AllowRename);
            this.SetToolStripItemVisibility("Delete", this.AllowDelete);
            this.ResizeColumn();
        }

        /// <summary>
        /// Sets the visibility of a tool strip item.
        /// </summary>
        /// <param name="tag">The tag of the item to set.</param>
        /// <param name="visible">The visibility to set for the item.</param>
        private void SetToolStripItemVisibility(string tag, bool visible)
        {
            ToolStripItem toolStripItem = this.FindToolStripItemByTag(tag);

            if (toolStripItem != null)
            {
                toolStripItem.Visible = visible;
            }
        }

        /// <summary>
        /// Finds a tool strip item by its tag.
        /// </summary>
        /// <param name="tag">The tag of the tool strip item to find.</param>
        /// <returns>The tool strip item, <c>null</c> if not found.</returns>
        private ToolStripItem FindToolStripItemByTag(string tag)
        {
            ToolStripItem toolStripItem = null;
            foreach (ToolStripItem item in this.contextMenuStrip.Items)
            {
                if (item.Tag.ToString() == tag)
                {
                    toolStripItem = item;
                    break;
                }
            }

            return toolStripItem;
        }

        /// <summary>
        /// Handles the AfterLabelEdit event.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        private void HandleAfterLabelEdit(object sender, LabelEditEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(e.Label) && this.LayoutRename != null)
            {
                RenameEventArgs rename = new RenameEventArgs(this.listViewLayouts.Items[e.Item].Text, e.Label);
                this.LayoutRename(this, rename);
                e.CancelEdit = rename.Cancel;
            }
            else
            {
                e.CancelEdit = true;
            }
        }

        /// <summary>
        /// Handles when a new layout is selected from the grid.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        private void ListViewLayouts_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.listViewLayouts.SelectedItems.Count > 0 && this.lastSelectedIndex != this.listViewLayouts.SelectedItems[0].Index)
            {
                int selectedItemIndex = this.listViewLayouts.SelectedItems[0].Index; // need to get it here, the SelectedItems collection can become empty later.

                LayoutInformation layout = this.GetSelectedLayout();
                if (layout != null)
                {
                    LayoutItemEventArgs args = new LayoutItemEventArgs(layout);

                    // When cancelling a selection the event is raised twice, not just when deselecting old and selecting new, but twice to select the new value. So must ignore
                    // second raise.
                    if (this.cancellingSelection)
                    {
                        // This was the second time the change was attempted, ignore it and don't prompt again.
                        this.cancellingSelection = false;
                        args.Cancel = true;
                    }
                    else
                    {
                        if (this.LayoutSelected != null)
                        {
                            this.LayoutSelected(this, args);
                        }

                        if (args.Cancel)
                        {
                            this.cancellingSelection = true;
                        }
                    }

                    if (args.Cancel)
                    {
                        this.listViewLayouts.Items[this.lastSelectedIndex].Selected = true;
                    }
                    else
                    {
                        this.lastSelectedIndex = selectedItemIndex;
                    }

                    this.SelectLayout(this.GetSelectedLayout().Name); // Ensures selected layout is highlighted correctly.
                }
            }
        }

        /// <summary>
        /// Handles when a key is pressed on the list view.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        private void ListViewLayouts_KeyDown(object sender, KeyEventArgs e)
        {
            if (((e.KeyCode & Keys.Back) != 0 || (e.KeyCode & Keys.Delete) != 0) && this.AllowDelete)
            {
                LayoutInformation layout = this.GetSelectedLayout();
                if (layout != null && this.LayoutDelete != null)
                {
                    this.LayoutDelete(this, new LayoutItemEventArgs(layout));
                }
            }
        }

        /// <summary>
        /// Gets the currently selected layout.
        /// </summary>
        /// <returns>The currently selected layout, <c>null</c> if there is no selected layout.</returns>
        private LayoutInformation GetSelectedLayout()
        {
            LayoutInformation ans = null;
            if (this.listViewLayouts.SelectedItems.Count > 0)
            {
                ans = this.listViewLayouts.SelectedItems[0].Tag as LayoutInformation;
            }

            return ans;
        }

        /// <summary>
        /// Handles the click event for the rename tool strip item.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        private void RenameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutInformation layout = this.GetSelectedLayout();
            if (layout != null)
            {
                this.StartLayoutNameEdit(layout.Name);
            }
        }

        /// <summary>
        /// Handles the click event for the delete tool strip item.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        private void DeleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutInformation layout = this.GetSelectedLayout();
            if (layout != null && this.LayoutDelete != null)
            {
                this.LayoutDelete(this, new LayoutItemEventArgs(layout));
            }
        }

        /// <summary>
        /// Handles the resize event for the list view so the columns can be set to the right width.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        private void ListViewLayouts_SizeChanged(object sender, EventArgs e)
        {
            this.ResizeColumn();
        }

        /// <summary>
        /// Resizes the column that displays the list of layout.s
        /// </summary>
        private void ResizeColumn()
        {
            this.listViewLayouts.Columns[0].AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
        }
    }
}
