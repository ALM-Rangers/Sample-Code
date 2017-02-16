//---------------------------------------------------------------------
// <copyright file="LayoutPickerWizardPageView.cs" company="Microsoft">
//    Copyright (c) Microsoft. All rights reserved.
//    This code is licensed under the MIT License.
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF 
//    ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
//    TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
//    PARTICULAR PURPOSE AND NONINFRINGEMENT.
// </copyright>
// <summary>The LayoutPickerWizardPageView type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.Word4Tfs.WordAddIn
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Windows.Forms;
    using Microsoft.Word4Tfs.Library.Data;
    using Microsoft.Word4Tfs.Library.View;

    /// <summary>
    /// This the wizard page used to select a layout.
    /// </summary>
    public partial class LayoutPickerWizardPageView : UserControl, ILayoutPickerWizardPageView
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LayoutPickerWizardPageView"/> class.
        /// </summary>
        public LayoutPickerWizardPageView()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Raised when the user selects a layout.
        /// </summary>
        public event EventHandler<LayoutItemEventArgs> LayoutSelected;

        /// <summary>
        /// Gets the layout that has been selected.
        /// </summary>
        /// <remarks>
        /// Only valid after <see cref="StartDialog"/> has returned <c>true</c>.
        /// </remarks>
        public LayoutInformation SelectedLayout { get; private set; }

        /// <summary>
        /// Loads the list of layouts to display to the user.
        /// </summary>
        /// <param name="layouts">The list of layouts to be displayed.</param>
        public void LoadLayouts(IEnumerable<LayoutInformation> layouts)
        {
            if (layouts == null)
            {
                throw new ArgumentNullException("layouts");
            }

            this.layoutListControl.SetLayoutList(layouts);
            this.HandleLayoutSelectedIndexChanged(this, new LayoutItemEventArgs(layouts.First()));
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
        /// Handles the <see cref="ComboBox.SelectedIndexChanged"/> event for the layouts combo box.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event data.</param>
        private void HandleLayoutSelectedIndexChanged(object sender, LayoutItemEventArgs e)
        {
            double scaleFactor = (double)this.pictureBoxPreview.Width / (double)e.LayoutItem.PreviewImage.Width;
            Size scaledSize = new Size(Convert.ToInt32(e.LayoutItem.PreviewImage.Width * scaleFactor), Convert.ToInt32(e.LayoutItem.PreviewImage.Height * scaleFactor));
            this.pictureBoxPreview.Image = new Bitmap(e.LayoutItem.PreviewImage, scaledSize);
            if (this.LayoutSelected != null)
            {
                this.LayoutSelected(this, e);
            }

            this.SelectedLayout = e.LayoutItem;
        }

        /// <summary>
        /// Handles the <see cref="VisibleChanged"/> event.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event data.</param>
        private void LayoutPickerWizardPageView_VisibleChanged(object sender, EventArgs e)
        {
            this.layoutListControl.Focus();
        }
    }
}
