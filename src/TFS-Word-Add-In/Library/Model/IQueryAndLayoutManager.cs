//---------------------------------------------------------------------
// <copyright file="IQueryAndLayoutManager.cs" company="Microsoft">
//    Copyright © ALM | DevOps Ranger Contributors. All rights reserved.
//    This code is licensed under the MIT License.
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF 
//    ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
//    TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
//    PARTICULAR PURPOSE AND NONINFRINGEMENT.
// </copyright>
// <summary>The IQueryAndLayoutManager type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.Word4Tfs.Library.Model
{
    using System.Collections.Generic;
    using Microsoft.TeamFoundation.WorkItemTracking.Client;
    using Microsoft.Word4Tfs.Library.Data;
    using Microsoft.Word4Tfs.Library.Model.TeamFoundation;

    /// <summary>
    /// Defines the operations required to manage queries and layouts.
    /// </summary>
    public interface IQueryAndLayoutManager
    {
        /// <summary>
        /// Gets a merged list of all the fields in all the layouts
        /// </summary>
        IEnumerable<string> AllLayoutFields { get; }

        /// <summary>
        /// Gets the current list of queries and layouts as originally chosen.
        /// </summary>
        IEnumerable<QueryAndLayoutInformation> OriginalQueriesAndLayouts { get; }

        /// <summary>
        /// Gets the final query definitions and the layouts.
        /// </summary>
        /// <remarks>
        /// The final queries include all the fields from all the layouts and exclude any fields not defined in the team project.
        /// </remarks>
        /// <returns>The query definitions for the final queries to be executed.</returns>
        IEnumerable<QueryAndLayoutInformation> FinalQueriesAndLayouts { get; }

        /// <summary>
        /// Adds a new query and layout to the manager.
        /// </summary>
        /// <param name="queryAndLayout">The query and layout to be added.</param>
        /// <returns>The query and layout with the query modified to include all the fields from all the layouts and exclude any fields not defined in the team project.</returns>
        QueryAndLayoutInformation Add(QueryAndLayoutInformation queryAndLayout);

        /// <summary>
        /// Adds new queries and layouts to the manager.
        /// </summary>
        /// <param name="queriesAndLayouts">The queries and layouts to be added.</param>
        void AddRange(params QueryAndLayoutInformation[] queriesAndLayouts);
    }
}