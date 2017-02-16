//---------------------------------------------------------------------
// <copyright file="FiddlerTextParser.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
//     THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
//     OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
//     LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR
//     FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
// <summary>The FiddlerTextParser type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.WcfUnit.Library
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.ServiceModel.Channels;
    using System.Text.RegularExpressions;
    using System.Xml;

    /// <summary>
    /// Used to parse Fiddler sessions saved as text files.
    /// </summary>
    /// <remarks>
    /// The client and service side flags are ignored by this class as the traces look the same and cannot be differentiated.
    /// </remarks>
    public class FiddlerTextParser : FormatParserBase
    {
        /// <summary>
        /// The pattern used to the soap action.
        /// </summary>
        private static Regex soapActionPattern = new Regex(@"^SOAPAction: ""(?<url>.*)""$");

        /// <summary>
        /// The stream reader used to read lines of text from the trace file.
        /// </summary>
        private TextReader textReader;

        /// <summary>
        /// Sets up to read the WCF trace file.
        /// </summary>
        protected override void SetupCore()
        {
            this.textReader = new MixedStreamAndXmlReader(this.TraceFile);
        }

        /// <summary>
        /// Reads the next valid request message from the trace file.
        /// </summary>
        /// <remarks>
        /// This method does some initial filtering.
        /// </remarks>
        /// <returns>The next request message from the trace file.</returns>
        protected override ParsedMessage ReadNextMessageCore()
        {
            ParsedMessage ans = null;
            string line;
            while ((line = this.textReader.ReadLine()) != null)
            {
                string soapAction = null;
                if ((soapAction = CheckLineForSoapAction(line)) != null)
                {
                    this.ReadToAfterNextBlankLine();

                    XmlReaderSettings settings = new XmlReaderSettings();
                    settings.CloseInput = false;
                    settings.ConformanceLevel = ConformanceLevel.Fragment;
                    settings.IgnoreWhitespace = true;

                    using (XmlReader envelopeReader = XmlReader.Create(this.textReader, settings))
                    {
                        envelopeReader.MoveToContent();
                        MessageVersion messageVersion = GetMessageVersion(envelopeReader);

                        using (Message tempMessage = Message.CreateMessage(envelopeReader.ReadSubtree(), int.MaxValue, messageVersion))
                        {
                            Trace.WriteLine(string.Format(CultureInfo.CurrentCulture, "Read message with soap action = {0}", tempMessage.Headers.Action));
                            ans = new ParsedMessage(tempMessage.CreateBufferedCopy(int.MaxValue), soapAction, DateTime.MinValue);
                        }
                    }

                    // Read and discard the response
                    this.ReadToAfterNextBlankLine(); // reader is positioned at EOL so looks like a blank line when it isn't
                    this.ReadToAfterNextBlankLine();
                    using (XmlReader envelopeReader = XmlReader.Create(this.textReader, settings))
                    {
                        envelopeReader.MoveToContent();
                        envelopeReader.ReadOuterXml();
                    }

                    break;
                }
            }

            return ans;
        }

        /// <summary>
        /// Disposes this instance.
        /// </summary>
        /// <param name="disposing">True if explicit disposal has been requested.</param>
        protected override void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!this.Disposed)
            {
                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if (disposing)
                {
                    if (this.textReader != null)
                    {
                        this.textReader.Dispose();
                    }
                }

                // Note disposing has been done.
                this.Disposed = true;
            }
        }

        /// <summary>
        /// Checks the line for a Soap action, returning the soap action if it is found, or null otherwise.
        /// </summary>
        /// <param name="line">The line to be checked.</param>
        /// <returns>The soap action, or null if none.</returns>
        private static string CheckLineForSoapAction(string line)
        {
            string ans = null;

            Match m = soapActionPattern.Match(line);
            if (m.Success)
            {
                ans = m.Groups[1].Captures[0].Value;
                Trace.WriteLine(string.Format(CultureInfo.InvariantCulture, "Fiddler text parser parsed soap action {0}", ans));
            }

            return ans;
        }

        /// <summary>
        /// Reads up to after the next blank line.
        /// </summary>
        private void ReadToAfterNextBlankLine()
        {
            while (!string.IsNullOrEmpty(this.textReader.ReadLine()))
            {
            }
        }

        /// <summary>
        /// Text reader used to read mixed files containing flat data and XML.
        /// </summary>
        /// <remarks>
        /// Assumes that the XmlReader is the only thing that wants to read in blocks using the Read overload that requests a block of data.
        /// Forces all these reads to read one character at a time so that can then resume reading flat data at the point the XmlReader has
        /// finished. This will make it slow but it avoids the need for a seekable stream and the need to parse XML in order to skip past it.
        /// </remarks>
        private class MixedStreamAndXmlReader : TextReader
        {
            /// <summary>
            /// The underlying StreamReader used to read the stream.
            /// </summary>
            private StreamReader sr;

            /// <summary>
            /// Initialises a new instance of the <see cref="MixedStreamAndXmlReader"/> class.
            /// </summary>
            /// <param name="stream">The stream to be read.</param>
            public MixedStreamAndXmlReader(Stream stream)
            {
                this.sr = new StreamReader(stream);
            }

            /// <summary>
            /// See <see cref="TextReader.Peek"/>.
            /// </summary>
            /// <returns>See <see cref="TextReader.Peek"/> return value.</returns>
            public override int Peek()
            {
                return this.sr.Peek();
            }

            /// <summary>
            /// See <see cref="TextReader.Read"/>.
            /// </summary>
            /// <returns>See <see cref="TextReader.Read"/> return value.</returns>
            public override int Read()
            {
                return this.sr.Read();
            }

            /// <summary>
            /// See <see cref="TextReader.Read"/>.
            /// </summary>
            /// <param name="buffer">See the buffer parameter in <see cref="TextReader.Read"/>.</param>
            /// <param name="index">See the index parameter in <see cref="TextReader.Read"/></param>
            /// <param name="count">See the count parameter in <see cref="TextReader.Read"/></param>
            /// <returns>See <see cref="TextReader.ReadBlock"/> return value.</returns>
            /// <returns>See <see cref="TextReader.Read"/> return value.</returns>
            public override int Read(char[] buffer, int index, int count)
            {
                return this.sr.Read(buffer, index, 1); // Force a single-character read.
            }

            /// <summary>
            /// See <see cref="TextReader.ReadBlock"/>.
            /// </summary>
            /// <param name="buffer">See the buffer parameter in <see cref="TextReader.ReadBlock"/>.</param>
            /// <param name="index">See the index parameter in <see cref="TextReader.ReadBlock"/></param>
            /// <param name="count">See the count parameter in <see cref="TextReader.ReadBlock"/></param>
            /// <returns>See <see cref="TextReader.ReadBlock"/> return value.</returns>
            public override int ReadBlock(char[] buffer, int index, int count)
            {
                int ans = this.sr.ReadBlock(buffer, index, 1); // Force a single-character read.
                return ans;
            }

            /// <summary>
            /// See <see cref="TextReader.ReadLine"/>.
            /// </summary>
            /// <returns>See <see cref="TextReader.ReadLine"/> return value.</returns>
            public override string ReadLine()
            {
                string ans = this.sr.ReadLine();
                return ans;
            }

            /// <summary>
            /// See <see cref="TextReader.ReadToEnd"/>.
            /// </summary>
            /// <returns>See <see cref="TextReader.ReadToEnd"/> return value.</returns>
            public override string ReadToEnd()
            {
                return this.sr.ReadToEnd();
            }
        }
    }
}
