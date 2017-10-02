//---------------------------------------------------------------------
// <copyright file="IBookmarkManager.cs" company="Microsoft">
//    Copyright © ALM | DevOps Ranger Contributors. All rights reserved.
//    This code is licensed under the MIT License.
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF 
//    ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
//    TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
//    PARTICULAR PURPOSE AND NONINFRINGEMENT.
// </copyright>
// <summary>The IBookmarkManager type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.Word4Tfs.Library.Model.Word
{
    using global::System.Collections.Generic;
    using Microsoft.Office.Interop.Word;

    /// <summary>
    /// Manages the bookmarks in a document. Caches the bookmarks for performance.
    /// </summary>
    public interface IBookmarkManager
    {
        /// <summary>
        /// Gets the list of bookmarks in the document.
        /// </summary>
        IEnumerable<Bookmark> Bookmarks { get; }

        /// <summary>
        /// Gets a bookmark given its name.
        /// </summary>
        /// <param name="name">The name of the bookmark to return.</param>
        /// <returns>The bookmark with the given name, <c>null</c> if there is no bookmark with the given name.</returns>
        Bookmark this[string name] { get; }

        /// <summary>
        /// Checks to see if a particular bookmark exists.
        /// </summary>
        /// <param name="name">The name of the bookmark to check for.</param>
        /// <returns><c>true</c> if the bookmark exists, <c>false</c> otherwise.</returns>
        bool Exists(string name);

        /// <summary>
        /// Adds a new bookmark to the document.
        /// </summary>
        /// <param name="name">The name of the bookmark.</param>
        /// <param name="range">The range of the bookmark.</param>
        /// <returns>The new bookmark.</returns>
        Bookmark Add(string name, Range range);

        /// <summary>
        /// Changes or adds a bookmark in the document.
        /// </summary>
        /// <param name="name">The name of the bookmark to change.</param>
        /// <param name="range">The new range of the bookmark.</param>
        /// <returns>The new bookmark.</returns>
        Bookmark ChangeOrAdd(string name, Range range);

        /// <summary>
        /// Deletes a bookmark.
        /// </summary>
        /// <param name="bookmark">The bookmark to delete.</param>
        void Delete(Bookmark bookmark);

        /// <summary>
        /// Deletes a bookmark.
        /// </summary>
        /// <param name="name">The name of the bookmark to delete.</param>
        void Delete(string name);

        /// <summary>
        /// Refreshes the bookmarks currently in the document.
        /// </summary>
        /// <remarks>
        /// Bookmarks that have been deleted are removed. Bookmarks that have been created are added.
        /// </remarks>
        void Refresh();
    }
}
