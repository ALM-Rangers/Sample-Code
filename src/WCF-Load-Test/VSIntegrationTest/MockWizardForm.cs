//---------------------------------------------------------------------
// <copyright file="MockWizardForm.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
//     THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
//     OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
//     LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR
//     FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
// <summary>The MockWizardForm type.</summary>
//---------------------------------------------------------------------

namespace VSIntegrationTest
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.WcfUnit.Library;
    using Microsoft.WcfUnit.VSIntegration;

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly", Justification = "proper implementation not needed, just checking it is actually disposed.")]
    public class MockWizardForm : IWizardForm
    {
        private static string executable;

        private static bool resultToReturn;

        private static WizardData dataToReturn;

        private bool runWizardThrowsException;

        private bool disposeCalled;

        private int runCallCount;

        public event EventHandler<CancellableWizardEventArgs> WizardCompleting;

        public static string Executable
        {
            get { return executable; }
            set { executable = value; }
        }

        public static bool ResultToReturn
        {
            get { return resultToReturn; }
            set { resultToReturn = value; }
        }

        public static WizardData DataToReturn
        {
            get { return dataToReturn; }
            set { dataToReturn = value; }
        }

        public bool RunWizardThrowsException
        {
            get { return this.runWizardThrowsException; }
            set { this.runWizardThrowsException = value; }
        }

        public bool DisposeCalled
        {
            get { return this.disposeCalled; }
        }

        #region IWizardForm Members

        public Microsoft.WcfUnit.Library.WizardData WizardData
        {
            get
            {
                return dataToReturn;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "RunWizard", Justification = "That is the name of the method")]
        public bool RunWizard()
        {
            this.runCallCount++;
            Assert.AreEqual<int>(1, this.runCallCount, "Run form must only ever be called once");
            Assert.IsNotNull(this.WizardCompleting);
            if (this.runWizardThrowsException)
            {
                throw new UserException("RunWizard exception");
            }

            if (Executable != null)
            {
                dataToReturn = new WizardData();
                dataToReturn.Configuration = new WcfUnitConfiguration();
                UILogic uiLogic = new UILogic();
                using (ScenarioRunManager srm = new ScenarioRunManager())
                {
                    uiLogic.RunProgramAndGetWizardData(Executable, srm, dataToReturn);
                }
            }

            if (resultToReturn)
            {
                CancellableWizardEventArgs cancel = new CancellableWizardEventArgs(dataToReturn, false);
                this.WizardCompleting(this, cancel);
                if (cancel.Cancel)
                {
                    resultToReturn = false;
                }
            }

            return resultToReturn;
        }

        #endregion

        #region IDisposable Members

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly", Justification = "proper implementation not needed, just checking it is actually disposed.")]
        public void Dispose()
        {
            Assert.IsFalse(this.disposeCalled);
            this.disposeCalled = true;
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
