//---------------------------------------------------------------------
// <copyright file="TfsWorkItemLink.cs" company="Microsoft">
//    Copyright © ALM | DevOps Ranger Contributors. All rights reserved.
//    This code is licensed under the MIT License.
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF 
//    ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
//    TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
//    PARTICULAR PURPOSE AND NONINFRINGEMENT.
// </copyright>
// <summary>The TfsWorkItemLink type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.Word4Tfs.Library.Model.TeamFoundation
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Microsoft.TeamFoundation.WorkItemTracking.Client;

    /// <summary>
    /// Provides access to the TFS <see cref="WorkItemLink"/> class.
    /// </summary>
    public class TfsWorkItemLink : ITfsWorkItemLink
    {
        /// <summary>
        /// The TFS work item link that this object wraps.
        /// </summary>
        private WorkItemLink workItemLink;

        /// <summary>
        /// Initializes a new instance of the <see cref="TfsWorkItemLink"/> class.
        /// </summary>
        /// <param name="workItemLink">The TFS work item link that this object wraps.</param>
        public TfsWorkItemLink(WorkItemLink workItemLink)
        {
            this.workItemLink = workItemLink;
        }

        /// <summary>
        /// Gets or sets the ID of the source <see cref="WorkItem"/> of this link.
        /// </summary>
        public int SourceId
        {
            get
            {
                return this.workItemLink.SourceId;
            }

            set
            {
                this.workItemLink.SourceId = value;
            }
        }

        /// <summary>
        /// Gets or sets the ID of the target <see cref="WorkItem"/> of this link.
        /// </summary>
        public int TargetId
        {
            get
            {
                return this.workItemLink.TargetId;
            }

            set
            {
                this.workItemLink.TargetId = value;
            }
        }
    }
}
