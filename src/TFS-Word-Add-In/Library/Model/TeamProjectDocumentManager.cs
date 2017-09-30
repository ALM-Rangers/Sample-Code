//---------------------------------------------------------------------
// <copyright file="TeamProjectDocumentManager.cs" company="Microsoft">
//    Copyright © ALM | DevOps Ranger Contributors. All rights reserved.
//    This code is licensed under the MIT License.
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF 
//    ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
//    TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
//    PARTICULAR PURPOSE AND NONINFRINGEMENT.
// </copyright>
// <summary>The TeamProjectDocumentManager type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.Word4Tfs.Library.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Linq;
    using Microsoft.Practices.Unity;
    using Microsoft.Word4Tfs.Library.Data;
    using Microsoft.Word4Tfs.Library.Model.Word;

    /// <summary>
    /// Manages the <see cref="ITeamProjectDocument"/> objects for the currently open documents.
    /// </summary>
    /// <remarks>
    /// The <see cref="ITeamProjectDocument"/> objects are only created the first time the document is opened (or if it is closed and reopened) and then cached.
    /// This is because loading a document can be expensive and slow.
    /// </remarks>
    public class TeamProjectDocumentManager : ITeamProjectDocumentManager
    {
        /// <summary>
        /// The container used to create objects.
        /// </summary>
        private IUnityContainer container;

        /// <summary>
        /// The underlying Word application.
        /// </summary>
        private IWordApplication application;

        /// <summary>
        /// The settings to be used.
        /// </summary>
        private ISettings settings;

        /// <summary>
        /// The object to use for logging.
        /// </summary>
        private ILogger logger;

        /// <summary>
        /// Stores the document contexts for the open documents, accessed by <see cref="DocumentHandle"/>.
        /// </summary>
        private Dictionary<DocumentHandle, DocumentContext> openDocuments = new Dictionary<DocumentHandle, DocumentContext>();

        /// <summary>
        /// Initializes a new instance of the <see cref="TeamProjectDocumentManager"/> class.
        /// </summary>
        /// <param name="container">The Unity container to be used to create <see cref="ITeamProjectDocument"/> objects.</param>
        /// <param name="application">The underlying Word application.</param>
        /// <param name="settings">The settings to be used.</param>
        /// <param name="logger">The object to be used for logging.</param>
        public TeamProjectDocumentManager(IUnityContainer container, IWordApplication application, ISettings settings, ILogger logger)
        {
            if (container == null)
            {
                throw new ArgumentNullException("container");
            }

            if (application == null)
            {
                throw new ArgumentNullException("application");
            }

            if (settings == null)
            {
                throw new ArgumentNullException("settings");
            }

            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }

            this.container = container;
            this.application = application;
            this.settings = settings;
            this.logger = logger;
        }

        /// <summary>
        /// Raised when the underlying document is changed.
        /// </summary>
        public event EventHandler DocumentChanged;

        /// <summary>
        /// Raised when the active document is being closed, close can be cancelled.
        /// </summary>
        public event CancelEventHandler DocumentBeforeClose;

        /// <summary>
        /// Raised when a message needs to be displayed to the user.
        /// </summary>
        public event EventHandler<UserMessageEventArgs> UserMessage;

        /// <summary>
        /// Gets the system template.
        /// </summary>
        /// <value>The system template, <c>null</c> if there is no system template.</value>
        public ITeamProjectTemplate SystemTemplate { get; private set; }

        /// <summary>
        /// Gets the active document.
        /// </summary>
        /// <value>The active document, <c>null</c> if there is no active document.</value>
        public ITeamProjectDocument ActiveDocument { get; private set; }

        /// <summary>
        /// Gets the active Unity container.
        /// </summary>
        /// <value>The active container, <c>null</c> if there is no active document.</value>
        public IUnityContainer ActiveContainer { get; private set; }

        /// <summary>
        /// Initialises the manager.
        /// </summary>
        public void Initialise()
        {
            this.application.DocumentChange += new EventHandler(this.HandleDocumentChange);
            this.application.DocumentBeforeClose += new EventHandler<CancellableDocumentOperationEventArgs>(this.HandleDocumentBeforeClose);

            Nullable<bool> showBookmarks = this.settings.ShowBookmarks;
            if (!showBookmarks.HasValue)
            {
                this.SetShowBookmarkOption(true);
            }

            this.SetSystemTemplate(); // Template needed to instantiate document, so do this first.
            this.SetDocument();
        }

        /// <summary>
        /// Adds a new team project document and makes it the current document.
        /// </summary>
        /// <param name="temporary">Indicates that the document is temporary, the document will never be saved and should not be importable.</param>
        /// <returns>The new team project document.</returns>
        public ITeamProjectDocument Add(bool temporary)
        {
            this.application.CreateNewDocument(); // Will raise a DocumentChange event.
            this.ActiveDocument.IsTemporary = temporary;
            return this.ActiveDocument;
        }

        /// <summary>
        /// Sets the option to show or hide bookmarks.
        /// </summary>
        /// <param name="showBookmarks">A value indicating whether bookmarks should be shown or not.</param>
        public void SetShowBookmarkOption(bool showBookmarks)
        {
            this.application.ShowBookmarks = showBookmarks;
            this.settings.ShowBookmarks = showBookmarks;
        }

        /// <summary>
        /// Makes a handle for a document.
        /// </summary>
        /// <param name="document">The document to make the handle for.</param>
        /// <returns>The <see cref="DocumentHandle"/> for the <paramref name="document"/>.</returns>
        private DocumentHandle MakeDocumentHandle(IWordDocument document)
        {
            DocumentHandle handle = document.Handle.HasValue ? new DocumentHandle(document.Handle.Value) : new DocumentHandle(document.Name);
            this.logger.Log(TraceEventType.Verbose, "Handle for document {0} is {1}", document.Name, handle);
            return handle;
        }

        /// <summary>
        /// Handles the <see cref="Application.DocumentChange"/> event.
        /// </summary>
        /// <param name="sender">The event arguments.</param>
        /// <param name="e">The sender of the event.</param>
        private void HandleDocumentChange(object sender, EventArgs e)
        {
            this.SetDocument();
            if (this.DocumentChanged != null)
            {
                this.DocumentChanged(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Handles the <see cref="Application.DocumentBeforeClose"/> event.
        /// </summary>
        /// <param name="sender">The event arguments.</param>
        /// <param name="e">The sender of the event.</param>
        private void HandleDocumentBeforeClose(object sender, CancellableDocumentOperationEventArgs e)
        {
            this.logger.Log(TraceEventType.Information, "Closing: {0}", e.Document.Name);
            DocumentHandle handle = this.MakeDocumentHandle(e.Document);
            if (this.openDocuments.ContainsKey(handle))
            {
                IUnityContainer childContainer = this.openDocuments[handle].Container;
                ITeamProjectDocument closingDocument = childContainer.Resolve<ITeamProjectDocument>();

                CancelEventArgs cancelArgs = new CancelEventArgs();
                if (this.ActiveDocument == closingDocument && this.DocumentBeforeClose != null)
                {
                    this.DocumentBeforeClose(this, cancelArgs);
                }

                if (!cancelArgs.Cancel)
                {
                    this.openDocuments.Remove(handle);
                    childContainer.Dispose();
                }

                e.Cancel = cancelArgs.Cancel;
            }
        }

        /// <summary>
        /// Sets the document according to the active document.
        /// </summary>
        private void SetDocument()
        {
            if (this.application.OpenDocumentCount > 0)
            {
                DocumentContext context = null;
                bool newContext = false;
                DocumentHandle handle = this.MakeDocumentHandle(this.application.ActiveDocument);
                if (!this.openDocuments.TryGetValue(handle, out context))
                {
                    context = new DocumentContext(this.container.CreateChildContainer());
                    this.openDocuments.Add(handle, context);
                    newContext = true;
                }

                context.Container.RegisterInstance<IWordDocument>(this.application.ActiveDocument);
                this.ActiveDocument = context.Container.Resolve<ITeamProjectDocument>();
                if (newContext)
                {
                    this.ActiveDocument.Connected += new EventHandler(this.HandleDocumentConnection);
                }

                // Set the WordDocument property explicitly here becaused Unity does not set it again if the object already exists in the container.
                // Keeping WordDocument as a dependency property, even though it is redundant, purely for defensive coding reasons.
                this.ActiveDocument.WordDocument = this.application.ActiveDocument;
                this.ActiveContainer = context.Container;

                this.application.ShowBookmarks = this.settings.ShowBookmarks.Value;

                this.logger.Log(TraceEventType.Information, "Document changed to: {0}", this.application.ActiveDocument.Name);
            }
            else
            {
                this.ActiveDocument = null;
                this.ActiveContainer = null;
                this.logger.Log(TraceEventType.Information, "Document changed to: <none>");
            }
        }

        /// <summary>
        /// Handles when the team project document becomes connected.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        private void HandleDocumentConnection(object sender, EventArgs e)
        {
            // Replace handle for the context of the document.
            foreach (KeyValuePair<DocumentHandle, DocumentContext> entry in this.openDocuments)
            {
                if (entry.Value.Container == this.ActiveContainer)
                {
                    this.openDocuments.Remove(entry.Key);
                    this.openDocuments.Add(this.MakeDocumentHandle(this.ActiveDocument.WordDocument), entry.Value);
                    break;
                }
            }
        }

        /// <summary>
        /// Sets the system template.
        /// </summary>
        private void SetSystemTemplate()
        {
            if (this.application.SystemTemplate != null)
            {
                this.logger.Log(TraceEventType.Verbose, "Setting system template");
                this.container.RegisterInstance<IWordTemplate>(this.application.SystemTemplate);
                this.SystemTemplate = this.container.Resolve<ITeamProjectTemplate>();
                this.SystemTemplate.Load();
            }
            else
            {
                this.logger.Log(TraceEventType.Verbose, "Setting null system template");
                this.container.RegisterInstance<IWordTemplate>(new NullWordTemplate());
                this.SystemTemplate = null;
            }
        }
        
        /// <summary>
        /// Context for a particular document.
        /// </summary>
        private class DocumentContext
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="DocumentContext"/> class.
            /// </summary>
            /// <param name="container">The unity container that is part of the document context.</param>
            public DocumentContext(IUnityContainer container)
            {
                this.Container = container;
            }

            /// <summary>
            /// Gets the unity container that is part of the document context.
            /// </summary>
            public IUnityContainer Container { get; private set; }
        }
    }
}
