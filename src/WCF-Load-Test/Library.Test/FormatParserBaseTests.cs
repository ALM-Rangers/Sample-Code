//---------------------------------------------------------------------
// <copyright file="FormatParserBaseTests.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
//     THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
//     OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
//     LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR
//     FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
// <summary>The FormatParserBaseTests type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.WcfUnit.Library.Test
{
    using System;
    using System.IO;
    using System.ServiceModel.Channels;
    using System.Xml;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Tests the format parser base class
    /// </summary>
    [TestClass]
    public class FormatParserBaseTests
    {
        /// <summary>
        /// Test that <see cref="FormatParserBase.GetMessageVersion"/> checks argument for null.
        /// </summary>
        [TestMethod]
        public void ParserBaseChecksArgumentForNullInGetMessageVersion()
        {
            MockFormatParser sut = new MockFormatParser();
            TestHelper.TestForArgumentNullException(() => sut.TestGetMessageVersion(null), "reader");
        }

        /// <summary>
        /// Base setup method sets up the main properties before calling the setup implementation method.
        /// </summary>
        [TestMethod]
        public void ParserBaseCallsSetupCoreTest()
        {
            // Arrange
            using (MockFormatParser sut = new MockFormatParser())
            {
                // Act
                using (MemoryStream ms = new MemoryStream())
                {
                    sut.Setup(ms, "dummy", false, false);

                    // Assert
                    Assert.AreEqual<int>(1, sut.SetupCoreCallCount);
                }
            }
        }

        /// <summary>
        /// Base setup method sets up the main properties before calling the setup implementation method.
        /// </summary>
        [TestMethod]
        public void ParserBaseCallsSetupCoreAfterSettingMainPropertiesTest()
        {
            // Arrange
            using (MockFormatParser sut = new MockFormatParser())
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    // Act
                    sut.Setup(ms, "dummy", true, true);

                    // Assert
                    Assert.AreSame(ms, sut.SetupTraceFile);
                    Assert.AreEqual<string>("dummy", sut.SetupFileName);
                    Assert.AreEqual<bool>(true, sut.SetupClientTrace);
                    Assert.AreEqual<bool>(true, sut.SetupServiceTrace);
                }
            }
        }

        /// <summary>
        /// Setup method is not called again and exception is thrown on second call to setup.
        /// </summary>
        [TestMethod]
        public void ParserBaseSecondCallToSetupThrowsExceptionAndDoesNotCallSetupCoreAgain()
        {
            // Arrange
            using (MockFormatParser sut = new MockFormatParser())
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    sut.Setup(ms, "dummy", false, false);

                    // Act
                    TestHelper.TestForInvalidOperationException(
                        delegate
                        {
                            sut.Setup(ms, "dummy", false, false);
                        });

                    // Assert
                    Assert.AreEqual<int>(1, sut.SetupCoreCallCount);
                }
            }
        }

        /// <summary>
        /// Test that SetupCore is called once when the parser is setup even if it throws an exception.
        /// </summary>
        [TestMethod]
        public void ParserBaseCallsSetupCoreOnlyOnceEvenIfCoreThrowsExceptionOnFirstCall()
        {
            // Arrange
            using (MockFormatParser sut = new MockFormatParser())
            {
                sut.SetupCoreToThrowException = true;
                try
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        sut.Setup(ms, "dummy", false, false);
                    }
                }
                catch (InvalidOperationException)
                {
                }

                sut.SetupCoreToThrowException = true;

                // Act
                TestHelper.TestForInvalidOperationException(
                    delegate
                    {
                        using (MemoryStream ms = new MemoryStream())
                        {
                            sut.Setup(ms, "dummy", false, false);
                        }
                    });

                // Assert
                Assert.AreEqual<int>(1, sut.SetupCoreCallCount);
            }
        }

        /// <summary>
        /// Test that cannot call ReadNextMessage until Setup has been called.
        /// </summary>
        [TestMethod]
        public void ParserBaseReadNextMessageCannotBeCalledUntilSetupHasBeenCalled()
        {
            // Arrange
            FormatParserBase sut = new MockFormatParser();

            // Act and assert
            TestHelper.TestForInvalidOperationException(
                delegate
                {
                    sut.ReadNextMessage();
                });
        }

        /// <summary>
        /// Tests that ReadNextMessage implementation is called.
        /// </summary>
        [TestMethod]
        public void ParserBaseReadNextMessageCoreCalled()
        {
            // Arrange
            using (MockFormatParser sut = new MockFormatParser())
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    sut.Setup(ms, "test", false, false);

                    // Act
                    sut.ReadNextMessage();

                    // Assert
                    Assert.AreEqual<int>(1, sut.ReadNextMessageCallCount);
                }
            }
        }

        private class MockFormatParser : FormatParserBase
        {
            public int SetupCoreCallCount = 0;
            public int ReadNextMessageCallCount = 0;
            public bool SetupCoreToThrowException = false;

            public Stream SetupTraceFile;
            public string SetupFileName;
            public bool SetupClientTrace;
            public bool SetupServiceTrace;

            internal MessageVersion TestGetMessageVersion(XmlReader reader)
            {
                return FormatParserBase.GetMessageVersion(reader);
            }

            protected override ParsedMessage ReadNextMessageCore()
            {
                this.ReadNextMessageCallCount++;

                return null;
            }

            protected override void SetupCore()
            {
                this.SetupCoreCallCount++;
                if (this.SetupCoreToThrowException)
                {
                    throw new InvalidOperationException();
                }

                this.SetupTraceFile = this.TraceFile;
                this.SetupFileName = this.FileName;
                this.SetupClientTrace = this.ClientTrace;
                this.SetupServiceTrace = this.ServiceTrace;
            }

            protected override void Dispose(bool disposing)
            {
            }
        }
    }
}
