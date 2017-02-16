//---------------------------------------------------------------------
// <copyright file="ITeamProjectDocumentFormatter.cs" company="Microsoft">
//    Copyright (c) Microsoft. All rights reserved.
//    This code is licensed under the MIT License.
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF 
//    ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
//    TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
//    PARTICULAR PURPOSE AND NONINFRINGEMENT.
// </copyright>
// <summary>The ITeamProjectDocumentFormatter type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.Word4Tfs.Library.Model
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using Microsoft.Office.Core;
    using Microsoft.Word4Tfs.Library.Data;

    /// <summary>
    /// Defines the operations to format a document with work items
    /// </summary>
    public interface ITeamProjectDocumentFormatter
    {
        /// <summary>
        /// Maps work items into the document using the given layout. Each work item is bookmarked.
        /// </summary>
        /// <param name="workItems">The work items to be mapped into the document.</param>
        /// <param name="layout">The layout to be used to map the saved work items.</param>
        /// <param name="bookmarkNamingFunction">A function to return the bookmark to use for the given work item id.</param>
        /// <param name="cancellationToken">Used to cancel the save.</param>
        /// <remarks>
        /// It is assumed that the document already contains the XML data for the work items.
        /// </remarks>
        void MapWorkItemsIntoDocument(WorkItemTree workItems, LayoutInformation layout, Func<int, string> bookmarkNamingFunction, CancellationToken cancellationToken);

        /// <summary>
        /// Refreshes the work items in the document.
        /// </summary>
        /// <remarks>
        /// <para>Deletes work items that are no longer returned, adds work items that are now returned, and re-orders work items that have changed position in query results.</para>
        /// <para>This call will also update the rich text content controls which are not bound to the Custom XML Parts.</para>
        /// </remarks>
        /// <param name="refreshData">The data needed for a refresh.</param>
        /// <param name="bookmarkNamingFunction">A function to return the bookmark to use for the given work item, first parameter is query index, second is work item id.</param>
        /// <param name="cancellationToken">Used to cancel the refresh.</param>
        void RefreshWorkItems(FormatterRefreshData refreshData, Func<int, int, string> bookmarkNamingFunction, CancellationToken cancellationToken);
    }

    /// <summary>
    /// Data transfer class used to pass data to the formatter refresh operation.
    /// </summary>
    /// <remarks>Required because Moq has a limitation of 4 parameters per method on callbacks.
    /// </remarks>
    public class FormatterRefreshData
    {
        /// <summary>
        /// Gets or sets the work item manager that contains the new work work items for a refresh operation.
        /// </summary>
        public WorkItemManager WorkItemManager { get; set; }

        /// <summary>
        /// Gets or sets the query to work item association data from before the refresh.
        /// </summary>
        public IEnumerable<QueryWorkItems> QueryWorkItemsBefore { get; set; }

        /// <summary>
        /// Gets or sets the query to work item association data as a result of the refresh.
        /// </summary>
        public IEnumerable<QueryWorkItems> QueryWorkItemsAfter { get; set; }

        /// <summary>
        /// Gets or sets the layout information associated with each query.
        /// </summary>
        public IEnumerable<LayoutInformation> Layouts { get; set; }

        /// <summary>
        /// Gets or sets the information about whether the query is flat or not for each query.
        /// </summary>
        public IEnumerable<bool> QueryIsFlat { get; set; }
    }
}
