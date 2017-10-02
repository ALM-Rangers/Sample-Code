//---------------------------------------------------------------------
// <copyright file="RenameEventArgs.cs" company="Microsoft">
//    Copyright © ALM | DevOps Ranger Contributors. All rights reserved.
//    This code is licensed under the MIT License.
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF 
//    ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
//    TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
//    PARTICULAR PURPOSE AND NONINFRINGEMENT.
// </copyright>
// <summary>The RenameEventArgs type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.Word4Tfs.Library.View
{
    using System.ComponentModel;

    /// <summary>
    /// Information relating to a rename event. 
    /// </summary>
    public class RenameEventArgs : CancelEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RenameEventArgs"/> class.
        /// </summary>
        /// <param name="oldName">The old name.</param>
        /// <param name="newName">The new name.</param>
        public RenameEventArgs(string oldName, string newName)
        {
            this.OldName = oldName;
            this.NewName = newName;
        }

        /// <summary>
        /// Gets the old name.
        /// </summary>
        public string OldName { get; private set; }

        /// <summary>
        /// Gets the new name.
        /// </summary>
        public string NewName { get; private set; }
    }
}
