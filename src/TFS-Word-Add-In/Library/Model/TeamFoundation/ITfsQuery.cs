//---------------------------------------------------------------------
// <copyright file="ITfsQuery.cs" company="Microsoft">
//    Copyright (c) Microsoft. All rights reserved.
//    This code is licensed under the MIT License.
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF 
//    ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
//    TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
//    PARTICULAR PURPOSE AND NONINFRINGEMENT.
// </copyright>
// <summary>The ITfsQuery type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.Word4Tfs.Library.Model.TeamFoundation
{
    using System.Collections.Generic;
    using System.Threading;
    using Microsoft.TeamFoundation.WorkItemTracking.Client;

    /// <summary>
    /// Defines the operations to be performed on a TFS query
    /// </summary>
    /// <remarks>
    /// This contains a subset of the operations of a <see cref="Query"/> so that queries can be mocked.
    /// </remarks>
    public interface ITfsQuery
    {
        /// <summary>
        /// Gets the <see cref="ITfsWorkItemStore"/> that is being queried.
        /// </summary>
        ITfsWorkItemStore WorkItemStore { get; }

        /// <summary>
        /// Gets the list of <see cref="Fields"/> that will be paged from the server when this query executes.
        /// </summary>
        IList<ITfsFieldDefinition> DisplayFieldList { get; }

        /// <summary>
        /// Executes a query that gets an array of <see cref="WorkItemLinkInfo"/> objects.
        /// </summary>
        /// <param name="cancellationToken">Used to cancel the operation.</param>
        /// <returns>The array of <see cref="WorkItemLinkInfo"/> objects that satisfied the query.</returns>
        WorkItemLinkInfo[] RunLinkQuery(CancellationToken cancellationToken);

        /// <summary>
        /// Executes a query that gets a <see cref="WorkItemCollection"/> that contains <see cref="WorkItem"/> objects that satisfy the query.
        /// </summary>
        /// <param name="cancellationToken">Used to cancel the operation.</param>
        /// <returns>A list that contains <see cref="ITfsWorkItem"/> objects that satisfy the query.</returns>
        IList<ITfsWorkItem> RunQuery(CancellationToken cancellationToken);
    }
}
