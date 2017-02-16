//---------------------------------------------------------------------
// <copyright file="TeamProjectQueryTests.cs" company="Microsoft">
//    Copyright (c) Microsoft. All rights reserved.
//    This code is licensed under the MIT License.
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF 
//    ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
//    TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
//    PARTICULAR PURPOSE AND NONINFRINGEMENT.
// </copyright>
// <summary>The TeamProjectQueryTests type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.Word4Tfs.Library.Test.UnitTest.Model
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Threading;
    using Microsoft.Practices.Unity;
    using Microsoft.TeamFoundation.WorkItemTracking.Client;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Word4Tfs.Library.Model;
    using Microsoft.Word4Tfs.Library.Model.TeamFoundation;
    using Moq;

    /// <summary>
    /// Tests the <see cref="TeamProjectQuery"/> class.
    /// </summary>
    [TestClass]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Unity container disposed in the test cleanup code.")]
    public class TeamProjectQueryTests
    {
        /// <summary>
        /// The Unity container to be used for Dependency Injection
        /// </summary>
        private IUnityContainer container;

        /// <summary>
        /// The mock query factory to be passed to the sut.
        /// </summary>
        private Mock<ITfsQueryFactory> mockQueryFactory;

        /// <summary>
        /// The mock query that is given to the sut to perform the queries.
        /// </summary>
        private Mock<ITfsQuery> mockQuery;

        /// <summary>
        /// The <see cref="TeamProjectQuery"/> to be tested.
        /// </summary>
        private ITeamProjectQuery sut;

        /// <summary>
        /// Initializes each test.
        /// </summary>
        [TestInitialize]
        public void InitializeTest()
        {
            this.mockQueryFactory = new Mock<ITfsQueryFactory>();
            this.mockQuery = new Mock<ITfsQuery>();
            this.container = new UnityContainer();
            this.container.RegisterInstance<ITfsQueryFactory>(this.mockQueryFactory.Object);

            this.sut = this.container.Resolve<TeamProjectQuery>(new ParameterOverride("projectName", TestHelper.ProjectName).OnType<TeamProjectQuery>());
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
        /// QueryForWorkItems tests for null argument.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void QueryForWorkItemsTestsForNullArgument()
        {
            TestHelper.TestForArgumentNullException(() => this.sut.QueryForWorkItems(null, new CancellationToken()), "queryDefinition");
        }

        /// <summary>
        /// Project name is added to the macro dictionary.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void ProjectNameIsAddedToTheMacroDictionary()
        {
            // Arrange
            QueryDefinition qd = this.SetupFlatQuery();

            // Act
            this.sut.QueryForWorkItems(qd, new CancellationToken());

            // Assert
            this.mockQueryFactory.Verify(qf => qf.CreateTfsQuery(qd.QueryText, Match<IDictionary>.Create(d => d.Contains("project") && d["project"].ToString() == TestHelper.ProjectName)));
        }

        /// <summary>
        /// Flat query returns work items in flat structure
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void FlatQueryReturnsWorkItemsInFlatStructure()
        {
            // Arrange
            ITfsWorkItem w1 = CreateWorkItem(1);
            ITfsWorkItem w2 = CreateWorkItem(2);
            ITfsWorkItem w3 = CreateWorkItem(3);
            QueryDefinition qd = this.SetupFlatQuery(w1, w2, w3);

            // Act
            WorkItemTree ans = this.sut.QueryForWorkItems(qd, new CancellationToken());

            // Assert
            Assert.AreEqual<int>(3, ans.RootNodes.Count, "Incorrect number of root nodes returned");
            Assert.AreEqual<int>(w1.Id, ans.RootNodes[0].WorkItem.Id);
            Assert.AreEqual<int>(w2.Id, ans.RootNodes[1].WorkItem.Id);
            Assert.AreEqual<int>(w3.Id, ans.RootNodes[2].WorkItem.Id);
        }

        /// <summary>
        /// One-hop query allows same work item to appear in multiple places.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void OneHopQueryAllowsSameWorkItemToAppearInMultiplePlaces()
        {
            // Arrange
            Tuple<int, int>[] links = new Tuple<int, int>[]
            {
                new Tuple<int, int>(0, 1),
                new Tuple<int, int>(1, 3),
                new Tuple<int, int>(1, 5),
                new Tuple<int, int>(0, 2),
                new Tuple<int, int>(0, 3),
                new Tuple<int, int>(3, 7),
                new Tuple<int, int>(0, 5),
                new Tuple<int, int>(5, 6)
            };
            ITfsWorkItem w1 = CreateWorkItem(1);
            ITfsWorkItem w2 = CreateWorkItem(2);
            ITfsWorkItem w3 = CreateWorkItem(3);
            ITfsWorkItem w4 = CreateWorkItem(4);
            ITfsWorkItem w5 = CreateWorkItem(5);
            ITfsWorkItem w6 = CreateWorkItem(6);
            ITfsWorkItem w7 = CreateWorkItem(7);
            ITfsWorkItem[] workItems = new ITfsWorkItem[] { w1, w2, w3, w4, w5, w6, w7 };

            QueryDefinition qd = this.SetupOneHopQuery(links, workItems);

            // Act
            WorkItemTree ans = this.sut.QueryForWorkItems(qd, new CancellationToken());

            // Assert
            Assert.AreEqual<int>(4, ans.RootNodes.Count, "Incorrect number of root nodes returned");
            Assert.AreEqual<int>(1, ans.RootNodes[0].WorkItem.Id, "Invalid tree structure returned");
            Assert.AreEqual<int>(2, ans.RootNodes[1].WorkItem.Id, "Invalid tree structure returned");
            Assert.AreEqual<int>(3, ans.RootNodes[2].WorkItem.Id, "Invalid tree structure returned");
            Assert.AreEqual<int>(5, ans.RootNodes[3].WorkItem.Id, "Invalid tree structure returned");

            Assert.AreEqual<int>(2, ans.RootNodes[0].Children.Count, "Invalid tree structure returned");
            Assert.AreEqual<int>(3, ans.RootNodes[0].Children[0].WorkItem.Id, "Invalid tree structure returned");
            Assert.AreEqual<int>(5, ans.RootNodes[0].Children[1].WorkItem.Id, "Invalid tree structure returned");

            Assert.AreEqual<int>(0, ans.RootNodes[1].Children.Count, "Invalid tree structure returned");

            Assert.AreEqual<int>(1, ans.RootNodes[2].Children.Count, "Invalid tree structure returned");
            Assert.AreEqual<int>(7, ans.RootNodes[2].Children[0].WorkItem.Id, "Invalid tree structure returned");

            Assert.AreEqual<int>(1, ans.RootNodes[3].Children.Count, "Invalid tree structure returned");
            Assert.AreEqual<int>(6, ans.RootNodes[3].Children[0].WorkItem.Id, "Invalid tree structure returned");
        }

        /// <summary>
        /// One-hop query does not allow nesting more than one level.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void OneHopQueryDoesNotAllowNestingMoreThanOneLevel()
        {
            // Arrange
            Tuple<int, int>[] links = new Tuple<int, int>[]
            {
                new Tuple<int, int>(0, 1),
                new Tuple<int, int>(1, 2),
                new Tuple<int, int>(2, 3)
            };
            ITfsWorkItem w1 = CreateWorkItem(1);
            ITfsWorkItem w2 = CreateWorkItem(2);
            ITfsWorkItem w3 = CreateWorkItem(3);
            ITfsWorkItem[] workItems = new ITfsWorkItem[] { w1, w2, w3 };

            QueryDefinition qd = this.SetupOneHopQuery(links, workItems);

            // Act
            TestHelper.TestForInvalidOperationException(() => this.sut.QueryForWorkItems(qd, new CancellationToken()), "Query does not produce a well-formed direct link structure.");
        }

        /// <summary>
        /// One-hop query must have children following root node.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void OneHopQueryMustHaveChildrenFollowingRootNode()
        {
            // Arrange
            Tuple<int, int>[] links = new Tuple<int, int>[]
            {
                new Tuple<int, int>(0, 1),
                new Tuple<int, int>(2, 3)
            };
            ITfsWorkItem w1 = CreateWorkItem(1);
            ITfsWorkItem w2 = CreateWorkItem(2);
            ITfsWorkItem w3 = CreateWorkItem(3);
            ITfsWorkItem[] workItems = new ITfsWorkItem[] { w1, w2, w3 };

            QueryDefinition qd = this.SetupOneHopQuery(links, workItems);

            // Act
            TestHelper.TestForInvalidOperationException(() => this.sut.QueryForWorkItems(qd, new CancellationToken()), "Query does not produce a well-formed direct link structure.");
        }

        /// <summary>
        /// One-hop query must start with a root node.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void OneHopQueryMustStartWithARootNode()
        {
            // Arrange
            Tuple<int, int>[] links = new Tuple<int, int>[]
            {
                new Tuple<int, int>(1, 2)
            };
            ITfsWorkItem w1 = CreateWorkItem(1);
            ITfsWorkItem w2 = CreateWorkItem(2);
            ITfsWorkItem[] workItems = new ITfsWorkItem[] { w1, w2 };

            QueryDefinition qd = this.SetupOneHopQuery(links, workItems);

            // Act
            TestHelper.TestForInvalidOperationException(() => this.sut.QueryForWorkItems(qd, new CancellationToken()), "Query does not produce a well-formed direct link structure.");
        }
 
        /// <summary>
        /// Tree query runs link query.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void TreeQueryReturnsItemsInTreeStructure()
        {
            // Arrange
            Tuple<int, int>[] links = new Tuple<int, int>[]
            {
                new Tuple<int, int>(0, 1),
                new Tuple<int, int>(0, 2),
                new Tuple<int, int>(0, 3),
                new Tuple<int, int>(1, 4),
                new Tuple<int, int>(1, 5),
                new Tuple<int, int>(4, 6),
                new Tuple<int, int>(3, 7)
            };
            ITfsWorkItem w1 = CreateWorkItem(1);
            ITfsWorkItem w2 = CreateWorkItem(2);
            ITfsWorkItem w3 = CreateWorkItem(3);
            ITfsWorkItem w4 = CreateWorkItem(4);
            ITfsWorkItem w5 = CreateWorkItem(5);
            ITfsWorkItem w6 = CreateWorkItem(6);
            ITfsWorkItem w7 = CreateWorkItem(7);
            ITfsWorkItem[] workItems = new ITfsWorkItem[] { w1, w2, w3, w4, w5, w6, w7 };

            QueryDefinition qd = this.SetupTreeQuery(links, workItems);

            // Act
            WorkItemTree ans = this.sut.QueryForWorkItems(qd, new CancellationToken());

            // Assert
            Assert.AreEqual<int>(3, ans.RootNodes.Count, "Incorrect number of root nodes returned");
            Assert.AreEqual<int>(1, ans.RootNodes[0].WorkItem.Id, "Invalid tree structure returned");
            Assert.AreEqual<int>(2, ans.RootNodes[0].Children.Count, "Invalid tree structure returned");

            Assert.AreEqual<int>(4, ans.RootNodes[0].Children[0].WorkItem.Id, "Invalid tree structure returned");
            Assert.AreEqual<int>(1, ans.RootNodes[0].Children[0].Children.Count, "Invalid tree structure returned");
            
            Assert.AreEqual<int>(6, ans.RootNodes[0].Children[0].Children[0].WorkItem.Id, "Invalid tree structure returned");
            Assert.AreEqual<int>(0, ans.RootNodes[0].Children[0].Children[0].Children.Count, "Invalid tree structure returned");

            Assert.AreEqual<int>(5, ans.RootNodes[0].Children[1].WorkItem.Id, "Invalid tree structure returned");

            Assert.AreEqual<int>(2, ans.RootNodes[1].WorkItem.Id, "Invalid tree structure returned");
            Assert.AreEqual<int>(0, ans.RootNodes[1].Children.Count, "Invalid tree structure returned");
            
            Assert.AreEqual<int>(3, ans.RootNodes[2].WorkItem.Id, "Invalid tree structure returned");
            Assert.AreEqual<int>(1, ans.RootNodes[2].Children.Count, "Invalid tree structure returned");

            Assert.AreEqual<int>(7, ans.RootNodes[2].Children[0].WorkItem.Id, "Invalid tree structure returned");
            Assert.AreEqual<int>(0, ans.RootNodes[2].Children[0].Children.Count, "Invalid tree structure returned");
        }

        /// <summary>
        /// Tree query runs link query.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void TreeQueryOverInvalidTreeStructureThrowsExcpetion()
        {
            // Arrange
            Tuple<int, int>[] links = new Tuple<int, int>[]
            {
                new Tuple<int, int>(0, 1),
                new Tuple<int, int>(0, 2),
                new Tuple<int, int>(0, 3),
                new Tuple<int, int>(1, 3),
                new Tuple<int, int>(1, 5),
                new Tuple<int, int>(5, 6),
                new Tuple<int, int>(3, 7)
            };
            ITfsWorkItem w1 = CreateWorkItem(1);
            ITfsWorkItem w2 = CreateWorkItem(2);
            ITfsWorkItem w3 = CreateWorkItem(3);
            ITfsWorkItem w4 = CreateWorkItem(4);
            ITfsWorkItem w5 = CreateWorkItem(5);
            ITfsWorkItem w6 = CreateWorkItem(6);
            ITfsWorkItem w7 = CreateWorkItem(7);
            ITfsWorkItem[] workItems = new ITfsWorkItem[] { w1, w2, w3, w4, w5, w6, w7 };

            QueryDefinition qd = this.SetupTreeQuery(links, workItems);

            // Act
            TestHelper.TestForInvalidOperationException(() => this.sut.QueryForWorkItems(qd, new CancellationToken()), "Query does not produce a well-formed tree.");
        }
        
        /// <summary>
        /// Tree query passes modified query to work item store with just the display fields.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void TreeQueryModifiesQueryToWorkItemStoreToJustGetDisplayFields()
        {
            // Arrange
            Tuple<int, int>[] links = new Tuple<int, int>[0];
            ITfsWorkItem[] workItems = new ITfsWorkItem[0];

            QueryDefinition qd = this.SetupTreeQuery(links, workItems, new string[] { "[field1]", "[field2]" });

            // Act
            WorkItemTree ans = this.sut.QueryForWorkItems(qd, new CancellationToken());

            // Assert
            this.mockQuery.Verify(q => q.WorkItemStore.Query(It.IsAny<int[]>(), It.Is<string>(s => s == "SELECT [field1], [field2] FROM WorkItems")));
        }

        /// <summary>
        /// Tree query with duplicate ids in the links passes de-duped array of ids to the work item store query.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void TreeQueryWithDuplicateLinkIdsPassedDedupedArrayToWorkItemStoreQuery()
        {
            // Arrange
            Tuple<int, int>[] links = new Tuple<int, int>[]
            {
                new Tuple<int, int>(0, 3),
                new Tuple<int, int>(0, 1),
                new Tuple<int, int>(0, 2),
                new Tuple<int, int>(0, 1),
                new Tuple<int, int>(0, 2)
            };
            ITfsWorkItem[] workItems = new ITfsWorkItem[] { CreateWorkItem(1), CreateWorkItem(2), CreateWorkItem(3) };

            QueryDefinition qd = this.SetupTreeQuery(links, workItems);

            // Act
            WorkItemTree ans = this.sut.QueryForWorkItems(qd, new CancellationToken());

            // Assert
            this.mockQuery.Verify(q => q.WorkItemStore.Query(TestHelper.UnorderedUniqueArray<int>(1, 2, 3), It.IsAny<string>()));
        }

        /// <summary>
        /// Tree query builds tree correctly even if work items returned in a different order.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void TreeQueryBuildsTreeCorrectlyEvenIfWorkItemsReturnedInADifferentOrder()
        {
            // Arrange
            Tuple<int, int>[] links = new Tuple<int, int>[]
            {
                new Tuple<int, int>(0, 1),
                new Tuple<int, int>(0, 2),
                new Tuple<int, int>(0, 3)
            };
            ITfsWorkItem[] workItems = new ITfsWorkItem[] { CreateWorkItem(3), CreateWorkItem(2), CreateWorkItem(1) };

            QueryDefinition qd = this.SetupTreeQuery(links, workItems);

            // Act
            WorkItemTree ans = this.sut.QueryForWorkItems(qd, new CancellationToken());

            // Assert
            Assert.AreEqual<int>(3, ans.RootNodes.Count, "Incorrect number of root nodes returned");
            Assert.AreEqual<int>(1, ans.RootNodes[0].WorkItem.Id);
            Assert.AreEqual<int>(2, ans.RootNodes[1].WorkItem.Id);
            Assert.AreEqual<int>(3, ans.RootNodes[2].WorkItem.Id);
        }

        /// <summary>
        /// Creates a mock work item with the given id.
        /// </summary>
        /// <param name="id">The id of the work item to create.</param>
        /// <returns>The mock work item.</returns>
        private static ITfsWorkItem CreateWorkItem(int id)
        {
            Mock<ITfsWorkItem> ans = new Mock<ITfsWorkItem>();
            ans.Setup(wi => wi.Id).Returns(id);
            return ans.Object;
        }

        /// <summary>
        /// Sets up a flat query to return some work items.
        /// </summary>
        /// <param name="workItems">The work items to return.</param>
        /// <returns>The query definition to use.</returns>
        private QueryDefinition SetupFlatQuery(params ITfsWorkItem[] workItems)
        {
            QueryDefinition qd = new QueryDefinition("Test", TestHelper.FlatQuery);
            this.mockQuery = new Mock<ITfsQuery>();
            this.mockQuery.Setup(q => q.RunQuery(It.IsAny<CancellationToken>())).Returns(new List<ITfsWorkItem>(workItems));
            this.mockQueryFactory.Setup(qf => qf.CreateTfsQuery(It.IsAny<string>(), It.IsAny<IDictionary>())).Returns(this.mockQuery.Object);
            return qd;
        }

        /// <summary>
        /// Sets up a tree query to return some work items.
        /// </summary>
        /// <param name="links">Set of work item id tuples to return from the link query. The first is the source id, the second is the target id.</param>
        /// <param name="workItems">The work items to return.</param>
        /// <returns>The query definition to use.</returns>
        private QueryDefinition SetupTreeQuery(Tuple<int, int>[] links, ITfsWorkItem[] workItems)
        {
            return this.SetupTreeQuery(links, workItems, new string[0]);
        }

        /// <summary>
        /// Sets up a tree query to return some work items.
        /// </summary>
        /// <param name="links">Set of work item id tuples to return from the link query. The first is the source id, the second is the target id.</param>
        /// <param name="workItems">The work items to return.</param>
        /// <param name="fieldNames">The list of field names to be returned by the query.</param>
        /// <returns>The query definition to use.</returns>
        private QueryDefinition SetupTreeQuery(Tuple<int, int>[] links, ITfsWorkItem[] workItems, string[] fieldNames)
        {
            QueryDefinition qd = new QueryDefinition("Test", TestHelper.TreeQuery);

            this.SetupHierarchicalQuery(links, workItems, fieldNames);

            return qd;
        }

        /// <summary>
        /// Sets up an one-hop query to return some work items.
        /// </summary>
        /// <param name="links">Set of work item id tuples to return from the link query. The first is the source id, the second is the target id.</param>
        /// <param name="workItems">The work items to return.</param>
        /// <returns>The query definition to use.</returns>
        private QueryDefinition SetupOneHopQuery(Tuple<int, int>[] links, ITfsWorkItem[] workItems)
        {
            return this.SetupOneHopQuery(links, workItems, new string[0]);
        }

        /// <summary>
        /// Sets up an one-hop query to return some work items.
        /// </summary>
        /// <param name="links">Set of work item id tuples to return from the link query. The first is the source id, the second is the target id.</param>
        /// <param name="workItems">The work items to return.</param>
        /// <param name="fieldNames">The list of field names to be returned by the query.</param>
        /// <returns>The query definition to use.</returns>
        private QueryDefinition SetupOneHopQuery(Tuple<int, int>[] links, ITfsWorkItem[] workItems, string[] fieldNames)
        {
            QueryDefinition qd = new QueryDefinition("Test", TestHelper.OneHopQuery);

            this.SetupHierarchicalQuery(links, workItems, fieldNames);

            return qd;
        }

        /// <summary>
        /// Sets up a hierarchical query to return some work items.
        /// </summary>
        /// <param name="links">Set of work item id tuples to return from the link query. The first is the source id, the second is the target id.</param>
        /// <param name="workItems">The work items to return.</param>
        /// <param name="fieldNames">The list of field names to be returned by the query.</param>
        private void SetupHierarchicalQuery(Tuple<int, int>[] links, ITfsWorkItem[] workItems, string[] fieldNames)
        {
            WorkItemLinkInfo[] wilis = new WorkItemLinkInfo[links.Length];
            for (int i = 0; i < links.Length; i++)
            {
                wilis[i] = new WorkItemLinkInfo() { SourceId = links[i].Item1, TargetId = links[i].Item2 };
            }

            IList<ITfsFieldDefinition> displayFields = new List<ITfsFieldDefinition>();
            foreach (string name in fieldNames)
            {
                Mock<ITfsFieldDefinition> mockFd = new Mock<ITfsFieldDefinition>();
                mockFd.Setup(fd => fd.ReferenceName).Returns(name);
                displayFields.Add(mockFd.Object);
            }

            this.mockQuery.Setup(q => q.RunLinkQuery(It.IsAny<CancellationToken>())).Returns(wilis);
            this.mockQuery.Setup(q => q.DisplayFieldList).Returns(displayFields);
            this.mockQuery.Setup(q => q.WorkItemStore.Query(It.IsAny<int[]>(), It.IsAny<string>())).Returns(new List<ITfsWorkItem>(workItems));
            this.mockQueryFactory.Setup(qf => qf.CreateTfsQuery(It.IsAny<string>(), It.IsAny<IDictionary>())).Returns(this.mockQuery.Object);
        }
    }
}
