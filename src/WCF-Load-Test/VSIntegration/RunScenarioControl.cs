//---------------------------------------------------------------------
// <copyright file="RunScenarioControl.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
//     THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
//     OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
//     LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR
//     FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
// <summary>The RunScenarioControl type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.WcfUnit.VSIntegration
{
    using System;
    using System.Windows.Forms;

    /// <summary>
    /// The control that runs the client program to exercise the scenario.
    /// </summary>
    public partial class RunScenarioControl : UserControl, IWizardViewPage
    {
        /// <summary>
        /// The controller to be used by the view.
        /// </summary>
        private WizardController wizardController;

        /// <summary>
        /// Initialises a new instance of the <see cref="RunScenarioControl"/> class.
        /// </summary>
        public RunScenarioControl()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Gets or sets the file name of the executable to be run.
        /// </summary>
        /// <value>The file name of the executable to be run.</value>
        public string ExecutableFileName
        {
            get
            {
                return this.textBoxExecutable.Text;
            }

            set
            {
                this.textBoxExecutable.Text = value;
            }
        }

        /// <summary>
        /// Gets or sets the file name of the executable to be run.
        /// </summary>
        /// <value>The file name of the executable to be run.</value>
        public string TraceFileName
        {
            get
            {
                return this.textBoxTraceFileName.Text;
            }

            set
            {
                this.textBoxTraceFileName.Text = value;
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
        /// Handler for Load event.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        private void RunScenarioControl_Load(object sender, EventArgs e)
        {
            this.textBoxDescription.Text = DialogResources.RunScenarioFormDescription;
            this.textBoxRunButtonDescription.Text = DialogResources.RunScenarioRunButtonDescription;
        }

        /// <summary>
        /// Handler for Click event.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        private void ButtonBrowse_Click(object sender, EventArgs e)
        {
            if (this.openExecutableFileDialog.ShowDialog() == DialogResult.OK)
            {
                this.textBoxExecutable.Text = this.openExecutableFileDialog.FileName; // fires event handler to update the controller
            }
        }

        /// <summary>
        /// Handler for Click event.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        private void ButtonRun_Click(object sender, EventArgs e)
        {
            this.wizardController.RunApplicationUnderTest();

            this.ParentForm.Activate(); // running the program makes us lose focus
        }

        /// <summary>
        /// Handler for Click event.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        private void ButtonParse_Click(object sender, EventArgs e)
        {
            this.wizardController.ParseTraceFile();
        }

        /// <summary>
        /// Handler for TextChanged event.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        private void TextBoxExecutable_TextChanged(object sender, EventArgs e)
        {
            this.wizardController.SetExecutableFileName(this.textBoxExecutable.Text, ActionDirection.FromView);
        }

        /// <summary>
        /// Handler for TextChanged event.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        private void TextBoxTraceFileName_TextChanged(object sender, EventArgs e)
        {
            this.wizardController.SetTraceFileName(this.textBoxTraceFileName.Text, ActionDirection.FromView);
        }

        /// <summary>
        /// Handler for VisibleChanged event.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        private void RunScenarioControl_VisibleChanged(object sender, EventArgs e)
        {
            if (this.Visible)
            {
                this.textBoxExecutable.Focus();
            }
        }

        /// <summary>
        /// Handler for CheckedChanged event.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        private void RadioButtonChooseExecutable_CheckedChanged(object sender, EventArgs e)
        {
            if (this.radioButtonChooseExecutable.Checked)
            {
                this.wizardController.ChooseExecutableOptions(ActionDirection.FromView);
            }
        }

        /// <summary>
        /// Handler for CheckedChanged event.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        private void RadioButtonChooseTraceFile_CheckedChanged(object sender, EventArgs e)
        {
            if (this.radioButtonChooseTraceFile.Checked)
            {
                this.wizardController.ChooseTraceFileOptions(ActionDirection.FromView);
            }
        }

        /// <summary>
        /// Handler for Click event.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        private void ButtonBrowseTraceFile_Click(object sender, EventArgs e)
        {
            if (this.openTraceFileDialog.ShowDialog() == DialogResult.OK)
            {
                this.textBoxTraceFileName.Text = this.openTraceFileDialog.FileName; // fires event handler to update the controller
            }
        }

        /// <summary>
        /// Handler for CheckedChanged event.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        private void RadioButtonChooseClientTrace_CheckedChanged(object sender, EventArgs e)
        {
            if (this.radioButtonChooseClientTrace.Checked)
            {
                this.wizardController.ChooseWcfClientTraceFile(ActionDirection.FromView);
            }
        }

        /// <summary>
        /// Handler for CheckedChanged event.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        private void RadioButtonChooseServerTrace_CheckedChanged(object sender, EventArgs e)
        {
            if (this.radioButtonChooseServerTrace.Checked)
            {
                this.wizardController.ChooseWcfServerTraceFile(ActionDirection.FromView);
            }
        }

        /// <summary>
        /// Handler for CheckedChanged event.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        private void RadioButtonChooseFiddlerTextTrace_CheckedChanged(object sender, EventArgs e)
        {
            if (this.radioButtonChooseFiddlerTextTrace.Checked)
            {
                this.wizardController.ChooseFiddlerTextTraceFile(ActionDirection.FromView);
            }
        }
    }
}
