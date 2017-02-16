//---------------------------------------------------------------------
// <copyright file="TfsQueryTests.cs" company="Microsoft">
//    Copyright (c) Microsoft. All rights reserved.
//    This code is licensed under the MIT License.
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF 
//    ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
//    TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
//    PARTICULAR PURPOSE AND NONINFRINGEMENT.
// </copyright>
// <summary>The TfsQueryTests type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.Word4Tfs.Library.Test.IntegrationTest
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.TeamFoundation.Client;
    using Microsoft.TeamFoundation.WorkItemTracking.Client;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Word4Tfs.Library.Model.TeamFoundation;

    /// <summary>
    /// Integration tests for the <see cref="TfsQuery"/> class.
    /// </summary>
    [TestClass]
    public class TfsQueryTests
    {
        /// <summary>
        /// The <see cref="TfsQueryFactory"/> to be tested.
        /// </summary>
        private ITfsQueryFactory factory;

        /// <summary>
        /// The dictionary of variables to use in the queries executed.
        /// </summary>
        private Dictionary<string, string> variables;

        /// <summary>
        /// Initializes each test.
        /// </summary>
        [TestInitialize]
        public void InitializeTest()
        {
            TfsTeamProjectCollection collection = IntegrationTestHelper.Tpc;
            this.factory = new TfsQueryFactory(new WorkItemStore(collection));
            this.variables = new Dictionary<string, string>();
            this.variables.Add("project", TestHelper.ProjectName);
        }

        /// <summary>
        /// Tests that can cancel a query.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryIntegration)]
        public void CanCancelQuery()
        {
            // Arrange
            ITfsQuery sut = this.factory.CreateTfsQuery(TestHelper.FlatQuery, this.variables);
            CancellationTokenSource cts = new CancellationTokenSource();

            // Act
            Task t = Task.Factory.StartNew(() => sut.RunQuery(cts.Token));
            cts.Cancel();

            // Assert
            TestHelper.AssertTaskCancelled(t);
        }

        /// <summary>
        /// Tests that can cancel a link query.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryIntegration)]
        public void CanCancelLinkQuery()
        {
            // Arrange
            ITfsQuery sut = this.factory.CreateTfsQuery(TestHelper.TreeQuery, this.variables);
            CancellationTokenSource cts = new CancellationTokenSource();

            // Act
            Task t = Task.Factory.StartNew(() => sut.RunLinkQuery(cts.Token));
            cts.Cancel();

            // Assert
            TestHelper.AssertTaskCancelled(t);
        }
    }
}
