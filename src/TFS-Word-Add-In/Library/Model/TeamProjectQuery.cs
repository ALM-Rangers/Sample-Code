//---------------------------------------------------------------------
// <copyright file="TeamProjectQuery.cs" company="Microsoft">
//    Copyright © ALM | DevOps Ranger Contributors. All rights reserved.
//    This code is licensed under the MIT License.
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF 
//    ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
//    TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
//    PARTICULAR PURPOSE AND NONINFRINGEMENT.
// </copyright>
// <summary>The TeamProjectQuery type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.Word4Tfs.Library.Model
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using Microsoft.TeamFoundation.WorkItemTracking.Client;
    using Microsoft.Word4Tfs.Library.Model.TeamFoundation;

    /// <summary>
    /// Provides facilities for working with team project queries.
    /// </summary>
    public class TeamProjectQuery : ITeamProjectQuery
    {
        /// <summary>
        /// The project name.
        /// </summary>
        private string projectName;

        /// <summary>
        /// The query factory to use to create the queries.
        /// </summary>
        private ITfsQueryFactory queryFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="TeamProjectQuery"/> class.
        /// </summary>
        /// <param name="projectName">The name of the project in the team project collection that this instance will represent.</param>
        /// <param name="queryFactory">The factory used to create <see cref="ITfsQuery"/> objects.</param>
        public TeamProjectQuery(string projectName, ITfsQueryFactory queryFactory)
        {
            this.projectName = projectName;
            this.queryFactory = queryFactory;
        }

        /// <summary>
        /// Executes the query to get the work items.
        /// </summary>
        /// <param name="queryDefinition">The definition of the query to execute</param>
        /// <param name="cancellationToken">Used to cancel the operation.</param>
        /// <returns>The tree of work items.</returns>
        public WorkItemTree QueryForWorkItems(QueryDefinition queryDefinition, CancellationToken cancellationToken)
        {
            WorkItemTree ans;

            if (queryDefinition == null)
            {
                throw new ArgumentNullException("queryDefinition");
            }

            Dictionary<string, string> variables = new Dictionary<string, string>();
            variables.Add("project", this.projectName);
            ITfsQuery query = this.queryFactory.CreateTfsQuery(queryDefinition.QueryText, variables);

            if (queryDefinition.QueryType == QueryType.List)
            {
                ans = RunFlatQuery(query, cancellationToken);
            }
            else if (queryDefinition.QueryType == QueryType.OneHop)
            {
                ans = RunOneHopQuery(query, cancellationToken);
            }
            else
            {
                ans = RunTreeQuery(query, cancellationToken);
            }

            return ans;
        }

        /// <summary>
        /// Runs a flat query.
        /// </summary>
        /// <param name="query">The query to execute.</param>
        /// <param name="cancellationToken">Used to cancel the operation.</param>
        /// <returns>A tree of work items, the root nodes will not have any children.</returns>
        private static WorkItemTree RunFlatQuery(ITfsQuery query, CancellationToken cancellationToken)
        {
            WorkItemTree ans = new WorkItemTree();
            foreach (ITfsWorkItem wi in query.RunQuery(cancellationToken))
            {
                ans.RootNodes.Add(new WorkItemTreeNode(wi, 0));
            }

            return ans;
        }

        /// <summary>
        /// Runs a one-hop query.
        /// </summary>
        /// <param name="query">The query to execute.</param>
        /// <param name="cancellationToken">Used to cancel the operation.</param>
        /// <returns>A tree of work items.</returns>
        private static WorkItemTree RunOneHopQuery(ITfsQuery query, CancellationToken cancellationToken)
        {
            WorkItemTree ans = new WorkItemTree();
            WorkItemLinkInfo[] wilis = query.RunLinkQuery(cancellationToken);
            int[] ids = new int[wilis.Length];
            for (int i = 0; i < ids.Length; i++)
            {
                ids[i] = wilis[i].TargetId;
            }

            string workItemQueryString = QueryUtilities.ConvertTreeQueryToQueryForItem(query);
            IList<ITfsWorkItem> workItems = query.WorkItemStore.Query(ids.Distinct().ToArray(), workItemQueryString);

            WorkItemTreeNode parent = null;
            for (int i = 0; i < wilis.Length; i++)
            {
                WorkItemLinkInfo wili = wilis[i];
                ITfsWorkItem wi = workItems.Select(item => item).Where(item => item.Id == wili.TargetId).Single();
                if (wili.SourceId <= 0)
                {
                    parent = new WorkItemTreeNode(wi, 0);
                    ans.RootNodes.Add(parent);
                }
                else
                {
                    if (parent == null || parent.WorkItem.Id != wili.SourceId)
                    {
                        throw new InvalidOperationException(ModelResources.QueryNotWellFormedOneHop);
                    }

                    parent.Children.Add(new WorkItemTreeNode(wi, parent.Level + 1));
                }
            }

            return ans;
        }

        /// <summary>
        /// Runs a tree query.
        /// </summary>
        /// <param name="query">The query to execute.</param>
        /// <param name="cancellationToken">Used to cancel the operation.</param>
        /// <returns>A tree of work items.</returns>
        private static WorkItemTree RunTreeQuery(ITfsQuery query, CancellationToken cancellationToken)
        {
            WorkItemTree ans = new WorkItemTree();
            WorkItemLinkInfo[] wilis = query.RunLinkQuery(cancellationToken);
            int[] ids = new int[wilis.Length];
            for (int i = 0; i < ids.Length; i++)
            {
                ids[i] = wilis[i].TargetId;
            }

            string workItemQueryString = QueryUtilities.ConvertTreeQueryToQueryForItem(query);
            IList<ITfsWorkItem> workItems = query.WorkItemStore.Query(ids.Distinct().ToArray(), workItemQueryString);

            for (int i = 0; i < wilis.Length; i++)
            {
                WorkItemLinkInfo wili = wilis[i];
                ITfsWorkItem wi = workItems.Select(item => item).Where(item => item.Id == wili.TargetId).Single();
                if (wili.SourceId <= 0)
                {
                    ans.RootNodes.Add(new WorkItemTreeNode(wi, 0));
                }
                else
                {
                    WorkItemTreeNode parent = null;
                    try
                    {
                        parent = ans.DepthFirstNodes().Where(node => node.WorkItem.Id == wili.SourceId).Single();
                    }
                    catch (InvalidOperationException)
                    {
                        throw new InvalidOperationException(ModelResources.QueryNotWellFormedTree);
                    }

                    parent.Children.Add(new WorkItemTreeNode(wi, parent.Level + 1));
                }
            }

            return ans;
        }
    }
}
