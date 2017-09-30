//---------------------------------------------------------------------
// <copyright file="QueryAndLayoutManager.cs" company="Microsoft">
//    Copyright © ALM | DevOps Ranger Contributors. All rights reserved.
//    This code is licensed under the MIT License.
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF 
//    ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
//    TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
//    PARTICULAR PURPOSE AND NONINFRINGEMENT.
// </copyright>
// <summary>The QueryAndLayoutManager type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.Word4Tfs.Library.Model
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text.RegularExpressions;
    using Microsoft.TeamFoundation.WorkItemTracking.Client;
    using Microsoft.Word4Tfs.Library.Data;
    using Microsoft.Word4Tfs.Library.Model.TeamFoundation;

    /// <summary>
    /// Manages multiple queries and layouts
    /// </summary>
    public class QueryAndLayoutManager : IQueryAndLayoutManager
    {
        /// <summary>
        /// The pattern used to identify and extract the field list and the rest of the query.
        /// </summary>
        private static readonly Regex WiqlPartsPattern = new Regex(@"\s*SELECT\s+(?<fields>.+) FROM (?<restOfQuery>.+)", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Singleline);

        /// <summary>
        /// The pattern used to extract the field name from a field name with white space and square brackets.
        /// </summary>
        private static readonly Regex FieldNamePattern = new Regex(@"(?<fieldName>(" + Constants.ReferenceNamePattern + "))");

        /// <summary>
        /// The actual list of queries and layouts.
        /// </summary>
        private List<QueryAndLayoutInformation> originalQueriesAndLayouts;

        /// <summary>
        /// The list of final queries.
        /// </summary>
        private List<QueryAndLayoutInformation> finalQueriesAndLayouts;

        /// <summary>
        /// All the fields defined in the team project.
        /// </summary>
        private IEnumerable<ITfsFieldDefinition> projectFields;

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryAndLayoutManager"/> class.
        /// </summary>
        /// <param name="projectFields">All the fields defined in the team project.</param>
        public QueryAndLayoutManager(IEnumerable<ITfsFieldDefinition> projectFields)
        {
            this.projectFields = projectFields;
            this.originalQueriesAndLayouts = new List<QueryAndLayoutInformation>();
            this.finalQueriesAndLayouts = new List<QueryAndLayoutInformation>();
            this.OriginalQueriesAndLayouts = this.originalQueriesAndLayouts;
            this.FinalQueriesAndLayouts = this.finalQueriesAndLayouts;
        }

        /// <summary>
        /// Gets a merged list of all the fields in all the layouts
        /// </summary>
        public IEnumerable<string> AllLayoutFields
        {
            get
            {
                return this.GetAllLayoutFields();
            }
        }

        /// <summary>
        /// Gets the current list of queries and layouts as originally chosen.
        /// </summary>
        public IEnumerable<QueryAndLayoutInformation> OriginalQueriesAndLayouts { get; private set; }

        /// <summary>
        /// Gets the final query definitions and the layouts.
        /// </summary>
        /// <remarks>
        /// The final queries include all the fields from all the layouts and exclude any fields not defined in the team project.
        /// </remarks>
        /// <returns>The query definitions for the final queries to be executed.</returns>
        public IEnumerable<QueryAndLayoutInformation> FinalQueriesAndLayouts { get; private set; }

        /// <summary>
        /// Adds a new query and layout to the manager.
        /// </summary>
        /// <param name="queryAndLayout">The query and layout to be added.</param>
        /// <returns>The query and layout with the query modified to include all the fields from all the layouts and exclude any fields not defined in the team project.</returns>
        public QueryAndLayoutInformation Add(QueryAndLayoutInformation queryAndLayout)
        {
            this.originalQueriesAndLayouts.Add(queryAndLayout);
            this.BuildFinalQueries();
            return this.finalQueriesAndLayouts.Last();
        }

        /// <summary>
        /// Adds new queries and layouts to the manager.
        /// </summary>
        /// <param name="queriesAndLayouts">The queries and layouts to be added.</param>
        public void AddRange(params QueryAndLayoutInformation[] queriesAndLayouts)
        {
            this.originalQueriesAndLayouts.AddRange(queriesAndLayouts);
            this.BuildFinalQueries();
        }

        /// <summary>
        /// Merges extra field names into a query.
        /// </summary>
        /// <param name="query">The query into which the fields are to be merged.</param>
        /// <param name="fieldsToMerge">The names of the fields to be merged into the query.</param>
        /// <param name="fieldsInProject">The fields which are currently defined in the team project.</param>
        /// <returns>A new <see cref="QueryDefinition"/> that contains all the fields from <paramref name="query"/> plus any not already there that are listed in <paramref name="fieldsToMerge"/>, excluding those not in <paramref name="fieldsInProject"/>.</returns>
        private static QueryDefinition MergeLayoutFieldsIntoQuery(QueryDefinition query, string[] fieldsToMerge, IEnumerable<ITfsFieldDefinition> fieldsInProject)
        {
            if (query == null)
            {
                throw new ArgumentNullException("query");
            }

            QueryDefinition ans = null;
            Match m = WiqlPartsPattern.Match(query.QueryText);
            if (m.Success)
            {
                string existingFieldsList = m.Groups[1].Captures[0].Value;
                IEnumerable<string> existingFields = existingFieldsList.Split(',').Select(f => ExtractFieldName(f));
                string restOfQuery = m.Groups[2].Captures[0].Value;

                IEnumerable<string> fieldNamesInProject = fieldsInProject.Select(f => ExtractFieldName(f.ReferenceName));

                IEnumerable<string> newFields = fieldsToMerge
                                                .Where(field => !existingFields.Contains(field, StringComparer.OrdinalIgnoreCase) && fieldNamesInProject.Contains(field, StringComparer.OrdinalIgnoreCase))
                                                .Select(field => string.Concat(", [", field, "]"));

                if (newFields.Count() > 0)
                {
                    string newQueryText = string.Concat("SELECT ", existingFieldsList, string.Concat(newFields.ToArray()), " FROM ", restOfQuery);
                    ans = new QueryDefinition(query.Name, newQueryText);
                }
                else
                {
                    ans = query;
                }
            }
            else
            {
                throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, ModelResources.QueryCouldNotBeParsed, query.QueryText));
            }

            return ans;
        }

        /// <summary>
        /// Takes a full field name, with optional square brackets and white space and gets the actual field name.
        /// </summary>
        /// <param name="fullFieldName">The field name to parse.</param>
        /// <returns>The actual field name.</returns>
        private static string ExtractFieldName(string fullFieldName)
        {
            string ans = null;
            Match m = FieldNamePattern.Match(fullFieldName);
            if (m.Success)
            {
                ans = m.Groups[1].Captures[0].Value;
            }

            return ans;
        }

        /// <summary>
        /// Builds the final queries from the existing queries and layout fields.
        /// </summary>
        private void BuildFinalQueries()
        {
            string[] allLayoutFields = this.GetAllLayoutFields();
            this.finalQueriesAndLayouts.Clear();
            int i = 0;
            this.finalQueriesAndLayouts.AddRange(this.originalQueriesAndLayouts.Select(ql => new QueryAndLayoutInformation(MergeLayoutFieldsIntoQuery(ql.Query, allLayoutFields, this.projectFields), ql.Layout, i++)));
        }

        /// <summary>
        /// Gets the list of all fields from all layouts.
        /// </summary>
        /// <returns>The list of fields.</returns>
        private string[] GetAllLayoutFields()
        {
            string[] allLayoutFields = new string[0];
            foreach (QueryAndLayoutInformation ql in this.originalQueriesAndLayouts)
            {
                allLayoutFields = allLayoutFields.Union(ql.Layout.FieldNames).ToArray();
            }

            allLayoutFields = allLayoutFields.Distinct(StringComparer.OrdinalIgnoreCase).ToArray();
            return allLayoutFields;
        }
    }
}
