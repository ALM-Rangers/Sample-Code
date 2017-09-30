//---------------------------------------------------------------------
// <copyright file="WorkItemManagerTests.cs" company="Microsoft">
//    Copyright © ALM | DevOps Ranger Contributors. All rights reserved.
//    This code is licensed under the MIT License.
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF 
//    ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
//    TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
//    PARTICULAR PURPOSE AND NONINFRINGEMENT.
// </copyright>
// <summary>The WorkItemManagerTests type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.Word4Tfs.Library.Test.UnitTest.Model
{
    using System.Linq;
    using Microsoft.Practices.Unity;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Word4Tfs.Library.Model;
    using Microsoft.Word4Tfs.Library.Model.TeamFoundation;

    /// <summary>
    /// Tests the <see cref="WorkItemManager"/> class.
    /// </summary>
    [TestClass]
    [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Unity container disposed in the test cleanup code.")]
    public class WorkItemManagerTests
    {
        /// <summary>
        /// The Unity container to be used for Dependency Injection
        /// </summary>
        private IUnityContainer container;

        /// <summary>
        /// The work item manager to be tested.
        /// </summary>
        private WorkItemManager sut;

        /// <summary>
        /// Initializes the test.
        /// </summary>
        [TestInitialize]
        public void InitializeTest()
        {
            this.container = new UnityContainer();

            this.sut = new WorkItemManager();
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
        /// Tests that work item manager initially has no work items.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void InitiallyHasNoWorkItems()
        {
            // Act
            int count = this.sut.WorkItems.Count();

            // Assert
            Assert.AreEqual<int>(0, count, "There should not be any work items stored by the manager");
        }

        /// <summary>
        /// <see cref="WorkItemManager.Add"/> throws an exception if passed a null.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void AddThrowsExceptionForANullArgument()
        {
            // Arrange
            ITfsWorkItem nullItem = null;

            // Act and Assert
            TestHelper.TestForArgumentNullException(() => this.sut.AddRange(nullItem), "workItem");
        }

        /// <summary>
        /// Can add a work item to the manager.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void CanAddAWorkItemToTheManager()
        {
            // Arrange
            ITfsWorkItem item = TestHelper.CreateMockWorkItem(1);

            // Act
            this.sut.Add(item);

            // Assert
            Assert.AreEqual<int>(1, this.sut.WorkItems.Count(), "There should be 1 work item stored by the manager now");
            Assert.AreSame(item, this.sut.WorkItems.First(), "Work item not stored in the manager");
        }

        /// <summary>
        /// Can add more than one work item to the manager.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void CanAddMoreThanOneWorkItemToTheManager()
        {
            // Arrange
            ITfsWorkItem item1 = TestHelper.CreateMockWorkItem(1);
            ITfsWorkItem item2 = TestHelper.CreateMockWorkItem(2);

            // Act
            this.sut.Add(item1);
            this.sut.Add(item2);

            // Assert
            Assert.AreEqual<int>(2, this.sut.WorkItems.Count(), "There should be 2 work items stored by the manager now");
            Assert.IsTrue(TestHelper.CheckUnorderedUniqueArrayMatch<ITfsWorkItem>(new ITfsWorkItem[] { item1, item2 }, this.sut.WorkItems.ToArray()), "Work items stored by manager do not match those passed to the Add call.");
        }

        /// <summary>
        /// Can add more than one work item to the manager at the same time.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void CanAddMoreThanOneWorkItemToTheManagerAtTheSameTime()
        {
            // Arrange
            ITfsWorkItem item1 = TestHelper.CreateMockWorkItem(1);
            ITfsWorkItem item2 = TestHelper.CreateMockWorkItem(2);

            // Act
            this.sut.AddRange(item1, item2);

            // Assert
            Assert.AreEqual<int>(2, this.sut.WorkItems.Count(), "There should be 2 work items stored by the manager now");
            Assert.IsTrue(TestHelper.CheckUnorderedUniqueArrayMatch<ITfsWorkItem>(new ITfsWorkItem[] { item1, item2 }, this.sut.WorkItems.ToArray()), "Work items stored by manager do not match those passed to the Add call.");
        }

        /// <summary>
        /// Adding a work item with the same id as one that already exists overwrites the existing one.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void AddOverwritesExistingWorkItemWithSameId()
        {
            // Arrange
            ITfsWorkItem item1 = TestHelper.CreateMockWorkItem(1);
            ITfsWorkItem item2 = TestHelper.CreateMockWorkItem(1);

            // Act
            this.sut.Add(item1);
            this.sut.Add(item2);

            // Assert
            Assert.AreEqual<int>(1, this.sut.WorkItems.Count(), "There should be only 1 work item stored by the manager now");
            Assert.AreSame(item2, this.sut.WorkItems.First(), "Work item not stored in the manager");
        }

        /// <summary>
        /// <see cref="WorkItemManager.AddRange"/> throws an exception if passed a null.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void AddRangeThrowsExceptionForANullArgument()
        {
            // Arrange
            ITfsWorkItem[] nullList = null;

            // Act and Assert
            TestHelper.TestForArgumentNullException(() => this.sut.AddRange(nullList), "workItems");
        }

        /// <summary>
        /// Adding a set of work item with some with the same id as one that already exists overwrites the existing one.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void AddRangeOverwritesExistingWorkItemWithSameId()
        {
            // Arrange
            ITfsWorkItem item1 = TestHelper.CreateMockWorkItem(1);
            ITfsWorkItem item2 = TestHelper.CreateMockWorkItem(2);
            ITfsWorkItem item3 = TestHelper.CreateMockWorkItem(1);
            ITfsWorkItem item4 = TestHelper.CreateMockWorkItem(2);
            ITfsWorkItem item5 = TestHelper.CreateMockWorkItem(3);
            Assert.AreNotEqual<ITfsWorkItem>(item1, item3, "Pre-req check failed");

            // Act
            this.sut.AddRange(item1, item2);
            this.sut.AddRange(item3, item4, item5);

            // Assert
            Assert.AreEqual<int>(3, this.sut.WorkItems.Count(), "There should be only 3 work item stored by the manager now");
            Assert.IsTrue(TestHelper.CheckUnorderedUniqueArrayMatch<ITfsWorkItem>(new ITfsWorkItem[] { item3, item4, item5 }, this.sut.WorkItems.ToArray()), "Work items not overwritten.");
        }

        /// <summary>
        /// Adding a list of work item with duplicates in the list being added overwrites the earlier ones in the list.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void AddRangeWithDuplicatesInListBeingAddedOverwritesEarlierWorkItemWithSameId()
        {
            // Arrange
            ITfsWorkItem item1 = TestHelper.CreateMockWorkItem(1);
            ITfsWorkItem item2 = TestHelper.CreateMockWorkItem(2);
            ITfsWorkItem item3 = TestHelper.CreateMockWorkItem(1);
            ITfsWorkItem item4 = TestHelper.CreateMockWorkItem(2);
            ITfsWorkItem item5 = TestHelper.CreateMockWorkItem(3);

            // Act
            this.sut.AddRange(item1, item2, item3, item4, item5);

            // Assert
            Assert.AreEqual<int>(3, this.sut.WorkItems.Count(), "There should be only 3 work item stored by the manager now");
            Assert.IsTrue(TestHelper.CheckUnorderedUniqueArrayMatch<ITfsWorkItem>(new ITfsWorkItem[] { item3, item4, item5 }, this.sut.WorkItems.ToArray()), "Work items not overwritten.");
        }

        /// <summary>
        /// Can lookup by work item id.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void CanLookupByWorkItemId()
        {
            // Arrange
            ITfsWorkItem item1 = TestHelper.CreateMockWorkItem(1);
            ITfsWorkItem item2 = TestHelper.CreateMockWorkItem(2);
            this.sut.Add(item1);
            this.sut.Add(item2);

            // Act
            ITfsWorkItem ans = this.sut[2];

            // Assert
            Assert.AreSame(item2, ans, "Failed to lookup by work item id");
        }

        /// <summary>
        /// Lookup returns null for work item id that does not exist.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void LookupReturnsNullForNonExistentWorkItemId()
        {
            // Arrange
            ITfsWorkItem item1 = TestHelper.CreateMockWorkItem(1);
            this.sut.Add(item1);

            // Act
            ITfsWorkItem ans = this.sut[2];

            // Assert
            Assert.IsNull(ans, "Expect null for work item id that does not exist.");
        }

        /// <summary>
        /// Tests that <see cref="WorkItemManager.Clear"/> removes existing work items.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void ClearRemovesItems()
        {
            // Arrange
            ITfsWorkItem item = TestHelper.CreateMockWorkItem(1);
            this.sut.Add(item);

            // Act
            this.sut.Clear();

            // Assert
            Assert.AreEqual<int>(0, this.sut.WorkItems.Count(), "There should not be any work items stored by the manager now");
        }
    }
}
