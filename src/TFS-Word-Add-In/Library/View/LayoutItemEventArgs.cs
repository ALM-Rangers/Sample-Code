//---------------------------------------------------------------------
// <copyright file="LayoutItemEventArgs.cs" company="Microsoft">
//    Copyright (c) Microsoft. All rights reserved.
//    This code is licensed under the MIT License.
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF 
//    ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
//    TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
//    PARTICULAR PURPOSE AND NONINFRINGEMENT.
// </copyright>
// <summary>The LayoutItemEventArgs type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.Word4Tfs.Library.View
{
    using System.ComponentModel;
    using Microsoft.Word4Tfs.Library.Data;

    /// <summary>
    /// Information relating to an event about a layout item.
    /// </summary>
    public class LayoutItemEventArgs : CancelEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LayoutItemEventArgs"/> class.
        /// </summary>
        /// <param name="layoutItem">The layout item the event relates to.</param>
        public LayoutItemEventArgs(LayoutInformation layoutItem)
        {
            this.LayoutItem = layoutItem;
        }

        /// <summary>
        /// Gets the <see cref="LayoutInformation"/> that the event relates to.
        /// </summary>
        public LayoutInformation LayoutItem { get; private set; }
    }
}
