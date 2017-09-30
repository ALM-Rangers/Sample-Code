//---------------------------------------------------------------------
// <copyright file="ITeamProjectTemplate.cs" company="Microsoft">
//    Copyright © ALM | DevOps Ranger Contributors. All rights reserved.
//    This code is licensed under the MIT License.
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF 
//    ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
//    TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
//    PARTICULAR PURPOSE AND NONINFRINGEMENT.
// </copyright>
// <summary>The ITeamProjectTemplate type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.Word4Tfs.Library.Model
{
    using global::System.Collections.Generic;
    using Microsoft.Office.Interop.Word;
    using Microsoft.Word4Tfs.Library.Data;

    /// <summary>
    /// The operations on the model that represents a team project template.
    /// </summary>
    public interface ITeamProjectTemplate
    {
        /// <summary>
        /// Gets a value indicating whether the document is importable.
        /// </summary>
        bool IsImportable { get; }

        /// <summary>
        /// Gets the list of currently available layouts
        /// </summary>
        /// <remarks>
        /// This call does not require an active document as it accesses the currently available templates.
        /// </remarks>
        IEnumerable<LayoutInformation> Layouts { get; }

        /// <summary>
        /// Loads the template.
        /// </summary>
        void Load();

        /// <summary>
        /// Saves changes to the template.
        /// </summary>
        void Save();

        /// <summary>
        /// Deletes a layout from the template.
        /// </summary>
        /// <param name="layoutName">The name of the layout to be deleted.</param>
        void DeleteLayout(string layoutName);

        /// <summary>
        /// Creates a layout.
        /// </summary>
        /// <param name="layoutName">The name of the layout being created.</param>
        /// <param name="blocks">List of building block definitions for the building blocks to be created that make up the layout.</param>
        void CreateLayout(string layoutName, IEnumerable<BuildingBlockDefinition> blocks);

        /// <summary>
        /// Gets a named building block from a layout.
        /// </summary>
        /// <param name="layout">The layout for which the building block is to be retrieved.</param>
        /// <param name="name">The name of the building block to retrieve.</param>
        /// <returns>The named <see cref="BuildingBlock"/> from the given layout.</returns>
        BuildingBlock GetLayoutBuildingBlock(LayoutInformation layout, BuildingBlockName name);
    }
}
