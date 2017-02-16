//---------------------------------------------------------------------
// <copyright file="ITeamProjectDocumentManager.cs" company="Microsoft">
//    Copyright (c) Microsoft. All rights reserved.
//    This code is licensed under the MIT License.
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF 
//    ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
//    TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
//    PARTICULAR PURPOSE AND NONINFRINGEMENT.
// </copyright>
// <summary>The ITeamProjectDocumentManager type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.Word4Tfs.Library.Model
{
    using System;
    using System.ComponentModel;
    using Microsoft.Practices.Unity;

    /// <summary>
    /// Defines the operations used to manage the <see cref="ITeamProjectDocument"/> objects for the currently open documents.
    /// </summary>
    public interface ITeamProjectDocumentManager
    {
        /// <summary>
        /// Raised when the underlying document is changed.
        /// </summary>
        event EventHandler DocumentChanged;

        /// <summary>
        /// Raised when the active document is being closed, close can be cancelled.
        /// </summary>
        event CancelEventHandler DocumentBeforeClose;

        /// <summary>
        /// Raised when a message needs to be displayed to the user.
        /// </summary>
        event EventHandler<UserMessageEventArgs> UserMessage;

        /// <summary>
        /// Gets the system template.
        /// </summary>
        /// <value>The system template, <c>null</c> if there is no system template.</value>
        ITeamProjectTemplate SystemTemplate { get; }

        /// <summary>
        /// Gets the active document.
        /// </summary>
        /// <value>The active document, <c>null</c> if there is no active document.</value>
        ITeamProjectDocument ActiveDocument { get; }

        /// <summary>
        /// Gets the active Unity container.
        /// </summary>
        /// <value>The active container, <c>null</c> if there is no active document.</value>
        IUnityContainer ActiveContainer { get; }

        /// <summary>
        /// Initialises the manager.
        /// </summary>
        void Initialise();

        /// <summary>
        /// Adds a new team project document and makes it the current document.
        /// </summary>
        /// <param name="temporary">Indicates that the document is temporary, the document will never be saved and should not be importable.</param>
        /// <returns>The new team project document.</returns>
        ITeamProjectDocument Add(bool temporary);

        /// <summary>
        /// Sets the option to show or hide bookmarks.
        /// </summary>
        /// <param name="showBookmarks">A value indicating whether bookmarks should be shown or not.</param>
        void SetShowBookmarkOption(bool showBookmarks);
    }
}
