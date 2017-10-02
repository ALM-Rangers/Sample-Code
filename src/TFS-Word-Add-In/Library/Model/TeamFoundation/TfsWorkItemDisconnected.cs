//---------------------------------------------------------------------
// <copyright file="TfsWorkItemDisconnected.cs" company="Microsoft">
//    Copyright © ALM | DevOps Ranger Contributors. All rights reserved.
//    This code is licensed under the MIT License.
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF 
//    ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
//    TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
//    PARTICULAR PURPOSE AND NONINFRINGEMENT.
// </copyright>
// <summary>The TfsWorkItemDisconnected type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.Word4Tfs.Library.Model.TeamFoundation
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;

    /// <summary>
    /// Represents a TFS work item that is not connected to TFS.
    /// </summary>
    /// <remarks>
    /// This class is used to represent the work items that have been read back from the document.
    /// </remarks>
    public class TfsWorkItemDisconnected : ITfsWorkItem
    {
        /// <summary>
        /// Stores the names and values of the fields.
        /// </summary>
        private Dictionary<string, object> fields = new Dictionary<string, object>(StringComparer.Ordinal);

        /// <summary>
        /// Initializes a new instance of the <see cref="TfsWorkItemDisconnected"/> class.
        /// </summary>
        /// <param name="id">The id of the work item.</param>
        /// <param name="type">The type of the work item</param>
        /// <param name="fields">Name value pairs for all the fields.</param>
        public TfsWorkItemDisconnected(int id, string type, params Tuple<string, object>[] fields)
        {
            if (fields == null)
            {
                throw new ArgumentNullException("fields");
            }

            this.Id = id;
            this.Type = type;
            this.AddField(Constants.SystemIdFieldReferenceName, id);
            this.AddField(Constants.SystemWorkItemTypeFieldReferenceName, type);
            foreach (Tuple<string, object> field in fields)
            {
                this.AddField(field.Item1, field.Item2);
            }
        }

        /// <summary>
        /// Gets the ID of this work item.
        /// </summary>
        public int Id { get; private set; }
 
        /// <summary>
        /// Gets the name of the work item type.
        /// </summary>
        public string Type { get; private set; }

        /// <summary>
        /// Gets the list of reference names of the fields in the work item.
        /// </summary>
        public IEnumerable<string> FieldReferenceNames
        {
            get
            {
                return this.fields.Keys;
            }
        }

        /// <summary>
        /// Gets the value of a field in this work item specified by the field name.
        /// </summary>
        /// <param name="name">The name that is passed in <paramref name="name"/>could be either the field name or a reference name</param>
        /// <returns>The object that is contained in this field.</returns>
        public object this[string name]
        {
            get
            {
                return this.fields[name];
            }
        }

        /// <summary>
        /// Adds a field to the list of fields.
        /// </summary>
        /// <param name="name">The name of the field.</param>
        /// <param name="value">The value of the field.</param>
        private void AddField(string name, object value)
        {
            if (this.fields.ContainsKey(name))
            {
                this.fields[name] = value;
            }
            else
            {
                this.fields.Add(name, value);
            }

            if (StringComparer.OrdinalIgnoreCase.Compare(name, Constants.SystemIdFieldReferenceName) == 0)
            {
                this.Id = int.Parse(value.ToString(), CultureInfo.InvariantCulture);
            }

            if (StringComparer.OrdinalIgnoreCase.Compare(name, Constants.SystemWorkItemTypeFieldReferenceName) == 0)
            {
                this.Type = (string)value;
            }
        }
    }
}
