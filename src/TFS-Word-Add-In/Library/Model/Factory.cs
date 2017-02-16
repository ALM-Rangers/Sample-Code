//---------------------------------------------------------------------
// <copyright file="Factory.cs" company="Microsoft">
//    Copyright (c) Microsoft. All rights reserved.
//    This code is licensed under the MIT License.
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF 
//    ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
//    TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
//    PARTICULAR PURPOSE AND NONINFRINGEMENT.
// </copyright>
// <summary>The Factory type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.Word4Tfs.Library.Model
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Net;
    using Microsoft.TeamFoundation.Client;
    using Microsoft.Word4Tfs.Library.Model.TeamFoundation;

    /// <summary>
    /// Factory to create objects that have integration dependencies.
    /// </summary>
    /// <remarks>
    /// This factory is used when objects have to be dynamically created and which are not suitable for creation with Unity.
    /// </remarks>
    public class Factory : IFactory
    {
        /// <summary>
        /// The object to use for logging.
        /// </summary>
        private ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="Factory"/> class.
        /// </summary>
        /// <param name="logger">The object to use for logging.</param>
        public Factory(ILogger logger)
        {
            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }

            this.logger = logger;
        }
        
        /// <summary>
        /// Creates an <see cref="ITeamProject"/> object.
        /// </summary>
        /// <param name="projectCollectionUri">The URI of the team project collection.</param>
        /// <param name="projectName">The name of the project in the team project collection.</param>
        /// <param name="credentials">The credentials used to access the Team Project, <c>null</c> if the credentials are not available.</param>
        /// <returns>The <see cref="ITeamProject"/> object that was created.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "The team project collection is disposed when the TeamProject object is disposed.")]
        public ITeamProject CreateTeamProject(Uri projectCollectionUri, string projectName, ICredentials credentials)
        {
            TeamProject ans;

            this.logger.Log(TraceEventType.Verbose, "Creating new team project collection for collection at {0} with name {1}", projectCollectionUri, projectName);
            TfsTeamProjectCollection tpc = null;
            if (credentials != null)
            {
                tpc = new TfsTeamProjectCollection(projectCollectionUri, credentials);
            }
            else
            {
                tpc = new TfsTeamProjectCollection(projectCollectionUri, new TfsClientCredentials());
                tpc.Authenticate();
            }

            ans = new TeamProject(tpc, projectName);
            return ans;
        }

        /// <summary>
        /// Creates an <see cref="IQueryAndLayoutManager"/> object.
        /// </summary>
        /// <param name="projectFields">The fields in the project that the query and layout manager is to work with.</param>
        /// <returns>The <see cref="IQueryAndLayoutManager"/> object that was created.</returns>
        public IQueryAndLayoutManager CreateQueryAndLayoutManager(IEnumerable<ITfsFieldDefinition> projectFields)
        {
            return new QueryAndLayoutManager(projectFields);
        }
    }
}
