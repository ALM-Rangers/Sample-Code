//---------------------------------------------------------------------
// <copyright file="IWizardView.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
//     THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
//     OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
//     LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR
//     FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
// <summary>The IWizardView type and related types.</summary>
//---------------------------------------------------------------------

namespace Microsoft.WcfUnit.VSIntegration
{
    /// <summary>
    /// The pages in the wizard.
    /// </summary>
    public enum WizardPage
    {
        /// <summary>
        /// The welcome page.
        /// </summary>
        Welcome,

        /// <summary>
        /// The page from which the executable to run is selected and executed.
        /// </summary>
        RunScenario,

        /// <summary>
        /// The page where the options are set.
        /// </summary>
        SetOptions,

        /// <summary>
        /// The page where the assemblies are selected.
        /// </summary>
        SelectAssemblies
    }

    /// <summary>
    /// The buttons on the wizard.
    /// </summary>
    public enum WizardButton
    {
        /// <summary>
        /// The Previous page button to move to the previous page in the wizard.
        /// </summary>
        Previous,

        /// <summary>
        /// The Next page button to move to the next page in the wizard.
        /// </summary>
        Next,

        /// <summary>
        /// The Finish button to finish the wizard.
        /// </summary>
        Finish,

        /// <summary>
        /// The Cancel button to cancel the wizard.
        /// </summary>
        Cancel,

        /// <summary>
        /// The radio button to choose the executable option.
        /// </summary>
        ChooseExecutable,

        /// <summary>
        /// The radio button to choose the trace file option.
        /// </summary>
        ChooseTraceFile,

        /// <summary>
        /// The button to run the selected executable.
        /// </summary>
        RunExecutable,

        /// <summary>
        /// The button used to browse for an executable file.
        /// </summary>
        BrowseExecutable,

        /// <summary>
        /// The button to parse the chosen trace file.
        /// </summary>
        ParseTraceFile,

        /// <summary>
        /// The button used to browse for a trace file.
        /// </summary>
        BrowseTraceFile,

        /// <summary>
        /// The radio button to choose a client-side WCF trace file.
        /// </summary>
        ChooseWcfClientTrace,

        /// <summary>
        /// The radio button to choose a server-side WCF trace file.
        /// </summary>
        ChooseWcfServerTrace,

        /// <summary>
        /// The radio button to choose a Fiddler text trace file.
        /// </summary>
        ChooseFiddlerTextTrace,

        /// <summary>
        /// The button to select all the soap actions.
        /// </summary>
        SelectAllSoapActions,

        /// <summary>
        /// The button to clear all the selected soap actions.
        /// </summary>
        ClearAllSoapActions,

        /// <summary>
        /// The button to add an assembly.
        /// </summary>
        AddAssembly
    }

    /// <summary>
    /// Controls other than buttons and user controls
    /// </summary>
    public enum WizardControl
    {
        /// <summary>
        /// Text box containing the executable file name.
        /// </summary>
        ExecutableFileName,

        /// <summary>
        /// Text box containing the trace file name.
        /// </summary>
        TraceFileName
    }

    /// <summary>
    /// Enumeration to define where an action was initiated from.
    /// </summary>
    public enum ActionDirection
    {
        /// <summary>
        /// The action came from a user gesture in the view.
        /// </summary>
        FromView,

        /// <summary>
        /// The action came from the controller.
        /// </summary>
        FromController
    }

    /// <summary>
    /// The contract presented by the view which is controlled by the wizard controller.
    /// </summary>
    public interface IWizardView : IWizardViewBase
    {
        /// <summary>
        /// Gets or sets the file name of the executable that is to be executed.
        /// </summary>
        /// <value>The file name of the executable that is to be executed.</value>
        string ExecutableFileName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the file name of the trace that is to be used.
        /// </summary>
        /// <value>The file name of the trace that is to be used.</value>
        string TraceFileName
        {
            get;
            set;
        }

        /// <summary>
        /// Sets the list of soap actions and their selection status.
        /// </summary>
        /// <param name="soapActions">The list of soap actions and their selection status.</param>
        void SetSoapActionList(WcfUnitConfigurationSoapActions soapActions);

        /// <summary>
        /// Sets the flag to indicate whether operation timers should be included.
        /// </summary>
        /// <param name="includeOperationTimers">True if operation timers are to be generated.</param>
        void SetIncludeOperationTimers(bool includeOperationTimers);

        /// <summary>
        /// Sets the flag to indicate whether individual unit tests should be generated.
        /// </summary>
        /// <param name="includeUnitTestPerOperation">True if individual unit tests are to be generated.</param>
        void SetIncludeUnitTestPerOperation(bool includeUnitTestPerOperation);

        /// <summary>
        /// Sets the list of assemblies for the proxy.
        /// </summary>
        /// <param name="assemblyList">The list of assemblies for the proxy.</param>
        void SetAssemblyList(AssemblyType[] assemblyList);

        /// <summary>
        /// Adds an assembly to the end of the list of selected assemblies.
        /// </summary>
        /// <param name="fileName">The file name of the assembly that has been selected.</param>
        void AddAssembly(string fileName);

        /// <summary>
        /// Removes an assembly from the list of selected assemblies.
        /// </summary>
        /// <param name="index">The index into the list of assemblies of the assembly that is to be deleted.</param>
        void RemoveAssembly(int index);
    }
}
