//---------------------------------------------------------------------
// <copyright file="TeamProject.cs" company="Microsoft">
//    Copyright (c) Microsoft. All rights reserved.
//    This code is licensed under the MIT License.
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF 
//    ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
//    TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
//    PARTICULAR PURPOSE AND NONINFRINGEMENT.
// </copyright>
// <summary>The TeamProject type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.Word4Tfs.Library.Model
{
    using System;
    using System.Collections.Generic;
    using Microsoft.TeamFoundation.Client;
    using Microsoft.TeamFoundation.WorkItemTracking.Client;
    using Microsoft.Word4Tfs.Library.Data;
    using Microsoft.Word4Tfs.Library.Model.TeamFoundation;

    /// <summary>
    /// Represents a Team Project
    /// </summary>
    public class TeamProject : ITeamProject
    {
        /// <summary>
        /// Track whether <see cref="Dispose"/> has been called.
        /// </summary>
        private bool disposed;

        /// <summary>
        /// The information associated with this team project.
        /// </summary>
        private TeamProjectInformation teamProjectInformation;

        /// <summary>
        /// The name of the project.
        /// </summary>
        private string projectName;

        /// <summary>
        /// The team project collection to use.
        /// </summary>
        private TfsTeamProjectCollection projectCollection;

        /// <summary>
        /// The actual work item store to work with.
        /// </summary>
        private WorkItemStore workItemStore;

        /// <summary>
        /// The actual project.
        /// </summary>
        private Project project;

        /// <summary>
        /// The query runner to use to query the team project.
        /// </summary>
        private TeamProjectQuery queryRunner;

        /// <summary>
        /// Initializes a new instance of the <see cref="TeamProject"/> class.
        /// </summary>
        /// <param name="projectCollection">The team project collection to use.</param>
        /// <param name="projectName">The name of the project in the team project collection that this instance will represent.</param>
        public TeamProject(TfsTeamProjectCollection projectCollection, string projectName)
        {
            if (projectCollection == null)
            {
                throw new ArgumentNullException("projectCollection");
            }

            this.projectCollection = projectCollection;
            this.projectName = projectName;
        }
        
        /// <summary>
        /// Finalizes an instance of the <see cref="TeamProject"/> class.
        /// </summary>
        ~TeamProject()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// Gets the information about the Team Project.
        /// </summary>
        public TeamProjectInformation TeamProjectInformation
        {
            get
            {
                // Create info on first request as it establishes a connection to the TFS instance to get the instance id.
                if (this.teamProjectInformation == null)
                {
                    this.teamProjectInformation = new TeamProjectInformation(this.projectCollection.Uri, this.projectCollection.InstanceId, this.projectName, this.projectCollection.Credentials);
                }

                return this.teamProjectInformation;
            }
        }

        /// <summary>
        /// Gets the root query folder for the Team Project.
        /// </summary>
        public QueryFolder RootQueryFolder
        {
            get
            {
                this.Connect();
                return this.project.QueryHierarchy;
            }
        }

        /// <summary>
        /// Gets the object needed to run queries against the Team Project.
        /// </summary>
        public ITeamProjectQuery QueryRunner
        {
            get
            {
                this.Connect();
                return this.queryRunner;
            }
        }

        /// <summary>
        /// Gets the list of fields defined for this Team Project.
        /// </summary>
        public IEnumerable<ITfsFieldDefinition> FieldDefinitions
        {
            get
            {
                this.Connect();
                List<ITfsFieldDefinition> ans = new List<ITfsFieldDefinition>();
                foreach (FieldDefinition fd in this.workItemStore.FieldDefinitions)
                {
                    ans.Add(new TfsFieldDefinition(fd));
                }

                return ans;
            }
        }

        /// <summary>
        /// Disposes of the object and any resources it holds.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Performs actual dispose.
        /// </summary>
        /// <param name="disposing"><c>true</c> if <see cref="Dispose"/> has been called explicitly, <c>false</c> if disposal is being done by the finalizer.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    // Dispose managed resources.
                    this.projectCollection.Dispose();
                }

                // Dispose of unmanaged resources here.

                // Note disposing has been done.
                this.disposed = true;
            }
        }

        /// <summary>
        /// Connects to TFS, if not already connected.
        /// </summary>
        private void Connect()
        {
            if (this.workItemStore == null)
            {
                this.workItemStore = new WorkItemStore(this.projectCollection);
                this.project = this.workItemStore.Projects[this.TeamProjectInformation.ProjectName];
                this.queryRunner = new TeamProjectQuery(this.TeamProjectInformation.ProjectName, new TfsQueryFactory(this.workItemStore));
            }
        }
    }
}
