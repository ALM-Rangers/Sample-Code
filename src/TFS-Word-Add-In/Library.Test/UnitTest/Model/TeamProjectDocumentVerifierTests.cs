//---------------------------------------------------------------------
// <copyright file="TeamProjectDocumentVerifierTests.cs" company="Microsoft">
//    Copyright (c) Microsoft. All rights reserved.
//    This code is licensed under the MIT License.
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF 
//    ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
//    TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
//    PARTICULAR PURPOSE AND NONINFRINGEMENT.
// </copyright>
// <summary>The TeamProjectDocumentVerifierTests type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.Word4Tfs.Library.Test.UnitTest.Model
{
    using global::System.Collections.Generic;
    using global::System.Linq;
    using Microsoft.Office.Interop.Word;
    using Microsoft.Practices.Unity;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Word4Tfs.Library.Data;
    using Microsoft.Word4Tfs.Library.Model;
    using Microsoft.Word4Tfs.Library.Model.Word;
    using Moq;

    /// <summary>
    /// Tests the <see cref="TeamProjectDocumentVerifier class"/>.
    /// </summary>
    [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Disposed in test cleanup method.")]
    [TestClass]
    public class TeamProjectDocumentVerifierTests
    {
        /// <summary>
        /// The Unity container to be used for Dependency Injection
        /// </summary>
        private IUnityContainer container;

        /// <summary>
        /// The mock word document used to test the model.
        /// </summary>
        private Mock<IWordDocument> mockWordDocument;

        /// <summary>
        /// The mock document managed by the bookmark manager.
        /// </summary>
        private Mock<Document> mockDocument;

        /// <summary>
        /// The model to be tested.
        /// </summary>
        private ITeamProjectDocumentVerifier sut;

        /// <summary>
        /// Initializes the test.
        /// </summary>
        [TestInitialize]
        public void InitializeTest()
        {
            this.container = new UnityContainer();

            this.mockWordDocument = new Mock<IWordDocument>();
            this.container.RegisterInstance<IWordDocument>(this.mockWordDocument.Object);

            this.mockWordDocument.Setup(doc => doc.ContentControlsInRange(It.IsAny<Microsoft.Office.Interop.Word.Range>()))
                                 .Returns(
                                 (Microsoft.Office.Interop.Word.Range r) =>
                                 {
                                     List<ContentControl> ans = new List<ContentControl>();
                                     foreach (ContentControl cc in r.ContentControls)
                                     {
                                         ans.Add(cc);
                                     }

                                     return ans;
                                 });

            this.sut = this.container.Resolve<TeamProjectDocumentVerifier>();
        }

        /// <summary>
        /// Cleans up the test.
        /// </summary>
        [TestCleanup]
        public void CleanupTest()
        {
            this.container.Dispose();
        }

        /// <summary>
        /// Test that <see cref="TeamProjectDocumentVerifier.VerifyDocument"/> returns success even if there are missing bookmarks.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void VerifyDocument_WhenBookmarkIsMissing_ReturnsSuccess()
        {
            // Arrange
            QueryWorkItems[] queryWorkItems = TestHelper.CreateQueryWorkItems(new int[] { 1, 2, 3 }, new int[] { 3, 4, 5 });
            this.SetupDocumentBookmarks(
                                        TestHelper.CreateMockBookmark(0, 1),
                                        TestHelper.CreateMockBookmark(0, 2),
                                        TestHelper.CreateMockBookmark(1, 5));

            // Act
            string[] ans = this.VerifyDocument(queryWorkItems);

            // Assert
            Assert.AreEqual<int>(0, ans.Length, "Did not detect the correct number of errors");
        }

        /// <summary>
        /// Test that <see cref="TeamProjectDocumentVerifier.VerifyDocument"/> returns errors for any bookmarks for which the work item does not exist.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void VerifyDocument_WhenBookmarkIsPresentButWorkItemIsMissing_ReturnsErrorAboutMissingWorkItem()
        {
            // Arrange
            QueryWorkItems[] queryWorkItems = TestHelper.CreateQueryWorkItems(new int[] { 2, 3 }, new int[] { 3 });
            this.SetupDocumentBookmarks(
                                        TestHelper.CreateMockBookmark(0, 1),
                                        TestHelper.CreateMockBookmark(0, 2),
                                        TestHelper.CreateMockBookmark(0, 3),
                                        TestHelper.CreateMockBookmark(1, 2),
                                        TestHelper.CreateMockBookmark(1, 3),
                                        TestHelper.CreateMockBookmark(99, 1));

            // Act
            string[] ans = this.VerifyDocument(queryWorkItems);

            // Assert
            Assert.AreEqual<int>(3, ans.Length, "Did not detect the correct number of errors");
            Assert.AreEqual<string>("Work item 1 bookmark is present but the work item content is missing from the document for query 0", ans[0]);
            Assert.AreEqual<string>("Work item 2 bookmark is present but the work item content is missing from the document for query 1", ans[1]);
            Assert.AreEqual<string>("Work item 1 bookmark is present but the work item content is missing from the document for query 99", ans[2]);
        }

        /// <summary>
        /// Test that <see cref="TeamProjectDocumentVerifier.VerifyDocument"/> does not return errors for any bookmarks which are not in the work item name format.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void VerifyDocument_WhenNonWorkItemBookmarkIsPresent_NoErrorsAreReturned()
        {
            // Arrange
            QueryWorkItems[] queryWorkItems = TestHelper.CreateQueryWorkItems(new int[] { 1 });
            this.SetupDocumentBookmarks(TestHelper.CreateMockBookmark("NonWorkItembookmark"), TestHelper.CreateMockBookmark(0, 1));

            // Act
            string[] ans = this.VerifyDocument(queryWorkItems);

            // Assert
            Assert.AreEqual<int>(0, ans.Length, "Should not have detected any errors");
        }

        /// <summary>
        /// Test that <see cref="TeamProjectDocumentVerifier.VerifyDocument"/> returns errors for content controls bound to work items which do not belong to the bookmark.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void VerifyDocument_WhenBookmarkContainsContentControlsNotMappedToSameWorkItem_ReturnsErrorsAboutMisplacedContent()
        {
            // Arrange
            QueryWorkItems[] queryWorkItems = TestHelper.CreateQueryWorkItems(new int[] { 1, 2 });
            this.SetupDocumentBookmarks(
                                        TestHelper.CreateMockBookmark(0, 1, TestHelper.CreateMockContentControl(1, 2, true, "2")),
                                        TestHelper.CreateMockBookmark(0, 2, TestHelper.CreateMockContentControl(11, 12, true, "1")));

            // Act
            string[] ans = this.VerifyDocument(queryWorkItems);

            // Assert
            Assert.AreEqual<int>(2, ans.Length, "Did not detect the correct number of errors");
            Assert.AreEqual<string>("Work item 1 for query 0 bookmark contains fields from work item 2", ans[0]);
            Assert.AreEqual<string>("Work item 2 for query 0 bookmark contains fields from work item 1", ans[1]);
        }

        /// <summary>
        /// Test that <see cref="TeamProjectDocumentVerifier.VerifyDocument"/> gets content controls via word document and not via the range directly.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void VerifyDocument_GetsContentControlsViaWordDocumentAndNotRange()
        {
            // Arrange
            QueryWorkItems[] queryWorkItems = TestHelper.CreateQueryWorkItems(new int[] { 1, 2 });
            this.SetupDocumentBookmarks(
                                        TestHelper.CreateMockBookmark(0, 1, TestHelper.CreateMockContentControl(1, 2, true, "2")),
                                        TestHelper.CreateMockBookmark(0, 2, TestHelper.CreateMockContentControl(11, 12, true, "1")));

            this.mockWordDocument.Setup(doc => doc.ContentControlsInRange(It.IsAny<Microsoft.Office.Interop.Word.Range>())).Returns(new ContentControl[0]);

            // Act
            string[] ans = this.VerifyDocument(queryWorkItems);

            // Assert
            Assert.AreEqual<int>(0, ans.Length, "Got content controls from bookmark range and not from document");
        }

        /// <summary>
        /// Test that <see cref="TeamProjectDocumentVerifier.VerifyDocument"/> returns no errors for content controls mapped but not bound to work items.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void VerifyDocument_WhenBookmarkContainsContentControlsMappedButNotToAWorkItem_ReturnsNoErrors()
        {
            // Arrange
            QueryWorkItems[] queryWorkItems = TestHelper.CreateQueryWorkItems(new int[] { 1 });
            this.SetupDocumentBookmarks(TestHelper.CreateMockBookmark(0, 1, TestHelper.CreateMockContentControl(1, 2, true, "A")));

            // Act
            string[] ans = this.VerifyDocument(queryWorkItems);

            // Assert
            Assert.AreEqual<int>(0, ans.Length, "Should not detect any errors");
        }

        /// <summary>
        /// Test that <see cref="TeamProjectDocumentVerifier.VerifyDocument"/> returns no errors for content controls that are not mapped.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void VerifyDocument_WhenBookmarkContainsContentControlsNotMapped_ReturnsNoErrors()
        {
            // Arrange
            QueryWorkItems[] queryWorkItems = TestHelper.CreateQueryWorkItems(new int[] { 1 });
            this.SetupDocumentBookmarks(TestHelper.CreateMockBookmark(0, 1, TestHelper.CreateMockContentControl(1, 2, false, null)));

            // Act
            string[] ans = this.VerifyDocument(queryWorkItems);

            // Assert
            Assert.AreEqual<int>(0, ans.Length, "Should not detect any errors");
        }

        /// <summary>
        /// Test that <see cref="TeamProjectDocumentVerifier.VerifyDocument"/> returns errors for content controls mapped to work items but not inside any bookmark.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void VerifyDocument_WhenMappedContentControlIsNotInABookmark_ReturnsErrors()
        {
            // Arrange
            QueryWorkItems[] queryWorkItems = TestHelper.CreateQueryWorkItems(new int[] { 1 });
            this.SetupDocumentBookmarks(TestHelper.CreateMockBookmark(0, 1, 1, 10));
            this.mockWordDocument.Setup(doc => doc.AllContentControls()).Returns(new ContentControl[] { TestHelper.CreateMockContentControl(11, 12, true, "1") });

            // Act
            string[] ans = this.VerifyDocument(queryWorkItems);

            // Assert
            Assert.AreEqual<int>(1, ans.Length, "Did not detect the correct number of errors");
            Assert.AreEqual<string>("Field from work item 1 is not in any bookmark", ans[0]);
        }

        /// <summary>
        /// Test that <see cref="TeamProjectDocumentVerifier.VerifyDocument"/> returns no errors for content controls not mapped to work items and not inside any bookmark.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void VerifyDocument_WhenUnmappedContentControlIsNotInABookmark_ReturnsNoErrors()
        {
            // Arrange
            QueryWorkItems[] queryWorkItems = TestHelper.CreateQueryWorkItems(new int[0]);
            this.SetupDocumentBookmarks(new Bookmark[0]);
            this.mockWordDocument.Setup(doc => doc.AllContentControls()).Returns(new ContentControl[] { TestHelper.CreateMockContentControl(10, 11, false, string.Empty) });

            // Act
            string[] ans = this.VerifyDocument(queryWorkItems);

            // Assert
            Assert.AreEqual<int>(0, ans.Length, "Should not detect any errors");
        }

        /// <summary>
        /// Test that <see cref="TeamProjectDocumentVerifier.VerifyDocument"/> returns errors for work item bookmarks that overlap.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void VerifyDocument_WhenWorkItemBookmarksOverlap_ReturnsErrors()
        {
            // Arrange
            QueryWorkItems[] queryWorkItems = TestHelper.CreateQueryWorkItems(new int[] { 1, 2, 3, 4, 5 });
            this.SetupDocumentBookmarks(
                                        TestHelper.CreateMockBookmark(0, 1, 1, 10),
                                        TestHelper.CreateMockBookmark(0, 2, 2, 3),
                                        TestHelper.CreateMockBookmark(0, 3, 9, 11),
                                        TestHelper.CreateMockBookmark(0, 4, 21, 30),
                                        TestHelper.CreateMockBookmark(0, 5, 21, 30),
                                        TestHelper.CreateMockBookmark("nonbookmark1", 4, 5),
                                        TestHelper.CreateMockBookmark("nonbookmark2", 8, 12));

            // Act
            string[] ans = this.VerifyDocument(queryWorkItems);

            // Assert
            Assert.AreEqual<int>(3, ans.Length, "Did not detect the correct number of errors");
            Assert.AreEqual<string>("Bookmark for work item 1 in query 0 overlaps with bookmark for work item 2 in query 0", ans[0]);
            Assert.AreEqual<string>("Bookmark for work item 1 in query 0 overlaps with bookmark for work item 3 in query 0", ans[1]);
            Assert.AreEqual<string>("Bookmark for work item 4 in query 0 overlaps with bookmark for work item 5 in query 0", ans[2]);
        }

        /// <summary>
        /// Test that <see cref="TeamProjectDocumentVerifier.VerifyDocument"/> adjacent work item bookmarks are not considered to overlap.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void VerifyDocument_WhenWorkItemBookmarksAreAdjacent_NoErrorsAreReturned()
        {
            // Arrange
            QueryWorkItems[] queryWorkItems = TestHelper.CreateQueryWorkItems(new int[] { 1, 2, 3 });
            this.SetupDocumentBookmarks(
                                        TestHelper.CreateMockBookmark(0, 1, 1, 10),
                                        TestHelper.CreateMockBookmark(0, 2, 10, 20),
                                        TestHelper.CreateMockBookmark(0, 3, 20, 30));

            // Act
            string[] ans = this.VerifyDocument(queryWorkItems);

            // Assert
            Assert.AreEqual<int>(0, ans.Length, "Should not detect any errors");
        }

        /// <summary>
        /// Test that <see cref="TeamProjectDocumentVerifier.VerifyDocument"/> refreshes bookmarks each time.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void VerifyDocument_RefreshesBookmarksEachTime()
        {
            // Arrange
            QueryWorkItems[] queryWorkItems = TestHelper.CreateQueryWorkItems(new int[] { 1, 2, 3 });
            this.SetupDocumentBookmarks(
                                        TestHelper.CreateMockBookmark(0, 1),
                                        TestHelper.CreateMockBookmark(0, 2),
                                        TestHelper.CreateMockBookmark(0, 3));
            this.VerifyDocument(queryWorkItems);
            TestHelper.SetMockBookmarksForMockDocument(
                                                       this.mockDocument,
                                                       TestHelper.CreateMockBookmark(0, 1),
                                                       TestHelper.CreateMockBookmark(0, 2, 0, 1),
                                                       TestHelper.CreateMockBookmark(0, 3, 0, 1));

            // Act
            string[] ans = this.VerifyDocument(queryWorkItems);

            // Assert
            Assert.AreEqual<int>(1, ans.Length, "Did not refresh");
            Assert.AreEqual<string>("Bookmark for work item 2 in query 0 overlaps with bookmark for work item 3 in query 0", ans[0]);
        }

        /// <summary>
        /// Sets up the mock document with some bookmarks.
        /// </summary>
        /// <param name="bookmarks">The bookmarks to add to the document.</param>
        private void SetupDocumentBookmarks(params Bookmark[] bookmarks)
        {
            this.mockDocument = TestHelper.CreateMockDocument(bookmarks);
            this.mockWordDocument.Setup(doc => doc.BookmarkManager).Returns(TestHelper.CreateMockBookmarkManager(this.mockDocument));
        }

        /// <summary>
        /// Runs the verification.
        /// </summary>
        /// <param name="queryWorkItems">The work items in the mock document for each query.</param>
        /// <returns>Array of errors returned from the verification.</returns>
        private string[] VerifyDocument(IEnumerable<QueryWorkItems> queryWorkItems)
        {
            return this.sut.VerifyDocument(queryWorkItems, TestHelper.DummyBookmarkNamingFunc, TestHelper.DummyBookmarkParsingFunc, TestHelper.DummyXPathParsingFunc).ToArray();
        }
    }
}
