//---------------------------------------------------------------------
// <copyright file="WordTemplate.cs" company="Microsoft">
//    Copyright © ALM | DevOps Ranger Contributors. All rights reserved.
//    This code is licensed under the MIT License.
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF 
//    ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
//    TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
//    PARTICULAR PURPOSE AND NONINFRINGEMENT.
// </copyright>
// <summary>The WordTemplate type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.Word4Tfs.Library.Model.Word
{
    using global::System;
    using global::System.Collections.Generic;
    using global::System.Diagnostics;
    using global::System.Runtime.InteropServices;
    using Microsoft.Office.Interop.Word;

    /// <summary>
    /// The concrete Word template class.
    /// </summary>
    /// <remarks>
    /// Because we need to use Embed Interop Types to allow the add-in to work on Word 2007 this class must be in the same assembly that configures Unity, otherwise the interop types
    /// are not seen as equivalent and type resolution fails on the interop types.
    /// </remarks>
    public class WordTemplate : IWordTemplate
    {
        /// <summary>
        /// The building block type to be used to store TFS layouts.
        /// </summary>
        private const WdBuildingBlockTypes TfsBuildingBlockType = WdBuildingBlockTypes.wdTypeCustom1;

        /// <summary>
        /// The logger used for logging.
        /// </summary>
        private ILogger logger;

        /// <summary>
        /// The underlying Word template.
        /// </summary>
        private Template template;

        /// <summary>
        /// Initializes a new instance of the <see cref="WordTemplate"/> class.
        /// </summary>
        /// <param name="logger">The logger used for logging.</param>
        /// <param name="template">The underlying Word template.</param>
        public WordTemplate(ILogger logger, Template template)
        {
            this.logger = logger;
            this.template = template;
        }

        /// <summary>
        /// Enumerates the building block categories for the building block type used to store TFS layouts
        /// </summary>
        /// <returns>An enumerator for the names of the building block categories.</returns>
        public IEnumerable<string> EnumerateBuildingBlockCategories()
        {
            List<string> ans = new List<string>();
            Categories categories = this.GetTfsBuildingBlockCategories();
            if (categories != null)
            {
                for (int i = 1; i <= categories.Count; i++)
                {
                    Category c = categories.Item(i);
                    ans.Add(c.Name);
                }
            }

            return ans;
        }

        /// <summary>
        /// Enumerates the building blocks for the particular category.
        /// </summary>
        /// <param name="categoryName">The name of the category to get the building blocks for.</param>
        /// <returns>An enumerator for the building blocks in the given category.</returns>
        public IEnumerable<BuildingBlock> EnumerateBuildingBlocksForCategory(string categoryName)
        {
            List<BuildingBlock> ans = new List<BuildingBlock>();
            Category category = this.GetTfsBuildingBlockCategory(categoryName);
            if (category != null)
            {
                BuildingBlocks buildingBlocks = category.BuildingBlocks;
                for (int j = 1; j <= buildingBlocks.Count; j++)
                {
                    BuildingBlock bb = buildingBlocks.Item(j);
                    this.logger.Log(TraceEventType.Information, "Enumerating building block for category {3}: {0} hash {1} ID {2}", bb.Name, bb.GetHashCode(), bb.ID, categoryName);
                    ans.Add(bb);
                }
            }

            return ans;
        }

        /// <summary>
        /// Deletes the building block for a category.
        /// </summary>
        /// <param name="categoryName">The name of the category for which the building blocks are to be deleted.</param>
        public void DeleteBuildingBlocksForCategory(string categoryName)
        {
            // Note that obvious solutions such as calling EnumerateBuildingBlocksForCategory and then deleting them fails. Even cloning the enumeration results does not work. So working directly
            // with the category seems to be the best way to do this.
            this.logger.Log(TraceEventType.Information, "Deleting building blocks for category {0} from the Word template", categoryName);
            Category category = this.GetTfsBuildingBlockCategory(categoryName);
            for (int i = category.BuildingBlocks.Count; i > 0; i--)
            {
                category.BuildingBlocks.Item(i).Delete();
            }
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
            Range r = this.template.Application.ActiveDocument.Range(startPosition, endPosition);
            BuildingBlock ans = this.template.BuildingBlockEntries.Add(blockName, WdBuildingBlockTypes.wdTypeCustom1, categoryName, r, Type.Missing, WdDocPartInsertOptions.wdInsertContent);
            this.logger.Log(TraceEventType.Information, "Created building block for category {3}: {0} hash {1} ID {2}", ans.Name, ans.GetHashCode(), ans.ID, categoryName);
            return ans;
        }

        /// <summary>
        /// Saves the template back to disk.
        /// </summary>
        public void Save()
        {
            this.logger.Log(TraceEventType.Information, "Saving template at {0}", this.template.FullName);
            try
            {
                this.template.Save();
            }
            catch (COMException cex)
            {
                // Save already tells the user that it cannot save, so no point raising an error to say the same thing again.
                this.logger.Log(TraceEventType.Error, "Error saving template file: {0}", cex.Message);
            }
        }

        /// <summary>
        /// Gets the <see cref="Category"/> for a TFS building block type.
        /// </summary>
        /// <param name="categoryName">The name of the category to get.</param>
        /// <returns>The <see cref="Category"/> or <c>null</c> if not found.</returns>
        private Category GetTfsBuildingBlockCategory(string categoryName)
        {
            Category ans = null;
            Categories categories = this.GetTfsBuildingBlockCategories();
            if (categories != null)
            {
                // If the template is no longer available the category will not be there so we need to check rather than just index into the collection.
                for (int i = 1; i <= categories.Count; i++)
                {
                    Category category = categories.Item(i);
                    if (category.Name == categoryName)
                    {
                        ans = category;
                        break;
                    }
                }
            }

            return ans;
        }

        /// <summary>
        /// Gets the categories for the TFS layout building blocks
        /// </summary>
        /// <returns>The categories, or <c>null</c> if the categories are not available.</returns>
        private Categories GetTfsBuildingBlockCategories()
        {
            Categories ans = null;
            if (this.template != null)
            {
                ans = this.template.BuildingBlockTypes.Item(TfsBuildingBlockType).Categories;
            }

            return ans;
        }
    }
}
