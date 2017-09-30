//---------------------------------------------------------------------
// <copyright file="TeamProjectTemplate.cs" company="Microsoft">
//    Copyright © ALM | DevOps Ranger Contributors. All rights reserved.
//    This code is licensed under the MIT License.
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF 
//    ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
//    TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
//    PARTICULAR PURPOSE AND NONINFRINGEMENT.
// </copyright>
// <summary>The TeamProjectTemplate type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.Word4Tfs.Library.Model
{
    using global::System;
    using global::System.Collections.Generic;
    using global::System.Diagnostics;
    using global::System.Globalization;
    using global::System.Linq;
    using global::System.Text.RegularExpressions;
    using Microsoft.Office.Interop.Word;
    using Microsoft.Word4Tfs.Library.Data;
    using Microsoft.Word4Tfs.Library.Model.Word;

    /// <summary>
    /// The model that represents a template that documents have available for providing layouts.
    /// </summary>
    public class TeamProjectTemplate : ITeamProjectTemplate
    {
        /// <summary>
        /// The prefix used on categories to mark those used for layouts.
        /// </summary>
        private const string CategoryPrefix = "TFS_";

        /// <summary>
        /// The pattern used to identify and extract layouts from category names.
        /// </summary>
        private static readonly Regex LayoutCategoryPattern = new Regex(CategoryPrefix + @"(?<layoutName>.+)", RegexOptions.None);

        /// <summary>
        /// The Word template that underlies the team project template.
        /// </summary>
        private readonly IWordTemplate wordTemplate;

        /// <summary>
        /// The Word application.
        /// </summary>
        private readonly IWordApplication application;

        /// <summary>
        /// The logger used for logging.
        /// </summary>
        private ILogger logger;

        /// <summary>
        /// The collection of layouts.
        /// </summary>
        private List<LayoutInformation> layouts;

        /// <summary>
        /// Caches the building blocks for the layouts to speed up retrieval. Retrieved by layout name. The  building blocks are also stored in a named dictionary.
        /// </summary>
        private Dictionary<string, Dictionary<string, BuildingBlock>> buildingBlockCache = new Dictionary<string, Dictionary<string, BuildingBlock>>();

        // TODO: Not happy passing a word app or doc to this constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="TeamProjectTemplate"/> class.
        /// </summary>
        /// <param name="logger">The logger used for logging.</param>
        /// <param name="wordTemplate">The Word template that underlies the team project template.</param>
        /// <param name="application">The Word application.</param>
        public TeamProjectTemplate(ILogger logger, IWordTemplate wordTemplate, IWordApplication application)
        {
            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }

            this.logger = logger;
            this.wordTemplate = wordTemplate;
            this.application = application;
            this.logger.Log(TraceEventType.Verbose, "Constructing template");
        }

        /// <summary>
        /// Gets a value indicating whether the document is importable.
        /// </summary>
        public bool IsImportable
        {
            get
            {
                if (this.Layouts == null)
                {
                    throw new InvalidOperationException(ModelResources.LoadTemplateFirst);
                }

                bool ans = this.Layouts.Count() > 0;
                return ans;
            }
        }

        /// <summary>
        /// Gets the list of currently available layouts
        /// </summary>
        /// <remarks>
        /// This call does not require an active document as it accesses the currently available templates.
        /// </remarks>
        public IEnumerable<LayoutInformation> Layouts
        {
            get
            {
                this.logger.Log(TraceEventType.Verbose, "Getting layouts from template.");
                if (this.layouts == null)
                {
                    throw new InvalidOperationException(ModelResources.LoadTemplateFirst);
                }

                return this.layouts.ToList(); // clone so can enumerate to delete the layouts.
            }
        }

        /// <summary>
        /// Loads the template.
        /// </summary>
        public void Load()
        {
            this.logger.Log(TraceEventType.Verbose, "Loading template.");
            this.buildingBlockCache.Clear();
            IWordDocument tempDocument = this.GetDocument();
            tempDocument.StartUpdate();
            try
            {
                this.layouts = this.wordTemplate.EnumerateBuildingBlockCategories()
                                                                      .Where(category => this.CategoryIsLayout(category))
                                                                      .Select(category => this.GetLayoutForCategory(tempDocument, category)).ToList();
            }
            finally
            {
                tempDocument.EndUpdate();
            }
        }

        /// <summary>
        /// Saves changes to the template.
        /// </summary>
        public void Save()
        {
            this.wordTemplate.Save();
        }

        /// <summary>
        /// Deletes a layout from the template.
        /// </summary>
        /// <param name="layoutName">The name of the layout to be deleted.</param>
        public void DeleteLayout(string layoutName)
        {
            this.logger.Log(TraceEventType.Information, "Deleting layout {0}", layoutName);
            LayoutInformation li = this.layouts.Where(l => l.Name == layoutName).SingleOrDefault();
            if (li != null)
            {
                this.wordTemplate.DeleteBuildingBlocksForCategory(CategoryPrefix + layoutName);
                this.layouts.Remove(li);

                // The building block cache must be cleared because after deleting building blocks, references to existing building blocks can become invalid.
                this.buildingBlockCache.Clear();
            }
        }

        /// <summary>
        /// Creates a layout.
        /// </summary>
        /// <param name="layoutName">The name of the layout being created.</param>
        /// <param name="blocks">List of building block definitions for the building blocks to be created that make up the layout.</param>
        public void CreateLayout(string layoutName, IEnumerable<BuildingBlockDefinition> blocks)
        {
            if (blocks == null)
            {
                throw new ArgumentNullException("blocks");
            }

            if (this.layouts == null)
            {
                throw new InvalidOperationException(ModelResources.LoadTemplateFirst);
            }

            if (!blocks.Any(block => block.Name == BuildingBlockName.Default))
            {
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, ModelResources.DefaultNeededToCreateLayout, layoutName));
            }

            this.DeleteLayout(layoutName);

            BuildingBlock[] buildingBlocks = blocks.Select(block => this.wordTemplate.CreateBuildingBlock(CategoryPrefix + layoutName, block.Name.ToString(), block.StartPosition, block.EndPosition)).ToArray();
            BuildingBlockInfo[] buildingBlocksInfo = blocks.Select(block => this.GetDocument().ReadBuildingBlockInfo(block.Name, block.StartPosition, block.EndPosition)).ToArray();
            LayoutInformation newLayout = this.CreateLayout(CategoryPrefix + layoutName, buildingBlocks, buildingBlocksInfo);
            this.layouts.Add(newLayout);
        }

        /// <summary>
        /// Gets a named building block from a layout.
        /// </summary>
        /// <param name="layout">The layout for which the building block is to be retrieved.</param>
        /// <param name="name">The name of the building block to retrieve.</param>
        /// <returns>The named <see cref="BuildingBlock"/> from the given layout.</returns>
        public BuildingBlock GetLayoutBuildingBlock(LayoutInformation layout, BuildingBlockName name)
        {
            if (layout == null)
            {
                throw new ArgumentNullException("layout");
            }

            if (name == null)
            {
                throw new ArgumentNullException("name");
            }

            BuildingBlock ans = null;
            Dictionary<string, BuildingBlock> layoutBlockDictionary = null;
            if (!this.buildingBlockCache.TryGetValue(layout.Name, out layoutBlockDictionary))
            {
                IEnumerable<BuildingBlock> blocks = this.wordTemplate.EnumerateBuildingBlocksForCategory(CategoryPrefix + layout.Name);
                this.AddLayoutBlocksToCache(layout.Name, blocks);
                layoutBlockDictionary = this.buildingBlockCache[layout.Name];
            }
            
            if (layoutBlockDictionary != null)
            {
                layoutBlockDictionary.TryGetValue(name.ToString(), out ans);
            }

            return ans;
        }

        /// <summary>
        /// Extracts the layout name from a category name.
        /// </summary>
        /// <param name="categoryName">The category name.</param>
        /// <returns>The layout name taken from the category name.</returns>
        private static string ExtractLayoutName(string categoryName)
        {
            string ans = null;
            Match m = LayoutCategoryPattern.Match(categoryName);
            if (m.Success)
            {
                ans = m.Groups[1].Captures[0].Value;
            }

            return ans;
        }

        /// <summary>
        /// Checks if a building block is the default building block.
        /// </summary>
        /// <param name="bb">The building block to check.</param>
        /// <returns><c>True</c> if the building block is the default building block, <c>false</c> otherwise.</returns>
        private static bool IsDefaultBuildingBlock(BuildingBlock bb)
        {
            return new BuildingBlockName(bb) == BuildingBlockName.Default;
        }

        /// <summary>
        /// Checks if a building block is the default building block.
        /// </summary>
        /// <param name="bbi">The building block info to check.</param>
        /// <returns><c>True</c> if the building block is the default building block, <c>false</c> otherwise.</returns>
        private static bool IsDefaultBuildingBlock(BuildingBlockInfo bbi)
        {
            return bbi.Name == BuildingBlockName.Default;
        }

        /// <summary>
        /// Checks if a building block is the preview building block.
        /// </summary>
        /// <param name="bbi">The building block info to check.</param>
        /// <returns><c>True</c> if the building block is the preview building block, <c>false</c> otherwise.</returns>
        private static bool IsPreviewBuildingBlock(BuildingBlockInfo bbi)
        {
            return bbi.Name == BuildingBlockName.Preview;
        }

        /// <summary>
        /// Creates a layout information object from the building blocks that make it up.
        /// </summary>
        /// <param name="category">The category from which to get the layout name.</param>
        /// <param name="blocks">The building blocks that make up the layout.</param>
        /// <param name="blocksInfo">Information about the building blocks that make up the layout.</param>
        /// <returns>The layout information for the new layout.</returns>
        private LayoutInformation CreateLayout(string category, IEnumerable<BuildingBlock> blocks, IEnumerable<BuildingBlockInfo> blocksInfo)
        {
            BuildingBlockInfo defaultBbi = null;
            BuildingBlockInfo previewBbi = null;
            string layoutName = ExtractLayoutName(category);

            string[] fields = new string[0];
            foreach (BuildingBlockInfo bbi in blocksInfo)
            {
                if (IsDefaultBuildingBlock(bbi))
                {
                    defaultBbi = bbi;
                }
                else if (IsPreviewBuildingBlock(bbi))
                {
                    previewBbi = bbi;
                }

                fields = fields.Union(bbi.ContentControlTags.Where(tag => Utilities.IsValidTag(tag))).ToArray();
            }

            this.AddLayoutBlocksToCache(layoutName, blocks);

            return new LayoutInformation(
                                         layoutName,
                                         blocks.Select(bb => new BuildingBlockName(bb)).ToArray(),
                                         fields.Distinct(StringComparer.Ordinal).ToArray(),
                                         previewBbi == null ? defaultBbi.PreviewImage : previewBbi.PreviewImage);
        }

        /// <summary>
        /// Adds building blocks for a layout to the building block cache.
        /// </summary>
        /// <param name="layoutName">The name of the layout.</param>
        /// <param name="blocks">The building blocks for the layout.</param>
        private void AddLayoutBlocksToCache(string layoutName, IEnumerable<BuildingBlock> blocks)
        {
            Dictionary<string, BuildingBlock> layoutBlockDictionary = new Dictionary<string, BuildingBlock>(StringComparer.OrdinalIgnoreCase);
            foreach (BuildingBlock bb in blocks)
            {
                layoutBlockDictionary.Add(bb.Name, bb);
            }

            this.buildingBlockCache[layoutName] = layoutBlockDictionary;
        }

        /// <summary>
        /// Gets the Word document to be used for processing a building block.
        /// </summary>
        /// <returns>The Word document.</returns>
        private IWordDocument GetDocument()
        {
            return this.application.ActiveDocument; // TODO: consider creating a new temporary document instead.
        }

        /// <summary>
        /// Checks to see if a particular category name corresponds to a layout.
        /// </summary>
        /// <remarks>
        /// A category name corresponds to a layout if it matches the regular expression for the name, and the category contains a Default building block.
        /// </remarks>
        /// <param name="categoryName">The name of the category to be tested.</param>
        /// <returns><c>true</c> if the category is a valid layout</returns>
        private bool CategoryIsLayout(string categoryName)
        {
            bool ans = false;
            if (LayoutCategoryPattern.IsMatch(categoryName))
            {
                ans = this.wordTemplate.EnumerateBuildingBlocksForCategory(categoryName).Where(bb => IsDefaultBuildingBlock(bb)).Count() == 1;
            }

            return ans;
        }

        /// <summary>
        /// Reads the building blocks for a category and creates a <see cref="LayoutInformation"/> object from them.
        /// </summary>
        /// <param name="document">The document to be used to temporarily insert the building blocks so they can be scanned for content control tags.</param>
        /// <param name="category">The category to read from.</param>
        /// <returns>The layout.</returns>
        private LayoutInformation GetLayoutForCategory(IWordDocument document, string category)
        {
            IEnumerable<BuildingBlock> blocks = this.wordTemplate.EnumerateBuildingBlocksForCategory(category);
            IEnumerable<BuildingBlockInfo> blocksInfo = blocks.Select(block => document.ReadBuildingBlockInfo(block));

            return this.CreateLayout(category, blocks, blocksInfo);
        }
    }
}
