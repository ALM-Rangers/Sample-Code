//---------------------------------------------------------------------
// <copyright file="IFactory.cs" company="Microsoft">
//    Copyright © ALM | DevOps Ranger Contributors. All rights reserved.
//    This code is licensed under the MIT License.
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF 
//    ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
//    TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
//    PARTICULAR PURPOSE AND NONINFRINGEMENT.
// </copyright>
// <summary>The IFactory type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.Word4Tfs.Library.Model
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using Microsoft.Word4Tfs.Library.Model.TeamFoundation;

    /// <summary>
    /// Defines the operations to create objects that have integration dependencies.
    /// </summary>
    /// <remarks>
    /// This factory is used when objects have to be dynamically created and which are not suitable for creation with Unity.
    /// </remarks>
    public interface IFactory
    {
        /// <summary>
        /// Creates an <see cref="ITeamProject"/> object.
        /// </summary>
        /// <param name="projectCollectionUri">The URI of the team project collection.</param>
        /// <param name="projectName">The name of the project in the team project collection.</param>
        /// <returns>The <see cref="ITeamProject"/> object that was created.</returns>
        /// <param name="credentials">The credentials used to access the Team Project, <c>null</c> if the credentials are not available.</param>
        ITeamProject CreateTeamProject(Uri projectCollectionUri, string projectName, ICredentials credentials);

        /// <summary>
        /// Creates an <see cref="IQueryAndLayoutManager"/> object.
        /// </summary>
        /// <param name="projectFields">The fields in the project that the query and layout manager is to work with.</param>
        /// <returns>The <see cref="IQueryAndLayoutManager"/> object that was created.</returns>
        IQueryAndLayoutManager CreateQueryAndLayoutManager(IEnumerable<ITfsFieldDefinition> projectFields);
    }
}
