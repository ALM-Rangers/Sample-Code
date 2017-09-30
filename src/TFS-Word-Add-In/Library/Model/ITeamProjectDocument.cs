//---------------------------------------------------------------------
// <copyright file="ITeamProjectDocument.cs" company="Microsoft">
//    Copyright © ALM | DevOps Ranger Contributors. All rights reserved.
//    This code is licensed under the MIT License.
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF 
//    ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
//    TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
//    PARTICULAR PURPOSE AND NONINFRINGEMENT.
// </copyright>
// <summary>The ITeamProjectDocument type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.Word4Tfs.Library.Model
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using Microsoft.Word4Tfs.Library.Data;
    using Microsoft.Word4Tfs.Library.Model.TeamFoundation;
    using Microsoft.Word4Tfs.Library.Model.Word;

    /// <summary>
    /// The operations on the model that represents the team project and the work items that the document is working with.
    /// </summary>
    public interface ITeamProjectDocument : IDisposable
    {
        /// <summary>
        /// Raised when the document is closed.
        /// </summary>
        event EventHandler Close;

        /// <summary>
        /// Raised when the document becomes connected.
        /// </summary>
        event EventHandler Connected;

        /// <summary>
        /// Gets a value indicating whether the document is connected to a Team Project.
        /// </summary>
        bool IsConnected { get; }

        /// <summary>
        /// Gets a value indicating whether the document is insertable at the current location.
        /// </summary>
        bool IsInsertable { get; }

        /// <summary>
        /// Gets a value indicating whether the document is refreshable.
        /// </summary>
        bool IsRefreshable { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the document is temporary.
        /// </summary>
        /// <remarks>
        /// A temporary document will never be saved. It is used when hosting the layout designer. Setting this to true disables import and refresh.
        /// </remarks>
        bool IsTemporary { get; set; }

        /// <summary>
        /// Gets a value indicating whether the document has changed since <see cref="MarkDocumentClean"/> was called.
        /// </summary>
        bool HasChanged { get; }

        /// <summary>
        /// Gets or sets the team project.
        /// </summary>
        ITeamProject TeamProject { get; set; }

        /// <summary>
        /// Gets or sets the Word document that the Team Project Document is using.
        /// </summary>
        IWordDocument WordDocument { get; set; }

        /// <summary>
        /// Gets the current list of queries and layouts after modification to account for all the other queries in the document.
        /// </summary>
        IEnumerable<QueryAndLayoutInformation> QueriesAndLayouts { get; }

        /// <summary>
        /// Gets the work items associated with each saved set of work items.
        /// </summary>
        IEnumerable<QueryWorkItems> QueryWorkItems { get; }

        /// <summary>
        /// Loads the current state from the document.
        /// </summary>
        /// <param name="rebindCallback">The callback to be called if the document must be rebound because the collection cannot be found or the id does not match. The callback must return null to cancel the rebind.</param>
        /// <returns>List of load warnings, empty if there were no warnings. The document is still considered loaded if there are warnings.</returns>
        IEnumerable<string> Load(Func<Uri> rebindCallback);

        /// <summary>
        /// Saves the team project in the document.
        /// </summary>
        void SaveTeamProject();

        /// <summary>
        /// Adds the <paramref name="queryAndLayout"/> to the collection of queries and layouts in the document, but does not save the new query and layout.
        /// </summary>
        /// <param name="queryAndLayout">The query and layout to be added.</param>
        /// <returns>The query and layout with the query modified to include all the fields from all the layouts and exclude any fields not defined in the team project.</returns>
        QueryAndLayoutInformation AddQueryAndLayout(QueryAndLayoutInformation queryAndLayout);

        /// <summary>
        /// Saves the queries and layout names previously added to the document by <see cref="AddQueryAndLayout"/>.
        /// </summary>
        void SaveQueriesAndLayouts();

        /// <summary>
        /// Saves a tree of work items in the document, merging them with any existing work items.
        /// </summary>
        /// <param name="workItems">The work items to be saved.</param>
        /// <param name="fields">The fields in the work item to be saved.</param>
        /// <param name="cancellationToken">Used to cancel the save.</param>
        void SaveWorkItems(WorkItemTree workItems, string[] fields, CancellationToken cancellationToken);

        /// <summary>
        /// Maps the saved work items into the document using the given layout.
        /// </summary>
        /// <param name="layout">The layout to be used to map the saved work items.</param>
        /// <param name="index">The index in the query and layout manager of the query that id being processed.</param>
        /// <param name="cancellationToken">Used to cancel the save.</param>
        /// <exception cref="InvalidOperationException">Thrown if <see cref="SaveWorkItems"/> has not been called to save the work items first.</exception>
        void MapWorkItemsIntoDocument(LayoutInformation layout, int index, CancellationToken cancellationToken);

        /// <summary>
        /// Refreshes the work items in the document.
        /// </summary>
        /// <remarks>
        /// This call will update the rich text content controls which are not bound to the Custom XML Parts.
        /// </remarks>
        /// <param name="cancellationToken">Used to cancel the save.</param>
        /// <returns>List of verification errors, empty if there were no errors.</returns>
        IEnumerable<string> RefreshWorkItems(CancellationToken cancellationToken);

        /// <summary>
        /// Inserts the named layout definition into the document so that the layout can be edited and then updated.
        /// </summary>
        /// <param name="layoutName">The name of the layout to be displayed.</param>
        void DisplayLayoutDefinition(string layoutName);

        /// <summary>
        /// Saves a layout defined in the document into the template.
        /// </summary>
        /// <param name="layoutName">The name of the layout to be saved.</param>
        void SaveLayoutDefinition(string layoutName);

        /// <summary>
        /// Renames a layout.
        /// </summary>
        /// <param name="oldLayoutName">The name of the layout to be renamed.</param>
        /// <param name="newLayoutName">The new name for the layout.</param>
        void RenameLayoutDefinition(string oldLayoutName, string newLayoutName);

        /// <summary>
        /// Adds a prototype layout definition to the document, used when creating a new layout.
        /// </summary>
        void AddPrototypeLayoutDefinition();

        /// <summary>
        /// Marks the document as clean.
        /// </summary>
        void MarkDocumentClean();

        /// <summary>
        /// Adds a field to the document.
        /// </summary>
        /// <param name="field">The field to be added.</param>
        void AddField(ITfsFieldDefinition field);
    }
}
