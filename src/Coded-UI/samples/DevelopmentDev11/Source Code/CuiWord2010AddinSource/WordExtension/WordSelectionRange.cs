//-----------------------------------------------------------------------
// <copyright file="WordSelectionRange.cs" company="ALM | DevOps Ranger Contributors"> 
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
    using System.Runtime.InteropServices;
    using Microsoft.VisualStudio.TestTools.UITest.Extension;
    using Microsoft.VisualStudio.TestTools.UITesting;

    /// <summary>
    /// Class for Word selection.
    /// </summary>
    /// <remarks>Should be visible to COM.</remarks>
    [ComVisible(true)]
    public sealed class WordSelectionRange : WordRange
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WordSelectionRange"/> class.
        /// </summary>
        /// <param name="windowHandle">The window handle.</param>
        /// <param name="selectionInfo">The selection info.</param>
        /// <param name="manager">The technology manager.</param>
        internal WordSelectionRange(IntPtr windowHandle, WordSelectionInfo selectionInfo, WordTechnologyManager manager)
            : base(windowHandle, manager)
        {
            this.SelectionInfo = selectionInfo;
        }

        /// <summary>
        /// Gets a QueryId that can be used to uniquely identify/find this element.
        /// In some cases, like TreeItem, the QueryIds might contain the entire element hierarchy
        /// but in most cases it will contain only important ancestors of the element.
        /// The technology manager needs to choose which ancestor to capture in the hierarchy
        /// by appropriately setting the QueryId.Ancestor property of each element.
        /// The APIs in condition classes like AndCondition.ToString() and AndCondition.Parse()
        /// may be used to convert from this class to string or vice-versa.
        /// </summary>
        public override IQueryElement QueryId
        {
            // For Word selection, ControlType with StartIndex and EndIndex is unique identifier.
            get
            {
                return this.elementQueryId ?? (this.elementQueryId = new QueryElement
                {
                    Condition = new AndCondition(
                        new PropertyCondition(PropertyNames.ControlType, this.ControlTypeName),
                        new PropertyCondition(PropertyNames.StartIndex, this.SelectionInfo.StartIndex),
                        new PropertyCondition(PropertyNames.EndIndex, this.SelectionInfo.EndIndex)),
                    Ancestor = this.Parent
                });
            }
        }

        /// <summary>
        /// Gets the 0-based position in the parent range's collection.
        /// </summary>
        public override int ChildIndex
        {
            get { return this.SelectionInfo.StartIndex * this.SelectionInfo.EndIndex; }
        }

        /// <summary>
        /// Gets the class name of this range.
        /// </summary>
        public override string ClassName
        {
            get { return "Word.Selection"; }
        }

        /// <summary>
        /// Gets the universal control type of this range.
        /// </summary>
        public override string ControlTypeName
        {
            get
            {
                return ControlType.Document.Name;
            }
        }

        /// <summary>
        /// Gets whether this range is a leaf node (i.e. does not have any children) or not.
        /// </summary>
        public override bool IsLeafNode
        {
            get { return true; }
        }

        /// <summary>
        /// Gets the name of this range.
        /// </summary>
        public override string Name
        {
            get
            {
                return this.SelectionInfo.ToString();
            }
        }

        /// <summary>
        /// Gets the underlying native technology element (like IAccessible) corresponding this range.
        /// </summary>
        public override object NativeElement
        {
            // Here WindowHandle with SelectionInfo uniquely identifies underlying Word control.
            get { return new object[] { this.WindowHandle, this.SelectionInfo, this }; }
        }

        /// <summary>
        /// Gets the selection info.
        /// </summary>
        internal WordSelectionInfo SelectionInfo { get; private set; }

        /// <summary>
        /// Gets the parent of this control in this technology.
        /// </summary>
        internal override UITechnologyElement Parent
        {
            get { return this.parent ?? (this.parent = this.techManager.GetWordRange(this.WindowHandle, this.SelectionInfo.Parent)); }
        }

        /// <summary>
        /// Gets the coordinates of the rectangle that completely encloses this range.
        /// </summary>
        /// <remarks>This is in screen coordinates and never cached.</remarks>
        public override void GetBoundingRectangle(out int left, out int top, out int width, out int height)
        {
            left = top = width = height = -1;

            NativeMethods.RECT windowRect;
            if (NativeMethods.GetWindowRect(NativeMethods.FindWindowEx(0, 0, "OpusApp", 0), out windowRect))
            {
                double[] selectionRect = WordCommunicator.Instance.GetBoundingRectangle(this.SelectionInfo);

                // The Word OM method GetPoint provides pixels
                left = (int)selectionRect[0];
                top = (int)selectionRect[1];
                width = (int)selectionRect[2];
                height = (int)selectionRect[3]; 
            }
        }

        /// <summary>
        /// Gets the value for the specified property for this range.
        /// </summary>
        /// <param name="propertyName">The name of the property.</param>
        /// <returns>The value of the property.</returns>
        public override object GetPropertyValue(string propertyName)
        {
            // At the very least, all the properties used in QueryId should be supported here.
            if (string.Equals(PropertyNames.DocumentName, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                return this.SelectionInfo.Parent.DocName;
            }

            if (string.Equals(PropertyNames.StartIndex, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                return this.SelectionInfo.StartIndex;
            }

            return string.Equals(PropertyNames.EndIndex, propertyName, StringComparison.OrdinalIgnoreCase) ? this.SelectionInfo.EndIndex : base.GetPropertyValue(propertyName);
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
                WordSelectionRange otherElement = element as WordSelectionRange;
                if (otherElement != null)
                {
                    return this.SelectionInfo.Equals(otherElement.SelectionInfo);
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
            return this.SelectionInfo.GetHashCode();
        }

        /// <summary>
        /// Sets the focus on this element.
        /// </summary>
        public override void SetFocus()
        {
            // Use Word to set focus (activate) the selection.
            WordCommunicator.Instance.SetFocus(this.SelectionInfo);
        }

        /// <summary>
        /// Scrolls this element into view.
        /// If the technology manager does not support scrolling multiple containers, 
        /// then the outPointX and outPointY should be returned as -1, -1.
        /// </summary>
        /// <param name="pointX">The relative x coordinate of point to make visible.</param>
        /// <param name="pointY">The relative y coordinate of point to make visible.</param>
        /// <param name="outpointX">The relative x coordinate of the point with respect to top most container after scrolling.</param>
        /// <param name="outpointY">The relative y coordinate of the point with respect to top most container after scrolling.</param>
        /// <seealso cref="UITechnologyManagerProperty.ContainerScrollingSupported"/>
        public override void EnsureVisibleByScrolling(int pointX, int pointY, ref int outpointX, ref int outpointY)
        {
            // Use Word to get the selection into view.
            WordCommunicator.Instance.ScrollIntoView(this.SelectionInfo);
        }

        /// <summary>
        /// Gets the children of this control in this technology matching the given condition. 
        /// </summary>
        /// <param name="condition">The condition to match.</param>
        /// <returns>The enumerator for children.</returns>
        internal override System.Collections.IEnumerator GetChildren(AndCondition condition)
        {
            // Selection has no child.
            return new ChildrenEnumerator();
        }

        public class ChildrenEnumerator : System.Collections.IEnumerator, IDisposable
        {
            public ChildrenEnumerator()
            {
                this.Reset();
            }

            public object Current
            {
                get
                {
                    return this.Current;
                }
            }

            public bool MoveNext()
            {
                return false;
            }

            public void Reset()
            {
            }

            public void Dispose()
            {
            }
        }
    }
}
