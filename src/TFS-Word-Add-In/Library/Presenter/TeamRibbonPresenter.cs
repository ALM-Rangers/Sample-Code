//---------------------------------------------------------------------
// <copyright file="TeamRibbonPresenter.cs" company="Microsoft">
//    Copyright (c) Microsoft. All rights reserved.
//    This code is licensed under the MIT License.
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF 
//    ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
//    TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
//    PARTICULAR PURPOSE AND NONINFRINGEMENT.
// </copyright>
// <summary>The TeamRibbonPresenter type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.Word4Tfs.Library.Presenter
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using Microsoft.Practices.Unity;
    using Microsoft.Word4Tfs.Library.Model;
    using Microsoft.Word4Tfs.Library.View;

    /// <summary>
    /// The presenter for the Team Ribbon.
    /// </summary>
    public class TeamRibbonPresenter : PresenterBase, ITeamRibbonPresenter
    {
        /// <summary>
        /// The unity container used to inject dependencies.
        /// </summary>
        private IUnityContainer container;

        /// <summary>
        /// The ribbon this presenter controls.
        /// </summary>
        private ITeamRibbonView ribbon;

        /// <summary>
        /// The layout designer view.
        /// </summary>
        private ILayoutDesignerView designer;

        /// <summary>
        /// The model this class is to use.
        /// </summary>
        private ITeamProjectDocumentManager model;

        /// <summary>
        /// The settings.
        /// </summary>
        private ISettings settings;

        /// <summary>
        /// The callback to be called when the active document must be rebound. The callback must return null to cancel the rebind.
        /// </summary>
        private Func<Uri> rebindingCallback;

        /// <summary>
        /// Initializes a new instance of the <see cref="TeamRibbonPresenter"/> class.
        /// </summary>
        /// <param name="container">The unity container used to inject dependencies.</param>
        /// <param name="ribbon">The ribbon view being controlled by the presenter.</param>
        /// <param name="model">The model to be used.</param>
        /// <param name="settings">The settings.</param>
        public TeamRibbonPresenter(IUnityContainer container, ITeamRibbonView ribbon, ITeamProjectDocumentManager model, ISettings settings)
        {
            if (ribbon == null)
            {
                throw new ArgumentNullException("ribbon");
            }

            if (model == null)
            {
                throw new ArgumentNullException("model");
            }

            if (settings == null)
            {
                throw new ArgumentNullException("settings");
            }

            this.container = container;
            this.ribbon = ribbon;
            this.model = model;
            this.settings = settings;
            this.ribbon.Import += new EventHandler(this.HandleRibbonImport);
            this.ribbon.Refresh += new EventHandler(this.HandleRibbonRefresh);
            this.ribbon.ShowLayoutDesigner += new EventHandler(this.HandleShowLayoutDesigner);
            this.ribbon.HideLayoutDesigner += new EventHandler(this.HandleHideLayoutDesigner);
            this.ribbon.ShowBookmarks += new EventHandler(this.HandleShowBookmarks);
            this.ribbon.HideBookmarks += new EventHandler(this.HandleHideBookmarks);
            this.model.DocumentChanged += new EventHandler(this.HandleDocumentChanged);
            this.model.UserMessage += new EventHandler<UserMessageEventArgs>(this.HandleUserMessage);
        }

        /// <summary>
        /// Raised when the user requests that work items be imported into the document from a team project.
        /// </summary>
        public event EventHandler Import;

        /// <summary>
        /// Raised when the user requests the refresh of work items that have been previously imported into the document from a team project.
        /// </summary>
        public event EventHandler Refresh;

        /// <summary>
        /// Raised when the user requests to see the layout designer.
        /// </summary>
        public event EventHandler ShowLayoutDesigner;

        /// <summary>
        /// Raised when the user requests to hide the layout designer.
        /// </summary>
        public event EventHandler HideLayoutDesigner;

        /// <summary>
        /// Gets or sets the layout designer view.
        /// </summary>
        /// <remarks>
        /// This view is just used to keep the toggle state of the show/hide layout designer button up to date. When the property is <c>null</c> there is no view.
        /// </remarks>
        public ILayoutDesignerView LayoutDesignerView
        {
            get
            {
                return this.designer;
            }

            set
            {
                this.designer = value;
                if (this.designer != null)
                {
                    this.designer.Hidden += new EventHandler(this.HandleLayoutDesignerHidden);
                }
            }
        }

        /// <summary>
        /// Gets the title to be used when displaying errors.
        /// </summary>
        protected override string ErrorTitle
        {
            get
            {
                return PresenterResources.MicrosoftWord;
            }
        }

        /// <summary>
        /// Initialises the presenter.
        /// </summary>
        /// <param name="rebindCallback">The callback to be called when a document rebind is required. The callback must return null to cancel the rebind.</param>
        public void Initialise(Func<Uri> rebindCallback)
        {
            this.rebindingCallback = rebindCallback;
        }

        /// <summary>
        /// Displays an error message.
        /// </summary>
        /// <param name="message">The message to be displayed.</param>
        /// <param name="details">Optional additional details about the error.</param>
        public void DisplayError(string message, string details)
        {
            this.DisplayError(UIMessageType.Error, this.ribbon, message, details);
        }

        /// <summary>
        /// Displays a question to the user which has only a Yes or No answer.
        /// </summary>
        /// <param name="message">The text of the question.</param>
        /// <returns><c>True</c> if the user responds with a Yes, <c>false</c> otherwise.</returns>
        public bool AskYesNoQuestion(string message)
        {
            IMessageBoxView msgbox = this.container.Resolve<IMessageBoxView>();
            return msgbox.Show(message, PresenterResources.MicrosoftWord, MessageBoxViewButtons.YesNo, MessageBoxViewIcon.Warning) == MessageBoxResult.Yes;
        }

        /// <summary>
        /// Loads the document state and initialises the ribbon according to the state.
        /// </summary>
        public void LoadState()
        {
            if (this.model.ActiveDocument != null)
            {
                IEnumerable<string> warnings = this.model.ActiveDocument.Load(this.Rebind);
                if (warnings.Count() > 0)
                {
                    this.DisplayWarning(PresenterResources.DocumentLoadWarnings, string.Join(Environment.NewLine, warnings.ToArray()));
                }
            }

            this.UpdateState();
        }

        /// <summary>
        /// Updates the ribbon based on the current state.
        /// </summary>
        public void UpdateState()
        {
            this.ribbon.SetButtonState(TeamRibbonButton.Import, this.model.ActiveDocument != null && !this.model.ActiveDocument.IsTemporary && this.model.SystemTemplate != null && this.model.SystemTemplate.IsImportable);
            this.ribbon.SetButtonState(TeamRibbonButton.Refresh, this.model.ActiveDocument != null && !this.model.ActiveDocument.IsTemporary && this.model.ActiveDocument.IsRefreshable);
            this.ribbon.SetButtonState(TeamRibbonButton.ShowDesigner, this.model.SystemTemplate != null);

            Nullable<bool> showBookmarks = this.settings.ShowBookmarks;
            if (showBookmarks.HasValue)
            {
                this.ribbon.SetButtonCheckState(TeamRibbonButton.ShowBookmarks, showBookmarks.Value);
            }
            else
            {
                throw new InvalidOperationException(PresenterResources.ShowBookmarksSettingNotSet);
            }
        }

        /// <summary>
        /// Advises the user that a long cancellable operation has been started and allows the user to cancel it.
        /// </summary>
        /// <param name="message">A message to tell the user what is happening.</param>
        /// <param name="cancellationTokenSource">Used to cancel the operation.</param>
        public void StartCancellableOperation(string message, CancellationTokenSource cancellationTokenSource)
        {
            this.ribbon.StartCancellableOperation(PresenterResources.MicrosoftWord, message, cancellationTokenSource);
        }

        /// <summary>
        /// Updates the message associated with the long cancellable operation.
        /// </summary>
        /// <param name="message">The new message to be displayed.</param>
        public void UpdateCancellableOperation(string message)
        {
            this.ribbon.UpdateCancellableOperation(message);
        }

        /// <summary>
        /// Removes the advice to the user about a long cancellable operation.
        /// </summary>
        public void EndCancellableOperation()
        {
            this.ribbon.EndCancellableOperation();
        }

        /// <summary>
        /// Displays a warning message.
        /// </summary>
        /// <param name="message">The message to be displayed.</param>
        /// <param name="details">Optional additional details about the error.</param>
        private void DisplayWarning(string message, string details)
        {
            this.DisplayError(UIMessageType.Warning, this.ribbon, message, details);
        }

        /// <summary>
        /// Callback which is called if a document needs to be rebound.
        /// </summary>
        /// <returns>The new Uri that the document should be bound to. <c>Null</c> to cancel the rebind.</returns>
        private Uri Rebind()
        {
            return this.rebindingCallback();
        }

        /// <summary>
        /// Handles the import event from the view.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event args.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "All exceptions need to be shown to the user.")]
        private void HandleRibbonImport(object sender, EventArgs e)
        {
            try
            {
                if (this.Import != null)
                {
                    this.Import(this, EventArgs.Empty);
                }
            }
            catch (Exception ex)
            {
                this.DisplayError(this.ribbon, ex);
            }
        }

        /// <summary>
        /// Handles the refresh event from the view.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event args.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "All exceptions need to be shown to the user.")]
        private void HandleRibbonRefresh(object sender, EventArgs e)
        {
            try
            {
                if (this.Refresh != null)
                {
                    this.Refresh(this, EventArgs.Empty);
                }
            }
            catch (Exception ex)
            {
                this.DisplayError(this.ribbon, ex);
            }
        }

        /// <summary>
        ///  Handles the <see cref="ITeamProjectDocument.DocumentChanged"/> event from the model.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event args.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "All exceptions need to be shown to the user.")]
        private void HandleDocumentChanged(object sender, EventArgs e)
        {
            try
            {
                this.LoadState();
            }
            catch (Exception ex)
            {
                this.DisplayError(this.ribbon, ex);
            }
        }

        /// <summary>
        /// Handles the <see cref="ITeamProjectDocumentManager.UserMessage"/> event.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The message details.</param>
        private void HandleUserMessage(object sender, UserMessageEventArgs e)
        {
            this.ribbon.DisplayMessage(e.MessageType, this.ErrorTitle, e.Message, null);
        }

        /// <summary>
        /// Handles the <see cref="ITeamRibbon.ShowLayoutDesigner"/> event from the view.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event args.</param>
        private void HandleShowLayoutDesigner(object sender, EventArgs e)
        {
            if (this.ShowLayoutDesigner != null)
            {
                this.ShowLayoutDesigner(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Handles the <see cref="ITeamRibbon.HideLayoutDesigner"/> event from the view.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event args.</param>
        private void HandleHideLayoutDesigner(object sender, EventArgs e)
        {
            if (this.HideLayoutDesigner != null)
            {
                this.HideLayoutDesigner(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Handles the <see cref="ITeamRibbon.ShowBookmarks"/> event from the view.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event args.</param>
        private void HandleShowBookmarks(object sender, EventArgs e)
        {
            this.model.SetShowBookmarkOption(true);
        }

        /// <summary>
        /// Handles the <see cref="ITeamRibbon.HideBookmarks"/> event from the view.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event args.</param>
        private void HandleHideBookmarks(object sender, EventArgs e)
        {
            this.model.SetShowBookmarkOption(false);
        }

        /// <summary>
        /// Handles the <see cref="ILayoutDesignerView.Hidden"/> event.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        private void HandleLayoutDesignerHidden(object sender, EventArgs e)
        {
            this.ribbon.SetButtonCheckState(TeamRibbonButton.ShowDesigner, false);
        }
    }
}
