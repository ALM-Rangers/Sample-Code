//---------------------------------------------------------------------
// <copyright file="QueryUtilitiesTests.cs" company="Microsoft">
//    Copyright (c) Microsoft. All rights reserved.
//    This code is licensed under the MIT License.
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF 
//    ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
//    TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
//    PARTICULAR PURPOSE AND NONINFRINGEMENT.
// </copyright>
// <summary>The QueryUtilitiesTests type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.Word4Tfs.Library.Test.UnitTest.Model
{
    using System.Collections.Generic;
    using Microsoft.TeamFoundation.WorkItemTracking.Client;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Word4Tfs.Library.Model;
    using Microsoft.Word4Tfs.Library.Model.TeamFoundation;
    using Moq;

    /// <summary>
    /// Tests the <see cref="QueryUtilities"/> class.
    /// </summary>
    [TestClass]
    public class QueryUtilitiesTests
    {
        /// <summary>
        /// Tests that ConvertTreeQueryToQueryForItem tests argument for null.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void ConvertTreeQueryToQueryForItemTestsArgumentForNull()
        {
            TestHelper.TestForArgumentNullException(() => QueryUtilities.ConvertTreeQueryToQueryForItem(null), "query");
        }

        /// <summary>
        /// Tests that ConvertTreeQueryToQueryForItem constructs a query string from the WorkItems entity
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void ConvertTreeQueryToQueryForItemConstructsAQueryStringFromWorkItems()
        {
            // Arrange
            // select [System.Id] from WorkItemLinks
            Mock<ITfsQuery> treeQuery = new Mock<ITfsQuery>();
            treeQuery.Setup(q => q.DisplayFieldList).Returns(TestHelper.CreateMockFieldDefinitions("[System.Id]"));

            // Act
            string convertedQueryString = QueryUtilities.ConvertTreeQueryToQueryForItem(treeQuery.Object);

            // Assert
            Assert.AreEqual<string>("SELECT [System.Id] FROM WorkItems", convertedQueryString);
        }

        /// <summary>
        /// Tests that ConvertTreeQueryToQueryForItem handles more than one display field.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void ConvertTreeQueryToQueryHandlesMoreThanOneDisplayField()
        {
            // Arrange
            Mock<ITfsQuery> treeQuery = new Mock<ITfsQuery>();
            treeQuery.Setup(q => q.DisplayFieldList).Returns(TestHelper.CreateMockFieldDefinitions("[System.Id]", "[System.Title]"));

            // Act
            string convertedQueryString = QueryUtilities.ConvertTreeQueryToQueryForItem(treeQuery.Object);

            // Assert
            Assert.AreEqual<string>("SELECT [System.Id], [System.Title] FROM WorkItems", convertedQueryString, "Multiple fields not being handled correctly");
        }
    }
}
