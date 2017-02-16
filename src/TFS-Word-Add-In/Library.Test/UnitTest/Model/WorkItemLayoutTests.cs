//---------------------------------------------------------------------
// <copyright file="WorkItemLayoutTests.cs" company="Microsoft">
//    Copyright (c) Microsoft. All rights reserved.
//    This code is licensed under the MIT License.
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF 
//    ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
//    TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
//    PARTICULAR PURPOSE AND NONINFRINGEMENT.
// </copyright>
// <summary>The WorkItemLayoutTests type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.Word4Tfs.Library.Test.UnitTest.Model
{
    using global::System.Linq;
    using Microsoft.Office.Interop.Word;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Word4Tfs.Library.Data;
    using Microsoft.Word4Tfs.Library.Model;
    using Microsoft.Word4Tfs.Library.Model.TeamFoundation;
    using Microsoft.Word4Tfs.Library.Model.Word;
    using Moq;

    /// <summary>
    /// Tests the <see cref="WorkItemLayout"/> class.
    /// </summary>
    [TestClass]
    public class WorkItemLayoutTests
    {
        /// <summary>
        /// The name of the work item type to use in the tests.
        /// </summary>
        private const string Wit = "User Story";

        /// <summary>
        /// The mock word template from which the building blocks will be obtained.
        /// </summary>
        private Mock<ITeamProjectTemplate> mockTeamProjectTemplate = new Mock<ITeamProjectTemplate>();

        /// <summary>
        /// Constructor throws <see cref="ArgumentNullException"/> if layout information is null.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void ConstructorChecksLayoutInformationParameterForNull()
        {
            TestHelper.TestForArgumentNullException(() => new WorkItemLayout(null, this.mockTeamProjectTemplate.Object), "layoutInformation");
        }

        /// <summary>
        /// Constructor throws <see cref="ArgumentNullException"/> if team project template is null.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void ConstructorChecksTeamProjectTemplateParameterForNull()
        {
            LayoutInformation li = TestHelper.CreateTestLayoutInformation(BuildingBlockName.Default);
            TestHelper.TestForArgumentNullException(() => new WorkItemLayout(li, null), "teamProjectTemplate");
        }

        /// <summary>
        /// Test that <see cref="IWorkItemLayout.ChooseBuildingBlock"/> checks argument for null.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void ChooseBuildingBlockArgumentForNull()
        {
            // Arrange
            LayoutInformation li = TestHelper.CreateTestLayoutInformation(BuildingBlockName.Default);
            WorkItemLayout sut = this.CreateWorkItemLayout(li);

            // Act and Assert
            TestHelper.TestForArgumentNullException(() => sut.ChooseBuildingBlock(null), "workItemNode");
        }

        /// <summary>
        /// Test that work item type specific and level specific building block overrides just work item specific building block for items of that level.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void BuildingBlockForSpecificWorkItemTypeAndLevelOverridesWorkItemTypeSpecificForWorkItemsOfThatLevel()
        {
            // Arrange
            WorkItemTreeNode item = CreateWorkItemNode(Wit, 1);
            LayoutInformation li = this.SetupTestLayout(BuildingBlockName.Default, new BuildingBlockName(Wit), new BuildingBlockName(Wit, 1));
            WorkItemLayout sut = this.CreateWorkItemLayout(li);

            // Act
            BuildingBlock ans = sut.ChooseBuildingBlock(item);

            // Assert
            Assert.AreEqual<BuildingBlockName>(new BuildingBlockName(Wit, 1), new BuildingBlockName(ans));
        }

        /// <summary>
        /// Test that work item type specific and level specific building block overrides default building block event if work item type specific is not defined.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void BuildingBlockForSpecificWorkItemTypeAndLevelOverridesDefaultIfWorkItemTypeSpecificNotDefined()
        {
            WorkItemTreeNode item = CreateWorkItemNode(Wit, 1);
            LayoutInformation li = this.SetupTestLayout(BuildingBlockName.Default, new BuildingBlockName(Wit, 1));
            WorkItemLayout sut = this.CreateWorkItemLayout(li);

            // Act
            BuildingBlock ans = sut.ChooseBuildingBlock(item);

            // Assert
            Assert.AreEqual<BuildingBlockName>(new BuildingBlockName(Wit, 1), new BuildingBlockName(ans));
        }

        /// <summary>
        /// Test that work item type specific building block is used for work items of that type.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void BuildingBlockForSpecificWorkItemTypeIsUsedForWorkItemsOfThatType()
        {
            WorkItemTreeNode item = CreateWorkItemNode(Wit, 1);
            LayoutInformation li = this.SetupTestLayout(BuildingBlockName.Default, new BuildingBlockName(Wit));
            WorkItemLayout sut = this.CreateWorkItemLayout(li);

            // Act
            BuildingBlock ans = sut.ChooseBuildingBlock(item);

            // Assert
            Assert.AreEqual<BuildingBlockName>(new BuildingBlockName(Wit), new BuildingBlockName(ans));
        }

        /// <summary>
        /// Test that work item type specific and level specific building block not used for work items of the same type not on that level.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void BuildingBlockForSpecificWorkItemTypeAndLevelNotUsedForWorkItemsNotOnThatLevel()
        {
            WorkItemTreeNode item = CreateWorkItemNode(Wit, 0);
            LayoutInformation li = this.SetupTestLayout(BuildingBlockName.Default, new BuildingBlockName(Wit), new BuildingBlockName(Wit, 1));
            WorkItemLayout sut = this.CreateWorkItemLayout(li);

            // Act
            BuildingBlock ans = sut.ChooseBuildingBlock(item);

            // Assert
            Assert.AreEqual<BuildingBlockName>(new BuildingBlockName(Wit), new BuildingBlockName(ans));
        }

        /// <summary>
        /// Test that <see cref="IWorkItemLayout.ChooseBuildingBlock"/> identifies the default building block even if it is not the first one in the layout's list of building blocks.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void ChooseBuildingBlockLocatesDefaultBuildingBlockEvenIfNotTheFirstInALayout()
        {
            WorkItemTreeNode item = CreateWorkItemNode(Wit, 0);
            LayoutInformation li = this.SetupTestLayout(new BuildingBlockName("dummy"), BuildingBlockName.Default);
            WorkItemLayout sut = this.CreateWorkItemLayout(li);

            // Act
            BuildingBlock ans = sut.ChooseBuildingBlock(item);

            // Assert
            Assert.AreEqual<BuildingBlockName>(BuildingBlockName.Default, new BuildingBlockName(ans), "The default building block was not used.");
        }

        /// <summary>
        /// Test that work item type specific building block is not used for work items not of that type.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void BuildingBlockForSpecificWorkItemTypeIsNotUsedForWorkItemsNotOfThatType()
        {
            WorkItemTreeNode item = CreateWorkItemNode("SomeOtherWorkItemType", 0);
            LayoutInformation li = this.SetupTestLayout(BuildingBlockName.Default, new BuildingBlockName(Wit));
            WorkItemLayout sut = this.CreateWorkItemLayout(li);

            // Act
            BuildingBlock ans = sut.ChooseBuildingBlock(item);

            // Assert
            Assert.AreEqual<BuildingBlockName>(BuildingBlockName.Default, new BuildingBlockName(ans), "The default building block was not used.");
        }

        /// <summary>
        /// Test that <see cref="IWorkItemLayout.ChooseBuildingBlock"/>  identifies a default with level building block if one exists for that level.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void ChooseBuildingBlockLocatesDefaultBuildingBlockWithLevelIfOneExistsForThatLevel()
        {
            WorkItemTreeNode item = CreateWorkItemNode(Wit, 0);
            LayoutInformation li = this.SetupTestLayout(BuildingBlockName.Default, new BuildingBlockName(0));
            WorkItemLayout sut = this.CreateWorkItemLayout(li);

            // Act
            BuildingBlock ans = sut.ChooseBuildingBlock(item);

            // Assert
            Assert.AreEqual<BuildingBlockName>(new BuildingBlockName(0), new BuildingBlockName(ans), "The level-specific default building block was not used.");
        }

        /// <summary>
        /// Test that <see cref="IWorkItemLayout.ChooseBuildingBlock"/>  identifies a default building block if level specific default does not exist.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void ChooseBuildingBlockLocatesDefaultBuildingBlockIfNoLevelSpecificDefault()
        {
            WorkItemTreeNode item = CreateWorkItemNode(Wit, 0);
            LayoutInformation li = this.SetupTestLayout(BuildingBlockName.Default, new BuildingBlockName(1));
            WorkItemLayout sut = this.CreateWorkItemLayout(li);

            // Act
            BuildingBlock ans = sut.ChooseBuildingBlock(item);

            // Assert
            Assert.AreEqual<BuildingBlockName>(BuildingBlockName.Default, new BuildingBlockName(ans), "The default building block was not used.");
        }

        /// <summary>
        /// Test that <see cref="IWorkItemLayout.ChooseBuildingBlock"/>  identifies work item specific building block even if there is a level-specific default.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void ChooseBuildingBlockLocatesWorkItemSpecificBuildingBlockInPreferenceToLevelSpecificDefault()
        {
            WorkItemTreeNode item = CreateWorkItemNode(Wit, 0);
            LayoutInformation li = this.SetupTestLayout(BuildingBlockName.Default, new BuildingBlockName(0), new BuildingBlockName(Wit));
            WorkItemLayout sut = this.CreateWorkItemLayout(li);

            // Act
            BuildingBlock ans = sut.ChooseBuildingBlock(item);

            // Assert
            Assert.AreEqual<BuildingBlockName>(new BuildingBlockName(Wit), new BuildingBlockName(ans), "The work item specific building block was not used.");
        }

        /// <summary>
        /// Test that <see cref="IWorkItemLayout.ChooseBuildingBlock"/>  identifies work item level-specific building block even if there is a level-specific default.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void ChooseBuildingBlockLocatesWorkItemLevelSpecificBuildingBlockInPreferenceToLevelSpecificDefault()
        {
            WorkItemTreeNode item = CreateWorkItemNode(Wit, 0);
            LayoutInformation li = this.SetupTestLayout(BuildingBlockName.Default, new BuildingBlockName(0), new BuildingBlockName(Wit, 0));
            WorkItemLayout sut = this.CreateWorkItemLayout(li);

            // Act
            BuildingBlock ans = sut.ChooseBuildingBlock(item);

            // Assert
            Assert.AreEqual<BuildingBlockName>(new BuildingBlockName(Wit, 0), new BuildingBlockName(ans), "The work item level-specific building block was not used.");
        }

        // TODO: add layout class more generally.

        /// <summary>
        /// Creates a work item tree node for testing.
        /// </summary>
        /// <param name="workItemType">The name of the work item type.</param>
        /// <param name="level">The level in the tree.</param>
        /// <returns>The work item tree node.</returns>
        private static WorkItemTreeNode CreateWorkItemNode(string workItemType, int level)
        {
            ITfsWorkItem item = TestHelper.CreateMockWorkItem(workItemType, 1, "test");
            WorkItemTreeNode ans = new WorkItemTreeNode(item, level);
            return ans;
        }

        /// <summary>
        /// Creates a work item layout.
        /// </summary>
        /// <param name="layoutInformation">The list information to be used to create the work item layout.</param>
        /// <returns>An instance of the <see cref="WorkItemLayout"/> class.</returns>
        private WorkItemLayout CreateWorkItemLayout(LayoutInformation layoutInformation)
        {
            WorkItemLayout sut = new WorkItemLayout(layoutInformation, this.mockTeamProjectTemplate.Object);
            return sut;
        }

        /// <summary>
        /// Sets up a test layout.
        /// </summary>
        /// <param name="buildingBlockNames">The names of the building blocks that make up the test layout.</param>
        /// <returns>The <see cref="LayoutInformation"/> for the layout.</returns>
        private LayoutInformation SetupTestLayout(params BuildingBlockName[] buildingBlockNames)
        {
            LayoutInformation li = TestHelper.CreateTestLayoutInformation(buildingBlockNames);
            TestHelper.SetupMockTemplateWithBuildingBlocks(this.mockTeamProjectTemplate, li, buildingBlockNames.Select(bbn => TestHelper.CreateMockBuildingBlock(bbn)).ToArray());
            return li;
        }
    }
}
