//---------------------------------------------------------------------
// <copyright file="TeamProjectPickerPresenterTests.cs" company="Microsoft">
//    Copyright (c) Microsoft. All rights reserved.
//    This code is licensed under the MIT License.
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF 
//    ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
//    TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
//    PARTICULAR PURPOSE AND NONINFRINGEMENT.
// </copyright>
// <summary>The TeamProjectPickerPresenterTests type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.Word4Tfs.Library.Test.UnitTest.Presenter
{
    using System;
    using Microsoft.Practices.Unity;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Word4Tfs.Library.Data;
    using Microsoft.Word4Tfs.Library.Model;
    using Microsoft.Word4Tfs.Library.Presenter;
    using Microsoft.Word4Tfs.Library.View;
    using Moq;

    /// <summary>
    /// Tests the <see cref="TeamProjectPickerPresenter"/> class.
    /// </summary>
    [TestClass]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Unity container disposed in the test cleanup code.")]
    public class TeamProjectPickerPresenterTests
    {
        /// <summary>
        /// The Unity container to be used for Dependency Injection
        /// </summary>
        private IUnityContainer container;

        /// <summary>
        /// The mock team project picker used to test the presenter.
        /// </summary>
        private Mock<ITeamProjectPickerView> mockPickerView;

        /// <summary>
        /// The mock document to be used by the presenter.
        /// </summary>
        private Mock<ITeamProjectDocument> mockDocument;

        /// <summary>
        /// The mock factory used to test the model.
        /// </summary>
        private Mock<IFactory> mockFactory;

        /// <summary>
        /// The mock wait notifier used to test the model.
        /// </summary>
        private Mock<IWaitNotifier> mockWaitNotifier;

        /// <summary>
        /// The presenter to be tested.
        /// </summary>
        private ITeamProjectPickerPresenter sut;

        /// <summary>
        /// Initializes the test.
        /// </summary>
        [TestInitialize]
        public void InitializeTest()
        {
            this.container = new UnityContainer();

            this.mockPickerView = TestHelper.CreateAndRegisterMock<ITeamProjectPickerView>(this.container);
            this.mockDocument = TestHelper.CreateAndRegisterMock<ITeamProjectDocument>(this.container);
            this.mockDocument.SetupAllProperties();
            this.mockFactory = TestHelper.CreateAndRegisterMock<IFactory>(this.container);
            this.mockWaitNotifier = TestHelper.CreateAndRegisterMock<IWaitNotifier>(this.container);

            this.sut = this.container.Resolve<TeamProjectPickerPresenter>();
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
        /// Constructor throws <see cref="ArgumentNullException"/> if the view it is passed is null
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void TeamProjectPickerPresenterConstructorThrowsExceptionIfNullViewIsPassed()
        {
            TestHelper.TestForArgumentNullException(() => this.sut = new TeamProjectPickerPresenter(null, this.mockDocument.Object, this.mockFactory.Object, this.mockWaitNotifier.Object), "pickerView");
        }

        /// <summary>
        /// Constructor throws <see cref="ArgumentNullException"/> if the project document it is passed is null
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void TeamProjectPickerPresenterConstructorThrowsExceptionIfNullProjectDocumentIsPassed()
        {
            TestHelper.TestForArgumentNullException(() => this.sut = new TeamProjectPickerPresenter(this.mockPickerView.Object, null, this.mockFactory.Object, this.mockWaitNotifier.Object), "projectDocument");
        }

        /// <summary>
        /// Constructor throws <see cref="ArgumentNullException"/> if the factory it is passed is null
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void TeamProjectPickerPresenterConstructorThrowsExceptionIfNullFactoryIsPassed()
        {
            TestHelper.TestForArgumentNullException(() => this.sut = new TeamProjectPickerPresenter(this.mockPickerView.Object, this.mockDocument.Object, null, this.mockWaitNotifier.Object), "factory");
        }

        /// <summary>
        /// Constructor throws <see cref="ArgumentNullException"/> if the wait notifier it is passed is null
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void TeamProjectPickerPresenterConstructorThrowsExceptionIfNullWaitNotifierIsPassed()
        {
            TestHelper.TestForArgumentNullException(() => this.sut = new TeamProjectPickerPresenter(this.mockPickerView.Object, this.mockDocument.Object, this.mockFactory.Object, null), "waitNotifier");
        }

        /// <summary>
        /// Tests that <see cref="ChooseTeamProject"/> invokes the team project picker view.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void ChooseTeamProjectStartsTheTeamProjectPickerView()
        {
            // Act
            this.sut.ChooseTeamProject();

            // Assert
            this.mockPickerView.Verify(picker => picker.ChooseTeamProject(), Times.Once(), "The team project picker should be invoked.");
        }

        /// <summary>
        /// Tests that <see cref="ChooseTeamProject"/> returns data if the team project picker returns a project.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void ChooseTeamProjectReturnsDataIfTheViewReturnsAProject()
        {
            // Arrange
            using (ITeamProject mockTeamProject = this.CreateMockTeamProjectFromFactory())
            {
                this.mockPickerView.Setup(picker => picker.ChooseTeamProject()).Returns(mockTeamProject.TeamProjectInformation);

                // Act
                TeamProjectInformation ans = this.sut.ChooseTeamProject().TeamProjectInformation;

                // Assert
                Assert.AreSame(mockTeamProject.TeamProjectInformation, ans, "The call should return the team project data that came from the view.");
                this.AssertNoErrrorsDisplayed();
            }
        }

        /// <summary>
        /// Tests that <see cref="ChooseTeamProject"/> sets the <see cref="ITeamProjectDocument.TeamProject"/> property on the document if a project has been chosen.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void ChooseTeamProjectSetsTeamProjectDocumentTeamProjectPropertyIfProjectHasBeenChosen()
        {
            // Arrange
            using (ITeamProject mockTeamProject = this.CreateMockTeamProjectFromFactory())
            {
                this.mockPickerView.Setup(picker => picker.ChooseTeamProject()).Returns(mockTeamProject.TeamProjectInformation);

                // Act
                ITeamProject ans = this.sut.ChooseTeamProject();

                // Assert
                Assert.AreSame(ans, this.mockDocument.Object.TeamProject, "The team project should be set on the team project document too.");
            }
        }

        /// <summary>
        /// Tests that <see cref="ChooseTeamProject"/> returns <c>null</c> if the team project picker does not return a project.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void ChooseTeamProjectReturnsNullIfTheViewReturnsNull()
        {
            // Arrange
            TeamProjectInformation project = null;
            this.mockPickerView.Setup(picker => picker.ChooseTeamProject()).Returns(project);

            // Act
            ITeamProject ans = this.sut.ChooseTeamProject();

            // Assert
            Assert.IsNull(ans, "The call should return null if team project data is null");
            this.AssertNoErrrorsDisplayed();
        }

        /// <summary>
        /// Tests that <see cref="ChooseTeamProject"/> invokes the wait notifier.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void ChooseTeamProjectInvokesTheWaitNotifier()
        {
            // Act
            this.sut.ChooseTeamProject();

            // Assert
            this.mockWaitNotifier.Verify(notifier => notifier.StartWait(), Times.Once(), "The wait notifier should be started.");
            this.mockWaitNotifier.Verify(notifier => notifier.EndWait(), Times.Once(), "The wait notifier should be ended.");
        }

        /// <summary>
        /// Tests that <see cref="ChooseTeamProjectCollection"/> invokes the team project picker view.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void ChooseTeamProjectCollectionStartsTheTeamProjectPickerView()
        {
            // Act
            this.sut.ChooseTeamProjectCollection();

            // Assert
            this.mockPickerView.Verify(picker => picker.ChooseTeamProjectCollection(), Times.Once(), "The team project picker should be invoked.");
        }

        /// <summary>
        /// Tests that <see cref="ChooseTeamProjectCollection"/> returns data if the team project picker returns a project collection.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void ChooseTeamProjectCollectionReturnsDataIfTheViewReturnsAProjectCollection()
        {
            // Arrange
            using (ITeamProject mockTeamProject = this.CreateMockTeamProjectFromFactory())
            {
                this.mockPickerView.Setup(picker => picker.ChooseTeamProjectCollection()).Returns(mockTeamProject.TeamProjectInformation.CollectionUri);

                // Act
                Uri ans = this.sut.ChooseTeamProjectCollection();

                // Assert
                Assert.AreSame(mockTeamProject.TeamProjectInformation.CollectionUri, ans, "The call should return the Uri that came from the view.");
                this.AssertNoErrrorsDisplayed();
            }
        }

        /// <summary>
        /// Tests that <see cref="ChooseTeamProjectCollection"/> returns <c>null</c> if the team project picker does not return a project collection.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void ChooseTeamProjectCollectionReturnsNullIfTheViewReturnsNull()
        {
            // Arrange
            Uri uri = null;
            this.mockPickerView.Setup(picker => picker.ChooseTeamProjectCollection()).Returns(uri);

            // Act
            Uri ans = this.sut.ChooseTeamProjectCollection();

            // Assert
            Assert.IsNull(ans, "The call should return null if team project collection data is null");
            this.AssertNoErrrorsDisplayed();
        }

        /// <summary>
        /// Tests that <see cref="TeamProjectPickerPresenter.SaveTeamProject"/> throws exception if a project has not been picked yet.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void SaveTeamProjectThrowsExceptionIfProjectHasNotBeenPickedYet()
        {
            TestHelper.TestForInvalidOperationException(() => this.sut.SaveTeamProject(), "Must choose a Team Project before it can be saved.");
        }

        /// <summary>
        /// Tests that <see cref="TeamProjectPickerPresenter.SaveTeamProject"/> writes the team project to the document.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void SaveTeamProjectWritesTeamProjectToTheDocument()
        {
            // Arrange
            using (ITeamProject mockTeamProject = this.CreateMockTeamProjectFromFactory())
            {
                this.mockPickerView.Setup(picker => picker.ChooseTeamProject()).Returns(mockTeamProject.TeamProjectInformation);
                this.sut.ChooseTeamProject();

                // Act
                this.sut.SaveTeamProject();

                // Assert
                this.mockDocument.Verify(document => document.SaveTeamProject(), Times.Once());
                this.AssertNoErrrorsDisplayed();
            }
        }

        /// <summary>
        /// Tests that <see cref="ChooseTeamProject"/> tells the view to display an error when there is an error getting the projects.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void DisplaysErrorIfChooseTeamProjectThrowsException()
        {
            // Arrange
            this.mockPickerView.Setup(picker => picker.ChooseTeamProject()).Throws(new Exception("test"));

            // Act
            this.sut.ChooseTeamProject();

            // Assert
            this.mockPickerView.Verify(picker => picker.DisplayMessage(UIMessageType.Error, "Team Foundation Error", "test", TestHelper.MessageStartsWith("test")), Times.Once(), "The error should be displayed with the message from the exception.");
        }

        /// <summary>
        /// Tests that <see cref="ChooseTeamProjectCollection"/> tells the view to display an error when there is an error getting the project collection.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void DisplaysErrorIfChooseTeamProjectCollectionThrowsException()
        {
            // Arrange
            this.mockPickerView.Setup(picker => picker.ChooseTeamProjectCollection()).Throws(new Exception("test"));

            // Act
            this.sut.ChooseTeamProjectCollection();

            // Assert
            this.mockPickerView.Verify(picker => picker.DisplayMessage(UIMessageType.Error, "Team Foundation Error", "test", TestHelper.MessageStartsWith("test")), Times.Once(), "The error should be displayed with the message from the exception.");
        }

        /// <summary>
        /// Creates a test <see cref="TeamProjectInformation"/> object.
        /// </summary>
        /// <returns>The test <see cref="TeamProjectInformation"/> object.</returns>
        private static TeamProjectInformation CreateTestTeamProjectInformation()
        {
            TeamProjectInformation tpi = new TeamProjectInformation(new Uri("http://test"), Guid.NewGuid(), "test", null);
            return tpi;
        }

        /// <summary>
        /// Creates a mock team project and arranges for the mock factory to return it.
        /// </summary>
        /// <returns>The mock team project.</returns>
        private ITeamProject CreateMockTeamProjectFromFactory()
        {
            ITeamProject mockTeamProject = null;
            ITeamProject tempMockTeamProject = null;
            try
            {
                tempMockTeamProject = TestHelper.CreateMockTeamProject().Object;
                Uri uri = tempMockTeamProject.TeamProjectInformation.CollectionUri;
                string projectName = tempMockTeamProject.TeamProjectInformation.ProjectName;
                this.mockFactory.Setup(factory => factory.CreateTeamProject(uri, projectName, null)).Returns(tempMockTeamProject);
                mockTeamProject = tempMockTeamProject;
                tempMockTeamProject = null;
            }
            finally
            {
                if (tempMockTeamProject != null)
                {
                    tempMockTeamProject.Dispose();
                }
            }

            return mockTeamProject;
        }

        /// <summary>
        /// Asserts that no errors are displayed by the view.
        /// </summary>
        private void AssertNoErrrorsDisplayed()
        {
            this.mockPickerView.Verify(view => view.DisplayMessage(It.IsAny<UIMessageType>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never());
        }
    }
}
