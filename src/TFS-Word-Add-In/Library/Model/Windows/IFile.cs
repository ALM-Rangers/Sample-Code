//---------------------------------------------------------------------
// <copyright file="IFile.cs" company="Microsoft">
//    Copyright (c) Microsoft. All rights reserved.
//    This code is licensed under the MIT License.
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF 
//    ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
//    TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
//    PARTICULAR PURPOSE AND NONINFRINGEMENT.
// </copyright>
// <summary>The IFile type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.Word4Tfs.Library.Model.Windows
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Defines operations on files.
    /// </summary>
    public interface IFile
    {
        /// <summary>
        /// Returns a value indicating whether the file named by <paramref name="path"/> exists or not.
        /// /// </summary>
        /// <param name="path">The path to the file to check for existence.</param>
        /// <returns><c>True</c> if the file exists, <c>false</c> otherwise.</returns>
        bool Exists(string path);

        /// <summary>
        /// Copies a file from <paramref name="fromPath"/> to <paramref name="toPath"/>.
        /// </summary>
        /// <remarks>Fails silently.</remarks>
        /// <param name="fromPath">The path of the source file to copy.</param>
        /// <param name="toPath">The path to copy the source file to.</param>
        /// <param name="overwrite">Indicates if the destination file can be overwritten.</param>
        void Copy(string fromPath, string toPath, bool overwrite);

        /// <summary>
        /// Gets the last write time of the file named by <paramref name="path"/>.
        /// </summary>
        /// <param name="path">The path of the file to get the last write time for.</param>
        /// <returns>The date and time when the file was last written to, in UTC.</returns>
        DateTime GetLastWriteTimeUtc(string path);

        /// <summary>
        /// Makes the given file writable.
        /// </summary>
        /// <remarks>Fails silently.</remarks>
        /// <param name="path">The path to the file to be made writable.</param>
        void SetWritable(string path);
    }
}
