//-----------------------------------------------------------------------
// <copyright file="NativeMethods.cs" company="Microsoft">(c) Microsoft ALM Rangers This source is subject to the Microsoft Permissive License. See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx. All other rights reserved.</copyright>
//-----------------------------------------------------------------------
namespace Microsoft.ALMRangers.UITest.Extension.Word
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.Runtime.InteropServices;
    using System.Text;
    using Accessibility;

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
        /// Checks if the given window is Word window.
        /// </summary>
        /// <param name="windowHandle">The window handle to check.</param>
        /// <returns>True if the given window is Word window, false otherwise.</returns>
        internal static bool IsWordDocumentWindow(IntPtr windowHandle)
        {
            string className = GetClassName(windowHandle);
            return string.Equals(className, WordClassName, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Gets the window title given a window handle.
        /// </summary>
        /// <param name="windowHandle">The window handle.</param>
        /// <returns>The window title.</returns>
        internal static string GetWindowText(IntPtr windowHandle)
        {
            const int MaxWindowTextLength = 1024;
            StringBuilder textBuilder = new StringBuilder(MaxWindowTextLength);
            if (GetWindowText(windowHandle, textBuilder, textBuilder.Capacity) <= 0)
            {
                Trace.WriteLine(string.Format(CultureInfo.InvariantCulture, "GetWindowText for handle {0} Writelineed", windowHandle));
            }

            return textBuilder.ToString();
        }

        /// <summary>
        /// Returns the window classname for the given window handle.
        /// </summary>
        /// <param name="windowHandle">The window handle.</param>
        /// <returns>The window classname.</returns>
        internal static string GetClassName(IntPtr windowHandle)
        {
            if (windowHandle == IntPtr.Zero)
            {
                return string.Empty;
            }

            const int MaxClassLength = 0x100;
            StringBuilder classNameStringBuilder = new StringBuilder(MaxClassLength);
            if (GetClassName(windowHandle, classNameStringBuilder, classNameStringBuilder.Capacity) <= 0)
            {
                Trace.WriteLine(string.Format(CultureInfo.InvariantCulture, "GetClassName for handle {0} Writelineed", windowHandle));
            }

            return classNameStringBuilder.ToString();
        }

        /// <summary>
        /// Gets the IAccessible from the given window handle.
        /// </summary>
        /// <param name="windowHandle">The window handle.</param>
        /// <returns>The IAccessible object.</returns>
        internal static IAccessible AccessibleObjectFromWindow(IntPtr windowHandle)
        {
            Guid accessibleGuid = typeof(IAccessible).GUID;
            IAccessible accessible = null;

            if (AccessibleObjectFromWindow(windowHandle, 0, ref accessibleGuid, ref accessible) != 0)
            {
                Trace.WriteLine(string.Format(CultureInfo.InvariantCulture, "AccessibleObjectFromWindow for handle {0} Writelineed", windowHandle));
            }

            return accessible;
        }

        /// <summary>
        /// Gets the window handle of the window at the given screen location.
        /// </summary>
        /// <param name="pointX">The x-coordinate of the location.</param>
        /// <param name="pointY">The y-coordinate of the location.</param>
        /// <returns>The window handle.</returns>
        /// <remarks>
        /// For some reason, MSDN doc does not match with FxCop results.
        /// MSDN says POINT is 8 byte long whereas FxCop says it is 4 bytes.
        /// Making POINT 4 bytes results in crash indicating probably FxCop
        /// is wrong.  Ignoring the FxCop warning for time being.
        /// </remarks>
        [DllImport("user32.dll")]
        internal static extern IntPtr WindowFromPoint(int pointX, int pointY);

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
        /// Gets the text of the given window.
        /// </summary>
        /// <param name="windowHandle">The window handle.</param>
        /// <param name="windowText">The buffer that will receive the text.</param>
        /// <param name="maxCharCount">The maximum number of characters to copy to the buffer,
        /// including the NULL character.</param>
        /// <returns>If the function succeeds, the return value is the length, in characters,
        /// of the copied string, not including the terminating NULL character.</returns>
        /// <seealso href="http://msdn2.microsoft.com/en-us/library/ms633520.aspx"/>
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern int GetWindowText(IntPtr windowHandle, StringBuilder windowText, int maxCharCount);

        /// <summary>
        /// Gets the class name of the given window.
        /// </summary>
        /// <param name="windowHandle">The window handle.</param>
        /// <param name="classNameText">The buffer that will receive the class name.</param>
        /// <param name="maxCharCount">The maximum number of characters to copy to the buffer,
        /// including the NULL character.</param>
        /// <returns>If the function succeeds, the return value is the length, in characters,
        /// of the copied string, not including the terminating NULL character.</returns>
        /// <seealso href="http://msdn2.microsoft.com/en-us/library/ms633582.aspx"/>
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern int GetClassName(IntPtr windowHandle, StringBuilder classNameText, int maxCharCount);
        
        /// <summary>
        /// The AccessibleObjectFromWindow function retrieves the address of the specified interface to the object associated with the given window.
        /// See Documentation at http://msdn2.microsoft.com/en-us/library/ms696137.aspx
        /// </summary>
        /// <param name="windowHandle">TODO</param>
        /// <param name="dwObjectID">TODO</param>
        /// <param name="riid">TODO</param>
        /// <param name="pAcc">TODO</param>
        /// <returns>TODO</returns>
        [DllImport("oleacc.dll")]
        private static extern int AccessibleObjectFromWindow(IntPtr windowHandle, int dwObjectID, ref Guid riid, ref IAccessible pAcc);
        
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
