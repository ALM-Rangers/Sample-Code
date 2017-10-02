//---------------------------------------------------------------------
// <copyright file="BookmarkManagerTests.cs" company="Microsoft">
//    Copyright © ALM | DevOps Ranger Contributors. All rights reserved.
//    This code is licensed under the MIT License.
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF 
//    ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
//    TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
//    PARTICULAR PURPOSE AND NONINFRINGEMENT.
// </copyright>
// <summary>The BookmarkManagerTests type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.Word4Tfs.Library.Test.UnitTest
{
    using global::System.Linq;
    using Microsoft.Office.Interop.Word;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Word4Tfs.Library.Model.Word;
    using Moq;

    /// <summary>
    /// Tests the <see cref="BookmarkManager"/> class.
    /// </summary>
    [TestClass]
    public class BookmarkManagerTests
    {
        /// <summary>
        /// The bookmark manager to test.
        /// </summary>
        private IBookmarkManager sut;

        /// <summary>
        /// A test bookmark
        /// </summary>
        private Bookmark bookmark1;

        /// <summary>
        /// A test bookmark
        /// </summary>
        private Bookmark bookmark2;

        /// <summary>
        /// A test bookmark
        /// </summary>
        private Bookmark bookmark3;

        /// <summary>
        /// A test bookmark
        /// </summary>
        private Bookmark bookmark4;

        /// <summary>
        /// The mock document from which the bookmarks are taken.
        /// </summary>
        private Mock<Document> mockDocument;

        /// <summary>
        /// Sets up the bookmark manager with an initial set of bookmarks.
        /// </summary>
        [TestInitialize]
        public void InitializeTest()
        {
            this.bookmark1 = TestHelper.CreateMockBookmark("1");
            this.bookmark2 = TestHelper.CreateMockBookmark("2");
            this.bookmark3 = TestHelper.CreateMockBookmark("3");
            this.bookmark4 = TestHelper.CreateMockBookmark("4");

            this.mockDocument = TestHelper.CreateMockDocument(this.bookmark1, this.bookmark2, this.bookmark3);
            this.sut = new BookmarkManager(this.mockDocument.Object);
        }

        /// <summary>
        /// Tests that the <see cref="BookmarkManager"/> constructor adds correct number of bookmarks to the collection.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void BookmarkManagerConstructorAddsCorrectNumberOfBookmarksToCollection()
        {
            // Assert
            Assert.AreEqual<int>(3, this.sut.Bookmarks.Count());
        }

        /// <summary>
        /// Tests that the <see cref="BookmarkManager"/> constructor adds the bookmarks to the collection.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void BookmarkManagerConstructorAddsBookmarksToCollection()
        {
            // Assert
            Assert.AreSame(this.bookmark1, this.sut["1"], "Bookmark was not added to the collection");
            Assert.AreSame(this.bookmark2, this.sut["2"], "Bookmark was not added to the collection");
            Assert.AreSame(this.bookmark3, this.sut["3"], "Bookmark was not added to the collection");
        }

        /// <summary>
        /// Tests that <see cref="BookmarkManager"/> returns the same bookmarks from the <see cref="BookmarkManager.Bookmarks"/> collection.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void BookmarkManagerReturnsSameBookmarksThroughTheBookmarksProperty()
        {
            // Assert
            Assert.AreSame(this.bookmark1, this.sut.Bookmarks.Where(b => b.Name == "1").Single(), "Bookmark was not added to the collection");
            Assert.AreSame(this.bookmark2, this.sut.Bookmarks.Where(b => b.Name == "2").Single(), "Bookmark was not added to the collection");
            Assert.AreSame(this.bookmark3, this.sut.Bookmarks.Where(b => b.Name == "3").Single(), "Bookmark was not added to the collection");
        }

        /// <summary>
        /// Tests that <see cref="BookmarkManager"/> returns <c>null</c> if bookmark not in the collection.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void BookmarkManagerReturnsNullIfBookmarkDoesNotExist()
        {
            // Assert
            Assert.IsNull(this.sut["4"], "Bookmark should not be in the collection");
        }

        /// <summary>
        /// Tests that <see cref="BookmarkManager.Exists"/> returns <c>true</c> for an existing bookmark.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void BookmarkManager_Exists_ReturnsTrueIfBookmarkExists()
        {
            // Assert
            Assert.IsTrue(this.sut.Exists("1"), "Bookmark should be in the collection");
        }

        /// <summary>
        /// Tests that <see cref="BookmarkManager.Exists"/> returns <c>false</c> for a bookmark not in the collection.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void BookmarkManager_Exists_ReturnsFalseIfBookmarkDoesNotExist()
        {
            // Assert
            Assert.IsFalse(this.sut.Exists("4"), "Bookmark should not be in the collection");
        }

        /// <summary>
        /// Tests that <see cref="BookmarkManager.Add"/> adds a bookmark to the collection.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void BookmarkManager_Add_AddsNewBookmark()
        {
            // Act
            this.sut.Add("4", TestHelper.CreateMockRange(1, 1).Object);

            // Assert
            Assert.IsTrue(this.sut.Exists("4"), "Bookmark should now be in the collection");
        }

        /// <summary>
        /// Tests that <see cref="BookmarkManager.ChangeOrAdd"/> adds a new bookmark to the collection.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void BookmarkManager_ChangeOrAdd_AddsNewBookmark()
        {
            // Act
            this.sut.ChangeOrAdd("4", TestHelper.CreateMockRange(1, 1).Object);

            // Assert
            Assert.IsTrue(this.sut.Exists("4"), "Bookmark should now be in the collection");
        }

        /// <summary>
        /// Tests that <see cref="BookmarkManager.ChangeOrAdd"/> replaces existing bookmark in the collection.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void BookmarkManager_ChangeOrAdd_ReplacesExistingBookmark()
        {
            // Act
            this.sut.ChangeOrAdd("3", TestHelper.CreateMockRange(1, 1).Object);

            // Assert
            Assert.IsTrue(this.sut.Exists("3"), "Bookmark should still be in the collection");
            Assert.AreNotSame(this.bookmark3, this.sut["3"]);
        }

        /// <summary>
        /// Tests that <see cref="BookmarkManager.Delete"/> by <see cref="Bookmark"/> deletes the bookmark.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void BookmarkManager_DeleteByBookmark_DeletesBookmark()
        {
            // Act
            this.sut.Delete(this.bookmark2);

            // Assert
            Assert.IsFalse(this.sut.Exists("2"), "Bookmark should not longer be in the collection");
        }

        /// <summary>
        /// Tests that <see cref="BookmarkManager.Delete"/> throws an exception for a null bookmark.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void BookmarkManager_DeleteByNullBookmark_DeletesBookmark()
        {
            Bookmark temp = null;
            TestHelper.TestForArgumentNullException(() => this.sut.Delete(temp), "bookmark");
        }

        /// <summary>
        /// Tests that <see cref="BookmarkManager.Delete"/> by name deletes the bookmark.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void BookmarkManager_DeleteByName_DeletesBookmark()
        {
            // Act
            this.sut.Delete("2");

            // Assert
            Assert.IsFalse(this.sut.Exists("2"), "Bookmark should not longer be in the collection");
        }

        /// <summary>
        /// Tests that <see cref="BookmarkManager.Delete"/> by non-existent name deletes the bookmark silently.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void BookmarkManager_DeleteByNonExistentName_DeletesBookmarkSilently()
        {
            // Act
            this.sut.Delete("4");

            // Assert
            Assert.IsFalse(this.sut.Exists("4"), "Bookmark should not be in the collection");
        }

        /// <summary>
        /// Tests that <see cref="BookmarkManager.Refresh"/> removes bookmarks that no longer exist.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void BookmarkManager_Refresh_DeletesBookmarkThatNoLongerExist()
        {
            // Arrange
            TestHelper.SetMockBookmarksForMockDocument(this.mockDocument, this.bookmark1, this.bookmark3);

            // Act
            this.sut.Refresh();

            // Assert
            Assert.AreEqual<int>(2, this.sut.Bookmarks.Count(), "Deleted bookmark not removed.");
            Assert.AreSame(this.bookmark1, this.sut["1"], "Bookmark should be in the collection");
            Assert.AreSame(this.bookmark3, this.sut["3"], "Bookmark should be in the collection");
        }

        /// <summary>
        /// Tests that <see cref="BookmarkManager.Refresh"/> adds new bookmarks.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void BookmarkManager_Refresh_AddsNewBookmarks()
        {
            // Arrange
            TestHelper.SetMockBookmarksForMockDocument(this.mockDocument, this.bookmark1, this.bookmark2, this.bookmark3, this.bookmark4);

            // Act
            this.sut.Refresh();

            // Assert
            Assert.AreEqual<int>(4, this.sut.Bookmarks.Count(), "New bookmarks not added.");
            Assert.AreSame(this.bookmark1, this.sut["1"], "Bookmark should be in the collection");
            Assert.AreSame(this.bookmark2, this.sut["2"], "Bookmark should be in the collection");
            Assert.AreSame(this.bookmark3, this.sut["3"], "Bookmark should be in the collection");
            Assert.AreSame(this.bookmark4, this.sut["4"], "Bookmark should be in the collection");
        }
    }
}
