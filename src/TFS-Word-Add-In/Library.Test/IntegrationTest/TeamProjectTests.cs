//---------------------------------------------------------------------
// <copyright file="TeamProjectTests.cs" company="Microsoft">
//    Copyright (c) Microsoft. All rights reserved.
//    This code is licensed under the MIT License.
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF 
//    ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
//    TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
//    PARTICULAR PURPOSE AND NONINFRINGEMENT.
// </copyright>
// <summary>The TeamProjectTests type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.Word4Tfs.Library.Test.IntegrationTest
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using Microsoft.Practices.Unity;
    using Microsoft.TeamFoundation.Client;
    using Microsoft.TeamFoundation.WorkItemTracking.Client;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Word4Tfs.Library.Model;
    using Microsoft.Word4Tfs.Library.Model.TeamFoundation;

    /// <summary>
    /// Integration tests for the TeamProject class
    /// </summary>
    [TestClass]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Unity container disposed in the test cleanup code.")]
    public class TeamProjectTests
    {
        /// <summary>
        /// The Unity container to be used for Dependency Injection
        /// </summary>
        private IUnityContainer container;

        /// <summary>
        /// The <see cref="TeamProject"/> to be tested.
        /// </summary>
        private ITeamProject sut;

        /// <summary>
        /// Initializes each test.
        /// </summary>
        [TestInitialize]
        public void InitializeTest()
        {
            this.container = new UnityContainer();
            TfsTeamProjectCollection collection = IntegrationTestHelper.Tpc;
            this.container.RegisterInstance<TfsTeamProjectCollection>(collection);
            this.container.RegisterInstance<ITfsQueryFactory>(new TfsQueryFactory(new WorkItemStore(collection)));

            this.sut = this.container.Resolve<TeamProject>(new ParameterOverride("projectName", TestHelper.ProjectName).OnType<TeamProject>());
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
        /// Tests that the constructor does not itself try to connect to TFS.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryIntegration)]
        public void ConstructorDoesNotConnectToTFS()
        {
            // Arrange
            using (TfsTeamProjectCollection bogusCollection = new TfsTeamProjectCollection(new Uri("http://bogusUri")))
            {
                // Act and assert, expecting constructor not to throw an exception for a bad collection uri.
                using (new TeamProject(bogusCollection, "dummy"))
                {
                }
            }
        }

        /// <summary>
        /// Tests that the project name passed to the constructor is returned in the <see cref="TeamProject.ProjectName"/> property.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryIntegration)]
        public void ProjectNamePropertyIsSetByConstructor()
        {
            // Act
            string ans = this.sut.TeamProjectInformation.ProjectName;

            // Assert
            Assert.AreEqual<string>(TestHelper.ProjectName, ans, "ProjectName property is not returning the value set in the constructor");
        }

        /// <summary>
        /// Tests that the uri of the team project collection passed to the constructor is returned in the <see cref="TeamProject.CollectionUri"/> property.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryIntegration)]
        public void CollectionUriPropertyIsSetByConstructor()
        {
            // Act
            Uri ans = this.sut.TeamProjectInformation.CollectionUri;

            // Assert
            Assert.AreEqual<Uri>(IntegrationTestHelper.Tpc.Uri, ans, "CollectionUri property is not returning the value set in the constructor");
        }

        /// <summary>
        /// Tests that the root query folder of the team project collection passed to the constructor is returned in the <see cref="TeamProject.RootQueryFolder"/> property.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryIntegration)]
        public void RootQueryFolderPropertyIsSetByConstructor()
        {
            // Act
            QueryFolder ans = this.sut.RootQueryFolder;

            // Assert
            Assert.AreEqual<string>(TestHelper.ProjectName, ans.Path, "RootQueryFolder property is not returning the value set in the constructor");
        }

        /// <summary>
        /// Tests that a flat list query returns a flat list of work items.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryIntegration)]
        public void FlatListQueryReturnsAFlatListOfWorkItems()
        {
            // Arrange
            QueryDefinition query = new QueryDefinition("Test", TestHelper.FlatQuery);

            // Act
            WorkItemTree ans = this.sut.QueryRunner.QueryForWorkItems(query, new CancellationToken());

            // Assert
            int count = 0;
            foreach (WorkItemTreeNode node in ans.DepthFirstNodes())
            {
                count++;
                Assert.AreEqual<int>(0, node.Level, "Flat list nodes should all be level 0");
            }

            Assert.IsTrue(count > 0, "The query did not return any work items");
            VerifyAndDumpWorkItemTree(ans.RootNodes, 0);
        }

        /// <summary>
        /// Tests that a tree query returns a tree of work items.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryIntegration)]
        public void TreeQueryReturnsATreeOfWorkItems()
        {
            // Arrange
            QueryDefinition query = new QueryDefinition("Test", TestHelper.TreeQuery);

            // Act
            WorkItemTree ans = this.sut.QueryRunner.QueryForWorkItems(query, new CancellationToken());

            // Assert
            int count = 0;
            int maxLevel = int.MinValue;
            int minLevel = int.MaxValue;
            foreach (WorkItemTreeNode node in ans.DepthFirstNodes())
            {
                count++;
                maxLevel = Math.Max(maxLevel, node.Level);
                minLevel = Math.Min(minLevel, node.Level);
            }

            Assert.IsTrue(count > 0, "The query did not return any work items.");
            Assert.AreEqual<int>(0, minLevel, "The root nodes should be at level 0.");
            Assert.IsTrue(maxLevel > 1, "There should be more than two levels in the result.");
            VerifyAndDumpWorkItemTree(ans.RootNodes, 0);
        }

        /// <summary>
        /// Tests that a one-hop query returns a two-level tree of work items.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryIntegration)]
        public void OneHopQueryReturnsATwoLevelTreeOfWorkItems()
        {
            // Arrange
            QueryDefinition query = new QueryDefinition("Test", TestHelper.OneHopQuery);

            // Act
            WorkItemTree ans = this.sut.QueryRunner.QueryForWorkItems(query, new CancellationToken());

            // Assert
            int count = 0;
            int maxLevel = int.MinValue;
            int minLevel = int.MaxValue;
            foreach (WorkItemTreeNode node in ans.DepthFirstNodes())
            {
                count++;
                maxLevel = Math.Max(maxLevel, node.Level);
                minLevel = Math.Min(minLevel, node.Level);
            }

            VerifyAndDumpWorkItemTree(ans.RootNodes, 0);
            Assert.IsTrue(count > 0, "The query did not return any work items.");
            Assert.AreEqual<int>(0, minLevel, "The root nodes should be at level 0.");
            Assert.AreEqual<int>(1, maxLevel, "There should be more than no more than two levels in the result.");
        }

        /// <summary>
        /// Tests that can get the list of field definitions for a Team Project.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryIntegration)]
        public void CanGetTheListOfFieldsForTheProject()
        {
            // Act
            ITfsFieldDefinition[] ans = this.sut.FieldDefinitions.ToArray();

            // Assert
            Assert.IsTrue(ans.Length > 0, "Could not get the list of fields defined for the project.");
            foreach (ITfsFieldDefinition fd in ans)
            {
                Console.WriteLine(fd.ReferenceName);
            }
        }

        /// <summary>
        /// Tests that can get the full history of a work item.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryIntegration)]
        public void CanGetFullHistoryOfAWorkItem()
        {
            // Arrange
            QueryDefinition query = new QueryDefinition("Test", TestHelper.AllFieldTypesQuery + " WHERE [System.Id] = 21406");

            // Act
            WorkItemTree ans = this.sut.QueryRunner.QueryForWorkItems(query, new CancellationToken());

            // Assert
            ITfsWorkItem item = ans.DepthFirstNodes().First().WorkItem;
            string expect = @"<html><head/><body>11/03/2011 10:25:11 Resolved with changeset 24111.<br>11/03/2011 10:19:43 &lt;no comment&gt;<br>10/03/2011 10:16:27 Associated with changeset 23952.<br>10/03/2011 10:14:22 Did some research and need some advice from the Word Product Group. Have sent email and now awaiting response.<br>10/03/2011 09:21:46 &lt;no comment&gt;<br></body></html>";
            Assert.AreEqual(expect, item["System.History"], "History not correct");
        }

        /// <summary>
        /// Tests that HTML field is retrieved with a &lt;HTML&gt; tag around it.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryIntegration)]
        public void HtmlFieldGetsHtmlTagWrapper()
        {
            // Arrange
            QueryDefinition query = new QueryDefinition("Test", TestHelper.AllFieldTypesQuery + " WHERE [System.Id] = 21404");

            // Act
            WorkItemTree ans = this.sut.QueryRunner.QueryForWorkItems(query, new CancellationToken());

            // Assert
            ITfsWorkItem item = ans.DepthFirstNodes().First().WorkItem;
            string actual = item["Microsoft.VSTS.TCM.ReproSteps"].ToString();
            Assert.IsTrue(actual.StartsWith("<html><head/><body><P"), "Field is not well-formed HTML, field content is: " + actual);
            Assert.IsTrue(actual.EndsWith("</P></body></html>"), "Field is not well-formed HTML, field content is: " + actual);
        }

        /// <summary>
        /// Prints out a work item tree and verifies that the levels are correct.
        /// </summary>
        /// <param name="nodes">The array of nodes to print.</param>
        /// <param name="level">The level of this node as seen from the point of view of the code traversing the tree.</param>
        private static void VerifyAndDumpWorkItemTree(IList<WorkItemTreeNode> nodes, int level)
        {
            foreach (WorkItemTreeNode node in nodes)
            {
                Assert.AreEqual<int>(level, node.Level);
                string indent = new string(' ', node.Level * 2);
                Console.WriteLine("{0} {1} {2}", indent, node.WorkItem.Id, node.WorkItem["System.Title"]);
                if (node.Children != null)
                {
                    VerifyAndDumpWorkItemTree(node.Children, level + 1);
                }
            }
        }
    }
}
