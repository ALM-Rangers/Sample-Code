//---------------------------------------------------------------------
// <copyright file="ParserTests.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
//     THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
//     OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
//     LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR
//     FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
// <summary>The ParserTests type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.WcfUnit.Library.Test
{
    using System.IO;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Rhino.Mocks;

    /// <summary>
    /// Tests the <see cref="Parser"/> class.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Disposable objects disposed in cleanup method")]
    [TestClass]
    public class ParserTests
    {
        private Parser sut;
        private IFormatParser mockFormatParser;

        /// <summary>
        /// The mock repository used to set up and then verify expected behaviour.
        /// </summary>
        private MockRepository mocks;

        public ParserTests()
        {
        }

        [TestInitialize]
        public void SetupTest()
        {
            this.mocks = new MockRepository();
        }

        /// <summary>
        /// Cleans up after each test and verifies behaviour in the mocks.
        /// </summary>
        [TestCleanup]
        public void CleanupTest()
        {
            if (TestHelper.LastTraceFileOpened != null)
            {
                TestHelper.LastTraceFileOpened.Close();
            }

            if (this.sut != null)
            {
                this.sut.Dispose();
            }

            this.mocks.VerifyAll();
        }

        /// <summary>
        /// Test that can open a trace file for read only.
        /// </summary>
        [TestMethod]
        public void ParserOpenTraceFileForRead()
        {
            using (Stream s = Parser.OpenTraceFile("FullSampleTest.svclog"))
            {
                Assert.IsNotNull(s);
                Assert.IsTrue(s.CanRead);
                Assert.IsFalse(s.CanWrite);
            }
        }

        /// <summary>
        /// Test that an attempt to open a non-existent trace file throws an application exception.
        /// </summary>
        [TestMethod]
        public void ParserOpenTraceFileNonexistentFile()
        {
            TestHelper.TestForUserException(
                delegate
                {
                    using (Parser.OpenTraceFile("NoSuchFile.svclog"))
                    {
                    }
                });
        }

        /// <summary>
        /// Test that parser is disposable
        /// </summary>
        [TestMethod]
        public void ParserIsDisposable()
        {
            // Arrange
            Stream s = null;
            try
            {
                s = Parser.OpenTraceFile("WithMessageBodies.svclog");

                // Act and assert
                typeType wcfFormatParser = TestHelper.CreateConfigTypeForWcfFormatParser();
                using (Parser p = Parser.CreateParser(wcfFormatParser, s, "test", true, false, SoapActionMode.Include))
                {
                    s = null;
                }
            }
            finally
            {
                if (s != null)
                {
                    s.Dispose();
                }
            }
        }

        /// <summary>
        /// Create parser creates and sets up the format specific parser.
        /// </summary>
        [TestMethod]
        public void ParserCreateParserCreatesAndSetsUpTheFormatSpecificParser()
        {
            // Arrange
            Stream s = null;
            try
            {
                s = Parser.OpenTraceFile("WithMessageBodies.svclog");

                // Act
                typeType wcfFormatParser = TestHelper.CreateConfigTypeForWcfFormatParser();
                using (Parser p = Parser.CreateParser(wcfFormatParser, s, "test", true, false, SoapActionMode.Include))
                {
                    s = null;

                    // Assert
                    Assert.IsNotNull(p); // checks it is created
                    // Check it is setup
                    p.ReadNextRequest();
                }
            }
            finally
            {
                if (s != null)
                {
                    s.Dispose();
                }
            }
        }

        /// <summary>
        /// Test that Parser calls format specific parser until there are no more messages.
        /// </summary>
        [TestMethod]
        public void ParserCallsFormatSpecificParserUntilThereAreNoMoreMessages()
        {
            // Arrange
            this.SetupMockFormatParser("a", "b", "c");
            this.SetupSut(SoapActionMode.Include);
            int count = 0;

            // Act
            while (this.sut.ReadNextRequest() != null)
            {
                count++;
            }

            // Assert
            Assert.AreEqual<int>(3, count);
        }

        /// <summary>
        /// Test that Parser returns the messages actually returned by the format-specific parser.
        /// </summary>
        [TestMethod]
        public void ParserReturnsMessagesReturnedByTheFormatSpecificParser()
        {
            // Arrange
            ParsedMessage[] msgs = this.SetupMockFormatParser("a", "b", "c");
            this.SetupSut(SoapActionMode.Include);

            // Act and assert
            for (int i = 0; i < 3; i++)
            {
                Assert.AreSame(msgs[i], this.sut.ReadNextRequest());
            }

            this.sut.ReadNextRequest(); // last read to get the null return.
        }

        /// <summary>
        /// Test that the parser will exclude a single message.
        /// </summary>
        [TestMethod]
        public void ParserExcludeSingleReturnsCorrectNumberOfMessages()
        {
            // Arrange
            this.SetupMockFormatParser("a", "b", "c");
            this.SetupSut(SoapActionMode.Exclude, "b");

            int count = 0;

            // Act
            while (this.sut.ReadNextRequest() != null)
            {
                count++;
            }

            // Assert
            Assert.AreEqual<int>(2, count);
        }

        /// <summary>
        /// Test that the parser will not return the excluded message.
        /// </summary>
        [TestMethod]
        public void ParserExcludeSingleReturnsCorrectActualMessages()
        {
            // Arrange
            ParsedMessage[] msgs = this.SetupMockFormatParser("a", "b", "c");
            this.SetupSut(SoapActionMode.Exclude, "b");

            // Act and assert
            Assert.AreSame(msgs[0], this.sut.ReadNextRequest());
            Assert.AreSame(msgs[2], this.sut.ReadNextRequest());

            this.sut.ReadNextRequest(); // last read to get the null return.
        }

        /// <summary>
        /// Test that excluding all soap actions returns nothing.
        /// </summary>
        [TestMethod]
        public void ParserExcludeAll()
        {
            // Arrange
            this.SetupMockFormatParser("a", "b", "c");
            this.SetupSut(SoapActionMode.Exclude, "a", "b", "c");

            int count = 0;

            // Act
            while (this.sut.ReadNextRequest() != null)
            {
                count++;
            }

            Assert.AreEqual<int>(0, count);
        }

        /// <summary>
        /// Test that excluding no soap actions returns all messages.
        /// </summary>
        [TestMethod]
        public void ParserExcludeNothingReturnsAllMessages()
        {
            // Arrange
            this.SetupMockFormatParser("a", "b", "c");
            this.SetupSut(SoapActionMode.Exclude);

            int count = 0;

            // Act
            while (this.sut.ReadNextRequest() != null)
            {
                count++;
            }

            Assert.AreEqual<int>(3, count);
        }

        /// <summary>
        /// Test that excluding no soap actions returns all actual messages.
        /// </summary>
        [TestMethod]
        public void ParserExcludeNothingReturnsAllActualMessages()
        {
            // Arrange
            ParsedMessage[] msgs = this.SetupMockFormatParser("a", "b", "c");
            this.SetupSut(SoapActionMode.Exclude);

            // Act and assert
            for (int i = 0; i < 3; i++)
            {
                Assert.AreSame(msgs[i], this.sut.ReadNextRequest());
            }

            this.sut.ReadNextRequest(); // last read to get the null return.
        }

        /// <summary>
        /// Test that Parser returns all messages if the include actions list is null
        /// </summary>
        [TestMethod]
        public void ParserReturnsAllMessagesWithNullIncludeList()
        {
            // Arrange
            this.SetupMockFormatParser("a", "b", "c");
            this.SetupSut(SoapActionMode.Include, null);
            int count = 0;

            // Act
            while (this.sut.ReadNextRequest() != null)
            { 
                count++;
            }

            // Assert
            Assert.AreEqual<int>(3, count);
        }

        private ParsedMessage[] SetupMockFormatParser(params string[] actions)
        {
            ParsedMessage[] ans = new ParsedMessage[actions.Length];

            this.mockFormatParser = this.mocks.StrictMock<IFormatParser>();
            for (int i = 0; i < actions.Length; i++)
            {
                ans[i] = TestHelper.CreateParsedMessage(actions[i]);
                Expect.Call(this.mockFormatParser.ReadNextMessage()).Return(ans[i]);
            }

            Expect.Call(this.mockFormatParser.ReadNextMessage()).Return(null);
            Expect.Call(() => this.mockFormatParser.Dispose());
            this.mocks.ReplayAll();

            return ans;
        }

        private void SetupSut(SoapActionMode soapActionMode, params string[] soapActions)
        {
            this.sut = new Parser(this.mockFormatParser, soapActionMode, soapActions);
        }
    }
}
