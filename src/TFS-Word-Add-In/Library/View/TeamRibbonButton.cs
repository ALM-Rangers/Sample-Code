//---------------------------------------------------------------------
// <copyright file="TeamRibbonButton.cs" company="Microsoft">
//    Copyright (c) Microsoft. All rights reserved.
//    This code is licensed under the MIT License.
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF 
//    ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
//    TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
//    PARTICULAR PURPOSE AND NONINFRINGEMENT.
// </copyright>
// <summary>The TeamRibbonButton type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.Word4Tfs.Library.View
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// The buttons on the team ribbon.
    /// </summary>
    public enum TeamRibbonButton
    {
        /// <summary>
        /// The Import button to request that the document be connected to a Team Project and work items imported.
        /// </summary>
        Import,

        /// <summary>
        /// The Refresh button to request that the document, which is already connected to a Team Project, to refresh the work items already in the document.
        /// </summary>
        Refresh,

        /// <summary>
        /// The ShowDesigner button to request that a layout designer document be shown.
        /// </summary>
        ShowDesigner,

        /// <summary>
        /// The check button used to indicate if bookmarks are to be shown.
        /// </summary>
        ShowBookmarks
    }
}
