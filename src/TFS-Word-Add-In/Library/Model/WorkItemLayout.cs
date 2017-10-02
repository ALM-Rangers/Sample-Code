//---------------------------------------------------------------------
// <copyright file="WorkItemLayout.cs" company="Microsoft">
//    Copyright © ALM | DevOps Ranger Contributors. All rights reserved.
//    This code is licensed under the MIT License.
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF 
//    ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
//    TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
//    PARTICULAR PURPOSE AND NONINFRINGEMENT.
// </copyright>
// <summary>The WorkItemLayout type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.Word4Tfs.Library.Model
{
    using global::System;
    using global::System.Globalization;
    using Microsoft.Office.Interop.Word;
    using Microsoft.Word4Tfs.Library.Data;
using Microsoft.Word4Tfs.Library.Model.Word;

    /// <summary>
    /// Represents a work item layout.
    /// </summary>
    public class WorkItemLayout : IWorkItemLayout
    {
        /// <summary>
        /// The layout information the layout is based on.
        /// </summary>
        private LayoutInformation layoutInformation;

        /// <summary>
        /// The team project template to get the building blocks from.
        /// </summary>
        private ITeamProjectTemplate teamProjectTemplate;

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkItemLayout"/> class.
        /// </summary>
        /// <param name="layoutInformation">The layout information the layout is based on.</param>
        /// <param name="teamProjectTemplate">The team project template to get the building blocks from.</param>
        public WorkItemLayout(LayoutInformation layoutInformation, ITeamProjectTemplate teamProjectTemplate)
        {
            if (layoutInformation == null)
            {
                throw new ArgumentNullException("layoutInformation");
            }

            if (teamProjectTemplate == null)
            {
                throw new ArgumentNullException("teamProjectTemplate");
            }

            this.layoutInformation = layoutInformation;
            this.teamProjectTemplate = teamProjectTemplate;
        }

        /// <summary>
        /// Chooses the building block to be used for a work item.
        /// </summary>
        /// <param name="workItemNode">The work item to choose the building block for.</param>
        /// <returns>The chosen building block.</returns>
        public BuildingBlock ChooseBuildingBlock(WorkItemTreeNode workItemNode)
        {
            if (workItemNode == null)
            {
                throw new ArgumentNullException("workItemNode");
            }

            BuildingBlock ans = null;

            if (ans == null)
            {
                ans = this.GetBuildingBlock(new BuildingBlockName(workItemNode.WorkItem.Type, workItemNode.Level));
            }

            if (ans == null)
            {
                ans = this.GetBuildingBlock(new BuildingBlockName(workItemNode.WorkItem.Type));
            }

            if (ans == null)
            {
                ans = this.GetBuildingBlock(new BuildingBlockName(workItemNode.Level));
            }

            if (ans == null)
            {
                ans = this.GetBuildingBlock(BuildingBlockName.Default);
            }

            return ans;
        }

        /// <summary>
        /// Gets the building block for a given building block name.
        /// </summary>
        /// <param name="name">The name of the building block to get.</param>
        /// <returns>The building block, or <c>null</c> if not found.</returns>
        private BuildingBlock GetBuildingBlock(BuildingBlockName name)
        {
            BuildingBlock ans = null;

            ans = this.teamProjectTemplate.GetLayoutBuildingBlock(this.layoutInformation, name);

            return ans;
        }
    }
}
