//---------------------------------------------------------------------
// <copyright file="LayoutInformation.cs" company="Microsoft">
//    Copyright (c) Microsoft. All rights reserved.
//    This code is licensed under the MIT License.
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF 
//    ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
//    TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
//    PARTICULAR PURPOSE AND NONINFRINGEMENT.
// </copyright>
// <summary>The LayoutInformation type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.Word4Tfs.Library.Data
{
    using System;
    using global::System.Collections.Generic;
    using global::System.Drawing;
    using global::System.Linq;
    using Microsoft.Office.Interop.Word;

    /// <summary>
    /// A layout that can be applied to a set of work items.
    /// </summary>
    public class LayoutInformation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LayoutInformation"/> class.
        /// </summary>
        /// <param name="name">The name of the layout.</param>
        /// <param name="buildingBlockNames">The names of the building blocks that go to make up the layout.</param>
        /// <param name="fieldNames">The names of the fields that are used by the layout.</param>
        /// <param name="previewImage">The preview image for the layout.</param>
        public LayoutInformation(string name, BuildingBlockName[] buildingBlockNames, string[] fieldNames, Image previewImage)
        {
            if (buildingBlockNames == null)
            {
                throw new ArgumentNullException("buildingBlockNames");
            }

            this.Name = name;
            this.FieldNames = fieldNames;
            this.PreviewImage = previewImage;
            this.BuildingBlockNames = buildingBlockNames;
        }

        /// <summary>
        /// Gets the name of a layout.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the names of the building blocks that make up the layout.
        /// </summary>
        public IEnumerable<BuildingBlockName> BuildingBlockNames { get; private set; }

        /// <summary>
        /// Gets the names of all the fields that are used by the layout.
        /// </summary>
        public IEnumerable<string> FieldNames { get; private set; }

        /// <summary>
        /// Gets the preview image for the layout.
        /// </summary>
        public Image PreviewImage { get; private set; }

        /// <summary>
        /// Returns a string representing the layout.
        /// </summary>
        /// <returns>A string representing the layout.</returns>
        public override string ToString()
        {
            return this.Name;
        }
    }
}
