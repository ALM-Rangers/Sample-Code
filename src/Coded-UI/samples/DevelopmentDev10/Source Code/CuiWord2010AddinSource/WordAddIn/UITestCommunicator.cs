//-----------------------------------------------------------------------
// <copyright file="UITestCommunicator.cs" company="Microsoft">© ALM | DevOps Ranger Contributors This source is subject to the Microsoft Permissive License. See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx. All other rights reserved.</copyright>
//-----------------------------------------------------------------------
using Microsoft.Office.Interop.Word;

namespace Microsoft.ALMRangers.UITest.Extension.Word
{
    using System;

    /// <summary>
    /// Implementation of IWordUITestCommunication which provides information
    /// to the WordExtension (loaded in the Coded UI Test process) from the
    /// WordAddin (loaded in the Word process) via .NET Remoting.
    /// </summary>
    internal class UITestCommunicator : MarshalByRefObject, IWordUITestCommunication 
    {
        /// <summary>
        /// The Word application.
        /// </summary>
        private readonly Microsoft.Office.Interop.Word.Application application;

        /// <summary>
        /// Initializes a new instance of the <see cref="UITestCommunicator"/> class.
        /// </summary>
        public UITestCommunicator()
        {
            if (ThisAddIn.Instance == null || ThisAddIn.Instance.Application == null)
            {
                throw new InvalidOperationException();
            }

            // Cache the Word application of this addin.
            this.application = ThisAddIn.Instance.Application;
        }

        /// <summary>
        /// Gets the Word UI selected/focus range at the given screen location in the active document. 
        /// </summary>
        /// <param name="x">The x-coordinate of the location.</param>
        /// <param name="y">The y-coordinate of the location.</param>
        /// <returns>The Word UI range info.</returns>
        public WordRangeInfo GetRangeFromPoint(int x, int y)
        {
            // Application.Selection.Range approach
            // Range selection = this.application.Selection.Range;
            Range selection = this.application.ActiveWindow.RangeFromPoint(x, y) as Range;
            if ((selection != null) && (ThisAddIn.Instance.Application.Selection.Range.Start != ThisAddIn.Instance.Application.Selection.Range.End))
            {
                return new WordSelectionInfo(ThisAddIn.Instance.Application.Selection.Range.Start, ThisAddIn.Instance.Application.Selection.Range.End, new WordDocumentInfo(ThisAddIn.Instance.Application.ActiveDocument.Name));
            }

            return new WordDocumentInfo(ThisAddIn.Instance.Application.ActiveDocument.Name);
        }

        /// <summary>
        /// Gets the current Word range in the document under keyboard focus, which
        /// is the character typed in. So, the range here is limited to 1 character
        /// </summary>
        /// <returns>The Word UI range info.</returns>
        public WordRangeInfo GetFocussedRange() 
        {
            // Use Word's Object Model to get the required.
            Document doc = this.application.ActiveDocument;
            if (doc != null)
            {
                Range selection = this.application.Selection.Range;

                // Range selection = this.application.ActiveDocument.Content;
                if (selection != null)
                {
                    return new WordDocumentInfo(doc.Name);
                }

                return new WordDocumentInfo(doc.Name);
            }

            return null;
        }

        /// <summary>
        /// Gets the bounding rectangle of the Word range. (Maps to Word)
        /// </summary>
        /// <param name="selectionInfo">The selection info.</param>
        /// <returns>The bounding rectangle as an array.
        /// The values from Word are in Pixels).</returns>
        public double[] GetBoundingRectangle(WordSelectionInfo selectionInfo)
        {
            // Use Words's Object Model to get the required.
            double[] rect = new double[4];
            rect[0] = rect[1] = rect[2] = rect[3] = -1;

            Range selection = this.GetSelection(selectionInfo);
            if (selection != null)
            {
                int left, top, width, height;
                selection.Document.Windows[selection.Document.Name].GetPoint(out left, out top, out width, out height, selection);

                rect[0] = left;
                rect[1] = top;
                rect[2] = width;
                rect[3] = height;
            }

            return rect;
        }

        /// <summary>
        /// Sets focus on a given selection. (in Word, the focus is where the cursor is)
        /// </summary>
        /// <param name="selectionInfo">The selection info.</param>
        public void SetFocus(WordSelectionInfo selectionInfo)
        {
            if (selectionInfo == null)
            {
                throw new ArgumentNullException("selectionInfo");
            }

            // Use Word's Object Model to get the required.
            Document doc = this.GetDocument(selectionInfo.Parent);
            if (doc != null)
            {
                doc.Activate();
                Range selection = this.GetSelection(selectionInfo);
                if (selection != null)
                {
                    selection.GoTo(doc.Application.Selection);
                }
            }
        }

        /// <summary>
        /// Scrolls a given selection into view.
        /// </summary>
        /// <param name="selectionInfo">The selection info.</param>
        public void ScrollIntoView(WordSelectionInfo selectionInfo)
        {
            if (selectionInfo == null)
            {
                throw new ArgumentNullException("selectionInfo");
            }

            // Use Word's Object Model to get the required.
            Document doc = this.GetDocument(selectionInfo.Parent);
            if (doc != null)
            {
                doc.Activate();
                this.GetBoundingRectangle(selectionInfo);
            }
        }

        /// <summary>
        /// Gets the property of a given selection.
        /// </summary>
        /// <param name="selectionInfo">The selection info.</param>
        /// <param name="propertyName">The name of the property.</param>
        /// <returns>The value of the property.</returns>
        public object GetSelectionProperty(WordSelectionInfo selectionInfo, string propertyName)
        {
            // Use Word's Object Model to get the required.
            Range selection = this.GetSelection(selectionInfo);
            if (selection == null)
            {
                throw new InvalidOperationException();
            }

            switch (propertyName)
            {
                case PropertyNames.Enabled: return true; // TODO - Needed to add support for a "locked" selection.
                case PropertyNames.DocumentName: return selection.Document.Name;
                case PropertyNames.End: return selection.End;
                case PropertyNames.Start: return selection.Start;
                case PropertyNames.Text: return selection.Text;
                case PropertyNames.TextLength: return selection.Text.Length;

                case PropertyNames.BoundingRectangle:
                    double[] selectionRect = this.GetBoundingRectangle(selectionInfo);
                    NativeMethods.RECT windowRect;
                    int top, width, height;
                    int left = top = width = height = -1;

                    if (NativeMethods.GetWindowRect(NativeMethods.FindWindowEx(0, 0, "OpusApp", 0), out windowRect))
                    {
                        left = (int)selectionRect[0];
                        top = (int)selectionRect[1];
                        width = (int)selectionRect[2];
                        height = (int)selectionRect[3];
                    }

                    return new System.Drawing.Rectangle(left, top, width, height);
                default:
                    throw new NotSupportedException();
            }
        }

        /// <summary>
        /// Sets the property of a given selection.
        /// </summary>
        /// <param name="selectionInfo">The selection info.</param>
        /// <param name="propertyName">The name of the property.</param>
        /// <param name="propertyValue">The value of the property.</param>
        public void SetSelectionProperty(WordSelectionInfo selectionInfo, string propertyName, object propertyValue)
        {
            Range selection = this.GetSelection(selectionInfo);
            if (selection == null)
            {
                throw new InvalidOperationException();
            }

            if (propertyValue == null)
            {
                throw new ArgumentNullException("propertyValue");
            }

            switch (propertyName)
            {
                case PropertyNames.Text:
                    selection.Text = propertyValue.ToString();
                    break;

                case PropertyNames.Start:
                    selection.Start = (int)propertyValue;
                    break;

                case PropertyNames.End:
                    selection.End = (int)propertyValue;
                    break;

                default:
                    throw new NotSupportedException();
            }
        }

        /// <summary>
        /// Gets the Range (selection) from the selection info.
        /// </summary>
        /// <param name="selectionInfo">The selection info.</param>
        /// <returns>The Range.</returns>
        private Range GetSelection(WordSelectionInfo selectionInfo)
        {
            Range sel = null;
            Document doc = this.GetDocument(selectionInfo.Parent);
            if (doc != null)
            {
                sel = doc.Application.Selection.Range;
            }

            return sel;
        }

        /// <summary>
        /// Gets the Document from the document info.
        /// </summary>
        /// <param name="docInfo">The document info.</param>
        /// <returns>The Document.</returns>
        private Document GetDocument(WordDocumentInfo docInfo)
        {
            return this.application.Documents[docInfo.DocName];
        }
    }
}
