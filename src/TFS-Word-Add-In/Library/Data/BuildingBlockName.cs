//---------------------------------------------------------------------
// <copyright file="BuildingBlockName.cs" company="Microsoft">
//    Copyright (c) Microsoft. All rights reserved.
//    This code is licensed under the MIT License.
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF 
//    ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
//    TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
//    PARTICULAR PURPOSE AND NONINFRINGEMENT.
// </copyright>
// <summary>The BuildingBlockName type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.Word4Tfs.Library.Data
{
    using global::System;
    using global::System.Globalization;
    using global::System.Text.RegularExpressions;
    using Microsoft.Office.Interop.Word;

    /// <summary>
    /// Represents the name of a building block.
    /// </summary>
    public class BuildingBlockName : IEquatable<BuildingBlockName>
    {
        /// <summary>
        /// Pattern used to parse a building block name.
        /// </summary>
        private static Regex buildingBlockNameAndLevelPattern = new Regex(@"^(?<name>.+)_(?<level>\d+)$");

        /// <summary>
        /// The default building block name.
        /// </summary>
        private static BuildingBlockName defaultName = new BuildingBlockName(Constants.DefaultBuildingBlockName);

        /// <summary>
        /// The preview building block name.
        /// </summary>
        private static BuildingBlockName previewName = new BuildingBlockName(Constants.PreviewBuildingBlockName);

        /// <summary>
        /// The type of the work item that this building block is for.
        /// </summary>
        private string workItemType;

        /// <summary>
        /// The level the building block is for, <c>null</c> if not level-specific.
        /// </summary>
        private Nullable<int> level;

        /// <summary>
        /// Initializes a new instance of the <see cref="BuildingBlockName"/> class with the name used for the default building block.
        /// </summary>
        public BuildingBlockName()
        {
            this.workItemType = Constants.DefaultBuildingBlockName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BuildingBlockName"/> class with the name used for a level-specific default building block.
        /// </summary>
        /// <param name="level">The level the building block is for.</param>
        public BuildingBlockName(int level)
        {
            this.workItemType = Constants.DefaultBuildingBlockName;
            this.level = level;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BuildingBlockName"/> class for a non-level specific work item.
        /// </summary>
        /// <param name="workItemType">The work item type the building block is for.</param>
        public BuildingBlockName(string workItemType)
        {
            this.workItemType = workItemType;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BuildingBlockName"/> class for a level-specific work item.
        /// </summary>
        /// <param name="workItemType">The work item type the building block is for.</param>
        /// <param name="level">The level the building block is for.</param>
        public BuildingBlockName(string workItemType, int level)
        {
            this.workItemType = workItemType;
            this.level = level;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BuildingBlockName"/> class parsing the name of an existing <see cref="BuildingBlock"/>.
        /// </summary>
        /// <param name="buildingBlock">The building block to parse the name from.</param>
        public BuildingBlockName(BuildingBlock buildingBlock)
        {
            if (buildingBlock == null)
            {
                throw new ArgumentNullException("buildingBlock");
            }

            BuildingBlockName temp = BuildingBlockName.Parse(buildingBlock.Name);
            this.workItemType = temp.workItemType;
            this.level = temp.level;
        }

        /// <summary>
        /// Gets the name of the default building block.
        /// </summary>
        public static BuildingBlockName Default
        {
            get
            {
                return defaultName;
            }
        }

        /// <summary>
        /// Gets the name of the preview building block.
        /// </summary>
        public static BuildingBlockName Preview
        {
            get
            {
                return previewName;
            }
        }

        /// <summary>
        /// Equality operator.
        /// </summary>
        /// <param name="name1">The first building block name to compare.</param>
        /// <param name="name2">The second building block name to compare.</param>
        /// <returns><c>True</c> if <paramref name="name1"/> and <paramref name="name2"/> have the same work item type and level, <c>false</c> otherwise.</returns>
        public static bool operator ==(BuildingBlockName name1, BuildingBlockName name2)
        {
            if (object.ReferenceEquals(name1, null))
            {
                return object.ReferenceEquals(name2, null);
            }

            return name1.Equals(name2);
        }

        /// <summary>
        /// Inequality operator.
        /// </summary>
        /// <param name="name1">The first building block name to compare.</param>
        /// <param name="name2">The second building block name to compare.</param>
        /// <returns><c>True</c> if <paramref name="name1"/> and <paramref name="name2"/> do not have the same work item type and level, <c>false</c> otherwise.</returns>
        public static bool operator !=(BuildingBlockName name1, BuildingBlockName name2)
        {
            if (object.ReferenceEquals(name1, null))
            {
                return false;
            }

            return !name1.Equals(name2);
        }

        /// <summary>
        /// Parses a string for a <see cref="BuildingBlockName"/>.
        /// </summary>
        /// <param name="name">The name to be parsed.</param>
        /// <returns>The parsed building block name.</returns>
        public static BuildingBlockName Parse(string name)
        {
            BuildingBlockName ans = null;

            Match m = buildingBlockNameAndLevelPattern.Match(name);
            if (m.Success)
            {
                string workItemTypeString = m.Groups[1].Captures[0].Value;
                string levelString = m.Groups[2].Captures[0].Value;
                int level = int.Parse(levelString, CultureInfo.InvariantCulture);
                ans = new BuildingBlockName(workItemTypeString, level);
            }
            else
            {
                ans = new BuildingBlockName(name);
            }

            if (ans == BuildingBlockName.Default)
            {
                ans = BuildingBlockName.Default;
            }
            else if (ans == BuildingBlockName.Preview)
            {
                ans = BuildingBlockName.Preview;
            }

            return ans;
        }

        /// <summary>
        /// Returns the string representation of the building block name.
        /// </summary>
        /// <returns>The string representation of the building block name.</returns>
        public override string ToString()
        {
            string ans;
            if (this.level.HasValue)
            {
                ans = string.Format(CultureInfo.InvariantCulture, "{0}_{1}", this.workItemType, this.level.Value);
            }
            else
            {
                ans = this.workItemType;
            }

            return ans;
        }

        /// <summary>
        /// Checks for equality.
        /// </summary>
        /// <param name="other">The comparand.</param>
        /// <returns><c>True</c> if <paramref name="other"/> has the same work item type and level, <c>false</c> otherwise.</returns>
        public bool Equals(BuildingBlockName other)
        {
            if (object.ReferenceEquals(other, null))
            {
                return false;
            }

            if (string.Compare(this.workItemType, other.workItemType, StringComparison.OrdinalIgnoreCase) != 0)
            {
                return false;
            }

            return string.Compare(this.ToString(), other.ToString(), StringComparison.OrdinalIgnoreCase) == 0;
        }

        /// <summary>
        /// Checks for equality.
        /// </summary>
        /// <param name="obj">The comparand.</param>
        /// <returns><c>True</c> if <paramref name="obj"/> has the same work item type and level, <c>false</c> otherwise.</returns>
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            BuildingBlockName comparand = obj as BuildingBlockName;
            if (object.ReferenceEquals(comparand, null))
            {
                return false;
            }

            return this.Equals(comparand);
        }

        /// <summary>
        /// Gets the hash code for the building block name.
        /// </summary>
        /// <returns>The hash code for the building block name.</returns>
        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }
    }
}
