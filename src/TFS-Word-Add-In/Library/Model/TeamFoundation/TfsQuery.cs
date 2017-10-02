//---------------------------------------------------------------------
// <copyright file="TfsQuery.cs" company="Microsoft">
//    Copyright © ALM | DevOps Ranger Contributors. All rights reserved.
//    This code is licensed under the MIT License.
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF 
//    ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
//    TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
//    PARTICULAR PURPOSE AND NONINFRINGEMENT.
// </copyright>
// <summary>The TfsQuery type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.Word4Tfs.Library.Model.TeamFoundation
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Threading;
    using Microsoft.TeamFoundation.Client;
    using Microsoft.TeamFoundation.WorkItemTracking.Client;

    /// <summary>
    /// Provides access to the TFS <see cref="Query"/> class.
    /// </summary>
    public class TfsQuery : ITfsQuery
    {
        /// <summary>
        /// The TFS query that this object wraps.
        /// </summary>
        private Query query;

        /// <summary>
        /// Initializes a new instance of the <see cref="TfsQuery"/> class.
        /// </summary>
        /// <param name="store">The <see cref="WorkItemStore"/> to query.</param>
        /// <param name="wiql">The query string to execute.</param>
        public TfsQuery(WorkItemStore store, string wiql)
        {
            this.query = new Query(store, wiql);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TfsQuery"/> class.
        /// </summary>
        /// <param name="store">The <see cref="WorkItemStore"/> to query.</param>
        /// <param name="wiql">The query string to execute.</param>
        /// <param name="context">A dictionary of macros and values.</param>
        public TfsQuery(WorkItemStore store, string wiql, IDictionary context)
        {
            this.query = new Query(store, wiql, context);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TfsQuery"/> class.
        /// </summary>
        /// <param name="store">The <see cref="WorkItemStore"/> to query.</param>
        /// <param name="wiql">The query string to execute.</param>
        /// <param name="ids">An array of <see cref="WorkItem"/> IDs.</param>
        public TfsQuery(WorkItemStore store, string wiql, int[] ids)
        {
            this.query = new Query(store, wiql, ids);
        }

        #region ITfsQuery Members

        /// <summary>
        /// Gets the <see cref="ITfsWorkItemStore"/> that is being queried.
        /// </summary>
        public ITfsWorkItemStore WorkItemStore
        {
            get
            {
                return new TfsWorkItemStore(this.query.WorkItemStore);
            }
        }

        /// <summary>
        /// Gets the list of <see cref="Fields"/> that will be paged from the server when this query executes.
        /// </summary>
        public IList<ITfsFieldDefinition> DisplayFieldList
        {
            get
            {
                List<ITfsFieldDefinition> ans = new List<ITfsFieldDefinition>();
                foreach (FieldDefinition fd in this.query.DisplayFieldList)
                {
                    ans.Add(new TfsFieldDefinition(fd));
                }

                return ans;
            }
        }

        /// <summary>
        /// Executes a query that gets an array of <see cref="WorkItemLinkInfo"/> objects.
        /// </summary>
        /// <param name="cancellationToken">Used to cancel the operation.</param>
        /// <returns>The array of <see cref="WorkItemLinkInfo"/> objects that satisfied the query.</returns>
        public WorkItemLinkInfo[] RunLinkQuery(CancellationToken cancellationToken)
        {
            WorkItemLinkInfo[] ans = null;
            ICancelableAsyncResult result = this.query.BeginLinkQuery();
            try
            {
                cancellationToken.Register(s => ((ICancelableAsyncResult)s).Cancel(), result);
                result.AsyncWaitHandle.WaitOne();
                cancellationToken.ThrowIfCancellationRequested();
            }
            finally
            {
                ans = this.query.EndLinkQuery(result);
            }

            return ans;
         }

        /// <summary>
        /// Executes a query that gets a <see cref="WorkItemCollection"/> that contains <see cref="WorkItem"/> objects that satisfy the query.
        /// </summary>
        /// <param name="cancellationToken">Used to cancel the operation.</param>
        /// <returns>A list that contains <see cref="ITfsWorkItem"/> objects that satisfy the query.</returns>
        public IList<ITfsWorkItem> RunQuery(CancellationToken cancellationToken)
        {
            WorkItemCollection workItems = null;
            ICancelableAsyncResult result = this.query.BeginQuery();
            try
            {
                cancellationToken.Register(s => ((ICancelableAsyncResult)s).Cancel(), result);
                result.AsyncWaitHandle.WaitOne();
                cancellationToken.ThrowIfCancellationRequested();
            }
            finally
            {
                workItems = this.query.EndQuery(result);
            }

            IList<ITfsWorkItem> ans = new List<ITfsWorkItem>();
            foreach (WorkItem wi in workItems)
            {
                ans.Add(new TfsWorkItem(wi));
            }

            return ans;
        }

        #endregion
    }
}
