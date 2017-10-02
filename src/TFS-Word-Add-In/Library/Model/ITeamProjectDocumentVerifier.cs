//---------------------------------------------------------------------
// <copyright file="ITeamProjectDocumentVerifier.cs" company="Microsoft">
//    Copyright © ALM | DevOps Ranger Contributors. All rights reserved.
//    This code is licensed under the MIT License.
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF 
//    ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
//    TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
//    PARTICULAR PURPOSE AND NONINFRINGEMENT.
// </copyright>
// <summary>The ITeamProjectDocumentVerifier type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.Word4Tfs.Library.Model
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Word4Tfs.Library.Data;

    /// <summary>
    /// Verifies the structure of a Team Project Document, reporting all errors found.
    /// </summary>
    public interface ITeamProjectDocumentVerifier
    {
        /// <summary>
        /// Verifies that the document is valid.
        /// </summary>
        /// <param name="expectedWorkItems">The work items that should be in the document for each query.</param>
        /// <param name="bookmarkNamingFunction">Function to compute the bookmark name from the query index and the work item id.</param>
        /// <param name="bookmarkParsingFunction">Parses a bookmark name to extract query index and work item id, returns <c>null</c> if not a work item bookmark.</param>
        /// <param name="xpathParsingFunction">Parses an xpath mapping to extract the work item id, returns <c>null</c> if not a mapping for a work item.</param>
        /// <returns>List of error messages.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Using Funcs to return generic Tuple makes this unavoidable")]
        IEnumerable<string> VerifyDocument(IEnumerable<QueryWorkItems> expectedWorkItems, Func<int, int, string> bookmarkNamingFunction, Func<string, Tuple<int, int>> bookmarkParsingFunction, Func<string, Nullable<int>> xpathParsingFunction);
    }
}
