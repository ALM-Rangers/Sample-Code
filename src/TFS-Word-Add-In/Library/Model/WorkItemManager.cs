//---------------------------------------------------------------------
// <copyright file="WorkItemManager.cs" company="Microsoft">
//    Copyright (c) Microsoft. All rights reserved.
//    This code is licensed under the MIT License.
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF 
//    ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
//    TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
//    PARTICULAR PURPOSE AND NONINFRINGEMENT.
// </copyright>
// <summary>The WorkItemManager type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.Word4Tfs.Library.Model
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Word4Tfs.Library.Model.TeamFoundation;

    // TODO: consider making work item manager a proper collection.

    /// <summary>
    /// Manages work items.
    /// </summary>
    public class WorkItemManager
    {
        /// <summary>
        /// The internal dictionary of work items being managed, accessed by work item id.
        /// </summary>
        private Dictionary<int, ITfsWorkItem> workItemList;

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkItemManager"/> class.
        /// </summary>
        public WorkItemManager()
        {
            this.workItemList = new Dictionary<int, ITfsWorkItem>();
        }

        /// <summary>
        /// Gets the list of work items currently stored by the manager.
        /// </summary>
        public IEnumerable<ITfsWorkItem> WorkItems
        {
            get
            {
                return this.workItemList.Values;
            }
        }

        /// <summary>
        /// Gets the work item with the given id.
        /// </summary>
        /// <param name="id">The id of the work item to retrieve.</param>
        /// <returns>The work item with the given id.</returns>
        public ITfsWorkItem this[int id]
        {
            get
            {
                ITfsWorkItem ans = null;
                this.workItemList.TryGetValue(id, out ans);
                return ans;
            }
        }

        /// <summary>
        /// Adds a new work item to the manager.
        /// </summary>
        /// <remarks>
        /// Existing work items with the same work item id are replaced.
        /// </remarks>
        /// <param name="workItem">The work item to be added.</param>
        public void Add(ITfsWorkItem workItem)
        {
            if (workItem == null)
            {
                throw new ArgumentNullException("workItem");
            }

            ITfsWorkItem existingItem = null;
            this.workItemList.TryGetValue(workItem.Id, out existingItem);
            if (existingItem != null)
            {
                this.workItemList.Remove(workItem.Id);
            }

            this.workItemList.Add(workItem.Id, workItem);
        }

        /// <summary>
        /// Adds a set of new work item to the manager.
        /// </summary>
        /// <remarks>
        /// Existing work items with the same work item id are replaced.
        /// </remarks>
        /// <param name="workItems">The work items to be added.</param>
        public void AddRange(params ITfsWorkItem[] workItems)
        {
            if (workItems == null)
            {
                throw new ArgumentNullException("workItems");
            }

            foreach (ITfsWorkItem wi in workItems)
            {
                this.Add(wi);
            }
        }

        /// <summary>
        /// Removes all existing work items.
        /// </summary>
        public void Clear()
        {
            this.workItemList.Clear();
        }
    }
}
