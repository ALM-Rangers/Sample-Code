//---------------------------------------------------------------------
// <copyright file="TeamProjectDocumentManagerTests.cs" company="Microsoft">
//    Copyright © ALM | DevOps Ranger Contributors. All rights reserved.
//    This code is licensed under the MIT License.
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF 
//    ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
//    TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
//    PARTICULAR PURPOSE AND NONINFRINGEMENT.
// </copyright>
// <summary>The TeamProjectDocumentManagerTests type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.Word4Tfs.Library.Test.UnitTest.Model
{
    using global::System;
    using global::System.Collections.Generic;
    using global::System.Threading;
    using Microsoft.Office.Interop.Word;
    using Microsoft.Practices.Unity;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Word4Tfs.Library.Data;
    using Microsoft.Word4Tfs.Library.Model;
    using Microsoft.Word4Tfs.Library.Model.TeamFoundation;
    using Microsoft.Word4Tfs.Library.Model.Word;
    using Moq;

    /// <summary>
    /// Tests the <see cref="TeamProjectDocumentManager"/> class.
    /// </summary>
    [TestClass]
    [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Unity container disposed in the test cleanup code.")]
    public class TeamProjectDocumentManagerTests
    {
        /// <summary>
        /// The Unity container to be used for Dependency Injection
        /// </summary>
        private IUnityContainer container;

        /// <summary>
        /// The mock Word application to be used to test the model.
        /// </summary>
        private Mock<IWordApplication> mockApplication;

        /// <summary>
        /// The collection of mock Word documents returned by the Word application.
        /// </summary>
        private List<Mock<IWordDocument>> mockDocuments;

        /// <summary>
        /// The mock system template document returned by the Word application.
        /// </summary>
        private Mock<IWordTemplate> mockSystemTemplate;

        /// <summary>
        /// The mock settings to be used.
        /// </summary>
        private Mock<ISettings> mockSettings;

        /// <summary>
        /// The team project document manager to be tested.
        /// </summary>
        private ITeamProjectDocumentManager sut;

        /// <summary>
        /// Initializes the test.
        /// </summary>
        [TestInitialize]
        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "This is following the pattern recommended but it is not being recognised because in this case it is not a return value but what is passed to another object.")]
        public void InitializeTest()
        {
            HierarchicalLifetimeManager tempLifetimeManager = null;
            try
            {
                tempLifetimeManager = new HierarchicalLifetimeManager();
                this.container = new UnityContainer();
                this.container.RegisterInstance<ILogger>(new Logger());
                this.container.RegisterType<ITeamProjectDocumentManager, TeamProjectDocumentManager>();
                this.container.RegisterType<ITeamProjectTemplate, MockTeamProjectTemplate>();
                this.container.RegisterType<ITeamProjectDocument, MockTeamProjectDocument>(tempLifetimeManager);
                this.mockApplication = TestHelper.CreateAndRegisterMock<IWordApplication>(this.container);
                this.mockSettings = TestHelper.CreateAndRegisterMock<ISettings>(this.container);
                this.mockSystemTemplate = new Mock<IWordTemplate>();

                this.CreateAndSetupMockDocuments(); // no documents
                this.mockSettings.Setup(s => s.ShowBookmarks).Returns(true);

                this.sut = this.container.Resolve<ITeamProjectDocumentManager>();
                tempLifetimeManager = null;
            }
            finally
            {
                if (tempLifetimeManager != null)
                {
                    tempLifetimeManager.Dispose();
                }
            }
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
        /// Constructor throws exception if the container object is null.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void ConstructorChecksContainerForNull()
        {
            TestHelper.TestForArgumentNullException(() => new TeamProjectDocumentManager(null, this.mockApplication.Object, this.mockSettings.Object, new Logger()), "container");
        }

        /// <summary>
        /// Constructor throws exception if the application object is null.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void ConstructorChecksApplicationForNull()
        {
            TestHelper.TestForArgumentNullException(() => new TeamProjectDocumentManager(this.container, null, this.mockSettings.Object, new Logger()), "application");
        }

        /// <summary>
        /// Constructor throws exception if the settings object is null.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void ConstructorChecksSettingsForNull()
        {
            Mock<IWordApplication> mockWordApplication = new Mock<IWordApplication>();
            TestHelper.TestForArgumentNullException(() => new TeamProjectDocumentManager(this.container, this.mockApplication.Object, null, new Logger()), "settings");
        }

        /// <summary>
        /// Constructor throws exception if the logger object is null.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void ConstructorChecksLoggerForNull()
        {
            Mock<IWordApplication> mockWordApplication = new Mock<IWordApplication>();
            TestHelper.TestForArgumentNullException(() => new TeamProjectDocumentManager(this.container, this.mockApplication.Object, this.mockSettings.Object, null), "logger");
        }

        /// <summary>
        /// On construction there is no active document if Word has no active document.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void InitiallyHasNoActiveDocumentIfWordHasNoActiveDocument()
        {
            // Act
            ITeamProjectDocument ans = this.sut.ActiveDocument;

            // Assert
            Assert.IsNull(ans, "There should not be an active document immediately after construction of the manager if Word does not have an active document.");
        }

        /// <summary>
        /// On construction there is no active container if Word has no active document.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void InitiallyHasNoActiveContainerIfWordHasNoActiveDocument()
        {
            // Act
            IUnityContainer ans = this.sut.ActiveContainer;

            // Assert
            Assert.IsNull(ans, "There should not be an active container immediately after construction of the manager if Word does not have an active document.");
        }

        /// <summary>
        /// On initialisation there is an active document if Word already has an active document.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void InitiallyHasActiveDocumentIfWordAlreadyHasActiveDocument()
        {
            // Arrange
            this.CreateAndSetupMockDocuments("doc1");
            this.SetMockActiveDocument(0);
            this.sut = this.container.Resolve<ITeamProjectDocumentManager>();

            // Act
            this.sut.Initialise();
            ITeamProjectDocument ans = this.sut.ActiveDocument;

            // Assert
            Assert.IsNotNull(ans, "There should be an active document immediately after construction of the manager when Word already has an active document.");
        }

        /// <summary>
        /// On construction there is an active container if Word already has an active document.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void InitiallyHasActiveContainerIfWordAlreadyHasActiveDocument()
        {
            // Arrange
            this.CreateAndSetupMockDocuments("doc1");
            this.SetMockActiveDocument(0);
            this.sut = this.container.Resolve<ITeamProjectDocumentManager>();

            // Act
            this.sut.Initialise();
            IUnityContainer ans = this.sut.ActiveContainer;

            // Assert
            Assert.IsNotNull(ans, "There should be an active container immediately after construction of the manager when Word already has an active document.");
        }

        /// <summary>
        /// Active container is a child of the root container.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void ActiveContainerIsChildOfRootContainer()
        {
            // Arrange
            this.CreateAndSetupMockDocuments("doc1");
            this.SetMockActiveDocument(0);
            this.sut = this.container.Resolve<ITeamProjectDocumentManager>();

            // Act
            this.sut.Initialise();
            IUnityContainer ans = this.sut.ActiveContainer;

            // Assert
            Assert.AreSame(this.container, ans.Parent, "The active container is not a child of the root container.");
        }

        /// <summary>
        /// The currently active document on the Word application is passed to the active document when it is created.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void ActiveDocumentIsCreatedWithCurrentlyActiveDocumentInWordApplication()
        {
            // Arrange
            this.CreateAndSetupMockDocuments("doc1");
            this.SetMockActiveDocument(0);
            this.sut = this.container.Resolve<ITeamProjectDocumentManager>();

            // Act
            this.sut.Initialise();

            // Assert
            MockTeamProjectDocument mockTpd = (MockTeamProjectDocument)this.sut.ActiveDocument;
            Assert.AreSame(this.mockDocuments[0].Object, mockTpd.WordDocument, "The Team Project Document was not created with the Word application's active document");
        }

        /// <summary>
        /// Creating a Team Project Document does not leave an <see cref="IWordDocument"/> object in the Unity container.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void CreatingATeamProjectDocumentDoesNotLeaveAnIWordDocumentObjectInTheUnityContainer()
        {
            // Arrange
            this.CreateAndSetupMockDocuments("doc1");
            this.SetMockActiveDocument(0);

            // Act
            this.sut = this.container.Resolve<ITeamProjectDocumentManager>(); // assign to this so it can be disposed.

            // Assert
            Assert.IsFalse(this.container.IsRegistered<IWordDocument>(), "Unity container should not be left containing an IWordDocument");
        }

        /// <summary>
        /// Application document change event changes the active document.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void ApplicationDocumentChangeEventChangesTheActiveDocument()
        {
            // Arrange
            this.CreateAndSetupMockDocuments("doc1");
            this.SetMockActiveDocument(0);

            // Act
            Assert.IsNull(this.sut.ActiveDocument, "Pre-condition check failed, there should not be an active document at this point");
            this.sut.Initialise();
            this.RaiseDocumentChangeEvent();
            ITeamProjectDocument ans = this.sut.ActiveDocument;

            // Assert
            Assert.IsNotNull(ans, "A team project document should be created after an application document change event.");
        }

        /// <summary>
        /// User-created document is not marked temporary.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void UserCreatedDocumentIsNotMarkedTemporary()
        {
            // Arrange
            this.CreateAndSetupMockDocuments("doc1");
            this.SetMockActiveDocument(0);

            // Act
            Assert.IsNull(this.sut.ActiveDocument, "Pre-condition check failed, there should not be an active document at this point");
            this.sut.Initialise();
            this.RaiseDocumentChangeEvent();
            ITeamProjectDocument ans = this.sut.ActiveDocument;

            // Assert
            Assert.IsFalse(ans.IsTemporary, "Team Project Document should not be temporary.");
        }

        /// <summary>
        /// Application document change event changes the active container.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void ApplicationDocumentChangeEventChangesTheActiveContainer()
        {
            // Arrange
            this.CreateAndSetupMockDocuments("doc1");
            this.SetMockActiveDocument(0);

            // Act
            Assert.IsNull(this.sut.ActiveContainer, "Pre-condition check failed, there should not be an active container at this point");
            this.sut.Initialise();
            this.RaiseDocumentChangeEvent();
            IUnityContainer ans = this.sut.ActiveContainer;

            // Assert
            Assert.IsNotNull(ans, "A child container should be created after an application document change event.");
        }

        /// <summary>
        /// Application document change event raises <see cref="ITeamProjectDocumentManager.DocumentChanged"/> event on the manager, after the active document has changed.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void ApplicationDocumentChangeEventRaisesDocumentChangedEventAfterChangingActiveDocument()
        {
            // Arrange
            bool eventRaised = false;
            this.sut.DocumentChanged += delegate
            {
                Assert.IsNotNull(this.sut.ActiveDocument, "Active document must be set before calling the event hander.");
                eventRaised = true;
            };

            this.CreateAndSetupMockDocuments("doc1");
            this.SetMockActiveDocument(0);

            // Act
            Assert.IsNull(this.sut.ActiveDocument, "Pre-condition check failed, there should not be an active document at this point");
            this.sut.Initialise();
            this.RaiseDocumentChangeEvent();

            // Assert
            Assert.IsTrue(eventRaised, "The DocumentChanged event was not raised");
        }

        /// <summary>
        /// Manager does not re-create team project document if it is already open.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void ManagerDoesNotRecreateTeamProjectDocumentIfItIsAlreadyOpen()
        {
            // Arrange
            this.sut.Initialise();
            this.CreateAndSetupMockDocuments("doc1");

            // Act
            this.SetMockActiveDocument(0);
            this.RaiseDocumentChangeEvent();
            ITeamProjectDocument ans1 = this.sut.ActiveDocument;
            this.RaiseDocumentChangeEvent();
            ITeamProjectDocument ans2 = this.sut.ActiveDocument;

            // Assert
            Assert.AreSame(ans2, ans1, "Should not recreate the team project document once it has been opened.");
        }

        /// <summary>
        /// Manager re-uses Team Project Document connection after unconnected document becomes connected.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void ManagerReusesTeamProjectDocumentConnectionAfterDocumentBecomesConnected()
        {
            // Arrange
            this.sut.Initialise();
            this.CreateAndSetupMockDocuments("doc1", "doc2");
            this.SetMockActiveDocument(0);
            this.RaiseDocumentChangeEvent();
            ITeamProjectDocument originalTpd = this.sut.ActiveDocument;
            this.mockDocuments[0].Setup(doc => doc.Handle).Returns(Guid.NewGuid());

            // Act
            MockTeamProjectDocument mockDoc = (MockTeamProjectDocument)this.sut.ActiveDocument;
            mockDoc.RaiseConnectedEvent();
            this.SetMockActiveDocument(1);
            this.RaiseDocumentChangeEvent();
            this.SetMockActiveDocument(0);
            this.RaiseDocumentChangeEvent();
            ITeamProjectDocument newTpd = this.sut.ActiveDocument;

            // Assert
            Assert.AreSame(originalTpd, newTpd, "Should not recreate the team project document once it has been connected.");
        }

        // TODO: Add tests for when handle is null, and more than one non-tpd doc is open.

        /// <summary>
        /// Manager sets the word document on the team project document even if it is already open.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void ManagerSetsWordDocumentOnTeamProjectDocumentEvenIfItIsAlreadyOpen()
        {
            // Arrange
            this.sut.Initialise();
            this.CreateAndSetupMockDocuments("doc1", "doc2");
            this.mockDocuments[1].Setup(doc => doc.Handle).Returns(this.mockDocuments[0].Object.Handle); // Make it have same handle so appears to be the same document, but is actually a different object.

            // Act
            this.SetMockActiveDocument(0);
            this.RaiseDocumentChangeEvent();
            this.SetMockActiveDocument(1);
            this.RaiseDocumentChangeEvent();
            IWordDocument ans = this.sut.ActiveDocument.WordDocument;

            // Assert
            Assert.AreSame(this.mockDocuments[1].Object, ans, "Should set active document into team project document each time.");
        }

        /// <summary>
        /// Manager does not re-create container if document is already open.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void ManagerDoesNotRecreateContainerIfDocumentIsAlreadyOpen()
        {
            // Arrange
            this.sut.Initialise();
            this.CreateAndSetupMockDocuments("doc1");

            // Act
            this.SetMockActiveDocument(0);
            this.RaiseDocumentChangeEvent();
            IUnityContainer ans1 = this.sut.ActiveContainer;
            this.RaiseDocumentChangeEvent();
            IUnityContainer ans2 = this.sut.ActiveContainer;

            // Assert
            Assert.AreSame(ans2, ans1, "Should not recreate the container once the document has been opened.");
        }

        /// <summary>
        /// Manager can keep more than one document open.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void MangerCanKeepMoreThanOneDocumentOpen()
        {
            // Arrange
            this.sut.Initialise();
            this.CreateAndSetupMockDocuments("doc1", "doc2");

            // Act
            this.SetMockActiveDocument(0);
            this.RaiseDocumentChangeEvent();
            ITeamProjectDocument doc1_1 = this.sut.ActiveDocument;

            this.SetMockActiveDocument(1);
            this.RaiseDocumentChangeEvent();
            ITeamProjectDocument doc2_1 = this.sut.ActiveDocument;

            this.SetMockActiveDocument(0);
            this.RaiseDocumentChangeEvent();
            ITeamProjectDocument doc1_2 = this.sut.ActiveDocument;

            this.SetMockActiveDocument(1);
            this.RaiseDocumentChangeEvent();
            ITeamProjectDocument doc2_2 = this.sut.ActiveDocument;

            // Assert
            Assert.AreSame(doc1_2, doc1_1, "Should not recreate the team project document once it has been opened.");
            Assert.AreSame(doc2_2, doc2_1, "Should not recreate the team project document once it has been opened.");
        }

        /// <summary>
        /// Application document change event raises <see cref="ITeamProjectDocumentManager.DocumentChanged"/> event on the manager, even if the document is already open.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void ApplicationDocumentChangeEventRaisesDocumentChangedEventEvenIfDocumentIsAlreadyOpen()
        {
            // Arrange
            this.sut.Initialise();
            this.CreateAndSetupMockDocuments("doc1", "doc2");
            this.SetMockActiveDocument(0);
            this.RaiseDocumentChangeEvent();
            this.SetMockActiveDocument(1);
            this.RaiseDocumentChangeEvent();
            this.SetMockActiveDocument(0);
            bool eventRaised = false;
            this.sut.DocumentChanged += delegate { eventRaised = true; };

            // Act
            this.RaiseDocumentChangeEvent();

            // Assert
            Assert.IsTrue(eventRaised, "The DocumentChanged event was not raised");
        }

        /// <summary>
        /// Document change event from application object after closing the last document causes the manager not to have an active document.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void NoActiveDocumentWhenApplicationHasNoActiveDocumentsAfterADocumentChange()
        {
            // Arrange
            this.sut.Initialise();
            this.CreateAndSetupMockDocuments("doc1");
            this.SetMockActiveDocument(0);
            this.RaiseDocumentChangeEvent();
            this.CreateAndSetupMockDocuments();

            // Act
            this.RaiseDocumentChangeEvent();

            // Assert
            Assert.IsNull(this.sut.ActiveDocument, "With no documents left there should be no active document.");
        }

        /// <summary>
        /// Document change event from application object after closing the last document causes the manager not to have an active container.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void NoActiveDocumentWhenApplicationHasNoActiveContainerAfterADocumentChange()
        {
            // Arrange
            this.sut.Initialise();
            this.CreateAndSetupMockDocuments("doc1");
            this.SetMockActiveDocument(0);
            this.RaiseDocumentChangeEvent();
            this.CreateAndSetupMockDocuments();

            // Act
            this.RaiseDocumentChangeEvent();

            // Assert
            Assert.IsNull(this.sut.ActiveContainer, "With no documents left there should be no active container.");
        }

        /// <summary>
        /// Document change event from application object after closing the last document still causes manager to raise the <see cref="ITeamProjectDocumentManager.DocumentChanged"/> event.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void DocumentChangedEventStillRaisedWhenApplicationHasNoActiveDocumentsAfterADocumentChange()
        {
            // Arrange
            this.CreateAndSetupMockDocuments("doc1");
            this.SetMockActiveDocument(0);
            this.RaiseDocumentChangeEvent();
            this.CreateAndSetupMockDocuments();
            bool eventRaised = false;
            this.sut.DocumentChanged += delegate { eventRaised = true; };
            this.sut.Initialise();

            // Act
            this.RaiseDocumentChangeEvent();

            // Assert
            Assert.IsTrue(eventRaised, "The DocumentChanged event should have been raised to say there was no active document.");
        }

        /// <summary>
        /// Document closure event from the application disposes the team project document.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void DocumentClosureEventFromTheApplicationDisposesTheTeamProjectDocument()
        {
            // Arrange
            this.sut.Initialise();
            this.CreateAndSetupMockDocuments("doc1");
            this.SetMockActiveDocument(0);
            this.RaiseDocumentChangeEvent();
            MockTeamProjectDocument tpd = (MockTeamProjectDocument)this.sut.ActiveDocument;
            this.SetMockActiveDocumentNull(); // Don't actually know if Active Document would be null at this point, but defensively it is best to assume so.

            // Act
            this.RaiseDocumentBeforeCloseEvent(0);

            // Assert
            Assert.IsTrue(tpd.DisposeCalled, "The team project document should be disposed when the underlying document is closed.");
        }

        /// <summary>
        /// Team Project Document constructed again after closing it if it is reopened.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void ReopeningClosedDocumentRecreatesTheTeamProjectDocument()
        {
            // Arrange
            this.CreateAndSetupMockDocuments("doc1");
            this.SetMockActiveDocument(0);
            this.RaiseDocumentChangeEvent();
            ITeamProjectDocument tpd1 = this.sut.ActiveDocument;
            this.SetMockActiveDocumentNull(); // Don't actually know if Active Document would be null at this point, but defensively it is best to assume so.
            this.RaiseDocumentBeforeCloseEvent(0);
            this.SetMockActiveDocument(0);
            this.sut.Initialise();

            // Act
            this.RaiseDocumentChangeEvent();
            ITeamProjectDocument tpd2 = this.sut.ActiveDocument;

            // Assert
            Assert.AreNotSame(tpd2, tpd1, "The team project document should be created again if it is opened again after it was closed.");
        }

        /// <summary>
        /// Container constructed again after closing the document it if it is reopened.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void ReopeningClosedDocumentRecreatesTheContainer()
        {
            // Arrange
            this.CreateAndSetupMockDocuments("doc1");
            this.SetMockActiveDocument(0);
            this.RaiseDocumentChangeEvent();
            IUnityContainer container1 = this.sut.ActiveContainer;
            this.SetMockActiveDocumentNull(); // Don't actually know if Active Document would be null at this point, but defensively it is best to assume so.
            this.RaiseDocumentBeforeCloseEvent(0);
            this.SetMockActiveDocument(0);
            this.sut.Initialise();

            // Act
            this.RaiseDocumentChangeEvent();
            IUnityContainer container2 = this.sut.ActiveContainer;

            // Assert
            Assert.AreNotSame(container2, container1, "The container should be created again if the document is opened again after it was closed.");
        }

        /// <summary>
        /// On initialisation there is no system template if one is not available.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void InitiallyHasNoSystemTemplateIfSystemTemplateIsNotPresent()
        {
            // Arrange
            IWordTemplate template = null;
            this.mockApplication.Setup(app => app.SystemTemplate).Returns(template);
            this.sut = this.container.Resolve<ITeamProjectDocumentManager>();

            // Act
            this.sut.Initialise();
            ITeamProjectTemplate ans = this.sut.SystemTemplate;

            // Assert
            Assert.IsNull(ans, "There should not be a system template immediately after initialisation of the manager when one is not present.");
        }

        /// <summary>
        /// On initialisation there should be a null template registered in the container if there is no system template present.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void NullWordTemplateRegisteredInContainerEvenIfSystemTemplateIsNotPresent()
        {
            // Arrange
            IWordTemplate template = null;
            this.mockApplication.Setup(app => app.SystemTemplate).Returns(template);
            this.sut = this.container.Resolve<ITeamProjectDocumentManager>();

            // Act
            this.sut.Initialise();

            // Assert
            Assert.IsInstanceOfType(this.container.Resolve<IWordTemplate>(), typeof(NullWordTemplate), "Null template should be registered in container if there is no system template");
        }

        /// <summary>
        /// On initialisation there should be a  template registered in the container if there is a system template present.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void FullTemplateRegisteredInContainerIfSystemTemplateIsPresent()
        {
            // Arrange
            this.sut = this.container.Resolve<ITeamProjectDocumentManager>();

            // Act
            this.sut.Initialise();
            ITeamProjectTemplate ans = this.sut.SystemTemplate;

            // Assert
            Assert.IsTrue(this.container.IsRegistered<IWordTemplate>(), "Full template should be registered in container if there is a system template");
        }

        /// <summary>
        /// On initialisation there is a system template if one is available.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void InitiallyHasSystemTemplateIfSystemTemplateIsPresent()
        {
            // Arrange
            this.CreateAndSetupSystemTemplate();
            this.sut = this.container.Resolve<ITeamProjectDocumentManager>();

            // Act
            this.sut.Initialise();
            ITeamProjectTemplate ans = this.sut.SystemTemplate;

            // Assert
            Assert.IsNotNull(ans, "There should be a system template immediately after construction of the manager when one is present.");
        }

        /// <summary>
        /// On initialisation the system template is loaded if one is available.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void SystemTemplateIsLoadedIfSystemTemplateIsPresent()
        {
            // Arrange
            this.CreateAndSetupSystemTemplate();
            this.sut = this.container.Resolve<ITeamProjectDocumentManager>();

            // Act
            this.sut.Initialise();
            ITeamProjectTemplate ans = this.sut.SystemTemplate;

            // Assert
            MockTeamProjectTemplate mock = (MockTeamProjectTemplate)ans;
            Assert.IsTrue(mock.LoadCalled, "Load was not called for the template.");
        }

        /// <summary>
        /// Test that <see cref="ITeamProjectDocumentManager.Add"/> adds a new team project document.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void AddCreatesANewTeamProjectDocument()
        {
            // Arrange
            this.sut = this.container.Resolve<ITeamProjectDocumentManager>();
            this.sut.Initialise();
            this.CreateAndSetupMockDocuments("doc1");
            this.SetMockActiveDocument(0);
            this.mockApplication.Setup(app => app.CreateNewDocument()).Callback(() => this.RaiseDocumentChangeEvent());

            // Act
            this.sut.Add(false);

            // Assert
            this.mockApplication.Verify(app => app.CreateNewDocument(), Times.Once(), "A new document was not created.");
        }

        /// <summary>
        /// Test that <see cref="ITeamProjectDocumentManager.Add"/> returns the new team project document.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void AddReturnsNewTeamProjectDocument()
        {
            // Arrange
            this.sut = this.container.Resolve<ITeamProjectDocumentManager>();
            this.sut.Initialise();
            Assert.IsNull(this.sut.ActiveDocument, "Pre-requisite is that there is no active document.");
            this.CreateAndSetupMockDocuments("doc1");
            this.SetMockActiveDocument(0);
            this.mockApplication.Setup(app => app.CreateNewDocument()).Callback(() => this.RaiseDocumentChangeEvent());

            // Act
            ITeamProjectDocument ans = this.sut.Add(false);

            // Assert
            Assert.IsNotNull(ans, "No team project document returned");
            Assert.IsNotNull(this.sut.ActiveDocument, "No team project document is active");
        }

        /// <summary>
        /// Test that <see cref="ITeamProjectDocumentManager.Add"/> returns a new team project document that is marked as temporary if true is passed.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void AddReturnsNewTeamProjectDocumentAsTemporary()
        {
            // Arrange
            this.sut = this.container.Resolve<ITeamProjectDocumentManager>();
            this.sut.Initialise();
            this.CreateAndSetupMockDocuments("doc1");
            this.SetMockActiveDocument(0);
            this.mockApplication.Setup(app => app.CreateNewDocument()).Callback(() => this.RaiseDocumentChangeEvent());

            // Act
            ITeamProjectDocument ans = this.sut.Add(true);

            // Assert
            Assert.IsTrue(ans.IsTemporary, "Document not created as temporary");
        }

        /// <summary>
        /// Test that <see cref="ITeamProjectDocumentManager.Add"/> returns a new team project document that is not marked as temporary if false is passed.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void AddReturnsNewTeamProjectDocumentAsPermanent()
        {
            // Arrange
            this.sut = this.container.Resolve<ITeamProjectDocumentManager>();
            this.CreateAndSetupMockDocuments("doc1");
            this.SetMockActiveDocument(0);
            this.sut.Initialise();
            this.mockApplication.Setup(app => app.CreateNewDocument()).Callback(() => this.RaiseDocumentChangeEvent());

            // Act
            ITeamProjectDocument ans = this.sut.Add(false);

            // Assert
            Assert.IsFalse(ans.IsTemporary, "Document not created as permanent");
        }

        /// <summary>
        /// Test that <see cref="IWordApplication.DocumentBeforeClose"/> event raises the <see cref="ITeamProjectDocumentManager.DocumentBeforeClose"/> event if the active document is about to be closed.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void RaisesDocumentBeforeCloseIfActiveDocumentIsAboutToBeClosed()
        {
            // Arrange
            this.CreateAndSetupMockDocuments("doc1", "doc2");
            this.SetMockActiveDocument(0);
            this.sut.Initialise();
            this.RaiseDocumentChangeEvent();
            bool eventRaised = false;
            this.sut.DocumentBeforeClose += delegate { eventRaised = true; };

            // Act
            this.RaiseDocumentBeforeCloseEvent(0);

            // Assert
            Assert.IsTrue(eventRaised, "The BeforeDocumentClose event should have been raised.");
        }

        /// <summary>
        /// Test that <see cref="IWordApplication.DocumentBeforeClose"/> event does not raise the <see cref="ITeamProjectDocumentManager.DocumentBeforeClose"/> event if a non-active document is about to be closed.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void DoesNotRaiseDocumentBeforeCloseIfNonActiveDocumentIsAboutToBeClosed()
        {
            // Arrange
            this.CreateAndSetupMockDocuments("doc1", "doc2");
            this.SetMockActiveDocument(0);
            this.sut.Initialise();
            this.RaiseDocumentChangeEvent();
            bool eventRaised = false;
            this.sut.DocumentBeforeClose += delegate { eventRaised = true; };

            // Act
            this.RaiseDocumentBeforeCloseEvent(1);

            // Assert
            Assert.IsFalse(eventRaised, "The BeforeDocumentClose event should not have been raised because the document being closed is not active.");
        }

        /// <summary>
        /// Test that the cancel value from raising the <see cref="ITeamProjectDocumentManager.DocumentBeforeClose"/> event is passed to the Word Application
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void CancelValueFromRaisingDocumentBeforeCloseIsPassedBackToWordApplication()
        {
            // Arrange
            this.CreateAndSetupMockDocuments("doc1", "doc2");
            this.SetMockActiveDocument(0);
            this.sut.Initialise();
            this.RaiseDocumentChangeEvent();
            this.sut.DocumentBeforeClose += (s, e) => e.Cancel = true;

            // Act
            bool cancel = this.RaiseDocumentBeforeCloseEvent(0);

            // Assert
            Assert.IsTrue(cancel, "The cancel value was not passed back to the application.");
        }

        /// <summary>
        /// Test that cancelling the <see cref="ITeamProjectDocumentManager.DocumentBeforeClose"/> event causes the document to remain as managed by the manager.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void CancellingDocumentBeforeCloseCausesDocumentToRemainAsManagedByManager()
        {
            // Arrange
            this.CreateAndSetupMockDocuments("doc1", "doc2");
            this.SetMockActiveDocument(0);
            this.sut.Initialise();
            this.RaiseDocumentChangeEvent();
            this.sut.DocumentBeforeClose += (s, e) => e.Cancel = true;
            MockTeamProjectDocument tpd = (MockTeamProjectDocument)this.sut.ActiveDocument;

            // Act
            bool cancel = this.RaiseDocumentBeforeCloseEvent(0);

            // Assert
            Assert.IsFalse(tpd.DisposeCalled, "The team project document should not be disposed when the underlying document close is cancelled.");
        }

        /// <summary>
        /// Test that the <see cref="ITeamProjectDocument.ConnectionStatusChanged"/> event causes the bookmarks setting to be set if the document is now connected.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void ConnectionStatusChangedToConnectedSetsShowBookmarkSettingsToFollowSetting()
        {
            // Arrange
            this.mockSettings.Setup(settings => settings.ShowBookmarks).Returns(true);
            this.CreateAndSetupMockDocuments("doc1", "doc2");
            this.SetMockActiveDocument(0);
            this.sut.Initialise();
            this.RaiseDocumentChangeEvent();
            this.sut.DocumentBeforeClose += (s, e) => e.Cancel = true;
            MockTeamProjectDocument tpd = (MockTeamProjectDocument)this.sut.ActiveDocument;

            // Act
            this.RaiseConnectionStatusChangedEvent(tpd, true);

            // Assert
            this.mockApplication.VerifySet(app => app.ShowBookmarks = true);
        }

        /// <summary>
        /// Test that the <see cref="ITeamProjectDocument.ConnectionStatusChanged"/> event causes the bookmarks setting to be cleared if the document is now connected.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void ConnectionStatusChangedToConnectedClearsShowBookmarkSettingsToFollowSetting()
        {
            // Arrange
            this.mockSettings.Setup(settings => settings.ShowBookmarks).Returns(false);
            this.CreateAndSetupMockDocuments("doc1", "doc2");
            this.SetMockActiveDocument(0);
            this.sut.Initialise();
            this.RaiseDocumentChangeEvent();
            this.sut.DocumentBeforeClose += (s, e) => e.Cancel = true;
            MockTeamProjectDocument tpd = (MockTeamProjectDocument)this.sut.ActiveDocument;

            // Act
            this.RaiseConnectionStatusChangedEvent(tpd, true);

            // Assert
            this.mockApplication.VerifySet(app => app.ShowBookmarks = false);
        }

        /// <summary>
        /// Test that the <see cref="TeamProjectDocumentManager.Initialise"/> sets <see cref="IWordApplication.ShowBookmarks"/> true if <see cref="ISettings.ShowBookmarks"/> is not set.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void InitialiseSetsApplicationShowBookmarksTrueIfShowBookmarksSettingIsNotSet()
        {
            // Arrange
            this.CreateAndSetupMockDocuments("doc1");
            this.SetMockActiveDocument(0); // force full initialisation
            this.mockSettings.SetupProperty<Nullable<bool>>(s => s.ShowBookmarks, null);

            // Act
            this.sut.Initialise();

            // Assert
            this.mockApplication.VerifySet(app => app.ShowBookmarks = true);
        }

        /// <summary>
        /// Test that the <see cref="TeamProjectDocumentManager.Initialise"/> sets <see cref="ISettings.ShowBookmarks"/> true if <see cref="ISettings.ShowBookmarks"/> is not set.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void InitialiseSetsSettingsShowBookmarksTrueIfShowBookmarksSettingIsNotSet()
        {
            // Arrange
            Nullable<bool> nullValue = null;
            this.mockSettings.Setup(s => s.ShowBookmarks).Returns(nullValue);

            // Act
            this.sut.Initialise();

            // Assert
            this.mockSettings.VerifySet(s => s.ShowBookmarks = true);
        }

        /// <summary>
        /// Test that the <see cref="TeamProjectDocumentManager.Initialise"/> does not display a warning message if <see cref="ISettings.ShowBookmarks"/> is set.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void InitialiseDoesNotDisplayWarningIfShowBookmarksSettingIsSet()
        {
            // Arrange
            this.mockSettings.Setup(s => s.ShowBookmarks).Returns(false);
            UserMessageEventArgs raisedEvent = null;
            this.sut.UserMessage += (s, e) => raisedEvent = e;

            // Act
            this.sut.Initialise();

            // Assert
            Assert.IsNull(raisedEvent, "A user message event was raised.");
        }

        /// <summary>
        /// Test that <see cref="TeamProjectDocumentManager.Initialise"/> does not set <see cref="ISettings.ShowBookmarks"/> if <see cref="ISettings.ShowBookmarks"/> is set.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void InitialiseDoesNotSetSettingsShowBookmarksTrueIfShowBookmarksSettingIsSet()
        {
            // Arrange
            this.mockSettings.Setup(s => s.ShowBookmarks).Returns(true);

            // Act
            this.sut.Initialise();

            // Assert
            this.mockSettings.VerifySet(s => s.ShowBookmarks = true, Times.Never());
        }

        /// <summary>
        /// Test that <see cref="TeamProjectDocumentManager.Initialise"/> does not set <see cref="ISettings.ShowBookmarks"/> if <see cref="ISettings.ShowBookmarks"/> is clear.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void InitialiseDoesNotSetSettingsShowBookmarksFalseIfShowBookmarksSettingIsClear()
        {
            // Arrange
            this.mockSettings.Setup(s => s.ShowBookmarks).Returns(false);

            // Act
            this.sut.Initialise();

            // Assert
            this.mockSettings.VerifySet(s => s.ShowBookmarks = false, Times.Never());
        }

        /// <summary>
        /// Test that the <see cref="TeamProjectDocumentManager.SetShowBookmarkOption"/> sets <see cref="IWordApplication.ShowBookmarks"/> true.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void SetShowBookmarkOptionSetsApplicationShowBookmarksTrue()
        {
            // Act
            this.sut.SetShowBookmarkOption(true);

            // Assert
            this.mockApplication.VerifySet(app => app.ShowBookmarks = true);
        }

        /// <summary>
        /// Test that the <see cref="TeamProjectDocumentManager.SetShowBookmarkOption"/> sets <see cref="ISettings.ShowBookmarks"/> true.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void SetShowBookmarkOptionSetsSettingsShowBookmarksTrue()
        {
            // Act
            this.sut.SetShowBookmarkOption(true);

            // Assert
            this.mockSettings.VerifySet(s => s.ShowBookmarks = true);
        }

        /// <summary>
        /// Test that the <see cref="TeamProjectDocumentManager.SetShowBookmarkOption"/> sets <see cref="IWordApplication.ShowBookmarks"/> false.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void SetShowBookmarkOptionSetsApplicationShowBookmarksFalse()
        {
            // Act
            this.sut.SetShowBookmarkOption(false);

            // Assert
            this.mockApplication.VerifySet(app => app.ShowBookmarks = false);
        }

        /// <summary>
        /// Test that the <see cref="TeamProjectDocumentManager.SetShowBookmarkOption"/> sets <see cref="ISettings.ShowBookmarks"/> false.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void SetShowBookmarkOptionSetsSettingsShowBookmarksFalse()
        {
            // Act
            this.sut.SetShowBookmarkOption(false);

            // Assert
            this.mockSettings.VerifySet(s => s.ShowBookmarks = false);
        }

        /// <summary>
        /// Changing to another document sets the bookmarks setting to match the actual setting.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void ChangingToAnotherDocumentSetsShowBookmarksToMatchSetting()
        {
            // Arrange
            this.mockSettings.Setup(s => s.ShowBookmarks).Returns(true);
            this.CreateAndSetupMockDocuments("doc1", "doc2");
            this.SetMockActiveDocument(0);
            this.RaiseDocumentChangeEvent();
            this.sut.Initialise();
            this.mockApplication.VerifySet(app => app.ShowBookmarks = true, Times.Exactly(1), "pre-requisite");

            // Act
            this.SetMockActiveDocument(1);
            this.RaiseDocumentChangeEvent();

            // Assert
            this.mockApplication.VerifySet(app => app.ShowBookmarks = true, Times.Exactly(2), "Must set the show bookmarks setting on document change.");
        }

        /// <summary>
        /// Sets the mock active document to be null.
        /// </summary>
        private void SetMockActiveDocumentNull()
        {
            IWordDocument returnValue = null;
            this.mockApplication.Setup(application => application.ActiveDocument).Returns(returnValue);
        }

        /// <summary>
        /// Sets the mock active document in the mock application.
        /// </summary>
        /// <param name="index">The index in the mock documents collection to set as the active document.</param>
        private void SetMockActiveDocument(int index)
        {
            this.mockApplication.Setup(application => application.ActiveDocument).Returns(this.mockDocuments[index].Object);
        }

        /// <summary>
        /// Creates mock documents, puts them into the mock documents collection and sets the open document count for the mock application object
        /// </summary>
        /// <param name="names">The names of the documents.</param>
        private void CreateAndSetupMockDocuments(params string[] names)
        {
            this.mockDocuments = new List<Mock<IWordDocument>>();
            for (int i = 0; i < names.Length; i++)
            {
                this.CreateAndSetupMockDocument(names[i]);
            }

            this.mockApplication.Setup(application => application.OpenDocumentCount).Returns(this.mockDocuments.Count);
        }

        /// <summary>
        /// Creates a mock document and adds it to the mock documents list, setting the open document count for the mock application object.
        /// </summary>
        /// <param name="name">The name of the new mock document to create.</param>
        private void CreateAndSetupMockDocument(string name)
        {
            Mock<IWordDocument> mockDocument = new Mock<IWordDocument>();
            mockDocument.Setup(document => document.Name).Returns(name);
            mockDocument.Setup(document => document.Handle).Returns(Guid.NewGuid());
            this.mockDocuments.Add(mockDocument);
        }

        /// <summary>
        /// Sets up the mock system template.
        /// </summary>
        private void CreateAndSetupSystemTemplate()
        {
            this.mockApplication.Setup(app => app.SystemTemplate).Returns(this.mockSystemTemplate.Object);
        }

        /// <summary>
        /// Raises the <see cref="IWordApplication.DocumentChange"/> event on the mock application object.
        /// </summary>
        private void RaiseDocumentChangeEvent()
        {
            this.mockApplication.Raise(application => application.DocumentChange += null, EventArgs.Empty);
        }

        /// <summary>
        /// Raises the <see cref="ITeamProjectDocument.ConnectionStatusChanged"/> event on the mock team project document.
        /// </summary>
        /// <param name="tpd">The mock team project document.</param>
        /// <param name="isConnected">Indicates if the document is now connected or not.</param>
        private void RaiseConnectionStatusChangedEvent(MockTeamProjectDocument tpd, bool isConnected)
        {
            tpd.IsConnected = isConnected; // raises event too.
        }

        /// <summary>
        /// Raises the <see cref="IWordApplication.DocumentBeforeClose"/> event on the mock application.
        /// </summary>
        /// <param name="document">The document to raise the event for.</param>
        /// <returns>The cancel value.</returns>
        private bool RaiseDocumentBeforeCloseEvent(IWordDocument document)
        {
            CancellableDocumentOperationEventArgs args = new CancellableDocumentOperationEventArgs(document);
            this.mockApplication.Raise(application => application.DocumentBeforeClose += null, args);
            return args.Cancel;
        }

        /// <summary>
        /// Raises the <see cref="IWordApplication.DocumentBeforeClose"/> event on the mock application.
        /// </summary>
        /// <param name="index">The index of the mock document to raise the event for.</param>
        /// <returns>The cancel value.</returns>
        private bool RaiseDocumentBeforeCloseEvent(int index)
        {
            return this.RaiseDocumentBeforeCloseEvent(this.mockDocuments[index].Object);
        }

        /// <summary>
        /// Mock <see cref="ITeamProjectDocument"/>. Needed so that there is a class for Unity to construct that implements the <see cref="ITeamProjectDocument"/> interface.
        /// </summary>
        private class MockTeamProjectDocument : ITeamProjectDocument
        {
            /// <summary>
            /// Tracks connection status.
            /// </summary>
            private bool isConnected;

            /// <summary>
            /// Initializes a new instance of the <see cref="MockTeamProjectDocument"/> class.
            /// </summary>
            /// <param name="wordDocument">The word document that underlies the team project document.</param>
            public MockTeamProjectDocument(IWordDocument wordDocument)
            {
                this.WordDocument = wordDocument;
            }

            /// <summary>
            /// Raised when the document is closed.
            /// </summary>
            public event EventHandler Close;

            /// <summary>
            /// Raised when the document becomes connected.
            /// </summary>
            public event EventHandler Connected;

            /// <summary>
            /// Gets a value indicating whether the object has been disposed or not.
            /// </summary>
            public bool DisposeCalled { get; private set; }

            /// <summary>
            /// Gets or sets a value indicating whether the document is connected to a Team Project.
            /// </summary>
            public bool IsConnected
            {
                get
                {
                    return this.isConnected;
                }

                set
                {
                    this.isConnected = value;
                }
            }

            /// <summary>
            /// Gets a value indicating whether the document is refreshable.
            /// </summary>
            public bool IsRefreshable
            {
                get { throw new NotImplementedException(); }
            }

            /// <summary>
            /// Gets or sets a value indicating whether the document is temporary.
            /// </summary>
            /// <remarks>
            /// A temporary document will never be saved. It is used when hosting the layout designer. Setting this to true disables import and refresh.
            /// </remarks>
            public bool IsTemporary { get; set; }

            /// <summary>
            /// Gets a value indicating whether the document contains any queries.
            /// </summary>
            public bool ContainsQueries
            {
                get { throw new NotImplementedException(); }
            }

            /// <summary>
            /// Gets a value indicating whether the document is insertable (work items can be added).
            /// </summary>
            public bool IsInsertable
            {
                get { throw new NotImplementedException(); }
            }
            
            /// <summary>
            /// Gets or sets the word document that the class was constructed with.
            /// </summary>
            public IWordDocument WordDocument { get; set; }

            /// <summary>
            /// Gets or sets the team project.
            /// </summary>
            public ITeamProject TeamProject { get; set; }

            /// <summary>
            /// Gets the current list of queries and layouts after modification to account for all the other queries in the document.
            /// </summary>
            public IEnumerable<QueryAndLayoutInformation> QueriesAndLayouts
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            /// <summary>
            /// Gets the work items associated with each saved set of work items.
            /// </summary>
            public IEnumerable<QueryWorkItems> QueryWorkItems
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            /// <summary>
            /// Gets a value indicating whether the document has changed since <see cref="MarkDocumentClean"/> was called.
            /// </summary>
            public bool HasChanged
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            /// <summary>
            /// Loads the current state from the document.
            /// </summary>
            /// <param name="rebindCallback">The callback to be called if the document must be rebound because the collection cannot be found or the id does not match. The callback must return null to cancel the rebind.</param>
            /// <returns>List of load warnings, empty if there were no warnings. The document is still considered loaded if there are warnings.</returns>
            public IEnumerable<string> Load(Func<Uri> rebindCallback)
            {
                // The following code uses the Close event just to remove a compiler warning that it is not used.
                if (this.Close != null)
                {
                    this.Close(this, EventArgs.Empty);
                }

                throw new NotImplementedException();
            }

            /// <summary>
            /// Saves the team project in the document.
            /// </summary>
            public void SaveTeamProject()
            {
                throw new NotImplementedException();
            }

            /// <summary>
            /// Adds the <paramref name="queryAndLayout"/> to the collection of queries and layouts in the document, but does not save the new query and layout.
            /// </summary>
            /// <param name="queryAndLayout">The query and layout to be added.</param>
            /// <returns>The query and layout with the query modified to include all the fields from all the layouts and exclude any fields not defined in the team project.</returns>
            public QueryAndLayoutInformation AddQueryAndLayout(QueryAndLayoutInformation queryAndLayout)
            {
                throw new NotImplementedException();
            }

            /// <summary>
            /// Saves the queries and layout names previously added to the document by <see cref="AddQueryAndLayout"/>.
            /// </summary>
            public void SaveQueriesAndLayouts()
            {
                throw new NotImplementedException();
            }

            /// <summary>
            /// Saves a tree of work items in the document.
            /// </summary>
            /// <param name="workItems">The work items to be saved.</param>
            /// <param name="fields">The fields in the work item to be saved.</param>
            /// <param name="cancellationToken">Used to cancel the save.</param>
            public void SaveWorkItems(WorkItemTree workItems, string[] fields, CancellationToken cancellationToken)
            {
                throw new NotImplementedException();
            }

            /// <summary>
            /// Maps the saved work items into the document using the given layout.
            /// </summary>
            /// <param name="layout">The layout to be used to map the saved work items.</param>
            /// <param name="index">The index in the query and layout manager of the query that id being processed.</param>
            /// <param name="cancellationToken">Used to cancel the save.</param>
            /// <exception cref="InvalidOperationException">Thrown if <see cref="SaveWorkItems"/> has not been called to save the work items first.</exception>
            public void MapWorkItemsIntoDocument(LayoutInformation layout, int index, CancellationToken cancellationToken)
            {
                throw new NotImplementedException();
            }

            /// <summary>
            /// Refreshes the work items in the document.
            /// </summary>
            /// <remarks>
            /// This call will update the rich text content controls which are not bound to the Custom XML Parts.
            /// </remarks>
            /// <param name="cancellationToken">Used to cancel the save.</param>
            /// <returns>List of verification errors, empty if there were no errors.</returns>
            public IEnumerable<string> RefreshWorkItems(CancellationToken cancellationToken)
            {
                throw new NotImplementedException();
            }

            /// <summary>
            /// Inserts the named layout definition into the document so that the layout can be edited and then updated.
            /// </summary>
            /// <param name="layoutName">The name of the layout to be displayed.</param>
            public void DisplayLayoutDefinition(string layoutName)
            {
                throw new NotImplementedException();
            }

            /// <summary>
            /// Saves a layout defined in the document into the template.
            /// </summary>
            /// <param name="layoutName">The name of the layout to be saved.</param>
            public void SaveLayoutDefinition(string layoutName)
            {
                throw new NotImplementedException();
            }

            /// <summary>
            /// Renames a layout.
            /// </summary>
            /// <param name="oldLayoutName">The name of the layout to be renamed.</param>
            /// <param name="newLayoutName">The new name for the layout.</param>
            public void RenameLayoutDefinition(string oldLayoutName, string newLayoutName)
            {
                throw new NotImplementedException();
            }

            /// <summary>
            /// Adds a prototype layout definition to the document, used when creating a new layout.
            /// </summary>
            public void AddPrototypeLayoutDefinition()
            {
                throw new NotImplementedException();
            }

            /// <summary>
            /// Marks the document as clean.
            /// </summary>
            public void MarkDocumentClean()
            {
                throw new NotImplementedException();
            }

            /// <summary>
            /// Adds a field to the document.
            /// </summary>
            /// <param name="field">The field to be added.</param>
            public void AddField(ITfsFieldDefinition field)
            {
                throw new NotImplementedException();
            }

            /// <summary>
            /// Records the disposal of the object.
            /// </summary>
            public void Dispose()
            {
                this.DisposeCalled = true;
            }

            /// <summary>
            /// Raises the <see cref="MockTeamProjectDocument.Connected"/> event.
            /// </summary>
            internal void RaiseConnectedEvent()
            {
                if (this.Connected != null)
                {
                    this.Connected(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Mock <see cref="ITeamProjectTemplate"/>. Needed so that there is a class for Unity to construct that implements the <see cref="ITeamProjectTemplate"/> interface.
        /// </summary>
        private class MockTeamProjectTemplate : ITeamProjectTemplate
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="MockTeamProjectTemplate"/> class.
            /// </summary>
            public MockTeamProjectTemplate()
            {
            }

            /// <summary>
            /// Gets a value indicating whether the <see cref="Load"/> method was called.
            /// </summary>
            public bool LoadCalled { get; private set; }

            /// <summary>
            /// Gets a value indicating whether the document is importable.
            /// </summary>
            public bool IsImportable
            {
                get { throw new NotImplementedException(); }
            }

            /// <summary>
            /// Gets the list of currently available layouts
            /// </summary>
            /// <remarks>
            /// This call does not require an active document as it accesses the currently available templates.
            /// </remarks>
            public IEnumerable<LayoutInformation> Layouts
            {
                get { throw new NotImplementedException(); }
            }

            /// <summary>
            /// Loads the template.
            /// </summary>
            public void Load()
            {
                this.LoadCalled = true;
            }

            /// <summary>
            /// Saves changes to the template.
            /// </summary>
            public void Save()
            {
                throw new NotImplementedException();
            }

            /// <summary>
            /// Deletes a layout from the template.
            /// </summary>
            /// <param name="layoutName">The name of the layout to be deleted.</param>
            public void DeleteLayout(string layoutName)
            {
                throw new NotImplementedException();
            }

            /// <summary>
            /// Creates a layout.
            /// </summary>
            /// <param name="layoutName">The name of the layout being created.</param>
            /// <param name="blocks">List of building block definitions for the building blocks to be created that make up the layout.</param>
            public void CreateLayout(string layoutName, IEnumerable<BuildingBlockDefinition> blocks)
            {
                throw new NotImplementedException();
            }

            /// <summary>
            /// Gets a named building block from a layout.
            /// </summary>
            /// <param name="layout">The layout for which the building block is to be retrieved.</param>
            /// <param name="name">The name of the building block to retrieve.</param>
            /// <returns>The named <see cref="BuildingBlock"/> from the given layout.</returns>
            public BuildingBlock GetLayoutBuildingBlock(LayoutInformation layout, BuildingBlockName name)
            {
                throw new NotImplementedException();
            }
        }
    }
}
