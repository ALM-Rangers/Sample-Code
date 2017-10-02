//---------------------------------------------------------------------
// <copyright file="TeamProjectTemplateTests.cs" company="Microsoft">
//    Copyright © ALM | DevOps Ranger Contributors. All rights reserved.
//    This code is licensed under the MIT License.
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF 
//    ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
//    TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
//    PARTICULAR PURPOSE AND NONINFRINGEMENT.
// </copyright>
// <summary>The TeamProjectTemplateTests type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.Word4Tfs.Library.Test.UnitTest.Model
{
    using global::System;
    using global::System.Collections.Generic;
    using global::System.Drawing;
    using global::System.Linq;
    using Microsoft.Office.Interop.Word;
    using Microsoft.Practices.Unity;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Word4Tfs.Library.Data;
    using Microsoft.Word4Tfs.Library.Model;
    using Microsoft.Word4Tfs.Library.Model.Word;
    using Moq;

    /// <summary>
    /// Tests the <see cref="TeamProjectTemplate"/> class.
    /// </summary>
    [TestClass]
    [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Unity container disposed in the test cleanup code.")]
    public class TeamProjectTemplateTests
    {
        /// <summary>
        /// The prefix to category names that indicates a category is being used as a layout
        /// </summary>
        private const string CategoryPrefix = "TFS_";

        /// <summary>
        /// A test field name used in the tests.
        /// </summary>
        private const string FieldA = "a.a";

        /// <summary>
        /// A test field name used in the tests.
        /// </summary>
        private const string FieldAUpper = "A.A";

        /// <summary>
        /// A test field name used in the tests.
        /// </summary>
        private const string FieldB = "b.b";

        /// <summary>
        /// A test field name used in the tests.
        /// </summary>
        private const string FieldC = "c.c";

        /// <summary>
        /// A test field name used in the tests.
        /// </summary>
        private const string FieldD = "d.d";

        /// <summary>
        /// The Unity container to be used for Dependency Injection
        /// </summary>
        private IUnityContainer container;

        /// <summary>
        /// The mock word template used to test the model.
        /// </summary>
        private Mock<IWordTemplate> mockWordTemplate;

        /// <summary>
        /// The mock word document used to test the model.
        /// </summary>
        private Mock<IWordDocument> mockWordDocument;

        /// <summary>
        /// The mock word application used to test the model.
        /// </summary>
        private Mock<IWordApplication> mockWordApplication;

        /// <summary>
        /// The list of building block categories to be returned by the mock word template.
        /// </summary>
        private List<string> mockBuildingBlockCategories = new List<string>();

        /// <summary>
        /// The model to be tested.
        /// </summary>
        private ITeamProjectTemplate sut;

        /// <summary>
        /// Initializes the test.
        /// </summary>
        [TestInitialize]
        public void InitializeTest()
        {
            this.container = new UnityContainer();
            this.container.RegisterInstance<ILogger>(new Logger());

            this.mockWordDocument = TestHelper.CreateAndRegisterMock<IWordDocument>(this.container);
            this.mockWordTemplate = TestHelper.CreateAndRegisterMock<IWordTemplate>(this.container);

            this.mockWordApplication = TestHelper.CreateAndRegisterMock<IWordApplication>(this.container);
            this.mockWordApplication.Setup(app => app.ActiveDocument).Returns(this.mockWordDocument.Object);

            this.mockWordTemplate.Setup(template => template.EnumerateBuildingBlockCategories()).Returns(() => this.mockBuildingBlockCategories.ToArray()); // lambda used for return so latest list always returned rather than point in time state here

            this.sut = this.container.Resolve<TeamProjectTemplate>();
        }

        /// <summary>
        /// Cleans up after each test.
        /// </summary>
        [TestCleanup]
        public void CleanupTest()
        {
            this.container.Dispose();
        }

        /// <summary>
        /// Test that <see cref="TeamProjectTemplate"/> constructor checks logger for null.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void ConstructorChecksLoggerForNull()
        {
            TestHelper.TestForArgumentNullException(() => { new TeamProjectTemplate(null, null, null); }, "logger");
        }

        /// <summary>
        /// Test that <see cref="ITeamProjectTemplate.IsImportable"/> throws an <see cref="InvalidOperationException"/> if <see cref="ITeamProjectTemplate.Load"/> has not been called.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void IsImportableThrowsInvalidOperationExceptionIfLoadHasNotBeenCalled()
        {
            TestHelper.TestForInvalidOperationException(() => { bool temp = this.sut.IsImportable; }, "The template must be loaded first.");
        }

        /// <summary>
        /// Test that <see cref="ITeamProjectTemplate.Layouts"/> throws an <see cref="InvalidOperationException"/> if <see cref="ITeamProjectTemplate.Load"/> has not been called.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void LayoutsThrowsInvalidOperationExceptionIfLoadHasNotBeenCalled()
        {
            TestHelper.TestForInvalidOperationException(() => { IEnumerable<LayoutInformation> temp = this.sut.Layouts; }, "The template must be loaded first.");
        }

        /// <summary>
        /// Test that <see cref="TeamProjectTemplate.Layouts"/> returns no layouts if there are no categories in the template.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void LayoutsReturnsNoLayoutsWhenThereAreNoCategoriesInTheTemplate()
        {
            // Arrange
            this.sut.Load();

            // Act
            int count = this.sut.Layouts.Count();

            // Assert
            Assert.AreEqual<int>(0, count, "There should not be any layouts because there are no categories.");
        }

        /// <summary>
        /// Test that <see cref="TeamProjectTemplate.Layouts"/> returns no layouts if there are no categories in the template with the right prefix.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void LayoutsReturnsNoLayoutsWhenThereAreNoCategoriesWithTheRightPrefixInTheTemplate()
        {
            // Arrange
            this.SetupWordTemplateWithCategoryAndBuildingBlocks(CategoryPrefix.ToLower() + "blah");
            this.SetupWordTemplateWithCategoryAndBuildingBlocks("blah");
            this.sut.Load();

            // Act
            int count = this.sut.Layouts.Count();

            // Assert
            Assert.AreEqual<int>(0, count, "There should not be any layouts because there are no categories with the " + CategoryPrefix + " prefix.");
        }

        /// <summary>
        /// Test that <see cref="TeamProjectTemplate.Layouts"/> does not enumerate categories when it is called.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void LayoutsDoesNotEnumeratesCategories()
        {
            // Arrange
            this.SetupWordTemplateWithCategoryAndBuildingBlocks(CategoryPrefix.ToLower() + "blah");
            this.SetupWordTemplateWithCategoryAndBuildingBlocks("blah");
            this.sut.Load();
            this.SetupWordTemplateWithCategoryAndBuildingBlocks(CategoryPrefix + "blah", BuildingBlockName.Default);

            // Act
            int ans = this.sut.Layouts.Count();

            // Assert
            Assert.AreEqual<int>(0, ans, "Layouts should not be enumerated after loading.");
        }

        /// <summary>
        /// Test that <see cref="TeamProjectTemplate.Load"/> causes categories to be enumerated every time it is called.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void LoadEnumeratesCategoriesEachTime()
        {
            // Arrange
            this.SetupWordTemplateWithCategoryAndBuildingBlocks(CategoryPrefix.ToLower() + "blah");
            this.SetupWordTemplateWithCategoryAndBuildingBlocks("blah");
            this.sut.Load();
            this.SetupWordTemplateWithCategoryAndBuildingBlocks(CategoryPrefix + "blah", BuildingBlockName.Default);

            // Act
            this.sut.Load();
            int ans = this.sut.Layouts.Count();

            // Assert
            Assert.AreEqual<int>(1, ans, "Layouts should be enumerated on every load.");
        }

        /// <summary>
        /// Test that <see cref="TeamProjectTemplate.Load"/> does not load any layouts if there are valid categories in the template but they do not contain a Default building block.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void LoadReturnsNoLayoutsWhenThereAreCategoriesWithTheRightPrefixInTheTemplateButDoNotContainADefaultBuildingBlock()
        {
            // Arrange
            this.SetupWordTemplateWithCategoryAndBuildingBlocks(CategoryPrefix + "blah1");
            this.SetupWordTemplateWithCategoryAndBuildingBlocks(CategoryPrefix + "blah2");

            // Act
            this.sut.Load();
            int count = this.sut.Layouts.Count();

            // Assert
            Assert.AreEqual<int>(0, count, "There should not be any layouts because there are no categories with the TFS_ prefix that have a Default building block.");
        }

        /// <summary>
        /// Test that <see cref="TeamProjectTemplate.Load"/> loads a layout when there is a category with the right name and a default building block.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void LoadReturnsALayoutWhenTheCategoryIsValid()
        {
            // Arrange
            this.SetupWordTemplateWithCategoryAndBuildingBlocks(CategoryPrefix + "Blah1", new BuildingBlockName("Blah1"), BuildingBlockName.Default, new BuildingBlockName("Blah2"));

            // Act
            this.sut.Load();
            LayoutInformation[] layouts = this.sut.Layouts.ToArray();

            // Assert
            Assert.AreEqual<int>(1, layouts.Length, "There should be a layout.");
            Assert.AreEqual<string>("Blah1", layouts[0].Name, "The layout should have the name without the TFS_ prefix.");
            this.AssertLayoutContainsBuildingBlocks(layouts[0], "Blah1", BuildingBlockName.Default.ToString(), "Blah2"); 
        }

        /// <summary>
        /// Test that <see cref="TeamProjectTemplate.Load"/> can return a layout name which contains white space.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void LoadCanReturnALayoutNameWithWhiteSpaceInIt()
        {
            // Arrange
            this.SetupLayoutWithDefaultBuildingBlock("Blah 1");

            // Act
            this.sut.Load();
            LayoutInformation[] layouts = this.sut.Layouts.ToArray();

            // Assert
            Assert.AreEqual<string>("Blah 1", layouts[0].Name, "The layout should contain a white space.");
        }

        /// <summary>
        /// Test that <see cref="TeamProjectTemplate.Load"/> can load more than one layout.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void LoadCanReturnMoreThanOneLayout()
        {
            // Arrange
            this.SetupLayoutWithDefaultBuildingBlock("Blah1", "Blah2");

            // Act
            this.sut.Load();
            LayoutInformation[] layouts = this.sut.Layouts.ToArray();

            // Assert
            Assert.AreEqual<int>(2, layouts.Length, "There should be 2 layouts.");
            Assert.AreEqual<string>("Blah1", layouts[0].Name, "The layout should have the name without the TFS_ prefix.");
            Assert.AreEqual<string>("Blah2", layouts[1].Name, "The layout should have the name without the TFS_ prefix.");
        }

        /// <summary>
        /// Test that <see cref="TeamProjectTemplate.Layouts"/> returns the distinct fields for each valid layout. Case sensitive
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void LayoutsReturnsDistinctFieldsCaseSensitiveForEachValidLayout()
        {
            // Arrange
            BuildingBlock bb1 = this.SetupWordTemplateWithCategoryAndBuildingBlocks(CategoryPrefix + "_Layout1", BuildingBlockName.Default)[0];
            this.SetupBuildingBlockFieldNames(bb1, FieldA, FieldB, FieldAUpper, FieldC);
            this.sut.Load();

            // Act
            string[] fields = this.sut.Layouts.First().FieldNames.ToArray();

            // Assert
            Assert.IsTrue(TestHelper.CheckUnorderedUniqueArrayMatch<string>(new string[] { FieldA, FieldAUpper, FieldB, FieldC }, fields), "Expected field list did not match");
        }

        /// <summary>
        /// Test that <see cref="TeamProjectTemplate.Layouts"/> returns the distinct fields for each valid layout across multiple building blocks
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void LayoutsReturnsDistinctFieldsForEachValidLayoutAcrossMultipleBuildingBlocks()
        {
            // Arrange
            BuildingBlock[] blocks = this.SetupWordTemplateWithCategoryAndBuildingBlocks(CategoryPrefix + "_Layout1", BuildingBlockName.Default, new BuildingBlockName("Blah2"));
            this.SetupBuildingBlockFieldNames(blocks[0], FieldA, FieldB);
            this.SetupBuildingBlockFieldNames(blocks[1], FieldAUpper, FieldB, FieldC);
            this.sut.Load();

            // Act
            string[] fields = this.sut.Layouts.First().FieldNames.ToArray();

            // Assert
            Assert.IsTrue(TestHelper.CheckUnorderedUniqueArrayMatch<string>(new string[] { FieldA, FieldAUpper, FieldB, FieldC }, fields), "Expected field list did not match");
        }

        /// <summary>
        /// Test that <see cref="TeamProjectTemplate.Layouts"/> returns the fields that apply to each layout
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void LayoutsReturnsFieldsForEachLayout()
        {
            // Arrange
            BuildingBlock[] layout1Blocks = this.SetupWordTemplateWithCategoryAndBuildingBlocks(CategoryPrefix + "Layout1", BuildingBlockName.Default);
            this.SetupBuildingBlockFieldNames(layout1Blocks[0], FieldA, FieldB);
            BuildingBlock[] layout2Blocks = this.SetupWordTemplateWithCategoryAndBuildingBlocks(CategoryPrefix + "Layout2", BuildingBlockName.Default);
            this.SetupBuildingBlockFieldNames(layout2Blocks[0], FieldC, FieldD);
            this.sut.Load();

            // Act
            LayoutInformation[] layouts = this.sut.Layouts.ToArray();

            // Assert
            Assert.IsTrue(TestHelper.CheckUnorderedUniqueArrayMatch<string>(new string[] { FieldA, FieldB }, layouts[0].FieldNames.ToArray()), "Expected field list did not match for first layout");
            Assert.IsTrue(TestHelper.CheckUnorderedUniqueArrayMatch<string>(new string[] { FieldC, FieldD }, layouts[1].FieldNames.ToArray()), "Expected field list did not match for second layout");
        }

        /// <summary>
        /// Test that <see cref="TeamProjectTemplate.Layouts"/> filters out fields that are not valid reference names.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void LayoutsFiltersOutFieldsThatAreNotValidReferenceNames()
        {
            // Arrange
            BuildingBlock[] layoutBlocks = this.SetupWordTemplateWithCategoryAndBuildingBlocks(CategoryPrefix + "Layout1", BuildingBlockName.Default);
            this.SetupBuildingBlockFieldNames(layoutBlocks[0], FieldA, @"?/\ &*^%");
            this.sut.Load();

            // Act
            LayoutInformation[] layouts = this.sut.Layouts.ToArray();

            // Assert
            Assert.IsTrue(TestHelper.CheckUnorderedUniqueArrayMatch<string>(new string[] { FieldA }, layouts[0].FieldNames.ToArray()), "Invalid reference name not removed from list of fields");
        }

        /// <summary>
        /// Test that <see cref="TeamProjectTemplate.Layouts"/> returns the preview image from the default building block if there is no preview building block.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void LayoutsReturnsPreviewImageFromDefaultBuildingBlockIfThereIsNoPreviewBuildingBlock()
        {
            // Arrange
            using (Image layout1Bb1Preview = new Bitmap(1, 1), layout1Bb2Preview = new Bitmap(1, 1), layout2Bb1Preview = new Bitmap(1, 1), layout2Bb2Preview = new Bitmap(1, 1))
            {
                BuildingBlock[] layout1Blocks = this.SetupWordTemplateWithCategoryAndBuildingBlocks(CategoryPrefix + "Layout1", BuildingBlockName.Default, new BuildingBlockName(0));
                BuildingBlock[] layout2Blocks = this.SetupWordTemplateWithCategoryAndBuildingBlocks(CategoryPrefix + "Layout2", BuildingBlockName.Default, new BuildingBlockName(0));
                this.SetupBuildingBlockInformation(layout1Blocks[0], layout1Bb1Preview, FieldA, FieldB);
                this.SetupBuildingBlockInformation(layout1Blocks[1], layout1Bb2Preview, FieldA, FieldB);
                this.SetupBuildingBlockInformation(layout2Blocks[0], layout2Bb1Preview, FieldC, FieldD);
                this.SetupBuildingBlockInformation(layout2Blocks[1], layout2Bb2Preview, FieldC, FieldD);
                this.sut.Load();

                // Act
                LayoutInformation[] layouts = this.sut.Layouts.ToArray();

                // Assert
                Assert.AreSame(layout1Bb1Preview, layouts[0].PreviewImage, "Default building block preview image not selected.");
                Assert.AreSame(layout2Bb1Preview, layouts[1].PreviewImage, "Default building block preview image not selected.");
            }
        }

        /// <summary>
        /// Test that <see cref="TeamProjectTemplate.Layouts"/> returns the preview image from the preview building block even if there is a default building block.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void LayoutsReturnsPreviewImageFromPreviewBuildingBlockEvenIfThereIsADefaultBuildingBlock()
        {
            // Arrange
            using (Image layout1Bb1Preview = new Bitmap(1, 1), layout1Bb2Preview = new Bitmap(1, 1), layout1Bb3Preview = new Bitmap(1, 1))
            {
                BuildingBlock[] layoutBlocks = this.SetupWordTemplateWithCategoryAndBuildingBlocks(CategoryPrefix + "Layout1", BuildingBlockName.Default, BuildingBlockName.Preview, new BuildingBlockName(0));
                this.SetupBuildingBlockInformation(layoutBlocks[0], layout1Bb1Preview, FieldA, FieldB);
                this.SetupBuildingBlockInformation(layoutBlocks[1], layout1Bb2Preview, FieldA, FieldB);
                this.SetupBuildingBlockInformation(layoutBlocks[2], layout1Bb2Preview, FieldA, FieldB);
                this.sut.Load();

                // Act
                LayoutInformation[] layouts = this.sut.Layouts.ToArray();

                // Assert
                Assert.AreSame(layout1Bb2Preview, layouts[0].PreviewImage, "Preview building block preview image not selected.");
            }
        }

        /// <summary>
        /// Test that <see cref="TeamProjectTemplate.IsImportable"/> returns <c>true</c> when the template has at least one TFS layout.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void IsImportableReturnsTrueWhenTheTemplateHasAtLeastOneTFSLayout()
        {
            // Arrange
            this.SetupLayoutWithDefaultBuildingBlock("Layout1");
            this.sut.Load();

            // Act and Assert
            Assert.IsTrue(this.sut.IsImportable, "Team Project Document should be importable when the underlying document is not connected but the template has at least one layout.");
        }

        /// <summary>
        /// Test that <see cref="TeamProjectTemplate.IsImportable"/> returns <c>false</c> when the template does not contain any TFS layouts.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void IsImportableReturnsFalseWhenTheTemplateDoesNotHaveAnyTFSLayouts()
        {
            // Arrange
            this.SetupLayoutWithDefaultBuildingBlock(new string[0]);
            this.sut.Load();

            // Act and Assert
            Assert.IsFalse(this.sut.IsImportable, "Team Project Document should not be importable when the underlying document is not connected and the template does not contain any TFS layouts.");
        }

        /// <summary>
        /// Test that <see cref="TeamProjectTemplate.Load"/> turns off word updates while loading the template.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void LoadTurnsOffWordUpdatesWhileLoadingTemplate()
        {
            // Arrange
            bool updatesDisabled = false;
            this.mockWordDocument.Setup(doc => doc.StartUpdate()).Callback(() => { updatesDisabled = true; });
            this.mockWordDocument.Setup(doc => doc.EndUpdate()).Callback(() => { updatesDisabled = false; });
            this.mockWordTemplate.Setup(template => template.EnumerateBuildingBlockCategories()).Returns(new string[0]).Callback(() => Assert.IsTrue(updatesDisabled, "Updates should be disabled during the enumeration."));

            // Act
            this.sut.Load();

            // Assert
            Assert.IsFalse(updatesDisabled, "Updates not reenabled.");
        }

        /// <summary>
        /// Test that <see cref="TeamProjectTemplate.Load"/> turns word updates on again even if there is an exception while loading the template.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void LoadTurnsOnWordUpdatesEvenIfExceptionWhileLoadingTemplate()
        {
            // Arrange
            bool updatesDisabled = false;
            this.mockWordDocument.Setup(doc => doc.StartUpdate()).Callback(() => { updatesDisabled = true; });
            this.mockWordDocument.Setup(doc => doc.EndUpdate()).Callback(() => { updatesDisabled = false; });
            this.mockWordTemplate.Setup(template => template.EnumerateBuildingBlockCategories()).Throws(new InvalidOperationException());

            // Act
            try
            {
                this.sut.Load();
            }
            catch (InvalidOperationException)
            {
            }

            // Assert
            Assert.IsFalse(updatesDisabled, "Updates not reenabled after exception.");
        }

        /// <summary>
        /// Test that <see cref="TeamProjectTemplate.Save"/> saves the changes to the template document.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void SaveSavesTheChangesToTemplateDocument()
        {
            // Act
            this.sut.Save();

            // Assert
            this.mockWordTemplate.Verify(t => t.Save(), "The template was not saved.");
        }

        /// <summary>
        /// Test that <see cref="TeamProjectTemplate.Delete"/> deletes the named layout.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void DeleteDeletesTheNamedLayout()
        {
            // Arrange
            this.SetupWordTemplateWithCategoryAndBuildingBlocks(CategoryPrefix + "Layout1", BuildingBlockName.Default);
            this.SetupWordTemplateWithCategoryAndBuildingBlocks(CategoryPrefix + "Layout2", BuildingBlockName.Default);
            this.sut.Load();

            // Act
            foreach (LayoutInformation li in this.sut.Layouts)
            {
                this.sut.DeleteLayout(li.Name);
            }

            // Assert
            Assert.AreEqual<int>(0, this.sut.Layouts.Count(), "Layouts have not been deleted");
        }

        /// <summary>
        /// Test that <see cref="TeamProjectTemplate.Delete"/> silently ignores request to delete a layout that does not exist.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void DeleteSilentlyIgnoresRequestToDeleteLayoutThatDoesNotExist()
        {
            // Arrange
            this.sut.Load();

            // Act
            this.sut.DeleteLayout("no such layout");
        }

        /// <summary>
        /// Test that <see cref="TeamProjectTemplate.Delete"/> deletes the building blocks when a layout is deleted.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void DeleteDeletesTheBuildingBlocksForALayout()
        {
            // Arrange
            this.SetupWordTemplateWithCategoryAndBuildingBlocks(CategoryPrefix + "Layout1", BuildingBlockName.Default, new BuildingBlockName("Block"));
            this.sut.Load();

            // Act
            this.sut.DeleteLayout("Layout1");

            // Assert
            this.mockWordTemplate.Verify(t => t.DeleteBuildingBlocksForCategory(CategoryPrefix + "Layout1"), Times.Once(), "Building blocks for category not deleted");
        }

        /// <summary>
        /// Test that <see cref="TeamProjectTemplate.CreateLayout"/> throws exception if the parameter is null.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void CreateLayoutThrowsExceptionIfParameterIsNull()
        {
            // Act
            TestHelper.TestForArgumentNullException(() => this.sut.CreateLayout("Layout1", null), "blocks");
        }

        /// <summary>
        /// Test that <see cref="TeamProjectTemplate.CreateLayout"/> throws exception if <see cref="ITeamProjectTemplate.Load"/> has not been called yet.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void CreateLayoutThrowsExceptionIfLoadNotCalled()
        {
            // Act
            TestHelper.TestForInvalidOperationException(() => this.sut.CreateLayout("Layout1", new BuildingBlockDefinition[] { new BuildingBlockDefinition(BuildingBlockName.Default, 1, 2) }), "The template must be loaded first.");
        }

        /// <summary>
        /// Test that <see cref="TeamProjectTemplate.CreateLayout"/> creates the building blocks for the layout.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void CreateLayoutCreatesTheBuildingBlocksForTheLayout()
        {
            // Arrange
            this.SetupTemplateForCreateBuildingBlock("Layout1", BuildingBlockName.Default, 1, 2);
            this.SetupTemplateForCreateBuildingBlock("Layout1", new BuildingBlockName("bb2"), 3, 4);
            this.SetupBuildingBlockInformation(BuildingBlockName.Default.ToString(), 1, 2, null);
            this.SetupBuildingBlockInformation("bb2", 3, 4, null);
            this.sut.Load();

            // Act
            this.sut.CreateLayout("Layout1", new BuildingBlockDefinition[] { new BuildingBlockDefinition(BuildingBlockName.Default, 1, 2), new BuildingBlockDefinition("bb2", 3, 4) });

            // Assert
            this.mockWordTemplate.Verify(t => t.CreateBuildingBlock(CategoryPrefix + "Layout1", BuildingBlockName.Default.ToString(), 1, 2), Times.Once(), "Building block not created");
            this.mockWordTemplate.Verify(t => t.CreateBuildingBlock(CategoryPrefix + "Layout1", "bb2", 3, 4), Times.Once(), "Building block not created");
        }

        /// <summary>
        /// Test that <see cref="TeamProjectTemplate.CreateLayout"/> throws exception if there is no default building block.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void CreateLayoutThrowsExceptionIfThereIsNoDefaultBuildingBlock()
        {
            // Arrange
            this.sut.Load();

            // Act
            TestHelper.TestForInvalidOperationException(
                () => this.sut.CreateLayout("Layout1", new BuildingBlockDefinition[] { new BuildingBlockDefinition("bb1", 1, 2), new BuildingBlockDefinition("bb2", 3, 4) }),
                "Cannot create layout Layout1 because the Default layout item is either empty or missing.");
        }

        /// <summary>
        /// Test that <see cref="TeamProjectTemplate.CreateLayout"/> gets building block information without changing the document as the data is already in the document.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void CreateLayoutGetsBuildingBlockInformationWithoutChangingDocument()
        {
            // Arrange
            this.SetupTemplateForCreateBuildingBlock("Layout1", BuildingBlockName.Default, 1, 2);
            this.SetupTemplateForCreateBuildingBlock("Layout1", new BuildingBlockName("bb2"), 3, 4);
            this.SetupBuildingBlockInformation(BuildingBlockName.Default.ToString(), 1, 2, null, FieldA, FieldB);
            this.SetupBuildingBlockInformation("bb2", 3, 4, null, FieldC, FieldD);
            this.sut.Load();

            // Act
            this.sut.CreateLayout("Layout1", new BuildingBlockDefinition[] { new BuildingBlockDefinition(BuildingBlockName.Default, 1, 2), new BuildingBlockDefinition("bb2", 3, 4) });
            LayoutInformation[] layouts = this.sut.Layouts.ToArray();

            // Assert
            this.mockWordDocument.Verify(doc => doc.ReadBuildingBlockInfo(BuildingBlockName.Default, 1, 2), Times.Once(), "Non-modifying ReadBuildingBlockInfo method not called.");
            this.mockWordDocument.Verify(doc => doc.ReadBuildingBlockInfo(new BuildingBlockName("bb2"), 3, 4), Times.Once(), "Non-modifying ReadBuildingBlockInfo method not called.");
            Assert.IsTrue(TestHelper.CheckUnorderedUniqueArrayMatch<string>(new string[] { FieldA, FieldB, FieldC, FieldD }, layouts[0].FieldNames.ToArray()), "Expected field list did not match");
        }

        /// <summary>
        /// Test that <see cref="TeamProjectTemplate.CreateLayout"/> adds the new layout to the <see cref="ITeamProjectTemplate.Layouts"/> collection.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void CreateLayoutAddsNewLayoutToLayoutsCollection()
        {
            // Arrange
            using (Image previewImage = new Bitmap(1, 1))
            {
                this.sut.Load();
                Assert.AreEqual<int>(0, this.sut.Layouts.Count(), "Pre-requisite is that there should be no layouts.");
                this.SetupTemplateForCreateBuildingBlock("Layout1", BuildingBlockName.Default, 1, 2);
                this.SetupTemplateForCreateBuildingBlock("Layout1", new BuildingBlockName("bb2"), 3, 4);
                this.SetupBuildingBlockInformation(BuildingBlockName.Default.ToString(), 1, 2, previewImage);
                this.SetupBuildingBlockInformation("bb2", 3, 4, null);

                // Act
                this.sut.CreateLayout("Layout1", new BuildingBlockDefinition[] { new BuildingBlockDefinition(BuildingBlockName.Default, 1, 2), new BuildingBlockDefinition("bb2", 3, 4) });

                // Assert
                Assert.AreEqual<int>(1, this.sut.Layouts.Count(), "There should be a new layout in the Layouts collection.");
                Assert.AreEqual<string>("Layout1", this.sut.Layouts.First().Name, "Expected layout not added to layouts collection.");
                Assert.AreEqual<int>(2, this.sut.Layouts.First().BuildingBlockNames.Count(), "Layout does not contain the building blocks");
                this.AssertLayoutContainsBuildingBlock(this.sut.Layouts.First(), BuildingBlockName.Default);
                this.AssertLayoutContainsBuildingBlock(this.sut.Layouts.First(), new BuildingBlockName("bb2"));
                Assert.AreSame(previewImage, this.sut.Layouts.First().PreviewImage, "No preview image for the layout");
            }
        }

        /// <summary>
        /// Test that <see cref="TeamProjectTemplate.CreateLayout"/> replaces an existing layout in the <see cref="ITeamProjectTemplate.Layouts"/> collection.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void CreateLayoutReplacesExistingLayoutInLayoutsCollection()
        {
            // Arrange
            using (Image previewImage = new Bitmap(1, 1))
            {
                this.SetupLayoutWithDefaultBuildingBlock("Layout1");
                this.sut.Load();
                Assert.AreEqual<int>(1, this.sut.Layouts.Count(), "Pre-requisite is that there should be one layout.");
                this.SetupTemplateForCreateBuildingBlock("Layout1", BuildingBlockName.Default, 1, 2);
                this.SetupTemplateForCreateBuildingBlock("Layout1", new BuildingBlockName("bb2"), 3, 4);
                this.SetupBuildingBlockInformation(BuildingBlockName.Default.ToString(), 1, 2, previewImage);
                this.SetupBuildingBlockInformation("bb2", 3, 4, null);

                // Act
                this.sut.CreateLayout("Layout1", new BuildingBlockDefinition[] { new BuildingBlockDefinition(BuildingBlockName.Default, 1, 2), new BuildingBlockDefinition("bb2", 3, 4) });

                // Assert
                Assert.AreEqual<int>(1, this.sut.Layouts.Count(), "There should be a new layout in the Layouts collection.");
            }
        }

        /// <summary>
        /// Test that <see cref="TeamProjectTemplate.GetLayoutBuildingBlock"/> checks layout parameter for null.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void GetLayoutBuildingBlockChecksLayoutParameterForNull()
        {
            TestHelper.TestForArgumentNullException(() => this.sut.GetLayoutBuildingBlock(null, BuildingBlockName.Default), "layout");
        }

        /// <summary>
        /// Test that <see cref="TeamProjectTemplate.GetLayoutBuildingBlock"/> checks name parameter for null.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void GetLayoutBuildingBlockChecksNameParameterForNull()
        {
            TestHelper.TestForArgumentNullException(() => this.sut.GetLayoutBuildingBlock(TestHelper.CreateTestLayoutInformation(), null), "name");
        }

        /// <summary>
        /// Test that <see cref="TeamProjectTemplate.GetLayoutBuildingBlock"/> gets the named building block for the requested layout.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void GetLayoutBuildingBlockReturnsTheBuildingBlockBelongingToTheLayout()
        {
            // Arrange
            BuildingBlock[] layout1Blocks = this.SetupWordTemplateWithCategoryAndBuildingBlocks(CategoryPrefix + "Layout1", BuildingBlockName.Default);
            BuildingBlock[] layout2Blocks = this.SetupWordTemplateWithCategoryAndBuildingBlocks(CategoryPrefix + "Layout2", BuildingBlockName.Default);
            this.sut.Load();
            LayoutInformation layout2 = this.sut.Layouts.Skip(1).First();

            // Act
            BuildingBlock ans = this.sut.GetLayoutBuildingBlock(layout2, BuildingBlockName.Default);

            // Assert
            Assert.AreSame(layout2Blocks[0], ans, "Should get default building block");
        }

        /// <summary>
        /// Test that <see cref="TeamProjectTemplate.GetLayoutBuildingBlock"/> finds building block ignoring case.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void GetLayoutBuildingBlockReturnsTheBuildingBlockBelongingToTheLayoutIgnoringCase()
        {
            // Arrange
            BuildingBlock[] layout1Blocks = this.SetupWordTemplateWithCategoryAndBuildingBlocks(CategoryPrefix + "Layout1", BuildingBlockName.Default, new BuildingBlockName("lower"));
            this.sut.Load();
            LayoutInformation layout1 = this.sut.Layouts.First();

            // Act
            BuildingBlock ans1 = this.sut.GetLayoutBuildingBlock(layout1, new BuildingBlockName("LOWER"));

            // Assert
            Assert.AreSame(layout1Blocks[1], ans1, "Should get building block with differently cased name");
        }

        /// <summary>
        /// Test that <see cref="TeamProjectTemplate.GetLayoutBuildingBlock"/> returns null if the layout does not exist.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void GetLayoutBuildingBlockReturnsNullIfKLayoutDoesNotExist()
        {
            // Arrange
            BuildingBlock[] layout1Blocks = this.SetupWordTemplateWithCategoryAndBuildingBlocks(CategoryPrefix + "Layout1", BuildingBlockName.Default);
            this.sut.Load();
            LayoutInformation layout = new LayoutInformation("Invalid", new BuildingBlockName[] { BuildingBlockName.Default }, new string[0], null);

            // Act
            BuildingBlock ans = this.sut.GetLayoutBuildingBlock(layout, new BuildingBlockName("Invalid"));

            // Assert
            Assert.IsNull(ans);
        }

        /// <summary>
        /// Test that <see cref="TeamProjectTemplate.GetLayoutBuildingBlock"/> returns null if the building block does not exist.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void GetLayoutBuildingBlockReturnsNullIfBuildingBlockDoesNotExist()
        {
            // Arrange
            BuildingBlock[] layout1Blocks = this.SetupWordTemplateWithCategoryAndBuildingBlocks(CategoryPrefix + "Layout1", BuildingBlockName.Default);
            this.sut.Load();
            LayoutInformation layout = this.sut.Layouts.First();

            // Act
            BuildingBlock ans = this.sut.GetLayoutBuildingBlock(layout, new BuildingBlockName("Invalid"));

            // Assert
            Assert.IsNull(ans);
        }

        /// <summary>
        /// Test that <see cref="TeamProjectTemplate.GetLayoutBuildingBlock"/> reloads building block cache after a layout has been deleted.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void GetLayoutBuildingBlockReloadsCacheAfterALayoutHasBeenDeleted()
        {
            // Arrange
            BuildingBlock[] layout1Blocks = this.SetupWordTemplateWithCategoryAndBuildingBlocks(CategoryPrefix + "Layout1", BuildingBlockName.Default);
            this.SetupWordTemplateWithCategoryAndBuildingBlocks(CategoryPrefix + "Layout2", BuildingBlockName.Default);
            this.sut.Load();
            LayoutInformation layout = this.sut.Layouts.First();

            // Act
            this.sut.DeleteLayout(layout.Name);
            this.SetupWordTemplateWithCategoryAndBuildingBlocks(CategoryPrefix + "Layout1", BuildingBlockName.Default); 
            BuildingBlock ans = this.sut.GetLayoutBuildingBlock(layout, BuildingBlockName.Default);

            // Assert
            Assert.AreNotSame(layout1Blocks[0], ans, "Building block cache was not refreshed");
        }

        /// <summary>
        /// Test that <see cref="TeamProjectTemplate.GetLayoutBuildingBlock"/> returns null for building block after the layout has been deleted.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void GetLayoutBuildingBlockReturnsNullForBuildingBlockAfterLayoutHasBeenDeleted()
        {
            // Arrange
            this.SetupWordTemplateWithCategoryAndBuildingBlocks(CategoryPrefix + "Layout1", BuildingBlockName.Default);
            this.SetupWordTemplateWithCategoryAndBuildingBlocks(CategoryPrefix + "Layout2", BuildingBlockName.Default);
            this.sut.Load();
            LayoutInformation layout = this.sut.Layouts.First();

            // Act
            this.sut.DeleteLayout(layout.Name);
            this.SetupWordTemplateWithCategoryAndBuildingBlocks(CategoryPrefix + "Layout1");
            BuildingBlock ans = this.sut.GetLayoutBuildingBlock(layout, BuildingBlockName.Default);

            // Assert
            Assert.IsNull(ans, "Building block should not be returned");
        }

        /// <summary>
        /// Asserts that a layout contains a set of named building blocks.
        /// </summary>
        /// <param name="layout">The layout to check.</param>
        /// <param name="expectedBuildingBlockNames">The expected building blocks.</param>
        private void AssertLayoutContainsBuildingBlocks(LayoutInformation layout, params string[] expectedBuildingBlockNames)
        {
            foreach (BuildingBlockName bbn in layout.BuildingBlockNames)
            {
                Assert.AreEqual<int>(1, expectedBuildingBlockNames.Where(n => n == bbn.ToString()).Count(), "Layout contains unexpected building blocks");
            }

            foreach (string name in expectedBuildingBlockNames)
            {
                Assert.AreEqual<int>(1, layout.BuildingBlockNames.Where(bbn => bbn.ToString() == name).Count(), "Layout is missing some building blocks");
            }
        }

        /// <summary>
        /// Asserts that a layout contains a particular building block.
        /// </summary>
        /// <param name="layout">The layout to check.</param>
        /// <param name="expectedName">The name of the expected building block.</param>
        private void AssertLayoutContainsBuildingBlock(LayoutInformation layout, BuildingBlockName expectedName)
        {
            Assert.IsTrue(layout.BuildingBlockNames.Where(bbn => bbn == expectedName).Count() == 1, "Layout does not contain expected building block: " + expectedName);
        }

        /// <summary>
        /// Sets up the template for a call to <see cref="IWordTemplate.CreateBuildingBlock"/>.
        /// </summary>
        /// <param name="layoutName">The name of the layout.</param>
        /// <param name="blockName">The name of the building block.</param>
        /// <param name="startPosition">Start position for the building block.</param>
        /// <param name="endPosition">End position for the building block.</param>
        private void SetupTemplateForCreateBuildingBlock(string layoutName, BuildingBlockName blockName, int startPosition, int endPosition)
        {
            Mock<BuildingBlock> mockBuildingBlock = new Mock<BuildingBlock>();
            mockBuildingBlock.Setup(bb => bb.Name).Returns(blockName.ToString());
            this.mockWordTemplate.Setup(t => t.CreateBuildingBlock(CategoryPrefix + layoutName, blockName.ToString(), startPosition, endPosition)).Returns(mockBuildingBlock.Object);
        }

        /// <summary>
        /// Sets up the word template with layouts with a default building block and no fields in the content controls.
        /// </summary>
        /// <param name="layoutNames">The names of the layouts to create.</param>
        private void SetupLayoutWithDefaultBuildingBlock(params string[] layoutNames)
        {
            foreach (string layoutName in layoutNames)
            {
                this.SetupWordTemplateWithCategoryAndBuildingBlocks(CategoryPrefix + layoutName, BuildingBlockName.Default);
            }
        }

        /// <summary>
        /// Sets up the mock word document to return a list of field names for a particular building block.
        /// </summary>
        /// <param name="buildingBlock">The building block to be associated with the field list.</param>
        /// <param name="fieldNames">The list of field names.</param>
        private void SetupBuildingBlockFieldNames(BuildingBlock buildingBlock, params string[] fieldNames)
        {
            this.SetupBuildingBlockInformation(buildingBlock, null, fieldNames);
        }

        /// <summary>
        /// Sets up the mock word document to return building block information.
        /// </summary>
        /// <param name="buildingBlock">The building block to be associated with the information.</param>
        /// <param name="previewImage">The preview image for the building block.</param>
        /// <param name="fieldNames">The list of field names.</param>
        private void SetupBuildingBlockInformation(BuildingBlock buildingBlock, Image previewImage, params string[] fieldNames)
        {
            BuildingBlockInfo bbi = new BuildingBlockInfo(new BuildingBlockName(buildingBlock), fieldNames, previewImage);
            this.mockWordDocument.Setup(wordDoc => wordDoc.ReadBuildingBlockInfo(buildingBlock)).Returns(bbi);
        }

        /// <summary>
        /// Sets up the mock word document to return building block information for a range.
        /// </summary>
        /// <param name="name">The name of the building block for which information is to be set up.</param>
        /// <param name="startPosition">The start position of the building block.</param>
        /// <param name="endPosition">The end position of the building block.</param>
        /// <param name="previewImage">The preview image for the building block.</param>
        /// <param name="fieldNames">The list of field names.</param>
        private void SetupBuildingBlockInformation(string name, int startPosition, int endPosition, Image previewImage, params string[] fieldNames)
        {
            BuildingBlockName bbname = new BuildingBlockName(name);
            BuildingBlockInfo bbi = new BuildingBlockInfo(bbname, fieldNames, previewImage);
            this.mockWordDocument.Setup(wordDoc => wordDoc.ReadBuildingBlockInfo(bbname, startPosition, endPosition)).Returns(bbi);
        }

        /// <summary>
        /// Sets up categories and building blocks for a layout.
        /// </summary>
        /// <param name="categoryName">The name of the category.</param>
        /// <param name="buildingBlockNames">The names of the building blocks to add to the category.</param>
        /// <returns>The building blocks that were created.</returns>
        private BuildingBlock[] SetupWordTemplateWithCategoryAndBuildingBlocks(string categoryName, params BuildingBlockName[] buildingBlockNames)
        {
            this.mockBuildingBlockCategories.Add(categoryName);
            BuildingBlock[] testBuildingBlocks = buildingBlockNames.Select(name => TestHelper.CreateMockBuildingBlock(name)).ToArray();
            this.mockWordTemplate.Setup(template => template.EnumerateBuildingBlocksForCategory(categoryName)).Returns(testBuildingBlocks);
            foreach (BuildingBlock bb in testBuildingBlocks)
            {
                this.SetupBuildingBlockFieldNames(bb, new string[0]);
            }

            return testBuildingBlocks;
        }
    }
}
