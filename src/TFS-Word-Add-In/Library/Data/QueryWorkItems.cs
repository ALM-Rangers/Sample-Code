//---------------------------------------------------------------------
// <copyright file="QueryWorkItems.cs" company="Microsoft">
//    Copyright © ALM | DevOps Ranger Contributors. All rights reserved.
//    This code is licensed under the MIT License.
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF 
//    ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
//    TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
//    PARTICULAR PURPOSE AND NONINFRINGEMENT.
// </copyright>
// <summary>The QueryWorkItems type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.Word4Tfs.Library.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.Word4Tfs.Library.Model;

    /// <summary>
    /// Records the sequence of work items returned by a query.
    /// </summary>
    public class QueryWorkItems
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QueryWorkItems"/> class.
        /// </summary>
        /// <param name="queryIndex">The index of the query.</param>
        /// <param name="workItemIds">The ids of the work items returned by the query.</param>
        public QueryWorkItems(int queryIndex, int[] workItemIds)
        {
            this.QueryIndex = queryIndex;
            this.WorkItemIds = workItemIds;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryWorkItems"/> class.
        /// </summary>
        /// <param name="queryIndex">The index of the query.</param>
        /// <param name="workItemTreeNodes">The work item tree nodes from which to construct the list of work item ids returned by the query.</param>
        public QueryWorkItems(int queryIndex, WorkItemTreeNode[] workItemTreeNodes)
        {
            this.QueryIndex = queryIndex;
            this.WorkItemIds = workItemTreeNodes.Select(node => node.WorkItem.Id);
            this.WorkItemTreeNodes = workItemTreeNodes;
        }

        /// <summary>
        /// Gets the index of the query.
        /// </summary>
        public int QueryIndex { get; private set; }

        /// <summary>
        /// Gets the ids of the work items returned by the query in the sequence they were returned.
        /// </summary>
        public IEnumerable<int> WorkItemIds { get; private set; }

        /// <summary>
        /// Gets the work item tree nodes of the work items returned by the query in the sequence they were returned. Only valid if the constructor that uses tree nodes is used.
        /// </summary>
        public IEnumerable<WorkItemTreeNode> WorkItemTreeNodes { get; private set; }
    }
}
