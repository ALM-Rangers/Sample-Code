//---------------------------------------------------------------------
// <copyright file="TeamProjectInformation.cs" company="Microsoft">
//    Copyright (c) Microsoft. All rights reserved.
//    This code is licensed under the MIT License.
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF 
//    ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
//    TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
//    PARTICULAR PURPOSE AND NONINFRINGEMENT.
// </copyright>
// <summary>The TeamProjectInformation type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.Word4Tfs.Library.Data
{
    using System;
    using System.Net;

    /// <summary>
    /// Information about a Team Project.
    /// </summary>
    public class TeamProjectInformation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TeamProjectInformation"/> class.
        /// </summary>
        /// <param name="collectionUri">The team project collection URI.</param>
        /// <param name="collectionId">The id of the team project collection.</param>
        /// <param name="projectName">The name of the Team Project within the team project collection.</param>
        /// <param name="credentials">The credentials used to access the Team Project, may be <c>null</c> if the credentials are not available.</param>
        public TeamProjectInformation(Uri collectionUri, Guid collectionId, string projectName, ICredentials credentials)
        {
            this.CollectionUri = collectionUri;
            this.CollectionId = collectionId;
            this.ProjectName = projectName;
            this.Credentials = credentials;
        }

        /// <summary>
        /// Gets the project collection Uri.
        /// </summary>
        public Uri CollectionUri { get; private set; }

        /// <summary>
        /// Gets the project collection id.
        /// </summary>
        public Guid CollectionId { get; private set; }

        /// <summary>
        /// Gets the name of the Team Project.
        /// </summary>
        public string ProjectName { get; private set; }

        /// <summary>
        /// Gets the credentials used to access the Team Project, may be <c>null</c> if the credentials are not available.
        /// </summary>
        public ICredentials Credentials { get; private set; }
    }
}
