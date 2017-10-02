//---------------------------------------------------------------------
// <copyright file="IWaitNotifier.cs" company="Microsoft">
//    Copyright © ALM | DevOps Ranger Contributors. All rights reserved.
//    This code is licensed under the MIT License.
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF 
//    ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
//    TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
//    PARTICULAR PURPOSE AND NONINFRINGEMENT.
// </copyright>
// <summary>The IWaitNotifier type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.Word4Tfs.Library.View
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Represents an object used to notify the user that a long running operation is taking place.
    /// </summary>
    public interface IWaitNotifier
    {
        /// <summary>
        /// Notifies the user that a long-running operation has started.
        /// </summary>
        void StartWait();

        /// <summary>
        /// Notifies the user that a long-running operation has ended.
        /// </summary>
        void EndWait();
    }
}
