//---------------------------------------------------------------------
// <copyright file="LayoutPickerWizardPagePresenter.cs" company="Microsoft">
//    Copyright (c) Microsoft. All rights reserved.
//    This code is licensed under the MIT License.
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF 
//    ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
//    TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
//    PARTICULAR PURPOSE AND NONINFRINGEMENT.
// </copyright>
// <summary>The LayoutPickerWizardPagePresenter type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.Word4Tfs.Library.Presenter
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Word4Tfs.Library.Data;
    using Microsoft.Word4Tfs.Library.Model;
    using Microsoft.Word4Tfs.Library.View;

    /// <summary>
    /// Presenter for a wizard page to choose a work item query.
    /// </summary>
    public class LayoutPickerWizardPagePresenter : WizardPagePresenterBase, ILayoutPickerWizardPagePresenter
    {
        /// <summary>
        /// The view for this page.
        /// </summary>
        private ILayoutPickerWizardPageView pageView;

        /// <summary>
        /// The template used to provide the layouts.
        /// </summary>
        private ITeamProjectTemplate template;

        /// <summary>
        /// Initializes a new instance of the <see cref="LayoutPickerWizardPagePresenter"/> class.
        /// </summary>
        /// <param name="wizardView">The overall wizard view.</param>
        /// <param name="pageView">The view for this page.</param>
        /// <param name="template">The template used to provide the layouts.</param>
        public LayoutPickerWizardPagePresenter(IWorkItemQueryAndLayoutPickerWizardView wizardView, ILayoutPickerWizardPageView pageView, ITeamProjectTemplate template) : base(wizardView, pageView)
        {
            this.pageView = pageView;
            this.template = template;
        }

        /// <summary>
        /// Gets the title of the page.
        /// </summary>
        public override string Title
        {
            get
            {
                return PresenterResources.LayoutPickerWizardPageTitle;
            }
        }

        /// <summary>
        /// Gets the selected layout.
        /// </summary>
        public LayoutInformation SelectedLayout { get; private set; }

        /// <summary>
        /// Starts the presenter.
        /// </summary>
        protected override void OnStart()
        {
            IEnumerable<LayoutInformation> sortedLayouts = this.template.Layouts.OrderBy(li => li.Name);
            this.pageView.LoadLayouts(sortedLayouts);
            this.pageView.LayoutSelected += new EventHandler<LayoutItemEventArgs>(this.HandleLayoutSelected);
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
        }

        /// <summary>
        /// Handles the <see cref="ILayoutPickerWizardPageView.LayoutSelected"/> event and sets the button state according to the state of the query selection.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event data</param>
        private void HandleLayoutSelected(object sender, LayoutItemEventArgs e)
        {
            this.SelectedLayout = e.LayoutItem;
            this.IsValid = true;
            this.OnValidityChanged();
        }
    }
}
