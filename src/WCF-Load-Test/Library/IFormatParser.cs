//---------------------------------------------------------------------
// <copyright file="IFormatParser.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
//     THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
//     OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
//     LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR
//     FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
// <summary>The IFormatParser type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.WcfUnit.Library
{
    using System;
    using System.IO;

    /// <summary>
    /// Defines a format-specific parser.
    /// </summary>
    public interface IFormatParser : IDisposable
    {
        /// <summary>
        /// Sets up the format specific parser for operation.
        /// </summary>
        /// <param name="traceFile">The name of the trace file to be parsed.</param>
        /// <param name="fileName">The name of the trace file being parsed. Used in error messages and tracing only.</param>
        /// <param name="clientTrace">True if the parser is to search for client side trace entries.</param>
        /// <param name="serviceTrace">True if the parser is to search for service side trace entries.</param>
        void Setup(Stream traceFile, string fileName, bool clientTrace, bool serviceTrace);

        /// <summary>
        /// Reads the next valid request message from the trace file.
        /// </summary>
        /// <remarks>
        /// This method may do some initial filtering. For example in WCF metadata exchange messages are filtered out here.
        /// </remarks>
        /// <returns>The next request message from the trace file.</returns>
        ParsedMessage ReadNextMessage();
    }
}
