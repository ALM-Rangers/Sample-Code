//---------------------------------------------------------------------
// <copyright file="AsmxSerializationInfoTests.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
//     THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
//     OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
//     LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR
//     FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
// <summary>The AsmxSerializationInfoTests type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.WcfUnit.Library.Test
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Xml;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.WcfUnit.Library.Test.TestContracts;

    /// <summary>
    /// Tests the <see cref="AsmxSerializationInfo"/> class.
    /// </summary>
    [TestClass]
    public class AsmxSerializationInfoTests
    {
        private AsmxSerializationInfoClass sut;

        [TestInitialize]
        public void InitializeTest()
        {
            this.sut = new AsmxSerializationInfoClass();
        }

        /// <summary>
        /// Tests that the <see cref="AsmxSerializationInfoClass.Supports"/> method throws exception for null argument.
        /// </summary>
        [TestMethod]
        public void ASISupportsMethodThrowsExceptionForNullArgument()
        {
            TestHelper.TestForArgumentNullException(() => this.sut.Supports(null), "method");
        }

        /// <summary>
        /// Tests that a service contract that has no format attribute and has a method
        /// that does not have a format attribute is not supported.
        /// </summary>
        [TestMethod]
        public void ASIFormatlessServiceContractWithFormatlessMethodIsNotSupported()
        {
            // Arrange
            MethodInfo mi = typeof(IFormatlessServiceContract).GetMethod("None");

            // Act
            bool ans = this.sut.Supports(mi);

            // Assert
            Assert.IsFalse(ans);
        }

        /// <summary>
        /// Format-less class with data contract method is not supported.
        /// </summary>
        [TestMethod]
        public void ASIFormatlessServiceContractWithDataContractMethodIsNotSupported()
        {
            // Arrange
            MethodInfo mi = typeof(IFormatlessServiceContract).GetMethod("DataContract");

            // Act
            bool ans = this.sut.Supports(mi);

            // Assert
            Assert.IsFalse(ans);
        }

        /// <summary>
        /// Format-less class with xml serializer method is supported.
        /// </summary>
        [TestMethod]
        public void ASIFormatlessServiceContractWithXmlSerializerMethodIsSupported()
        {
            // Arrange
            MethodInfo mi = typeof(IFormatlessServiceContract).GetMethod("Xml");

            // Act
            bool ans = this.sut.Supports(mi);

            // Assert
            Assert.IsTrue(ans);
        }

        /// <summary>
        /// Tests that a service contract with a data contract format attribute and which
        /// has a method that does not have a format attribute is not supported.
        /// </summary>
        [TestMethod]
        public void ASIDataContractServiceContractWithFormatlessMethodIsNotSupported()
        {
            // Arrange
            MethodInfo mi = typeof(IDataContractFormatServiceContract).GetMethod("None");

            // Act
            bool ans = this.sut.Supports(mi);

            // Assert
            Assert.IsFalse(ans);
        }

        /// <summary>
        /// Tests that a service contract with a data contract format method attribute
        /// and which has a data contract method is not supported.
        /// </summary>
        [TestMethod]
        public void ASIDataContractServiceContractWithDataContractMethodIsNotSupported()
        {
            // Arrange
            MethodInfo mi = typeof(IDataContractFormatServiceContract).GetMethod("DataContract");

            // Act
            bool ans = this.sut.Supports(mi);

            // Assert
            Assert.IsFalse(ans);
        }

        /// <summary>
        /// Tests that a service contract with a data contract format method attribute
        /// and which has an xml serializer method is supported.
        /// </summary>
        [TestMethod]
        public void ASIDataContractServiceContractWithXmlSerializerMethodIsSupported()
        {
            // Arrange
            MethodInfo mi = typeof(IDataContractFormatServiceContract).GetMethod("Xml");

            // Act
            bool ans = this.sut.Supports(mi);

            // Assert
            Assert.IsTrue(ans);
        }

        /// <summary>
        /// Tests that a service contract with an xml serializer format attribute and which
        /// has a method that does not have a format attribute is supported.
        /// </summary>
        [TestMethod]
        public void ASIXmlSerializerServiceContractWithFormatlessMethodIsSupported()
        {
            // Arrange
            MethodInfo mi = typeof(IXmlSerializerFormatServiceContract).GetMethod("None");

            // Act
            bool ans = this.sut.Supports(mi);

            // Assert
            Assert.IsTrue(ans);
        }

        /// <summary>
        /// Tests that a service contract with an xml serializer format method attribute
        /// and which has a data contract method is not supported.
        /// </summary>
        [TestMethod]
        public void ASIXmlSerializerServiceContractWithDataContractMethodIsNotSupported()
        {
            // Arrange
            MethodInfo mi = typeof(IXmlSerializerFormatServiceContract).GetMethod("DataContract");

            // Act
            bool ans = this.sut.Supports(mi);

            // Assert
            Assert.IsFalse(ans);
        }

        /// <summary>
        /// Tests that a service contract with an xml serializer format method attribute
        /// and which has an xml serializer method is supported.
        /// </summary>
        [TestMethod]
        public void ASIXmlSerializerServiceContractWithXmlSerializerMethodIsSupported()
        {
            // Arrange
            MethodInfo mi = typeof(IFormatlessServiceContract).GetMethod("Xml");

            // Act
            bool ans = this.sut.Supports(mi);

            // Assert
            Assert.IsTrue(ans);
        }

        /// <summary>
        /// Tests that the right error message is returned if a type is not serializable.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "msg", Justification = "Want to ensure property getter is executed")]
        [TestMethod]
        public void ASINotSerializableErrorMessageTest()
        {
            TestHelper.TestForNotSupportedException(
                delegate
                {
                    string msg = this.sut.NotSerializableError;
                });
        }

        /// <summary>
        /// Tests that the <see cref="AsmxSerializationInfoClass.IsSimpleType"/> method throws exception for null argument.
        /// </summary>
        [TestMethod]
        public void ASIIsSimpleTypeMethodThrowsExceptionForNullArgument()
        {
            TestHelper.TestForArgumentNullException(() => this.sut.IsSimpleType(null), "objectType");
        }

        /// <summary>
        /// Tests that an <see cref="int"/> is a simple type
        /// </summary>
        [TestMethod]
        public void ASIIntIsSimpleTypeTest()
        {
            // Arrange

            // Act
            bool ans = this.sut.IsSimpleType(typeof(int));

            // Assert
            Assert.IsTrue(ans);
        }

        /// <summary>
        /// Tests that an <see cref="Enum"/> is a simple type
        /// </summary>
        [TestMethod]
        public void ASIEnumIsSimpleTypeTest()
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
        public void ASIStringIsSimpleTypeTest()
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
        public void ASIDecimalIsSimpleTypeTest()
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
        public void ASIDateTimeIsSimpleTypeTest()
        {
            // Arrange

            // Act
            bool ans = this.sut.IsSimpleType(typeof(DateTime));

            // Assert
            Assert.IsTrue(ans);
        }

        /// <summary>
        /// Tests that a timespan is not a simple type
        /// </summary>
        [TestMethod]
        public void ASITimeSpanIsNotSimpleTypeTest()
        {
            // Arrange

            // Act
            bool ans = this.sut.IsSimpleType(typeof(TimeSpan));

            // Assert
            Assert.IsFalse(ans);
        }

        /// <summary>
        /// Tests that a <see cref="Guid"/> is a simple type
        /// </summary>
        [TestMethod]
        public void ASIGuidIsSimpleTypeTest()
        {
            // Arrange

            // Act
            bool ans = this.sut.IsSimpleType(typeof(Guid));

            // Assert
            Assert.IsTrue(ans);
        }

        /// <summary>
        /// Tests that a uri is not a simple type
        /// </summary>
        [TestMethod]
        public void ASIUriIsNotSimpleTypeTest()
        {
            // Arrange

            // Act
            bool ans = this.sut.IsSimpleType(typeof(Uri));

            // Assert
            Assert.IsFalse(ans);
        }

        /// <summary>
        /// Tests that an <see cref="XmlQualifiedName"/> is a simple type
        /// </summary>
        [TestMethod]
        public void ASIXmlQualifiedNameIsSimpleTypeTest()
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
        public void ASINullableIsSimpleTypeTest()
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
        public void ASIComplexTypeIsNotASimpleTypeTest()
        {
            // Arrange

            // Act
            bool ans = this.sut.IsSimpleType(typeof(AsmxSerializationInfoClass));

            // Assert
            Assert.IsFalse(ans);
        }

        /// <summary>
        /// Tests that any type is serializable.
        /// </summary>
        [TestMethod]
        public void ASIAnyClassIsSerializableTest()
        {
            // Arrange

            // Act
            bool ans = this.sut.IsSerializable(typeof(NonDataContractClass));

            // Assert
            Assert.IsTrue(ans);
        }

        /// <summary>
        /// Tests that the <see cref="AsmxSerializationInfoClass.SerializableMembers"/> method throws exception for null argument.
        /// </summary>
        [TestMethod]
        public void ASISerializableMembersMethodThrowsExceptionForNullArgument()
        {
            TestHelper.TestForArgumentNullException(() => this.sut.SerializableMembers(null), "objectType");
        }

        /// <summary>
        /// Tests that a read/write property is serializable.
        /// </summary>
        [TestMethod]
        public void ASISerializableMembersTest()
        {
            // Arrange
            MemberInfo mip1 = typeof(SimpleCompoundTypeDataContract).GetMember("Property1")[0];
            MemberInfo mip2 = typeof(SimpleCompoundTypeDataContract).GetMember("Property2")[0];
            MemberInfo mif1 = typeof(SimpleCompoundTypeDataContract).GetMember("Field1")[0];
            MemberInfo mif2 = typeof(SimpleCompoundTypeDataContract).GetMember("Field2")[0];
            MemberInfo nonmember = typeof(SimpleCompoundTypeDataContract).GetMember("Nonmember")[0];

            // Act
            List<MemberInfo> members = new List<MemberInfo>(this.sut.SerializableMembers(typeof(SimpleCompoundTypeDataContract)));

            // Assert
            Assert.AreEqual<int>(5, members.Count);
            Assert.IsTrue(members.Contains(mip1));
            Assert.IsTrue(members.Contains(mip2));
            Assert.IsTrue(members.Contains(mif1));
            Assert.IsTrue(members.Contains(mif2));
            Assert.IsTrue(members.Contains(nonmember));
        }

        /// <summary>
        /// Tests that a read/write property is serializable.
        /// </summary>
        [TestMethod]
        public void ASIReadWritePropertyIsSerializableTest()
        {
            // Arrange
            MemberInfo mi = typeof(SimpleCompoundTypeDataContract).GetMember("Property1")[0];

            // Act
            List<MemberInfo> members = new List<MemberInfo>(this.sut.SerializableMembers(typeof(SimpleCompoundTypeDataContract)));

            // Assert
            Assert.IsTrue(members.Contains(mi));
        }

        /// <summary>
        /// Tests that a read-only property is not serializable.
        /// </summary>
        [TestMethod]
        public void ASIReadOnlyPropertyIsNotSerializableTest()
        {
            // Arrange
            MemberInfo mi = typeof(SimpleCompoundTypeDataContract).GetMember("Property3")[0];

            // Act
            List<MemberInfo> members = new List<MemberInfo>(this.sut.SerializableMembers(typeof(SimpleCompoundTypeDataContract)));

            // Assert
            Assert.IsFalse(members.Contains(mi));
        }

        /// <summary>
        /// Tests that a field is serializable.
        /// </summary>
        [TestMethod]
        public void ASIFieldIsSerializableTest()
        {
            // Arrange
            MemberInfo mi = typeof(SimpleCompoundTypeDataContract).GetMember("Field1")[0];

            // Act
            List<MemberInfo> members = new List<MemberInfo>(this.sut.SerializableMembers(typeof(SimpleCompoundTypeDataContract)));

            // Assert
            Assert.IsTrue(members.Contains(mi));
        }

        /// <summary>
        /// Tests that a member marked with XmlIgnore is not serializable.
        /// </summary>
        [TestMethod]
        public void ASIXmlIgnoreMemberIsNotSerializableTest()
        {
            // Arrange
            MemberInfo mi = typeof(AsmxIgnoreFields).GetMember("AlwaysIgnored")[0];

            // Act
            List<MemberInfo> members = new List<MemberInfo>(this.sut.SerializableMembers(typeof(AsmxIgnoreFields)));

            // Assert
            Assert.IsFalse(members.Contains(mi));
        }

        /// <summary>
        /// Tests that a member marked with XmlIgnore on a base class is not serializable.
        /// </summary>
        [TestMethod]
        public void ASIXmlIgnoreMemberOnBaseClassIsNotSerializableTest()
        {
            // Arrange
            MemberInfo mi = typeof(AsmxIgnoreFieldsDerived).GetMember("AlwaysIgnored")[0];

            // Act
            List<MemberInfo> members = new List<MemberInfo>(this.sut.SerializableMembers(typeof(AsmxIgnoreFieldsDerived)));

            // Assert
            Assert.IsFalse(members.Contains(mi));
        }
        
        /// <summary>
        /// Tests that a non-overridden virtual member marked with XmlIgnore on a base class is not serializable.
        /// </summary>
        [TestMethod]
        public void ASIXmlIgnoreNonOverriddenVirtualMemberOnBaseClassIsNotSerializableTest()
        {
            // Arrange
            MemberInfo mi = typeof(AsmxIgnoreFieldsDerived).GetMember("IgnoredBase")[0];

            // Act
            List<MemberInfo> members = new List<MemberInfo>(this.sut.SerializableMembers(typeof(AsmxIgnoreFieldsDerived)));

            // Assert
            Assert.IsFalse(members.Contains(mi));
        }

        /// <summary>
        /// Tests that an overridden virtual member marked without XmlIgnore in the derived class is serializable.
        /// </summary>
        [TestMethod]
        public void ASIXmlIgnoreAbsentOnOverriddenVirtualMemberInDerivedClassIsSerializableTest()
        {
            // Arrange
            MemberInfo mi = typeof(AsmxIgnoreFieldsDerived).GetMember("IgnoredBaseOverridden")[0];

            // Act
            List<MemberInfo> members = new List<MemberInfo>(this.sut.SerializableMembers(typeof(AsmxIgnoreFieldsDerived)));

            // Assert
            Assert.IsTrue(members.Contains(mi));
        }

        /// <summary>
        /// Tests that a member marked with XmlIgnore is  serializable if it is the "Specified" value for another field.
        /// </summary>
        [TestMethod]
        public void ASIXmlIgnoreForOptionalFieldIsSerializableTest()
        {
            // Arrange
            MemberInfo mi = typeof(AsmxIgnoreFields).GetMember("OptionalFieldSpecified")[0];

            // Act
            List<MemberInfo> members = new List<MemberInfo>(this.sut.SerializableMembers(typeof(AsmxIgnoreFields)));

            // Assert
            Assert.IsTrue(members.Contains(mi));
        }

        /// <summary>
        /// Tests that a member marked with XmlIgnore is serializable if it is the "Specified" value for another property.
        /// </summary>
        [TestMethod]
        public void ASIXmlIgnoreForOptionalPropertyIsSerializableTest()
        {
            // Arrange
            MemberInfo mi = typeof(AsmxIgnoreFields).GetMember("OptionalPropertySpecified")[0];

            // Act
            List<MemberInfo> members = new List<MemberInfo>(this.sut.SerializableMembers(typeof(AsmxIgnoreFields)));

            // Assert
            Assert.IsTrue(members.Contains(mi));
        }

        /// <summary>
        /// Tests that a member marked with XmlIgnore is not serializable if it has the "Specified" suffix but there is no field or property it corresponds to.
        /// </summary>
        [TestMethod]
        public void ASIXmlIgnoreOnSpecifiedFieldWithNoCorrespondingOptionalMemberIsNotSerializableTest()
        {
            // Arrange
            MemberInfo mi = typeof(AsmxIgnoreFields).GetMember("NonOptionalFieldSpecified")[0];

            // Act
            List<MemberInfo> members = new List<MemberInfo>(this.sut.SerializableMembers(typeof(AsmxIgnoreFields)));

            // Assert
            Assert.IsFalse(members.Contains(mi));
        }
    }
}
