//-----------------------------------------------------------------------
// <copyright file="WordDocumentRange.cs" company="Microsoft">(c) Microsoft ALM Rangers This source is subject to the Microsoft Permissive License. See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx. All other rights reserved.</copyright>
//-----------------------------------------------------------------------
namespace Microsoft.ALMRangers.UITest.Extension.Word
{
    using System;
    using System.Runtime.InteropServices;
    using Microsoft.VisualStudio.TestTools.UITest.Extension;
    using Microsoft.VisualStudio.TestTools.UITesting;

    /// <summary>
    /// Class for Word document. 
    /// </summary>
    /// <remarks>Should be visible to COM.</remarks>
    [ComVisible(true)]
    public sealed class WordDocumentRange : WordRange
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WordDocumentRange"/> class.
        /// </summary>
        /// <param name="windowHandle">The window handle.</param>
        /// <param name="documentInfo">The document info.</param>
        /// <param name="manager">The technology manager.</param>
        internal WordDocumentRange(IntPtr windowHandle, WordDocumentInfo documentInfo, WordTechnologyManager manager)
            : base(windowHandle, manager)
        {
            this.DocumentInfo = documentInfo;
        }

        /// <summary>
        /// Gets the class name of this element.
        /// </summary>
        public override string ClassName
        {
            get { return "Word.Document"; }
        }

        /// <summary>
        /// Gets the universal control type of this element.
        /// </summary>
        public override string ControlTypeName
        {
            get
            {
                return ControlType.Document.Name;
            }
        }

        /// <summary>
        /// Gets the name of this element.
        /// </summary>
        public override string Name
        {
            get
            {
                return this.DocumentInfo.DocName;
            }
        }

        /// <summary>
        /// Gets the document info.
        /// </summary>
        internal WordDocumentInfo DocumentInfo { get; private set; }

        /// <summary>
        /// Gets the parent of this control in this technology.
        /// </summary>
        internal override UITechnologyElement Parent
        {
            get { return this.parent ?? (this.parent = this.techManager.GetWordRange(this.WindowHandle, null)); }
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="element">The object to compare with the current object.</param>
        /// <returns>True if the specified object is equal to the current object; otherwise, false.</returns>
        public override bool Equals(IUITechnologyElement element)
        {
            if (base.Equals(element))
            {
                WordDocumentRange otherRange = element as WordDocumentRange;
                if (otherRange != null)
                {
                    return object.Equals(this.DocumentInfo, otherRange.DocumentInfo);
                }
            }

            return false;
        }

        /// <summary>
        /// Gets the hash code for this object.
        /// .NET Design Guidelines suggests overridding this too if Equals is overridden. 
        /// </summary>
        /// <returns>The hash code.</returns>
        public override int GetHashCode()
        {
            return this.DocumentInfo.GetHashCode();
        }

        /// <summary>
        /// Gets the children of this control in this technology matching the given condition. 
        /// </summary>
        /// <param name="condition">The condition to match.</param>
        /// <returns>The enumerator for children.</returns>
        internal override System.Collections.IEnumerator GetChildren(AndCondition condition)
        {
            int start, end;
            string startString = condition.GetPropertyValue(PropertyNames.StartIndex) as string;
            string endString = condition.GetPropertyValue(PropertyNames.EndIndex) as string;
            if (int.TryParse(startString, out start) && int.TryParse(endString, out end))
            {
                UITechnologyElement selectionElement = this.techManager.GetWordRange(this.WindowHandle, new WordSelectionInfo(start, end, this.DocumentInfo));
                return new[] { selectionElement }.GetEnumerator();
            }

            return null;
        }
    }
}