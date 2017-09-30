//---------------------------------------------------------------------
// <copyright file="NullWordTemplate.cs" company="Microsoft">
//    Copyright © ALM | DevOps Ranger Contributors. All rights reserved.
//    This code is licensed under the MIT License.
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF 
//    ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
//    TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
//    PARTICULAR PURPOSE AND NONINFRINGEMENT.
// </copyright>
// <summary>The NullWordTemplate type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.Word4Tfs.Library.Model.Word
{
    using System;
    using global::System.Collections.Generic;
    using Microsoft.Office.Interop.Word;

    /// <summary>
    /// Implements a null Word template, used when there is no actual template.
    /// </summary>
    public class NullWordTemplate : IWordTemplate
    {
        /// <summary>
        /// Enumerates the building block categories for the building block type used to store TFS layouts
        /// </summary>
        /// <returns>An enumerator for the names of the building block categories.</returns>
        public IEnumerable<string> EnumerateBuildingBlockCategories()
        {
            return new string[0];
        }

        /// <summary>
        /// Enumerates the building blocks for the particular category.
        /// </summary>
        /// <param name="categoryName">The name of the category to get the building blocks for.</param>
        /// <returns>An enumerator for the building blocks in the given category.</returns>
        public IEnumerable<BuildingBlock> EnumerateBuildingBlocksForCategory(string categoryName)
        {
            return new BuildingBlock[0];
        }

        /// <summary>
        /// Deletes the building block for a category.
        /// </summary>
        /// <param name="categoryName">The name of the category for which the building blocks are to be deleted.</param>
        public void DeleteBuildingBlocksForCategory(string categoryName)
        {
        }

        /// <summary>
        /// Creates a building block.
        /// </summary>
        /// <param name="categoryName">The category name for the building block.</param>
        /// <param name="blockName">The name of the building block.</param>
        /// <param name="startPosition">The start of the range for the building block.</param>
        /// <param name="endPosition">The end of the range for the building block.</param>
        /// <returns>The new building block.</returns>
        public BuildingBlock CreateBuildingBlock(string categoryName, string blockName, int startPosition, int endPosition)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Saves the template back to disk.
        /// </summary>
        public void Save()
        {
            throw new NotImplementedException();
        }
    }
}
