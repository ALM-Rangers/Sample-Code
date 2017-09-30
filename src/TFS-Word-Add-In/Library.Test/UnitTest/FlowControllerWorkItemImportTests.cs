//---------------------------------------------------------------------
// <copyright file="FlowControllerWorkItemImportTests.cs" company="Microsoft">
//    Copyright © ALM | DevOps Ranger Contributors. All rights reserved.
//    This code is licensed under the MIT License.
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF 
//    ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
//    TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
//    PARTICULAR PURPOSE AND NONINFRINGEMENT.
// </copyright>
// <summary>The FlowControllerWorkItemImportTests type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.Word4Tfs.Library.Test.UnitTest
{
    using System;
    using System.Threading;
    using Microsoft.TeamFoundation;
    using Microsoft.TeamFoundation.WorkItemTracking.Client;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Word4Tfs.Library.Data;
    using Microsoft.Word4Tfs.Library.Model;
    using Moq;

    /// <summary>
    /// Tests the flow controller work item import functionality.
    /// </summary>
    [TestClass]
    public class FlowControllerWorkItemImportTests : FlowControllerTestsBase
    {
        /// <summary>
        /// Tests that an import event from the ribbon presenter uses the manager's active container.
        /// </summary>
        /// <remarks>
        /// This has to be an indirect test and is consequently a little fragile.
        /// </remarks>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void ImportEventFromTheRibbonPresenterCreatesTheProjectPickerFromTheManagerActiveContainer()
        {
            // Arrange
            Mock<ITeamProject> mockProject = this.SetupGoodImportDialogSequence();
            WorkItemTree workItems = new WorkItemTree();
            mockProject.Setup(project => project.QueryRunner.QueryForWorkItems(It.IsAny<QueryDefinition>(), It.IsAny<CancellationToken>())).Returns(workItems);

            // Act
            this.MockRibbonPresenter.Raise(ribbonPresenter => ribbonPresenter.Import += null, EventArgs.Empty);

            // Assert
            int expected = 3;
#if DEBUG
            expected++;
#endif
            this.MockTeamProjectDocumentManager.Verify(manager => manager.ActiveContainer, Times.Exactly(expected), "The number of times the active container is used does not reflect the number of resolutions the presenter must perform on the document-specific child container");
        }

        /// <summary>
        /// Tests that an import event from the ribbon presenter invokes the project picker.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void ImportEventFromTheRibbonPresenterInvokesTheProjectPicker()
        {
            // Act
            this.MockRibbonPresenter.Raise(ribbonPresenter => ribbonPresenter.Import += null, EventArgs.Empty);

            // Assert
            this.MockTeamProjectPickerPresenter.Verify(picker => picker.ChooseTeamProject(), Times.Once());
        }

        /// <summary>
        /// Tests that if team project data is obtained that the flow controller initialises the query and layout picker.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void InitialisesQueryAndLayoutPickerAfterGettingTeamProjectData()
        {
            // Arrange
            this.SetupDummyPickedProject();

            // Act
            this.MockRibbonPresenter.Raise(ribbonPresenter => ribbonPresenter.Import += null, EventArgs.Empty);

            // Assert
            this.MockWorkItemQueryAndLayoutPickerWizardPresenter.Verify(picker => picker.Initialise(), Times.Once());
        }

        /// <summary>
        /// Tests that if team project data is obtained that the flow controller moves on to the query and layout picker.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void MovesOnToQueryAndLayoutPickerAfterGettingTeamProjectData()
        {
            // Arrange
            Mock<ITeamProject> mockProject = this.SetupDummyPickedProject();

            // Act
            this.MockRibbonPresenter.Raise(ribbonPresenter => ribbonPresenter.Import += null, EventArgs.Empty);

            // Assert
            this.MockWorkItemQueryAndLayoutPickerWizardPresenter.Verify(picker => picker.Start(), Times.Once());
        }

        /// <summary>
        /// Tests that if team project data is not obtained that the flow controller does not invoke the query and layout picker.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void QueryAndLayoutPickerNotInvokedIfTeamProjectDataIsNotObtained()
        {
            // Arrange
            ITeamProject project = this.SetupDummyNoPickedProject();

            // Act
            this.MockRibbonPresenter.Raise(ribbonPresenter => ribbonPresenter.Import += null, EventArgs.Empty);

            // Assert
            this.MockWorkItemQueryAndLayoutPickerWizardPresenter.Verify(picker => picker.Start(), Times.Never());
        }

        /// <summary>
        /// Tests that Import maps the chosen work items into the document using the chosen layout.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void ImportMapsChosenWorkItemsInDocumentUsingChosenLayout()
        {
            // Arrange
            this.SetupGoodImportDialogSequenceWithAnyWorkItems();

            // Act
            this.RunTheImport();

            // Assert
            this.MockTeamProjectDocument.Verify(document => document.MapWorkItemsIntoDocument(It.IsAny<LayoutInformation>(), It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Once());
        }

        /// <summary>
        /// Tests that Import writes the chosen project, query, layout and work items to the document.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void ImportWritesProjectAndQueryAndLayoutAndWorkItemsToTheDocument()
        {
            // Arrange
            string[] fields = new string[] { "A.B", "A.C" };
            Mock<ITeamProject> mockProject = this.SetupDummyPickedProject();
            Mock<ITeamProjectQuery> mockTeamProjectQuery = new Mock<ITeamProjectQuery>();
            mockProject.Setup(teamProject => teamProject.QueryRunner).Returns(mockTeamProjectQuery.Object);
            this.SetupDummyPickedQueryAndLayout(mockProject, fields);
            WorkItemTree workItems = new WorkItemTree();
            mockTeamProjectQuery.Setup(q => q.QueryForWorkItems(It.IsAny<QueryDefinition>(), It.IsAny<CancellationToken>())).Returns(workItems);

            // Act
            this.RunTheImport();

            // Assert
            this.AssertProjectInformationSaved();
            this.AssertQueryAndLayoutInformationSaved();
            this.AssertWorkItemsSaved(workItems, fields);
            this.AssertNoErrrorsDisplayed();
        }

        /// <summary>
        /// Tests that Import does not save anything and displays error if it can't get the query runner
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void ImportSavesNothingIfCannotGetQueryRunner()
        {
            // Arrange
            Mock<ITeamProject> mockProject = this.SetupGoodImportDialogSequence();
            mockProject.Setup(project => project.QueryRunner).Throws(new TeamFoundationServerException("testmessage"));

            // Act
            this.RunTheImport();

            // Assert
            this.AssertProjectInformationNotSaved();
            this.AssertQueryAndLayoutInformationNotSaved();
            this.AssertWorkItemsNotSaved();
            this.AssertErrorMessageDisplayed("testmessage");
        }

        /// <summary>
        /// Tests that Import does not save anything and displays error if the query runner fails.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void ImportSavesNothingIfQueryRunnerFails()
        {
            // Arrange
            Mock<ITeamProject> mockProject = this.SetupGoodImportDialogSequence();
            mockProject.Setup(project => project.QueryRunner.QueryForWorkItems(It.IsAny<QueryDefinition>(), It.IsAny<CancellationToken>())).Throws(new TeamFoundationServerException("testmessage"));

            // Act
            this.RunTheImport();

            // Assert
            this.AssertProjectInformationNotSaved();
            this.AssertQueryAndLayoutInformationNotSaved();
            this.AssertWorkItemsNotSaved();
            this.AssertErrorMessageDisplayed("testmessage");
        }

        /// <summary>
        /// Does not save project and query and layout information if project not selected.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void DoesNotSaveProjectAndQueryAndLayoutInformationInDocumentIfProjectNotSelected()
        {
            // Arrange
            this.SetupDummyNoPickedProject();

            // Act
            this.MockRibbonPresenter.Raise(ribbonPresenter => ribbonPresenter.Import += null, EventArgs.Empty);

            // Assert
            this.AssertProjectInformationNotSaved();
            this.AssertQueryAndLayoutInformationNotSaved();
        }

        /// <summary>
        /// Does not save project and query and layout information if work items and layout not selected.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void DoesNotSaveProjectAndQueryInformationInDocumentIfWorkItemsAndLayoutNotSelected()
        {
            // Arrange
            Mock<ITeamProject> mockProject = this.SetupDummyPickedProject();
            this.SetupDummyNoPickedQueryAndLayout(mockProject);

            // Act
            this.MockRibbonPresenter.Raise(ribbonPresenter => ribbonPresenter.Import += null, EventArgs.Empty);

            // Assert
            this.AssertProjectInformationNotSaved();
            this.AssertQueryAndLayoutInformationNotSaved();
        }

        /// <summary>
        /// Does not map the work items into the document if query and layout is not selected.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void DoesNotMapWorkItemsIntoDocumentIfQueryAndLayoutNotSelected()
        {
            // Arrange
            Mock<ITeamProject> mockProject = this.SetupDummyPickedProject();
            this.SetupDummyNoPickedQueryAndLayout(mockProject);

            // Act
            this.MockRibbonPresenter.Raise(ribbonPresenter => ribbonPresenter.Import += null, EventArgs.Empty);

            // Assert
            this.MockTeamProjectDocument.Verify(document => document.MapWorkItemsIntoDocument(It.IsAny<LayoutInformation>(), It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never());
        }

        /// <summary>
        /// Updates ribbon state after importing from a Team Project
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void UpdatesRibbonStateAfterImportingFromTeamProject()
        {
            // Arrange
            this.SetupGoodImportDialogSequenceWithAnyWorkItems();

            // Act
            this.MockRibbonPresenter.Raise(ribbonPresenter => ribbonPresenter.Import += null, EventArgs.Empty);

            // Assert
            this.MockRibbonPresenter.Verify(ribbonPresenter => ribbonPresenter.UpdateState(), Times.Once());
        }

        /// <summary>
        /// Updates ribbon state after import from a Team Project is cancelled at the project selection
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void UpdatesRibbonStateAfterImportFromTeamProjectIsCancelledAtTheProjectSelection()
        {
            // Arrange
            this.SetupDummyNoPickedProject();

            // Act
            this.MockRibbonPresenter.Raise(ribbonPresenter => ribbonPresenter.Import += null, EventArgs.Empty);

            // Assert
            this.MockRibbonPresenter.Verify(ribbonPresenter => ribbonPresenter.UpdateState(), Times.Once());
        }

        /// <summary>
        /// Updates ribbon state after import from a Team Project is cancelled at the query and layout selection
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void UpdatesRibbonStateAfterImportFromTeamProjectIsCancelledAtTheQueryAndLayoutSelection()
        {
            // Arrange
            Mock<ITeamProject> mockProject = this.SetupDummyPickedProject();
            this.SetupDummyNoPickedQueryAndLayout(mockProject);

            // Act
            this.MockRibbonPresenter.Raise(ribbonPresenter => ribbonPresenter.Import += null, EventArgs.Empty);

            // Assert
            this.MockRibbonPresenter.Verify(ribbonPresenter => ribbonPresenter.UpdateState(), Times.Once());
        }

        /// <summary>
        /// Tests that a second import does not invoke the project picker but goes straight to the query and layout picker.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void SecondImportEventFromTheRibbonPresenterInvokesQueryAndLayoutPickerBypassingProjectPicker()
        {
            // Arrange
            this.MockTeamProjectDocument.Setup(doc => doc.IsConnected).Returns(true);
            Mock<ITeamProject> mockProject = TestHelper.CreateMockTeamProject("[System.Id]", "[System.Area]", "[System.Title]");
            this.MockTeamProjectDocument.Setup(doc => doc.TeamProject).Returns(mockProject.Object);

            // Act
            this.MockRibbonPresenter.Raise(ribbonPresenter => ribbonPresenter.Import += null, EventArgs.Empty);

            // Assert
            this.MockTeamProjectPickerPresenter.Verify(picker => picker.ChooseTeamProject(), Times.Never());
            this.MockWorkItemQueryAndLayoutPickerWizardPresenter.Verify(picker => picker.Initialise(), Times.Once());
            this.MockWorkItemQueryAndLayoutPickerWizardPresenter.Verify(picker => picker.Start(), Times.Once());
        }

        /// <summary>
        /// Tests that a second import does not save the project information.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void SecondImportEventFromTheRibbonPresenterDoesNotSaveProjectInformation()
        {
            // Arrange
            this.MockTeamProjectDocument.Setup(doc => doc.IsConnected).Returns(true);
            Mock<ITeamProject> mockProject = this.SetupGoodImportDialogSequenceWithAnyWorkItems();
            this.MockTeamProjectDocument.Setup(doc => doc.TeamProject).Returns(mockProject.Object);

            // Act
            this.MockRibbonPresenter.Raise(ribbonPresenter => ribbonPresenter.Import += null, EventArgs.Empty);

            // Assert
            this.MockTeamProjectPickerPresenter.Verify(picker => picker.SaveTeamProject(), Times.Never());
        }

        /// <summary>
        /// Tests that a cancellable operation dialog is started, updated and closed when doing a good import.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void GoodImportDisplaysCancellableOperationDialogInCorrectSequence()
        {
            // Arrange
            int callSequence = 1;
            this.SetupGoodImportDialogSequenceWithAnyWorkItems();
            this.MockRibbonPresenter.Setup(p => p.StartCancellableOperation(FlowControllerResources.StartQueryExecution, It.IsAny<CancellationTokenSource>())).Callback(() => Assert.AreEqual<int>(1, callSequence++, "StartCancellableOperation must be called first"));
            this.MockRibbonPresenter.Setup(p => p.UpdateCancellableOperation(FlowControllerResources.AddingWorkItemsToDocument)).Callback(() => Assert.AreEqual<int>(2, callSequence++, "UpdateCancellableOperation must be called second"));
            this.MockRibbonPresenter.Setup(p => p.EndCancellableOperation()).Callback(() => Assert.AreEqual<int>(3, callSequence++, "EndCancellableOperation must be called last"));

            // Act
            this.MockRibbonPresenter.Raise(ribbonPresenter => ribbonPresenter.Import += null, EventArgs.Empty);

            // Assert
            this.MockRibbonPresenter.Verify(p => p.StartCancellableOperation(FlowControllerResources.StartQueryExecution, It.IsAny<CancellationTokenSource>()), Times.Once(), "Start must be called synchronously as it is a blocking call");
            this.Scheduler.Start();
            this.Scheduler.Wait();
            this.AssertCancellableOperationProcessedNormally();
        }

        /// <summary>
        /// Tests that a cancellable operation dialog is started and closed when there is an error running the query.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void ImportDisplaysCancellableOperationDialogInCorrectSequenceWhenThereIsAnErrorRunningTheQuery()
        {
            // Arrange
            int callSequence = 1;
            Mock<ITeamProject> mockProject = this.SetupGoodImportDialogSequence();
            mockProject.Setup(project => project.QueryRunner.QueryForWorkItems(It.IsAny<QueryDefinition>(), It.IsAny<CancellationToken>())).Throws(new TeamFoundationServerException("testmessage"));
            this.MockRibbonPresenter.Setup(p => p.StartCancellableOperation(FlowControllerResources.StartQueryExecution, It.IsAny<CancellationTokenSource>())).Callback(() => { Console.WriteLine("start cbk"); Assert.AreEqual<int>(1, callSequence++, "StartCancellableOperation must be called first"); });
            this.MockRibbonPresenter.Setup(p => p.EndCancellableOperation()).Callback(() => { Console.WriteLine("update cbk"); Assert.AreEqual<int>(2, callSequence++, "EndCancellableOperation must be called last"); });

            // Act
            this.RunTheImport();

            // Assert
            this.AssertCancellableOperationStartedButFailedAtTheQueryStep();
        }

        /// <summary>
        /// Tests that cancelling the initial query stops the import.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void CancellingInitialQueryStopsImport()
        {
            // Arrange
            this.SetupGoodImportDialogSequenceWithAnyWorkItems();
            this.MockRibbonPresenter
                .Setup(p => p.StartCancellableOperation(FlowControllerResources.StartQueryExecution, It.IsAny<CancellationTokenSource>()))
                .Callback((string message, CancellationTokenSource cts) => cts.Cancel());

            // Act
            this.RunTheImport();

            // Assert
            this.AssertCancellableOperationStartedButFailedAtTheQueryStep();
            this.AssertNoErrrorsDisplayed();
        }

        /// <summary>
        /// Tests that cancelling the after query stops the import.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void CancellingAfterSaveStopsImport()
        {
            // Arrange
            this.SetupGoodImportDialogSequenceWithAnyWorkItems();
            CancellationTokenSource capturedCts = null;
            this.MockRibbonPresenter
                .Setup(p => p.StartCancellableOperation(FlowControllerResources.StartQueryExecution, It.IsAny<CancellationTokenSource>()))
                .Callback((string message, CancellationTokenSource cts) => { capturedCts = cts; });
            this.MockRibbonPresenter
                .Setup(p => p.UpdateCancellableOperation(FlowControllerResources.AddingWorkItemsToDocument))
                .Callback((string message) => capturedCts.Cancel());

            // Act
            this.RunTheImport();

            // Assert
            this.AssertCancellableOperationStartedButFailedAtTheAddStep();
            this.AssertNoErrrorsDisplayed();
        }

        /// <summary>
        /// Tests that an error is displayed if the document is not insertable at the time import is selected.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void ImportDisplaysErrorIfDocumentNotInsertable()
        {
            // Arrange
            this.MockTeamProjectDocument.Setup(doc => doc.IsInsertable).Returns(false);

            // Act)
            this.RunTheImport();

            // Assert
            this.AssertSingleErrorMessageDisplayed("Cannot insert new work items at the current location.");
        }

        /// <summary>
        /// Sets up a dialog sequence for importing that confirms all the dialogues.
        /// </summary>
        /// <returns>The mock project.</returns>
        private Mock<ITeamProject> SetupGoodImportDialogSequence()
        {
            Mock<ITeamProject> mockProject = this.SetupDummyPickedProject();
            this.SetupDummyPickedQueryAndLayout(mockProject);
 
            return mockProject;
        }

        /// <summary>
        /// Sets up a dialog sequence for importing that confirms all the dialogues and returns a dummy set of work items.
        /// </summary>
        /// <returns>
        /// The mock team project that is created.
        /// </returns>
        private Mock<ITeamProject> SetupGoodImportDialogSequenceWithAnyWorkItems()
        {
            Mock<ITeamProject> mockProject = this.SetupGoodImportDialogSequence();
            WorkItemTree workItems = new WorkItemTree();
            mockProject.Setup(project => project.QueryRunner.QueryForWorkItems(It.IsAny<QueryDefinition>(), It.IsAny<CancellationToken>())).Returns(workItems);
            return mockProject;
        }

        /// <summary>
        /// Sets up a dummy null query and layout for the given project.
        /// </summary>
        /// <param name="project">The mock project to create the query and layout for.</param>
        private void SetupDummyNoPickedQueryAndLayout(Mock<ITeamProject> project)
        {
            QueryAndLayoutInformation queryLayout = null;
            this.MockWorkItemQueryAndLayoutPickerWizardPresenter.Setup(picker => picker.QueryAndLayout).Returns(queryLayout);
        }

        /// <summary>
        /// Sets up a dummy query and layout for the given project.
        /// </summary>
        /// <param name="project">The mock project to create the query and layout for.</param>
        /// <param name="fields">Any extra fields that the layout specifies should be included in the query.</param>
        /// <returns>The dummy query and layout.</returns>
        private QueryAndLayoutInformation SetupDummyPickedQueryAndLayout(Mock<ITeamProject> project, params string[] fields)
        {
            QueryAndLayoutInformation queryLayout = new QueryAndLayoutInformation(TestHelper.ValidQueryDefinition, TestHelper.CreateTestLayoutInformation(fields));
            this.MockWorkItemQueryAndLayoutPickerWizardPresenter.Setup(picker => picker.QueryAndLayout).Returns(queryLayout);
            return queryLayout;
        }

        /// <summary>
        /// Runs the import.
        /// </summary>
        private void RunTheImport()
        {
            this.MockRibbonPresenter.Raise(ribbonPresenter => ribbonPresenter.Import += null, EventArgs.Empty);
            this.Scheduler.Start();
            this.Scheduler.Wait();
        }

        /// <summary>
        /// Asserts that query and layout information was never saved.
        /// </summary>
        private void AssertQueryAndLayoutInformationNotSaved()
        {
            this.MockWorkItemQueryAndLayoutPickerWizardPresenter.Verify(picker => picker.SaveQueryAndLayout(), Times.Never());
        }

        /// <summary>
        /// Asserts that project information was never saved.
        /// </summary>
        private void AssertProjectInformationNotSaved()
        {
            this.MockTeamProjectPickerPresenter.Verify(picker => picker.SaveTeamProject(), Times.Never());
        }

        /// <summary>
        /// Asserts that query and layout information was saved.
        /// </summary>
        private void AssertQueryAndLayoutInformationSaved()
        {
            this.MockWorkItemQueryAndLayoutPickerWizardPresenter.Verify(picker => picker.SaveQueryAndLayout(), Times.Once());
        }

        /// <summary>
        /// Asserts that project information was saved.
        /// </summary>
        private void AssertProjectInformationSaved()
        {
            this.MockTeamProjectPickerPresenter.Verify(picker => picker.SaveTeamProject(), Times.Once());
        }

        /// <summary>
        /// Asserts that no errors are displayed by the ribbon.
        /// </summary>
        private void AssertNoErrrorsDisplayed()
        {
            this.MockRibbonPresenter.Verify(presenter => presenter.DisplayError(It.IsAny<string>(), It.IsAny<string>()), Times.Never());
        }

        /// <summary>
        /// Asserts that the cancellable operation started, was updated and then ended.
        /// </summary>
        private void AssertCancellableOperationProcessedNormally()
        {
            this.MockRibbonPresenter.Verify(p => p.StartCancellableOperation(FlowControllerResources.StartQueryExecution, It.IsAny<CancellationTokenSource>()), Times.Once());
            this.MockRibbonPresenter.Verify(p => p.UpdateCancellableOperation(FlowControllerResources.AddingWorkItemsToDocument), Times.Once());
            this.MockRibbonPresenter.Verify(p => p.EndCancellableOperation(), Times.Once());
        }

        /// <summary>
        /// Asserts that the cancellable operation started, but the query failed or was cancelled.
        /// </summary>
        private void AssertCancellableOperationStartedButFailedAtTheQueryStep()
        {
            this.MockRibbonPresenter.Verify(p => p.StartCancellableOperation(FlowControllerResources.StartQueryExecution, It.IsAny<CancellationTokenSource>()), Times.Once());
            this.MockRibbonPresenter.Verify(p => p.UpdateCancellableOperation(FlowControllerResources.AddingWorkItemsToDocument), Times.Never());
            this.MockRibbonPresenter.Verify(p => p.EndCancellableOperation(), Times.Once());
        }

        /// <summary>
        /// Asserts that the cancellable operation started, but the add step failed or was cancelled.
        /// </summary>
        private void AssertCancellableOperationStartedButFailedAtTheAddStep()
        {
            this.MockRibbonPresenter.Verify(p => p.StartCancellableOperation(FlowControllerResources.StartQueryExecution, It.IsAny<CancellationTokenSource>()), Times.Once());
            this.MockRibbonPresenter.Verify(p => p.UpdateCancellableOperation(FlowControllerResources.AddingWorkItemsToDocument), Times.Once());
            this.MockRibbonPresenter.Verify(p => p.EndCancellableOperation(), Times.Once());
        }
    }
}
