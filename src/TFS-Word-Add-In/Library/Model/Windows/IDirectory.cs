//---------------------------------------------------------------------
// <copyright file="IDirectory.cs" company="Microsoft">
//    Copyright © ALM | DevOps Ranger Contributors. All rights reserved.
//    This code is licensed under the MIT License.
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF 
//    ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
//    TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
//    PARTICULAR PURPOSE AND NONINFRINGEMENT.
// </copyright>
// <summary>The IDirectory type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.Word4Tfs.Library.Model.Windows
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Defines operations on directories.
    /// </summary>
    public interface IDirectory
    {
        /// <summary>
        /// Creates all directories and subdirectories in the specified path.
        /// </summary>
        /// <param name="path">The directory path to create.</param>
        void CreateDirectory(string path);
    }
}
