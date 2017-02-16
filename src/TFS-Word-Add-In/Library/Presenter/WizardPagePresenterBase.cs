//---------------------------------------------------------------------
// <copyright file="WizardPagePresenterBase.cs" company="Microsoft">
//    Copyright (c) Microsoft. All rights reserved.
//    This code is licensed under the MIT License.
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF 
//    ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
//    TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
//    PARTICULAR PURPOSE AND NONINFRINGEMENT.
// </copyright>
// <summary>The WizardPagePresenterBase type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.Word4Tfs.Library.Presenter
{
    using System;
    using System.Collections.Generic;
    using System.Text;
using Microsoft.Word4Tfs.Library.View;

    /// <summary>
    /// Base implementation of a wizard page
    /// </summary>
    public abstract class WizardPagePresenterBase : IWizardPagePresenter
    {
        /// <summary>
        /// Indicates if the presenter has been initialised.
        /// </summary>
        private bool initialised;

        /// <summary>
        /// Indicates if the presenter has been started.
        /// </summary>
        private bool started;

        /// <summary>
        /// The view for this page.
        /// </summary>
        private IWizardPageView pageView;

        /// <summary>
        /// Initializes a new instance of the <see cref="WizardPagePresenterBase"/> class.
        /// </summary>
        /// <param name="wizardView">The overall wizard view.</param>
        /// <param name="pageView">The view for this page.</param>
        protected WizardPagePresenterBase(IWizardViewBase wizardView, IWizardPageView pageView)
        {
            this.WizardView = wizardView;
            this.pageView = pageView;
        }

        /// <summary>
        /// Raised when the validity of the page has changed.
        /// </summary>
        public event EventHandler ValidityChanged;

        /// <summary>
        /// Gets the title of the page.
        /// </summary>
        public abstract string Title { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the page is valid and navigation can proceed to the next page, or finish.
        /// </summary>
        public bool IsValid { get; protected set; }

        /// <summary>
        /// Gets the overall wizard view.
        /// </summary>
        protected IWizardViewBase WizardView { get; private set; }

        /// <summary>
        /// Initialises the wizard page presenter.
        /// </summary>
        /// <remarks>
        /// This is where the presenter adds the page view to the list of pages managed by the overall wizard view.
        /// </remarks>
        public void Initialise()
        {
            this.WizardView.AddPage(this.pageView);
            this.pageView.Hide();
            this.initialised = true;
        }

        /// <summary>
        /// Makes the wizard page presenter instruct the view to start, for example by loading data.
        /// </summary>
        public void Start()
        {
            if (!this.initialised)
            {
                throw new InvalidOperationException(PresenterResources.PagePresenterNotInitialized);
            }

            this.OnStart();

            this.started = true;
        }

        /// <summary>
        /// Activates the wizard page presenter so that the page is the active one.
        /// </summary>
        /// <remarks>This does not deactivate any other pages.</remarks>
        public void Activate()
        {
            if (!this.started)
            {
                throw new InvalidOperationException(PresenterResources.PagePresenterNotStarted);
            }

            this.OnActivate();

            this.pageView.Show();
        }

        /// <summary>
        /// Deactivates the wizard page presenter so that the page is no longer the active one.
        /// </summary>
        /// <remarks>This does not activate any other pages.</remarks>
        public void Deactivate()
        {
            if (!this.started)
            {
                throw new InvalidOperationException(PresenterResources.PagePresenterNotStarted);
            }

            this.pageView.Hide();
        }

        /// <summary>
        /// Commits the changes made by the user.
        /// </summary>
        public void Commit()
        {
            if (!this.started)
            {
                throw new InvalidOperationException(PresenterResources.PagePresenterNotStarted);
            }

            this.OnCommit();
        }

        /// <summary>
        /// Cancels the page.
        /// </summary>
        public void Cancel()
        {
            if (!this.started)
            {
                throw new InvalidOperationException(PresenterResources.PagePresenterNotStarted);
            }

            this.OnCancel();
        }

        /// <summary>
        /// Raises the <see cref="ValidityChanged"/> event.
        /// </summary>
        protected void OnValidityChanged()
        {
            if (this.ValidityChanged != null)
            {
                this.ValidityChanged(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Called to start the derived presenter.
        /// </summary>
        protected abstract void OnStart();

        /// <summary>
        /// Called to activate the derived presenter.
        /// </summary>
        protected abstract void OnActivate();

        /// <summary>
        /// Called to commit the derived presenter.
        /// </summary>
        protected abstract void OnCommit();

        /// <summary>
        /// Called to start the derived presenter.
        /// </summary>
        protected abstract void OnCancel();
    }
}
