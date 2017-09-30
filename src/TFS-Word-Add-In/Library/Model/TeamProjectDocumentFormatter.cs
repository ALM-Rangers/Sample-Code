//---------------------------------------------------------------------
// <copyright file="TeamProjectDocumentFormatter.cs" company="Microsoft">
//    Copyright © ALM | DevOps Ranger Contributors. All rights reserved.
//    This code is licensed under the MIT License.
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF 
//    ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
//    TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
//    PARTICULAR PURPOSE AND NONINFRINGEMENT.
// </copyright>
// <summary>The TeamProjectDocumentFormatter type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.Word4Tfs.Library.Model
{
    using System;
    using global::System.Collections.Generic;
    using global::System.Diagnostics;
    using global::System.Globalization;
    using global::System.Linq;
    using global::System.Text.RegularExpressions;
    using global::System.Threading;
    using Microsoft.Office.Core;
    using Microsoft.Office.Interop.Word;
    using Microsoft.Word4Tfs.Library.Data;
    using Microsoft.Word4Tfs.Library.Model.TeamFoundation;
    using Microsoft.Word4Tfs.Library.Model.Word;

    /// <summary>
    /// Formats a document with work items
    /// </summary>
    public class TeamProjectDocumentFormatter : ITeamProjectDocumentFormatter
    {
        /// <summary>
        /// The pattern used to check and capture the elements of a rich text content control tag.
        /// </summary>
        private static readonly Regex RichTextContentControlTagPattern = new Regex(@"(?<fieldName>" + Constants.ReferenceNamePattern + @")-(?<id>[0-9]+)", RegexOptions.None);

        /// <summary>
        /// The object to use for logging.
        /// </summary>
        private ILogger logger;

        /// <summary>
        /// The Word document to be used to store the project data.
        /// </summary>
        private IWordDocument wordDocument;

        /// <summary>
        /// The team project template to be used for accessing building blocks.
        /// </summary>
        private ITeamProjectTemplate teamProjectTemplate;

        /// <summary>
        /// Initializes a new instance of the <see cref="TeamProjectDocumentFormatter"/> class.
        /// </summary>
        /// <param name="wordDocument">The <see cref="IWordDocument"/> object to be used by the formatter.</param>
        /// <param name="teamProjectTemplate">The <see cref="ITeamProjectTemplate"/> object to be used by the formatter to get building block definitions.</param>
        /// <param name="logger">The object to be used for logging.</param>
        public TeamProjectDocumentFormatter(IWordDocument wordDocument, ITeamProjectTemplate teamProjectTemplate, ILogger logger)
        {
            this.wordDocument = wordDocument;
            this.teamProjectTemplate = teamProjectTemplate;
            this.logger = logger;
        }

        /// <summary>
        /// Indicates where content is to be inserted.
        /// </summary>
        private enum InsertionPoint
        {
            /// <summary>
            /// Content is to be inserted at the "current" location.
            /// </summary>
            CurrentLocation,

            /// <summary>
            /// Content is to be inserted before a relative bookmark.
            /// </summary>
            BeforeBookmark,

            /// <summary>
            /// Content is to be inserted after a relative bookmark.
            /// </summary>
            AfterBookmark
        }

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
        public void MapWorkItemsIntoDocument(WorkItemTree workItems, LayoutInformation layout, Func<int, string> bookmarkNamingFunction, CancellationToken cancellationToken)
        {
            if (workItems == null)
            {
                throw new ArgumentNullException("workItems");
            }

            if (layout == null)
            {
                throw new ArgumentNullException("layout");
            }

            if (bookmarkNamingFunction == null)
            {
                throw new ArgumentNullException("bookmarkNamingFunction");
            }

            DateTime start = DateTime.Now;
            int count = 0;

            CustomXMLPart part = this.wordDocument.GetXmlPart(Constants.WorkItemNamespace);

            if (this.wordDocument.IsAtStart())
            {
                this.wordDocument.InsertParagraph(ModelResources.FormatterStartOfImportBoilerPlate, Constants.NormalStyleName);
            }

            foreach (WorkItemTreeNode node in workItems.DepthFirstNodes())
            {
                cancellationToken.ThrowIfCancellationRequested();

                count++;
                IWorkItemLayout workItemLayout = new WorkItemLayout(layout, this.teamProjectTemplate);

                this.MapWorkItemIntoDocument(node, workItemLayout, bookmarkNamingFunction, part, InsertionPoint.CurrentLocation, 0);
            }

            if (this.wordDocument.IsAtEnd())
            {
                this.wordDocument.InsertParagraph(ModelResources.FormatterEndOfImportBoilerPlate, Constants.NormalStyleName);
            }

            DateTime end = DateTime.Now;
            this.logger.Log(TraceEventType.Information, "Elapsed time to insert {0} items was {1} seconds", count, (end - start).TotalSeconds);
        }

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
        public void RefreshWorkItems(FormatterRefreshData refreshData, Func<int, int, string> bookmarkNamingFunction, CancellationToken cancellationToken)
        {
            if (refreshData == null)
            {
                throw new ArgumentNullException("refreshData");
            }

            Debug.Assert(refreshData.QueryWorkItemsBefore.Count() == refreshData.QueryWorkItemsAfter.Count(), "Before and after data have different numbers of queries");
            Debug.Assert(refreshData.QueryWorkItemsBefore.Count() == refreshData.Layouts.Count(), "Number of layouts does not match number of queries");
            Debug.Assert(refreshData.QueryWorkItemsBefore.Count() == refreshData.QueryIsFlat.Count(), "Number of QueryIsFlat values does not match number of queries");

            DateTime start = DateTime.Now;

            CustomXMLPart part = this.wordDocument.GetXmlPart(Constants.WorkItemNamespace);

            int n = refreshData.QueryWorkItemsBefore.Count();
            QueryWorkItems[] queryWorkItemsBefore = refreshData.QueryWorkItemsBefore.ToArray();
            QueryWorkItems[] queryWorkItemsAfter = refreshData.QueryWorkItemsAfter.ToArray();
            bool[] queryIsFlat = refreshData.QueryIsFlat.ToArray();
            LayoutInformation[] layouts = refreshData.Layouts.ToArray();
            for (int queryIndex = 0; queryIndex < n; queryIndex++)
            {
                Func<int, string> queryBookmarkNamingFunction = (int id) => bookmarkNamingFunction(queryIndex, id);
                this.logger.Log(TraceEventType.Verbose, "Refreshing query {0}", queryIndex);

                // Compute the actual before work items that are still in the document.
                QueryWorkItems actualBeforeWorkItems = new QueryWorkItems(queryIndex, queryWorkItemsBefore[queryIndex].WorkItemIds.Where(id => this.FindBookmark(queryBookmarkNamingFunction(id)) != null).ToArray());

                bool isManuallySorted = this.IsManuallySorted(queryWorkItemsBefore[queryIndex].WorkItemIds, queryBookmarkNamingFunction);
                if (!queryIsFlat[queryIndex] || (queryIsFlat[queryIndex] && !isManuallySorted))
                {
                    this.SortExistingWorkItems(actualBeforeWorkItems, queryWorkItemsAfter[queryIndex], queryBookmarkNamingFunction, cancellationToken);
                    isManuallySorted = false;
                }

                IWorkItemLayout workItemLayout = new WorkItemLayout(layouts[queryIndex], this.teamProjectTemplate);
                this.AddNewWorkItems(actualBeforeWorkItems, queryWorkItemsAfter[queryIndex], workItemLayout, part, queryBookmarkNamingFunction, isManuallySorted, cancellationToken);
            }

            this.RefreshDeleteRemovedWorkItems(refreshData.QueryWorkItemsBefore.ToArray(), refreshData.QueryWorkItemsAfter.ToArray(), bookmarkNamingFunction, cancellationToken);
            this.RefreshRichTextContentControls(refreshData.WorkItemManager, cancellationToken);

            DateTime end = DateTime.Now;
            this.logger.Log(TraceEventType.Information, "Elapsed time to refresh was {0} seconds", (end - start).TotalSeconds);
        }

        /// <summary>
        /// Finds the first work item id in <paramref name="afterIds"/> that already exists in <paramref name="beforeIds"/>
        /// </summary>
        /// <param name="afterIds">The list of work item ids to scan for the first existing work item id.</param>
        /// <param name="beforeIds">The list of existing work item ids</param>
        /// <returns>The id of the first work item in <paramref name="afterIds"/> that exists in <paramref name="beforeIds"/>, returns -1 if none found.</returns>
        private static int FindFirstExistingWorkItemId(int[] afterIds, int[] beforeIds)
        {
            int ans = -1;
            foreach (int id in afterIds)
            {
                if (beforeIds.Contains(id))
                {
                    ans = id;
                    break;
                }
            }

            return ans;
        }

        /// <summary>
        /// Finds the last work item id in <paramref name="afterIds"/> that already exists in <paramref name="beforeIds"/>.
        /// </summary>
        /// <param name="afterIds">The list of work item ids to scan for the last existing work item id.</param>
        /// <param name="beforeIds">The list of existing work item ids</param>
        /// <returns>The id of the last work item in <paramref name="afterIds"/> that exists in <paramref name="beforeIds"/>, returns -1 if none found.</returns>
        private static int FindLastExistingWorkItemId(int[] afterIds, int[] beforeIds)
        {
            int ans = -1;
            for (int i = afterIds.Length - 1; i >= 0; i--)
            {
                int id = afterIds[i];
                if (beforeIds.Contains(id))
                {
                    ans = id;
                    break;
                }
            }

            return ans;
        }

        /// <summary>
        /// Maps a single work item into the document.
        /// </summary>
        /// <param name="node">The node for the work item to map.</param>
        /// <param name="workItemLayout">The work item layout that is to be used.</param>
        /// <param name="bookmarkNamingFunction">A function to return the bookmark to use for the given work item id.</param>
        /// <param name="workItemsCustomXMLPart">The custom XML part that contains the work item data.</param>
        /// <param name="insertionPoint">The relative location where the work item is to be mapped.</param>
        /// <param name="relativeWorkItemId">If inserting relative to a work item, the id of the work item relative to which the insertion is to be done, otherwise ignored.</param>
        private void MapWorkItemIntoDocument(WorkItemTreeNode node, IWorkItemLayout workItemLayout, Func<int, string> bookmarkNamingFunction, CustomXMLPart workItemsCustomXMLPart, InsertionPoint insertionPoint, int relativeWorkItemId)
        {
            BuildingBlock buildingBlock = workItemLayout.ChooseBuildingBlock(node);
            IEnumerable<ContentControl> contentControls = null;
            switch (insertionPoint)
            {
                case InsertionPoint.CurrentLocation:
                    {
                        contentControls = this.wordDocument.InsertBuildingBlock(buildingBlock, bookmarkNamingFunction(node.WorkItem.Id));
                        break;
                    }

                case InsertionPoint.BeforeBookmark:
                    {
                       contentControls = this.wordDocument.InsertBuildingBlockBeforeBookmark(buildingBlock, bookmarkNamingFunction(node.WorkItem.Id), bookmarkNamingFunction(relativeWorkItemId));
                       break;
                    }

                case InsertionPoint.AfterBookmark:
                    {
                        contentControls = this.wordDocument.InsertBuildingBlockAfterBookmark(buildingBlock, bookmarkNamingFunction(node.WorkItem.Id), bookmarkNamingFunction(relativeWorkItemId));
                        break;
                    }
            }

            foreach (ContentControl c in contentControls)
            {
                if (Utilities.IsValidTag(c.Tag) && node.WorkItem.FieldReferenceNames.Contains(c.Tag))
                {
                    if (c.Type == WdContentControlType.wdContentControlText || c.Type == WdContentControlType.wdContentControlDate)
                    {
                        string xpath = string.Format(CultureInfo.InvariantCulture, "/wi:WorkItems/wi:WorkItem[wi:Field[@name='{0}']={1}]/wi:Field[@name='{2}']", Constants.SystemIdFieldReferenceName, node.WorkItem.Id, c.Tag);
                        this.wordDocument.MapContentControl(c, xpath, "xmlns:wi='" + Constants.WorkItemNamespace + "'", workItemsCustomXMLPart);
                    }
                    else if (c.Type == WdContentControlType.wdContentControlRichText)
                    {
                        this.wordDocument.PopulateRichTextContentControlWithHtml(c, node.WorkItem[c.Tag].ToString());
                        c.Tag = string.Concat(c.Tag, "-", node.WorkItem.Id.ToString(CultureInfo.InvariantCulture));
                    }
                }
            }
        }

        /// <summary>
        /// Deletes the work items that have been removed by this refresh.
        /// </summary>
        /// <param name="queryWorkItemsBefore">The query to work item association data from before the refresh.</param>
        /// <param name="queryWorkItemsAfter">The query to work item association data as a result of the refresh.</param>
        /// <param name="bookmarkNamingFunction">A function to return the bookmark to use for the given work item, first parameter is query index, second is work item id.</param>
        /// <param name="cancellationToken">Used to cancel the refresh.</param>
        private void RefreshDeleteRemovedWorkItems(QueryWorkItems[] queryWorkItemsBefore, QueryWorkItems[] queryWorkItemsAfter, Func<int, int, string> bookmarkNamingFunction, CancellationToken cancellationToken)
        {
            this.logger.Log(TraceEventType.Verbose, "Deleting work items that have been removed");
            Debug.Assert(queryWorkItemsBefore.Length == queryWorkItemsAfter.Length, "Before and after data have different numbers of queries");
            IEnumerable<Tuple<QueryWorkItems, QueryWorkItems>> beforeAndAfter = from before in queryWorkItemsBefore
                                                                                join after in queryWorkItemsAfter on before.QueryIndex equals after.QueryIndex
                                                                                select new Tuple<QueryWorkItems, QueryWorkItems>(before, after);
            foreach (Tuple<QueryWorkItems, QueryWorkItems> ba in beforeAndAfter)
            {
                cancellationToken.ThrowIfCancellationRequested();
                foreach (int id in ba.Item1.WorkItemIds.Except(ba.Item2.WorkItemIds))
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    this.wordDocument.DeleteBookmarkAndContent(bookmarkNamingFunction(ba.Item1.QueryIndex, id));
                }
            }
        }

        /// <summary>
        /// Checks to see if the document has been manually sorted in respect of the work items belonging to a particular query.
        /// </summary>
        /// <param name="workItemIds">The work item ids in the order they were returned the last time the query was executed.</param>
        /// <param name="bookmarkNamingFunction">A function to return the bookmark to use for the given work item, given the work item id.</param>
        /// <returns><c>true</c> if the work items have been manually sorted, in other words the order in the document does not match the order the last time the query was executed.</returns>
        private bool IsManuallySorted(IEnumerable<int> workItemIds, Func<int, string> bookmarkNamingFunction)
        {
            bool ans = false;

            Bookmark lastBookmark = null;
            foreach (int workItemId in workItemIds)
            {
                string name = bookmarkNamingFunction(workItemId);
                Bookmark nextBookmark = this.FindBookmark(name);
                if (nextBookmark != null)
                {
                    if (lastBookmark != null)
                    {
                        if (nextBookmark.Start < lastBookmark.End)
                        {
                            ans = true;
                            break;
                        }
                    }

                    lastBookmark = nextBookmark;
                }
            }

            return ans;
        }

        /// <summary>
        /// Sorts the work items that were in the last run of a query.
        /// </summary>
        /// <remarks>
        /// Takes account of the physical order of the bookmarks in the document.
        /// </remarks>
        /// <param name="queryWorkItemsBefore">The query to work item association data from before the refresh.</param>
        /// <param name="queryWorkItemsAfter">The query to work item association data as a result of the refresh.</param>
        /// <param name="bookmarkNamingFunction">A function to return the bookmark to use for the given work item, given the work item id.</param>
        /// <param name="cancellationToken">Used to cancel the refresh.</param>
        private void SortExistingWorkItems(QueryWorkItems queryWorkItemsBefore, QueryWorkItems queryWorkItemsAfter, Func<int, string> bookmarkNamingFunction, CancellationToken cancellationToken)
        {
            this.logger.Log(TraceEventType.Verbose, "Sorting existing work items");
            cancellationToken.ThrowIfCancellationRequested();
            int[] currentIds = queryWorkItemsBefore.WorkItemIds.OrderBy((int id) => this.FindBookmark(bookmarkNamingFunction(id)).Start)
                                                               .ToArray(); // sort into physical order
            int[] afterIds = queryWorkItemsAfter.WorkItemIds.Intersect(currentIds).ToArray();
            int afterIdsLen = afterIds.Length;
            int currentIdsLen = currentIds.Length;
            for (int i = 0; i < afterIdsLen - 1; i++)
            {
                if (afterIds[i] != currentIds[i])
                {
                    this.wordDocument.MoveBookmarkAndContentToBefore(bookmarkNamingFunction(afterIds[i]), bookmarkNamingFunction(currentIds[i]));
                    int oldPosOfMovedItem = -1;
                    for (int k = i + 1; k < currentIdsLen; k++)
                    {
                        if (currentIds[k] == afterIds[i])
                        {
                            oldPosOfMovedItem = k;
                            break;
                        }
                    }

                    for (int j = oldPosOfMovedItem; j > i; j--)
                    {
                        currentIds[j] = currentIds[j - 1];
                    }

                    currentIds[i] = afterIds[i];
                }
            }
        }

        /// <summary>
        /// Adds the new work items that were not in the last run of a query.
        /// </summary>
        /// <remarks>
        /// If the current work items have been manually sorted then new items are just added to the end, otherwise they are added in the new sort order.
        /// </remarks>
        /// <param name="queryWorkItemsBefore">The query to work item association data from before the refresh.</param>
        /// <param name="queryWorkItemsAfter">The query to work item association data as a result of the refresh.</param>
        /// <param name="workItemLayout">The work item layout associated with the query.</param>
        /// <param name="customXMLPart">The custom XML part that contains the work item data.</param>
        /// <param name="bookmarkNamingFunction">A function to return the bookmark to use for the given work item, given the work item id.</param>
        /// <param name="isManuallySorted">Indicates if the current work items are manually sorted.</param>
        /// <param name="cancellationToken">Used to cancel the refresh.</param>
        private void AddNewWorkItems(QueryWorkItems queryWorkItemsBefore, QueryWorkItems queryWorkItemsAfter, IWorkItemLayout workItemLayout, CustomXMLPart customXMLPart, Func<int, string> bookmarkNamingFunction, bool isManuallySorted, CancellationToken cancellationToken)
        {
            this.logger.Log(TraceEventType.Verbose, "Adding new work items");
            cancellationToken.ThrowIfCancellationRequested();
            int[] newIds = queryWorkItemsAfter.WorkItemIds.Except(queryWorkItemsBefore.WorkItemIds).ToArray();
            if (newIds.Length > 0)
            {
                // We have some new work items that were not present last time the query was executed.
                int[] beforeIds = queryWorkItemsBefore.WorkItemIds.ToArray();
                int[] afterIds = queryWorkItemsAfter.WorkItemIds.ToArray();
                WorkItemTreeNode[] nodes = queryWorkItemsAfter.WorkItemTreeNodes.ToArray();
                Debug.Assert(afterIds.Length == nodes.Length, "Number of after nodes must match number of after ids.");
                if (isManuallySorted)
                {
                    this.AddAllNewWorkItemsAfterLastExisting(newIds, this.GetLastPhysicalWorkItem(beforeIds, bookmarkNamingFunction), nodes, workItemLayout, customXMLPart, bookmarkNamingFunction);
                }
                else if (afterIds.Intersect(beforeIds).Count() > 0)
                {
                    this.AddNewWorkItemsBeforeExisting(newIds, beforeIds, afterIds, nodes, workItemLayout, customXMLPart, (int id) => bookmarkNamingFunction(id));
                    this.AddNewWorkItemsAfterExisting(newIds, beforeIds, afterIds, nodes, workItemLayout, customXMLPart, (int id) => bookmarkNamingFunction(id));
                }
                else
                {
                    this.AddNewWorkItemsWhenNoneBefore(afterIds, nodes, workItemLayout, customXMLPart, (int id) => bookmarkNamingFunction(id));
                }
            }
        }

        /// <summary>
        /// Finds the work item which is the last to physically appear in the document.
        /// </summary>
        /// <param name="workItemIds">The list of work items ids to check for the last physical one.</param>
        /// <param name="bookmarkNamingFunction">A function to return the bookmark to use for the given work item, given the work item id.</param>
        /// <returns>The id of the last work item to physically appear in the document, -1 if none found.</returns>
        private int GetLastPhysicalWorkItem(int[] workItemIds, Func<int, string> bookmarkNamingFunction)
        {
            int ans = -1;
            int lastStart = -1;
            foreach (int workItemId in workItemIds)
            {
                string name = bookmarkNamingFunction(workItemId);
                Bookmark bookmark = this.FindBookmark(name);
                if (bookmark.Start > lastStart)
                {
                    ans = workItemId;
                    lastStart = bookmark.Start;
                }
            }

            return ans;
        }

        /// <summary>
        /// Finds a bookmark by name.
        /// </summary>
        /// <param name="name">The name of the bookmark to find.</param>
        /// <returns>The bookmark, <c>null</c> if not found.</returns>
        private Bookmark FindBookmark(string name)
        {
            return this.wordDocument.BookmarkManager[name];
        }

        /// <summary>
        /// Adds new work items that come before any existing ones.
        /// </summary>
        /// <param name="newIds">The new work item ids.</param>
        /// <param name="beforeIds">The old list of work item ids.</param>
        /// <param name="afterIds">The new list of work item ids.</param>
        /// <param name="nodes">The work item tree nodes for the query being processed.</param>
        /// <param name="workItemLayout">The work item layout to apply to the newly added work items.</param>
        /// <param name="customXMLPart">The custom XML part that contains the work item data.</param>
        /// <param name="bookmarkNamingFunction">A function to return the bookmark to use for the given work item, parameter is work item id.</param>
        private void AddNewWorkItemsBeforeExisting(int[] newIds, int[] beforeIds, int[] afterIds, WorkItemTreeNode[] nodes, IWorkItemLayout workItemLayout, CustomXMLPart customXMLPart, Func<int, string> bookmarkNamingFunction)
        {
            for (int i = 0; i < afterIds.Length; i++)
            {
                if (newIds.Contains(afterIds[i]))
                {
                    int precedes = FindFirstExistingWorkItemId(afterIds.Skip(i + 1).ToArray(), beforeIds);
                    if (precedes >= 0)
                    {
                        this.MapWorkItemIntoDocument(nodes[i], workItemLayout, (int id) => bookmarkNamingFunction(id), customXMLPart, InsertionPoint.BeforeBookmark, precedes);
                    }
                }
            }
        }

        /// <summary>
        /// Adds new work items that come after any existing ones, stopping when the first existing one is found.
        /// </summary>
        /// <param name="newIds">The new work item ids.</param>
        /// <param name="beforeIds">The old list of work item ids.</param>
        /// <param name="afterIds">The new list of work item ids.</param>
        /// <param name="nodes">The work item tree nodes for the query being processed.</param>
        /// <param name="workItemLayout">The work item layout to apply to the newly added work items.</param>
        /// <param name="customXMLPart">The custom XML part that contains the work item data.</param>
        /// <param name="bookmarkNamingFunction">A function to return the bookmark to use for the given work item, parameter is work item id.</param>
        private void AddNewWorkItemsAfterExisting(int[] newIds, int[] beforeIds, int[] afterIds, WorkItemTreeNode[] nodes, IWorkItemLayout workItemLayout, CustomXMLPart customXMLPart, Func<int, string> bookmarkNamingFunction)
        {
            for (int i = afterIds.Length - 1; i >= 0; i--)
            {
                if (newIds.Contains(afterIds[i]))
                {
                    int succeeds = FindLastExistingWorkItemId(afterIds.Take(i).ToArray(), beforeIds);
                    if (succeeds >= 0)
                    {
                        this.MapWorkItemIntoDocument(nodes[i], workItemLayout, (int id) => bookmarkNamingFunction(id), customXMLPart, InsertionPoint.AfterBookmark, succeeds);
                    }
                }
                else
                {
                    break;
                }
            }
        }

        /// <summary>
        /// Adds new work items after the last existing one.
        /// </summary>
        /// <param name="newIds">The new work items ids.</param>
        /// <param name="succeeds">The id of the last existing work item.</param>
        /// <param name="nodes">The work item tree nodes for the query being processed.</param>
        /// <param name="workItemLayout">The work item layout to apply to the newly added work items.</param>
        /// <param name="customXMLPart">The custom XML part that contains the work item data.</param>
        /// <param name="bookmarkNamingFunction">A function to return the bookmark to use for the given work item, parameter is work item id.</param>
        private void AddAllNewWorkItemsAfterLastExisting(int[] newIds, int succeeds, WorkItemTreeNode[] nodes, IWorkItemLayout workItemLayout, CustomXMLPart customXMLPart, Func<int, string> bookmarkNamingFunction)
        {
            int lastId = succeeds;
            for (int i = 0; i < newIds.Length; i++)
            {
                this.MapWorkItemIntoDocument(nodes.Where(n => n.WorkItem.Id == newIds[i]).Single(), workItemLayout, (int id) => bookmarkNamingFunction(id), customXMLPart, InsertionPoint.AfterBookmark, lastId);
                lastId = newIds[i];
            }
        }

        /// <summary>
        /// Adds new work items when there were none before to place the new ones relative to.
        /// </summary>
        /// <param name="afterIds">The new list of work item ids.</param>
        /// <param name="nodes">The work item tree nodes for the query being processed.</param>
        /// <param name="layout">The layout to apply to the newly added work items.</param>
        /// <param name="customXMLPart">The custom XML part that contains the work item data.</param>
        /// <param name="bookmarkNamingFunction">A function to return the bookmark to use for the given work item, parameter is work item id.</param>
        private void AddNewWorkItemsWhenNoneBefore(int[] afterIds, WorkItemTreeNode[] nodes, IWorkItemLayout layout, CustomXMLPart customXMLPart, Func<int, string> bookmarkNamingFunction)
        {
            for (int i = 0; i < afterIds.Length; i++)
            {
                this.MapWorkItemIntoDocument(nodes[i], layout, (int id) => bookmarkNamingFunction(id), customXMLPart, InsertionPoint.CurrentLocation, 0);
            }
        }

        /// <summary>
        /// Refreshes the rich text content controls which are not bound to the Custom XML Parts.
        /// </summary>
        /// <param name="workItemManager">The work item manager that contains the new work items.</param>
        /// <param name="cancellationToken">Used to cancel the refresh.</param>
        private void RefreshRichTextContentControls(WorkItemManager workItemManager, CancellationToken cancellationToken)
        {
            this.logger.Log(TraceEventType.Verbose, "Refreshing rich text content controls");
            foreach (ContentControl c in this.wordDocument.AllContentControls())
            {
                cancellationToken.ThrowIfCancellationRequested();
                if (c.Type == WdContentControlType.wdContentControlRichText)
                {
                    Match m = RichTextContentControlTagPattern.Match(c.Tag);
                    if (m.Success)
                    {
                        string fieldName = m.Groups["fieldName"].Captures[0].Value;
                        int id = int.Parse(m.Groups["id"].Captures[0].Value, CultureInfo.InvariantCulture);
                        ITfsWorkItem workItem = workItemManager[id];
                        if (workItem != null)
                        {
                            if (workItem.FieldReferenceNames.Contains(fieldName))
                            {
                                this.wordDocument.PopulateRichTextContentControlWithHtml(c, workItem[fieldName].ToString());
                            }
                        }
                        else
                        {
                            this.wordDocument.PopulateRichTextContentControlWithHtml(c, string.Empty);
                        }
                    }
                }
            }
        }
    }
}
