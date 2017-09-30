//---------------------------------------------------------------------
// <copyright file="TfsQueryFactory.cs" company="Microsoft">
//    Copyright © ALM | DevOps Ranger Contributors. All rights reserved.
//    This code is licensed under the MIT License.
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF 
//    ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
//    TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
//    PARTICULAR PURPOSE AND NONINFRINGEMENT.
// </copyright>
// <summary>The TfsQueryFactory type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.Word4Tfs.Library.Model.TeamFoundation
{
    using System.Collections;
    using Microsoft.TeamFoundation.WorkItemTracking.Client;

    /// <summary>
    /// Factory to create <see cref="ITfsQuery"/> objects.
    /// </summary>
    public class TfsQueryFactory : ITfsQueryFactory
    {
        /// <summary>
        /// The work item store to which the created queries are tied.
        /// </summary>
        private WorkItemStore workItemStore;

        /// <summary>
        /// Initializes a new instance of the <see cref="TfsQueryFactory"/> class.
        /// </summary>
        /// <param name="workItemStore">The work item store to which the created queries are tied.</param>
        public TfsQueryFactory(WorkItemStore workItemStore)
        {
            this.workItemStore = workItemStore;
        }
        
        /// <summary>
        /// Creates a new instance of the <see cref="TfsQuery"/> class.
        /// </summary>
        /// <param name="wiql">The query string to execute.</param>
        /// <returns>The new query.</returns>
        public ITfsQuery CreateTfsQuery(string wiql)
        {
            return new TfsQuery(this.workItemStore, wiql);
        }

        /// <summary>
        /// Creates a new instance of the <see cref="TfsQuery"/> class.
        /// </summary>
        /// <param name="wiql">The query string to execute.</param>
        /// <param name="context">A dictionary of macros and values.</param>
        /// <returns>The new query.</returns>
        public ITfsQuery CreateTfsQuery(string wiql, IDictionary context)
        {
            return new TfsQuery(this.workItemStore, wiql, context);
        }

        /// <summary>
        /// Creates a new instance of the <see cref="TfsQuery"/> class.
        /// </summary>
        /// <param name="wiql">The query string to execute.</param>
        /// <param name="ids">An array of <see cref="WorkItem"/> IDs.</param>
        /// <returns>The new query.</returns>
        public ITfsQuery CreateTfsQuery(string wiql, int[] ids)
        {
            return new TfsQuery(this.workItemStore, wiql, ids);
        }
    }
}
