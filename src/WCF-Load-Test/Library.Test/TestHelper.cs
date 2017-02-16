//---------------------------------------------------------------------
// <copyright file="TestHelper.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
//     THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
//     OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
//     LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR
//     FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
// <summary>The TestHelper type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.WcfUnit.Library.Test
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.ServiceModel.Channels;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Xml;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Provides general utility methods for testing.
    /// </summary>
    public static class TestHelper
    {
        /// <summary>
        /// The library assembly name.
        /// </summary>
        public const string LibraryAssembly = "Microsoft.WcfUnit.Library, Version=4.0.0.0, Culture=neutral, PublicKeyToken=33a5eb443961c4bf";

        /// <summary>
        /// Gets the last trace file opened.
        /// </summary>
        internal static Stream LastTraceFileOpened { get; private set; }

        /// <summary>
        /// Checks that a file contains a certain number of occurrences of a regular expression.
        /// </summary>
        /// <param name="fileName">The name of the file to check.</param>
        /// <param name="regex">The regular expression to be matched.</param>
        /// <param name="matchCount">The expected number of matches.</param>
        public static void CheckFileContains(string fileName, string regex, int matchCount)
        {
            Regex pattern = new Regex(regex, RegexOptions.Singleline);
            using (StreamReader sr = new StreamReader(fileName))
            {
                string contents = sr.ReadToEnd();
                MatchCollection matches = pattern.Matches(contents);
                Assert.AreEqual<int>(matchCount, matches.Count, string.Format(CultureInfo.InvariantCulture, "For \"{0}\" in {1}. Full output is\n{2}", regex, fileName, contents));
            }
        }

        /// <summary>
        /// Compares two pieces of generated code and highlights where they differ.
        /// </summary>
        /// <param name="expectedCode">The expected code.</param>
        /// <param name="actualCode">The actual code.</param>
        internal static void CompareCode(string expectedCode, string actualCode)
        {
            string strippedExpectedCode = StripHeaderComment(expectedCode);
            string strippedActualCode = StripHeaderComment(actualCode);

            StringBuilder actualCodeBuilder = new StringBuilder(strippedActualCode);
            if (strippedExpectedCode != strippedActualCode)
            {
                // Find the first difference and insert a marker
                int i;
                for (i = 0; i < Math.Min(strippedExpectedCode.Length, strippedActualCode.Length); i++)
                {
                    if (strippedExpectedCode[i] != strippedActualCode[i])
                    {
                        break;
                    }
                }

                actualCodeBuilder.Insert(i, "<HERE>");
            }

            Assert.AreEqual<string>(strippedExpectedCode, actualCodeBuilder.ToString());
        }

        /// <summary>
        /// Copies the complex sample to the temp folder.
        /// </summary>
        /// <returns>The location of the temp folder.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Want code to continue if it possibly can")]
        internal static string CopyComplexSampleToTempFolder()
        {
            // Exceptions are ignored as the assemblies get loaded and are then mapped so they can't
            // be deleted or overwritten if another test has already used them.
            string tempDir = Path.GetFullPath(@"..\..\..\..\UnitTestTempFiles");
            if (!Directory.Exists(tempDir))
            {
                Directory.CreateDirectory(tempDir);
            }

            foreach (string existingFile in Directory.GetFiles(tempDir))
            {
                try
                {
                    File.Delete(existingFile);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Warning: failed to delete {0}, reason {1}", existingFile, ex.Message);
                }
            }

            foreach (string filename in Directory.GetFiles(@"..\..\..\Library.Test\TestData\ComplexSample"))
            {
                string dest = tempDir + @"\" + Path.GetFileName(filename);
                try
                {
                    File.Copy(filename, dest, true);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Warning: failed to copy {0} to temp folder, reason {1}", filename, ex.Message);
                }
            }

            return tempDir;
        }

        /// <summary>
        /// Creates a parsed message.
        /// </summary>
        /// <param name="action">The soap action.</param>
        /// <param name="envelope">The envelope.</param>
        /// <returns>The resulting parsed message.</returns>
        internal static ParsedMessage CreateParsedMessage(string action, string envelope)
        {
            ParsedMessage ans = null;
            StringReader sr = null;
            try
            {
                sr = new StringReader(envelope);
                using (XmlReader reader = XmlReader.Create(sr))
                {
                    sr = null;
                    using (Message msg = Message.CreateMessage(reader, int.MaxValue, MessageVersion.Soap11))
                    {
                        MessageBuffer buffer = msg.CreateBufferedCopy(int.MaxValue);
                        ans = new ParsedMessage(buffer, action, DateTime.Now);
                    }
                }
            }
            finally
            {
                if (sr != null)
                {
                    sr.Dispose();
                }
            }

            return ans;
        }

        /// <summary>
        /// Creates a parsed message.
        /// </summary>
        /// <param name="action">The soap action.</param>
        /// <returns>The resulting parsed message.</returns>
        internal static ParsedMessage CreateParsedMessage(string action)
        {
            ParsedMessage parsedMessage = TestHelper.CreateParsedMessage(action, @"
<soap:Envelope xmlns:soap='http://schemas.xmlsoap.org/soap/envelope/' xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance' xmlns:xsd='http://www.w3.org/2001/XMLSchema'>
	<soap:Header>
		<Action soap:mustUnderstand='1' xmlns='http://schemas.microsoft.com/ws/2005/05/addressing/none'>http://tempuri.org/HelloWorldXmlSerializerRequest</Action>
	</soap:Header>
	<soap:Body xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance' xmlns:xsd='http://www.w3.org/2001/XMLSchema'>
		<SimpleXmlSerializerRequest xmlns='http://tempuri.org/'>
			<r S='wcfxml' I='1001'/>
		</SimpleXmlSerializerRequest>
	</soap:Body>
</soap:Envelope>
");
            return parsedMessage;
        }

        /// <summary>
        /// Creates a parsed message.
        /// </summary>
        /// <returns>The resulting parsed message.</returns>
        internal static ParsedMessage CreateParsedMessage()
        {
            return TestHelper.CreateParsedMessage("http://tempuri.org/HelloWorldXmlSerializerRequest");
        }

        /// <summary>
        /// Creates a stream from a string of data.
        /// </summary>
        /// <param name="data">The data to go into the stream.</param>
        /// <returns>The stream from which the data can be read back.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Stream needs to stay open.")]
        internal static Stream BuildFile(string data)
        {
            MemoryStream ans = new MemoryStream();
            StreamWriter sw = new StreamWriter(ans);
            sw.Write(data);
            sw.Flush();
            ans.Position = 0;

            return ans;
        }

        /// <summary>
        /// Creates a <see cref="Parser"/> for a WCF format log file.
        /// </summary>
        /// <param name="fileName">The name of the file from which to create the parser.</param>
        /// <param name="clientTrace">Indicates if the log is to be parsed for client side messages.</param>
        /// <param name="serviceTrace">Indicates if the log is to be parsed for service side messages.</param>
        /// <param name="soapActionMode">How to treat the soap actions.</param>
        /// <param name="soapActions">Any soap actions to be included or excluded.</param>
        /// <returns>The <see cref="Parser"/>.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Object is used after this call.")]
        internal static Parser CreateWcfParserFromFile(string fileName, bool clientTrace, bool serviceTrace, SoapActionMode soapActionMode, params string[] soapActions)
        {
            Parser ans;
            LastTraceFileOpened = Parser.OpenTraceFile(fileName);
            IFormatParser formatParser = new WcfParser();
            formatParser.Setup(LastTraceFileOpened, fileName, clientTrace, serviceTrace);

            ans = new Parser(formatParser, soapActionMode, soapActions);

            return ans;
        }

        /// <summary>
        /// Creates a <see cref="Parser"/> for a WCF format log file assembled from some strings.
        /// </summary>
        /// <param name="clientTrace">Indicates if the log is to be parsed for client side messages.</param>
        /// <param name="serviceTrace">Indicates if the log is to be parsed for service side messages.</param>
        /// <param name="segments">The segments of log file to be concatenated into a single message log.</param>
        /// <returns>The <see cref="Parser"/>.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Object is used by caller")]
        internal static IFormatParser CreateWcfFormatParserFromString(bool clientTrace, bool serviceTrace, params string[] segments)
        {
            IFormatParser formatParser = new WcfParser();
            formatParser.Setup(BuildFile(string.Concat(segments)), "in memory", clientTrace, serviceTrace);
            return formatParser;
        }

        /// <summary>
        /// Creates a <see cref="Parser"/> for a WCF format client-side log file assembled from some strings.
        /// </summary>
        /// <param name="segments">The segments of log file to be concatenated into a single message log.</param>
        /// <returns>The <see cref="Parser"/>.</returns>
        internal static IFormatParser CreateWcfFormatParserClientTraceFromString(params string[] segments)
        {
            return CreateWcfFormatParserFromString(true, false, segments);
        }

        /// <summary>
        /// Creates a <see cref="Parser"/> for a WCF format service-side log file assembled from some strings.
        /// </summary>
        /// <param name="segments">The segments of log file to be concatenated into a single message log.</param>
        /// <returns>The <see cref="Parser"/>.</returns>
        internal static IFormatParser CreateWcfFormatParserServiceTraceFromString(params string[] segments)
        {
            return CreateWcfFormatParserFromString(false, true, segments);
        }

        /// <summary>
        /// Creates a <see cref="Parser"/> for a Fiddler format log file assembled from some strings.
        /// </summary>
        /// <param name="segments">The segments of log file to be concatenated into a single message log.</param>
        /// <returns>The <see cref="Parser"/>.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Object is used by caller")]
        internal static IFormatParser CreateFiddlerFormatParserFromString(params string[] segments)
        {
            IFormatParser formatParser = new FiddlerTextParser();
            formatParser.Setup(BuildFile(string.Concat(segments)), "in memory", false, false);
            return formatParser;
        }

        /// <summary>
        /// Tests that a <see cref="ParsedMessage"/> matches the expected SOAP action and timestamp, header is expected to contain SOAP action.
        /// </summary>
        /// <param name="ans">The <see cref="ParsedMessage"/> to be checked.</param>
        /// <param name="expectedSoapAction">The expected SOAP action.</param>
        /// <param name="expectedTimestamp">The expected timestamp.</param>
        internal static void AssertParsedMessage(ParsedMessage ans, string expectedSoapAction, DateTime expectedTimestamp)
        {
            Assert.IsNotNull(ans);
            Assert.AreEqual<string>(expectedSoapAction, ans.SoapAction);
            Assert.AreEqual<DateTime>(expectedTimestamp, ans.Timestamp.ToUniversalTime());
            Message m = ans.Message.CreateMessage();
            Assert.AreEqual<string>(expectedSoapAction, m.Headers.Action);
        }

        /// <summary>
        /// Tests that a <see cref="ParsedMessage"/> matches the expected SOAP action and timestamp, header not expected to contain SOAP action.
        /// </summary>
        /// <param name="ans">The <see cref="ParsedMessage"/> to be checked.</param>
        /// <param name="expectedSoapAction">The expected SOAP action.</param>
        /// <param name="expectedTimestamp">The expected timestamp.</param>
        internal static void AssertParsedMessageNoHeader(ParsedMessage ans, string expectedSoapAction, DateTime expectedTimestamp)
        {
            Assert.IsNotNull(ans);
            Assert.AreEqual<string>(expectedSoapAction, ans.SoapAction);
            Assert.AreEqual<DateTime>(expectedTimestamp, ans.Timestamp.ToUniversalTime());
        }

        /// <summary>
        /// Creates the configuration type for the <see cref="WcfParser"/>.
        /// </summary>
        /// <returns>The configuration type for the <see cref="WcfParser"/></returns>
        internal static typeType CreateConfigTypeForWcfFormatParser()
        {
            typeType wcfFormatParser = new typeType();
            wcfFormatParser.assembly = TestHelper.LibraryAssembly;
            wcfFormatParser.type = "Microsoft.WcfUnit.Library.WcfParser";
            return wcfFormatParser;
        }

        /// <summary>
        /// Creates the configuration type for the <see cref="FiddlerTextParser"/>.
        /// </summary>
        /// <returns>The configuration type for the <see cref="FiddlerTextParser"/></returns>
        internal static typeType CreateConfigTypeForFiddlerTextFormatParser()
        {
            typeType wcfFormatParser = new typeType();
            wcfFormatParser.assembly = TestHelper.LibraryAssembly;
            wcfFormatParser.type = "Microsoft.WcfUnit.Library.FiddlerTextParser";
            return wcfFormatParser;
        }

        /// <summary>
        /// Tests an operation to see if it will throw an <see cref="UserException"/>.
        /// </summary>
        /// <param name="methodUnderTest">The operation to test.</param>
        /// <param name="expectedMessage">The expected exception message.</param>
        internal static void TestForUserException(Action methodUnderTest, string expectedMessage)
        {
            try
            {
                methodUnderTest();
                Assert.Fail("Expected UserException");
            }
            catch (UserException ue)
            {
                if (expectedMessage != null)
                {
                    Assert.AreEqual(expectedMessage, ue.Message, "The correct exception was thrown but for the wrong reason");
                }
            }
        }

        /// <summary>
        /// Tests an operation to see if it will throw an <see cref="UserException"/>.
        /// </summary>
        /// <param name="methodUnderTest">The operation to test.</param>
        internal static void TestForUserException(Action methodUnderTest)
        {
            TestForUserException(methodUnderTest, null);
        }

        /// <summary>
        /// Tests an operation to see if it will throw a <see cref="NotSupportedException"/>.
        /// </summary>
        /// <param name="methodUnderTest">The operation to test.</param>
        internal static void TestForNotSupportedException(Action methodUnderTest)
        {
            try
            {
                methodUnderTest();
                Assert.Fail("Expected NotSupportedException");
            }
            catch (NotSupportedException)
            {
            }
        }

        /// <summary>
        /// Tests an operation to see if it will throw an <see cref="InvalidOperationException"/>.
        /// </summary>
        /// <param name="methodUnderTest">The operation to test.</param>
        /// <param name="expectedMessage">The expected exception message.</param>
        internal static void TestForInvalidOperationException(Action methodUnderTest, string expectedMessage)
        {
            try
            {
                methodUnderTest();
                Assert.Fail("Expected InvalidOperationException");
            }
            catch (InvalidOperationException ioe)
            {
                if (expectedMessage != null)
                {
                    Assert.AreEqual(expectedMessage, ioe.Message, "The correct exception was thrown but for the wrong reason");
                }
            }
        }

        /// <summary>
        /// Tests an operation to see if it will throw an <see cref="InvalidOperationException"/>.
        /// </summary>
        /// <param name="methodUnderTest">The operation to test.</param>
        internal static void TestForInvalidOperationException(Action methodUnderTest)
        {
            TestForInvalidOperationException(methodUnderTest, null);
        }

        /// <summary>
        /// Tests an operation to see if it will throw a <see cref="ObjectDisposedException"/>.
        /// </summary>
        /// <param name="methodUnderTest">The operation to test.</param>
        internal static void TestForObjectDisposedException(Action methodUnderTest)
        {
            try
            {
                methodUnderTest();
                Assert.Fail("Expected ObjectDisposedException");
            }
            catch (ObjectDisposedException)
            {
            }
        }

        /// <summary>
        /// Tests an operation to see if it will throw an <see cref="ArgumentNullException"/>.
        /// </summary>
        /// <param name="methodUnderTest">The operation to test.</param>
        /// <param name="expectedParameterName">The expected parameter name in the exception message.</param>
        internal static void TestForArgumentNullException(Action methodUnderTest, string expectedParameterName)
        {
            try
            {
                methodUnderTest();
                Assert.Fail("Expected ArgumentNullException");
            }
            catch (ArgumentNullException ae)
            {
                Assert.IsNotNull(ae.Message, "ArgumentNullException should contain a message");
                Assert.IsTrue(ae.Message.Contains(expectedParameterName), "expected parameter name not found");
            }
        }

        /// <summary>
        /// Removes the header comment from generated code.
        /// </summary>
        /// <param name="code">The code from which the header is to be removed.</param>
        /// <returns>The code without the header comment.</returns>
        private static string StripHeaderComment(string code)
        {
            string ans = code;
            int index;
            index = code.IndexOf("// </auto-generated>" + Environment.NewLine, StringComparison.OrdinalIgnoreCase);
            if (index >= 0)
            {
                index = code.IndexOf(Environment.NewLine, index, StringComparison.OrdinalIgnoreCase);
            }

            if (index >= 0)
            {
                ans = code.Substring(index);
            }

            return ans;
        }
    }
}
