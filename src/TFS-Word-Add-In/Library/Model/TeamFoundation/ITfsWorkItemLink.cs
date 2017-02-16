//---------------------------------------------------------------------
// <copyright file="ITfsWorkItemLink.cs" company="Microsoft">
//    Copyright (c) Microsoft. All rights reserved.
//    This code is licensed under the MIT License.
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF 
//    ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
//    TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
//    PARTICULAR PURPOSE AND NONINFRINGEMENT.
// </copyright>
// <summary>The ITfsWorkItemLink type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.Word4Tfs.Library.Model.TeamFoundation
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Defines a TFS work item link
    /// </summary>
    public interface ITfsWorkItemLink
    {
        /// <summary>
        /// Gets or sets the ID of the source <see cref="WorkItem"/> of this link.
        /// </summary>
        int SourceId { get; set; }

        /// <summary>
        /// Gets or sets the ID of the target <see cref="WorkItem"/> of this link.
        /// </summary>
        int TargetId { get; set; }
    }
}
