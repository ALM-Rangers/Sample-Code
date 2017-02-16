//---------------------------------------------------------------------
// <copyright file="WizardPresenterBase.cs" company="Microsoft">
//    Copyright (c) Microsoft. All rights reserved.
//    This code is licensed under the MIT License.
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF 
//    ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
//    TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
//    PARTICULAR PURPOSE AND NONINFRINGEMENT.
// </copyright>
// <summary>The WizardPresenterBase type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.Word4Tfs.Library.Presenter
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.TeamFoundation;
    using Microsoft.Word4Tfs.Library.View;

    /// <summary>
    /// The base class for wizard presenters.
    /// </summary>
    /// <remarks>
    /// This class manages a list of presenters for each of the pages of the wizard.
    /// </remarks>
    /// <typeparam name="T">The interface for the wizard view.</typeparam>
    public abstract class WizardPresenterBase<T> : PresenterBase, IWizardPresenterBase where T : IWizardViewBase
    {
        /// <summary>
        /// Indicates if the wizard has already been started.
        /// </summary>
        private bool started;

        /// <summary>
        /// The view that represents the wizard as a whole.
        /// </summary>
        private T wizardView;

        /// <summary>
        /// The index in <see cref="pagePresenters"/> of the current page.
        /// </summary>
        private int currentPageIndex;

        /// <summary>
        /// The page presenters that make up the pages in the wizard.
        /// </summary>
        private IWizardPagePresenter[] pagePresenters = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="WizardPresenterBase&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="wizardView">The view that represents the wizard as a whole.</param>
        protected WizardPresenterBase(T wizardView)
        {
            this.wizardView = wizardView;
        }

        /// <summary>
        /// Initialises the wizard presenter and all the page presenters.
        /// </summary>
        /// <remarks>
        /// Implementers must call the protected <see cref="OnInitialise"/> method to supply the page presenters.
        /// </remarks>
        public abstract void Initialise();

        /// <summary>
        /// Starts the wizard on the first page.
        /// </summary>
        /// <remarks>
        /// <see cref="Initialise"/> must have been called first.
        /// </remarks>
        /// <returns><c>True</c> if the wizard is finished, <c>false</c> if the wizard is cancelled.</returns>
        public bool Start()
        {
            if (this.started)
            {
                throw new InvalidOperationException(PresenterResources.WizardAlreadyStarted);
            }

            this.started = true;

            bool ans = false;
            if (this.pagePresenters == null)
            {
                throw new InvalidOperationException(PresenterResources.WizardNotInitialized);
            }

            try
            {
                foreach (IWizardPagePresenter pagePresenter in this.pagePresenters)
                {
                    pagePresenter.Start();
                }

                this.pagePresenters[0].Activate();
                this.wizardView.Title = this.pagePresenters[0].Title;
                this.SetButtonStates();
                ans = this.wizardView.StartDialog();

                if (ans)
                {
                    foreach (IWizardPagePresenter pagePresenter in this.pagePresenters)
                    {
                        pagePresenter.Commit();
                    }

                    this.OnCommit();
                }
                else
                {
                    foreach (IWizardPagePresenter pagePresenter in this.pagePresenters)
                    {
                        pagePresenter.Cancel();
                    }
                }
            }
            catch (TeamFoundationServerException ex)
            {
                this.DisplayError(this.wizardView, ex);
            }

            return ans;
        }

        /// <summary>
        /// Initialises the wizard presenter with all the presenter pages.
        /// </summary>
        /// <param name="wizardPagePresenters">The presenters for each page of the wizard. The sequence is the order the pages are presented in the wizard.</param>
        protected void OnInitialise(IEnumerable<IWizardPagePresenter> wizardPagePresenters)
        {
            if (this.pagePresenters != null)
            {
                throw new InvalidOperationException(PresenterResources.WizardAlreadyInitialized);
            }

            this.pagePresenters = wizardPagePresenters.ToArray();
            if (this.pagePresenters.Length <= 0)
            {
                throw new InvalidOperationException(PresenterResources.WizardNeedsNonEmptyPagePresenterList);
            }

            this.wizardView.Initialize();
            this.currentPageIndex = 0;

            foreach (IWizardPagePresenter pagePresenter in this.pagePresenters)
            {
                pagePresenter.Initialise();
                pagePresenter.ValidityChanged += new EventHandler(this.HandlePresenterValidityChanged);
            }

            this.wizardView.PreviousPage += new EventHandler(this.HandlePrevious);
            this.wizardView.NextPage += new EventHandler(this.HandleNext);
        }

        /// <summary>
        /// Commits the wizard as a whole.
        /// </summary>
        /// <remarks>
        /// Called after the page presenters have been committed.
        /// </remarks>
        protected abstract void OnCommit();

        /// <summary>
        /// Handles the event raised when the user clicks on the Previous button on the wizard view.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        private void HandlePrevious(object sender, EventArgs e)
        {
            this.MoveToPage(this.currentPageIndex - 1);
        }

        /// <summary>
        /// Handles the event raised when the user clicks on the Next button on the wizard view.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        private void HandleNext(object sender, EventArgs e)
        {
            this.MoveToPage(this.currentPageIndex + 1);
        }

        /// <summary>
        /// Moves to a different page.
        /// </summary>
        /// <param name="newPageIndex">The index of the new page to move to.</param>
        private void MoveToPage(int newPageIndex)
        {
            this.pagePresenters[this.currentPageIndex].Deactivate();
            this.currentPageIndex = newPageIndex;
            this.pagePresenters[this.currentPageIndex].Activate();
            this.wizardView.Title = this.pagePresenters[this.currentPageIndex].Title;
            this.SetButtonStates();
        }

        /// <summary>
        /// Sets the button states.
        /// </summary>
        private void SetButtonStates()
        {
            this.wizardView.SetButtonState(WizardButton.Cancel, true);
            this.wizardView.SetButtonState(WizardButton.Previous, this.currentPageIndex > 0);
            this.wizardView.SetButtonState(WizardButton.Next, this.pagePresenters[this.currentPageIndex].IsValid && this.currentPageIndex < this.pagePresenters.Length - 1);
            this.wizardView.SetButtonState(WizardButton.Finish, this.pagePresenters.All(p => p.IsValid));
        }

        /// <summary>
        /// Handles a change in the validity of a page.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        private void HandlePresenterValidityChanged(object sender, EventArgs e)
        {
            this.SetButtonStates();
        }
    }
}
