//---------------------------------------------------------------------
// <copyright file="MockWizardView.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
//     THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
//     OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
//     LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR
//     FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
// <summary>The MockWizardView type.</summary>
//---------------------------------------------------------------------

namespace VSIntegrationTest
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Microsoft.WcfUnit.Library;
    using Microsoft.WcfUnit.VSIntegration;

    internal class MockWizardView : IWizardView
    {
        private WizardController wizardController;
        private WizardPage currentPage = (WizardPage)(-1);

        private Dictionary<string, string> controlText = new Dictionary<string, string>();
        private Dictionary<string, bool> controlState = new Dictionary<string, bool>();
        private Dictionary<string, bool> pageState = new Dictionary<string, bool>();
        private Dictionary<string, bool> buttonState = new Dictionary<string, bool>();
        private Dictionary<string, bool> radioButtonState = new Dictionary<string, bool>();
        private string executableFileName = string.Empty;
        private string traceFileName = string.Empty;
        private List<AssemblyType> selectedAssemblies = new List<AssemblyType>();

        public MockWizardView()
        {
            foreach (string s in Enum.GetNames(typeof(WizardControl)))
            {
                this.controlText[s] = string.Empty;
                this.controlState[s] = false;
            }

            foreach (string s in Enum.GetNames(typeof(WizardPage)))
            {
                this.pageState[s] = false;
            }

            foreach (string s in Enum.GetNames(typeof(WizardButton)))
            {
                this.buttonState[s] = true; // simulate form written with all buttons enabled
                this.radioButtonState[s] = false; // all radio buttons disabled
            }
        }

        public string LastErrorMessage { get; set; }

        public string LastErrorMessageBoxTitle { get; set; }

        public WizardPage CurrentPage
        {
            get { return this.currentPage; }
        }

        public string ExecutableFileName
        {
            get
            {
                return this.executableFileName;
            }

            set
            {
                this.executableFileName = value;
                this.wizardController.SetExecutableFileName(value, ActionDirection.FromView);
            }
        }

        public string TraceFileName
        {
            get
            {
                return this.traceFileName;
            }

            set
            {
                this.traceFileName = value;
                this.wizardController.SetTraceFileName(value, ActionDirection.FromView);
            }
        }

        public WcfUnitConfigurationSoapActions SoapActions { get; set; }

        public bool IncludeOperationTimers { get; set; }

        public bool IncludeUnitTestPerOperation { get; set; }

        public List<AssemblyType> SelectedAssemblies
        {
            get { return this.selectedAssemblies; }
            set { this.selectedAssemblies = value; }
        }

        public void SetController(WizardController controller)
        {
            this.wizardController = controller;
        }

        public void MoveToPage(WizardPage page)
        {
            this.currentPage = page;
        }

        public void SetControlState(WizardControl control, bool enabled)
        {
            this.controlState[control.ToString()] = enabled;
        }

        public bool GetControlState(WizardControl control)
        {
            return this.controlState[control.ToString()];
        }

        public void SetPageState(WizardPage page, bool enabled)
        {
            this.pageState[page.ToString()] = enabled;
        }

        public bool GetPageState(WizardPage page)
        {
            return this.pageState[page.ToString()];
        }

        public void SetButtonState(WizardButton button, bool enabled)
        {
            this.buttonState[button.ToString()] = enabled;
        }

        public bool GetButtonState(WizardButton button)
        {
            return this.buttonState[button.ToString()];
        }

        public void SetRadioState(WizardButton button, bool check)
        {
            this.radioButtonState[button.ToString()] = check;
            if (button == WizardButton.ChooseExecutable && check)
            {
                this.wizardController.ChooseExecutableOptions(ActionDirection.FromView);
            }

            if (button == WizardButton.ChooseTraceFile && check)
            {
                this.wizardController.ChooseTraceFileOptions(ActionDirection.FromView);
            }

            if (button == WizardButton.ChooseWcfClientTrace && check)
            {
                this.wizardController.ChooseWcfClientTraceFile(ActionDirection.FromView);
            }

            if (button == WizardButton.ChooseWcfServerTrace && check)
            {
                this.wizardController.ChooseWcfServerTraceFile(ActionDirection.FromView);
            }

            if (button == WizardButton.ChooseFiddlerTextTrace && check)
            {
                this.wizardController.ChooseFiddlerTextTraceFile(ActionDirection.FromView);
            }
        }

        public bool GetRadioState(WizardButton button)
        {
            return this.radioButtonState[button.ToString()];
        }

        public void DisplayError(string title, string errorMessage)
        {
            this.LastErrorMessageBoxTitle = title;
            this.LastErrorMessage = errorMessage;
        }

        public void SetSoapActionList(WcfUnitConfigurationSoapActions soapActions)
        {
            this.SoapActions = soapActions;
        }

        public void SetIncludeOperationTimers(bool includeOperationTimers)
        {
            this.IncludeOperationTimers = includeOperationTimers;
            this.wizardController.SetOperationTimerMode(includeOperationTimers ? OperationTimerMode.IncludeOperationTimers : OperationTimerMode.NoOperationTimers, ActionDirection.FromView);
        }

        public void SetIncludeUnitTestPerOperation(bool includeUnitTestPerOperation)
        {
            this.IncludeUnitTestPerOperation = includeUnitTestPerOperation;
            this.wizardController.SetTestMethodMode(includeUnitTestPerOperation ? TestMethodMode.IncludeIndividualOperations : TestMethodMode.ScenarioMethodOnly, ActionDirection.FromView);
        }

        /// <summary>
        /// Sets the list of assemblies for the proxy.
        /// </summary>
        /// <param name="assemblyList">The list of assemblies for the proxy.</param>
        public void SetAssemblyList(AssemblyType[] assemblyList)
        {
            this.SelectedAssemblies = new List<AssemblyType>(assemblyList);
        }

        /// <summary>
        /// Adds an assembly to the end of the list of selected assemblies.
        /// </summary>
        /// <param name="selectedAssembly">The file name of the assembly that has been selected.</param>
        public void AddAssembly(string selectedAssembly)
        {
            this.SelectedAssemblies.Add(new AssemblyType(selectedAssembly));
        }

        /// <summary>
        /// Removes an assembly from the list of selected assemblies.
        /// </summary>
        /// <param name="index">The index into the list of assemblies of the assembly that is to be deleted.</param>
        public void RemoveAssembly(int index)
        {
            this.SelectedAssemblies.RemoveAt(index);
        }
    }
}
