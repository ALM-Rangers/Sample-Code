//---------------------------------------------------------------------
// <copyright file="QueryAndLayoutManagerTests.cs" company="Microsoft">
//    Copyright © ALM | DevOps Ranger Contributors. All rights reserved.
//    This code is licensed under the MIT License.
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF 
//    ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
//    TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
//    PARTICULAR PURPOSE AND NONINFRINGEMENT.
// </copyright>
// <summary>The QueryAndLayoutManagerTests type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.Word4Tfs.Library.Test.UnitTest.Model
{
    using System.Globalization;
    using System.Linq;
    using Microsoft.Practices.Unity;
    using Microsoft.TeamFoundation.WorkItemTracking.Client;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Word4Tfs.Library.Data;
    using Microsoft.Word4Tfs.Library.Model;

    /// <summary>
    /// Tests the <see cref="QueryAndLayoutManager"/>.
    /// </summary>
    [TestClass]
    [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Unity container disposed in the test cleanup code.")]
    public class QueryAndLayoutManagerTests
    {
        /// <summary>
        /// A simple query for just getting the id of work items.
        /// </summary>
        private const string SystemIdOnlyQuery = "SELECT [System.Id] FROM WorkItems";

        /// <summary>
        /// A simple query for just getting the title of work items.
        /// </summary>
        private const string SystemTitleOnlyQuery = "SELECT [System.Title] FROM WorkItems";

        /// <summary>
        /// The reference name of the id field.
        /// </summary>
        private const string SystemIdReferenceName = "System.Id";

        /// <summary>
        /// The reference name of the title field.
        /// </summary>
        private const string SystemTitleReferenceName = "System.Title";

        /// <summary>
        /// The reference name of the description field.
        /// </summary>
        private const string SystemDescriptionReferenceName = "System.Description";

        /// <summary>
        /// The reference name of the area field.
        /// </summary>
        private const string SystemAreaReferenceName = "System.Area";

        /// <summary>
        /// The Unity container to be used for Dependency Injection
        /// </summary>
        private IUnityContainer container;

        /// <summary>
        /// The query and layout manager to be tested.
        /// </summary>
        private IQueryAndLayoutManager sut;

        /// <summary>
        /// Initializes the test.
        /// </summary>
        [TestInitialize]
        public void InitializeTest()
        {
            this.container = new UnityContainer();

            this.sut = new QueryAndLayoutManager(TestHelper.CreateMockFieldDefinitions(SystemIdReferenceName, SystemTitleReferenceName, SystemDescriptionReferenceName, SystemAreaReferenceName));
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
        /// Tests that queries and layouts manager initially has no queries and layouts.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void InitiallyHasNoQueriesAndLayouts()
        {
            // Act
            int count = this.sut.OriginalQueriesAndLayouts.Count();

            // Assert
            Assert.AreEqual<int>(0, count, "There should not be any queries and layouts stored by the manager");
        }

        /// <summary>
        /// Can add a query and layout to the manager.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void CanAddAQueryAndLayoutToTheManager()
        {
            // Arrange
            QueryAndLayoutInformation newItem = TestHelper.ValidQueryDefinitionAndLayout;

            // Act
            this.sut.Add(newItem);

            // Assert
            Assert.AreEqual<int>(1, this.sut.OriginalQueriesAndLayouts.Count(), "There should now be one query and layout stored in the manager");
            Assert.AreSame(newItem, this.sut.OriginalQueriesAndLayouts.First(), "The query and layout object that was added should be returned.");
        }

        /// <summary>
        /// Can add more than one query and layout to the manager at one time.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void CanAddMoreThanOneQueryAndLayoutToTheManagerAtOneTime()
        {
            // Arrange
            QueryAndLayoutInformation newItem1 = TestHelper.ValidQueryDefinitionAndLayout;
            QueryAndLayoutInformation newItem2 = TestHelper.ValidQueryDefinitionAndLayout;
            Assert.AreNotSame(newItem1, newItem2, "Test not valid if the two objects are actually the same object.");

            // Act
            this.sut.AddRange(newItem1, newItem2);

            // Assert
            Assert.IsTrue(TestHelper.CheckUnorderedUniqueArrayMatch<QueryAndLayoutInformation>(new QueryAndLayoutInformation[] { newItem1, newItem2 }, this.sut.OriginalQueriesAndLayouts.ToArray()), "Distinct items not returned.");
        }

        /// <summary>
        /// Can add more than one query and layout to the manager.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void CanAddMoreThanOneQueryAndLayoutToTheManager()
        {
            // Arrange
            QueryAndLayoutInformation newItem1 = TestHelper.ValidQueryDefinitionAndLayout;
            QueryAndLayoutInformation newItem2 = TestHelper.ValidQueryDefinitionAndLayout;
            Assert.AreNotSame(newItem1, newItem2, "Test not valid if the two objects are actually the same object.");

            // Act
            this.sut.Add(newItem1);
            this.sut.Add(newItem2);

            // Assert
            Assert.IsTrue(TestHelper.CheckUnorderedUniqueArrayMatch<QueryAndLayoutInformation>(new QueryAndLayoutInformation[] { newItem1, newItem2 }, this.sut.OriginalQueriesAndLayouts.ToArray()), "Distinct items not returned.");
        }

        /// <summary>
        /// Tests that <see cref="QueryAndLayoutManager.FinalQueriesAndLayouts"/> preserves the name in the query definition.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void FinalQueriesAndLayoutsPreservesQueryDefinitionNames()
        {
            // Arrange
            QueryAndLayoutInformation newItem = TestHelper.ValidQueryDefinitionAndLayout;
            this.sut.Add(newItem);

            // Act
            QueryAndLayoutInformation ans = this.sut.FinalQueriesAndLayouts.First();

            // Assert
            Assert.AreEqual<string>(newItem.Query.Name, ans.Query.Name, "QueryDefinition name not preserved");
        }

        /// <summary>
        /// Tests that <see cref="QueryAndLayoutManager.FinalQueriesAndLayouts"/> preserves the layout.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void FinalQueriesAndLayoutsPreservesLayout()
        {
            // Arrange
            QueryAndLayoutInformation newItem = TestHelper.ValidQueryDefinitionAndLayout;
            this.sut.Add(newItem);

            // Act
            QueryAndLayoutInformation ans = this.sut.FinalQueriesAndLayouts.First();

            // Assert
            Assert.AreSame(newItem.Layout, ans.Layout, "Layout not preserved");
        }

        /// <summary>
        /// Tests that <see cref="QueryAndLayoutManager.FinalQueriesAndLayouts"/> can parse a query that contains newlines.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void FinalQueryCanBeConstructedFromQueryContainingNewlineCharacters()
        {
            // Arrange
            QueryAndLayoutInformation ql = TestHelper.CreateQueryAndLayout(
                                                                            @"SELECT [System.Id], [System.WorkItemType]
                                                                            FROM WorkItemLinks
                                                                            WHERE Source.[System.TeamProject] = @project
                                                                            ORDER BY [Microsoft.VSTS.Common.BacklogPriority] ASC, [System.Id] ASC
                                                                            MODE (Recursive)
                                                                            ",
                                                                            SystemIdReferenceName);
            this.sut.Add(ql);

            // Act
            QueryAndLayoutInformation ans = this.sut.FinalQueriesAndLayouts.First();

            // Assert
            Assert.IsNotNull(ans.Query);
        }

        /// <summary>
        /// Existing fields listed in the layout are not added again to a final query.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void ExistingFieldsInTheLayoutNotAddedAgainToFinalQuery()
        {
            // Arrange
            QueryAndLayoutInformation ql = TestHelper.CreateQueryAndLayout(SystemIdOnlyQuery, SystemIdReferenceName);
            this.sut.Add(ql);

            // Act
            QueryAndLayoutInformation ans = this.sut.FinalQueriesAndLayouts.First();

            // Assert
            Assert.AreEqual<string>(SystemIdOnlyQuery, ans.Query.QueryText, "Changed query when no new fields were supposed to be added.");
        }

        /// <summary>
        /// Fields listed in the layout which are not in the team project are not added to the final query.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void FieldsInTheLayoutNotInTeamProjectAreNotAddedToFinalQuery()
        {
            // Arrange
            QueryAndLayoutInformation ql = TestHelper.CreateQueryAndLayout(SystemIdOnlyQuery, "System.NoSuchField");
            this.sut.Add(ql);

            // Act
            QueryAndLayoutInformation ans = this.sut.FinalQueriesAndLayouts.First();

            // Assert
            Assert.AreEqual<string>(SystemIdOnlyQuery, ans.Query.QueryText, "Non-existent project field should not be added to the final query");
        }

        /// <summary>
        /// Tests that layout fields are merged into the final query for a flat query.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void LayoutFieldsMergedIntoFinalFlatQuery()
        {
            // Arrange
            QueryAndLayoutInformation ql = TestHelper.CreateQueryAndLayout(SystemIdOnlyQuery, SystemTitleReferenceName);
            this.sut.Add(ql);

            // Act
            QueryAndLayoutInformation ans = this.sut.FinalQueriesAndLayouts.First();

            // Assert
            Assert.AreEqual<string>("SELECT [System.Id], [System.Title] FROM WorkItems", ans.Query.QueryText, "Layout fields not merged correctly");
        }

        /// <summary>
        /// Tests that casing and white space is ignored when merging fields from the layout.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void IgnoresCaseAndWhiteSpaceWhenMergingFieldsFromLayout()
        {
            // Arrange
            QueryAndLayoutInformation ql = TestHelper.CreateQueryAndLayout("select    [system.id]   from WorkItems", SystemIdReferenceName.ToUpperInvariant(), SystemTitleReferenceName);
            this.sut.Add(ql);

            // Act
            QueryAndLayoutInformation ans = this.sut.FinalQueriesAndLayouts.First();

            // Assert
            Assert.AreEqual<string>("SELECT [system.id]  , [System.Title] FROM WorkItems", ans.Query.QueryText, "Layout fields not merged correctly");
        }

        /// <summary>
        /// Each query merged with all fields from all layouts.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void EachQueryMergedWithAllFieldsFromAllLayouts()
        {
            // Arrange
            QueryAndLayoutInformation ql1 = TestHelper.CreateQueryAndLayout(SystemIdOnlyQuery, SystemTitleReferenceName, SystemAreaReferenceName);
            QueryAndLayoutInformation ql2 = TestHelper.CreateQueryAndLayout(SystemTitleOnlyQuery, SystemIdReferenceName, SystemDescriptionReferenceName);
            this.sut.Add(ql1);
            this.sut.FinalQueriesAndLayouts.Count();
            this.sut.Add(ql2);

            // Act
            string[] ans = this.sut.FinalQueriesAndLayouts.Select(qli => qli.Query.QueryText).ToArray();

            // Assert
            Assert.AreEqual<int>(2, ans.Length, "Expecting two queries, not adding to list correctly");
            string[] expectedFields = new string[] { SystemIdReferenceName, SystemTitleReferenceName, SystemDescriptionReferenceName, SystemAreaReferenceName };
            foreach (string field in expectedFields)
            {
                foreach (string query in ans)
                {
                    Assert.IsTrue(query.Contains(field), string.Format(CultureInfo.InvariantCulture, "Not all fields have been merged into all queries. Missing field is {0}, failing query is {1}", field, query));
                }
            }
        }

        /// <summary>
        /// <see cref="QueryAndLayoutManager.Add"/> returns the modified query.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void AddReturnsTheModifiedQuery()
        {
            // Arrange
            QueryAndLayoutInformation ql1 = TestHelper.CreateQueryAndLayout(SystemIdOnlyQuery, SystemTitleReferenceName, SystemAreaReferenceName);
            QueryAndLayoutInformation ql2 = TestHelper.CreateQueryAndLayout(SystemTitleOnlyQuery, SystemIdReferenceName, SystemDescriptionReferenceName);
            this.sut.Add(ql1);

            // Act
            QueryAndLayoutInformation ans = this.sut.Add(ql2);

            // Assert
            Assert.AreSame(this.sut.FinalQueriesAndLayouts.Last().Query, ans.Query, "Add did not return the modified query.");
        }

        /// <summary>
        /// Tests that <see cref="QueryAndLayoutManager.FinalQueriesAndLayouts"/> has the assigned index for first query and layout.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void FirstEntryHasZeroIndex()
        {
            // Arrange
            QueryAndLayoutInformation newItem = TestHelper.ValidQueryDefinitionAndLayout;
            this.sut.Add(newItem);

            // Act
            QueryAndLayoutInformation ans = this.sut.FinalQueriesAndLayouts.First();

            // Assert
            Assert.AreEqual<int>(0, ans.Index, "First entry does not have correct index");
        }

        /// <summary>
        /// Tests that <see cref="QueryAndLayoutManager.FinalQueriesAndLayouts"/> has the assigned index for several queries and layouts.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void MultipleEntriesHaveSequentialIndices()
        {
            // Arrange
            QueryAndLayoutInformation newItem1 = TestHelper.ValidQueryDefinitionAndLayout;
            QueryAndLayoutInformation newItem2 = TestHelper.ValidQueryDefinitionAndLayout;
            this.sut.Add(newItem1);
            this.sut.Add(newItem1);

            // Act
            QueryAndLayoutInformation ans1 = this.sut.FinalQueriesAndLayouts.First();
            QueryAndLayoutInformation ans2 = this.sut.FinalQueriesAndLayouts.Skip(1).First();

            // Assert
            Assert.AreEqual<int>(0, ans1.Index, "First entry does not have correct index");
            Assert.AreEqual<int>(1, ans2.Index, "Second entry does not have correct index");
        }

        /// <summary>
        /// Tests that <see cref="QueryAndLayoutManager.FinalQueriesAndLayouts"/> has the assigned index for several queries and layouts when additions done in separate lots.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void MultipleEntriesHaveSequentialIndicesAfterSecondLotOfAdditions()
        {
            // Arrange
            QueryAndLayoutInformation newItem1 = TestHelper.ValidQueryDefinitionAndLayout;
            QueryAndLayoutInformation newItem2 = TestHelper.ValidQueryDefinitionAndLayout;
            QueryAndLayoutInformation newItem3 = TestHelper.ValidQueryDefinitionAndLayout;
            QueryAndLayoutInformation newItem4 = TestHelper.ValidQueryDefinitionAndLayout;
            this.sut.Add(newItem1);
            this.sut.Add(newItem1);
            this.sut.FinalQueriesAndLayouts.Count();
            this.sut.AddRange(newItem3, newItem4);

            // Act
            QueryAndLayoutInformation ans1 = this.sut.FinalQueriesAndLayouts.First();
            QueryAndLayoutInformation ans2 = this.sut.FinalQueriesAndLayouts.Skip(1).First();
            QueryAndLayoutInformation ans3 = this.sut.FinalQueriesAndLayouts.Skip(2).First();
            QueryAndLayoutInformation ans4 = this.sut.FinalQueriesAndLayouts.Skip(3).First();

            // Assert
            Assert.AreEqual<int>(0, ans1.Index, "First entry does not have correct index");
            Assert.AreEqual<int>(1, ans2.Index, "Second entry does not have correct index");
            Assert.AreEqual<int>(2, ans3.Index, "Third entry does not have correct index");
            Assert.AreEqual<int>(3, ans4.Index, "Fourth entry does not have correct index");
        }

        /// <summary>
        /// <see cref="QueryAndLayoutManager.AllLayoutFields"/> returns a merged list of all fields referenced in all layouts.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void AllLayoutFieldsReturnsListOfAllFieldsReferencedInAllLayouts()
        {
            // Arrange
            QueryAndLayoutInformation ql1 = TestHelper.CreateQueryAndLayout(SystemIdOnlyQuery, SystemIdReferenceName, SystemTitleReferenceName, SystemAreaReferenceName);
            QueryAndLayoutInformation ql2 = TestHelper.CreateQueryAndLayout(SystemTitleOnlyQuery, SystemIdReferenceName, SystemDescriptionReferenceName);
            this.sut.Add(ql1);
            this.sut.Add(ql2);

            // Act
            string[] ans = this.sut.AllLayoutFields.ToArray();

            // Assert
            Assert.IsTrue(TestHelper.CheckUnorderedUniqueArrayMatch<string>(new string[] { SystemIdReferenceName, SystemTitleReferenceName, SystemAreaReferenceName, SystemDescriptionReferenceName }, ans), "Did not merge all layout fields");
        }
    }
}
