//---------------------------------------------------------------------
// <copyright file="TfsWorkItemStore.cs" company="Microsoft">
//    Copyright (c) Microsoft. All rights reserved.
//    This code is licensed under the MIT License.
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF 
//    ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
//    TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
//    PARTICULAR PURPOSE AND NONINFRINGEMENT.
// </copyright>
// <summary>The TfsWorkItemStore type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.Word4Tfs.Library.Model.TeamFoundation
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Microsoft.TeamFoundation.WorkItemTracking.Client;

    /// <summary>
    /// Provides access to the TFS <see cref="WorkItemStore"/> class.
    /// </summary>
    public class TfsWorkItemStore : ITfsWorkItemStore
    {
        /// <summary>
        /// The TFS work item store that this object wraps.
        /// </summary>
        private WorkItemStore workItemStore;

        /// <summary>
        /// Initializes a new instance of the <see cref="TfsWorkItemStore"/> class.
        /// </summary>
        /// <param name="workItemStore">The TFS work item store that this object wraps.</param>
        public TfsWorkItemStore(WorkItemStore workItemStore)
        {
            this.workItemStore = workItemStore;
        }

        /// <summary>
        /// Gets the set of fields that is referred to in wiql for the work items that are specified by an ID number (<paramref name="ids"/>)..
        /// </summary>
        /// <param name="ids">A collection of work item IDs.</param>
        /// <param name="wiql">The definition of fields to return.</param>
        /// <returns>A list of work items that result from the query.</returns>
        public IList<ITfsWorkItem> Query(int[] ids, string wiql)
        {
            IList<ITfsWorkItem> ans = new List<ITfsWorkItem>();
            foreach (WorkItem wi in this.workItemStore.Query(ids, wiql))
            {
                ans.Add(new TfsWorkItem(wi));
            }

            return ans;
        }
    }
}
