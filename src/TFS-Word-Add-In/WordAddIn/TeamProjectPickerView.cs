//---------------------------------------------------------------------
// <copyright file="TeamProjectPickerView.cs" company="Microsoft">
//    Copyright © ALM | DevOps Ranger Contributors. All rights reserved.
//    This code is licensed under the MIT License.
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF 
//    ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
//    TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
//    PARTICULAR PURPOSE AND NONINFRINGEMENT.
// </copyright>
// <summary>The TeamProjectPickerView type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.Word4Tfs.WordAddIn
{
    using System;
    using System.Diagnostics;
    using System.Windows.Forms;
    using Microsoft.TeamFoundation.Client;
    using Microsoft.TeamFoundation.Server;
    using Microsoft.Word4Tfs.Library;
    using Microsoft.Word4Tfs.Library.Data;
    using Microsoft.Word4Tfs.Library.Model;
    using Microsoft.Word4Tfs.Library.View;

    /// <summary>
    /// The view which allows the user to select a Team Project.
    /// </summary>
    /// <remarks>
    /// This is a wrapper around the TFS <see cref="TeamProjectPicker"/> class.
    /// </remarks>
    public class TeamProjectPickerView : ITeamProjectPickerView
    {
        /// <summary>
        /// The object to use for logging.
        /// </summary>
        private ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="TeamProjectPickerView"/> class.
        /// </summary>
        /// <param name="logger">The object to be used for logging.</param>
        public TeamProjectPickerView(ILogger logger)
        {
            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }

            this.logger = logger;
        }

        /// <summary>
        /// Displays a dialog to get the user to select the team project collection and the team project.
        /// </summary>
        /// <returns>The selected Team Project, or <c>null</c> if no project was selected.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "The 'ans' object is not meant to be disposed as it is the result of the method call.")]
        public TeamProjectInformation ChooseTeamProject()
        {
            TeamProjectInformation ans = null;
            using (TeamProjectPicker tpp = new TeamProjectPicker(TeamProjectPickerMode.SingleProject, false, new UICredentialsProvider()))
            {
                this.logger.Log(TraceEventType.Verbose, "Showing Team Project Picker dialogue");
                DialogResult result = tpp.ShowDialog();
                if (result == DialogResult.OK)
                {
                    this.logger.Log(TraceEventType.Verbose, "Team Project Picker dialogue OK clicked");
                    TfsTeamProjectCollection projectCollection = tpp.SelectedTeamProjectCollection;
                    ProjectInfo projectInfo = tpp.SelectedProjects[0];
                    ans = new TeamProjectInformation(projectCollection.Uri, projectCollection.InstanceId, projectInfo.Name, projectCollection.Credentials);
                    this.logger.Log(TraceEventType.Verbose, "Using team project collection from project picker for collection at {0} with name {1}", projectCollection.Uri, projectInfo.Name);
                }
                else
                {
                    this.logger.Log(TraceEventType.Verbose, "Team Project Picker dialogue cancelled");
                }
            }

            return ans;
        }

        /// <summary>
        /// Displays a dialog to get the user to select the team project collection.
        /// </summary>
        /// <returns>The <see cref="Uri"/> for the selected Team Project Collection, or <c>null</c> if no Team Project Collection was selected.</returns>
        public Uri ChooseTeamProjectCollection()
        {
            Uri ans = null;
            using (TeamProjectPicker tpp = new TeamProjectPicker(TeamProjectPickerMode.NoProject, false, new UICredentialsProvider()))
            {
                tpp.Text = DialogueResources.RebindToCollection;
                this.logger.Log(TraceEventType.Verbose, "Showing Team Project Picker dialogue");
                DialogResult result = tpp.ShowDialog();
                if (result == DialogResult.OK)
                {
                    this.logger.Log(TraceEventType.Verbose, "Team Project Picker dialogue OK clicked");
                    TfsTeamProjectCollection projectCollection = tpp.SelectedTeamProjectCollection;
                    ans = projectCollection.Uri;
                    this.logger.Log(TraceEventType.Verbose, "Using team project collection from project picker for collection at {0}", projectCollection.Uri);
                }
                else
                {
                    this.logger.Log(TraceEventType.Verbose, "Team Project Picker dialogue cancelled");
                }
            }

            return ans;
        }

        /// <summary>
        /// Sets the button to be enabled or not
        /// </summary>
        /// <param name="button">The button for which the status is to be set.</param>
        /// <param name="enabled">The enabled/disabled status to set.</param>
        public void SetButtonState(Enum button, bool enabled)
        {
        }

        /// <summary>
        /// Displays a message.
        /// </summary>
        /// <param name="messageType">The type of message to display.</param>
        /// <param name="title">The title to be given to the message box.</param>
        /// <param name="message">The message to display.</param>
        /// <param name="details">Optional details to display.</param>
        public void DisplayMessage(UIMessageType messageType, string title, string message, string details)
        {
            Utilities.DisplayMessage(messageType, title, message, details);
        }
    }
}
