//---------------------------------------------------------------------
// <copyright file="WcfParser.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
//     THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
//     OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
//     LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR
//     FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
// <summary>The WcfParser type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.WcfUnit.Library
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.Xml;

    /// <summary>
    /// Parses WCF trace files.
    /// </summary>
    /// <remarks>
    /// Requires the log file to have been created with the LogMessagesAtServiceLevel and LogEntireMessage settings to be set, otherwise it
    /// will not find any messages.
    /// </remarks>
    public class WcfParser : FormatParserBase
    {
        /// <summary>
        /// The namespace for the system element
        /// </summary>
        private const string SystemNamespace = "http://schemas.microsoft.com/2004/06/windows/eventlog/system";

        /// <summary>
        /// The namespace for the message trace.
        /// </summary>
        private const string TraceNamespace = "http://schemas.microsoft.com/2004/06/ServiceModel/Management/MessageTrace";

        /// <summary>
        /// The namespace of the root E2E trace event element
        /// </summary>
        private const string E2ETraceNamespace = "http://schemas.microsoft.com/2004/06/E2ETraceEvent";

        /// <summary>
        /// The name of the root E2E trace event element
        /// </summary>
        private const string E2ETraceElementName = "E2ETraceEvent";

        /// <summary>
        /// The name of the message log trace record element.
        /// </summary>
        private const string MessageLogTraceRecordElementName = "MessageLogTraceRecord";

        /// <summary>
        /// The type of the null message.
        /// </summary>
        private const string NullMessageType = "System.ServiceModel.Channels.NullMessage";

        /// <summary>
        /// The name of the source used for message logging.
        /// </summary>
        private const string MessageLoggingSourceName = "System.ServiceModel.MessageLogging";

        /// <summary>
        /// The name of the HttpRequest element.
        /// </summary>
        private const string HttpRequestElementName = "HttpRequest";

        /// <summary>
        /// The name of the Soap Action element.
        /// </summary>
        private const string SoapActionElementName = "Action";

        /// <summary>
        /// The name of the system source element
        /// </summary>
        private const string SourceElementName = "Source";

        /// <summary>
        /// The name of the name attribute.
        /// </summary>
        private const string NameAttributeName = "Name";

        /// <summary>
        /// The name of the source attribute.
        /// </summary>
        private const string SourceAttributeName = "Source";

        /// <summary>
        /// The name of the type attribute.
        /// </summary>
        private const string TypeAttributeName = "Type";

        /// <summary>
        /// The name of the time attribute.
        /// </summary>
        private const string TimeAttributeName = "Time";

        /// <summary>
        /// List of Soap actions to always filter out.
        /// </summary>
        private static List<string> alwaysFilterOut = new List<string>(new string[]
        {
            "http://schemas.xmlsoap.org/ws/2004/09/transfer/Get",
            "http://schemas.xmlsoap.org/ws/2005/02/trust/RST/Issue",
            "http://schemas.xmlsoap.org/ws/2005/02/trust/RST/SCT",
            "http://schemas.xmlsoap.org/ws/2005/02/trust/RSTR/Issue"
        });

        /// <summary>
        /// List of trace record sources that indicate a client-side message log.
        /// </summary>
        private static List<string> clientSource = new List<string>(new string[] { "ServiceLevelSendRequest", "ServiceLevelSendDatagram" });

        /// <summary>
        /// List of trace record sources that indicate a service-side message log.
        /// </summary>
        private static List<string> serviceSource = new List<string>(new string[] { "ServiceLevelReceiveRequest", "ServiceLevelReceiveDatagram" });

        /// <summary>
        /// The XmlReader to be used to read the log file.
        /// </summary>
        private XmlReader reader;

        /// <summary>
        /// Sets up to read the WCF trace file.
        /// </summary>
        protected override void SetupCore()
        {
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.CloseInput = true;
            settings.ConformanceLevel = ConformanceLevel.Fragment;
            settings.IgnoreWhitespace = true;

            try
            {
                this.reader = XmlReader.Create(this.TraceFile, settings);
                this.reader.Read();
            }
            catch (IOException ioe)
            {
                throw new UserException(ioe.Message, ioe);
            }
        }

        /// <summary>
        /// Reads the next valid request message from the trace file.
        /// </summary>
        /// <remarks>
        /// This method does some initial filtering. WCF metadata exchange messages are filtered out here.
        /// </remarks>
        /// <returns>The next request message from the trace file.</returns>
        protected override ParsedMessage ReadNextMessageCore()
        {
            ParsedMessage ans = null;

            try
            {
                bool done = false;
                while (!done && !this.reader.EOF)
                {
                    if (this.reader.Name != E2ETraceElementName && this.reader.NamespaceURI != E2ETraceNamespace)
                    {
                        this.ThrowInvalidMessageLog(null);
                    }

                    if (this.reader.ReadToFollowing(SourceElementName, SystemNamespace))
                    {
                        if (this.reader.MoveToAttribute(NameAttributeName))
                        {
                            if (this.reader.Value != MessageLoggingSourceName)
                            {
                                throw new UserException(string.Format(CultureInfo.CurrentCulture, Messages.Parser_TraceFileFound, this.FileName));
                            }
                        }
                        else
                        {
                            this.ThrowInvalidMessageLog(null);
                        }
                    }
                    else
                    {
                        this.ThrowInvalidMessageLog(null);
                    }

                    if (this.reader.ReadToFollowing(MessageLogTraceRecordElementName, TraceNamespace))
                    {
                        if (this.reader.MoveToAttribute(SourceAttributeName))
                        {
                            if ((this.ClientTrace && clientSource.Contains(this.reader.Value)) || (this.ServiceTrace && serviceSource.Contains(this.reader.Value)))
                            {
                                if (this.reader.MoveToAttribute(TypeAttributeName))
                                {
                                    if (this.reader.Value != NullMessageType)
                                    {
                                        ParsedMessage temp = this.ProcessMessageLogTraceRecord();
                                        if (temp == null || IsMessageToBeSelected(temp.SoapAction))
                                        {
                                            done = true;
                                            ans = temp;
                                        }
                                    }
                                }
                            }
                        }

                        this.reader.ReadToFollowing(E2ETraceElementName, E2ETraceNamespace);
                    }
                    else
                    {
                        done = true;
                    }
                }
            }
            catch (XmlException xe)
            {
                this.ThrowInvalidMessageLog(xe);
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
                    if (this.reader != null)
                    {
                        this.reader.Close();
                    }
                }

                // Note disposing has been done.
                this.Disposed = true;
            }
        }

        /// <summary>
        /// Filters out metadata exchange messages.
        /// </summary>
        /// <param name="soapAction">The soap action to be checked.</param>
        /// <returns>True if the message is to be returned to the caller.</returns>
        private static bool IsMessageToBeSelected(string soapAction)
        {
            bool ans = true;
            if (alwaysFilterOut.Contains(soapAction))
            {
                ans = false;
            }

            return ans;
        }

        /// <summary>
        /// Gets the SOAP action from the message.
        /// </summary>
        /// <remarks>
        /// Also sets the Action property if it was not set. This can happen if the addressing namespace is not on the message envelope but on
        /// the individual headers instead.
        /// </remarks>
        /// <param name="message">The message to get the SOAP action from.</param>
        /// <returns>The SOAP action.</returns>
        private static string GetSoapAction(Message message)
        {
            string ans = message.Headers.Action;
            if (string.IsNullOrEmpty(ans))
            {
                int i;
                i = message.Headers.FindHeader(SoapActionElementName, FormatParserBase.Addressing10Namespace);
                if (i < 0)
                {
                    i = message.Headers.FindHeader(SoapActionElementName, FormatParserBase.AddressingAugust2004Namespace);
                }

                if (i >= 0)
                {
                    ans = message.Headers.GetHeader<string>(i);
                    message.Headers.Action = ans;
                }
            }

            Trace.WriteLine(string.Format(CultureInfo.CurrentCulture, "Read message with soap action = {0}", ans));
            return ans;
        }

        /// <summary>
        /// Throws exception to warn of an invalid message log file.
        /// </summary>
        /// <param name="inner">Any inner exception.</param>
        private void ThrowInvalidMessageLog(Exception inner)
        {
            throw new UserException(string.Format(CultureInfo.CurrentCulture, Messages.Parser_InvalidMessageLogFile, this.FileName), inner);
        }

        /// <summary>
        /// Process a message log trace record.
        /// </summary>
        /// <returns>The parsed message, or null if the message had no body.</returns>
        private ParsedMessage ProcessMessageLogTraceRecord()
        {
            ParsedMessage ans = null;
            DateTime timestamp = DateTime.MinValue;

            if (this.reader.MoveToAttribute(TimeAttributeName))
            {
                timestamp = DateTime.Parse(this.reader.Value, CultureInfo.InvariantCulture);
            }

            // Move to next element. Service side trace contains a HttpRequest node, if present skip this
            while (this.reader.Read() && this.reader.NodeType != XmlNodeType.Element)
            {
            }
            ////this._reader.Skip();
            if (this.reader.Name == HttpRequestElementName)
            {
                this.reader.Skip();
            }

            MessageVersion messageVersion = GetMessageVersion(this.reader);

            XmlReader envelopeReader = this.reader.ReadSubtree();
            Message tempMessage = null;
            try
            {
                using (tempMessage = Message.CreateMessage(envelopeReader, int.MaxValue, messageVersion))
                {
                    string soapAction = GetSoapAction(tempMessage);
                    ans = new ParsedMessage(tempMessage.CreateBufferedCopy(int.MaxValue), soapAction, timestamp);
                }
            }
            catch (CommunicationException)
            {
                // ignore if no body and just return null
            }
            finally
            {
                envelopeReader.Close();
            }

            return ans;
        }
    }
}
