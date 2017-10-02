//---------------------------------------------------------------------
// <copyright file="ILayoutDesignerPresenter.cs" company="Microsoft">
//    Copyright © ALM | DevOps Ranger Contributors. All rights reserved.
//    This code is licensed under the MIT License.
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF 
//    ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
//    TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
//    PARTICULAR PURPOSE AND NONINFRINGEMENT.
// </copyright>
// <summary>The ILayoutDesignerPresenter type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.Word4Tfs.Library.Presenter
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Defines the operations on a layout designer presenter.
    /// </summary>
    public interface ILayoutDesignerPresenter
    {
        /// <summary>
        /// Raised when the presenter needs the controller to connect the document to TFS.
        /// </summary>
        event EventHandler Connect;

        /// <summary>
        /// Raised when the designer document is closed.
        /// </summary>
        event EventHandler Close;

        /// <summary>
        /// Shows the layout designer.
        /// </summary>
        void Show();

        /// <summary>
        /// Hides the layout designer.
        /// </summary>
        void Hide();
    }
}
