//---------------------------------------------------------------------
// <copyright file="TeamProjectPickerPresenter.cs" company="Microsoft">
//    Copyright (c) Microsoft. All rights reserved.
//    This code is licensed under the MIT License.
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF 
//    ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
//    TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
//    PARTICULAR PURPOSE AND NONINFRINGEMENT.
// </copyright>
// <summary>The TeamProjectPickerPresenter type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.Word4Tfs.Library.Presenter
{
    using System;
    using Microsoft.Word4Tfs.Library.Data;
    using Microsoft.Word4Tfs.Library.Model;
    using Microsoft.Word4Tfs.Library.View;

    /// <summary>
    /// The presenter for the Team Project Picker
    /// </summary>
    public class TeamProjectPickerPresenter : PresenterBase, ITeamProjectPickerPresenter
    {
        /// <summary>
        /// The factory used to create other objects.
        /// </summary>
        private readonly IFactory factory;

        /// <summary>
        /// The picker view to be used.
        /// </summary>
        private readonly ITeamProjectPickerView pickerView;

        /// <summary>
        /// The team project document to be used.
        /// </summary>
        private readonly ITeamProjectDocument projectDocument;

        /// <summary>
        /// The Team Project that has been chosen by <see cref="ChooseTeamProject"/>.
        /// </summary>
        private ITeamProject chosenTeamProject;

        /// <summary>
        /// The wait notifier to be used to tell the user if there is a wait.
        /// </summary>
        private IWaitNotifier waitNotifier;

        /// <summary>
        /// Initializes a new instance of the <see cref="TeamProjectPickerPresenter"/> class.
        /// </summary>
        /// <param name="pickerView">The team project picker the presenter controls</param>
        /// <param name="projectDocument">The team project document the presenter uses for the data.</param>
        /// <param name="factory">The factory used to create other objects.</param>
        /// <param name="waitNotifier">The wait notifier to be used to tell the user if there is a wait.</param>
        public TeamProjectPickerPresenter(ITeamProjectPickerView pickerView, ITeamProjectDocument projectDocument, IFactory factory, IWaitNotifier waitNotifier)
        {
            if (pickerView == null)
            {
                throw new ArgumentNullException("pickerView");
            }

            if (projectDocument == null)
            {
                throw new ArgumentNullException("projectDocument");
            }

            if (factory == null)
            {
                throw new ArgumentNullException("factory");
            }

            if (waitNotifier == null)
            {
                throw new ArgumentNullException("waitNotifier");
            }

            this.pickerView = pickerView;
            this.projectDocument = projectDocument;
            this.factory = factory;
            this.waitNotifier = waitNotifier;
        }

        /// <summary>
        /// Gets the title to be used when displaying errors.
        /// </summary>
        protected override string ErrorTitle
        {
            get
            {
                return PresenterResources.TeamFoundationError;
            }
        }

        /// <summary>
        /// Gets the team and project information.
        /// </summary>
        /// <returns>The information for the selected team and project, <c>null</c> if none selected.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "All exceptions need to be shown to the user.")]
        public ITeamProject ChooseTeamProject()
        {
            try
            {
                TeamProjectInformation tpi = this.pickerView.ChooseTeamProject();

                this.waitNotifier.StartWait();

                if (tpi == null)
                {
                    this.chosenTeamProject = null;
                }
                else
                {
                    this.chosenTeamProject = this.factory.CreateTeamProject(tpi.CollectionUri, tpi.ProjectName, tpi.Credentials);
                }

                this.projectDocument.TeamProject = this.chosenTeamProject;
            }
            catch (Exception ex)
            {
                this.DisplayError(this.pickerView, ex);
            }
            finally
            {
                this.waitNotifier.EndWait();
            }

            return this.chosenTeamProject;
        }

        /// <summary>
        /// Chooses a team project collection.
        /// </summary>
        /// <remarks>Used when rebinding a document.</remarks>
        /// <returns>The <see cref="Uri"/> of the team project collection.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "All exceptions need to be shown to the user.")]
        public Uri ChooseTeamProjectCollection()
        {
            Uri ans = null;
            try
            {
                ans = this.pickerView.ChooseTeamProjectCollection();
            }
            catch (Exception ex)
            {
                this.DisplayError(this.pickerView, ex);
            }

            return ans;
        }

        /// <summary>
        /// Saves the team and project information in the model.
        /// </summary>
        public void SaveTeamProject()
        {
            if (this.chosenTeamProject == null)
            {
                throw new InvalidOperationException(PresenterResources.ChooseProjectBeforeSave);
            }

            this.projectDocument.SaveTeamProject();
        }
    }
}
