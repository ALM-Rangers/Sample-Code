//---------------------------------------------------------------------
// <copyright file="SetOptionsControl.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
//     THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
//     OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
//     LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR
//     FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
// <summary>The SetOptionsControl type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.WcfUnit.VSIntegration
{
    using System;
    using System.Windows.Forms;

    /// <summary>
    /// The control where the soap actions are selected and the options are set.
    /// </summary>
    public partial class SetOptionsControl : UserControl, IWizardViewPage
    {
        /// <summary>
        /// The controller for this control.
        /// </summary>
        private WizardController wizardController;
        
        /// <summary>
        /// Initialises a new instance of the <see cref="SetOptionsControl"/> class.
        /// </summary>
        public SetOptionsControl()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Populates the grid with the soap actions and their selection status
        /// </summary>
        /// <param name="soapActions">The list of soap actions and their selection status.</param>
        public void SetSoapActionList(WcfUnitConfigurationSoapActions soapActions)
        {
            this.dataGridViewSoapActions.Rows.Clear();
            if (soapActions != null && soapActions.soapAction != null)
            {
                foreach (SoapActionType sat in soapActions.soapAction)
                {
                    this.dataGridViewSoapActions.Rows.Add(sat.action, sat.Selected);
                }
            }
        }

        /// <summary>
        /// Sets the flag on whether each service call gets an operation timer.
        /// </summary>
        /// <param name="includeOperationTimers">Flag on whether each service call gets an operation timer.</param>
        public void SetIncludeOperationTimers(bool includeOperationTimers)
        {
            this.checkBoxTimers.Checked = includeOperationTimers;
        }

        /// <summary>
        /// Sets the flag on whether a unit test is generated per operation.
        /// </summary>
        /// <param name="includeUnitTestPerOperation">Flag on whether a unit test is generated per operation.</param>
        public void SetIncludeUnitTestPerOperation(bool includeUnitTestPerOperation)
        {
            this.checkBoxIndividualTests.Checked = includeUnitTestPerOperation;
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
        private void SetOptionsControl_Load(object sender, EventArgs e)
        {
            this.textBoxDescription.Text = DialogResources.SetOptionsFormDescription;
        }

        /// <summary>
        /// Handles the VisibleChanged event.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        private void SetOptionsControl_VisibleChanged(object sender, EventArgs e)
        {
            if (this.Visible)
            {
                this.dataGridViewSoapActions.Focus();
            }
        }

        /// <summary>
        /// Handles the Click event.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        private void ButtonSelectAll_Click(object sender, EventArgs e)
        {
            this.wizardController.SetAllSoapActions();
        }

        /// <summary>
        /// Handles the Click event.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        private void ButtonClearAll_Click(object sender, EventArgs e)
        {
            this.wizardController.ClearAllSoapActions();
        }

        /// <summary>
        /// Handles the CheckedChanged event.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        private void CheckBoxTimers_CheckedChanged(object sender, EventArgs e)
        {
            this.wizardController.SetOperationTimerMode(this.checkBoxTimers.Checked ? OperationTimerMode.IncludeOperationTimers : OperationTimerMode.NoOperationTimers, ActionDirection.FromView);
        }

        /// <summary>
        /// Handles the CheckedChanged event.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        private void CheckBoxIndividualTests_CheckedChanged(object sender, EventArgs e)
        {
            this.wizardController.SetTestMethodMode(this.checkBoxIndividualTests.Checked ? TestMethodMode.IncludeIndividualOperations : TestMethodMode.ScenarioMethodOnly, ActionDirection.FromView);
        }

        /// <summary>
        /// Handles the CellEndEdit event.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        private void DataGridViewSoapActions_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            DataGridView v = sender as DataGridView;
            if (e.ColumnIndex == 1)
            {
                if ((bool)v.Rows[e.RowIndex].Cells[e.ColumnIndex].Value)
                {
                    this.wizardController.AddSoapAction(e.RowIndex, ActionDirection.FromView);
                }
                else
                {
                    this.wizardController.RemoveSoapAction(e.RowIndex, ActionDirection.FromView);
                }
            }
        }
    }
}
