//---------------------------------------------------------------------
// <copyright file="WizardPresenterBaseTests.cs" company="Microsoft">
//    Copyright (c) Microsoft. All rights reserved.
//    This code is licensed under the MIT License.
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF 
//    ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
//    TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
//    PARTICULAR PURPOSE AND NONINFRINGEMENT.
// </copyright>
// <summary>The WizardPresenterBaseTests type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.Word4Tfs.Library.Test.UnitTest.Presenter
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using Microsoft.Practices.Unity;
    using Microsoft.TeamFoundation;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Word4Tfs.Library.Presenter;
    using Microsoft.Word4Tfs.Library.View;
    using Moq;

    /// <summary>
    /// Tests the <see cref="WizardPresenterBase"/> class.
    /// </summary>
    [TestClass]
    [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Unity container disposed in the test cleanup code.")]
    public class WizardPresenterBaseTests
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
        /// The derived presenter that is used to test the base presenter
        /// </summary>
        private DerivedWizard sut;

        /// <summary>
        /// The list of call counts for the <see cref="IWizardPagePresenter.Activate"/> call.
        /// </summary>
        private List<int> wizardPagePresenterActivateCallCounts = new List<int>();

        /// <summary>
        /// The list of call counts for the <see cref="IWizardPagePresenter.Deactivate"/> call.
        /// </summary>
        private List<int> wizardPagePresenterDeactivateCallCounts = new List<int>();

        /// <summary>
        /// The list of mock presenters to be used by the test wizard presenter when it is started.
        /// </summary>
        private List<Mock<IWizardPagePresenter>> wizardPagePresenters = new List<Mock<IWizardPagePresenter>>();

        /// <summary>
        /// Tracks the state of the wizard's Previous button
        /// </summary>
        private bool previousEnabled;

        /// <summary>
        /// Tracks the state of the wizard's Next button
        /// </summary>
        private bool nextEnabled;

        /// <summary>
        /// Tracks the state of the wizard's Finish button
        /// </summary>
        private bool finishEnabled;

        /// <summary>
        /// Tracks the state of the wizard's Cancel button
        /// </summary>
        private bool cancelEnabled;

        /// <summary>
        /// Initializes the test.
        /// </summary>
        [TestInitialize]
        public void InitializeTest()
        {
            this.container = new UnityContainer();

            this.mockWizardView = TestHelper.CreateAndRegisterMock<IWizardViewBase>(this.container);
            this.mockWizardView.Setup(view => view.SetButtonState(WizardButton.Previous, It.IsAny<bool>())).Callback((Enum button, bool enabled) => this.previousEnabled = enabled);
            this.mockWizardView.Setup(view => view.SetButtonState(WizardButton.Next, It.IsAny<bool>())).Callback((Enum button, bool enabled) => this.nextEnabled = enabled);
            this.mockWizardView.Setup(view => view.SetButtonState(WizardButton.Finish, It.IsAny<bool>())).Callback((Enum button, bool enabled) => this.finishEnabled = enabled);
            this.mockWizardView.Setup(view => view.SetButtonState(WizardButton.Cancel, It.IsAny<bool>())).Callback((Enum button, bool enabled) => this.cancelEnabled = enabled);
            this.mockWizardView.SetupProperty<string>(view => view.Title, null);

            this.sut = new DerivedWizard(this.wizardPagePresenters, this.mockWizardView.Object);
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
        /// Initializes the wizard view.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void InitialisingTheWizardInitializesTheWizardView()
        {
            // Arrange
            this.CreateMockPagePresenters(1);

            // Act
            this.sut.Initialise();

            // Assert
            this.mockWizardView.Verify(v => v.Initialize(), Times.Once());
        }

        /// <summary>
        /// Initializes all the presenters in the list when the wizard is initialised.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void InitialisingTheWizardInitializesAllThePagePresenters()
        {
            // Arrange
            this.CreateMockPagePresenters(2);

            // Act
            this.sut.Initialise();

            // Assert
            this.wizardPagePresenters[0].Verify(p => p.Initialise(), Times.Once());
            this.wizardPagePresenters[1].Verify(p => p.Initialise(), Times.Once());
        }

        /// <summary>
        /// Test that initialising the wizard with an empty list of page presenters throws an <see cref="InvalidOperationException"/>.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void InitialisingWizardWithEmptyListOfPagePresentersThrowsInvalidOperationException()
        {
            TestHelper.TestForInvalidOperationException(() => this.sut.Initialise(), "Wizard must be initialized with a non-empty list of page presenters.");
        }

        /// <summary>
        /// Initializing twice throws an exception.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void InitialisingTwiceThrowsException()
        {
            // Arrange
            this.CreateMockPagePresenters(2);
            this.sut.Initialise();

            // Act
            TestHelper.TestForInvalidOperationException(() => this.sut.Initialise(), "Wizard already initialized.");
        }

        /// <summary>
        /// Test that starting the wizard before initialising it throws an <see cref="InvalidOperationException"/>.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void StartingTheWizardBeforeInitialisingItThrowsInvalidOperationException()
        {
            TestHelper.TestForInvalidOperationException(() => this.sut.Start(), "Wizard must be initialized before it is started");
        }

        /// <summary>
        /// Starts all the presenters in the list when the wizard is started.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void StartingTheWizardStartsAllThePresenters()
        {
            // Arrange
            this.CreateMockPagePresenters(2);
            this.sut.Initialise();

            // Act
            this.sut.Start();

            // Assert
            this.wizardPagePresenters[0].Verify(p => p.Start(), Times.Once());
            this.wizardPagePresenters[1].Verify(p => p.Start(), Times.Once());
        }

        /// <summary>
        /// Starting the wizard twice throws exception.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void StartingTheWizardTwiceThrowsException()
        {
            // Arrange
            this.CreateMockPagePresenters(2);
            this.sut.Initialise();
            this.sut.Start();

            // Act
            TestHelper.TestForInvalidOperationException(() => this.sut.Start(), "Wizard already started.");
        }

        /// <summary>
        /// Activates the first presenter in the list when the wizard is started.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void StartingTheWizardActivatesOnlyTheFirstPresenter()
        {
            // Arrange
            this.CreateMockPagePresenters(2);
            this.sut.Initialise();

            // Act
            this.sut.Start();

            // Assert
            this.wizardPagePresenters[0].Verify(p => p.Activate(), Times.Once());
            this.wizardPagePresenters[1].Verify(p => p.Activate(), Times.Never());
        }

        /// <summary>
        /// Shows the title of the first presenter in the list when the wizard is started.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void StartingTheWizardShowsTheTitleOfTheFirstPresenter()
        {
            // Arrange
            this.CreateMockPagePresenters(2);
            this.sut.Initialise();

            // Act
            this.sut.Start();

            // Assert
            Assert.AreEqual<string>("Title 0", this.mockWizardView.Object.Title);
        }

        /// <summary>
        /// Tests that <see cref="WizardPresenterBase.Start"/> calls the <see cref="IWizardViewBase.StartDialog"/> method.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void StartingTheWizardStartsTheWizardDialog()
        {
            // Arrange
            this.CreateMockPagePresenters(1);
            this.sut.Initialise();

            // Act
            this.sut.Start();

            // Assert
            this.mockWizardView.Verify(wv => wv.StartDialog(), Times.Once());
        }

        /// <summary>
        /// Tests that <see cref="WizardPresenterBase.Start"/> enables the cancel button.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void StartingTheWizardEnablesTheCancelButton()
        {
            // Arrange
            this.CreateMockPagePresenters(1);
            this.sut.Initialise();

            // Act
            this.sut.Start();

            // Assert
            this.AssertCancelIsEnabled();
        }

        /// <summary>
        /// Tests that <see cref="WizardPresenterBase.Start"/> disables the previous button.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void StartingTheWizardDisablesThePreviousButton()
        {
            // Arrange
            this.CreateMockPagePresenters(1);
            this.sut.Initialise();

            // Act
            this.sut.Start();

            // Assert
            this.AssertPreviousButtonIsDisabled();
        }

        /// <summary>
        /// Tests that <see cref="WizardPresenterBase.Start"/> disables the next button if the first page is invalid on starting.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void StartingTheWizardDisablesTheNextButtonIfFirstPageIsInvalidOnStarting()
        {
            // Arrange
            this.CreateMockPagePresenters(1);
            this.SetPageValidity(0, false);
            this.sut.Initialise();

            // Act
            this.sut.Start();

            // Assert
            this.AssertNextButtonIsDisabled();
        }

        /// <summary>
        /// Tests that <see cref="WizardPresenterBase.Start"/> enables the next button if the first page is valid on starting.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void StartingTheWizardEnablesTheNextButtonIfFirstPageIsValidOnStarting()
        {
            // Arrange
            this.CreateMockPagePresenters(2);
            this.SetPageValidity(0, true);
            this.SetPageValidity(1, false);
            this.sut.Initialise();

            // Act
            this.sut.Start();

            // Assert
            this.AssertNextButtonIsEnabled();
        }

        /// <summary>
        /// Tests that <see cref="WizardPresenterBase"/> enables the next button if the first page is invalid on starting but then becomes valid.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void StartingTheWizardEnablesTheNextButtonIfFirstPageIsInvalidOnStartingButThenBecomesValid()
        {
            // Arrange
            this.CreateMockPagePresenters(2);
            this.SetPageValidity(0, false);
            this.SetPageValidity(1, false);
            this.sut.Initialise();
            this.sut.Start();

            // Act
            this.SetPageValidity(0, true);
            this.SignalValidityChange(0);

            // Assert
            this.AssertNextButtonIsEnabled();
        }

        /// <summary>
        /// Tests that <see cref="WizardPresenterBase"/> disables the next button if the first page is valid on starting but then becomes invalid.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void StartingTheWizardDisablesTheNextButtonIfFirstPageIsValidOnStartingButThenBecomesInvalid()
        {
            // Arrange
            this.CreateMockPagePresenters(1);
            this.SetPageValidity(0, true);
            this.sut.Initialise();
            this.sut.Start();

            // Act
            this.SetPageValidity(0, false);
            this.SignalValidityChange(0);

            // Assert
            this.AssertNextButtonIsDisabled();
        }

        /// <summary>
        /// Tests that <see cref="WizardPresenterBase.Start"/> disables the finish button one of the other pages is not valid.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void StartingTheWizardDisablesTheFinishButtonIfOtherPageIsInvalidOnStarting()
        {
            // Arrange
            this.CreateMockPagePresenters(2);
            this.SetPageValidity(0, true);
            this.SetPageValidity(1, false);
            this.sut.Initialise();

            // Act
            this.sut.Start();

            // Assert
            this.AssertFinishButtonIsDisabled();
        }

        /// <summary>
        /// Tests that <see cref="WizardPresenterBase.Start"/> enables the finish button all the pages are valid.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void StartingTheWizardEnablesTheFinishButtonAllPagesAreValidOnStarting()
        {
            // Arrange
            this.CreateMockPagePresenters(2);
            this.SetPageValidity(0, true);
            this.SetPageValidity(1, true);
            this.sut.Initialise();

            // Act
            this.sut.Start();

            // Assert
            this.AssertFinishButtonIsEnabled();
        }

        /// <summary>
        /// Tests that <see cref="WizardPresenterBase"/> does not enable the next button if a different page becomes valid.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void TheWizardDoesNotEnableTheFinishButtonIfADifferentPageBecomesValid()
        {
            // Arrange
            this.CreateMockPagePresenters(2);
            this.SetPageValidity(0, false);
            this.SetPageValidity(1, false);
            this.sut.Initialise();
            this.sut.Start();

            // Act
            this.SetPageValidity(1, true);
            this.SignalValidityChange(1);

            // Assert
            this.AssertNextButtonIsDisabled();
        }

        /// <summary>
        /// Tests that <see cref="WizardPresenterBase"/> enables the finish button if a different page becomes valid so they are all valid.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void TheWizardDoesEnableTheFinishButtonIfADifferentPageBecomesValidSoTheyAreAllValid()
        {
            // Arrange
            this.CreateMockPagePresenters(2);
            this.SetPageValidity(0, true);
            this.SetPageValidity(1, false);
            this.sut.Initialise();
            this.sut.Start();

            // Act
            this.SetPageValidity(1, true);
            this.SignalValidityChange(1);

            // Assert
            this.AssertFinishButtonIsEnabled();
        }

        /// <summary>
        /// Tests that <see cref="WizardPresenterBase"/> activates the next view and deactivates the current view when the user clicks next.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void TheWizardMovesToTheNextViewWhenNextIsClicked()
        {
            // Arrange
            this.CreateMockPagePresenters(2);
            this.SetPageValidity(0, true);
            this.SetPageValidity(1, false);
            this.sut.Initialise();
            this.sut.Start();

            // Act
            this.RaiseNext();

            // Assert
            this.wizardPagePresenters[0].Verify(p => p.Deactivate(), Times.Once());
            this.wizardPagePresenters[1].Verify(p => p.Activate(), Times.Once());
        }

        /// <summary>
        /// Tests that <see cref="WizardPresenterBase"/> sets changes the title when the user clicks next.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void TheWizardChangesTheTitleWhenNextIsClicked()
        {
            // Arrange
            this.CreateMockPagePresenters(2);
            this.SetPageValidity(0, true);
            this.SetPageValidity(1, false);
            this.sut.Initialise();
            this.sut.Start();

            // Act
            this.RaiseNext();

            // Assert
            Assert.AreEqual<string>("Title 1", this.mockWizardView.Object.Title);
        }

        /// <summary>
        /// Tests that <see cref="WizardPresenterBase"/> disables next button when on the last page.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void TheWizardDisablesNextButtonWhenOnLastPage()
        {
            // Arrange
            this.CreateMockPagePresenters(2);
            this.SetPageValidity(0, true);
            this.SetPageValidity(1, true);
            this.sut.Initialise();
            this.sut.Start();

            // Act
            this.RaiseNext();

            // Assert
            this.AssertNextButtonIsDisabled();
        }

        /// <summary>
        /// Tests that <see cref="WizardPresenterBase"/> enables previous button when not on the first page.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void TheWizardEnablesPreviousButtonWhenNotOnFirstPage()
        {
            // Arrange
            this.CreateMockPagePresenters(2);
            this.wizardPagePresenters[0].Setup(p => p.IsValid).Returns(true);
            this.wizardPagePresenters[1].Setup(p => p.IsValid).Returns(true);
            this.sut.Initialise();
            this.sut.Start();

            // Act
            this.RaiseNext();

            // Assert
            this.AssertPreviousButtonIsEnabled();
        }

        /// <summary>
        /// Tests that <see cref="WizardPresenterBase"/> finish button is not enabled when moving to last page which is not valid.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void TheWizardDisablesFinishButtonWhenMovingToLastPageWhichIsNotValid()
        {
            // Arrange
            this.CreateMockPagePresenters(2);
            this.wizardPagePresenters[0].Setup(p => p.IsValid).Returns(true);
            this.wizardPagePresenters[1].Setup(p => p.IsValid).Returns(false);
            this.sut.Initialise();
            this.sut.Start();

            // Act
            this.RaiseNext();

            // Assert
            this.AssertFinishButtonIsDisabled();
        }

        /// <summary>
        /// Tests that <see cref="WizardPresenterBase"/> finish button is enabled when moving to last page which is valid.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void TheWizardEnablesFinishButtonWhenMovingToLastPageWhichIsValid()
        {
            // Arrange
            this.CreateMockPagePresenters(2);
            this.wizardPagePresenters[0].Setup(p => p.IsValid).Returns(true);
            this.wizardPagePresenters[1].Setup(p => p.IsValid).Returns(true);
            this.sut.Initialise();
            this.sut.Start();

            // Act
            this.RaiseNext();

            // Assert
            this.AssertFinishButtonIsEnabled();
        }

        /// <summary>
        /// Tests that <see cref="WizardPresenterBase"/> finish button is enabled when on last page and it becomes valid.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void TheWizardEnablesFinishButtonWhenOnLastPageAndItBecomesValid()
        {
            // Arrange
            this.CreateMockPagePresenters(2);
            this.SetPageValidity(0, true);
            this.SetPageValidity(1, false);
            this.sut.Initialise();
            this.sut.Start();
            this.RaiseNext();

            // Act
            this.SetPageValidity(1, true);
            this.SignalValidityChange(1);

            // Assert
            this.AssertFinishButtonIsEnabled();
        }

        /// <summary>
        /// Tests that <see cref="WizardPresenterBase"/> finish button is disabled when on last page and it becomes invalid.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void TheWizardDisablesFinishButtonWhenOnLastPageAndItBecomesInvalid()
        {
            // Arrange
            this.CreateMockPagePresenters(2);
            this.SetPageValidity(0, true);
            this.SetPageValidity(1, true);
            this.sut.Initialise();
            this.sut.Start();
            this.RaiseNext();

            // Act
            this.SetPageValidity(1, false);
            this.SignalValidityChange(1);

            // Assert
            this.AssertFinishButtonIsDisabled();
        }

        /// <summary>
        /// Tests that <see cref="WizardPresenterBase"/> activates the previous view and deactivates the current view when the user clicks previous.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void TheWizardMovesToThePreviousViewWhenPreviousIsClicked()
        {
            // Arrange
            this.CreateMockPagePresenters(2);
            this.SetPageValidity(0, true);
            this.SetPageValidity(1, false);
            this.sut.Initialise();
            this.sut.Start();
            this.RaiseNext();
            this.ResetPagePresenterActivationCallCounts();

            // Act
            this.RaisePrevious();

            // Assert
            Assert.AreEqual<int>(1, this.wizardPagePresenterDeactivateCallCounts[1]);
            Assert.AreEqual<int>(1, this.wizardPagePresenterActivateCallCounts[0]);
        }

        /// <summary>
        /// Tests that <see cref="WizardPresenterBase"/> changes the title when the user clicks previous.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void TheWizardChangesTheTitleWhenPreviousIsClicked()
        {
            // Arrange
            this.CreateMockPagePresenters(2);
            this.SetPageValidity(0, true);
            this.SetPageValidity(1, false);
            this.sut.Initialise();
            this.sut.Start();
            this.RaiseNext();
            this.ResetPagePresenterActivationCallCounts();

            // Act
            this.RaisePrevious();

            // Assert
            Assert.AreEqual<string>("Title 0", this.mockWizardView.Object.Title);
        }

        /// <summary>
        /// Tests that <see cref="WizardPresenterBase"/> disables previous button when moving back to first page.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void TheWizardDisablesPreviousButtonWhenMoveBackToFirstPage()
        {
            // Arrange
            this.CreateMockPagePresenters(2);
            this.SetPageValidity(0, true);
            this.SetPageValidity(1, true);
            this.sut.Initialise();
            this.sut.Start();
            this.RaiseNext();

            // Act
            this.RaisePrevious();

            // Assert
            this.AssertPreviousButtonIsDisabled();
        }

        /// <summary>
        /// Tests that <see cref="WizardPresenterBase.Start"/> returns true when the wizard finishes.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void StartReturnsTrueWhenWizardFinishes()
        {
            // Arrange
            this.CreateMockPagePresenters(2);
            this.SetPageValidity(0, true);
            this.SetPageValidity(1, true);
            this.sut.Initialise();
            this.mockWizardView.Setup(v => v.StartDialog()).Returns(true);

            // Act
            bool ans = this.sut.Start();

            // Assert
            Assert.IsTrue(ans, "Start should return true if the wizard finished successfully");
        }

        /// <summary>
        /// Tests that <see cref="WizardPresenterBase"/> commits each page when the wizard finishes.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void TheWizardCommitsEachPageWhenWizardFinishes()
        {
            // Arrange
            this.CreateMockPagePresenters(2);
            this.SetPageValidity(0, true);
            this.SetPageValidity(1, true);
            this.sut.Initialise();
            this.mockWizardView.Setup(v => v.StartDialog()).Returns(true);

            // Act
            this.sut.Start();

            // Assert
            this.wizardPagePresenters[0].Verify(p => p.Commit(), Times.Once());
            this.wizardPagePresenters[1].Verify(p => p.Commit(), Times.Once());
        }

        /// <summary>
        /// Tests that <see cref="WizardPresenterBase"/> commits the wizard when the wizard finishes.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void TheWizardCommitsWhenWizardFinishes()
        {
            // Arrange
            this.CreateMockPagePresenters(1);
            this.SetPageValidity(0, true);
            this.sut.Initialise();
            this.mockWizardView.Setup(v => v.StartDialog()).Returns(true);

            // Act
            this.sut.Start();

            // Assert
            Assert.IsTrue(this.sut.Committed, "The wizard was not committed");
        }

        /// <summary>
        /// Tests that <see cref="WizardPresenterBase.Start"/> returns false when the wizard is cancelled.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void StartReturnsFalseWhenWizardIsCancelled()
        {
            // Arrange
            this.CreateMockPagePresenters(2);
            this.SetPageValidity(0, true);
            this.SetPageValidity(1, true);
            this.sut.Initialise();
            this.mockWizardView.Setup(v => v.StartDialog()).Returns(false);

            // Act
            bool ans = this.sut.Start();

            // Assert
            Assert.IsFalse(ans, "Start should return false if the wizard is cancelled");
        }

        /// <summary>
        /// Tests that <see cref="WizardPresenterBase"/> does not commit any page when the wizard is cancelled.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void TheWizardDoesNotCommitAnyPageWhenWizardIsCancelled()
        {
            // Arrange
            this.CreateMockPagePresenters(2);
            this.SetPageValidity(0, true);
            this.SetPageValidity(1, true);
            this.sut.Initialise();
            this.mockWizardView.Setup(v => v.StartDialog()).Returns(false);

            // Act
            this.sut.Start();

            // Assert
            this.wizardPagePresenters[0].Verify(p => p.Commit(), Times.Never());
            this.wizardPagePresenters[1].Verify(p => p.Commit(), Times.Never());
        }

        /// <summary>
        /// Tests that <see cref="WizardPresenterBase"/> cancels all pages when the wizard is cancelled.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void TheWizardCancelsAllPagesWhenWizardIsCancelled()
        {
            // Arrange
            this.CreateMockPagePresenters(2);
            this.SetPageValidity(0, true);
            this.SetPageValidity(1, true);
            this.sut.Initialise();
            this.mockWizardView.Setup(v => v.StartDialog()).Returns(false);

            // Act
            this.sut.Start();

            // Assert
            this.wizardPagePresenters[0].Verify(p => p.Cancel(), Times.Once());
            this.wizardPagePresenters[1].Verify(p => p.Cancel(), Times.Once());
        }

        /// <summary>
        /// Tests that <see cref="WizardPresenterBase"/> displays an error if there is a TFS exception.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void TheWizardDisplaysErrorIfThereIsATFSException()
        {
            // Arrange
            this.CreateMockPagePresenters(1);
            this.wizardPagePresenters[0].Setup(p => p.Start()).Throws(new TeamFoundationServerException("test"));
            this.sut.Initialise();

            // Act
            this.sut.Start();

            // Assert
            this.mockWizardView.Verify(view => view.DisplayMessage(UIMessageType.Error, "Team Foundation Error", "test", TestHelper.MessageStartsWith("test")), Times.Once());
        }

        /// <summary>
        /// Creates page presenters for the test.
        /// </summary>
        /// <param name="n">The number of page presenters to create.</param>
        private void CreateMockPagePresenters(int n)
        {
            this.wizardPagePresenterActivateCallCounts = new List<int>(new int[n]);
            this.wizardPagePresenterDeactivateCallCounts = new List<int>(new int[n]);
            for (int i = 0; i < n; i++)
            {
                Mock<IWizardPagePresenter> mockPagePresenter = new Mock<IWizardPagePresenter>();
                string title = "Title " + i.ToString(CultureInfo.InvariantCulture);
                mockPagePresenter.Setup(p => p.Title).Returns(() => title);
                this.wizardPagePresenterActivateCallCounts[i] = 0;
                this.wizardPagePresenterDeactivateCallCounts[i] = 0;
                mockPagePresenter.Setup(p => p.Activate()).Callback(() => 
                    {
                        int ix = this.wizardPagePresenters.IndexOf(mockPagePresenter);
                        this.wizardPagePresenterActivateCallCounts[ix] = this.wizardPagePresenterActivateCallCounts[ix] + 1;
                    });
                mockPagePresenter.Setup(p => p.Deactivate()).Callback(() =>
                {
                    int ix = this.wizardPagePresenters.IndexOf(mockPagePresenter);
                    this.wizardPagePresenterDeactivateCallCounts[ix] = this.wizardPagePresenterDeactivateCallCounts[ix] + 1;
                });
                this.wizardPagePresenters.Add(mockPagePresenter);
            }
        }

        /// <summary>
        /// Resets our own tracking of the call counts to activate and deactivate a page presenter.
        /// </summary>
        private void ResetPagePresenterActivationCallCounts()
        {
            for (int i = 0; i < this.wizardPagePresenters.Count; i++)
            {
                this.wizardPagePresenterActivateCallCounts[i] = 0;
                this.wizardPagePresenterDeactivateCallCounts[i] = 0;
            }
        }

        /// <summary>
        /// Sets the validity of a page.
        /// </summary>
        /// <param name="pageIndex">The index of the page.</param>
        /// <param name="validity">Indicates the validity of the page.</param>
        private void SetPageValidity(int pageIndex, bool validity)
        {
            this.wizardPagePresenters[pageIndex].Setup(p => p.IsValid).Returns(validity);
        }

        /// <summary>
        /// Raises an event to say that a page has changed validity.
        /// </summary>
        /// <param name="pageIndex">The index of the page.</param>
        private void SignalValidityChange(int pageIndex)
        {
            this.wizardPagePresenters[pageIndex].Raise(p => p.ValidityChanged += null, EventArgs.Empty);
        }

        /// <summary>
        /// Raises the event to move the wizard to the next page.
        /// </summary>
        private void RaiseNext()
        {
            this.mockWizardView.Raise(wv => wv.NextPage += null, EventArgs.Empty);
        }

        /// <summary>
        /// Raises the event to move the wizard to the previous page.
        /// </summary>
        private void RaisePrevious()
        {
            this.mockWizardView.Raise(wv => wv.PreviousPage += null, EventArgs.Empty);
        }

        /// <summary>
        /// Asserts that the cancel button is enabled.
        /// </summary>
        private void AssertCancelIsEnabled()
        {
            Assert.IsTrue(this.cancelEnabled, "Cancel button should be enabled");
        }

        /// <summary>
        /// Asserts that the previous button is disabled.
        /// </summary>
        private void AssertPreviousButtonIsDisabled()
        {
            Assert.IsFalse(this.previousEnabled, "Previous button should be disabled");
        }

        /// <summary>
        /// Asserts that the previous button is enabled.
        /// </summary>
        private void AssertPreviousButtonIsEnabled()
        {
            Assert.IsTrue(this.previousEnabled, "Previous button should be enabled");
        }

        /// <summary>
        /// Asserts that the next button is disabled.
        /// </summary>
        private void AssertNextButtonIsDisabled()
        {
            Assert.IsFalse(this.nextEnabled, "Next button should be disabled");
        }

        /// <summary>
        /// Asserts that the next button is enabled.
        /// </summary>
        private void AssertNextButtonIsEnabled()
        {
            Assert.IsTrue(this.nextEnabled, "Next button should be enabled");
        }

        /// <summary>
        /// Asserts that the finish button is disabled.
        /// </summary>
        private void AssertFinishButtonIsDisabled()
        {
            Assert.IsFalse(this.finishEnabled, "Finish button should be disabled");
        }

        /// <summary>
        /// Asserts that the finish button is enabled.
        /// </summary>
        private void AssertFinishButtonIsEnabled()
        {
            Assert.IsTrue(this.finishEnabled, "Finish button should be enabled");
        }

        /// <summary>
        /// Test wizard class used to test the base class
        /// </summary>
        private class DerivedWizard : WizardPresenterBase<IWizardViewBase>
        {
            /// <summary>
            /// The wizard page presenters to be used when the wizard is initialized.
            /// </summary>
            private IEnumerable<Mock<IWizardPagePresenter>> wizardPagePresenters;

            /// <summary>
            /// Initializes a new instance of the <see cref="DerivedWizard"/> class.
            /// </summary>
            /// <param name="wizardPagePresenters">The wizard page presenters to be used when the wizard is initialized.</param>
            /// <param name="wizardView">The wizard view.</param>
            /// <remarks>
            /// A real wizard presenter will know which presenters it uses internally, this constructor just allows the tests to vary the list for test purposes.
            /// </remarks>
            public DerivedWizard(IEnumerable<Mock<IWizardPagePresenter>> wizardPagePresenters, IWizardViewBase wizardView) : base(wizardView)
            {
                this.wizardPagePresenters = wizardPagePresenters;
            }

            /// <summary>
            /// Gets a value indicating whether the wizard has been committed.
            /// </summary>
            public bool Committed { get; private set; }

            /// <summary>
            /// Gets the title to be used when displaying errors.
            /// </summary>
            protected override string ErrorTitle
            {
                get
                {
                    return "Team Foundation Error";
                }
            }

            /// <summary>
            /// Initialises the wizard presenter and all the page presenters.
            /// </summary>
            public override void Initialise()
            {
                this.OnInitialise(this.wizardPagePresenters.Select(mock => mock.Object));
            }

            /// <summary>
            /// Commits the wizard as a whole.
            /// </summary>
            protected override void OnCommit()
            {
                this.Committed = true;
                foreach (Mock<IWizardPagePresenter> mock in this.wizardPagePresenters)
                {
                    mock.Verify(m => m.Commit(), Times.Once(), "The page presenters must have been committed before the wizard as a whole is committed");
                }
            }
        }
    }
}
