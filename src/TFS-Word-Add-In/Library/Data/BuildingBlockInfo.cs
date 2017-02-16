//---------------------------------------------------------------------
// <copyright file="BuildingBlockInfo.cs" company="Microsoft">
//    Copyright (c) Microsoft. All rights reserved.
//    This code is licensed under the MIT License.
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF 
//    ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
//    TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
//    PARTICULAR PURPOSE AND NONINFRINGEMENT.
// </copyright>
// <summary>The BuildingBlockInfo type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.Word4Tfs.Library.Data
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Text;

    /// <summary>
    /// Provides information about a <see cref="BuildingBlock"/>
    /// </summary>
    public class BuildingBlockInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BuildingBlockInfo"/> class.
        /// </summary>
        /// <param name="name">The name of the building block.</param>
        /// <param name="contentControlTags">The tags of the content controls associated with the <see cref="BuildingBlock"/>.</param>
        /// <param name="previewImage">The preview image for the <see cref="BuildingBlock"/>.</param>
        public BuildingBlockInfo(BuildingBlockName name, IEnumerable<string> contentControlTags, Image previewImage)
        {
            this.Name = name;
            this.ContentControlTags = contentControlTags;
            this.PreviewImage = previewImage;
        }

        /// <summary>
        /// Gets the name of the building block.
        /// </summary>
        public BuildingBlockName Name { get; private set; }

        /// <summary>
        /// Gets the content control tags associated with the building block.
        /// </summary>
        public IEnumerable<string> ContentControlTags { get; private set; }

        /// <summary>
        /// Gets the preview image for the building block.
        /// </summary>
        public Image PreviewImage { get; private set; }
    }
}
