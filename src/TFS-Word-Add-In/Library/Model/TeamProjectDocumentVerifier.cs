//---------------------------------------------------------------------
// <copyright file="TeamProjectDocumentVerifier.cs" company="Microsoft">
//    Copyright (c) Microsoft. All rights reserved.
//    This code is licensed under the MIT License.
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF 
//    ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
//    TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
//    PARTICULAR PURPOSE AND NONINFRINGEMENT.
// </copyright>
// <summary>The TeamProjectDocumentVerifier type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.Word4Tfs.Library.Model
{
    using System;
    using global::System.Collections.Generic;
    using global::System.Globalization;
    using global::System.Linq;
    using Microsoft.Office.Interop.Word;
    using Microsoft.Practices.Unity;
    using Microsoft.Word4Tfs.Library.Data;
    using Microsoft.Word4Tfs.Library.Model.Word;

    /// <summary>
    /// Verifies the structure of a Team Project Document, reporting all errors found.
    /// </summary>
    public class TeamProjectDocumentVerifier : ITeamProjectDocumentVerifier
    {
        /// <summary>
        /// The document to be verified.
        /// </summary>
        private IWordDocument document;

        /// <summary>
        /// Initializes a new instance of the <see cref="TeamProjectDocumentVerifier"/> class.
        /// </summary>
        /// <param name="document">The document to be verified.</param>
        public TeamProjectDocumentVerifier(IWordDocument document)
        {
            this.document = document;
        }

        /// <summary>
        /// Verifies that the document is valid.
        /// </summary>
        /// <param name="expectedWorkItems">The work items that should be in the document for each query.</param>
        /// <param name="bookmarkNamingFunction">Function to compute the bookmark name from the query index and the work item id.</param>
        /// <param name="bookmarkParsingFunction">Parses a bookmark name to extract query index and work item id, returns <c>null</c> if not a work item bookmark.</param>
        /// <param name="xpathParsingFunction">Parses an xpath mapping to extract the work item id, returns <c>null</c> if not a mapping for a work item.</param>
        /// <returns>List of error messages.</returns>
        public IEnumerable<string> VerifyDocument(IEnumerable<QueryWorkItems> expectedWorkItems, Func<int, int, string> bookmarkNamingFunction, Func<string, Tuple<int, int>> bookmarkParsingFunction, Func<string, Nullable<int>> xpathParsingFunction)
        {
            List<string> errors = new List<string>();

            this.document.BookmarkManager.Refresh();

            ////this.CheckDocumentContainsBookmarksForAllWorkItems(expectedWorkItems, bookmarkNamingFunction, errors);
            this.CheckDocumentContainsWorkItemsForAllWorkItemBookmarks(expectedWorkItems, bookmarkParsingFunction, errors);
            this.CheckWorkItemsAreMappedToCorrectBookmark(bookmarkParsingFunction, xpathParsingFunction, errors);
            this.CheckWorkItemBookmarksDoNotOverlap(bookmarkParsingFunction, errors);
            return errors;
        }

        /////// <summary>
        /////// Checks if two bookmarks overlap.
        /////// </summary>
        /////// <param name="bookmark1">The first bookmark to check.</param>
        /////// <param name="bookmark2">The second bookmark to check.</param>
        /////// <returns><c>true</c> if the bookmarks overlap, <c>false</c> otherwise.</returns>
        ////private static bool BookmarksOverlap(Bookmark bookmark1, Bookmark bookmark2)
        ////{
        ////    int bookmark1Start = bookmark1.Start;
        ////    int bookmark1End = bookmark1.End;
        ////    int bookmark2Start = bookmark2.Start;
        ////    int bookmark2End = bookmark2.End;
        ////    bool ans = (bookmark1Start < bookmark2End && bookmark1End >= bookmark2End)
        ////               ||
        ////               (bookmark2Start < bookmark1End && bookmark2End >= bookmark1End);
        ////    return ans;
        ////}

        /////// <summary>
        /////// Checks that all the work items have a bookmark.
        /////// </summary>
        /////// <param name="expectedWorkItems">The work items that should be in the document for each query.</param>
        /////// <param name="bookmarkNamingFunction">Function to compute the bookmark name from the query index and the work item id.</param>
        /////// <param name="errors">The list of errors to which any errors are to be added.</param>
        ////private void CheckDocumentContainsBookmarksForAllWorkItems(IEnumerable<QueryWorkItems> expectedWorkItems, Func<int, int, string> bookmarkNamingFunction, List<string> errors)
        ////{
        ////    int queryIndex = 0;
        ////    foreach (QueryWorkItems qwi in expectedWorkItems)
        ////    {
        ////        foreach (int workItemId in qwi.WorkItemIds)
        ////        {
        ////            if (!this.document.BookmarkManager.Exists(bookmarkNamingFunction(queryIndex, workItemId)))
        ////            {
        ////                errors.Add(string.Format(CultureInfo.InvariantCulture, ModelResources.VerifierWorkItemBookmarkMissing, queryIndex, workItemId));
        ////            }
        ////        }

        ////        queryIndex++;
        ////    }
        ////}

        /// <summary>
        /// Checks that all the bookmarks which are for work items have a work item stored in the document.
        /// </summary>
        /// <param name="expectedWorkItems">The work items that should be in the document for each query.</param>
        /// <param name="bookmarkParsingFunction">Parses a bookmark name for query index and work item id.</param>
        /// <param name="errors">The list of errors to which any errors are to be added.</param>
        private void CheckDocumentContainsWorkItemsForAllWorkItemBookmarks(IEnumerable<QueryWorkItems> expectedWorkItems, Func<string, Tuple<int, int>> bookmarkParsingFunction, List<string> errors)
        {
            QueryWorkItems[] queryWorkItems = expectedWorkItems.ToArray();
            foreach (Bookmark bookmark in this.document.BookmarkManager.Bookmarks)
            {
                Tuple<int, int> queryAndWorkItem = bookmarkParsingFunction(bookmark.Name);
                if (queryAndWorkItem != null)
                {
                    bool valid = false;
                    int queryIndex = queryAndWorkItem.Item1;
                    int workItemId = queryAndWorkItem.Item2;
                    if (queryIndex >= 0 && queryIndex < queryWorkItems.Length)
                    {
                        if (queryWorkItems[queryIndex].WorkItemIds.Contains(workItemId))
                        {
                            valid = true;
                        }
                    }

                    if (!valid)
                    {
                        errors.Add(string.Format(CultureInfo.InvariantCulture, ModelResources.VerifierWorkItemContentMissing, queryIndex, workItemId));
                    }
                }
            }
        }

        /// <summary>
        /// Checks that all the bookmarks which are for work items contain only content controls for their work item. Also checks that all mapped
        /// content controls which are inside a work item bookmark.
        /// </summary>
        /// <param name="bookmarkParsingFunction">Parses a bookmark name for query index and work item id.</param>
        /// <param name="xpathParsingFunction">Parses a content control mapping for the work item id.</param>
        /// <param name="errors">The list of errors to which any errors are to be added.</param>
        private void CheckWorkItemsAreMappedToCorrectBookmark(Func<string, Tuple<int, int>> bookmarkParsingFunction, Func<string, Nullable<int>> xpathParsingFunction, List<string> errors)
        {
            Dictionary<string, ContentControl> remainingContentControls = new Dictionary<string, ContentControl>();
            foreach (ContentControl cc in this.document.AllContentControls())
            {
                remainingContentControls.Add(cc.ID, cc);
            }

            foreach (Bookmark bookmark in this.document.BookmarkManager.Bookmarks)
            {
                Tuple<int, int> queryAndWorkItem = bookmarkParsingFunction(bookmark.Name);
                if (queryAndWorkItem != null)
                {
                    int queryIndex = queryAndWorkItem.Item1;
                    int workItemId = queryAndWorkItem.Item2;

                    foreach (ContentControl cc in this.document.ContentControlsInRange(bookmark.Range))
                    {
                        remainingContentControls.Remove(cc.ID);
                        if (cc.XMLMapping.IsMapped)
                        {
                            Nullable<int> mappedWorkItemId = xpathParsingFunction(cc.XMLMapping.XPath);
                            if (mappedWorkItemId.HasValue && mappedWorkItemId.Value != workItemId)
                            {
                                errors.Add(string.Format(CultureInfo.InvariantCulture, ModelResources.VerifierContentControlInWrongBookmark, queryIndex, workItemId, mappedWorkItemId.Value));
                            }
                        }
                    }
                }
            }

            foreach (ContentControl cc in remainingContentControls.Values)
            {
                if (cc.XMLMapping.IsMapped)
                {
                    Nullable<int> id = xpathParsingFunction(cc.XMLMapping.XPath);
                    if (id.HasValue)
                    {
                        errors.Add(string.Format(CultureInfo.InvariantCulture, ModelResources.VerifierMappedContentControlNotInBookmark, id));
                    }
                }
            }
        }

        /// <summary>
        /// Checks that the bookmarks that represent work items do not overlap.
        /// </summary>
        /// <param name="bookmarkParsingFunction">Parses a bookmark name for query index and work item id.</param>
        /// <param name="errors">The list of errors to which any errors are to be added.</param>
        private void CheckWorkItemBookmarksDoNotOverlap(Func<string, Tuple<int, int>> bookmarkParsingFunction, List<string> errors)
        {
            Bookmark[] workItemBookmarks = this.document.BookmarkManager.Bookmarks.Where(bm => bookmarkParsingFunction(bm.Name) != null).OrderBy(bm => bm.Start).ToArray();
            int[] starts = workItemBookmarks.Select(b => b.Start).ToArray();
            int[] ends = workItemBookmarks.Select(b => b.End).ToArray();
            for (int outer = 0; outer < workItemBookmarks.Length; outer++)
            {
                Tuple<int, int> queryAndWorkItem = bookmarkParsingFunction(workItemBookmarks[outer].Name);
                int queryIndex = queryAndWorkItem.Item1;
                int workItemId = queryAndWorkItem.Item2;
                for (int inner = outer + 1; inner < workItemBookmarks.Length; inner++)
                {
                    if (ends[outer] > starts[inner])
                    {
                        Tuple<int, int> overlappingQueryAndWorkItem = bookmarkParsingFunction(workItemBookmarks[inner].Name);
                        int overlappingQueryIndex = overlappingQueryAndWorkItem.Item1;
                        int overlapppingWorkItemId = overlappingQueryAndWorkItem.Item2;

                        errors.Add(string.Format(CultureInfo.InvariantCulture, ModelResources.VerifierBookmarkOverlap, queryIndex, workItemId, overlappingQueryIndex, overlapppingWorkItemId));
                    }
                }
            }
        }
    }
}
