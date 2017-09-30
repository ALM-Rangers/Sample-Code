//---------------------------------------------------------------------
// <copyright file="WorkItemQueryPickerWizardPagePresenterTests.cs" company="Microsoft">
//    Copyright © ALM | DevOps Ranger Contributors. All rights reserved.
//    This code is licensed under the MIT License.
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF 
//    ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
//    TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
//    PARTICULAR PURPOSE AND NONINFRINGEMENT.
// </copyright>
// <summary>The WorkItemQueryPickerWizardPagePresenterTests type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.Word4Tfs.Library.Test.UnitTest.Presenter
{
    using System;
    using Microsoft.Practices.Unity;
    using Microsoft.TeamFoundation.WorkItemTracking.Client;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Word4Tfs.Library.Model;
    using Microsoft.Word4Tfs.Library.Presenter;
    using Microsoft.Word4Tfs.Library.View;
    using Moq;

    /// <summary>
    /// Tests the <see cref="WorkItemQueryPickerWizardPagePresenter"/> class.
    /// </summary>
    [TestClass]
    [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Unity container disposed in the test cleanup code.")]
    public class WorkItemQueryPickerWizardPagePresenterTests
    {
        /// <summary>
        /// The Unity container to be used for Dependency Injection
        /// </summary>
        private IUnityContainer container;

        /// <summary>
        /// The mock view used to test the presenter.
        /// </summary>
        private Mock<IWorkItemQueryAndLayoutPickerWizardView> mockWizardView;

        /// <summary>
        /// The mock view used to test the presenter.
        /// </summary>
        private Mock<IWorkItemQueryPickerWizardPageView> mockView;

        /// <summary>
        /// The mock project used to test the presenter.
        /// </summary>
        private Mock<ITeamProject> mockProject;

        /// <summary>
        /// The presenter to be tested.
        /// </summary>
        private IWorkItemQueryPickerWizardPagePresenter sut;

        /// <summary>
        /// Initializes the test.
        /// </summary>
        [TestInitialize]
        public void InitializeTest()
        {
            this.container = new UnityContainer();

            this.mockWizardView = TestHelper.CreateAndRegisterMock<IWorkItemQueryAndLayoutPickerWizardView>(this.container);
            this.mockView = TestHelper.CreateAndRegisterMock<IWorkItemQueryPickerWizardPageView>(this.container);
            this.mockProject = TestHelper.CreateAndRegisterMock<ITeamProject>(this.container);
            this.mockProject.Setup(project => project.QueryRunner).Returns(new Mock<ITeamProjectQuery>().Object);

            this.sut = this.container.Resolve<WorkItemQueryPickerWizardPagePresenter>();
            this.sut.Initialise();
        }

        /// <summary>
        /// Cleans up the test, disposing the Unity container.
        /// </summary>
        [TestCleanup]
        public void CleanupTest()
        {
            this.container.Dispose();
        }

        /// <summary>
        /// Test that <see cref="IWizardPagePresenter.Title"/> is valid.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void TitleIsValid()
        {
            // Assert
            Assert.AreEqual<string>("Choose the Query", this.sut.Title);
        }

        /// <summary>
        /// Test that <see cref="IWorkItemQueryPickerWizardPagePresenter.Initialise"/> leaves the page invalid.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void InitialiseMakesPageInvalid()
        {
            // Assert
            Assert.IsFalse(this.sut.IsValid, "The page should initially be invalid.");
        }

        /// <summary>
        /// Test that <see cref="IWorkItemQueryPickerWizardPagePresenter.Start"/> leaves the page invalid.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void StartMakesPageInvalid()
        {
            // Act
            this.sut.Start();

            // Assert
            Assert.IsFalse(this.sut.IsValid, "The page should initially be invalid.");
        }

        /// <summary>
        /// Tests that the <see cref="IWorkItemQueryPickerWizardPagePresenter.Start"/> method passes a query hierarchy to the view for display.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void StartPassesQueryHierarchyToViewForDisplay()
        {
            // Arrange
            QueryFolder data = this.SetupDummyQueryHierarchyOnProject();

            // Act
            this.sut.Start();

            // Assert
            this.mockProject.Verify(project => project.RootQueryFolder, Times.Once(), "The presenter should ask the project for the query hierarchy");
            this.mockView.Verify(view => view.LoadQueryHierarchy(data), Times.Once(), "The query hierarchy should be passed to the view.");
        }

        /// <summary>
        /// When a query folder is selected, change page to be invalid.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void SelectingAQueryFolderMakesPageInvalid()
        {
            // Arrange
            bool validityEventRaised = false;
            this.sut.Start();
            this.UserSelectsQuery(TestHelper.ValidQueryDefinition);
            Assert.IsTrue(this.sut.IsValid, "Premise for test does not hold, page should be valid.");
            this.sut.ValidityChanged += new EventHandler((s, e) => validityEventRaised = true);

            // Act
            this.UserSelectsQuery(new QueryFolder("test"));

            // Assert
            Assert.IsTrue(validityEventRaised, "Validity event should have been raised");
            Assert.IsFalse(this.sut.IsValid, "Selecting a query folder should make the page invalid.");
        }

        /// <summary>
        /// When a query is selected, change page to be valid.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void SelectingAQueryMakesPageValid()
        {
            // Arrange
            bool validityEventRaised = false;
            this.sut.Start();
            this.sut.ValidityChanged += new EventHandler((s, e) => validityEventRaised = true);

            // Act
            this.UserSelectsQuery(TestHelper.ValidQueryDefinition);

            // Assert
            Assert.IsTrue(validityEventRaised, "Validity event should have been raised");
            Assert.IsTrue(this.sut.IsValid, "Selecting a query should make the page valid.");
        }

        /// <summary>
        /// Tests that the <see cref="IWorkItemQueryPickerWizardPagePresenter.Cancel"/> cancels the view.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void CancelCancelsTheView()
        {
            // Arrange
            QueryFolder data = this.SetupDummyQueryHierarchyOnProject();
            this.sut.Start();

            // Act
            this.sut.Cancel();

            // Assert
            this.mockView.Verify(view => view.Cancel(), Times.Once(), "The view was not cancelled.");
        }

        /// <summary>
        /// Sets <see cref="IWorkItemQueryPickerWizardPagePresenter.SelectedQuery"/> when a query is selected.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void SelectedQueryIsSetWhenAQueryIsSelected()
        {
            // Arrange
            this.sut.Start();

            // Act
            this.UserSelectsQuery(TestHelper.ValidQueryDefinition);

            // Assert
            Assert.AreEqual<string>(TestHelper.ValidQueryDefinition.QueryText, this.sut.SelectedQuery.QueryText, "Selected query not returned.");
        }

        /// <summary>
        /// When a direct link query is selected, warning message is displayed.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void SelectingDirectLinkQueryDisplaysWarningMessage()
        {
            // Arrange
            this.sut.Start();

            // Act
            this.UserSelectsQuery(new QueryDefinition("test", TestHelper.OneHopQuery));

            // Assert
            this.mockView.Verify(view => view.DisplayWarning("WARNING: If this query imports the same work item more than once, Refresh will not work."), Times.Once());
        }

        /// <summary>
        /// When a flat query is selected, warning message is cleared.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void SelectingFlatQueryClearsWarningMessage()
        {
            // Arrange
            this.sut.Start();

            // Act
            this.UserSelectsQuery(new QueryDefinition("test", TestHelper.FlatQuery));

            // Assert
            this.mockView.Verify(view => view.DisplayWarning(string.Empty), Times.Once());
        }

        /// <summary>
        /// When a tree query is selected, warning message is cleared.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void SelectingTreeQueryClearsWarningMessage()
        {
            // Arrange
            this.sut.Start();

            // Act
            this.UserSelectsQuery(new QueryDefinition("test", TestHelper.TreeQuery));

            // Assert
            this.mockView.Verify(view => view.DisplayWarning(string.Empty), Times.Once());
        }

        /// <summary>
        /// When a query folder is selected, warning message is cleared.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void SelectingAQueryFolderClearsWarningMessage()
        {
            // Arrange
            this.sut.Start();

            // Act
            this.UserSelectsQuery(new QueryFolder("test"));

            // Assert
            this.mockView.Verify(view => view.DisplayWarning(string.Empty), Times.Once());
        }

        /// <summary>
        /// Creates a dummy query hierarchy on the mock project.
        /// </summary>
        /// <returns>~The dummy query hierarchy.</returns>
        private QueryFolder SetupDummyQueryHierarchyOnProject()
        {
            QueryFolder data = new QueryFolder("root");
            this.mockProject.Setup(project => project.RootQueryFolder).Returns(data);
            return data;
        }

        /// <summary>
        /// Raises an event for the user selecting a query.
        /// </summary>
        /// <param name="queryItem">The selected query item.</param>
        private void UserSelectsQuery(QueryItem queryItem)
        {
            this.mockView.Raise(view => view.QuerySelected += null, new QueryItemEventArgs(queryItem));
        }
    }
}
