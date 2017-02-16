//-----------------------------------------------------------------------
// <copyright file="NativeMethods.cs" company="Microsoft">(c) Microsoft ALM Rangers This source is subject to the Microsoft Permissive License. See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx. All other rights reserved.</copyright>
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
