//---------------------------------------------------------------------
// <copyright file="UtilitiesTests.cs" company="Microsoft">
//    Copyright (c) Microsoft. All rights reserved.
//    This code is licensed under the MIT License.
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF 
//    ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
//    TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
//    PARTICULAR PURPOSE AND NONINFRINGEMENT.
// </copyright>
// <summary>The UtilitiesTests type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.Word4Tfs.Library.Test.UnitTest
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Tests the <see cref="Utilities"/> class.
    /// </summary>
    [TestClass]
    public class UtilitiesTests
    {
        /// <summary>
        /// Tests <see cref="Utilities.IsTagValid"/> validates tags correctly.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void IsValidTagValidatesTagsCorrectly()
        {
            Assert.IsTrue(Utilities.IsValidTag("a.b"));
            Assert.IsTrue(Utilities.IsValidTag("a.b.c"));
            Assert.IsTrue(Utilities.IsValidTag("a1.b1"));

            Assert.IsFalse(Utilities.IsValidTag("a"));
            Assert.IsFalse(Utilities.IsValidTag("a b"));
            Assert.IsFalse(Utilities.IsValidTag("[a]"));
            Assert.IsFalse(Utilities.IsValidTag("a["));
            Assert.IsFalse(Utilities.IsValidTag("("));
            Assert.IsFalse(Utilities.IsValidTag("a("));
            Assert.IsFalse(Utilities.IsValidTag(@"\a"));
            Assert.IsFalse(Utilities.IsValidTag(@"a\"));
        }

        /// <summary>
        /// Tests <see cref="Utilities.QuerySpecificBookmarkNamingFunction"/> generates correct bookmark name.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void QuerySpecificBookmarkNamingFunctionCreatesCorrectBookmarkName()
        {
            Assert.AreEqual<string>("TFS_WI_Q1_W2", Utilities.QuerySpecificBookmarkNamingFunction(1)(2));
        }

        /// <summary>
        /// Tests <see cref="Utilities.BookmarkNamingFunction"/> generates correct bookmark name.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void BookmarkNamingFunctionCreatesCorrectBookmarkName()
        {
            Assert.AreEqual<string>("TFS_WI_Q1_W2", Utilities.BookmarkNamingFunction(1, 2));
        }

        /// <summary>
        /// Tests <see cref="Utilities.BookmarkParsingFunction"/> extracts query index and work item id from a valid bookmark name.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void BookmarkParsingFunctionExtractsQueryIndexAndWorkItemIdFromValidBookmarkName()
        {
            Tuple<int, int> ans = Utilities.BookmarkParsingFunction("TFS_WI_Q1_W2");
            Assert.IsNotNull(ans, "The bookmark name supplied was valid and should have been parsed.");
            Assert.AreEqual<int>(1, ans.Item1, "Query index not properly parsed");
            Assert.AreEqual<int>(2, ans.Item2, "Work item id not properly parsed");
        }

        /// <summary>
        /// Tests <see cref="Utilities.BookmarkParsingFunction"/> returns null if query index is invalid.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void BookmarkParsingFunctionReturnsNullIfQueryIndexIsInvalid()
        {
            Tuple<int, int> ans = Utilities.BookmarkParsingFunction("TFS_WI_QA_W2");
            Assert.IsNull(ans, "The bookmark name supplied was invalid and should not have been parsed.");
        }

        /// <summary>
        /// Tests <see cref="Utilities.BookmarkParsingFunction"/> returns null if work item id is invalid.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void BookmarkParsingFunctionReturnsNullIfWorkItemIdIsInvalid()
        {
            Tuple<int, int> ans = Utilities.BookmarkParsingFunction("TFS_WI_Q1_WA");
            Assert.IsNull(ans, "The bookmark name supplied was invalid and should not have been parsed.");
        }

        /// <summary>
        /// Tests <see cref="Utilities.BookmarkParsingFunction"/> returns null if bookmark name does not have correct prefixes.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void BookmarkParsingFunctionReturnsNullIfBookmarkNameIsInvalid()
        {
            Tuple<int, int> ans = Utilities.BookmarkParsingFunction("WWW_XX_Y1_Z2");
            Assert.IsNull(ans, "The bookmark name supplied was invalid and should not have been parsed.");
        }

        /// <summary>
        /// Tests <see cref="Utilities.XpathParsingFunction"/> extracts work item id from a valid work item xpath mapping.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void XpathParsingFunctionExtractsWorkItemIdFromValidXpathMapping()
        {
            Nullable<int> ans = Utilities.XpathParsingFunction(@"/wi:WorkItems/wi:WorkItem[wi:Field[@name='System.Id']=1]/wi:Field[@name='System.Title']");
            Assert.IsTrue(ans.HasValue, "The xpath supplied was valid and should have been parsed.");
            Assert.AreEqual<int>(1, ans.Value, "Work item id not properly parsed");
        }

        /// <summary>
        /// Tests <see cref="Utilities.XpathParsingFunction"/> returns null if work item id is invalid.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void XpathParsingFunctionReturnsNullIfWorkItemIdIsInvalid()
        {
            Nullable<int> ans = Utilities.XpathParsingFunction(@"/wi:WorkItems/wi:WorkItem[wi:Field[@name='System.Id']=A]/wi:Field[@name='System.Title']");
            Assert.IsFalse(ans.HasValue, "The xpath supplied was invalid and should not have been parsed.");
        }

        /// <summary>
        /// Tests <see cref="Utilities.XpathParsingFunction"/> returns null if xpath is invalid.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void XpathParsingFunctionReturnsNullIfXPathIsInvalid()
        {
            Nullable<int> ans = Utilities.XpathParsingFunction("abcdef");
            Assert.IsFalse(ans.HasValue, "The xpath supplied was invalid and should not have been parsed.");
        }

        /// <summary>
        /// Tests <see cref="Utilities.FormatException"/> throws argument null exception if a null array is passed.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void FormatExceptionThrowsArgumentNullExceptionIfNullArrayPassed()
        {
            // Arrange
            Exception[] arr = null;

            // Act and assert
            TestHelper.TestForArgumentNullException(() => Utilities.FormatException(arr), "exceptions");
        }

        /// <summary>
        /// Tests <see cref="Utilities.FormatException"/> formats a single exception with no inner exception.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void FormatExceptionFormatsASingleExceptionWithNoInnerException()
        {
            // Arrange
            Exception ex = new Exception("test");

            // Act
            string ans = Utilities.FormatException(ex);

            // Assert
            Assert.AreEqual("test\r\n", ans);
        }

        /// <summary>
        /// Tests <see cref="Utilities.FormatException"/> formats a single exception with an inner exception.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void FormatExceptionFormatsASingleExceptionWithInnerException()
        {
            // Arrange
            Exception inner = new Exception("inner");
            Exception outer = new Exception("outer", inner);

            // Act
            string ans = Utilities.FormatException(outer);

            // Assert
            Assert.AreEqual("outer\r\n\r\nInner Exception: inner\r\n", ans);
        }

        /// <summary>
        /// Tests <see cref="Utilities.FormatException"/> formats multiple exceptions with no inner exception.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void FormatExceptionFormatsMultipleExceptionsWithNoInnerException()
        {
            // Arrange
            Exception ex1 = new Exception("test1");
            Exception ex2 = new Exception("test2");

            // Act
            string ans = Utilities.FormatException(ex1, ex2);

            // Assert
            Assert.AreEqual("1: test1\r\n\r\n2: test2\r\n", ans);
        }

        /// <summary>
        /// Tests <see cref="Utilities.FormatException"/> formats multiple exceptions with an inner exception.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void FormatExceptionFormatsMultipleExceptionsWithInnerException()
        {
            // Arrange
            Exception inner1 = new Exception("inner1");
            Exception outer1 = new Exception("outer1", inner1);
            Exception inner2 = new Exception("inner2");
            Exception outer2 = new Exception("outer2", inner2);

            // Act
            string ans = Utilities.FormatException(outer1, outer2);

            // Assert
            Assert.AreEqual("1: outer1\r\n\r\nInner Exception: inner1\r\n\r\n2: outer2\r\n\r\nInner Exception: inner2\r\n", ans);
        }

        /// <summary>
        /// Tests <see cref="Utilities.FormatTestCase"/> formats a TFS 2012 or 2013 test case.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void FormaTestCaseFormatsATFS2012Or2013TestCase()
        {
            // Arrange
            string testCase = @"<steps id=""0"" last=""3"">
  <step type=""ValidateStep"" id=""2"">
    <parameterizedString isformatted=""true"">&lt;DIV&gt;&lt;DIV&gt;&lt;P&gt;This is&amp;nbsp;step 1&amp;nbsp;&lt;/P&gt;&lt;/DIV&gt;&lt;/DIV&gt;</parameterizedString>
    <parameterizedString isformatted=""true"">&lt;DIV&gt;&lt;P&gt;Expect result&amp;nbsp;1&amp;nbsp;here&amp;nbsp;&lt;/P&gt;&lt;/DIV&gt;</parameterizedString>
    <description />
  </step>
  <step type=""ValidateStep"" id=""3"">
    <parameterizedString isformatted=""true"">&lt;DIV&gt;&lt;P&gt;This is step 2&amp;nbsp;&lt;/P&gt;&lt;/DIV&gt;</parameterizedString>
    <parameterizedString isformatted=""true"">&lt;P&gt;Expect result 2 here&amp;nbsp;&lt;/P&gt;</parameterizedString>
    <description />
  </step>
</steps>
";

            // Act
            string ans = Utilities.FormatTestCase(testCase);

            // Assert
            string expected = "<html><body>1 <DIV><DIV><P>This is&nbsp;step 1&nbsp;</P></DIV></DIV> <DIV><P>Expect result&nbsp;1&nbsp;here&nbsp;</P></DIV>2 <DIV><P>This is step 2&nbsp;</P></DIV> <P>Expect result 2 here&nbsp;</P></body></html>";
            Assert.AreEqual(expected, ans);
        }
    }
}
