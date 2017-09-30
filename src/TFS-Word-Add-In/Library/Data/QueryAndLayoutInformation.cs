//---------------------------------------------------------------------
// <copyright file="QueryAndLayoutInformation.cs" company="Microsoft">
//    Copyright © ALM | DevOps Ranger Contributors. All rights reserved.
//    This code is licensed under the MIT License.
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF 
//    ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
//    TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
//    PARTICULAR PURPOSE AND NONINFRINGEMENT.
// </copyright>
// <summary>The QueryAndLayoutInformation type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.Word4Tfs.Library.Data
{
    using System;
    using Microsoft.TeamFoundation.WorkItemTracking.Client;

    /// <summary>
    /// Information about a query and its related layout.
    /// </summary>
    public class QueryAndLayoutInformation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QueryAndLayoutInformation"/> class.
        /// </summary>
        /// <param name="query">The definition of the query.</param>
        /// <param name="layout">The associated layout.</param>
        public QueryAndLayoutInformation(QueryDefinition query, LayoutInformation layout) : this(query, layout, -1)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryAndLayoutInformation"/> class.
        /// </summary>
        /// <remarks>
        /// This constructor should only be used by the <see cref="QueryAndLayoutManager"/>
        /// </remarks>
        /// <param name="query">The definition of the query.</param>
        /// <param name="layout">The associated layout.</param>
        /// <param name="index">The position of this query and layout in the list of queries and layouts.</param>
        public QueryAndLayoutInformation(QueryDefinition query, LayoutInformation layout, int index)
        {
            if (query == null)
            {
                throw new ArgumentNullException("query");
            }

            if (layout == null)
            {
                throw new ArgumentNullException("layout");
            }

            this.Query = query;
            this.Layout = layout;
            this.Index = index;
        }

        /// <summary>
        /// Gets the position of this query and layout in the list of queries and layouts.
        /// </summary>
        /// <remarks>
        /// A value of -1 indicates that it is not stored in the query and layout manager and does not therefore have a position.
        /// </remarks>
        public int Index { get; private set; }

        /// <summary>
        /// Gets the definition of the query.
        /// </summary>
        public QueryDefinition Query { get; private set; }

        /// <summary>
        /// Gets the layout to be used with the query.
        /// </summary>
        public LayoutInformation Layout { get; private set; }
    }
}
