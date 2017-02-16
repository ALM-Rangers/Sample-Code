//---------------------------------------------------------------------
// <copyright file="FlowControllerWorkItemRefreshTests.cs" company="Microsoft">
//    Copyright (c) Microsoft. All rights reserved.
//    This code is licensed under the MIT License.
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF 
//    ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
//    TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
//    PARTICULAR PURPOSE AND NONINFRINGEMENT.
// </copyright>
// <summary>The FlowControllerWorkItemRefreshTests type.</summary>
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
    /// Tests the flow controller work item refresh functionality.
    /// </summary>
    [TestClass]
    public class FlowControllerWorkItemRefreshTests : FlowControllerTestsBase
    {
        /// <summary>
        /// Tests that a cancellable operation dialog is started, updated and closed when doing a good import.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void RefreshDisplaysCancellableOperationDialogInCorrectSequence()
        {
            // Arrange
            int callSequence = 1;
            this.SetupDocument();
            this.MockRibbonPresenter.Setup(p => p.StartCancellableOperation(FlowControllerResources.StartRefreshExecution, It.IsAny<CancellationTokenSource>())).Callback(() => Assert.AreEqual<int>(1, callSequence++, "StartCancellableOperation must be called first"));
            this.MockRibbonPresenter.Setup(p => p.EndCancellableOperation()).Callback(() => Assert.AreEqual<int>(2, callSequence++, "EndCancellableOperation must be called last"));

            // Act
            this.MockRibbonPresenter.Raise(ribbonPresenter => ribbonPresenter.Refresh += null, EventArgs.Empty);

            // Assert
            this.MockRibbonPresenter.Verify(p => p.StartCancellableOperation(FlowControllerResources.StartRefreshExecution, It.IsAny<CancellationTokenSource>()), Times.Once(), "Start must be called synchronously as it is a blocking call");
            this.Scheduler.Start();
            this.Scheduler.Wait();
            this.AssertCancellableOperationProcessedNormally();
        }

        /// <summary>
        /// Tests that cancelling the refresh query stops the refresh.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void CancellingRefreshStopsTheRefresh()
        {
            // This test deliberately succeeds as there is no suitable hook to really make sure that refresh is cancelled. Cancellation is tested for in the Team Project Document tests. This
            // is here to show that the scenario was considered.
        }

        /// <summary>
        /// Flow controller instructs ribbon presenter to display an error if TFS is not connected.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void RibbonPresenterToldToDisplayErrorMessageIfTFSNotConnected()
        {
            // Arrange
            Mock<ITeamProject> mockTeamProject = this.SetupDocument();
            this.MockTeamProjectDocument.Setup(doc => doc.RefreshWorkItems(It.IsAny<CancellationToken>())).Throws(new TeamFoundationServerException("testmessage"));

            // Act
            this.RunTheRefresh();

            // Assert
            this.MockRibbonPresenter.Verify(ribbonPresenter => ribbonPresenter.DisplayError(TestHelper.MessageStartsWith("testmessage"), string.Empty), Times.Once(), "Did not tell the ribbon presenter to display an error message.");
        }

        /// <summary>
        /// Tests that an error is displayed if the document is not insertable at the time refresh is selected.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void RefreshDisplaysErrorIfDocumentNotInsertable()
        {
            // Arrange
            this.MockTeamProjectDocument.Setup(doc => doc.IsInsertable).Returns(false);

            // Act)
            this.RunTheRefresh();

            // Assert
            this.AssertSingleErrorMessageDisplayed("Cannot execute a refresh with the cursor at the current location. This is because any new work items cannot be inserted here.");
        }

        /// <summary>
        /// Tests that an error is displayed if the document fails validation.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void RefreshDisplaysErrorIfDocumentNotValid()
        {
            // Arrange
            this.MockTeamProjectDocument.Setup(doc => doc.RefreshWorkItems(It.IsAny<CancellationToken>())).Returns(new string[] { "error 1", "error 2" });

            // Act)
            this.RunTheRefresh();

            // Assert
            this.AssertSingleErrorMessageDisplayed("Cannot refresh because the document structure is invalid.", "error 1\r\nerror 2");
        }

        /// <summary>
        /// Executes the refresh
        /// </summary>
        private void RunTheRefresh()
        {
            this.MockRibbonPresenter.Raise(ribbonPresenter => ribbonPresenter.Refresh += null, EventArgs.Empty);
            this.Scheduler.Start();
            this.Scheduler.Wait();
        }

        /// <summary>
        /// Sets up the team project document.
        /// </summary>
        /// <returns>Returns the team project mock.</returns>
        private Mock<ITeamProject> SetupDocument()
        {
            Mock<ITeamProject> mockTeamProject = TestHelper.CreateMockTeamProject("[System.Id]", "[System.Area]", "[System.Title]");
            this.MockTeamProjectDocument.Setup(document => document.TeamProject).Returns(mockTeamProject.Object);
            return mockTeamProject;
        }

        /// <summary>
        /// Asserts that the cancellable operation started, was updated and then ended.
        /// </summary>
        private void AssertCancellableOperationProcessedNormally()
        {
            this.MockRibbonPresenter.Verify(p => p.StartCancellableOperation(FlowControllerResources.StartRefreshExecution, It.IsAny<CancellationTokenSource>()), Times.Once());
            this.MockRibbonPresenter.Verify(p => p.EndCancellableOperation(), Times.Once());
        }
    }
}
