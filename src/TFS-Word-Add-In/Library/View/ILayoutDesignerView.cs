//---------------------------------------------------------------------
// <copyright file="ILayoutDesignerView.cs" company="Microsoft">
//    Copyright (c) Microsoft. All rights reserved.
//    This code is licensed under the MIT License.
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF 
//    ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
//    TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
//    PARTICULAR PURPOSE AND NONINFRINGEMENT.
// </copyright>
// <summary>The ILayoutDesignerView type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.Word4Tfs.Library.View
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Microsoft.Word4Tfs.Library.Data;
    using Microsoft.Word4Tfs.Library.Model.TeamFoundation;

    /// <summary>
    /// Defines the operations on the layout designer view.
    /// </summary>
    public interface ILayoutDesignerView : IViewBase
    {
        /// <summary>
        /// Raised when the user wants the layouts to be saved.
        /// </summary>
        event EventHandler Save;

        /// <summary>
        /// Raised when the user wants the designer to be connected to TFS.
        /// </summary>
        event EventHandler Connect;

        /// <summary>
        /// Raised when a field is to be added to the designer document.
        /// </summary>
        event EventHandler<FieldDefinitionEventArgs> AddField;

        /// <summary>
        /// Raised when a new layout is to be added to the designer document.
        /// </summary>
        event EventHandler AddNewLayout;

        /// <summary>
        /// Raised when the view is hidden or closed.
        /// </summary>
        event EventHandler Hidden;

        /// <summary>
        /// Raised when the user selects a layout. Cancellable so that the layout selection is not changed.
        /// </summary>
        event EventHandler<LayoutItemEventArgs> LayoutSelected;

        /// <summary>
        /// Raised when the user deletes a layout. Cancellable so that the layout deletion can be stopped.
        /// </summary>
        event EventHandler<LayoutItemEventArgs> LayoutDelete;

        /// <summary>
        /// Raised when a layout is to be renamed. Rename can be cancelled.
        /// </summary>
        event EventHandler<RenameEventArgs> LayoutRename;
        
        /// <summary>
        /// Shows the layout designer.
        /// </summary>
        void ShowLayoutDesigner();

        /// <summary>
        /// Hides the layout designer.
        /// </summary>
        void HideLayoutDesigner();

        /// <summary>
        /// Gives the view the list of layouts to display.
        /// </summary>
        /// <param name="layouts">The list of layouts to display.</param>
        void SetLayoutList(IEnumerable<LayoutInformation> layouts);

        /// <summary>
        /// Causes the given layout to be selected.
        /// </summary>
        /// <remarks>This will <u>not</u> raise the <see cref="LayoutSelected"/> event.</remarks>
        /// <param name="layoutName">The name of the layout to be selected</param>
        void SelectLayout(string layoutName);

        /// <summary>
        /// Causes the given layout name to be set into edit mode.
        /// </summary>
        /// <param name="layoutName">The name of the layout to be edited</param>
        void StartLayoutNameEdit(string layoutName);

        /// <summary>
        /// Gives the view the list of fields in the team project that are available for selection.
        /// </summary>
        /// <param name="fields">The fields.</param>
        void SetFieldList(IEnumerable<ITfsFieldDefinition> fields);

        /// <summary>
        /// Closes the view.
        /// </summary>
        void Close();

        /// <summary>
        /// Sets the string in the task pane status bar.
        /// </summary>
        /// <param name="status">The status message to be displayed.</param>
        /// <param name="timeout">The number of seconds to display the status message, less than 1 means infinite.</param>
        void SetStatus(string status, int timeout);
    }
}
