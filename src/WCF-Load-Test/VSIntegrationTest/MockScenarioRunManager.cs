//---------------------------------------------------------------------
// <copyright file="MockScenarioRunManager.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
//     THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
//     OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
//     LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR
//     FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
// <summary>The MockScenarioRunManager type.</summary>
//---------------------------------------------------------------------

namespace VSIntegrationTest
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.WcfUnit.Library;

    internal class MockScenarioRunManager : IScenarioRunManager
    {
        private WcfUnitConfiguration configurationToReturn;

        private bool throwExceptionForRun;

        private State state = State.Constructed;

        private bool disposed = false;

        public enum State
        {
            /// <summary>
            /// The run manager has been constructed.
            /// </summary>
            Constructed,

            /// <summary>
            /// The run manager has been initialized.
            /// </summary>
            Initialised,

            /// <summary>
            /// The run manager has been set up.
            /// </summary>
            Setup,

            /// <summary>
            /// The run manager has executed.
            /// </summary>
            Executed,

            /// <summary>
            /// The run manager has been restored.
            /// </summary>
            Restored
        }

        public State RunState
        {
            get { return this.state; }
        }

        public WcfUnitConfiguration ConfigurationToReturn
        {
            set { this.configurationToReturn = value; }
        }

        public bool ThrowExceptionForRun
        {
            set { this.throwExceptionForRun = value; }
        }
     
        #region IScenarioRunManager Members

        public void Initialize(string executableFileName)
        {
            Assert.IsFalse(this.disposed, "Disposed ScenarioRunManager re-used");
            Assert.IsTrue(this.state == State.Constructed || this.state == State.Restored);
            this.state = State.Initialised;
        }

        public string SetupForTrace()
        {
            Assert.IsFalse(this.disposed, "Disposed ScenarioRunManager re-used");
            Assert.AreEqual<State>(State.Initialised, this.state);
            this.state = State.Setup;
            return "mocktracefile";
        }

        public void Run(WcfUnitConfiguration configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException("configuration");
            }

            Assert.IsFalse(this.disposed, "Disposed ScenarioRunManager re-used");
            Assert.AreEqual<State>(State.Setup, this.state);
            this.state = State.Executed;
            if (this.throwExceptionForRun)
            {
                throw new UserException("Simulated exception for test run");
            }

            configuration.assembly = this.configurationToReturn.assembly;
            configuration.soapActions = this.configurationToReturn.soapActions;
        }

        public void RestoreOriginalConfiguration()
        {
            Assert.IsFalse(this.disposed, "Disposed ScenarioRunManager re-used");
            Assert.AreEqual<State>(State.Executed, this.state);
            this.state = State.Restored;
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            this.disposed = true;
        }

        #endregion
    }
}
