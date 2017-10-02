//---------------------------------------------------------------------
// <copyright file="WorkItemQueryAndLayoutPickerWizardPresenter.cs" company="Microsoft">
//    Copyright © ALM | DevOps Ranger Contributors. All rights reserved.
//    This code is licensed under the MIT License.
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF 
//    ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
//    TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
//    PARTICULAR PURPOSE AND NONINFRINGEMENT.
// </copyright>
// <summary>The WorkItemQueryAndLayoutPickerWizardPresenter type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.Word4Tfs.Library.Presenter
{
    using System;
    using Microsoft.Practices.Unity;
    using Microsoft.Word4Tfs.Library.Data;
    using Microsoft.Word4Tfs.Library.Model;
    using Microsoft.Word4Tfs.Library.View;

    /// <summary>
    /// The presenter for the wizard that lets the user choose a query and a layout.
    /// </summary>
    public class WorkItemQueryAndLayoutPickerWizardPresenter : WizardPresenterBase<IWorkItemQueryAndLayoutPickerWizardView>, IWorkItemQueryAndLayoutPickerWizardPresenter
    {
        /// <summary>
        /// The team project document used to store the query and layout.
        /// </summary>
        private ITeamProjectDocument projectDocument;

        /// <summary>
        /// The query page presenter.
        /// </summary>
        private IWorkItemQueryPickerWizardPagePresenter queryPagePresenter;

        /// <summary>
        /// The layout page presenter.
        /// </summary>
        private ILayoutPickerWizardPagePresenter layoutPagePresenter;

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkItemQueryAndLayoutPickerWizardPresenter"/> class.
        /// </summary>
        /// <param name="projectDocument">The team project document used to store the query and layout.</param>
        /// <param name="wizardView">The wizard view.</param>
        /// <param name="queryPagePresenter">The presenter to use for the query picker page.</param>
        /// <param name="layoutPagePresenter">The presenter to use for the layout picker page.</param>
        public WorkItemQueryAndLayoutPickerWizardPresenter(ITeamProjectDocument projectDocument, IWorkItemQueryAndLayoutPickerWizardView wizardView, IWorkItemQueryPickerWizardPagePresenter queryPagePresenter, ILayoutPickerWizardPagePresenter layoutPagePresenter) : base(wizardView)
        {
            this.projectDocument = projectDocument;
            this.queryPagePresenter = queryPagePresenter;
            this.layoutPagePresenter = layoutPagePresenter;
        }

        /// <summary>
        /// Gets the definition of the query and the associated layout to go into the document.
        /// </summary>
        public QueryAndLayoutInformation QueryAndLayout { get; private set; }

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
        /// Initialises the wizard presenter and all the page presenters.
        /// </summary>
        public override void Initialise()
        {
            this.OnInitialise(new IWizardPagePresenter[] { this.queryPagePresenter, this.layoutPagePresenter });
        }

        /// <summary>
        /// Saves the query and layout in the document.
        /// </summary>
        public void SaveQueryAndLayout()
        {
            if (this.QueryAndLayout == null)
            {
                throw new InvalidOperationException(PresenterResources.ChooseQueryAndLayoutBeforeSave);
            }

            this.projectDocument.SaveQueriesAndLayouts();
        }

        /// <summary>
        /// Commits the wizard as a whole.
        /// </summary>
        protected override void OnCommit()
        {
            this.QueryAndLayout = this.projectDocument.AddQueryAndLayout(new QueryAndLayoutInformation(this.queryPagePresenter.SelectedQuery, this.layoutPagePresenter.SelectedLayout));
        }
    }
}
