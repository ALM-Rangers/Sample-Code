//---------------------------------------------------------------------
// <copyright file="WordDocument.cs" company="Microsoft">
//    Copyright (c) Microsoft. All rights reserved.
//    This code is licensed under the MIT License.
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF 
//    ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
//    TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
//    PARTICULAR PURPOSE AND NONINFRINGEMENT.
// </copyright>
// <summary>The WordDocument type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.Word4Tfs.Library.Model.Word
{
    using System;
    using global::System.Collections.Generic;
    using global::System.Diagnostics;
    using global::System.Drawing;
    using global::System.Drawing.Imaging;
    using global::System.IO;
    using global::System.Linq;
    using global::System.Text;
    using global::System.Xml.Linq;
    using Microsoft.Office.Core;
    using Microsoft.Office.Interop.Word;
    using Microsoft.Word4Tfs.Library.Data;

    /// <summary>
    /// The concrete Word document class.
    /// </summary>
    /// <remarks>
    /// Because we need to use Embed Interop Types to allow the add-in to work on Word 2007 this class must be in the same assembly that configures Unity, otherwise the interop types
    /// are not seen as equivalent and type resolution fails on the interop types.
    /// </remarks>
    [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Not sure why I get this one, possibly through dependencies in the Word Object Model.")]
    public class WordDocument : IWordDocument
    {
        /// <summary>
        /// The object to use for logging.
        /// </summary>
        private ILogger logger;

        /// <summary>
        /// Track whether <see cref="Dispose"/> has been called.
        /// </summary>
        private bool disposed;

        /// <summary>
        /// Manager used to manage the bookmarks.
        /// </summary>
        private BookmarkManager bookmarkManager;

        /// <summary>
        /// The underlying Word document.
        /// </summary>
        private Document document;

        /// <summary>
        /// The <see cref="XName"/> of the handle element used to store the document handle.
        /// </summary>
        private XName handleXName;

        /// <summary>
        /// Initializes a new instance of the <see cref="WordDocument"/> class.
        /// </summary>
        /// <param name="document">The underlying Word document.</param>
        /// <param name="logger">The object to be used for logging.</param>
        public WordDocument(Document document, ILogger logger)
        {
            if (document == null)
            {
                throw new ArgumentNullException("document");
            }

            this.document = document;
            this.logger = logger;

            this.bookmarkManager = new BookmarkManager(document);

            ((DocumentEvents2_Event)this.document).Close += new DocumentEvents2_CloseEventHandler(this.HandleClose);

            XNamespace nsp = Constants.HandleInformationNamespace;
            this.handleXName = nsp + "Handle";

            this.logger.Log(TraceEventType.Verbose, "Constructing WordDocument for {0}, underlying document hash is {1}", this.document.Name, this.document.GetHashCode());
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="WordDocument"/> class.
        /// </summary>
        ~WordDocument()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// Raises when the document is closed.
        /// </summary>
        public event EventHandler Close;

        /// <summary>
        /// Gets or sets the unique handle that identifies this document.
        /// </summary>
        /// <remarks>
        /// When the handle it set it is persisted in a custom XML part. If the handle is returned as <c>null</c> this means the handle has not been set on this document.
        /// </remarks>
        public Nullable<Guid> Handle
        {
            get
            {
                Nullable<Guid> ans = null;
                CustomXMLPart handleInformationPart = this.GetXmlPart(Constants.HandleInformationNamespace);
                if (handleInformationPart != null)
                {
                    ans = new Guid(XDocument.Parse(handleInformationPart.XML).Descendants(this.handleXName).Single().Value);
                }

                return ans;
            }

            set
            {
                XElement root = new XElement(this.handleXName, value.ToString());
                this.DeleteXmlPart(Constants.HandleInformationNamespace);
                this.AddXmlPart(root.ToString());
            }
        }

        /// <summary>
        /// Gets the name of the Word document.
        /// </summary>
        public string Name
        {
            get
            {
                return this.document.Name;
            }
        }

        /// <summary>
        /// Gets the paragraphs in the Word document.
        /// </summary>
        public IEnumerable<Paragraph> Paragraphs
        {
            get
            {
                foreach (Paragraph p in this.document.Paragraphs)
                {
                    yield return p;
                }
            }
        }

        /// <summary>
        /// Gets the bookmark manager associated with the document.
        /// </summary>
        public IBookmarkManager BookmarkManager
        {
            get
            {
                return this.bookmarkManager;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the document has changed since <see cref="MarkDocumentClean"/> was called.
        /// </summary>
        public bool HasChanged
        {
            get
            {
                return !this.document.Saved;
            }
        }

        /// <summary>
        /// Indicates the start of a series of updates to the document.
        /// </summary>
        public void StartUpdate()
        {
            this.document.Application.ScreenUpdating = false;
        }

        /// <summary>
        /// Indicates the end of a series of updates to the document.
        /// </summary>
        public void EndUpdate()
        {
            this.document.Application.ScreenUpdating = true;
        }

        /// <summary>
        /// Deletes all the contents of the document.
        /// </summary>
        public void DeleteAllContents()
        {
            this.document.Content.Select();
            this.document.Application.Selection.Delete();
        }

        /// <summary>
        /// Adds an XML part to the Word document.
        /// </summary>
        /// <param name="xmlData">The data to add.</param>
        public void AddXmlPart(string xmlData)
        {
            this.document.CustomXMLParts.Add(xmlData, Type.Missing);
        }

        /// <summary>
        /// Gets an XML part from the Word document.
        /// </summary>
        /// <param name="partNamespace">The namespace of the part being looked for.</param>
        /// <returns>The XML part with the corresponding namespace, <c>null</c> if the part is not found.</returns>
        public CustomXMLPart GetXmlPart(string partNamespace)
        {
            CustomXMLPart ans = null;
            if (this.document.CustomXMLParts.SelectByNamespace(partNamespace).Count > 0)
            {
                ans = this.document.CustomXMLParts[partNamespace];
            }

            return ans;
        }

        /// <summary>
        /// Deletes all occurrences of an XML part from the Word document.
        /// </summary>
        /// <param name="partNamespace">The namespace of the part to be deleted.</param>
        public void DeleteXmlPart(string partNamespace)
        {
            CustomXMLParts parts = this.document.CustomXMLParts.SelectByNamespace(partNamespace);
            int count = parts.Count;
            for (int i = count; i > 0; i--)
            {
                parts[i].Delete();
            }
        }

        /// <summary>
        /// Reads information about a building block.
        /// </summary>
        /// <param name="buildingBlock">The building block to get the information for.</param>
        /// <returns>The information about the building block.</returns>
        public BuildingBlockInfo ReadBuildingBlockInfo(BuildingBlock buildingBlock)
        {
            BuildingBlockInfo ans = null;

            if (buildingBlock == null)
            {
                throw new ArgumentNullException("buildingBlock");
            }

            this.ExecuteDocumentModifyingAction(() =>
                {
                    Range temp = this.CreateTemporaryRange();
                    try
                    {
                        temp = buildingBlock.Insert(temp);
                        ans = CreateBuildingBlockInfoFromRange(new BuildingBlockName(buildingBlock), temp);
                    }
                    finally
                    {
                        this.document.Undo(2);
                    }
                });

            return ans;
        }

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
        public BuildingBlockInfo ReadBuildingBlockInfo(BuildingBlockName name, int startPosition, int endPosition)
        {
            return this.CreateBuildingBlockInfoFromRange(name, this.document.Range(startPosition, endPosition));
        }

        /// <summary>
        /// Inserts a paragraph at the current insertion point with the chosen style.
        /// </summary>
        /// <param name="content">The content of the paragraph.</param>
        /// <param name="styleName">The style to apply to the content.</param>
        public void InsertParagraph(string content, string styleName)
        {
            string actualStyleName = (styleName == Constants.NormalStyleName) ? this.document.Styles[WdBuiltinStyle.wdStyleNormal].NameLocal : styleName;
            this.document.Application.Selection.InsertParagraphAfter();
            Paragraph p = this.document.Application.Selection.Paragraphs[this.document.Application.Selection.Paragraphs.Count];
            Range r = p.Range;
            r.MoveEnd(WdUnits.wdCharacter, -1); // to before paragraph mark
            r.Text = content;
            p.set_Style(actualStyleName);
            this.document.Application.Selection.SetRange(p.Range.End + 1, p.Range.End + 1);
        }

        /// <summary>
        /// Inserts a building block at the current insertion point, bookmarks it, and returns the list of content controls in the newly inserted content.
        /// </summary>
        /// <param name="buildingBlock">The building block to insert.</param>
        /// <param name="bookmarkName">The name of the bookmark for the new content.</param>
        /// <returns>The list of content controls in the newly inserted content.</returns>
        public IEnumerable<ContentControl> InsertBuildingBlock(BuildingBlock buildingBlock, string bookmarkName)
        {
            ////Debug.Assert(this.IsInsertable(), "Cannot insert at this location");
            this.logger.Log(TraceEventType.Verbose, "Inserting building block for bookmark {0} at current position", bookmarkName);
            if (buildingBlock == null)
            {
                throw new ArgumentNullException("buildingBlock");
            }

            List<ContentControl> ans = null;

            this.ExecuteDocumentModifyingAction(() => ans = this.InsertBuildingBlock(buildingBlock, bookmarkName, this.document.Application.Selection.Range));

            // Move selection to end of the bookmark to allow for adding more work items in order.
            Range newInsertionPoint = this.bookmarkManager[bookmarkName].Range;
            newInsertionPoint.Collapse(WdCollapseDirection.wdCollapseEnd);
            newInsertionPoint.Select();

            return ans;
        }

        ////Table table;

        /////// <summary>
        /////// Inserts a building block at the current insertion point, bookmarks it, and returns the list of content controls in the newly inserted content.
        /////// </summary>
        /////// <param name="buildingBlock">The building block to insert.</param>
        /////// <param name="bookmarkName">The name of the bookmark for the new content.</param>
        /////// <returns>The list of content controls in the newly inserted content.</returns>
        ////public IEnumerable<ContentControl> InsertBuildingBlock(BuildingBlock buildingBlock, string bookmarkName)
        ////{
        ////    List<ContentControl> ans = new List<ContentControl>();
        ////    Range r;

        ////    if (table == null)
        ////    {
        ////        table = this.document.Tables.Add(this.document.Application.Selection.Range, 1, 1);
        ////        r = table.Rows[1].Cells[1].Range;
        ////    }
        ////    else
        ////    {
        ////        r = table.Rows.Add().Cells[1].Range;
        ////    }

        ////    r.MoveEnd(WdUnits.wdCharacter, -1);
        ////    r = buildingBlock.Insert(r);

        ////    foreach (ContentControl cc in r.ContentControls)
        ////    {
        ////        ans.Add(cc);
        ////    }

        ////    return ans;
        ////}

        /// <summary>
        /// Inserts a building block at the current insertion point,bookmarks it, and returns the list of content controls in the newly inserted content.
        /// </summary>
        /// <param name="buildingBlock">The building block to insert.</param>
        /// <param name="bookmarkName">The name of the bookmark for the new content.</param>
        /// <param name="relativeBookmarkName">The name of the bookmark the new content is to be placed before.</param>
        /// <returns>The list of content controls in the newly inserted content.</returns>
        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Microsoft.Word4Tfs.Library.Model.Word.WordDocument.LogBookmark(System.String,System.String)", Justification = "Logging only, not seen by the end user.")]
        public IEnumerable<ContentControl> InsertBuildingBlockBeforeBookmark(BuildingBlock buildingBlock, string bookmarkName, string relativeBookmarkName)
        {
            this.logger.Log(TraceEventType.Verbose, "Inserting building block for bookmark {0} before bookmark {1}", bookmarkName, relativeBookmarkName);
            if (buildingBlock == null)
            {
                throw new ArgumentNullException("buildingBlock");
            }

            List<ContentControl> ans = null;

            this.ExecuteDocumentModifyingAction(() =>
                {
                    Range relative = this.bookmarkManager[relativeBookmarkName].Range;
                    relative.Collapse(WdCollapseDirection.wdCollapseStart);
                    ans = this.InsertBuildingBlock(buildingBlock, bookmarkName, relative);
                });

            return ans;
        }

        /// <summary>
        /// Inserts a building block at the current insertion point,bookmarks it, and returns the list of content controls in the newly inserted content.
        /// </summary>
        /// <param name="buildingBlock">The building block to insert.</param>
        /// <param name="bookmarkName">The name of the bookmark for the new content.</param>
        /// <param name="relativeBookmarkName">The name of the bookmark the new content is to be placed after.</param>
        /// <returns>The list of content controls in the newly inserted content.</returns>
        public IEnumerable<ContentControl> InsertBuildingBlockAfterBookmark(BuildingBlock buildingBlock, string bookmarkName, string relativeBookmarkName)
        {
            this.logger.Log(TraceEventType.Verbose, "Inserting building block for bookmark {0} after bookmark {1}", bookmarkName, relativeBookmarkName);
            if (buildingBlock == null)
            {
                throw new ArgumentNullException("buildingBlock");
            }

            List<ContentControl> ans = null;

            this.ExecuteDocumentModifyingAction(() =>
                {
                    Range relative = this.bookmarkManager[relativeBookmarkName].Range;
                    relative.Collapse(WdCollapseDirection.wdCollapseEnd);
                    ans = this.InsertBuildingBlock(buildingBlock, bookmarkName, relative);
                });

            return ans;
        }

        /// <summary>
        /// Returns all the content controls in the document.
        /// </summary>
        /// <returns>The list of all the content controls in the document.</returns>
        public IEnumerable<ContentControl> AllContentControls()
        {
            List<ContentControl> ans = new List<ContentControl>();
            foreach (ContentControl cc in this.document.ContentControls)
            {
                ans.Add(cc);
            }

            return ans;
        }

        /// <summary>
        /// Returns all the content controls in a range.
        /// </summary>
        /// <param name="range">The range to get all the content controls for.</param>
        /// <returns>The list of all the content controls in the given range.</returns>
        public IEnumerable<ContentControl> ContentControlsInRange(Range range)
        {
            if (range == null)
            {
                throw new ArgumentNullException("range");
            }

            List<ContentControl> ans = new List<ContentControl>();

            foreach (ContentControl cc in range.ContentControls)
            {
                ans.Add(cc);
            }

            foreach (Microsoft.Office.Interop.Word.Shape shape in range.ShapeRange)
            {
                if (shape.TextFrame.HasText != 0)
                {
                    foreach (ContentControl cc in shape.TextFrame.TextRange.ContentControls)
                    {
                        ans.Add(cc);
                    }
                }
            }

            return ans;
        }

        /// <summary>
        /// Maps a content control to data in an XML part.
        /// </summary>
        /// <param name="contentControl">The content control to be mapped.</param>
        /// <param name="xpath">The xpath to the data in the XML part that is to be mapped into the content control.</param>
        /// <param name="xpathPrefix">The prefix to be used in the xpath.</param>
        /// <param name="part">The Custom XML part that contains the data which is to be mapped.</param>
        public void MapContentControl(ContentControl contentControl, string xpath, string xpathPrefix, CustomXMLPart part)
        {
            if (contentControl == null)
            {
                throw new ArgumentNullException("contentControl");
            }

            contentControl.XMLMapping.SetMapping(xpath, xpathPrefix, part);
        }

        /// <summary>
        /// Puts HTML into a rich text content control.
        /// </summary>
        /// <remarks>
        /// There are only two ways to convert HTML to WordProcessingML, either pasting from the clipboard or inserting a file. Code should not use the clipboard without
        /// the user's knowledge so a temporary file is used.
        /// </remarks>
        /// <param name="contentControl">The rich text content control.</param>
        /// <param name="html">The HTML to put into the rich text content control.</param>
        public void PopulateRichTextContentControlWithHtml(ContentControl contentControl, string html)
        {
            if (contentControl == null)
            {
                throw new ArgumentNullException("contentControl");
            }

            this.ExecuteDocumentModifyingAction(() =>
                {
                    string file = null;
                    try
                    {
                        // Alternative is to use System.Windows.Clipboard (presentationcore assembly), but apps should not do this without the knowledge of the user: Useful links
                        // http://msdn.microsoft.com/en-us/library/aa767917(VS.85).aspx
                        // http://blogs.msdn.com/b/jmstall/archive/2007/01/21/sample-code-html-clipboard.aspx
                        file = Path.GetTempFileName();
                        File.WriteAllText(file, html, Encoding.UTF8);
                        contentControl.LockContents = false;
                        contentControl.Range.InsertFile(file);
                        contentControl.LockContents = true;
                    }
                    finally
                    {
                        if (!string.IsNullOrEmpty(file))
                        {
                            File.Delete(file);
                        }
                    }
                });
        }

        /// <summary>
        /// Deletes a bookmark and its content.
        /// </summary>
        /// <param name="bookmarkName">The name of the bookmark that is to be deleted.</param>
        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Microsoft.Word4Tfs.Library.Model.Word.WordDocument.LogBookmark(System.String,System.String)", Justification = "Logging only, not seen by the end user")]
        public void DeleteBookmarkAndContent(string bookmarkName)
        {
            ////this.LogBookmark("Deleting bookmark", bookmarkName);
            this.ExecuteDocumentModifyingAction(() =>
                {
                    if (this.bookmarkManager.Exists(bookmarkName))
                    {
                        Bookmark b = this.bookmarkManager[bookmarkName];
                        Range toDelete = b.Range;
                        this.bookmarkManager.Delete(b);
                        DeleteRange(toDelete);
                    }
                });
        }

        /// <summary>
        /// Moves a bookmark and its content to the location before the relative bookmark.
        /// </summary>
        /// <param name="nameOfBookmarkToMove">The name of the bookmark that is to be moved.</param>
        /// <param name="nameOfRelativeBookmark">The name of the bookmark relative to which the bookmark is to be moved.</param>
        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Microsoft.Word4Tfs.Library.Model.Word.WordDocument.LogBookmark(System.String,System.String)", Justification = "Logging only, not seen by the end user.")]
        public void MoveBookmarkAndContentToBefore(string nameOfBookmarkToMove, string nameOfRelativeBookmark)
        {
            Debug.Assert(nameOfBookmarkToMove != nameOfRelativeBookmark, "Must not move bookmark relative to itself.");
            this.logger.Log(TraceEventType.Verbose, "Moving bookmark {0} to before {1}", nameOfBookmarkToMove, nameOfRelativeBookmark);
            ////this.LogBookmark("Before move. ", nameOfBookmarkToMove);
            ////this.LogBookmark("Before move. ", nameOfRelativeBookmark);

            this.ExecuteDocumentModifyingAction(() =>
                {
                    Range r1 = this.bookmarkManager[nameOfRelativeBookmark].Range;
                    Range r2 = this.bookmarkManager[nameOfBookmarkToMove].Range;

                    Row row = RowStartingInRange(r1);
                    r1.Collapse(WdCollapseDirection.wdCollapseStart);
                    this.ReplaceRangeContent(row, r1, r2, nameOfBookmarkToMove);
                    DeleteRange(r2);
                });

            ////this.LogBookmark("After move.  ", nameOfBookmarkToMove);
            ////this.LogBookmark("After move.  ", nameOfRelativeBookmark);
        }

        /// <summary>
        /// Checks to see if the current location in the document is insertable.
        /// </summary>
        /// <remarks>
        /// If the selection is partly inside a content control then the current location is not insertable.
        /// </remarks>
        /// <returns><c>True</c> if the current location is insertable, <c>false</c> otherwise</returns>
        public bool IsInsertable()
        {
            bool insertable = true;
            Range r = this.document.Application.Selection.Range;
            insertable = this.FindContainingContentControl(r) == null;

            this.logger.Log(TraceEventType.Verbose, "Current location {0}:{1} is {2}", r.Start, r.End, insertable ? "Insertable" : "Not insertable");

            return insertable;
        }

        /// <summary>
        /// Returns the content of a paragraph.
        /// </summary>
        /// <param name="paragraph">The paragraph to get the content of.</param>
        /// <returns>The content of the paragraph.</returns>
        public string ParagraphContent(Paragraph paragraph)
        {
            if (paragraph == null)
            {
                throw new ArgumentNullException("paragraph");
            }

            string ans = paragraph.Range.Text.Replace("\r", string.Empty);
            return ans;
        }

        /// <summary>
        /// Returns the style of a paragraph.
        /// </summary>
        /// <param name="paragraph">The paragraph to get the style of.</param>
        /// <returns>The style of the paragraph.</returns>
        public string ParagraphStyle(Paragraph paragraph)
        {
            if (paragraph == null)
            {
                throw new ArgumentNullException("paragraph");
            }

            Style s = (Style)paragraph.get_Style();
            return s.NameLocal;
        }

        /// <summary>
        /// Marks the document as clean.
        /// </summary>
        public void MarkDocumentClean()
        {
            this.document.Saved = true;
        }

        /// <summary>
        /// Adds a content control to the document at the current insertion location.
        /// </summary>
        /// <param name="title">The title to give to the content control.</param>
        /// <param name="tag">The tag for the content control.</param>
        /// <param name="contentControlType">The type of content control to add.</param>
        public void AddContentControl(string title, string tag, int contentControlType)
        {
            if (this.document.Application.Selection.Range.Start < this.document.Application.Selection.Range.End)
            {
                this.document.Application.Selection.Range.Delete();
            }

            ContentControl containing = this.FindContainingContentControl(this.document.Application.Selection.Range);
            if (containing != null)
            {
                // Move to append to end of the selected content control
                this.document.Application.Selection.SetRange(containing.Range.End + 1, containing.Range.End + 1);
            }

            // If inserting a rich text content control it needs to be a block level content control, and this can only be done in an otherwise empty paragraph on its own. You also
            // can't add a paragraph, add the content control and then delete the paragraph, as it reverts to inline at that point. Once it is inline it can't ever get back to
            // being block level either.
            if ((WdContentControlType)contentControlType == WdContentControlType.wdContentControlRichText)
            {
                Range r = this.document.Application.Selection.Range;
                if (r.Paragraphs[1].Range.Start != r.Start || r.Paragraphs[1].Range.End != r.End + 1)
                {
                    if (r.Paragraphs[1].Range.Start != r.Start)
                    {
                        r.InsertParagraphAfter();
                        r.Collapse(WdCollapseDirection.wdCollapseEnd);
                    }

                    if (r.Paragraphs[1].Range.End != r.End + 1)
                    {
                        r.InsertParagraphBefore();
                        r.Collapse(WdCollapseDirection.wdCollapseStart);
                    }

                    r.Select();
                }
            }

            ContentControl cc = this.document.Application.Selection.Range.ContentControls.Add((WdContentControlType)contentControlType, Type.Missing);
            cc.Title = title;
            cc.Tag = tag;
            if ((WdContentControlType)contentControlType == WdContentControlType.wdContentControlText)
            {
                cc.MultiLine = true;
            }

            cc.SetPlaceholderText(null, null, title);
        }

        /// <summary>
        /// Returns a value indicating whether the current insertion position is at the start of the document.
        /// </summary>
        /// <returns>A value indicating whether the current insertion position is at the start of the document</returns>
        public bool IsAtStart()
        {
            return this.document.Application.Selection.Range.Start == this.document.Paragraphs[1].Range.Start;
        }

        /// <summary>
        /// Returns a value indicating whether the current insertion position is at the end of the document.
        /// </summary>
        /// <returns>A value indicating whether the current insertion position is at the end of the document</returns>
        public bool IsAtEnd()
        {
            return this.document.Application.Selection.Range.End == this.document.Paragraphs[this.document.Paragraphs.Count].Range.Start;
        }

        /// <summary>
        /// Disposes of the object and any resources it holds.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Performs actual dispose.
        /// </summary>
        /// <param name="disposing"><c>true</c> if <see cref="Dispose"/> has been called explicitly, <c>false</c> if disposal is being done by the finalizer.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    // Dispose managed resources.
                }

                // Dispose of unmanaged resources here.
                ////global::System.Runtime.InteropServices.Marshal.ReleaseComObject(this.document); // commented out because it sometimes causes exception "COM object that has been separated from its underlying RCW cannot be used."

                // Note disposing has been done.
                this.disposed = true;
            }
        }

        /// <summary>
        /// Creates a preview image for a <see cref="Range"/>.
        /// </summary>
        /// <param name="range">The <see cref="Range"/> to create a preview image for.</param>
        /// <returns>The preview image.</returns>
        private static Image GetPreviewForBuildingBlock(Range range)
        {
            Image previewImage = null;
            byte[] previewImageBytes = (byte[])range.EnhMetaFileBits;
            using (MemoryStream previewStream = new MemoryStream(previewImageBytes))
            {
                previewImage = new Metafile(previewStream);
            }

            return previewImage;
        }

        /// <summary>
        /// Deletes the temporary range.
        /// </summary>
        /// <param name="temp">The temporary range to delete.</param>
        private static void DeleteTemporaryRange(Range temp)
        {
            temp.Delete();
            Row row = RowStartingInRange(temp);
            if (row != null)
            {
                Table table = (Table)row.Parent;
                table.Delete();
            }

            temp.Delete();
        }

        /// <summary>
        /// Deletes a range, including any table rows therein.
        /// </summary>
        /// <param name="toDelete">The range to delete.</param>
        private static void DeleteRange(Range toDelete)
        {
            DeleteAllRowsInRange(toDelete);
            if (toDelete.Start != toDelete.End)
            {
                toDelete.Delete();
            }
        }

        /// <summary>
        /// Returns the row of a table in a range if the range is in a table and the range is the start of row.
        /// </summary>
        /// <param name="range">The range to find the table row in.</param>
        /// <returns>The row that starts at the beginning of <paramref name="range"/>, or <c>null</c> if none.</returns>
        private static Row RowStartingInRange(Range range)
        {
            Row ans = null;
            int rangeStart = range.Start;
            if (range.Tables.Count > 0)
            {
                Rows rows = range.Tables[1].Rows;

                // for performance, check to see if the range is at or beyond the end of the table first
                Row lastRow = rows[rows.Count];
                int lastRowStart = lastRow.Range.Start;
                if (rangeStart < lastRowStart)
                {
                    // If there are still performance problems in this area, the loop below should be turned into a binary search.
                    foreach (Row row in rows)
                    {
                        int rowStart = row.Range.Start;
                        if (rangeStart == rowStart)
                        {
                            ans = row;
                            break;
                        }
                        else if (rangeStart < rowStart)
                        {
                            break;
                        }
                    }
                }
                else if (rangeStart == lastRowStart)
                {
                    ans = lastRow;
                }
            }

            return ans;
        }

        /// <summary>
        /// Deletes all the rows in the given range.
        /// </summary>
        /// <param name="r">The range where all rows are to be deleted.</param>
        private static void DeleteAllRowsInRange(Range r)
        {
            foreach (Table table in r.Tables)
            {
                foreach (Row row in table.Rows)
                {
                    if (row.Range.InRange(r))
                    {
                        row.Delete();
                    }
                }
            }
        }

        /// <summary>
        /// Determines if a given work item is represented by an entire table.
        /// </summary>
        /// <param name="workItemRange">The range that contains the work item to be checked.</param>
        /// <returns><c>true</c> if the work item uses a whole table, <c>false</c> otherwise.</returns>
        private static bool IsTablePerWorkItem(Range workItemRange)
        {
            bool ans = false;
            Row row = RowStartingInRange(workItemRange);
            if (row != null)
            {
                Table t = (Table)row.Parent;
                ans = workItemRange.Start < t.Range.Start || workItemRange.End > t.Range.End;
            }

            return ans;
        }

        /// <summary>
        /// Returns the bookmark that starts at a given position.
        /// </summary>
        /// <param name="range">The range to check for a starting bookmark.</param>
        /// <returns>The bookmark that starts at <paramref name="position"/>, or <c>null</c> if none.</returns>
        private static Bookmark BookmarkThatStartsAt(Range range)
        {
            Bookmark ans = null;
            foreach (Bookmark b in range.Bookmarks)
            {
                if (b.Start == range.Start)
                {
                    ans = b;
                    break;
                }
            }

            return ans;
        }

        /// <summary>
        /// Creates the building block information for a range.
        /// </summary>
        /// <param name="name">The name of the building block.</param>
        /// <param name="r">The range where the building block is in the document.</param>
        /// <returns>The building block information.</returns>
        private BuildingBlockInfo CreateBuildingBlockInfoFromRange(BuildingBlockName name, Range r)
        {
            BuildingBlockInfo ans;
            IEnumerable<ContentControl> contentControls = this.ContentControlsInRange(r);
            ans = new BuildingBlockInfo(name, contentControls.Select(cc => cc.Tag).ToArray(), GetPreviewForBuildingBlock(r));
            return ans;
        }

        /// <summary>
        /// Finds the content control inside which the given range is located.
        /// </summary>
        /// <param name="range">The range for which the containing content control is to be found.</param>
        /// <returns>The containing content control, <c>null</c> if none.</returns>
        private ContentControl FindContainingContentControl(Range range)
        {
            ContentControl ans = null;
            foreach (ContentControl cc in this.document.ContentControls)
            {
                if ((range.Start >= cc.Range.Start && range.Start <= cc.Range.End)
                    ||
                    (range.End >= cc.Range.Start && range.End <= cc.Range.End))
                {
                    ans = cc;
                    break;
                }
            }

            return ans;
        }

        /// <summary>
        /// Performs a document modifying action. Sets the environment up to perform the document modification while executing the action.
        /// </summary>
        /// <param name="action">The action to be executed that modifies the document.</param>
        private void ExecuteDocumentModifyingAction(Action action)
        {
            bool saveSmartCutAndPaste = this.document.Application.Options.SmartCutPaste;
            bool saveConfirmConversions = this.document.Application.Options.ConfirmConversions;
            try
            {
                this.document.Application.Options.SmartCutPaste = false;
                this.document.Application.Options.ConfirmConversions = false;
                action();
            }
            finally
            {
                this.document.Application.Options.SmartCutPaste = saveSmartCutAndPaste;
                this.document.Application.Options.ConfirmConversions = saveConfirmConversions;
            }
        }

        /// <summary>
        /// Creates a temporary range where text can be inserted temporarily
        /// </summary>
        /// <returns>The temporary range.</returns>
        private Range CreateTemporaryRange()
        {
            Range temp = this.document.Range(0, 0);
            temp.Expand(WdUnits.wdStory);
            temp.Collapse(WdCollapseDirection.wdCollapseEnd);
            temp.InsertParagraphAfter();
            temp.MoveStart(WdUnits.wdCharacter, 1);
            return temp;
        }

        /// <summary>
        /// Replaces a range with another range from elsewhere in the document and sets the bookmark name for the replacement content.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Surrounding bookmarks are adjusted.
        /// </para>
        /// <para>
        /// When inserting in a table, row is either the row to be replaced, or the row before which the new row is to go. If the destination is a collapsed range at the end of the row it is
        /// assumed that the row is the last one in the table.
        /// </para>
        /// </remarks>
        /// <param name="row">The row (if any) present at the current destination.</param>
        /// <param name="destination">The range to be replaced.</param>
        /// <param name="source">The range with the content that is to replace the content at the destination.</param>
        /// <param name="destinationBookmarkName">The bookmark to be defined for the new content at the destination.</param>
        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Microsoft.Word4Tfs.Library.Model.Word.WordDocument.LogRange(System.String,Microsoft.Office.Interop.Word.Range)", Justification = "Logging only, not seen by the end user.")]
        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Microsoft.Word4Tfs.Library.Model.Word.WordDocument.LogBookmark(System.String,System.String)", Justification = "Logging only, not seen by the end user.")]
        private void ReplaceRangeContent(Row row, Range destination, Range source, string destinationBookmarkName)
        {
            // TODO: probably need to generalize this to work items that add more than one row to a table.
            Debug.Assert(destination.Start == destination.End, "Destination is a point not a block of text, removed old code that seemed to want to work with a destination block");
            ////this.LogRange("Replacing range at", destination);
            ////this.LogRange("With range at", source);
            this.logger.Log(TraceEventType.Verbose, "Bookmark for new content at destination is {0}", destinationBookmarkName);
            int sourceLength = source.End - source.Start;
            if (row == null)
            {
                Bookmark nextBookmark = BookmarkThatStartsAt(destination);
                ////if (nextBookmark != null)
                ////{
                ////    this.LogBookmark("Next bookmark", nextBookmark.Name);
                ////}

                destination.InsertParagraphBefore();
                Range tempParaRange = destination.Document.Range(destination.Start, destination.End);
                destination.Collapse(WdCollapseDirection.wdCollapseStart);
    
                destination.FormattedText = source;

                tempParaRange.Collapse(WdCollapseDirection.wdCollapseEnd);
                tempParaRange.Start = tempParaRange.Start - 1;
                tempParaRange.Delete();

                this.RedefineBookmark(destinationBookmarkName, destination.Start, destination.Start + sourceLength);
                if (nextBookmark != null && nextBookmark.Name != destinationBookmarkName)
                {
                    this.RedefineBookmark(nextBookmark.Name, destination.Start + sourceLength, nextBookmark.End);
                }
            }
            else if (IsTablePerWorkItem(source))
            {
                Bookmark previousBookmark = BookmarkThatStartsAt(destination);
                destination.InsertBreak(WdBreakType.wdPageBreak); // so inserts outside table even if table at start of document
                destination.FormattedText = source;
                destination.Collapse(WdCollapseDirection.wdCollapseEnd);
                destination.End = destination.Start + 1;
                destination.Delete();
                if (previousBookmark != null)
                {
                    this.RedefineBookmark(previousBookmark.Name, destination.Start, previousBookmark.End);
                }

                this.RedefineBookmark(destinationBookmarkName, destination.End - sourceLength, destination.End);
            }
            else
            {
                Table t = (Table)row.Parent;
                Row previousRow = null;
                Bookmark previousBookmark = null;
                if (row.Index > 1)
                {
                    previousRow = t.Rows[row.Index - 1];
                    previousBookmark = BookmarkThatStartsAt(previousRow.Range);
                }

                if (destination.Start == t.Range.End)
                {
                    row = t.Rows.Add();
                }
                else
                {
                    row = t.Rows.Add(row);
                }

                if (previousBookmark != null)
                {
                    this.RedefineBookmark(previousBookmark.Name, previousRow.Range.Start, previousRow.Range.End);
                }

                row.Range.FormattedText = source;
                destination = t.Rows[row.Index - 1].Range;

                this.RedefineBookmark(destinationBookmarkName, destination.Start, destination.End);
                row.Delete();
            }
        }

        /// <summary>
        /// Returns the bookmark that ends at a given position.
        /// </summary>
        /// <param name="position">The position to check for an ending bookmark.</param>
        /// <returns>The bookmark that ends at <paramref name="position"/>, or <c>null</c> if none.</returns>
        private Bookmark BookmarkThatEndsAt(int position)
        {
            Bookmark ans = null;
            foreach (Bookmark b in this.bookmarkManager.Bookmarks)
            {
                if (b.Range.End == position)
                {
                    ans = b;
                    break;
                }
            }

            return ans;
        }

        /// <summary>
        /// Redefines a bookmark.
        /// </summary>
        /// <param name="name">The name of the bookmark to redefine.</param>
        /// <param name="start">The new start position for the bookmark.</param>
        /// <param name="end">The new end position for the bookmark.</param>
        private void RedefineBookmark(string name, int start, int end)
        {
            ////this.logger.Log(TraceEventType.Verbose, "Redefining bookmark {0} to be {1}-{2}", name, start, end);

            this.bookmarkManager.ChangeOrAdd(name, this.document.Range(start, end));
        }

        /// <summary>
        /// Inserts a building block at the given insertion point, bookmarks it, and returns the list of content controls in the newly inserted content.
        /// </summary>
        /// <param name="buildingBlock">The building block to insert.</param>
        /// <param name="bookmarkName">The name of the bookmark for the new content.</param>
        /// <param name="insertionPoint">The location where the content is to be inserted.</param>
        /// <returns>The list of content controls in the newly inserted content.</returns>
        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Microsoft.Word4Tfs.Library.Model.Word.WordDocument.LogBookmark(System.String,System.String)", Justification = "Logging only, not seen by the end user.")]
        private List<ContentControl> InsertBuildingBlock(BuildingBlock buildingBlock, string bookmarkName, Range insertionPoint)
        {
            // If inserting for first time there may be some selected text, this is deleted here so that it is effectively replaced.
            if (insertionPoint.Start != insertionPoint.End)
            {
                insertionPoint.Delete(WdUnits.wdCharacter, insertionPoint.End - insertionPoint.Start);
                insertionPoint.Collapse(WdCollapseDirection.wdCollapseStart);
            }

            Range temp = this.CreateTemporaryRange();
            try
            {
                Row row = RowStartingInRange(insertionPoint);
                if (row != null && insertionPoint.Start == row.Range.End)
                {
                    Table t = (Table)row.Parent;
                    if (row.Index < t.Rows.Count)
                    {
                        row = t.Rows[row.Index + 1];
                    }
                }

                temp = buildingBlock.Insert(temp);
                this.ReplaceRangeContent(row, insertionPoint, temp, bookmarkName);
            }
            finally
            {
                DeleteTemporaryRange(temp);
            }

            ////this.LogBookmark("Added bookmark", bookmarkName);
            return this.ContentControlsInRange(this.bookmarkManager[bookmarkName].Range).ToList();
        }

        /// <summary>
        /// Handles the closing of the document.
        /// </summary>
        private void HandleClose()
        {
            if (this.Close != null)
            {
                this.Close(this, EventArgs.Empty);
            }
        }

        /////// <summary>
        /////// Logs details of a bookmark for diagnostic purposes.
        /////// </summary>
        /////// <param name="message">A prefix for the log message.</param>
        /////// <param name="bookmark">The bookmark to log details of.</param>
        ////private void LogBookmark(string message, string bookmark)
        ////{
        ////    if (this.bookmarkManager.Exists(bookmark))
        ////    {
        ////        Bookmark b = this.bookmarkManager[bookmark];
        ////        this.logger.Log(TraceEventType.Verbose, "{0}: Name {1} from {2} to {3}", message, bookmark, b.Range.Start, b.Range.End);
        ////    }
        ////    else
        ////    {
        ////        this.logger.Log(TraceEventType.Verbose, "{0}: Name {1} does not exist.", message, bookmark);
        ////    }
        ////}

        /////// <summary>
        /////// Logs all the bookmarks for diagnostic purposes only.
        /////// </summary>
        /////// <param name="message">A message to add to the logged information.</param>
        ////private void LogBookmarks(string message)
        ////{
        ////    foreach (Bookmark b in this.bookmarkManager.Bookmarks)
        ////    {
        ////        this.LogBookmark(message, b.Name);
        ////    }
        ////}

        /////// <summary>
        /////// Logs a range for diagnostic purposes.
        /////// </summary>
        /////// <param name="message">A prefix for the log message.</param>
        /////// <param name="range">The range to log details of.</param>
        ////private void LogRange(string message, Range range)
        ////{
        ////    this.logger.Log(TraceEventType.Verbose, "{0}: Range from {1} to {2}", message, range.Start, range.End);
        ////    this.logger.Log(TraceEventType.Verbose, "Range contains {0} tables", range.Tables.Count);
        ////    foreach (Table t in range.Tables)
        ////    {
        ////        this.logger.Log(TraceEventType.Verbose, "Table range from {0} to {1}, rows = {2}", t.Range.Start, t.Range.End, t.Rows.Count);
        ////    }
        ////}
    }
}
