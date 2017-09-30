//---------------------------------------------------------------------
// <copyright file="TeamProjectDocumentFormatterTests.cs" company="Microsoft">
//    Copyright © ALM | DevOps Ranger Contributors. All rights reserved.
//    This code is licensed under the MIT License.
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF 
//    ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
//    TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
//    PARTICULAR PURPOSE AND NONINFRINGEMENT.
// </copyright>
// <summary>The TeamProjectDocumentFormatterTests type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.Word4Tfs.Library.Test.UnitTest.Model
{
    using System;
    using global::System.Collections.Generic;
    using global::System.Linq;
    using global::System.Text;
    using global::System.Threading;
    using Microsoft.Office.Core;
    using Microsoft.Office.Interop.Word;
    using Microsoft.Practices.Unity;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Word4Tfs.Library.Data;
    using Microsoft.Word4Tfs.Library.Model;
    using Microsoft.Word4Tfs.Library.Model.TeamFoundation;
    using Microsoft.Word4Tfs.Library.Model.Word;
    using Moq;

    /// <summary>
    /// Tests the <see cref="TeamProjectDocumentFormatter"/> class.
    /// </summary>
    [TestClass]
    [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Unity container disposed in the test cleanup code.")]
    public class TeamProjectDocumentFormatterTests
    {
        /// <summary>
        /// The name of a test field.
        /// </summary>
        private const string FieldId = "System.Id";

        /// <summary>
        /// The name of a test field.
        /// </summary>
        private const string FieldDescription = "System.Description";

        /// <summary>
        /// The name of a test field.
        /// </summary>
        private const string FieldTitle = "System.Title";

        /// <summary>
        /// The name of a test field.
        /// </summary>
        private const string FieldTest = "Test.Test";

        /// <summary>
        /// The Unity container to be used for Dependency Injection
        /// </summary>
        private IUnityContainer container;

        /// <summary>
        /// The mock word document used to test the model.
        /// </summary>
        private MockWordDocument mockWordDocument;

        /// <summary>
        /// The mock team project template used to test the model.
        /// </summary>
        private Mock<ITeamProjectTemplate> mockTeamProjectTemplate;

        /// <summary>
        /// The list used to store the mock building blocks for the mock layout.
        /// </summary>
        private List<BuildingBlock> mockBuildingBlocks;

        /// <summary>
        /// The list used to store the mock building block names for the mock layout.
        /// </summary>
        private List<BuildingBlockName> mockBuildingBlockNames;

        /// <summary>
        /// The tree of work items the test will work with.
        /// </summary>
        private WorkItemTree workItems;

        /// <summary>
        /// The default building block in the mock layout.
        /// </summary>
        private BuildingBlock mockDefaultBuildingBlock;

        /// <summary>
        /// The model to be tested.
        /// </summary>
        private ITeamProjectDocumentFormatter sut;

        /// <summary>
        /// Defines the possible types of content control mapping.
        /// </summary>
        private enum MappingType
        {
            /// <summary>
            /// No mapping.
            /// </summary>
            NotMapped,

            /// <summary>
            /// Bound to a Custom XML Part.
            /// </summary>
            DataBind,

            /// <summary>
            /// Bound as content to a rich text content control.
            /// </summary>
            RichText
        }

        /// <summary>
        /// Initializes the test.
        /// </summary>
        [TestInitialize]
        public void InitializeTest()
        {
            this.container = new UnityContainer();

            this.mockWordDocument = new MockWordDocument();
            this.mockTeamProjectTemplate = TestHelper.CreateAndRegisterMock<ITeamProjectTemplate>(this.container);
            this.container.RegisterInstance<ILogger>(new Logger());
            this.container.RegisterInstance<IWordDocument>(this.mockWordDocument.Object);

            this.sut = this.container.Resolve<TeamProjectDocumentFormatter>();

            this.workItems = new WorkItemTree();

            this.mockBuildingBlocks = new List<BuildingBlock>();
            this.mockBuildingBlockNames = new List<BuildingBlockName>();
        }

        /// <summary>
        /// Cleans up the test, disposing the Unity container.
        /// </summary>
        [TestCleanup]
        public void CleanupTest()
        {
            this.mockWordDocument.DumpActualDocument();
            this.container.Dispose();
        }

        /// <summary>
        /// Test that <see cref="MapWorkItemsIntoDocument"/> throws an exception if layout argument is null.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void MapWorksItemsIntoDocumentChecksForNullLayout()
        {
            // Arrange, Act and Assert
            TestHelper.TestForArgumentNullException(() => this.sut.MapWorkItemsIntoDocument(this.workItems, null, TestHelper.DummyBookmarkNamingFuncQuery0, new CancellationToken()), "layout");
        }

        /// <summary>
        /// Test that <see cref="MapWorkItemsIntoDocument"/> throws an exception if work items argument is null.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void MapWorksItemsIntoDocumentChecksForNullWorkItems()
        {
            // Arrange
            LayoutInformation testLayout = this.SetupTestLayout();

            // Act and Assert
            TestHelper.TestForArgumentNullException(() => this.sut.MapWorkItemsIntoDocument(null, testLayout, TestHelper.DummyBookmarkNamingFuncQuery0, new CancellationToken()), "workItems");
        }

        /// <summary>
        /// Test that <see cref="MapWorkItemsIntoDocument"/> throws an exception if bookmark naming function is null.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void MapWorksItemsIntoDocumentChecksForNullBookmarkNamingFunction()
        {
            // Arrange
            LayoutInformation testLayout = this.SetupTestLayout();

            // Act and Assert
            TestHelper.TestForArgumentNullException(() => this.sut.MapWorkItemsIntoDocument(this.workItems, testLayout, null, new CancellationToken()), "bookmarkNamingFunction");
        }

        /// <summary>
        /// Test that <see cref="TeamProjectDocumentFormatter.MapWorkItemsIntoDocument"/> adds each work item to the document as a building block with the correct bookmark.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void MapWorkItemsIntoDocumentAddsEachWorkItemToDocumentAsBuildingBlockWithCorrectBookmark()
        {
            // Arrange
            WorkItemTreeNode item1 = this.AddTestWorkItem(1, "Item1");
            WorkItemTreeNode item2 = this.AddTestWorkItem(2, "Item2");
            this.CreateMockXmlPartForWorkItemsAndAddToDocument();
            this.CreateMockDefaultBuildingBlock();
            this.CreateMockContentControlsAndAddToDocument();
            LayoutInformation testLayout = this.SetupTestLayout();

            // Act
            this.sut.MapWorkItemsIntoDocument(this.workItems, testLayout, TestHelper.DummyBookmarkNamingFuncQuery0, new CancellationToken());

            // Assert
            this.mockWordDocument.AssertBuildingBlockInserted(this.mockDefaultBuildingBlock, TestHelper.DummyBookmarkNamingFuncQuery0(item1.WorkItem.Id));
            this.mockWordDocument.AssertBuildingBlockInserted(this.mockDefaultBuildingBlock, TestHelper.DummyBookmarkNamingFuncQuery0(item2.WorkItem.Id));
            this.mockWordDocument.AssertEndOfDocument();
        }

        /// <summary>
        /// Test that <see cref="TeamProjectDocumentFormatter.MapWorkItemsIntoDocument"/> adds each work item to the document in depth-first order.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void MapWorkItemsIntoDocumentAddsEachWorkItemToDocumentInDepthFirstOrder()
        {
            // Arrange
            TestHelper.PopulateWorkItemTreeWithHierarchicalQueryResults(this.workItems, "1", "1.3", "2");

            this.CreateMockXmlPartForWorkItemsAndAddToDocument();
            this.CreateMockDefaultBuildingBlock();
            this.CreateMockContentControlsAndAddToDocument();
            LayoutInformation testLayout = this.SetupTestLayout();

            // Act
            this.sut.MapWorkItemsIntoDocument(this.workItems, testLayout, TestHelper.DummyBookmarkNamingFuncQuery0, new CancellationToken());

            // Assert
            this.VerifyPhysicalOrderOfWorkItems(1, 3, 2);
        }

        /// <summary>
        /// Test that <see cref="TeamProjectDocumentFormatter.MapWorkItemsIntoDocument"/> maps each work item with the correct work item id in the xpath.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void MapWorkItemsIntoDocumentMapsEachWorkItemWithCorrectWorkItemIdInXPath()
        {
            // Arrange
            this.AddTestWorkItem(123, "Item1");
            this.AddTestWorkItem(456, "Item2");
            CustomXMLPart xmlPart = this.CreateMockXmlPartForWorkItemsAndAddToDocument();
            this.CreateMockDefaultBuildingBlock();
            ContentControl[] contentControls = this.CreateMockContentControlsAndAddToDocument(FieldId);
            LayoutInformation testLayout = this.SetupTestLayout();

            // Act
            this.sut.MapWorkItemsIntoDocument(this.workItems, testLayout, TestHelper.DummyBookmarkNamingFuncQuery0, new CancellationToken());

            // Assert
            this.VerifyContentControlMapping(contentControls[0], 123, FieldId, xmlPart);
            this.VerifyContentControlMapping(contentControls[0], 456, FieldId, xmlPart);
        }

        /// <summary>
        /// Test that a plain text content control with an invalid tag is not mapped to a work item field.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void DoesNotMapsPlainTextContentControlToWorkItemFieldIfTheFieldNameIsNotValid()
        {
            this.TestWorkItemFieldToContentControlMapping(WdContentControlType.wdContentControlText, MappingType.NotMapped, true, "a", "test");
        }

        /// <summary>
        /// Test that a plain text content control is mapped to a work item field.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void MapsPlainTextContentControlToWorkItemField()
        {
            this.TestWorkItemFieldToContentControlMapping(WdContentControlType.wdContentControlText, MappingType.DataBind);
        }

        /// <summary>
        /// Test that a plain text content control with a field that is not the work item is still mapped to a work item field.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void DoesNotMapPlainTextContentControlToWorkItemFieldIfWorkItemDoesNotContainTheField()
        {
            this.TestWorkItemFieldToContentControlMapping(WdContentControlType.wdContentControlText, MappingType.NotMapped, false, FieldTest, "test");
        }

        /// <summary>
        /// Test that a date time picker content control is mapped to a work item field.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void MapsDateTimePickerContentControlTimeWorkItemField()
        {
            this.TestWorkItemFieldToContentControlMapping(WdContentControlType.wdContentControlDate, MappingType.DataBind);
        }

        /// <summary>
        /// Test that a rich text content control with an invalid tag is not mapped to a work item field.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void DoesNotMapRichTextContentControlToWorkItemFieldIfTheFieldNameIsNotValid()
        {
            this.TestWorkItemFieldToContentControlMapping(WdContentControlType.wdContentControlRichText, MappingType.NotMapped, true, "a", "test");
        }

        /// <summary>
        /// Test that a rich text content control with field that is not in the work item is not mapped to a work item field.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void DoesNotMapRichTextContentControlToWorkItemFieldIfWorkItemDoesNotContainTheField()
        {
            this.TestWorkItemFieldToContentControlMapping(WdContentControlType.wdContentControlRichText, MappingType.NotMapped, false, FieldTest, "test");
        }

        /// <summary>
        /// Test that a rich text content control is not mapped to a work item field.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void MapsRichTextContentControlToHtmlWorkItemField()
        {
            this.TestWorkItemFieldToContentControlMapping(WdContentControlType.wdContentControlRichText, MappingType.RichText);
        }

        /// <summary>
        /// Test that a combo box content control is not mapped to a work item field.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void DoesNotMapComboBoxContentControlToWorkItemField()
        {
            this.TestWorkItemFieldToContentControlMapping(WdContentControlType.wdContentControlComboBox, MappingType.NotMapped);
        }

        /// <summary>
        /// Test that a building block gallery content control is not mapped to a work item field.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void DoesNotMapBuildingBlockGalleryContentControlToWorkItemField()
        {
            this.TestWorkItemFieldToContentControlMapping(WdContentControlType.wdContentControlBuildingBlockGallery, MappingType.NotMapped);
        }

        /// <summary>
        /// Test that a check box content control is not mapped to a work item field.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void DoesNotMapCheckBoxGalleryContentControlToWorkItemField()
        {
            this.TestWorkItemFieldToContentControlMapping(WdContentControlType.wdContentControlCheckBox, MappingType.NotMapped);
        }

        /// <summary>
        /// Test that a dropdown list content control is not mapped to a work item field.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void DoesNotMapDropDownListContentControlToWorkItemField()
        {
            this.TestWorkItemFieldToContentControlMapping(WdContentControlType.wdContentControlDropdownList, MappingType.NotMapped);
        }

        /// <summary>
        /// Test that a group content control is not mapped to a work item field.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void DoesNotMapGroupContentControlToWorkItemField()
        {
            this.TestWorkItemFieldToContentControlMapping(WdContentControlType.wdContentControlGroup, MappingType.NotMapped);
        }

        /// <summary>
        /// Test that a picture content control is not mapped to a work item field.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void DoesNotMapPictureContentControlToWorkItemField()
        {
            this.TestWorkItemFieldToContentControlMapping(WdContentControlType.wdContentControlPicture, MappingType.NotMapped);
        }

        /// <summary>
        /// Test that a rich text content control tag is updated with the work item id.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void RichTextContentControlTagIsUpdatedWithWorkItemId()
        {
            this.AddTestWorkItem(1, "Item1", new Tuple<string, object>(FieldTest, "test"));
            CustomXMLPart xmlPart = this.CreateMockXmlPartForWorkItemsAndAddToDocument();
            this.CreateMockBuildingBlock(new BuildingBlockName("dummy1"));
            this.CreateMockDefaultBuildingBlock();
            ContentControl[] contentControls = this.CreateMockContentControlsAndAddToDocument(WdContentControlType.wdContentControlRichText, FieldTest);
            LayoutInformation testLayout = this.SetupTestLayout();

            // Act
            this.sut.MapWorkItemsIntoDocument(this.workItems, testLayout, TestHelper.DummyBookmarkNamingFuncQuery0, new CancellationToken());

            // Assert
            Assert.AreEqual<string>(FieldTest + "-1", contentControls[0].Tag, "Work item Id should have been added to the rich text content control tag.");
        }

        /// <summary>
        /// Test that multiple content controls are mapped to the associated work item fields.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void MultipleContentControlAreMappedToAssociatedWorkItemFields()
        {
            // Arrange
            this.AddTestWorkItem(1, "Item1", new Tuple<string, object>(FieldDescription, "description"));
            CustomXMLPart xmlPart = this.CreateMockXmlPartForWorkItemsAndAddToDocument();
            this.CreateMockBuildingBlock(new BuildingBlockName("dummy1"));
            this.CreateMockDefaultBuildingBlock();
            ContentControl[] contentControls = this.CreateMockContentControlsAndAddToDocument(FieldId, FieldDescription, FieldTitle);
            LayoutInformation testLayout = this.SetupTestLayout();

            // Act
            this.sut.MapWorkItemsIntoDocument(this.workItems, testLayout, TestHelper.DummyBookmarkNamingFuncQuery0, new CancellationToken());

            // Assert
            this.VerifyContentControlMapping(contentControls[0], 1, FieldId, xmlPart);
            this.VerifyContentControlMapping(contentControls[1], 1, FieldDescription, xmlPart);
            this.VerifyContentControlMapping(contentControls[2], 1, FieldTitle, xmlPart);
        }

        /// <summary>
        /// Test that only content controls with valid tags are mapped to the associated work item fields.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void ContentControlsWithInvalidTagsAreNotMapped()
        {
            // Arrange
            this.AddTestWorkItem(1, "Item1");
            CustomXMLPart xmlPart = this.CreateMockXmlPartForWorkItemsAndAddToDocument();
            this.CreateMockBuildingBlock(new BuildingBlockName("dummy1"));
            this.CreateMockDefaultBuildingBlock();
            ContentControl[] contentControls = this.CreateMockContentControlsAndAddToDocument(FieldId, FieldTitle, ":@() !$");
            LayoutInformation testLayout = this.SetupTestLayout();

            // Act
            this.sut.MapWorkItemsIntoDocument(this.workItems, testLayout, TestHelper.DummyBookmarkNamingFuncQuery0, new CancellationToken());

            // Assert
            this.VerifyContentControlMapping(contentControls[0], 1, FieldId, xmlPart);
            this.VerifyContentControlMapping(contentControls[1], 1, FieldTitle, xmlPart);
            this.VerifyContentControlNotMapped(contentControls[2], 1, ":@() !$", xmlPart);
        }

        /// <summary>
        /// Test that formatting can be cancelled.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void FormattingCanBeCancelled()
        {
            // Arrange
            CancellationTokenSource cts = new CancellationTokenSource();
            this.AddTestWorkItem("User Story", 2, "Item2", 0);
            CustomXMLPart xmlPart = this.CreateMockXmlPartForWorkItemsAndAddToDocument();
            this.CreateMockDefaultBuildingBlock();
            BuildingBlock bb = this.CreateMockBuildingBlock(new BuildingBlockName("User Story"));
            this.CreateMockBuildingBlock(new BuildingBlockName("User Story_1"));
            LayoutInformation testLayout = this.SetupTestLayout();

            // Act
            cts.Cancel();
            global::System.Threading.Tasks.Task t = global::System.Threading.Tasks.Task.Factory.StartNew(() => this.sut.MapWorkItemsIntoDocument(this.workItems, testLayout, TestHelper.DummyBookmarkNamingFuncQuery0, cts.Token));

            // Assert
            TestHelper.AssertTaskCancelled(t);
        }

        /// <summary>
        /// Test that <see cref="TeamProjectDocument.RefreshWorkItems"/> throws an exception if a null refresh data is passed.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void RefreshWorkItemsThrowsExceptionIfNullRefreshDataIsPassed()
        {
            TestHelper.TestForArgumentNullException(() => this.sut.RefreshWorkItems(null, TestHelper.DummyBookmarkNamingFunc, new CancellationToken()), "refreshData");
        }

        /// <summary>
        /// Test that <see cref="TeamProjectDocument.RefreshWorkItems"/> locates rich text content controls and refreshes them.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void RefreshWorkItemsLocatesRichTextContentControlsAndRefreshesThem()
        {
            // Arrange
            WorkItemManager wimgr = new WorkItemManager();
            ITfsWorkItem mockItem1 = TestHelper.CreateMockWorkItem(1, new Tuple<string, object>(FieldTest, "test"));
            ITfsWorkItem mockItem2 = TestHelper.CreateMockWorkItem(2, new Tuple<string, object>(FieldTest, "test2"));
            this.SetupDocumentWithSavedWorkItemsFromFlatQuery(mockItem1, mockItem2);
            wimgr.Add(mockItem1);
            wimgr.Add(mockItem2);

            Mock<ContentControl> cc1 = CreateMockContentControl(WdContentControlType.wdContentControlText, "System.PlainText");
            Mock<ContentControl> cc2 = CreateMockContentControl(WdContentControlType.wdContentControlRichText, ConstructTagForWorkItemFieldAndId(FieldTest, "1"));
            Mock<ContentControl> cc3 = CreateMockContentControl(WdContentControlType.wdContentControlText, "System.PlainText");
            Mock<ContentControl> cc4 = CreateMockContentControl(WdContentControlType.wdContentControlText, "System.PlainText");
            Mock<ContentControl> cc5 = CreateMockContentControl(WdContentControlType.wdContentControlRichText, FieldTest + "-2");
            Mock<ContentControl> cc6 = CreateMockContentControl(WdContentControlType.wdContentControlText, "System.PlainText");
            this.mockWordDocument.Setup(doc => doc.AllContentControls()).Returns(new ContentControl[] { cc1.Object, cc2.Object, cc3.Object, cc4.Object, cc5.Object, cc6.Object });
            QueryWorkItems[] qwi = TestHelper.CreateQueryWorkItems(new int[] { 1, 2 });
            LayoutInformation[] layouts = new LayoutInformation[] { this.SetupTestLayout() };
            bool[] queryIsFlat = Enumerable.Repeat<bool>(false, 1).ToArray();

            // Act
            FormatterRefreshData refreshData = new FormatterRefreshData { WorkItemManager = wimgr, QueryWorkItemsBefore = qwi, QueryWorkItemsAfter = qwi, Layouts = layouts, QueryIsFlat = queryIsFlat };
            this.sut.RefreshWorkItems(refreshData, TestHelper.DummyBookmarkNamingFunc, new CancellationToken());

            // Assert
            this.mockWordDocument.Verify(doc => doc.PopulateRichTextContentControlWithHtml(cc2.Object, "test"), Times.Once(), "First rich text control not updated");
            this.mockWordDocument.Verify(doc => doc.PopulateRichTextContentControlWithHtml(cc5.Object, "test2"), Times.Once(), "Second rich text control not updated");
            this.mockWordDocument.Verify(doc => doc.PopulateRichTextContentControlWithHtml(It.IsAny<ContentControl>(), It.IsAny<string>()), Times.Exactly(2), "No other content controls should be updated.");
        }

        /// <summary>
        /// Test that <see cref="TeamProjectDocument.RefreshWorkItems"/> locates rich text content controls and refreshes them.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void RefreshWorkItemsCanBeCancelled()
        {
            // Arrange
            CancellationTokenSource cts = new CancellationTokenSource();
            WorkItemManager wimgr = new WorkItemManager();
            wimgr.Add(TestHelper.CreateMockWorkItem(1, new Tuple<string, object>(FieldTest, "test")));
            wimgr.Add(TestHelper.CreateMockWorkItem(2, new Tuple<string, object>(FieldTest, "test2")));
            Mock<ContentControl> cc1 = CreateMockContentControl(WdContentControlType.wdContentControlText, "System.PlainText");
            Mock<ContentControl> cc2 = CreateMockContentControl(WdContentControlType.wdContentControlRichText, ConstructTagForWorkItemFieldAndId(FieldTest, "1"));
            Mock<ContentControl> cc3 = CreateMockContentControl(WdContentControlType.wdContentControlText, "System.PlainText");
            Mock<ContentControl> cc4 = CreateMockContentControl(WdContentControlType.wdContentControlText, "System.PlainText");
            Mock<ContentControl> cc5 = CreateMockContentControl(WdContentControlType.wdContentControlRichText, FieldTest + "-2");
            Mock<ContentControl> cc6 = CreateMockContentControl(WdContentControlType.wdContentControlText, "System.PlainText");
            this.mockWordDocument.Setup(doc => doc.AllContentControls()).Returns(new ContentControl[] { cc1.Object, cc2.Object, cc3.Object, cc4.Object, cc5.Object, cc6.Object });
            QueryWorkItems[] qwi = TestHelper.CreateQueryWorkItems(new int[] { 1, 2 });
            LayoutInformation[] layouts = new LayoutInformation[] { this.SetupTestLayout() };
            bool[] queryIsFlat = Enumerable.Repeat<bool>(true, 1).ToArray();

            // Act
            cts.Cancel();
            FormatterRefreshData refreshData = new FormatterRefreshData { WorkItemManager = wimgr, QueryWorkItemsBefore = qwi, QueryWorkItemsAfter = qwi, Layouts = layouts, QueryIsFlat = queryIsFlat };
            global::System.Threading.Tasks.Task t = global::System.Threading.Tasks.Task.Factory.StartNew(() => this.sut.RefreshWorkItems(refreshData, TestHelper.DummyBookmarkNamingFunc, cts.Token));

            // Assert
            TestHelper.AssertTaskCancelled(t);
        }

        /// <summary>
        /// Test that <see cref="TeamProjectDocument.RefreshWorkItems"/> does not refresh rich text content controls not tagged with a work item id.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void RefreshWorkItemsDoesNotRefreshRichTextContentControlNotTaggedWithWorkItemId()
        {
            // Arrange
            WorkItemManager wimgr = new WorkItemManager();
            wimgr.AddRange(this.SetupDocumentWithSavedWorkItemsFromFlatQuery(TestHelper.CreateMockWorkItem(1, new Tuple<string, object>(FieldTest, "test"))));
            Mock<ContentControl> cc = CreateMockContentControl(WdContentControlType.wdContentControlRichText, ConstructTagForWorkItemFieldAndId(FieldTest, string.Empty));
            this.mockWordDocument.Setup(doc => doc.AllContentControls()).Returns(new ContentControl[] { cc.Object });
            QueryWorkItems[] qwi = TestHelper.CreateQueryWorkItems(new int[] { 1 });
            LayoutInformation[] layouts = new LayoutInformation[] { this.SetupTestLayout() };
            bool[] queryIsFlat = Enumerable.Repeat<bool>(true, 1).ToArray();

            // Act
            FormatterRefreshData refreshData = new FormatterRefreshData { WorkItemManager = wimgr, QueryWorkItemsBefore = qwi, QueryWorkItemsAfter = qwi, Layouts = layouts, QueryIsFlat = queryIsFlat };
            this.sut.RefreshWorkItems(refreshData, TestHelper.DummyBookmarkNamingFunc, new CancellationToken());

            // Assert
            this.mockWordDocument.Verify(doc => doc.PopulateRichTextContentControlWithHtml(It.IsAny<ContentControl>(), It.IsAny<string>()), Times.Never(), "Content control should not be updated.");
        }

        /// <summary>
        /// Test that <see cref="TeamProjectDocument.RefreshWorkItems"/> does not refresh rich text content controls not tagged with a valid work item field name.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void RefreshWorkItemsDoesNotRefreshRichTextContentControlNotTaggedWithValidWorkItemFieldName()
        {
            // Arrange
            WorkItemManager wimgr = new WorkItemManager();
            wimgr.AddRange(this.SetupDocumentWithSavedWorkItemsFromFlatQuery(TestHelper.CreateMockWorkItem(1, new Tuple<string, object>(FieldTest, "test"))));
            Mock<ContentControl> cc = CreateMockContentControl(WdContentControlType.wdContentControlRichText, ConstructTagForWorkItemFieldAndId("!2 6", "1"));
            this.mockWordDocument.Setup(doc => doc.AllContentControls()).Returns(new ContentControl[] { cc.Object });
            QueryWorkItems[] qwi = TestHelper.CreateQueryWorkItems(new int[] { 1 });
            LayoutInformation[] layouts = new LayoutInformation[] { this.SetupTestLayout() };
            bool[] queryIsFlat = Enumerable.Repeat<bool>(true, 1).ToArray();

            // Act
            FormatterRefreshData refreshData = new FormatterRefreshData { WorkItemManager = wimgr, QueryWorkItemsBefore = qwi, QueryWorkItemsAfter = qwi, Layouts = layouts, QueryIsFlat = queryIsFlat };
            this.sut.RefreshWorkItems(refreshData, TestHelper.DummyBookmarkNamingFunc, new CancellationToken());

            // Assert
            this.mockWordDocument.Verify(doc => doc.PopulateRichTextContentControlWithHtml(It.IsAny<ContentControl>(), It.IsAny<string>()), Times.Never(), "Content control should not be updated.");
        }

        /// <summary>
        /// Test that <see cref="TeamProjectDocument.RefreshWorkItems"/> does not refresh rich text content controls not tagged with a the name of a field that actually exists.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void RefreshWorkItemsDoesNotRefreshRichTextContentControlNotTaggedWithExistingWorkItemFieldName()
        {
            // Arrange
            WorkItemManager wimgr = new WorkItemManager();
            wimgr.AddRange(this.SetupDocumentWithSavedWorkItemsFromFlatQuery(TestHelper.CreateMockWorkItem(1, new Tuple<string, object>(FieldTest, "test"))));
            Mock<ContentControl> cc = CreateMockContentControl(WdContentControlType.wdContentControlRichText, ConstructTagForWorkItemFieldAndId("Dummy.Dummy", "1"));
            this.mockWordDocument.Setup(doc => doc.AllContentControls()).Returns(new ContentControl[] { cc.Object });
            QueryWorkItems[] qwi = TestHelper.CreateQueryWorkItems(new int[] { 1 });
            LayoutInformation[] layouts = new LayoutInformation[] { this.SetupTestLayout() };
            bool[] queryIsFlat = Enumerable.Repeat<bool>(true, 1).ToArray();

            // Act
            FormatterRefreshData refreshData = new FormatterRefreshData { WorkItemManager = wimgr, QueryWorkItemsBefore = qwi, QueryWorkItemsAfter = qwi, Layouts = layouts, QueryIsFlat = queryIsFlat };
            this.sut.RefreshWorkItems(refreshData, TestHelper.DummyBookmarkNamingFunc, new CancellationToken());

            // Assert
            this.mockWordDocument.Verify(doc => doc.PopulateRichTextContentControlWithHtml(It.IsAny<ContentControl>(), It.IsAny<string>()), Times.Never(), "Content control should not be updated.");
        }

        /// <summary>
        /// Test that <see cref="TeamProjectDocument.RefreshWorkItems"/> empties rich text content controls tagged with the id of a work item that no longer exists.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void RefreshWorkItemsEmptiesRichTextContentControlTaggedWithIdOfWorkItemWhichNoLongerExists()
        {
            // Arrange
            WorkItemManager wimgr = new WorkItemManager();
            wimgr.AddRange(this.SetupDocumentWithSavedWorkItemsFromFlatQuery(TestHelper.CreateMockWorkItem(1, new Tuple<string, object>(FieldTest, "test"))));
            Mock<ContentControl> cc = CreateMockContentControl(WdContentControlType.wdContentControlRichText, ConstructTagForWorkItemFieldAndId(FieldTest, "2"));
            this.mockWordDocument.Setup(doc => doc.AllContentControls()).Returns(new ContentControl[] { cc.Object });
            QueryWorkItems[] qwi = TestHelper.CreateQueryWorkItems(new int[] { 1 });
            LayoutInformation[] layouts = new LayoutInformation[] { this.SetupTestLayout() };
            bool[] queryIsFlat = Enumerable.Repeat<bool>(true, 1).ToArray();

            // Act
            FormatterRefreshData refreshData = new FormatterRefreshData { WorkItemManager = wimgr, QueryWorkItemsBefore = qwi, QueryWorkItemsAfter = qwi, Layouts = layouts, QueryIsFlat = queryIsFlat };
            this.sut.RefreshWorkItems(refreshData, TestHelper.DummyBookmarkNamingFunc, new CancellationToken());

            // Assert
            this.mockWordDocument.Verify(doc => doc.PopulateRichTextContentControlWithHtml(It.IsAny<ContentControl>(), string.Empty), Times.Once(), "Content control should have been emptied");
        }

        /// <summary>
        /// Test that <see cref="TeamProjectDocument.RefreshWorkItems"/> deletes work items which are no longer returned by any queries.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void RefreshWorkItemsDeletesWorkItemsWhichAreNoLongerReturnedByAnyQueries()
        {
            // Arrange & Act
            this.TestRefreshBeforeAndAfter(new int[] { 1, 2, 3 }, new int[] { 1, 3 });

            // Assert
            this.mockWordDocument.Verify(doc => doc.DeleteBookmarkAndContent(TestHelper.DummyBookmarkNamingFunc(0, 2)), Times.Once(), "Work item not deleted");
            this.mockWordDocument.Verify(doc => doc.DeleteBookmarkAndContent(It.IsAny<string>()), Times.Once(), "Only the one work item should be deleted");
        }

        /// <summary>
        /// Test that <see cref="TeamProjectDocument.RefreshWorkItems"/> deletes work items which are no longer returned by specific queries only for those queries.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void RefreshWorkItemsDeletesWorkItemsWhichAreNoLongerReturnedBySpecificQueriesOnlyForThoseQueries()
        {
            // Arrange
            TestHelper.PopulateWorkItemTreeWithHierarchicalQueryResults(this.workItems, "1", "1.2", "3");
            this.CreateMockDefaultBuildingBlock();
            this.CreateMockContentControlsAndAddToDocument();
            LayoutInformation testLayout = this.SetupTestLayout();
            this.sut.MapWorkItemsIntoDocument(this.workItems, testLayout, TestHelper.DummyBookmarkNamingFuncQuery0, new CancellationToken());
            this.sut.MapWorkItemsIntoDocument(this.workItems, testLayout, TestHelper.DummyBookmarkNamingFuncQuery1, new CancellationToken());
            LayoutInformation[] layouts = new LayoutInformation[] { testLayout, testLayout };
            bool[] queryIsFlat = Enumerable.Repeat<bool>(false, 2).ToArray();

            QueryWorkItems[] before = TestHelper.CreateQueryWorkItems(new int[] { 1, 2, 3 }, new int[] { 1, 2, 3 });
            QueryWorkItems[] after = TestHelper.CreateQueryWorkItems(new int[] { 1, 3 }, new int[] { 1, 2 });

            // Act
            FormatterRefreshData refreshData = new FormatterRefreshData { WorkItemManager = new WorkItemManager(), QueryWorkItemsBefore = before, QueryWorkItemsAfter = after, Layouts = layouts, QueryIsFlat = queryIsFlat };
            this.sut.RefreshWorkItems(refreshData, TestHelper.DummyBookmarkNamingFunc, new CancellationToken());

            // Assert
            this.mockWordDocument.Verify(doc => doc.DeleteBookmarkAndContent(TestHelper.DummyBookmarkNamingFuncQuery0(2)), Times.Once(), "Work item not deleted");
            this.mockWordDocument.Verify(doc => doc.DeleteBookmarkAndContent(TestHelper.DummyBookmarkNamingFuncQuery1(3)), Times.Once(), "Work item not deleted");
            this.mockWordDocument.Verify(doc => doc.DeleteBookmarkAndContent(It.IsAny<string>()), Times.Exactly(2), "Only the specific work items for each query should be deleted");
        }

        /// <summary>
        /// Test that <see cref="TeamProjectDocument.RefreshWorkItems"/> leaves work items in original order when deleting work items.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void RefreshWorkItemsPreservesOrderWhenDeletingWorkItems()
        {
            this.TestRefreshBeforeAndAfter(new string[] { "1", "1.3", "2" }, new string[] { "1", "1.3" }, new int[] { 1, 3 }, null);
        }

        /// <summary>
        /// Test that <see cref="TeamProjectDocument.RefreshWorkItems"/> reorders work items if the query order changes and there has been no manual sorting.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void RefreshWorkItemsReordersWorkItemsIfQueryOrderChangesAndNoManualSorting()
        {
            this.TestRefreshBeforeAndAfter(new int[] { 1, 2, 3 }, new int[] { 3, 2, 1 });
        }

        /// <summary>
        /// Test that <see cref="TeamProjectDocument.RefreshWorkItems"/> reorders work items if the query order changes and preceding work items are also deleted.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void RefreshWorkItemsReordersWorkItemsIfQueryOrderChangesAndPrecedingWorkItemsAreAlsoDeleted()
        {
            this.TestRefreshBeforeAndAfter(new int[] { 1, 2, 3, 4, 5, 6, 7 }, new int[] { 4, 5, 3, 7, 6 });
        }

        /// <summary>
        /// Test that <see cref="TeamProjectDocument.RefreshWorkItems"/> reorders work items if the query order changes and following work items are also deleted.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void RefreshWorkItemsReordersWorkItemsIfQueryOrderChangesAndFollowingWorkItemsAreAlsoDeleted()
        {
            this.TestRefreshBeforeAndAfter(new int[] { 1, 2, 3, 4, 5, 6, 7 }, new int[] { 4, 5, 3, 2, 1 });
        }

        /// <summary>
        /// Test that <see cref="TeamProjectDocument.RefreshWorkItems"/> reorders work items if the query order changes and some mid-sequence work items are also deleted.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void RefreshWorkItemsReordersWorkItemsIfQueryOrderChangesAndSomeMidSequenceWorkItemsAreAlsoDeleted()
        {
            this.TestRefreshBeforeAndAfter(new int[] { 1, 2, 3, 4, 5, 6, 7 }, new int[] { 7, 4, 5, 3, 1 });
        }

        /// <summary>
        /// Test that <see cref="TeamProjectDocument.RefreshWorkItems"/> reorders work items if the query order changes and some mid-sequence work items are also added.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void RefreshWorkItemsReordersWorkItemsIfQueryOrderChangesAndSomeMidSequenceWorkItemsAreAlsoAdded()
        {
            this.TestRefreshBeforeAndAfter(new int[] { 1, 3, 4, 5, 6, 7 }, new int[] { 7, 4, 5, 3, 2, 1 });
        }

        /// <summary>
        /// Test that <see cref="TeamProjectDocument.RefreshWorkItems"/> adds new work items into an existing sequence in query order if there has been no manual sorting.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void RefreshWorkItemsAddsNewWorkItemsIntoAnExistingSequenceInQueryOrderIfNoManualSorting()
        {
            this.TestRefreshBeforeAndAfter(new int[] { 3, 6 }, new int[] { 1, 2, 3, 4, 5, 6, 7, 8 });
        }

        /// <summary>
        /// Test that <see cref="TeamProjectDocument.RefreshWorkItems"/> can work when all old items are deleted and replaced with all new ones.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void RefreshWorkItemsWhenAllOldOnesDeletedAndReplacedWithAllNewOnes()
        {
            this.TestRefreshBeforeAndAfter(new int[] { 1, 2, 3, 4 }, new int[] { 5, 6, 7, 8 });
        }

        /// <summary>
        /// Test that <see cref="TeamProjectDocument.RefreshWorkItems"/> can work when all old items are deleted in one refresh and replaced with all new ones in a later refresh.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void RefreshWorkItemsWhenAllOldOnesDeletedInOneRefreshAndReplacedWithAllNewOnesInALaterRefresh()
        {
            // Arrange
            ITfsWorkItem[] mockItems = this.SetupDocumentWithSavedWorkItemsFromFlatQuery(1, 2, 3, 4);
            LayoutInformation[] layouts = new LayoutInformation[] { this.SetupTestLayout() };
            bool[] queryIsFlat = Enumerable.Repeat<bool>(true, 1).ToArray();

            QueryWorkItems[] before = TestHelper.CreateQueryWorkItems(new int[] { 1, 2, 3, 4 });
            QueryWorkItems[] after = TestHelper.CreateQueryWorkItems(new int[0]);
            FormatterRefreshData refreshData = new FormatterRefreshData { WorkItemManager = new WorkItemManager(), QueryWorkItemsBefore = before, QueryWorkItemsAfter = after, Layouts = layouts, QueryIsFlat = queryIsFlat };
            this.sut.RefreshWorkItems(refreshData, TestHelper.DummyBookmarkNamingFunc, new CancellationToken());

            before = TestHelper.CreateQueryWorkItems(new int[0]);
            WorkItemTree workItemTree = new WorkItemTree();
            TestHelper.PopulateWorkItemTreeWithFlatQueryResults(workItemTree, 5, 6, 7, 8);

            after = TestHelper.CreateQueryWorkItems(workItemTree);

            // Act
            refreshData = new FormatterRefreshData { WorkItemManager = new WorkItemManager(), QueryWorkItemsBefore = before, QueryWorkItemsAfter = after, Layouts = layouts, QueryIsFlat = queryIsFlat };
            this.sut.RefreshWorkItems(refreshData, TestHelper.DummyBookmarkNamingFunc, new CancellationToken());

            // Assert
            this.VerifyPhysicalOrderOfWorkItems(5, 6, 7, 8);
        }

        /// <summary>
        /// Test that <see cref="TeamProjectDocument.RefreshWorkItems"/> can add, delete and sort all in one operation.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void RefreshWorkItemsCanAddDeleteAndSortAllInOneOperation()
        {
            this.TestRefreshBeforeAndAfter(new int[] { 2, 3, 5, 7, 8 }, new int[] { 1, 5, 3, 6, 9, 10 });
        }

        /// <summary>
        /// Test that <see cref="TeamProjectDocument.RefreshWorkItems"/> does not change the order of flat query items that have been manually sorted when there are no new work items.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void RefreshWorkItemsDoesNotChangeOrderForFlatQueryIfItemsManuallySortedAndNoItemsAdded()
        {
            this.TestRefreshBeforeAndAfter(new int[] { 1, 2, 3 }, new int[] { 1, 2, 3 }, new int[] { 1, 3, 2 }, () => this.MovePhysicalItemToBefore(3, 2));
        }

        /// <summary>
        /// Test that <see cref="TeamProjectDocument.RefreshWorkItems"/> does not change the order of flat query items that have been manually sorted when there are no new work items.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void RefreshWorkItemsAddsNewItemsAtTheEndForFlatQueryIfItemsManuallySorted()
        {
            this.TestRefreshBeforeAndAfter(new int[] { 1, 2, 3 }, new int[] { 4, 5, 1, 6, 2, 7, 3, 8 }, new int[] { 1, 3, 2, 4, 5, 6, 7, 8 }, () => this.MovePhysicalItemToBefore(3, 2));
        }

        /// <summary>
        /// Test that <see cref="TeamProjectDocument.RefreshWorkItems"/> sorts the work items again for a hierarchical query if they have been manually sorted even if the query results are unchanged.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void RefreshWorkItemsSortsWorkItemsForHierarchyQueryIfItemsManuallySortedEvenIfQueryResultsUnchanged()
        {
            this.TestRefreshBeforeAndAfter(new string[] { "1", "2.3", "2.4", "5" }, new string[] { "1", "2.3", "2.4", "5" }, new int[] { 1, 2, 3, 4, 5 }, () => this.MovePhysicalItemToBefore(4, 3));
        }

        /// <summary>
        /// Test that <see cref="TeamProjectDocument.RefreshWorkItems"/> sorts the work items again for a hierarchical query if they have been manually sorted and adds new items in sorted order.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void RefreshWorkItemsSortsWorkItemsForHierarchyQueryIfItemsManuallySortedAndAddsNewItemsInSortedOrder()
        {
            this.TestRefreshBeforeAndAfter(new string[] { "1", "2.3", "2.4", "5" }, new string[] { "1", "2.6", "2.3", "2.7", "2.4", "2.8", "5" }, new int[] { 1, 2, 6, 3, 7, 4, 8, 5 }, () => this.MovePhysicalItemToBefore(4, 3));
        }

        /// <summary>
        /// Test that <see cref="TeamProjectDocument.RefreshWorkItems"/> sorts the work items again for a hierarchical query if they have been manually sorted and the moved item gets children it did not have before.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void RefreshWorkItemsSortsWorkItemsForHierarchyQueryIfManuallySortedItemGetsChildren()
        {
            this.TestRefreshBeforeAndAfter(new string[] { "1", "2", "3", "6", "7" }, new string[] { "1", "2", "3", "3.4", "3.5", "6", "7" }, new int[] { 1, 2, 3, 4, 5, 6, 7 }, () => this.MovePhysicalItemToBefore(3, 1));
        }

        /// <summary>
        /// Test that <see cref="TeamProjectDocument.RefreshWorkItems"/> for a flat query refreshes work items even if bookmarks have been deleted, adds back deleted work items.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void RefreshWorkItemsForFlatQueryRefreshesEvenIfBookmarksDeletedAddsBackDeletedWorkItems()
        {
            this.TestRefreshBeforeAndAfter(new string[] { "1", "2", "3", "4", "5", "6", "7" }, new string[] { "1", "2", "3", "4", "5", "6", "7" }, new int[] { 1, 2, 3, 4, 5, 6, 7 }, () => this.DeleteBookmarksForWorkItem(1, 3, 7));
        }

        /// <summary>
        /// Test that <see cref="TeamProjectDocument.RefreshWorkItems"/> for a hierarchical query refreshes work items even if bookmarks have been deleted, adds back deleted work items.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void RefreshWorkItemsForHierarchicalQueryRefreshesEvenIfBookmarksDeletedAddsBackDeletedWorkItems()
        {
            this.TestRefreshBeforeAndAfter(new string[] { "1", "1.2", "1.3", "4", "4.5", "4.6", "4.7" }, new string[] { "1", "1.2", "1.3", "4", "4.5", "4.6", "4.7" }, new int[] { 1, 2, 3, 4, 5, 6, 7 }, () => this.DeleteBookmarksForWorkItem(1, 3, 7));
        }

        /// <summary>
        /// Test that <see cref="TeamProjectDocument.RefreshWorkItems"/> uses layout associated with query when adding new work items into an existing sequence.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void RefreshWorkItemsUsesLayoutAssociatedWithQueryWhenAddingNewWorkItemsIntoAnExistingSequence()
        {
            // Arrange
            ITfsWorkItem[] mockItems = TestHelper.PopulateWorkItemTreeWithFlatQueryResults(this.workItems, 1, 3);

            this.CreateMockContentControlsAndAddToDocument();

            BuildingBlock bb1 = this.CreateMockBuildingBlock(BuildingBlockName.Default);
            BuildingBlock bb2 = this.CreateMockBuildingBlock(BuildingBlockName.Default);
            Assert.AreNotSame(bb1, bb2, "Pre-requisite");
            LayoutInformation[] layouts = new LayoutInformation[] { this.SetupTestLayout(TestHelper.ValidLayoutName, bb1), this.SetupTestLayout(TestHelper.ValidLayoutName2, bb2) };
            this.sut.MapWorkItemsIntoDocument(this.workItems, layouts[0], TestHelper.DummyBookmarkNamingFuncQuery0, new CancellationToken());
            this.sut.MapWorkItemsIntoDocument(this.workItems, layouts[1], TestHelper.DummyBookmarkNamingFuncQuery1, new CancellationToken());
            bool[] queryIsFlat = Enumerable.Repeat<bool>(true, 2).ToArray();

            QueryWorkItems[] before = TestHelper.CreateQueryWorkItems(new int[] { 1 }, new int[] { 3 });
            WorkItemTree workItemTree1 = new WorkItemTree();
            TestHelper.PopulateWorkItemTreeWithFlatQueryResults(workItemTree1, 1, 2);
            WorkItemTree workItemTree2 = new WorkItemTree();
            TestHelper.PopulateWorkItemTreeWithFlatQueryResults(workItemTree2, 3, 4);
            QueryWorkItems[] after = TestHelper.CreateQueryWorkItems(workItemTree1, workItemTree2);

            // Act
            FormatterRefreshData refreshData = new FormatterRefreshData { WorkItemManager = new WorkItemManager(), QueryWorkItemsBefore = before, QueryWorkItemsAfter = after, Layouts = layouts, QueryIsFlat = queryIsFlat };
            this.sut.RefreshWorkItems(refreshData, TestHelper.DummyBookmarkNamingFunc, new CancellationToken());

            // Assert
            this.mockWordDocument.Verify(doc => doc.InsertBuildingBlockAfterBookmark(bb1, TestHelper.DummyBookmarkNamingFunc(0, 2), TestHelper.DummyBookmarkNamingFunc(0, 1)), Times.Once(), "Incorrect layout chosen");
            this.mockWordDocument.Verify(doc => doc.InsertBuildingBlockAfterBookmark(bb2, TestHelper.DummyBookmarkNamingFunc(1, 4), TestHelper.DummyBookmarkNamingFunc(1, 3)), Times.Once(), "Incorrect layout chosen");
        }

        /// <summary>
        /// Test that <see cref="TeamProjectDocument.RefreshWorkItems"/> uses building block respecting level in node hierarchy when adding new work items.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void RefreshWorkItemsUseBuildingBlockRespectingLevelInHierarchyWhenAddingNewWorkItems()
        {
            // Arrange
            this.SetupDocumentWithSavedWorkItemsFromFlatQuery(1, 3);
            BuildingBlock bb1 = this.CreateMockBuildingBlock(BuildingBlockName.Default);
            BuildingBlock bb2 = this.CreateMockBuildingBlock(new BuildingBlockName(TestHelper.StandardTestWorkItemType, 1));
            LayoutInformation[] layouts = new LayoutInformation[] { this.SetupTestLayout(TestHelper.ValidLayoutName, bb1, bb2) };
            bool[] queryIsFlat = Enumerable.Repeat<bool>(false, 1).ToArray();

            QueryWorkItems[] before = TestHelper.CreateQueryWorkItems(new int[] { 1, 3 });
            WorkItemTree workItemTree = new WorkItemTree();
            TestHelper.PopulateWorkItemTreeWithHierarchicalQueryResults(workItemTree, "1", "1.2", "3");
            QueryWorkItems[] after = TestHelper.CreateQueryWorkItems(workItemTree);

            // Act
            FormatterRefreshData refreshData = new FormatterRefreshData { WorkItemManager = new WorkItemManager(), QueryWorkItemsBefore = before, QueryWorkItemsAfter = after, Layouts = layouts, QueryIsFlat = queryIsFlat };
            this.sut.RefreshWorkItems(refreshData, TestHelper.DummyBookmarkNamingFunc, new CancellationToken());

            // Assert
            this.mockWordDocument.Verify(doc => doc.InsertBuildingBlockBeforeBookmark(bb2, TestHelper.DummyBookmarkNamingFunc(0, 2), TestHelper.DummyBookmarkNamingFunc(0, 3)), Times.Once(), "Incorrect building block chosen");
        }

        /// <summary>
        /// Test that <see cref="TeamProjectDocument.RefreshWorkItems"/> maps newly added work items to the custom XML.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void RefreshWorkItemsMapsNewlyAddedWorkItemsToTheCustomXml()
        {
            // Arrange
            this.SetupDocumentWithSavedWorkItemsFromFlatQuery(1, 3);
            CustomXMLPart xmlPart = this.CreateMockXmlPartForWorkItemsAndAddToDocument();
            BuildingBlock bb1 = this.CreateMockBuildingBlock(BuildingBlockName.Default);
            BuildingBlock bb2 = this.CreateMockBuildingBlock(new BuildingBlockName(TestHelper.StandardTestWorkItemType, 1));
            ContentControl[] contentControls = this.CreateMockContentControlsAndAddToDocument(FieldId);
            LayoutInformation[] layouts = new LayoutInformation[] { this.SetupTestLayout(TestHelper.ValidLayoutName, bb1, bb2) };
            bool[] queryIsFlat = Enumerable.Repeat<bool>(false, 1).ToArray();

            QueryWorkItems[] before = TestHelper.CreateQueryWorkItems(new int[] { 1, 3 });
            WorkItemTree workItemTree = new WorkItemTree();
            TestHelper.PopulateWorkItemTreeWithHierarchicalQueryResults(workItemTree, "1", "1.2", "3");
            QueryWorkItems[] after = TestHelper.CreateQueryWorkItems(workItemTree);

            // Act
            FormatterRefreshData refreshData = new FormatterRefreshData { WorkItemManager = new WorkItemManager(), QueryWorkItemsBefore = before, QueryWorkItemsAfter = after, Layouts = layouts, QueryIsFlat = queryIsFlat };
            this.sut.RefreshWorkItems(refreshData, TestHelper.DummyBookmarkNamingFunc, new CancellationToken());

            // Assert
            this.VerifyContentControlMapping(contentControls[0], 2, FieldId, xmlPart);
        }

        /// <summary>
        /// Test that <see cref="TeamProjectDocumentFormatter.MapWorkItemsIntoDocument"/> inserts boilerplate text before the import if the insertion location is at the start of the document.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void MapWorkItemsIntoDocumentAddsBoilerplateTextBeforeImportIfInsertAtStartOfDocument()
        {
            // Arrange
            this.AddTestWorkItem(1, "Item1");
            this.CreateMockXmlPartForWorkItemsAndAddToDocument();
            this.CreateMockDefaultBuildingBlock();
            this.CreateMockContentControlsAndAddToDocument();
            LayoutInformation testLayout = this.SetupTestLayout();

            this.mockWordDocument.Setup(document => document.IsAtStart()).Returns(true);
            this.mockWordDocument.Setup(document => document.IsAtEnd()).Returns(false);

            // Act
            this.sut.MapWorkItemsIntoDocument(this.workItems, testLayout, TestHelper.DummyBookmarkNamingFuncQuery0, new CancellationToken());

            // Assert
            this.mockWordDocument.AssertParagraphInserted("Start of import (added this text because the import was at the start and it can be difficult to insert text before the import, delete this text if you wish)", "Normal");
            this.mockWordDocument.AssertBuildingBlockInserted(this.mockDefaultBuildingBlock, TestHelper.DummyBookmarkNamingFuncQuery0(1));
            this.mockWordDocument.AssertEndOfDocument();
        }

        /// <summary>
        /// Test that <see cref="TeamProjectDocumentFormatter.MapWorkItemsIntoDocument"/> inserts boilerplate text after the import if the insertion location is at the end of the document.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void MapWorkItemsIntoDocumentAddsBoilerplateTextAfterImportIfInsertAtEndOfDocument()
        {
            // Arrange
            this.AddTestWorkItem(1, "Item1");
            this.CreateMockXmlPartForWorkItemsAndAddToDocument();
            this.CreateMockDefaultBuildingBlock();
            this.CreateMockContentControlsAndAddToDocument();
            LayoutInformation testLayout = this.SetupTestLayout();

            this.mockWordDocument.Setup(document => document.IsAtStart()).Returns(false);
            this.mockWordDocument.Setup(document => document.IsAtEnd()).Returns(true);

            // Act
            this.sut.MapWorkItemsIntoDocument(this.workItems, testLayout, TestHelper.DummyBookmarkNamingFuncQuery0, new CancellationToken());

            // Assert
            this.mockWordDocument.AssertBuildingBlockInserted(this.mockDefaultBuildingBlock, TestHelper.DummyBookmarkNamingFuncQuery0(1));
            this.mockWordDocument.AssertParagraphInserted("End of import (added this text because the import was at the end and it can be difficult to insert text after the import, delete this text if you wish)", "Normal");
            this.mockWordDocument.AssertEndOfDocument();
        }

        /// <summary>
        /// Constructs a tag for a work item field and id that goes on a rich text content control.
        /// </summary>
        /// <param name="fieldName">The name of the work item field.</param>
        /// <param name="id">The id of the work item.</param>
        /// <returns>The tag to use for the field and id combination.</returns>
        private static string ConstructTagForWorkItemFieldAndId(string fieldName, string id)
        {
            return fieldName + "-" + id;
        }

        /// <summary>
        /// Creates a mock content control with a specific tag.
        /// </summary>
        /// <param name="type">The type of content control.</param>
        /// <param name="tag">The tag to associate with the mock content control.</param>
        /// <returns>The new mock content control.</returns>
        private static Mock<ContentControl> CreateMockContentControl(WdContentControlType type, string tag)
        {
            Mock<ContentControl> mockContentControl = new Mock<ContentControl>();
            mockContentControl.SetupProperty(cc => cc.Tag, tag);
            mockContentControl.Setup(cc => cc.Type).Returns(type);
            return mockContentControl;
        }

        /// <summary>
        /// Computes the expected XPath for a particular work item and field
        /// </summary>
        /// <param name="expectedWorkItemId">The id of the work item to compute the XPath for.</param>
        /// <param name="expectedFieldName">The name of the field in the work item to compute the XPath for.</param>
        /// <returns>The expected XPath.</returns>
        private static string ExpectedXPathForWorkItemAndField(int expectedWorkItemId, string expectedFieldName)
        {
            return "/wi:WorkItems/wi:WorkItem[wi:Field[@name='" + Constants.SystemIdFieldReferenceName + "']=" + expectedWorkItemId.ToString() + "]/wi:Field[@name='" + expectedFieldName + "']";
        }

        /// <summary>
        /// Deletes the bookmark for the given work item in query 0.
        /// </summary>
        /// <param name="workItemIds">The ids of the work items for which the bookmark is to be deleted.</param>
        private void DeleteBookmarksForWorkItem(params int[] workItemIds)
        {
            foreach (int id in workItemIds)
            {
                this.mockWordDocument.Object.DeleteBookmarkAndContent(TestHelper.DummyBookmarkNamingFuncQuery0(id));
            }
        }

        /// <summary>
        /// Physically moves a work item in the document, to simulate the manual sorting of work items.
        /// </summary>
        /// <param name="workItemIdToMove">The id of the work item to move.</param>
        /// <param name="relativeWorkItemId">The id of the work item before which the moved work item should now appear.</param>
        private void MovePhysicalItemToBefore(int workItemIdToMove, int relativeWorkItemId)
        {
            this.mockWordDocument.MoveBookmarkAndContentToBefore(TestHelper.DummyBookmarkNamingFuncQuery0(workItemIdToMove), TestHelper.DummyBookmarkNamingFuncQuery0(relativeWorkItemId));
        }

        /// <summary>
        /// Tests a simple refresh with a sequence of work items before the refresh and a sequence of work items after the refresh.
        /// </summary>
        /// <param name="beforeIds">The sequence of work items before the refresh.</param>
        /// <param name="afterIds">The sequence of work items after the refresh.</param>
        private void TestRefreshBeforeAndAfter(int[] beforeIds, int[] afterIds)
        {
            this.TestRefreshBeforeAndAfter(beforeIds, afterIds, afterIds, () => { });
        }

        /// <summary>
        /// Tests a simple refresh with a sequence of work items before the refresh and a sequence of work items after the refresh.
        /// </summary>
        /// <param name="beforeIdsInQueryOrder">The sequence of work items before the refresh, in the order returned by the query.</param>
        /// <param name="afterIdsInQueryOrder">The sequence of work items after the refresh, in the order returned by the query.</param>
        /// <param name="afterIdsInExpectedPhysicalOrder">The expected physical sequence after the refresh.</param>
        /// <param name="preRefreshAction">An action to execute after the document has been setup and before doing the refresh.</param>
        private void TestRefreshBeforeAndAfter(int[] beforeIdsInQueryOrder, int[] afterIdsInQueryOrder, int[] afterIdsInExpectedPhysicalOrder, Action preRefreshAction)
        {
            this.TestRefreshBeforeAndAfter(beforeIdsInQueryOrder.Select(id => id.ToString()).ToArray(), afterIdsInQueryOrder.Select(id => id.ToString()).ToArray(), afterIdsInExpectedPhysicalOrder, preRefreshAction);
        }

        /// <summary>
        /// Tests a simple refresh with a sequence of work items before the refresh and a sequence of work items after the refresh.
        /// </summary>
        /// <param name="beforeIdsInQueryOrder">The sequence of work items before the refresh, in the order returned by the query.</param>
        /// <param name="afterIdsInQueryOrder">The sequence of work items after the refresh, in the order returned by the query.</param>
        /// <param name="afterIdsInExpectedPhysicalOrder">The expected physical sequence after the refresh.</param>
        /// <param name="preRefreshAction">An action to execute after the document has been setup and before doing the refresh.</param>
        private void TestRefreshBeforeAndAfter(string[] beforeIdsInQueryOrder, string[] afterIdsInQueryOrder, int[] afterIdsInExpectedPhysicalOrder, Action preRefreshAction)
        {
            // Arrange
            ITfsWorkItem[] mockItems = this.SetupDocumentWithSavedWorkItemsFromHierarchicalQuery(beforeIdsInQueryOrder);

            LayoutInformation[] layouts = new LayoutInformation[] { this.SetupTestLayout() };

            QueryWorkItems[] before = TestHelper.CreateQueryWorkItems(mockItems.Select(item => item.Id).ToArray());
            WorkItemTree workItemTree = new WorkItemTree();
            TestHelper.PopulateWorkItemTreeWithHierarchicalQueryResults(workItemTree, afterIdsInQueryOrder);

            QueryWorkItems[] after = TestHelper.CreateQueryWorkItems(workItemTree);

            bool isFlat = workItemTree.DepthFirstNodes().All(n => n.Level == 0);

            if (preRefreshAction != null)
            {
                preRefreshAction();
            }

            // Act
            FormatterRefreshData refreshData = new FormatterRefreshData { WorkItemManager = new WorkItemManager(), QueryWorkItemsBefore = before, QueryWorkItemsAfter = after, Layouts = layouts, QueryIsFlat = new bool[] { isFlat } };
            this.sut.RefreshWorkItems(refreshData, TestHelper.DummyBookmarkNamingFunc, new CancellationToken());

            // Assert
            this.VerifyPhysicalOrderOfWorkItems(afterIdsInExpectedPhysicalOrder);
        }

        /// <summary>
        /// Verifies the order of the work items in the document.
        /// </summary>
        /// <param name="expectedWorkItemIdsInPhysicalOrder">The work item ids in the physical order in which they are expected.</param>
        private void VerifyPhysicalOrderOfWorkItems(params int[] expectedWorkItemIdsInPhysicalOrder)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("\r\nExpected bookmark order:");
            foreach (int id in expectedWorkItemIdsInPhysicalOrder)
            {
                sb.AppendFormat("\t{0}", TestHelper.DummyBookmarkNamingFuncQuery0(id));
            }

            sb.AppendLine();

            sb.Append("Actual bookmark order:");
            foreach (string b in this.mockWordDocument.BookmarkNames)
            {
                sb.AppendFormat("\t{0}", b);
            }

            Assert.AreEqual<int>(expectedWorkItemIdsInPhysicalOrder.Length, this.mockWordDocument.BookmarkNames.Count, "Wrong number of bookmarks in the document\n" + sb.ToString());
            for (int i = 0; i < expectedWorkItemIdsInPhysicalOrder.Length; i++)
            {
                Assert.AreEqual<string>(TestHelper.DummyBookmarkNamingFuncQuery0(expectedWorkItemIdsInPhysicalOrder[i]), this.mockWordDocument.BookmarkNames[i], "Bookmark order incorrect\n" + sb.ToString());
            }
        }

        /// <summary>
        /// Sets up the mock document with a layout and initial items stored in <see cref="workItems"/>.
        /// </summary>
        private void SetupDocumentWithLayoutAndInitialItems()
        {
            this.CreateMockDefaultBuildingBlock();
            this.CreateMockContentControlsAndAddToDocument();
            LayoutInformation testLayout = this.SetupTestLayout();
            this.sut.MapWorkItemsIntoDocument(this.workItems, testLayout, TestHelper.DummyBookmarkNamingFuncQuery0, new CancellationToken());
        }

        /// <summary>
        /// Sets up a document with <see cref="n"/> saved work items.
        /// </summary>
        /// <param name="n">The number of work items to add.</param>
        /// <returns>The mock items in work item id order.</returns>
        private ITfsWorkItem[] SetupDocumentWithNSavedWorkItems(int n)
        {
            ITfsWorkItem[] mockItems = TestHelper.PopulateWorkItemTreeWithFlatQueryResults(this.workItems, Enumerable.Range(0, n).ToArray());

            this.SetupDocumentWithLayoutAndInitialItems();

            return mockItems;
        }

        /// <summary>
        /// Sets up a document with a specific set of saved work items from a flat query.
        /// </summary>
        /// <param name="workItemIds">The ids of work items to add.</param>
        /// <returns>The mock items in the order they appear in the document.</returns>
        private ITfsWorkItem[] SetupDocumentWithSavedWorkItemsFromFlatQuery(params int[] workItemIds)
        {
            ITfsWorkItem[] mockItems = TestHelper.PopulateWorkItemTreeWithFlatQueryResults(this.workItems, workItemIds);

            this.SetupDocumentWithLayoutAndInitialItems();

            return mockItems;
        }

        /// <summary>
        /// Sets up a document with a specific set of saved work items from a hierarchical query.
        /// </summary>
        /// <param name="workItemIds">The ids of the work items to add, expressed as a dotted hierarchy of ids.</param>
        /// <returns>The mock items in the order they appear in the document.</returns>
        private ITfsWorkItem[] SetupDocumentWithSavedWorkItemsFromHierarchicalQuery(params string[] workItemIds)
        {
            ITfsWorkItem[] mockItems = TestHelper.PopulateWorkItemTreeWithHierarchicalQueryResults(this.workItems, workItemIds);

            this.SetupDocumentWithLayoutAndInitialItems();

            return mockItems;
        }

        /// <summary>
        /// Sets up a document with a specific set of saved work items from a flat query.
        /// </summary>
        /// <param name="workItems">The work items to add.</param>
        /// <returns>The items in the order they appear in the document.</returns>
        private ITfsWorkItem[] SetupDocumentWithSavedWorkItemsFromFlatQuery(params ITfsWorkItem[] workItems)
        {
            TestHelper.PopulateWorkItemTreeWithFlatQueryResults(this.workItems, workItems);

            this.SetupDocumentWithLayoutAndInitialItems();

            return workItems;
        }

        /// <summary>
        /// Creates and adds a test work item to the test work item tree.
        /// </summary>
        /// <param name="id">The id of the work item.</param>
        /// <param name="title">The title of the work item.</param>
        /// <param name="fieldsAndValues">Name value pairs for the fields in the work item.</param>
        /// <returns>The work item tree node.</returns>
        private WorkItemTreeNode AddTestWorkItem(int id, string title, params Tuple<string, object>[] fieldsAndValues)
        {
            return this.AddTestWorkItem(TestHelper.StandardTestWorkItemType, id, title, fieldsAndValues);
        }

        /// <summary>
        /// Creates and adds a test work item to the test work item tree.
        /// </summary>
        /// <param name="type">The work item type.</param>
        /// <param name="id">The id of the work item.</param>
        /// <param name="title">The title of the work item.</param>
        /// <param name="fieldsAndValues">Name value pairs for the fields in the work item.</param>
        /// <returns>The work item tree node.</returns>
        private WorkItemTreeNode AddTestWorkItem(string type, int id, string title, params Tuple<string, object>[] fieldsAndValues)
        {
            return this.AddTestWorkItem(type, id, title, 0, fieldsAndValues);
        }

        /// <summary>
        /// Creates and adds a test work item to the test work item tree.
        /// </summary>
        /// <param name="type">The work item type.</param>
        /// <param name="id">The id of the work item.</param>
        /// <param name="title">The title of the work item.</param>
        /// <param name="level">The level in the tree, 0 is the root.</param>
        /// <param name="fieldsAndValues">Name value pairs for the fields in the work item.</param>
        /// <returns>The work item tree node.</returns>
        private WorkItemTreeNode AddTestWorkItem(string type, int id, string title, int level, params Tuple<string, object>[] fieldsAndValues)
        {
            ITfsWorkItem item = TestHelper.CreateMockWorkItem(type, id, title, fieldsAndValues);
            WorkItemTreeNode ans = new WorkItemTreeNode(item, level);
            this.workItems.RootNodes.Add(ans);
            return ans;
        }

        /// <summary>
        /// Creates a mock default building block.
        /// </summary>
        private void CreateMockDefaultBuildingBlock()
        {
            this.CreateMockBuildingBlock(BuildingBlockName.Default);
            this.mockDefaultBuildingBlock = this.mockBuildingBlocks.Last();
        }

        /// <summary>
        /// Creates a mock building block.
        /// </summary>
        /// <param name="name">The name of the building block to create.</param>
        /// <returns>The created building block</returns>
        private BuildingBlock CreateMockBuildingBlock(BuildingBlockName name)
        {
            BuildingBlock ans = TestHelper.CreateMockBuildingBlock(name);
            this.mockBuildingBlocks.Add(ans);
            this.mockBuildingBlockNames.Add(name);
            return ans;
        }

        /// <summary>
        /// Sets up a test layout using the accumulated building block information that has been set up.
        /// </summary>
        /// <returns>The layout information for the layout.</returns>
        private LayoutInformation SetupTestLayout()
        {
            LayoutInformation testLayout = TestHelper.CreateTestLayoutInformation(this.mockBuildingBlockNames.ToArray());
            TestHelper.SetupMockTemplateWithBuildingBlocks(this.mockTeamProjectTemplate, testLayout, this.mockBuildingBlocks.ToArray());

            return testLayout;
        }

        /// <summary>
        /// Sets up a test layout using the building blocks supplied.
        /// </summary>
        /// <param name="layoutName">The name of the layout to create.</param>
        /// <param name="buildingBlocks">The building blocks to use to create the layout.</param>
        /// <returns>The layout information for the layout.</returns>
        private LayoutInformation SetupTestLayout(string layoutName, params BuildingBlock[] buildingBlocks)
        {
            LayoutInformation testLayout = TestHelper.CreateTestLayoutInformation(layoutName, buildingBlocks.Select(bb => new BuildingBlockName(bb)).ToArray());
            TestHelper.SetupMockTemplateWithBuildingBlocks(this.mockTeamProjectTemplate, testLayout, buildingBlocks);

            return testLayout;
        }

        /// <summary>
        /// Creates a set of mock content controls (all plain text) and sets them up for return on building block insertion.
        /// </summary>
        /// <param name="tags">The tags of the content controls to return when inserting a building block.</param>
        /// <returns>The array of content controls created.</returns>
        private ContentControl[] CreateMockContentControlsAndAddToDocument(params string[] tags)
        {
            return this.CreateMockContentControlsAndAddToDocument(WdContentControlType.wdContentControlText, tags);
        }

        /// <summary>
        /// Creates a set of mock content controls and sets them up for return on building block insertion.
        /// </summary>
        /// <param name="contentControlType">The type of the content controls to create.</param>
        /// <param name="tags">The tags of the content controls to return when inserting a building block.</param>
        /// <returns>The array of content controls created.</returns>
        private ContentControl[] CreateMockContentControlsAndAddToDocument(WdContentControlType contentControlType, params string[] tags)
        {
            List<ContentControl> contentControls = new List<ContentControl>();
            foreach (string tag in tags)
            {
                Mock<ContentControl> mockContentControl = CreateMockContentControl(contentControlType, tag);
                contentControls.Add(mockContentControl.Object);
            }

            this.mockWordDocument.Setup(document => document.InsertBuildingBlock(It.IsAny<BuildingBlock>(), It.IsAny<string>())).Callback((BuildingBlock bb, string bookmark) => this.mockWordDocument.InsertBuildingBlockCallback(bb, bookmark)).Returns(contentControls);

            this.mockWordDocument.Setup(document => document.InsertBuildingBlockBeforeBookmark(It.IsAny<BuildingBlock>(), It.IsAny<string>(), It.IsAny<string>())).Callback((BuildingBlock bb, string bookmark, string relativeBookmark) => this.mockWordDocument.InsertBuildingBlockBeforeBookmarkCallback(bookmark, relativeBookmark)).Returns(contentControls);

            this.mockWordDocument.Setup(document => document.InsertBuildingBlockAfterBookmark(It.IsAny<BuildingBlock>(), It.IsAny<string>(), It.IsAny<string>())).Callback((BuildingBlock bb, string bookmark, string relativeBookmark) => this.mockWordDocument.InsertBuildingBlockAfterBookmarkCallback(bookmark, relativeBookmark)).Returns(contentControls);

            return contentControls.ToArray();
        }

        /// <summary>
        /// Creates a mock XML part and sets it up in the mock document for return for the work items.
        /// </summary>
        /// <returns>The mock XML part.</returns>
        private CustomXMLPart CreateMockXmlPartForWorkItemsAndAddToDocument()
        {
            Mock<CustomXMLPart> mockXmlPart = new Mock<CustomXMLPart>();
            this.mockWordDocument.Setup(document => document.GetXmlPart(Constants.WorkItemNamespace)).Returns(mockXmlPart.Object);
            return mockXmlPart.Object;
        }

        /// <summary>
        /// Verifies that a content control has been correctly mapped to the work item field.
        /// </summary>
        /// <param name="expectedContentControl">The content control that is expected to have been passed to <see cref="MapContentControl"/>.</param>
        /// <param name="expectedWorkItemId">The expected work item id used in the xpath to search the work items XML.</param>
        /// <param name="expectedFieldName">The expected field name used to construct the xpath query.</param>
        /// <param name="expectedXmlPart">The custom XML part that is expected to have been passed to <see cref="MapContentControl"/>.</param>
        private void VerifyContentControlMapping(ContentControl expectedContentControl, int expectedWorkItemId, string expectedFieldName, CustomXMLPart expectedXmlPart)
        {
            this.mockWordDocument.Verify(document => document.MapContentControl(expectedContentControl, ExpectedXPathForWorkItemAndField(expectedWorkItemId, expectedFieldName), "xmlns:wi='" + Constants.WorkItemNamespace + "'", expectedXmlPart), Times.Once(), "The work item field has not been mapped correctly.");
        }

        /// <summary>
        /// Verifies that a content control has not been mapped to the work item field.
        /// </summary>
        /// <param name="contentControl">The content control that is not expected to have been passed to <see cref="MapContentControl"/>.</param>
        /// <param name="workItemId">The work item id that is not expected.</param>
        /// <param name="fieldName">The name of the field that is not expected.</param>
        private void VerifyContentControlNotMapped(ContentControl contentControl, int workItemId, string fieldName)
        {
            this.mockWordDocument.Verify(document => document.MapContentControl(contentControl, ExpectedXPathForWorkItemAndField(workItemId, fieldName), "xmlns:wi='" + Constants.WorkItemNamespace + "'", It.IsAny<CustomXMLPart>()), Times.Never(), "The work item field should not have been mapped.");
            this.mockWordDocument.Verify(document => document.PopulateRichTextContentControlWithHtml(contentControl, It.IsAny<string>()), Times.Never(), "The work item field should not have been mapped.");
        }

        /// <summary>
        /// Verifies that a content control has not been mapped to the work item.
        /// </summary>
        /// <param name="expectedContentControl">The content control that is expected not to have been passed to <see cref="MapContentControl"/>.</param>
        /// <param name="expectedWorkItemId">The expected work item id used in the xpath to search the work items XML.</param>
        /// <param name="expectedFieldName">The expected field name used to construct the xpath query.</param>
        /// <param name="expectedXmlPart">The custom XML part that is expected to have been passed to <see cref="MapContentControl"/>.</param>
        private void VerifyContentControlNotMapped(ContentControl expectedContentControl, int expectedWorkItemId, string expectedFieldName, CustomXMLPart expectedXmlPart)
        {
            this.mockWordDocument.Verify(document => document.MapContentControl(expectedContentControl, "/wi:WorkItems/wi:WorkItem[wi:System.Id=" + expectedWorkItemId.ToString() + "]/wi:" + expectedFieldName, "xmlns:wi='" + Constants.WorkItemNamespace + "'", expectedXmlPart), Times.Never(), "The work item has been mapped when it should not.");
        }

        /// <summary>
        /// Verifies that a rich text content control has been correctly mapped to the work item field.
        /// </summary>
        /// <param name="expectedContentControl">The content control that is expected to have been passed to <see cref="IWordDocument.PopulateRichTextContentControlWithHtml"/>.</param>
        /// <param name="expectedContent">The content that is expected to have been passed to <see cref="IWordDocument.PopulateRichTextContentControlWithHtml"/>.</param>
        private void VerifyContentControlMapping(ContentControl expectedContentControl, string expectedContent)
        {
            this.mockWordDocument.Verify(document => document.PopulateRichTextContentControlWithHtml(expectedContentControl, expectedContent), Times.Once(), "The work item field has not been mapped correctly.");
        }

        /// <summary>
        /// Tests the mapping of a content control to work item field.
        /// </summary>
        /// <param name="contentControlType">The type of the content control.</param>
        /// <param name="mapping">The type of mapping expected.</param>
        private void TestWorkItemFieldToContentControlMapping(WdContentControlType contentControlType, MappingType mapping)
        {
            this.TestWorkItemFieldToContentControlMapping(contentControlType, mapping, true, FieldTest, "test");
        }

        /// <summary>
        /// Tests the mapping of a content control to work item field.
        /// </summary>
        /// <param name="contentControlType">The type of the content control.</param>
        /// <param name="mapping">The type of mapping expected.</param>
        /// <param name="fieldPresentInWorkItem"><c>True</c> if the field is to be defined in the work item. <c>false</c> otherwise</param>
        /// <param name="testFieldName">The name of the field to be tested against.</param>
        /// <param name="testValue">The value to be set for the field.</param>
        private void TestWorkItemFieldToContentControlMapping(WdContentControlType contentControlType, MappingType mapping, bool fieldPresentInWorkItem, string testFieldName, object testValue)
        {
            // Arrange
            if (fieldPresentInWorkItem)
            {
                this.AddTestWorkItem(1, "Item1", new Tuple<string, object>(testFieldName, testValue));
            }
            else
            {
                this.AddTestWorkItem(1, "Item1");
            }

            CustomXMLPart xmlPart = this.CreateMockXmlPartForWorkItemsAndAddToDocument();
            this.CreateMockBuildingBlock(new BuildingBlockName("dummy1"));
            this.CreateMockDefaultBuildingBlock();
            ContentControl[] contentControls = this.CreateMockContentControlsAndAddToDocument(contentControlType, testFieldName);
            LayoutInformation testLayout = this.SetupTestLayout();

            // Act
            this.sut.MapWorkItemsIntoDocument(this.workItems, testLayout, TestHelper.DummyBookmarkNamingFuncQuery0, new CancellationToken());

            // Assert
            if (mapping == MappingType.DataBind)
            {
                this.VerifyContentControlMapping(contentControls[0], 1, testFieldName, xmlPart);
            }
            else if (mapping == MappingType.NotMapped)
            {
                this.VerifyContentControlNotMapped(contentControls[0], 1, testFieldName);
            }
            else
            {
                this.VerifyContentControlMapping(contentControls[0], testValue.ToString());
            }
        }
    }
}
