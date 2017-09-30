//---------------------------------------------------------------------
// <copyright file="WorkItemQueryPickerWizardPagePresenter.cs" company="Microsoft">
//    Copyright © ALM | DevOps Ranger Contributors. All rights reserved.
//    This code is licensed under the MIT License.
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF 
//    ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
//    TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
//    PARTICULAR PURPOSE AND NONINFRINGEMENT.
// </copyright>
// <summary>The WorkItemQueryPickerWizardPagePresenter type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.Word4Tfs.Library.Presenter
{
    using System;
    using Microsoft.TeamFoundation.WorkItemTracking.Client;
    using Microsoft.Word4Tfs.Library.Model;
    using Microsoft.Word4Tfs.Library.View;

    /// <summary>
    /// Presenter for a wizard page to choose a work item query.
    /// </summary>
    public class WorkItemQueryPickerWizardPagePresenter : WizardPagePresenterBase, IWorkItemQueryPickerWizardPagePresenter
    {
        /// <summary>
        /// The view for this page.
        /// </summary>
        private IWorkItemQueryPickerWizardPageView pageView;

        /// <summary>
        /// The team project from which the queries are to be loaded.
        /// </summary>
        private ITeamProject teamProject;

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkItemQueryPickerWizardPagePresenter"/> class.
        /// </summary>
        /// <param name="wizardView">The overall wizard view.</param>
        /// <param name="pageView">The view for this page.</param>
        /// <param name="teamProject">The team project that supplies the query definitions.</param>
        public WorkItemQueryPickerWizardPagePresenter(IWorkItemQueryAndLayoutPickerWizardView wizardView, IWorkItemQueryPickerWizardPageView pageView, ITeamProject teamProject) : base(wizardView, pageView)
        {
            this.pageView = pageView;
            this.teamProject = teamProject;
        }

        /// <summary>
        /// Gets the title of the page.
        /// </summary>
        public override string Title
        {
            get
            {
                return PresenterResources.WorkItemQueryPickerWizardPageTitle;
            }
        }

        /// <summary>
        /// Gets the definition of the chosen query. Only valid when <see cref="IsValid"/> is true.
        /// </summary>
        public QueryDefinition SelectedQuery { get; private set; }

        /// <summary>
        /// Starts the presenter.
        /// </summary>
        protected override void OnStart()
        {
            this.pageView.QuerySelected += new EventHandler<QueryItemEventArgs>(this.HandleQueryItemSelected);
            this.pageView.LoadQueryHierarchy(this.teamProject.RootQueryFolder);
        }

        /// <summary>
        /// Activates the presenter.
        /// </summary>
        protected override void OnActivate()
        {
        }

        /// <summary>
        /// Commits the presenter.
        /// </summary>
        protected override void OnCommit()
        {
        }

        /// <summary>
        /// Cancels the presenter.
        /// </summary>
        protected override void OnCancel()
        {
            this.pageView.Cancel();
        }

        /// <summary>
        /// Handles the <see cref="IWorkItemQueryPickerWizardPageView.QuerySelected"/> event and sets the page validity according to the type of query selected.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event data</param>
        private void HandleQueryItemSelected(object sender, QueryItemEventArgs e)
        {
            this.SelectedQuery = e.QueryItem as QueryDefinition;
            this.IsValid = e.QueryItem is QueryDefinition;
            if (this.IsValid && this.SelectedQuery.QueryType == QueryType.OneHop)
            {
                this.pageView.DisplayWarning(PresenterResources.OneHopQueryCanBlockRefresh);
            }
            else
            {
                this.pageView.DisplayWarning(string.Empty);
            }

            this.OnValidityChanged();
        }
    }
}
