//---------------------------------------------------------------------
// <copyright file="IWordApplication.cs" company="Microsoft">
//    Copyright © ALM | DevOps Ranger Contributors. All rights reserved.
//    This code is licensed under the MIT License.
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF 
//    ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
//    TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
//    PARTICULAR PURPOSE AND NONINFRINGEMENT.
// </copyright>
// <summary>The IWordApplication type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.Word4Tfs.Library.Model.Word
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Defines the operations on the underlying Word application.
    /// </summary>
    public interface IWordApplication
    {
        /// <summary>
        /// Raised when the active document has been changed.
        /// </summary>
        event EventHandler DocumentChange;

        /// <summary>
        /// Raised before a document is closed.
        /// </summary>
        event EventHandler<CancellableDocumentOperationEventArgs> DocumentBeforeClose;

        /// <summary>
        /// Gets the number of open documents.
        /// </summary>
        int OpenDocumentCount { get; }

        /// <summary>
        /// Gets or sets a value indicating whether bookmarks should be shown or not.
        /// </summary>
        bool ShowBookmarks { get; set; }

        /// <summary>
        /// Gets the system template.
        /// </summary>
        /// <value>The <see cref="IWordTemplate"/> object for the system template, or <c>null</c> if there is no system template.</value>
        IWordTemplate SystemTemplate { get; }

        /// <summary>
        /// Gets the active Word document.
        /// </summary>
        /// <value>The <see cref="IWordDocument"/> object for the active document, or <c>null</c> if there is no active document.</value>
        IWordDocument ActiveDocument { get; }

        /// <summary>
        /// Gets the path to the directory where user templates are stored.
        /// </summary>
        string UserTemplatesPath { get; }

        /// <summary>
        /// Performs initial processing.
        /// </summary>
        void Initialise();

        /// <summary>
        /// Creates a new document and makes it the active document.
        /// </summary>
        /// <remarks>
        /// Causes the <see cref="DocumentChanged"/> event to be raised.
        /// </remarks>
        /// <returns>The new document.</returns>
        IWordDocument CreateNewDocument();
    }

    /// <summary>
    /// Event arguments for a cancellable operation on a document.
    /// </summary>
    public class CancellableDocumentOperationEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CancellableDocumentOperationEventArgs"/> class.
        /// </summary>
        /// <param name="document">The document to which the operation is being applied.</param>
        public CancellableDocumentOperationEventArgs(IWordDocument document)
        {
            this.Document = document;
            this.Cancel = false;
        }

        /// <summary>
        /// Gets the document to which the operation is being applied.
        /// </summary>
        public IWordDocument Document { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether the operation should be cancelled.
        /// </summary>
        public bool Cancel { get; set; }
    }
}
