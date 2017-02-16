//---------------------------------------------------------------------
// <copyright file="WizardForm.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
//     THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
//     OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
//     LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR
//     FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
// <summary>The WizardForm type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.WcfUnit.VSIntegration
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Drawing;
    using System.Windows.Forms;
    using Microsoft.WcfUnit.Library;

    /// <summary>
    /// The form that displays the wizard.
    /// </summary>
    public partial class WizardForm : Form, IWizardForm, IWizardView
    {
        /// <summary>
        /// The controller for this form.
        /// </summary>
        private WizardController wizardController;

        /// <summary>
        /// Tracks the current page.
        /// </summary>
        private WizardPage currentPage;

        /// <summary>
        /// The Welcome control.
        /// </summary>
        private WelcomeControl welcomeControl;

        /// <summary>
        /// The Run Scenario control.
        /// </summary>
        private RunScenarioControl runScenarioControl;

        /// <summary>
        /// The Set Options control.
        /// </summary>
        private SetOptionsControl setOptionsControl;

        /// <summary>
        /// The Select Assemblies control.
        /// </summary>
        private SelectAssembliesControl selectAssembliesControl;

        /// <summary>
        /// Initialises a new instance of the <see cref="WizardForm"/> class.
        /// </summary>
        public WizardForm()
        {
            this.InitializeComponent();
            this.welcomeControl = new WelcomeControl();
            this.runScenarioControl = new RunScenarioControl();
            this.setOptionsControl = new SetOptionsControl();
            this.selectAssembliesControl = new SelectAssembliesControl();
            this.panelWizardArea.Controls.Add(this.welcomeControl);
            this.panelWizardArea.Controls.Add(this.runScenarioControl);
            this.panelWizardArea.Controls.Add(this.setOptionsControl);
            this.panelWizardArea.Controls.Add(this.selectAssembliesControl);
            this.DialogResult = DialogResult.Cancel;
            this.labelVersion.Text = Utility.ReadVersion();
        }

        /// <summary>
        /// Raised when the wizard is completing, allows the completion to be cancelled.
        /// </summary>
        public event EventHandler<CancellableWizardEventArgs> WizardCompleting;

        /// <summary>
        /// Gets the current page.
        /// </summary>
        /// <value>The current page.</value>
        public WizardPage CurrentPage
        {
            get { return this.currentPage; }
        }

        /// <summary>
        /// Gets or sets the file name of the executable that is to be executed.
        /// </summary>
        /// <value>The file name of the executable that is to be executed.</value>
        public string ExecutableFileName
        {
            get
            {
                return this.runScenarioControl.ExecutableFileName;
            }

            set
            {
                this.runScenarioControl.ExecutableFileName = value;
            }
        }

        /// <summary>
        /// Gets or sets the file name of the trace that is to be used.
        /// </summary>
        /// <value>The file name of the trace that is to be used.</value>
        public string TraceFileName
        {
            get
            {
                return this.runScenarioControl.TraceFileName;
            }

            set
            {
                this.runScenarioControl.TraceFileName = value;
            }
        }

        #region IWizardForm Members

        /// <summary>
        /// Gets the data gathered by the wizard.
        /// </summary>
        /// <value>The data gathered by the wizard.</value>
        public WizardData WizardData
        {
            get { return this.wizardController.FilteredWizardData; }
        }

        /// <summary>
        /// Runs the wizard
        /// </summary>
        /// <returns>True if the wizard completed, false if it was cancelled.</returns>
        public bool RunWizard()
        {
            DialogResult result = this.ShowDialog();
            return result == DialogResult.OK;
        }

        #endregion

        #region IWizardViewPage Members

        /// <summary>
        /// Sets the controller to be used by the view.
        /// </summary>
        /// <param name="controller">The controller to be used by the view.</param>
        public void SetController(WizardController controller)
        {
            this.wizardController = controller;
            this.runScenarioControl.SetController(this.wizardController);
            this.setOptionsControl.SetController(this.wizardController);
            this.selectAssembliesControl.SetController(this.wizardController);
        }

        #endregion

        #region IWizardView Members
        /// <summary>
        /// Sets the list of soap actions and their selection status.
        /// </summary>
        /// <param name="soapActions">The list of soap actions and their selection status.</param>
        public void SetSoapActionList(WcfUnitConfigurationSoapActions soapActions)
        {
            this.setOptionsControl.SetSoapActionList(soapActions);
        }

        /// <summary>
        /// Sets the flag to indicate whether operation timers should be included.
        /// </summary>
        /// <param name="includeOperationTimers">True if operation timers are to be generated.</param>
        public void SetIncludeOperationTimers(bool includeOperationTimers)
        {
            this.setOptionsControl.SetIncludeOperationTimers(includeOperationTimers);
        }

        /// <summary>
        /// Sets the flag to indicate whether individual unit tests should be generated.
        /// </summary>
        /// <param name="includeUnitTestPerOperation">True if individual unit tests are to be generated.</param>
        public void SetIncludeUnitTestPerOperation(bool includeUnitTestPerOperation)
        {
            this.setOptionsControl.SetIncludeUnitTestPerOperation(includeUnitTestPerOperation);
        }

        /// <summary>
        /// Sets the list of assemblies for the proxy.
        /// </summary>
        /// <param name="assemblyList">The list of assemblies for the proxy.</param>
        public void SetAssemblyList(AssemblyType[] assemblyList)
        {
            this.selectAssembliesControl.SetAssemblyList(assemblyList);
        }

        /// <summary>
        /// Adds an assembly to the end of the list of selected assemblies.
        /// </summary>
        /// <param name="fileName">The file name of the assembly that has been selected.</param>
        public void AddAssembly(string fileName)
        {
            this.selectAssembliesControl.AddAssembly(fileName);
        }

        /// <summary>
        /// Removes an assembly from the list of selected assemblies.
        /// </summary>
        /// <param name="index">The index into the list of assemblies of the assembly that is to be deleted.</param>
        public void RemoveAssembly(int index)
        {
            this.selectAssembliesControl.RemoveAssembly(index);
        }

        #endregion

        #region IWizardViewBase Members

        /// <summary>
        /// Moves the wizard to nominated page.
        /// </summary>
        /// <param name="page">The page that the wizard must move to.</param>
        public void MoveToPage(WizardPage page)
        {
            string tag = GetPageTag(page);
            foreach (Control c in this.Controls)
            {
                if (c is LinkLabel)
                {
                    if ((string)c.Tag == tag)
                    {
                        c.BackColor = Color.FromKnownColor(KnownColor.ControlDark);
                    }
                    else
                    {
                        c.BackColor = Color.FromKnownColor(KnownColor.ControlLightLight);
                    }
                }
            }

            foreach (Control c in this.panelWizardArea.Controls)
            {
                c.Visible = (string)c.Tag == tag;
            }

            switch (page)
            {
                case WizardPage.Welcome:
                    {
                        this.textBoxWizardPageSummary.Text = DialogResources.WelcomeSummary;
                        break;
                    }

                case WizardPage.RunScenario:
                    {
                        this.textBoxWizardPageSummary.Text = DialogResources.RunScenarioSummary;
                        break;
                    }

                case WizardPage.SetOptions:
                    {
                        this.textBoxWizardPageSummary.Text = DialogResources.SetOptionsSummary;
                        break;
                    }

                case WizardPage.SelectAssemblies:
                    {
                        this.textBoxWizardPageSummary.Text = DialogResources.SelectAssembliesSummary;
                        break;
                    }
            }

            this.currentPage = page;
        }

        /// <summary>
        /// Sets the control to be enabled or not.
        /// </summary>
        /// <param name="control">The control for which the status is to be set.</param>
        /// <param name="enabled">The enabled/disabled status to set.</param>
        public void SetControlState(WizardControl control, bool enabled)
        {
            Control c = this.FindControl(control);
            Debug.Assert(c != null, "Could not find the control");
            c.Enabled = enabled;
        }

        /// <summary>
        /// Gets the enabled/disabled state of a control.
        /// </summary>
        /// <param name="control">The control to get the state of.</param>
        /// <returns>True if the control is enabled, false otherwise.</returns>
        public bool GetControlState(WizardControl control)
        {
            Control c = this.FindControl(control);
            Debug.Assert(c != null, "Could not find the control");
            return c.Enabled;
        }

        /// <summary>
        /// Sets the status of a page.
        /// </summary>
        /// <param name="page">The page to set the status of.</param>
        /// <param name="enabled">True to enable the page, false otherwise.</param>
        public void SetPageState(WizardPage page, bool enabled)
        {
            Control c = this.FindLinkForPage(page);
            Debug.Assert(c != null, "Could not find the control");
            c.Enabled = enabled;
        }

        /// <summary>
        /// Returns status of a page.
        /// </summary>
        /// <param name="page">The page to get the status of.</param>
        /// <returns>True if the page is enabled, false otherwise.</returns>
        public bool GetPageState(WizardPage page)
        {
            Control c = this.FindLinkForPage(page);
            Debug.Assert(c != null, "Could not find the control");
            return c.Enabled;
        }

        /// <summary>
        /// Enables or disables a button.
        /// </summary>
        /// <param name="button">The button to set the status of.</param>
        /// <param name="enabled">True to enable the button, false otherwise.</param>
        public void SetButtonState(WizardButton button, bool enabled)
        {
            Control c = this.FindButton(button);
            Debug.Assert(c != null, "Could not find the control");
            c.Enabled = enabled;
        }

        /// <summary>
        /// Returns the enabled/disabled status of a button.
        /// </summary>
        /// <param name="button">The button to get the status of.</param>
        /// <returns>True if the button is enabled, false otherwise.</returns>
        public bool GetButtonState(WizardButton button)
        {
            Control c = this.FindButton(button);
            Debug.Assert(c != null, "Could not find the control");
            return c.Enabled;
        }

        /// <summary>
        /// Sets the wizard radio button to be checked or not
        /// </summary>
        /// <param name="button">The button for which the checked value is to be set.</param>
        /// <param name="check">The checked/unchecked status to set.</param>
        public void SetRadioState(WizardButton button, bool check)
        {
            RadioButton rb = this.FindButton(button) as RadioButton;
            Debug.Assert(rb != null, "Could not find the control");
            rb.Checked = check;
        }

        /// <summary>
        /// Gets the checked/unchecked state of a radio button
        /// </summary>
        /// <param name="button">The radio button to get the checked/unchecked of.</param>
        /// <returns>True if the radio button is checked, false otherwise.</returns>
        public bool GetRadioState(WizardButton button)
        {
            RadioButton rb = this.FindButton(button) as RadioButton;
            Debug.Assert(rb != null, "Could not find the control");
            return rb.Checked;
        }

        /// <summary>
        /// Displays an error to the user in a message box.
        /// </summary>
        /// <param name="title">The title of the message box.</param>
        /// <param name="errorMessage">The message to display.</param>
        public void DisplayError(string title, string errorMessage)
        {
            MessageBox.Show(errorMessage, title, MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, (MessageBoxOptions)0);
        }

        #endregion

        /// <summary>
        /// Raises the <see cref="WizardCompleting"/> event.
        /// </summary>
        /// <param name="e">Cancellable event object.</param>
        protected override void OnClosing(CancelEventArgs e)
        {
            if (e == null)
            {
                throw new ArgumentNullException("e");
            }

            base.OnClosing(e);
            if (this.DialogResult == DialogResult.OK)
            {
                if (this.WizardCompleting != null)
                {
                    CancellableWizardEventArgs cw = new CancellableWizardEventArgs(this.wizardController.FilteredWizardData, e.Cancel);
                    this.WizardCompleting(this, cw);
                    e.Cancel = cw.Cancel;
                }
            }
        }

        /// <summary>
        /// Gets the tag for a page.
        /// </summary>
        /// <param name="page">The page to get the tag for.</param>
        /// <returns>The tag for the page.</returns>
        private static string GetPageTag(WizardPage page)
        {
            string tag = page.ToString();
            return tag;
        }

        /// <summary>
        /// Gets the tag for a button.
        /// </summary>
        /// <param name="button">The button to get the tag for.</param>
        /// <returns>The tag for the button.</returns>
        private static string GetButtonTag(WizardButton button)
        {
            string tag = button.ToString();
            return tag;
        }

        /// <summary>
        /// Gets the tag for a control.
        /// </summary>
        /// <param name="control">The control to get the tag for.</param>
        /// <returns>The tag for the control.</returns>
        private static string GetControlTag(WizardControl control)
        {
            string tag = control.ToString();
            return tag;
        }

        /// <summary>
        /// Finds a link for a page.
        /// </summary>
        /// <param name="page">The page to find the link for.</param>
        /// <returns>The control that is the link to the page.</returns>
        private Control FindLinkForPage(WizardPage page)
        {
            Control ans = null;
            string tag = GetPageTag(page);
            foreach (Control c in this.Controls)
            {
                if (c is LinkLabel && (string)c.Tag == tag)
                {
                    ans = c;
                    break;
                }
            }

            return ans;
        }

        /// <summary>
        /// Finds a button.
        /// </summary>
        /// <param name="button">The button to find.</param>
        /// <returns>The button.</returns>
        private Control FindButton(WizardButton button)
        {
            string tag = GetButtonTag(button);
            return this.FindControl(this, tag);
        }

        /// <summary>
        /// Finds a control.
        /// </summary>
        /// <param name="control">The control to find.</param>
        /// <returns>The control.</returns>
        private Control FindControl(WizardControl control)
        {
            string tag = GetControlTag(control);
            return this.FindControl(this, tag);
        }

        /// <summary>
        /// Finds a control with a given tag.
        /// </summary>
        /// <param name="container">The control container to search.</param>
        /// <param name="tag">The tag being searched for.</param>
        /// <returns>The control, <c>null</c> if not found.</returns>
        private Control FindControl(Control container, string tag)
        {
            Control ans = null;
            foreach (Control c in container.Controls)
            {
                if ((string)c.Tag == tag)
                {
                    ans = c;
                }
                else
                {
                    ans = this.FindControl(c, tag);
                }

                if (ans != null)
                {
                    break;
                }
            }

            return ans;
        }

        /// <summary>
        /// Handler for Click event.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        private void ButtonNext_Click(object sender, EventArgs e)
        {
            this.wizardController.Next();
        }

        /// <summary>
        /// Handler for Click event.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        private void ButtonPrevious_Click(object sender, EventArgs e)
        {
            this.wizardController.Previous();
        }

        /// <summary>
        /// Handler for LinkClicked event.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        private void LinkWelcome_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.wizardController.MoveToPage(WizardPage.Welcome);
        }

        /// <summary>
        /// Handler for LinkClicked event.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        private void LinkLabelRunScenario_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.wizardController.MoveToPage(WizardPage.RunScenario);
        }

        /// <summary>
        /// Handler for LinkClicked event.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        private void LinkLabelSetOptions_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.wizardController.MoveToPage(WizardPage.SetOptions);
        }

        /// <summary>
        /// Handler for LinkClicked event.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        private void SelectAssemblies_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.wizardController.MoveToPage(WizardPage.SelectAssemblies);
        }

        /// <summary>
        /// Handler for Click event.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        private void ButtonFinish_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        /// <summary>
        /// Handler for Click event.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        private void ButtonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}