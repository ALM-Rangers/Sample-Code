//---------------------------------------------------------------------
// <copyright file="FieldDefinitionEventArgs.cs" company="Microsoft">
//    Copyright © ALM | DevOps Ranger Contributors. All rights reserved.
//    This code is licensed under the MIT License.
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF 
//    ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
//    TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
//    PARTICULAR PURPOSE AND NONINFRINGEMENT.
// </copyright>
// <summary>The FieldDefinitionEventArgs type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.Word4Tfs.Library.View
{
    using System;
    using Microsoft.Word4Tfs.Library.Model.TeamFoundation;

    /// <summary>
    /// Information relating to an event about a field definition.
    /// </summary>
    public class FieldDefinitionEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FieldDefinitionEventArgs"/> class.
        /// </summary>
        /// <param name="fieldDefinition">The field definition associated with the event.</param>
        public FieldDefinitionEventArgs(ITfsFieldDefinition fieldDefinition)
        {
            this.FieldDefinition = fieldDefinition;
        }

        /// <summary>
        /// Gets or sets the field definition associated with the event.
        /// </summary>
        public ITfsFieldDefinition FieldDefinition { get; set; }
    }
}
