//---------------------------------------------------------------------
// <copyright file="TeamRibbonPresenterTests.cs" company="Microsoft">
//    Copyright (c) Microsoft. All rights reserved.
//    This code is licensed under the MIT License.
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF 
//    ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
//    TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
//    PARTICULAR PURPOSE AND NONINFRINGEMENT.
// </copyright>
// <summary>The TeamRibbonPresenterTests type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.Word4Tfs.Library.Test.UnitTest.Presenter
{
    using System;
    using global::System.Collections.Generic;
    using global::System.Threading;
    using Microsoft.Office.Interop.Word;
    using Microsoft.Practices.Unity;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Word4Tfs.Library.Data;
    using Microsoft.Word4Tfs.Library.Model;
    using Microsoft.Word4Tfs.Library.Presenter;
    using Microsoft.Word4Tfs.Library.View;
    using Moq;

    /// <summary>
    /// Tests the <see cref="TeamRibbonPresenter"/>
    /// </summary>
    [TestClass]
    [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Unity container disposed in the test cleanup code.")]
    public class TeamRibbonPresenterTests
    {
        /// <summary>
        /// The Unity container to be used for Dependency Injection
        /// </summary>
        private IUnityContainer container;

        /// <summary>
        /// The mock ribbon used to test the presenter.
        /// </summary>
        private Mock<ITeamRibbonView> mockRibbon;

        /// <summary>
        /// The mock team project document used to test the presenter.
        /// </summary>
        private Mock<ITeamProjectDocument> mockDocument;

        /// <summary>
        /// The mock team project system template used to test the presenter.
        /// </summary>
        private Mock<ITeamProjectTemplate> mockSystemTemplate;

        /// <summary>
        /// The mock team project document manager used to test the presenter.
        /// </summary>
        private Mock<ITeamProjectDocumentManager> mockDocumentManager;

        /// <summary>
        /// The mock settings.
        /// </summary>
        private Mock<ISettings> mockSettings;

        /// <summary>
        /// The mock message box view.
        /// </summary>
        private Mock<IMessageBoxView> mockMessageBoxView;

        /// <summary>
        /// Indicates if the rebind callback has been called.
        /// </summary>
        private bool rebindCallbackCalled;

        /// <summary>
        /// The presenter to be tested.
        /// </summary>
        private ITeamRibbonPresenter sut;

        /// <summary>
        /// Initializes the test.
        /// </summary>
        [TestInitialize]
        public void InitializeTest()
        {
            this.rebindCallbackCalled = false;
            this.container = new UnityContainer();

            this.mockRibbon = TestHelper.CreateAndRegisterMock<ITeamRibbonView>(this.container);
            this.mockRibbon.SetupAllProperties();
            this.mockDocumentManager = TestHelper.CreateAndRegisterMock<ITeamProjectDocumentManager>(this.container);
            this.mockDocument = new Mock<ITeamProjectDocument>();
            this.mockSystemTemplate = new Mock<ITeamProjectTemplate>();
            this.mockMessageBoxView = TestHelper.CreateAndRegisterMock<IMessageBoxView>(this.container);
            this.mockSettings = TestHelper.CreateAndRegisterMock<ISettings>(this.container);

            this.mockSettings.Setup(s => s.ShowBookmarks).Returns(true);

            this.sut = this.container.Resolve<TeamRibbonPresenter>();
            this.sut.Initialise(() =>
                                    {
                                        this.rebindCallbackCalled = true;
                                        return null;
                                    });
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
        public void TeamRibbonPresenterConstructorThrowsExceptionIfNullViewIsPassed()
        {
            TestHelper.TestForArgumentNullException(() => this.sut = new TeamRibbonPresenter(this.container, null, this.mockDocumentManager.Object, this.mockSettings.Object), "ribbon");
        }

        /// <summary>
        /// Constructor throws <see cref="ArgumentNullException"/> if the model it is passed is null
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void TeamRibbonPresenterConstructorThrowsExceptionIfNullModelIsPassed()
        {
            Mock<ITeamRibbonView> view = new Mock<ITeamRibbonView>();
            TestHelper.TestForArgumentNullException(() => this.sut = new TeamRibbonPresenter(this.container, view.Object, null, this.mockSettings.Object), "model");
        }

        /// <summary>
        /// Constructor throws <see cref="ArgumentNullException"/> if the settings it is passed is null
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void TeamRibbonPresenterConstructorThrowsExceptionIfNullSettingsIsPassed()
        {
            Mock<ITeamRibbonView> view = new Mock<ITeamRibbonView>();
            TestHelper.TestForArgumentNullException(() => this.sut = new TeamRibbonPresenter(this.container, view.Object, this.mockDocumentManager.Object, null), "settings");
        }

        /// <summary>
        /// Tests that <see cref="TeamRibbonPresenter.LoadState"/> does not try to load a document if there is not active document.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void LoadStateDoesNotLoadDocumentIfNoActiveDocument()
        {
            // Act
            this.sut.LoadState();
        }

        /// <summary>
        /// Tests that the active document is loaded by <see cref="TeamRibbonPresenter.LoadState"/>.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void LoadStateLoadsDocument()
        {
            // Arrange
            this.SetActiveDocument();

            // Act
            this.sut.LoadState();

            // Assert
            this.mockDocument.Verify(document => document.Load(It.IsAny<Func<Uri>>()), Times.Once());
        }

        /// <summary>
        /// Tests that <see cref="TeamRibbonPresenter.LoadState"/> loads the model and then calls <see cref="TeamRibbonPresenter.UpdateState"/>.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void LoadStateLoadsTheModel()
        {
            // Arrange
            this.SetActiveDocument();
            int callSequence = 1;
            this.mockDocument.Setup(document => document.Load(It.IsAny<Func<Uri>>())).Returns(new string[0]).Callback(() => Assert.AreEqual<int>(1, callSequence++, "Load should be called first"));

            // Test of call to UpdateState is indirect.
            this.mockRibbon.Setup(r => r.SetButtonState(It.IsAny<Enum>(), It.IsAny<bool>())).Callback(() => Assert.IsTrue(callSequence > 1, "UpdateState should be called after Load"));

            // Act
            this.sut.LoadState();

            // Assert
            this.mockRibbon.Verify(r => r.SetButtonState(It.IsAny<Enum>(), It.IsAny<bool>()), Times.AtLeastOnce(), "LoadState did not result in any calls to update the state");
        }

        #region Import button tests

        /// <summary>
        /// Tests that the import button is disabled in the <see cref="TeamRibbonPresenter.UpdateState"/> call when there is no document at all and no system template.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void UpdateStateDisablesImportButtonWhenThereIsNoDocumentAndNoSystemTemplate()
        {
            // Arrange
            this.SetNoActiveDocument();
            this.SetNoSystemTemplate();

            // Act
            this.sut.UpdateState();

            // Assert
            this.mockRibbon.Verify(ribbon => ribbon.SetButtonState(TeamRibbonButton.Import, false), Times.Once(), "The Import button should be disabled when there is no active document and no system template.");
        }

        /// <summary>
        /// Tests that the import button is disabled in the <see cref="TeamRibbonPresenter.UpdateState"/> call when there is an active document but no system template.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void UpdateStateDisablesImportButtonWhenThereIsAnActiveDocumentButNoSystemTemplate()
        {
            // Arrange
            this.SetActiveDocument();
            this.SetNoSystemTemplate();

            // Act
            this.sut.UpdateState();

            // Assert
            this.mockRibbon.Verify(ribbon => ribbon.SetButtonState(TeamRibbonButton.Import, false), Times.Once(), "The Import button should be disabled when there is an active document but no system template.");
        }

        /// <summary>
        /// Tests that the import button is disabled in the <see cref="TeamRibbonPresenter.UpdateState"/> call when there is an active document but no importable system template.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void UpdateStateDisablesImportButtonWhenThereIsAnActiveDocumentButNoImportableSystemTemplate()
        {
            // Arrange
            this.SetActiveDocument();
            this.SetSystemTemplate();
            this.SetSystemTemplateNotImportable();

            // Act
            this.sut.UpdateState();

            // Assert
            this.mockRibbon.Verify(ribbon => ribbon.SetButtonState(TeamRibbonButton.Import, false), Times.Once(), "The Import button should be disabled when there is an active document but no importable system template.");
        }

        /// <summary>
        /// Tests that the import button is disabled in the <see cref="TeamRibbonPresenter.UpdateState"/> call when there is no document at all even if there is an importable system template.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void UpdateStateDisablesImportButtonWhenThereIsNoDocumentButAnImportableSystemTemplate()
        {
            // Arrange
            this.SetNoActiveDocument();
            this.SetSystemTemplate();
            this.SetSystemTemplateImportable();

            // Act
            this.sut.UpdateState();

            // Assert
            this.mockRibbon.Verify(ribbon => ribbon.SetButtonState(TeamRibbonButton.Import, false), Times.Once(), "The Import button should be disabled when there is no active document even with an importable system template.");
        }

        /// <summary>
        /// Tests that the import button is enabled in the <see cref="TeamRibbonPresenter.UpdateState"/> call when the template is importable and there is an active document.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void UpdateStateEnablesImportButtonWhenThereIsAnActiveDocumentAndSystemTemplateIsImportable()
        {
            // Arrange
            this.SetActiveDocument();
            this.SetSystemTemplate();
            this.SetTemplateImportable();

            // Act
            this.sut.UpdateState();

            // Assert
            this.mockRibbon.Verify(ribbon => ribbon.SetButtonState(TeamRibbonButton.Import, true), Times.Once(), "The Import button should be enabled when the active document is importable and there is an importable system template.");
        }

        /// <summary>
        /// Tests that the import button is disabled in the <see cref="TeamRibbonPresenter.UpdateState"/> call when the active document is temporary
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void UpdateStateDisablesImportButtonWhenActiveDocumentIsTemporary()
        {
            // Arrange
            this.SetActiveTemporaryDocument();
            this.SetSystemTemplate();
            this.SetTemplateImportable();

            // Act
            this.sut.UpdateState();

            // Assert
            this.mockRibbon.Verify(ribbon => ribbon.SetButtonState(TeamRibbonButton.Import, false), Times.Once(), "The Import button should not be enabled when the active document is temporary.");
        }

        /// <summary>
        /// The Import action on the view raises Import event.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void ViewImportEventRaisesPresenterImportEvent()
        {
            // Arrange
            bool eventRaised = false;
            this.sut.Import += (s, e) => eventRaised = true;

            // Act
            this.mockRibbon.Raise(ribbon => ribbon.Import += null, EventArgs.Empty); 

            // Assert
            Assert.IsTrue(eventRaised, "The presenter did not raise the import event when the view told it that an import has been requested");
        }

        /// <summary>
        /// The Import action reports any exceptions.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void ImportActionReportsAnyExceptions()
        {
            // Arrange
            this.sut.Import += (s, e) => { throw new InvalidOperationException("test"); };

            // Act
            this.mockRibbon.Raise(ribbon => ribbon.Import += null, EventArgs.Empty);

            // Assert
            this.mockRibbon.Verify(ribbon => ribbon.DisplayMessage(UIMessageType.Error, "Microsoft Word", "test", TestHelper.MessageStartsWith("test")));
        }

        #endregion

        #region Refresh button tests

        /// <summary>
        /// Tests that the refresh button is disabled in the <see cref="TeamRibbonPresenter.UpdateState"/> call when there is no document at all.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void UpdateStateDisablesRefreshButtonWhenThereIsNoDocument()
        {
            // Arrange
            this.SetNoActiveDocument();

            // Act
            this.sut.UpdateState();

            // Assert
            this.mockRibbon.Verify(ribbon => ribbon.SetButtonState(TeamRibbonButton.Refresh, false), Times.Once(), "The Refresh button should be disabled when there is no active document.");
        }

        /// <summary>
        /// Tests that the refresh button is enabled in the <see cref="TeamRibbonPresenter.UpdateState"/> call when the document is refreshable.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void UpdateStateEnablesRefreshButtonWhenDocumentIsRefreshable()
        {
            // Arrange
            this.SetActiveDocument();
            this.SetActiveDocumentRefreshable();

            // Act
            this.sut.UpdateState();

            // Assert
            this.mockRibbon.Verify(ribbon => ribbon.SetButtonState(TeamRibbonButton.Refresh, true), Times.Once(), "The Refresh button should be enabled when the active document is refreshable.");
        }

        /// <summary>
        /// Tests that the refresh button is disabled in the <see cref="TeamRibbonPresenter.UpdateState"/> call when the document is refreshable but temporary.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void UpdateStateDisablesRefreshButtonWhenDocumentIsRefreshableButTemporary()
        {
            // Arrange
            this.SetActiveTemporaryDocument();
            this.SetActiveDocumentRefreshable();

            // Act
            this.sut.UpdateState();

            // Assert
            this.mockRibbon.Verify(ribbon => ribbon.SetButtonState(TeamRibbonButton.Refresh, false), Times.Once(), "The Refresh button should be disabled when the active document is refreshable but temporary.");
        }

        /// <summary>
        /// Tests that the show designer button is unchecked when the designer is closed or hidden.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void UnchecksShowDesignerButtonWhenDesignerViewIsClosedOrHidden()
        {
            // Arrange
            Mock<ILayoutDesignerView> mockView = new Mock<ILayoutDesignerView>();
            this.sut.LayoutDesignerView = mockView.Object;

            // Act
            mockView.Raise(view => view.Hidden += null, EventArgs.Empty);

            // Assert
            this.mockRibbon.Verify(ribbon => ribbon.SetButtonCheckState(TeamRibbonButton.ShowDesigner, false), Times.Once(), "The Show Designer button should be unchecked when there is no temporary document.");
        }

        /// <summary>
        /// Tests that the <see cref="TeamRibbonPresenter.LayoutDesignerView"/> property can be set to null.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void LayoutDesignerViewCanBeSetToNull()
        {
            this.sut.LayoutDesignerView = null;
        }

        /// <summary>
        /// Tests that the refresh button is disabled in the <see cref="TeamRibbonPresenter.UpdateState"/> call when the document is not refreshable.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void UpdateStateDisablesRefreshButtonWhenDocumentIsNotRefreshable()
        {
            // Arrange
            this.SetActiveDocument();
            this.SetActiveDocumentNotRefreshable();

            // Act
            this.sut.UpdateState();

            // Assert
            this.mockRibbon.Verify(ribbon => ribbon.SetButtonState(TeamRibbonButton.Refresh, false), Times.Once(), "The Refresh button should be disabled when the active document is not refreshable.");
        }

        /// <summary>
        /// The Refresh action on the view raises Refresh event.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void ViewRefreshEventRaisesPresenterRefreshEvent()
        {
            // Arrange
            bool eventRaised = false;
            this.sut.Refresh += (s, e) => eventRaised = true;

            // Act
            this.mockRibbon.Raise(ribbon => ribbon.Refresh += null, EventArgs.Empty);

            // Assert
            Assert.IsTrue(eventRaised, "The presenter did not raise the refresh event when the view told it that a refresh has been requested");
        }

        /// <summary>
        /// The Refresh action reports any exceptions.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void RefreshActionReportsAnyExceptions()
        {
            // Arrange
            this.sut.Refresh += (s, e) => { throw new InvalidOperationException("test"); };

            // Act
            this.mockRibbon.Raise(ribbon => ribbon.Refresh += null, EventArgs.Empty);

            // Assert
            this.mockRibbon.Verify(ribbon => ribbon.DisplayMessage(UIMessageType.Error, "Microsoft Word", "test", TestHelper.MessageStartsWith("test")));
        }

        #endregion

        #region Layout Designer button tests

        /// <summary>
        /// Tests that the layout designer button is disabled in the <see cref="TeamRibbonPresenter.UpdateState"/> call if there is no active document and no system template.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void UpdateStateDisablesLayourDesignerButtonWhenThereIsNoDocumentAndNoSystemTemplate()
        {
            // Arrange
            this.SetNoActiveDocument();
            this.SetNoSystemTemplate();

            // Act
            this.sut.UpdateState();

            // Assert
            this.mockRibbon.Verify(ribbon => ribbon.SetButtonState(TeamRibbonButton.ShowDesigner, false), Times.Once(), "The Layout Designer button should be disabled when there is no active document and no system template.");
        }

        /// <summary>
        /// Tests that the layout designer button is disabled in the <see cref="TeamRibbonPresenter.UpdateState"/> call when there is an active document but no system template.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void UpdateStateDisablesLayoutDesignerButtonWhenThereIsAnActiveDocumentButNoSystemTemplate()
        {
            // Arrange
            this.SetActiveDocument();
            this.SetNoSystemTemplate();

            // Act
            this.sut.UpdateState();

            // Assert
            this.mockRibbon.Verify(ribbon => ribbon.SetButtonState(TeamRibbonButton.ShowDesigner, false), Times.Once(), "The Layout Designer button should be disabled when there is an active document but no system template.");
        }

        /// <summary>
        /// Tests that the layout designer button is enabled in the <see cref="TeamRibbonPresenter.UpdateState"/> call when there is an active document but no importable system template.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void UpdateStateEnablesLayoutDesignerButtonWhenThereIsAnActiveDocumentButNoImportableSystemTemplate()
        {
            // Arrange
            this.SetActiveDocument();
            this.SetSystemTemplate();
            this.SetSystemTemplateNotImportable();

            // Act
            this.sut.UpdateState();

            // Assert
            this.mockRibbon.Verify(ribbon => ribbon.SetButtonState(TeamRibbonButton.ShowDesigner, true), Times.Once(), "The Layout Designer button should be enabled when there is an active document but no importable system template.");
        }

        /// <summary>
        /// Tests that the layout designer button is enabled in the <see cref="TeamRibbonPresenter.UpdateState"/> call when there is no document at all and there is an importable system template.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void UpdateStateEnablesLayoutDesigerButtonWhenThereIsNoDocumentButAnImportableSystemTemplate()
        {
            // Arrange
            this.SetNoActiveDocument();
            this.SetSystemTemplate();
            this.SetSystemTemplateImportable();

            // Act
            this.sut.UpdateState();

            // Assert
            this.mockRibbon.Verify(ribbon => ribbon.SetButtonState(TeamRibbonButton.ShowDesigner, true), Times.Once(), "The Layout Designer button should be enabled when there is no active document and an importable system template.");
        }

        /// <summary>
        /// Tests that the layout designer button is enabled in the <see cref="TeamRibbonPresenter.UpdateState"/> call when the template is importable and there is an active document.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void UpdateStateEnablesLayoutDesignerButtonWhenThereIsAnActiveDocumentAndSystemTemplateIsImportable()
        {
            // Arrange
            this.SetActiveDocument();
            this.SetSystemTemplate();
            this.SetTemplateImportable();

            // Act
            this.sut.UpdateState();

            // Assert
            this.mockRibbon.Verify(ribbon => ribbon.SetButtonState(TeamRibbonButton.ShowDesigner, true), Times.Once(), "The Layout Designer button should be enabled when the active document is importable and there is an importable system template.");
        }
        
        /// <summary>
        /// The ShowLayoutDesigner action on the view raises ShowLayoutDesigner event.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void ViewShowLayoutDesignerEventRaisesPresenterShowLayoutDesignerEvent()
        {
            // Arrange
            bool eventRaised = false;
            this.sut.ShowLayoutDesigner += (s, e) => eventRaised = true;

            // Act
            this.mockRibbon.Raise(ribbon => ribbon.ShowLayoutDesigner += null, EventArgs.Empty);

            // Assert
            Assert.IsTrue(eventRaised, "The presenter did not raise the show layout designer event when the view told it that this has been requested");
        }

        /// <summary>
        /// The HideLayoutDesigner action on the view raises HideLayoutDesigner event.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void ViewHideLayoutDesignerEventRaisesPresenterHideLayoutDesignerEvent()
        {
            // Arrange
            bool eventRaised = false;
            this.sut.HideLayoutDesigner += (s, e) => eventRaised = true;

            // Act
            this.mockRibbon.Raise(ribbon => ribbon.HideLayoutDesigner += null, EventArgs.Empty);

            // Assert
            Assert.IsTrue(eventRaised, "The presenter did not raise the hide layout designer event when the view told it that this has been requested");
        }

        #endregion

        /// <summary>
        /// When the manager raises the <see cref="ITeamProjectDocument.DocumentChanged"/> event the presenter calls <see cref="TeamRibbonViewPresenter.LoadState"/>.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void LoadStateCalledWhenDocumentChanges()
        {
            // Arrange
            this.SetActiveDocument();
            this.mockDocument.Verify(document => document.Load(It.IsAny<Func<Uri>>()), Times.Never()); // Just to make sure the test is valid.

            // Act
            this.mockDocumentManager.Raise(manager => manager.DocumentChanged += null, EventArgs.Empty);

            // Assert
            // Test of call to LoadState is indirect.
            this.mockDocument.Verify(document => document.Load(It.IsAny<Func<Uri>>()), Times.Once());
        }

        /// <summary>
        /// A document change reports any exceptions occurring when handling the change.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void DocumentChangeReportsAnyExceptions()
        {
            // Arrange
            this.mockDocumentManager.Setup(mgr => mgr.ActiveDocument).Throws(new InvalidOperationException("test"));

            // Act
            this.mockDocumentManager.Raise(mgr => mgr.DocumentChanged += null, EventArgs.Empty);

            // Assert
            this.mockRibbon.Verify(ribbon => ribbon.DisplayMessage(UIMessageType.Error, "Microsoft Word", "test", TestHelper.MessageStartsWith("test")));
        }

        /// <summary>
        /// A document change reports any warnings occurring when loading the document.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void DocumentChangeReportsAnyWarningsFromDocumentLoad()
        {
            // Arrange
            this.mockDocument.Setup(doc => doc.Load(It.IsAny<Func<Uri>>())).Returns(new string[] { "warning" });
            this.SetActiveDocument();

            // Act
            this.mockDocumentManager.Raise(mgr => mgr.DocumentChanged += null, EventArgs.Empty);

            // Assert
            this.mockRibbon.Verify(ribbon => ribbon.DisplayMessage(UIMessageType.Warning, "Microsoft Word", "There were some problems loading the document", TestHelper.MessageStartsWith("warning")));
        }

        /// <summary>
        /// Tests that <see cref="TeamRibbonPresenter.DisplayError"/> calls <see cref="ITeamRibbonView.DisplayError"/>.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void DisplayErrorCallsDisplayErrorOnTheView()
        {
            // Act
            this.sut.DisplayError("testmessage", "details");

            // Assert
            this.mockRibbon.Verify(ribbon => ribbon.DisplayMessage(UIMessageType.Error, "Microsoft Word", TestHelper.MessageStartsWith("testmessage"), "details"), Times.Once(), "The message box was not displayed.");
        }

        /// <summary>
        /// Tests that <see cref="TeamRibbonPresenter.StartCancellableOperation"/> calls <see cref="ITeamRibbonView.StartCancellableOperation"/>.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void StartCancellableOperationCallsStartCancellableOperationOnTheView()
        {
            // Arrange
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

            // Act
            this.sut.StartCancellableOperation("testmessage", cancellationTokenSource);

            // Assert
            this.mockRibbon.Verify(ribbon => ribbon.StartCancellableOperation("Microsoft Word", "testmessage", cancellationTokenSource), Times.Once(), "The message box was not displayed.");
        }

        /// <summary>
        /// Tests that <see cref="TeamRibbonPresenter.UpdateCancellableOperation"/> calls <see cref="ITeamRibbonView.UpdateCancellableOperation"/>.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void UpdateCancellableOperationCallsUpdateCancellableOperationOnTheView()
        {
            // Arrange
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource())
            {
                this.sut.StartCancellableOperation("testmessage", cancellationTokenSource);

                // Act
                this.sut.UpdateCancellableOperation("updatemessage");

                // Assert
                this.mockRibbon.Verify(ribbon => ribbon.UpdateCancellableOperation("updatemessage"), Times.Once(), "The message box was not updated.");
            }
        }

        /// <summary>
        /// Tests that <see cref="TeamRibbonPresenter.EndCancellableOperation"/> calls <see cref="ITeamRibbonView.EndCancellableOperation"/>.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void EndCancellableOperationCallsEndCancellableOperationOnTheView()
        {
            // Arrange
            using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource())
            {
                this.sut.StartCancellableOperation("testmessage", cancellationTokenSource);

                // Act
                this.sut.EndCancellableOperation();

                // Assert
                this.mockRibbon.Verify(ribbon => ribbon.EndCancellableOperation(), Times.Once(), "The message box was not updated.");
            }
        }

        /// <summary>
        /// Rebind callback not called if there is no rebind.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void RebindCallbacNotCalledIfNoRebind()
        {
            // Arrange
            this.SetActiveDocument();
            Assert.IsFalse(this.rebindCallbackCalled, "Pre-condition failed");

            // Act
            this.sut.LoadState();

            // Assert
            Assert.IsFalse(this.rebindCallbackCalled, "The presenter called the rebind callback when the document did not require rebinding");
        }

        /// <summary>
        /// A rebind callback on a document raises the Rebind event.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void RebindCallbackRaisesPresenterRebindEvent()
        {
            // Arrange
            this.SetActiveDocument();
            this.mockDocument.Setup(document => document.Load(It.IsAny<Func<Uri>>())).Returns(new string[0]).Callback((Func<Uri> callback) => callback());
            Assert.IsFalse(this.rebindCallbackCalled, "Pre-condition failed");

            // Act
            this.sut.LoadState();

            // Assert
            Assert.IsTrue(this.rebindCallbackCalled, "The presenter did not call the rebind callback when the document required rebinding");
        }

        /// <summary>
        /// The <see cref="ITeamRibbonPresenter.AskYesNoQuestion"/> method displays the <see cref="IMessageBoxView"/> view.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void AskYesNoQuestionDisplaysMessageBoxView()
        {
            // Act
            this.sut.AskYesNoQuestion("testmessage");

            // Assert
            this.mockMessageBoxView.Verify(v => v.Show("testmessage", "Microsoft Word", MessageBoxViewButtons.YesNo, MessageBoxViewIcon.Warning));
        }

        /// <summary>
        /// The <see cref="ITeamRibbonPresenter.AskYesNoQuestion"/> returns <c>true</c> if the user selects Yes on the <see cref="IMessageBoxView"/> view.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void AskYesNoQuestionReturnsTrueIfUserSelectsYesOnMessageBoxView()
        {
            // Arrange
            this.mockMessageBoxView.Setup(v => v.Show("testmessage", "Microsoft Word", MessageBoxViewButtons.YesNo, MessageBoxViewIcon.Warning)).Returns(MessageBoxResult.Yes);

            // Act
            bool ans = this.sut.AskYesNoQuestion("testmessage");

            // Assert
            Assert.IsTrue(ans, "Should return true if yes button pressed");
        }

        /// <summary>
        /// The <see cref="ITeamRibbonPresenter.AskYesNoQuestion"/> returns <c>false</c> if the user selects No on the <see cref="IMessageBoxView"/> view.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void AskYesNoQuestionReturnsFalseIfUserSelectsNoOnMessageBoxView()
        {
            // Arrange
            this.mockMessageBoxView.Setup(v => v.Show("testmessage", "Microsoft Word", MessageBoxViewButtons.YesNo, MessageBoxViewIcon.Warning)).Returns(MessageBoxResult.No);

            // Act
            bool ans = this.sut.AskYesNoQuestion("testmessage");

            // Assert
            Assert.IsFalse(ans, "Should return false if no button pressed");
        }

        /// <summary>
        /// Tests that <see cref="TeamRibbonPresenter.UpdateState"/> tells the view to check the Show Bookmarks control if <see cref="ISettings.ShowBookmarks"/> is set to true.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void UpdateStateShowBookmarksCheckedIfSettingIsTrue()
        {
            // Arrange
            this.mockSettings.Setup(settings => settings.ShowBookmarks).Returns(true);

            // Act
           this.sut.UpdateState();

            // Assert
           this.mockRibbon.Verify(view => view.SetButtonCheckState(TeamRibbonButton.ShowBookmarks, true), Times.Once(), "The show bookmarks button was not checked");
        }

        /// <summary>
        /// Tests that <see cref="TeamRibbonPresenter.UpdateState"/> tells the view to uncheck the Show Bookmarks control if <see cref="ISettings.ShowBookmarks"/> is set to false.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void UpdateStateShowBookmarksUncheckedIfSettingIsFalse()
        {
            // Arrange
            this.mockSettings.Setup(settings => settings.ShowBookmarks).Returns(false);

            // Act
            this.sut.UpdateState();

            // Assert
            this.mockRibbon.Verify(view => view.SetButtonCheckState(TeamRibbonButton.ShowBookmarks, false), Times.Once(), "The show bookmarks button was not unchecked");
        }

        /// <summary>
        /// Tests that <see cref="TeamRibbonPresenter.UpdateState"/> throws exception if <see cref="ISettings.ShowBookmarks"/> is null.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void UpdateStateThrowsExceptionIfShowBookmarksSettingIsNull()
        {
            // Arrange
            Nullable<bool> nullValue = null;
            this.mockSettings.Setup(settings => settings.ShowBookmarks).Returns(nullValue);

            // Act and Assert
            TestHelper.TestForInvalidOperationException(() => this.sut.UpdateState(), "ShowBookmarks setting is not set");
        }

        /// <summary>
        /// Tests that handling the <see cref="ITeamRibbonView.ShowBookmarks"/> sets the option on the <see cref="ITeamProjectDocumentManager"/>.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void ShowBookmarksEventSetsShowBookmarksOptionOnTeamProjectDocumentManager()
        {
            // Act
            this.mockRibbon.Raise(view => view.ShowBookmarks += null, EventArgs.Empty);

            // Assert
            this.mockDocumentManager.Verify(mgr => mgr.SetShowBookmarkOption(true));
        }

        /// <summary>
        /// Tests that handling the <see cref="ITeamRibbonView.HideBookmarks"/> clears the option on the <see cref="ITeamProjectDocumentManager"/>.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void ShowBookmarksEventClearsShowBookmarksOptionOnTeamProjectDocumentManager()
        {
            // Act
            this.mockRibbon.Raise(view => view.HideBookmarks += null, EventArgs.Empty);

            // Assert
            this.mockDocumentManager.Verify(mgr => mgr.SetShowBookmarkOption(false));
        }

        /// <summary>
        /// Tests that handling the <see cref="ITeamProjectDocumentManager.UserMessage"/> event displays the associated warning message.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void UserMessageEventDisplaysWarningMessage()
        {
            // Act
            this.mockDocumentManager.Raise(mgr => mgr.UserMessage += null, new UserMessageEventArgs("test", UIMessageType.Warning));

            // Assert
            this.mockRibbon.Verify(view => view.DisplayMessage(UIMessageType.Warning, "Microsoft Word", "test", null), Times.Once(), "Message not displayed");
        }

        /// <summary>
        /// Tests that handling the <see cref="ITeamProjectDocumentManager.UserMessage"/> event displays the associated error message.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void UserMessageEventDisplaysErrorMessage()
        {
            // Act
            this.mockDocumentManager.Raise(mgr => mgr.UserMessage += null, new UserMessageEventArgs("test", UIMessageType.Error));

            // Assert
            this.mockRibbon.Verify(view => view.DisplayMessage(UIMessageType.Error, "Microsoft Word", "test", null), Times.Once(), "Message not displayed");
        }

        /// <summary>
        /// Sets the active document on the mock manager to be the mock document.
        /// </summary>
        private void SetActiveDocument()
        {
            this.mockDocumentManager.Setup(manager => manager.ActiveDocument).Returns(this.mockDocument.Object);
        }

        /// <summary>
        /// Sets the active document on the mock manager to be the mock document and make it temporary.
        /// </summary>
        private void SetActiveTemporaryDocument()
        {
            this.mockDocumentManager.Setup(manager => manager.ActiveDocument).Returns(this.mockDocument.Object);
            this.mockDocument.Setup(doc => doc.IsTemporary).Returns(true);
        }

        /// <summary>
        /// Sets the mock manager not to have any active document.
        /// </summary>
        private void SetNoActiveDocument()
        {
            ITeamProjectDocument document = null;
            this.mockDocumentManager.Setup(manager => manager.ActiveDocument).Returns(document);
        }

        /// <summary>
        /// Sets the mock active document to be disconnected.
        /// </summary>
        private void SetActiveDocumentDisconnected()
        {
            this.mockDocument.Setup(document => document.IsConnected).Returns(false);
        }

        /// <summary>
        /// Sets the mock active document to be connected.
        /// </summary>
        private void SetActiveDocumentConnected()
        {
            this.mockDocument.Setup(document => document.IsConnected).Returns(true);
        }

        /// <summary>
        /// Sets the mock template to be importable.
        /// </summary>
        private void SetTemplateImportable()
        {
            this.mockSystemTemplate.Setup(template => template.IsImportable).Returns(true);
        }

        /// <summary>
        /// Sets the mock active document to be refreshable.
        /// </summary>
        private void SetActiveDocumentRefreshable()
        {
            this.mockDocument.Setup(document => document.IsRefreshable).Returns(true);
        }

        /// <summary>
        /// Sets the mock active document not to be refreshable.
        /// </summary>
        private void SetActiveDocumentNotRefreshable()
        {
            this.mockDocument.Setup(document => document.IsRefreshable).Returns(false);
        }

        /// <summary>
        /// Sets the system template on the mock manager to be the mock system template.
        /// </summary>
        private void SetSystemTemplate()
        {
            this.mockDocumentManager.Setup(manager => manager.SystemTemplate).Returns(this.mockSystemTemplate.Object);
        }

        /// <summary>
        /// Sets the mock manager not to have any active system template.
        /// </summary>
        private void SetNoSystemTemplate()
        {
            ITeamProjectTemplate template = null;
            this.mockDocumentManager.Setup(manager => manager.SystemTemplate).Returns(template);
        }

        /// <summary>
        /// Sets the mock system template to be importable.
        /// </summary>
        private void SetSystemTemplateImportable()
        {
            this.mockSystemTemplate.Setup(document => document.IsImportable).Returns(true);
        }

        /// <summary>
        /// Sets the mock active document not to be importable.
        /// </summary>
        private void SetSystemTemplateNotImportable()
        {
            this.mockSystemTemplate.Setup(document => document.IsImportable).Returns(false);
        }

        /// <summary>
        /// Sets up the mock template to return a list of TFS layouts.
        /// </summary>
        /// <param name="layouts">The list of layouts that the document should return.</param>
        private void SetupTfsLayout(params LayoutInformation[] layouts)
        {
            this.SetSystemTemplate();
            this.mockSystemTemplate.Setup(template => template.Layouts).Returns(new List<LayoutInformation>(layouts));
        }
    }
}
