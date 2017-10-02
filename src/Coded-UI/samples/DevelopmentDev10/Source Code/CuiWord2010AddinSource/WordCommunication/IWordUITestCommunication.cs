//-----------------------------------------------------------------------
// <copyright file="IWordUITestCommunication.cs" company="ALM | DevOps Ranger Contributors"> 
// © ALM | DevOps Ranger Contributors 
 
 // MIT License 
 // 
 // Permission is hereby granted, free of charge, to any person obtaining a copy 
 // of this software and associated documentation files (the "Software"), to deal 
 // in the Software without restriction, including without limitation the rights 
 // to use, copy, modify, merge, publish, distribute, sublicense, and/or sell 
 // copies of the Software, and to permit persons to whom the Software is 
 // furnished to do so, subject to the following conditions: 
 // 
 // The above copyright notice and this permission notice shall be included in all 
 // copies or substantial portions of the Software. 
 // 
 // THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
 // IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
 // FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
 // AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
 // LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
 // OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE 
 // SOFTWARE. 
 // </copyright>
//-----------------------------------------------------------------------
namespace Microsoft.ALMRangers.UITest.Extension.Word
{
    using System;
    using System.Globalization;

    /// <summary>
    /// This interface is used by the WordExtension (loaded in the Coded UI Test process)
    /// to communicate with the WordAddin (loaded in the Word process) via .NET Remoting.
    /// </summary>
    public interface IWordUITestCommunication
    {
        /// <summary>
        /// Gets a Word document start and end position numbers for selected range within a Word document.
        /// </summary>
        /// <param name="x">The starting document position number of the selected range.</param>
        /// <param name="y">The end document position number of the selected range..</param>
        /// <returns>The Word selected range info.</returns>
        WordRangeInfo GetRangeFromPoint(int x, int y); // BH - was GetElementFromPoint
        
        /// <summary>
        /// Gets the Word UI range current under keyboard focus.
        /// </summary>
        /// <returns>The Word UI range info.</returns>
        WordRangeInfo GetFocussedRange(); // BH - was GetFocussedElement

        /// <summary>
        /// Gets the bounding rectangle of the Word Selection.
        /// </summary>
        /// <param name="selectionInfo">The selection info.</param>
        /// <returns>The bounding rectangle as an array.
        /// The values are relative to the parent window and in Points (instead of Pixels).</returns>
        double[] GetBoundingRectangle(WordSelectionInfo selectionInfo);

        /// <summary>
        /// Sets focus on a given selection.
        /// </summary>
        /// <param name="selectionInfo">The selection info.</param>
        void SetFocus(WordSelectionInfo selectionInfo);

        /// <summary>
        /// Scrolls a given selection into view.
        /// </summary>
        /// <param name="selectionInfo">The selection info.</param>
        void ScrollIntoView(WordSelectionInfo selectionInfo);

        /// <summary>
        /// Gets the property of a given selection.
        /// </summary>
        /// <param name="selectionInfo">The selection info.</param>
        /// <param name="propertyName">The name of the property.</param>
        /// <returns>The value of the property.</returns>
        object GetSelectionProperty(WordSelectionInfo selectionInfo, string propertyName);

        /// <summary>
        /// Sets the property of a given selection.
        /// </summary>
        /// <param name="selectionInfo">The selection info.</param>
        /// <param name="propertyName">The name of the property.</param>
        /// <param name="propertyValue">The value of the property.</param>
        void SetSelectionProperty(WordSelectionInfo selectionInfo, string propertyName, object propertyValue);
    }

    /// <summary>
    /// Names of various properties of an Word selection.
    /// </summary>
    public static class PropertyNames
    {
        public const string Enabled = "Enabled";
        public const string ControlType = "ControlType";
        public const string BoundingRectangle = "BoundingRectangle";
        public const string ClassName = "ClassName";
        public const string Name = "Name";
        public const string DocumentName = "DocumentName";
        public const string StartIndex = "StartIndex";
        public const string EndIndex = "EndIndex";
        public const string Start = "Start";
        public const string End = "End";
        public const string Text = "Text";
        public const string TextLength = "TextLength";

        public const string ApplicationName = "ApplicationName";
        public const string Bold = "Bold";
        public const string BoldBi = "BoldBi";
        public const string BookmarkID = "BookmarkID";
        public const string Bookmarks = "Bookmarks";
        public const string Borders = "Borders";
        public const string Case = "Case";
        public const string Selection = "Selection";
        public const string Characters = "Characters";
        public const string CharacterStyle = "CharacterStyle";
        public const string CharacterWidth = "CharacterWidth";
        public const string Columns = "Columns";
        public const string CombineCharacters = "CombineCharacters";
        public const string Comments = "Comments";
        public const string Conflicts = "Conflicts";
        public const string ContentControls = "ContentControls";
        public const string Creator = "Creator";
        public const string DisableCharacterSpaceGrid = "DisableCharacterSpaceGrid";
        public const string Document = "Document";
        public const string Duplicate = "Duplicate";
        public const string Editors = "Editors";
        public const string EmphasisMark = "EmphasisMark";

        public const string EndnoteOptions = "EndnoteOptions";
        public const string Endnotes = "Endnotes";
        public const string EnhMetaFileBits = "EnhMetaFileBits";
        public const string Fields = "Fields";
        public const string Find = "Find";
        public const string FitTextWidth = "FitTextWidth";
        public const string Font = "Font";
        public const string FootnoteOptions = "FootnoteOptions";
        public const string Footnotes = "Footnotes";
        public const string FormattedText = "FormattedText";
        public const string FormFields = "FormFields";
        public const string Frames = "Frames";
        public const string GrammarChecked = "GrammarChecked";
        public const string GrammaticalErrors = "GrammaticalErrors";
        public const string HighlightColorIndex = "HighlightColorIndex";
        public const string HorizontalInVertical = "HorizontalInVertical";
        public const string HTMLDivisions = "HTMLDivisions";
        public const string Hyperlinks = "Hyperlinks";
        public const string ID = "ID";
        public const string Information = "Information";
        public const string InlineShapes = "InlineShapes";
        public const string IsEndOfRowMark = "IsEndOfRowMark";
        public const string Italic = "Italic";
        public const string ItalicBi = "ItalicBi";
        public const string Kana = "Kana";
        public const string LanguageDetected = "LanguageDetected";
        public const string LanguageID = "LanguageID";
        public const string LanguageIDFarEast = "LanguageIDFarEast";
        public const string LanguageIDOther = "LanguageIDOther";
        public const string ListFormat = "ListFormat";
        public const string ListParagraphs = "ListParagraphs";
        public const string ListStyle = "ListStyle";
        public const string Locks = "Locks";
        public const string NextStoryRange = "NextStoryRange";
        public const string NoProofing = "NoProofing";
        public const string OMaths = "OMaths";
        public const string Orientation = "Orientation";
        public const string PageSetup = "PageSetup";
        public const string ParagraphFormat = "ParagraphFormat";
        public const string Paragraphs = "Paragraphs";
        public const string ParagraphStyle = "ParagraphStyle";
        public const string Parent = "Parent";
        public const string ParentContentControl = "ParentContentControl";
        public const string PreviousBookmarkID = "PreviousBookmarkID";
        public const string ReadabilityStatistics = "ReadabilityStatistics";
        public const string Revisions = "Revisions";
        public const string Rows = "Rows";
        public const string Scripts = "Scripts";
        public const string Sections = "Sections";
        public const string Sentences = "Sentences";
        public const string Shading = "Shading";
        public const string ShapeRange = "ShapeRange";
        public const string ShowAll = "ShowAll";
        public const string SpellingChecked = "SpellingChecked";
        public const string SpellingErrors = "SpellingErrors";

        public const string StoryLength = "StoryLength";
        public const string StoryType = "StoryType";
        public const string Subdocuments = "Subdocuments";
        public const string SynonymInfo = "SynonymInfo";
        public const string Tables = "Tables";
        public const string TableStyle = "TableStyle";

        public const string TextRetrievalMode = "TextRetrievalMode";
        public const string TopLevelTables = "TopLevelTables";
        public const string TwoLinesInOne = "TwoLinesInOne";
        public const string Underline = "Underline";
        public const string Updates = "Updates";
        public const string WordOpenXML = "WordOpenXML";
        public const string Words = "Words";
        public const string XML = "XML";
        public const string XMLNodes = "XMLNodes";
        public const string XMLParentNode = "XMLParentNode";
    }

    /// <summary>
    /// Abstract base class for all Word UI range info.
    /// </summary>
    [Serializable]
    public abstract class WordRangeInfo
    {
    }

    /// <summary>
    /// Class for Word document info.
    /// </summary>
    [Serializable]
    public class WordDocumentInfo : WordRangeInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WordDocumentInfo"/> class.
        /// </summary>
        /// <param name="docName">The name of the document.</param>
        public WordDocumentInfo(string docName)
        {
            if (docName == null)
            {
                throw new ArgumentNullException("docName");
            }

            this.DocName = docName;
        }

        /// <summary>
        /// Gets the name of the document.
        /// </summary>
        public string DocName { get; private set; }

        #region Object Overrides

        // Helpful in debugging.
        public override string ToString()
        {
            return this.DocName;
        }

        // Needed to find out if two objects are same or not.
        public override bool Equals(object obj)
        {
            WordDocumentInfo other = obj as WordDocumentInfo;
            if (other != null)
            {
                return string.Equals(this.DocName, other.DocName, StringComparison.Ordinal);
            }

            return false;
        }

        // Good practice to override this when overriding Equals.
        public override int GetHashCode()
        {
            return this.DocName.GetHashCode();
        }

        #endregion
    }

    /// <summary>
    /// Class for Word selection info.
    /// </summary>
    [Serializable]
    public class WordSelectionInfo : WordRangeInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WordSelectionInfo"/> class.
        /// </summary>
        /// <param name="startIndex">The selection start index.</param>
        /// <param name="endIndex">The selection end index.</param>
        /// <param name="parent">The parent document.</param>
        public WordSelectionInfo(int startIndex, int endIndex, WordDocumentInfo parent)
        {
            if (parent == null)
            {
                throw new ArgumentNullException("parent");
            }

            this.StartIndex = startIndex;
            this.EndIndex = endIndex;
            this.Parent = parent;
        }

        /// <summary>
        /// Gets the start index of the selection range.
        /// </summary>
        public int StartIndex { get; private set; }

        /// <summary>
        /// Gets the end index of the selection range.
        /// </summary>
        public int EndIndex { get; private set; }

        /// <summary>
        /// Gets the parent document of the selection.
        /// </summary>
        public WordDocumentInfo Parent { get; private set; }

        #region Object Overrides

        // Helpful in debugging.
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "{0}[{1}, {2}]", this.Parent, this.StartIndex, this.EndIndex);
        }

        // Needed to find out if two objects are same or not.
        public override bool Equals(object obj)
        {
            WordSelectionInfo other = obj as WordSelectionInfo;
            if (other != null)
            {
                return this.StartIndex == other.StartIndex && this.EndIndex == other.EndIndex && object.Equals(this.Parent, other.Parent);
            }

            return false;
        }

        // Good practice to override this when overriding Equals.
        public override int GetHashCode()
        {
            return this.StartIndex.GetHashCode() ^ this.EndIndex.GetHashCode() ^ this.Parent.GetHashCode();
        }

        #endregion
    }
}
