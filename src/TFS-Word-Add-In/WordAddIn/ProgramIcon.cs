//---------------------------------------------------------------------
// <copyright file="ProgramIcon.cs" company="Microsoft">
//    Copyright (c) Microsoft. All rights reserved.
//    This code is licensed under the MIT License.
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF 
//    ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
//    TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
//    PARTICULAR PURPOSE AND NONINFRINGEMENT.
// </copyright>
// <summary>The ProgramIcon type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.Word4Tfs.WordAddIn
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Enumeration of all the icons to be used by the program.
    /// </summary>
    public enum ProgramIcon
    {
        /// <summary>
        /// The import button on the Team ribbon.
        /// </summary>
        Import,

        /// <summary>
        /// A folder containing queries.
        /// </summary>
        QueryFolder,

        /// <summary>
        /// A flat query.
        /// </summary>
        FlatQuery,

        /// <summary>
        /// A hierarchical query.
        /// </summary>
        TreeQuery
    }
}
