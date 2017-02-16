//---------------------------------------------------------------------
// <copyright file="FormatParserBase.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
//     THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
//     OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
//     LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR
//     FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
// <summary>The FormatParserBase type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.WcfUnit.Library
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.Xml;

    /// <summary>
    /// The base format-specific parser.
    /// </summary>
    /// <remarks>
    /// The implementer must override <see cref="SetupCore"/> and <see cref="ReadNextMessageCore"/> rather than the interface methods so that
    /// the base class can ensure that the calling protocol is obeyed.
    /// </remarks>
    public abstract class FormatParserBase : IFormatParser
    {
        /// <summary>
        /// SOAP 1.1 namespace.
        /// </summary>
        protected const string Soap11Namespace = "http://schemas.xmlsoap.org/soap/envelope/";

        /// <summary>
        /// SOAP 1.2 namespace
        /// </summary>
        protected const string Soap12Namespace = "http://www.w3.org/2003/05/soap-envelope";

        /// <summary>
        /// Addressing namespace.
        /// </summary>
        protected const string Addressing10Namespace = "http://www.w3.org/2005/08/addressing";

        /// <summary>
        /// August 2004 addressing namespace.
        /// </summary>
        protected const string AddressingAugust2004Namespace = "http://schemas.xmlsoap.org/ws/2004/08/addressing";

        /// <summary>
        /// Namespace for the namespace
        /// </summary>
        protected const string NamespaceNamespace = "http://www.w3.org/2000/xmlns/";

        /// <summary>
        /// Flag used to check if Setup has already been called.
        /// </summary>
        private bool alreadySetup;

        /// <summary>
        /// Finalises an instance of the <see cref="FormatParserBase"/> class.
        /// </summary>
        ~FormatParserBase()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance has been disposed.
        /// </summary>
        /// <value>A value indicating whether this instance has been disposed.</value>
        protected bool Disposed
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the stream that is used to read the trace file.
        /// </summary>
        /// <value>The stream that is used to read the trace file.</value>
        protected Stream TraceFile
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the name of the trace file being read. Used for error messages and tracing.
        /// </summary>
        /// <value>The name of the trace file being read.</value>
        protected string FileName
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a value indicating whether this is a client-side trace.
        /// </summary>
        /// <value>A value indicating whether this is a client-side trace.</value>
        protected bool ClientTrace
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a value indicating whether this is a service-side trace.
        /// </summary>
        /// <value>A value indicating whether this is a service-side trace</value>
        protected bool ServiceTrace
        {
            get;
            private set;
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
        /// Sets up the format specific parser for operation.
        /// </summary>
        /// <remarks>
        /// Override this method for specific setup, always call the base method at the start.
        /// </remarks>
        /// <param name="traceFile">The name of the trace file to be parsed.</param>
        /// <param name="fileName">The name of the trace file being parsed. Used in error messages and tracing only.</param>
        /// <param name="clientTrace">True if the parser is to search for client side trace entries.</param>
        /// <param name="serviceTrace">True if the parser is to search for service side trace entries.</param>
        public void Setup(Stream traceFile, string fileName, bool clientTrace, bool serviceTrace)
        {
            if (this.alreadySetup)
            {
                throw new InvalidOperationException(Messages.Parser_SetupAlreadyDone);
            }

            Trace.WriteLine(string.Format(CultureInfo.InvariantCulture, "Parsing trace file: {0}", fileName));

            this.TraceFile = traceFile;
            this.FileName = fileName;
            this.ClientTrace = clientTrace;
            this.ServiceTrace = serviceTrace;

            this.alreadySetup = true;

            this.SetupCore();
        }

        /// <summary>
        /// Reads the next valid request message from the trace file.
        /// </summary>
        /// <remarks>
        /// This method may do some initial filtering. For example in WCF metadata exchange messages are filtered out here.
        /// </remarks>
        /// <returns>The next request message from the trace file.</returns>
        public ParsedMessage ReadNextMessage()
        {
            if (!this.alreadySetup)
            {
                throw new InvalidOperationException(Messages.Parser_SetupNotCalled);
            }

            return this.ReadNextMessageCore();
        }

        /// <summary>
        /// Gets the message version for the message.
        /// </summary>
        /// <param name="reader">The reader, must be positioned on the envelope.</param>
        /// <returns>The message version for the message.</returns>
        protected static MessageVersion GetMessageVersion(XmlReader reader)
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }

            EnvelopeVersion envelopeVersion;
            if (reader.NamespaceURI == Soap11Namespace)
            {
                envelopeVersion = EnvelopeVersion.Soap11;
            }
            else if (reader.NamespaceURI == Soap12Namespace)
            {
                envelopeVersion = EnvelopeVersion.Soap12;
            }
            else
            {
                throw new UserException(string.Format(CultureInfo.CurrentCulture, Messages.UnexpectedSoapNamespace, reader.NamespaceURI));
            }

            AddressingVersion addressingVersion = AddressingVersion.None;
            for (int i = 0; i < reader.AttributeCount; i++)
            {
                reader.MoveToAttribute(i);
                if (reader.NamespaceURI == NamespaceNamespace)
                {
                    if (reader.Value == Addressing10Namespace)
                    {
                        addressingVersion = AddressingVersion.WSAddressing10;
                    }
                    else if (reader.Value == AddressingAugust2004Namespace)
                    {
                        addressingVersion = AddressingVersion.WSAddressingAugust2004;
                    }
                }
            }

            MessageVersion messageVersion = MessageVersion.CreateVersion(envelopeVersion, addressingVersion);
            reader.MoveToElement();
            return messageVersion;
        }

        /// <summary>
        /// Disposes this instance.
        /// </summary>
        /// <param name="disposing">True if explicit disposal has been requested.</param>
        protected abstract void Dispose(bool disposing);

        /// <summary>
        /// Called by <see cref="Setup"/> to perform format-specific setup
        /// </summary>
        protected abstract void SetupCore();

        /// <summary>
        /// Called by <see cref="ReadNextMessage"/> to perform the actual format-specific read.
        /// </summary>
        /// <returns>The next request message from the trace file.</returns>
        protected abstract ParsedMessage ReadNextMessageCore();
    }
}
