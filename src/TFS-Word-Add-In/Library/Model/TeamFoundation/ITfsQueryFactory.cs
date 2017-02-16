//---------------------------------------------------------------------
// <copyright file="ITfsQueryFactory.cs" company="Microsoft">
//    Copyright (c) Microsoft. All rights reserved.
//    This code is licensed under the MIT License.
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF 
//    ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
//    TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
//    PARTICULAR PURPOSE AND NONINFRINGEMENT.
// </copyright>
// <summary>The ITfsQueryFactory type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.Word4Tfs.Library.Model.TeamFoundation
{
    using System.Collections;

    /// <summary>
    /// Defines the operations for a factory to create <see cref="ITfsQuery"/> objects.
    /// </summary>
    public interface ITfsQueryFactory
    {
        /// <summary>
        /// Creates a new instance of the <see cref="TfsQuery"/> class.
        /// </summary>
        /// <param name="wiql">The query string to execute.</param>
        /// <returns>The new query.</returns>
        ITfsQuery CreateTfsQuery(string wiql);

        /// <summary>
        /// Creates a new instance of the <see cref="TfsQuery"/> class.
        /// </summary>
        /// <param name="wiql">The query string to execute.</param>
        /// <param name="context">A dictionary of macros and values.</param>
        /// <returns>The new query.</returns>
        ITfsQuery CreateTfsQuery(string wiql, IDictionary context);

        /// <summary>
        /// Creates a new instance of the <see cref="TfsQuery"/> class.
        /// </summary>
        /// <param name="wiql">The query string to execute.</param>
        /// <param name="ids">An array of <see cref="WorkItem"/> IDs.</param>
        /// <returns>The new query.</returns>
        ITfsQuery CreateTfsQuery(string wiql, int[] ids);
    }
}
