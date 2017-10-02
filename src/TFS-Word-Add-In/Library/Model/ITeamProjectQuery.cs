//---------------------------------------------------------------------
// <copyright file="ITeamProjectQuery.cs" company="Microsoft">
//    Copyright © ALM | DevOps Ranger Contributors. All rights reserved.
//    This code is licensed under the MIT License.
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF 
//    ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
//    TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
//    PARTICULAR PURPOSE AND NONINFRINGEMENT.
// </copyright>
// <summary>The ITeamProjectQuery type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.Word4Tfs.Library.Model
{
    using System.Threading;
    using Microsoft.TeamFoundation.WorkItemTracking.Client;

    /// <summary>
    /// Defines the operation to be performed on team project queries.
    /// </summary>
    public interface ITeamProjectQuery
    {
        /// <summary>
        /// Executes the query to get the work items.
        /// </summary>
        /// <param name="queryDefinition">The definition of the query to execute</param>
        /// <param name="cancellationToken">Used to cancel the operation.</param>
        /// <returns>The tree of work items.</returns>
        WorkItemTree QueryForWorkItems(QueryDefinition queryDefinition, CancellationToken cancellationToken);
    }
}
