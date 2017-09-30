//---------------------------------------------------------------------
// <copyright file="IWordTemplate.cs" company="Microsoft">
//    Copyright © ALM | DevOps Ranger Contributors. All rights reserved.
//    This code is licensed under the MIT License.
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF 
//    ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
//    TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
//    PARTICULAR PURPOSE AND NONINFRINGEMENT.
// </copyright>
// <summary>The IWordTemplate type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.Word4Tfs.Library.Model.Word
{
    using global::System.Collections.Generic;
    using Microsoft.Office.Interop.Word;

    /// <summary>
    /// Defines the operations on a Word template.
    /// </summary>
    public interface IWordTemplate
    {
        /// <summary>
        /// Enumerates the building block categories for the building block type used to store TFS layouts
        /// </summary>
        /// <returns>An enumerator for the names of the building block categories.</returns>
        IEnumerable<string> EnumerateBuildingBlockCategories();

        /// <summary>
        /// Enumerates the building blocks for the particular category.
        /// </summary>
        /// <param name="categoryName">The name of the category to get the building blocks for.</param>
        /// <returns>An enumerator for the building blocks in the given category.</returns>
        IEnumerable<BuildingBlock> EnumerateBuildingBlocksForCategory(string categoryName);

        /// <summary>
        /// Deletes the building block for a category.
        /// </summary>
        /// <param name="categoryName">The name of the category for which the building blocks are to be deleted.</param>
        void DeleteBuildingBlocksForCategory(string categoryName);

        /// <summary>
        /// Creates a building block.
        /// </summary>
        /// <param name="categoryName">The category name for the building block.</param>
        /// <param name="blockName">The name of the building block.</param>
        /// <param name="startPosition">The start of the range for the building block.</param>
        /// <param name="endPosition">The end of the range for the building block.</param>
        /// <returns>The new building block.</returns>
        BuildingBlock CreateBuildingBlock(string categoryName, string blockName, int startPosition, int endPosition);

        /// <summary>
        /// Saves the template back to disk.
        /// </summary>
        void Save();
    }
}
