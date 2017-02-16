//---------------------------------------------------------------------
// <copyright file="IWordDocument.cs" company="Microsoft">
//    Copyright (c) Microsoft. All rights reserved.
//    This code is licensed under the MIT License.
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF 
//    ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
//    TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
//    PARTICULAR PURPOSE AND NONINFRINGEMENT.
// </copyright>
// <summary>The IWordDocument type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.Word4Tfs.Library.Model.Word
{
    using System;
    using global::System.Collections.Generic;
    using Microsoft.Office.Core;
    using Microsoft.Office.Interop.Word;
    using Microsoft.Word4Tfs.Library.Data;

    /// <summary>
    /// Defines the operations on the underlying Word document.
    /// </summary>
    public interface IWordDocument : IDisposable
    {
        /// <summary>
        /// Raises when the document is closed.
        /// </summary>
        event EventHandler Close;

        /// <summary>
        /// Gets or sets the unique handle that identifies this document.
        /// </summary>
        /// <remarks>
        /// When the handle it set it is persisted in a custom XML part. If the handle is returned as <c>null</c> this means the handle has not been set on this document.
        /// </remarks>
        Nullable<Guid> Handle { get; set; }

        /// <summary>
        /// Gets the name of the Word document.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the paragraphs in the Word document.
        /// </summary>
        IEnumerable<Paragraph> Paragraphs { get; }

        /// <summary>
        /// Gets the bookmark manager associated with the document.
        /// </summary>
        IBookmarkManager BookmarkManager { get; }

        /// <summary>
        /// Gets a value indicating whether the document has changed since <see cref="MarkDocumentClean"/> was called.
        /// </summary>
        bool HasChanged { get; }

        /// <summary>
        /// Indicates the start of a series of updates to the document.
        /// </summary>
        void StartUpdate();

        /// <summary>
        /// Indicates the end of a series of updates to the document.
        /// </summary>
        void EndUpdate();

        /// <summary>
        /// Deletes all the contents of the document.
        /// </summary>
        void DeleteAllContents();

        /// <summary>
        /// Adds an XML part to the Word document.
        /// </summary>
        /// <param name="xmlData">The data to add.</param>
        void AddXmlPart(string xmlData);

        /// <summary>
        /// Gets an XML part from the Word document.
        /// </summary>
        /// <param name="partNamespace">The namespace of the part being looked for.</param>
        /// <returns>The XML part with the corresponding namespace, <c>null</c> if the part is not found.</returns>
        CustomXMLPart GetXmlPart(string partNamespace);

        /// <summary>
        /// Deletes all occurrences of an XML part from the Word document.
        /// </summary>
        /// <param name="partNamespace">The namespace of the part to be deleted.</param>
        void DeleteXmlPart(string partNamespace);

        /// <summary>
        /// Reads information about a building block.
        /// </summary>
        /// <param name="buildingBlock">The building block to get the information for.</param>
        /// <returns>The information about the building block.</returns>
        BuildingBlockInfo ReadBuildingBlockInfo(BuildingBlock buildingBlock);

        /// <summary>
        /// Reads information about a building block from a range in the document.
        /// </summary>
        /// <remarks>
        /// This method generates the information from a given range in a document and so does not need to insert the building block to get the information.
        /// </remarks>
        /// <param name="name">The name of the building block.</param>
        /// <param name="startPosition">The start of the range to get the information for.</param>
        /// <param name="endPosition">The end of the range to get the information for.</param>
        /// <returns>The information about the building block.</returns>
        BuildingBlockInfo ReadBuildingBlockInfo(BuildingBlockName name, int startPosition, int endPosition);

        /// <summary>
        /// Inserts a paragraph at the current insertion point with the chosen style.
        /// </summary>
        /// <param name="content">The content of the paragraph.</param>
        /// <param name="styleName">The style </param>
        void InsertParagraph(string content, string styleName);

        /// <summary>
        /// Inserts a building block at the current insertion point, bookmarks it, and returns the list of content controls in the newly inserted content.
        /// </summary>
        /// <param name="buildingBlock">The building block to insert.</param>
        /// <param name="bookmarkName">The name of the bookmark for the new content.</param>
        /// <returns>The list of content controls in the newly inserted content.</returns>
        IEnumerable<ContentControl> InsertBuildingBlock(BuildingBlock buildingBlock, string bookmarkName);

        /// <summary>
        /// Inserts a building block at the current insertion point, bookmarks it, and returns the list of content controls in the newly inserted content.
        /// </summary>
        /// <param name="buildingBlock">The building block to insert.</param>
        /// <param name="bookmarkName">The name of the bookmark for the new content.</param>
        /// <param name="relativeBookmarkName">The name of the bookmark the new content is to be placed before.</param>
        /// <returns>The list of content controls in the newly inserted content.</returns>
        IEnumerable<ContentControl> InsertBuildingBlockBeforeBookmark(BuildingBlock buildingBlock, string bookmarkName, string relativeBookmarkName);

        /// <summary>
        /// Inserts a building block at the current insertion point, bookmarks it, and returns the list of content controls in the newly inserted content.
        /// </summary>
        /// <param name="buildingBlock">The building block to insert.</param>
        /// <param name="bookmarkName">The name of the bookmark for the new content.</param>
        /// <param name="relativeBookmarkName">The name of the bookmark the new content is to be placed after.</param>
        /// <returns>The list of content controls in the newly inserted content.</returns>
        IEnumerable<ContentControl> InsertBuildingBlockAfterBookmark(BuildingBlock buildingBlock, string bookmarkName, string relativeBookmarkName);

        /// <summary>
        /// Returns all the content controls in the document.
        /// </summary>
        /// <returns>The list of all the content controls in the document.</returns>
        IEnumerable<ContentControl> AllContentControls();

        /// <summary>
        /// Returns all the content controls in a range.
        /// </summary>
        /// <param name="range">The range to get all the content controls for.</param>
        /// <returns>The list of all the content controls in the given range.</returns>
        IEnumerable<ContentControl> ContentControlsInRange(Range range);

        /// <summary>
        /// Maps a content control to data in an XML part.
        /// </summary>
        /// <param name="contentControl">The content control to be mapped.</param>
        /// <param name="xpath">The xpath to the data in the XML part that is to be mapped into the content control.</param>
        /// <param name="xpathPrefix">The prefix to be used in the xpath.</param>
        /// <param name="part">The Custom XML part that contains the data which is to be mapped.</param>
        void MapContentControl(ContentControl contentControl, string xpath, string xpathPrefix, CustomXMLPart part);

        /// <summary>
        /// Puts HTML into a rich text content control.
        /// </summary>
        /// <param name="contentControl">The rich text content control.</param>
        /// <param name="html">The HTML to put into the rich text content control.</param>
        void PopulateRichTextContentControlWithHtml(ContentControl contentControl, string html);

        /// <summary>
        /// Deletes a bookmark and its content.
        /// </summary>
        /// <param name="bookmarkName">The name of the bookmark that is to be deleted.</param>
        void DeleteBookmarkAndContent(string bookmarkName);

        /// <summary>
        /// Moves a bookmark and its content to the location before the relative bookmark.
        /// </summary>
        /// <param name="nameOfBookmarkToMove">The name of the bookmark that is to be moved.</param>
        /// <param name="nameOfRelativeBookmark">The name of the bookmark relative to which the bookmark is to be moved.</param>
        void MoveBookmarkAndContentToBefore(string nameOfBookmarkToMove, string nameOfRelativeBookmark);

        /// <summary>
        /// Checks to see if the current location in the document is insertable.
        /// </summary>
        /// <remarks>
        /// If the selection is partly inside a content control then the current location is not insertable.
        /// </remarks>
        /// <returns><c>True</c> if the current location is insertable, <c>false</c> otherwise</returns>
        bool IsInsertable();

        /// <summary>
        /// Returns the content of a paragraph.
        /// </summary>
        /// <param name="paragraph">The paragraph to get the content of.</param>
        /// <returns>The content of the paragraph.</returns>
        string ParagraphContent(Paragraph paragraph);

        /// <summary>
        /// Returns the style of a paragraph.
        /// </summary>
        /// <param name="paragraph">The paragraph to get the style of.</param>
        /// <returns>The style of the paragraph.</returns>
        string ParagraphStyle(Paragraph paragraph);

        /// <summary>
        /// Marks the document as clean.
        /// </summary>
        void MarkDocumentClean();

        /// <summary>
        /// Adds a content control to the document at the current insertion location.
        /// </summary>
        /// <param name="title">The title to give to the content control.</param>
        /// <param name="tag">The tag for the content control.</param>
        /// <param name="contentControlType">The type of content control to add.</param>
        void AddContentControl(string title, string tag, int contentControlType);

        /// <summary>
        /// Returns a value indicating whether the current insertion position is at the start of the document.
        /// </summary>
        /// <returns>A value indicating whether the current insertion position is at the start of the document</returns>
        bool IsAtStart();

        /// <summary>
        /// Returns a value indicating whether the current insertion position is at the end of the document.
        /// </summary>
        /// <returns>A value indicating whether the current insertion position is at the end of the document</returns>
        bool IsAtEnd();
    }
}
