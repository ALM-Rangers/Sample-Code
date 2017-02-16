//---------------------------------------------------------------------
// <copyright file="TeamProjectDocumentTests.cs" company="Microsoft">
//    Copyright (c) Microsoft. All rights reserved.
//    This code is licensed under the MIT License.
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF 
//    ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
//    TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
//    PARTICULAR PURPOSE AND NONINFRINGEMENT.
// </copyright>
// <summary>The TeamProjectDocumentTests type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.Word4Tfs.Library.Test.UnitTest.Model
{
    using System;
    using global::System.Collections.Generic;
    using global::System.Globalization;
    using global::System.Linq;
    using global::System.Threading;
    using global::System.Xml.Linq;
    using Microsoft.Office.Core;
    using Microsoft.Office.Interop.Word;
    using Microsoft.Practices.Unity;
    using Microsoft.TeamFoundation;
    using Microsoft.TeamFoundation.WorkItemTracking.Client;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Word4Tfs.Library.Data;
    using Microsoft.Word4Tfs.Library.Model;
    using Microsoft.Word4Tfs.Library.Model.TeamFoundation;
    using Microsoft.Word4Tfs.Library.Model.Word;
    using Moq;

    /// <summary>
    /// Tests the <see cref="TeamProjectDocument"/> class.
    /// </summary>
    [TestClass]
    [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Unity container disposed in the test cleanup code.")]
    public class TeamProjectDocumentTests
    {
        /// <summary>
        /// A test field name used in the tests.
        /// </summary>
        private const string FieldA = "a.a";

        /// <summary>
        /// A test field name used in the tests.
        /// </summary>
        private const string FieldB = "b.b";

        /// <summary>
        /// A test field name used in the tests.
        /// </summary>
        private const string FieldC = "c.c";

        /// <summary>
        /// The Unity container to be used for Dependency Injection
        /// </summary>
        private IUnityContainer container;

        /// <summary>
        /// The mock word document used to test the model.
        /// </summary>
        private Mock<IWordDocument> mockWordDocument;

        /// <summary>
        /// The mock team project template used to test the model.
        /// </summary>
        private Mock<ITeamProjectTemplate> mockTeamProjectTemplate;

        /// <summary>
        /// The mock team project document formatter used to test the model.
        /// </summary>
        private Mock<ITeamProjectDocumentFormatter> mockTeamProjectDocumentFormatter;

        /// <summary>
        /// The mock team project document verifier used to test the model.
        /// </summary>
        private Mock<ITeamProjectDocumentVerifier> mockTeamProjectDocumentVerifier;

        /// <summary>
        /// The mock team project query to use in some tests.
        /// </summary>
        private Mock<ITeamProjectQuery> mockTeamProjectQuery;

        /// <summary>
        /// The mock factory used to test the model.
        /// </summary>
        private Mock<IFactory> mockFactory;

        /// <summary>
        /// The mock query and layout manager to use.
        /// </summary>
        private Mock<IQueryAndLayoutManager> mockQueryAndLayoutManager;

        /// <summary>
        /// The mock layout definition formatter to use.
        /// </summary>
        private Mock<ILayoutDefinitionFormatter> mockLayoutDefinitionFormatter;

        /// <summary>
        /// Maintains the list of queries and layouts added to <see cref="mockQueryAndLayoutManager"/>.
        /// </summary>
        private List<QueryAndLayoutInformation> originalQueriesAndLayouts = new List<QueryAndLayoutInformation>();

        /// <summary>
        /// The model to be tested.
        /// </summary>
        private ITeamProjectDocument sut;

        /// <summary>
        /// Initializes the test.
        /// </summary>
        [TestInitialize]
        public void InitializeTest()
        {
            this.container = new UnityContainer();
            this.container.RegisterInstance<ILogger>(new Logger());

            this.mockWordDocument = TestHelper.CreateAndRegisterMock<IWordDocument>(this.container);
            this.mockWordDocument.Setup(document => document.Name).Returns("test.doc");

            this.mockTeamProjectTemplate = TestHelper.CreateAndRegisterMock<ITeamProjectTemplate>(this.container);
            
            this.mockTeamProjectDocumentFormatter = TestHelper.CreateAndRegisterMock<ITeamProjectDocumentFormatter>(this.container);
            this.mockTeamProjectDocumentVerifier = TestHelper.CreateAndRegisterMock<ITeamProjectDocumentVerifier>(this.container);
            this.mockLayoutDefinitionFormatter = TestHelper.CreateAndRegisterMock<ILayoutDefinitionFormatter>(this.container);
            
            this.mockFactory = TestHelper.CreateAndRegisterMock<IFactory>(this.container);

            this.sut = this.container.Resolve<TeamProjectDocument>();
        }

        /// <summary>
        /// Cleans up after each test.
        /// </summary>
        [TestCleanup]
        public void CleanupTest()
        {
            this.container.Dispose();
            this.sut.Dispose();
        }

        /// <summary>
        /// Constructor throws exception if null team project template is passed.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void ConstructorChecksTeamProjectTemplateForNull()
        {
            // Arrange, Act and Assert
            TestHelper.TestForArgumentNullException(
                () =>
                {
                    using (new TeamProjectDocument(container, null, this.mockTeamProjectDocumentFormatter.Object, this.mockLayoutDefinitionFormatter.Object, this.mockTeamProjectDocumentVerifier.Object, this.mockFactory.Object))
                    {
                    }
                },
                "template");
        }

        /// <summary>
        /// Constructor throws exception if null formatter is passed.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void ConstructorChecksFormatterForNull()
        {
            // Arrange, Act and Assert
            TestHelper.TestForArgumentNullException(
                () =>
                {
                    using (new TeamProjectDocument(container, this.mockTeamProjectTemplate.Object, null, this.mockLayoutDefinitionFormatter.Object, this.mockTeamProjectDocumentVerifier.Object, this.mockFactory.Object))
                    {
                    }
                },
                "formatter");
        }

        /// <summary>
        /// Constructor throws exception if null layout definition formatter is passed.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void ConstructorChecksLayoutDefinitionFormatterForNull()
        {
            // Arrange, Act and Assert
            TestHelper.TestForArgumentNullException(
                () =>
                {
                    using (new TeamProjectDocument(container, this.mockTeamProjectTemplate.Object, this.mockTeamProjectDocumentFormatter.Object, null, this.mockTeamProjectDocumentVerifier.Object, this.mockFactory.Object))
                    {
                    }
                },
                "layoutDefinitionFormatter");
        }

        /// <summary>
        /// Constructor throws exception if null verifier is passed.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void ConstructorChecksVerifierForNull()
        {
            // Arrange, Act and Assert
            TestHelper.TestForArgumentNullException(
                () =>
                {
                    using (new TeamProjectDocument(container, this.mockTeamProjectTemplate.Object, this.mockTeamProjectDocumentFormatter.Object, this.mockLayoutDefinitionFormatter.Object, null, this.mockFactory.Object))
                    {
                    }
                },
                "verifier");
        }

        /// <summary>
        /// Constructor throws exception if null factory is passed.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void ConstructorChecksFactoryForNull()
        {
            // Arrange, Act and Assert
            TestHelper.TestForArgumentNullException(
                () =>
                {
                    using (new TeamProjectDocument(container, this.mockTeamProjectTemplate.Object, this.mockTeamProjectDocumentFormatter.Object, this.mockLayoutDefinitionFormatter.Object, this.mockTeamProjectDocumentVerifier.Object, null))
                    {
                    }
                },
                "factory");
        }

        /// <summary>
        /// Setter for the <see cref="TeamProjectDocument.WordDocument"/> property checks for null
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void WordDocumentSetterChecksForNull()
        {
            // Arrange, Act and Assert
            TestHelper.TestForArgumentNullException(() => this.sut.WordDocument = null, "value");
        }

        /// <summary>
        /// Test that cannot add queries and layouts if <see cref="TeamProjectDocument.TeamProject"/> has not been set.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void CannotAddQueriesAndLayoutsIfTeamProjectPropertyHasNotBeenSet()
        {
            TestHelper.TestForInvalidOperationException(() => this.sut.AddQueryAndLayout(TestHelper.ValidQueryDefinitionAndLayout), "Team Project has not been set.");
        }

        /// <summary>
        /// Team Project Document must be disposable.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void TeamProjectDocumentIsDisposable()
        {
            using (this.sut)
            {
            }
        }

        /// <summary>
        /// Disposing the Team Project Document disposes the word document.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void DisposingAlsoDisposesTheWordDocument()
        {
            // Act
            this.sut.Dispose();

            // Assert
            this.mockWordDocument.Verify(document => document.Dispose(), Times.Once());
        }

        /// <summary>
        /// Disposing the Team Project Document disposes the  team project.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void DisposingAlsoDisposesTheTeamProject()
        {
            // Arrange
            this.SetupValidConnectedDocument();
            Mock<ITeamProject> mockTeamProject = this.SetupDocumentWithMockFactory();
            this.sut.Load(TestHelper.DummyRebindCallback);

            // Act
            this.sut.Dispose();

            // Assert
            mockTeamProject.Verify(teamProject => teamProject.Dispose(), Times.Once());
        }

        /// <summary>
        /// Can construct a new Team Project Document for the same document if the previous one was disposed.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void CanConstructSecondTeamProjectDocumentForTheSameDocumentIfPreviousOneWasDisposed()
        {
            this.sut.Dispose();
            using (ITeamProjectDocument secondObject = this.container.Resolve<TeamProjectDocument>())
            {
            }
        }

        /// <summary>
        /// Test that <see cref="TeamProjectDocument.TeamProject"/> is <c>null</c> if it has not been set and if <see cref="TeamProjectDocument.Load"/> has not been called first.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void TeamProjectNullIfLoadNotCalledAndNotSet()
        {
            // Arrange
            this.SetupValidConnectedDocument();

            // Act and Assert
            Assert.IsNull(this.sut.TeamProject, "TeamProject should not be set.");
        }

        /// <summary>
        /// Test that <see cref="TeamProjectDocument.SaveTeamProject"/> throws exception if <see cref="TeamProjectDocument.TeamProject"/> is null.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void SaveTeamProjectThrowsExceptionIfTeamProjectIsNull()
        {
            // Arrange, Act and Assert
            TestHelper.TestForInvalidOperationException(() => this.sut.SaveTeamProject(), "Team Project has not been set.");
        }

        /// <summary>
        /// Test that the <see cref="TeamProjectDocument.SaveTeamProject"/> saves the data in the expected Custom XML.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void SaveTeamProjectSavesTheDataInTheExpectedCustomXML()
        {
            // Arrange
            string actualXml = string.Empty;
            this.mockWordDocument.Setup(wordDoc => wordDoc.AddXmlPart(TestHelper.IsAnyXmlDocumentInNamespace(Constants.ProjectInformationNamespace))).Callback((string xml) => actualXml = xml);

            using (ITeamProject project = TestHelper.CreateMockTeamProject().Object)
            {
                this.sut.TeamProject = project;

                // Act
                this.sut.SaveTeamProject();
            }

            // Assert
            Assert.AreEqual<string>(TestHelper.ValidProjectXml, actualXml, "Saved XML does not match the expected XML");
        }

        /// <summary>
        /// Test that the <see cref="TeamProjectDocument.SaveTeamProject"/> deletes any existing XML before saving.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void SaveTeamProjectDeletesBeforeSaving()
        {
            // Arrange
            int callSequence = 1;
            this.mockWordDocument.Setup(wordDoc => wordDoc.DeleteXmlPart(Constants.ProjectInformationNamespace)).Callback(() => Assert.AreEqual<int>(1, callSequence++, "Delete must come first"));
            this.mockWordDocument.Setup(wordDoc => wordDoc.AddXmlPart(TestHelper.ValidProjectXml)).Callback(() => Assert.AreEqual<int>(2, callSequence++, "Add must come after delete"));

            using (ITeamProject project = TestHelper.CreateMockTeamProject().Object)
            {
                this.sut.TeamProject = project;

                // Act
                this.sut.SaveTeamProject();
            }

            // Assert
            this.mockWordDocument.Verify(wordDoc => wordDoc.DeleteXmlPart(Constants.ProjectInformationNamespace), Times.Once());
            this.mockWordDocument.Verify(wordDoc => wordDoc.AddXmlPart(TestHelper.ValidProjectXml), Times.Once());
        }

        /// <summary>
        /// Test that the <see cref="TeamProjectDocument.SaveQueriesAndLayouts"/> saves the data in the expected Custom XML.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void SaveQueriesAndLayoutsSavesTheDataInTheExpectedCustomXML()
        {
            // Arrange
            this.SetupDocumentWithASavedTeamProject();
            string actualXml = string.Empty;
            this.mockWordDocument.Setup(wordDoc => wordDoc.AddXmlPart(TestHelper.IsAnyXmlDocumentInNamespace(Constants.QueryNamespace))).Callback((string xml) => actualXml = xml);
            this.sut.AddQueryAndLayout(TestHelper.ValidQueryDefinitionAndLayout);

            // Act
            this.sut.SaveQueriesAndLayouts();

            // Assert
            TestHelper.AssertXmlEqual(TestHelper.ValidQueryAndLayoutXml, actualXml, "Saved XML does not match the expected XML");
        }

        /// <summary>
        /// Test that the <see cref="TeamProjectDocument.SaveQueriesAndLayouts"/> saves multiple queries and layouts data in the expected Custom XML.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void SaveQueriesAndLayoutsSavesMultipleQueriesAndLayoutsInTheExpectedCustomXML()
        {
            // Arrange
            this.SetupDocumentWithASavedTeamProject();
            string actualXml = string.Empty;
            this.mockWordDocument.Setup(wordDoc => wordDoc.AddXmlPart(TestHelper.IsAnyXmlDocumentInNamespace(Constants.QueryNamespace))).Callback((string xml) => actualXml = xml);
            this.sut.AddQueryAndLayout(TestHelper.ValidQueryDefinitionAndLayout);
            this.sut.AddQueryAndLayout(TestHelper.ValidQueryDefinitionAndLayout2);

            // Act
            this.sut.SaveQueriesAndLayouts();

            // Assert
            TestHelper.AssertXmlEqual(TestHelper.ValidQueryAndLayoutXmlMultiple, actualXml, "Saved XML does not match the expected XML for multiple queries and layouts");
        }

        /// <summary>
        /// Test that the <see cref="TeamProjectDocument.AddQueryAndLayout"/> deletes any existing XML before saving.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void SaveQueryAndLayoutDeletesBeforeSaving()
        {
            // Arrange
            this.SetupDocumentWithASavedTeamProject();
            bool deleted = false;
            bool added = false;
            this.mockWordDocument.Setup(wordDoc => wordDoc.DeleteXmlPart(Constants.QueryNamespace)).Callback(() => deleted = true);
            this.mockWordDocument.Setup(wordDoc => wordDoc.AddXmlPart(TestHelper.IsAnyXmlDocumentInNamespace(Constants.QueryNamespace))).Callback(() => { added = true; Assert.IsTrue(deleted, "Delete not called before add"); });

            this.sut.AddQueryAndLayout(TestHelper.ValidQueryDefinitionAndLayout);

            // Act
            this.sut.SaveQueriesAndLayouts();

            // Assert
            Assert.IsTrue(added, "The query and layout was not saved.");
        }

        /// <summary>
        /// Test that <see cref="TeamProjectDocument.SaveWorkItems"/> saves the work items in the expected Custom XML.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void SaveWorkItemsSavesTheWorkItemsInTheExpectedCustomXML()
        {
            // Arrange
            string[] fields = new string[] { "A.B", "A.C" };
            string actualXml = string.Empty;
            this.mockWordDocument.Setup(wordDoc => wordDoc.AddXmlPart(TestHelper.IsAnyXmlDocumentInNamespace(Constants.WorkItemNamespace))).Callback((string xml) => actualXml = xml);
            ITfsWorkItem mockItem1 = TestHelper.CreateMockWorkItem(1, new Tuple<string, object>("A.B", "ab1"), new Tuple<string, object>("A.C", "ac1"));
            ITfsWorkItem mockItem2 = TestHelper.CreateMockWorkItem(2, new Tuple<string, object>("A.B", "ab2"), new Tuple<string, object>("A.C", "ac2"));
            WorkItemTree workItems = new WorkItemTree();
            TestHelper.PopulateWorkItemTreeWithFlatQueryResults(workItems, mockItem1, mockItem2);
            XElement mockItem1Element = TfsWorkItem.Serialize(mockItem1, fields);
            XElement mockItem2Element = TfsWorkItem.Serialize(mockItem2, fields);

            // Act
            this.sut.SaveWorkItems(workItems, fields, new CancellationToken());

            // Assert
            TestHelper.AssertXmlEqual(TestHelper.ValidWorkItemsXml(mockItem1Element, mockItem2Element), actualXml, "Saved XML does not match the expected XML");
        }

        /// <summary>
        /// Test that <see cref="TeamProjectDocument.SaveWorkItems"/> saves the query work item data in the expected Custom XML.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void SaveWorkItemsSavesTheQueryWorkItemDataInTheExpectedCustomXML()
        {
            // Arrange
            string actualXml = string.Empty;
            this.mockWordDocument.Setup(wordDoc => wordDoc.AddXmlPart(TestHelper.IsAnyXmlDocumentInNamespace(Constants.QueryWorkItemNamespace))).Callback((string xml) => actualXml = xml);

            WorkItemTree workItems1 = new WorkItemTree();
            TestHelper.PopulateWorkItemTreeWithFlatQueryResults(workItems1, 1, 2);

            WorkItemTree workItems2 = new WorkItemTree();
            TestHelper.PopulateWorkItemTreeWithFlatQueryResults(workItems2, 3, 1, 2);

            // Act
            this.sut.SaveWorkItems(workItems1, new string[0], new CancellationToken());
            this.sut.SaveWorkItems(workItems2, new string[0], new CancellationToken());

            // Assert
            string expectedXml = TestHelper.ValidQueryWorkItemsXml(new int[] { 1, 2 }, new int[] { 3, 1, 2 });
            TestHelper.AssertXmlEqual(expectedXml, actualXml, "Saved XML does not match the expected XML");
        }

        /// <summary>
        /// Test that <see cref="TeamProjectDocument.SaveWorkItems"/> merges work items from the document with new work items.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void SaveWorkItemsMergesWorkItemsFromTheDocumentWithNewWorkItems()
        {
            // Arrange
            string savedXml = string.Empty;
            this.mockWordDocument.Setup(wordDoc => wordDoc.AddXmlPart(TestHelper.IsAnyXmlDocumentInNamespace(Constants.WorkItemNamespace))).Callback((string xml) => savedXml = xml);

            ITfsWorkItem savedItem1 = TestHelper.CreateMockWorkItem("Task", 1, new Tuple<string, object>("A.B", "hello"));
            ITfsWorkItem savedItem2 = TestHelper.CreateMockWorkItem("Test", 2);
            this.SetupValidConnectedDocument(savedItem1, savedItem2);
            this.SetupDocumentWithMockFactory();

            string[] fields = new string[] { "A.B", "A.C" };
            ITfsWorkItem newItem2 = TestHelper.CreateMockWorkItem(2, new Tuple<string, object>("A.B", "ab1"), new Tuple<string, object>("A.C", "ac1"));
            ITfsWorkItem newItem3 = TestHelper.CreateMockWorkItem(3, new Tuple<string, object>("A.B", "ab2"), new Tuple<string, object>("A.C", "ac2"));
            WorkItemTree workItems = new WorkItemTree();
            TestHelper.PopulateWorkItemTreeWithFlatQueryResults(workItems, newItem2, newItem3);
            XElement newItem2Element = TfsWorkItem.Serialize(newItem2, fields);

            // Act
            this.sut.Load(TestHelper.DummyRebindCallback);
            this.sut.SaveWorkItems(workItems, fields, new CancellationToken());

            // Assert
            XElement root = XElement.Parse(savedXml);
            XNamespace ns = Constants.WorkItemNamespace;
            Assert.AreEqual<int>(3, root.Elements(ns + "WorkItem").Count(), "Saved and new work items not merged");
        }

        /// <summary>
        /// Test that <see cref="TeamProjectDocument.SaveWorkItems"/> checks for a null work item tree.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void SaveWorkItemsChecksWorkItemsForNull()
        {
            // Arrange, Act and Assert
            TestHelper.TestForArgumentNullException(() => this.sut.SaveWorkItems(null, new string[0], new CancellationToken()), "workItems");
        }

        /// <summary>
        /// Test that <see cref="TeamProjectDocument.SaveWorkItems"/> deletes any existing work item data before saving.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void SaveWorkItemsDeletesWorkItemsBeforeSaving()
        {
            // Arrange
            int callSequence = 1;
            this.mockWordDocument.Setup(wordDoc => wordDoc.DeleteXmlPart(Constants.WorkItemNamespace)).Callback(() => Assert.AreEqual<int>(1, callSequence++, "Delete must come first"));
            this.mockWordDocument.Setup(wordDoc => wordDoc.AddXmlPart(TestHelper.IsAnyXmlDocumentInNamespace(Constants.WorkItemNamespace))).Callback(() => Assert.AreEqual<int>(2, callSequence++, "Add must come after delete"));

            // Act
            this.sut.SaveWorkItems(new WorkItemTree(), new string[0], new CancellationToken());

            // Assert
            this.mockWordDocument.Verify(wordDoc => wordDoc.DeleteXmlPart(Constants.WorkItemNamespace), Times.Once());
            this.mockWordDocument.Verify(wordDoc => wordDoc.AddXmlPart(TestHelper.IsAnyXmlDocumentInNamespace(Constants.WorkItemNamespace)), Times.Once());
        }

        /// <summary>
        /// Test that <see cref="TeamProjectDocument.SaveWorkItems"/> deletes any existing query work item data before saving.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void SaveWorkItemsDeletesQueryWorkItemsBeforeSaving()
        {
            // Arrange
            int callSequence = 1;
            this.mockWordDocument.Setup(wordDoc => wordDoc.DeleteXmlPart(Constants.QueryWorkItemNamespace)).Callback(() => Assert.AreEqual<int>(1, callSequence++, "Delete must come first"));
            this.mockWordDocument.Setup(wordDoc => wordDoc.AddXmlPart(TestHelper.IsAnyXmlDocumentInNamespace(Constants.QueryWorkItemNamespace))).Callback(() => Assert.AreEqual<int>(2, callSequence++, "Add must come after delete"));

            // Act
            this.sut.SaveWorkItems(new WorkItemTree(), new string[0], new CancellationToken());

            // Assert
            this.mockWordDocument.Verify(wordDoc => wordDoc.DeleteXmlPart(Constants.QueryWorkItemNamespace), Times.Once());
            this.mockWordDocument.Verify(wordDoc => wordDoc.AddXmlPart(TestHelper.IsAnyXmlDocumentInNamespace(Constants.QueryWorkItemNamespace)), Times.Once());
        }

        /// <summary>
        /// Test that <see cref="TeamProjectDocument.SaveWorkItems"/> can be cancelled.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void SaveWorkItemsCanBeCancelled()
        {
            // Arrange
            CancellationTokenSource cts = new CancellationTokenSource();
            ITfsWorkItem mockItem1 = TestHelper.CreateMockWorkItem(1, new Tuple<string, object>("A.B", "ab1"), new Tuple<string, object>("A.C", "ac1"));
            WorkItemTree workItems = new WorkItemTree();
            TestHelper.PopulateWorkItemTreeWithFlatQueryResults(workItems, mockItem1);

            // Act
            cts.Cancel();
            global::System.Threading.Tasks.Task t = global::System.Threading.Tasks.Task.Factory.StartNew(() => this.sut.SaveWorkItems(workItems, new string[0], cts.Token));

            // Assert
            TestHelper.AssertTaskCancelled(t);
        }

        /// <summary>
        /// Test that <see cref="TeamProjectDocument.SaveWorkItems"/> saves the query work item data even if the overall save is cancelled.
        /// </summary>
        /// <remarks>
        /// To avoid a situation where the query and layout information has more entries in it than the query to work item data association data we must save the query to work item
        /// association data even if the save is cancelled.
        /// </remarks>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void SaveWorkItemsSavesTheQueryWorkItemDataEvenIfOverallSaveIsCancelled()
        {
            // Arrange
            this.mockWordDocument.Setup(wordDoc => wordDoc.AddXmlPart(TestHelper.IsAnyXmlDocumentInNamespace(Constants.QueryWorkItemNamespace)));
            CancellationTokenSource cts = new CancellationTokenSource();
            ITfsWorkItem mockItem1 = TestHelper.CreateMockWorkItem(1, new Tuple<string, object>("A.B", "ab1"), new Tuple<string, object>("A.C", "ac1"));
            WorkItemTree workItems = new WorkItemTree();
            TestHelper.PopulateWorkItemTreeWithFlatQueryResults(workItems, mockItem1);

            // Act
            cts.Cancel();
            global::System.Threading.Tasks.Task t = global::System.Threading.Tasks.Task.Factory.StartNew(() => this.sut.SaveWorkItems(workItems, new string[0], cts.Token));

            // Assert
            TestHelper.AssertTaskCancelled(t);
            this.mockWordDocument.Verify(wordDoc => wordDoc.AddXmlPart(TestHelper.IsAnyXmlDocumentInNamespace(Constants.QueryWorkItemNamespace)), Times.Once(), "Query Work Item association data should be saved even when the save is cancelled");
        }

        /// <summary>
        /// Tests that document is not considered connected if after construction.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void NotConnectedAfterConstruction()
        {
             Assert.IsFalse(this.sut.IsConnected, "Should not be considered connected after construction");
        }

        /// <summary>
        /// Tests that document is considered connected if the project information has been saved.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void ConnectedIfProjectInformationHasBeenSaved()
        {
            // Arrange
            this.mockWordDocument.Setup(wordDoc => wordDoc.AddXmlPart(TestHelper.IsAnyXmlDocumentInNamespace(Constants.ProjectInformationNamespace)));

            // Act
            using (ITeamProject project = TestHelper.CreateMockTeamProject().Object)
            {
                this.sut.TeamProject = project;
                this.sut.SaveTeamProject();
            }

            // Assert
            Assert.IsTrue(this.sut.IsConnected, "Should be considered connected after saving project information");
        }

        /// <summary>
        /// Tests that document raises the <see cref="ITeamProjectDocument.Connected"/> event when the document is connected.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void ConnectedEventRaisedIfProjectInformationHasBeenSaved()
        {
            // Arrange
            this.mockWordDocument.Setup(wordDoc => wordDoc.AddXmlPart(TestHelper.IsAnyXmlDocumentInNamespace(Constants.ProjectInformationNamespace)));
            bool raised = false;
            this.sut.Connected += (s, e) => raised = true;

            // Act
            using (ITeamProject project = TestHelper.CreateMockTeamProject().Object)
            {
                this.sut.TeamProject = project;
                this.sut.SaveTeamProject();
            }

            // Assert
            Assert.IsTrue(raised, "Should have raised connected event after saving project information");
        }

        /// <summary>
        /// Tests that document is not considered connected if the project information has been set but not saved.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void NotConnectedIfProjectInformationSetButNotSaved()
        {
            // Arrange
            this.mockWordDocument.Setup(wordDoc => wordDoc.AddXmlPart(TestHelper.IsAnyXmlDocumentInNamespace(Constants.ProjectInformationNamespace)));

            // Act
            using (ITeamProject project = TestHelper.CreateMockTeamProject().Object)
            {
                this.sut.TeamProject = project;
            }

            // Assert
            Assert.IsFalse(this.sut.IsConnected, "Should not be considered connected after setting project information but before saving it.");
        }

        /// <summary>
        /// Tests that project information is loaded from the word document by <see cref="TeamProjectDocument.Load"/>.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void LoadGetsProjectInformationFromTheWordDocument()
        {
            // Arrange
            this.SetupValidConnectedDocument();
            this.SetupDocumentWithMockFactory();

            // Act
            this.sut.Load(TestHelper.DummyRebindCallback);

            // Assert
            this.mockWordDocument.Verify(wordDoc => wordDoc.GetXmlPart(Constants.ProjectInformationNamespace), Times.Once());
        }

        /// <summary>
        /// Test that <see cref="TeamProjectDocument.Load"/> does not set <see cref="TeamProjectDocument.TeamProject"/> if the document is not connected.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void LoadDoesNotSetTeamProjectForDisconnectedDocument()
        {
            // Arrange
            this.SetupDisconnectedDocument();

            // Act
            this.sut.Load(TestHelper.DummyRebindCallback);

            // Assert
            Assert.IsNull(this.sut.TeamProject, "Should not retrieve a team project when the document is not connected.");
        }

        /// <summary>
        /// Test that <see cref="TeamProjectDocument.Load"/> does not set <see cref="TeamProjectDocument.TeamProject"/> if the document contains invalid project information data.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void LoadsDoesNotSetTeamProjecForDocumentWithInvalidProjectInformation()
        {
            // Arrange
            this.SetupConnectedDocument("non-xml data", TestHelper.ValidQueryAndLayoutXml, TestHelper.ValidWorkItemsXml(), TestHelper.ValidQueryWorkItemsXml());
            this.SetupDocumentWithMockFactory();

            // Act
            this.sut.Load(TestHelper.DummyRebindCallback);

            // Assert
            ITeamProject ans = this.sut.TeamProject;
            Assert.IsNull(ans, "Should not retrieve a team project when the project information in the document is not valid.");
        }

        /// <summary>
        /// Test that cannot add queries and layouts after calling <see cref="TeamProjectDocument.Load"/> if the document is not connected.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void CannotAddQueriesAndLayoutsAfterCallingLoadOnADisconnectedDocument()
        {
            // Arrange
            this.SetupDisconnectedDocument();

            // Act
            this.sut.Load(TestHelper.DummyRebindCallback);

            // Assert
            TestHelper.TestForInvalidOperationException(() => this.sut.AddQueryAndLayout(TestHelper.ValidQueryDefinitionAndLayout), "Team Project has not been set.");
        }

        /// <summary>
        /// Test that cannot add queries and layouts after calling <see cref="TeamProjectDocument.Load"/> if the document contains invalid query and layout data.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void LoadCannotAddQueriesAndLayoutsAfterCallingLoadOnDocumentWithInvalidProjectInformation()
        {
            // Arrange
            this.SetupConnectedDocument(TestHelper.ValidProjectCollectionUri, "non-xml data", TestHelper.ValidWorkItemsXml(), TestHelper.ValidQueryWorkItemsXml());

            // Act
            this.sut.Load(TestHelper.DummyRebindCallback);

            // Assert
            TestHelper.TestForInvalidOperationException(() => this.sut.AddQueryAndLayout(TestHelper.ValidQueryDefinitionAndLayout), "Team Project has not been set.");
        }

        /// <summary>
        /// Tests that document is connected if project information is loaded from the word document by <see cref="TeamProjectDocument.Load"/>.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void LoadOfValidProjectInformationFromTheWordDocumentCausesDocumentToBeConsideredConnected()
        {
            // Arrange
            this.SetupValidConnectedDocument();
            this.SetupDocumentWithMockFactory();

            // Act
            this.sut.Load(TestHelper.DummyRebindCallback);

            // Assert
            Assert.IsTrue(this.sut.IsConnected, "Should be considered connected after loading valid project information from document.");
        }

        /// <summary>
        /// Tests that project information is loaded from the word document by <see cref="TeamProjectDocument.Load"/> only once.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void LoadGetsProjectInformationFromTheWordDocumentOnlyOnce()
        {
            // Arrange
            this.SetupValidConnectedDocument();
            this.SetupDocumentWithMockFactory();

            // Act
            this.sut.Load(TestHelper.DummyRebindCallback);
            this.sut.Load(TestHelper.DummyRebindCallback);

            // Assert
            this.mockWordDocument.Verify(wordDoc => wordDoc.GetXmlPart(Constants.ProjectInformationNamespace), Times.Once());
        }

        /// <summary>
        /// Tests that query is loaded from the word document by <see cref="TeamProjectDocument.Load"/>.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void LoadGetsQueryFromTheWordDocument()
        {
            // Arrange
            this.SetupValidConnectedDocument();
            this.SetupDocumentWithMockFactory();

            // Act
            this.sut.Load(TestHelper.DummyRebindCallback);

            // Assert
            this.mockWordDocument.Verify(wordDoc => wordDoc.GetXmlPart(Constants.QueryNamespace), Times.Once());
        }

        /// <summary>
        /// Tests that query is loaded from the word document by <see cref="TeamProjectDocument.Load"/> only once.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void LoadGetsQueryFromTheWordDocumentOnlyOnce()
        {
            // Arrange
            this.SetupValidConnectedDocument();
            this.SetupDocumentWithMockFactory();

            // Act
            this.sut.Load(TestHelper.DummyRebindCallback);
            this.sut.Load(TestHelper.DummyRebindCallback);

            // Assert
            this.mockWordDocument.Verify(wordDoc => wordDoc.GetXmlPart(Constants.QueryNamespace), Times.Once());
        }

        /// <summary>
        /// Tests that loading project information alone makes the document connected.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void LoadingProjectInformationAloneMakesTheDocumentConnected()
        {
            // Arrange
            this.SetupDocumentWithMockFactory();
            this.SetupConnectedDocument(TestHelper.ValidProjectXml, string.Empty, string.Empty, string.Empty);

            // Act
            this.sut.Load(TestHelper.DummyRebindCallback);

            // Assert
            Assert.IsTrue(this.sut.IsConnected, "Document should be connected if no query and layout information is in the document");
        }

        /// <summary>
        /// Tests that loading query alone does not make the document connected.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void LoadingQueryAloneDoesNotMakeTheDocumentConnected()
        {
            // Arrange
            this.SetupConnectedDocument(string.Empty, TestHelper.ValidQueryAndLayoutXml, TestHelper.ValidWorkItemsXml(), TestHelper.ValidQueryWorkItemsXml());
            this.SetupDocumentWithMockFactory();

            // Act
            this.sut.Load(TestHelper.DummyRebindCallback);

            // Assert
            Assert.IsFalse(this.sut.IsConnected, "Document should not be connected if project information is missing from the document load");
        }

        /// <summary>
        /// Test that <see cref="TeamProjectDocument.Load"/> loads the query and layout information from the document and the template.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void LoadReadsQueryAndLayoutInformationFromTheDocumentAndTemplate()
        {
            // Arrange
            this.SetupValidConnectedDocumentMultiple();
            LayoutInformation[] layouts = this.SetupTemplateLayouts(TestHelper.ValidLayoutName, TestHelper.ValidLayoutName2);
            this.SetupDocumentWithMockFactory();

            // Act
            this.sut.Load(TestHelper.DummyRebindCallback);
            QueryAndLayoutInformation[] ans = this.sut.QueriesAndLayouts.ToArray();

            // Assert
            Assert.AreEqual<int>(2, ans.Length, "There should be two query and layout entries");

            Assert.AreEqual<string>(TestHelper.ValidQuery, ans[0].Query.QueryText, "The query was not retrieved from the word document");
            Assert.AreSame(layouts[0], ans[0].Layout, "The layout was not retrieved from the word document");

            Assert.AreEqual<string>(TestHelper.ValidQuery2, ans[1].Query.QueryText, "The query was not retrieved from the word document");
            Assert.AreSame(layouts[1], ans[1].Layout, "The layout was not retrieved from the word document");
        }

        /// <summary>
        /// Test that <see cref="TeamProjectDocument.Load"/> returns a warning if the template is missing a layout referenced by the document being loaded.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void LoadReportsWarningIfDocumentIsMissingAReferencedLayout()
        {
            // Arrange
            this.SetupValidConnectedDocumentMultiple();
            this.SetupTemplateLayouts(TestHelper.ValidLayoutName);
            this.SetupDocumentWithMockFactory();

            // Act
            string[] warnings = this.sut.Load(TestHelper.DummyRebindCallback).ToArray();

            // Assert
            Assert.AreEqual<int>(1, warnings.Length, "There should be one warning.");
            Assert.AreEqual<string>("The definition of the " + TestHelper.ValidLayoutName2 + " layout is missing, the document will not be refreshable", warnings[0], "The expected warning was not produced");
        }

        /// <summary>
        /// Test that <see cref="TeamProjectDocument.Load"/> a layout with no fields or building blocks if the template is missing a layout referenced by the document being loaded.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void LoadReturnsEmptyLayoutIfDocumentIsMissingAReferencedLayout()
        {
            // Arrange
            this.SetupValidConnectedDocument();
            this.SetupTemplateLayouts(TestHelper.ValidLayoutName2); // cause mismatch in layout name
            this.SetupDocumentWithMockFactory();

            // Act
            this.sut.Load(TestHelper.DummyRebindCallback).ToArray();
            QueryAndLayoutInformation[] ans = this.sut.QueriesAndLayouts.ToArray();

            // Assert
            Assert.AreEqual<int>(1, ans.Length, "There should still be a query and layout.");
            Assert.AreEqual<int>(0, ans[0].Layout.BuildingBlockNames.Count(), "There should not be any building blocks in the layout");
            Assert.AreEqual<int>(0, ans[0].Layout.FieldNames.Count(), "There should not be any field names in the layout");
        }

        /// <summary>
        /// Test that <see cref="TeamProjectDocument.Load"/> loads the query and layout information from the document and preserves the query and layout and indices.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void LoadReadsQueryAndLayoutInformationFromTheDocumentPreservingQueryAndLayoutIndices()
        {
            // Arrange
            this.SetupValidConnectedDocumentMultiple();
            this.SetupTemplateLayouts(TestHelper.ValidLayoutName, TestHelper.ValidLayoutName2);
            this.SetupDocumentWithMockFactory();

            // Act
            this.sut.Load(TestHelper.DummyRebindCallback);
            QueryAndLayoutInformation[] ans = this.sut.QueriesAndLayouts.ToArray();

            // Assert
            Assert.AreEqual<int>(0, ans.Where(qli => qli.Query.QueryText == TestHelper.ValidQuery).Single().Index);
            Assert.AreEqual<int>(1, ans.Where(qli => qli.Query.QueryText == TestHelper.ValidQuery2).Single().Index);
        }

        /// <summary>
        /// Tests that <see cref="TeamProjectDocument.Load"/> does not load any work items if there is no work item data.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void LoadDoesNotLoadAnyWorkItemsIfThereIsNoWorkItemData()
        {
            // Arrange
            string savedXml = string.Empty;
            this.mockWordDocument.Setup(wordDoc => wordDoc.AddXmlPart(TestHelper.IsAnyXmlDocumentInNamespace(Constants.WorkItemNamespace))).Callback((string xml) => savedXml = xml);
            this.SetupConnectedDocument(TestHelper.ValidProjectXml, TestHelper.ValidQueryAndLayoutXml, null, null);
            this.SetupTemplateWithValidLayout();
            this.SetupDocumentWithMockFactory();

            // Act
            this.sut.Load(TestHelper.DummyRebindCallback);
            this.SaveExistingWorkItemsOnly();

            // Assert
            TestHelper.AssertXmlEqual(TestHelper.ValidWorkItemsXml(), savedXml, "Expected no work items to be saved.");
        }

        /// <summary>
        /// Test that <see cref="TeamProjectDocument.Load"/> does not load any work items if the document contains non-XML work item data.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void LoadDoesNotLoadAnyWorkItemsForDocumentWithNonXmlWorkItemData()
        {
            // Arrange
            string savedXml = string.Empty;
            this.mockWordDocument.Setup(wordDoc => wordDoc.AddXmlPart(It.IsAny<string>())).Callback((string xml) => savedXml = xml);
            this.SetupConnectedDocument(TestHelper.ValidProjectXml, TestHelper.ValidQueryAndLayoutXml, "non-xml data", null);
            this.SetupTemplateWithValidLayout();
            this.SetupDocumentWithMockFactory();

            // Act
            this.sut.Load(TestHelper.DummyRebindCallback);
            this.SaveExistingWorkItemsOnly();

            // Assert
            TestHelper.AssertXmlEqual(TestHelper.ValidWorkItemsXml(), savedXml, "Expected no work items to be saved.");
        }

        /// <summary>
        /// Test that <see cref="TeamProjectDocument.Load"/> does not load any work items if the document contains work item data with a bad field name.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void LoadDoesNotLoadWorkItemsForDocumentWithBadFieldName()
        {
            // Arrange
            string savedXml = string.Empty;
            this.mockWordDocument.Setup(wordDoc => wordDoc.AddXmlPart(TestHelper.IsAnyXmlDocumentInNamespace(Constants.WorkItemNamespace))).Callback((string xml) => savedXml = xml);
            string workItemXml = TestHelper.ValidWorkItemsXml(TfsWorkItem.Serialize(TestHelper.CreateMockWorkItem("Task", 1, new Tuple<string, object>("A.B", "test")), "A.B")).Replace("A.B", "1a");
            this.SetupConnectedDocument(TestHelper.ValidProjectXml, TestHelper.ValidQueryAndLayoutXml, workItemXml, TestHelper.ValidQueryWorkItemsXml());
            this.SetupTemplateWithValidLayout();
            this.SetupDocumentWithMockFactory();

            // Act
            this.sut.Load(TestHelper.DummyRebindCallback);
            this.SaveExistingWorkItemsOnly();

            // Assert
            TestHelper.AssertXmlEqual(TestHelper.ValidWorkItemsXml(), savedXml, "Expected no work items to be saved.");
        }

        /// <summary>
        /// Tests that <see cref="TeamProjectDocument.Load"/> loads existing work items in the word document.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void LoadLoadsExistingWorkItemsFromTheWordDocument()
        {
            // Arrange
            string savedXml = string.Empty;
            this.mockWordDocument.Setup(wordDoc => wordDoc.AddXmlPart(TestHelper.IsAnyXmlDocumentInNamespace(Constants.WorkItemNamespace))).Callback((string xml) => savedXml = xml);
            ITfsWorkItem item1 = TestHelper.CreateMockWorkItem("Task", 1, new Tuple<string, object>("A.B", "hello"));
            ITfsWorkItem item2 = TestHelper.CreateMockWorkItem("Test", 2);
            this.SetupValidConnectedDocument(item1, item2);
            this.SetupDocumentWithMockFactory();

            // Act
            this.sut.Load(TestHelper.DummyRebindCallback);
            this.SaveExistingWorkItemsOnly();

            // Assert
            TestHelper.AssertXmlEqual(TestHelper.ValidWorkItemsXml(TfsWorkItem.Serialize(item1), TfsWorkItem.Serialize(item2)), savedXml, "Expected work items already in the document to be saved.");
        }

        /// <summary>
        /// Tests that work item data is loaded from the word document by <see cref="TeamProjectDocument.Load"/> only once.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void LoadGetsWorkItemDataFromTheWordDocumentOnlyOnce()
        {
            // Arrange
            this.SetupValidConnectedDocument();
            this.SetupDocumentWithMockFactory();

            // Act
            this.sut.Load(TestHelper.DummyRebindCallback);
            this.sut.Load(TestHelper.DummyRebindCallback);

            // Assert
            this.mockWordDocument.Verify(wordDoc => wordDoc.GetXmlPart(Constants.WorkItemNamespace), Times.Once());
        }

        /// <summary>
        /// Tests that <see cref="TeamProjectDocument.Load"/> does not load any query and work item association data if there is no such data.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void LoadDoesNotLoadAnyQueryAndWorkItemAssociationDataIfThereIsNoSuchData()
        {
            // Arrange
            this.SetupConnectedDocument(TestHelper.ValidProjectXml, TestHelper.ValidQueryAndLayoutXml, TestHelper.ValidWorkItemsXml(), null);
            this.SetupTemplateWithValidLayout();
            this.SetupDocumentWithMockFactory();

            // Act
            this.sut.Load(TestHelper.DummyRebindCallback);

            // Assert
            Assert.AreEqual<int>(0, this.sut.QueryWorkItems.Count());
        }

        /// <summary>
        /// Test that <see cref="TeamProjectDocument.Load"/> does not load any query and work item association data if the document contains non-XML query and work item association data.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void LoadDoesNotLoadAnyQueryAndWorkItemAssociationDataForDocumentWithNonXmlQueryWorkItemData()
        {
            // Arrange
            this.SetupConnectedDocument(TestHelper.ValidProjectXml, TestHelper.ValidQueryAndLayoutXml, TestHelper.ValidWorkItemsXml(), "non-xml data");
            this.SetupTemplateWithValidLayout();
            this.SetupDocumentWithMockFactory();

            // Act
            this.sut.Load(TestHelper.DummyRebindCallback);

            // Assert
            Assert.AreEqual<int>(0, this.sut.QueryWorkItems.Count());
        }

        /// <summary>
        /// Test that <see cref="TeamProjectDocument.Load"/> does not load any query and work item association data if the document contains invalid query and work item association data.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void LoadDoesNotLoadQueryAndWorkItemAssociationDataForDocumentWithInvalidQueryAndWorkItemAssociationData()
        {
            // Arrange
            string badData = @"<Queries xmlns='" + Constants.QueryWorkItemNamespace + "'><Query><WorkItems><WorkItem Id='1'/><rubbish/></WorkItems></Query></Queries>";
            this.SetupConnectedDocument(TestHelper.ValidProjectXml, TestHelper.ValidQueryAndLayoutXml, TestHelper.ValidWorkItemsXml(), badData);
            this.SetupTemplateWithValidLayout();
            this.SetupDocumentWithMockFactory();

            // Act
            this.sut.Load(TestHelper.DummyRebindCallback);

            // Assert
            Assert.AreEqual<int>(0, this.sut.QueryWorkItems.Count());
        }

        /// <summary>
        /// Tests that <see cref="TeamProjectDocument.Load"/> loads existing query and work item association data in the word document.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void LoadLoadsExistingQueryAndWorkItemAssociationDataFromTheWordDocument()
        {
            // Arrange
            this.SetupConnectedDocument(TestHelper.ValidProjectXml, TestHelper.ValidQueryAndLayoutXml, TestHelper.ValidWorkItemsXml(), TestHelper.ValidQueryWorkItemsXml(new int[] { 1, 2, 3 }, new int[] { 3, 2, 1 }));
            this.SetupTemplateWithValidLayout();
            this.SetupDocumentWithMockFactory();

            // Act
            this.sut.Load(TestHelper.DummyRebindCallback);
 
            // Assert
            QueryWorkItems[] ans = this.sut.QueryWorkItems.ToArray();
            Assert.AreEqual<int>(2, ans.Length, "There should be one entry per saved query.");

            Assert.AreEqual<int>(0, ans[0].QueryIndex, "Queries not returned in order");
            Assert.AreEqual<int>(1, ans[1].QueryIndex, "Queries not returned in order");

            Assert.AreEqual<int>(3, ans[0].WorkItemIds.Count(), "There should be two work items for the first query.");
            Assert.AreEqual<int>(1, ans[0].WorkItemIds.ToArray()[0], "Wrong work item id or not in order");
            Assert.AreEqual<int>(2, ans[0].WorkItemIds.ToArray()[1], "Wrong work item id or not in order");
            Assert.AreEqual<int>(3, ans[0].WorkItemIds.ToArray()[2], "Wrong work item id or not in order");

            Assert.AreEqual<int>(3, ans[1].WorkItemIds.Count(), "There should be three work items for the second query.");
            Assert.AreEqual<int>(3, ans[1].WorkItemIds.ToArray()[0], "Wrong work item id or not in order");
            Assert.AreEqual<int>(2, ans[1].WorkItemIds.ToArray()[1], "Wrong work item id or not in order");
            Assert.AreEqual<int>(1, ans[1].WorkItemIds.ToArray()[2], "Wrong work item id or not in order");
        }

        /// <summary>
        /// Setting <see cref="TeamProjectDocument.TeamProject"/> creates query and layout manager, passing it the project's fields.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void SettingTeamProjectCreatesQueryAndLayoutManagerInstancePassingItProjectFields()
        {
            // Arrange
            Mock<ITeamProject> mockTeamProject = this.SetupDocumentWithMockFactory();
            ITfsFieldDefinition[] fields = TestHelper.CreateMockFieldDefinitions("system.id");
            mockTeamProject.Setup(tp => tp.FieldDefinitions).Returns(fields);

            // Act
            this.sut.TeamProject = mockTeamProject.Object;

            // Assert
            this.mockFactory.Verify(factory => factory.CreateQueryAndLayoutManager(fields), Times.Once());
        }

        /// <summary>
        /// Setting <see cref="TeamProjectDocument.TeamProject"/> to <c>null</c> prevents adding queries and layouts.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void SettingTeamProjectToNullPreventsAddingQueriesAndLayouts()
        {
            // Arrange
            Mock<ITeamProject> mockTeamProject = this.SetupDocumentWithMockFactory();
            ITfsFieldDefinition[] fields = TestHelper.CreateMockFieldDefinitions("system.id");
            mockTeamProject.Setup(tp => tp.FieldDefinitions).Returns(fields);
            this.sut.TeamProject = mockTeamProject.Object;

            // Act
            this.sut.TeamProject = null;

            // Assert
            TestHelper.TestForInvalidOperationException(() => this.sut.AddQueryAndLayout(TestHelper.ValidQueryDefinitionAndLayout), "Team Project has not been set.");
        }

        /// <summary>
        /// Test that <see cref="TeamProjectDocument.Load"/> reads the team project information loaded from the document.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void TeamProjectReadsTeamProjectInformationLoadedFromTheDocument()
        {
            // Arrange
            this.SetupValidConnectedDocument();
            this.SetupDocumentWithMockFactory();

            // Act
            this.sut.Load(TestHelper.DummyRebindCallback);

            // Assert
            Assert.AreEqual<Uri>(new Uri(TestHelper.ValidProjectCollectionUri), this.sut.TeamProject.TeamProjectInformation.CollectionUri, "The team project collection URI was not retrieved from the team project document");
            Assert.AreEqual<string>(TestHelper.ValidProjectName, this.sut.TeamProject.TeamProjectInformation.ProjectName, "The project name was not retrieved from the team project document");
        }

        /// <summary>
        /// <see cref="TeamProjectDocument.Load"/> creates query and layout manager, passing it the project's fields.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void LoadCreatesQueryAndLayoutManagerInstancePassingItProjectFields()
        {
            // Arrange
            this.SetupValidConnectedDocument();
            Mock<ITeamProject> mockTeamProject = this.SetupDocumentWithMockFactory();
            ITfsFieldDefinition[] fields = TestHelper.CreateMockFieldDefinitions("system.id");
            mockTeamProject.Setup(tp => tp.FieldDefinitions).Returns(fields);

            // Act
            this.sut.Load(TestHelper.DummyRebindCallback);

            // Assert
            this.mockFactory.Verify(factory => factory.CreateQueryAndLayoutManager(fields), Times.Once());
        }

        /// <summary>
        /// Non-xml project information causes document to be considered not connected.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void LoadNonXmlProjectInformationMakesDocumentDisconnected()
        {
            // Arrange
            this.SetupConnectedDocument("non-xml data", TestHelper.ValidQueryAndLayoutXml, TestHelper.ValidWorkItemsXml(), TestHelper.ValidQueryWorkItemsXml());
            this.SetupDocumentWithMockFactory();

            // Act
            this.sut.Load(TestHelper.DummyRebindCallback);

            // Assert
            Assert.IsFalse(this.sut.IsConnected, "Document should not be connected if project information is not XML.");
        }

        /// <summary>
        /// Missing namespace in project information XML causes document to be disconnected.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void LoadMissingNamespaceInProjectInformationMakesDocumentDisconnected()
        {
            // Arrange
            this.SetupConnectedDocument(
                string.Format(CultureInfo.InvariantCulture, @"<Project CollectionUri=""{0}"" CollectionId=""{1}"" ProjectName=""{2}""/>", TestHelper.ValidProjectCollectionUri, TestHelper.ValidProjectCollectionId, TestHelper.ValidProjectName),
                TestHelper.ValidQuery,
                TestHelper.ValidWorkItemsXml(),
                TestHelper.ValidQueryWorkItemsXml());

            // Act
            this.sut.Load(TestHelper.DummyRebindCallback);

            // Assert
            Assert.IsFalse(this.sut.IsConnected, "Document should not be connected if project information is missing the namespace.");
        }

        /// <summary>
        /// Missing Uri in project information XML causes document to be disconnected.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void LoadMissingUriInProjectInformationMakesDocumentDisconnected()
        {
            // Arrange
            this.SetupConnectedDocument(
                string.Format(CultureInfo.InvariantCulture, @"<Project CollectionId=""{0}"" ProjectName=""{1}"" xmlns=""{2}"" />", TestHelper.ValidProjectCollectionId, TestHelper.ValidProjectName, Constants.ProjectInformationNamespace),
                TestHelper.ValidQuery,
                TestHelper.ValidWorkItemsXml(),
                TestHelper.ValidQueryWorkItemsXml());

            // Act
            this.sut.Load(TestHelper.DummyRebindCallback);

            // Assert
            Assert.IsFalse(this.sut.IsConnected, "Document should not be connected if project information is missing the URI.");
        }

        /// <summary>
        /// Blank Uri in project information XML causes document to be disconnected.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void LoadBlankUriInProjectInformationMakesDocumentDisconnected()
        {
            // Arrange
            this.SetupConnectedDocument(
                string.Format(CultureInfo.InvariantCulture, @"<Project CollectionUri= """" CollectionId=""{0}"" ProjectName=""{1}"" xmlns=""{2}"" />", TestHelper.ValidProjectCollectionId, TestHelper.ValidProjectName, Constants.ProjectInformationNamespace),
                TestHelper.ValidQuery,
                TestHelper.ValidWorkItemsXml(),
                TestHelper.ValidQueryWorkItemsXml());

            // Act
            this.sut.Load(TestHelper.DummyRebindCallback);

            // Assert
            Assert.IsFalse(this.sut.IsConnected, "Document should not be connected if project information contains a blank URI.");
        }

        /// <summary>
        /// Invalid Uri in project information XML causes document to be disconnected.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void LoadInvalidUriInProjectInformationMakesDocumentDisconnected()
        {
            // Arrange
            this.SetupConnectedDocument(
                string.Format(CultureInfo.InvariantCulture, @"<Project CollectionUri= "" "" CollectionId=""{0}"" ProjectName=""{1}"" xmlns=""{2}"" />", TestHelper.ValidProjectCollectionId, TestHelper.ValidProjectName, Constants.ProjectInformationNamespace),
                TestHelper.ValidQuery,
                TestHelper.ValidWorkItemsXml(),
                TestHelper.ValidQueryWorkItemsXml());

            // Act
            this.sut.Load(TestHelper.DummyRebindCallback);

            // Assert
            Assert.IsFalse(this.sut.IsConnected, "Document should not be connected if project information contains an invalid URI.");
        }

        /// <summary>
        /// Missing collection id in project information XML causes document to be disconnected.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void LoadMissingCollectionIdInProjectInformationMakesDocumentDisconnected()
        {
            // Arrange
            this.SetupConnectedDocument(
                string.Format(CultureInfo.InvariantCulture, @"<Project CollectionUri=""{0}"" ProjectName=""{1}"" xmlns=""{2}"" />", TestHelper.ValidProjectCollectionUri, TestHelper.ValidProjectName, Constants.ProjectInformationNamespace),
                TestHelper.ValidQuery,
                TestHelper.ValidWorkItemsXml(),
                TestHelper.ValidQueryWorkItemsXml());

            // Act
            this.sut.Load(TestHelper.DummyRebindCallback);

            // Assert
            Assert.IsFalse(this.sut.IsConnected, "Document should not be connected if project information is missing the collection id.");
        }

        /// <summary>
        /// Blank collection id in project information XML causes document to be disconnected.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void LoadBlankCollectionIdInProjectInformationMakesDocumentDisconnected()
        {
            // Arrange
            this.SetupConnectedDocument(
                string.Format(CultureInfo.InvariantCulture, @"<Project CollectionUri=""{0}"" CollectionId="""" ProjectName=""{1}"" xmlns=""{2}"" />", TestHelper.ValidProjectCollectionUri, TestHelper.ValidProjectName, Constants.ProjectInformationNamespace),
                TestHelper.ValidQuery,
                TestHelper.ValidWorkItemsXml(),
                TestHelper.ValidQueryWorkItemsXml());

            // Act
            this.sut.Load(TestHelper.DummyRebindCallback);

            // Assert
            Assert.IsFalse(this.sut.IsConnected, "Document should not be connected if project information contains a blank collection id.");
        }

        /// <summary>
        /// Invalid collection id in project information XML causes document to be disconnected.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void LoadInvalidCollectionIdInProjectInformationMakesDocumentDisconnected()
        {
            // Arrange
            this.SetupConnectedDocument(
                string.Format(CultureInfo.InvariantCulture, @"<Project CollectionUri=""{0}"" CollectionId=""X"" ProjectName=""{1}"" xmlns=""{2}"" />", TestHelper.ValidProjectCollectionUri, TestHelper.ValidProjectName, Constants.ProjectInformationNamespace),
                TestHelper.ValidQuery,
                TestHelper.ValidWorkItemsXml(),
                TestHelper.ValidQueryWorkItemsXml());

            // Act
            this.sut.Load(TestHelper.DummyRebindCallback);

            // Assert
            Assert.IsFalse(this.sut.IsConnected, "Document should not be connected if project information contains an invalid collection id.");
        }

        /// <summary>
        /// Missing project name in project information XML causes document to be disconnected.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void LoadMissingProjectNameInProjectInformationMakesDocumentDisconnected()
        {
            // Arrange
            this.SetupConnectedDocument(
                string.Format(CultureInfo.InvariantCulture, @"<Project CollectionUri=""{0}"" CollectionId=""{1}"" xmlns=""{2}"" />", TestHelper.ValidProjectCollectionUri, TestHelper.ValidProjectCollectionId, Constants.ProjectInformationNamespace),
                TestHelper.ValidQuery,
                TestHelper.ValidWorkItemsXml(),
                TestHelper.ValidQueryWorkItemsXml());

            // Act
            this.sut.Load(TestHelper.DummyRebindCallback);

            // Assert
            Assert.IsFalse(this.sut.IsConnected, "Document should not be connected if project information is missing the project name.");
        }

        /// <summary>
        /// Blank project name in project information XML causes document to be disconnected.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void LoadBlankProjectNameInProjectInformationMakesDocumentDisconnected()
        {
            // Arrange
            this.SetupConnectedDocument(
                string.Format(CultureInfo.InvariantCulture, @"<Project CollectionUri=""{0}"" CollectionId=""{1}"" ProjectName="""" xmlns=""{2}"" />", TestHelper.ValidProjectCollectionUri, TestHelper.ValidProjectCollectionId, Constants.ProjectInformationNamespace),
                TestHelper.ValidQuery,
                TestHelper.ValidWorkItemsXml(),
                TestHelper.ValidQueryWorkItemsXml());

            // Act
            this.sut.Load(TestHelper.DummyRebindCallback);

            // Assert
            Assert.IsFalse(this.sut.IsConnected, "Document should not be connected if project information is the project name is blank.");
        }

        /// <summary>
        /// Non-xml query is not loaded.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void LoadNonXmlQueryIsNotLoaded()
        {
            // Arrange
            this.SetupDocumentWithMockFactory();
            this.SetupConnectedDocument(TestHelper.ValidProjectXml, "non-xml data", TestHelper.ValidWorkItemsXml(), TestHelper.ValidQueryWorkItemsXml());

            // Act
            this.sut.Load(TestHelper.DummyRebindCallback);

            // Assert
            this.AssertNoQueriesOrLayoutsLoaded();
        }

        /// <summary>
        /// Query and layout data not conforming to schema causes QueryAndLayoutManager to be empty.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void LoadInvalidQueryAndLayoutDataMakesQueryAndLayoutManagerEmpty()
        {
            // Arrange
            this.SetupDocumentWithMockFactory();
            this.SetupConnectedDocument(TestHelper.ValidProjectXml, TestHelper.ValidQueryAndLayoutXml.Replace("LayoutName", "BadLayoutName"), TestHelper.ValidWorkItemsXml(), TestHelper.ValidQueryWorkItemsXml());

            // Act
            this.sut.Load(TestHelper.DummyRebindCallback);

            // Assert
            this.AssertNoQueriesOrLayoutsLoaded();
        }

        /// <summary>
        /// Missing namespace causes QueryAndLayoutManager to be empty.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void LoadMissingNamespaceInQueryMakesQueryAndLayoutManagerEmpty()
        {
            // Arrange
            this.SetupDocumentWithMockFactory();
            this.SetupConnectedDocument(
                TestHelper.ValidProjectXml,
                string.Format(CultureInfo.InvariantCulture, @"<Query>""{0}""</Query>", TestHelper.ValidQuery),
                TestHelper.ValidWorkItemsXml(),
                TestHelper.ValidQueryWorkItemsXml());

            // Act
            this.sut.Load(TestHelper.DummyRebindCallback);

            // Assert
            this.AssertNoQueriesOrLayoutsLoaded();
        }

        /// <summary>
        /// Missing query text in query XML causes QueryAndLayoutManager to be empty.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void LoadMissingQueryTextInQueryMakesQueryAndLayoutManagerEmpty()
        {
            // Arrange
            this.SetupDocumentWithMockFactory();
            this.SetupConnectedDocument(
                TestHelper.ValidProjectXml,
                string.Format(CultureInfo.InvariantCulture, @"<Query xmlns=""{0}""></Query>", Constants.QueryNamespace),
                TestHelper.ValidWorkItemsXml(),
                TestHelper.ValidQueryWorkItemsXml());

            // Act
            this.sut.Load(TestHelper.DummyRebindCallback);

            // Assert
            this.AssertNoQueriesOrLayoutsLoaded();
        }       

        /// <summary>
        /// Test that <see cref="TeamProjectDocument.MapWorkItemsIntoDocument"/> throws an exception if layout argument is null.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void MapWorksItemsIntoDocumentChecksForNullLayout()
        {
            // Arrange, Act and Assert
            TestHelper.TestForArgumentNullException(() => this.sut.MapWorkItemsIntoDocument(null, 1, new CancellationToken()), "layout");
        }

        /// <summary>
        /// Test that <see cref="TeamProjectDocument.MapWorkItemsIntoDocument"/> throws an exception if <see cref="TeamProjectDocument.SaveWorkItems"/> has not been called.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void MapWorkItemsIntoDocumentThrowsExceptionIfWorkItemsHaveNotBeenSaved()
        {
            // Arrange
            LayoutInformation testLayout = TestHelper.CreateTestLayoutInformation(new BuildingBlockName[0]);

            // Act and Assert
            TestHelper.TestForInvalidOperationException(() => this.sut.MapWorkItemsIntoDocument(testLayout, 1, new CancellationToken()), "Work items must be saved into the document first.");
        }

        /// <summary>
        /// Test that <see cref="TeamProjectDocument.MapWorkItemsIntoDocument"/> invokes the <see cref="TeamProjectDocument.TeamProjectDocumentFormatter"/> class.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void MapWorkItemsIntoDocumentInvokesTheTeamProjectDocumentFormatterClassWithFunctionThatCreatesCorrectFormatForBookmark()
        {
            // Arrange
            this.SetupValidConnectedDocument();
            WorkItemTree workItems = new WorkItemTree();
            ITfsWorkItem[] mockItems = TestHelper.PopulateWorkItemTreeWithFlatQueryResults(workItems, 1);
            this.sut.SaveWorkItems(workItems, new string[0], new CancellationToken());
            LayoutInformation testLayout = TestHelper.CreateTestLayoutInformation(new BuildingBlockName[0]);
            string actualBookmark = string.Empty;
            this.mockTeamProjectDocumentFormatter
                .Setup(formatter => formatter.MapWorkItemsIntoDocument(workItems, testLayout, It.IsAny<Func<int, string>>(), It.IsAny<CancellationToken>()))
                .Callback((WorkItemTree wit, LayoutInformation li, Func<int, string> bookmarkFunc, CancellationToken ct) => actualBookmark = bookmarkFunc(mockItems[0].Id));

            // Act
            this.sut.MapWorkItemsIntoDocument(testLayout, 1, new CancellationToken());

            // Assert
            this.mockTeamProjectDocumentFormatter.Verify(formatter => formatter.MapWorkItemsIntoDocument(workItems, testLayout, It.IsAny<Func<int, string>>(), It.IsAny<CancellationToken>()), Times.Once(), "The formatter was not invoked to map the work items.");
            Assert.AreEqual<string>("TFS_WI_Q1_W1", actualBookmark, "Bookmark name not calculated correctly");
        }

        /// <summary>
        /// Test that <see cref="TeamProjectDocument.MapWorkItemsIntoDocument"/> can be cancelled.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void MapWorkItemsIntoDocumentCanBeCancelled()
        {
            // no test body because this calls through to the formatter, the formatter is already tested for cancellation. This is here to show that this scenario
            // was considered.
        }

        /// <summary>
        /// Test that <see cref="TeamProjectDocument.RefreshWorkItems"/> throws exception if <see cref="TeamProjectDocument.TeamProject"/> is null.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void RefreshWorkItemsThrowsExceptionIfTeamProjectIsNull()
        {
            TestHelper.TestForInvalidOperationException(() => this.sut.RefreshWorkItems(new CancellationToken()), "Team Project has not been set.");
        }

        /// <summary>
        /// Tests that <see cref="TeamProjectDocument.RefreshWorkItems"/> saves the work items returned by all the queries into the document.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void RefreshWorkItemsSavesTheWorkItemsReturnedByAllTheQueriesInTheDocument()
        {
            // Arrange
            Tuple<QueryAndLayoutInformation[], ITfsWorkItem[]> info = this.SetupDocumentWithTwoWorkItemsInTwoQueries();

            // Act
            this.sut.RefreshWorkItems(new CancellationToken());

            // Assert
            this.mockWordDocument.Verify(wordDoc => wordDoc.AddXmlPart(TestHelper.IsAnyXmlDocumentInNamespace(Constants.WorkItemNamespace)), Times.Once(), "Should only save work items once.");
            this.mockWordDocument.Verify(wordDoc => wordDoc.AddXmlPart(TestHelper.ValidWorkItemsXml(TfsWorkItem.Serialize(info.Item2[0]), TfsWorkItem.Serialize(info.Item2[1]))), Times.Once(), "Not all queries executed");
        }

        /// <summary>
        /// Tests that <see cref="TeamProjectDocument.RefreshWorkItems"/> uses the final queries returned by the <see cref="QueryAndLayoutManager"/>.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void RefreshWorkItemsUsesFinalQueries()
        {
            // Arrange
            Tuple<QueryAndLayoutInformation[], ITfsWorkItem[]> info = this.SetupDocumentWithTwoWorkItemsInTwoQueries();
            Assert.AreSame(info.Item1[0].Query, this.mockQueryAndLayoutManager.Object.OriginalQueriesAndLayouts.First().Query, "Pre-requisite is that original query from query and layout manager is same query definition");
            Assert.AreSame(info.Item1[1].Query, this.mockQueryAndLayoutManager.Object.OriginalQueriesAndLayouts.Skip(1).First().Query, "Pre-requisite is that original query from query and layout manager is same query definition");
            Assert.AreNotSame(info.Item1[0].Query, this.mockQueryAndLayoutManager.Object.FinalQueriesAndLayouts.First().Query, "Pre-requisite is that final query from query and layout manager is not same query definition");
            Assert.AreNotSame(info.Item1[1].Query, this.mockQueryAndLayoutManager.Object.FinalQueriesAndLayouts.Skip(1).First().Query, "Pre-requisite is that final query from query and layout manager is not same query definition");

            // Act
            this.sut.RefreshWorkItems(new CancellationToken());

            // Assert
            this.mockTeamProjectQuery.Verify(projectQuery => projectQuery.QueryForWorkItems(info.Item1[0].Query, It.IsAny<CancellationToken>()), Times.Never(), "Did not use final query");
            this.mockTeamProjectQuery.Verify(projectQuery => projectQuery.QueryForWorkItems(info.Item1[1].Query, It.IsAny<CancellationToken>()), Times.Never(), "Did not use final query");
        }

        /// <summary>
        /// Tests that <see cref="TeamProjectDocument.RefreshWorkItems"/> replaces the query and work item association data.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void RefreshWorkItemsReplacesQueryAndWorkItemAssociationData()
        {
            // Arrange
            this.SetupDocumentWithTwoWorkItemsInTwoQueries();
            this.sut.RefreshWorkItems(new CancellationToken());
            Assert.AreEqual<int>(2, this.sut.QueryWorkItems.Count(), "Pre-requisite is for two queries");
            Assert.AreEqual<int>(1, this.sut.QueryWorkItems.ToArray()[1].WorkItemIds.Count(), "Pre-requisite, there should be  work items for the second query.");
            this.mockTeamProjectQuery.Setup(projectQuery => projectQuery.QueryForWorkItems(TestHelper.IsQueryText(TestHelper.ValidQueryDefinitionAndLayout2.Query.QueryText), It.IsAny<CancellationToken>())).Returns(new WorkItemTree());

            // Act
            this.sut.RefreshWorkItems(new CancellationToken());

            // Assert
            QueryWorkItems[] ans = this.sut.QueryWorkItems.ToArray();
            Assert.AreEqual<int>(2, ans.Length, "Should remain at two queries");

            Assert.AreEqual<int>(0, ans[0].QueryIndex, "Queries not returned in order");
            Assert.AreEqual<int>(1, ans[1].QueryIndex, "Queries not returned in order");

            Assert.AreEqual<int>(1, ans[0].WorkItemIds.Count(), "There should be one work item for the first query.");
            Assert.AreEqual<int>(1, ans[0].WorkItemIds.ToArray()[0], "Wrong work item id");

            Assert.AreEqual<int>(0, ans[1].WorkItemIds.Count(), "There should be no work items for the second query.");
        }

        /// <summary>
        /// Tests that <see cref="TeamProjectDocument.RefreshWorkItems"/> passes before and after query and work item association data to the formatter.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void RefreshWorkItemsPassesBeforeAndAfterQueryAndWorkItemAssociationDataToTheFormatter()
        {
            // Arrange
            this.SetupDocumentWithTwoWorkItemsInTwoQueries();
            this.sut.RefreshWorkItems(new CancellationToken());
            Assert.AreEqual<int>(2, this.sut.QueryWorkItems.Count(), "Pre-requisite is for two queries");
            Assert.AreEqual<int>(1, this.sut.QueryWorkItems.ToArray()[1].WorkItemIds.Count(), "Pre-requisite, there should be  work items for the second query.");
            this.mockTeamProjectQuery.Setup(projectQuery => projectQuery.QueryForWorkItems(TestHelper.IsQueryText(TestHelper.ValidQueryDefinitionAndLayout2.Query.QueryText), It.IsAny<CancellationToken>())).Returns(new WorkItemTree());
            IEnumerable<QueryWorkItems> capturedBefore = null;
            IEnumerable<QueryWorkItems> capturedAfter = null;
            CancellationToken cancellationToken = new CancellationToken();
            this.mockTeamProjectDocumentFormatter
                .Setup(formatter => formatter.RefreshWorkItems(It.IsAny<FormatterRefreshData>(), It.IsAny<Func<int, int, string>>(), cancellationToken))
                .Callback((FormatterRefreshData refreshData, Func<int, int, string> func, CancellationToken ct) => { capturedBefore = refreshData.QueryWorkItemsBefore; capturedAfter = refreshData.QueryWorkItemsAfter; });

            // Act
            this.sut.RefreshWorkItems(cancellationToken);

            // Assert
            Assert.AreNotSame(capturedAfter, capturedBefore, "Before and after should not be the same object");

            Assert.AreEqual<int>(2, capturedBefore.Count(), "Before data should contain information for two queries");
            Assert.AreEqual<int>(1, capturedBefore.ToArray()[0].WorkItemIds.Count(), "Before data should contain work items for first query.");
            Assert.AreEqual<int>(1, capturedBefore.ToArray()[0].WorkItemIds.ToArray()[0], "Before data should contain correct work item for first query.");
            Assert.AreEqual<int>(1, capturedBefore.ToArray()[1].WorkItemIds.Count(), "Before data should contain work items for second query.");
            Assert.AreEqual<int>(2, capturedBefore.ToArray()[1].WorkItemIds.ToArray()[0], "Before data should contain correct work item for second query.");

            Assert.AreEqual<int>(2, capturedAfter.Count(), "After data should still contain information for two queries");
            Assert.AreEqual<int>(1, capturedAfter.ToArray()[0].WorkItemIds.Count(), "After data should still contain work items for first query.");
            Assert.AreEqual<int>(1, capturedAfter.ToArray()[0].WorkItemIds.ToArray()[0], "After data should still contain same work item for first query.");
            Assert.AreEqual<int>(0, capturedAfter.ToArray()[1].WorkItemIds.Count(), "After data should not contain work items for second query.");
            Assert.IsNotNull(capturedAfter.ToArray()[0].WorkItemTreeNodes, "After query work item data must include the work item tree nodes.");
            Assert.AreEqual<int>(1, capturedAfter.ToArray()[0].WorkItemTreeNodes.Count(), "Correct nodes not passed for work item tree nodes in after query work item data");
            Assert.AreEqual<int>(capturedAfter.ToArray()[0].WorkItemIds.ToArray()[0], capturedAfter.ToArray()[0].WorkItemTreeNodes.ToArray()[0].WorkItem.Id, "Correct nodes not passed for work item tree nodes in after query work item data");
            Assert.IsNotNull(capturedAfter.ToArray()[1].WorkItemTreeNodes, "After query work item data must include the work item tree nodes.");
            Assert.AreEqual<int>(0, capturedAfter.ToArray()[1].WorkItemTreeNodes.Count(), "Correct nodes not passed for work item tree nodes in after query work item data");
        }

        /// <summary>
        /// Tests that <see cref="TeamProjectDocument.RefreshWorkItems"/> passes all required refresh data to the formatter.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void RefreshWorkItemsPassesAllRequiredRefreshDataToTheFormatter()
        {
            // Arrange
            this.SetupDocumentWithTwoWorkItemsInTwoQueries();
            this.sut.RefreshWorkItems(new CancellationToken());
            Assert.AreEqual<int>(2, this.sut.QueryWorkItems.Count(), "Pre-requisite is for two queries");
            Assert.AreEqual<int>(1, this.sut.QueryWorkItems.ToArray()[1].WorkItemIds.Count(), "Pre-requisite, there should be  work items for the second query.");
            this.mockTeamProjectQuery.Setup(projectQuery => projectQuery.QueryForWorkItems(TestHelper.IsQueryText(TestHelper.ValidQueryDefinitionAndLayout2.Query.QueryText), It.IsAny<CancellationToken>())).Returns(new WorkItemTree());
            FormatterRefreshData capturedRefreshData = null;
            CancellationToken cancellationToken = new CancellationToken();
            this.mockTeamProjectDocumentFormatter
                .Setup(formatter => formatter.RefreshWorkItems(It.IsAny<FormatterRefreshData>(), It.IsAny<Func<int, int, string>>(), cancellationToken))
                .Callback((FormatterRefreshData refreshData, Func<int, int, string> func, CancellationToken ct) => capturedRefreshData = refreshData);

            // Act
            this.sut.RefreshWorkItems(cancellationToken);

            // Assert
            Assert.IsNotNull(capturedRefreshData.WorkItemManager, "Missing refresh data");
            Assert.IsNotNull(capturedRefreshData.QueryWorkItemsBefore, "Missing refresh data");
            Assert.IsNotNull(capturedRefreshData.QueryWorkItemsAfter, "Missing refresh data");
            Assert.IsNotNull(capturedRefreshData.Layouts, "Missing refresh data");
        }

        /// <summary>
        /// Tests that <see cref="TeamProjectDocument.RefreshWorkItems"/> does not save work items if TFS is not connected.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void RefreshWorkItemsDoesNotSaveWorkItemsIfTFSIsNotConnected()
        {
            // Arrange
            this.SetupDocumentWithTwoWorkItemsInTwoQueries();
            this.mockTeamProjectQuery.Setup(queryRunner => queryRunner.QueryForWorkItems(It.IsAny<QueryDefinition>(), It.IsAny<CancellationToken>())).Throws(new TeamFoundationServerException("testmessage"));

            // Act
            try
            {
                this.sut.RefreshWorkItems(new CancellationToken());
            }
            catch (TeamFoundationServerException)
            {
            }

            // Assert
            this.mockWordDocument.Verify(wordDoc => wordDoc.AddXmlPart(TestHelper.IsAnyXmlDocumentInNamespace(Constants.WorkItemNamespace)), Times.Never(), "Should not have saved work items.");
        }

        /// <summary>
        /// Tests that <see cref="TeamProjectDocument.RefreshWorkItems"/> saves the work items using the combined field names from all the layouts.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void RefreshWorkItemsSavesWorkItemsUsingCombinedFieldNamesFromAllTheLayouts()
        {
            // Arrange
            this.SetupDocumentWithTwoWorkItemsInTwoQueries();
            this.mockQueryAndLayoutManager.Setup(qlm => qlm.AllLayoutFields).Returns(new string[] { Constants.SystemIdFieldReferenceName, "System.Area", "System.Title", "System.Description" });
            string actualXml = string.Empty;
            this.mockWordDocument.Setup(wordDoc => wordDoc.AddXmlPart(TestHelper.IsAnyXmlDocumentInNamespace(Constants.WorkItemNamespace))).Callback((string xml) => actualXml = xml);

            // Act
            this.sut.RefreshWorkItems(new CancellationToken());

            // Assert
            XDocument xdoc = XDocument.Parse(actualXml);
            Assert.AreEqual<int>(2, CountWorkItemsInXmlWithField(xdoc, Constants.SystemIdFieldReferenceName), "There should be 2 id fields in the document");
            Assert.AreEqual<int>(2, CountWorkItemsInXmlWithField(xdoc, "System.Area"), "There should be 2 area fields in the document");
            Assert.AreEqual<int>(2, CountWorkItemsInXmlWithField(xdoc, "System.Title"), "There should be 2 title fields in the document");
            Assert.AreEqual<int>(2, CountWorkItemsInXmlWithField(xdoc, "System.Description"), "There should be 2 description fields in the document");
        }

        /// <summary>
        /// Tests that <see cref="TeamProjectDocument.RefreshWorkItems"/> saves the query to work item association data.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void RefreshWorkItemsSavesQueryToWorkItemAssociationData()
        {
            // Arrange
            this.SetupDocumentWithTwoWorkItemsInTwoQueries();
            string actualXml = string.Empty;
            this.mockWordDocument.Setup(wordDoc => wordDoc.AddXmlPart(TestHelper.IsAnyXmlDocumentInNamespace(Constants.QueryWorkItemNamespace))).Callback((string xml) => actualXml = xml);

            // Act
            this.sut.RefreshWorkItems(new CancellationToken());

            // Assert
            string expectedXml = TestHelper.ValidQueryWorkItemsXml(new int[] { 1 }, new int[] { 2 });
            TestHelper.AssertXmlEqual(expectedXml, actualXml, "Saved XML does not match the expected XML");
        }

        /// <summary>
        /// Tests that <see cref="TeamProjectDocument.RefreshWorkItems"/> clears the work item list and saves only the new work items.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void RefreshWorkItemsClearsOldWorkItemsAndSavesOnlyNewWorkItems()
        {
            // Arrange
            this.SetupDocumentWithTwoWorkItemsInTwoQueries();
            string actualXml = string.Empty;
            this.mockWordDocument.Setup(wordDoc => wordDoc.AddXmlPart(TestHelper.IsAnyXmlDocumentInNamespace(Constants.WorkItemNamespace))).Callback((string xml) => actualXml = xml);
            this.sut.RefreshWorkItems(new CancellationToken());
            this.mockTeamProjectQuery.Setup(projectQuery => projectQuery.QueryForWorkItems(TestHelper.IsQueryText(TestHelper.ValidQueryDefinitionAndLayout2.Query.QueryText), It.IsAny<CancellationToken>())).Returns(new WorkItemTree());

            // Act
            this.sut.RefreshWorkItems(new CancellationToken());

            // Assert
            XDocument xdoc = XDocument.Parse(actualXml);
            Assert.AreEqual<int>(1, CountWorkItemsInXmlWithField(xdoc, Constants.SystemIdFieldReferenceName), "There should only be 1 work item saved after query results changed on second refresh.");
        }

        /// <summary>
        /// Test that <see cref="TeamProjectDocument.RefreshWorkItems"/> invokes the <see cref="TeamProjectDocumentFormatter.RefreshWorkItems"/> method on the combined work items.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void RefreshWorkItemsInvokesTheTeamProjectDocumentFormatterRefreshWorkItemsMethodOnTheCombinedWorkItems()
        {
            // Arrange
            Tuple<QueryAndLayoutInformation[], ITfsWorkItem[]> info = this.SetupDocumentWithTwoWorkItemsInTwoQueries();
            CancellationToken cancellationToken = new CancellationToken();
            FormatterRefreshData capturedRefreshData = null;
            this.mockTeamProjectDocumentFormatter
                .Setup(formatter => formatter.RefreshWorkItems(It.IsAny<FormatterRefreshData>(), It.IsAny<Func<int, int, string>>(), cancellationToken))
                .Callback((FormatterRefreshData refreshData, Func<int, int, string> func, CancellationToken ct) => capturedRefreshData = refreshData);

            // Act
            this.sut.RefreshWorkItems(cancellationToken);

            // Assert
            this.mockTeamProjectDocumentFormatter.Verify(formatter => formatter.RefreshWorkItems(It.IsAny<FormatterRefreshData>(), It.IsAny<Func<int, int, string>>(), cancellationToken), Times.Once(), "The formatter was not invoked to refresh the work items.");
            Assert.IsTrue(TestHelper.CheckUnorderedUniqueArrayMatch<ITfsWorkItem>(info.Item2, capturedRefreshData.WorkItemManager.WorkItems.ToArray()), "Formatter not called on combined work items.");
        }

        /// <summary>
        /// Test that <see cref="TeamProjectDocument.MapWorkItemsIntoDocument"/> can be cancelled.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void RefreshWorkItemsCanBeCancelled()
        {
            // Arrange
            this.SetupDocumentWithTwoWorkItemsInTwoQueries();
            CancellationTokenSource cts = new CancellationTokenSource();
            
            // Act
            cts.Cancel();
            global::System.Threading.Tasks.Task t = global::System.Threading.Tasks.Task.Factory.StartNew(() => this.sut.RefreshWorkItems(cts.Token));

            // Assert
            TestHelper.AssertTaskCancelled(t);
        }

        /// <summary>
        /// Test that <see cref="TeamProjectDocument.Refresh"/> succeeds if the document verifier says it is valid.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void RefreshSucceedsIfVerifierValidatesTheDocument()
        {
            Tuple<QueryAndLayoutInformation[], ITfsWorkItem[]> info = this.SetupDocumentWithTwoWorkItemsInTwoQueries();
            CancellationToken cancellationToken = new CancellationToken();
            this.mockTeamProjectDocumentVerifier.Setup(v => v.VerifyDocument(It.IsAny<IEnumerable<QueryWorkItems>>(), It.IsAny<Func<int, int, string>>(), It.IsAny<Func<string, Tuple<int, int>>>(), It.IsAny<Func<string, Nullable<int>>>()))
                                                .Returns(new string[0]);

            // Act
            IEnumerable<string> ans = this.sut.RefreshWorkItems(cancellationToken);

            // Assert
            Assert.AreEqual<int>(0, ans.Count(), "Should not return any errors");
        }

        /// <summary>
        /// Test that <see cref="TeamProjectDocument.Refresh"/> returns verification errors from the document verifier.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void RefreshReturnsVerificationErrorsFromTheVerifier()
        {
            Tuple<QueryAndLayoutInformation[], ITfsWorkItem[]> info = this.SetupDocumentWithTwoWorkItemsInTwoQueries();
            CancellationToken cancellationToken = new CancellationToken();
            string[] errors = new string[] { "error 1" };
            this.mockTeamProjectDocumentVerifier.Setup(v => v.VerifyDocument(It.IsAny<IEnumerable<QueryWorkItems>>(), It.IsAny<Func<int, int, string>>(), It.IsAny<Func<string, Tuple<int, int>>>(), It.IsAny<Func<string, Nullable<int>>>()))
                                                .Returns(errors);

            // Act
            IEnumerable<string> ans = this.sut.RefreshWorkItems(cancellationToken);

            // Assert
            Assert.AreSame(errors, ans, "Should return the errors returned by the verifier.");
        }

        /// <summary>
        /// Test that <see cref="TeamProjectDocument.Refresh"/> verification errors cause refresh to be aborted.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void RefreshVerificationErrorsCauseRefreshToBeAborted()
        {
            Tuple<QueryAndLayoutInformation[], ITfsWorkItem[]> info = this.SetupDocumentWithTwoWorkItemsInTwoQueries();
            CancellationToken cancellationToken = new CancellationToken();
            string[] errors = new string[] { "error 1" };
            this.mockTeamProjectDocumentVerifier.Setup(v => v.VerifyDocument(It.IsAny<IEnumerable<QueryWorkItems>>(), It.IsAny<Func<int, int, string>>(), It.IsAny<Func<string, Tuple<int, int>>>(), It.IsAny<Func<string, Nullable<int>>>()))
                                                .Returns(errors);

            // Act
            this.sut.RefreshWorkItems(cancellationToken);

            // Assert
            this.mockTeamProjectDocumentFormatter.Verify(f => f.RefreshWorkItems(It.IsAny<FormatterRefreshData>(), It.IsAny<Func<int, int, string>>(), It.IsAny<CancellationToken>()), Times.Never(), "Should not have run the formatter.");
            this.mockTeamProjectQuery.Verify(q => q.QueryForWorkItems(It.IsAny<QueryDefinition>(), It.IsAny<CancellationToken>()), Times.Never(), "Should not have queried for work items");
            this.mockWordDocument.Verify(doc => doc.AddXmlPart(TestHelper.IsAnyXmlDocumentInNamespace(Constants.WorkItemNamespace)), Times.Never(), "Should not have saved work items.");
        }

        /// <summary>
        /// Test that <see cref="TeamProjectDocument.QueryWorkItems"/> returns the work items returned by the queries.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void QueryWorkItemsReturnsTheWorkItemsReturnedByTheQueries()
        {
            // Arrange
            WorkItemTree workItems1 = new WorkItemTree();
            TestHelper.PopulateWorkItemTreeWithFlatQueryResults(workItems1, 1, 2);
            this.sut.SaveWorkItems(workItems1, new string[0], new CancellationToken());

            WorkItemTree workItems2 = new WorkItemTree();
            TestHelper.PopulateWorkItemTreeWithFlatQueryResults(workItems2, 3, 1, 2);
            this.sut.SaveWorkItems(workItems2, new string[0], new CancellationToken());

            // Act
            QueryWorkItems[] ans = this.sut.QueryWorkItems.ToArray();

            // Assert
            Assert.AreEqual<int>(2, ans.Length, "There should be one entry per query.");

            Assert.AreEqual<int>(0, ans[0].QueryIndex, "Queries not returned in order");
            Assert.AreEqual<int>(1, ans[1].QueryIndex, "Queries not returned in order");

            Assert.AreEqual<int>(2, ans[0].WorkItemIds.Count(), "There should be two work items for the first query.");
            Assert.AreEqual<int>(1, ans[0].WorkItemIds.ToArray()[0], "Wrong work item id or not in order");
            Assert.AreEqual<int>(2, ans[0].WorkItemIds.ToArray()[1], "Wrong work item id or not in order");

            Assert.AreEqual<int>(3, ans[1].WorkItemIds.Count(), "There should be three work items for the second query.");
            Assert.AreEqual<int>(3, ans[1].WorkItemIds.ToArray()[0], "Wrong work item id or not in order");
            Assert.AreEqual<int>(1, ans[1].WorkItemIds.ToArray()[1], "Wrong work item id or not in order");
            Assert.AreEqual<int>(2, ans[1].WorkItemIds.ToArray()[2], "Wrong work item id or not in order");
        }

        /// <summary>
        /// Test that <see cref="TeamProjectDocument.QueryWorkItems"/> returns the work items returned by a tree query in depth-first order.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void QueryWorkItemsReturnsTheWorkItemsReturnedByTreeQueryInDepthFirstOrder()
        {
            // Arrange
            WorkItemTree workItems1 = new WorkItemTree();
            TestHelper.PopulateWorkItemTreeWithHierarchicalQueryResults(workItems1, "1", "1.3", "2");
            this.sut.SaveWorkItems(workItems1, new string[0], new CancellationToken());

            // Act
            QueryWorkItems[] ans = this.sut.QueryWorkItems.ToArray();

            // Assert
            Assert.AreEqual<int>(1, ans[0].WorkItemIds.ToArray()[0], "Wrong work item id or not in order");
            Assert.AreEqual<int>(3, ans[0].WorkItemIds.ToArray()[1], "Wrong work item id or not in order");
            Assert.AreEqual<int>(2, ans[0].WorkItemIds.ToArray()[2], "Wrong work item id or not in order");
        }

        /// <summary>
        /// Test that <see cref="TeamProjectDocument.IsInsertable"/> returns insertable status of the document.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void IsInsertableReturnsInsertableStatusOfWordDocument()
        {
            this.mockWordDocument.Setup(doc => doc.IsInsertable()).Returns(false);
            Assert.IsFalse(this.sut.IsInsertable, "Team Project Document should not be insertable when the underlying document is not insertable");
            this.mockWordDocument.Setup(doc => doc.IsInsertable()).Returns(true);
            Assert.IsTrue(this.sut.IsInsertable, "Team Project Document should be insertable when the underlying document is insertable");
        }

        /// <summary>
        /// Test that <see cref="TeamProjectDocument.IsRefreshable"/> returns <c>false</c> if the document is not connected.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void IsRefreshableReturnsFalseIfDocumentIsNotConnected()
        {
            Assert.IsFalse(this.sut.IsConnected, "Pre-requisite");
            Assert.IsFalse(this.sut.IsRefreshable, "Team Project Document should not be refreshable when the underlying document is not connected");
        }

        /// <summary>
        /// Test that <see cref="TeamProjectDocument.IsRefreshable"/> returns <c>false</c> if the document is connected but has no queries.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void IsRefreshableReturnsFalseIfDocumentIsConnectedButHasNoQueries()
        {
            // Arrange
            this.SetupDocumentWithASavedTeamProject();

            // Act and Assert
            Assert.IsFalse(this.sut.IsRefreshable, "Team Project Document should not be refreshable when the underlying document is not connected");
        }

        /// <summary>
        /// Test that <see cref="TeamProjectDocument.IsRefreshable"/> returns <c>false</c> if the document is connected, contains a query and the layout but the template does not have the layout.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void IsRefreshableReturnsFalseIfDocumentIsConnectedHasQueryAndLayoutButNoLayoutInTemplate()
        {
            // Arrange
            this.SetupValidConnectedDocument();
            this.SetupTemplateLayouts(new string[0]); // replace list of layouts with no layouts.
            this.SetupDocumentWithMockFactory();
            this.sut.Load(TestHelper.DummyRebindCallback);

            // Act and Assert
            Assert.IsFalse(this.sut.IsRefreshable, "Team Project Document should not be refreshable when the document is valid but the template is missing or does not have the layouts used.");
        }

        /// <summary>
        /// Test that <see cref="TeamProjectDocument.IsRefreshable"/> returns <c>false</c> if the document is connected, contains a query and the layout and the template contains only some of the layouts used.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void IsRefreshableReturnsFalseIfDocumentIsConnectedHasQueryAndLayoutAndTemplateContainsSomeLayouts()
        {
            // Arrange
            this.SetupValidConnectedDocumentMultiple();
            this.SetupTemplateWithValidLayout(); // replace the return of both layouts with return of only one.
            this.SetupDocumentWithMockFactory();
            this.sut.Load(TestHelper.DummyRebindCallback);

            // Act and Assert
            Assert.IsFalse(this.sut.IsRefreshable, "Team Project Document should not be refreshable when the document is connected, contains a query and the layout and the template contains only some of the layouts used.");
        }

        /// <summary>
        /// Test that <see cref="TeamProjectDocument.IsRefreshable"/> returns <c>true</c> if the document is connected, contains a query and the layout and the template contains all the layouts used.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void IsRefreshableReturnsTrueIfDocumentIsConnectedHasQueryAndLayoutAndTemplateContainsAllLayouts()
        {
            // Arrange
            this.SetupValidConnectedDocumentMultiple();
            this.SetupTemplateLayouts(TestHelper.ValidLayoutName, TestHelper.ValidLayoutName2);
            this.SetupDocumentWithMockFactory();
            this.sut.Load(TestHelper.DummyRebindCallback);

            // Act and Assert
            Assert.IsTrue(this.sut.IsRefreshable, "Team Project Document should be refreshable when the document is connected, contains a query and the layout and the template contains all the layouts used.");
        }

        /// <summary>
        /// Test that <see cref="TeamProjectDocument.Load"/> calls the rebind callback if the team project collection id does not match the document.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void LoadRebindsIfCollectionIdMismatch()
        {
            this.SetupValidConnectedDocumentMultiple();
            this.SetupDocumentWithMockFactory(Guid.NewGuid()); // This guid differs from the standard test collection guid
            int rebindCalled = 0;

            // Act
            this.sut.Load(() =>
                              {
                                  rebindCalled++;
                                   if (rebindCalled > 2)
                                  {
                                      this.SetupDocumentWithMockFactory();
                                  }

                                  return new Uri(TestHelper.ValidProjectCollectionUri);
                              });

            // Assert
            Assert.AreEqual<int>(3, rebindCalled, "The rebind callback must be called until the collection id returned by the team project collection matches the expected one.");
        }

        /// <summary>
        /// Test that <see cref="TeamProjectDocument.Load"/> saves the new uri if the document is rebound.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void LoadUpdatesDocumentWithNewUriIfItRebinds()
        {
            this.SetupValidConnectedDocumentMultiple();
            this.SetupDocumentWithMockFactory(Guid.NewGuid()); // This guid differs from the standard test collection guid

            // Act
            this.sut.Load(() =>
            {
                this.SetupDocumentWithMockFactory();
                return new Uri(TestHelper.ValidProjectCollectionUri);
            });

            // Assert
            this.mockWordDocument.Verify(wordDoc => wordDoc.AddXmlPart(TestHelper.ValidProjectXml), Times.Once(), "Should save the new uri in the project info in the event of a rebind.");
        }

        /// <summary>
        /// Test that <see cref="TeamProjectDocument.Load"/> saves the new uri if the document is rebound.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void LoadLeavesDocumentDisconnectedIfRebindIsCancelled()
        {
            this.SetupValidConnectedDocumentMultiple();
            this.SetupDocumentWithMockFactory(Guid.NewGuid()); // This guid differs from the standard test collection guid

            // Act
            this.sut.Load(() => null);

            // Assert
            Assert.IsFalse(this.sut.IsConnected, "Document must be disconnected if the rebind is cancelled.");
        }

        /// <summary>
        /// Test that <see cref="TeamProjectDocument.DisplayLayoutDefinition"/> invokes the layout definition formatter to display the chosen layout definition.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void DisplayLayoutDefinitionInvokesLayoutDefinitionFormatterForChosenLayout()
        {
            // Act
            this.sut.DisplayLayoutDefinition("test");

            // Assert
            this.mockLayoutDefinitionFormatter.Verify(ldf => ldf.DisplayDefinition(this.mockTeamProjectTemplate.Object, "test"), Times.Once(), "The layout definition formatter was not invoked to display the chosen definition.");
        }

        /// <summary>
        /// Test that <see cref="TeamProjectDocument.SaveLayoutDefinition"/> invokes the layout definition formatter to save the given layout definition.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void SaveLayoutDefinitionInvokesLayoutDefinitionFormatterForGivenLayout()
        {
            // Arrange
            this.SetupTemplateWithValidLayout();

            // Act
            this.sut.SaveLayoutDefinition(TestHelper.ValidLayoutName);

            // Assert
            this.mockLayoutDefinitionFormatter.Verify(ldf => ldf.SaveDefinition(this.mockTeamProjectTemplate.Object, TestHelper.ValidLayoutName), Times.Once(), "The layout definition formatter was not invoked to save the chosen layout definition.");
        }

        /// <summary>
        /// Test that <see cref="TeamProjectDocument.AddPrototypeLayoutDefinition"/> invokes the layout definition formatter to add a prototype layout definition.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void AddPrototypeLayoutDefinitionInvokesLayoutDefinitionFormatter()
        {
            // Act
            this.sut.AddPrototypeLayoutDefinition();

            // Assert
            this.mockLayoutDefinitionFormatter.Verify(ldf => ldf.AddPrototypeDefinition(), Times.Once(), "The layout definition formatter was not invoked to add the prototype layout definition.");
        }

        /// <summary>
        /// Test that <see cref="TeamProjectDocument.HasChanged"/> invokes returns the value from the underlying Word document.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void HasChangedGetsValueFromUnderlyingWordDocument()
        {
            // Act and Assert
            this.mockWordDocument.Setup(doc => doc.HasChanged).Returns(false);
            Assert.IsFalse(this.sut.HasChanged, "Did not return value from word document.");

            this.mockWordDocument.Setup(doc => doc.HasChanged).Returns(true);
            Assert.IsTrue(this.sut.HasChanged, "Did not return value from word document.");
        }

        /// <summary>
        /// Test that <see cref="TeamProjectDocument.MarkDocumentClean"/> marks the Word document as clean.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void MarkDocumentCleanCausesWordDocumentToBeMarkedClean()
        {
            // Act
            this.sut.MarkDocumentClean();

            // Assert
            this.mockWordDocument.Verify(doc => doc.MarkDocumentClean(), Times.Once(), "Did not mark document as clean.");
        }

        /// <summary>
        /// Test that <see cref="TeamProjectDocument.AddField"/> checks for a null argument.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void AddFieldChecksForNullArgument()
        {
            TestHelper.TestForArgumentNullException(() => this.sut.AddField(null), "field");
        }

        /// <summary>
        /// Test that <see cref="TeamProjectDocument.AddField"/> adds a plain text content control to the document.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void AddFieldAddsPlainTextContentControlToTheDocument()
        {
            // Arrange
            ITfsFieldDefinition field = TestHelper.CreateMockFieldDefinition("RefName", "FriendlyName");

            // Act
            this.sut.AddField(field);

            // Assert
            this.mockWordDocument.Verify(doc => doc.AddContentControl("FriendlyName", "RefName", (int)WdContentControlType.wdContentControlText), Times.Once(), "Content control not added.");
        }

        /// <summary>
        /// Test that <see cref="TeamProjectDocument.AddField"/> adds a rich text content control to the document for a Html field.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void AddFieldAddsRichTextContentControlToTheDocumentForHtmlField()
        {
            // Arrange
            ITfsFieldDefinition field = TestHelper.CreateMockFieldDefinition("RefName", "FriendlyName", FieldType.Html);

            // Act
            this.sut.AddField(field);

            // Assert
            this.mockWordDocument.Verify(doc => doc.AddContentControl("FriendlyName", "RefName", (int)WdContentControlType.wdContentControlRichText), Times.Once(), "Content control not added.");
        }

        /// <summary>
        /// Test that <see cref="TeamProjectDocument.AddField"/> adds a rich text content control to the document for a History field.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void AddFieldAddsRichTextContentControlToTheDocumentForHistoryField()
        {
            // Arrange
            ITfsFieldDefinition field = TestHelper.CreateMockFieldDefinition("RefName", "FriendlyName", FieldType.History);

            // Act
            this.sut.AddField(field);

            // Assert
            this.mockWordDocument.Verify(doc => doc.AddContentControl("FriendlyName", "RefName", (int)WdContentControlType.wdContentControlRichText), Times.Once(), "Content control not added.");
        }

        /// <summary>
        /// Test that <see cref="TeamProjectDocument.AddField"/> adds a date content control to the document for a DateTime field.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void AddFieldAddsDateContentControlToTheDocumentForDateTimeField()
        {
            // Arrange
            ITfsFieldDefinition field = TestHelper.CreateMockFieldDefinition("RefName", "FriendlyName", FieldType.DateTime);

            // Act
            this.sut.AddField(field);

            // Assert
            this.mockWordDocument.Verify(doc => doc.AddContentControl("FriendlyName", "RefName", (int)WdContentControlType.wdContentControlDate), Times.Once(), "Content control not added.");
        }

        /// <summary>
        /// Test that <see cref="TeamProjectDocument.RenameLayoutDefinition"/> renames the layout definition.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void RenameLayoutDefinitionRenamesTheLayoutDefinition()
        {
            // Act
            this.sut.RenameLayoutDefinition("Layout1", "Layout2");

            // Assert
            this.mockTeamProjectTemplate.Verify(t => t.DeleteLayout("Layout1"), Times.Once(), "Old layout not deleted.");
            this.mockLayoutDefinitionFormatter.Verify(f => f.SaveDefinition(this.mockTeamProjectTemplate.Object, "Layout2"), Times.Once(), "New layout not parsed");
        }

        /// <summary>
        /// Test that <see cref="TeamProjectDocument.RenameLayoutDefinition"/> makes the document dirty.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void RenameLayoutDefinitionSavesTemplate()
        {
            // Arrange
            Assert.IsFalse(this.sut.HasChanged, "Pre-requisite");

            // Act
            this.sut.RenameLayoutDefinition("Layout1", "Layout2");

            // Assert
            this.mockTeamProjectTemplate.Verify(t => t.Save(), Times.Once(), "The template was not saved.");
        }

        /// <summary>
        /// Test that the <see cref="TeamProjectDocument.Close"/> event is raised if the underlying document is closed.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void CloseEventRaisedWhenUnderlyingDocumentIsClosed()
        {
            // Arrange
            bool eventRaised = false;
            this.sut.Close += (s, e) => eventRaised = true;

            // Act
            this.mockWordDocument.Raise(doc => doc.Close += null, EventArgs.Empty);

            // Assert
            Assert.IsTrue(eventRaised, "Close event was not raised");
        }

        /// <summary>
        /// Test that the <see cref="TeamProjectDocument.SaveTeamProject"/> writes a handle to the word document.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void SaveTeamProjectWritesHandleToWordDocument()
        {
            // Arrange
            this.SetupDocumentWithATeamProject();
            this.mockWordDocument.VerifySet(word => word.Handle = It.IsAny<Guid>(), Times.Never(), "Pre-requisite");

            // Act
            this.sut.SaveTeamProject();

            // Assert
            this.mockWordDocument.VerifySet(word => word.Handle = It.IsAny<Guid>(), Times.Once(), "Handle must be set on the word document.");
        }

        /// <summary>
        /// Test that the <see cref="TeamProjectDocument.SaveTeamProject"/> does not write handle again if it has already been written.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void SaveTeamProjectWritesHandleToWordDocumentOnlyOnce()
        {
            // Arrange
            this.SetupDocumentWithATeamProject();
            this.mockWordDocument.Setup(word => word.Handle).Returns(Guid.NewGuid());

            // Act
            this.sut.SaveTeamProject();

            // Assert
            this.mockWordDocument.VerifySet(word => word.Handle = It.IsAny<Guid>(), Times.Never(), "Handle must not be set on the word document if it is already set.");
        }

        /// <summary>
        /// Test that the <see cref="TeamProjectDocument.SaveTeamProject"/> raises the <see cref="TeamProjectDocument.Connected"/> event.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void SaveTeamProjecRaisesConnectedEventOnSuccessfulSave()
        {
            // Arrange
            this.SetupDocumentWithATeamProject();
            this.mockWordDocument.Setup(word => word.Handle).Returns(Guid.NewGuid());
            bool raised = false;
            this.sut.Connected += (s, e) => raised = true;

            // Act
            this.sut.SaveTeamProject();

            // Assert
            Assert.IsTrue(raised, "event was not raised");
        }

        // TODO: Need to add a UI feature to reconnect server if it has been renamed. Currently only works if the original server still exists but does not have the TPC

        /// <summary>
        /// Creates a mock content control.
        /// </summary>
        /// <param name="tag">The tag to be put on the content control.</param>
        /// <returns>The mock content control.</returns>
        private static ContentControl CreateMockContentControl(string tag)
        {
            Mock<ContentControl> contentControl = new Mock<ContentControl>();
            contentControl.Setup(cc => cc.Tag).Returns(tag);
            return contentControl.Object;
        }

        /// <summary>
        /// Counts the number of work items in work item XML that contain a particular field.
        /// </summary>
        /// <param name="xdoc">The xml document to be checked.</param>
        /// <param name="name">The name of the field being counted.</param>
        /// <returns>The count of the number of work items in work item XML that contain the field.</returns>
        private static int CountWorkItemsInXmlWithField(XDocument xdoc, string name)
        {
            XNamespace ns = Constants.WorkItemNamespace;
            return xdoc.Descendants(ns + Constants.WorkItemFieldElementName).Attributes(Constants.WorkItemFieldNameAttributeName).Where(attr => attr.Value == name).Count();
        }

        /// <summary>
        /// Sets up a connected document with the given project and query information.
        /// </summary>
        /// <param name="projectInformationXml">The project information XML.</param>
        /// <param name="queryXml">The query XML.</param>
        /// <param name="workItemXml">The work items XML.</param>
        /// <param name="queryWorkItemXml">The query and work item association XML.</param>
        private void SetupConnectedDocument(string projectInformationXml, string queryXml, string workItemXml, string queryWorkItemXml)
        {
            this.SetupCustomXmlPart(Constants.ProjectInformationNamespace, projectInformationXml);
            this.SetupCustomXmlPart(Constants.QueryNamespace, queryXml);
            this.SetupCustomXmlPart(Constants.WorkItemNamespace, workItemXml);
            this.SetupCustomXmlPart(Constants.QueryWorkItemNamespace, queryWorkItemXml);
        }

        /// <summary>
        /// Sets up a disconnected document.
        /// </summary>
        private void SetupDisconnectedDocument()
        {
            this.SetupCustomXmlPartForAnyNamespace(null);
        }

        /// <summary>
        /// Sets up a valid connected document.
        /// </summary>
        /// <param name="workItems">Work items that go into the document.</param>
        private void SetupValidConnectedDocument(params ITfsWorkItem[] workItems)
        {
            this.SetupConnectedDocument(
                TestHelper.ValidProjectXml,
                TestHelper.ValidQueryAndLayoutXml,
                TestHelper.ValidWorkItemsXml(workItems.Select(wi => TfsWorkItem.Serialize(wi, wi.FieldReferenceNames.ToArray())).ToArray()),
                TestHelper.ValidQueryWorkItemsXml());
            this.SetupTemplateWithValidLayout();
        }

        /// <summary>
        /// Sets up a valid connected document with multiple queries and layouts.
        /// </summary>
        private void SetupValidConnectedDocumentMultiple()
        {
            this.SetupConnectedDocument(TestHelper.ValidProjectXml, TestHelper.ValidQueryAndLayoutXmlMultiple, TestHelper.ValidWorkItemsXml(), TestHelper.ValidQueryWorkItemsXml());
            this.SetupTemplateLayouts(TestHelper.ValidLayoutName, TestHelper.ValidLayoutName2);
        }

        /// <summary>
        /// Sets up a custom XML part for testing.
        /// </summary>
        /// <param name="partNamespace">The namespace of the XML part.</param>
        /// <param name="partData">The data in the XML part.</param>
        private void SetupCustomXmlPart(string partNamespace, string partData)
        {
            Mock<CustomXMLPart> mockPart = new Mock<CustomXMLPart>();
            mockPart.Setup(part => part.XML).Returns(partData);
            this.mockWordDocument.Setup(wordDoc => wordDoc.GetXmlPart(partNamespace)).Returns(mockPart.Object);
        }

        /// <summary>
        /// Sets up a custom XML part for testing, will match any namespace.
        /// </summary>
        /// <param name="partData">The data in the XML part.</param>
        private void SetupCustomXmlPartForAnyNamespace(string partData)
        {
            Mock<CustomXMLPart> mockPart = new Mock<CustomXMLPart>();
            mockPart.Setup(part => part.XML).Returns(partData);
            this.mockWordDocument.Setup(wordDoc => wordDoc.GetXmlPart(It.IsAny<string>())).Returns(mockPart.Object);
        }

        /// <summary>
        /// Sets up the document with a team project that has been saved.
        /// </summary>
        /// <returns>The mock team project that was created.</returns>
        private Mock<ITeamProject> SetupDocumentWithASavedTeamProject()
        {
            Mock<ITeamProject> project = this.SetupDocumentWithATeamProject();
            this.sut.SaveTeamProject();
            return project;
        }

        /// <summary>
        /// Sets up the document with a team project that has not been saved.
        /// </summary>
        /// <returns>The mock team project that was created.</returns>
        private Mock<ITeamProject> SetupDocumentWithATeamProject()
        {
            Mock<ITeamProject> project = this.SetupDocumentWithMockFactory();
            this.sut.TeamProject = project.Object;
            return project;
        }

        /// <summary>
        /// Sets up the mock factory.
        /// </summary>
        /// <param name="collectionId">Override the collection id with this value on the team project that gets created by the factory.</param>
        /// <returns>The mock team project.</returns>
        private Mock<ITeamProject> SetupDocumentWithMockFactory(Guid collectionId)
        {
            Uri uri = new Uri(TestHelper.ValidProjectCollectionUri);
            Mock<ITeamProject> teamProject = TestHelper.CreateMockTeamProject(uri, collectionId, TestHelper.ValidProjectName);
            this.mockFactory.Setup(factory => factory.CreateTeamProject(uri, TestHelper.ValidProjectName, null)).Returns(teamProject.Object);
            this.SetupMockQueryAndLayoutManager();
            this.mockFactory.Setup(factory => factory.CreateQueryAndLayoutManager(It.IsAny<IEnumerable<ITfsFieldDefinition>>())).Returns(() => this.mockQueryAndLayoutManager.Object);
            ITfsFieldDefinition[] fields = TestHelper.CreateMockFieldDefinitions(FieldA);
            teamProject.Setup(tp => tp.FieldDefinitions).Returns(fields);

            this.mockTeamProjectQuery = new Mock<ITeamProjectQuery>();
            teamProject.Setup(project => project.QueryRunner).Returns(this.mockTeamProjectQuery.Object);

            return teamProject;
        }

        /// <summary>
        /// Sets up the mock factory.
        /// </summary>
        /// <returns>The mock team project.</returns>
        private Mock<ITeamProject> SetupDocumentWithMockFactory()
        {
            return this.SetupDocumentWithMockFactory(TestHelper.ValidProjectCollectionId);
        }

        /// <summary>
        /// Sets up the mock word document with a template with some layouts.
        /// </summary>
        /// <param name="layoutNames">The layouts in the template.</param>
        /// <returns>The created layouts.</returns>
        private LayoutInformation[] SetupTemplateLayouts(params string[] layoutNames)
        {
            LayoutInformation[] layouts = layoutNames.Select(name => TestHelper.CreateTestLayoutInformation(name, BuildingBlockName.Default)).ToArray();
            this.mockTeamProjectTemplate.Setup(template => template.Layouts).Returns(layouts);
            return layouts;
        }

        /// <summary>
        /// Sets up a template with the valid layout.
        /// </summary>
        private void SetupTemplateWithValidLayout()
        {
            this.SetupTemplateLayouts(TestHelper.ValidLayoutName);
        }

        /// <summary>
        /// Sets up query and layout manager.
        /// </summary>
        /// <remarks>
        /// When a query and layout is added to manager the entries are manipulated as follows. For the original queries the same <see cref="QueryAndLayoutInformation"/> objects are returned.
        /// For final queries, new <see cref="QueryAndLayoutInformation"/> objects are returned, which contain new <see cref="QueryDefinition"/> objects that contain the same query text as the original.
        /// </remarks>
        private void SetupMockQueryAndLayoutManager()
        {
            this.mockQueryAndLayoutManager = new Mock<IQueryAndLayoutManager>();
            this.mockQueryAndLayoutManager.Setup(qlm => qlm.OriginalQueriesAndLayouts).Returns(this.originalQueriesAndLayouts);
            this.mockQueryAndLayoutManager
                .Setup(qlm => qlm.FinalQueriesAndLayouts)
                .Returns(() =>
                {
                    int i = 0;
                    return this.originalQueriesAndLayouts.Select(
                        qli =>
                        {
                            return new QueryAndLayoutInformation(new TeamFoundation.WorkItemTracking.Client.QueryDefinition(qli.Query.Name + "final", qli.Query.QueryText), qli.Layout, i++);
                        });
                });
            this.mockQueryAndLayoutManager
                .Setup(qlm => qlm.Add(It.IsAny<QueryAndLayoutInformation>()))
                .Callback((QueryAndLayoutInformation qli) => { Console.WriteLine("adding {0}", qli.Query.GetHashCode()); this.originalQueriesAndLayouts.Add(qli); });
            this.mockQueryAndLayoutManager
                .Setup(qlm => qlm.AddRange(It.IsAny<QueryAndLayoutInformation[]>()))
                .Callback((QueryAndLayoutInformation[] qlis) => { this.originalQueriesAndLayouts.AddRange(qlis); });
        }

        /// <summary>
        /// Sets up a document that will get two work items in two queries when queried.
        /// </summary>
        /// <returns>The query and layouts used and the work items.</returns>
        private Tuple<QueryAndLayoutInformation[], ITfsWorkItem[]> SetupDocumentWithTwoWorkItemsInTwoQueries()
        {
            Mock<ITeamProject> mockProject = this.SetupDocumentWithASavedTeamProject();
            QueryAndLayoutInformation qli1 = TestHelper.ValidQueryDefinitionAndLayout;
            QueryAndLayoutInformation qli2 = TestHelper.ValidQueryDefinitionAndLayout2;
            this.sut.AddQueryAndLayout(qli1);
            this.sut.AddQueryAndLayout(qli2);

            WorkItemTree workItems1 = new WorkItemTree();
            ITfsWorkItem workItem1 = TestHelper.CreateMockWorkItem("Task", 1, "title1", new Tuple<string, object>("System.Area", "testarea1"), new Tuple<string, object>("System.Description", "testdesc1"));
            TestHelper.PopulateWorkItemTreeWithFlatQueryResults(workItems1, workItem1);

            WorkItemTree workItems2 = new WorkItemTree();
            ITfsWorkItem workItem2 = TestHelper.CreateMockWorkItem("Task", 2, "title2", new Tuple<string, object>("System.Area", "testarea2"), new Tuple<string, object>("System.Description", "testdesc2"));
            TestHelper.PopulateWorkItemTreeWithFlatQueryResults(workItems2, workItem2);

            this.mockTeamProjectQuery.Setup(projectQuery => projectQuery.QueryForWorkItems(TestHelper.IsQueryText(TestHelper.ValidQueryDefinitionAndLayout.Query.QueryText), It.IsAny<CancellationToken>())).Returns(workItems1);
            this.mockTeamProjectQuery.Setup(projectQuery => projectQuery.QueryForWorkItems(TestHelper.IsQueryText(TestHelper.ValidQueryDefinitionAndLayout2.Query.QueryText), It.IsAny<CancellationToken>())).Returns(workItems2);

            return new Tuple<QueryAndLayoutInformation[], ITfsWorkItem[]>(
                new QueryAndLayoutInformation[] { qli1, qli2 },
                new ITfsWorkItem[] { workItem1, workItem2 });
        }

        /// <summary>
        /// Saves the existing work items without adding additional work items.
        /// </summary>
        private void SaveExistingWorkItemsOnly()
        {
            this.sut.SaveWorkItems(new WorkItemTree(), new string[0], new CancellationToken());
        }

        /// <summary>
        /// Asserts that no queries or layouts have been loaded.
        /// </summary>
        private void AssertNoQueriesOrLayoutsLoaded()
        {
            string actualXml = string.Empty;
            this.mockWordDocument.Setup(wordDoc => wordDoc.AddXmlPart(TestHelper.IsAnyXmlDocumentInNamespace(Constants.QueryNamespace))).Callback((string xml) => actualXml = xml);
            this.sut.SaveQueriesAndLayouts();
            Assert.AreEqual<string>(TestHelper.EmptyQueryAndLayoutXml, actualXml, "No queries or layouts should have been loaded.");
        }
    }
}
