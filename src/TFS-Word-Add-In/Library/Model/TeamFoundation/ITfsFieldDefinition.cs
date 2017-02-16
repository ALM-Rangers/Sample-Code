//---------------------------------------------------------------------
// <copyright file="ITfsFieldDefinition.cs" company="Microsoft">
//    Copyright (c) Microsoft. All rights reserved.
//    This code is licensed under the MIT License.
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF 
//    ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
//    TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
//    PARTICULAR PURPOSE AND NONINFRINGEMENT.
// </copyright>
// <summary>The ITfsFieldDefinition type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.Word4Tfs.Library.Model.TeamFoundation
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Microsoft.TeamFoundation.WorkItemTracking.Client;

    /// <summary>
    /// Defines the operations on a TFS field definition
    /// </summary>
    /// <remarks>
    /// This contains a subset of the operations of a <see cref="FieldDefinition"/> so that queries can be mocked.
    /// </remarks>
    public interface ITfsFieldDefinition
    {
        /// <summary>
        /// Gets the reference name of the <see cref="Field"/>.
        /// </summary>
        string ReferenceName { get; }

        /// <summary>
        /// Gets the friendly name of the <see cref="Field"/>.
        /// </summary>
        string FriendlyName { get; }

        /// <summary>
        /// Gets the type of the field.
        /// </summary>
        FieldType FieldType { get; }
    }
}
