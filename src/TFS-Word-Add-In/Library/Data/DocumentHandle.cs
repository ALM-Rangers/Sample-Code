//---------------------------------------------------------------------
// <copyright file="DocumentHandle.cs" company="Microsoft">
//    Copyright © ALM | DevOps Ranger Contributors. All rights reserved.
//    This code is licensed under the MIT License.
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF 
//    ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
//    TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
//    PARTICULAR PURPOSE AND NONINFRINGEMENT.
// </copyright>
// <summary>The DocumentHandle type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.Word4Tfs.Library.Data
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Represents a handle for a document.
    /// </summary>
    /// <remarks>
    /// There is no property of a Word Document that uniquely identifies it. Therefore, for connected documents a Guid is stored in the document. For documents that are not connected a Guid
    /// cannot be stored because it alters the document and it would be odd if the user opened a document and then was asked to save changes on closing it, just because this addin was present. So for those
    /// documents the document name is used as the handle. This is less reliable because we cannot identify renames.
    /// </remarks>
    public class DocumentHandle : IEquatable<DocumentHandle>
    {
        /// <summary>
        /// The guid handle for a connected document.
        /// </summary>
        private Guid guidHandle = Guid.Empty;

        /// <summary>
        /// The name handle for an unconnected document.
        /// </summary>
        private string nameHandle;

        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentHandle"/> class for a handle that represents a connected document.
        /// </summary>
        /// <param name="handle">The guid handle for the connected document.</param>
        public DocumentHandle(Guid handle)
        {
            this.guidHandle = handle;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentHandle"/> class for a handle that represents an unconnected document.
        /// </summary>
        /// <param name="handle">The name handle for the unconnected document.</param>
        public DocumentHandle(string handle)
        {
            this.nameHandle = handle;
        }

        /// <summary>
        /// Equality operator.
        /// </summary>
        /// <param name="handle1">The first document handle to compare.</param>
        /// <param name="handle2">The second document handle to compare.</param>
        /// <returns><c>True</c> if <paramref name="handle1"/> and <paramref name="handle2"/> have the same handle type and value, <c>false</c> otherwise.</returns>
        public static bool operator ==(DocumentHandle handle1, DocumentHandle handle2)
        {
            if (object.ReferenceEquals(handle1, null))
            {
                return false;
            }

            return handle1.Equals(handle2);
        }

        /// <summary>
        /// Inequality operator.
        /// </summary>
        /// <param name="handle1">The first document handle to compare.</param>
        /// <param name="handle2">The second document handle to compare.</param>
        /// <returns><c>True</c> if <paramref name="handle1"/> and <paramref name="handle2"/> do not have the same handle type and value, <c>false</c> otherwise.</returns>
        public static bool operator !=(DocumentHandle handle1, DocumentHandle handle2)
        {
            if (object.ReferenceEquals(handle1, null))
            {
                return false;
            }

            return !handle1.Equals(handle2);
        }

        /// <summary>
        /// Checks for equality.
        /// </summary>
        /// <param name="other">The comparand.</param>
        /// <returns><c>True</c> if <paramref name="other"/> has the same work item type and level, <c>false</c> otherwise.</returns>
        public bool Equals(DocumentHandle other)
        {
            if (object.ReferenceEquals(other, null))
            {
                return false;
            }

            if (this.guidHandle != Guid.Empty)
            {
                return this.guidHandle == other.guidHandle;
            }
            else
            {
                return this.nameHandle == other.nameHandle;
            }
        }

        /// <summary>
        /// Checks for equality.
        /// </summary>
        /// <param name="obj">The comparand.</param>
        /// <returns><c>True</c> if <paramref name="obj"/> has the same kind of handle and the same value, <c>false</c> otherwise.</returns>
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            DocumentHandle comparand = obj as DocumentHandle;
            if (object.ReferenceEquals(comparand, null))
            {
                return false;
            }

            return this.Equals(comparand);
        }

        /// <summary>
        /// Gets the hash code for the building block name.
        /// </summary>
        /// <returns>The hash code for the building block name.</returns>
        public override int GetHashCode()
        {
            int ans = 0;
            if (this.guidHandle != Guid.Empty)
            {
                ans = this.guidHandle.GetHashCode();
            }
            else
            {
                ans = this.nameHandle.GetHashCode();
            }

            return ans;
        }

        /// <summary>
        /// Returns the string value of the handle.
        /// </summary>
        /// <returns>The string value of the handle.</returns>
        public override string ToString()
        {
            string ans;
            if (this.guidHandle != Guid.Empty)
            {
                ans = this.guidHandle.ToString();
            }
            else
            {
                ans = this.nameHandle;
            }

            return ans;
        }
    }
}
