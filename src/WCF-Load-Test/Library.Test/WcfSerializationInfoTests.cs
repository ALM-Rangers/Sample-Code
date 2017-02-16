//---------------------------------------------------------------------
// <copyright file="WcfSerializationInfoTests.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
//     THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
//     OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
//     LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR
//     FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
// <summary>The WcfSerializationInfoTests type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.WcfUnit.Library.Test
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Reflection;
    using System.Xml;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.WcfUnit.Library.Test.TestContracts;

    /// <summary>
    /// Tests the <see cref="WcfSerializationInfo"/> class.
    /// </summary>
    [TestClass]
    public class WcfSerializationInfoTests
    {
        /// <summary>
        /// The object under test.
        /// </summary>
        private WcfSerializationInfoClass sut;

        /// <summary>
        /// Initializes the tests.
        /// </summary>
        [TestInitialize]
        public void InitializeTest()
        {
            this.sut = new WcfSerializationInfoClass();
        }

        /// <summary>
        /// Tests that the <see cref="WcfSerializationInfoClass.Supports"/> method throws exception for null argument.
        /// </summary>
        [TestMethod]
        public void WcfSerSupportsMethodThrowsExceptionForNullArgument()
        {
            TestHelper.TestForArgumentNullException(() => this.sut.Supports(null), "method");
        }

        /// <summary>
        /// Tests that a service contract that has no format attribute and has a method
        /// that does not have a format attribute is supported.
        /// </summary>
        [TestMethod]
        public void WcfSerFormatlessServiceContractWithFormatlessMethodIsSupported()
        {
            // Arrange
            MethodInfo mi = typeof(IFormatlessServiceContract).GetMethod("None");

            // Act
            bool ans = this.sut.Supports(mi);

            // Assert
            Assert.IsTrue(ans);
        }

        /// <summary>
        /// Format-less class with data contract method is supported.
        /// </summary>
        [TestMethod]
        public void WcfSerFormatlessServiceContractWithDataContractMethodIsSupported()
        {
            // Arrange
            MethodInfo mi = typeof(IFormatlessServiceContract).GetMethod("DataContract");

            // Act
            bool ans = this.sut.Supports(mi);

            // Assert
            Assert.IsTrue(ans);
        }

        /// <summary>
        /// Format-less class with xml serializer method is not supported.
        /// </summary>
        [TestMethod]
        public void WcfSerFormatlessServiceContractWithXmlSerializerMethodIsNotSupported()
        {
            // Arrange
            MethodInfo mi = typeof(IFormatlessServiceContract).GetMethod("Xml");

            // Act
            bool ans = this.sut.Supports(mi);

            // Assert
            Assert.IsFalse(ans);
        }

        /// <summary>
        /// Tests that a service contract with a data contract format attribute and which
        /// has a method that does not have a format attribute is supported.
        /// </summary>
        [TestMethod]
        public void WcfSerDataContractServiceContractWithFormatlessMethodIsSupported()
        {
            // Arrange
            MethodInfo mi = typeof(IDataContractFormatServiceContract).GetMethod("None");

            // Act
            bool ans = this.sut.Supports(mi);

            // Assert
            Assert.IsTrue(ans);
        }

        /// <summary>
        /// Tests that a service contract with a data contract format method attribute
        /// and which has a data contract method is supported.
        /// </summary>
        [TestMethod]
        public void WcfSerDataContractServiceContractWithDataContractMethodIsSupported()
        {
            // Arrange
            MethodInfo mi = typeof(IDataContractFormatServiceContract).GetMethod("DataContract");

            // Act
            bool ans = this.sut.Supports(mi);

            // Assert
            Assert.IsTrue(ans);
        }

        /// <summary>
        /// Tests that a service contract with a data contract format method attribute
        /// and which has an xml serializer method is not supported.
        /// </summary>
        [TestMethod]
        public void WcfSerDataContractServiceContractWithXmlSerializerMethodIsNotSupported()
        {
            // Arrange
            MethodInfo mi = typeof(IDataContractFormatServiceContract).GetMethod("Xml");

            // Act
            bool ans = this.sut.Supports(mi);

            // Assert
            Assert.IsFalse(ans);
        }

        /// <summary>
        /// Tests that a service contract with an xml serializer format attribute and which
        /// has a method that does not have a format attribute is not supported.
        /// </summary>
        [TestMethod]
        public void WcfSerXmlSerializerServiceContractWithFormatlessMethodIsNotSupported()
        {
            // Arrange
            MethodInfo mi = typeof(IXmlSerializerFormatServiceContract).GetMethod("None");

            // Act
            bool ans = this.sut.Supports(mi);

            // Assert
            Assert.IsFalse(ans);
        }

        /// <summary>
        /// Tests that a service contract with an xml serializer format method attribute
        /// and which has a data contract method is supported.
        /// </summary>
        [TestMethod]
        public void WcfSerXmlSerializerServiceContractWithDataContractMethodIsSupported()
        {
            // Arrange
            MethodInfo mi = typeof(IXmlSerializerFormatServiceContract).GetMethod("DataContract");

            // Act
            bool ans = this.sut.Supports(mi);

            // Assert
            Assert.IsTrue(ans);
        }

        /// <summary>
        /// Tests that a service contract with an xml serializer format method attribute
        /// and which has an xml serializer method is not supported.
        /// </summary>
        [TestMethod]
        public void WcfSerXmlSerializerServiceContractWithXmlSerializerMethodIsNotSupported()
        {
            // Arrange
            MethodInfo mi = typeof(IFormatlessServiceContract).GetMethod("Xml");

            // Act
            bool ans = this.sut.Supports(mi);

            // Assert
            Assert.IsFalse(ans);
        }

        /// <summary>
        /// Tests that the right error message is returned if a type is not serializable.
        /// </summary>
        [TestMethod]
        public void WcfSerNotSerializableErrorMessageTest()
        {
            // Arrange

            // Act
            string msg = this.sut.NotSerializableError;

            // Assert
            Assert.AreEqual<string>("Data or message contract attribute expected on type {0}", msg);
        }

        /// <summary>
        /// Tests that the <see cref="WcfSerializationInfoClass.IsSimpleType"/> method throws exception for null argument.
        /// </summary>
        [TestMethod]
        public void WcfSerIsSimpleTypeMethodThrowsExceptionForNullArgument()
        {
            TestHelper.TestForArgumentNullException(() => this.sut.IsSimpleType(null), "objectType");
        }

        /// <summary>
        /// Tests that an <see cref="int"/> is a simple type
        /// </summary>
        [TestMethod]
        public void WcfSerIntIsSimpleTypeTest()
        {
            // Arrange

            // Act
            bool ans = this.sut.IsSimpleType(typeof(int));

            // Assert
            Assert.IsTrue(ans);
        }

        /// <summary>
        /// Tests that an enumeration is a simple type
        /// </summary>
        [TestMethod]
        public void WcfSerEnumIsSimpleTypeTest()
        {
            // Arrange

            // Act
            bool ans = this.sut.IsSimpleType(typeof(ConsoleColor));

            // Assert
            Assert.IsTrue(ans);
        }

        /// <summary>
        /// Tests that a string is a simple type
        /// </summary>
        [TestMethod]
        public void WcfSerStringIsSimpleTypeTest()
        {
            // Arrange

            // Act
            bool ans = this.sut.IsSimpleType(typeof(string));

            // Assert
            Assert.IsTrue(ans);
        }

        /// <summary>
        /// Tests that a decimal is a simple type
        /// </summary>
        [TestMethod]
        public void WcfSerDecimalIsSimpleTypeTest()
        {
            // Arrange

            // Act
            bool ans = this.sut.IsSimpleType(typeof(decimal));

            // Assert
            Assert.IsTrue(ans);
        }

        /// <summary>
        /// Tests that a <see cref="DateTime"/> is a simple type
        /// </summary>
        [TestMethod]
        public void WcfSerDateTimeIsSimpleTypeTest()
        {
            // Arrange

            // Act
            bool ans = this.sut.IsSimpleType(typeof(DateTime));

            // Assert
            Assert.IsTrue(ans);
        }

        /// <summary>
        /// Tests that a timespan is a simple type
        /// </summary>
        [TestMethod]
        public void WcfSerTimeSpanIsSimpleTypeTest()
        {
            // Arrange

            // Act
            bool ans = this.sut.IsSimpleType(typeof(TimeSpan));

            // Assert
            Assert.IsTrue(ans);
        }

        /// <summary>
        /// Tests that a <see cref="Guid"/> is a simple type
        /// </summary>
        [TestMethod]
        public void WcfSerGuidIsSimpleTypeTest()
        {
            // Arrange

            // Act
            bool ans = this.sut.IsSimpleType(typeof(Guid));

            // Assert
            Assert.IsTrue(ans);
        }

        /// <summary>
        /// Tests that a uri is a simple type
        /// </summary>
        [TestMethod]
        public void WcfSerUriIsSimpleTypeTest()
        {
            // Arrange

            // Act
            bool ans = this.sut.IsSimpleType(typeof(Uri));

            // Assert
            Assert.IsTrue(ans);
        }

        /// <summary>
        /// Tests that an <see cref="XmlQualifiedName"/> is a simple type
        /// </summary>
        [TestMethod]
        public void WcfSerXmlQualifiedNameIsSimpleTypeTest()
        {
            // Arrange

            // Act
            bool ans = this.sut.IsSimpleType(typeof(XmlQualifiedName));

            // Assert
            Assert.IsTrue(ans);
        }

        /// <summary>
        /// Tests that a nullable generic is a simple type
        /// </summary>
        [TestMethod]
        public void WcfSerNullableIsSimpleTypeTest()
        {
            // Arrange

            // Act
            bool ans = this.sut.IsSimpleType(typeof(Nullable<int>));

            // Assert
            Assert.IsTrue(ans);
        }

        /// <summary>
        /// Tests that a complex type is not a simple type
        /// </summary>
        [TestMethod]
        public void WcfSerComplexTypeIsNotASimpleTypeTest()
        {
            // Arrange

            // Act
            bool ans = this.sut.IsSimpleType(typeof(WcfSerializationInfoClass));

            // Assert
            Assert.IsFalse(ans);
        }

        /// <summary>
        /// Tests that a DataContract class is serializable.
        /// </summary>
        [TestMethod]
        public void WcfSerDataContractClassIsSerializableTest()
        {
            // Arrange

            // Act
            bool ans = this.sut.IsSerializable(typeof(ClassWithSerializableMemberOfNonSerializableType));

            // Assert
            Assert.IsTrue(ans);
        }

        /// <summary>
        /// Tests that a MessageContract class is serializable.
        /// </summary>
        [TestMethod]
        public void WcfSerMessageContractClassIsSerializableTest()
        {
            // Arrange

            // Act
            bool ans = this.sut.IsSerializable(typeof(WithMessageContractButNoneInside));

            // Assert
            Assert.IsTrue(ans);
        }

        /// <summary>
        /// Tests that a class which implements IXmlSerializable is serializable.
        /// </summary>
        [TestMethod]
        public void WcfSerIXmlSerializableClassIsSerializableTest()
        {
            // Arrange

            // Act
            bool ans = this.sut.IsSerializable(typeof(DataSet));

            // Assert
            Assert.IsTrue(ans);
        }

        /// <summary>
        /// Tests that a non data contract and non message contract is not serializable
        /// </summary>
        [TestMethod]
        public void WcfSerNoncontractIsNotSerializableTest()
        {
            // Arrange

            // Act
            bool ans = this.sut.IsSerializable(typeof(NonDataContractClass));

            // Assert
            Assert.IsFalse(ans);
        }

        /// <summary>
        /// Tests that the <see cref="WcfSerializationInfoClass.SerializableMembers"/> method throws exception for null argument.
        /// </summary>
        [TestMethod]
        public void WcfSerSerializableMembersMethodThrowsExceptionForNullArgument()
        {
            TestHelper.TestForArgumentNullException(() => this.sut.SerializableMembers(null), "objectType");
        }

        /// <summary>
        /// Tests that the right serializable members are retrieved for a data contract.
        /// </summary>
        [TestMethod]
        public void WcfSerDataContractSerializableMembersTest()
        {
            // Arrange
            MemberInfo mip1 = typeof(SimpleCompoundTypeDataContract).GetMember("Property1")[0];
            MemberInfo mip2 = typeof(SimpleCompoundTypeDataContract).GetMember("Property2")[0];
            MemberInfo mif1 = typeof(SimpleCompoundTypeDataContract).GetMember("Field1")[0];
            MemberInfo mif2 = typeof(SimpleCompoundTypeDataContract).GetMember("Field2")[0];

            // Act
            List<MemberInfo> members = new List<MemberInfo>(this.sut.SerializableMembers(typeof(SimpleCompoundTypeDataContract)));

            // Assert
            Assert.AreEqual<int>(4, members.Count);
            Assert.IsTrue(members.Contains(mip1));
            Assert.IsTrue(members.Contains(mip2));
            Assert.IsTrue(members.Contains(mif1));
            Assert.IsTrue(members.Contains(mif2));
        }

        /// <summary>
        /// Tests that right serializable members are retrieved for a message contract.
        /// </summary>
        [TestMethod]
        public void WcfSerMessageContractSerializableMembersTest()
        {
            // Arrange
            MemberInfo mip1 = typeof(SimpleCompoundTypeMessageContract).GetMember("Property1")[0];
            MemberInfo mip2 = typeof(SimpleCompoundTypeMessageContract).GetMember("Property2")[0];
            MemberInfo mif1 = typeof(SimpleCompoundTypeMessageContract).GetMember("Field1")[0];
            MemberInfo mif2 = typeof(SimpleCompoundTypeMessageContract).GetMember("Field2")[0];

            // Act
            List<MemberInfo> members = new List<MemberInfo>(this.sut.SerializableMembers(typeof(SimpleCompoundTypeMessageContract)));

            // Assert
            Assert.AreEqual<int>(4, members.Count);
            Assert.IsTrue(members.Contains(mip1));
            Assert.IsTrue(members.Contains(mip2));
            Assert.IsTrue(members.Contains(mif1));
            Assert.IsTrue(members.Contains(mif2));
        }
    }
}
