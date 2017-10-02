//---------------------------------------------------------------------
// <copyright file="FlowControllerTestsBase.cs" company="Microsoft">
//    Copyright © ALM | DevOps Ranger Contributors. All rights reserved.
//    This code is licensed under the MIT License.
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF 
//    ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
//    TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
//    PARTICULAR PURPOSE AND NONINFRINGEMENT.
// </copyright>
// <summary>The FlowControllerTestsBase type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.Word4Tfs.Library.Test.UnitTest
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Practices.Unity;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Word4Tfs.Library.Model;
    using Microsoft.Word4Tfs.Library.Model.Windows;
    using Microsoft.Word4Tfs.Library.Model.Word;
    using Microsoft.Word4Tfs.Library.Presenter;
    using Moq;

    /// <summary>
    /// Provides common facilities needed by flow controller tests that use an already initialised flow controller.
    /// </summary>
    [TestClass]
    public class FlowControllerTestsBase
    {
        /// <summary>
        /// The path to the template in the roaming profile.
        /// </summary>
        protected const string RoamingTemplateDir = @"Z:\Test\Microsoft\Templates";

        /// <summary>
        /// Gets or sets the Unity container to be used for Dependency Injection
        /// </summary>
        protected IUnityContainer Container { get; set; }

        /// <summary>
        /// Gets or sets the mock ribbon presenter.
        /// </summary>
        protected Mock<ITeamRibbonPresenter> MockRibbonPresenter { get; set; }

        /// <summary>
        /// Gets or sets the mock team project picker presenter.
        /// </summary>
        protected Mock<ITeamProjectPickerPresenter> MockTeamProjectPickerPresenter { get; set; }

        /// <summary>
        /// Gets or sets the mock work item query and layout picker presenter.
        /// </summary>
        protected Mock<IWorkItemQueryAndLayoutPickerWizardPresenter> MockWorkItemQueryAndLayoutPickerWizardPresenter { get; set; }

        /// <summary>
        /// Gets or sets the mock team project document used as the active document.
        /// </summary>
        protected Mock<ITeamProjectDocument> MockTeamProjectDocument { get; set; }

        /// <summary>
        /// Gets or sets the mock team project document manager.
        /// </summary>
        protected Mock<ITeamProjectDocumentManager> MockTeamProjectDocumentManager { get; set; }

        /// <summary>
        /// Gets or sets the mock file object.
        /// </summary>
        protected Mock<IFile> MockFile { get; set; }

        /// <summary>
        /// Gets or sets the mock directory object.
        /// </summary>
        protected Mock<IDirectory> MockDirectory { get; set; }

        /// <summary>
        /// Gets or sets the mock Word Application.
        /// </summary>
        protected Mock<IWordApplication> MockApplication { get; set; }

        /// <summary>
        /// Gets or sets the mock settings.
        /// </summary>
        protected Mock<ISettings> MockSettings { get; set; }

        /// <summary>
        /// Gets or sets the mock layout designer presenter.
        /// </summary>
        protected Mock<ILayoutDesignerPresenter> MockLayoutDesignerPresenter { get; set; }

        /// <summary>
        /// Gets or sets the scheduler used for asynchronous operations.
        /// </summary>
        protected DeterministicTaskScheduler Scheduler { get; set; }

        /// <summary>
        /// Gets or sets the captured callback passed to Team Ribbon Presenter by the flow controller.
        /// </summary>
        protected Func<Uri> CapturedRebindCallback { get; set; }

        /// <summary>
        /// Gets or sets the flow controller to be tested.
        /// </summary>
        protected FlowController Sut { get; set; }

        /// <summary>
        /// Initializes the test, creating an initialised flow controller.
        /// </summary>
        [TestInitialize]
        public void InitializeTest()
        {
            this.Container = new UnityContainer();
            this.Container.RegisterInstance<ILogger>(new Logger());
            this.Scheduler = new DeterministicTaskScheduler();
            this.Container.RegisterInstance(typeof(TaskScheduler), this.Scheduler);
            this.MockRibbonPresenter = TestHelper.CreateAndRegisterMock<ITeamRibbonPresenter>(this.Container);
            this.MockRibbonPresenter.Setup(p => p.Initialise(It.IsAny<Func<Uri>>())).Callback((Func<Uri> rebindCallback) => this.CapturedRebindCallback = rebindCallback);
            this.MockTeamProjectPickerPresenter = TestHelper.CreateAndRegisterMock<ITeamProjectPickerPresenter>(this.Container);
            this.MockWorkItemQueryAndLayoutPickerWizardPresenter = TestHelper.CreateAndRegisterMock<IWorkItemQueryAndLayoutPickerWizardPresenter>(this.Container);
            this.MockTeamProjectDocumentManager = TestHelper.CreateAndRegisterMock<ITeamProjectDocumentManager>(this.Container);
            this.MockTeamProjectDocument = TestHelper.CreateAndRegisterMock<ITeamProjectDocument>(this.Container);
            this.MockTeamProjectDocument.Setup(doc => doc.IsInsertable).Returns(true);
            this.MockFile = TestHelper.CreateAndRegisterMock<IFile>(this.Container);
            this.MockDirectory = TestHelper.CreateAndRegisterMock<IDirectory>(this.Container);
            this.MockApplication = TestHelper.CreateAndRegisterMock<IWordApplication>(this.Container);
            this.MockSettings = TestHelper.CreateAndRegisterMock<ISettings>(this.Container);
            this.MockLayoutDesignerPresenter = TestHelper.CreateAndRegisterMock<ILayoutDesignerPresenter>(this.Container);

            this.MockTeamProjectDocumentManager.Setup(manager => manager.ActiveContainer).Callback(() => System.Console.WriteLine("Active container call")).Returns(this.Container);
            this.MockTeamProjectDocumentManager.Setup(manager => manager.ActiveDocument).Returns(this.MockTeamProjectDocument.Object);

            this.MockApplication.Setup(app => app.UserTemplatesPath).Returns(RoamingTemplateDir);

            this.Sut = this.Container.Resolve<FlowController>();

            this.Sut.Initialise();
        }

        /// <summary>
        /// Sets up a dummy null project that has been picked.
        /// </summary>
        /// <returns>The dummy picked project.</returns>
        protected ITeamProject SetupDummyNoPickedProject()
        {
            ITeamProject project = null;
            Uri uri = null;
            this.MockTeamProjectPickerPresenter.Setup(picker => picker.ChooseTeamProject()).Returns(project);
            this.MockTeamProjectPickerPresenter.Setup(picker => picker.ChooseTeamProjectCollection()).Returns(uri);
            return project;
        }

        /// <summary>
        /// Sets up a dummy project that has been picked.
        /// </summary>
        /// <returns>The dummy picked project.</returns>
        protected Mock<ITeamProject> SetupDummyPickedProject()
        {
            Mock<ITeamProject> project = TestHelper.CreateMockTeamProject("[System.Id]", "[System.Area]", "[System.Title]");
            this.MockTeamProjectPickerPresenter.Setup(picker => picker.ChooseTeamProject()).Returns(project.Object);
            this.MockTeamProjectPickerPresenter.Setup(picker => picker.ChooseTeamProjectCollection()).Returns(project.Object.TeamProjectInformation.CollectionUri);

            return project;
        }

        /// <summary>
        /// Asserts that work items were never saved.
        /// </summary>
        protected void AssertWorkItemsNotSaved()
        {
            this.MockTeamProjectDocument.Verify(document => document.SaveWorkItems(It.IsAny<WorkItemTree>(), It.IsAny<string[]>(), It.IsAny<CancellationToken>()), Times.Never());
        }

        /// <summary>
        /// Asserts that work items were saved.
        /// </summary>
        /// <param name="workItems">The work items that are expected to have been saved.</param>
        /// <param name="fields">The fields to be included in the list of fields saved.</param>
        protected void AssertWorkItemsSaved(WorkItemTree workItems, string[] fields)
        {
            this.MockTeamProjectDocument.Verify(document => document.SaveWorkItems(workItems, fields, It.IsAny<CancellationToken>()), Times.Once());
        }

        /// <summary>
        /// Asserts that an error is displayed by the ribbon.
        /// </summary>
        /// <param name="expectedMessage">The expected message.</param>
        protected void AssertErrorMessageDisplayed(string expectedMessage)
        {
            this.MockRibbonPresenter.Verify(presenter => presenter.DisplayError(TestHelper.MessageStartsWith(expectedMessage + Environment.NewLine), string.Empty), Times.Once());
        }

        /// <summary>
        /// Asserts that a single error is displayed by the ribbon.
        /// </summary>
        /// <param name="expectedMessage">The expected message.</param>
        protected void AssertSingleErrorMessageDisplayed(string expectedMessage)
        {
            this.MockRibbonPresenter.Verify(presenter => presenter.DisplayError(expectedMessage, string.Empty), Times.Once());
        }

        /// <summary>
        /// Asserts that a single error is displayed by the ribbon.
        /// </summary>
        /// <param name="expectedMessage">The expected message.</param>
        /// <param name="details">The expected details message.</param>
        protected void AssertSingleErrorMessageDisplayed(string expectedMessage, string details)
        {
            this.MockRibbonPresenter.Verify(presenter => presenter.DisplayError(expectedMessage, details), Times.Once());
        }
    }
}
