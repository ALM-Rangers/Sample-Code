//---------------------------------------------------------------------
// <copyright file="TfsWorkItemTests.cs" company="Microsoft">
//    Copyright (c) Microsoft. All rights reserved.
//    This code is licensed under the MIT License.
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF 
//    ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
//    TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
//    PARTICULAR PURPOSE AND NONINFRINGEMENT.
// </copyright>
// <summary>The TfsWorkItemTests type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.Word4Tfs.Library.Test.UnitTest.Model
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Word4Tfs.Library.Model.TeamFoundation;
    using Moq;

    /// <summary>
    /// Tests the <see cref="TfsWorkItem"/> class. Only the unit testable aspects are tested here.
    /// </summary>
    [TestClass]
    public class TfsWorkItemTests
    {
        /// <summary>
        /// Serialize throws exception for null argument.
        /// </summary>
        [TestMethod]
        public void SerializeThrowsExceptionForNullArgument()
        {
            TestHelper.TestForArgumentNullException(() => TfsWorkItem.Serialize(null), "workItem");
        }

        /// <summary>
        /// Serializes the id of the work item.
        /// </summary>
        [TestMethod]
        public void SerializesTheIdOfTheWorkItemByReferenceName()
        {
            // Arrange
            Mock<ITfsWorkItem> mockItem = CreateMockWorkItem();
            AddFieldToMockItem(mockItem, Constants.SystemIdFieldReferenceName, 1);

            // Act
            XElement ans = TfsWorkItem.Serialize(mockItem.Object);

            // Assert
            TestHelper.AssertXmlNodeContent("1", ans, "/wi:WorkItem/wi:Field[@name='" + Constants.SystemIdFieldReferenceName + "']", "Work item id not serialized correctly or missing");
        }

        /// <summary>
        /// Serializes the id of the work item.
        /// </summary>
        [TestMethod]
        public void SerializesTheTypeOfTheWorkItemByReferenceName()
        {
            // Arrange
            Mock<ITfsWorkItem> mockItem = CreateMockWorkItem();
            AddFieldToMockItem(mockItem, Constants.SystemWorkItemTypeFieldReferenceName, "wit");

            // Act
            XElement ans = TfsWorkItem.Serialize(mockItem.Object);

            // Assert
            TestHelper.AssertXmlNodeContent("wit", ans, "/wi:WorkItem/wi:Field[@name='System.WorkItemType']", "Work item type not serialized correctly or missing");
        }

        /// <summary>
        /// Serializes other fields as requested by the display field list.
        /// </summary>
        [TestMethod]
        public void SerializesOtherFieldsAsRequested()
        {
            // Arrange
            Mock<ITfsWorkItem> mockItem = CreateMockWorkItem();
            AddFieldToMockItem(mockItem, "A.B", "ab");
            AddFieldToMockItem(mockItem, "A.C", "ac");

            // Act
            XElement ans = TfsWorkItem.Serialize(mockItem.Object, "A.B", "A.C");

            // Assert
            TestHelper.AssertXmlNodeContent("ab", ans, "/wi:WorkItem/wi:Field[@name='A.B']", "Work item field not serialized correctly or missing");
            TestHelper.AssertXmlNodeContent("ac", ans, "/wi:WorkItem/wi:Field[@name='A.C']", "Work item field not serialized correctly or missing");
        }

        /// <summary>
        /// Does not serializes fields requested by the display field list but which are not in the work item.
        /// </summary>
        [TestMethod]
        public void DoesNotSerializeRequestedFieldsNotInTheWorkItem()
        {
            // Arrange
            Mock<ITfsWorkItem> mockItem = CreateMockWorkItem();
            AddFieldToMockItem(mockItem, "A.B", "ab");
            AddFieldToMockItem(mockItem, "A.C", "ac");
            mockItem.Setup(wi => wi["A.D"]).Throws(new Exception("should not try to access this field"));

            // Act
            XElement ans = TfsWorkItem.Serialize(mockItem.Object, "A.B", "A.C", "A.D");

            // Assert
            TestHelper.AssertXmlNodeMissing(ans, "/wi:WorkItem/wi:A.D", "Work item field should not be serialized");
        }

        /// <summary>
        /// Creates a mock work item.
        /// </summary>
        /// <returns>The mock work item.</returns>
        private static Mock<ITfsWorkItem> CreateMockWorkItem()
        {
            Mock<ITfsWorkItem> mockItem = new Mock<ITfsWorkItem>();
            mockItem.Setup(wi => wi.FieldReferenceNames).Returns(new List<string>());
            return mockItem;
        }

        /// <summary>
        /// Adds a field to a mock work item.
        /// </summary>
        /// <param name="mockItem">The mock item to add the field to.</param>
        /// <param name="name">The name of the field.</param>
        /// <param name="value">The value of the field.</param>
        private static void AddFieldToMockItem(Mock<ITfsWorkItem> mockItem, string name, object value)
        {
            mockItem.Setup(wi => wi[name]).Returns(value);
            List<string> fields = (List<string>)mockItem.Object.FieldReferenceNames;
            fields.Add(name);
        }
    }
}
