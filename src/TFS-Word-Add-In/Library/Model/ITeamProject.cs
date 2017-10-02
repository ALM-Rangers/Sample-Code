//---------------------------------------------------------------------
// <copyright file="ITeamProject.cs" company="Microsoft">
//    Copyright © ALM | DevOps Ranger Contributors. All rights reserved.
//    This code is licensed under the MIT License.
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF 
//    ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
//    TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
//    PARTICULAR PURPOSE AND NONINFRINGEMENT.
// </copyright>
// <summary>The ITeamProject type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.Word4Tfs.Library.Model
{
    using System;
    using System.Collections.Generic;
    using Microsoft.TeamFoundation.WorkItemTracking.Client;
    using Microsoft.Word4Tfs.Library.Data;
    using Microsoft.Word4Tfs.Library.Model.TeamFoundation;

    /// <summary>
    /// Defines the operations to be performed on a Team Project.
    /// </summary>
    public interface ITeamProject : IDisposable
    {
        /// <summary>
        /// Gets the information about the Team Project.
        /// </summary>
        TeamProjectInformation TeamProjectInformation { get; }

        /// <summary>
        /// Gets the root query folder for the Team Project.
        /// </summary>
        QueryFolder RootQueryFolder { get; }

        /// <summary>
        /// Gets the object needed to run queries against the Team Project.
        /// </summary>
        ITeamProjectQuery QueryRunner { get; }

        /// <summary>
        /// Gets the list of fields defined for this Team Project.
        /// </summary>
        IEnumerable<ITfsFieldDefinition> FieldDefinitions { get; }
    }
}
