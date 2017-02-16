//---------------------------------------------------------------------
// <copyright file="LayoutDesignerPresenter.cs" company="Microsoft">
//    Copyright (c) Microsoft. All rights reserved.
//    This code is licensed under the MIT License.
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF 
//    ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
//    TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
//    PARTICULAR PURPOSE AND NONINFRINGEMENT.
// </copyright>
// <summary>The LayoutDesignerPresenter type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.Word4Tfs.Library.Presenter
{
    using global::System;
    using global::System.Collections.Generic;
    using global::System.ComponentModel;
    using global::System.Globalization;
    using global::System.Linq;
    using Microsoft.Office.Interop.Word;
    using Microsoft.Practices.Unity;
    using Microsoft.Word4Tfs.Library.Data;
    using Microsoft.Word4Tfs.Library.Model;
    using Microsoft.Word4Tfs.Library.View;

    /// <summary>
    /// The presenter for the layout designer.
    /// </summary>
    public class LayoutDesignerPresenter : PresenterBase, ILayoutDesignerPresenter
    {
        /// <summary>
        /// The unity container used to inject dependencies.
        /// </summary>
        private IUnityContainer container;

        /// <summary>
        /// The view being controlled by the presenter.
        /// </summary>
        private ILayoutDesignerView view;

        /// <summary>
        /// The team ribbon presenter used to show the visible/hidden status of the layout designer.
        /// </summary>
        private ITeamRibbonPresenter ribbonPresenter;

        /// <summary>
        /// The team project document manager to be used.
        /// </summary>
        private ITeamProjectDocumentManager manager;

        /// <summary>
        /// The temporary team project document used to host the layouts while they are being designed.
        /// </summary>
        private ITeamProjectDocument designerDocument;

        /// <summary>
        ///  The current layout the user is working on.
        /// </summary>
        private LayoutInformation currentLayout;

        /// <summary>
        /// Temporary layout to add to the list of layouts, used when adding a new layout, before it is committed to the template.
        /// </summary>
        private LayoutInformation temporaryLayout;

        /// <summary>
        /// If we have a pending rename this is set to the new name.
        /// </summary>
        private string pendingRenameTo;

        /// <summary>
        /// Initializes a new instance of the <see cref="LayoutDesignerPresenter"/> class.
        /// </summary>
        /// <param name="container">The unity container used to inject dependencies.</param>
        /// <param name="view">The view being controlled by the presenter.</param>
        /// <param name="manager">The team project document manager to be used.</param>
        /// <param name="ribbonPresenter">The team ribbon presenter.</param>
        public LayoutDesignerPresenter(IUnityContainer container, ILayoutDesignerView view, ITeamProjectDocumentManager manager, ITeamRibbonPresenter ribbonPresenter)
        {
            if (view == null)
            {
                throw new ArgumentNullException("view");
            }

            if (manager == null)
            {
                throw new ArgumentNullException("manager");
            }

            if (ribbonPresenter == null)
            {
                throw new ArgumentNullException("ribbonPresenter");
            }

            this.container = container;
            this.view = view;
            this.manager = manager;
            this.ribbonPresenter = ribbonPresenter;

            this.ribbonPresenter.LayoutDesignerView = view;
            this.view.Save += new EventHandler(this.HandleSave);
            this.view.Connect += new EventHandler(this.HandleConnect);
            this.view.AddField += new EventHandler<FieldDefinitionEventArgs>(this.HandleAddField);
            this.view.LayoutSelected += new EventHandler<LayoutItemEventArgs>(this.HandleLayoutSelected);
            this.view.AddNewLayout += new EventHandler(this.HandleAddNewLayout);
            this.view.LayoutRename += new EventHandler<RenameEventArgs>(this.HandleLayoutRename);
            this.view.LayoutDelete += new EventHandler<LayoutItemEventArgs>(this.HandleLayoutDelete);
            this.manager.DocumentBeforeClose += new CancelEventHandler(this.HandleDocumentBeforeClose);
        }

        /// <summary>
        /// Raised when the presenter needs the controller to connect the document to TFS.
        /// </summary>
        public event EventHandler Connect;

        /// <summary>
        /// Raised when the designer document is closed.
        /// </summary>
        public event EventHandler Close;

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
        /// Shows the layout designer.
        /// </summary>
        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "All exceptions need to be shown to the user.")]
        public void Show()
        {
            try
            {
                if (this.designerDocument == null)
                {
                    ITeamProject existingConnection = null;
                    if (this.manager.ActiveDocument != null && this.manager.ActiveDocument.IsConnected)
                    {
                        existingConnection = this.manager.ActiveDocument.TeamProject;
                    }

                    this.designerDocument = this.manager.Add(true);
                    this.designerDocument.Close += new EventHandler(this.HandleDesignerDocumentClose);
                    if (existingConnection != null)
                    {
                        this.designerDocument.TeamProject = existingConnection;
                        this.designerDocument.SaveTeamProject();
                        this.ConnectDesignerDocument(true);
                    }
                }

                this.SetEditorControls();

                this.SetViewLayoutList();
                LayoutInformation layout = this.GetViewOrderedLayoutList().FirstOrDefault();
                if (layout != null)
                {
                    this.ChangeDisplayedLayout(layout);
                }

                this.view.ShowLayoutDesigner();
            }
            catch (Exception ex)
            {
                this.DisplayError(this.view, ex);
            }
        }

        /// <summary>
        /// Hides the layout designer.
        /// </summary>
        public void Hide()
        {
            this.view.HideLayoutDesigner();
        }

        /// <summary>
        /// Handles the <see cref="ILayoutDesignerView.Save"/> event.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "All exceptions need to be shown to the user.")]
        private void HandleSave(object sender, EventArgs e)
        {
            try
            {
                this.view.SetStatus(string.Format(CultureInfo.CurrentCulture, PresenterResources.SavingLayout, this.currentLayout.Name), 0);
                this.SaveChanges();
                this.view.SetStatus(string.Format(CultureInfo.CurrentCulture, PresenterResources.SavedLayout, this.currentLayout.Name), 3);

                if (this.CurrentLayoutIsTemporary())
                {
                    this.RemoveTemporaryLayout();
                }
            }
            catch (Exception ex)
            {
                this.view.SetStatus(string.Empty, 0);
                this.DisplayError(this.view, ex);
            }
        }

        /// <summary>
        /// Handles the <see cref="ITeamProjectDocument.Close"/> event.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        private void HandleDesignerDocumentClose(object sender, EventArgs e)
        {
            if (this.Close != null)
            {
                this.Close(this, EventArgs.Empty);
            }

            this.ribbonPresenter.LayoutDesignerView = null;
            this.view.Close();
        }

        /// <summary>
        /// Handles the <see cref="ILayoutDesignerView.Connect"/> event.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "All exceptions need to be shown to the user.")]
        private void HandleConnect(object sender, EventArgs e)
        {
            try
            {
                bool clean = !this.designerDocument.HasChanged;
                if (this.Connect != null)
                {
                    this.Connect(this, EventArgs.Empty);
                }

                if (this.designerDocument.IsConnected)
                {
                    this.ConnectDesignerDocument(clean);
                }
            }
            catch (Exception ex)
            {
                this.DisplayError(this.view, ex);
            }
        }

        /// <summary>
        /// Handles the <see cref="ILayoutDesignerView.AddField"/> event.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "All exceptions need to be shown to the user.")]
        private void HandleAddField(object sender, FieldDefinitionEventArgs e)
        {
            try
            {
                this.designerDocument.AddField(e.FieldDefinition);
            }
            catch (Exception ex)
            {
                this.DisplayError(this.view, ex);
            }
        }

        /// <summary>
        /// Handles the <see cref="ILayoutDesignerView.AddNewLayout"/> event.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "All exceptions need to be shown to the user.")]
        private void HandleAddNewLayout(object sender, EventArgs e)
        {
            try
            {
                bool confirmed = this.CheckForChangesAndConfirmDisposition(new CancelEventArgs());
                if (confirmed)
                {
                    this.designerDocument.AddPrototypeLayoutDefinition();
                    string layoutName = this.GetNewLayoutName();
                    this.temporaryLayout = new LayoutInformation(layoutName, new BuildingBlockName[0], new string[0], null);
                    this.SetViewLayoutList();
                    this.SetCurrentLayout(layoutName);
                    this.view.StartLayoutNameEdit(layoutName);
                    this.SetEditorControls();
                }
            }
            catch (Exception ex)
            {
                this.DisplayError(this.view, ex);
            }
        }

        /// <summary>
        /// Handles the <see cref="ILayoutDesignerView.LayoutRename"/> event.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        private void HandleLayoutRename(object sender, RenameEventArgs e)
        {
            e.Cancel = this.GetLayoutList().Any(li => li.Name == e.NewName);
            if (!e.Cancel)
            {
                this.pendingRenameTo = e.NewName;
            }
        }

        /// <summary>
        /// Handles the <see cref="ILayoutDesignerView.LayoutDelete"/> event.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        private void HandleLayoutDelete(object sender, LayoutItemEventArgs e)
        {
            IMessageBoxView messageBox = this.container.Resolve<IMessageBoxView>();
            MessageBoxResult result = messageBox.Show(string.Format(CultureInfo.InvariantCulture, PresenterResources.ConfirmLayoutDeletion, this.SaveLayoutName()), PresenterResources.MicrosoftWord, MessageBoxViewButtons.YesNo, MessageBoxViewIcon.Warning);
            if (result == MessageBoxResult.Yes)
            {
                int index = 0;
                LayoutInformation[] layouts = this.GetLayoutList().ToArray();
                for (int i = 0; i < layouts.Length; i++)
                {
                    if (e.LayoutItem.Name == layouts[i].Name)
                    {
                        index = i;
                        break;
                    }
                }

                int nextIndex = -1;
                if (index < layouts.Length - 1)
                {
                    nextIndex = index + 1;
                }
                else if (index >= layouts.Length - 1)
                {
                    nextIndex = index - 1;
                }

                this.manager.SystemTemplate.DeleteLayout(e.LayoutItem.Name);
                this.manager.SystemTemplate.Save();
                if (this.CurrentLayoutIsTemporary())
                {
                    this.RemoveTemporaryLayout();
                }

                this.SetDocumentClean();
                this.SetViewLayoutList();

                if (nextIndex != -1)
                {
                    this.ChangeDisplayedLayout(layouts[nextIndex]);
                }

                this.SetEditorControls();
            }
        }

        /// <summary>
        /// Completes the connection of the designer document.
        /// </summary>
        /// <param name="wasCleanBeforeConnection">Indicates if the document was clean before it was connected.</param>
        private void ConnectDesignerDocument(bool wasCleanBeforeConnection)
        {
            this.SetEditorControls();
            this.view.SetFieldList(this.designerDocument.TeamProject.FieldDefinitions.OrderBy(fd => fd.FriendlyName));
            if (wasCleanBeforeConnection)
            {
                this.designerDocument.MarkDocumentClean();
            }
        }

        /// <summary>
        /// Returns a value indicating whether the current layout is the temporary layout.
        /// </summary>
        /// <returns>A value indicating whether the current layout is the temporary layout.</returns>
        private bool CurrentLayoutIsTemporary()
        {
            return this.temporaryLayout == this.currentLayout;
        }

        /// <summary>
        /// The temporary layout is removed.
        /// </summary>
        private void RemoveTemporaryLayout()
        {
            this.temporaryLayout = null;
            this.SetViewLayoutList();
            this.SetEditorControls();
        }

        /// <summary>
        /// Allocates a new layout name, guaranteeing that it does not already exist
        /// </summary>
        /// <returns>The new layout name.</returns>
        private string GetNewLayoutName()
        {
            string ans = Constants.PrototypeLayoutName;
            string[] currentNames = this.GetLayoutList().Select(li => li.Name).ToArray();
            int i = 1;
            while (currentNames.Any(n => n == ans))
            {
                i++;
                ans = string.Format(CultureInfo.InvariantCulture, "{0} {1}", Constants.PrototypeLayoutName, i);
            }

            return ans;
        }

        /// <summary>
        /// Sets the named layout as the current layout.
        /// </summary>
        /// <param name="layoutName">The name of the layout to be made current.</param>
        private void SetCurrentLayout(string layoutName)
        {
            this.currentLayout = this.GetLayoutList().Where(li => li.Name == layoutName).Single();
            this.view.SelectLayout(layoutName);
        }

        /// <summary>
        /// Handles the <see cref="ILayoutDesignerView.LayoutSelected"/> event.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The information about the selected layout.</param>
        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "All exceptions need to be shown to the user.")]
        private void HandleLayoutSelected(object sender, LayoutItemEventArgs e)
        {
            try
            {
                bool confirmed = this.CheckForChangesAndConfirmDisposition(e);
                if (confirmed)
                {
                    if (this.CurrentLayoutIsTemporary() && e.LayoutItem != this.temporaryLayout)
                    {
                        this.RemoveTemporaryLayout();
                    }

                    this.ChangeDisplayedLayout(e.LayoutItem);
                }
                else
                {
                    e.Cancel = true;
                }
            }
            catch (Exception ex)
            {
                this.DisplayError(this.view, ex);
            }
        }

        /// <summary>
        /// Handles the <see cref="ITeamProjectDocumentManager.DocumentBeforeClose"/> event.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The information about the selected layout.</param>
        private void HandleDocumentBeforeClose(object sender, CancelEventArgs e)
        {
            if (this.manager.ActiveDocument == this.designerDocument)
            {
                this.CheckForChangesAndConfirmDisposition(e);
            }
        }

        /// <summary>
        /// Changes the layout that is displayed and makes it the current one.
        /// </summary>
        /// <param name="layout">The layout to change to.</param>
        private void ChangeDisplayedLayout(LayoutInformation layout)
        {
            this.DisplayLayoutDefinition(layout);
            this.SetCurrentLayout(layout.Name);
        }

        /// <summary>
        /// Displays the given layout.
        /// </summary>
        /// <param name="li">The layout to display.</param>
        private void DisplayLayoutDefinition(LayoutInformation li)
        {
            this.designerDocument.DisplayLayoutDefinition(li.Name);
            this.SetDocumentClean();
        }

        /// <summary>
        /// Checks if the designer document has changed and asks the user to confirm the change. Will save current layout if requested. Document is marked clean if the change is not cancelled.
        /// </summary>
        /// <param name="e">The event args that allow for cancellation.</param>
        /// <returns>Value indicating whether the change is confirmed or not.</returns>
        private bool CheckForChangesAndConfirmDisposition(CancelEventArgs e)
        {
            bool confirmed = true;

            if (this.currentLayout != null && this.IsDocumentDirty())
            {
                IMessageBoxView messageBox = this.container.Resolve<IMessageBoxView>();
                MessageBoxResult result = messageBox.Show(string.Format(CultureInfo.InvariantCulture, PresenterResources.SaveLayoutChanges, this.SaveLayoutName()), PresenterResources.MicrosoftWord, MessageBoxViewButtons.YesNoCancel, MessageBoxViewIcon.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    this.SaveChanges();
                }
                else if (result == MessageBoxResult.No)
                {
                    if (this.RenameIsPending())
                    {
                        this.SetViewLayoutList();
                    }

                    this.SetDocumentClean();
                }

                confirmed = result == MessageBoxResult.Yes || result == MessageBoxResult.No;

                e.Cancel = !confirmed;
            }

            return confirmed;
        }

        /// <summary>
        /// Returns a value indicating whether there is a pending rename.
        /// </summary>
        /// <returns>A value indicating whether there is a pending rename.</returns>
        private bool RenameIsPending()
        {
            return !string.IsNullOrEmpty(this.pendingRenameTo);
        }

        /// <summary>
        /// Saves the current layout.
        /// </summary>
        private void SaveChanges()
        {
            this.designerDocument.SaveLayoutDefinition(this.currentLayout.Name);
            if (this.RenameIsPending())
            {
                this.designerDocument.RenameLayoutDefinition(this.currentLayout.Name, this.pendingRenameTo);
                if (this.CurrentLayoutIsTemporary())
                {
                    this.RemoveTemporaryLayout();
                }

                this.SetViewLayoutList();
                this.SetCurrentLayout(this.pendingRenameTo);
            }

            this.manager.SystemTemplate.Save();
            this.SetDocumentClean();
        }

        /// <summary>
        /// Returns the name the layout will have when it is saved.
        /// </summary>
        /// <returns>The name the layout will have when it is saved.</returns>
        private string SaveLayoutName()
        {
            string ans = this.currentLayout.Name;
            if (this.RenameIsPending())
            {
                ans = this.pendingRenameTo;
            }

            return ans;
        }

        /// <summary>
        /// Sets the state to indicate that the document is clean.
        /// </summary>
        private void SetDocumentClean()
        {
            this.designerDocument.MarkDocumentClean();
            this.pendingRenameTo = string.Empty;
        }

        /// <summary>
        /// Returns a value indicating whether the document is dirty.
        /// </summary>
        /// <returns>A value indicating whether the document is dirty.</returns>
        private bool IsDocumentDirty()
        {
            return this.designerDocument.HasChanged || this.RenameIsPending();
        }

        /// <summary>
        /// Sets the list of layouts on the view.
        /// </summary>
        private void SetViewLayoutList()
        {
            this.view.SetLayoutList(this.GetViewOrderedLayoutList());
        }

        /// <summary>
        /// Gets the list of layouts in the order in which they are to be listed in the view. Includes the temporary layout, if any.
        /// </summary>
        /// <returns>List of layouts in the order in which they are to appear on the view.</returns>
        private IOrderedEnumerable<LayoutInformation> GetViewOrderedLayoutList()
        {
            IEnumerable<LayoutInformation> layouts = this.GetLayoutList();

            return layouts.OrderBy(li => li.Name);
        }

        /// <summary>
        /// Gets the list of layouts. Includes the temporary layout, if any.
        /// </summary>
        /// <returns>List of layouts.</returns>
        private IEnumerable<LayoutInformation> GetLayoutList()
        {
            IEnumerable<LayoutInformation> layouts = this.manager.SystemTemplate.Layouts;
            if (this.temporaryLayout != null)
            {
                layouts = layouts.Concat(new LayoutInformation[] { this.temporaryLayout });
            }

            return layouts;
        }

        /// <summary>
        /// Sets the editor controls according to the state of the designer document.
        /// </summary>
        private void SetEditorControls()
        {
            this.view.SetButtonState(LayoutDesignerControl.Connect, !this.designerDocument.IsConnected);
            this.view.SetButtonState(LayoutDesignerControl.AddField, this.designerDocument.IsConnected);
            this.view.SetButtonState(LayoutDesignerControl.AddNew, this.temporaryLayout == null);
            this.view.SetButtonState(LayoutDesignerControl.Save, this.GetLayoutList().FirstOrDefault() != null);
        }
    }
}
