//---------------------------------------------------------------------
// <copyright file="ITfsWorkItemStore.cs" company="Microsoft">
//    Copyright (c) Microsoft. All rights reserved.
//    This code is licensed under the MIT License.
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF 
//    ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
//    TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
//    PARTICULAR PURPOSE AND NONINFRINGEMENT.
// </copyright>
// <summary>The ITfsWorkItemStore type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.Word4Tfs.Library.Model.TeamFoundation
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Microsoft.TeamFoundation.WorkItemTracking.Client;

    /// <summary>
    /// Defines the operations to be performed on a TFS work item store
    /// </summary>
    /// <remarks>
    /// This contains a subset of the operations of a <see cref="WorkItemStore"/> so that the work item store can be mocked.
    /// </remarks>
    public interface ITfsWorkItemStore
    {
        /// <summary>
        /// Gets the set of fields that is referred to in wiql for the work items that are specified by an ID number (<paramref name="ids"/>)..
        /// </summary>
        /// <param name="ids">A collection of work item IDs.</param>
        /// <param name="wiql">The definition of fields to return.</param>
        /// <returns>A list of work items that result from the query.</returns>
        IList<ITfsWorkItem> Query(int[] ids, string wiql);
    }
}
