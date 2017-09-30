//---------------------------------------------------------------------
// <copyright file="ITfsWorkItem.cs" company="Microsoft">
//    Copyright © ALM | DevOps Ranger Contributors. All rights reserved.
//    This code is licensed under the MIT License.
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF 
//    ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
//    TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
//    PARTICULAR PURPOSE AND NONINFRINGEMENT.
// </copyright>
// <summary>The ITfsWorkItem type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.Word4Tfs.Library.Model.TeamFoundation
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Xml.Linq;

    /// <summary>
    /// Defines a TFS work item.
    /// </summary>
    public interface ITfsWorkItem
    {
        /// <summary>
        /// Gets the ID of this work item.
        /// </summary>
        int Id { get; }

        /// <summary>
        /// Gets the name of the work item type.
        /// </summary>
        string Type { get; }

        /// <summary>
        /// Gets the list of reference names of the fields in the work item.
        /// </summary>
        IEnumerable<string> FieldReferenceNames { get; }

        /// <summary>
        /// Gets the value of a field in this work item specified by the field name.
        /// </summary>
        /// <param name="name">The name that is passed in <paramref name="name"/>could be either the field name or a reference name</param>
        /// <returns>The object that is contained in this field.</returns>
        object this[string name]
        {
            get;
        }
    }
}
