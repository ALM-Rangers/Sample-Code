//---------------------------------------------------------------------
// <copyright file="WorkItemQueryAndLayoutPickerWizardPresenterTests.cs" company="Microsoft">
//    Copyright (c) Microsoft. All rights reserved.
//    This code is licensed under the MIT License.
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF 
//    ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
//    TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
//    PARTICULAR PURPOSE AND NONINFRINGEMENT.
// </copyright>
// <summary>The WorkItemQueryAndLayoutPickerWizardPresenterTests type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.Word4Tfs.Library.Test.UnitTest.Presenter
{
    using Microsoft.Practices.Unity;
    using Microsoft.TeamFoundation.WorkItemTracking.Client;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Word4Tfs.Library.Data;
    using Microsoft.Word4Tfs.Library.Model;
    using Microsoft.Word4Tfs.Library.Presenter;
    using Microsoft.Word4Tfs.Library.View;
    using Moq;

    /// <summary>
    /// Tests the <see cref="WorkItemQueryAndLayoutPickerWizardPresenter"/> class.
    /// </summary>
    [TestClass]
    [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Unity container disposed in the test cleanup code.")]
    public class WorkItemQueryAndLayoutPickerWizardPresenterTests
    {
        /// <summary>
        /// The Unity container to be used for Dependency Injection
        /// </summary>
        private IUnityContainer container;

        /// <summary>
        /// The mock view used to test the presenter.
        /// </summary>
        private Mock<IWorkItemQueryAndLayoutPickerWizardView> mockView;

        /// <summary>
        /// The mock document to be used by the presenter.
        /// </summary>
        private Mock<ITeamProjectDocument> mockDocument;

        /// <summary>
        /// The mock work item query picker wizard page presenter used to test the presenter.
        /// </summary>
        private Mock<IWorkItemQueryPickerWizardPagePresenter> mockWorkItemQueryPagePresenter;

        /// <summary>
        /// The mock layout picker wizard page view used to test the presenter.
        /// </summary>
        private Mock<ILayoutPickerWizardPagePresenter> mockLayoutPagePresenter;

        /// <summary>
        /// The presenter to be tested.
        /// </summary>
        private IWorkItemQueryAndLayoutPickerWizardPresenter sut;

        /// <summary>
        /// Initializes the test.
        /// </summary>
        [TestInitialize]
        public void InitializeTest()
        {
            this.container = new UnityContainer();

            this.mockView = TestHelper.CreateAndRegisterMock<IWorkItemQueryAndLayoutPickerWizardView>(this.container);
            this.mockWorkItemQueryPagePresenter = TestHelper.CreateAndRegisterMock<IWorkItemQueryPickerWizardPagePresenter>(this.container);
            this.mockLayoutPagePresenter = TestHelper.CreateAndRegisterMock<ILayoutPickerWizardPagePresenter>(this.container);
            this.mockDocument = TestHelper.CreateAndRegisterMock<ITeamProjectDocument>(this.container);

            this.sut = (IWorkItemQueryAndLayoutPickerWizardPresenter)this.container.Resolve<WorkItemQueryAndLayoutPickerWizardPresenter>();
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
        /// Presenter creates and initialises the page presenters.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void CreatesAndInitialisesThePagePresenters()
        {
            // Arrange
            int seq = 1;
            int workItemQuerySequence = 0;
            int layoutSequence = 0;
            this.mockWorkItemQueryPagePresenter.Setup(p => p.Initialise()).Callback(() => { workItemQuerySequence = seq++; });
            this.mockLayoutPagePresenter.Setup(p => p.Initialise()).Callback(() => { layoutSequence = seq++; });

            // Act
            this.sut.Initialise();

            // Assert
            Assert.AreEqual<int>(1, workItemQuerySequence, "Query presenter not initialised in the correct order");
            Assert.AreEqual<int>(2, layoutSequence, "Layout presenter not initialised in the correct order");
        }

        /// <summary>
        /// When the wizard is cancelled the presenter returns a null query and layout information.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void CancellingWizardReturnsNullQueryAndLayoutDefinition()
        {
            // Arrange
            this.mockView.Setup(view => view.StartDialog()).Returns(false);
            this.sut.Initialise();

            // Act
            this.sut.Start();
            QueryAndLayoutInformation ans = this.sut.QueryAndLayout;

            // Assert
            Assert.IsNull(ans, "Should return a null query and layout if the wizard is cancelled");
        }

        /// <summary>
        /// When the wizard is cancelled the team project document is not saved with any new query and layout.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void CancellingWizardDoesNotSaveToTeamProjectDocument()
        {
            // Arrange
            this.mockView.Setup(view => view.StartDialog()).Returns(false);
            this.sut.Initialise();

            // Act
            this.sut.Start();

            // Assert
            this.mockDocument.Verify(doc => doc.SaveQueriesAndLayouts(), Times.Never(), "Should not save anything to the document");
        }

        /// <summary>
        /// When a query and layout is confirmed (Finish button clicked) the presenter returns the chosen query definition (modified for the layouts and other queries) and layout.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void ConfirmingAQueryAndLayoutReturnsChosenInformationWithModifiedQuery()
        {
            // Arrange
            QueryDefinition query = this.SetupSelectedValidQueryOnQueryPagePresenter();
            LayoutInformation layout = this.SetupSelectedLayoutOnLayoutPagePresenter();
            QueryAndLayoutInformation expected = new QueryAndLayoutInformation(TestHelper.ValidQueryDefinition, layout);
            Assert.AreNotSame(query, expected.Query, "Pre-requisites of test not satisfied, using same query as initial and final");
            this.mockView.Setup(view => view.StartDialog()).Returns(true);
            this.mockDocument.Setup(doc => doc.AddQueryAndLayout(It.IsAny<QueryAndLayoutInformation>())).Returns(expected);
            this.sut.Initialise();

            // Act
            this.sut.Start();
            QueryAndLayoutInformation ans = this.sut.QueryAndLayout;

            // Assert
            Assert.AreSame(expected.Query, ans.Query, "Should return the definition of the chosen query after modification by the document.");
            Assert.AreSame(layout, ans.Layout, "Should return the chosen layout.");
        }

        /// <summary>
        /// When a query and layout is confirmed (Finish button clicked) the presenter adds the information to the document.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void ConfirmingAQueryAndLayoutAddsTheInformationToTheDocument()
        {
            // Arrange
            QueryDefinition query = this.SetupSelectedValidQueryOnQueryPagePresenter();
            LayoutInformation layout = this.SetupSelectedLayoutOnLayoutPagePresenter();
            this.mockView.Setup(view => view.StartDialog()).Returns(true);
            this.sut.Initialise();

            // Act
            this.sut.Start();

            // Assert
            this.mockDocument.Verify(doc => doc.AddQueryAndLayout(It.IsAny<QueryAndLayoutInformation>()), Times.Once());
        }

        /// <summary>
        /// Tests that <see cref="WorkItemQueryAndLayoutPickerWizardPresenter.SaveQueryAndLayout"/> throws exception if a query and layout have not been picked.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void SaveQueryAndLayoutThrowsExceptionIfQueryAndLayoutHaveNotBeenPicked()
        {
            // Arrange
            this.mockView.Setup(view => view.StartDialog()).Returns(false);
            this.sut.Initialise();
            this.sut.Start();

            // Act and assert
            TestHelper.TestForInvalidOperationException(() => this.sut.SaveQueryAndLayout(), "Must choose a query and layout before it can be saved.");
        }

        /// <summary>
        /// Tests that <see cref="WorkItemQueryAndLayoutPickerPresenter.SaveQueryAndLayout"/> writes the queries and layouts to the document.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void SaveQueryAndLayoutWritesQueriesAndLayoutsToTheDocument()
        {
            // Arrange
            QueryDefinition query = this.SetupSelectedValidQueryOnQueryPagePresenter();
            LayoutInformation layout = this.SetupSelectedLayoutOnLayoutPagePresenter();
            this.mockDocument
                .Setup(doc => doc.AddQueryAndLayout(It.IsAny<QueryAndLayoutInformation>()))
                .Callback((QueryAndLayoutInformation qli) => { Assert.AreSame(query, qli.Query); Assert.AreSame(layout, qli.Layout); })
                .Returns(new QueryAndLayoutInformation(query, layout));
            this.mockView.Setup(view => view.StartDialog()).Returns(true);
            this.sut.Initialise();
            this.sut.Start();

            // Act
            this.sut.SaveQueryAndLayout();

            // Assert
            this.mockDocument.Verify(document => document.SaveQueriesAndLayouts(), Times.Once(), "Did not save the query and layout");
        }

        /// <summary>
        /// Sets up the mock view with a selected valid query
        /// </summary>
        /// <returns>The selected valid query.</returns>
        private QueryDefinition SetupSelectedValidQueryOnQueryPagePresenter()
        {
            QueryDefinition query = TestHelper.ValidQueryDefinition;
            this.mockWorkItemQueryPagePresenter.Setup(p => p.SelectedQuery).Returns(query);
            return query;
        }

        /// <summary>
        /// Sets up the mock view with a selected layout.
        /// </summary>
        /// <returns>The selected layout.</returns>
        private LayoutInformation SetupSelectedLayoutOnLayoutPagePresenter()
        {
            LayoutInformation layout = TestHelper.CreateTestLayoutInformation(new BuildingBlockName[0]);
            this.mockLayoutPagePresenter.Setup(p => p.SelectedLayout).Returns(layout);
            return layout;
        }
    }
}
