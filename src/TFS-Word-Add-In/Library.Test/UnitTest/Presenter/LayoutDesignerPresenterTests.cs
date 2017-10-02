//---------------------------------------------------------------------
// <copyright file="LayoutDesignerPresenterTests.cs" company="Microsoft">
//    Copyright © ALM | DevOps Ranger Contributors. All rights reserved.
//    This code is licensed under the MIT License.
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF 
//    ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
//    TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
//    PARTICULAR PURPOSE AND NONINFRINGEMENT.
// </copyright>
// <summary>The LayoutDesignerPresenterTests type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.Word4Tfs.Library.Test.UnitTest.Presenter
{
    using global::System;
    using global::System.Collections.Generic;
    using global::System.ComponentModel;
    using global::System.Globalization;
    using global::System.Linq;
    using Microsoft.Practices.Unity;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Word4Tfs.Library.Data;
    using Microsoft.Word4Tfs.Library.Model;
    using Microsoft.Word4Tfs.Library.Model.TeamFoundation;
    using Microsoft.Word4Tfs.Library.Presenter;
    using Microsoft.Word4Tfs.Library.View;
    using Moq;

    /// <summary>
    /// Tests the layout designer presenter.
    /// </summary>
    [TestClass]
    [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Unity container disposed in the test cleanup code.")]
    public class LayoutDesignerPresenterTests
    {
        /// <summary>
        /// The Unity container to be used for Dependency Injection
        /// </summary>
        private IUnityContainer container;

        /// <summary>
        /// The mock view.
        /// </summary>
        private Mock<ILayoutDesignerView> mockView;

        /// <summary>
        /// The mock team ribbon presenter.
        /// </summary>
        private Mock<ITeamRibbonPresenter> mockRibbonPresenter;

        /// <summary>
        /// The mock team project document manager.
        /// </summary>
        private Mock<ITeamProjectDocumentManager> mockManager;

        /// <summary>
        /// The mock team project document that is the designer document.
        /// </summary>
        private Mock<ITeamProjectDocument> mockDesignProjectDocument;

        /// <summary>
        /// The mock team project document that is the document from which the designer was invoked.
        /// </summary>
        private Mock<ITeamProjectDocument> mockInvokingProjectDocument;

        /// <summary>
        /// The mock team project.
        /// </summary>
        private Mock<ITeamProject> mockProject;

        /// <summary>
        /// The mock team project template.
        /// </summary>
        private Mock<ITeamProjectTemplate> mockTemplate;

        /// <summary>
        /// The mock message box view.
        /// </summary>
        private Mock<IMessageBoxView> mockMessageBoxView;

        /// <summary>
        /// The layout designer presenter to be tested.
        /// </summary>
        private ILayoutDesignerPresenter sut;

        /// <summary>
        /// Tracks the state of the Save button.
        /// </summary>
        private bool saveButtonEnabled;

        /// <summary>
        /// Tracks the state of the Connect button.
        /// </summary>
        private bool connectButtonEnabled;

        /// <summary>
        /// Tracks the state of the AddField control.
        /// </summary>
        private bool addFieldControlEnabled;

        /// <summary>
        /// Tracks the state of the AddNew button.
        /// </summary>
        private bool addNewButtonEnabled;
        
        /// <summary>
        /// Tracks the state of the layout currently selected on the view.
        /// </summary>
        private string selectedLayout = string.Empty;

        /// <summary>
        /// Tracks the current list of layouts being displayed by the view.
        /// </summary>
        private LayoutInformation[] currentViewLayoutList;

        /// <summary>
        /// Tracks the clean/dirty status of the designer document.
        /// </summary>
        private bool documentIsClean;

        /// <summary>
        /// Initializes the test.
        /// </summary>
        [TestInitialize]
        public void InitializeTest()
        {
            this.container = new UnityContainer();

            this.mockView = TestHelper.CreateAndRegisterMock<ILayoutDesignerView>(this.container);
            this.mockRibbonPresenter = TestHelper.CreateAndRegisterMock<ITeamRibbonPresenter>(this.container);
            this.mockManager = TestHelper.CreateAndRegisterMock<ITeamProjectDocumentManager>(this.container);
            this.mockMessageBoxView = TestHelper.CreateAndRegisterMock<IMessageBoxView>(this.container);
            this.mockInvokingProjectDocument = new Mock<ITeamProjectDocument>();
            this.mockDesignProjectDocument = new Mock<ITeamProjectDocument>();
            this.mockProject = new Mock<ITeamProject>();
            this.mockTemplate = new Mock<ITeamProjectTemplate>();

            this.mockRibbonPresenter.SetupAllProperties();
            this.mockInvokingProjectDocument.Setup(doc => doc.TeamProject).Returns(this.mockProject.Object);
            this.mockDesignProjectDocument.Setup(doc => doc.TeamProject).Returns(this.mockProject.Object);
            this.SetupManagerForMockTemplate();
            this.SetupManagerForMockDesignDocument();
            this.mockView.Setup(v => v.SetButtonState(LayoutDesignerControl.Save, It.IsAny<bool>())).Callback<Enum, bool>((e, v) => this.saveButtonEnabled = v);
            this.mockView.Setup(v => v.SetButtonState(LayoutDesignerControl.Connect, It.IsAny<bool>())).Callback<Enum, bool>((e, v) => this.connectButtonEnabled = v);
            this.mockView.Setup(v => v.SetButtonState(LayoutDesignerControl.AddField, It.IsAny<bool>())).Callback<Enum, bool>((e, v) => this.addFieldControlEnabled = v);
            this.mockView.Setup(v => v.SetButtonState(LayoutDesignerControl.AddNew, It.IsAny<bool>())).Callback<Enum, bool>((e, v) => this.addNewButtonEnabled = v);
            this.mockView.Setup(v => v.SelectLayout(It.IsAny<string>())).Callback<string>(s => this.selectedLayout = s);
            this.mockView.Setup(v => v.SetLayoutList(It.IsAny<IEnumerable<LayoutInformation>>())).Callback<IEnumerable<LayoutInformation>>(layouts => this.currentViewLayoutList = layouts.ToArray());
            this.mockDesignProjectDocument.Setup(doc => doc.RenameLayoutDefinition(It.IsAny<string>(), It.IsAny<string>()))
                                          .Callback((string oldName, string newName) => this.SetupTemplateWithLayouts(this.currentViewLayoutList.Where(li => li.Name != oldName).Select(li => li.Name).Concat(new string[] { newName }).ToArray()));
            this.mockDesignProjectDocument.Setup(doc => doc.MarkDocumentClean())
                                          .Callback(() => { this.documentIsClean = true; });

            this.sut = this.container.Resolve<LayoutDesignerPresenter>();
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
        /// Test that <see cref="LayoutDesignerPresenter"/> constructor throws exception if the view is null.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void ConstructorThrowsExceptionIfViewIsNull()
        {
            TestHelper.TestForArgumentNullException(() => new LayoutDesignerPresenter(this.container, null, this.mockManager.Object, this.mockRibbonPresenter.Object), "view");
        }

        /// <summary>
        /// Test that <see cref="LayoutDesignerPresenter"/> constructor throws exception if the manager is null.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void ConstructorThrowsExceptionIfManagerIsNull()
        {
            TestHelper.TestForArgumentNullException(() => new LayoutDesignerPresenter(this.container, this.mockView.Object, null, this.mockRibbonPresenter.Object), "manager");
        }

        /// <summary>
        /// Test that <see cref="LayoutDesignerPresenter"/> constructor throws exception if the ribbon presenter is null.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void ConstructorThrowsExceptionIfRibbonPresenterIsNull()
        {
            TestHelper.TestForArgumentNullException(() => new LayoutDesignerPresenter(this.container, this.mockView.Object, this.mockManager.Object, null), "ribbonPresenter");
        }

        /// <summary>
        /// Test that the layout designer view is also passed to <see cref="ITeamRibbonPresenter.LayoutDesignerView"/>.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void LayoutDesignerViewIsGivenToTheTeamRibbonPresenter()
        {
            // Assert
            Assert.AreSame(this.mockView.Object, this.mockRibbonPresenter.Object.LayoutDesignerView, "View not passed to ribbon presenter");
        }

        /// <summary>
        /// Test that <see cref="ILayoutDesignerPresenter.Show"/> causes the view to be shown.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void ShowCausesViewToBeShown()
        {
            // Arrange
            this.SetupTemplateWithLayouts("Layout1");

            // Act
            this.sut.Show();

            // Assert
            this.mockView.Verify(view => view.ShowLayoutDesigner(), "View not made visible");
        }

        /// <summary>
        /// Test that <see cref="ILayoutDesignerPresenter.Show"/> causes the view to be shown even if there are no layouts.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void ShowCausesViewToBeShownEvenIfThereAreNoLayouts()
        {
            // Arrange
            this.SetupTemplateWithLayouts(new string[0]);

            // Act
            this.sut.Show();

            // Assert
            this.AssertNoErrorsDisplayed();
            this.mockView.Verify(view => view.ShowLayoutDesigner(), "View not made visible");
        }

        /// <summary>
        /// Test that <see cref="ILayoutDesignerPresenter.Show"/> causes the view to be shown with the Save button enabled if there is at least one layout.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void ShowLeavesSaveButtonEnabledIfThereIsAtLeastOneLayout()
        {
            // Arrange
            this.SetupTemplateWithLayouts("Layout1");

            // Act
            this.sut.Show();

            // Assert
            this.AssertNoErrorsDisplayed();
            this.AssertControlEnabled(LayoutDesignerControl.Save, this.saveButtonEnabled);
        }

        /// <summary>
        /// Test that <see cref="ILayoutDesignerPresenter.Show"/> causes the view to be shown with the Save button disabled if there are no layouts.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void ShowLeavesSaveButtonDisabledIfThereAreNoLayouts()
        {
            // Arrange
            this.SetupTemplateWithLayouts(new string[0]);

            // Act
            this.sut.Show();

            // Assert
            this.AssertNoErrorsDisplayed();
            this.AssertControlDisabled(LayoutDesignerControl.Save, this.saveButtonEnabled);
        }

        /// <summary>
        /// Test that <see cref="ILayoutDesignerPresenter.Show"/> causes the view to be shown with the Connect button enabled.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void ShowLeavesConnectButtonEnabled()
        {
            // Act
            this.sut.Show();

            // Assert
            this.AssertControlEnabled(LayoutDesignerControl.Connect, this.connectButtonEnabled);
        }

        /// <summary>
        /// Test that <see cref="ILayoutDesignerPresenter.Show"/> causes the view to be shown with the Add New button enabled.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void ShowLeavesAddNewButtonEnabled()
        {
            // Act
            this.sut.Show();

            // Assert
            this.AssertControlEnabled(LayoutDesignerControl.AddNew, this.addNewButtonEnabled);
        }

        /// <summary>
        /// Test that <see cref="ILayoutDesignerPresenter.Show"/> causes the view to be shown with the Add Field control disabled.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void ShowLeavesAddFieldControlDisabled()
        {
            // Act
            this.sut.Show();

            // Assert
            this.AssertControlDisabled(LayoutDesignerControl.AddField, this.addFieldControlEnabled);
        }

        /// <summary>
        /// Test that <see cref="ILayoutDesignerPresenter.Hide"/> causes the view to be hidden.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void HideCausesViewToBeHidden()
        {
            // Act
            this.sut.Hide();

            // Assert
            this.mockView.Verify(view => view.HideLayoutDesigner(), "View not hidden");
        }

        /// <summary>
        /// Test that <see cref="ILayoutDesignerPresenter.Show"/> causes a new temporary team project document to be created.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void ShowCausesNewTemporaryTeamProjectDocumentToBeCreated()
        {
            // Act
            this.sut.Show();

            // Assert
            this.mockManager.Verify(mgr => mgr.Add(true), Times.Once(), "New temporary document not created");
        }

        /// <summary>
        /// Test that <see cref="ILayoutDesignerPresenter.Show"/> connects new temporary document to existing document's connection, if it is connected.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void ShowConnectsNewTemporaryTeamProjectDocumentToExistingDocumentConnectionIfConnected()
        {
            // Arrange
            this.mockInvokingProjectDocument.Setup(doc => doc.IsConnected).Returns(true);

            // Act
            this.sut.Show();

            // Assert
            this.mockDesignProjectDocument.VerifySet(doc => doc.TeamProject = this.mockProject.Object, Times.Once(), "Team project connection not set");
            this.mockDesignProjectDocument.Verify(doc => doc.SaveTeamProject(), Times.Once(), "The designer document was not saved.");
            this.mockView.Verify(view => view.SetFieldList(It.IsAny<IEnumerable<ITfsFieldDefinition>>()), Times.Once(), "Fields not added to the view");
            this.AssertDesignerDocumentIsClean();
        }

        /// <summary>
        /// Test that <see cref="ILayoutDesignerPresenter.Show"/> does not connect new temporary document if existing document is not connected.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void ShowDoesNotConnectNewTemporaryTeamProjectDocumentIfExistingDocumentNotConnected()
        {
            // Arrange
            this.mockInvokingProjectDocument.Setup(doc => doc.IsConnected).Returns(false);

            // Act
            this.sut.Show();

            // Assert
            this.mockDesignProjectDocument.VerifySet(doc => doc.TeamProject = this.mockProject.Object, Times.Never(), "Team project connection was set");
            this.mockDesignProjectDocument.Verify(doc => doc.SaveTeamProject(), Times.Never(), "The designer document was saved.");
        }

        /// <summary>
        /// Test that <see cref="ILayoutDesignerPresenter.Show"/> causes a new temporary team project document to be created only the first time the view is shown.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void ShowCausesNewTemporaryTeamProjectDocumentToBeCreatedOnlyFirstTimeViewIsShown()
        {
            // Arrange
            this.sut.Show();
            this.sut.Hide();

            // Act
            this.sut.Show();

            // Assert
            this.mockManager.Verify(mgr => mgr.Add(true), Times.Once(), "New temporary document not created");
        }

        /// <summary>
        /// Test that <see cref="ILayoutDesignerPresenter.Show"/> does connect new temporary document only the first time the view is shown.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void ShowCausesNewTemporaryTeamProjectDocumentToBeConnectedOnlyFirstTimeViewIsShown()
        {
            // Arrange
            this.mockInvokingProjectDocument.Setup(doc => doc.IsConnected).Returns(true);
            this.sut.Show();
            this.sut.Hide();

            // Act
            this.sut.Show();

            // Assert
            this.mockDesignProjectDocument.VerifySet(doc => doc.TeamProject = this.mockProject.Object, Times.Once(), "Team project connection was set more than once");
            this.mockDesignProjectDocument.Verify(doc => doc.SaveTeamProject(), Times.Once(), "The designer document was saved more than once.");
        }

        /// <summary>
        /// Test that <see cref="ILayoutDesignerPresenter.Show"/> enables Save button if was shown before and document is dirty.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void ShowEnablesSaveButtonIfPreviouslyDocumentWasDirty()
        {
            // Arrange
            this.SetupTemplateWithLayouts("Layout1");
            this.sut.Show();
            this.sut.Hide();
            this.mockDesignProjectDocument.Setup(doc => doc.HasChanged).Returns(true);

            // Act
            this.sut.Show();

            // Assert
            this.AssertControlEnabled(LayoutDesignerControl.Save, this.saveButtonEnabled);
        }

        /// <summary>
        /// Test that <see cref="ILayoutDesignerPresenter.Show"/> causes the first layout definition in the template to be displayed, respecting order in the displayed list.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void ShowCausesFirstLayoutDefinitionToBeDisplayedInDesignDocumentRespectingSortOrder()
        {
            // Arrange
            this.SetupTemplateWithLayouts("Layout2", "Layout1");

            // Act
            this.sut.Show();

            // Assert
            this.mockDesignProjectDocument.Verify(doc => doc.DisplayLayoutDefinition("Layout1"), Times.Once(), "Layouts not exported to the design document");
        }

        /// <summary>
        /// Test that <see cref="ILayoutDesignerPresenter.Show"/> causes the first layout definition to be selected in the view, respecting order in the displayed list..
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void ShowCausesFirstLayoutDefinitionToBeSelectedInTheViewRespectingSortOrder()
        {
            // Arrange
            this.SetupTemplateWithLayouts("Layout2", "Layout1");

            // Act
            this.sut.Show();

            // Assert
            Assert.AreEqual<string>("Layout1", this.selectedLayout, "First layout not selected on the view.");
        }

        /// <summary>
        /// Test that <see cref="ILayoutDesignerPresenter.Show"/> passes the list of layouts in the template to the view in alphabetical order.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void ShowPassesOrderedListOfLayoutsInTemplateToTheView()
        {
            // Arrange
            LayoutInformation[] layouts = this.SetupTemplateWithLayouts("ZLayout1", "ALayout2");

            // Act
            this.sut.Show();

            // Assert
            Assert.IsTrue(TestHelper.CheckOrderedArrayMatch(new LayoutInformation[] { layouts[1], layouts[0] }, this.currentViewLayoutList), "Layouts not passed in correct order.");
        }

        /// <summary>
        /// Test that <see cref="ILayoutDesignerPresenter.Show"/> displays error if an exception occurs.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void ShowDisplaysErrorIfExceptionOccurs()
        {
            // Arrange
            this.SetupTemplateWithLayouts("Layout1");
            InvalidOperationException ioe = new InvalidOperationException("test");
            this.mockView.Setup(view => view.ShowLayoutDesigner()).Throws(ioe);

            // Act
            this.sut.Show();

            // Assert

            // Assert
            this.mockView.Verify(view => view.DisplayMessage(UIMessageType.Error, "Microsoft Word", "test", TestHelper.MessageStartsWith("test")));
        }

        /// <summary>
        /// Test that <see cref="ILayoutDesignerView.Save"/> event causes the current layout in the document to be saved to the system template.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void SaveEventSavesCurrentLayoutToSystemTemplate()
        {
            // Arrange
            LayoutInformation[] layouts = this.SetupTemplateWithLayouts("Layout1", "Layout2");
            this.sut.Show();

            // Act
            this.mockView.Raise(v => v.Save += null, EventArgs.Empty);

            // Assert
            this.AssertLayoutWasSaved(layouts[0]);
        }

        /// <summary>
        /// Test that <see cref="ILayoutDesignerView.Save"/> event causes the template to be saved after the changes have been saved into the template.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void SaveEventSavesTheSystemTemplateAfterLayoutsSavedIntoTheTemplate()
        {
            // Arrange
            LayoutInformation[] layouts = this.SetupTemplateWithLayouts("Layout1", "Layout2");
            bool savedToTemplate = false;
            this.mockDesignProjectDocument.Setup(doc => doc.SaveLayoutDefinition(layouts[0].Name)).Callback(() => savedToTemplate = true);
            this.mockTemplate.Setup(t => t.Save()).Callback(() => Assert.IsTrue(savedToTemplate, "The definitions must be saved to the template first."));
            this.sut.Show();

            // Act
            this.mockView.Raise(v => v.Save += null, EventArgs.Empty);

            // Assert
            this.AssertTemplateWasSaved();
        }

        /// <summary>
        /// Test that <see cref="ILayoutDesignerView.Save"/> event causes the document to be marked clean.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void SaveEventCausesDocumentToBeMarkedClean()
        {
            // Arrange
            this.SetupTemplateWithLayouts("Layout1");
            this.sut.Show();
            this.SetupDesignerDocumentDirty();

            // Act
            this.mockView.Raise(v => v.Save += null, EventArgs.Empty);

            // Assert
            this.AssertDesignerDocumentIsClean();
        }

        /// <summary>
        /// Test that <see cref="ILayoutDesignerView.Save"/> event leaves the save button enabled.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void SaveEventLeavesSaveButtonEnabled()
        {
            // Arrange
            this.SetupTemplateWithLayouts("Layout1");
            this.sut.Show();

            // Act
            this.mockView.Raise(v => v.Save += null, EventArgs.Empty);

            // Assert
            this.AssertControlEnabled(LayoutDesignerControl.Save, this.saveButtonEnabled);
        }

        /// <summary>
        /// Test that exception while processing the <see cref="ILayoutDesignerView.Save"/> event displays error.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void SaveDisplaysErrorIfExceptionOccurs()
        {
            // Arrange
            this.SetupTemplateWithLayouts("Layout1", "Layout2");
            InvalidOperationException ioe = new InvalidOperationException("test");
            this.mockDesignProjectDocument.Setup(d => d.SaveLayoutDefinition(It.IsAny<string>())).Throws(ioe);
            this.sut.Show();

            // Act
            this.mockView.Raise(v => v.Save += null, EventArgs.Empty);

            // Assert
            this.mockView.Verify(view => view.DisplayMessage(UIMessageType.Error, "Microsoft Word", "test", TestHelper.MessageStartsWith("test")));
        }

        /// <summary>
        /// Test that <see cref="ILayoutDesignerView.Save"/> causes save to display status messages.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void SaveEventCausesSaveToDisplayStatusMessages()
        {
            // Arrange
            InvalidOperationException ioe = new InvalidOperationException("test");
            this.mockDesignProjectDocument.Setup(d => d.SaveLayoutDefinition(It.IsAny<string>())).Throws(ioe);
            LayoutInformation[] layouts = this.SetupTemplateWithLayouts("Layout1", "Layout2");
            this.sut.Show();
            List<Tuple<string, int>> statusCalls = new List<Tuple<string, int>>();
            this.mockView.Setup(v => v.SetStatus(It.IsAny<string>(), It.IsAny<int>())).Callback((string status, int timeout) => statusCalls.Add(new Tuple<string, int>(status, timeout)));

            // Act
            this.mockView.Raise(v => v.Save += null, EventArgs.Empty);

            // Assert
            Assert.AreEqual<int>(2, statusCalls.Count, "Did not display correct status messages");
            Assert.AreEqual<string>("Saving Layout1", statusCalls[0].Item1, "incorrect status message");
            Assert.AreEqual<int>(0, statusCalls[0].Item2, "incorrect status message timeout");
            Assert.AreEqual<string>(string.Empty, statusCalls[1].Item1, "incorrect status message");
            Assert.AreEqual<int>(0, statusCalls[1].Item2, "incorrect status message timeout");
        }

        /// <summary>
        /// Test that error processing <see cref="ILayoutDesignerView.Save"/> causes status message to be cleared.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void SaveEventErrorClearsStatusMessages()
        {
            // Arrange
            LayoutInformation[] layouts = this.SetupTemplateWithLayouts("Layout1", "Layout2");
            this.sut.Show();
            List<Tuple<string, int>> statusCalls = new List<Tuple<string, int>>();
            this.mockView.Setup(v => v.SetStatus(It.IsAny<string>(), It.IsAny<int>())).Callback((string status, int timeout) => statusCalls.Add(new Tuple<string, int>(status, timeout)));

            // Act
            this.mockView.Raise(v => v.Save += null, EventArgs.Empty);

            // Assert
            Assert.AreEqual<int>(2, statusCalls.Count, "Did not display correct status messages");
            Assert.AreEqual<string>("Saving Layout1", statusCalls[0].Item1, "incorrect status message");
            Assert.AreEqual<int>(0, statusCalls[0].Item2, "incorrect status message timeout");
            Assert.AreEqual<string>("Saved Layout1", statusCalls[1].Item1, "incorrect status message");
            Assert.AreEqual<int>(3, statusCalls[1].Item2, "incorrect status message timeout");
        }

        /// <summary>
        /// Test that <see cref="ILayoutDesignerView.LayoutSelected"/> event causes the chosen layout to be displayed in the design document if no change to design document.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void LayoutSelectedEventDisplaysChosenLayout()
        {
            // Arrange
            LayoutInformation layout = TestHelper.CreateTestLayoutInformation("Layout1");
            this.SetupDocumentNotChanged();
            this.sut.Show();

            // Act
            this.mockView.Raise(v => v.LayoutSelected += null, new LayoutItemEventArgs(layout));

            // Assert
            this.mockDesignProjectDocument.Verify(doc => doc.DisplayLayoutDefinition(layout.Name), Times.Once(), "Layout not exported to the design document");
        }

        /// <summary>
        /// Test that after <see cref="ILayoutDesignerView.LayoutSelected"/> causes the chosen layout to be displayed that the Save button is still enabled.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void LayoutSelectedEventLeavesSaveButtonEnabled()
        {
            // Arrange
            LayoutInformation[] layouts = this.SetupTemplateWithLayouts("Layout1");
            this.SetupDocumentNotChanged();
            this.sut.Show();

            // Act
            this.mockView.Raise(v => v.LayoutSelected += null, new LayoutItemEventArgs(layouts[0]));

            // Assert
            this.AssertControlEnabled(LayoutDesignerControl.Save, this.saveButtonEnabled);
        }

        /// <summary>
        /// Test that <see cref="ILayoutDesignerView.LayoutSelected"/> marks document clean after displaying the new layout definition.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void LayoutSelectedEventMarksDocumentCleanAfterDisplayingLayout()
        {
            // Arrange
            bool displayed = false;
            this.mockDesignProjectDocument.Setup(doc => doc.DisplayLayoutDefinition(It.IsAny<string>())).Callback(() => displayed = true);
            this.mockDesignProjectDocument.Setup(doc => doc.MarkDocumentClean()).Callback(() => Assert.IsTrue(displayed, "Marked document clean before displaying layout."));
            LayoutInformation layout = TestHelper.CreateTestLayoutInformation("Layout1");
            this.SetupDocumentNotChanged();
            this.sut.Show();

            // Act
            this.mockView.Raise(v => v.LayoutSelected += null, new LayoutItemEventArgs(layout));

            // Assert
            this.mockDesignProjectDocument.Verify(doc => doc.MarkDocumentClean(), Times.Once(), "Should mark document clean");
        }

        /// <summary>
        /// Test that exception while processing the <see cref="ILayoutDesignerView.LayoutSelected"/> event displays error.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void LayoutSelectedEventDisplaysErrorIfExceptionOccurs()
        {
            // Arrange
            InvalidOperationException ioe = new InvalidOperationException("test");
            LayoutInformation layout = TestHelper.CreateTestLayoutInformation("Layout1");
            this.SetupDocumentNotChanged();
            this.mockDesignProjectDocument.Setup(d => d.DisplayLayoutDefinition(It.IsAny<string>())).Throws(ioe);
            this.sut.Show();

            // Act
            this.mockView.Raise(v => v.LayoutSelected += null, new LayoutItemEventArgs(layout));

            // Assert
            this.mockView.Verify(view => view.DisplayMessage(UIMessageType.Error, "Microsoft Word", "test", TestHelper.MessageStartsWith("test")));
        }

        /// <summary>
        /// Test that <see cref="ILayoutDesignerView.LayoutSelected"/> event causes prompt if document changed.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void LayoutSelectedEventCausesPromptIfDocumentChanged()
        {
            // Arrange
            LayoutInformation[] layouts = this.SetupTemplateWithLayouts("Layout1", "Layout2");
            this.SetupDocumentChanged();
            this.sut.Show();

            // Act
            this.mockView.Raise(v => v.LayoutSelected += null, new LayoutItemEventArgs(layouts[1]));

            // Assert
            this.AssertPromptToSaveChangesIsDisplayed("Layout1");
        }

        /// <summary>
        /// Test that <see cref="ILayoutDesignerView.LayoutSelected"/> event causes changes to be discarded if user does not want to keep them.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void LayoutSelectedEventDiscardsChangesIfUserDoesNotWantToKeepThem()
        {
            // Arrange
            LayoutInformation layout = TestHelper.CreateTestLayoutInformation("Layout1");
            this.SetupDocumentChanged();
            this.sut.Show();
            this.SetupMessageBoxViewResponse(MessageBoxResult.No);

            // Act
            this.mockView.Raise(v => v.LayoutSelected += null, new LayoutItemEventArgs(layout));

            // Assert
            this.AssertNoLayoutsSaved();
            this.mockDesignProjectDocument.Verify(doc => doc.DisplayLayoutDefinition(layout.Name), Times.Once(), "Layout not exported to the design document");
        }

        /// <summary>
        /// Test that <see cref="ILayoutDesignerView.LayoutSelected"/> event causes changes to be saved if user wants to keep them.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void LayoutSelectedEventSavesChangesIfUserWantsToKeepThem()
        {
            // Arrange
            LayoutInformation[] layouts = this.SetupTemplateWithLayouts("Layout1", "Layout2");
            bool saved = false;
            this.mockDesignProjectDocument.Setup(doc => doc.SaveLayoutDefinition(layouts[0].Name)).Callback(() => saved = true);
            this.mockDesignProjectDocument.Setup(doc => doc.DisplayLayoutDefinition(layouts[1].Name)).Callback(() => Assert.IsTrue(saved, "Layout not saved before displaying new one"));
            this.SetupDocumentChanged();
            this.sut.Show();
            this.SetupMessageBoxViewResponse(MessageBoxResult.Yes);

            // Act
            this.mockView.Raise(v => v.LayoutSelected += null, new LayoutItemEventArgs(layouts[1]));

            // Assert
            this.AssertLayoutWasSaved(layouts[0]);
            this.mockDesignProjectDocument.Verify(doc => doc.DisplayLayoutDefinition(layouts[1].Name), Times.Once(), "Layout not exported to the design document");
        }

        /// <summary>
        /// Test that <see cref="ILayoutDesignerView.LayoutSelected"/> event causes layout not be changed if user cancels at prompt.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void LayoutSelectedEventCancelsLayoutChangeIfUserCancelsAtPrompt()
        {
            // Arrange
            LayoutInformation[] layouts = this.SetupTemplateWithLayouts("Layout1", "Layout2");
            this.SetupDocumentChanged();
            this.sut.Show();
            this.SetupMessageBoxViewResponse(MessageBoxResult.Cancel);
            LayoutItemEventArgs eventArgs = new LayoutItemEventArgs(layouts[1]);

            // Act
            this.mockView.Raise(v => v.LayoutSelected += null, eventArgs);

            // Assert
            this.AssertLayoutNotSaved(layouts[0]);
            this.mockDesignProjectDocument.Verify(doc => doc.DisplayLayoutDefinition(layouts[1].Name), Times.Never(), "Layout should not have been changed");
            Assert.IsTrue(eventArgs.Cancel, "Selection was not cancelled");
        }

        /// <summary>
        /// Test that a Connect event from the view raises the <see cref="ILayoutDesignerPresenter.Connect"/> event.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void ConnectEventFromViewRaisesConnectEvent()
        {
            // Arrange
            bool eventRaised = false;
            this.sut.Show();
            this.sut.Connect += (s, e) => eventRaised = true;

            // Act
            this.mockView.Raise(v => v.Connect += null, EventArgs.Empty);

            // Assert
            Assert.IsTrue(eventRaised, "Connect event not raised");
        }

        /// <summary>
        /// Test that exception while processing the <see cref="ILayoutDesignerView.Connect"/> event displays error.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void ConnectEventDisplaysErrorIfExceptionOccurs()
        {
            // Arrange
            InvalidOperationException ioe = new InvalidOperationException("test");
            LayoutInformation layout = TestHelper.CreateTestLayoutInformation("Layout1");
            this.SetupDocumentNotChanged();
            this.mockDesignProjectDocument.Setup(doc => doc.HasChanged).Throws(ioe);
            this.sut.Show();

            // Act
            this.mockView.Raise(v => v.Connect += null, EventArgs.Empty);

            // Assert
            this.mockView.Verify(view => view.DisplayMessage(UIMessageType.Error, "Microsoft Word", "test", TestHelper.MessageStartsWith("test")));
        }

        /// <summary>
        /// Tests that Connect remains enabled and editor controls remain disabled if the connect is cancelled.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void ConnectButtonRemainsEnabledAndEditorControlsRemainDisabledIfConnectIsCancelled()
        {
            // Arrange
            this.sut.Show();
            this.mockDesignProjectDocument.Setup(doc => doc.IsConnected).Returns(false);

            // Act
            this.mockView.Raise(v => v.Connect += null, EventArgs.Empty);

            // Assert
            this.AssertControlEnabled(LayoutDesignerControl.Connect, this.connectButtonEnabled);
            this.AssertControlDisabled(LayoutDesignerControl.AddField, this.addFieldControlEnabled);
        }

        /// <summary>
        /// Tests that Connect is disabled and editor controls become enabled if the connect is completed successfully.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void ConnectButtonIsDisabledAndEditorControlsAreEnabledIfConnectIsCompletedSuccessfully()
        {
            // Arrange
            this.sut.Show();
            this.mockDesignProjectDocument.Setup(doc => doc.IsConnected).Returns(true);

            // Act
            this.mockView.Raise(v => v.Connect += null, EventArgs.Empty);

            // Assert
            this.AssertControlDisabled(LayoutDesignerControl.Connect, this.connectButtonEnabled);
            this.AssertControlEnabled(LayoutDesignerControl.AddField, this.addFieldControlEnabled);
        }

        /// <summary>
        /// Tests that designer document is set to be clean after a successful connect if it was clean before.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void DesignerDocumentIsSetToCleanAfterSuccessfulConnectIfWasCleanBefore()
        {
            // Arrange
            this.sut.Show();
            this.mockDesignProjectDocument.Setup(doc => doc.IsConnected).Returns(true);
            this.SetupDocumentNotChanged();

            // Act
            this.mockView.Raise(v => v.Connect += null, EventArgs.Empty);

            // Assert
            this.AssertDesignerDocumentIsClean();
        }

        /// <summary>
        /// Tests that designer document is not set to be clean after a successful connect if it was not clean before.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void DesignerDocumentIsNotSetToCleanAfterSuccessfulConnectIfWasNotCleanBefore()
        {
            // Arrange
            this.sut.Show();
            this.mockDesignProjectDocument.Setup(doc => doc.IsConnected).Returns(true);
            this.SetupDocumentChanged();

            // Act
            this.mockView.Raise(v => v.Connect += null, EventArgs.Empty);

            // Assert
            this.AssertDesignerDocumentIsDirty();
        }

        /// <summary>
        /// Tests field list in friendly name order is passed to the view on successful connection.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void FieldListInFriendlyNameOrderPassedToViewIfConnectIsCompletedSuccessfully()
        {
            // Arrange
            ITfsFieldDefinition field1 = TestHelper.CreateMockFieldDefinition("A", "Z");
            ITfsFieldDefinition field2 = TestHelper.CreateMockFieldDefinition("Z", "A");
            this.mockProject.Setup(tp => tp.FieldDefinitions).Returns(new ITfsFieldDefinition[] { field1, field2 });
            this.sut.Show();
            this.mockDesignProjectDocument.Setup(doc => doc.IsConnected).Returns(true);

            // Act
            this.mockView.Raise(v => v.Connect += null, EventArgs.Empty);

            // Assert
            this.mockView.Verify(view => view.SetFieldList(TestHelper.OrderedArray(field2, field1)), Times.Once(), "Fields not passed in friendly name order");
        }

        /// <summary>
        /// Tests that the <see cref="ILayoutDesignerView.AddField"/> event causes the field to be added to the designer document.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void AddFieldEventAddsFieldToDesignerDocument()
        {
            // Arrange
            ITfsFieldDefinition field = TestHelper.CreateMockFieldDefinition("A", "Z");
            this.sut.Show();

            // Act
            this.mockView.Raise(v => v.AddField += null, new FieldDefinitionEventArgs(field));

            // Assert
            this.mockDesignProjectDocument.Verify(doc => doc.AddField(field), Times.Once(), "Field should be added to the document");
        }

        /// <summary>
        /// Test that exception while processing the <see cref="ILayoutDesignerView.AddField"/> event displays error.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void AddFieldEventDisplaysErrorIfExceptionOccurs()
        {
            // Arrange
            InvalidOperationException ioe = new InvalidOperationException("test");
            LayoutInformation layout = TestHelper.CreateTestLayoutInformation("Layout1");
            this.SetupDocumentNotChanged();
            this.mockDesignProjectDocument.Setup(doc => doc.AddField(It.IsAny<ITfsFieldDefinition>())).Throws(ioe);
            this.sut.Show();

            // Act
            this.mockView.Raise(v => v.AddField += null, new FieldDefinitionEventArgs(null));

            // Assert
            this.mockView.Verify(view => view.DisplayMessage(UIMessageType.Error, "Microsoft Word", "test", TestHelper.MessageStartsWith("test")));
        }

        /// <summary>
        /// Test that <see cref="ITeamProjectDocumentManager.DocumentBeforeClose"/> event causes prompt if designer document changed.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void ClosingEventCausesPromptIfDocumentChanged()
        {
            // Arrange
            this.SetupTemplateWithLayouts("Layout1");
            this.SetupDocumentChanged();
            this.sut.Show();

            // Act
            this.mockManager.Raise(mgr => mgr.DocumentBeforeClose += null, new CancelEventArgs());

            // Assert
            this.AssertPromptToSaveChangesIsDisplayed("Layout1");
        }

        /// <summary>
        /// Test that <see cref="ITeamProjectDocumentManager.DocumentBeforeClose"/> event causes prompt if there is a pending rename.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void ClosingEventCausesPromptIfThereIsAPendingRename()
        {
            // Arrange
            this.SetupTemplateWithLayouts("Layout1");
            this.sut.Show();
            this.mockView.Raise(view => view.LayoutRename += null, new RenameEventArgs("Layout1", "Layout2"));

            // Act
            this.mockManager.Raise(mgr => mgr.DocumentBeforeClose += null, new CancelEventArgs());

            // Assert
            this.AssertPromptToSaveChangesIsDisplayed("Layout2");
        }

        /// <summary>
        /// Test that <see cref="ITeamProjectDocumentManager.DocumentBeforeClose"/> event causes prompt if a new layout has been added.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void ClosingEventCausesPromptIfANewLayoutHasBeenAdded()
        {
            // Arrange
            this.SetupTemplateWithLayouts("Layout1");
            this.sut.Show();
            this.mockView.Raise(view => view.AddNewLayout += null, EventArgs.Empty);
            this.SetupDocumentChanged();

            // Act
            this.mockManager.Raise(mgr => mgr.DocumentBeforeClose += null, new CancelEventArgs());

            // Assert
            this.AssertPromptToSaveChangesIsDisplayed(Constants.PrototypeLayoutName);
        }

        /// <summary>
        /// Test that <see cref="ITeamProjectDocumentManager.DocumentBeforeClosing"/> event causes document to be marked clean if user elects not to save changes.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void ClosingEventCausesDocumentToBeMarkedCleanIfUserElectsNotToSaveChanges()
        {
            // Arrange
            this.SetupTemplateWithLayouts("Layout1");
            this.SetupDocumentChanged();
            this.sut.Show();
            this.SetupMessageBoxViewResponse(MessageBoxResult.No);
            this.SetupDesignerDocumentDirty();

            // Act
            this.mockManager.Raise(mgr => mgr.DocumentBeforeClose += null, new CancelEventArgs());

            // Assert
            this.AssertDesignerDocumentIsClean();
        }

        /// <summary>
        /// Test that <see cref="ITeamProjectDocumentManager.DocumentBeforeClosing"/> event causes document to be marked clean if user elects to save changes.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void ClosingEventCausesDocumentToBeMarkedCleanIfUserElectsToSaveChanges()
        {
            // Arrange
            this.SetupTemplateWithLayouts("Layout1");
            this.SetupDocumentChanged();
            this.sut.Show();
            this.SetupMessageBoxViewResponse(MessageBoxResult.Yes);
            this.SetupDesignerDocumentDirty();

            // Act
            this.mockManager.Raise(mgr => mgr.DocumentBeforeClose += null, new CancelEventArgs());

            // Assert
            this.AssertDesignerDocumentIsClean();
        }

        /// <summary>
        /// Test that <see cref="ITeamProjectDocumentManager.DocumentBeforeClosing"/> event does not cause prompt when a different document is closed, even if designer document changed.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void ClosingEventForADifferentDocumentDoesNotCausesPromptIfDocumentChanged()
        {
            // Arrange
            this.SetupTemplateWithLayouts("Layout1");
            this.SetupDocumentChanged();
            this.sut.Show();
            this.mockManager.Setup(mgr => mgr.ActiveDocument).Returns(new Mock<ITeamProjectDocument>().Object);

            // Act
            this.mockManager.Raise(mgr => mgr.DocumentBeforeClose += null, new CancelEventArgs());

            // Assert
            this.mockMessageBoxView.Verify(v => v.Show(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<MessageBoxViewButtons>(), It.IsAny<MessageBoxViewIcon>()), Times.Never(), "Prompt should not be displayed");
        }

        /// <summary>
        /// Test that <see cref="ILayoutDesignerView.AddNewLayout"/> event prompts to save previous changes before adding new layout.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void AddNewLayoutEventPromptsToSavePreviousChanges()
        {
            // Arrange
            this.SetupTemplateWithLayouts("Layout1");
            this.SetupDocumentChanged();
            this.sut.Show();

            // Act
            this.mockView.Raise(view => view.AddNewLayout += null, EventArgs.Empty);

            // Assert
            this.AssertPromptToSaveChangesIsDisplayed("Layout1");
        }

        /// <summary>
        /// Test that <see cref="ILayoutDesignerView.AddNewLayout"/> event adds a prototype layout to the document but does not add it to the template.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void AddNewLayoutEventAddsPrototypeLayoutToDocumentButDoesNotSaveItToTheTemplate()
        {
            // Arrange
            this.sut.Show();

            // Act
            this.mockView.Raise(view => view.AddNewLayout += null, EventArgs.Empty);

            // Assert
            this.mockDesignProjectDocument.Verify(doc => doc.AddPrototypeLayoutDefinition(), Times.Once(), "Prototype layout not added to the document");
            this.mockDesignProjectDocument.Verify(doc => doc.SaveLayoutDefinition(Constants.PrototypeLayoutName), Times.Never(), "Prototype layout was saved to the template");
        }

        /// <summary>
        /// Test that <see cref="ILayoutDesignerView.AddNewLayout"/> event causes the new layout list to be made the current one.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void AddNewLayoutEventMakesNewLayoutTheCurrentLayout()
        {
            // Arrange
            LayoutInformation[] layouts = this.SetupTemplateWithLayouts("Layout1");
            this.sut.Show();
 
            // Act
            this.mockView.Raise(view => view.AddNewLayout += null, EventArgs.Empty);

            // Assert
            string savedLayout = string.Empty;
            this.mockDesignProjectDocument.Setup(doc => doc.SaveLayoutDefinition(It.IsAny<string>())).Callback<string>(s => savedLayout = s);
            this.mockView.Raise(view => view.Save += null, EventArgs.Empty); // find out which one is current by saving it.
            Assert.AreEqual(Constants.PrototypeLayoutName, savedLayout, "The wrong layout was current");
        }

        /// <summary>
        /// Test that <see cref="ILayoutDesignerView.AddNewLayout"/> event causes new layout to be selected on the view.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void AddNewLayoutEventSelectsNewLayoutOnTheView()
        {
            // Arrange
            LayoutInformation[] layouts = this.SetupTemplateWithLayouts("Layout1");
            this.sut.Show();

            // Act
            this.mockView.Raise(view => view.AddNewLayout += null, EventArgs.Empty);

            // Assert
            Assert.AreEqual<string>(Constants.PrototypeLayoutName, this.selectedLayout, "New layout is not selected on the view");
        }

        /// <summary>
        /// Test that <see cref="ILayoutDesignerView.AddNewLayout"/> event causes new layout name to be edited.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void AddNewLayoutEventEditsNewLayoutNameOnTheView()
        {
            // Arrange
            LayoutInformation[] layouts = this.SetupTemplateWithLayouts("Layout1");
            this.sut.Show();

            // Act
            this.mockView.Raise(view => view.AddNewLayout += null, EventArgs.Empty);

            // Assert
            this.mockView.Verify(view => view.StartLayoutNameEdit(Constants.PrototypeLayoutName), Times.Once(), "Did not start editing new name");
        }

        /// <summary>
        /// Test that <see cref="ILayoutDesignerView.AddNewLayout"/> event causes new layout list to be passed to the view with a "New Layout" entry.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void AddNewLayoutEventAddsNewLayoutToView()
        {
            // Arrange
            this.SetupTemplateWithLayouts("Layout1");
            this.sut.Show();

            // Act
            this.mockView.Raise(view => view.AddNewLayout += null, EventArgs.Empty);

            // Assert
            this.AssertCurrentViewLayoutListNames("Layout1", Constants.PrototypeLayoutName);
        }

        /// <summary>
        /// Test that <see cref="ILayoutDesignerView.AddNewLayout"/> event causes the save button to be enabled if there were no layouts before.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void AddNewLayoutEventEnablesSaveButtonIfThereWereNoLayoutsBefore()
        {
            // Arrange
            this.SetupTemplateWithLayouts(new string[0]);
            this.sut.Show();

            // Act
            this.mockView.Raise(view => view.AddNewLayout += null, EventArgs.Empty);

            // Assert
            this.AssertControlEnabled(LayoutDesignerControl.Save, this.saveButtonEnabled);
        }

        /// <summary>
        /// Test that <see cref="ILayoutDesignerView.LayoutSelected"/> event causes unsaved new layout to be removed from the view.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void NewLayoutIsRemovedFromViewIfItIsNotSavedWhenSwitchingLayouts()
        {
            // Arrange
            LayoutInformation[] layouts = this.SetupTemplateWithLayouts("Layout1");
            this.SetupDocumentChanged();
            this.sut.Show();
            this.mockView.Raise(view => view.AddNewLayout += null, EventArgs.Empty);
            this.SetupMessageBoxViewResponse(MessageBoxResult.No);

            // Act
            this.mockView.Raise(view => view.LayoutSelected += null, new LayoutItemEventArgs(layouts[0]));

            // Assert
            this.AssertCurrentViewLayoutListNames("Layout1");
        }

        /// <summary>
        /// Test that <see cref="ILayoutDesignerView.Save"/> event causes new layout to become permanent.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void SavingNewLayoutCausesNewLayoutToBecomePermanent()
        {
            // Arrange
            LayoutInformation[] layouts = this.SetupTemplateWithLayouts("Layout1");
            this.sut.Show();
            this.mockView.Raise(view => view.AddNewLayout += null, EventArgs.Empty);
            this.SetupMessageBoxViewResponse(MessageBoxResult.No);
            this.mockDesignProjectDocument.Setup(doc => doc.SaveLayoutDefinition(Constants.PrototypeLayoutName)).Callback(() => layouts = this.SetupTemplateWithLayouts("Layout1", Constants.PrototypeLayoutName));

            // Act
            this.mockView.Raise(view => view.Save += null, EventArgs.Empty);

            // Assert
            this.mockView.Raise(view => view.LayoutSelected += null, new LayoutItemEventArgs(layouts[0]));
            this.AssertCurrentViewLayoutListNames("Layout1", Constants.PrototypeLayoutName);
        }

        /// <summary>
        /// Test that <see cref="ILayoutDesignerView.LayoutSelected"/> event causes prompt to save unsaved new layout.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void LayoutSelectedEventPromptsToSaveUnsavedNewLayout()
        {
            // Arrange
            LayoutInformation[] layouts = this.SetupTemplateWithLayouts("Layout1");
            this.sut.Show();
            this.mockView.Raise(view => view.AddNewLayout += null, EventArgs.Empty);
            this.SetupDocumentChanged(); // adding the layout changes the document (not the template though)

            // Act
            this.mockView.Raise(view => view.LayoutSelected += null, new LayoutItemEventArgs(layouts[0]));

            // Assert
            this.AssertPromptToSaveChangesIsDisplayed(Constants.PrototypeLayoutName);
        }

        /// <summary>
        /// Test that <see cref="ILayoutDesignerView.AddNewLayout"/> event creates a layout with a sequential number if the standard new layout name already exists.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void AddNewLayoutEventAddsNewLayoutWithSequentialNumberIfNewLayoutNameAlreadyExists()
        {
            // Arrange
            this.SetupTemplateWithLayouts(Constants.PrototypeLayoutName, Constants.PrototypeLayoutName + " 2", Constants.PrototypeLayoutName + " 3");
            this.sut.Show();

            // Act
            this.mockView.Raise(view => view.AddNewLayout += null, EventArgs.Empty);

            // Assert
            this.AssertCurrentViewLayoutListNames(Constants.PrototypeLayoutName, Constants.PrototypeLayoutName + " 2", Constants.PrototypeLayoutName + " 3", Constants.PrototypeLayoutName + " 4");
        }

        /// <summary>
        /// Test that <see cref="ILayoutDesignerView.AddNewLayout"/> event disables the Add New button.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void AddNewLayoutEventDisablesTheAddNewButton()
        {
            // Arrange
            this.SetupTemplateWithLayouts(Constants.PrototypeLayoutName, Constants.PrototypeLayoutName + " 2", Constants.PrototypeLayoutName + " 3");
            this.sut.Show();

            // Act
            this.mockView.Raise(view => view.AddNewLayout += null, EventArgs.Empty);

            // Assert
            this.AssertControlDisabled(LayoutDesignerControl.AddNew, this.addNewButtonEnabled);
        }

        /// <summary>
        /// Test that exception while processing the <see cref="ILayoutDesignerView.AddNewLayout"/> event displays error.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void AddNewLayoutEventDisplaysErrorIfExceptionOccurs()
        {
            // Arrange
            InvalidOperationException ioe = new InvalidOperationException("test");
            LayoutInformation layout = TestHelper.CreateTestLayoutInformation("Layout1");
            this.SetupDocumentNotChanged();
            this.mockDesignProjectDocument.Setup(doc => doc.AddPrototypeLayoutDefinition()).Throws(ioe);
            this.sut.Show();

            // Act
            this.mockView.Raise(v => v.AddNewLayout += null, EventArgs.Empty);

            // Assert
            this.mockView.Verify(view => view.DisplayMessage(UIMessageType.Error, "Microsoft Word", "test", TestHelper.MessageStartsWith("test")));
        }

        /// <summary>
        /// Test that <see cref="ILayoutDesignerView.LayoutSelected"/> event enables the Add New button when switching away from a new layout.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void LayoutSelectedEventEnablesTheAddNewButtonWhenSwitchingAwayFromANewLayout()
        {
            // Arrange
            LayoutInformation[] layouts = this.SetupTemplateWithLayouts("Layout1");
            this.sut.Show();
            this.mockView.Raise(view => view.AddNewLayout += null, EventArgs.Empty);
            this.SetupDocumentChanged(); // adding the layout changes the document (not the template though)
            this.SetupMessageBoxViewResponse(MessageBoxResult.No);

            // Act
            this.mockView.Raise(view => view.LayoutSelected += null, new LayoutItemEventArgs(layouts[0]));

            // Assert
            this.AssertControlEnabled(LayoutDesignerControl.AddNew, this.addNewButtonEnabled);
        }

        /// <summary>
        /// Test that <see cref="ILayoutDesignerView.Save"/> event enables the Add New button when saving a new layout.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void SaveEventEnablesTheAddNewButtonWhenSavingANewLayout()
        {
            // Arrange
            LayoutInformation[] layouts = this.SetupTemplateWithLayouts("Layout1");
            this.sut.Show();
            this.mockView.Raise(view => view.AddNewLayout += null, EventArgs.Empty);
            this.SetupDocumentChanged(); // adding the layout changes the document (not the template though)
            this.SetupMessageBoxViewResponse(MessageBoxResult.No);

            // Act
            this.mockView.Raise(view => view.Save += null, EventArgs.Empty);

            // Assert
            this.AssertControlEnabled(LayoutDesignerControl.AddNew, this.addNewButtonEnabled);
        }

        /// <summary>
        /// Test that <see cref="ILayoutDesignerView.LayoutRename"/> event cancels a rename to an existing name.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void LayoutRenameCancelsRenameIfNewNameAlreadyExists()
        {
            // Arrange
            LayoutInformation[] layouts = this.SetupTemplateWithLayouts("Layout1", "Layout2");
            this.sut.Show();

            // Act
            RenameEventArgs renameArgs = new RenameEventArgs("Layout2", "Layout1");
            this.mockView.Raise(view => view.LayoutRename += null, renameArgs);

            // Assert
            Assert.IsTrue(renameArgs.Cancel, "The rename was not cancelled");
            this.mockDesignProjectDocument.Verify(doc => doc.RenameLayoutDefinition(It.IsAny<string>(), It.IsAny<string>()), Times.Never(), "Layout should not be renamed.");
        }

        /// <summary>
        /// Test that <see cref="ILayoutDesignerView.LayoutRename"/> event does not cancel a rename to a new name.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void LayoutRenameDoesNotCancelRenameToNewName()
        {
            // Arrange
            LayoutInformation[] layouts = this.SetupTemplateWithLayouts("Layout1", "Layout2");
            this.sut.Show();

            // Act
            RenameEventArgs renameArgs = new RenameEventArgs("Layout2", "Layout3");
            this.mockView.Raise(view => view.LayoutRename += null, renameArgs);

            // Assert
            Assert.IsFalse(renameArgs.Cancel, "The rename was cancelled");
        }

        /// <summary>
        /// Test that <see cref="ILayoutDesignerView.LayoutRename"/> event pends the rename.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void LayoutRenamePendsTheRename()
        {
            // Arrange
            LayoutInformation[] layouts = this.SetupTemplateWithLayouts("Layout1");
            this.sut.Show();

            // Act
            this.mockView.Raise(view => view.LayoutRename += null, new RenameEventArgs("Layout1", "Layout2"));

            // Assert
            this.mockDesignProjectDocument.Verify(doc => doc.RenameLayoutDefinition("Layout1", "Layout2"), Times.Never(), "The layout was not pended.");
        }

        /// <summary>
        /// Test that pended rename performed on changing layout.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void LayoutRenamePerformsPendedRenameOnChangingLayout()
        {
            // Arrange
            LayoutInformation[] layouts = this.SetupTemplateWithLayouts("Layout1", "Layout2");
            this.sut.Show();
            this.SetupMessageBoxViewResponse(MessageBoxResult.Yes);

            // Act
            this.mockView.Raise(view => view.LayoutRename += null, new RenameEventArgs("Layout1", "Layout3"));
            this.mockView.Raise(view => view.LayoutSelected += null, new LayoutItemEventArgs(layouts[1]));

            // Assert
            this.mockDesignProjectDocument.Verify(doc => doc.RenameLayoutDefinition("Layout1", "Layout3"), Times.Once(), "The layout was not renamed.");
        }

        /// <summary>
        /// Test that pended rename is not done on changing layout if user chooses not to save.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void LayoutRenameDoesNotGetDoneIfUserChoosesNotToSave()
        {
            // Arrange
            LayoutInformation[] layouts = this.SetupTemplateWithLayouts("Layout1", "Layout2");
            this.sut.Show();
            this.SetupMessageBoxViewResponse(MessageBoxResult.No);

            // Act
            this.mockView.Raise(view => view.LayoutRename += null, new RenameEventArgs("Layout1", "Layout3"));
            this.mockView.Raise(view => view.LayoutSelected += null, new LayoutItemEventArgs(layouts[1]));

            // Assert
            this.mockDesignProjectDocument.Verify(doc => doc.RenameLayoutDefinition("Layout1", "Layout3"), Times.Never(), "The layout was renamed.");
        }

        /// <summary>
        /// Test that pended rename performed on saving layout.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void LayoutRenamePerformsPendedRenameOnSavingLayout()
        {
            // Arrange
            LayoutInformation[] layouts = this.SetupTemplateWithLayouts("Layout1", "Layout2");
            this.sut.Show();
            this.SetupMessageBoxViewResponse(MessageBoxResult.Yes);

            // Act
            this.mockView.Raise(view => view.LayoutRename += null, new RenameEventArgs("Layout1", "Layout3"));
            this.mockView.Raise(view => view.Save += null, EventArgs.Empty);

            // Assert
            this.mockDesignProjectDocument.Verify(doc => doc.RenameLayoutDefinition("Layout1", "Layout3"), Times.Once(), "The layout was not renamed.");
        }

        /// <summary>
        /// Test that pended rename performed on adding new layout.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void LayoutRenamePerformsPendedRenameOnAddingNewLayout()
        {
            // Arrange
            LayoutInformation[] layouts = this.SetupTemplateWithLayouts("Layout1", "Layout2");
            this.sut.Show();
            this.SetupMessageBoxViewResponse(MessageBoxResult.Yes);

            // Act
            this.mockView.Raise(view => view.LayoutRename += null, new RenameEventArgs("Layout1", "Layout3"));
            this.mockView.Raise(view => view.AddNewLayout += null, EventArgs.Empty);

            // Assert
            this.mockDesignProjectDocument.Verify(doc => doc.RenameLayoutDefinition("Layout1", "Layout3"), Times.Once(), "The layout was not renamed.");
        }

        /// <summary>
        /// Test that pended rename on adding new layout causes prompt to ask to save changes to new name.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void LayoutRenameOnAddingNewLayoutPromptsForChangesToNewName()
        {
            // Arrange
            LayoutInformation[] layouts = this.SetupTemplateWithLayouts("Layout1", "Layout2");
            this.sut.Show();
            this.SetupMessageBoxViewResponse(MessageBoxResult.Yes);

            // Act
            this.mockView.Raise(view => view.LayoutRename += null, new RenameEventArgs("Layout1", "Layout3"));
            this.mockView.Raise(view => view.AddNewLayout += null, EventArgs.Empty);

            // Assert
            this.AssertPromptToSaveChangesIsDisplayed("Layout3");
        }

        /// <summary>
        /// Test that multiple pended renames only rename once.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void MultiplePendedRenamesOnlyDoneOnce()
        {
            // Arrange
            LayoutInformation[] layouts = this.SetupTemplateWithLayouts("Layout1", "Layout2");
            this.sut.Show();
            this.SetupMessageBoxViewResponse(MessageBoxResult.Yes);

            // Act
            this.mockView.Raise(view => view.LayoutRename += null, new RenameEventArgs("Layout1", "Layout3"));
            this.mockView.Raise(view => view.LayoutRename += null, new RenameEventArgs("Layout3", "Layout4"));
            this.mockView.Raise(view => view.Save += null, EventArgs.Empty);

            // Assert
            this.mockDesignProjectDocument.Verify(doc => doc.RenameLayoutDefinition("Layout1", "Layout4"), Times.Once(), "The layout was not renamed.");
        }

        /// <summary>
        /// Test that pended rename is cleared after user saves the rename.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void PendedLayoutRenameClearedAfterUserSavesTheRename()
        {
            // Arrange
            LayoutInformation[] layouts = this.SetupTemplateWithLayouts("Layout1", "Layout2");
            this.sut.Show();
            this.mockDesignProjectDocument.Setup(d => d.RenameLayoutDefinition("Layout1", "Layout0")).Callback(() => this.SetupTemplateWithLayouts("Layout0", "Layout2"));
            this.SetupMessageBoxViewResponse(MessageBoxResult.Yes);

            // Act
            this.mockView.Raise(view => view.LayoutRename += null, new RenameEventArgs("Layout1", "Layout0"));
            this.mockView.Raise(view => view.LayoutSelected += null, new LayoutItemEventArgs(layouts[1]));
            this.mockView.Raise(view => view.LayoutSelected += null, new LayoutItemEventArgs(layouts[0]));

            // Assert
            // should display once only as a result of the first layout selection, and not for the second.
            this.mockMessageBoxView.Verify(v => v.Show(It.IsAny<string>(), "Microsoft Word", MessageBoxViewButtons.YesNoCancel, MessageBoxViewIcon.Warning), Times.Once(), "Prompt not displayed");
        }

        /// <summary>
        /// Test that pended rename is cleared after user elects not to save the rename.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void PendedLayoutRenameClearedAfterUserChoosesNotToSave()
        {
            // Arrange
            LayoutInformation[] layouts = this.SetupTemplateWithLayouts("Layout1", "Layout2");
            this.sut.Show();
            this.SetupMessageBoxViewResponse(MessageBoxResult.No);

            // Act
            this.mockView.Raise(view => view.LayoutRename += null, new RenameEventArgs("Layout1", "Layout3"));
            this.mockView.Raise(view => view.LayoutSelected += null, new LayoutItemEventArgs(layouts[1]));
            this.mockView.Raise(view => view.LayoutSelected += null, new LayoutItemEventArgs(layouts[0]));

            // Assert
            // should display once only as a result of the first layout selection, and not for the second.
            this.mockMessageBoxView.Verify(v => v.Show(It.IsAny<string>(), "Microsoft Word", MessageBoxViewButtons.YesNoCancel, MessageBoxViewIcon.Warning), Times.Once(), "Prompt not displayed");
        }

        /// <summary>
        /// Test that cancelling a pended rename puts the original name back in the view.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void CancellingPendedLayoutRenameRestoresOriginalNameInTheView()
        {
            // Arrange
            LayoutInformation[] layouts = this.SetupTemplateWithLayouts("Layout1", "Layout2");
            this.sut.Show();
            this.SetupMessageBoxViewResponse(MessageBoxResult.No);
            this.mockView.Raise(view => view.LayoutRename += null, new RenameEventArgs("Layout1", "Layout3"));
            this.currentViewLayoutList = new LayoutInformation[] { TestHelper.CreateTestLayoutInformation("Layout1"), TestHelper.CreateTestLayoutInformation("Layout3") }; // this won't really happen but simulates the actual state of the view so we can check that it gets changed back

            // Act
            this.mockView.Raise(view => view.LayoutSelected += null, new LayoutItemEventArgs(layouts[1]));

            // Assert
            this.AssertCurrentViewLayoutListNames("Layout1", "Layout2");
        }

        /// <summary>
        /// Test that adding a new layout, renaming it and then not saving it does not do a rename.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void AddingNewLayoutRenamingAndNotSavingDoesNotRename()
        {
            // Arrange
            LayoutInformation[] layouts = this.SetupTemplateWithLayouts("Layout1", "Layout2");
            this.sut.Show();
            this.SetupMessageBoxViewResponse(MessageBoxResult.No);

            // Act
            this.mockView.Raise(view => view.AddNewLayout += null, EventArgs.Empty);
            this.mockView.Raise(view => view.LayoutRename += null, new RenameEventArgs("Layout1", "Layout3"));
            this.mockView.Raise(view => view.LayoutSelected += null, new LayoutItemEventArgs(layouts[1]));

            // Assert
            this.mockDesignProjectDocument.Verify(doc => doc.RenameLayoutDefinition("Layout1", "Layout3"), Times.Never(), "The layout was renamed.");
        }

        /// <summary>
        /// Test that can select a renamed layout after the rename has been saved.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void RenamedLayoutCanBeSelectedAfterRenameHasBeenSaved()
        {
            // Arrange
            LayoutInformation[] layouts = this.SetupTemplateWithLayouts("Layout1", "Layout3");
            this.sut.Show();
            this.mockDesignProjectDocument.Setup(d => d.RenameLayoutDefinition("Layout1", "Layout2")).Callback(() => this.SetupTemplateWithLayouts("Layout2", "Layout3"));

            // Act
            this.mockView.Raise(view => view.LayoutRename += null, new RenameEventArgs("Layout1", "Layout2"));
            this.mockView.Raise(v => v.Save += null, EventArgs.Empty);
            LayoutInformation renamedLayout = TestHelper.CreateTestLayoutInformation("Layout2");
            this.mockView.Raise(v => v.LayoutSelected += null, new LayoutItemEventArgs(renamedLayout));

            // Assert
            this.AssertNoErrorsDisplayed();
            Assert.AreEqual<string>("Layout2", this.selectedLayout, "Renamed layout not selected");
        }

        /// <summary>
        /// Test that after saving with a pending rename the currently selected layout is still selected.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void SaveWithAPendingRenameLeavesCurrentLayoutSelected()
        {
            // Arrange
            LayoutInformation[] layouts = this.SetupTemplateWithLayouts("Layout1", "Layout2", "Layout4");
            this.sut.Show();
            this.mockView.Raise(v => v.LayoutSelected += null, new LayoutItemEventArgs(layouts[1]));
            this.mockDesignProjectDocument.Setup(d => d.RenameLayoutDefinition("Layout2", "Layout3")).Callback(() => this.SetupTemplateWithLayouts("Layout1", "Layout3", "Layout4"));

            // Act
            this.mockView.Raise(view => view.LayoutRename += null, new RenameEventArgs("Layout2", "Layout3"));
            this.mockView.Raise(v => v.Save += null, EventArgs.Empty);

            // Assert
            Assert.AreEqual<string>("Layout3", this.selectedLayout, "Renamed layout not selected");
        }

        /// <summary>
        /// Test that after saving a new layout with a pending rename the old name does not appear in the list of layouts anymore.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void SaveOfNewLayoutWithAPendingRenameRemovesOldNameFromListOfLayouts()
        {
            // Arrange
            LayoutInformation[] layouts = this.SetupTemplateWithLayouts("Layout1");
            this.sut.Show();
            this.mockView.Raise(view => view.AddNewLayout += null, EventArgs.Empty);
            LayoutInformation newLayout = this.currentViewLayoutList.Where(li => li.Name == Constants.PrototypeLayoutName).Single(); // capture layout created and sent to the view for the new layout
            this.mockView.Raise(v => v.LayoutSelected += null, new LayoutItemEventArgs(newLayout));
            this.mockView.Raise(view => view.LayoutRename += null, new RenameEventArgs(Constants.PrototypeLayoutName, "Layout2"));

            // Act
            this.mockView.Raise(v => v.Save += null, EventArgs.Empty);

            // Assert
            this.AssertCurrentViewLayoutListNames("Layout1", "Layout2");
        }

        /// <summary>
        /// Test that the <see cref="ITeamProjectDocument.Close"/> event is raised if the underlying document is closed.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void CloseEventRaisedWhenUnderlyingDocumentIsClosed()
        {
            // Arrange
            bool eventRaised = false;
            this.sut.Show();
            this.sut.Close += (s, e) => eventRaised = true;

            // Act
            this.mockDesignProjectDocument.Raise(doc => doc.Close += null, EventArgs.Empty);

            // Assert
            Assert.IsTrue(eventRaised, "Close event was not raised");
        }

        /// <summary>
        /// Test that the layout designer view is cleared from <see cref="ITeamRibbonPresenter.LayoutDesignerView"/> when the document is closed.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void LayoutDesignerViewClearedFromTeamRibbonPresenterWhenDocumentIsClosed()
        {
            // Arrange
            this.sut.Show();

            // Act
            this.mockDesignProjectDocument.Raise(doc => doc.Close += null, EventArgs.Empty);

            // Assert
            Assert.IsNull(this.mockRibbonPresenter.Object.LayoutDesignerView, "View not cleared from ribbon presenter");
        }

        /// <summary>
        /// Test that the layout designer view is closed when the document is closed.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void LayoutDesignerViewIsClosedWhenDocumentIsClosed()
        {
            // Arrange
            this.sut.Show();

            // Act
            this.mockDesignProjectDocument.Raise(doc => doc.Close += null, EventArgs.Empty);

            // Assert
            this.mockView.Verify(view => view.Close(), Times.Once(), "The view was not closed");
        }

        /// <summary>
        /// Test that <see cref="ILayoutDesignerView.LayoutDelete"/> event prompts to save previous changes before adding new layout.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void DeleteLayoutEventPromptsForConfirmation()
        {
            // Arrange
            LayoutInformation[] layouts = this.SetupTemplateWithLayouts("Layout1");
            this.sut.Show();

            // Act
            this.mockView.Raise(view => view.LayoutDelete += null, new LayoutItemEventArgs(layouts[0]));

            // Assert
            this.AssertPromptToConfirmDeletion("Layout1");
        }

        /// <summary>
        /// Test that <see cref="ILayoutDesignerView.LayoutDelete"/> event deletes layout from template if the deletion is confirmed.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void DeleteLayoutEventDeletesLayoutFromTemplateIfDeletionConfirmed()
        {
            // Arrange
            LayoutInformation[] layouts = this.SetupTemplateWithLayouts("Layout2", "Layout1");
            this.sut.Show();
            this.SetupMessageBoxViewResponse(MessageBoxResult.Yes);

            // Act
            this.mockView.Raise(view => view.LayoutDelete += null, new LayoutItemEventArgs(layouts[1]));

            // Assert
            this.mockTemplate.Verify(t => t.DeleteLayout("Layout1"), Times.Once(), "The layout was not deleted from the template");
        }

        /// <summary>
        /// Test that <see cref="ILayoutDesignerView.LayoutDelete"/> event does not delete layout from template if the deletion is not confirmed.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void DeleteLayoutEventDoesNotDeleteLayoutFromTemplateIfDeletionNotConfirmed()
        {
            // Arrange
            LayoutInformation[] layouts = this.SetupTemplateWithLayouts("Layout1", "Layout2");
            this.sut.Show();
            this.SetupMessageBoxViewResponse(MessageBoxResult.No);

            // Act
            this.mockView.Raise(view => view.LayoutDelete += null, new LayoutItemEventArgs(layouts[0]));

            // Assert
            this.mockTemplate.Verify(t => t.DeleteLayout("Layout1"), Times.Never(), "The layout was should not have been deleted from the template");
        }

        /// <summary>
        /// Test that <see cref="ILayoutDesignerView.LayoutDelete"/> event deletes layout from list if the deletion is confirmed.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void DeleteLayoutEventDeletesLayoutFromListIfDeletionConfirmed()
        {
            // Arrange
            LayoutInformation[] layouts = this.SetupTemplateWithLayouts("Layout3", "Layout2", "Layout1");
            this.sut.Show();
            this.SetupMessageBoxViewResponse(MessageBoxResult.Yes);
            this.SetupLayoutListAfterDeletionOfALayout(layouts[0], layouts[1]);

            // Act
            this.mockView.Raise(view => view.LayoutDelete += null, new LayoutItemEventArgs(layouts[2]));

            // Assert
            this.AssertCurrentViewLayoutListNames("Layout2", "Layout3");
        }

        /// <summary>
        /// Test that <see cref="ILayoutDesignerView.LayoutDelete"/> event leaves layout list unchanged if the deletion is not confirmed.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void DeleteLayoutEventLeavesLayoutListUnchangedIfDeletionNotConfirmed()
        {
            // Arrange
            LayoutInformation[] layouts = this.SetupTemplateWithLayouts("Layout2", "Layout1");
            this.sut.Show();
            this.SetupMessageBoxViewResponse(MessageBoxResult.No);
            this.SetupLayoutListAfterDeletionOfALayout(layouts[0]);

            // Act
            this.mockView.Raise(view => view.LayoutDelete += null, new LayoutItemEventArgs(layouts[1]));

            // Assert
            this.AssertCurrentViewLayoutListNames("Layout1", "Layout2");
        }

        /// <summary>
        /// Test that <see cref="ILayoutDesignerView.LayoutDelete"/> event selects next layout when deletion is confirmed, respecting sort order.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void DeleteLayoutEventSelectsAndDisplaysNextLayoutIfDeletionConfirmedRespectingSortOrder()
        {
            // Arrange
            LayoutInformation[] layouts = this.SetupTemplateWithLayouts("Layout4", "Layout3", "Layout2", "Layout1");
            this.sut.Show();
            this.SetupMessageBoxViewResponse(MessageBoxResult.Yes);
            this.SetupLayoutListAfterDeletionOfALayout(layouts[0], layouts[1], layouts[2]);

            // Act
            this.mockView.Raise(view => view.LayoutDelete += null, new LayoutItemEventArgs(layouts[3]));

            // Assert
            Assert.AreEqual<string>("Layout2", this.selectedLayout);
            this.mockDesignProjectDocument.Verify(doc => doc.DisplayLayoutDefinition("Layout2"), Times.Once(), "Newly selected layout not displayed.");
        }

        /// <summary>
        /// Test that <see cref="ILayoutDesignerView.LayoutDelete"/> event selects previous layout when deletion is confirmed of last layout, respecting sort order.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void DeleteLayoutEventSelectsPreviousLayoutIfDeletionConfirmedOfLastLayoutRespectingSortOrder()
        {
            // Arrange
            LayoutInformation[] layouts = this.SetupTemplateWithLayouts("Layout4", "Layout3", "Layout2", "Layout1");
            this.sut.Show();
            this.SetupMessageBoxViewResponse(MessageBoxResult.Yes);
            this.SetupLayoutListAfterDeletionOfALayout(layouts[1], layouts[2], layouts[3]);

            // Act
            this.mockView.Raise(view => view.LayoutDelete += null, new LayoutItemEventArgs(layouts[0]));

            // Assert
            Assert.AreEqual<string>("Layout3", this.selectedLayout);
            this.mockDesignProjectDocument.Verify(doc => doc.DisplayLayoutDefinition("Layout3"), Times.Once(), "Newly selected layout not displayed.");
        }

        /// <summary>
        /// Test that <see cref="ILayoutDesignerView.LayoutDelete"/> event disables save button if no more layouts left after deletion.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void DeleteLayoutEventDisablesSaveButtonIfNoMoreLayoutsLeftAfterDeletion()
        {
            // Arrange
            LayoutInformation[] layouts = this.SetupTemplateWithLayouts("Layout1");
            this.sut.Show();
            this.SetupMessageBoxViewResponse(MessageBoxResult.Yes);
            this.SetupLayoutListAfterDeletionOfALayout(new LayoutInformation[0]);

            // Act
            this.mockView.Raise(view => view.LayoutDelete += null, new LayoutItemEventArgs(layouts[0]));

            // Assert
            this.AssertControlDisabled(LayoutDesignerControl.Save, this.saveButtonEnabled);
        }

        /// <summary>
        /// Test that <see cref="ILayoutDesignerView.LayoutDelete"/> event deletes a new unsaved layout from list if the deletion is confirmed.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void DeleteLayoutEventDeletesNewUnsavedLayoutFromListIfDeletionConfirmed()
        {
            // Arrange
            LayoutInformation[] layouts = this.SetupTemplateWithLayouts("Layout1", "Layout2");
            this.sut.Show();
            this.mockView.Raise(view => view.AddNewLayout += null, EventArgs.Empty);
            this.SetupMessageBoxViewResponse(MessageBoxResult.Yes);
            this.SetupLayoutListAfterDeletionOfALayout(layouts[0], layouts[1]);

            // Act
            this.mockView.Raise(view => view.LayoutDelete += null, new LayoutItemEventArgs(TestHelper.CreateTestLayoutInformation(Constants.PrototypeLayoutName)));

            // Assert
            this.AssertCurrentViewLayoutListNames("Layout1", "Layout2");
        }

        /// <summary>
        /// Test that <see cref="ILayoutDesignerView.LayoutDelete"/> event causes the document to be marked clean.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void DeleteLayoutEventCausesDocumentToBeMarkedClean()
        {
            // Arrange
            LayoutInformation[] layouts = this.SetupTemplateWithLayouts("Layout1");
            this.sut.Show();
            this.SetupMessageBoxViewResponse(MessageBoxResult.Yes);
            this.SetupDesignerDocumentDirty();

            // Act
            this.mockView.Raise(view => view.LayoutDelete += null, new LayoutItemEventArgs(layouts[0]));

            // Assert
            this.AssertDesignerDocumentIsClean();
        }

        /// <summary>
        /// Test that <see cref="ILayoutDesignerView.LayoutDelete"/> event causes the template to be saved.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void DeleteLayoutEventCausesTemplateToBeSaved()
        {
            // Arrange
            LayoutInformation[] layouts = this.SetupTemplateWithLayouts("Layout1");
            this.sut.Show();
            this.SetupMessageBoxViewResponse(MessageBoxResult.Yes);
            this.SetupDesignerDocumentDirty();

            // Act
            this.mockView.Raise(view => view.LayoutDelete += null, new LayoutItemEventArgs(layouts[0]));

            // Assert
            this.AssertTemplateWasSaved();
        }

        /// <summary>
        /// Sets up the manager with a mock design document.
        /// </summary>
        private void SetupManagerForMockDesignDocument()
        {
            this.mockManager.Setup(mgr => mgr.Add(true))
                            .Returns(this.mockDesignProjectDocument.Object)
                            .Callback(() => this.mockManager.Setup(mgr => mgr.ActiveDocument).Returns(this.mockDesignProjectDocument.Object));
        }

        /// <summary>
        /// Sets up the manager with a mock template.
        /// </summary>
        private void SetupManagerForMockTemplate()
        {
            this.mockManager.Setup(mgr => mgr.SystemTemplate).Returns(this.mockTemplate.Object);
            this.mockManager.Setup(mgr => mgr.ActiveDocument).Returns(this.mockInvokingProjectDocument.Object);
        }

        /// <summary>
        /// Sets up the mock design document to be considered unchanged.
        /// </summary>
        private void SetupDocumentNotChanged()
        {
            this.mockDesignProjectDocument.Setup(doc => doc.HasChanged).Returns(false);
        }

        /// <summary>
        /// Sets up the mock design document to be considered unchanged.
        /// </summary>
        private void SetupDocumentChanged()
        {
            this.mockDesignProjectDocument.Setup(doc => doc.HasChanged).Returns(true);
        }

        /// <summary>
        /// Sets up the response from the user to the message box.
        /// </summary>
        /// <param name="response">The response from the user.</param>
        private void SetupMessageBoxViewResponse(MessageBoxResult response)
        {
            this.mockMessageBoxView.Setup(v => v.Show(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<MessageBoxViewButtons>(), It.IsAny<MessageBoxViewIcon>())).Returns(response);
        }

        /// <summary>
        /// Sets up the mock template with a list of mock layouts.
        /// </summary>
        /// <param name="layoutNames">The names of the layouts to create.</param>
        /// <returns>The array of layouts.</returns>
        private LayoutInformation[] SetupTemplateWithLayouts(params string[] layoutNames)
        {
            LayoutInformation[] layouts = layoutNames.Select(n => TestHelper.CreateTestLayoutInformation(n)).ToArray();
            this.mockTemplate.Setup(t => t.Layouts).Returns(layouts);
            return layouts;
        }

        /// <summary>
        /// Sets up to change the layout list after any deletion of a layout.
        /// </summary>
        /// <param name="layouts">The new layout list after deletion.</param>
        private void SetupLayoutListAfterDeletionOfALayout(params LayoutInformation[] layouts)
        {
            this.mockTemplate.Setup(t => t.DeleteLayout(It.IsAny<string>())).Callback(() => this.mockTemplate.Setup(t => t.Layouts).Returns(layouts));
        }

        /// <summary>
        /// Sets up the designer document status to be dirty.
        /// </summary>
        private void SetupDesignerDocumentDirty()
        {
            this.documentIsClean = false;
        }

        /// <summary>
        /// Asserts that the state of a control was set to enabled.
        /// </summary>
        /// <param name="control">The control being checked.</param>
        /// <param name="actual">The actual tracked value.</param>
        private void AssertControlEnabled(LayoutDesignerControl control, bool actual)
        {
            this.mockView.Verify(view => view.SetButtonState(control, It.IsAny<bool>()), Times.AtLeastOnce());
            Assert.IsTrue(actual, control.ToString() + " control should be enabled.");
        }

        /// <summary>
        /// Asserts that the state of a control was set to disabled.
        /// </summary>
        /// <param name="control">The control being checked.</param>
        /// <param name="actual">The actual tracked value.</param>
        private void AssertControlDisabled(LayoutDesignerControl control, bool actual)
        {
            this.mockView.Verify(view => view.SetButtonState(control, It.IsAny<bool>()), Times.AtLeastOnce());
            Assert.IsFalse(actual, control.ToString() + " control should be disabled.");
        }

        /// <summary>
        /// Asserts that a layout was not saved.
        /// </summary>
        /// <param name="layout">The layout to be checked.</param>
        private void AssertLayoutNotSaved(LayoutInformation layout)
        {
            this.mockDesignProjectDocument.Verify(doc => doc.SaveLayoutDefinition(layout.Name), Times.Never(), "Layout should not have been saved");
            this.mockTemplate.Verify(t => t.Save(), Times.Never(), "Layout should not have been committed to disk");
        }

        /// <summary>
        /// Asserts that no layouts were saved.
        /// </summary>
        private void AssertNoLayoutsSaved()
        {
            this.mockDesignProjectDocument.Verify(doc => doc.SaveLayoutDefinition(It.IsAny<string>()), Times.Never(), "No layouts should have been saved");
            this.mockTemplate.Verify(t => t.Save(), Times.Never(), "Layout should not have been committed to disk");
        }

        /// <summary>
        /// Asserts that a layout was saved.
        /// </summary>
        /// <param name="layout">The layout to be checked.</param>
        private void AssertLayoutWasSaved(LayoutInformation layout)
        {
            this.mockDesignProjectDocument.Verify(doc => doc.SaveLayoutDefinition(layout.Name), Times.Once(), "Layout was not saved");
            this.AssertTemplateWasSaved();
        }

        /// <summary>
        /// Asserts that the template was saved.
        /// </summary>
        private void AssertTemplateWasSaved()
        {
            this.mockTemplate.Verify(t => t.Save(), Times.Once(), "Template was not saved");
        }

        /// <summary>
        /// Asserts that the prompt to save changes is displayed.
        /// </summary>
        /// <param name="layoutName">The name of the layout that will be saved.</param>
        private void AssertPromptToSaveChangesIsDisplayed(string layoutName)
        {
            this.mockMessageBoxView.Verify(v => v.Show(string.Format(CultureInfo.InvariantCulture, "Do you want to save the changes you made to the '{0}' layout?", layoutName), "Microsoft Word", MessageBoxViewButtons.YesNoCancel, MessageBoxViewIcon.Warning), Times.Once(), "Prompt not displayed");
        }

        /// <summary>
        /// Asserts that the prompt to confirm deletion is displayed.
        /// </summary>
        /// <param name="layoutName">The name of the layout that will be deleted.</param>
        private void AssertPromptToConfirmDeletion(string layoutName)
        {
            this.mockMessageBoxView.Verify(v => v.Show(string.Format(CultureInfo.InvariantCulture, "Are you sure you want to delete the '{0}' layout?", layoutName), "Microsoft Word", MessageBoxViewButtons.YesNo, MessageBoxViewIcon.Warning), Times.Once(), "Prompt not displayed");
        }

        /// <summary>
        /// Asserts that the current list of layouts in the view is the expected list.
        /// </summary>
        /// <param name="expectedLayoutNames">The list of expected names, in order.</param>
        private void AssertCurrentViewLayoutListNames(params string[] expectedLayoutNames)
        {
            Assert.IsTrue(TestHelper.CheckOrderedArrayMatch(expectedLayoutNames, this.currentViewLayoutList.Select(li => li.Name)));
        }

        /// <summary>
        /// Asserts that no error was displayed.
        /// </summary>
        private void AssertNoErrorsDisplayed()
        {
            this.mockView.Verify(view => view.DisplayMessage(It.IsAny<UIMessageType>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never(), "No error should have been displayed");
        }

        /// <summary>
        /// Asserts that the designer document is clean.
        /// </summary>
        private void AssertDesignerDocumentIsClean()
        {
            Assert.IsTrue(this.documentIsClean, "The document should be marked clean.");
        }

        /// <summary>
        /// Asserts that the designer document is dirty.
        /// </summary>
        private void AssertDesignerDocumentIsDirty()
        {
            Assert.IsFalse(this.documentIsClean, "The document should be marked dirty.");
        }
    }
}
