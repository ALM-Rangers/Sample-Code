//-----------------------------------------------------------------------
// <copyright file="NativeMethods.cs" company="ALM | DevOps Ranger Contributors"> 
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
// This file is exactly the same as the NativeMethods.cs in the WordExtension project except 
// for the different namespace and it (only) has the method:
// [DllImport("user32.dll")]
//        public static extern int FindWindow(string strclassName, string strWindowName);

namespace Microsoft.ALMRangers.UITest.Extension.Word
{
    using System;
    using System.Runtime.InteropServices;

    internal static class NativeMethods
    {
        /// <summary>
        /// The technology name.
        /// </summary>
        internal const string WordTechnologyName = "Word";

        /// <summary>
        /// The class name of Word window
        /// </summary>
        internal const string WordClassName = "_WwG";

        /// <summary>
        /// Gets the bounding rectangle in screen co-ordinates for a window.
        /// </summary>
        /// <param name="windowHandle">The window handle.</param>
        /// <param name="lpRect">Returns the rectangle info.</param>
        /// <returns>Returns true on success</returns>
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetWindowRect(IntPtr windowHandle, out RECT lpRect);
        
        /// <summary>
        /// Gets the will return you an Hwnd given a point in Word
        /// </summary>
        /// <param name="hwndParent">Word’s main window Hwnd based on the registered classname (“OpusApp”).</param>
        /// <param name="hwndChildAfter">TODO</param>
        /// <param name="lpClassName">TODO</param>
        /// <param name="missing">TODO</param>
        /// <returns>TODO</returns>
        [DllImport("user32", ExactSpelling = false, CharSet = CharSet.Unicode)]
        internal static extern IntPtr FindWindowEx(int hwndParent, int hwndChildAfter, string lpClassName, int missing);

        /// <summary>
        /// The rectangle structure used by GetWindowRect.
        /// </summary>
        /// <remarks>
        /// The System.Drawing.Rectangle cannot be used because member variables do not match -
        /// right and bottom here vs. width and height in System.Drawing.Rectangle.
        /// </remarks>
        [StructLayout(LayoutKind.Sequential)]
        internal struct RECT
        {
            internal int left;
            internal int top;
            internal int right;
            internal int bottom;
        }
    }
}
