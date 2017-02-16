//---------------------------------------------------------------------
// <copyright file="QueryItemEventArgs.cs" company="Microsoft">
//    Copyright (c) Microsoft. All rights reserved.
//    This code is licensed under the MIT License.
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF 
//    ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
//    TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
//    PARTICULAR PURPOSE AND NONINFRINGEMENT.
// </copyright>
// <summary>The QueryItemEventArgs type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.Word4Tfs.Library.View
{
    using System;
    using Microsoft.TeamFoundation.WorkItemTracking.Client;

    /// <summary>
    /// Information relating to an event about a query item.
    /// </summary>
    public class QueryItemEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QueryItemEventArgs"/> class.
        /// </summary>
        /// <param name="queryItem">The query item the event relates to.</param>
        public QueryItemEventArgs(QueryItem queryItem)
        {
            this.QueryItem = queryItem;
        }

        /// <summary>
        /// Gets or sets the <see cref="QueryItem"/> that the event relates to.
        /// </summary>
        public QueryItem QueryItem { get; set; }
    }
}
