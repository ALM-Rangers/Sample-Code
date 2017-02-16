//---------------------------------------------------------------------
// <copyright file="WorkItemTree.cs" company="Microsoft">
//    Copyright (c) Microsoft. All rights reserved.
//    This code is licensed under the MIT License.
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF 
//    ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
//    TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
//    PARTICULAR PURPOSE AND NONINFRINGEMENT.
// </copyright>
// <summary>The WorkItemTree type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.Word4Tfs.Library.Model
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Represents a tree of work items.
    /// </summary>
    public class WorkItemTree
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WorkItemTree"/> class.
        /// </summary>
        public WorkItemTree()
        {
            this.RootNodes = new List<WorkItemTreeNode>();
        }

        /// <summary>
        /// Gets the list of root nodes in the tree.
        /// </summary>
        public IList<WorkItemTreeNode> RootNodes { get; private set; }

        /// <summary>
        /// Enumerates the nodes depth first.
        /// </summary>
        /// <returns>A depth-first enumerator for nodes.</returns>
        public IEnumerable<WorkItemTreeNode> DepthFirstNodes()
        {
            foreach (WorkItemTreeNode node in this.RootNodes)
            {
                yield return node;
                foreach (WorkItemTreeNode subNode in node.DepthFirstNodes())
                {
                    yield return subNode;
                }
            }
        }
    }
}
