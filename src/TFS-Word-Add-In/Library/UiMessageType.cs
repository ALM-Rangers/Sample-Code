//---------------------------------------------------------------------
// <copyright file="UiMessageType.cs" company="Microsoft">
//    Copyright © ALM | DevOps Ranger Contributors. All rights reserved.
//    This code is licensed under the MIT License.
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF 
//    ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
//    TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
//    PARTICULAR PURPOSE AND NONINFRINGEMENT.
// </copyright>
// <summary>The UiMessageType type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.Word4Tfs.Library
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Classification of messages to display on the user interface.
    /// </summary>
    public enum UIMessageType
    {
        /// <summary>
        /// A warning message.
        /// </summary>
        Warning,

        /// <summary>
        /// An error message.
        /// </summary>
        Error
    }
}
