//---------------------------------------------------------------------
// <copyright file="DirectoryAbstraction.cs" company="Microsoft">
//    Copyright © ALM | DevOps Ranger Contributors. All rights reserved.
//    This code is licensed under the MIT License.
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF 
//    ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
//    TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
//    PARTICULAR PURPOSE AND NONINFRINGEMENT.
// </copyright>
// <summary>The DirectoryAbstraction type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.Word4Tfs.Library.Model.Windows
{
    using System.IO;

    /// <summary>
    /// Implements an abstraction for the <see cref="Directory"/> class to allow directories to be manipulated in unit tests.
    /// </summary>
    public class DirectoryAbstraction : IDirectory
    {
        /// <summary>
        /// Creates all directories and subdirectories in the specified path.
        /// </summary>
        /// <param name="path">The directory path to create.</param>
        public void CreateDirectory(string path)
        {
            Directory.CreateDirectory(path);
        }
    }
}
