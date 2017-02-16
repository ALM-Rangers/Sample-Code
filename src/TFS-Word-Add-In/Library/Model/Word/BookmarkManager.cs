//---------------------------------------------------------------------
// <copyright file="BookmarkManager.cs" company="Microsoft">
//    Copyright (c) Microsoft. All rights reserved.
//    This code is licensed under the MIT License.
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF 
//    ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
//    TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
//    PARTICULAR PURPOSE AND NONINFRINGEMENT.
// </copyright>
// <summary>The BookmarkManager type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.Word4Tfs.Library.Model.Word
{
    using global::System;
    using global::System.Collections.Generic;
    using global::System.Linq;
    using global::Microsoft.Office.Interop.Word;

    /// <summary>
    /// Manages the bookmarks in a document. Caches the bookmarks for performance.
    /// </summary>
    public class BookmarkManager : IBookmarkManager
    {
        /// <summary>
        /// The document the manager is managing the bookmarks for.
        /// </summary>
        private Document document;

        /// <summary>
        /// The cache of bookmarks.
        /// </summary>
        private Dictionary<string, Bookmark> bookmarkCache = new Dictionary<string, Bookmark>();

        /// <summary>
        /// Initializes a new instance of the <see cref="BookmarkManager"/> class.
        /// </summary>
        /// <param name="document">The document for which the bookmarks are to be managed.</param>
        public BookmarkManager(Document document)
        {
            if (document == null)
            {
                throw new ArgumentNullException("document");
            }

            this.document = document;

            this.Refresh();
        }

        /// <summary>
        /// Gets the list of bookmarks in the document.
        /// </summary>
        public IEnumerable<Bookmark> Bookmarks
        {
            get
            {
                return this.bookmarkCache.Values;
            }
        }

        /// <summary>
        /// Gets a bookmark given its name.
        /// </summary>
        /// <param name="name">The name of the bookmark to return.</param>
        /// <returns>The bookmark with the given name, <c>null</c> if there is no bookmark with the given name.</returns>
        public Bookmark this[string name]
        {
            get
            {
                Bookmark ans = null;
                this.bookmarkCache.TryGetValue(name, out ans);
                return ans;
            }
        }

        /// <summary>
        /// Checks to see if a particular bookmark exists.
        /// </summary>
        /// <param name="name">The name of the bookmark to check for.</param>
        /// <returns><c>true</c> if the bookmark exists, <c>false</c> otherwise.</returns>
        public bool Exists(string name)
        {
            return this.bookmarkCache.ContainsKey(name);
        }

        /// <summary>
        /// Adds a new bookmark to the document.
        /// </summary>
        /// <param name="name">The name of the bookmark.</param>
        /// <param name="range">The range of the bookmark.</param>
        /// <returns>The new bookmark.</returns>
        public Bookmark Add(string name, Range range)
        {
            Bookmark bookmark = this.document.Bookmarks.Add(name, range);
            this.bookmarkCache.Add(name, bookmark);
            return bookmark;
        }

        /// <summary>
        /// Changes or adds a bookmark in the document.
        /// </summary>
        /// <param name="name">The name of the bookmark to change.</param>
        /// <param name="range">The new range of the bookmark.</param>
        /// <returns>The new bookmark.</returns>
        public Bookmark ChangeOrAdd(string name, Range range)
        {
            Bookmark bookmark = null;
            this.bookmarkCache.TryGetValue(name, out bookmark);
            if (bookmark != null)
            {
                bookmark.Delete();
                bookmark = this.document.Bookmarks.Add(name, range);
                this.bookmarkCache[name] = bookmark;
            }
            else
            {
                bookmark = this.Add(name, range);
            }

            return bookmark;
        }

        /// <summary>
        /// Deletes a bookmark.
        /// </summary>
        /// <param name="bookmark">The bookmark to delete.</param>
        public void Delete(Bookmark bookmark)
        {
            if (bookmark == null)
            {
                throw new ArgumentNullException("bookmark");
            }

            this.bookmarkCache.Remove(bookmark.Name);
            bookmark.Delete();
        }

        /// <summary>
        /// Deletes a bookmark.
        /// </summary>
        /// <param name="name">The name of the bookmark to delete.</param>
        public void Delete(string name)
        {
            Bookmark bookmark = this[name];
            if (bookmark != null)
            {
                this.Delete(bookmark);
            }
        }

        /// <summary>
        /// Refreshes the bookmarks currently in the document.
        /// </summary>
        /// <remarks>
        /// Bookmarks that have been deleted are removed. Bookmarks that have been created are added.
        /// </remarks>
        public void Refresh()
        {
            this.bookmarkCache.Clear();
            foreach (Bookmark b in this.document.Bookmarks)
            {
                this.bookmarkCache.Add(b.Name, b);
            }
        }
    }
}
