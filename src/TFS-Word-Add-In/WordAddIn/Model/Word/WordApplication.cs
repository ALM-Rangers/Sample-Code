//---------------------------------------------------------------------
// <copyright file="WordApplication.cs" company="Microsoft">
//    Copyright (c) Microsoft. All rights reserved.
//    This code is licensed under the MIT License.
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF 
//    ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
//    TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
//    PARTICULAR PURPOSE AND NONINFRINGEMENT.
// </copyright>
// <summary>The WordApplication type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.Word4Tfs.Library.Model.Word
{
    using global::System;
    using global::System.Diagnostics;
    using global::System.IO;
    using global::System.Linq;
    using global::System.Reflection;
    using Microsoft.Office.Interop.Word;

    /// <summary>
    /// The concrete Word application class that represents the whole Word instance.
    /// </summary>
    /// <remarks>
    /// Because we need to use Embed Interop Types to allow the add-in to work on Word 2007 this class must be in the same assembly that configures Unity, otherwise the interop types
    /// are not seen as equivalent and type resolution fails on the interop types.
    /// </remarks>
    public class WordApplication : IWordApplication
    {
        /// <summary>
        /// The object to use for logging.
        /// </summary>
        private ILogger logger;

        /// <summary>
        /// The underlying Word application.
        /// </summary>
        private Application application;

        /// <summary>
        /// The path to the system template.
        /// </summary>
        private string systemTemplatePath;

        /// <summary>
        /// Initializes a new instance of the <see cref="WordApplication"/> class.
        /// </summary>
        /// <param name="application">The underlying Word application.</param>
        /// <param name="logger">The object to be used for logging.</param>
        public WordApplication(Application application, ILogger logger)
        {
            if (application == null)
            {
                throw new ArgumentNullException("application");
            }

            this.application = application;
            this.logger = logger;
            if (this.application.Documents.Count > 0 && this.application.ActiveDocument != null)
            {
                this.ActiveDocument = this.CreateWordDocument(this.application.ActiveDocument);
            }

            ////this.application.DocumentOpen += new ApplicationEvents4_DocumentOpenEventHandler(HandleDocumentOpen);
            this.application.DocumentChange += new ApplicationEvents4_DocumentChangeEventHandler(this.HandleDocumentChange);
            this.application.DocumentBeforeClose += new ApplicationEvents4_DocumentBeforeCloseEventHandler(this.HandleDocumentBeforeClose);
        }
        
        /// <summary>
        /// Raised when the active document has been changed.
        /// </summary>
        public event EventHandler DocumentChange;

        /// <summary>
        /// Raised before a document is closed.
        /// </summary>
        public event EventHandler<CancellableDocumentOperationEventArgs> DocumentBeforeClose;

        /// <summary>
        /// Gets the number of open documents.
        /// </summary>
        public int OpenDocumentCount
        {
            get
            {
                return this.application.Documents.Count;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether bookmarks should be shown or not.
        /// </summary>
        public bool ShowBookmarks
        {
            get
            {
                return this.application.ActiveWindow.View.ShowBookmarks;
            }

            set
            {
                this.application.ActiveWindow.View.ShowBookmarks = value;
            }
        }
        
        /// <summary>
        /// Gets the system template.
        /// </summary>
        /// <value>The <see cref="IWordTemplate"/> object for the system template, or <c>null</c> if there is no system template.</value>
        public IWordTemplate SystemTemplate { get; private set; }

        /// <summary>
        /// Gets the active Word document.
        /// </summary>
        /// <value>The <see cref="IWordDocument"/> object for the active document, or <c>null</c> if there is no active document.</value>
        public IWordDocument ActiveDocument { get; private set; }

        /// <summary>
        /// Gets the path to the directory where user templates are stored.
        /// </summary>
        public string UserTemplatesPath
        {
            get
            {
                return this.application.Options.DefaultFilePath[WdDefaultFilePath.wdUserTemplatesPath];
            }
        }

        /// <summary>
        /// Performs initial processing.
        /// </summary>
        public void Initialise()
        {
            this.SetupSystemTemplate();
        }

        /// <summary>
        /// Creates a new document and makes it the active document.
        /// </summary>
        /// <remarks>
        /// Causes the <see cref="DocumentChanged"/> event to be raised.
        /// </remarks>
        /// <returns>The new document.</returns>
        public IWordDocument CreateNewDocument()
        {
            this.application.Documents.Add(this.systemTemplatePath, false, WdNewDocumentType.wdNewBlankDocument, true);
            ////this.application.Documents.Open(this.systemTemplatePath, false, true, false, Type.Missing, Type.Missing, false, Type.Missing, Type.Missing, Type.Missing, Type.Missing, true, false, Type.Missing, false, Type.Missing);
            return this.ActiveDocument; // event handling sets ActiveDocument to the new document.
        }

        /// <summary>
        /// Sets the document according to the active document.
        /// </summary>
        private void SetDocument()
        {
            if (this.ActiveDocument != null)
            {
                this.ActiveDocument.Dispose();
            }

            if (this.application.Documents.Count > 0)
            {
                this.ActiveDocument = this.CreateWordDocument(this.application.ActiveDocument);
                this.logger.Log(TraceEventType.Information, "Word document changed to: {0}", this.ActiveDocument.Name);
            }
            else
            {
                this.ActiveDocument = null;
                this.logger.Log(TraceEventType.Information, "Word document changed to: <none>");
            }
        }

        /// <summary>
        /// Sets up the system template document.
        /// </summary>
        private void SetupSystemTemplate()
        {
            this.systemTemplatePath = Path.Combine(this.UserTemplatesPath, Constants.SystemTemplateName);

            if (!string.IsNullOrEmpty(this.systemTemplatePath) && File.Exists(this.systemTemplatePath))
            {
                this.application.AddIns.Add(this.systemTemplatePath);
                foreach (Template t in this.application.Templates)
                {
                    if (t.Name == Constants.SystemTemplateName)
                    {
                        this.SystemTemplate = new WordTemplate(this.logger, t);
                        break;
                    }
                }
            }
        }

        /////// <summary>
        /////// Handles the <see cref="Application.DocumentOpen"/> event.
        /////// </summary>
        /////// <param name="Doc">The document being opened.</param>
        ////void HandleDocumentOpen(Document Doc)
        ////{
        ////    this.logger.Log(TraceEventType.Verbose, "Application document open event for {0}", Doc.Name);
        ////    this.SetDocument();
        ////    if (this.DocumentChange != null)
        ////    {
        ////        this.DocumentChange(this, EventArgs.Empty);
        ////    }
        ////}

        /// <summary>
        /// Handles the <see cref="Application.DocumentChange"/> event.
        /// </summary>
        private void HandleDocumentChange()
        {
            this.logger.Log(TraceEventType.Verbose, "Application document change event.");
            this.SetDocument();
            if (this.DocumentChange != null)
            {
                this.DocumentChange(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Handles the <see cref="Application.DocumentBeforeClose"/> event.
        /// </summary>
        /// <param name="doc">The document that is being closed.</param>
        /// <param name="cancel">The cancel flag that is to be set to cancel the operation.</param>
        private void HandleDocumentBeforeClose(Document doc, ref bool cancel)
        {
            this.logger.Log(TraceEventType.Information, "Word document closing: {0}", doc.Name);
            if (this.DocumentBeforeClose != null)
            {
                using (IWordDocument wordDoc = this.CreateWordDocument(doc))
                {
                    CancellableDocumentOperationEventArgs e = new CancellableDocumentOperationEventArgs(wordDoc);
                    this.DocumentBeforeClose(this, e);
                    cancel = e.Cancel;
                }
            }
        }

        /// <summary>
        /// Creates a <see cref="WordDocument"/>.
        /// </summary>
        /// <param name="doc">The word document that is behind the <see cref="WordDocument"/>.</param>
        /// <returns>The <see cref="WordDocument"/>.</returns>
        private IWordDocument CreateWordDocument(Document doc)
        {
            IWordDocument ans = new WordDocument(doc, this.logger);
            return ans;
        }
    }
}
