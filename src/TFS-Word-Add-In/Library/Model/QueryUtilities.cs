//---------------------------------------------------------------------
// <copyright file="QueryUtilities.cs" company="Microsoft">
//    Copyright (c) Microsoft. All rights reserved.
//    This code is licensed under the MIT License.
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF 
//    ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
//    TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
//    PARTICULAR PURPOSE AND NONINFRINGEMENT.
// </copyright>
// <summary>The QueryUtilities type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.Word4Tfs.Library.Model
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using Microsoft.TeamFoundation.WorkItemTracking.Client;
    using Microsoft.Word4Tfs.Library.Model.TeamFoundation;

    /// <summary>
    /// Provides some utility functions for work item queries.
    /// </summary>
    public static class QueryUtilities
    {
        /// <summary>
        /// Takes a tree query and returns a query string to get the associated items by Id.
        /// </summary>
        /// <param name="query">The query to be converted.</param>
        /// <returns>The query for the fields without any clauses that will not allow items to be retrieved by id.</returns>
        public static string ConvertTreeQueryToQueryForItem(ITfsQuery query)
        {
            if (query == null)
            {
                throw new ArgumentNullException("query");
            }

            StringBuilder newQueryString = new StringBuilder();
            newQueryString.Append("SELECT ");
            bool first = true;
            foreach (ITfsFieldDefinition df in query.DisplayFieldList)
            {
                if (!first)
                {
                    newQueryString.Append(", ");
                }

                newQueryString.Append(df.ReferenceName);
                first = false;
            }

            newQueryString.Append(" FROM WorkItems");

            return newQueryString.ToString();
        }
    }
}
