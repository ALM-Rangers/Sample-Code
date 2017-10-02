//---------------------------------------------------------------------
// <copyright file="ILowLevelSettings.cs" company="Microsoft">
//    Copyright © ALM | DevOps Ranger Contributors. All rights reserved.
//    This code is licensed under the MIT License.
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF 
//    ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
//    TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
//    PARTICULAR PURPOSE AND NONINFRINGEMENT.
// </copyright>
// <summary>The ILowLevelSettings type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.Word4Tfs.Library.Model
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Provides raw low level access to the settings.
    /// </summary>
    public interface ILowLevelSettings
    {
        /// <summary>
        /// Gets or sets a named setting.
        /// </summary>
        /// <param name="name">The name of the setting.</param>
        /// <returns>The value of the setting. <c>Null</c> if the setting does not exist.</returns>
        object this[string name] { get; set; }
    }
}
