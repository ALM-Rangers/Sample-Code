//---------------------------------------------------------------------
// <copyright file="WorkItemTreeTests.cs" company="Microsoft">
//    Copyright (c) Microsoft. All rights reserved.
//    This code is licensed under the MIT License.
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF 
//    ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
//    TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
//    PARTICULAR PURPOSE AND NONINFRINGEMENT.
// </copyright>
// <summary>The WorkItemTreeTests type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.Word4Tfs.Library.Test.UnitTest.Model
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Word4Tfs.Library.Model;

    /// <summary>
    /// Tests the <see cref="WorkItemTree"/> class.
    /// </summary>
    [TestClass]
    public class WorkItemTreeTests
    {
        /// <summary>
        /// Tests that the depth-first iterator iterates the tree depth first.
        /// </summary>
        [TestMethod]
        public void DepthFirstIterationTest()
        {
            // Arrange
            WorkItemTreeNode node0 = new WorkItemTreeNode(null, 0);
            WorkItemTreeNode node0_0 = new WorkItemTreeNode(null, 1);
            WorkItemTreeNode node0_0_0 = new WorkItemTreeNode(null, 2);
            WorkItemTreeNode node0_0_1 = new WorkItemTreeNode(null, 2);
            WorkItemTreeNode node1 = new WorkItemTreeNode(null, 0);
            WorkItemTreeNode node1_0 = new WorkItemTreeNode(null, 0);

            node0.Children.Add(node0_0);
            node0_0.Children.Add(node0_0_0);
            node0_0.Children.Add(node0_0_1);
            node1.Children.Add(node1_0);

            WorkItemTree sut = new WorkItemTree();
            sut.RootNodes.Add(node0);
            sut.RootNodes.Add(node1);

            // Act
            IEnumerator<WorkItemTreeNode> e = sut.DepthFirstNodes().GetEnumerator();

            // Assert
            Assert.IsTrue(e.MoveNext());
            Assert.AreSame(node0, e.Current);
            Assert.IsTrue(e.MoveNext());
            Assert.AreSame(node0_0, e.Current);
            Assert.IsTrue(e.MoveNext());
            Assert.AreSame(node0_0_0, e.Current);
            Assert.IsTrue(e.MoveNext());
            Assert.AreSame(node0_0_1, e.Current);
            Assert.IsTrue(e.MoveNext());
            Assert.AreSame(node1, e.Current);
            Assert.IsTrue(e.MoveNext());
            Assert.AreSame(node1_0, e.Current);
        }
    }
}
