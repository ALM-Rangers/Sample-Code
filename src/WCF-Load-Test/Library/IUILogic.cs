//---------------------------------------------------------------------
// <copyright file="IUILogic.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
//     THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
//     OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
//     LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR
//     FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
// <summary>The IUILogic type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.WcfUnit.Library
{
    /// <summary>
    /// Encapsulates the logic for using a scenario run manager or parsing a trace.
    /// </summary>
    public interface IUILogic
    {
        /// <summary>
        /// Gets the configuration from a trace file
        /// </summary>
        /// <param name="fileName">The name of the file to parse.</param>
        /// <param name="config">Tells this method what type of trace file it is, the configuration is updated with the soap actions.</param>
        void ParseTraceFile(string fileName, WcfUnitConfiguration config);

        /// <summary>
        /// Runs the executable and gets the data needed by the wizard.
        /// </summary>
        /// <param name="executableFile">The file to be executed.</param>
        /// <param name="runManager">The run manager to be used to execute the program.</param>
        /// <param name="data">The data object to be populated.</param>
        void RunProgramAndGetWizardData(string executableFile, IScenarioRunManager runManager, WizardData data);
    }
}
