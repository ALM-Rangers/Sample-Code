//---------------------------------------------------------------------
// <copyright file="LayoutDefinitionFormatterTests.cs" company="Microsoft">
//    Copyright (c) Microsoft. All rights reserved.
//    This code is licensed under the MIT License.
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF 
//    ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
//    TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
//    PARTICULAR PURPOSE AND NONINFRINGEMENT.
// </copyright>
// <summary>The LayoutDefinitionFormatterTests type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.Word4Tfs.Library.Test.UnitTest.Model
{
    using System;
    using global::System.Collections.Generic;
    using global::System.Linq;
    using Microsoft.Office.Interop.Word;
    using Microsoft.Practices.Unity;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Word4Tfs.Library.Data;
    using Microsoft.Word4Tfs.Library.Model;
    using Microsoft.Word4Tfs.Library.Model.Word;
    using Moq;

    /// <summary>
    /// Tests the <see cref="LayoutDefinitionFormatter"/> class.
    /// </summary>
    [TestClass]
    [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Unity container disposed in the test cleanup code.")]
    public class LayoutDefinitionFormatterTests
    {
        /// <summary>
        /// Name of a test layout.
        /// </summary>
        private const string Layout1Name = "Layout1";

        /// <summary>
        /// Name of a test layout.
        /// </summary>
        private const string Layout2Name = "Layout2";

        /// <summary>
        /// The Unity container to be used for Dependency Injection
        /// </summary>
        private IUnityContainer container;

        /// <summary>
        /// The mock word document used to test the model.
        /// </summary>
        private Mock<IWordDocument> mockWordDocument;

        /// <summary>
        /// The mock template used to test the model.
        /// </summary>
        private Mock<ITeamProjectTemplate> mockTemplate;

        /// <summary>
        /// The list used to store the mock building blocks for the mock layout.
        /// </summary>
        private List<LayoutInformation> mockLayouts;

        /// <summary>
        /// The list of paragraphs to be returned by the mock word document.
        /// </summary>
        private List<Paragraph> mockParagraphs;

        /// <summary>
        /// The model to be tested.
        /// </summary>
        private ILayoutDefinitionFormatter sut;

        /// <summary>
        /// Initializes the test.
        /// </summary>
        [TestInitialize]
        public void InitializeTest()
        {
            this.container = new UnityContainer();

            this.mockLayouts = new List<LayoutInformation>();
            this.mockParagraphs = new List<Paragraph>();

            this.container.RegisterInstance<ILogger>(new Logger());
            this.mockWordDocument = TestHelper.CreateAndRegisterMock<IWordDocument>(this.container);
            this.mockTemplate = TestHelper.CreateAndRegisterMock<ITeamProjectTemplate>(this.container);
            this.mockWordDocument.Setup(doc => doc.Paragraphs).Returns(this.mockParagraphs);

            this.sut = this.container.Resolve<LayoutDefinitionFormatter>();
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
        /// Test that <see cref="ILayoutDefinitionFormatter.DisplayDefinition"/> throws an exception if template argument is null.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void DisplayDefinitionChecksForNullTemplate()
        {
            // Arrange, Act and Assert
            TestHelper.TestForArgumentNullException(() => this.sut.DisplayDefinition(null, "Test1"), "projectTemplate");
        }

        /// <summary>
        /// Test that <see cref="ILayoutDefinitionFormatter.DisplayDefinition"/> clears the contents of the document before inserting the layout definition.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void DisplayDefinitionMustDeleteAllContentsBeforeInsertingLayoutDefinition()
        {
            // Arrange
            this.CreateTestLayout(Layout1Name, "bb-1");
            int seq = 0;
            this.mockWordDocument.Setup(doc => doc.DeleteAllContents()).Callback(() => Assert.AreEqual<int>(0, seq++, "Delete must be done first"));
            this.mockWordDocument.Setup(doc => doc.InsertParagraph(It.IsAny<string>(), It.IsAny<string>())).Callback(() => Assert.AreEqual<int>(1, seq++, "Delete must have been done first"));

            // Act
            this.sut.DisplayDefinition(this.mockTemplate.Object, Layout1Name);

            // Assert
            this.mockWordDocument.Verify(doc => doc.InsertParagraph(It.IsAny<string>(), It.IsAny<string>()), Times.AtLeastOnce(), "Did not delete or insert content");
        }

        /// <summary>
        /// Test that <see cref="ILayoutDefinitionFormatter.DisplayDefinition"/> inserts a layout definition in the required format.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void DisplayDefinitionInsertsLayoutInRequiredFormat()
        {
            // Arrange
            Queue<Tuple<string, string, string>> expectedInserts = new Queue<Tuple<string, string, string>>();

            this.mockWordDocument.Setup(d => d.InsertParagraph(It.IsAny<string>(), It.IsAny<string>())).Callback((string text, string style) => expectedInserts.Enqueue(new Tuple<string, string, string>("paragraph", text, style)));
            this.mockWordDocument.Setup(d => d.InsertBuildingBlock(It.IsAny<BuildingBlock>(), It.IsAny<string>())).Callback((BuildingBlock bb, string bookmark) => expectedInserts.Enqueue(new Tuple<string, string, string>("buildingblock", bb.Name, bookmark)));
            this.CreateTestLayout(Layout1Name, "bb1-1", "bb1-2");

            // Act
            this.sut.DisplayDefinition(this.mockTemplate.Object, Layout1Name);

            // Assert
            this.AssertParagraphInserted(expectedInserts, "bb1-1", Constants.WorkItemDefinitionStyleName);
            this.AssertBuildingBlockInserted(expectedInserts, "bb1-1");
            this.AssertParagraphInserted(expectedInserts, "bb1-2", Constants.WorkItemDefinitionStyleName);
            this.AssertBuildingBlockInserted(expectedInserts, "bb1-2");
            Assert.AreEqual<int>(0, expectedInserts.Count, "There should not be any other content added");
        }

        /// <summary>
        /// Test that <see cref="ILayoutDefinitionFormatter.SaveDefinition"/> throws exception if a null template is passed in.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void SaveDefinitionThrowsExceptionIfTemplateIsNull()
        {
            TestHelper.TestForArgumentNullException(() => this.sut.SaveDefinition(null, "Dummy"), "projectTemplate");
        }

        /// <summary>
        /// Test that <see cref="ILayoutDefinitionFormatter.SaveDefinition"/> throws exception if no layout name is passed in.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void SaveDefinitionThrowsExceptionIfNoLayoutName()
        {
            TestHelper.TestForArgumentNullException(() => this.sut.SaveDefinition(this.mockTemplate.Object, string.Empty), "layoutName");
        }

        /// <summary>
        /// Test that <see cref="ILayoutDefinitionFormatter.SaveDefinition"/> deletes the existing building blocks for the layout from the template.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void SaveDefinitionDeletesExistingBuildingBlocksForTheLayoutFromTheTemplate()
        {
            // Arrange
            this.CreateTestLayout(Layout1Name, "bb1-1", "bb1-2");
            this.CreateTestLayout(Layout2Name, "bb2-1", "bb2-2");

            // Act
            this.sut.SaveDefinition(this.mockTemplate.Object, Layout1Name);

            // Assert
            this.mockTemplate.Verify(t => t.DeleteLayout(Layout1Name), Times.Once(), "Should delete the layout");
            this.mockTemplate.Verify(t => t.DeleteLayout(Layout2Name), Times.Never(), "Should not delete the layout not being saved");
        }

        /// <summary>
        /// Test that <see cref="ILayoutDefinitionFormatter.SaveDefinition"/> creates a layout.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void SaveDefinitionCreatesALayout()
        {
            // Arrange
            this.SetupMockParagraph(BuildingBlockName.Default.ToString(), Constants.WorkItemDefinitionStyleName);
            Paragraph start = this.SetupMockParagraph("text");
            Paragraph end = this.SetupMockParagraph("Text");

            // Act
            this.sut.SaveDefinition(this.mockTemplate.Object, Layout1Name);

            // Assert
            this.mockTemplate.Verify(t => t.CreateLayout(Layout1Name, new BuildingBlockDefinition[] { new BuildingBlockDefinition(BuildingBlockName.Default.ToString(), start.Range.Start, end.Range.End - 1) }), Times.Once(), "The building block was not saved.");
        }

        /// <summary>
        /// Test that <see cref="ILayoutDefinitionFormatter.SaveDefinitions"/> creates a layout from single paragraph.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void SaveDefinitionsCreatesALayoutFromSingleParagraph()
        {
            // Arrange
            this.SetupMockParagraph(BuildingBlockName.Default.ToString(), Constants.WorkItemDefinitionStyleName);
            Paragraph start = this.SetupMockParagraph("text");

            // Act
            this.sut.SaveDefinition(this.mockTemplate.Object, Layout1Name);

            // Assert
            this.mockTemplate.Verify(t => t.CreateLayout(Layout1Name, new BuildingBlockDefinition[] { new BuildingBlockDefinition(BuildingBlockName.Default.ToString(), start.Range.Start, start.Range.End - 1) }), Times.Once(), "The building block was not saved.");
        }

        /// <summary>
        /// Test that <see cref="ILayoutDefinitionFormatter.SaveDefinitions"/> creates a layout from three paragraphs.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void SaveDefinitionsCreatesALayoutFromThreeParagraphs()
        {
            // Arrange
            this.SetupMockParagraph(BuildingBlockName.Default.ToString(), Constants.WorkItemDefinitionStyleName);
            Paragraph start = this.SetupMockParagraph("text");
            this.SetupMockParagraph("middle");
            Paragraph end = this.SetupMockParagraph("Text");

            // Act
            this.sut.SaveDefinition(this.mockTemplate.Object, Layout1Name);

            // Assert
            this.mockTemplate.Verify(t => t.CreateLayout(Layout1Name, new BuildingBlockDefinition[] { new BuildingBlockDefinition(BuildingBlockName.Default.ToString(), start.Range.Start, end.Range.End - 1) }), Times.Once(), "The building block was not saved.");
        }

        /// <summary>
        /// Test that <see cref="ILayoutDefinitionFormatter.SaveDefinitions"/> creates a layout with more than one block.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void SaveDefinitionsCreatesALayoutWithMoreThanOneBlock()
        {
            // Arrange
            this.SetupMockParagraph(BuildingBlockName.Default.ToString(), Constants.WorkItemDefinitionStyleName);
            Paragraph start1 = this.SetupMockParagraph("text");
            Paragraph end1 = this.SetupMockParagraph("Text");
            this.SetupMockParagraph("Other Block", Constants.WorkItemDefinitionStyleName);
            Paragraph start2 = this.SetupMockParagraph("text");
            Paragraph end2 = this.SetupMockParagraph("Text");

            // Act
            this.sut.SaveDefinition(this.mockTemplate.Object, Layout1Name);

            // Assert
            BuildingBlockDefinition[] blocks = new BuildingBlockDefinition[]
                                                                          {
                                                                          new BuildingBlockDefinition(BuildingBlockName.Default, start1.Range.Start, end1.Range.End),
                                                                          new BuildingBlockDefinition("Other Block", start2.Range.Start, end2.Range.End - 1),
                                                                          };
            this.mockTemplate.Verify(t => t.CreateLayout(Layout1Name, blocks), Times.Once(), "The building block was not saved.");
        }

        /// <summary>
        /// Test that <see cref="ILayoutDefinitionFormatter.SaveDefinitions"/> does not create a layout block from an empty block (no paragraphs).
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void SaveDefinitionsCreatesALayoutWithoutABlockThatHasNoParagraphs()
        {
            // Arrange
            this.SetupMockParagraph("Empty Block", Constants.WorkItemDefinitionStyleName);
            this.SetupMockParagraph(BuildingBlockName.Default.ToString(), Constants.WorkItemDefinitionStyleName);
            Paragraph start = this.SetupMockParagraph("text");
            Paragraph end = this.SetupMockParagraph("Text");

            // Act
            this.sut.SaveDefinition(this.mockTemplate.Object, Layout1Name);

            // Assert
            this.mockTemplate.Verify(t => t.CreateLayout(Layout1Name, new BuildingBlockDefinition[] { new BuildingBlockDefinition(BuildingBlockName.Default.ToString(), start.Range.Start, end.Range.End - 1) }), Times.Once(), "The building block was not saved.");
        }

        /// <summary>
        /// Test that <see cref="ILayoutDefinitionFormatter.SaveDefinitions"/> ignores paragraphs before the first block.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void SaveDefinitionsIgnoresParagraphsBeforeTheFirstBlock()
        {
            // Arrange
            this.SetupMockParagraph("ignore");
            this.SetupMockParagraph(BuildingBlockName.Default.ToString(), Constants.WorkItemDefinitionStyleName);
            Paragraph start = this.SetupMockParagraph("text");
            Paragraph end = this.SetupMockParagraph("Text");

            // Act
            this.sut.SaveDefinition(this.mockTemplate.Object, Layout1Name);

            // Assert
            this.mockTemplate.Verify(t => t.CreateLayout(Layout1Name, new BuildingBlockDefinition[] { new BuildingBlockDefinition(BuildingBlockName.Default.ToString(), start.Range.Start, end.Range.End - 1) }), Times.Once(), "The building block was not saved.");
        }

        /// <summary>
        /// Test that <see cref="ILayoutDefinitionFormatter.AddPrototypeDefinition"/> clears the contents of the document before inserting the prototype layout definition.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void AddPrototypeDefinitionMustDeleteAllContentsBeforeInsertingPrototypeLayoutDefinition()
        {
            // Arrange
            int seq = 0;
            this.mockWordDocument.Setup(doc => doc.DeleteAllContents()).Callback(() => Assert.AreEqual<int>(0, seq++, "Delete must be done first"));
            this.mockWordDocument.Setup(doc => doc.InsertParagraph(It.IsAny<string>(), It.IsAny<string>())).Callback(() => Assert.IsTrue(seq > 0, "Delete must have been done first"));

            // Act
            this.sut.AddPrototypeDefinition();

            // Assert
            this.mockWordDocument.Verify(doc => doc.InsertParagraph(It.IsAny<string>(), It.IsAny<string>()), Times.AtLeastOnce(), "Did not delete or insert content");
        }

        /// <summary>
        /// Test that <see cref="ILayoutDefinitionFormatter.AddPrototypeDefinition"/> inserts a prototype layout definition in the required format.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void AddPrototypeDefinitionInsertsPrototypeLayoutInRequiredFormat()
        {
            // Arrange
            Queue<Tuple<string, string, string>> expectedInserts = new Queue<Tuple<string, string, string>>();

            this.mockWordDocument.Setup(d => d.InsertParagraph(It.IsAny<string>(), It.IsAny<string>())).Callback((string text, string style) => expectedInserts.Enqueue(new Tuple<string, string, string>("paragraph", text, style)));

            // Act
            this.sut.AddPrototypeDefinition();

            // Assert
            this.AssertParagraphInserted(expectedInserts, BuildingBlockName.Default.ToString(), Constants.WorkItemDefinitionStyleName);
            this.AssertParagraphInserted(expectedInserts, "Insert work item layout here. Double-click the fields you require, add any surrounding text and format the content. When formatting content make sure that formatting is done from outside the fields rather than inside, as any formatting inside the fields is lost when the field content is added.", Constants.NormalStyleName);
            Assert.AreEqual<int>(0, expectedInserts.Count, "There should not be any other content added");
        }

        /// <summary>
        /// Asserts that a paragraph was inserted in the call order.
        /// </summary>
        /// <param name="expectedInserts">The queue of inserts</param>
        /// <param name="expectedContent">The expected content.</param>
        /// <param name="expectedStyle">The expected style.</param>
        private void AssertParagraphInserted(Queue<Tuple<string, string, string>> expectedInserts, string expectedContent, string expectedStyle)
        {
            Tuple<string, string, string> item = expectedInserts.Dequeue();
            Assert.AreEqual<string>("paragraph", item.Item1);
            Assert.AreEqual<string>(expectedContent, item.Item2);
            Assert.AreEqual<string>(expectedStyle, item.Item3);
        }

        /// <summary>
        /// Asserts that a building block was inserted in the call order.
        /// </summary>
        /// <param name="expectedInserts">The queue of inserts.</param>
        /// <param name="expectedBuildingblockName">The expected building block name.</param>
        private void AssertBuildingBlockInserted(Queue<Tuple<string, string, string>> expectedInserts, string expectedBuildingblockName)
        {
            Tuple<string, string, string> item = expectedInserts.Dequeue();
            Assert.AreEqual<string>("buildingblock", item.Item1);
            Assert.AreEqual<string>(expectedBuildingblockName, item.Item2);
        }

        /// <summary>
        /// Asserts that a layout heading for the given layout was inserted into the document.
        /// </summary>
        /// <param name="expectedName">The expected name of the layout.</param>
        private void AssertLayoutHeadingInserted(string expectedName)
        {
            this.mockWordDocument.Verify(doc => doc.InsertParagraph(expectedName, "Title"), Times.Once());
        }

        /// <summary>
        /// Creates a test layout and adds it to the list of layouts returned by the template.
        /// </summary>
        /// <param name="layoutName">The name of the layout.</param>
        /// <param name="buildingBlockNames">The names of the building blocks that make up the layout.</param>
        private void CreateTestLayout(string layoutName, params string[] buildingBlockNames)
        {
            BuildingBlockName[] bbns = buildingBlockNames.Select(name => new BuildingBlockName(name)).ToArray();
            LayoutInformation layout = TestHelper.CreateTestLayoutInformation(layoutName, bbns);
            this.mockLayouts.Add(layout);
            TestHelper.SetupMockTemplateWithBuildingBlocks(this.mockTemplate, layout, bbns.Select(bbn => TestHelper.CreateMockBuildingBlock(bbn)).ToArray());
            this.mockTemplate.Setup(t => t.Layouts).Returns(this.mockLayouts);
        }

        /// <summary>
        /// Sets up a mock paragraph to be added to the mock document.
        /// </summary>
        /// <param name="content">The content of the mock paragraph.</param>
        /// <param name="style">The style of the mock paragraph.</param>
        /// <returns>The mock paragraph.</returns>
        private Paragraph SetupMockParagraph(string content, string style)
        {
            int start = this.mockParagraphs.Count * 2;
            Mock<Microsoft.Office.Interop.Word.Range> mockRange = new Mock<Microsoft.Office.Interop.Word.Range>();
            mockRange.Setup(r => r.Start).Returns(start);
            mockRange.Setup(r => r.End).Returns(start + 1);

            Mock<Paragraph> mockParagraph = new Mock<Paragraph>();
            mockParagraph.Setup(p => p.Range).Returns(mockRange.Object);
            this.mockWordDocument.Setup(doc => doc.ParagraphContent(mockParagraph.Object)).Returns(content);
            this.mockWordDocument.Setup(doc => doc.ParagraphStyle(mockParagraph.Object)).Returns(style);
            this.mockParagraphs.Add(mockParagraph.Object);
            return mockParagraph.Object;
        }

        /// <summary>
        /// Sets up a mock paragraph to be added to the mock document.
        /// </summary>
        /// <param name="content">The content of the mock paragraph.</param>
        /// <returns>The mock paragraph.</returns>
        private Paragraph SetupMockParagraph(string content)
        {
            return this.SetupMockParagraph(content, "Normal");
        }
    }
}
