//---------------------------------------------------------------------
// <copyright file="MockWordDocument.cs" company="Microsoft">
//    Copyright (c) Microsoft. All rights reserved.
//    This code is licensed under the MIT License.
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF 
//    ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
//    TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
//    PARTICULAR PURPOSE AND NONINFRINGEMENT.
// </copyright>
// <summary>The MockWordDocument type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.Word4Tfs.Library.Test.UnitTest.Model
{
    using System;
    using global::System.Collections.Generic;
    using global::System.Globalization;
    using global::System.Linq;
    using Microsoft.Office.Interop.Word;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Word4Tfs.Library.Model.Word;
    using Moq;

    /// <summary>
    /// Mock word document that also records the order in which bookmarks appear in the document.
    /// </summary>
    internal class MockWordDocument : Mock<IWordDocument>
    {
        // TODO: Extract this mock class to make it used in other tests like testing layout definition formatter and anywhere a mock word document is used.

        /// <summary>
        /// The mock bookmarks.
        /// </summary>
        private IList<Mock<Bookmark>> mockBookmarks = new List<Mock<Bookmark>>();

        /// <summary>
        /// Records the logical content of the Word document.
        /// </summary>
        private IList<object> logicalContentList = new List<object>();

        /// <summary>
        ///  The position in the logical content list of the next item to verify against.
        /// </summary>
        private int logicalContentListVerificationIndex = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="MockWordDocument"/> class.
        /// </summary>
        public MockWordDocument()
        {
            this.Setup(doc => doc.DeleteBookmarkAndContent(It.IsAny<string>())).Callback((string bookmark) => this.DeleteBookmarkAndContentCallback(bookmark));
            this.Setup(doc => doc.MoveBookmarkAndContentToBefore(It.IsAny<string>(), It.IsAny<string>())).Callback((string bookmark, string relative) => this.MoveBookmarkAndContentToBefore(bookmark, relative));
            this.Setup(doc => doc.BookmarkManager).Returns(() => TestHelper.CreateMockBookmarkManager(this.BuildBookmarks().ToArray())); // Func so that list is generated at the time the property is requested, not when the mock is set up.
            this.Setup(document => document.InsertParagraph(It.IsAny<string>(), It.IsAny<string>()))
                .Callback((string content, string style) => this.InsertParagraphCallback(content, style));
        }

        /// <summary>
        /// Gets the list of bookmark names, in the order in which they appear in the document.
        /// </summary>
        public IList<string> BookmarkNames
        {
            get
            {
                return this.mockBookmarks.Select(b => b.Object.Name).ToList();
            }
        }

        // TODO: Extend the logical content to all parts of the word document and verify all tests in this manner.

        /// <summary>
        /// This method must be used as a callback when setting up a call to <see cref="IWordDocument.InsertParagraph"/>, so that logical content can be recorded.
        /// </summary>
        /// <param name="content">The content being inserted.</param>
        /// <param name="style">The style applied to the content.</param>
        public void InsertParagraphCallback(string content, string style)
        {
            this.logicalContentList.Add(new LogicalParagraph(content, style));
        }

        /// <summary>
        /// This method must be used as a callback when setting up a call to <see cref="IWordDocument.DeleteBookmarkAndContent"/>, so that bookmark order can be recorded.
        /// </summary>
        /// <param name="bookmark">The bookmark being removed.</param>
        public void DeleteBookmarkAndContentCallback(string bookmark)
        {
            Console.WriteLine("Removing {0} from bookmark collection", bookmark);
            this.DeleteBookmark(bookmark);
        }

        /// <summary>
        /// This method must be used as a callback when setting up a call to <see cref="IWordDocument.InsertBuildingBlock"/>, so that bookmark order can be recorded.
        /// </summary>
        /// <param name="buildingBlock">The building block being inserted.</param>
        /// <param name="bookmark">The bookmark being inserted.</param>
        public void InsertBuildingBlockCallback(BuildingBlock buildingBlock, string bookmark)
        {
            Console.WriteLine("Adding {0} to bookmark collection for building block {1}", bookmark, buildingBlock.Name);
            this.mockBookmarks.Add(this.CreateMockBookmark(bookmark));
            this.logicalContentList.Add(new LogicalBookmark(bookmark, true));
            this.logicalContentList.Add(new LogicalBuildingBlock(buildingBlock));
            this.logicalContentList.Add(new LogicalBookmark(bookmark, false));
        }

        /// <summary>
        /// This method must be used as a callback when setting up a call to <see cref="IWordDocument.InsertBuildingBlockBeforeBookmark"/>, so that bookmark order can be recorded.
        /// </summary>
        /// <param name="bookmark">The bookmark being inserted.</param>
        /// <param name="nameOfRelativeBookmark">The name of the bookmark to insert before.</param>
        public void InsertBuildingBlockBeforeBookmarkCallback(string bookmark, string nameOfRelativeBookmark)
        {
            Console.WriteLine("Adding {0} to bookmark collection before {1}", bookmark, nameOfRelativeBookmark);
            int index = this.FindBookmarkIndex(nameOfRelativeBookmark);
            this.mockBookmarks.Insert(index, this.CreateMockBookmark(bookmark));
        }

        /// <summary>
        /// This method must be used as a callback when setting up a call to <see cref="IWordDocument.InsertBuildingBlockAfterBookmark"/>, so that bookmark order can be recorded.
        /// </summary>
        /// <param name="bookmark">The bookmark being inserted.</param>
        /// <param name="nameOfRelativeBookmark">The name of the bookmark to insert after.</param>
        public void InsertBuildingBlockAfterBookmarkCallback(string bookmark, string nameOfRelativeBookmark)
        {
            Console.WriteLine("Adding {0} to bookmark collection after {1}", bookmark, nameOfRelativeBookmark);
            int index = this.FindBookmarkIndex(nameOfRelativeBookmark);
            this.mockBookmarks.Insert(index + 1, this.CreateMockBookmark(bookmark));
        }

        /// <summary>
        /// Moves a bookmark and its content to the location before the relative bookmark.
        /// </summary>
        /// <param name="nameOfBookmarkToMove">The name of the bookmark that is to be moved.</param>
        /// <param name="nameOfRelativeBookmark">The name of the bookmark relative to which the bookmark is to be moved.</param>
        public void MoveBookmarkAndContentToBefore(string nameOfBookmarkToMove, string nameOfRelativeBookmark)
        {
            Assert.AreNotEqual<string>(nameOfBookmarkToMove, nameOfRelativeBookmark, "Must not move bookmark relative to itself.");
            Console.WriteLine("Moving {0} to before {1}", nameOfBookmarkToMove, nameOfRelativeBookmark);
            this.DeleteBookmark(nameOfBookmarkToMove);
            int relIndex = this.FindBookmarkIndex(nameOfRelativeBookmark);
            this.mockBookmarks.Insert(relIndex, this.CreateMockBookmark(nameOfBookmarkToMove));
        }

        /// <summary>
        /// Moves a bookmark and its content to the location after the relative bookmark.
        /// </summary>
        /// <param name="nameOfBookmarkToMove">The name of the bookmark that is to be moved.</param>
        /// <param name="nameOfRelativeBookmark">The name of the bookmark relative to which the bookmark is to be moved.</param>
        public void MoveBookmarkAndContentToAfter(string nameOfBookmarkToMove, string nameOfRelativeBookmark)
        {
            Assert.AreNotEqual<string>(nameOfBookmarkToMove, nameOfRelativeBookmark, "Must not move bookmark relative to itself.");
            Console.WriteLine("Moving {0} to after {1}", nameOfBookmarkToMove, nameOfRelativeBookmark);
            this.DeleteBookmark(nameOfBookmarkToMove);
            int index = this.FindBookmarkIndex(nameOfRelativeBookmark);
            this.mockBookmarks.Insert(index + 1, this.CreateMockBookmark(nameOfBookmarkToMove));
        }

        /// <summary>
        /// Asserts that a paragraph was inserted in the call order.
        /// </summary>
        /// <param name="expectedContent">The expected content.</param>
        /// <param name="expectedStyle">The expected style.</param>
        public void AssertParagraphInserted(string expectedContent, string expectedStyle)
        {
            LogicalParagraph p = this.GetNextVerificationLogicalContent<LogicalParagraph>();
            Assert.AreEqual<string>(expectedContent, p.Content);
            Assert.AreEqual<string>(expectedStyle, p.Style);
        }

        /// <summary>
        /// Asserts that a bookmark was inserted in the call order.
        /// </summary>
        /// <param name="expectedName">The expected bookmark name.</param>
        /// <param name="expectedIsStart">The expected value indicating whether this is the start of end of the bookmark.</param>
        public void AssertBookmarkInserted(string expectedName, bool expectedIsStart)
        {
            LogicalBookmark b = this.GetNextVerificationLogicalContent<LogicalBookmark>();
            Assert.AreEqual<string>(expectedName, b.Name);
            Assert.AreEqual<bool>(expectedIsStart, b.IsStart);
        }

        /// <summary>
        /// Asserts that a building block was inserted in the call order.
        /// </summary>
        /// <param name="expectedBuildingBlock">The expected building block.</param>
        public void AssertBuildingBlockInserted(BuildingBlock expectedBuildingBlock)
        {
            LogicalBuildingBlock bb = this.GetNextVerificationLogicalContent<LogicalBuildingBlock>();
            Assert.AreSame(expectedBuildingBlock, bb.BuildingBlock);
        }

        /// <summary>
        /// Asserts that a building block was inserted surrounded by a bookmark, in the call order.
        /// </summary>
        /// <param name="expectedBuildingBlock">The expected building block.</param>
        /// <param name="expectedBookmarkName">The expected bookmark name.</param>
        public void AssertBuildingBlockInserted(BuildingBlock expectedBuildingBlock, string expectedBookmarkName)
        {
            this.AssertBookmarkInserted(expectedBookmarkName, true);
            this.AssertBuildingBlockInserted(expectedBuildingBlock);
            this.AssertBookmarkInserted(expectedBookmarkName, false);
        }

        /// <summary>
        /// Asserts that there is no more content in the mock document.
        /// </summary>
        public void AssertEndOfDocument()
        {
            Assert.AreEqual<int>(this.logicalContentList.Count, this.logicalContentListVerificationIndex, "There should not be any other content added");
        }

        /// <summary>
        /// Dumps the actual mock document that has been accumulated.
        /// </summary>
        public void DumpActualDocument()
        {
            Console.WriteLine("Dump of accumulated document.");
            foreach (object o in this.logicalContentList)
            {
                Console.WriteLine("{0}", o);
            }
        }

        /// <summary>
        /// Gets the next item in the logical content to be verified.
        /// </summary>
        /// <typeparam name="T">The type of the next logical content expected.</typeparam>
        /// <returns>The next item in the logical content for verification.</returns>
        private T GetNextVerificationLogicalContent<T>() where T : class
        {
            Assert.IsTrue(this.logicalContentListVerificationIndex < this.logicalContentList.Count, "There is no more logical content to be verified.");
            object o = this.logicalContentList[this.logicalContentListVerificationIndex++];
            T ans = o as T;
            Assert.IsNotNull(ans, string.Format(CultureInfo.InvariantCulture, "Expected {0}, but found {1}", typeof(T).Name, o));
            return ans;
        }

        /// <summary>
        /// Builds the list of bookmarks, with ranges that reflect the actual sequence of bookmarks.
        /// </summary>
        /// <returns>List of bookmarks with ordered ranges.</returns>
        private IEnumerable<Bookmark> BuildBookmarks()
        {
            int i = 0;
            foreach (Mock<Bookmark> mockBookmark in this.mockBookmarks)
            {
                mockBookmark.Setup(b => b.Start).Returns((i * 2) + 1);
                mockBookmark.Setup(b => b.End).Returns((i * 2) + 2);
                Mock<Microsoft.Office.Interop.Word.Range> mockRange = new Mock<Microsoft.Office.Interop.Word.Range>();
                mockRange.Setup(r => r.Start).Returns(mockBookmark.Object.Start);
                mockRange.Setup(r => r.End).Returns(mockBookmark.Object.End);
                yield return mockBookmark.Object;
                i++;
            }
        }

        /// <summary>
        /// Finds a bookmark by name.
        /// </summary>
        /// <param name="bookmarkName">The name of the bookmark to find.</param>
        /// <returns>The bookmark with that name.</returns>
        private Mock<Bookmark> FindBookmark(string bookmarkName)
        {
            Mock<Bookmark> ans = this.mockBookmarks.Where(b => b.Object.Name == bookmarkName).SingleOrDefault();
            return ans;
        }

        /// <summary>
        /// Finds a bookmark by name returning the index.
        /// </summary>
        /// <param name="bookmarkName">The name of the bookmark to find.</param>
        /// <returns>The index of bookmark with that name.</returns>
        private int FindBookmarkIndex(string bookmarkName)
        {
            int ans = -1;
            Mock<Bookmark> b = this.FindBookmark(bookmarkName);
            if (b != null)
            {
                ans = this.mockBookmarks.IndexOf(b);
            }

            return ans;
        }

        /// <summary>
        /// Deletes a bookmark by name.
        /// </summary>
        /// <param name="bookmarkName">The name of the bookmark to delete.</param>
        private void DeleteBookmark(string bookmarkName)
        {
            int ix = this.FindBookmarkIndex(bookmarkName);
            if (ix >= 0)
            {
                this.mockBookmarks.RemoveAt(ix);
            }
        }

        /// <summary>
        /// Creates a mock bookmark.
        /// </summary>
        /// <param name="bookmarkName">The name of the bookmark to create.</param>
        /// <returns>The mock bookmark.</returns>
        private Mock<Bookmark> CreateMockBookmark(string bookmarkName)
        {
            Mock<Bookmark> ans = new Mock<Bookmark>();
            ans.Setup(b => b.Name).Returns(bookmarkName);
            return ans;
        }

        /// <summary>
        /// Represents a bookmark in the logical content of the Word document.
        /// </summary>
        internal class LogicalBookmark
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="LogicalBookmark"/> class.
            /// </summary>
            /// <param name="name">The name of the bookmark.</param>
            /// <param name="start">A value indicating whether this is the start of end of the bookmark.</param>
            public LogicalBookmark(string name, bool start)
            {
                this.Name = name;
                this.IsStart = start;
            }

            /// <summary>
            /// Gets the name of the logical bookmark.
            /// </summary>
            public string Name { get; private set; }

            /// <summary>
            /// Gets a value indicating whether this is the start of end of the bookmark.
            /// </summary>
            public bool IsStart { get; private set; }

            /// <summary>
            /// Gets the string representation of the logical bookmark.
            /// </summary>
            /// <returns>The string representation of the logical bookmark.</returns>
            public override string ToString()
            {
                return "Bookmark " + this.Name + (this.IsStart ? " Start" : " End");
            }
        }

        /// <summary>
        /// Represents a paragraph in the logical content of the Word document.
        /// </summary>
        internal class LogicalParagraph
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="LogicalParagraph"/> class.
            /// </summary>
            /// <param name="content">The content of the paragraph.</param>
            /// <param name="style">The style of the paragraph.</param>
            public LogicalParagraph(string content, string style)
            {
                this.Content = content;
                this.Style = style;
            }

            /// <summary>
            /// Gets the content of the logical paragraph.
            /// </summary>
            public string Content { get; private set; }

            /// <summary>
            /// Gets the style of the logical paragraph.
            /// </summary>
            public string Style { get; private set; }

            /// <summary>
            /// Gets the string representation of the logical paragraph.
            /// </summary>
            /// <returns>The string representation of the logical paragraph.</returns>
            public override string ToString()
            {
                return "Paragraph " + this.Style + " " + this.Content;
            }
        }

        /// <summary>
        /// Represents a building block in the logical content of the Word document.
        /// </summary>
        internal class LogicalBuildingBlock
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="LogicalBuildingBlock"/> class.
            /// </summary>
            /// <param name="buildingBlock">The building block.</param>
            public LogicalBuildingBlock(BuildingBlock buildingBlock)
            {
                this.BuildingBlock = buildingBlock;
            }

            /// <summary>
            /// Gets the building block.
            /// </summary>
            public BuildingBlock BuildingBlock { get; private set; }

            /// <summary>
            /// Gets the string representation of the logical building block.
            /// </summary>
            /// <returns>The string representation of the logical building block.</returns>
            public override string ToString()
            {
                return "Building Block " + this.BuildingBlock.Name;
            }
        }
    }
}
