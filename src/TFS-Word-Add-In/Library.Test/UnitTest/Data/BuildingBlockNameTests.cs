//---------------------------------------------------------------------
// <copyright file="BuildingBlockNameTests.cs" company="Microsoft">
//    Copyright (c) Microsoft. All rights reserved.
//    This code is licensed under the MIT License.
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF 
//    ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
//    TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
//    PARTICULAR PURPOSE AND NONINFRINGEMENT.
// </copyright>
// <summary>The BuildingBlockNameTests type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.Word4Tfs.Library.Test.UnitTest.Data
{
    using Microsoft.Office.Interop.Word;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Word4Tfs.Library.Data;
    using Moq;

    /// <summary>
    /// Tests the <see cref="BuildingBlockName"/> class.
    /// </summary>
    [TestClass]
    public class BuildingBlockNameTests
    {
        /// <summary>
        /// The <see cref="BuildingBlockName"/> object under test.
        /// </summary>
        private BuildingBlockName sut;

        /// <summary>
        /// Tests that constructing a <see cref="BuildingBlockName"/> without a work item type constructs a default building block name.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void ConstructingDefaultNameWithoutLevelReturnsDefaultStringRepresentation()
        {
            // Arrange
            this.sut = new BuildingBlockName();

            // Act
            string ans = this.sut.ToString();

            // Assert
            Assert.AreEqual<string>(Constants.DefaultBuildingBlockName, ans);
        }

        /// <summary>
        /// Tests that constructing a <see cref="BuildingBlockName"/> without a work item type but with a level constructs a level-specific default building block name.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void ConstructingDefaultNameWithLevelReturnsLevelSpecificDefaultStringRepresentation()
        {
            // Arrange
            this.sut = new BuildingBlockName(1);

            // Act
            string ans = this.sut.ToString();

            // Assert
            Assert.AreEqual<string>(Constants.DefaultBuildingBlockName + "_1", ans);
        }

        /// <summary>
        /// Tests that constructing a <see cref="BuildingBlockName"/> without a level does not add the level to the string representation of the name.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void ConstructingNameWithoutLevelDoesNotAddLevelToStringRepresentation()
        {
            // Arrange
            this.sut = new BuildingBlockName("Test");

            // Act
            string ans = this.sut.ToString();

            // Assert
            Assert.AreEqual<string>("Test", ans);
        }

        /// <summary>
        /// Tests that constructing a <see cref="BuildingBlockName"/> with a level adds the level to the string representation of the name.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void ConstructingNameWithLevelAddsLevelToStringRepresentation()
        {
            // Arrange
            this.sut = new BuildingBlockName("Test", 1);

            // Act
            string ans = this.sut.ToString();

            // Assert
            Assert.AreEqual<string>("Test_1", ans);
        }

        /// <summary>
        /// Tests that constructing a <see cref="BuildingBlockName"/> with a <see cref="BuildingBlock"/> parses a non-level specific name
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void ConstructingNameWithBuildingBlockParsesNonLevelSpecificName()
        {
            // Arrange
            Mock<BuildingBlock> mockBb = new Mock<BuildingBlock>();
            mockBb.Setup(bb => bb.Name).Returns("Test");

            // Act
            this.sut = new BuildingBlockName(mockBb.Object);

            // Assert
            Assert.AreEqual<BuildingBlockName>(new BuildingBlockName("Test"), this.sut);
        }

        /// <summary>
        /// Tests that constructing a <see cref="BuildingBlockName"/> with a <see cref="BuildingBlock"/> parses a level-specific name.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void ConstructingNameWithBuildingBlockParsesLevelSpecificName()
        {
            // Arrange
            Mock<BuildingBlock> mockBb = new Mock<BuildingBlock>();
            mockBb.Setup(bb => bb.Name).Returns("Test_1");

            // Act
            this.sut = new BuildingBlockName(mockBb.Object);

            // Assert
            Assert.AreEqual<BuildingBlockName>(new BuildingBlockName("Test", 1), this.sut);
        }

        /// <summary>
        /// Tests that constructing a <see cref="BuildingBlockName"/> with a null <see cref="BuildingBlock"/> throws an <see cref="ArgumentNullException"/>.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void ConstructingNameWithNullBuildingBlockThrowsException()
        {
            TestHelper.TestForArgumentNullException(() => new BuildingBlockName(null as BuildingBlock), "buildingBlock");
        }

        /// <summary>
        /// Tests that the <see cref="BuildingBlockName.Default"/> property is the default building block.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void DefaultPropertyIsTheDefaultBuildingBlockName()
        {
            // Arrange
            this.sut = BuildingBlockName.Default;

            // Act
            string ans = this.sut.ToString();

            // Assert
            Assert.AreEqual<string>(Constants.DefaultBuildingBlockName, ans);
        }

        /// <summary>
        /// Tests that the <see cref="BuildingBlockName.Preview"/> property is the preview building block.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void PreviewPropertyIsThePreviewBuildingBlockName()
        {
            // Arrange
            this.sut = BuildingBlockName.Preview;

            // Act
            string ans = this.sut.ToString();

            // Assert
            Assert.AreEqual<string>(Constants.PreviewBuildingBlockName, ans);
        }

        /// <summary>
        /// Tests that <see cref="BuildingBlockName.Parse"/> parses a non-level specific name
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void ParseParsesNonLevelSpecificName()
        {
            // Act
            this.sut = BuildingBlockName.Parse("Test");

            // Assert
            Assert.AreEqual<BuildingBlockName>(new BuildingBlockName("Test"), this.sut);
        }

        /// <summary>
        /// Tests that <see cref="BuildingBlockName.Parse"/> parses a level-specific name.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void ParseParsesLevelSpecificName()
        {
            // Act
            this.sut = BuildingBlockName.Parse("Test_1");

            // Assert
            Assert.AreEqual<BuildingBlockName>(new BuildingBlockName("Test", 1), this.sut);
        }

        /// <summary>
        /// Tests that <see cref="BuildingBlockName.Parse"/> returns <see cref="BuildingBlockName.Default"/> when parsing the default name
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void ParseReturnsDefaultWhenParsesDefaultName()
        {
            // Act
            this.sut = BuildingBlockName.Parse(BuildingBlockName.Default.ToString());

            // Assert
            Assert.AreSame(BuildingBlockName.Default, this.sut, "Should return actual default object");
        }

        /// <summary>
        /// Tests that <see cref="BuildingBlockName.Parse"/> returns <see cref="BuildingBlockName.Preview"/> when parsing the preview name
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void ParseReturnsPreviewWhenParsesPreviewName()
        {
            // Act
            this.sut = BuildingBlockName.Parse(BuildingBlockName.Preview.ToString());

            // Assert
            Assert.AreSame(BuildingBlockName.Preview, this.sut, "Should return actual preview object");
        }

        /// <summary>
        /// Tests that <see cref="BuildingBlockName.Equals"/> does not match null.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void EqualsDoesNotMatchNull()
        {
            // Arrange
            BuildingBlockName name1 = new BuildingBlockName("Test");

            // Assert
            Assert.IsFalse(name1.Equals(null), "Should not be equal");
        }

        /// <summary>
        /// Tests that <see cref="BuildingBlockName.Equals"/> does not match non <see cref="BuildingBlockName"/>.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void EqualsDoesNotMatchNonBuildingBlockName()
        {
            // Arrange
            BuildingBlockName name1 = new BuildingBlockName("Test");

            // Assert
            Assert.IsFalse(name1.Equals("dummystring"), "Should not be equal");
        }

        /// <summary>
        /// Tests that two <see cref="BuildingBlockName"/> objects are equal if they have the same non-level specific name.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void EqualsMatchesTwoNonLevelSpecificNames()
        {
            // Arrange
            BuildingBlockName name1 = new BuildingBlockName("Test");
            BuildingBlockName name2 = new BuildingBlockName("Test");

            // Assert
            Assert.IsTrue(name1.Equals(name2), "Should be equal");
        }

        /// <summary>
        /// Tests that two <see cref="BuildingBlockName"/> objects are equal if they have the same non-level specific name using a different casing.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void EqualsMatchesTwoNonLevelSpecificNamesCaseInsensitively()
        {
            // Arrange
            BuildingBlockName name1 = new BuildingBlockName("Test");
            BuildingBlockName name2 = new BuildingBlockName("test");

            // Assert
            Assert.IsTrue(name1.Equals(name2), "Should be equal");
        }

        /// <summary>
        /// Tests that two <see cref="BuildingBlockName"/> objects are not equal if they have different non-level specific names.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void EqualsDoesNotMatchTwoDifferentNonLevelSpecificNames()
        {
            // Arrange
            BuildingBlockName name1 = new BuildingBlockName("Test");
            BuildingBlockName name2 = new BuildingBlockName("Test2");

            // Assert
            Assert.IsFalse(name1.Equals(name2), "Should not be equal");
        }

        /// <summary>
        /// Tests that two <see cref="BuildingBlockName"/> objects are equal if they have the same level-specific name.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void EqualsMatchesTwoLevelSpecificNames()
        {
            // Arrange
            BuildingBlockName name1 = new BuildingBlockName("Test", 1);
            BuildingBlockName name2 = new BuildingBlockName("Test", 1);

            // Assert
            Assert.IsTrue(name1.Equals(name2), "Should be equal");
        }

        /// <summary>
        /// Tests that two <see cref="BuildingBlockName"/> objects are equal if they have the same level-specific name using a different casing.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void EqualsMatchesTwoLevelSpecificNamesCaseInsensitively()
        {
            // Arrange
            BuildingBlockName name1 = new BuildingBlockName("Test", 1);
            BuildingBlockName name2 = new BuildingBlockName("test", 1);

            // Assert
            Assert.IsTrue(name1.Equals(name2), "Should be equal");
        }

        /// <summary>
        /// Tests that two <see cref="BuildingBlockName"/> objects are not equal if they have different levels for the same name.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void EqualsDoesNotMatchTwoDifferentLevelsForSameName()
        {
            // Arrange
            BuildingBlockName name1 = new BuildingBlockName("Test", 1);
            BuildingBlockName name2 = new BuildingBlockName("Test", 2);

            // Assert
            Assert.IsFalse(name1.Equals(name2), "Should not be equal");
        }

        /// <summary>
        /// Tests that two <see cref="BuildingBlockName"/> objects are equal using the == operator.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void EqualsOperatorMatchesTwoEqualBuildingBlockNames()
        {
            // Arrange
            BuildingBlockName name1 = new BuildingBlockName("Test");
            BuildingBlockName name2 = new BuildingBlockName("Test");

            // Assert
            Assert.IsTrue(name1 == name2, "Should be equal");
        }

        /// <summary>
        /// Tests that two <see cref="BuildingBlockName"/> objects are equal using the == operator ignoring case.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void EqualsOperatorMatchesTwoEqualBuildingBlockNamesCaseInsensitively()
        {
            // Arrange
            BuildingBlockName name1 = new BuildingBlockName("Test");
            BuildingBlockName name2 = new BuildingBlockName("test");

            // Assert
            Assert.IsTrue(name1 == name2, "Should be equal");
        }

        /// <summary>
        /// Tests that a null <see cref="BuildingBlockName"/> object is equal to null using the == operator.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void EqualsOperatorMatchesTwoNulls()
        {
            // Arrange
            BuildingBlockName name = null;

            // Assert
            Assert.IsTrue(name == null, "Should be equal");
        }

        /// <summary>
        /// Tests that two <see cref="BuildingBlockName"/> objects are not equal using the != operator.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void NotEqualsOperatorChecksObjectsForInequality()
        {
            // Arrange
            BuildingBlockName name1 = new BuildingBlockName("Test");
            BuildingBlockName name2 = new BuildingBlockName("Test");

            // Assert
            Assert.IsFalse(name1 != name2, "Should be equal");
        }

        /// <summary>
        /// Tests that two <see cref="BuildingBlockName"/> objects that are equal have the same hash code.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void EqualBuildingNamesHaveSameHashCode()
        {
            // Arrange
            BuildingBlockName name1 = new BuildingBlockName("Test");
            BuildingBlockName name2 = new BuildingBlockName("Test");

            // Assert
            Assert.AreEqual<int>(name1.GetHashCode(), name2.GetHashCode(), "Should be equal");
        }

        /// <summary>
        /// Tests that two <see cref="BuildingBlockName"/> objects that are not equal do not have the same hash code.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void NotEqualBuildingNamesHaveDifferentHashCode()
        {
            // Arrange
            BuildingBlockName name1 = new BuildingBlockName("Test");
            BuildingBlockName name2 = new BuildingBlockName("Test2");

            // Assert
            Assert.AreNotEqual<int>(name1.GetHashCode(), name2.GetHashCode(), "Should not be equal");
        }

        /// <summary>
        /// Tests that work item type with a level number in its name is not the same as a name with a proper level using == operator.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void LevelNumberInNameDoesNotMatchRealLevelNumberUsingEqualsOperator()
        {
            // Arrange
            BuildingBlockName name1 = new BuildingBlockName("Test_1");
            BuildingBlockName name2 = new BuildingBlockName("Test", 1);

            // Assert
            Assert.IsFalse(name1 == name2, "Should not be equal");
        }

        /// <summary>
        /// Tests that work item type with a level number in its name is not the same as a name with a proper level using != operator.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void LevelNumberInNameDoesNotMatchRealLevelNumberUsingNotEqualsOperator()
        {
            // Arrange
            BuildingBlockName name1 = new BuildingBlockName("Test_1");
            BuildingBlockName name2 = new BuildingBlockName("Test", 1);

            // Assert
            Assert.IsTrue(name1 != name2, "Should not be equal");
        }

        /// <summary>
        /// Tests that work item type with a level number in its name is not the same as a name with a proper level.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void LevelNumberInNameDoesNotMatchRealLevelNumber()
        {
            // Arrange
            BuildingBlockName name1 = new BuildingBlockName("Test_1");
            BuildingBlockName name2 = new BuildingBlockName("Test", 1);

            // Assert
            Assert.AreNotEqual<BuildingBlockName>(name1, name2, "Should not be equal");
        }

        /// <summary>
        /// Tests that work item type with a level number does not match work item type without a level number.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void WorkItemTypeWithLevelDoesNotMatchWorkItemTypeWithoutALevel()
        {
            // Arrange
            BuildingBlockName name1 = new BuildingBlockName("Test");
            BuildingBlockName name2 = new BuildingBlockName("Test", 1);

            // Assert
            Assert.AreNotEqual<BuildingBlockName>(name1, name2, "Should not be equal");
        }
    }
}
