//---------------------------------------------------------------------
// <copyright file="WizardController.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
//     THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
//     OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
//     LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR
//     FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
// <summary>The WizardController type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.WcfUnit.VSIntegration
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Microsoft.WcfUnit.Library;

    /// <summary>
    /// Controller for the Wizard.
    /// </summary>
    public class WizardController
    {
        /// <summary>
        /// The view that is managed by this controller.
        /// </summary>
        private IWizardView view;

        /// <summary>
        /// The scenario run manager
        /// </summary>
        private IScenarioRunManager scenarioRunManager;

        /// <summary>
        /// The UI logic interface to be used.
        /// </summary>
        private IUILogic uiLogic;

        /// <summary>
        /// The data that is returned after the wizard has been completed.
        /// </summary>
        private WizardData wizardData;

        /// <summary>
        /// Initialises a new instance of the <see cref="WizardController"/> class.
        /// </summary>
        /// <param name="view">The view to be controlled by the controller.</param>
        /// <param name="scenarioRunManager">The manager used to control the scenario.</param>
        /// <param name="uiLogic">The UI logic object to use to run scenarios and parse trace files.</param>
        public WizardController(IWizardView view, IScenarioRunManager scenarioRunManager, IUILogic uiLogic)
        {
            this.view = view;
            this.scenarioRunManager = scenarioRunManager;
            this.uiLogic = uiLogic;
        }

        /// <summary>
        /// An action to take against the view, the action is expected to re-invoke the controller with a
        /// <see cref="ActionDirection.FromView"/> action direction.
        /// </summary>
        private delegate void Action();

        /// <summary>
        /// The type of trace.
        /// </summary>
        private enum TraceType
        {
            /// <summary>
            /// WCF client side trace.
            /// </summary>
            WcfClient,

            /// <summary>
            /// WCF server side trace.
            /// </summary>
            WcfServer,

            /// <summary>
            /// Fiddler trace.
            /// </summary>
            FiddlerText
        }

        /// <summary>
        /// Gets the <see cref="WizardData"/> that has been gathered by the wizard, including unselected soap actions.
        /// </summary>
        /// <value>
        /// The <see cref="WizardData"/> that has been gathered by the wizard, including unselected soap actions.
        /// </value>
        public WizardData WizardData
        {
            get
            {
                return this.wizardData;
            }
        }

        /// <summary>
        /// Gets the <see cref="WizardData"/> that has been gathered by the wizard, including only selected soap actions.
        /// </summary>
        /// <value>
        /// The <see cref="WizardData"/> that has been gathered by the wizard, including only selected soap actions.
        /// </value>
        public WizardData FilteredWizardData
        {
            get
            {
                List<SoapActionType> filteredSoapActions = new List<SoapActionType>();
                if (this.wizardData.Configuration.soapActions != null)
                {
                    foreach (SoapActionType sat in this.wizardData.Configuration.soapActions.soapAction)
                    {
                        if (sat.Selected)
                        {
                            filteredSoapActions.Add(sat);
                        }
                    }
                }

                WizardData ans = new WizardData();
                ans.TraceFile = this.wizardData.TraceFile;
                ans.TraceFileSource = this.wizardData.TraceFileSource;
                ans.Configuration = this.wizardData.Configuration.Clone();
                if (ans.Configuration.soapActions != null)
                {
                    ans.Configuration.soapActions.soapAction = filteredSoapActions.ToArray();
                }

                return ans;
            }
        }

        /// <summary>
        /// Initializes the view.
        /// </summary>
        public void Initialize()
        {
            this.InitializeConfigurationData();
            this.view.SetPageState(WizardPage.Welcome, true);
            this.MoveToPage(WizardPage.Welcome);
            this.SetTraceFileTypeState(TraceType.WcfClient);
            this.SetTraceFileSource(ActionDirection.FromController, TraceFileSource.Dynamic);
            this.SetNavigationControls();
        }

        /// <summary>
        /// Moves to the next wizard page.
        /// </summary>
        public void Next()
        {
            this.CheckButtonState(WizardButton.Next);
            this.MoveToPage(this.view.CurrentPage + 1);
        }

        /// <summary>
        /// Moves to the previous wizard page.
        /// </summary>
        public void Previous()
        {
            this.CheckButtonState(WizardButton.Previous);
            this.MoveToPage(this.view.CurrentPage - 1);
        }

        /// <summary>
        /// Moves to a specific page in the wizard.
        /// </summary>
        /// <param name="page">The page to move to.</param>
        public void MoveToPage(WizardPage page)
        {
            this.CheckPageState(page);
            this.view.MoveToPage(page);
            this.SetNavigationControls();
        }

        /// <summary>
        /// Chooses to run an executable to capture a client-side trace.
        /// </summary>
        /// <param name="direction">Used to tell the controller whether the view needs to be updated or not, to avoid recursive loops.</param>
        public void ChooseExecutableOptions(ActionDirection direction)
        {
            this.ResetConfigurationData();
            this.SetTraceFileSource(direction, TraceFileSource.Dynamic);
            this.SetNavigationControls();
        }

        /// <summary>
        /// Chooses a pre-existing trace file for processing.
        /// </summary>
        /// <param name="direction">Used to tell the controller whether the view needs to be updated or not, to avoid recursive loops.</param>
        public void ChooseTraceFileOptions(ActionDirection direction)
        {
            this.ResetConfigurationData();
            this.SetTraceFileSource(direction, TraceFileSource.PreCaptured);
            this.SetNavigationControls();
        }

        /// <summary>
        /// Sets the name of the file to be executed.
        /// </summary>
        /// <param name="executableFileName">The name of the file to be executed</param>
        /// <param name="direction">Used to tell the controller whether the view needs to be updated or not, to avoid recursive loops.</param>
        public void SetExecutableFileName(string executableFileName, ActionDirection direction)
        {
            if (direction == ActionDirection.FromController)
            {
                this.view.ExecutableFileName = executableFileName;
            }

            this.view.SetButtonState(WizardButton.RunExecutable, !string.IsNullOrEmpty(executableFileName));
            this.ResetConfigurationData();
            this.SetNavigationControls();
        }

        /// <summary>
        /// Sets the name of the trace file to be processed.
        /// </summary>
        /// <param name="traceFileName">The name of the trace file to be processed</param>
        /// <param name="direction">Used to tell the controller whether the view needs to be updated or not, to avoid recursive loops.</param>
        public void SetTraceFileName(string traceFileName, ActionDirection direction)
        {
            this.wizardData.TraceFile = traceFileName;
            if (direction == ActionDirection.FromController)
            {
                this.view.TraceFileName = traceFileName;
            }

            this.view.SetButtonState(WizardButton.ParseTraceFile, !string.IsNullOrEmpty(traceFileName));
            this.ResetConfigurationData();
            this.SetNavigationControls();
        }

        /// <summary>
        /// Chooses the option that the trace file is a WCF client-side trace file.
        /// </summary>
        /// <param name="direction">Used to tell the controller whether the view needs to be updated or not, to avoid recursive loops.</param>
        public void ChooseWcfClientTraceFile(ActionDirection direction)
        {
            this.SetTraceFileTypeStateOnTheWizardData(TraceType.WcfClient);
            if (direction == ActionDirection.FromController)
            {
                this.SetTraceFileTypeStateOnTheView(TraceType.WcfClient);
            }
        }

        /// <summary>
        /// Chooses the option that the trace file is a WCF server-side trace file.
        /// </summary>
        /// <param name="direction">Used to tell the controller whether the view needs to be updated or not, to avoid recursive loops.</param>
        public void ChooseWcfServerTraceFile(ActionDirection direction)
        {
            this.SetTraceFileTypeStateOnTheWizardData(TraceType.WcfServer);
            if (direction == ActionDirection.FromController)
            {
                this.SetTraceFileTypeStateOnTheView(TraceType.WcfServer);
            }
        }

        /// <summary>
        /// Chooses the option that the trace file is a Fiddler text trace file.
        /// </summary>
        /// <param name="direction">Used to tell the controller whether the view needs to be updated or not, to avoid recursive loops.</param>
        public void ChooseFiddlerTextTraceFile(ActionDirection direction)
        {
            this.SetTraceFileTypeStateOnTheWizardData(TraceType.FiddlerText);
            if (direction == ActionDirection.FromController)
            {
                this.SetTraceFileTypeStateOnTheView(TraceType.FiddlerText);
            }
        }

        /// <summary>
        /// Runs the application under test.
        /// </summary>
        /// <remarks>
        /// Displays the error if an error occurred, or if there were no SOAP actions detected.
        /// </remarks>
        /// <returns>True if the run succeeded and at least one SOAP action detected, false otherwise.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031", Justification = "We really want to show a message for anything that may occur")]
        public bool RunApplicationUnderTest()
        {
            this.CheckButtonState(WizardButton.RunExecutable);

            bool success = false;
            this.ResetConfigurationData();
            try
            {
                this.uiLogic.RunProgramAndGetWizardData(this.view.ExecutableFileName, this.scenarioRunManager, this.wizardData);
                success = true;
            }
            catch (Exception ex)
            {
                this.view.DisplayError(ErrorMessages.ProgramSetupAndExecutionErrorMessageBoxTitle, ex.Message);
            }

            if (success && this.HaveNoSoapActions())
            {
                success = false;
                this.DisplayNoSoapActionsError();
            }

            if (success)
            {
                this.SetSoapActionList(ActionDirection.FromController);
                this.SetAssemblyList();
            }

            this.SetNavigationControls();
            return success;
        }

        /// <summary>
        /// Parses the selected trace file.
        /// </summary>
        /// <remarks>
        /// Displays the error if an error occurred, or if there were no SOAP actions detected.
        /// </remarks>
        /// <returns>True if the run succeeded and at least one SOAP action detected, false otherwise.</returns>
        public bool ParseTraceFile()
        {
            this.CheckButtonState(WizardButton.ParseTraceFile);

            bool success = false;
            this.ResetConfigurationData();
            try
            {
                this.uiLogic.ParseTraceFile(this.view.TraceFileName, this.wizardData.Configuration);
                success = true;
            }
            catch (UserException ex)
            {
                this.view.DisplayError(ErrorMessages.TraceFileParseErrorMessageBoxTitle, ex.Message);
            }

            if (success && this.HaveNoSoapActions())
            {
                success = false;
                this.DisplayNoSoapActionsError();
            }

            if (success)
            {
                this.SetSoapActionList(ActionDirection.FromController);
                this.SetAssemblyList();
            }

            this.SetNavigationControls();
            return success;
        }

        /// <summary>
        /// Unselects all the selected SOAP actions.
        /// </summary>
        public void ClearAllSoapActions()
        {
            this.SetAllSoapActionsSelectionState(false);
        }

        /// <summary>
        /// Removes the selected SOAP action from the selected actions and makes it unselected.
        /// </summary>
        /// <param name="index">Index in <see cref="IWizardView.SoapActions"/></param>
        /// <param name="direction">Used to tell the controller whether the view needs to be updated or not, to avoid recursive loops.</param>
        public void RemoveSoapAction(int index, ActionDirection direction)
        {
            // TODO: consider making the interface to the view have a set state method for soap actions, rather than re-assign the list each time.
            this.wizardData.Configuration.soapActions.soapAction[index].Selected = false;
            this.SetSoapActionList(direction);
        }

        /// <summary>
        /// Adds the unselected SOAP action and makes it selected.
        /// </summary>
        /// <param name="index">Index in <see cref="IWizardView.UnelectedSoapActions"/></param>
        /// <param name="direction">Used to tell the controller whether the view needs to be updated or not, to avoid recursive loops.</param>
        public void AddSoapAction(int index, ActionDirection direction)
        {
            this.wizardData.Configuration.soapActions.soapAction[index].Selected = true;
            this.SetSoapActionList(direction);
        }

        /// <summary>
        /// Sets the operation timer mode.
        /// </summary>
        /// <param name="operationTimerMode">The timer mode.</param>
        /// <param name="direction">Indicates where the set came from.</param>
        public void SetOperationTimerMode(OperationTimerMode operationTimerMode, ActionDirection direction)
        {
            if (direction == ActionDirection.FromController)
            {
                this.view.SetIncludeOperationTimers(operationTimerMode == OperationTimerMode.IncludeOperationTimers);
            }

            this.wizardData.Configuration.operationTimerMode = operationTimerMode;
        }

        /// <summary>
        /// Sets the test method.
        /// </summary>
        /// <param name="testMethodMode">The test method.</param>
        /// <param name="direction">Indicates where the set came from.</param>
        public void SetTestMethodMode(TestMethodMode testMethodMode, ActionDirection direction)
        {
            if (direction == ActionDirection.FromController)
            {
                this.view.SetIncludeUnitTestPerOperation(testMethodMode == TestMethodMode.IncludeIndividualOperations);
            }

            this.wizardData.Configuration.testMethodMode = testMethodMode;
        }

        /// <summary>
        /// Adds an assembly file to the list of selected assemblies.
        /// </summary>
        /// <param name="fileName">The assembly file name to add</param>
        public void AddAssembly(string fileName)
        {
            this.wizardData.Configuration.AddAssembly(fileName);
            this.view.AddAssembly(fileName);
            this.SetNavigationControls();
        }

        /// <summary>
        /// Removes the selected assembly from the list of assemblies.
        /// </summary>
        /// <param name="index">Index in <see cref="IWizardView.SelectedSoapActions"/></param>
        /// <param name="direction">Used to tell the controller whether the view needs to be updated or not, to avoid recursive loops.</param>
        public void RemoveAssembly(int index, ActionDirection direction)
        {
            this.wizardData.Configuration.RemoveAssembly(index);
            if (direction == ActionDirection.FromController)
            {
                this.view.RemoveAssembly(index);
            }

            this.SetNavigationControls();
        }

        /// <summary>
        /// Sets all the unselected SOAP actions as selected.
        /// </summary>
        public void SetAllSoapActions()
        {
            this.SetAllSoapActionsSelectionState(true);
        }

        /// <summary>
        /// Sets the trace type state.
        /// </summary>
        /// <param name="traceType">The trace type.</param>
        private void SetTraceFileTypeState(TraceType traceType)
        {
            this.SetTraceFileTypeStateOnTheWizardData(traceType);
            this.SetTraceFileTypeStateOnTheView(traceType);
        }

        /// <summary>
        /// Sets the trace type state in the view.
        /// </summary>
        /// <param name="traceType">The trace type.</param>
        private void SetTraceFileTypeStateOnTheView(TraceType traceType)
        {
            this.view.SetRadioState(WizardButton.ChooseWcfClientTrace, traceType == TraceType.WcfClient);
            this.view.SetRadioState(WizardButton.ChooseWcfServerTrace, traceType == TraceType.WcfServer);
            this.view.SetRadioState(WizardButton.ChooseFiddlerTextTrace, traceType == TraceType.FiddlerText);
        }

        /// <summary>
        /// Sets the trace type state in the wizard data.
        /// </summary>
        /// <param name="traceType">The trace type.</param>
        private void SetTraceFileTypeStateOnTheWizardData(TraceType traceType)
        {
            this.wizardData.Configuration.clientTrace = traceType == TraceType.WcfClient;
            this.wizardData.Configuration.serviceTrace = traceType == TraceType.WcfServer;
            if (traceType == TraceType.FiddlerText)
            {
                this.wizardData.Configuration.parser = new typeType();
                this.wizardData.Configuration.parser.assembly = typeof(FiddlerTextParser).Assembly.FullName;
                this.wizardData.Configuration.parser.type = typeof(FiddlerTextParser).FullName;
            }
            else
            {
                this.wizardData.Configuration.parser = null;
            }
        }

        /// <summary>
        /// Sets the trace file source.
        /// </summary>
        /// <param name="direction">Where the action initiated.</param>
        /// <param name="source">The source.</param>
        private void SetTraceFileSource(ActionDirection direction, TraceFileSource source)
        {
            this.wizardData.TraceFileSource = source;
            if (source == TraceFileSource.Dynamic)
            {
                this.view.SetControlState(WizardControl.ExecutableFileName, true);
                this.view.SetButtonState(WizardButton.BrowseExecutable, true);
                this.view.SetControlState(WizardControl.TraceFileName, false);
                this.view.SetButtonState(WizardButton.BrowseTraceFile, false);
                this.view.SetButtonState(WizardButton.RunExecutable, !string.IsNullOrEmpty(this.view.ExecutableFileName));
                this.view.SetButtonState(WizardButton.ParseTraceFile, false);

                if (direction == ActionDirection.FromController)
                {
                    this.view.SetRadioState(WizardButton.ChooseTraceFile, false);
                    this.view.SetRadioState(WizardButton.ChooseExecutable, true);
                }

                this.view.SetButtonState(WizardButton.ChooseWcfClientTrace, false);
                this.view.SetButtonState(WizardButton.ChooseWcfServerTrace, false);
                this.view.SetButtonState(WizardButton.ChooseFiddlerTextTrace, false);
            }
            else
            {
                this.view.SetControlState(WizardControl.ExecutableFileName, false);
                this.view.SetButtonState(WizardButton.BrowseExecutable, false);
                this.view.SetControlState(WizardControl.TraceFileName, true);
                this.view.SetButtonState(WizardButton.BrowseTraceFile, true);
                this.view.SetButtonState(WizardButton.RunExecutable, false);
                this.view.SetButtonState(WizardButton.ParseTraceFile, !string.IsNullOrEmpty(this.view.TraceFileName));

                if (direction == ActionDirection.FromController)
                {
                    this.view.SetRadioState(WizardButton.ChooseExecutable, false);
                    this.view.SetRadioState(WizardButton.ChooseTraceFile, true);
                }

                this.view.SetButtonState(WizardButton.ChooseWcfClientTrace, true);
                this.view.SetButtonState(WizardButton.ChooseWcfServerTrace, true);
                this.view.SetButtonState(WizardButton.ChooseFiddlerTextTrace, true);
            }
        }

        /// <summary>
        /// Displays error that there are no Soap actions.
        /// </summary>
        private void DisplayNoSoapActionsError()
        {
            this.view.DisplayError(ErrorMessages.NoSoapActionsMessageBoxTitle, ErrorMessages.NoSoapActions);
        }

        /// <summary>
        /// Returns an indication of whether there are any Soap actions or not.
        /// </summary>
        /// <returns><c>True</c> if there are no Soap actions, <c>false</c> otherwise.</returns>
        private bool HaveNoSoapActions()
        {
            return this.wizardData.Configuration.soapActions.soapAction.Length <= 0;
        }

        /// <summary>
        /// Initializes the configuration data.
        /// </summary>
        private void InitializeConfigurationData()
        {
            this.wizardData = new WizardData();
            this.wizardData.TraceFileSource = TraceFileSource.Dynamic;
            this.wizardData.Configuration = new WcfUnitConfiguration();
            this.SetOperationTimerMode(OperationTimerMode.IncludeOperationTimers, ActionDirection.FromController);
            this.SetTestMethodMode(TestMethodMode.ScenarioMethodOnly, ActionDirection.FromController);
        }

        /// <summary>
        /// Resets the configuration data.
        /// </summary>
        private void ResetConfigurationData()
        {
            this.wizardData.Configuration.soapActions = null;
            this.wizardData.Configuration.assembly = null;
        }

        /// <summary>
        /// Sets the wizard navigation controls.
        /// </summary>
        private void SetNavigationControls()
        {
            bool haveSoapActions = this.wizardData != null
                                   &&
                                   this.wizardData.Configuration != null
                                   &&
                                   this.wizardData.Configuration.soapActions != null
                                   &&
                                   this.wizardData.Configuration.soapActions.soapAction != null
                                   &&
                                   this.wizardData.Configuration.soapActions.soapAction.Length > 0;

            bool haveSelectedSoapActions = haveSoapActions && this.wizardData.Configuration.soapActions.SelectedCount > 0;

            bool haveSelectedAssemblies = this.wizardData != null
                                          &&
                                          this.wizardData.Configuration != null
                                          &&
                                          this.wizardData.Configuration.assembly != null
                                          &&
                                          this.wizardData.Configuration.assembly.Length > 0;

            this.view.SetPageState(WizardPage.Welcome, true);
            this.view.SetPageState(WizardPage.RunScenario, true);
            this.view.SetPageState(WizardPage.SetOptions, haveSoapActions);
            this.view.SetPageState(WizardPage.SelectAssemblies, haveSoapActions && haveSelectedSoapActions);

            this.view.SetButtonState(WizardButton.Cancel, true);
            this.view.SetButtonState(WizardButton.Finish, haveSoapActions && haveSelectedSoapActions && haveSelectedAssemblies);
            switch (this.view.CurrentPage)
            {
                case WizardPage.Welcome:
                    {
                        this.view.SetButtonState(WizardButton.Previous, false);
                        this.view.SetButtonState(WizardButton.Next, true);
                        break;
                    }

                case WizardPage.RunScenario:
                    {
                        this.view.SetButtonState(WizardButton.Previous, true);
                        this.view.SetButtonState(WizardButton.Next, haveSoapActions);
                        break;
                    }

                case WizardPage.SetOptions:
                    {
                        this.view.SetButtonState(WizardButton.Previous, true);
                        this.view.SetButtonState(WizardButton.Next, haveSoapActions && haveSelectedSoapActions);
                        break;
                    }

                case WizardPage.SelectAssemblies:
                    {
                        this.view.SetButtonState(WizardButton.Previous, true);
                        this.view.SetButtonState(WizardButton.Next, false);
                        break;
                    }

                default:
                    {
                        System.Diagnostics.Debug.Assert(false, "Unrecognised page");
                        break;
                    }
            }
        }

        /// <summary>
        /// Checks the state of a button.
        /// </summary>
        /// <param name="button">The button to check.</param>
        private void CheckButtonState(WizardButton button)
        {
            if (!this.view.GetButtonState(button))
            {
                throw new InvalidOperationException(ErrorMessages.ButtonStateInconsistency);
            }
        }

        /// <summary>
        /// Checks the state of a wizard page.
        /// </summary>
        /// <param name="page">The page to check.</param>
        private void CheckPageState(WizardPage page)
        {
            if (!this.view.GetPageState(page))
            {
                throw new InvalidOperationException(ErrorMessages.PageStateInconsistency);
            }
        }

        /// <summary>
        /// Sets the selection state for all the Soap actions.
        /// </summary>
        /// <param name="selectionState">The selection state to be set.</param>
        private void SetAllSoapActionsSelectionState(bool selectionState)
        {
            foreach (SoapActionType sat in this.wizardData.Configuration.soapActions.soapAction)
            {
                sat.Selected = selectionState;
            }

            this.SetSoapActionList(ActionDirection.FromController);
        }

        /// <summary>
        /// Sets the Soap action list.
        /// </summary>
        /// <param name="direction">Where the action initiated.</param>
        private void SetSoapActionList(ActionDirection direction)
        {
            if (direction == ActionDirection.FromController)
            {
                this.view.SetSoapActionList(this.wizardData.Configuration.soapActions);
            }

            this.SetSoapActionSelectionButtons();
            this.SetNavigationControls();
        }

        /// <summary>
        /// Sets the list of assemblies.
        /// </summary>
        private void SetAssemblyList()
        {
            this.view.SetAssemblyList(new AssemblyType[0]);
            this.wizardData.Configuration.assembly = new AssemblyType[0];
            this.SetNavigationControls();
        }

        /// <summary>
        /// Sets the state of the soap action selection button.
        /// </summary>
        private void SetSoapActionSelectionButtons()
        {
            this.view.SetButtonState(WizardButton.SelectAllSoapActions, this.wizardData.Configuration.soapActions.UnselectedCount > 0);
            this.view.SetButtonState(WizardButton.ClearAllSoapActions, this.wizardData.Configuration.soapActions.SelectedCount > 0);
        }
    }
}
