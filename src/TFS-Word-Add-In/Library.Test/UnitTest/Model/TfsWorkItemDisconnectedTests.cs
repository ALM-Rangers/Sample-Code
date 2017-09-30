//---------------------------------------------------------------------
// <copyright file="TfsWorkItemDisconnectedTests.cs" company="Microsoft">
//    Copyright © ALM | DevOps Ranger Contributors. All rights reserved.
//    This code is licensed under the MIT License.
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF 
//    ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
//    TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
//    PARTICULAR PURPOSE AND NONINFRINGEMENT.
// </copyright>
// <summary>The TfsWorkItemDisconnectedTests type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.Word4Tfs.Library.Test.UnitTest.Model
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Word4Tfs.Library.Model.TeamFoundation;

    /// <summary>
    /// Tests the <see cref="TfsWorkItemDisconnected"/> class.
    /// </summary>
    [TestClass]
    public class TfsWorkItemDisconnectedTests
    {
        /// <summary>
        /// Tests that the constructor throws an exception if passed a null list of fields.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void ConstructorThrowsExceptionForNullListOfFields()
        {
            TestHelper.TestForArgumentNullException(() => new TfsWorkItemDisconnected(1, "dummy", null), "fields");
        }

        /// <summary>
        /// Tests that the id passed to the constructor is used for the <see cref="TfsWorkItemDisconnected.Id"/> property.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void ConstructorSetsIdProperty()
        {
            // Arrange
            ITfsWorkItem sut = new TfsWorkItemDisconnected(1, "dummy");

            // Act
            int ans = sut.Id;

            // Assert
            Assert.AreEqual<int>(1, ans, "Id not set by constructor");
        }

        /// <summary>
        /// Tests that the work item type passed to the constructor is used for the <see cref="TfsWorkItemDisconnected.Type"/> property.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void ConstructorSetsTypeProperty()
        {
            // Arrange
            ITfsWorkItem sut = new TfsWorkItemDisconnected(1, "dummy");

            // Act
            string ans = sut.Type;

            // Assert
            Assert.AreEqual<string>("dummy", ans, "Type not set by constructor");
        }

        /// <summary>
        /// Tests that the id passed to the constructor surfaces as a field even if the field is not provided.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void IdFieldCreatedEvenIfFieldNotSupplied()
        {
            // Arrange
            ITfsWorkItem sut = new TfsWorkItemDisconnected(1, "dummy");

            // Assert
            Assert.IsTrue(sut.FieldReferenceNames.Contains("System.Id", StringComparer.OrdinalIgnoreCase), "System.Id field not present");
        }

        /// <summary>
        /// Tests that the id passed to the constructor is also the value of the corresponding field.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void IdFieldSetEvenIfFieldNotSupplied()
        {
            // Arrange
            ITfsWorkItem sut = new TfsWorkItemDisconnected(1, "dummy");

            // Act
            int ans = (int)sut["System.Id"];

            // Assert
            Assert.AreEqual<int>(1, ans, "System.Id field value not set by constructor");
        }

        /// <summary>
        /// Tests that id field is overridden by the value passed as the corresponding field.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void IdFieldOverriddenByAField()
        {
            // Arrange
            ITfsWorkItem sut = new TfsWorkItemDisconnected(1, "dummy", new Tuple<string, object>("System.Id", 2));

            // Act
            int ans = (int)sut["System.Id"];

            // Assert
            Assert.AreEqual<int>(2, ans, "System.Id field value not overridden by the field");
        }

        /// <summary>
        /// Tests that id property is overridden by the value passed as the corresponding field.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void IdPropertyOverriddenByAField()
        {
            // Arrange
            ITfsWorkItem sut = new TfsWorkItemDisconnected(1, "dummy", new Tuple<string, object>("System.Id", 2));

            // Act
            int ans = sut.Id;

            // Assert
            Assert.AreEqual<int>(2, ans, "Id property not overridden by the field");
        }

        /// <summary>
        /// Tests that the type passed to the constructor surfaces as a field even if the field is not provided.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void TypeFieldCreatedEvenIfFieldNotSupplied()
        {
            // Arrange
            ITfsWorkItem sut = new TfsWorkItemDisconnected(1, "dummy");

            // Assert
            Assert.IsTrue(sut.FieldReferenceNames.Contains("System.WorkItemType", StringComparer.OrdinalIgnoreCase), "System.WorkItemType field not present");
        }

        /// <summary>
        /// Tests that the type passed to the constructor is also the value of the corresponding field.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void TypeFieldSetEvenIfFieldNotSupplied()
        {
            // Arrange
            ITfsWorkItem sut = new TfsWorkItemDisconnected(1, "dummy");

            // Act
            string ans = (string)sut["System.WorkItemType"];

            // Assert
            Assert.AreEqual<string>("dummy", ans, "System.WorkItemType field value not set by constructor");
        }

        /// <summary>
        /// Tests that type field is overridden by the value passed as the corresponding field.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void TypeFieldOverriddenByAField()
        {
            // Arrange
            ITfsWorkItem sut = new TfsWorkItemDisconnected(1, "dummy", new Tuple<string, object>("System.WorkItemType", "dummy2"));

            // Act
            string ans = (string)sut["System.WorkItemType"];

            // Assert
            Assert.AreEqual<string>("dummy2", ans, "System.WorkItemType field value not overridden by the field");
        }

        /// <summary>
        /// Tests that type property is overridden by the value passed as the corresponding field.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void TypePropertyOverriddenByAField()
        {
            // Arrange
            ITfsWorkItem sut = new TfsWorkItemDisconnected(1, "dummy", new Tuple<string, object>("System.WorkItemType", "dummy2"));

            // Act
            string ans = sut.Type;

            // Assert
            Assert.AreEqual<string>("dummy2", ans, "IType property not overridden by the field");
        }

        /// <summary>
        /// Tests that field names are case sensitive.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void FieldNamesCaseSensitive()
        {
            // Arrange
            ITfsWorkItem sut = new TfsWorkItemDisconnected(1, "dummy", new Tuple<string, object>("System.Id", 2), new Tuple<string, object>("System.ID", 3));

            // Act
            int ansLower = (int)sut["System.Id"];
            int ansUpper = (int)sut["System.ID"];

            // Assert
            Assert.AreEqual<int>(2, ansLower, "Field name not case sensitive.");
            Assert.AreEqual<int>(3, ansUpper, "Field name not case sensitive.");
        }

        /// <summary>
        /// Tests that other fields can be added.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void CanAddOtherFields()
        {
            // Arrange and Act
            ITfsWorkItem sut = new TfsWorkItemDisconnected(1, "dummy", new Tuple<string, object>("A.B", 2), new Tuple<string, object>("C.D", "test"));

            // Act
            int ans = (int)sut["System.Id"];

            // Assert
            Assert.IsTrue(sut.FieldReferenceNames.Contains("A.B", StringComparer.OrdinalIgnoreCase), "Other field not present");
            Assert.IsTrue(sut.FieldReferenceNames.Contains("C.D", StringComparer.OrdinalIgnoreCase), "Other field not present");
        }

        /// <summary>
        /// Tests that other fields can be added and can get the values.
        /// </summary>
        [TestMethod]
        [TestCategory(TestHelper.TestCategoryUnit)]
        public void CanAddOtherFieldsAndGetValues()
        {
            // Arrange and Act
            ITfsWorkItem sut = new TfsWorkItemDisconnected(1, "dummy", new Tuple<string, object>("A.B", 2), new Tuple<string, object>("C.D", "test"));

            // Act
            int ans1 = (int)sut["A.B"];
            string ans2 = (string)sut["C.D"];

            // Assert
            Assert.AreEqual<int>(2, ans1, "Other field value not stored correctly");
            Assert.AreEqual<string>("test", ans2, "Other field value not stored correctly");
        }
    }
}
