//---------------------------------------------------------------------
// <copyright file="SimpleAsmxRequest.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
//     THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
//     OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
//     LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR
//     FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
// <summary>The SimpleAsmxRequest type.</summary>
//---------------------------------------------------------------------

namespace AsmxWebService
{
    using System.Xml.Serialization;

    /// <summary>
    /// Test request class.
    /// </summary>
    public class SimpleAsmxRequest
    {
        /// <summary>
        /// Test attribute.
        /// </summary>
        /// <value>Test value.</value>
        [XmlAttribute]
        public int A;

        /// <summary>
        /// Test attribute.
        /// </summary>
        /// <value>Test value.</value>
        public string B;

        /// <summary>
        /// Test attribute.
        /// </summary>
        /// <value>Test value.</value>
        [XmlIgnore]
        public int C;

        public int Optional;

        [XmlIgnore]
        public bool OptionalSpecified;
    }
}
