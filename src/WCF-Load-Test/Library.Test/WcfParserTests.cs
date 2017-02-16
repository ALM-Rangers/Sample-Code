//---------------------------------------------------------------------
// <copyright file="WcfParserTests.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
//     THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
//     OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
//     LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR
//     FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
// <summary>The WcfParserTests type.</summary>
//---------------------------------------------------------------------

// TODO: Consider removing as many of the testdata files as possible.

namespace Microsoft.WcfUnit.Library.Test
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Tests the specific aspects of the <see cref="WcfParser"/> class.
    /// </summary>
    [TestClass]
    public class WcfParserTests
    {
        /// <summary>
        /// Test that WCF parser which is not setup can be disposed.
        /// </summary>
        [TestMethod]
        public void WcfParserDisposeAfterConstruction()
        {
            // Arrange, act and assert
            using (WcfParser sut = new WcfParser())
            {
            }
        }

        /// <summary>
        /// Test that a trace file containing no service level logging traces returns no data.
        /// </summary>
        [TestMethod]
        public void WcfParserNoServiceLevelLogging()
        {
            // Arrange
            using (IFormatParser sut = TestHelper.CreateWcfFormatParserClientTraceFromString(
                WcfTraceFileTestData.AddWithoutServiceLevelLogging,
                WcfTraceFileTestData.AddWithoutServiceLevelLogging,
                WcfTraceFileTestData.AddWithoutServiceLevelLogging))
            {
                // Act
                ParsedMessage ans = sut.ReadNextMessage();

                // Assert
                Assert.IsNull(ans);
            }
        }

        /// <summary>
        /// Tests that a message request body can be extracted from a WCF trace.
        /// </summary>
        [TestMethod]
        public void WcfParserReadRequestBody()
        {
            // Arrange
            using (IFormatParser sut = TestHelper.CreateWcfFormatParserClientTraceFromString(WcfTraceFileTestData.Add))
            {
                // Act
                ParsedMessage ans = sut.ReadNextMessage();

                // Assert
                TestHelper.AssertParsedMessage(ans, "http://tempuri.org/IArithmetic/Add", new DateTime(2007, 02, 21, 08, 39, 21, 966));
            }
        }

        /// <summary>
        /// Tests that null message is returned after reading the last message.
        /// </summary>
        [TestMethod]
        public void WcfParserNullReturnedAfterLastMessage()
        {
            // Arrange
            using (IFormatParser sut = TestHelper.CreateWcfFormatParserClientTraceFromString(WcfTraceFileTestData.Add))
            {
                // Act
                sut.ReadNextMessage();
                ParsedMessage ans = sut.ReadNextMessage();

                // Assert
                Assert.IsNull(ans);
            }
        }

        /// <summary>
        /// Tests that a message request body can be extracted from a WCF trace when preceded by a message with no service level logging.
        /// </summary>
        [TestMethod]
        public void WcfParserReadRequestBodyAfterANoServiceLevelLoggingMessage()
        {
            // Arrange
            using (IFormatParser sut = TestHelper.CreateWcfFormatParserClientTraceFromString(WcfTraceFileTestData.AddWithoutServiceLevelLogging, WcfTraceFileTestData.Add))
            {
                // Act
                ParsedMessage ans = sut.ReadNextMessage();

                // Assert
                TestHelper.AssertParsedMessage(ans, "http://tempuri.org/IArithmetic/Add", new DateTime(2007, 02, 21, 08, 39, 21, 966));
            }
        }

        /// <summary>
        /// Tests that a message request body can be extracted from a WCF trace when preceded by a message with no service level logging.
        /// </summary>
        [TestMethod]
        public void WcfParserNullReturnedIfANoServiceLevelLoggingMessageFollowsAProperMessageBody()
        {
            // Arrange
            using (IFormatParser sut = TestHelper.CreateWcfFormatParserClientTraceFromString(WcfTraceFileTestData.Add, WcfTraceFileTestData.AddWithoutServiceLevelLogging))
            {
                // Act
                sut.ReadNextMessage();
                ParsedMessage ans = sut.ReadNextMessage();

                // Assert
                Assert.IsNull(ans);
            }
        }

        /// <summary>
        /// Test that a message without the body is not returned.
        /// </summary>
        [TestMethod]
        public void WcfParserNoMessageBodies()
        {
            // Arrange
            using (IFormatParser sut = TestHelper.CreateWcfFormatParserClientTraceFromString(WcfTraceFileTestData.AddWithNoMessageBody))
            {
                // Act
                ParsedMessage ans = sut.ReadNextMessage();

                // Assert
                Assert.IsNull(ans);
            }
        }

        /// <summary>
        /// Test that reading a sequence of messages returns that sequence of messages and then a null.
        /// </summary>
        [TestMethod]
        public void WcfParserSequenceOfMessagesIsReadFollowedByNull()
        {
            // Arrange
            using (IFormatParser sut = TestHelper.CreateWcfFormatParserClientTraceFromString(WcfTraceFileTestData.Add, WcfTraceFileTestData.Add2, WcfTraceFileTestData.Add3))
            {
                // Act and assert
                TestHelper.AssertParsedMessage(sut.ReadNextMessage(), "http://tempuri.org/IArithmetic/Add", new DateTime(2007, 02, 21, 08, 39, 21, 966));
                TestHelper.AssertParsedMessage(sut.ReadNextMessage(), "http://tempuri.org/IArithmetic/Add2", new DateTime(2007, 02, 21, 08, 39, 22, 067));
                TestHelper.AssertParsedMessage(sut.ReadNextMessage(), "http://tempuri.org/IArithmetic/Add3", new DateTime(2007, 02, 21, 08, 39, 22, 071));

                // Assert
                Assert.IsNull(sut.ReadNextMessage());
            }
        }

        /// <summary>
        /// Tests that a MEX message is filtered out when it is the first message in the trace.
        /// </summary>
        [TestMethod]
        public void WcfParserWithMetadataExchangeAtTheStart()
        {
            // Arrange
            using (IFormatParser sut = TestHelper.CreateWcfFormatParserClientTraceFromString(WcfTraceFileTestData.Mex, WcfTraceFileTestData.Add))
            {
                // Act and assert
                TestHelper.AssertParsedMessage(sut.ReadNextMessage(), "http://tempuri.org/IArithmetic/Add", new DateTime(2007, 02, 21, 08, 39, 21, 966));
            }
        }

        /// <summary>
        /// Tests that a MEX message is filtered out when it is the last message in the trace.
        /// </summary>
        [TestMethod]
        public void WcfParserWithMetadataExchangeAtTheEnd()
        {
            // Arrange
            using (IFormatParser sut = TestHelper.CreateWcfFormatParserClientTraceFromString(WcfTraceFileTestData.Add, WcfTraceFileTestData.Mex))
            {
                // Act and assert
                TestHelper.AssertParsedMessage(sut.ReadNextMessage(), "http://tempuri.org/IArithmetic/Add", new DateTime(2007, 02, 21, 08, 39, 21, 966));
            }
        }

        /// <summary>
        /// Tests that RST message are filtered out.
        /// </summary>
        [TestMethod]
        public void WcfParserFiltersOutRSTMessages()
        {
            // Arrange
            using (IFormatParser sut = TestHelper.CreateWcfFormatParserClientTraceFromString(WcfTraceFileTestData.RstIssue, WcfTraceFileTestData.RstSct, WcfTraceFileTestData.RstrIssue))
            {
                // Act and assert
                Assert.IsNull(sut.ReadNextMessage());
            }
        }

        /// <summary>
        /// Tests that no message is returned if the message log only contains messages to be filtered out.
        /// </summary>
        [TestMethod]
        public void WcfParserReturnsNothingIfAllMessagesAreFiltered()
        {
            // Arrange
            using (IFormatParser sut = TestHelper.CreateWcfFormatParserClientTraceFromString(WcfTraceFileTestData.Mex))
            {
                // Act and assert
                Assert.IsNull(sut.ReadNextMessage());
            }
        }

        /// <summary>
        /// Tests that a service side log is read.
        /// </summary>
        [TestMethod]
        public void WcfParserFindsMessagesFromServiceLogWhenServiceTraceRequested()
        {
            // Arrange
            using (IFormatParser sut = TestHelper.CreateWcfFormatParserServiceTraceFromString(WcfTraceFileTestData.AddServiceSide, WcfTraceFileTestData.SubtractServiceSide))
            {
                // Act and assert
                TestHelper.AssertParsedMessage(sut.ReadNextMessage(), "http://tempuri.org/IArithmetic/Add", new DateTime(2007, 07, 28, 16, 35, 38, 593));
                TestHelper.AssertParsedMessage(sut.ReadNextMessage(), "http://tempuri.org/IArithmetic/Sub", new DateTime(2007, 07, 28, 16, 35, 40, 587));
                Assert.IsNull(sut.ReadNextMessage());
            }
        }

        /// <summary>
        /// Tests that reading a service log does not return anything if requesting a client trace.
        /// </summary>
        [TestMethod]
        public void WcfParserFindsNoMessagesFromServiceLogWhenClientTraceRequested()
        {
            // Arrange
            using (IFormatParser sut = TestHelper.CreateWcfFormatParserClientTraceFromString(WcfTraceFileTestData.AddServiceSide, WcfTraceFileTestData.SubtractServiceSide))
            {
                // Act and assert
                Assert.IsNull(sut.ReadNextMessage());
            }
        }

        /// <summary>
        /// Tests that reading a client log does not return anything if requesting a service trace.
        /// </summary>
        [TestMethod]
        public void WcfParserFindsNoMessagesFromClientLogWhenServiceTraceRequested()
        {
            // Arrange
            using (IFormatParser sut = TestHelper.CreateWcfFormatParserServiceTraceFromString(WcfTraceFileTestData.Add, WcfTraceFileTestData.Add2))
            {
                // Act and assert
                Assert.IsNull(sut.ReadNextMessage());
            }
        }

        /// <summary>
        /// Tests that both client and service side traces are picked up when client and service side traces are both requested.
        /// </summary>
        [TestMethod]
        public void WcfParserFindsClientAndServiceMessagesFromClientAndServiceLogWhenClientAndServiceTraceRequested()
        {
            // Arrange
            using (IFormatParser sut = TestHelper.CreateWcfFormatParserFromString(true, true, WcfTraceFileTestData.Add, WcfTraceFileTestData.SubtractServiceSide))
            {
                // Act and assert
                TestHelper.AssertParsedMessage(sut.ReadNextMessage(), "http://tempuri.org/IArithmetic/Add", new DateTime(2007, 02, 21, 08, 39, 21, 966));
                TestHelper.AssertParsedMessage(sut.ReadNextMessage(), "http://tempuri.org/IArithmetic/Sub", new DateTime(2007, 07, 28, 16, 35, 40, 587));
                Assert.IsNull(sut.ReadNextMessage());
            }
        }

        /// <summary>
        /// Tests that reading a service log does not return anything if requesting a client trace for a one way operation.
        /// </summary>
        [TestMethod]
        public void WcfParserFindsNoMessagesFromServiceLogWhenClientTraceRequestedForOneWayOperation()
        {
            // Arrange
            using (IFormatParser sut = TestHelper.CreateWcfFormatParserClientTraceFromString(WcfTraceFileTestData.OneWayServiceSide))
            {
                // Act and assert
                Assert.IsNull(sut.ReadNextMessage());
            }
        }

        /// <summary>
        /// Tests that reading a client log does not return anything if requesting a service trace for a one way operation.
        /// </summary>
        [TestMethod]
        public void WcfParserFindsNoMessagesFromClientLogWhenServiceTraceRequestedForOneWayOperation()
        {
            // Arrange
            using (IFormatParser sut = TestHelper.CreateWcfFormatParserServiceTraceFromString(WcfTraceFileTestData.OneWayClientSide))
            {
                // Act and assert
                Assert.IsNull(sut.ReadNextMessage());
            }
        }

        /// <summary>
        /// Tests that both client and service side one-way traces are picked up when client and service side traces are both requested.
        /// </summary>
        [TestMethod]
        public void WcfParserFindsClientAndServiceMessagesFromClientAndServiceLogWhenClientAndServiceTraceRequestedForOneWayOperation()
        {
            // Arrange
            using (IFormatParser sut = TestHelper.CreateWcfFormatParserFromString(true, true, WcfTraceFileTestData.OneWayClientSide, WcfTraceFileTestData.OneWayServiceSide))
            {
                // Act and assert
                TestHelper.AssertParsedMessage(sut.ReadNextMessage(), "http://contoso.com/service/test/IArithmetic/OneWayOperation", new DateTime(2009, 07, 23, 09, 52, 16, 616));
                TestHelper.AssertParsedMessage(sut.ReadNextMessage(), "http://contoso.com/service/test/IArithmetic/OneWayOperation", new DateTime(2009, 07, 23, 09, 54, 38, 155));
                Assert.IsNull(sut.ReadNextMessage());
            }
        }

        /// <summary>
        /// Test that can read a bare soap 1.1 message.
        /// </summary>
        [TestMethod]
        public void WcfParserCanParseAsmxBareSoap11()
        {
            // Arrange
            using (IFormatParser sut = TestHelper.CreateWcfFormatParserClientTraceFromString(WcfTraceFileTestData.ProcessSimpleAsmxRequestBareSoap11))
            {
                // Act and assert
                TestHelper.AssertParsedMessage(sut.ReadNextMessage(), "http://contoso.com/asmxservice/test/ProcessSimpleAsmxRequestBare", new DateTime(2008, 09, 17, 13, 31, 46, 602));
                Assert.IsNull(sut.ReadNextMessage());
            }
        }

        /// <summary>
        /// Test that can read a bare soap 1.2 message.
        /// </summary>
        [TestMethod]
        public void WcfParserCanParseAsmxBareSoap12()
        {
            // Arrange
            using (IFormatParser sut = TestHelper.CreateWcfFormatParserClientTraceFromString(WcfTraceFileTestData.ProcessSimpleAsmxRequestBareSoap12))
            {
                // Act and assert
                TestHelper.AssertParsedMessage(sut.ReadNextMessage(), "http://contoso.com/asmxservice/test/ProcessSimpleAsmxRequestBare", new DateTime(2008, 09, 17, 14, 31, 46, 666));
                Assert.IsNull(sut.ReadNextMessage());
            }
        }

        /// <summary>
        /// Test that can read a wrapped soap 1.1 message
        /// </summary>
        [TestMethod]
        public void WcfParserCanParseAsmxWrappedSoap11()
        {
            // Arrange
            using (IFormatParser sut = TestHelper.CreateWcfFormatParserClientTraceFromString(WcfTraceFileTestData.ProcessSimpleAsmxRequestWrappedSoap11))
            {
                // Act and assert
                TestHelper.AssertParsedMessage(sut.ReadNextMessage(), "http://contoso.com/asmxservice/test/ProcessSimpleAsmxRequestWrapped", new DateTime(2008, 09, 17, 14, 31, 45, 352));
                Assert.IsNull(sut.ReadNextMessage());
            }
        }

        /// <summary>
        /// Tests that can read a wrapped soap 1.2 message
        /// </summary>
        [TestMethod]
        public void WcfParserCanParseAsmxWrappedSoap12()
        {
            // Arrange
            using (IFormatParser sut = TestHelper.CreateWcfFormatParserClientTraceFromString(WcfTraceFileTestData.ProcessSimpleAsmxRequestWrappedSoap12))
            {
                // Act and assert
                TestHelper.AssertParsedMessage(sut.ReadNextMessage(), "http://contoso.com/asmxservice/test/ProcessSimpleAsmxRequestWrapped", new DateTime(2008, 09, 17, 14, 31, 46, 623));
                Assert.IsNull(sut.ReadNextMessage());
            }
        }

        /// <summary>
        /// In some circumstances the addressing namespace is not in the envelope but on the actual action header, this tests for that situation.
        /// </summary>
        [TestMethod]
        public void WcfParserCanParseWhenAddressingNamespaceOnActionHeaderRatherThanEnvelope()
        {
            // Arrange
            using (IFormatParser sut = TestHelper.CreateWcfFormatParserServiceTraceFromString(WcfTraceFileTestData.RequestWithAddressingNamespaceOnTheActionElement))
            {
                // Act
                ParsedMessage ans = sut.ReadNextMessage();

                // Assert
                TestHelper.AssertParsedMessage(ans, "http://tempuri.org/ICalculator/Ping", new DateTime(2009, 04, 24, 18, 10, 59, 668));
            }
        }

        /// <summary>
        /// In some circumstances the addressing namespace is not in the envelope but on the actual action header, this tests for that situation,
        /// when the action header is not the first one.
        /// </summary>
        [TestMethod]
        public void WcfParserCanParseWhenAddressingNamespaceOnActionHeaderRatherThanEnvelopeAndNotFirstHeader()
        {
            // Arrange
            using (IFormatParser sut = TestHelper.CreateWcfFormatParserServiceTraceFromString(WcfTraceFileTestData.RequestWithAddressingNamespaceOnTheActionElementAndNotFirstHeader))
            {
                // Act
                ParsedMessage ans = sut.ReadNextMessage();

                // Assert
                TestHelper.AssertParsedMessage(ans, "http://contoso.com/service/test/ISharePrices/PriceOneWay", new DateTime(2010, 06, 22, 15, 22, 51, 613));
            }
        }

        /// <summary>
        /// Check that we can parse a one-way client side message log
        /// </summary>
        [TestMethod]
        public void WcfParserCanParseAOneWayClientSideMessageLog()
        {
            // Arrange
            using (IFormatParser sut = TestHelper.CreateWcfFormatParserClientTraceFromString(WcfTraceFileTestData.OneWayClientSide))
            {
                // Act and assert
                TestHelper.AssertParsedMessage(sut.ReadNextMessage(), "http://contoso.com/service/test/IArithmetic/OneWayOperation", new DateTime(2009, 07, 23, 09, 52, 16, 616));
                Assert.IsNull(sut.ReadNextMessage());
            }
        }

        /// <summary>
        /// Check that we can parse a one-way service side message log
        /// </summary>
        [TestMethod]
        public void WcfParserCanParseAOneWayServiceSideMessageLog()
        {
            // Arrange
            using (IFormatParser sut = TestHelper.CreateWcfFormatParserServiceTraceFromString(WcfTraceFileTestData.OneWayServiceSide))
            {
                // Act and assert
                TestHelper.AssertParsedMessage(sut.ReadNextMessage(), "http://contoso.com/service/test/IArithmetic/OneWayOperation", new DateTime(2009, 07, 23, 09, 54, 38, 155));
                Assert.IsNull(sut.ReadNextMessage());
            }
        }

        /// <summary>
        /// Check that null messages are ignored in service side message logs.
        /// </summary>
        [TestMethod]
        public void WcfParserIgnoresServiceSideNullMessage()
        {
            // Arrange
            using (IFormatParser sut = TestHelper.CreateWcfFormatParserServiceTraceFromString(WcfTraceFileTestData.ServiceSideNullMessage))
            {
                // Act and assert
                Assert.IsNull(sut.ReadNextMessage());
            }
        }

        /// <summary>
        /// Checks that if a trace file is used instead of a log file, this is explicitly mentioned in the error message 
        /// </summary>
        [TestMethod]
        public void WcfParserWarnsNotMessageLogIfFindsATrace()
        {
            // Arrange
            using (IFormatParser sut = TestHelper.CreateWcfFormatParserClientTraceFromString(WcfTraceFileTestData.TraceRecord))
            {
                // Act and assert
                TestHelper.TestForUserException(() => sut.ReadNextMessage(), "The file in memory is a trace file, a message log file is required.");
            }
        }

        /// <summary>
        /// Checks that a non XML file generates a warning about an invalid log file.
        /// </summary>
        [TestMethod]
        public void WcfParserWarnsNotMessageLogIfGivenNonXmlFile()
        {
            // Arrange
            using (IFormatParser sut = TestHelper.CreateWcfFormatParserClientTraceFromString(WcfTraceFileTestData.RandomTextTraceRecord))
            {
                // Act and assert
                TestHelper.TestForUserException(() => sut.ReadNextMessage(), "The file in memory is not a valid message log file.");
            }
        }

        /// <summary>
        /// Checks that an  XML file which is nothing to do with WCF generates a warning about an invalid log file.
        /// </summary>
        [TestMethod]
        public void WcfParserWarnsNotMessageLogIfGivenRandomXmlFile()
        {
            // Arrange
            using (IFormatParser sut = TestHelper.CreateWcfFormatParserClientTraceFromString(WcfTraceFileTestData.RandomXmlData))
            {
                // Act and assert
                TestHelper.TestForUserException(() => sut.ReadNextMessage(), "The file in memory is not a valid message log file.");
            }
        }

        /// <summary>
        /// Checks that a log file without a source generates a warning about an invalid log file.
        /// </summary>
        [TestMethod]
        public void WcfParserWarnsNotMessageLogIfSourceIsMissing()
        {
            // Arrange
            using (IFormatParser sut = TestHelper.CreateWcfFormatParserClientTraceFromString(WcfTraceFileTestData.LogRecordWithoutASource))
            {
                // Act and assert
                TestHelper.TestForUserException(() => sut.ReadNextMessage(), "The file in memory is not a valid message log file.");
            }
        }

        /// <summary>
        /// Checks that a log file with a source that does not have the name attribute generates a warning about an invalid log file.
        /// </summary>
        [TestMethod]
        public void WcfParserWarnsNotMessageLogIfSourcePresentButNameIsMissing()
        {
            // Arrange
            using (IFormatParser sut = TestHelper.CreateWcfFormatParserClientTraceFromString(WcfTraceFileTestData.LogRecordWithSourceButNoSourceName))
            {
                // Act and assert
                TestHelper.TestForUserException(() => sut.ReadNextMessage(), "The file in memory is not a valid message log file.");
            }
        }
    }
}
