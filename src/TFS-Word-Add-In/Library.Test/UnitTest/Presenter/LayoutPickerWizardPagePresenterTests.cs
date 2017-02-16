//---------------------------------------------------------------------
// <copyright file="LayoutPickerWizardPagePresenterTests.cs" company="Microsoft">
//    Copyright (c) Microsoft. All rights reserved.
//    This code is licensed under the MIT License.
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF 
//    ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
//    TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
//    PARTICULAR PURPOSE AND NONINFRINGEMENT.
// </copyright>
// <summary>The LayoutPickerWizardPagePresenterTests type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.Word4Tfs.Library.Test.UnitTest.Presenter
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Practices.Unity;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Word4Tfs.Library.Data;
    using Microsoft.Word4Tfs.Library.Model;
    using Microsoft.Word4Tfs.Library.Presenter;
    using Microsoft.Word4Tfs.Library.View;
    using Moq;

    /// <summary>
    /// Tests the <see cref="LayoutPickerWizardPagePresenter"/> class.
    /// </summary>
    [TestClass]
    [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Unity container disposed in the test cleanup code.")]
    public class LayoutPickerWizardPagePresenterTests
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
        private Mock<ILayoutPickerWizardPageView> mockView;

        /// <summary>
        /// The mock document to be used by the presenter.
        /// </summary>
        private Mock<ITeamProjectTemplate> mockTemplate;

        /// <summary>
        /// The presenter to be tested.
        /// </summary>
        private ILayoutPickerWizardPagePresenter sut;

        /// <summary>
        /// Initializes the test.
        /// </summary>
        [TestInitialize]
        public void InitializeTest()
        {
            this.container = new UnityContainer();

            this.mockWizardView = TestHelper.CreateAndRegisterMock<IWorkItemQueryAndLayoutPickerWizardView>(this.container);
            this.mockView = TestHelper.CreateAndRegisterMock<ILayoutPickerWizardPageView>(this.container);
            this.mockTemplate = TestHelper.CreateAndRegisterMock<ITeamProjectTemplate>(this.container);

            this.sut = this.container.Resolve<LayoutPickerWizardPagePresenter>();
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
            Assert.AreEqual<string>("Choose the Layout", this.sut.Title);
        }

        /// <summary>
        /// Test that <see cref="ILayoutPickerWizardPagePresenter.Initialise"/> leaves the page invalid.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void InitialiseMakesPageInvalid()
        {
            // Assert
            Assert.IsFalse(this.sut.IsValid, "The page should initially be invalid.");
        }

        /// <summary>
        /// Test that <see cref="ILayoutPickerWizardPagePresenter.Start"/> leaves the page invalid.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void StartMakesPageInvalid()
        {
            // Arrange
            this.SetupDummyLayoutListOnTemplate("Layout1");

            // Act
            this.sut.Start();

            // Assert
            Assert.IsFalse(this.sut.IsValid, "The page should initially be invalid.");
        }

        /// <summary>
        /// Tests that the <see cref="ILayoutPickerWizardPagePresenter.Start"/> gets the list of layouts and passes it to the view in alphabetical order.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void StartPassesLayoutsToViewForDisplayInAlphabeticalOrder()
        {
            // Arrange
            LayoutInformation[] layouts = this.SetupDummyLayoutListOnTemplate("Layout2", "Layout1").ToArray();

            // Act
            this.sut.Start();

            // Assert
            this.mockView.Verify(view => view.LoadLayouts(TestHelper.OrderedArray<LayoutInformation>(layouts[1], layouts[0])), Times.Once(), "The list of layouts should be passed to the view, and should be sorted.");
        }

        /// <summary>
        /// Tests that the <see cref="ILayoutPickerWizardPagePresenter.Start"/> does not select a layout.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void StartDoesNotSelectALayout()
        {
            // Arrange
            LayoutInformation[] layouts = this.SetupDummyLayoutListOnTemplate("Layout2", "Layout1").ToArray();

            // Act
            this.sut.Start();

            // Assert
            this.mockView.Verify(view => view.SelectLayout(It.IsAny<string>()), Times.Never(), "No layout should be selected.");
        }

        /// <summary>
        /// Tests that the <see cref="ILayoutPickerWizardPagePresenter.Start"/> does not raise a validity event.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void StartDoesNotRaiseValidityEvent()
        {
            // Arrange
            LayoutInformation[] layouts = this.SetupDummyLayoutListOnTemplate("Layout2", "Layout1").ToArray();
            bool validityEventRaised = false;
            this.sut.ValidityChanged += new EventHandler((s, e) => validityEventRaised = true);

            // Act
            this.sut.Start();

            // Assert
            Assert.IsFalse(validityEventRaised, "The validity event should not be raised.");
        }

        /// <summary>
        /// When a layout is selected, change page to be valid.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void SelectingALayoutMakesPageValid()
        {
            // Arrange
            IEnumerable<LayoutInformation> layouts = this.SetupDummyLayoutListOnTemplate("Layout1");
            bool validityEventRaised = false;
            this.sut.Start();
            this.sut.ValidityChanged += new EventHandler((s, e) => validityEventRaised = true);

            // Act
            this.UserSelectsLayout(TestHelper.CreateTestLayoutInformation(new BuildingBlockName[0]));

            // Assert
            Assert.IsTrue(validityEventRaised, "Validity event should have been raised");
            Assert.IsTrue(this.sut.IsValid, "Selecting a layout should make the page valid.");
        }

        /// <summary>
        /// Initial select on start does not make the page valid. This is to force the user to have to click Next on the wizard
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void InitialSelectOnStartDoesNotMakePageValid()
        {
            // Arrange
            IEnumerable<LayoutInformation> layouts = this.SetupDummyLayoutListOnTemplate("Layout1");
            bool validityEventRaised = false;
            this.mockView.Setup(v => v.SelectLayout(It.IsAny<string>())).Callback(() => this.UserSelectsLayout(layouts.First()));
            this.sut.ValidityChanged += new EventHandler((s, e) => validityEventRaised = true);

            // Act
            this.sut.Start();

            // Assert
            Assert.IsFalse(validityEventRaised, "Validity event should not have been raised");
            Assert.IsFalse(this.sut.IsValid, "Page should not be valid.");
        }

        /// <summary>
        /// Sets <see cref="ILayoutPickerWizardPagePresenter.SelectedLayout"/> when a layout is selected.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void SelectedLayoutIsSetWhenALayoutIsSelected()
        {
            // Arrange
            LayoutInformation testLayout = this.SetupDummyLayoutListOnTemplate("Layout1").First();
            this.sut.Start();

            // Act
            this.UserSelectsLayout(testLayout);

            // Assert
            Assert.AreSame(testLayout, this.sut.SelectedLayout, "Selected layout not returned.");
        }

        /// <summary>
        /// Creates a dummy list of layouts on the mock template.
        /// </summary>
        /// <param name="layoutNames">The list of layouts to create.</param>
        /// <returns>The dummy list of layouts.</returns>
        private IEnumerable<LayoutInformation> SetupDummyLayoutListOnTemplate(params string[] layoutNames)
        {
            LayoutInformation[] layouts = layoutNames.Select(n => TestHelper.CreateTestLayoutInformation(n)).ToArray();
            this.mockTemplate.Setup(template => template.Layouts).Returns(layouts);
            return layouts;
        }

        /// <summary>
        /// Raises an event for the user selecting a layout.
        /// </summary>
        /// <param name="layout">The selected layout.</param>
        private void UserSelectsLayout(LayoutInformation layout)
        {
            this.mockView.Raise(view => view.LayoutSelected += null, new LayoutItemEventArgs(layout));
        }
    }
}
