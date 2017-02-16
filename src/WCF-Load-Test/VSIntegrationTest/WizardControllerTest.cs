//---------------------------------------------------------------------
// <copyright file="WizardControllerTest.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
//     THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
//     OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
//     LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR
//     FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
// <summary>The WizardControllerTest type.</summary>
//---------------------------------------------------------------------

namespace VSIntegrationTest
{
    using System.Collections.Generic;
    using System.Globalization;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.WcfUnit.Library;
    using Microsoft.WcfUnit.Library.Test;
    using Microsoft.WcfUnit.VSIntegration;

    /// <summary>
    /// Tests the <see cref="WizardController"/> class.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "disposable objects disposed in test cleanup")]
    [TestClass]
    public class WizardControllerTest
    {
        private WizardController controller;
        private MockWizardView view;
        private MockScenarioRunManager mgr;
        private MockUILogic uiLogic;
        private WcfUnitConfiguration noAssembliesConfig;
        private WcfUnitConfiguration noSoapActionsConfig;

        public WizardControllerTest()
        {
        }

        [TestInitialize]
        public void InitializeTest()
        {
            AssemblyType a = this.SetupNoAssembliesConfig();

            this.SetupNoSoapActionsConfig(a);

            this.view = new MockWizardView();
            this.mgr = new MockScenarioRunManager();
            this.uiLogic = new MockUILogic();
            this.SetConfigToReturn(this.noAssembliesConfig);
            this.controller = new WizardController(this.view, this.mgr, this.uiLogic);
            this.view.SetController(this.controller);
            this.controller.Initialize();
        }

        [TestCleanup]
        public void CleanupTest()
        {
            this.mgr.Dispose();
        }

        [TestMethod]
        public void VSWZVWGoToFirstPage()
        {
            this.controller.Initialize();
            Assert.AreEqual<WizardPage>(WizardPage.Welcome, this.view.CurrentPage);

            this.CheckPagesEnabledUpto(WizardPage.RunScenario);

            Assert.IsFalse(this.view.GetButtonState(WizardButton.Previous));
            Assert.IsTrue(this.view.GetButtonState(WizardButton.Next));
            Assert.IsFalse(this.view.GetButtonState(WizardButton.Finish));
            Assert.IsTrue(this.view.GetButtonState(WizardButton.Cancel));

            Assert.IsTrue(string.IsNullOrEmpty(this.view.LastErrorMessageBoxTitle));
            Assert.IsTrue(string.IsNullOrEmpty(this.view.LastErrorMessage));
        }

        [TestMethod]
        public void VSWZVWGoToRunPageWithNext()
        {
            this.controller.Initialize();
            this.controller.Next();
            Assert.AreEqual<WizardPage>(WizardPage.RunScenario, this.view.CurrentPage);

            this.CheckPagesEnabledUpto(WizardPage.RunScenario);

            Assert.IsTrue(this.view.GetButtonState(WizardButton.Previous));
            Assert.IsFalse(this.view.GetButtonState(WizardButton.Next));
            Assert.IsFalse(this.view.GetButtonState(WizardButton.Finish));
            Assert.IsTrue(this.view.GetButtonState(WizardButton.Cancel));

            Assert.IsTrue(string.IsNullOrEmpty(this.view.LastErrorMessageBoxTitle));
            Assert.IsTrue(string.IsNullOrEmpty(this.view.LastErrorMessage));
        }

        [TestMethod]
        public void VSWZVWGoToRunPageWithMove()
        {
            this.controller.Initialize();
            this.controller.MoveToPage(WizardPage.RunScenario);
            Assert.AreEqual<WizardPage>(WizardPage.RunScenario, this.view.CurrentPage);

            this.CheckPagesEnabledUpto(WizardPage.RunScenario);

            Assert.IsTrue(this.view.GetButtonState(WizardButton.Previous));
            Assert.IsFalse(this.view.GetButtonState(WizardButton.Next));
            Assert.IsFalse(this.view.GetButtonState(WizardButton.Finish));
            Assert.IsTrue(this.view.GetButtonState(WizardButton.Cancel));

            Assert.IsTrue(string.IsNullOrEmpty(this.view.LastErrorMessageBoxTitle));
            Assert.IsTrue(string.IsNullOrEmpty(this.view.LastErrorMessage));
        }

        [TestMethod]
        public void VSWZVWGoToRunPageThenBackToWelcomeWithNext()
        {
            this.controller.Initialize();
            this.controller.Next();
            this.controller.Previous();
            Assert.AreEqual<WizardPage>(WizardPage.Welcome, this.view.CurrentPage);

            this.CheckPagesEnabledUpto(WizardPage.RunScenario);

            Assert.IsFalse(this.view.GetButtonState(WizardButton.Previous));
            Assert.IsTrue(this.view.GetButtonState(WizardButton.Next));
            Assert.IsFalse(this.view.GetButtonState(WizardButton.Finish));
            Assert.IsTrue(this.view.GetButtonState(WizardButton.Cancel));

            Assert.IsTrue(string.IsNullOrEmpty(this.view.LastErrorMessageBoxTitle));
            Assert.IsTrue(string.IsNullOrEmpty(this.view.LastErrorMessage));
        }

        [TestMethod]
        public void VSWZVWGoToRunPageThenBackToWelcomeWithMove()
        {
            this.controller.Initialize();
            this.controller.MoveToPage(WizardPage.RunScenario);
            this.controller.MoveToPage(WizardPage.Welcome);
            Assert.AreEqual<WizardPage>(WizardPage.Welcome, this.view.CurrentPage);

            this.CheckPagesEnabledUpto(WizardPage.RunScenario);

            Assert.IsFalse(this.view.GetButtonState(WizardButton.Previous));
            Assert.IsTrue(this.view.GetButtonState(WizardButton.Next));
            Assert.IsFalse(this.view.GetButtonState(WizardButton.Finish));
            Assert.IsTrue(this.view.GetButtonState(WizardButton.Cancel));

            Assert.IsTrue(string.IsNullOrEmpty(this.view.LastErrorMessageBoxTitle));
            Assert.IsTrue(string.IsNullOrEmpty(this.view.LastErrorMessage));
        }

        [TestMethod]
        public void VSWZVWGoToRunPageAndSetExecutableFileName()
        {
            this.controller.Initialize();
            Assert.IsTrue(string.IsNullOrEmpty(this.view.ExecutableFileName));
            this.controller.Next();
            Assert.AreEqual<WizardPage>(WizardPage.RunScenario, this.view.CurrentPage);
            Assert.IsFalse(this.view.GetButtonState(WizardButton.RunExecutable));
            this.controller.SetExecutableFileName(null, ActionDirection.FromView);
            Assert.IsFalse(this.view.GetButtonState(WizardButton.RunExecutable));
            this.controller.SetExecutableFileName(string.Empty, ActionDirection.FromView);
            Assert.IsFalse(this.view.GetButtonState(WizardButton.RunExecutable));
            this.controller.SetExecutableFileName("abc.def", ActionDirection.FromView);
            Assert.IsTrue(this.view.GetButtonState(WizardButton.RunExecutable));

            this.CheckPagesEnabledUpto(WizardPage.RunScenario);

            Assert.IsTrue(this.view.GetButtonState(WizardButton.Previous));
            Assert.IsFalse(this.view.GetButtonState(WizardButton.Next));
            Assert.IsFalse(this.view.GetButtonState(WizardButton.Finish));
            Assert.IsTrue(this.view.GetButtonState(WizardButton.Cancel));

            Assert.IsTrue(string.IsNullOrEmpty(this.view.LastErrorMessageBoxTitle));
            Assert.IsTrue(string.IsNullOrEmpty(this.view.LastErrorMessage));
        }

        [TestMethod]
        public void VSWZVWRunExecutableSuccessfully()
        {
            this.controller.Initialize();
            this.controller.Next();
            this.controller.SetExecutableFileName("abc.def", ActionDirection.FromView);
            Assert.IsTrue(this.controller.RunApplicationUnderTest());

            this.CheckPagesEnabledUpto(WizardPage.SelectAssemblies);
            Assert.IsTrue(this.view.GetButtonState(WizardButton.Previous));
            Assert.IsTrue(this.view.GetButtonState(WizardButton.Next));
            Assert.IsFalse(this.view.GetButtonState(WizardButton.Finish));
            Assert.IsTrue(this.view.GetButtonState(WizardButton.Cancel));
            this.CheckSoapActionSelectionButtons();
            
            this.controller.Next();
            Assert.AreEqual<WizardPage>(WizardPage.SetOptions, this.view.CurrentPage);

            this.CheckPagesEnabledUpto(WizardPage.SelectAssemblies);
            Assert.IsTrue(this.view.GetButtonState(WizardButton.Previous));
            Assert.IsTrue(this.view.GetButtonState(WizardButton.Next));
            Assert.IsFalse(this.view.GetButtonState(WizardButton.Finish));
            Assert.IsTrue(this.view.GetButtonState(WizardButton.Cancel));

            Assert.AreEqual<MockScenarioRunManager.State>(MockScenarioRunManager.State.Restored, this.mgr.RunState);
            this.CheckSoapActions(new string[] { "action1", "action2" }, new string[] { });

            Assert.IsTrue(string.IsNullOrEmpty(this.view.LastErrorMessageBoxTitle));
            Assert.IsTrue(string.IsNullOrEmpty(this.view.LastErrorMessage));

            this.CheckAssemblies();
        }

        [TestMethod]
        public void VSWZVWRunExecutableWithFailure()
        {
            this.mgr.ThrowExceptionForRun = true;

            this.controller.Initialize();
            this.controller.Next();
            this.controller.SetExecutableFileName("abc.def", ActionDirection.FromView);
            Assert.IsFalse(this.controller.RunApplicationUnderTest());

            this.CheckPagesEnabledUpto(WizardPage.RunScenario);
            Assert.IsTrue(this.view.GetButtonState(WizardButton.Previous));
            Assert.IsFalse(this.view.GetButtonState(WizardButton.Next));
            Assert.IsFalse(this.view.GetButtonState(WizardButton.Finish));
            Assert.IsTrue(this.view.GetButtonState(WizardButton.Cancel));

            Assert.AreEqual<string>("Error while setting up or running the program under test", this.view.LastErrorMessageBoxTitle);
            Assert.AreEqual<string>("Simulated exception for test run", this.view.LastErrorMessage);
        }

        [TestMethod]
        public void VSWZVWRunExecutableSuccessfullyRepeatWithFailure()
        {
            this.controller.Initialize();
            this.controller.Next();
            this.controller.SetExecutableFileName("abc.def", ActionDirection.FromView);
            Assert.IsTrue(this.controller.RunApplicationUnderTest());
            this.mgr.ThrowExceptionForRun = true;
            Assert.IsFalse(this.controller.RunApplicationUnderTest());

            this.CheckPagesEnabledUpto(WizardPage.RunScenario);
            Assert.IsTrue(this.view.GetButtonState(WizardButton.Previous));
            Assert.IsFalse(this.view.GetButtonState(WizardButton.Next));
            Assert.IsFalse(this.view.GetButtonState(WizardButton.Finish));
            Assert.IsTrue(this.view.GetButtonState(WizardButton.Cancel));

            Assert.AreEqual<string>("Error while setting up or running the program under test", this.view.LastErrorMessageBoxTitle);
            Assert.AreEqual<string>("Simulated exception for test run", this.view.LastErrorMessage);
        }

        [TestMethod]
        public void VSWZVWRunExecutableWithFailureRepeatWithSuccess()
        {
            this.mgr.ThrowExceptionForRun = true;

            this.controller.Initialize();
            this.controller.Next();
            this.controller.SetExecutableFileName("abc.def", ActionDirection.FromView);
            Assert.IsFalse(this.controller.RunApplicationUnderTest());

            this.mgr.ThrowExceptionForRun = false;
            this.view.LastErrorMessageBoxTitle = null;
            this.view.LastErrorMessage = null;
            Assert.IsTrue(this.controller.RunApplicationUnderTest());

            this.CheckPagesEnabledUpto(WizardPage.SelectAssemblies);
            Assert.IsTrue(this.view.GetButtonState(WizardButton.Previous));
            Assert.IsTrue(this.view.GetButtonState(WizardButton.Next));
            Assert.IsFalse(this.view.GetButtonState(WizardButton.Finish));
            Assert.IsTrue(this.view.GetButtonState(WizardButton.Cancel));
            this.CheckSoapActionSelectionButtons();

            Assert.IsTrue(string.IsNullOrEmpty(this.view.LastErrorMessageBoxTitle));
            Assert.IsTrue(string.IsNullOrEmpty(this.view.LastErrorMessage));
        }

        [TestMethod]
        public void VSWZVWRunExecutableWithNoSoapActionsReturned()
        {
            this.SetConfigToReturn(this.noSoapActionsConfig);
            this.controller.Initialize();
            this.controller.Next();
            this.controller.SetExecutableFileName("abc.def", ActionDirection.FromView);
            Assert.IsFalse(this.controller.RunApplicationUnderTest());

            this.CheckPagesEnabledUpto(WizardPage.RunScenario);
            Assert.IsTrue(this.view.GetButtonState(WizardButton.Previous));
            Assert.IsFalse(this.view.GetButtonState(WizardButton.Next));
            Assert.IsFalse(this.view.GetButtonState(WizardButton.Finish));
            Assert.IsTrue(this.view.GetButtonState(WizardButton.Cancel));

            Assert.AreEqual<string>("Message Log File Problem", this.view.LastErrorMessageBoxTitle);
            Assert.AreEqual<string>("The message log did not contain any calls to WCF services", this.view.LastErrorMessage);
        }

        [TestMethod]
        public void VSWZVWRunExecutableSuccessfullyResetExecutableFileName()
        {
            this.controller.Initialize();
            this.controller.Next();
            this.controller.SetExecutableFileName("abc.def", ActionDirection.FromView);
            Assert.IsTrue(this.controller.RunApplicationUnderTest());
            this.controller.SetExecutableFileName("def.abc", ActionDirection.FromView);

            this.CheckPagesEnabledUpto(WizardPage.RunScenario);
            Assert.IsTrue(this.view.GetButtonState(WizardButton.Previous));
            Assert.IsFalse(this.view.GetButtonState(WizardButton.Next));
            Assert.IsFalse(this.view.GetButtonState(WizardButton.Finish));
            Assert.IsTrue(this.view.GetButtonState(WizardButton.Cancel));

            Assert.IsTrue(string.IsNullOrEmpty(this.view.LastErrorMessageBoxTitle));
            Assert.IsTrue(string.IsNullOrEmpty(this.view.LastErrorMessage));
        }

        [TestMethod]
        public void VSWZVWRunExecutableSuccessfullyTwiceAssemblyListIsResetAfterSecondRun()
        {
            this.controller.Initialize();
            this.controller.Next();
            this.controller.SetExecutableFileName("abc.def", ActionDirection.FromView);
            this.controller.RunApplicationUnderTest();
            this.controller.AddAssembly("abc.def");
            this.controller.RunApplicationUnderTest();

            this.CheckPagesEnabledUpto(WizardPage.SelectAssemblies);
            this.CheckAssemblies();
        }

        [TestMethod]
        public void VSWZVWGoToRunPageAndSetTraceFileName()
        {
            this.controller.Initialize();
            Assert.IsTrue(string.IsNullOrEmpty(this.view.TraceFileName));
            this.controller.Next();
            this.controller.ChooseTraceFileOptions(ActionDirection.FromController);
            Assert.IsFalse(this.view.GetButtonState(WizardButton.ParseTraceFile));
            this.controller.SetTraceFileName(null, ActionDirection.FromView);
            Assert.IsFalse(this.view.GetButtonState(WizardButton.ParseTraceFile));
            this.controller.SetTraceFileName(string.Empty, ActionDirection.FromView);
            Assert.IsFalse(this.view.GetButtonState(WizardButton.ParseTraceFile));
            this.controller.SetTraceFileName("abc.def", ActionDirection.FromView);
            Assert.IsTrue(this.view.GetButtonState(WizardButton.ParseTraceFile));

            Assert.IsTrue(this.view.GetButtonState(WizardButton.Previous));
            Assert.IsFalse(this.view.GetButtonState(WizardButton.Next));
            Assert.IsFalse(this.view.GetButtonState(WizardButton.Finish));
            Assert.IsTrue(this.view.GetButtonState(WizardButton.Cancel));

            Assert.IsTrue(string.IsNullOrEmpty(this.view.LastErrorMessageBoxTitle));
            Assert.IsTrue(string.IsNullOrEmpty(this.view.LastErrorMessage));
        }

        [TestMethod]
        public void VSWZVWParseTraceSuccessfully()
        {
            this.NavigateToRunPageAndSelectTraceFileOptions();
            this.controller.SetTraceFileName("abc.def", ActionDirection.FromView);
            Assert.IsTrue(this.controller.ParseTraceFile());

            this.CheckPagesEnabledUpto(WizardPage.SelectAssemblies);
            Assert.IsTrue(this.view.GetButtonState(WizardButton.Previous));
            Assert.IsTrue(this.view.GetButtonState(WizardButton.Next));
            Assert.IsFalse(this.view.GetButtonState(WizardButton.Finish));
            Assert.IsTrue(this.view.GetButtonState(WizardButton.Cancel));
            this.CheckSoapActionSelectionButtons();

            this.controller.Next();
            Assert.AreEqual<WizardPage>(WizardPage.SetOptions, this.view.CurrentPage);

            this.CheckPagesEnabledUpto(WizardPage.SelectAssemblies);
            Assert.IsTrue(this.view.GetButtonState(WizardButton.Previous));
            Assert.IsTrue(this.view.GetButtonState(WizardButton.Next));
            Assert.IsFalse(this.view.GetButtonState(WizardButton.Finish));
            Assert.IsTrue(this.view.GetButtonState(WizardButton.Cancel));

            this.CheckSoapActions(new string[] { "action1", "action2" }, new string[] { });

            Assert.IsTrue(string.IsNullOrEmpty(this.view.LastErrorMessageBoxTitle));
            Assert.IsTrue(string.IsNullOrEmpty(this.view.LastErrorMessage));

            this.CheckAssemblies();
        }

        [TestMethod]
        public void VSWZVWParseExecutableWithFailure()
        {
            this.NavigateToRunPageAndSelectTraceFileOptions();
            this.uiLogic.ThrowExceptionForParse = true;
            this.controller.SetTraceFileName("abc.def", ActionDirection.FromView);
            Assert.IsFalse(this.controller.ParseTraceFile());

            this.CheckPagesEnabledUpto(WizardPage.RunScenario);
            Assert.IsTrue(this.view.GetButtonState(WizardButton.Previous));
            Assert.IsFalse(this.view.GetButtonState(WizardButton.Next));
            Assert.IsFalse(this.view.GetButtonState(WizardButton.Finish));
            Assert.IsTrue(this.view.GetButtonState(WizardButton.Cancel));

            Assert.AreEqual<string>("Error while parsing message log file", this.view.LastErrorMessageBoxTitle);
            Assert.AreEqual<string>("Simulated exception for parse", this.view.LastErrorMessage);
        }

        [TestMethod]
        public void VSWZVWParseTraceFileSuccessfullyRepeatWithFailure()
        {
            this.NavigateToRunPageAndSelectTraceFileOptions();
            this.controller.SetTraceFileName("abc.def", ActionDirection.FromView);
            Assert.IsTrue(this.controller.ParseTraceFile());
            this.controller.SetTraceFileName("abc.def", ActionDirection.FromView);
            this.uiLogic.ThrowExceptionForParse = true;
            Assert.IsFalse(this.controller.ParseTraceFile());

            this.CheckPagesEnabledUpto(WizardPage.RunScenario);
            Assert.IsTrue(this.view.GetButtonState(WizardButton.Previous));
            Assert.IsFalse(this.view.GetButtonState(WizardButton.Next));
            Assert.IsFalse(this.view.GetButtonState(WizardButton.Finish));
            Assert.IsTrue(this.view.GetButtonState(WizardButton.Cancel));

            Assert.AreEqual<string>("Error while parsing message log file", this.view.LastErrorMessageBoxTitle);
            Assert.AreEqual<string>("Simulated exception for parse", this.view.LastErrorMessage);
        }

        [TestMethod]
        public void VSWZVWParseTraceFileWithFailureRepeatWithSuccess()
        {
            this.uiLogic.ThrowExceptionForParse = true;

            this.NavigateToRunPageAndSelectTraceFileOptions();
            this.controller.SetTraceFileName("abc.def", ActionDirection.FromView);
            Assert.IsFalse(this.controller.ParseTraceFile());

            this.uiLogic.ThrowExceptionForParse = false;
            this.view.LastErrorMessageBoxTitle = null;
            this.view.LastErrorMessage = null;
            Assert.IsTrue(this.controller.ParseTraceFile());

            this.CheckPagesEnabledUpto(WizardPage.SelectAssemblies);
            Assert.IsTrue(this.view.GetButtonState(WizardButton.Previous));
            Assert.IsTrue(this.view.GetButtonState(WizardButton.Next));
            Assert.IsFalse(this.view.GetButtonState(WizardButton.Finish));
            Assert.IsTrue(this.view.GetButtonState(WizardButton.Cancel));
            this.CheckSoapActionSelectionButtons();

            Assert.IsTrue(string.IsNullOrEmpty(this.view.LastErrorMessageBoxTitle));
            Assert.IsTrue(string.IsNullOrEmpty(this.view.LastErrorMessage));
        }

        [TestMethod]
        public void VSWZVWParseTraceFileWithNoSoapActionsReturned()
        {
            this.SetConfigToReturn(this.noSoapActionsConfig);
            this.NavigateToRunPageAndSelectTraceFileOptions();
            this.controller.SetTraceFileName("abc.def", ActionDirection.FromView);
            Assert.IsFalse(this.controller.ParseTraceFile());

            this.CheckPagesEnabledUpto(WizardPage.RunScenario);
            Assert.IsTrue(this.view.GetButtonState(WizardButton.Previous));
            Assert.IsFalse(this.view.GetButtonState(WizardButton.Next));
            Assert.IsFalse(this.view.GetButtonState(WizardButton.Finish));
            Assert.IsTrue(this.view.GetButtonState(WizardButton.Cancel));

            Assert.AreEqual<string>("Message Log File Problem", this.view.LastErrorMessageBoxTitle);
            Assert.AreEqual<string>("The message log did not contain any calls to WCF services", this.view.LastErrorMessage);
        }

        [TestMethod]
        public void VSWZVWParseTraceFileSuccessfullyResetTraceFileName()
        {
            this.NavigateToRunPageAndSelectTraceFileOptions();
            this.controller.SetTraceFileName("abc.def", ActionDirection.FromView);
            Assert.IsTrue(this.controller.ParseTraceFile());
            this.controller.SetTraceFileName("abc.def", ActionDirection.FromView);

            this.CheckPagesEnabledUpto(WizardPage.RunScenario);
            Assert.IsTrue(this.view.GetButtonState(WizardButton.Previous));
            Assert.IsFalse(this.view.GetButtonState(WizardButton.Next));
            Assert.IsFalse(this.view.GetButtonState(WizardButton.Finish));
            Assert.IsTrue(this.view.GetButtonState(WizardButton.Cancel));

            Assert.IsTrue(string.IsNullOrEmpty(this.view.LastErrorMessageBoxTitle));
            Assert.IsTrue(string.IsNullOrEmpty(this.view.LastErrorMessage));
        }

        [TestMethod]
        public void VSWZVWParseTraceFileSuccessfullyTwiceAssemblyListIsResetAfterSecondRun()
        {
            this.NavigateToRunPageAndSelectTraceFileOptions();
            this.controller.SetTraceFileName("abc.def", ActionDirection.FromView);
            this.controller.ParseTraceFile();
            this.controller.AddAssembly("abc.def");
            this.controller.ParseTraceFile();

            this.CheckPagesEnabledUpto(WizardPage.SelectAssemblies);
            this.CheckAssemblies();
        }

        [TestMethod]
        public void VSWZVWClearAllSoapActions()
        {
            this.controller.Initialize();
            this.controller.Next();
            this.controller.SetExecutableFileName("abc.def", ActionDirection.FromView);
            this.controller.RunApplicationUnderTest();
            this.controller.Next();

            // Clear all
            this.controller.ClearAllSoapActions();
            this.CheckSoapActions(new string[] { }, new string[] { "action1", "action2" });

            this.CheckPagesEnabledUpto(WizardPage.SetOptions);
            Assert.IsTrue(this.view.GetButtonState(WizardButton.Previous));
            Assert.IsFalse(this.view.GetButtonState(WizardButton.Next));
            Assert.IsFalse(this.view.GetButtonState(WizardButton.Finish));
            Assert.IsTrue(this.view.GetButtonState(WizardButton.Cancel));
            this.CheckSoapActionSelectionButtons();

            Assert.IsTrue(string.IsNullOrEmpty(this.view.LastErrorMessageBoxTitle));
            Assert.IsTrue(string.IsNullOrEmpty(this.view.LastErrorMessage));
        }

        [TestMethod]
        public void VSWZVWSetAllSoapActions()
        {
            this.controller.Initialize();
            this.controller.Next();
            this.controller.SetExecutableFileName("abc.def", ActionDirection.FromView);
            this.controller.RunApplicationUnderTest();
            this.controller.Next();
            this.controller.ClearAllSoapActions();

            // Set all again
            this.controller.SetAllSoapActions();
            this.CheckSoapActions(new string[] { "action1", "action2" }, new string[] { });

            this.CheckPagesEnabledUpto(WizardPage.SelectAssemblies);
            Assert.IsTrue(this.view.GetButtonState(WizardButton.Previous));
            Assert.IsTrue(this.view.GetButtonState(WizardButton.Next));
            Assert.IsFalse(this.view.GetButtonState(WizardButton.Finish));
            Assert.IsTrue(this.view.GetButtonState(WizardButton.Cancel));
            this.CheckSoapActionSelectionButtons();

            Assert.IsTrue(string.IsNullOrEmpty(this.view.LastErrorMessageBoxTitle));
            Assert.IsTrue(string.IsNullOrEmpty(this.view.LastErrorMessage));
        }

        [TestMethod]
        public void VSWZVWClearSoapAction()
        {
            this.controller.Initialize();
            this.controller.Next();
            this.controller.SetExecutableFileName("abc.def", ActionDirection.FromView);
            this.controller.RunApplicationUnderTest();
            this.controller.Next();

            // Remove one
            this.controller.RemoveSoapAction(0, ActionDirection.FromView);
            this.CheckSoapActions(new string[] { "action2" }, new string[] { "action1" });

            this.CheckPagesEnabledUpto(WizardPage.SelectAssemblies);
            Assert.IsTrue(this.view.GetButtonState(WizardButton.Previous));
            Assert.IsTrue(this.view.GetButtonState(WizardButton.Next));
            Assert.IsFalse(this.view.GetButtonState(WizardButton.Finish));
            Assert.IsTrue(this.view.GetButtonState(WizardButton.Cancel));
            this.CheckSoapActionSelectionButtons();

            // Remove second one, should be none left
            this.controller.RemoveSoapAction(1, ActionDirection.FromView);
            this.CheckSoapActions(new string[] { }, new string[] { "action1", "action2" });

            this.CheckPagesEnabledUpto(WizardPage.SetOptions);
            Assert.IsTrue(this.view.GetButtonState(WizardButton.Previous));
            Assert.IsFalse(this.view.GetButtonState(WizardButton.Next));
            Assert.IsFalse(this.view.GetButtonState(WizardButton.Finish));
            Assert.IsTrue(this.view.GetButtonState(WizardButton.Cancel));
            this.CheckSoapActionSelectionButtons();

            Assert.IsTrue(string.IsNullOrEmpty(this.view.LastErrorMessageBoxTitle));
            Assert.IsTrue(string.IsNullOrEmpty(this.view.LastErrorMessage));
        }

        [TestMethod]
        public void VSWZVWAddSoapAction()
        {
            this.controller.Initialize();
            this.controller.Next();
            this.controller.SetExecutableFileName("abc.def", ActionDirection.FromView);
            this.controller.RunApplicationUnderTest();
            this.controller.Next();
            this.controller.ClearAllSoapActions();

            // Add one back
            this.controller.AddSoapAction(0, ActionDirection.FromView);
            this.CheckSoapActions(new string[] { "action1" }, new string[] { "action2" });

            this.CheckPagesEnabledUpto(WizardPage.SelectAssemblies);
            Assert.IsTrue(this.view.GetButtonState(WizardButton.Previous));
            Assert.IsTrue(this.view.GetButtonState(WizardButton.Next));
            Assert.IsFalse(this.view.GetButtonState(WizardButton.Finish));
            Assert.IsTrue(this.view.GetButtonState(WizardButton.Cancel));
            this.CheckSoapActionSelectionButtons();

            // Add second one back
            this.controller.AddSoapAction(1, ActionDirection.FromView);
            this.CheckSoapActions(new string[] { "action1", "action2" }, new string[] { });

            this.CheckPagesEnabledUpto(WizardPage.SelectAssemblies);
            Assert.IsTrue(this.view.GetButtonState(WizardButton.Previous));
            Assert.IsTrue(this.view.GetButtonState(WizardButton.Next));
            Assert.IsFalse(this.view.GetButtonState(WizardButton.Finish));
            Assert.IsTrue(this.view.GetButtonState(WizardButton.Cancel));
            this.CheckSoapActionSelectionButtons();

            Assert.IsTrue(string.IsNullOrEmpty(this.view.LastErrorMessageBoxTitle));
            Assert.IsTrue(string.IsNullOrEmpty(this.view.LastErrorMessage));
        }

        [TestMethod]
        public void VSWZVWCheckCanSetAndGetBoolOptions()
        {
            this.controller.Initialize();
            this.controller.Next();
            this.controller.SetExecutableFileName("abc.def", ActionDirection.FromView);
            this.controller.RunApplicationUnderTest();

            this.CheckOptions(OperationTimerMode.IncludeOperationTimers, TestMethodMode.ScenarioMethodOnly);  // default

            this.controller.SetOperationTimerMode(OperationTimerMode.NoOperationTimers, ActionDirection.FromController);
            this.CheckOptions(OperationTimerMode.NoOperationTimers, TestMethodMode.ScenarioMethodOnly);

            this.controller.SetOperationTimerMode(OperationTimerMode.IncludeOperationTimers, ActionDirection.FromController);
            this.CheckOptions(OperationTimerMode.IncludeOperationTimers, TestMethodMode.ScenarioMethodOnly);

            this.controller.SetTestMethodMode(TestMethodMode.IncludeIndividualOperations, ActionDirection.FromController);
            this.CheckOptions(OperationTimerMode.IncludeOperationTimers, TestMethodMode.IncludeIndividualOperations);

            this.controller.SetTestMethodMode(TestMethodMode.ScenarioMethodOnly, ActionDirection.FromController);
            this.CheckOptions(OperationTimerMode.IncludeOperationTimers, TestMethodMode.ScenarioMethodOnly);

            Assert.IsTrue(string.IsNullOrEmpty(this.view.LastErrorMessageBoxTitle));
            Assert.IsTrue(string.IsNullOrEmpty(this.view.LastErrorMessage));
        }

        [TestMethod]
        public void VSWZVWGoToSetOptionsPageThenBackToWelcomeWithNextAndMove()
        {
            this.controller.Initialize();
            this.controller.Next();
            this.controller.SetExecutableFileName("abc.def", ActionDirection.FromView);
            Assert.IsTrue(this.controller.RunApplicationUnderTest());
            this.controller.Next();

            this.controller.Previous();
            Assert.AreEqual<WizardPage>(WizardPage.RunScenario, this.view.CurrentPage);

            this.CheckPagesEnabledUpto(WizardPage.SelectAssemblies);
            Assert.IsTrue(this.view.GetButtonState(WizardButton.Previous));
            Assert.IsTrue(this.view.GetButtonState(WizardButton.Next));
            Assert.IsFalse(this.view.GetButtonState(WizardButton.Finish));
            Assert.IsTrue(this.view.GetButtonState(WizardButton.Cancel));

            this.controller.Previous();
            Assert.AreEqual<WizardPage>(WizardPage.Welcome, this.view.CurrentPage);

            this.CheckPagesEnabledUpto(WizardPage.SelectAssemblies);
            Assert.IsFalse(this.view.GetButtonState(WizardButton.Previous));
            Assert.IsTrue(this.view.GetButtonState(WizardButton.Next));
            Assert.IsFalse(this.view.GetButtonState(WizardButton.Finish));
            Assert.IsTrue(this.view.GetButtonState(WizardButton.Cancel));

            this.controller.MoveToPage(WizardPage.SetOptions);
            Assert.AreEqual<WizardPage>(WizardPage.SetOptions, this.view.CurrentPage);

            this.CheckPagesEnabledUpto(WizardPage.SelectAssemblies);
            Assert.IsTrue(this.view.GetButtonState(WizardButton.Previous));
            Assert.IsTrue(this.view.GetButtonState(WizardButton.Next));
            Assert.IsFalse(this.view.GetButtonState(WizardButton.Finish));
            Assert.IsTrue(this.view.GetButtonState(WizardButton.Cancel));

            this.controller.MoveToPage(WizardPage.Welcome);
            Assert.AreEqual<WizardPage>(WizardPage.Welcome, this.view.CurrentPage);

            this.CheckPagesEnabledUpto(WizardPage.SelectAssemblies);
            Assert.IsFalse(this.view.GetButtonState(WizardButton.Previous));
            Assert.IsTrue(this.view.GetButtonState(WizardButton.Next));
            Assert.IsFalse(this.view.GetButtonState(WizardButton.Finish));
            Assert.IsTrue(this.view.GetButtonState(WizardButton.Cancel));

            this.controller.MoveToPage(WizardPage.RunScenario);
            Assert.AreEqual<WizardPage>(WizardPage.RunScenario, this.view.CurrentPage);

            this.CheckPagesEnabledUpto(WizardPage.SelectAssemblies);
            Assert.IsTrue(this.view.GetButtonState(WizardButton.Previous));
            Assert.IsTrue(this.view.GetButtonState(WizardButton.Next));
            Assert.IsFalse(this.view.GetButtonState(WizardButton.Finish));
            Assert.IsTrue(this.view.GetButtonState(WizardButton.Cancel));

            Assert.IsTrue(string.IsNullOrEmpty(this.view.LastErrorMessageBoxTitle));
            Assert.IsTrue(string.IsNullOrEmpty(this.view.LastErrorMessage));
        }

        [TestMethod]
        public void VSWZVWGoToSelectAssembliesPageWithNextThenBackWithNextAndMove()
        {
            this.controller.Initialize();
            this.controller.Next();
            this.controller.SetExecutableFileName("abc.def", ActionDirection.FromView);
            Assert.IsTrue(this.controller.RunApplicationUnderTest());
            this.controller.Next();
            this.controller.Next();

            Assert.AreEqual<WizardPage>(WizardPage.SelectAssemblies, this.view.CurrentPage);

            this.CheckPagesEnabledUpto(WizardPage.SelectAssemblies);
            Assert.IsTrue(this.view.GetButtonState(WizardButton.Previous));
            Assert.IsFalse(this.view.GetButtonState(WizardButton.Next));
            Assert.IsFalse(this.view.GetButtonState(WizardButton.Finish));
            Assert.IsTrue(this.view.GetButtonState(WizardButton.Cancel));

            this.controller.Previous();
            Assert.AreEqual<WizardPage>(WizardPage.SetOptions, this.view.CurrentPage);

            this.CheckPagesEnabledUpto(WizardPage.SelectAssemblies);
            Assert.IsTrue(this.view.GetButtonState(WizardButton.Previous));
            Assert.IsTrue(this.view.GetButtonState(WizardButton.Next));
            Assert.IsFalse(this.view.GetButtonState(WizardButton.Finish));
            Assert.IsTrue(this.view.GetButtonState(WizardButton.Cancel));

            this.controller.MoveToPage(WizardPage.SelectAssemblies);
            this.controller.MoveToPage(WizardPage.SetOptions);
            Assert.AreEqual<WizardPage>(WizardPage.SetOptions, this.view.CurrentPage);

            this.CheckPagesEnabledUpto(WizardPage.SelectAssemblies);
            Assert.IsTrue(this.view.GetButtonState(WizardButton.Previous));
            Assert.IsTrue(this.view.GetButtonState(WizardButton.Next));
            Assert.IsFalse(this.view.GetButtonState(WizardButton.Finish));
            Assert.IsTrue(this.view.GetButtonState(WizardButton.Cancel));
        }

        [TestMethod]
        public void VSWZVWAddAssembly()
        {
            this.controller.Initialize();
            this.controller.Next();
            this.controller.SetExecutableFileName("abc.def", ActionDirection.FromView);
            this.controller.RunApplicationUnderTest();
            this.controller.Next();
            this.controller.Next();

            this.CheckPagesEnabledUpto(WizardPage.SelectAssemblies);
            Assert.IsTrue(this.view.GetButtonState(WizardButton.Previous));
            Assert.IsFalse(this.view.GetButtonState(WizardButton.Next));
            Assert.IsFalse(this.view.GetButtonState(WizardButton.Finish));
            Assert.IsTrue(this.view.GetButtonState(WizardButton.Cancel));
            Assert.IsTrue(this.view.GetButtonState(WizardButton.AddAssembly));

            this.CheckAssemblies(new AssemblyType[0]);

            this.controller.AddAssembly("abc.def");
            this.CheckAssemblies(new AssemblyType("abc.def"));
            Assert.IsTrue(this.view.GetButtonState(WizardButton.Finish));

            this.controller.AddAssembly("def.abc");
            this.CheckAssemblies(new AssemblyType("abc.def"), new AssemblyType("def.abc"));
            Assert.IsTrue(this.view.GetButtonState(WizardButton.Finish));
        }

        [TestMethod]
        public void VSWZVWRemoveAssembly()
        {
            this.controller.Initialize();
            this.controller.Next();
            this.controller.SetExecutableFileName("abc.def", ActionDirection.FromView);
            this.controller.RunApplicationUnderTest();
            this.controller.Next();
            this.controller.Next();
            this.controller.AddAssembly("abc.def");
            this.controller.AddAssembly("def.abc");

            this.controller.RemoveAssembly(0, ActionDirection.FromController);
            this.CheckAssemblies(new AssemblyType("def.abc"));

            this.CheckPagesEnabledUpto(WizardPage.SelectAssemblies);
            Assert.IsTrue(this.view.GetButtonState(WizardButton.Previous));
            Assert.IsFalse(this.view.GetButtonState(WizardButton.Next));
            Assert.IsTrue(this.view.GetButtonState(WizardButton.Finish));
            Assert.IsTrue(this.view.GetButtonState(WizardButton.Cancel));
            Assert.IsTrue(this.view.GetButtonState(WizardButton.AddAssembly));

            this.controller.RemoveAssembly(0, ActionDirection.FromController);
            this.CheckAssemblies(new AssemblyType[0]);

            this.CheckPagesEnabledUpto(WizardPage.SelectAssemblies);
            Assert.IsTrue(this.view.GetButtonState(WizardButton.Previous));
            Assert.IsFalse(this.view.GetButtonState(WizardButton.Next));
            Assert.IsFalse(this.view.GetButtonState(WizardButton.Finish));
            Assert.IsTrue(this.view.GetButtonState(WizardButton.Cancel));
            Assert.IsTrue(this.view.GetButtonState(WizardButton.AddAssembly));
        }

        [TestMethod]
        public void VSWZVWRemoveAssemblyAndAddBack()
        {
            this.controller.Initialize();
            this.controller.Next();
            this.controller.SetExecutableFileName("abc.def", ActionDirection.FromView);
            this.controller.RunApplicationUnderTest();
            this.controller.Next();
            this.controller.Next();
            this.controller.AddAssembly("abc.def");
            this.controller.AddAssembly("def.abc");
            this.controller.RemoveAssembly(0, ActionDirection.FromController);
            this.controller.RemoveAssembly(0, ActionDirection.FromController);

            this.CheckPagesEnabledUpto(WizardPage.SelectAssemblies);
            Assert.IsTrue(this.view.GetButtonState(WizardButton.Previous));
            Assert.IsFalse(this.view.GetButtonState(WizardButton.Next));
            Assert.IsFalse(this.view.GetButtonState(WizardButton.Finish));
            Assert.IsTrue(this.view.GetButtonState(WizardButton.Cancel));
            Assert.IsTrue(this.view.GetButtonState(WizardButton.AddAssembly));

            Assert.AreEqual<int>(0, this.view.SelectedAssemblies.Count);

            this.controller.AddAssembly("abc.def");
            this.CheckAssemblies(new AssemblyType("abc.def"));
            Assert.IsTrue(this.view.GetButtonState(WizardButton.Finish));
        }

        [TestMethod]
        public void VSWZVWCheckGatheredData()
        {
            this.controller.Initialize();
            this.controller.Next();
            this.controller.SetExecutableFileName("abc.def", ActionDirection.FromView);
            this.controller.RunApplicationUnderTest();
            this.controller.Next();
            this.controller.Next();
            this.controller.AddAssembly("abc.def");
            this.controller.AddAssembly("def.abc");

            WizardData data = this.controller.WizardData;
            Assert.AreEqual<string>("mocktracefile", data.TraceFile);
            Assert.AreEqual<TraceFileSource>(TraceFileSource.Dynamic, data.TraceFileSource);
            Assert.AreEqual<OperationTimerMode>(OperationTimerMode.IncludeOperationTimers, data.Configuration.operationTimerMode);
            Assert.AreEqual<TestMethodMode>(TestMethodMode.ScenarioMethodOnly, data.Configuration.testMethodMode);
            Assert.AreEqual<SoapActionMode>(SoapActionMode.Include, data.Configuration.soapActions.soapActionMode);
            Assert.AreEqual<int>(2, data.Configuration.soapActions.soapAction.Length);
            Assert.AreEqual<string>("action1", data.Configuration.soapActions.soapAction[0].action);
            Assert.AreEqual<string>("action2", data.Configuration.soapActions.soapAction[1].action);
            Assert.AreEqual<int>(2, data.Configuration.assembly.Length);
            Assert.AreEqual<string>("abc.def", data.Configuration.assembly[0].fileName);
            Assert.AreEqual<string>("def.abc", data.Configuration.assembly[1].fileName);
        }

        [TestMethod]
        public void VSWZVWCheckGatheredDataIsEmptyAfterACancellation()
        {
            this.controller.Initialize();

            WizardData data = this.controller.FilteredWizardData;
            Assert.IsNull(data.Configuration.soapActions);
            Assert.IsNull(data.Configuration.assembly);
        }

        [TestMethod]
        public void VSWZVWCheckGatheredDataSetsTraceSourceToPrecapturedWhenClientTraceFileNameIsSpecified()
        {
            this.controller.Initialize();
            this.controller.Next();
            this.controller.SetTraceFileName("FullSampleTest.svclog", ActionDirection.FromController);
            this.controller.ChooseWcfClientTraceFile(ActionDirection.FromController);
            this.controller.ChooseTraceFileOptions(ActionDirection.FromController);
            this.controller.ParseTraceFile();
            this.controller.Next();
            this.controller.Next();
            this.controller.AddAssembly("abc.def");
            this.controller.AddAssembly("def.abc");

            WizardData data = this.controller.WizardData;
            Assert.AreEqual<TraceFileSource>(TraceFileSource.PreCaptured, data.TraceFileSource);
            Assert.AreEqual<TraceFileSource>(data.TraceFileSource, this.controller.FilteredWizardData.TraceFileSource);
            Assert.IsNull(this.controller.FilteredWizardData.Configuration.parser);
        }

        [TestMethod]
        public void VSWZVWCheckGatheredDataSetsTraceSourceToPrecapturedWhenServerTraceFileNameIsSpecified()
        {
            this.controller.Initialize();
            this.controller.Next();
            this.controller.SetTraceFileName("ServiceSideLog.svclog", ActionDirection.FromController);
            this.controller.ChooseWcfServerTraceFile(ActionDirection.FromController);
            this.controller.ChooseTraceFileOptions(ActionDirection.FromController);
            this.controller.ParseTraceFile();
            this.controller.Next();
            this.controller.Next();
            this.controller.AddAssembly("abc.def");
            this.controller.AddAssembly("def.abc");

            WizardData data = this.controller.WizardData;
            Assert.AreEqual<TraceFileSource>(TraceFileSource.PreCaptured, data.TraceFileSource);
            Assert.AreEqual<TraceFileSource>(data.TraceFileSource, this.controller.FilteredWizardData.TraceFileSource);
            Assert.IsNull(this.controller.FilteredWizardData.Configuration.parser);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "data", Justification = "Want to make retrieval of data more explicit")]
        [TestMethod]
        public void VSWZVWCheckGatheredDataSetsFiddlerParserWhenFiddlerFormatSpecified()
        {
            // Arrange and act
            this.controller.Initialize();
            this.controller.Next();
            this.controller.SetTraceFileName("ServiceSideLog.svclog", ActionDirection.FromController);
            this.controller.ChooseFiddlerTextTraceFile(ActionDirection.FromController);
            this.controller.ChooseTraceFileOptions(ActionDirection.FromController);
            this.controller.ParseTraceFile();
            this.controller.Next();
            this.controller.Next();
            this.controller.AddAssembly("abc.def");
            this.controller.AddAssembly("def.abc");

            WizardData data = this.controller.WizardData;

            // Assert
            Assert.IsNotNull(this.controller.FilteredWizardData.Configuration.parser);
            Assert.AreEqual<string>(TestHelper.LibraryAssembly, this.controller.FilteredWizardData.Configuration.parser.assembly);
            Assert.AreEqual<string>(typeof(FiddlerTextParser).FullName, this.controller.FilteredWizardData.Configuration.parser.type);
        }

        [TestMethod]
        public void VSWZVWCheckGatheredDataSetsTraceSourceToDynamicAfterSwitchBackToExecutable()
        {
            this.controller.Initialize();
            this.controller.Next();
            this.controller.SetTraceFileName("FullSampleTest.svclog", ActionDirection.FromController);
            this.controller.ChooseWcfClientTraceFile(ActionDirection.FromController);
            this.controller.ChooseTraceFileOptions(ActionDirection.FromController);
            this.controller.ChooseExecutableOptions(ActionDirection.FromController);
            this.controller.SetExecutableFileName("abc.def", ActionDirection.FromController);
            this.controller.RunApplicationUnderTest();
            this.controller.Next();
            this.controller.Next();
            this.controller.AddAssembly("abc.def");
            this.controller.AddAssembly("def.abc");

            WizardData data = this.controller.WizardData;
            Assert.AreEqual<TraceFileSource>(TraceFileSource.Dynamic, data.TraceFileSource);
            Assert.AreEqual<TraceFileSource>(data.TraceFileSource, this.controller.FilteredWizardData.TraceFileSource);
        }

        [TestMethod]
        public void VSWZVWCheckGatheredDataSetsTraceSourceToPrecapturedAfterSwitchBackToTraceFileAgain()
        {
            this.controller.Initialize();
            this.controller.Next();
            this.controller.SetTraceFileName("FullSampleTest.svclog", ActionDirection.FromController);
            this.controller.ChooseWcfClientTraceFile(ActionDirection.FromController);
            this.controller.ChooseTraceFileOptions(ActionDirection.FromController);
            this.controller.ChooseExecutableOptions(ActionDirection.FromController);
            this.controller.SetExecutableFileName("abc.def", ActionDirection.FromController);
            this.controller.SetTraceFileName("FullSampleTest.svclog", ActionDirection.FromController);
            this.controller.ChooseWcfClientTraceFile(ActionDirection.FromController);
            this.controller.ChooseTraceFileOptions(ActionDirection.FromController);
            this.controller.ParseTraceFile();
            this.controller.Next();
            this.controller.Next();
            this.controller.AddAssembly("abc.def");
            this.controller.AddAssembly("def.abc");

            WizardData data = this.controller.WizardData;
            Assert.AreEqual<TraceFileSource>(TraceFileSource.PreCaptured, data.TraceFileSource);
            Assert.AreEqual<TraceFileSource>(data.TraceFileSource, this.controller.FilteredWizardData.TraceFileSource);
            Assert.IsNull(this.controller.FilteredWizardData.Configuration.parser);
        }

        [TestMethod]
        public void VSWZVWFilteredWizardDataDoesNotHaveUnselectedSoapActions()
        {
            this.controller.Initialize();
            this.controller.Next();
            this.controller.SetExecutableFileName("abc.def", ActionDirection.FromView);
            this.controller.RunApplicationUnderTest();
            this.controller.Next();
            this.controller.Next();
            this.controller.AddAssembly("abc.def");
            this.controller.AddAssembly("def.abc");
            this.controller.RemoveSoapAction(0, ActionDirection.FromController);

            this.CheckSoapActions(new string[] { "action2" }, new string[] { }, this.controller.FilteredWizardData.Configuration.soapActions);
        }

        [TestMethod]
        public void VSWZVWExecutableFileNameIsSetOnViewWhenActionDirectionIsFromController()
        {
            this.controller.Initialize();
            this.view.ExecutableFileName = "dummy";

            this.controller.SetExecutableFileName("abc.def", ActionDirection.FromController);

            Assert.AreEqual<string>("abc.def", this.view.ExecutableFileName);
        }

        [TestMethod]
        public void VSWZVWExecutableFileNameNotSetOnViewWhenActionDirectionIsFromView()
        {
            this.controller.Initialize();
            this.view.ExecutableFileName = "dummy";

            this.controller.SetExecutableFileName("abc.def", ActionDirection.FromView);

            Assert.AreEqual<string>("dummy", this.view.ExecutableFileName);
        }

        [TestMethod]
        public void VSWZVWRemoveSoapActionSetsListOnViewWhenActionDirectionIsFromController()
        {
            this.controller.Initialize();
            this.controller.SetExecutableFileName("abc.def", ActionDirection.FromView);
            this.controller.RunApplicationUnderTest();
            this.view.SoapActions = null;

            this.controller.RemoveSoapAction(0, ActionDirection.FromController);

            Assert.IsNotNull(this.view.SoapActions);
        }

        [TestMethod]
        public void VSWZVWRemoveSoapActionDoesNotSetListOnViewWhenActionDirectionIsFromView()
        {
            this.controller.Initialize();
            this.controller.SetExecutableFileName("abc.def", ActionDirection.FromView);
            this.controller.RunApplicationUnderTest();
            this.view.SoapActions = null;

            this.controller.RemoveSoapAction(0, ActionDirection.FromView);

            Assert.IsNull(this.view.SoapActions);
        }

        [TestMethod]
        public void VSWZVWAddSoapActionSetsListOnViewWhenActionDirectionIsFromController()
        {
            this.controller.Initialize();
            this.controller.SetExecutableFileName("abc.def", ActionDirection.FromView);
            this.controller.RunApplicationUnderTest();
            this.view.SoapActions = null;

            this.controller.AddSoapAction(0, ActionDirection.FromController);

            Assert.IsNotNull(this.view.SoapActions);
        }

        [TestMethod]
        public void VSWZVWAddSoapActionDoesNotSetListOnViewWhenActionDirectionIsFromView()
        {
            this.controller.Initialize();
            this.controller.SetExecutableFileName("abc.def", ActionDirection.FromView);
            this.controller.RunApplicationUnderTest();
            this.view.SoapActions = null;

            this.controller.AddSoapAction(0, ActionDirection.FromView);

            Assert.IsNull(this.view.SoapActions);
        }

        [TestMethod]
        public void VSWZVWOperationTimerModeIsSetOnViewWhenActionDirectionIsFromController()
        {
            this.controller.Initialize();
            this.view.SetIncludeOperationTimers(false);

            this.controller.SetOperationTimerMode(OperationTimerMode.IncludeOperationTimers, ActionDirection.FromController);
            Assert.IsTrue(this.view.IncludeOperationTimers);
        }

        [TestMethod]
        public void VSWZVWOperationTimerModeNotSetOnViewWhenActionDirectionIsFromView()
        {
            this.controller.Initialize();
            this.view.SetIncludeOperationTimers(false);

            this.controller.SetOperationTimerMode(OperationTimerMode.IncludeOperationTimers, ActionDirection.FromView);
            Assert.IsFalse(this.view.IncludeOperationTimers);
        }

        [TestMethod]
        public void VSWZVWTestMethodModeIsSetOnViewWhenActionDirectionIsFromController()
        {
            this.controller.Initialize();
            this.view.SetIncludeUnitTestPerOperation(false);

            this.controller.SetTestMethodMode(TestMethodMode.IncludeIndividualOperations, ActionDirection.FromController);
            Assert.IsTrue(this.view.IncludeUnitTestPerOperation);
        }

        [TestMethod]
        public void VSWZVWTestMethodModeNotSetOnViewWhenActionDirectionIsFromView()
        {
            this.controller.Initialize();
            this.view.SetIncludeUnitTestPerOperation(false);

            this.controller.SetTestMethodMode(TestMethodMode.IncludeIndividualOperations, ActionDirection.FromView);
            Assert.IsFalse(this.view.IncludeUnitTestPerOperation);
        }

        [TestMethod]
        public void VSWZVWRemoveAssemblyUpdatesViewWhenActionDirectionIsFromController()
        {
            this.controller.Initialize();
            this.controller.SetExecutableFileName("abc.def", ActionDirection.FromView);
            this.controller.RunApplicationUnderTest();
            this.view.SoapActions = null;
            this.controller.AddSoapAction(0, ActionDirection.FromController);
            this.controller.AddAssembly("abc.def");

            this.controller.RemoveAssembly(0, ActionDirection.FromController);

            Assert.AreEqual<int>(0, this.view.SelectedAssemblies.Count);
        }

        [TestMethod]
        public void VSWZVWRemoveAssemblyDoesNotUpdateViewWhenActionDirectionIsFromView()
        {
            this.controller.Initialize();
            this.controller.SetExecutableFileName("abc.def", ActionDirection.FromView);
            this.controller.RunApplicationUnderTest();
            this.view.SoapActions = null;
            this.controller.AddSoapAction(0, ActionDirection.FromController);
            this.controller.AddAssembly("abc.def");

            this.controller.RemoveAssembly(0, ActionDirection.FromView);

            Assert.AreEqual<int>(1, this.view.SelectedAssemblies.Count);
        }

        [TestMethod]
        public void VSWZVWSetAllSoapActionSetsListOnView()
        {
            this.controller.Initialize();
            this.controller.SetExecutableFileName("abc.def", ActionDirection.FromView);
            this.controller.RunApplicationUnderTest();
            this.view.SoapActions = null;

            this.controller.SetAllSoapActions();

            Assert.IsNotNull(this.view.SoapActions);
        }

        [TestMethod]
        public void VSWZVWClearAllSoapActionSetsListOnView()
        {
            this.controller.Initialize();
            this.controller.SetExecutableFileName("abc.def", ActionDirection.FromView);
            this.controller.RunApplicationUnderTest();
            this.view.SoapActions = null;

            this.controller.ClearAllSoapActions();

            Assert.IsNotNull(this.view.SoapActions);
        }

        [TestMethod]
        public void VSWZVWRunPageDefaultsToEnablingExecutableAndDisablingTraceFileSelection()
        {
            this.controller.Initialize();

            this.controller.Next();
            
            Assert.IsTrue(this.view.GetRadioState(WizardButton.ChooseExecutable));
            Assert.IsFalse(this.view.GetButtonState(WizardButton.RunExecutable));
            Assert.IsTrue(this.view.GetButtonState(WizardButton.BrowseExecutable));
            Assert.IsTrue(this.view.GetControlState(WizardControl.ExecutableFileName));
            Assert.AreEqual<string>(string.Empty, this.view.ExecutableFileName);

            Assert.IsFalse(this.view.GetRadioState(WizardButton.ChooseTraceFile));
            Assert.IsFalse(this.view.GetButtonState(WizardButton.ParseTraceFile));
            Assert.IsFalse(this.view.GetButtonState(WizardButton.BrowseTraceFile));
            Assert.IsFalse(this.view.GetControlState(WizardControl.TraceFileName));
            Assert.IsFalse(this.view.GetButtonState(WizardButton.BrowseTraceFile));
            Assert.AreEqual<string>(string.Empty, this.view.TraceFileName);
            Assert.IsFalse(this.view.GetButtonState(WizardButton.ChooseWcfClientTrace));
            Assert.IsFalse(this.view.GetButtonState(WizardButton.ChooseWcfServerTrace));
            Assert.IsFalse(this.view.GetButtonState(WizardButton.ChooseFiddlerTextTrace));
            Assert.IsTrue(this.view.GetRadioState(WizardButton.ChooseWcfClientTrace));
            Assert.IsFalse(this.view.GetRadioState(WizardButton.ChooseWcfServerTrace));
            Assert.IsFalse(this.view.GetRadioState(WizardButton.ChooseFiddlerTextTrace));
        }

        [TestMethod]
        public void VSWZVWRunPageSwitchToTraceFileDisablesExecutionOptionsAndEnablesTraceFileOptionsNextAndFinishDisabled()
        {
            this.NavigateToRunPageAndSelectTraceFileOptions();

            Assert.IsFalse(this.view.GetRadioState(WizardButton.ChooseExecutable));
            Assert.IsFalse(this.view.GetButtonState(WizardButton.RunExecutable));
            Assert.IsFalse(this.view.GetButtonState(WizardButton.BrowseExecutable));
            Assert.IsFalse(this.view.GetControlState(WizardControl.ExecutableFileName));
            Assert.AreEqual<string>(string.Empty, this.view.ExecutableFileName);

            Assert.IsTrue(this.view.GetRadioState(WizardButton.ChooseTraceFile));
            Assert.IsFalse(this.view.GetButtonState(WizardButton.ParseTraceFile));
            Assert.IsTrue(this.view.GetButtonState(WizardButton.BrowseTraceFile));
            Assert.IsTrue(this.view.GetControlState(WizardControl.TraceFileName));
            Assert.IsTrue(this.view.GetButtonState(WizardButton.BrowseTraceFile));
            Assert.AreEqual<string>(string.Empty, this.view.TraceFileName);
            Assert.IsTrue(this.view.GetButtonState(WizardButton.ChooseWcfClientTrace));
            Assert.IsTrue(this.view.GetButtonState(WizardButton.ChooseWcfServerTrace));
            Assert.IsTrue(this.view.GetButtonState(WizardButton.ChooseFiddlerTextTrace));
            Assert.IsTrue(this.view.GetRadioState(WizardButton.ChooseWcfClientTrace));
            Assert.IsFalse(this.view.GetRadioState(WizardButton.ChooseWcfServerTrace));
            Assert.IsFalse(this.view.GetRadioState(WizardButton.ChooseFiddlerTextTrace));

            Assert.IsTrue(this.view.GetButtonState(WizardButton.Previous));
            Assert.IsFalse(this.view.GetButtonState(WizardButton.Next));
            Assert.IsFalse(this.view.GetButtonState(WizardButton.Finish));
            Assert.IsTrue(this.view.GetButtonState(WizardButton.Cancel));
        }

        [TestMethod]
        public void VSWZVWRunPageSelectExecutableEnablesRunWhichIsThenDisabledOnSelectingTheTraceFileOption()
        {
            this.controller.Initialize();
            this.controller.Next();
            this.controller.SetExecutableFileName("abc.def", ActionDirection.FromController);

            this.controller.ChooseTraceFileOptions(ActionDirection.FromController);

            Assert.IsFalse(this.view.GetButtonState(WizardButton.RunExecutable));

            Assert.IsTrue(this.view.GetButtonState(WizardButton.Previous));
            Assert.IsFalse(this.view.GetButtonState(WizardButton.Next));
            Assert.IsFalse(this.view.GetButtonState(WizardButton.Finish));
            Assert.IsTrue(this.view.GetButtonState(WizardButton.Cancel));
        }

        [TestMethod]
        public void VSWZVWRunPageSelectExecutableEnablesRunWhichIsThenDisabledOnSelectingTheTraceFileOptionAndReenabledWhenSelectingTheExecutableOption()
        {
            this.controller.Initialize();
            this.controller.Next();
            this.controller.SetExecutableFileName("abc.def", ActionDirection.FromController);

            this.controller.ChooseTraceFileOptions(ActionDirection.FromController);
            this.controller.ChooseExecutableOptions(ActionDirection.FromController);

            Assert.IsTrue(this.view.GetButtonState(WizardButton.RunExecutable));

            Assert.IsTrue(this.view.GetButtonState(WizardButton.Previous));
            Assert.IsFalse(this.view.GetButtonState(WizardButton.Next));
            Assert.IsFalse(this.view.GetButtonState(WizardButton.Finish));
            Assert.IsTrue(this.view.GetButtonState(WizardButton.Cancel));
        }

        [TestMethod]
        public void VSWZVWRunButtonRemainsDisabledAfterSwitchingToTraceFileOptionAndBack()
        {
            this.NavigateToRunPageAndSelectTraceFileOptions();

            this.controller.ChooseExecutableOptions(ActionDirection.FromController);

            Assert.IsFalse(this.view.GetButtonState(WizardButton.RunExecutable));
        }

        [TestMethod]
        public void VSWZVWRunPageSelectTraceFileEnablesParseWhichIsThenDisabledOnSelectingTheExecutableOption()
        {
            this.controller.Initialize();
            this.controller.Next();
            this.controller.SetTraceFileName("abc.def", ActionDirection.FromController);

            this.controller.ChooseExecutableOptions(ActionDirection.FromController);

            Assert.IsFalse(this.view.GetButtonState(WizardButton.ParseTraceFile));

            Assert.IsTrue(this.view.GetButtonState(WizardButton.Previous));
            Assert.IsFalse(this.view.GetButtonState(WizardButton.Next));
            Assert.IsFalse(this.view.GetButtonState(WizardButton.Finish));
            Assert.IsTrue(this.view.GetButtonState(WizardButton.Cancel));
        }

        [TestMethod]
        public void VSWZVWRunPageSelectTraceFileEnablesParseWhichIsThenDisabledOnSelectingTheExecutableOptionAndReenabledWhenSelectingTheTraceFileOption()
        {
            this.controller.Initialize();
            this.controller.Next();
            this.controller.SetTraceFileName("abc.def", ActionDirection.FromController);

            this.controller.ChooseExecutableOptions(ActionDirection.FromController);
            this.controller.ChooseTraceFileOptions(ActionDirection.FromController);

            Assert.IsTrue(this.view.GetButtonState(WizardButton.ParseTraceFile));

            Assert.IsTrue(this.view.GetButtonState(WizardButton.Previous));
            Assert.IsFalse(this.view.GetButtonState(WizardButton.Next));
            Assert.IsFalse(this.view.GetButtonState(WizardButton.Finish));
            Assert.IsTrue(this.view.GetButtonState(WizardButton.Cancel));
        }

        [TestMethod]
        public void VSWZVWParseButtonRemainsDisabledAfterSwitchingToExecutableOptionAndBack()
        {
            this.controller.Initialize();
            this.controller.Next();

            this.controller.ChooseExecutableOptions(ActionDirection.FromController);
            this.controller.ChooseTraceFileOptions(ActionDirection.FromController);

            Assert.IsFalse(this.view.GetButtonState(WizardButton.ParseTraceFile));
        }

        [TestMethod]
        public void VSWZVWNextAndFinishButtonsRemainDisabledAfterChangingTraceFileType()
        {
            this.NavigateToRunPageAndSelectTraceFileOptions();
            this.controller.SetTraceFileName("abc.def", ActionDirection.FromController);

            this.controller.ChooseWcfClientTraceFile(ActionDirection.FromView);
            this.controller.ChooseWcfServerTraceFile(ActionDirection.FromView);

            Assert.IsFalse(this.view.GetButtonState(WizardButton.Next));
            Assert.IsFalse(this.view.GetButtonState(WizardButton.Finish));
        }

        [TestMethod]
        public void VSWZVWSettingEmptyTraceFileDisablesNext()
        {
            this.NavigateToRunPageAndSelectTraceFileOptions();

            this.controller.SetTraceFileName(string.Empty, ActionDirection.FromView);

            Assert.IsTrue(this.view.GetButtonState(WizardButton.Previous));
            Assert.IsFalse(this.view.GetButtonState(WizardButton.Next));
            Assert.IsFalse(this.view.GetButtonState(WizardButton.Finish));
            Assert.IsTrue(this.view.GetButtonState(WizardButton.Cancel));
        }

        [TestMethod]
        public void VSWZVWSettingNullTraceFileDisablesNext()
        {
            this.NavigateToRunPageAndSelectTraceFileOptions();

            this.controller.SetTraceFileName(null, ActionDirection.FromView);

            Assert.IsTrue(this.view.GetButtonState(WizardButton.Previous));
            Assert.IsFalse(this.view.GetButtonState(WizardButton.Next));
            Assert.IsFalse(this.view.GetButtonState(WizardButton.Finish));
            Assert.IsTrue(this.view.GetButtonState(WizardButton.Cancel));
        }

        [TestMethod]
        public void VSWZVWSettingTraceFileSwitchingToExecutableDisablesNext()
        {
            this.NavigateToRunPageAndSelectTraceFileOptions();
            this.controller.SetTraceFileName("WithMessageBodies.svclog", ActionDirection.FromView);

            this.controller.ChooseExecutableOptions(ActionDirection.FromController);

            Assert.IsTrue(this.view.GetButtonState(WizardButton.Previous));
            Assert.IsFalse(this.view.GetButtonState(WizardButton.Next));
            Assert.IsFalse(this.view.GetButtonState(WizardButton.Finish));
            Assert.IsTrue(this.view.GetButtonState(WizardButton.Cancel));
        }

        [TestMethod]
        public void VSWZVWTraceFileNameIsSetOnConfigDataAndViewWhenActionDirectionIsFromController()
        {
            this.controller.Initialize();
            this.controller.WizardData.TraceFile = "ServiceSideLog.svclog";
            this.view.TraceFileName = "ServiceSideLog.svclog";

            this.controller.SetTraceFileName("WithMessageBodies.svclog", ActionDirection.FromController);

            Assert.AreEqual<string>("WithMessageBodies.svclog", this.view.TraceFileName);
            Assert.AreEqual<string>("WithMessageBodies.svclog", this.controller.WizardData.TraceFile);
        }

        [TestMethod]
        public void VSWZVWTraceFileNameIsSetOnConfigDataButIsNotSetOnViewWhenActionDirectionIsFromView()
        {
            this.controller.Initialize();
            this.controller.WizardData.TraceFile = "ServiceSideLog.svclog";
            this.view.TraceFileName = "ServiceSideLog.svclog";

            this.controller.SetTraceFileName("WithMessageBodies.svclog", ActionDirection.FromView);

            Assert.AreEqual<string>("ServiceSideLog.svclog", this.view.TraceFileName);
            Assert.AreEqual<string>("WithMessageBodies.svclog", this.controller.WizardData.TraceFile);
        }

        [TestMethod]
        public void VSWZVWClientTraceFileTypeIsSetOnConfigDataAndViewWhenActionDirectionIsFromController()
        {
            this.controller.Initialize();
            this.controller.WizardData.Configuration.clientTrace = false;
            this.controller.WizardData.Configuration.serviceTrace = true;
            this.view.SetRadioState(WizardButton.ChooseWcfClientTrace, false);
            this.view.SetRadioState(WizardButton.ChooseWcfServerTrace, true);
            this.view.SetRadioState(WizardButton.ChooseFiddlerTextTrace, false);

            this.controller.ChooseWcfClientTraceFile(ActionDirection.FromController);

            Assert.IsTrue(this.controller.WizardData.Configuration.clientTrace);
            Assert.IsFalse(this.controller.WizardData.Configuration.serviceTrace);
            Assert.IsNull(this.controller.WizardData.Configuration.parser);
            Assert.IsTrue(this.view.GetRadioState(WizardButton.ChooseWcfClientTrace));
            Assert.IsFalse(this.view.GetRadioState(WizardButton.ChooseWcfServerTrace));
            Assert.IsFalse(this.view.GetRadioState(WizardButton.ChooseFiddlerTextTrace));
        }

        [TestMethod]
        public void VSWZVWClientTraceFileTypeIsSetOnConfigDataButIsNotSetOnViewWhenActionDirectionIsFromView()
        {
            this.controller.Initialize();
            this.controller.WizardData.Configuration.clientTrace = false;
            this.controller.WizardData.Configuration.serviceTrace = true;
            this.view.SetRadioState(WizardButton.ChooseWcfClientTrace, false);
            this.view.SetRadioState(WizardButton.ChooseWcfServerTrace, true);
            this.view.SetRadioState(WizardButton.ChooseFiddlerTextTrace, false);

            this.controller.ChooseWcfClientTraceFile(ActionDirection.FromView);

            Assert.IsTrue(this.controller.WizardData.Configuration.clientTrace);
            Assert.IsFalse(this.controller.WizardData.Configuration.serviceTrace);
            Assert.IsNull(this.controller.WizardData.Configuration.parser);
            Assert.IsFalse(this.view.GetRadioState(WizardButton.ChooseWcfClientTrace));
            Assert.IsTrue(this.view.GetRadioState(WizardButton.ChooseWcfServerTrace));
            Assert.IsFalse(this.view.GetRadioState(WizardButton.ChooseFiddlerTextTrace));
        }

        [TestMethod]
        public void VSWZVWServerTraceFileTypeIsSetOnConfigDataAndViewWhenActionDirectionIsFromController()
        {
            this.controller.Initialize();
            this.controller.WizardData.Configuration.clientTrace = true;
            this.controller.WizardData.Configuration.serviceTrace = false;
            this.view.SetRadioState(WizardButton.ChooseWcfClientTrace, true);
            this.view.SetRadioState(WizardButton.ChooseWcfServerTrace, false);
            this.view.SetRadioState(WizardButton.ChooseFiddlerTextTrace, false);

            this.controller.ChooseWcfServerTraceFile(ActionDirection.FromController);

            Assert.IsFalse(this.controller.WizardData.Configuration.clientTrace);
            Assert.IsTrue(this.controller.WizardData.Configuration.serviceTrace);
            Assert.IsNull(this.controller.WizardData.Configuration.parser);
            Assert.IsFalse(this.view.GetRadioState(WizardButton.ChooseWcfClientTrace));
            Assert.IsTrue(this.view.GetRadioState(WizardButton.ChooseWcfServerTrace));
            Assert.IsFalse(this.view.GetRadioState(WizardButton.ChooseFiddlerTextTrace));
        }

        [TestMethod]
        public void VSWZVWServerTraceFileTypeIsSetOnConfigDataButIsNotSetOnViewWhenActionDirectionIsFromView()
        {
            this.controller.Initialize();
            this.controller.WizardData.Configuration.clientTrace = true;
            this.controller.WizardData.Configuration.serviceTrace = false;
            this.view.SetRadioState(WizardButton.ChooseWcfClientTrace, true);
            this.view.SetRadioState(WizardButton.ChooseWcfServerTrace, false);
            this.view.SetRadioState(WizardButton.ChooseFiddlerTextTrace, false);

            this.controller.ChooseWcfServerTraceFile(ActionDirection.FromView);

            Assert.IsFalse(this.controller.WizardData.Configuration.clientTrace);
            Assert.IsTrue(this.controller.WizardData.Configuration.serviceTrace);
            Assert.IsNull(this.controller.WizardData.Configuration.parser);
            Assert.IsTrue(this.view.GetRadioState(WizardButton.ChooseWcfClientTrace));
            Assert.IsFalse(this.view.GetRadioState(WizardButton.ChooseWcfServerTrace));
            Assert.IsFalse(this.view.GetRadioState(WizardButton.ChooseFiddlerTextTrace));
        }

        [TestMethod]
        public void VSWZVWFiddlerTextTraceFileTypeIsSetOnConfigDataAndViewWhenActionDirectionIsFromController()
        {
            this.controller.Initialize();
            this.controller.WizardData.Configuration.clientTrace = true;
            this.controller.WizardData.Configuration.serviceTrace = false;
            this.view.SetRadioState(WizardButton.ChooseWcfClientTrace, true);
            this.view.SetRadioState(WizardButton.ChooseWcfServerTrace, false);
            this.view.SetRadioState(WizardButton.ChooseFiddlerTextTrace, false);

            this.controller.ChooseFiddlerTextTraceFile(ActionDirection.FromController);

            Assert.IsFalse(this.controller.WizardData.Configuration.clientTrace);
            Assert.IsFalse(this.controller.WizardData.Configuration.serviceTrace);
            Assert.IsNotNull(this.controller.WizardData.Configuration.parser);
            Assert.AreEqual<string>(TestHelper.LibraryAssembly, this.controller.WizardData.Configuration.parser.assembly);
            Assert.AreEqual<string>(typeof(FiddlerTextParser).FullName, this.controller.WizardData.Configuration.parser.type);
            Assert.IsFalse(this.view.GetRadioState(WizardButton.ChooseWcfClientTrace));
            Assert.IsFalse(this.view.GetRadioState(WizardButton.ChooseWcfServerTrace));
            Assert.IsTrue(this.view.GetRadioState(WizardButton.ChooseFiddlerTextTrace));
        }

        [TestMethod]
        public void VSWZVWFiddlerTextTraceFileTypeIsSetOnConfigDataButIsNotSetOnViewWhenActionDirectionIsFromView()
        {
            this.controller.Initialize();
            this.controller.WizardData.Configuration.clientTrace = true;
            this.controller.WizardData.Configuration.serviceTrace = false;
            this.view.SetRadioState(WizardButton.ChooseWcfClientTrace, true);
            this.view.SetRadioState(WizardButton.ChooseWcfServerTrace, false);
            this.view.SetRadioState(WizardButton.ChooseFiddlerTextTrace, false);

            this.controller.ChooseFiddlerTextTraceFile(ActionDirection.FromView);

            Assert.IsFalse(this.controller.WizardData.Configuration.clientTrace);
            Assert.IsFalse(this.controller.WizardData.Configuration.serviceTrace);
            Assert.IsNotNull(this.controller.WizardData.Configuration.parser);
            Assert.AreEqual<string>(TestHelper.LibraryAssembly, this.controller.WizardData.Configuration.parser.assembly);
            Assert.AreEqual<string>(typeof(FiddlerTextParser).FullName, this.controller.WizardData.Configuration.parser.type);
            Assert.IsTrue(this.view.GetRadioState(WizardButton.ChooseWcfClientTrace));
            Assert.IsFalse(this.view.GetRadioState(WizardButton.ChooseWcfServerTrace));
            Assert.IsFalse(this.view.GetRadioState(WizardButton.ChooseFiddlerTextTrace));
        }

        [TestMethod]
        public void VSWZVWParserInConfigDataIsSetBackToNullIfSwitchBackToNonFiddlerTraceType()
        {
            // Arrange
            this.controller.Initialize();
            this.controller.WizardData.Configuration.clientTrace = true;
            this.controller.WizardData.Configuration.serviceTrace = false;
            this.view.SetRadioState(WizardButton.ChooseWcfClientTrace, true);
            this.view.SetRadioState(WizardButton.ChooseWcfServerTrace, false);
            this.view.SetRadioState(WizardButton.ChooseFiddlerTextTrace, false);

            this.controller.ChooseFiddlerTextTraceFile(ActionDirection.FromController);

            // Act
            this.controller.ChooseWcfClientTraceFile(ActionDirection.FromController);

            Assert.IsNull(this.controller.WizardData.Configuration.parser);
        }

        [TestMethod]
        public void VSWZVWChooseExecutableIsSetOnViewWhenActionDirectionIsFromController()
        {
            this.controller.Initialize();
            this.view.SetRadioState(WizardButton.ChooseExecutable, false);
            this.view.SetRadioState(WizardButton.ChooseTraceFile, true);

            this.controller.ChooseExecutableOptions(ActionDirection.FromController);

            Assert.IsTrue(this.view.GetRadioState(WizardButton.ChooseExecutable));
            Assert.IsFalse(this.view.GetRadioState(WizardButton.ChooseTraceFile));
        }

        [TestMethod]
        public void VSWZVWChooseExecutableIsNotSetOnViewWhenActionDirectionIsFromView()
        {
            this.controller.Initialize();
            this.view.SetRadioState(WizardButton.ChooseExecutable, false);
            this.view.SetRadioState(WizardButton.ChooseTraceFile, true);

            this.controller.ChooseExecutableOptions(ActionDirection.FromView);

            Assert.IsFalse(this.view.GetRadioState(WizardButton.ChooseExecutable));
            Assert.IsTrue(this.view.GetRadioState(WizardButton.ChooseTraceFile));
        }

        [TestMethod]
        public void VSWZVWChooseTraceFileIsSetOnViewWhenActionDirectionIsFromController()
        {
            this.controller.Initialize();
            this.view.SetRadioState(WizardButton.ChooseExecutable, true);
            this.view.SetRadioState(WizardButton.ChooseTraceFile, false);

            this.controller.ChooseTraceFileOptions(ActionDirection.FromController);

            Assert.IsFalse(this.view.GetRadioState(WizardButton.ChooseExecutable));
            Assert.IsTrue(this.view.GetRadioState(WizardButton.ChooseTraceFile));
        }

        [TestMethod]
        public void VSWZVWChooseTraceFileIsNotSetOnViewWhenActionDirectionIsFromView()
        {
            this.controller.Initialize();
            this.view.SetRadioState(WizardButton.ChooseExecutable, true);
            this.view.SetRadioState(WizardButton.ChooseTraceFile, false);

            this.controller.ChooseTraceFileOptions(ActionDirection.FromView);

            Assert.IsTrue(this.view.GetRadioState(WizardButton.ChooseExecutable));
            Assert.IsFalse(this.view.GetRadioState(WizardButton.ChooseTraceFile));
        }

        private void SetupNoSoapActionsConfig(AssemblyType a)
        {
            this.noSoapActionsConfig = new WcfUnitConfiguration();
            this.noSoapActionsConfig.testMethodMode = TestMethodMode.ScenarioMethodOnly;
            this.noSoapActionsConfig.operationTimerMode = OperationTimerMode.IncludeOperationTimers;
            this.noSoapActionsConfig.assembly = new AssemblyType[] { a };
            this.noSoapActionsConfig.soapActions = new WcfUnitConfigurationSoapActions();
            this.noSoapActionsConfig.soapActions.soapActionMode = SoapActionMode.Include;
            this.noSoapActionsConfig.soapActions.soapAction = new SoapActionType[0];
        }

        private AssemblyType SetupNoAssembliesConfig()
        {
            this.noAssembliesConfig = new WcfUnitConfiguration();
            this.noAssembliesConfig.testMethodMode = TestMethodMode.ScenarioMethodOnly;
            this.noAssembliesConfig.operationTimerMode = OperationTimerMode.IncludeOperationTimers;
            AssemblyType a = new AssemblyType();

            // a.fileName = "dummy.dll";
            // populatedConfig.assembly = new assemblyType[] { a };
            this.noAssembliesConfig.soapActions = new WcfUnitConfigurationSoapActions();
            this.noAssembliesConfig.soapActions.soapActionMode = SoapActionMode.Include;
            SoapActionType action1 = new SoapActionType();
            action1.action = "action1";
            action1.Selected = true;
            SoapActionType action2 = new SoapActionType();
            action2.action = "action2";
            action2.Selected = true;
            this.noAssembliesConfig.soapActions.soapAction = new SoapActionType[] { action1, action2 };
            return a;
        }

        private void CheckSoapActions(string[] expectedSelected, string[] expectedUnselected)
        {
            this.CheckSoapActions(expectedSelected, expectedUnselected, this.view.SoapActions);
            this.CheckSoapActions(expectedSelected, expectedUnselected, this.controller.WizardData.Configuration.soapActions);
        }

        private void CheckSoapActions(string[] expectedSelected, string[] expectedUnselected, WcfUnitConfigurationSoapActions actions)
        {
            Assert.AreEqual<int>(expectedSelected.Length + expectedUnselected.Length, actions.soapAction.Length);
            Assert.AreEqual<int>(expectedUnselected.Length, actions.UnselectedCount);
            Assert.AreEqual<int>(expectedSelected.Length, actions.SelectedCount);
            Dictionary<string, SoapActionType> actionDictionary = this.BuildSoapActionDictionary(actions.soapAction);
            foreach (string s in expectedSelected)
            {
                Assert.IsTrue(actionDictionary[s].Selected);
            }

            foreach (string s in expectedUnselected)
            {
                Assert.IsFalse(actionDictionary[s].Selected);
            }
        }

        private void CheckOptions(OperationTimerMode operationTimerMode, TestMethodMode testMethodMode)
        {
            Assert.AreEqual<OperationTimerMode>(operationTimerMode, this.controller.WizardData.Configuration.operationTimerMode);
            Assert.AreEqual<bool>(operationTimerMode == OperationTimerMode.IncludeOperationTimers, this.view.IncludeOperationTimers);

            Assert.AreEqual<TestMethodMode>(testMethodMode, this.controller.WizardData.Configuration.testMethodMode);
            Assert.AreEqual<bool>(testMethodMode == TestMethodMode.IncludeIndividualOperations, this.view.IncludeUnitTestPerOperation);
        }

        private void CheckAssemblies(params AssemblyType[] expectedAssemblies)
        {
            this.CheckAssemblies(expectedAssemblies, this.controller.WizardData.Configuration.assembly);
            this.CheckAssemblies(expectedAssemblies, this.view.SelectedAssemblies.ToArray());
        }

        private void CheckAssemblies(AssemblyType[] expectedAssemblies, AssemblyType[] actualAssemblies)
        {
            if (actualAssemblies == null)
            {
                Assert.AreEqual<int>(expectedAssemblies.Length, 0);
            }
            else
            {
                Assert.AreEqual<int>(expectedAssemblies.Length, actualAssemblies.Length);
            }

            for (int i = 0; i < expectedAssemblies.Length; i++)
            {
                Assert.AreEqual<string>(expectedAssemblies[i].fileName, actualAssemblies[i].fileName);
            }
        }

        private Dictionary<string, SoapActionType> BuildSoapActionDictionary(SoapActionType[] actionArray)
        {
            Dictionary<string, SoapActionType> actions = new Dictionary<string, SoapActionType>();
            foreach (SoapActionType sat in actionArray)
            {
                actions.Add(sat.action, sat);
            }

            return actions;
        }

        private void CheckSoapActionSelectionButtons()
        {
            Assert.AreEqual<bool>(this.view.SoapActions.UnselectedCount > 0, this.view.GetButtonState(WizardButton.SelectAllSoapActions));
            Assert.AreEqual<bool>(this.view.SoapActions.SelectedCount > 0, this.view.GetButtonState(WizardButton.ClearAllSoapActions));
        }

        private void CheckPagesEnabledUpto(WizardPage highestEnabledPage)
        {
            foreach (WizardPage p in System.Enum.GetValues(typeof(WizardPage)))
            {
                if (p <= highestEnabledPage)
                {
                    Assert.IsTrue(this.view.GetPageState(p), string.Format(CultureInfo.InvariantCulture, "{0} should be enabled but it is disabled", p));
                }
                else
                {
                    Assert.IsFalse(this.view.GetPageState(p), string.Format(CultureInfo.InvariantCulture, "{0} should be disabled but it is enabled", p));
                }
            }
        }

        private void NavigateToRunPageAndSelectTraceFileOptions()
        {
            this.controller.Initialize();
            this.controller.Next();
            this.controller.ChooseTraceFileOptions(ActionDirection.FromController);
        }

        private void SetConfigToReturn(WcfUnitConfiguration config)
        {
            this.mgr.ConfigurationToReturn = config;
            this.uiLogic.ConfigurationToReturn = config;
        }
    }
}
