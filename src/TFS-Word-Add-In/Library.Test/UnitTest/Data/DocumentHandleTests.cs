//---------------------------------------------------------------------
// <copyright file="DocumentHandleTests.cs" company="Microsoft">
//    Copyright (c) Microsoft. All rights reserved.
//    This code is licensed under the MIT License.
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF 
//    ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
//    TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
//    PARTICULAR PURPOSE AND NONINFRINGEMENT.
// </copyright>
// <summary>The DocumentHandleTests type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.Word4Tfs.Library.Test.UnitTest.Data
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Word4Tfs.Library.Data;

    /// <summary>
    /// Tests the <see cref="DocumentHandle"/> class.
    /// </summary>
    [TestClass]
    public class DocumentHandleTests
    {
        /// <summary>
        /// Test that a guid handle is not equal to a name handle.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void GuidHandleNotEqualToNameHandle()
        {
            // Act
            DocumentHandle guidHandle = new DocumentHandle(Guid.NewGuid());
            DocumentHandle nameHandle = new DocumentHandle("test");

            // Assert
            Assert.IsFalse(guidHandle.Equals(nameHandle), "Should be considered different");
            Assert.IsFalse(nameHandle.Equals(guidHandle), "Should be considered different");
        }

        /// <summary>
        /// Tests that <see cref="DocumentHandle.Equals"/> does not match null.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void EqualsDoesNotMatchNull()
        {
            // Arrange
            DocumentHandle guidHandle = new DocumentHandle(Guid.NewGuid());
            DocumentHandle nameHandle = new DocumentHandle("test");

            // Assert
            Assert.IsFalse(guidHandle.Equals(null), "Should not be equal");
            Assert.IsFalse(nameHandle.Equals(null), "Should not be equal");
        }

        /// <summary>
        /// Tests that <see cref="DocumentHandle.Equals"/> does not match non <see cref="DocumentHandle"/>.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void EqualsDoesNotMatchNonDocumentHandle()
        {
            // Arrange
            DocumentHandle guidHandle = new DocumentHandle(Guid.NewGuid());
            DocumentHandle nameHandle = new DocumentHandle("test");
            object o = (object)1;

            // Assert
            Assert.IsFalse(guidHandle.Equals(o), "Should not be equal");
            Assert.IsFalse(nameHandle.Equals(o), "Should not be equal");
        }

        /// <summary>
        /// Tests that two <see cref="DocumentHandle"/> objects are equal if they have the same guid.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void EqualsMatchesTwoGuids()
        {
            // Arrange
            Guid guid = Guid.NewGuid();
            DocumentHandle guidHandle1 = new DocumentHandle(guid);
            DocumentHandle guidHandle2 = new DocumentHandle(guid);

            // Assert
            Assert.IsTrue(guidHandle1.Equals(guidHandle2), "Should be equal");
        }

        /// <summary>
        /// Tests that two <see cref="DocumentHandle"/> objects are equal if they have the same name.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void EqualsMatchesTwoNames()
        {
            // Arrange
            DocumentHandle nameHandle1 = new DocumentHandle("test");
            DocumentHandle nameHandle2 = new DocumentHandle("test");

            // Assert
            Assert.IsTrue(nameHandle1.Equals(nameHandle2), "Should be equal");
        }

        /// <summary>
        /// Tests that two different <see cref="DocumentHandle"/> objects are not equal if they have different guids.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void EqualsDoesNotMatchTwoDifferentGuids()
        {
            // Arrange
            DocumentHandle guidHandle1 = new DocumentHandle(Guid.NewGuid());
            DocumentHandle guidHandle2 = new DocumentHandle(Guid.NewGuid());

            // Assert
            Assert.IsFalse(guidHandle1.Equals(guidHandle2), "Should not be equal");
        }

        /// <summary>
        /// Tests that two different <see cref="DocumentHandle"/> objects are not equal if they have different names.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void EqualsDoesNotMatchTwoDifferentNames()
        {
            // Arrange
            DocumentHandle nameHandle1 = new DocumentHandle("test1");
            DocumentHandle nameHandle2 = new DocumentHandle("test2");

            // Assert
            Assert.IsFalse(nameHandle1.Equals(nameHandle2), "Should not be equal");
        }

        /// <summary>
        /// Tests that two <see cref="DocumentHandle"/> objects are equal using the == operator.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void EqualsOperatorMatchesTwoEqualDocumentHandles()
        {
            // Arrange
            DocumentHandle handle1 = new DocumentHandle("Test");
            DocumentHandle handle2 = new DocumentHandle("Test");

            // Assert
            Assert.IsTrue(handle1 == handle2, "Should be equal");
        }

        /// <summary>
        /// Tests that two <see cref="DocumentHandle"/> objects are not equal using the != operator.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void NotEqualsOperatorChecksObjectsForInequality()
        {
            // Arrange
            DocumentHandle handle1 = new DocumentHandle("Test");
            DocumentHandle handle2 = new DocumentHandle("Test");

            // Assert
            Assert.IsFalse(handle1 != handle2, "Should be equal");
        }

        /// <summary>
        /// Tests that two <see cref="DocumentHandle"/> objects based on a guid that are equal have the same hash code.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void EqualGuidDocumentHandlesHaveSameHashCode()
        {
            // Arrange
            Guid guid = Guid.NewGuid();
            DocumentHandle handle1 = new DocumentHandle(guid);
            DocumentHandle handle2 = new DocumentHandle(guid);

            // Assert
            Assert.AreEqual<int>(handle1.GetHashCode(), handle2.GetHashCode(), "Should be equal");
        }

        /// <summary>
        /// Tests that two <see cref="DocumentHandle"/> objects based on a name that are equal have the same hash code.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void EqualNameDocumentHandlesHaveSameHashCode()
        {
            // Arrange
            DocumentHandle handle1 = new DocumentHandle("test");
            DocumentHandle handle2 = new DocumentHandle("test");

            // Assert
            Assert.AreEqual<int>(handle1.GetHashCode(), handle2.GetHashCode(), "Should be equal");
        }

        /// <summary>
        /// Tests that two <see cref="DocumentHandle"/> objects that are not equal do not have the same hash code for a guid handle.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void NotEqualGuidDocumentHandlesHaveDifferentHashCode()
        {
            // Arrange
            DocumentHandle handle1 = new DocumentHandle(Guid.NewGuid());
            DocumentHandle handle2 = new DocumentHandle(Guid.NewGuid());

            // Assert
            Assert.AreNotEqual<int>(handle1.GetHashCode(), handle2.GetHashCode(), "Should not be equal");
        }

        /// <summary>
        /// Tests that two <see cref="DocumentHandle"/> objects that are not equal do not have the same hash code for a name handle.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void NotEqualNameDocumentHandlesHaveDifferentHashCode()
        {
            // Arrange
            DocumentHandle handle1 = new DocumentHandle("test1");
            DocumentHandle handle2 = new DocumentHandle("test2");

            // Assert
            Assert.AreNotEqual<int>(handle1.GetHashCode(), handle2.GetHashCode(), "Should not be equal");
        }

        /// <summary>
        /// Test that <see cref="DocumentHandle.ToString"/> is the guid for a guid handle.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void GuidHandleToStringIsGuid()
        {
            // Arrange
            Guid testGuid = Guid.NewGuid();
            DocumentHandle guidHandle = new DocumentHandle(testGuid);

            // Act
            string ans = guidHandle.ToString();

            // Assert
            Assert.AreEqual<string>(testGuid.ToString(), ans, "string should be the guid handle");
        }

        /// <summary>
        /// Test that <see cref="DocumentHandle.ToString"/> is the string for a name handle.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void NameHandleToStringIsName()
        {
            // Arrange
            string testName = "test";
            DocumentHandle nameHandle = new DocumentHandle(testName);

            // Act
            string ans = nameHandle.ToString();

            // Assert
            Assert.AreEqual<string>(testName, ans, "string should be the name handle");
        }
    }
}
