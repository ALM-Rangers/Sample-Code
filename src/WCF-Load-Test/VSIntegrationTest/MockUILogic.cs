//---------------------------------------------------------------------
// <copyright file="MockUILogic.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
//     THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
//     OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
//     LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR
//     FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
// <summary>The MockUILogic type.</summary>
//---------------------------------------------------------------------

namespace VSIntegrationTest
{
    using System;
    using Microsoft.WcfUnit.Library;

    internal class MockUILogic : IUILogic
    {
        private WcfUnitConfiguration configurationToReturn;

        private bool throwExceptionForParse;

        public WcfUnitConfiguration ConfigurationToReturn
        {
            set { this.configurationToReturn = value; }
        }

        public bool ThrowExceptionForParse
        {
            set { this.throwExceptionForParse = value; }
        }

        #region IUILogic Members

        public void ParseTraceFile(string fileName, WcfUnitConfiguration config)
        {
            if (config == null)
            {
                throw new ArgumentNullException("config");
            }

            if (this.throwExceptionForParse)
            {
                throw new UserException("Simulated exception for parse");
            }

            config.assembly = this.configurationToReturn.assembly;
            config.soapActions = this.configurationToReturn.soapActions;
        }

        public void RunProgramAndGetWizardData(string executableFile, IScenarioRunManager runManager, WizardData data)
        {
            UILogic uiLogic = new UILogic();
            uiLogic.RunProgramAndGetWizardData(executableFile, runManager, data);
        }

        #endregion
    }
}
