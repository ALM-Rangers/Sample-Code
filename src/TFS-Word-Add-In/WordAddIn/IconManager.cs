//---------------------------------------------------------------------
// <copyright file="IconManager.cs" company="Microsoft">
//    Copyright © ALM | DevOps Ranger Contributors. All rights reserved.
//    This code is licensed under the MIT License.
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF 
//    ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
//    TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
//    PARTICULAR PURPOSE AND NONINFRINGEMENT.
// </copyright>
// <summary>The IconManager type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.Word4Tfs.WordAddIn
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using Microsoft.Word4Tfs.Library;

    /// <summary>
    /// Manages access to the icons and bitmaps in a resource DLL.
    /// </summary>
    public static class IconManager
    {
        /// <summary>
        /// Gets an image from an embedded resource.
        /// </summary>
        /// <param name="name">The name of the embedded resource containing the image.</param>
        /// <returns>The image.</returns>
        public static Image GetImage(string name)
        {
            Image ans = Bitmap.FromStream(Assembly.GetExecutingAssembly().GetManifestResourceStream("Microsoft.Word4Tfs.WordAddIn.Images." + name));
            return ans;
        }
    }
}
