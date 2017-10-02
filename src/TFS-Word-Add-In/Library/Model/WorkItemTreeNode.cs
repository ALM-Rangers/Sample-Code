//---------------------------------------------------------------------
// <copyright file="WorkItemTreeNode.cs" company="Microsoft">
//    Copyright © ALM | DevOps Ranger Contributors. All rights reserved.
//    This code is licensed under the MIT License.
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF 
//    ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
//    TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
//    PARTICULAR PURPOSE AND NONINFRINGEMENT.
// </copyright>
// <summary>The WorkItemTreeNode type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.Word4Tfs.Library.Model
{
    using System.Collections.Generic;
    using Microsoft.Word4Tfs.Library.Model.TeamFoundation;

    /// <summary>
    /// Represents a node in a tree of work items.
    /// </summary>
    public class WorkItemTreeNode
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WorkItemTreeNode"/> class.
        /// </summary>
        /// <param name="workItem">The work item represented at this node.</param>
        /// <param name="level">The level in the tree, 0 is the highest level in the tree</param>
        public WorkItemTreeNode(ITfsWorkItem workItem, int level)
        {
            this.WorkItem = workItem;
            this.Level = level;
            this.Children = new List<WorkItemTreeNode>();
        }

        /// <summary>
        /// Gets the level in the tree. 0 is the highest level in the tree
        /// </summary>
        public int Level { get; private set; }

        /// <summary>
        /// Gets the work item at this node.
        /// </summary>
        public ITfsWorkItem WorkItem { get; private set; }

        /// <summary>
        /// Gets the children of the current node.
        /// </summary>
        public IList<WorkItemTreeNode> Children { get; private set; }

        /// <summary>
        /// Enumerates the child nodes depth first.
        /// </summary>
        /// <returns>Enumerable that returns an enumerator.</returns>
        public IEnumerable<WorkItemTreeNode> DepthFirstNodes()
        {
            foreach (WorkItemTreeNode node in this.Children)
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
