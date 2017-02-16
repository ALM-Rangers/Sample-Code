//---------------------------------------------------------------------
// <copyright file="LayoutDefinitionFormatter.cs" company="Microsoft">
//    Copyright (c) Microsoft. All rights reserved.
//    This code is licensed under the MIT License.
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF 
//    ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
//    TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
//    PARTICULAR PURPOSE AND NONINFRINGEMENT.
// </copyright>
// <summary>The LayoutDefinitionFormatter type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.Word4Tfs.Library.Model
{
    using global::System;
    using global::System.Collections.Generic;
    using global::System.Diagnostics;
    using global::System.Globalization;
    using global::System.Linq;
    using Microsoft.Office.Interop.Word;
    using Microsoft.Word4Tfs.Library.Data;
    using Microsoft.Word4Tfs.Library.Model.Word;

    /// <summary>
    /// Displays and creates layout definitions in a document.
    /// </summary>
    public class LayoutDefinitionFormatter : ILayoutDefinitionFormatter
    {
        /// <summary>
        /// Used to generate unique bookmarks.
        /// </summary>
        private int bookmarkSequence = 0;

        /// <summary>
        /// The logger used to log trace information.
        /// </summary>
        private ILogger logger;

        /// <summary>
        /// The word document in which the layout definitions are viewed and edited.
        /// </summary>
        private IWordDocument document;

        /// <summary>
        /// The team project template in which the layout definitions are stored.
        /// </summary>
        private ITeamProjectTemplate template;

        /// <summary>
        /// Initializes a new instance of the <see cref="LayoutDefinitionFormatter"/> class.
        /// </summary>
        /// <param name="logger">The logger to be used for logging.</param>
        /// <param name="document">The word document in which the layout definitions are viewed and edited.</param>
        /// <param name="template">The team project template which contains the layout building blocks to be used.</param>
        public LayoutDefinitionFormatter(ILogger logger, IWordDocument document, ITeamProjectTemplate template)
        {
            this.logger = logger;
            this.document = document;
            this.template = template;
        }

        /// <summary>
        /// Displays a layout definition in a template.
        /// </summary>
        /// <param name="projectTemplate">The template containing the layout definition.</param>
        /// <param name="layoutName">The name of the layout to be displayed.</param>
        public void DisplayDefinition(ITeamProjectTemplate projectTemplate, string layoutName)
        {
            if (projectTemplate == null)
            {
                throw new ArgumentNullException("projectTemplate");
            }

            this.document.DeleteAllContents();
            LayoutInformation layout = GetLayout(projectTemplate, layoutName);
            if (layout != null)
            {
                this.DisplayDefinition(layout);
            }
        }

        /// <summary>
        /// Saves a layout definition to the template
        /// </summary>
        /// <param name="projectTemplate">The template to save the definitions to.</param>
        /// <param name="layoutName">The name of the layout to be saved.</param>
        public void SaveDefinition(ITeamProjectTemplate projectTemplate, string layoutName)
        {
            if (projectTemplate == null)
            {
                throw new ArgumentNullException("projectTemplate");
            }

            if (string.IsNullOrEmpty(layoutName))
            {
                throw new ArgumentNullException("layoutName");
            }

            this.logger.Log(TraceEventType.Information, "Saving the layout definition {0} in the document to the template.", layoutName);
            projectTemplate.DeleteLayout(layoutName);

            string block = null;
            Paragraph startOfBlock = null;
            Paragraph previous = null;

            List<BuildingBlockDefinition> blockDefinitions = new List<BuildingBlockDefinition>();

            block = null;
            this.logger.Log(TraceEventType.Verbose, "Total paragraphs is {0}", this.document.Paragraphs.Count());
            foreach (Paragraph p in this.document.Paragraphs)
            {
                if (this.ParagraphIsOfStyle(p, Constants.WorkItemDefinitionStyleName))
                {
                    if (startOfBlock != null)
                    {
                        AddBuildingBlockDefinition(blockDefinitions, block, startOfBlock, previous, false);
                    }

                    this.StartNewBlock(ref block, ref startOfBlock, p);
                }
                else if (startOfBlock == null && !string.IsNullOrEmpty(block))
                {
                    startOfBlock = p;
                }

                previous = p;
            }

            if (startOfBlock != null)
            {
                AddBuildingBlockDefinition(blockDefinitions, block, startOfBlock, previous, true);
            }

            projectTemplate.CreateLayout(layoutName, blockDefinitions);
        }

        /// <summary>
        /// Adds a prototype layout definition to the document, used when creating a new layout.
        /// </summary>
        public void AddPrototypeDefinition()
        {
            this.document.DeleteAllContents();
            this.document.InsertParagraph(BuildingBlockName.Default.ToString(), Constants.WorkItemDefinitionStyleName);
            this.document.InsertParagraph(ModelResources.InstructionsDefaultBuildingBlock, Constants.NormalStyleName);
        }

        /// <summary>
        /// Gets a layout from a template.
        /// </summary>
        /// <param name="projectTemplate">The template to get the layout from.</param>
        /// <param name="layoutName">The name of the layout to get.</param>
        /// <returns>The layout, <c>null</c> if not found.</returns>
        private static LayoutInformation GetLayout(ITeamProjectTemplate projectTemplate, string layoutName)
        {
            LayoutInformation layout = projectTemplate.Layouts.Where(li => li.Name == layoutName).SingleOrDefault();
            return layout;
        }

        /// <summary>
        /// Adds a building block definition to the list of building blocks.
        /// </summary>
        /// <param name="blockDefinitions">The list of building blocks to add the definition to.</param>
        /// <param name="blockName">The name of the block to be added.</param>
        /// <param name="firstParagraph">The first paragraph of the block.</param>
        /// <param name="lastParagraph">The last paragraph of the block.</param>
        /// <param name="isEndOfDocument">A value indicating if the <paramref name="lastParagraph"/> is the last one in the document.</param>
        private static void AddBuildingBlockDefinition(List<BuildingBlockDefinition> blockDefinitions, string blockName, Paragraph firstParagraph, Paragraph lastParagraph, bool isEndOfDocument)
        {
            int correctionFactor = isEndOfDocument ? 1 : 0;
            blockDefinitions.Add(new BuildingBlockDefinition(blockName, firstParagraph.Range.Start, lastParagraph.Range.End - correctionFactor));
        }

        /// <summary>
        /// Displays a layout definition.
        /// </summary>
        /// <param name="layout">The layout to be displayed.</param>
        private void DisplayDefinition(LayoutInformation layout)
        {
            this.logger.Log(TraceEventType.Information, "Exporting layout {0}", layout.Name);
            foreach (BuildingBlockName bbn in layout.BuildingBlockNames)
            {
                BuildingBlock bb = this.template.GetLayoutBuildingBlock(layout, bbn);
                this.logger.Log(TraceEventType.Information, "Exporting layout building block {0} hash {1} ID {2}", bb.Name, bb.GetHashCode(), bb.ID);
                this.document.InsertParagraph(bb.Name, Constants.WorkItemDefinitionStyleName);
                this.document.InsertBuildingBlock(bb, string.Format(CultureInfo.InvariantCulture, "_Temp_{0}", this.bookmarkSequence++));
            }
        }

        /// <summary>
        /// Checks if a paragraph has a particular style.
        /// </summary>
        /// <param name="p">The paragraph to check.</param>
        /// <param name="styleName">The name of the style to check for.</param>
        /// <returns><c>true</c> if the paragraph is of the given style, <c>false</c> otherwise.</returns>
        private bool ParagraphIsOfStyle(Paragraph p, string styleName)
        {
            return this.document.ParagraphStyle(p) == styleName;
        }

        /// <summary>
        /// Starts a new block.
        /// </summary>
        /// <param name="block">Gets the text of the paragraph.</param>
        /// <param name="startOfBlock">The first paragraph of the content.</param>
        /// <param name="p">The current paragraph.</param>
        private void StartNewBlock(ref string block, ref Paragraph startOfBlock, Paragraph p)
        {
            block = this.document.ParagraphContent(p);
            startOfBlock = null;
        }
    }
}
