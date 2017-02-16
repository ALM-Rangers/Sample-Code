//---------------------------------------------------------------------
// <copyright file="CodeTypeReferenceRequestEventArgs.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
//     THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
//     OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
//     LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR
//     FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
// <summary>The CodeTypeReferenceRequestEventArgs type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.WcfUnit.Library
{
    using System;
    using System.CodeDom;

    /// <summary>
    /// Event arguments used to get a <see cref="CodeTypeReference"/> for a type.
    /// </summary>
    public class CodeTypeReferenceRequestEventArgs : EventArgs
    {
        /// <summary>
        /// The type to be created.
        /// </summary>
        private readonly Type requestedType;

        /// <summary>
        /// Any generic type parameters.
        /// </summary>
        private readonly CodeTypeReference[] genericParameters;

        /// <summary>
        /// The code type reference.
        /// </summary>
        private CodeTypeReference codeTypeReference;

        /// <summary>
        /// Initialises a new instance of the <see cref="CodeTypeReferenceRequestEventArgs"/> class.
        /// </summary>
        /// <param name="requestedType">The type to be created.</param>
        /// <param name="genericParameters">Any generic type parameters.</param>
        public CodeTypeReferenceRequestEventArgs(Type requestedType, CodeTypeReference[] genericParameters)
        {
            this.requestedType = requestedType;
            this.genericParameters = genericParameters;
        }

        /// <summary>
        /// Gets the type that a reference is being requested for.
        /// </summary>
        /// <value>The type that a reference is being requested for.</value>
        public Type RequestedType
        {
            get { return this.requestedType; }
        }

        /// <summary>
        /// Gets the generic parameters.
        /// </summary>
        /// <value>The generic parameters.</value>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1819", Justification = "No significant performance impact in this case")]
        public CodeTypeReference[] GenericParameters
        {
            get { return this.genericParameters; }
        }

        /// <summary>
        /// Gets or sets the code type reference.
        /// </summary>
        /// <value>The code type reference.</value>
        public CodeTypeReference CodeTypeReference
        {
            get { return this.codeTypeReference; }
            set { this.codeTypeReference = value; }
        }
    }
}
