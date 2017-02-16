//---------------------------------------------------------------------
// <copyright file="WizardPagePresenterBaseTests.cs" company="Microsoft">
//    Copyright (c) Microsoft. All rights reserved.
//    This code is licensed under the MIT License.
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF 
//    ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
//    TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
//    PARTICULAR PURPOSE AND NONINFRINGEMENT.
// </copyright>
// <summary>The WizardPagePresenterBaseTests type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.Word4Tfs.Library.Test.UnitTest.Presenter
{
    using System;
    using Microsoft.Practices.Unity;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Word4Tfs.Library.Presenter;
    using Microsoft.Word4Tfs.Library.View;
    using Moq;

    /// <summary>
    /// Tests the <see cref="WizardPagePresenterBase"/> class.
    /// </summary>
    [TestClass]
    [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Unity container disposed in the test cleanup code.")]
    public class WizardPagePresenterBaseTests
    {
        /// <summary>
        /// The Unity container to be used for Dependency Injection
        /// </summary>
        private IUnityContainer container;

        /// <summary>
        /// The mock view used to test the presenter.
        /// </summary>
        private Mock<IWizardViewBase> mockWizardView;

        /// <summary>
        /// The mock page view used to test the presenter.
        /// </summary>
        private Mock<IWizardPageView> mockPageView;

        /// <summary>
        /// The derived presenter that is used to test the base presenter.
        /// </summary>
        private DerivedPage sut;

        /// <summary>
        /// Tracks the state of the view.
        /// </summary>
        private ViewState viewState = ViewState.Unknown;

        /// <summary>
        /// The states of a view.
        /// </summary>
        private enum ViewState
        {
            /// <summary>
            /// The page state is unknown (not set).
            /// </summary>
            Unknown,

            /// <summary>
            /// The page is hidden.
            /// </summary>
            Hidden,

            /// <summary>
            /// The page is visible.
            /// </summary>
            Visible
        }

        /// <summary>
        /// Initializes the test.
        /// </summary>
        [TestInitialize]
        public void InitializeTest()
        {
            this.container = new UnityContainer();

            this.mockPageView = TestHelper.CreateAndRegisterMock<IWizardPageView>(this.container);

            this.mockWizardView = TestHelper.CreateAndRegisterMock<IWizardViewBase>(this.container);
            this.mockPageView = TestHelper.CreateAndRegisterMock<IWizardPageView>(this.container);

            this.mockPageView.Setup(v => v.Hide()).Callback(() => this.viewState = ViewState.Hidden);
            this.mockPageView.Setup(v => v.Show()).Callback(() => this.viewState = ViewState.Visible);

            this.sut = this.container.Resolve<DerivedPage>();
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
        /// Test that <see cref="WizardPagePresenterBase.Initialise"/> adds the page view to the set of wizard pages.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void InitialiseAddsPageToSetOfWizardPages()
        {
            // Act
            this.sut.Initialise();

            // Assert
            this.mockWizardView.Verify(mv => mv.AddPage(this.mockPageView.Object), Times.Once(), "The page was not added to the set of wizard pages.");
        }

        /// <summary>
        /// Test that <see cref="WizardPagePresenterBase.Initialise"/> hides the page
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void InitialiseHidesThePage()
        {
            // Act
            this.sut.Initialise();

            // Assert
            Assert.AreEqual<ViewState>(ViewState.Hidden, this.viewState, "The page should initially be hidden.");
        }

        /// <summary>
        /// Test that <see cref="WizardPagePresenterBase.Start"/> starts the page.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void StartStartsThePage()
        {
            // Arrange
            this.sut.Initialise();

            // Act
            this.sut.Start();

            // Assert
            Assert.IsTrue(this.sut.Started, "The presenter was not started.");
        }

        /// <summary>
        /// Test that <see cref="WizardPagePresenterBase.Activate"/> activates the page.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void ActivateActivatesThePage()
        {
            // Arrange
            this.sut.Initialise();
            this.sut.Start();

            // Act
            this.sut.Activate();

            // Assert
            Assert.IsTrue(this.sut.Activated, "The presenter was not activated.");
        }

        /// <summary>
        /// Test that <see cref="WizardPagePresenterBase.Activate"/> shows the page
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void ActivateShowsThePage()
        {
            // Arrange
            this.sut.Initialise();
            this.sut.Start();

            // Act
            this.sut.Activate();

            // Assert
            Assert.AreEqual<ViewState>(ViewState.Visible, this.viewState, "The page should initially be visible after it is activated.");
        }

        /// <summary>
        /// Test that <see cref="WizardPagePresenterBase.Deactivate"/> hides the page
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void DeactivateHidesThePage()
        {
            // Arrange
            this.sut.Initialise();
            this.sut.Start();

            // Act
            this.sut.Deactivate();

            // Assert
            Assert.AreEqual<ViewState>(ViewState.Hidden, this.viewState, "The page should not be visible after it is deactivated.");
        }

        /// <summary>
        /// Test that <see cref="WizardPagePresenterBase.Deactivate"/> passes itself in the <see cref="IWizardPagePresenter.ValidityChanged"/> event.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void PassesSelfInValidityChangedEvent()
        {
            // Arrange and assert
            this.sut.Initialise();
            this.sut.ValidityChanged += new EventHandler((s, e) => Assert.AreSame(this.sut, s, "The event must send itself as the sender"));

            // Act
            this.sut.ChangeValidity();
        }

        /// <summary>
        /// Test that <see cref="WizardPagePresenterBase.Start"/> throws exception if <see cref="WizardPagePresenterBase.Initialise"/> has not been called first.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void StartThrowsExceptionIfInitialiseNotCalled()
        {
            TestHelper.TestForInvalidOperationException(() => this.sut.Start(), "Must initialize the page presenter before starting it.");
        }

        /// <summary>
        /// Test that <see cref="WizardPagePresenterBase.Activate"/> throws exception if <see cref="WizardPagePresenterBase.Start"/> has not been called first.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void ActivateThrowsExceptionIfStartNotCalled()
        {
            // Arrange
            this.sut.Initialise();

            // Act and assert
            TestHelper.TestForInvalidOperationException(() => this.sut.Activate(), "Cannot perform this operation because the page presenter is not started.");
        }

        /// <summary>
        /// Test that <see cref="WizardPagePresenterBase.Deactivate"/> throws exception if <see cref="WizardPagePresenterBase.Start"/> has not been called first.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void DeactivateThrowsExceptionIfStartNotCalled()
        {
            // Arrange
            this.sut.Initialise();

            // Act and assert
            TestHelper.TestForInvalidOperationException(() => this.sut.Deactivate(), "Cannot perform this operation because the page presenter is not started.");
        }

        /// <summary>
        /// Test that <see cref="WizardPagePresenterBase.Commit"/> throws exception if <see cref="WizardPagePresenterBase.Start"/> has not been called first.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void CommitThrowsExceptionIfStartNotCalled()
        {
            // Arrange
            this.sut.Initialise();

            // Act and assert
            TestHelper.TestForInvalidOperationException(() => this.sut.Commit(), "Cannot perform this operation because the page presenter is not started.");
        }

        /// <summary>
        /// Test that <see cref="WizardPagePresenterBase.Cancel"/> throws exception if <see cref="WizardPagePresenterBase.Start"/> has not been called first.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void CancelThrowsExceptionIfStartNotCalled()
        {
            // Arrange
            this.sut.Initialise();

            // Act and assert
            TestHelper.TestForInvalidOperationException(() => this.sut.Cancel(), "Cannot perform this operation because the page presenter is not started.");
        }

        /// <summary>
        /// Test that <see cref="WizardPagePresenterBase.Commit"/> commits the page.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void CommitCommitsThePage()
        {
            // Arrange
            this.sut.Initialise();
            this.sut.Start();

            // Act
            this.sut.Commit();

            // Assert
            Assert.IsTrue(this.sut.Committed, "The presenter was not committed.");
        }

        /// <summary>
        /// Test that <see cref="WizardPagePresenterBase.Cancel"/> cancels the page.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void CancelCancelsThePage()
        {
            // Arrange
            this.sut.Initialise();
            this.sut.Start();

            // Act
            this.sut.Cancel();

            // Assert
            Assert.IsTrue(this.sut.Cancelled, "The presenter was not cancelled.");
        }
        
        /// <summary>
        /// The derived presenter class used to test the base presenter.
        /// </summary>
        private class DerivedPage : WizardPagePresenterBase
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="DerivedPage"/> class.
            /// </summary>
            /// <param name="wizardView">The wizard view.</param>
            /// <param name="pageView">The page view.</param>
            public DerivedPage(IWizardViewBase wizardView, IWizardPageView pageView) : base(wizardView, pageView)
            {
            }

            /// <summary>
            /// Gets the title of the page.
            /// </summary>
            public override string Title
            {
                get
                {
                    return string.Empty;
                }
            }

            /// <summary>
            /// Gets a value indicating whether the presenter was started.
            /// </summary>
            public bool Started { get; private set; }

            /// <summary>
            /// Gets a value indicating whether the presenter was activated.
            /// </summary>
            public bool Activated { get; private set; }

            /// <summary>
            /// Gets a value indicating whether the presenter was committed.
            /// </summary>
            public bool Committed { get; private set; }

            /// <summary>
            /// Gets a value indicating whether the presenter was cancelled.
            /// </summary>
            public bool Cancelled { get; private set; }

            /// <summary>
            /// Causes the <see cref="ValidityChanged"/> event to be raised
            /// </summary>
            public void ChangeValidity()
            {
                this.OnValidityChanged();
            }

            /// <summary>
            /// Starts the presenter.
            /// </summary>
            protected override void OnStart()
            {
                this.Started = true;
            }

            /// <summary>
            /// Activates the presenter.
            /// </summary>
            protected override void OnActivate()
            {
                this.Activated = true;
            }

            /// <summary>
            /// Commits the presenter.
            /// </summary>
            protected override void OnCommit()
            {
                this.Committed = true;
            }

            /// <summary>
            /// Cancels the presenter.
            /// </summary>
            protected override void OnCancel()
            {
                this.Cancelled = true;
            }
        }
    }
}
