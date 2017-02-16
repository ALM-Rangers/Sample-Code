//---------------------------------------------------------------------
// <copyright file="BuildingBlockDefinition.cs" company="Microsoft">
//    Copyright (c) Microsoft. All rights reserved.
//    This code is licensed under the MIT License.
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF 
//    ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
//    TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
//    PARTICULAR PURPOSE AND NONINFRINGEMENT.
// </copyright>
// <summary>The BuildingBlockDefinition type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.Word4Tfs.Library.Data
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Defines the information needed to create a building block
    /// </summary>
    public class BuildingBlockDefinition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BuildingBlockDefinition"/> class.
        /// </summary>
        /// <param name="name">The name of the building block.</param>
        /// <param name="startPosition">The start position of the building block.</param>
        /// <param name="endPosition">The end position of the building block.</param>
        public BuildingBlockDefinition(string name, int startPosition, int endPosition)
        {
            this.Name = BuildingBlockName.Parse(name);
            this.StartPosition = startPosition;
            this.EndPosition = endPosition;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BuildingBlockDefinition"/> class.
        /// </summary>
        /// <param name="name">The name of the building block.</param>
        /// <param name="startPosition">The start position of the building block.</param>
        /// <param name="endPosition">The end position of the building block.</param>
        public BuildingBlockDefinition(BuildingBlockName name, int startPosition, int endPosition)
        {
            this.Name = name;
            this.StartPosition = startPosition;
            this.EndPosition = endPosition;
        }

        /// <summary>
        /// Gets the name of the building block.
        /// </summary>
        public BuildingBlockName Name { get; private set; }

        /// <summary>
        /// Gets the start position of the building block.
        /// </summary>
        public int StartPosition { get; private set; }

        /// <summary>
        /// Gets the end position of the building block.
        /// </summary>
        public int EndPosition { get; private set; }

        /// <summary>
        /// Checks for equality.
        /// </summary>
        /// <param name="obj">The comparand.</param>
        /// <returns><c>True</c> if <paramref name="obj"/> has the same name and start/end position, <c>false</c> otherwise.</returns>
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            BuildingBlockDefinition comparand = obj as BuildingBlockDefinition;
            if (object.ReferenceEquals(comparand, null))
            {
                return false;
            }

            return this.Name == comparand.Name && this.StartPosition == comparand.StartPosition && this.EndPosition == comparand.EndPosition;
        }

        /// <summary>
        /// Gets the hash code for the building block definition.
        /// </summary>
        /// <returns>The hash code for the building block definition.</returns>
        public override int GetHashCode()
        {
            return this.Name.GetHashCode() ^ this.StartPosition ^ this.EndPosition;
        }
    }
}
