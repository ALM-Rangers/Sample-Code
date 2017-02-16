//---------------------------------------------------------------------
// <copyright file="ProxyAssemblyEventArgs.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
//     THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
//     OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
//     LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR
//     FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
// <summary>The ProxyAssemblyEventArgs type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.WcfUnit.Library
{
    using System;
    using System.Reflection;

    /// <summary>
    /// Event data relating to a proxy assembly.
    /// </summary>
    public class ProxyAssemblyEventArgs : EventArgs
    {
        /// <summary>
        /// The assembly which is the subject of the event.
        /// </summary>
        private Assembly assembly;

        /// <summary>
        /// Gets or sets the assembly the event relates to.
        /// </summary>
        /// <value>The assembly the event relates to.</value>
        public Assembly Assembly
        {
            get { return this.assembly; }
            set { this.assembly = value; }
        }
    }
}
