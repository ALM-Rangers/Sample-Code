//---------------------------------------------------------------------
// <copyright file="UILogic.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
//     THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
//     OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
//     LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR
//     FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
// <summary>The UILogic type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.WcfUnit.Library
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    /// <summary>
    /// Encapsulates the logic for using a scenario run manager or parsing a trace.
    /// </summary>
    public class UILogic : Microsoft.WcfUnit.Library.IUILogic
    {
        /// <summary>
        /// Runs the executable and gets the data needed by the wizard.
        /// </summary>
        /// <param name="executableFile">The file to be executed.</param>
        /// <param name="runManager">The run manager to be used to execute the program.</param>
        /// <param name="data">The data object to be populated.</param>
        public void RunProgramAndGetWizardData(string executableFile, IScenarioRunManager runManager, WizardData data)
        {
            if (runManager == null)
            {
                throw new ArgumentNullException("runManager");
            }

            if (data == null)
            {
                throw new ArgumentNullException("data");
            }

            try
            {
                runManager.Initialize(executableFile);
                data.TraceFile = runManager.SetupForTrace();
                runManager.Run(data.Configuration);
            }
            finally
            {
                runManager.RestoreOriginalConfiguration();
            }
        }

        /// <summary>
        /// Gets the configuration from a trace file
        /// </summary>
        /// <param name="fileName">The name of the file to parse.</param>
        /// <param name="config">Tells this method what type of trace file it is, the configuration is updated with the soap actions.</param>
        public void ParseTraceFile(string fileName, WcfUnitConfiguration config)
        {
            if (config == null)
            {
                throw new ArgumentNullException("config");
            }

            List<SoapActionType> soapActions = new List<SoapActionType>();
            Stream traceFile = null;
            try
            {
                traceFile = Parser.OpenTraceFile(fileName);
                using (Parser parser = Parser.CreateParser(config.parser, traceFile, fileName, config.clientTrace, config.serviceTrace, SoapActionMode.Include))
                {
                    traceFile = null;
                    ParsedMessage msg = null;
                    do
                    {
                        msg = parser.ReadNextRequest();
                        if (msg != null)
                        {
                            SoapActionType newSat = new SoapActionType();
                            newSat.action = msg.SoapAction;
                            newSat.Selected = true;
                            if (!soapActions.Contains(newSat))
                            {
                                soapActions.Add(newSat);
                            }
                        }
                    }
                    while (msg != null);
                }
            }
            finally
            {
                if (traceFile != null)
                {
                    traceFile.Dispose();
                }
            }

            config.soapActions = new WcfUnitConfigurationSoapActions();
            config.soapActions.soapActionMode = SoapActionMode.Include;
            config.soapActions.soapAction = soapActions.ToArray();

            //// Set up the assemblies

            ////Trace.WriteLine(string.Format(CultureInfo.InvariantCulture, "Collecting information about referenced assemblies"));
            ////List<string> assemblies = new List<string>();
            ////GetReferencedAssemblies(_exeFileName, assemblies);
            ////Trace.WriteLine(string.Format(CultureInfo.InvariantCulture, "There are {0} referenced assemblies", assemblies.Count));
            ////config.assembly = new assemblyType[assemblies.Count];
            ////for (int i = 0; i < assemblies.Count; i++)
            ////{
            ////    config.assembly[i] = new assemblyType();
            ////    config.assembly[i].fileName = assemblies[i];
            ////}
        }
    }
}
