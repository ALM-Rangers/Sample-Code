//---------------------------------------------------------------------
// <copyright file="TfsFieldDefinition.cs" company="Microsoft">
//    Copyright © ALM | DevOps Ranger Contributors. All rights reserved.
//    This code is licensed under the MIT License.
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF 
//    ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
//    TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
//    PARTICULAR PURPOSE AND NONINFRINGEMENT.
// </copyright>
// <summary>The TfsFieldDefinition type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.Word4Tfs.Library.Model.TeamFoundation
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Microsoft.TeamFoundation.WorkItemTracking.Client;

    /// <summary>
    /// Provides access to the TFS <see cref="FieldDefinition"/> class.
    /// </summary>
    public class TfsFieldDefinition : ITfsFieldDefinition
    {
        /// <summary>
        /// The TFS field definition that this object wraps.
        /// </summary>
        private FieldDefinition fieldDefinition;

        /// <summary>
        /// Initializes a new instance of the <see cref="TfsFieldDefinition"/> class.
        /// </summary>
        /// <param name="fieldDefinition">The TFS field definition that this object wraps.</param>
        public TfsFieldDefinition(FieldDefinition fieldDefinition)
        {
            this.fieldDefinition = fieldDefinition;
        }

        #region ITfsFieldDefinition Members

        /// <summary>
        /// Gets the reference name of the <see cref="Field"/>.
        /// </summary>
        public string ReferenceName
        {
            get
            {
                return this.fieldDefinition.ReferenceName;
            }
        }

        /// <summary>
        /// Gets the friendly name of the <see cref="Field"/>.
        /// </summary>
        public string FriendlyName
        {
            get
            {
                return this.fieldDefinition.Name;
            }
        }

        /// <summary>
        /// Gets the type of the field.
        /// </summary>
        public FieldType FieldType
        {
            get
            {
                return this.fieldDefinition.FieldType;
            }
        }

        #endregion
    }
}
