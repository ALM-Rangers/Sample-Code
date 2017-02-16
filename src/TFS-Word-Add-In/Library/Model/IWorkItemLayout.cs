//---------------------------------------------------------------------
// <copyright file="IWorkItemLayout.cs" company="Microsoft">
//    Copyright (c) Microsoft. All rights reserved.
//    This code is licensed under the MIT License.
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF 
//    ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
//    TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
//    PARTICULAR PURPOSE AND NONINFRINGEMENT.
// </copyright>
// <summary>The IWorkItemLayout type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.Word4Tfs.Library.Model
{
    using Microsoft.Office.Interop.Word;

    /// <summary>
    /// Defines the operations on a work item layout.
    /// </summary>
    public interface IWorkItemLayout
    {
        /// <summary>
        /// Chooses the building block to be used for a work item.
        /// </summary>
        /// <param name="workItemNode">The work item to choose the building block for.</param>
        /// <returns>The chosen building block.</returns>
        BuildingBlock ChooseBuildingBlock(WorkItemTreeNode workItemNode);
    }
}
