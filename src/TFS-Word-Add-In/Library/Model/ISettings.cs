//---------------------------------------------------------------------
// <copyright file="ISettings.cs" company="Microsoft">
//    Copyright (c) Microsoft. All rights reserved.
//    This code is licensed under the MIT License.
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF 
//    ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
//    TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
//    PARTICULAR PURPOSE AND NONINFRINGEMENT.
// </copyright>
// <summary>The ISettings type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.Word4Tfs.Library.Model
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Defines the operations on persistent settings.
    /// </summary>
    public interface ISettings
    {
        /// <summary>
        /// Gets or sets a value indicating whether bookmarks should be shown or not.
        /// </summary>
        /// <remarks>A <c>null</c> value indicates the setting is not present.</remarks>
        Nullable<bool> ShowBookmarks { get; set; }

        /// <summary>
        /// Gets or sets the timestamp of system template for which we are not to prompt for an upgrade.
        /// </summary>
        /// <remarks>A <c>null</c> value indicates the setting is not present.</remarks>
        Nullable<DateTime> IgnoreSystemTemplateUpgradeFor { get; set; }
    }
}
