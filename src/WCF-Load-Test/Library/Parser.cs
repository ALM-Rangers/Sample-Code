//---------------------------------------------------------------------
// <copyright file="Parser.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
//     THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
//     OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
//     LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR
//     FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
// <summary>The Parser type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.WcfUnit.Library
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;

    /// <summary>
    /// Base class for parsing trace files.
    /// </summary>
    /// <remarks>
    /// The parser extracts selected request messages from a trace file. The derived classes process specific trace file formats.
    /// </remarks>
    public class Parser : IDisposable
    {
        /// <summary>
        /// The format-specific parser to be used.
        /// </summary>
        private readonly IFormatParser formatParser;

        /// <summary>
        /// Indicates how to process the list of SOAP actions.
        /// </summary>
        private readonly SoapActionMode soapActionMode;

        /// <summary>
        /// SOAP actions to be filtered for
        /// </summary>
        private readonly List<string> soapActions;

        /// <summary>
        /// Indicates if the object has already been disposed.
        /// </summary>
        private bool disposed;

        /// <summary>
        /// Initialises a new instance of the <see cref="Parser"/> class.
        /// </summary>
        /// <param name="formatParser">The format-specific parser to be used.</param>
        /// <param name="soapActionMode">Indicates how to process the list of SOAP actions.</param>
        /// <param name="soapActions">List of SOAP actions to be filtered for. Only the SOAP actions in this list are
        /// returned. If the list is empty all SOAP actions are returned.</param>
        public Parser(IFormatParser formatParser, SoapActionMode soapActionMode, params string[] soapActions)
        {
            this.formatParser = formatParser;

            this.soapActionMode = soapActionMode;
            if (soapActions == null)
            {
                this.soapActions = new List<string>();
            }
            else
            {
                this.soapActions = new List<string>(soapActions);
            }
        }

        /// <summary>
        /// Finalises an instance of the <see cref="Parser"/> class.
        /// </summary>
        ~Parser()
        {
            this.Dispose(false);
        }
        
        /// <summary>
        /// Opens a trace file for reading.
        /// </summary>
        /// <param name="fileName">The name of the file to be opened.</param>
        /// <returns>The stream that represents the opened file.</returns>
        public static Stream OpenTraceFile(string fileName)
        {
            if (!File.Exists(fileName))
            {
                throw new UserException(string.Format(CultureInfo.CurrentCulture, Messages.Parser_FileNotFound, fileName));
            }

            Stream ans = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);

            return ans;
        }

        /// <summary>
        /// Creates and sets up a parser for operation.
        /// </summary>
        /// <param name="formatParserType">The configuration information for the format parser to use, WCF is used if this is null.</param>
        /// <param name="traceFile">The trace file to be parsed.</param>
        /// <param name="fileName">The name of the trace file being parsed. Used in error messages and tracing only.</param>
        /// <param name="clientTrace">True if the parser is to search for client side trace entries.</param>
        /// <param name="serviceTrace">True if the parser is to search for service side trace entries.</param>
        /// <param name="soapActionMode">Indicates how to process the list of SOAP actions.</param>
        /// <param name="soapActions">List of SOAP actions to be filtered for. Only the SOAP actions in this list are
        /// returned. If the list is empty all SOAP actions are returned.</param>
        /// <returns>The parser that has been created and set up.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "disposable format parser passed to parser which is also disposable and will dispose the format parser")]
        public static Parser CreateParser(typeType formatParserType, Stream traceFile, string fileName, bool clientTrace, bool serviceTrace, SoapActionMode soapActionMode, params string[] soapActions)
        {
            IFormatParser formatParser = null;
            if (formatParserType == null)
            {
                formatParser = new WcfParser();
            }
            else
            {
                formatParser = Utility.CreateConfiguredType<IFormatParser>(formatParserType);
            }
            
            formatParser.Setup(traceFile, fileName, clientTrace, serviceTrace);
            Parser ans = new Parser(formatParser, soapActionMode, soapActions);

            return ans;
        }

        /// <summary>
        /// Disposes the object.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Gets the nest message from the file, subject to the list of SOAP actions passed to the constructor.
        /// </summary>
        /// <returns><see cref="ParsedMessage"/> containing the message and its details, or <b>null</b> if none found.</returns>
        public ParsedMessage ReadNextRequest()
        {
            ParsedMessage ans = null;
            bool done = false;
            while (!done)
            {
                ans = this.formatParser.ReadNextMessage();
                if (ans != null)
                {
                    Trace.Write(string.Format(CultureInfo.CurrentCulture, "Parsed message with action {0}", ans.SoapAction));
                    Console.Write(string.Format(CultureInfo.CurrentCulture, Messages.Parser_ParsedMessage, ans.SoapAction));
                    if (this.IsMessageToBeSelected(ans.SoapAction))
                    {
                        done = true;
                        Trace.WriteLine(" [OK]");
                        Console.WriteLine(Messages.Parser_OK);
                    }
                    else
                    {
                        Trace.WriteLine(" [FILTERED OUT]");
                        Console.WriteLine(Messages.Parser_FilteredOut);
                    }
                }
                else
                {
                    done = true;
                }
            }

            return ans;
        }

        /// <summary>
        /// Checks to see if a message with the given SOAP action is to be selected.
        /// </summary>
        /// <param name="soapAction">The soap action to be checked.</param>
        /// <returns>True if the message is to be selected.</returns>
        private bool IsMessageToBeSelected(string soapAction)
        {
            bool ans = false;
            if (this.soapActionMode == SoapActionMode.Include)
            {
                ans = this.soapActions.Count == 0 || this.soapActions.Contains(soapAction);
            }
            else
            {
                ans = this.soapActions.Count == 0 || !this.soapActions.Contains(soapAction);
            }

            return ans;
        }

        /// <summary>
        /// Disposes this instance.
        /// </summary>
        /// <param name="disposing">True if explicit disposal has been requested.</param>
        private void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!this.disposed)
            {
                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if (disposing)
                {
                    this.formatParser.Dispose();
                }

                // Note disposing has been done.
                this.disposed = true;
            }
        }
    }
}
