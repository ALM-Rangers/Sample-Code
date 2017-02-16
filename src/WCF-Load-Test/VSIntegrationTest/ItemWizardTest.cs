//---------------------------------------------------------------------
// <copyright file="ItemWizardTest.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
//     THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
//     OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
//     LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR
//     FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
// <summary>The ItemWizardTest type.</summary>
//---------------------------------------------------------------------

namespace VSIntegrationTest
{
    using System;
    using System.IO;
    using Microsoft.VisualStudio.TemplateWizard;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.WcfUnit.Library;
    using Microsoft.WcfUnit.Library.Test;
    using Microsoft.WcfUnit.VSIntegration;

    /// <summary>
    /// Tests the item wizard.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Disposed as part of test")]
    [TestClass]
    public class ItemWizardTest
    {
        /// <summary>
        /// The wizard to be tested.
        /// </summary>
        private ItemWizardImplementation wizard = null;

        /// <summary>
        /// The mock form.
        /// </summary>
        private MockWizardForm mockForm;

        /// <summary>
        /// The wizard data to be used.
        /// </summary>
        private WizardData wizardData;

        /// <summary>
        /// Flag to say if an error has been displayed.
        /// </summary>
        private bool errorDisplayCalled;

        /// <summary>
        /// Flag to control how the wizard is executed.
        /// </summary>
        private bool runWizardThrowsException;

        /// <summary>
        /// Initialises a new instance of the <see cref="ItemWizardTest"/> class.
        /// </summary>
        public ItemWizardTest()
        {
        }

        /// <summary>
        /// Sets up each test.
        /// </summary>
        [TestInitialize]
        public void InitializeTest()
        {
            this.wizard = new ItemWizardImplementation(new FormFactory(this.MockFactory), new ErrorDisplay(this.DisplayError));
            this.mockForm = null;

            this.wizardData = new WizardData();
            this.wizardData.TraceFile = Path.GetTempFileName();
            this.wizardData.TraceFileSource = TraceFileSource.Dynamic;
            this.wizardData.Configuration = new WcfUnitConfiguration();
            this.wizardData.Configuration.testMethodMode = TestMethodMode.ScenarioMethodOnly;
            this.wizardData.Configuration.operationTimerMode = OperationTimerMode.IncludeOperationTimers;
            AssemblyType a = new AssemblyType();
            a.fileName = "ClientProxies.dll";
            this.wizardData.Configuration.assembly = new AssemblyType[] { a };
            this.wizardData.Configuration.soapActions = new WcfUnitConfigurationSoapActions();
            this.wizardData.Configuration.soapActions.soapActionMode = SoapActionMode.Include;

            this.errorDisplayCalled = false;

            File.Copy(@"..\..\..\Library.Test\TestData\SampleWithNamespaces.svclog", this.wizardData.TraceFile, true);
            File.SetAttributes(this.wizardData.TraceFile, FileAttributes.Normal); // make sure not read only
            File.Delete("ignore.cs");
            File.Delete("test.cs");
            File.Delete("ignorestub.cs");
            File.Delete("teststub.cs");
        }

        /// <summary>
        /// Cleans up any left over files at the end of the test.
        /// </summary>
        [TestCleanup]
        public void CleanupTest()
        {
            File.Delete("ignore.cs");
            File.Delete("test.cs");
            File.Delete("ignorestub.cs");
            File.Delete("teststub.cs");
            File.Delete(this.wizardData.TraceFile);
            if (this.mockForm != null)
            {
                Assert.IsTrue(this.mockForm.DisposeCalled, "form should have been disposed already");
                if (!this.mockForm.DisposeCalled)
                {
                    this.mockForm.Dispose();
                }
            }
        }

        /// <summary>
        /// Test normal cycle with a valid scenario name.
        /// </summary>
        [TestMethod]
        public void VSWizNormalCycleValidName()
        {
            MockWizardForm.ResultToReturn = true;
            MockWizardForm.DataToReturn = this.wizardData;

            this.wizard.RunStarted("MyWCFTest.cs");
            Assert.IsNull(this.mockForm);
            Assert.IsTrue(this.wizard.ShouldAddProjectItem("ignore.cs"));
            Assert.AreEqual<int>(1, this.wizard.ProxyAssemblies.Count);
            Assert.AreEqual<string>("ClientProxies", Path.GetFileNameWithoutExtension(this.wizard.ProxyAssemblies[0]));
            Assert.AreEqual<string>(Path.GetFullPath("ClientProxies.dll"), this.wizard.ProxyAssemblies[0]);
            Assert.IsFalse(File.Exists(this.wizardData.TraceFile));
            Assert.IsNotNull(this.mockForm);
            Assert.IsTrue(this.mockForm.DisposeCalled);
            this.wizard.ProjectItemFinishedGenerating("test.cs");
            Assert.IsTrue(this.wizard.ShouldAddProjectItem("ignorestub.cs"));
            this.wizard.ProjectItemFinishedGenerating("teststub.cs");
            this.wizard.BeforeOpeningFile();
            Assert.IsTrue(File.Exists("test.cs"));
            Assert.IsTrue(File.Exists("teststub.cs"));
            Assert.IsFalse(File.Exists("ignore.cs"));
            Assert.IsFalse(File.Exists("ignorestub.cs"));
            this.wizard.RunFinished();
            Assert.IsFalse(this.errorDisplayCalled);
            TestHelper.CheckFileContains("test.cs", @"namespace MyWCFTest\s*\{", 1);
        }

        /// <summary>
        /// Tests exception is displayed and re-thrown if RunWizard throws an exception.
        /// </summary>
        [TestMethod]
        public void VSWizRunWizardExceptionIsDisplayedAndRethrown()
        {
            this.runWizardThrowsException = true;

            this.wizard.RunStarted("MyWCFTest.cs");
            try
            {
                this.wizard.ShouldAddProjectItem("ignore.cs");
                Assert.Fail("RunWizard exception not rethrown");
            }
            catch (UserException)
            {
                Assert.IsTrue(this.errorDisplayCalled);
            }
        }

        /// <summary>
        /// Tests normal cycle with a valid name and pre-existing log file does not delete the log file.
        /// </summary>
        [TestMethod]
        public void VSWizNormalCycleValidNameWithPreconfiguredTraceFileDoesNotDeleteTheTraceFile()
        {
            MockWizardForm.ResultToReturn = true;
            this.wizardData.TraceFileSource = TraceFileSource.PreCaptured;
            MockWizardForm.DataToReturn = this.wizardData;

            this.wizard.RunStarted("MyWCFTest.cs");
            this.wizard.ShouldAddProjectItem("ignore.cs");
            this.wizard.ProjectItemFinishedGenerating("test.cs");
            this.wizard.ShouldAddProjectItem("ignorestub.cs");
            this.wizard.ProjectItemFinishedGenerating("teststub.cs");
            this.wizard.BeforeOpeningFile();
            this.wizard.RunFinished();
            Assert.IsTrue(File.Exists(this.wizardData.TraceFile));
        }

        /// <summary>
        /// Tests with a partially invalid scenario name.
        /// </summary>
        [TestMethod]
        public void VSWizNormalCycleNameWithInvalidCharacters()
        {
            MockWizardForm.ResultToReturn = true;
            MockWizardForm.DataToReturn = this.wizardData;

            this.wizard.RunStarted("  MyWCF _Test.cs");
            Assert.IsNull(this.mockForm);
            Assert.IsTrue(this.wizard.ShouldAddProjectItem("ignore.cs"));
            Assert.AreEqual<int>(1, this.wizard.ProxyAssemblies.Count);
            Assert.AreEqual<string>("ClientProxies", Path.GetFileNameWithoutExtension(this.wizard.ProxyAssemblies[0]));
            Assert.AreEqual<string>(Path.GetFullPath("ClientProxies.dll"), this.wizard.ProxyAssemblies[0]);
            Assert.IsFalse(File.Exists(this.wizardData.TraceFile));
            Assert.IsNotNull(this.mockForm);
            Assert.IsTrue(this.mockForm.DisposeCalled);
            this.wizard.ProjectItemFinishedGenerating("test.cs");
            Assert.IsTrue(this.wizard.ShouldAddProjectItem("ignorestub.cs"));
            this.wizard.ProjectItemFinishedGenerating("teststub.cs");
            this.wizard.BeforeOpeningFile();
            Assert.IsTrue(File.Exists("test.cs"));
            Assert.IsTrue(File.Exists("teststub.cs"));
            Assert.IsFalse(File.Exists("ignore.cs"));
            Assert.IsFalse(File.Exists("ignorestub.cs"));
            this.wizard.RunFinished();
            Assert.IsFalse(this.errorDisplayCalled);
            TestHelper.CheckFileContains("test.cs", @"namespace MyWCF_Test\s*\{", 1);
        }

        /// <summary>
        /// Tests with a wholly invalid scenario name.
        /// </summary>
        [TestMethod]
        public void VSWizNormalCycleNameWithAllInvalidCharacters()
        {
            MockWizardForm.ResultToReturn = true;
            MockWizardForm.DataToReturn = this.wizardData;

            this.wizard.RunStarted(".....");
            Assert.IsNull(this.mockForm);
            Assert.IsTrue(this.wizard.ShouldAddProjectItem("ignore.cs"));
            Assert.AreEqual<int>(1, this.wizard.ProxyAssemblies.Count);
            Assert.AreEqual<string>("ClientProxies", Path.GetFileNameWithoutExtension(this.wizard.ProxyAssemblies[0]));
            Assert.AreEqual<string>(Path.GetFullPath("ClientProxies.dll"), this.wizard.ProxyAssemblies[0]);
            Assert.IsFalse(File.Exists(this.wizardData.TraceFile));
            Assert.IsNotNull(this.mockForm);
            Assert.IsTrue(this.mockForm.DisposeCalled);
            this.wizard.ProjectItemFinishedGenerating("test.cs");
            Assert.IsTrue(this.wizard.ShouldAddProjectItem("ignorestub.cs"));
            this.wizard.ProjectItemFinishedGenerating("teststub.cs");
            this.wizard.BeforeOpeningFile();
            Assert.IsTrue(File.Exists("test.cs"));
            Assert.IsTrue(File.Exists("teststub.cs"));
            Assert.IsFalse(File.Exists("ignore.cs"));
            Assert.IsFalse(File.Exists("ignorestub.cs"));
            this.wizard.RunFinished();
            Assert.IsFalse(this.errorDisplayCalled);
            TestHelper.CheckFileContains("test.cs", @"namespace WCFTest\s*\{", 1);
        }

        /// <summary>
        /// Test wizard cancelled before capturing message log.
        /// </summary>
        [TestMethod]
        public void VSWizCancelWizardBeforeTraceHasBeenCaptured()
        {
            MockWizardForm.ResultToReturn = false;
            MockWizardForm.DataToReturn = new WizardData();
            MockWizardForm.DataToReturn.Configuration = new WcfUnitConfiguration();

            this.wizard.RunStarted("test");
            Assert.IsNull(this.mockForm);
            try
            {
                Assert.IsTrue(this.wizard.ShouldAddProjectItem("ignore.cs"));
                Assert.Fail("Should have thrown a WizardCancelledException");
            }
            catch (WizardCancelledException)
            {
            }

            Assert.IsNotNull(this.mockForm);
            Assert.IsTrue(this.mockForm.DisposeCalled);
            Assert.IsFalse(File.Exists("test.cs"));
            Assert.IsFalse(File.Exists("teststub.cs"));
            Assert.IsFalse(File.Exists("ignore.cs"));
            Assert.IsFalse(File.Exists("ignorestub.cs"));
            Assert.IsFalse(this.errorDisplayCalled);
        }

        /// <summary>
        /// Test wizard cancelled after log has been captured.
        /// </summary>
        [TestMethod]
        public void VSWizCancelWizardAfterTraceHasBeenCaptured()
        {
            MockWizardForm.ResultToReturn = false;
            MockWizardForm.DataToReturn = this.wizardData;

            this.wizard.RunStarted("test");
            Assert.IsNull(this.mockForm);
            try
            {
                Assert.IsTrue(this.wizard.ShouldAddProjectItem("ignore.cs"));
                Assert.Fail("Should have thrown a WizardCancelledException");
            }
            catch (WizardCancelledException)
            {
            }

            Assert.IsFalse(File.Exists(this.wizardData.TraceFile));
            Assert.IsNotNull(this.mockForm);
            Assert.IsTrue(this.mockForm.DisposeCalled);
            Assert.IsFalse(File.Exists("test.cs"));
            Assert.IsFalse(File.Exists("teststub.cs"));
            Assert.IsFalse(File.Exists("ignore.cs"));
            Assert.IsFalse(File.Exists("ignorestub.cs"));
            Assert.IsFalse(this.errorDisplayCalled);
        }

        /// <summary>
        /// Test when proxy is not found.
        /// </summary>
        [TestMethod]
        public void VSWizProxyNotFound()
        {
            MockWizardForm.ResultToReturn = true;
            MockWizardForm.DataToReturn = this.wizardData;
            this.wizardData.Configuration.assembly[0].fileName = "Microsoft.WcfUnit.VSIntegration.dll";

            this.wizard.RunStarted("test");
            Assert.IsNull(this.mockForm);
            try
            {
                this.wizard.ShouldAddProjectItem("ignore.cs");
                Assert.Fail("Should have thrown a WizardCancelledException");
            }
            catch (WizardCancelledException)
            {
            }

            Assert.IsNotNull(this.mockForm);
            Assert.IsTrue(this.mockForm.DisposeCalled);
            Assert.IsFalse(File.Exists(this.wizardData.TraceFile));
            Assert.IsFalse(File.Exists("test.cs"));
            Assert.IsFalse(File.Exists("teststub.cs"));
            Assert.IsFalse(File.Exists("ignore.cs"));
            Assert.IsFalse(File.Exists("ignorestub.cs"));
            Assert.IsTrue(this.errorDisplayCalled);
        }

        /// <summary>
        /// Test when there is no output from the trace.
        /// </summary>
        [TestMethod]
        public void VSWizNoTraceOutput()
        {
            MockWizardForm.ResultToReturn = true;
            MockWizardForm.DataToReturn = this.wizardData;

            // Create a new empty trace file.
            File.Delete(this.wizardData.TraceFile);
            this.wizardData.TraceFile = Path.GetTempFileName();
            File.SetAttributes(this.wizardData.TraceFile, FileAttributes.Normal); // make sure not read only

            this.wizardData.Configuration.assembly[0].fileName = "Microsoft.WcfUnit.VSIntegration.dll";

            this.wizard.RunStarted("test");
            Assert.IsNull(this.mockForm);
            try
            {
                this.wizard.ShouldAddProjectItem("ignore.cs");
                Assert.Fail("Should have thrown a WizardCancelledException");
            }
            catch (WizardCancelledException)
            {
            }

            Assert.IsNotNull(this.mockForm);
            Assert.IsTrue(this.mockForm.DisposeCalled);
            Assert.IsFalse(File.Exists(this.wizardData.TraceFile));
            Assert.IsFalse(File.Exists("test.cs"));
            Assert.IsFalse(File.Exists("teststub.cs"));
            Assert.IsFalse(File.Exists("ignore.cs"));
            Assert.IsFalse(File.Exists("ignorestub.cs"));
            Assert.IsTrue(this.errorDisplayCalled);
        }

        /// <summary>
        /// Creates a wizard form.
        /// </summary>
        /// <returns>A wizard form</returns>
        private IWizardForm MockFactory()
        {
            Assert.IsNull(this.mockForm, "The wizard should never request a form more than once");
            this.mockForm = new MockWizardForm();
            this.mockForm.RunWizardThrowsException = this.runWizardThrowsException;
            return this.mockForm;
        }

        /// <summary>
        /// Mock error display method.
        /// </summary>
        /// <param name="errorMessage">The error message.</param>
        private void DisplayError(string errorMessage)
        {
            this.errorDisplayCalled = true;
        }
    }
}
