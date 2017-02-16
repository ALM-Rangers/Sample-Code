//---------------------------------------------------------------------
// <copyright file="FiddlerTextParserTests.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
//     THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
//     OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
//     LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR
//     FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
// <summary>The FiddlerTextParserTests type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.WcfUnit.Library.Test
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Tests the specific aspects of the <see cref="FiddlerTextParser"/> class.
    /// </summary>
    [TestClass]
    public class FiddlerTextParserTests
    {
        /// <summary>
        /// Test that Fiddler parser which is not setup can be disposed.
        /// </summary>
        [TestMethod]
        public void FiddlerParserDisposeAfterConstruction()
        {
            // Arrange, act and assert
            using (FiddlerTextParser sut = new FiddlerTextParser())
            {
            }
        }

        /// <summary>
        /// Test that a message can be retrieved from a Fiddler trace.
        /// </summary>
        [TestMethod]
        public void FiddlerParserReadSingleMessage()
        {
            // Arrange
            using (IFormatParser sut = TestHelper.CreateFiddlerFormatParserFromString(FiddlerTraceFileTestData.ProcessSimpleAsmxRequestWrapped))
            {
                // Act
                ParsedMessage ans = sut.ReadNextMessage();

                // Assert
                TestHelper.AssertParsedMessageNoHeader(ans, "http://contoso.com/asmxservice/test/ProcessSimpleAsmxRequestWrapped", DateTime.MinValue);
            }
        }

        /// <summary>
        /// Test that a message retrieved from a Fiddler trace can be deserialized.
        /// </summary>
        [TestMethod]
        public void FiddlerParserMessageCanBeDeserialized()
        {
            // Arrange
            using (IFormatParser sut = TestHelper.CreateFiddlerFormatParserFromString(FiddlerTraceFileTestData.ProcessSimpleAsmxRequestWrapped))
            {
                // Act
                ParsedMessage ans = sut.ReadNextMessage();

                // Assert
                Deserializer ds = new Deserializer();
                CallParameterInfo[] parameters = ds.DeserializeInputParameters(ans.Message, typeof(GeneratedContractsAsmx.TestAsmxServiceSoap).GetMethod("ProcessSimpleAsmxRequestWrapped"));
                Assert.AreEqual<int>(1, parameters.Length);
                Assert.IsNotNull(parameters[0].Value);
            }
        }

        /// <summary>
        /// Test that multiple different messages can be retrieved from a Fiddler trace.
        /// </summary>
        [TestMethod]
        public void FiddlerParserReadMultipleMessages()
        {
            // Arrange
            using (IFormatParser sut = TestHelper.CreateFiddlerFormatParserFromString(
                FiddlerTraceFileTestData.ProcessSimpleAsmxRequestWrapped,
                FiddlerTraceFileTestData.ProcessSimpleAsmxRequestBare,
                FiddlerTraceFileTestData.ProcessMultipleParametersWrapped))
            {
                // Act and assert
                TestHelper.AssertParsedMessageNoHeader(sut.ReadNextMessage(), "http://contoso.com/asmxservice/test/ProcessSimpleAsmxRequestWrapped", DateTime.MinValue);
                TestHelper.AssertParsedMessageNoHeader(sut.ReadNextMessage(), "http://contoso.com/asmxservice/test/ProcessSimpleAsmxRequestBare", DateTime.MinValue);
                TestHelper.AssertParsedMessageNoHeader(sut.ReadNextMessage(), "http://contoso.com/asmxservice/test/ProcessMultipleParametersWrapped", DateTime.MinValue);
            }
        }

        /// <summary>
        /// Test that a response containing the soap action pattern in the data does not get recognised.
        /// </summary>
        [TestMethod]
        public void FiddlerParserDoesNotDetectSoapActionWhenPatternPresentInResponseData()
        {
            // Arrange
            using (IFormatParser sut = TestHelper.CreateFiddlerFormatParserFromString(
                FiddlerTraceFileTestData.ProcessMultipleParametersWrappedWithSoapActionInResponse,
                FiddlerTraceFileTestData.ProcessSimpleAsmxRequestBare))
            {
                // Act and assert
                TestHelper.AssertParsedMessageNoHeader(sut.ReadNextMessage(), "http://contoso.com/asmxservice/test/ProcessMultipleParametersWrapped", DateTime.MinValue);
                TestHelper.AssertParsedMessageNoHeader(sut.ReadNextMessage(), "http://contoso.com/asmxservice/test/ProcessSimpleAsmxRequestBare", DateTime.MinValue);
            }
        }
    }
}
