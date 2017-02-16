//---------------------------------------------------------------------
// <copyright file="SelectAssembliesControl.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
//     THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
//     OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
//     LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR
//     FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
// <summary>The SelectAssembliesControl type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.WcfUnit.VSIntegration
{
    using System;
    using System.Windows.Forms;

    /// <summary>
    /// The control used to select the assemblies containing the proxy code.
    /// </summary>
    public partial class SelectAssembliesControl : UserControl, IWizardViewPage
    {
        /// <summary>
        /// The controller that controls this control.
        /// </summary>
        private WizardController wizardController;

        /// <summary>
        /// Indicates the source of an update.
        /// </summary>
        private bool updateFromController;

        /// <summary>
        /// Initialises a new instance of the <see cref="SelectAssembliesControl"/> class.
        /// </summary>
        public SelectAssembliesControl()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Sets the list of assemblies for the proxy.
        /// </summary>
        /// <param name="assemblyList">The list of assemblies for the proxy.</param>
        public void SetAssemblyList(AssemblyType[] assemblyList)
        {
            if (assemblyList == null)
            {
                throw new ArgumentNullException("assemblyList");
            }

            try
            {
                this.updateFromController = true;
                this.dataGridViewAssemblies.Rows.Clear();
                foreach (AssemblyType a in assemblyList)
                {
                    this.AddAssembly(a.fileName);
                }
            }
            finally
            {
                this.updateFromController = false;
            }
        }

        /// <summary>
        /// Adds an assembly to the list of assemblies.
        /// </summary>
        /// <param name="fileName">The filename of the assembly.</param>
        public void AddAssembly(string fileName)
        {
            try
            {
                this.updateFromController = true;
                this.dataGridViewAssemblies.Rows.Add(fileName);
            }
            finally
            {
                this.updateFromController = false;
            }
        }

        /// <summary>
        /// Removes an assembly from the list of assemblies.
        /// </summary>
        /// <param name="index">The index into the list of assemblies of the assembly to remove.</param>
        public void RemoveAssembly(int index)
        {
            try
            {
                this.updateFromController = true;
                this.dataGridViewAssemblies.Rows.RemoveAt(index);
            }
            finally
            {
                this.updateFromController = false;
            }
        }

        #region IWizardViewPage Members

        /// <summary>
        /// Sets the controller to be used by the view.
        /// </summary>
        /// <param name="controller">The controller to be used by the view.</param>
        public void SetController(WizardController controller)
        {
            this.wizardController = controller;
        }

        #endregion

        /// <summary>
        /// Handles the Load event.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        private void SelectAssembliesControl_Load(object sender, EventArgs e)
        {
            this.textBoxDescription.Text = DialogResources.SelectAssembliesFormDescription;
        }

        /// <summary>
        /// Handles the VisibleChanged event.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        private void SelectAssembliesControl_VisibleChanged(object sender, EventArgs e)
        {
            if (this.Visible)
            {
                this.dataGridViewAssemblies.Focus();
            }
        }

        /// <summary>
        /// Handles the Click event.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        private void ButtonAdd_Click(object sender, EventArgs e)
        {
            if (this.openFileDialog.ShowDialog() == DialogResult.OK)
            {
                foreach (string s in this.openFileDialog.FileNames)
                {
                    if (!this.updateFromController)
                    {
                        this.wizardController.AddAssembly(s);
                    }
                }
            }
        }

        /// <summary>
        /// Handles the RowsRemoved event.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        private void DataGridViewAssemblies_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            if (!this.updateFromController)
            {
                this.wizardController.RemoveAssembly(e.RowIndex, ActionDirection.FromView);
            }
        }
    }
}
