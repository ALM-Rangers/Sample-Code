//---------------------------------------------------------------------
// <copyright file="ObjectGeneratorTests.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
//     THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
//     OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
//     LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR
//     FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
// <summary>The ObjectGeneratorTests type.</summary>
//---------------------------------------------------------------------

// TODO: Check the CollectionDataContractAttribute as this may also affect how types are deserialized.
// TODO: See if can remove restriction on reformatting trace file when there are XML types in there (newlines cause XML to be broken into different nodes).

namespace Microsoft.WcfUnit.Library.Test
{
    using System;
    using System.CodeDom;
    using System.CodeDom.Compiler;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data;
    using System.Globalization;
    using System.IO;
    using System.Reflection;
    using System.Text;
    using System.Xml;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.WcfUnit.Library.Test.TestContracts;
    using Rhino.Mocks;

    /// <summary>
    /// General tests that are not really specific to the type of web service technology.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "This class is testing the generation for many different classes.")]
    [TestClass]
    public class ObjectGeneratorTests
    {
        private CodeCompileUnit ccu;
        private CodeNamespaceImportCollection imports;
        private CodeStatementCollection methodBody;
        private ObjectGenerator sut;
        private bool useFullQualification = false;

        /// <summary>
        /// The mock repository used to set up and then verify expected behaviour.
        /// </summary>
        private MockRepository mocks;

        [TestInitialize]
        public void SetupTest()
        {
            this.mocks = new MockRepository();
        }

        /// <summary>
        /// Cleans up after each test and verifies behaviour in the mocks.
        /// </summary>
        [TestCleanup]
        public void CleanupTest()
        {
            this.mocks.VerifyAll();
        }

        #region Type mismatches

        [TestMethod]
        public void OGObjectTypeMismatch()
        {
            // Arrange
            this.SetupStandardTypesTest();

            // Act and Assert
            TestHelper.TestForInvalidOperationException(
                delegate
                {
                    this.sut.GenerateObject("a", typeof(string), 2);
                });
        }

        [TestMethod]
        public void OGGenericObjectTypeMismatch()
        {
            // Arrange
            this.SetupStandardTypesTest();

            // Act and Assert
            TestHelper.TestForInvalidOperationException(
                delegate
                {
                    this.sut.GenerateObject("a", typeof(Nullable<bool>), 2);
                });
        }

        #endregion

        #region Non-serializable types and members

        [TestMethod]
        public void OGTypeNotSerializableThrowsUserException()
        {
            // Arrange
            this.SetupStandardTypesTest();

            NonDataContractClass a = new NonDataContractClass();

            // Act and Assert
            TestHelper.TestForUserException(
                delegate
                {
                    this.sut.GenerateObject("a", typeof(NonDataContractClass), a);
                });
        }

        [TestMethod]
        public void OGTypeWithNonSerializableMemberThrowsUserException()
        {
            // Arrange
            this.SetupStandardTypesTest();

            ClassWithSerializableMemberOfNonSerializableType a = new ClassWithSerializableMemberOfNonSerializableType();
            a.NonSerializableTypeMember = new NonDataContractClass();

            // Act and Assert
            TestHelper.TestForUserException(
                delegate
                {
                    this.sut.GenerateObject("a", typeof(ClassWithSerializableMemberOfNonSerializableType), a);
                });
        }

        #endregion

        #region Other error situations

        [TestMethod]
        public void OGCyclicStructure()
        {
            // Arrange
            this.SetupStandardTypesTest();

            CyclicStructure a = new CyclicStructure();
            a.BackRef = a;

            // Act and assert
            TestHelper.TestForUserException(
                delegate
                {
                    this.sut.GenerateObject("a", typeof(CyclicStructure), a);
                });
        }

        #endregion

        #region Simple types

        [TestMethod]
        public void OGEnum()
        {
            // Arrange
            this.SetupStandardTypesTest();

            // Act
            CodeVariableReferenceExpression var = this.sut.GenerateObject("a", typeof(TestEnum), TestEnum.Two);

            // Assert
            Assert.AreEqual<string>("a", var.VariableName);

            CompareCode(this.ccu, @"//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.1433
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Microsoft.WcfUnit.Library.Test.TestContracts;



public class Test
{
    
    public void TestMethod()
    {
        TestEnum a = TestEnum.Two;
    }
}
");
        }

        [TestMethod]
        public void OGInt()
        {
            // Arrange
            this.SetupStandardTypesTest();

            // Act
            CodeVariableReferenceExpression var = this.sut.GenerateObject("a", typeof(int), 2);

            // Assert
            Assert.AreEqual<string>("a", var.VariableName);

            CompareCode(this.ccu, @"//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.1433
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;



public class Test
{
    
    public void TestMethod()
    {
        Int32 a = 2;
    }
}
");
        }

        [TestMethod]
        public void OGString()
        {
            // Arrange
            this.SetupStandardTypesTest();

            // Act
            CodeVariableReferenceExpression var = this.sut.GenerateObject("a", typeof(string), "my string with quotes \"");

            // Assert
            Assert.AreEqual<string>("a", var.VariableName);
            CompareCode(this.ccu, @"//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.1433
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;



public class Test
{
    
    public void TestMethod()
    {
        String a = ""my string with quotes \"""";
    }
}
");
        }

        [TestMethod]
        public void OGNullString()
        {
            // Arrange
            this.SetupStandardTypesTest();

            // Act
            CodeVariableReferenceExpression var = this.sut.GenerateObject("a", typeof(string), null);

            // Assert
            Assert.AreEqual<string>("a", var.VariableName);
            CompareCode(this.ccu, @"//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.1433
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;



public class Test
{
    
    public void TestMethod()
    {
        String a = null;
    }
}
");
        }

        [TestMethod]
        public void OGDecimal()
        {
            // Arrange
            this.SetupStandardTypesTest();

            // Act
            CodeVariableReferenceExpression var = this.sut.GenerateObject("a", typeof(decimal), 1.23m);

            // Assert
            Assert.AreEqual<string>("a", var.VariableName);
            CompareCode(this.ccu, @"//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.1433
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;



public class Test
{
    
    public void TestMethod()
    {
        Decimal a = 1.23m;
    }
}
");
        }

        [TestMethod]
        public void OGDateTime()
        {
            // Arrange
            this.SetupStandardTypesTest();

            // Act
            CodeVariableReferenceExpression var = this.sut.GenerateObject("a", typeof(DateTime), new DateTime(2007, 2, 26));

            // Assert
            Assert.AreEqual<string>("a", var.VariableName);
            CompareCode(this.ccu, @"//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.1433
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;



public class Test
{
    
    public void TestMethod()
    {
        DateTime a = new DateTime(2007, 2, 26, 0, 0, 0, 0);
    }
}
");
        }

        [TestMethod]
        public void OGTimeSpan()
        {
            // Assert
            this.SetupStandardTypesTest();

            // Act
            CodeVariableReferenceExpression var = this.sut.GenerateObject("a", typeof(TimeSpan), new TimeSpan(1, 2, 3, 4, 5));

            // Assert
            Assert.AreEqual<string>("a", var.VariableName);

            CompareCode(this.ccu, @"//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.1433
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;



public class Test
{
    
    public void TestMethod()
    {
        TimeSpan a = new TimeSpan(1, 2, 3, 4, 5);
    }
}
");
        }

        [TestMethod]
        public void OGGuid()
        {
            // Arrange
            this.SetupStandardTypesTest();
            Guid a = new Guid("A2BF47AC-3431-4f60-B572-94E91D6D8E0E");

            // Act
            CodeVariableReferenceExpression var = this.sut.GenerateObject("a", typeof(Guid), a);

            // Assert
            Assert.AreEqual<string>("a", var.VariableName);
            CompareCode(this.ccu, @"//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.1433
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;



public class Test
{
    
    public void TestMethod()
    {
        Guid a = new Guid(""a2bf47ac-3431-4f60-b572-94e91d6d8e0e"");
    }
}
");
        }

        #endregion

        #region Nullable types

        [TestMethod]
        public void OGNullableIntWithValue()
        {
            // Arrange
            this.SetupStandardTypesTest();
            Nullable<int> a = 1;

            // Act
            CodeVariableReferenceExpression var = this.sut.GenerateObject("a", typeof(Nullable<int>), a);

            // Assert
            Assert.AreEqual<string>("a", var.VariableName);

            CompareCode(this.ccu, @"//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.1433
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;



public class Test
{
    
    public void TestMethod()
    {
        Nullable<Int32> a = 1;
    }
}
");
        }

        [TestMethod]
        public void OGNullableIntWithNullValue()
        {
            // Arrange
            this.SetupStandardTypesTest();
            Nullable<int> a = null;

            // Act
            CodeVariableReferenceExpression var = this.sut.GenerateObject("a", typeof(Nullable<int>), a);

            // Assert
            Assert.AreEqual<string>("a", var.VariableName);

            CompareCode(this.ccu, @"//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.1433
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;



public class Test
{
    
    public void TestMethod()
    {
        Nullable<Int32> a = null;
    }
}
");
        }

        [TestMethod]
        public void OGNullableDateTimeWithValue()
        {
            // Arrange
            this.SetupStandardTypesTest();
            Nullable<DateTime> a = new DateTime(2007, 2, 26);

            // Act
            CodeVariableReferenceExpression var = this.sut.GenerateObject("a", typeof(Nullable<DateTime>), a);

            // Assert
            Assert.AreEqual<string>("a", var.VariableName);
            CompareCode(this.ccu, @"//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.1433
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;



public class Test
{
    
    public void TestMethod()
    {
        Nullable<DateTime> a = new DateTime(2007, 2, 26, 0, 0, 0, 0);
    }
}
");
        }

        [TestMethod]
        public void OGNullableCustomValueTypeWithNullValue()
        {
            // Arrange
            this.SetupStandardTypesTest();

            Nullable<CustomValueType> a = null;

            // Act
            CodeVariableReferenceExpression var = this.sut.GenerateObject("a", typeof(Nullable<CustomValueType>), a);

            // Assert
            Assert.AreEqual<string>("a", var.VariableName);

            CompareCode(this.ccu, @"//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.1433
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Microsoft.WcfUnit.Library.Test.TestContracts;
using System;



public class Test
{
    
    public void TestMethod()
    {
        Nullable<CustomValueType> a = null;
    }
}
");
        }

        [TestMethod]
        public void OGNullableCustomValueTypeWithValue()
        {
            // Arrange
            this.SetupStandardTypesTest();

            CustomValueType aValue = new CustomValueType();
            aValue.A = 1;
            aValue.B = 2;
            Nullable<CustomValueType> a = aValue;

            // Act
            CodeVariableReferenceExpression var = this.sut.GenerateObject("a", typeof(Nullable<CustomValueType>), a);

            // Assert
            Assert.AreEqual<string>("a", var.VariableName);

            CompareCode(this.ccu, @"//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.1433
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Microsoft.WcfUnit.Library.Test.TestContracts;
using System;



public class Test
{
    
    public void TestMethod()
    {
        CustomValueType temp0 = new CustomValueType();
        temp0.A = 1;
        temp0.B = 2;
        Nullable<CustomValueType> a = temp0;
    }
}
");
        }

        [TestMethod]
        public void OGNullableCustomValueTypeWithValueFullyQualified()
        {
            // Arrange
            this.SetupStandardTypesTest();
            this.useFullQualification = true;
            CustomValueType aValue = new CustomValueType();

            aValue.A = 1;
            aValue.B = 2;
            Nullable<CustomValueType> a = aValue;

            // Act
            CodeVariableReferenceExpression var = this.sut.GenerateObject("a", typeof(Nullable<CustomValueType>), a);

            // Assert
            Assert.AreEqual<string>("a", var.VariableName);

            CompareCode(this.ccu, @"//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.1433
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------



public class Test
{
    
    public void TestMethod()
    {
        Microsoft.WcfUnit.Library.Test.TestContracts.CustomValueType temp0 = new Microsoft.WcfUnit.Library.Test.TestContracts.CustomValueType();
        temp0.A = 1;
        temp0.B = 2;
        System.Nullable<Microsoft.WcfUnit.Library.Test.TestContracts.CustomValueType> a = temp0;
    }
}
");
        }

        #endregion

        #region Uri type

        [TestMethod]
        public void OGUriStringConstructor()
        {
            // Arrange
            this.SetupStandardTypesTest();
            Uri a = new Uri("urn:abc");

            // Act
            CodeVariableReferenceExpression var = this.sut.GenerateObject("a", typeof(Uri), a);

            // Assert
            Assert.AreEqual<string>("a", var.VariableName);

            CompareCode(this.ccu, @"//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.1433
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;



public class Test
{
    
    public void TestMethod()
    {
        Uri a = new Uri(""urn:abc"");
    }
}
");
        }

        [TestMethod]
        public void OGUriStringAndKindConstructor()
        {
            // Arrange
            this.SetupStandardTypesTest();

            Uri a = new Uri("relative", UriKind.Relative);

            // Act
            CodeVariableReferenceExpression var = this.sut.GenerateObject("a", typeof(Uri), a);

            // Assert
            Assert.AreEqual<string>("a", var.VariableName);

            CompareCode(this.ccu, @"//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.1433
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;



public class Test
{
    
    public void TestMethod()
    {
        Uri a = new Uri(""relative"", UriKind.Relative);
    }
}
");
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2234:PassSystemUriObjectsInsteadOfStrings", Justification = "Not necessary for test code")]
        [TestMethod]
        public void OGUriBaseAndStringRelativeConstructor()
        {
            // Arrange
            this.SetupStandardTypesTest();

            Uri a = new Uri(new Uri("http://base"), "/relative");

            // Act
            CodeVariableReferenceExpression var = this.sut.GenerateObject("a", typeof(Uri), a);

            // Assert
            Assert.AreEqual<string>("a", var.VariableName);

            CompareCode(this.ccu, @"//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.1433
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;



public class Test
{
    
    public void TestMethod()
    {
        Uri a = new Uri(""http://base/relative"");
    }
}
");
        }

        [TestMethod]
        public void OGUriBaseAndUriRelativeConstructor()
        {
            // Arrange
            this.SetupStandardTypesTest();

            Uri a = new Uri(new Uri("http://base"), new Uri("/relative", UriKind.Relative));

            // Act
            CodeVariableReferenceExpression var = this.sut.GenerateObject("a", typeof(Uri), a);

            // Assert
            Assert.AreEqual<string>("a", var.VariableName);

            CompareCode(this.ccu, @"//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.1433
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;



public class Test
{
    
    public void TestMethod()
    {
        Uri a = new Uri(""http://base/relative"");
    }
}
");
        }

        #endregion

        #region XmlQualifiedName type

        [TestMethod]
        public void OGXmlQualifiedNameDefaultConstructor()
        {
            // Arrange
            this.SetupStandardTypesTest();

            XmlQualifiedName a = new XmlQualifiedName();

            // Act
            CodeVariableReferenceExpression var = this.sut.GenerateObject("a", typeof(XmlQualifiedName), a);

            // Assert
            Assert.AreEqual<string>("a", var.VariableName);

            CompareCode(this.ccu, @"//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.1433
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System.Xml;



public class Test
{
    
    public void TestMethod()
    {
        XmlQualifiedName a = new XmlQualifiedName();
    }
}
");
        }

        [TestMethod]
        public void OGXmlQualifiedNameNameOnlyConstructor()
        {
            // Arrange
            this.SetupStandardTypesTest();

            XmlQualifiedName a = new XmlQualifiedName("abc");

            // Act
            CodeVariableReferenceExpression var = this.sut.GenerateObject("a", typeof(XmlQualifiedName), a);

            // Assert
            Assert.AreEqual<string>("a", var.VariableName);

            CompareCode(this.ccu, @"//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.1433
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System.Xml;



public class Test
{
    
    public void TestMethod()
    {
        XmlQualifiedName a = new XmlQualifiedName(""abc"");
    }
}
");
        }

        [TestMethod]
        public void OGXmlQualifiedNameNameAndNamespaceConstructor()
        {
            // Arrange
            this.SetupStandardTypesTest();

            XmlQualifiedName a = new XmlQualifiedName("abc", "def");

            // Act
            CodeVariableReferenceExpression var = this.sut.GenerateObject("a", typeof(XmlQualifiedName), a);

            // Assert
            Assert.AreEqual<string>("a", var.VariableName);

            CompareCode(this.ccu, @"//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.1433
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System.Xml;



public class Test
{
    
    public void TestMethod()
    {
        XmlQualifiedName a = new XmlQualifiedName(""abc"", ""def"");
    }
}
");
        }

        #endregion

        #region Simple compound types

        [TestMethod]
        public void OGSimpleCompoundTypeIsNull()
        {
            // Arrange
            this.SetupStandardTypesTest();

            // Act
            CodeVariableReferenceExpression var = this.sut.GenerateObject("a", typeof(SimpleCompoundTypeDataContract), null);

            // Assert
            Assert.AreEqual<string>("a", var.VariableName);

            CompareCode(this.ccu, @"//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.1433
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Microsoft.WcfUnit.Library.Test.TestContracts;



public class Test
{
    
    public void TestMethod()
    {
        SimpleCompoundTypeDataContract a = null;
    }
}
");
        }

        [TestMethod]
        public void OGSimpleCompoundType()
        {
            // Arrange
            this.SetupStandardTypesTest();

            SimpleCompoundTypeDataContract a = new SimpleCompoundTypeDataContract();
            a.Property1 = "hello";
            a.Property2 = 0;
            a.Field1 = 1.23M;
            a.Field2 = null;

            // Act
            CodeVariableReferenceExpression var = this.sut.GenerateObject("a", typeof(SimpleCompoundTypeDataContract), a);

            // Assert
            Assert.AreEqual<string>("a", var.VariableName);
            CompareCode(this.ccu, @"//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.1433
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Microsoft.WcfUnit.Library.Test.TestContracts;
using System;



public class Test
{
    
    public void TestMethod()
    {
        SimpleCompoundTypeDataContract a = new SimpleCompoundTypeDataContract();
        a.Property1 = ""hello"";
        a.Property2 = 0;
        a.Field1 = 1.23m;
        a.Field2 = null;
    }
}
");
        }

        [TestMethod]
        public void OGSimpleCompoundTypeFullyQualified()
        {
            // Arrange
            this.SetupStandardTypesTest();
            this.useFullQualification = true;

            SimpleCompoundTypeDataContract a = new SimpleCompoundTypeDataContract();
            a.Property1 = "hello";
            a.Property2 = 0;
            a.Field1 = 1.23M;
            a.Field2 = null;

            // Act
            CodeVariableReferenceExpression var = this.sut.GenerateObject("a", typeof(SimpleCompoundTypeDataContract), a);

            // Assert
            Assert.AreEqual<string>("a", var.VariableName);

            CompareCode(this.ccu, @"//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.1433
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------



public class Test
{
    
    public void TestMethod()
    {
        Microsoft.WcfUnit.Library.Test.TestContracts.SimpleCompoundTypeDataContract a = new Microsoft.WcfUnit.Library.Test.TestContracts.SimpleCompoundTypeDataContract();
        a.Property1 = ""hello"";
        a.Property2 = 0;
        a.Field1 = 1.23m;
        a.Field2 = null;
    }
}
");
        }

        [TestMethod]
        public void OGNestedCompoundType()
        {
            // Arrange
            this.SetupStandardTypesTest();

            NestedCompoundType a = new NestedCompoundType();
            a.Compound1 = new SimpleCompoundTypeDataContract();
            a.Compound1.Property1 = "c1";
            a.Compound1.Property2 = 0;
            a.Compound1.Field1 = 1.23m;
            a.Compound1.Field2 = null;
            a.Compound2 = new SimpleCompoundTypeDataContract();
            a.Compound2.Property1 = "c2";
            a.Compound2.Property2 = 99;
            a.Compound2.Field1 = 4.56m;
            a.Compound2.Field2 = "c2";

            // Act
            CodeVariableReferenceExpression var = this.sut.GenerateObject("a", typeof(NestedCompoundType), a);

            // Assert
            Assert.AreEqual<string>("a", var.VariableName);

            CompareCode(this.ccu, @"//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.1433
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Microsoft.WcfUnit.Library.Test.TestContracts;
using System;



public class Test
{
    
    public void TestMethod()
    {
        NestedCompoundType a = new NestedCompoundType();
        a.Compound1 = new SimpleCompoundTypeDataContract();
        a.Compound1.Property1 = ""c1"";
        a.Compound1.Property2 = 0;
        a.Compound1.Field1 = 1.23m;
        a.Compound1.Field2 = null;
        a.Compound2 = new SimpleCompoundTypeDataContract();
        a.Compound2.Property1 = ""c2"";
        a.Compound2.Property2 = 99;
        a.Compound2.Field1 = 4.56m;
        a.Compound2.Field2 = ""c2"";
    }
}
");
        }

        [TestMethod]
        public void OGCustomValueType()
        {
            // Arrange
            this.SetupStandardTypesTest();

            CustomValueType a = new CustomValueType();
            a.A = 1;
            a.B = 2;

            // Act
            CodeVariableReferenceExpression var = this.sut.GenerateObject("a", typeof(CustomValueType), a);

            // Assert
            Assert.AreEqual<string>("a", var.VariableName);

            CompareCode(this.ccu, @"//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.1433
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Microsoft.WcfUnit.Library.Test.TestContracts;
using System;



public class Test
{
    
    public void TestMethod()
    {
        CustomValueType a = new CustomValueType();
        a.A = 1;
        a.B = 2;
    }
}
");
        }

        #endregion

        #region Arrays

        [TestMethod]
        public void OGSimpleArray()
        {
            // Arrange
            this.SetupStandardTypesTest();

            int[] a = new int[] { 0, 1, 2, 3, 4 };

            // Act
            CodeVariableReferenceExpression var = this.sut.GenerateObject("a", typeof(int[]), a);

            // Assert
            Assert.AreEqual<string>("a", var.VariableName);

            CompareCode(this.ccu, @"//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.1433
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;



public class Test
{
    
    public void TestMethod()
    {
        Int32[] a = new Int32[5];
        a[0] = 0;
        a[1] = 1;
        a[2] = 2;
        a[3] = 3;
        a[4] = 4;
    }
}
");
        }

        [TestMethod]
        public void OGSimpleEmptyArray()
        {
            // Arrange
            this.SetupStandardTypesTest();

            int[] a = new int[0];

            // Act
            CodeVariableReferenceExpression var = this.sut.GenerateObject("a", typeof(int[]), a);

            // Assert
            Assert.AreEqual<string>("a", var.VariableName);

            CompareCode(this.ccu, @"//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.1433
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;



public class Test
{
    
    public void TestMethod()
    {
        Int32[] a = new Int32[0];
    }
}
");
        }

        [TestMethod]
        public void OGCompoundWithArrayType()
        {
            // Arrange
            this.SetupStandardTypesTest();

            CompoundWithArrayType a = new CompoundWithArrayType();
            a.Arr = new int[] { 0, 1, 2, 3, 4 };

            // Act
            CodeVariableReferenceExpression var = this.sut.GenerateObject("a", typeof(CompoundWithArrayType), a);

            // Assert
            Assert.AreEqual<string>("a", var.VariableName);

            CompareCode(this.ccu, @"//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.1433
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Microsoft.WcfUnit.Library.Test.TestContracts;
using System;



public class Test
{
    
    public void TestMethod()
    {
        CompoundWithArrayType a = new CompoundWithArrayType();
        a.Arr = new Int32[5];
        a.Arr[0] = 0;
        a.Arr[1] = 1;
        a.Arr[2] = 2;
        a.Arr[3] = 3;
        a.Arr[4] = 4;
    }
}
");
        }

        [TestMethod]
        public void OGArrayOfSimpleCompoundType()
        {
            // Arrange
            this.SetupStandardTypesTest();

            SimpleCompoundTypeDataContract[] a = new SimpleCompoundTypeDataContract[2];
            a[0] = new SimpleCompoundTypeDataContract();
            a[0].Property1 = "0";
            a[0].Property2 = 0;
            a[0].Field1 = 0m;
            a[0].Field2 = "0";
            a[1] = new SimpleCompoundTypeDataContract();
            a[1].Property1 = "1";
            a[1].Property2 = 1;
            a[1].Field1 = 1m;
            a[1].Field2 = "1";

            // Act
            CodeVariableReferenceExpression var = this.sut.GenerateObject("a", typeof(SimpleCompoundTypeDataContract[]), a);

            // Assert
            Assert.AreEqual<string>("a", var.VariableName);

            CompareCode(this.ccu, @"//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.1433
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Microsoft.WcfUnit.Library.Test.TestContracts;
using System;



public class Test
{
    
    public void TestMethod()
    {
        SimpleCompoundTypeDataContract[] a = new SimpleCompoundTypeDataContract[2];
        a[0] = new SimpleCompoundTypeDataContract();
        a[0].Property1 = ""0"";
        a[0].Property2 = 0;
        a[0].Field1 = 0m;
        a[0].Field2 = ""0"";
        a[1] = new SimpleCompoundTypeDataContract();
        a[1].Property1 = ""1"";
        a[1].Property2 = 1;
        a[1].Field1 = 1m;
        a[1].Field2 = ""1"";
    }
}
");
        }

        #endregion

        #region Xml types

        [TestMethod]
        public void OGXmlElementNull()
        {
            // Arrange
            this.SetupStandardTypesTest();
            XmlElement a = null;

            // Act
            CodeVariableReferenceExpression var = this.sut.GenerateObject("a", typeof(XmlElement), a);

            // Assert
            Assert.AreEqual<string>("a", var.VariableName);

            CompareCode(this.ccu, @"//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.1433
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System.Xml;



public class Test
{
    
    public void TestMethod()
    {
        XmlElement a = null;
    }
}
");
        }

        [TestMethod]
        public void OGXmlElementWithValue()
        {
            // Arrange
            this.SetupStandardTypesTest();

            XmlDocument doc = new XmlDocument();
            doc.LoadXml("<test>Hello</test>");
            XmlElement a = doc.DocumentElement;

            // Act
            CodeVariableReferenceExpression var = this.sut.GenerateObject("a", typeof(XmlElement), a);

            // Assert
            Assert.AreEqual<string>("a", var.VariableName);

            CompareCode(this.ccu, @"//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.1433
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System.Xml;



public class Test
{
    
    public void TestMethod()
    {
        XmlDocument temp0 = new XmlDocument();
        temp0.LoadXml(""<test>Hello</test>"");
        XmlElement a = temp0.DocumentElement;
    }
}
");
        }

        [TestMethod]
        public void OGXmlElementInClassNull()
        {
            // Arrange
            this.SetupStandardTypesTest();

            CompoundTypeWithXmlElement a = new CompoundTypeWithXmlElement();
            a.Element = null;

            // Act
            CodeVariableReferenceExpression var = this.sut.GenerateObject("a", typeof(CompoundTypeWithXmlElement), a);

            // Assert
            Assert.AreEqual<string>("a", var.VariableName);

            CompareCode(this.ccu, @"//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.1433
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Microsoft.WcfUnit.Library.Test.TestContracts;
using System.Xml;



public class Test
{
    
    public void TestMethod()
    {
        CompoundTypeWithXmlElement a = new CompoundTypeWithXmlElement();
        a.Element = null;
    }
}
");
        }

        [TestMethod]
        public void OGXmlElementInClassWithValue()
        {
            // Arrange
            this.SetupStandardTypesTest();

            CompoundTypeWithXmlElement a = new CompoundTypeWithXmlElement();
            XmlDocument temp0 = new XmlDocument();
            temp0.LoadXml("<test>Hello</test>");
            a.Element = temp0.DocumentElement;

            // Act
            CodeVariableReferenceExpression var = this.sut.GenerateObject("a", typeof(CompoundTypeWithXmlElement), a);

            // Assert
            Assert.AreEqual<string>("a", var.VariableName);

            CompareCode(this.ccu, @"//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.1433
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Microsoft.WcfUnit.Library.Test.TestContracts;
using System.Xml;



public class Test
{
    
    public void TestMethod()
    {
        CompoundTypeWithXmlElement a = new CompoundTypeWithXmlElement();
        XmlDocument temp0 = new XmlDocument();
        temp0.LoadXml(""<test>Hello</test>"");
        a.Element = temp0.DocumentElement;
    }
}
");
        }

        [TestMethod]
        public void OGXmlNodeArrayNull()
        {
            // Arrange
            this.SetupStandardTypesTest();

            XmlNode[] a = null;

            // Act
            CodeVariableReferenceExpression var = this.sut.GenerateObject("a", typeof(XmlNode[]), a);

            // Assert
            Assert.AreEqual<string>("a", var.VariableName);

            CompareCode(this.ccu, @"//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.1433
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System.Xml;



public class Test
{
    
    public void TestMethod()
    {
        XmlNode[] a = null;
    }
}
");
        }

        [TestMethod]
        public void OGXmlNodeArrayEmpty()
        {
            // Arrange
            this.SetupStandardTypesTest();

            XmlNode[] a = new XmlNode[0];

            // Act
            CodeVariableReferenceExpression var = this.sut.GenerateObject("a", typeof(XmlNode[]), a);

            // Assert
            Assert.AreEqual<string>("a", var.VariableName);

            CompareCode(this.ccu, @"//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.1433
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System.Xml;



public class Test
{
    
    public void TestMethod()
    {
        XmlNode[] a = new XmlNode[0];
    }
}
");
        }

        [TestMethod]
        public void OGXmlNodeArrayWithValues()
        {
            // Arrange
            this.SetupStandardTypesTest();

            XmlNode[] a = new XmlNode[2];
            XmlDocument temp0 = new XmlDocument();
            temp0.LoadXml("<test>Hello1</test>");
            a[0] = temp0.DocumentElement;
            XmlDocument temp1 = new XmlDocument();
            temp1.LoadXml("<test>Hello1</test>");
            a[1] = temp1.DocumentElement;

            // Act
            CodeVariableReferenceExpression var = this.sut.GenerateObject("a", typeof(XmlNode[]), a);

            // Assert
            Assert.AreEqual<string>("a", var.VariableName);

            CompareCode(this.ccu, @"//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.1433
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System.Xml;



public class Test
{
    
    public void TestMethod()
    {
        XmlNode[] a = new XmlNode[2];
        XmlDocument temp0 = new XmlDocument();
        temp0.LoadXml(""<test>Hello1</test>"");
        a[0] = temp0.DocumentElement;
        XmlDocument temp1 = new XmlDocument();
        temp1.LoadXml(""<test>Hello1</test>"");
        a[1] = temp1.DocumentElement;
    }
}
");
        }

        [TestMethod]
        public void OGXmlNodeArrayInClassNull()
        {
            // Arrange
            this.SetupStandardTypesTest();

            CompoundTypeWithXmlNodeArray a = new CompoundTypeWithXmlNodeArray();
            a.Nodes = null;

            // Act
            CodeVariableReferenceExpression var = this.sut.GenerateObject("a", typeof(CompoundTypeWithXmlNodeArray), a);

            // Assert
            Assert.AreEqual<string>("a", var.VariableName);

            CompareCode(this.ccu, @"//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.1433
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Microsoft.WcfUnit.Library.Test.TestContracts;
using System.Xml;



public class Test
{
    
    public void TestMethod()
    {
        CompoundTypeWithXmlNodeArray a = new CompoundTypeWithXmlNodeArray();
        a.Nodes = null;
    }
}
");
        }

        [TestMethod]
        public void OGXmlNodeArrayInClassEmpty()
        {
            // Arrange
            this.SetupStandardTypesTest();

            CompoundTypeWithXmlNodeArray a = new CompoundTypeWithXmlNodeArray();
            a.Nodes = new XmlNode[0];

            // Act
            CodeVariableReferenceExpression var = this.sut.GenerateObject("a", typeof(CompoundTypeWithXmlNodeArray), a);

            // Assert
            Assert.AreEqual<string>("a", var.VariableName);

            CompareCode(this.ccu, @"//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.1433
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Microsoft.WcfUnit.Library.Test.TestContracts;
using System.Xml;



public class Test
{
    
    public void TestMethod()
    {
        CompoundTypeWithXmlNodeArray a = new CompoundTypeWithXmlNodeArray();
        a.Nodes = new XmlNode[0];
    }
}
");
        }

        [TestMethod]
        public void OGXmlNodeArrayInClassWithValues()
        {
            // Arrange
            this.SetupStandardTypesTest();

            CompoundTypeWithXmlNodeArray a = new CompoundTypeWithXmlNodeArray();
            a.Nodes = new XmlNode[2];
            XmlDocument temp0 = new XmlDocument();
            temp0.LoadXml("<test>Hello1</test>");
            a.Nodes[0] = temp0.DocumentElement;
            XmlDocument temp1 = new XmlDocument();
            temp1.LoadXml("<test>Hello1</test>");
            a.Nodes[1] = temp1.DocumentElement;

            // Act
            CodeVariableReferenceExpression var = this.sut.GenerateObject("a", typeof(CompoundTypeWithXmlNodeArray), a);

            // Assert
            Assert.AreEqual<string>("a", var.VariableName);

            CompareCode(this.ccu, @"//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.1433
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Microsoft.WcfUnit.Library.Test.TestContracts;
using System.Xml;



public class Test
{
    
    public void TestMethod()
    {
        CompoundTypeWithXmlNodeArray a = new CompoundTypeWithXmlNodeArray();
        a.Nodes = new XmlNode[2];
        XmlDocument temp0 = new XmlDocument();
        temp0.LoadXml(""<test>Hello1</test>"");
        a.Nodes[0] = temp0.DocumentElement;
        XmlDocument temp1 = new XmlDocument();
        temp1.LoadXml(""<test>Hello1</test>"");
        a.Nodes[1] = temp1.DocumentElement;
    }
}
");
        }

        #endregion

        #region Generic collections of simple types

        [TestMethod]
        public void OGGenericCollectionOfSimpleType()
        {
            // Arrange
            this.SetupStandardTypesTest();

            Collection<int> a = new Collection<int>(new int[] { 0, 1, 2, 3, 4 });

            // Act
            CodeVariableReferenceExpression var = this.sut.GenerateObject("a", typeof(Collection<int>), a);

            // Assert
            Assert.AreEqual<string>("a", var.VariableName);

            CompareCode(this.ccu, @"//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.1433
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections.ObjectModel;



public class Test
{
    
    public void TestMethod()
    {
        Collection<Int32> a = new Collection<Int32>();
        a.Add(0);
        a.Add(1);
        a.Add(2);
        a.Add(3);
        a.Add(4);
    }
}
");
        }

        [TestMethod]
        public void OGGenericCollectionOfDateTime()
        {
            // Arrange
            this.SetupStandardTypesTest();

            Collection<DateTime> a = new Collection<DateTime>(new DateTime[] { new DateTime(2008, 2, 1), new DateTime(2008, 1, 31) });

            // Act
            CodeVariableReferenceExpression var = this.sut.GenerateObject("a", typeof(Collection<DateTime>), a);

            // Assert
            Assert.AreEqual<string>("a", var.VariableName);

            CompareCode(this.ccu, @"//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.1433
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections.ObjectModel;



public class Test
{
    
    public void TestMethod()
    {
        Collection<DateTime> a = new Collection<DateTime>();
        a.Add(new DateTime(2008, 2, 1, 0, 0, 0, 0));
        a.Add(new DateTime(2008, 1, 31, 0, 0, 0, 0));
    }
}
");
        }

        [TestMethod]
        public void OGGenericICollectionOfSimpleType()
        {
            // Arrange
            this.SetupStandardTypesTest();

            int[] a = new int[] { 0, 1, 2, 3, 4 };

            // Act
            CodeVariableReferenceExpression var = this.sut.GenerateObject("a", typeof(ICollection<int>), a);

            // Assert
            Assert.AreEqual<string>("a", var.VariableName);

            CompareCode(this.ccu, @"//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.1433
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections.ObjectModel;



public class Test
{
    
    public void TestMethod()
    {
        Collection<Int32> a = new Collection<Int32>();
        a.Add(0);
        a.Add(1);
        a.Add(2);
        a.Add(3);
        a.Add(4);
    }
}
");
        }

        [TestMethod]
        public void OGGenericListOfSimpleType()
        {
            // Arrange
            this.SetupStandardTypesTest();

            List<int> a = new List<int>(new int[] { 0, 1, 2, 3, 4 });

            // Act
            CodeVariableReferenceExpression var = this.sut.GenerateObject("a", typeof(List<int>), a);

            // Assert
            Assert.AreEqual<string>("a", var.VariableName);

            CompareCode(this.ccu, @"//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.1433
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;



public class Test
{
    
    public void TestMethod()
    {
        List<Int32> a = new List<Int32>();
        a.Add(0);
        a.Add(1);
        a.Add(2);
        a.Add(3);
        a.Add(4);
    }
}
");
        }

        [TestMethod]
        public void OGGenericIListOfSimpleType()
        {
            // Arrange
            this.SetupStandardTypesTest();

            int[] a = new int[] { 0, 1, 2, 3, 4 };

            // Act
            CodeVariableReferenceExpression var = this.sut.GenerateObject("a", typeof(IList<int>), a);

            // Assert
            Assert.AreEqual<string>("a", var.VariableName);

            CompareCode(this.ccu, @"//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.1433
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections.ObjectModel;



public class Test
{
    
    public void TestMethod()
    {
        Collection<Int32> a = new Collection<Int32>();
        a.Add(0);
        a.Add(1);
        a.Add(2);
        a.Add(3);
        a.Add(4);
    }
}
");
        }

        #endregion

        #region Generic collections with type mismatches

        [TestMethod]
        public void OGGenericCollectionOfSimpleTypeWithElementTypeMismatch()
        {
            // Arrange
            this.SetupStandardTypesTest();

            Collection<int> a = new Collection<int>(new int[] { 0, 1, 2, 3, 4 });

            // Act and assert
            TestHelper.TestForInvalidOperationException(
                delegate
                {
                    this.sut.GenerateObject("a", typeof(Collection<string>), a);
                });
        }

        [TestMethod]
        public void OGGenericICollectionOfSimpleTypeWithElementTypeMismatch()
        {
            // Arrange
            this.SetupStandardTypesTest();

            int[] a = new int[] { 0, 1, 2, 3, 4 };

            // Act and assert
            TestHelper.TestForInvalidOperationException(
                delegate
                {
                    this.sut.GenerateObject("a", typeof(ICollection<string>), a);
                });
        }

        [TestMethod]
        public void OGGenericCollectionOfSimpleTypeWithCollectionMismatch()
        {
            // Arrange
            this.SetupStandardTypesTest();

            int a = 0;

            // Act and assert
            TestHelper.TestForInvalidOperationException(
                delegate
                {
                    this.sut.GenerateObject("a", typeof(Collection<string>), a);
                });
        }

        [TestMethod]
        public void OGGenericICollectionOfSimpleTypeWithArrayMismatch()
        {
            // Arrange
            this.SetupStandardTypesTest();

            int a = 0;

            // Act and assert
            TestHelper.TestForInvalidOperationException(
                delegate
                {
                    this.sut.GenerateObject("a", typeof(ICollection<string>), a);
                });
        }

        #endregion

        #region Generic collections of compound types

        [TestMethod]
        public void OGGenericCollectionOfSimpleCompoundType()
        {
            // Assert
            this.SetupStandardTypesTest();

            SimpleCompoundTypeDataContract[] arr = new SimpleCompoundTypeDataContract[2];
            arr[0] = new SimpleCompoundTypeDataContract();
            arr[0].Property1 = "0";
            arr[0].Property2 = 0;
            arr[0].Field1 = 0m;
            arr[0].Field2 = "0";
            arr[1] = new SimpleCompoundTypeDataContract();
            arr[1].Property1 = "1";
            arr[1].Property2 = 1;
            arr[1].Field1 = 1m;
            arr[1].Field2 = "1";
            Collection<SimpleCompoundTypeDataContract> a = new Collection<SimpleCompoundTypeDataContract>(arr);

            // Act
            CodeVariableReferenceExpression var = this.sut.GenerateObject("a", typeof(Collection<SimpleCompoundTypeDataContract>), a);

            // Assert
            Assert.AreEqual<string>("a", var.VariableName);

            CompareCode(this.ccu, @"//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.1433
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Microsoft.WcfUnit.Library.Test.TestContracts;
using System;
using System.Collections.ObjectModel;



public class Test
{
    
    public void TestMethod()
    {
        Collection<SimpleCompoundTypeDataContract> a = new Collection<SimpleCompoundTypeDataContract>();
        SimpleCompoundTypeDataContract temp0 = new SimpleCompoundTypeDataContract();
        temp0.Property1 = ""0"";
        temp0.Property2 = 0;
        temp0.Field1 = 0m;
        temp0.Field2 = ""0"";
        a.Add(temp0);
        SimpleCompoundTypeDataContract temp1 = new SimpleCompoundTypeDataContract();
        temp1.Property1 = ""1"";
        temp1.Property2 = 1;
        temp1.Field1 = 1m;
        temp1.Field2 = ""1"";
        a.Add(temp1);
    }
}
");
        }

        [TestMethod]
        public void OGGenericICollectionOfSimpleCompoundType()
        {
            // Assert
            this.SetupStandardTypesTest();

            SimpleCompoundTypeDataContract[] a = new SimpleCompoundTypeDataContract[2];
            a[0] = new SimpleCompoundTypeDataContract();
            a[0].Property1 = "0";
            a[0].Property2 = 0;
            a[0].Field1 = 0m;
            a[0].Field2 = "0";
            a[1] = new SimpleCompoundTypeDataContract();
            a[1].Property1 = "1";
            a[1].Property2 = 1;
            a[1].Field1 = 1m;
            a[1].Field2 = "1";

            // Act
            CodeVariableReferenceExpression var = this.sut.GenerateObject("a", typeof(ICollection<SimpleCompoundTypeDataContract>), a);

            // Assert
            Assert.AreEqual<string>("a", var.VariableName);

            CompareCode(this.ccu, @"//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.1433
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Microsoft.WcfUnit.Library.Test.TestContracts;
using System;
using System.Collections.ObjectModel;



public class Test
{
    
    public void TestMethod()
    {
        Collection<SimpleCompoundTypeDataContract> a = new Collection<SimpleCompoundTypeDataContract>();
        SimpleCompoundTypeDataContract temp0 = new SimpleCompoundTypeDataContract();
        temp0.Property1 = ""0"";
        temp0.Property2 = 0;
        temp0.Field1 = 0m;
        temp0.Field2 = ""0"";
        a.Add(temp0);
        SimpleCompoundTypeDataContract temp1 = new SimpleCompoundTypeDataContract();
        temp1.Property1 = ""1"";
        temp1.Property2 = 1;
        temp1.Field1 = 1m;
        temp1.Field2 = ""1"";
        a.Add(temp1);
    }
}
");
        }

        #endregion

        #region Compound type containing a generic collection

        [TestMethod]
        public void OGCompoundWithGenericSimpleCollection()
        {
            // Arrange
            this.SetupStandardTypesTest();

            CompoundWithGenericSimpleCollection a = new CompoundWithGenericSimpleCollection();
            a.Arr = new Collection<int>(new int[] { 0, 1, 2, 3, 4 });

            // Act
            CodeVariableReferenceExpression var = this.sut.GenerateObject("a", typeof(CompoundWithGenericSimpleCollection), a);

            // Assert
            Assert.AreEqual<string>("a", var.VariableName);

            CompareCode(this.ccu, @"//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.1433
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Microsoft.WcfUnit.Library.Test.TestContracts;
using System;
using System.Collections.ObjectModel;



public class Test
{
    
    public void TestMethod()
    {
        CompoundWithGenericSimpleCollection a = new CompoundWithGenericSimpleCollection();
        a.Arr = new Collection<Int32>();
        a.Arr.Add(0);
        a.Arr.Add(1);
        a.Arr.Add(2);
        a.Arr.Add(3);
        a.Arr.Add(4);
    }
}
");
        }

        [TestMethod]
        public void OGCompoundWithGenericSimpleICollection()
        {
            // Arrange
            this.SetupStandardTypesTest();

            CompoundWithGenericSimpleICollection a = new CompoundWithGenericSimpleICollection();
            a.Arr = new int[] { 0, 1, 2, 3, 4 };

            // Act
            CodeVariableReferenceExpression var = this.sut.GenerateObject("a", typeof(CompoundWithGenericSimpleICollection), a);

            // Assert
            Assert.AreEqual<string>("a", var.VariableName);

            CompareCode(this.ccu, @"//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.1433
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Microsoft.WcfUnit.Library.Test.TestContracts;
using System;
using System.Collections.ObjectModel;



public class Test
{
    
    public void TestMethod()
    {
        CompoundWithGenericSimpleICollection a = new CompoundWithGenericSimpleICollection();
        a.Arr = new Collection<Int32>();
        a.Arr.Add(0);
        a.Arr.Add(1);
        a.Arr.Add(2);
        a.Arr.Add(3);
        a.Arr.Add(4);
    }
}
");
        }

        [TestMethod]
        public void OGCompoundWithGenericCompoundCollection()
        {
            // Arrange
            this.SetupStandardTypesTest();

            CompoundWithGenericCompoundCollection a = new CompoundWithGenericCompoundCollection();
            a.Arr = new Collection<SimpleCompoundTypeDataContract>();
            SimpleCompoundTypeDataContract temp0 = new SimpleCompoundTypeDataContract();
            temp0.Property1 = "0";
            temp0.Property2 = 0;
            temp0.Field1 = 0m;
            temp0.Field2 = "0";
            a.Arr.Add(temp0);
            SimpleCompoundTypeDataContract temp1 = new SimpleCompoundTypeDataContract();
            temp1.Property1 = "1";
            temp1.Property2 = 1;
            temp1.Field1 = 1m;
            temp1.Field2 = "1";
            a.Arr.Add(temp1);

            // Act
            CodeVariableReferenceExpression var = this.sut.GenerateObject("a", typeof(CompoundWithGenericCompoundCollection), a);

            // Assert
            Assert.AreEqual<string>("a", var.VariableName);

            CompareCode(this.ccu, @"//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.1433
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Microsoft.WcfUnit.Library.Test.TestContracts;
using System;
using System.Collections.ObjectModel;



public class Test
{
    
    public void TestMethod()
    {
        CompoundWithGenericCompoundCollection a = new CompoundWithGenericCompoundCollection();
        a.Arr = new Collection<SimpleCompoundTypeDataContract>();
        SimpleCompoundTypeDataContract temp0 = new SimpleCompoundTypeDataContract();
        temp0.Property1 = ""0"";
        temp0.Property2 = 0;
        temp0.Field1 = 0m;
        temp0.Field2 = ""0"";
        a.Arr.Add(temp0);
        SimpleCompoundTypeDataContract temp1 = new SimpleCompoundTypeDataContract();
        temp1.Property1 = ""1"";
        temp1.Property2 = 1;
        temp1.Field1 = 1m;
        temp1.Field2 = ""1"";
        a.Arr.Add(temp1);
    }
}
");
        }

        [TestMethod]
        public void OGCompoundWithGenericCompoundICollection()
        {
            // Arrange
            this.SetupStandardTypesTest();

            CompoundWithGenericCompoundICollection a = new CompoundWithGenericCompoundICollection();
            SimpleCompoundTypeDataContract temp0 = new SimpleCompoundTypeDataContract();
            temp0.Property1 = "0";
            temp0.Property2 = 0;
            temp0.Field1 = 0m;
            temp0.Field2 = "0";
            SimpleCompoundTypeDataContract temp1 = new SimpleCompoundTypeDataContract();
            temp1.Property1 = "1";
            temp1.Property2 = 1;
            temp1.Field1 = 1m;
            temp1.Field2 = "1";
            a.Arr = new SimpleCompoundTypeDataContract[] { temp0, temp1 };

            // Act
            CodeVariableReferenceExpression var = this.sut.GenerateObject("a", typeof(CompoundWithGenericCompoundICollection), a);

            // Assert
            Assert.AreEqual<string>("a", var.VariableName);

            CompareCode(this.ccu, @"//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.1433
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Microsoft.WcfUnit.Library.Test.TestContracts;
using System;
using System.Collections.ObjectModel;



public class Test
{
    
    public void TestMethod()
    {
        CompoundWithGenericCompoundICollection a = new CompoundWithGenericCompoundICollection();
        a.Arr = new Collection<SimpleCompoundTypeDataContract>();
        SimpleCompoundTypeDataContract temp0 = new SimpleCompoundTypeDataContract();
        temp0.Property1 = ""0"";
        temp0.Property2 = 0;
        temp0.Field1 = 0m;
        temp0.Field2 = ""0"";
        a.Arr.Add(temp0);
        SimpleCompoundTypeDataContract temp1 = new SimpleCompoundTypeDataContract();
        temp1.Property1 = ""1"";
        temp1.Property2 = 1;
        temp1.Field1 = 1m;
        temp1.Field2 = ""1"";
        a.Arr.Add(temp1);
    }
}
");
        }

        #endregion

        #region Class derived from the generic list

        [TestMethod]
        public void OGWrappedGenericList()
        {
            // Arrange
            this.SetupStandardTypesTest();

            GenericListWrapper a = new GenericListWrapper();
            SimpleCompoundTypeDataContract temp0 = new SimpleCompoundTypeDataContract();
            temp0.Property1 = "0";
            temp0.Property2 = 0;
            temp0.Field1 = 0m;
            temp0.Field2 = "0";
            a.Add(temp0);

            // Act
            CodeVariableReferenceExpression var = this.sut.GenerateObject("a", typeof(GenericListWrapper), a);

            // Assert
            Assert.AreEqual<string>("a", var.VariableName);

            CompareCode(this.ccu, @"//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.1433
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Microsoft.WcfUnit.Library.Test.TestContracts;
using System;



public class Test
{
    
    public void TestMethod()
    {
        GenericListWrapper a = new GenericListWrapper();
        SimpleCompoundTypeDataContract temp0 = new SimpleCompoundTypeDataContract();
        temp0.Property1 = ""0"";
        temp0.Property2 = 0;
        temp0.Field1 = 0m;
        temp0.Field2 = ""0"";
        a.Add(temp0);
    }
}
");
        }

        [TestMethod]
        public void OGWrappedGenericListWithOtherMembers()
        {
            // Arrange
            this.SetupStandardTypesTest();

            GenericListWrapperWithOtherMembers a = new GenericListWrapperWithOtherMembers();
            SimpleCompoundTypeDataContract temp0 = new SimpleCompoundTypeDataContract();
            temp0.Property1 = "0";
            temp0.Property2 = 0;
            temp0.Field1 = 0m;
            temp0.Field2 = "0";
            a.Add(temp0);
            a.Property = 99;

            // Act
            CodeVariableReferenceExpression var = this.sut.GenerateObject("a", typeof(GenericListWrapperWithOtherMembers), a);

            // Assert
            Assert.AreEqual<string>("a", var.VariableName);

            CompareCode(this.ccu, @"//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.1433
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Microsoft.WcfUnit.Library.Test.TestContracts;
using System;



public class Test
{
    
    public void TestMethod()
    {
        GenericListWrapperWithOtherMembers a = new GenericListWrapperWithOtherMembers();
        SimpleCompoundTypeDataContract temp0 = new SimpleCompoundTypeDataContract();
        temp0.Property1 = ""0"";
        temp0.Property2 = 0;
        temp0.Field1 = 0m;
        temp0.Field2 = ""0"";
        a.Add(temp0);
        a.Property = 99;
    }
}
");
        }

        [TestMethod]
        public void OGWrappedGenericListThatContainsWrapperWithOtherMembers()
        {
            // Arrange
            this.SetupStandardTypesTest();

            GenericListWrapperWithNestedWrapperThatHasOtherMembers a = new GenericListWrapperWithNestedWrapperThatHasOtherMembers();
            SimpleCompoundTypeDataContract temp0 = new SimpleCompoundTypeDataContract();
            temp0.Property1 = "0";
            temp0.Property2 = 0;
            temp0.Field1 = 0m;
            temp0.Field2 = "0";
            a.Add(temp0);
            a.Nested = new GenericListWrapperWithOtherMembers();
            SimpleCompoundTypeDataContract temp1 = new SimpleCompoundTypeDataContract();
            temp1.Property1 = "0";
            temp1.Property2 = 0;
            temp1.Field1 = 0m;
            temp1.Field2 = "0";
            a.Nested.Add(temp1);
            a.Nested.Property = 99;

            // Act
            CodeVariableReferenceExpression var = this.sut.GenerateObject("a", typeof(GenericListWrapperWithNestedWrapperThatHasOtherMembers), a);

            // Assert
            Assert.AreEqual<string>("a", var.VariableName);

            CompareCode(this.ccu, @"//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.1433
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Microsoft.WcfUnit.Library.Test.TestContracts;
using System;



public class Test
{
    
    public void TestMethod()
    {
        GenericListWrapperWithNestedWrapperThatHasOtherMembers a = new GenericListWrapperWithNestedWrapperThatHasOtherMembers();
        SimpleCompoundTypeDataContract temp0 = new SimpleCompoundTypeDataContract();
        temp0.Property1 = ""0"";
        temp0.Property2 = 0;
        temp0.Field1 = 0m;
        temp0.Field2 = ""0"";
        a.Add(temp0);
        a.Nested = new GenericListWrapperWithOtherMembers();
        SimpleCompoundTypeDataContract temp1 = new SimpleCompoundTypeDataContract();
        temp1.Property1 = ""0"";
        temp1.Property2 = 0;
        temp1.Field1 = 0m;
        temp1.Field2 = ""0"";
        a.Nested.Add(temp1);
        a.Nested.Property = 99;
    }
}
");
        }

        #endregion

        #region Class derived from the generic dictionary

        [TestMethod]
        public void OGWrappedGenericDictionary()
        {
            // Arrange
            this.SetupStandardTypesTest();

            GenericDictionaryWrapper a = new GenericDictionaryWrapper();
            SimpleCompoundTypeDataContract temp0 = new SimpleCompoundTypeDataContract();
            temp0.Property1 = "0";
            temp0.Property2 = 0;
            temp0.Field1 = 0m;
            temp0.Field2 = "0";
            a.Add("Key1", temp0);
            temp0 = new SimpleCompoundTypeDataContract();
            temp0.Property1 = "1";
            temp0.Property2 = 1;
            temp0.Field1 = 1m;
            temp0.Field2 = "1";
            a.Add("Key2", temp0);

            // Act
            CodeVariableReferenceExpression var = this.sut.GenerateObject("a", typeof(GenericDictionaryWrapper), a);

            // Assert
            Assert.AreEqual<string>("a", var.VariableName);

            CompareCode(this.ccu, @"//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.1433
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Microsoft.WcfUnit.Library.Test.TestContracts;
using System;



public class Test
{
    
    public void TestMethod()
    {
        GenericDictionaryWrapper a = new GenericDictionaryWrapper();
        SimpleCompoundTypeDataContract temp0 = new SimpleCompoundTypeDataContract();
        temp0.Property1 = ""0"";
        temp0.Property2 = 0;
        temp0.Field1 = 0m;
        temp0.Field2 = ""0"";
        a.Add(""Key1"", temp0);
        SimpleCompoundTypeDataContract temp1 = new SimpleCompoundTypeDataContract();
        temp1.Property1 = ""1"";
        temp1.Property2 = 1;
        temp1.Field1 = 1m;
        temp1.Field2 = ""1"";
        a.Add(""Key2"", temp1);
    }
}
");
        }

        [TestMethod]
        public void OGWrappedGenericDictionaryWithOtherMembers()
        {
            // Arrange
            this.SetupStandardTypesTest();

            GenericDictionaryWrapperWithOtherMembers a = new GenericDictionaryWrapperWithOtherMembers();
            SimpleCompoundTypeDataContract temp0 = new SimpleCompoundTypeDataContract();
            temp0.Property1 = "0";
            temp0.Property2 = 0;
            temp0.Field1 = 0m;
            temp0.Field2 = "0";
            a.Add("Key1", temp0);
            temp0 = new SimpleCompoundTypeDataContract();
            temp0.Property1 = "1";
            temp0.Property2 = 1;
            temp0.Field1 = 1m;
            temp0.Field2 = "1";
            a.Add("Key2", temp0);
            a.Property = 99;

            // Act
            CodeVariableReferenceExpression var = this.sut.GenerateObject("a", typeof(GenericDictionaryWrapperWithOtherMembers), a);

            // Assert
            Assert.AreEqual<string>("a", var.VariableName);

            CompareCode(this.ccu, @"//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.1433
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Microsoft.WcfUnit.Library.Test.TestContracts;
using System;



public class Test
{
    
    public void TestMethod()
    {
        GenericDictionaryWrapperWithOtherMembers a = new GenericDictionaryWrapperWithOtherMembers();
        SimpleCompoundTypeDataContract temp0 = new SimpleCompoundTypeDataContract();
        temp0.Property1 = ""0"";
        temp0.Property2 = 0;
        temp0.Field1 = 0m;
        temp0.Field2 = ""0"";
        a.Add(""Key1"", temp0);
        SimpleCompoundTypeDataContract temp1 = new SimpleCompoundTypeDataContract();
        temp1.Property1 = ""1"";
        temp1.Property2 = 1;
        temp1.Field1 = 1m;
        temp1.Field2 = ""1"";
        a.Add(""Key2"", temp1);
        a.Property = 99;
    }
}
");
        }

        [TestMethod]
        public void OGWrappedGenericDictionaryThatContainsWrapperWithOtherMembers()
        {
            // Arrange
            this.SetupStandardTypesTest();

            GenericDictionaryWrapperWithNestedWrapperThatHasOtherMembers a = new GenericDictionaryWrapperWithNestedWrapperThatHasOtherMembers();
            SimpleCompoundTypeDataContract temp0 = new SimpleCompoundTypeDataContract();
            temp0.Property1 = "0";
            temp0.Property2 = 0;
            temp0.Field1 = 0m;
            temp0.Field2 = "0";
            a.Add("Key1", temp0);
            a.Nested = new GenericDictionaryWrapperWithOtherMembers();
            SimpleCompoundTypeDataContract temp1 = new SimpleCompoundTypeDataContract();
            temp1.Property1 = "1";
            temp1.Property2 = 1;
            temp1.Field1 = 1m;
            temp1.Field2 = "1";
            a.Nested.Add("Key2", temp1);
            a.Nested.Property = 99;

            // Act
            CodeVariableReferenceExpression var = this.sut.GenerateObject("a", typeof(GenericDictionaryWrapperWithNestedWrapperThatHasOtherMembers), a);

            // Assert
            Assert.AreEqual<string>("a", var.VariableName);

            CompareCode(this.ccu, @"//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.1433
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Microsoft.WcfUnit.Library.Test.TestContracts;
using System;



public class Test
{
    
    public void TestMethod()
    {
        GenericDictionaryWrapperWithNestedWrapperThatHasOtherMembers a = new GenericDictionaryWrapperWithNestedWrapperThatHasOtherMembers();
        SimpleCompoundTypeDataContract temp0 = new SimpleCompoundTypeDataContract();
        temp0.Property1 = ""0"";
        temp0.Property2 = 0;
        temp0.Field1 = 0m;
        temp0.Field2 = ""0"";
        a.Add(""Key1"", temp0);
        a.Nested = new GenericDictionaryWrapperWithOtherMembers();
        SimpleCompoundTypeDataContract temp1 = new SimpleCompoundTypeDataContract();
        temp1.Property1 = ""1"";
        temp1.Property2 = 1;
        temp1.Field1 = 1m;
        temp1.Field2 = ""1"";
        a.Nested.Add(""Key2"", temp1);
        a.Nested.Property = 99;
    }
}
");
        }

        #endregion

        #region Generic dictionaries of simple types

        [TestMethod]
        public void OGGenericDictionaryWithSimpleTypes()
        {
            // Arrange
            this.SetupStandardTypesTest();

            Dictionary<string, int> a = new Dictionary<string, int>();
            a.Add("Key1", 1);
            a.Add("Key2", 2);

            // Act
            CodeVariableReferenceExpression var = this.sut.GenerateObject("a", typeof(Dictionary<string, int>), a);

            // Assert
            Assert.AreEqual<string>("a", var.VariableName);

            CompareCode(this.ccu, @"//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.1433
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;



public class Test
{
    
    public void TestMethod()
    {
        Dictionary<String, Int32> a = new Dictionary<String, Int32>();
        a.Add(""Key1"", 1);
        a.Add(""Key2"", 2);
    }
}
");
        }

        [TestMethod]
        public void OGGenericDictionaryWithGuidAndDateTime()
        {
            // Arrange
            this.SetupStandardTypesTest();

            Dictionary<Guid, DateTime> a = new Dictionary<Guid, DateTime>();
            a.Add(new Guid("A2BF47AC-3431-4f60-B572-94E91D6D8E0E"), new DateTime(2007, 2, 26));
            a.Add(new Guid("A2BF47AC-3431-4f60-B572-94E91D6D8E0F"), new DateTime(2007, 2, 27));

            // Act
            CodeVariableReferenceExpression var = this.sut.GenerateObject("a", typeof(Dictionary<Guid, DateTime>), a);

            // Assert
            Assert.AreEqual<string>("a", var.VariableName);

            CompareCode(this.ccu, @"//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.1433
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;



public class Test
{
    
    public void TestMethod()
    {
        Dictionary<Guid, DateTime> a = new Dictionary<Guid, DateTime>();
        a.Add(new Guid(""a2bf47ac-3431-4f60-b572-94e91d6d8e0e""), new DateTime(2007, 2, 26, 0, 0, 0, 0));
        a.Add(new Guid(""a2bf47ac-3431-4f60-b572-94e91d6d8e0f""), new DateTime(2007, 2, 27, 0, 0, 0, 0));
    }
}
");
        }

        [TestMethod]
        public void OGGenericIDictionaryWithSimpleTypes()
        {
            // Arrange
            this.SetupStandardTypesTest();

            Dictionary<string, int> a = new Dictionary<string, int>();
            a.Add("Key1", 1);
            a.Add("Key2", 2);

            // Act
            CodeVariableReferenceExpression var = this.sut.GenerateObject("a", typeof(IDictionary<string, int>), a);

            // Assert
            Assert.AreEqual<string>("a", var.VariableName);

            CompareCode(this.ccu, @"//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.1433
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;



public class Test
{
    
    public void TestMethod()
    {
        Dictionary<String, Int32> a = new Dictionary<String, Int32>();
        a.Add(""Key1"", 1);
        a.Add(""Key2"", 2);
    }
}
");
        }

        #endregion

        #region Generic dictionaries of compound types

        [TestMethod]
        public void OGGenericDictionaryWithCompoundType()
        {
            // Arrange
            this.SetupStandardTypesTest();

            Dictionary<string, SimpleCompoundTypeDataContract> a = new Dictionary<string, SimpleCompoundTypeDataContract>();
            SimpleCompoundTypeDataContract temp0;
            temp0 = new SimpleCompoundTypeDataContract();
            temp0.Property1 = "0";
            temp0.Property2 = 0;
            temp0.Field1 = 0m;
            temp0.Field2 = "0";
            a.Add("Key1", temp0);
            temp0 = new SimpleCompoundTypeDataContract();
            temp0.Property1 = "1";
            temp0.Property2 = 1;
            temp0.Field1 = 1m;
            temp0.Field2 = "1";
            a.Add("Key2", temp0);

            // Act
            CodeVariableReferenceExpression var = this.sut.GenerateObject("a", typeof(Dictionary<string, SimpleCompoundTypeDataContract>), a);

            // Assert
            Assert.AreEqual<string>("a", var.VariableName);

            CompareCode(this.ccu, @"//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.1433
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Microsoft.WcfUnit.Library.Test.TestContracts;
using System;
using System.Collections.Generic;



public class Test
{
    
    public void TestMethod()
    {
        Dictionary<String, SimpleCompoundTypeDataContract> a = new Dictionary<String, SimpleCompoundTypeDataContract>();
        SimpleCompoundTypeDataContract temp0 = new SimpleCompoundTypeDataContract();
        temp0.Property1 = ""0"";
        temp0.Property2 = 0;
        temp0.Field1 = 0m;
        temp0.Field2 = ""0"";
        a.Add(""Key1"", temp0);
        SimpleCompoundTypeDataContract temp1 = new SimpleCompoundTypeDataContract();
        temp1.Property1 = ""1"";
        temp1.Property2 = 1;
        temp1.Field1 = 1m;
        temp1.Field2 = ""1"";
        a.Add(""Key2"", temp1);
    }
}
");
        }

        [TestMethod]
        public void OGGenericDictionaryWithCompoundTypesOnBothSides()
        {
            // Arrange
            this.SetupStandardTypesTest();

            Dictionary<SimpleCompoundTypeDataContract, SimpleCompoundTypeDataContract> a = new Dictionary<SimpleCompoundTypeDataContract, SimpleCompoundTypeDataContract>();
            SimpleCompoundTypeDataContract temp0 = new SimpleCompoundTypeDataContract();
            temp0.Property1 = "k0";
            temp0.Property2 = 0;
            temp0.Field1 = 0m;
            temp0.Field2 = "0";
            SimpleCompoundTypeDataContract temp1 = new SimpleCompoundTypeDataContract();
            temp1.Property1 = "v0";
            temp1.Property2 = 0;
            temp1.Field1 = 0m;
            temp1.Field2 = "0";
            a.Add(temp0, temp1);
            SimpleCompoundTypeDataContract temp2 = new SimpleCompoundTypeDataContract();
            temp2.Property1 = "k1";
            temp2.Property2 = 0;
            temp2.Field1 = 0m;
            temp2.Field2 = "0";
            SimpleCompoundTypeDataContract temp3 = new SimpleCompoundTypeDataContract();
            temp3.Property1 = "v1";
            temp3.Property2 = 0;
            temp3.Field1 = 0m;
            temp3.Field2 = "0";
            a.Add(temp2, temp3);
            CodeVariableReferenceExpression var = this.sut.GenerateObject("a", typeof(Dictionary<SimpleCompoundTypeDataContract, SimpleCompoundTypeDataContract>), a);
            Assert.AreEqual<string>("a", var.VariableName);

            CompareCode(this.ccu, @"//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.1433
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Microsoft.WcfUnit.Library.Test.TestContracts;
using System;
using System.Collections.Generic;



public class Test
{
    
    public void TestMethod()
    {
        Dictionary<SimpleCompoundTypeDataContract, SimpleCompoundTypeDataContract> a = new Dictionary<SimpleCompoundTypeDataContract, SimpleCompoundTypeDataContract>();
        SimpleCompoundTypeDataContract temp0 = new SimpleCompoundTypeDataContract();
        temp0.Property1 = ""k0"";
        temp0.Property2 = 0;
        temp0.Field1 = 0m;
        temp0.Field2 = ""0"";
        SimpleCompoundTypeDataContract temp1 = new SimpleCompoundTypeDataContract();
        temp1.Property1 = ""v0"";
        temp1.Property2 = 0;
        temp1.Field1 = 0m;
        temp1.Field2 = ""0"";
        a.Add(temp0, temp1);
        SimpleCompoundTypeDataContract temp2 = new SimpleCompoundTypeDataContract();
        temp2.Property1 = ""k1"";
        temp2.Property2 = 0;
        temp2.Field1 = 0m;
        temp2.Field2 = ""0"";
        SimpleCompoundTypeDataContract temp3 = new SimpleCompoundTypeDataContract();
        temp3.Property1 = ""v1"";
        temp3.Property2 = 0;
        temp3.Field1 = 0m;
        temp3.Field2 = ""0"";
        a.Add(temp2, temp3);
    }
}
");
        }

        [TestMethod]
        public void OGCompoundWithRecursiveDictionary()
        {
            // Arrange
            this.SetupStandardTypesTest();

            CompoundWithRecursiveDictionary a = new CompoundWithRecursiveDictionary();
            a.D = new Dictionary<string, CompoundWithRecursiveDictionary>();
            CompoundWithRecursiveDictionary temp0 = new CompoundWithRecursiveDictionary();
            temp0.D = new Dictionary<string, CompoundWithRecursiveDictionary>();
            CompoundWithRecursiveDictionary temp1 = new CompoundWithRecursiveDictionary();
            temp1.D = null;
            temp0.D.Add("abc", temp1);
            a.D.Add("def", temp0);

            // Act
            CodeVariableReferenceExpression var = this.sut.GenerateObject("a", typeof(CompoundWithRecursiveDictionary), a);

            // Assert
            Assert.AreEqual<string>("a", var.VariableName);

            CompareCode(this.ccu, @"//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.1433
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Microsoft.WcfUnit.Library.Test.TestContracts;
using System;
using System.Collections.Generic;



public class Test
{
    
    public void TestMethod()
    {
        CompoundWithRecursiveDictionary a = new CompoundWithRecursiveDictionary();
        a.D = new Dictionary<String, CompoundWithRecursiveDictionary>();
        CompoundWithRecursiveDictionary temp0 = new CompoundWithRecursiveDictionary();
        temp0.D = new Dictionary<String, CompoundWithRecursiveDictionary>();
        CompoundWithRecursiveDictionary temp1 = new CompoundWithRecursiveDictionary();
        temp1.D = null;
        temp0.D.Add(""abc"", temp1);
        a.D.Add(""def"", temp0);
    }
}
");
        }

        #endregion

        #region Compound type containing a generic dictionary

        [TestMethod]
        public void OGCompoundWithGenericSimpleDictionary()
        {
            // Arrange
            this.SetupStandardTypesTest();

            CompoundWithGenericSimpleDictionary a = new CompoundWithGenericSimpleDictionary();
            a.D = new Dictionary<string, int>();
            a.D.Add("Key1", 1);
            a.D.Add("Key2", 2);

            // Act
            CodeVariableReferenceExpression var = this.sut.GenerateObject("a", typeof(CompoundWithGenericSimpleDictionary), a);

            // Assert
            Assert.AreEqual<string>("a", var.VariableName);

            CompareCode(this.ccu, @"//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.1433
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Microsoft.WcfUnit.Library.Test.TestContracts;
using System;
using System.Collections.Generic;



public class Test
{
    
    public void TestMethod()
    {
        CompoundWithGenericSimpleDictionary a = new CompoundWithGenericSimpleDictionary();
        a.D = new Dictionary<String, Int32>();
        a.D.Add(""Key1"", 1);
        a.D.Add(""Key2"", 2);
    }
}
");
        }

        [TestMethod]
        public void OGCompoundWithGenericSimpleIDictionary()
        {
            // Arrange
            this.SetupStandardTypesTest();

            CompoundWithGenericSimpleIDictionary a = new CompoundWithGenericSimpleIDictionary();
            a.D = new Dictionary<string, int>();
            a.D.Add("Key1", 1);
            a.D.Add("Key2", 2);

            // Act
            CodeVariableReferenceExpression var = this.sut.GenerateObject("a", typeof(CompoundWithGenericSimpleIDictionary), a);

            // Assert
            Assert.AreEqual<string>("a", var.VariableName);

            CompareCode(this.ccu, @"//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.1433
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Microsoft.WcfUnit.Library.Test.TestContracts;
using System;
using System.Collections.Generic;



public class Test
{
    
    public void TestMethod()
    {
        CompoundWithGenericSimpleIDictionary a = new CompoundWithGenericSimpleIDictionary();
        a.D = new Dictionary<String, Int32>();
        a.D.Add(""Key1"", 1);
        a.D.Add(""Key2"", 2);
    }
}
");
        }

        [TestMethod]
        public void OGCompoundWithGenericCompoundDictionary()
        {
            // Arrange
            this.SetupStandardTypesTest();

            CompoundWithGenericCompoundDictionary a = new CompoundWithGenericCompoundDictionary();
            a.D = new Dictionary<string, SimpleCompoundTypeDataContract>();
            SimpleCompoundTypeDataContract temp0 = new SimpleCompoundTypeDataContract();
            temp0.Property1 = "v0";
            temp0.Property2 = 0;
            temp0.Field1 = 0m;
            temp0.Field2 = "0";
            a.D.Add("Key1", temp0);
            SimpleCompoundTypeDataContract temp1 = new SimpleCompoundTypeDataContract();
            temp1.Property1 = "v1";
            temp1.Property2 = 0;
            temp1.Field1 = 0m;
            temp1.Field2 = "0";
            a.D.Add("Key2", temp1);

            // Act
            CodeVariableReferenceExpression var = this.sut.GenerateObject("a", typeof(CompoundWithGenericCompoundDictionary), a);

            // Assert
            Assert.AreEqual<string>("a", var.VariableName);

            CompareCode(this.ccu, @"//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.1433
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Microsoft.WcfUnit.Library.Test.TestContracts;
using System;
using System.Collections.Generic;



public class Test
{
    
    public void TestMethod()
    {
        CompoundWithGenericCompoundDictionary a = new CompoundWithGenericCompoundDictionary();
        a.D = new Dictionary<String, SimpleCompoundTypeDataContract>();
        SimpleCompoundTypeDataContract temp0 = new SimpleCompoundTypeDataContract();
        temp0.Property1 = ""v0"";
        temp0.Property2 = 0;
        temp0.Field1 = 0m;
        temp0.Field2 = ""0"";
        a.D.Add(""Key1"", temp0);
        SimpleCompoundTypeDataContract temp1 = new SimpleCompoundTypeDataContract();
        temp1.Property1 = ""v1"";
        temp1.Property2 = 0;
        temp1.Field1 = 0m;
        temp1.Field2 = ""0"";
        a.D.Add(""Key2"", temp1);
    }
}
");
        }

        [TestMethod]
        public void OGCompoundWithGenericCompoundBothSidesDictionary()
        {
            // Arrange
            this.SetupStandardTypesTest();

            CompoundWithGenericCompoundBothSidesDictionary a = new CompoundWithGenericCompoundBothSidesDictionary();
            a.D = new Dictionary<SimpleCompoundTypeDataContract, SimpleCompoundTypeDataContract>();
            SimpleCompoundTypeDataContract temp0 = new SimpleCompoundTypeDataContract();
            temp0.Property1 = "k0";
            temp0.Property2 = 0;
            temp0.Field1 = 0m;
            temp0.Field2 = "0";
            SimpleCompoundTypeDataContract temp1 = new SimpleCompoundTypeDataContract();
            temp1.Property1 = "v1";
            temp1.Property2 = 0;
            temp1.Field1 = 0m;
            temp1.Field2 = "0";
            a.D.Add(temp0, temp1);

            // Act
            CodeVariableReferenceExpression var = this.sut.GenerateObject("a", typeof(CompoundWithGenericCompoundBothSidesDictionary), a);

            // Assert
            Assert.AreEqual<string>("a", var.VariableName);

            CompareCode(this.ccu, @"//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.1433
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Microsoft.WcfUnit.Library.Test.TestContracts;
using System;
using System.Collections.Generic;



public class Test
{
    
    public void TestMethod()
    {
        CompoundWithGenericCompoundBothSidesDictionary a = new CompoundWithGenericCompoundBothSidesDictionary();
        a.D = new Dictionary<SimpleCompoundTypeDataContract, SimpleCompoundTypeDataContract>();
        SimpleCompoundTypeDataContract temp0 = new SimpleCompoundTypeDataContract();
        temp0.Property1 = ""k0"";
        temp0.Property2 = 0;
        temp0.Field1 = 0m;
        temp0.Field2 = ""0"";
        SimpleCompoundTypeDataContract temp1 = new SimpleCompoundTypeDataContract();
        temp1.Property1 = ""v1"";
        temp1.Property2 = 0;
        temp1.Field1 = 0m;
        temp1.Field2 = ""0"";
        a.D.Add(temp0, temp1);
    }
}
");
        }

        #endregion

        #region Non generic dictionary

        [TestMethod]
        public void OGNonGenericEnumerableCollection()
        {
            // Arrange
            this.SetupStandardTypesTest();

            NonGenericEnumerableOnlyCollection a = new NonGenericEnumerableOnlyCollection();
            a.Add(1);
            a.Add("hello");
            SimpleCompoundTypeDataContract temp0 = new SimpleCompoundTypeDataContract();
            temp0.Property1 = "0";
            temp0.Property2 = 0;
            temp0.Field1 = 0m;
            temp0.Field2 = "0";
            a.Add(temp0);

            // Act
            CodeVariableReferenceExpression var = this.sut.GenerateObject("a", typeof(NonGenericEnumerableOnlyCollection), a);

            // Assert
            Assert.AreEqual<string>("a", var.VariableName);

            CompareCode(this.ccu, @"//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.1433
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Microsoft.WcfUnit.Library.Test.TestContracts;
using System;



public class Test
{
    
    public void TestMethod()
    {
        NonGenericEnumerableOnlyCollection a = new NonGenericEnumerableOnlyCollection();
        a.Add(1);
        a.Add(""hello"");
        SimpleCompoundTypeDataContract temp0 = new SimpleCompoundTypeDataContract();
        temp0.Property1 = ""0"";
        temp0.Property2 = 0;
        temp0.Field1 = 0m;
        temp0.Field2 = ""0"";
        a.Add(temp0);
    }
}
");
        }

        [TestMethod]
        public void OGCompoundContainingNonGenericEnumerableCollection()
        {
            // Arrange
            this.SetupStandardTypesTest();

            CompoundContainingNonGenericEnumerableOnlyCollection a = new CompoundContainingNonGenericEnumerableOnlyCollection();
            a.Arr = new NonGenericEnumerableOnlyCollection();
            a.Arr.Add(1);
            a.Arr.Add("hello");
            SimpleCompoundTypeDataContract temp0 = new SimpleCompoundTypeDataContract();
            temp0.Property1 = "0";
            temp0.Property2 = 0;
            temp0.Field1 = 0m;
            temp0.Field2 = "0";
            a.Arr.Add(temp0);
            
            // Act
            CodeVariableReferenceExpression var = this.sut.GenerateObject("a", typeof(CompoundContainingNonGenericEnumerableOnlyCollection), a);

            // Assert
            Assert.AreEqual<string>("a", var.VariableName);

            CompareCode(this.ccu, @"//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.1433
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Microsoft.WcfUnit.Library.Test.TestContracts;
using System;



public class Test
{
    
    public void TestMethod()
    {
        CompoundContainingNonGenericEnumerableOnlyCollection a = new CompoundContainingNonGenericEnumerableOnlyCollection();
        a.Arr = new NonGenericEnumerableOnlyCollection();
        a.Arr.Add(1);
        a.Arr.Add(""hello"");
        SimpleCompoundTypeDataContract temp0 = new SimpleCompoundTypeDataContract();
        temp0.Property1 = ""0"";
        temp0.Property2 = 0;
        temp0.Field1 = 0m;
        temp0.Field2 = ""0"";
        a.Arr.Add(temp0);
    }
}
");
        }

        [TestMethod]
        public void OGNonGenericDictionaryWithSimpleTypes()
        {
            // Arrange
            this.SetupStandardTypesTest();

            Hashtable a = new Hashtable();
            a.Add("Key1", 1);

            // Act
            CodeVariableReferenceExpression var = this.sut.GenerateObject("a", typeof(Hashtable), a);

            // Assert
            Assert.AreEqual<string>("a", var.VariableName);

            CompareCode(this.ccu, @"//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.1433
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System.Collections;



public class Test
{
    
    public void TestMethod()
    {
        Hashtable a = new Hashtable();
        a.Add(""Key1"", 1);
    }
}
");
        }

        [TestMethod]
        public void OGNonGenericDictionaryWithCompoundTypesOnBothSides()
        {
            // Arrange
            this.SetupStandardTypesTest();

            Hashtable a = new Hashtable();
            SimpleCompoundTypeDataContract temp0 = new SimpleCompoundTypeDataContract();
            temp0.Property1 = "k0";
            temp0.Property2 = 0;
            temp0.Field1 = 0m;
            temp0.Field2 = "0";
            SimpleCompoundTypeDataContract temp1 = new SimpleCompoundTypeDataContract();
            temp1.Property1 = "v0";
            temp1.Property2 = 0;
            temp1.Field1 = 0m;
            temp1.Field2 = "0";
            a.Add(temp0, temp1);

            // Act
            CodeVariableReferenceExpression var = this.sut.GenerateObject("a", typeof(Hashtable), a);

            // Assert
            Assert.AreEqual<string>("a", var.VariableName);

            CompareCode(this.ccu, @"//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.1433
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Microsoft.WcfUnit.Library.Test.TestContracts;
using System;
using System.Collections;



public class Test
{
    
    public void TestMethod()
    {
        Hashtable a = new Hashtable();
        SimpleCompoundTypeDataContract temp0 = new SimpleCompoundTypeDataContract();
        temp0.Property1 = ""k0"";
        temp0.Property2 = 0;
        temp0.Field1 = 0m;
        temp0.Field2 = ""0"";
        SimpleCompoundTypeDataContract temp1 = new SimpleCompoundTypeDataContract();
        temp1.Property1 = ""v0"";
        temp1.Property2 = 0;
        temp1.Field1 = 0m;
        temp1.Field2 = ""0"";
        a.Add(temp0, temp1);
    }
}
");
        }

        [TestMethod]
        public void OGCompoundContainingNonGenericDictionary()
        {
            // Arrange
            this.SetupStandardTypesTest();

            CompoundContainingNonGenericDictionary a = new CompoundContainingNonGenericDictionary();
            a.Arr = new Hashtable();
            a.Arr.Add(1, "one");

            // Act
            CodeVariableReferenceExpression var = this.sut.GenerateObject("a", typeof(CompoundContainingNonGenericDictionary), a);

            // Assert
            Assert.AreEqual<string>("a", var.VariableName);

            CompareCode(this.ccu, @"//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.1433
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Microsoft.WcfUnit.Library.Test.TestContracts;
using System.Collections;



public class Test
{
    
    public void TestMethod()
    {
        CompoundContainingNonGenericDictionary a = new CompoundContainingNonGenericDictionary();
        a.Arr = new Hashtable();
        a.Arr.Add(1, ""one"");
    }
}
");
        }

        #endregion

        #region Array lists

        [TestMethod]
        public void OGArrayList()
        {
            // Arrange
            this.SetupStandardTypesTest();

            ArrayList a = new ArrayList();
            a.Add(1);
            a.Add("hello");

            // Act
            CodeVariableReferenceExpression var = this.sut.GenerateObject("a", typeof(ArrayList), a);

            // Assert
            Assert.AreEqual<string>("a", var.VariableName);

            CompareCode(this.ccu, @"//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.1433
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System.Collections;



public class Test
{
    
    public void TestMethod()
    {
        ArrayList a = new ArrayList();
        a.Add(1);
        a.Add(""hello"");
    }
}
");
        }

        [TestMethod]
        public void OGCompoundContainingArrayList()
        {
            // Arrange
            this.SetupStandardTypesTest();

            CompoundContainingArrayList a = new CompoundContainingArrayList();
            a.Arr = new ArrayList();
            a.Arr.Add(1);
            a.Arr.Add(2);

            // Act
            CodeVariableReferenceExpression var = this.sut.GenerateObject("a", typeof(CompoundContainingArrayList), a);

            // Assert
            Assert.AreEqual<string>("a", var.VariableName);

            CompareCode(this.ccu, @"//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.1433
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Microsoft.WcfUnit.Library.Test.TestContracts;
using System.Collections;



public class Test
{
    
    public void TestMethod()
    {
        CompoundContainingArrayList a = new CompoundContainingArrayList();
        a.Arr = new ArrayList();
        a.Arr.Add(1);
        a.Arr.Add(2);
    }
}
");
        }

        #endregion

        #region Streams

        [TestMethod]
        public void OGStream()
        {
            // Arrange
            this.SetupStandardTypesTest();

            // Act
            CodeVariableReferenceExpression var;
            using (MemoryStream ms = new MemoryStream())
            {
                var = this.sut.GenerateObject("a", typeof(Stream), ms);
            }

            // Assert
            Assert.AreEqual<string>("a", var.VariableName);

            CompareCode(this.ccu, @"//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.1433
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System.IO;



public class Test
{
    
    public void TestMethod()
    {
        Stream a = null;
    }
}
");
        }

        [TestMethod]
        public void OGMemoryStream()
        {
            // Arrange
            this.SetupStandardTypesTest();

            // Act
            CodeVariableReferenceExpression var;
            using (MemoryStream ms = new MemoryStream())
            {
                var = this.sut.GenerateObject("a", typeof(MemoryStream), ms);
            }

            // Assert
            Assert.AreEqual<string>("a", var.VariableName);

            CompareCode(this.ccu, @"//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.1433
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System.IO;



public class Test
{
    
    public void TestMethod()
    {
        MemoryStream a = new MemoryStream();
    }
}
");
        }

        [TestMethod]
        public void OGNestedStream()
        {
            // Arrange
            this.SetupStandardTypesTest();

            NestedStreamMessageContract a = new NestedStreamMessageContract();
            a.Header = 1;
            a.Body = new MemoryStream();

            // Act
            CodeVariableReferenceExpression var = this.sut.GenerateObject("a", typeof(NestedStreamMessageContract), a);

            // Assert
            Assert.AreEqual<string>("a", var.VariableName);

            CompareCode(this.ccu, @"//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.1433
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Microsoft.WcfUnit.Library.Test.TestContracts;
using System;
using System.IO;



public class Test
{
    
    public void TestMethod()
    {
        NestedStreamMessageContract a = new NestedStreamMessageContract();
        a.Header = 1;
        a.Body = null;
    }
}
");
        }

        [TestMethod]
        public void OGNestedMemoryStream()
        {
            // Arrange
            this.SetupStandardTypesTest();

            NestedMemoryStreamMessageContract a = new NestedMemoryStreamMessageContract();
            a.Header = 1;
            a.Body = new MemoryStream();

            // Act
            CodeVariableReferenceExpression var = this.sut.GenerateObject("a", typeof(NestedMemoryStreamMessageContract), a);

            // Assert
            Assert.AreEqual<string>("a", var.VariableName);

            CompareCode(this.ccu, @"//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.1433
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Microsoft.WcfUnit.Library.Test.TestContracts;
using System;
using System.IO;



public class Test
{
    
    public void TestMethod()
    {
        NestedMemoryStreamMessageContract a = new NestedMemoryStreamMessageContract();
        a.Header = 1;
        a.Body = new MemoryStream();
    }
}
");
        }

        // Covers case of service-side traces of streamed requests with non-stream parameters, and out parameters.
        [TestMethod]
        public void OGNonNullableIntWithNullValue()
        {
            // Arrange
            this.SetupStandardTypesTest();

            // Act
            CodeVariableReferenceExpression var = this.sut.GenerateObject("a", typeof(int), null);

            // Assert
            Assert.AreEqual<string>("a", var.VariableName);

            CompareCode(this.ccu, @"//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.1433
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;



public class Test
{
    
    public void TestMethod()
    {
        Int32 a;
    }
}
");
        }
        #endregion

        #region Derived types

        [TestMethod]
        public void OGDerivedKnownType1()
        {
            // Arrange
            this.SetupStandardTypesTest();

            DerivedClass1Contract a = new DerivedClass1Contract();
            a.A = 99;
            a.A1 = 10;

            // Act
            CodeVariableReferenceExpression var = this.sut.GenerateObject("a", typeof(BaseClassContract), a);

            // Assert
            Assert.AreEqual<string>("a", var.VariableName);

            CompareCode(this.ccu, @"//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.1433
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Microsoft.WcfUnit.Library.Test.TestContracts;
using System;



public class Test
{
    
    public void TestMethod()
    {
        DerivedClass1Contract a = new DerivedClass1Contract();
        a.A1 = 10;
        a.A = 99;
    }
}
");
        }

        [TestMethod]
        public void OGDerivedKnownType2()
        {
            // Arrange
            this.SetupStandardTypesTest();

            DerivedClass2Contract a = new DerivedClass2Contract();
            a.A = 99;
            a.A2 = 10;

            // Act
            CodeVariableReferenceExpression var = this.sut.GenerateObject("a", typeof(BaseClassContract), a);

            // Assert
            Assert.AreEqual<string>("a", var.VariableName);

            CompareCode(this.ccu, @"//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.1433
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Microsoft.WcfUnit.Library.Test.TestContracts;
using System;



public class Test
{
    
    public void TestMethod()
    {
        DerivedClass2Contract a = new DerivedClass2Contract();
        a.A2 = 10;
        a.A = 99;
    }
}
");
        }

        [TestMethod]
        public void OGBaseTypeAssignedADerivedType()
        {
            // Arrange
            this.SetupStandardTypesTest();

            DerivedClass1Contract temp1 = new DerivedClass1Contract();
            temp1.A = 23;
            temp1.A1 = 11;
            BaseClassContract a = temp1;

            // Act
            CodeVariableReferenceExpression var = this.sut.GenerateObject("a", typeof(BaseClassContract), a);

            // Assert
            Assert.AreEqual<string>("a", var.VariableName);

            CompareCode(this.ccu, @"//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.1433
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Microsoft.WcfUnit.Library.Test.TestContracts;
using System;



public class Test
{
    
    public void TestMethod()
    {
        DerivedClass1Contract a = new DerivedClass1Contract();
        a.A1 = 11;
        a.A = 23;
    }
}
");
        }

        [TestMethod]
        public void OGBaseTypeAssignedADerivedTypeInACompoundType()
        {
            // Arrange
            this.SetupStandardTypesTest();

            CompoundClassContainingADerivedClass a = new CompoundClassContainingADerivedClass();
            DerivedClass1Contract temp1 = new DerivedClass1Contract();
            temp1.A1 = 67;
            temp1.A = 23;
            a.A1 = temp1;

            // Act
            CodeVariableReferenceExpression var = this.sut.GenerateObject("a", typeof(CompoundClassContainingADerivedClass), a);

            // Assert
            Assert.AreEqual<string>("a", var.VariableName);

            CompareCode(this.ccu, @"//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.1433
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Microsoft.WcfUnit.Library.Test.TestContracts;
using System;



public class Test
{
    
    public void TestMethod()
    {
        CompoundClassContainingADerivedClass a = new CompoundClassContainingADerivedClass();
        DerivedClass1Contract temp0 = new DerivedClass1Contract();
        temp0.A1 = 67;
        temp0.A = 23;
        a.A1 = temp0;
    }
}
");
        }

        [TestMethod]
        public void OGBaseTypeIsObjectPassedASimpleType()
        {
            // Arrange
            this.SetupStandardTypesTest();

            object a = 77;

            // Act
            CodeVariableReferenceExpression var = this.sut.GenerateObject("a", typeof(object), a);

            // Assert
            Assert.AreEqual<string>("a", var.VariableName);

            CompareCode(this.ccu, @"//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.1433
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;



public class Test
{
    
    public void TestMethod()
    {
        Int32 a = 77;
    }
}
");
        }

        [TestMethod]
        public void OGBaseTypeIsObjectPassedADateTime()
        {
            // Arrange
            this.SetupStandardTypesTest();

            object a = new DateTime(2009, 2, 4);

            // Act
            CodeVariableReferenceExpression var = this.sut.GenerateObject("a", typeof(object), a);

            // Assert
            Assert.AreEqual<string>("a", var.VariableName);

            CompareCode(this.ccu, @"//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.1433
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;



public class Test
{
    
    public void TestMethod()
    {
        DateTime a = new DateTime(2009, 2, 4, 0, 0, 0, 0);
    }
}
");
        }

        [TestMethod]
        public void OGBaseTypeIsObjectPassedASimpleCompoundDataContractType()
        {
            // Arrange
            this.SetupStandardTypesTest();

            SimpleCompoundTypeDataContract a = new SimpleCompoundTypeDataContract();
            a.Property1 = "hello";
            a.Property2 = 0;
            a.Field1 = 1.23M;
            a.Field2 = null;

            // Act
            CodeVariableReferenceExpression var = this.sut.GenerateObject("a", typeof(object), a);

            // Assert
            Assert.AreEqual<string>("a", var.VariableName);

            CompareCode(this.ccu, @"//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.1433
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Microsoft.WcfUnit.Library.Test.TestContracts;
using System;



public class Test
{
    
    public void TestMethod()
    {
        SimpleCompoundTypeDataContract a = new SimpleCompoundTypeDataContract();
        a.Property1 = ""hello"";
        a.Property2 = 0;
        a.Field1 = 1.23m;
        a.Field2 = null;
    }
}
");
        }

        [TestMethod]
        public void OGArrayOfBaseTypeAssignedADerivedType()
        {
            // Arrange
            this.SetupStandardTypesTest();

            BaseClassContract[] a = new BaseClassContract[2];
            DerivedClass1Contract temp0 = new DerivedClass1Contract();
            temp0.A = 1;
            temp0.A1 = 2;
            a[0] = temp0;
            a[1] = new BaseClassContract();
            a[1].A = 3;

            // Act
            CodeVariableReferenceExpression var = this.sut.GenerateObject("a", typeof(BaseClassContract[]), a);

            // Assert
            Assert.AreEqual<string>("a", var.VariableName);

            CompareCode(this.ccu, @"//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.1433
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Microsoft.WcfUnit.Library.Test.TestContracts;
using System;



public class Test
{
    
    public void TestMethod()
    {
        BaseClassContract[] a = new BaseClassContract[2];
        DerivedClass1Contract temp0 = new DerivedClass1Contract();
        temp0.A1 = 2;
        temp0.A = 1;
        a[0] = temp0;
        a[1] = new BaseClassContract();
        a[1].A = 3;
    }
}
");
        }

        #endregion

        #region DataSets

        /// <summary>
        /// Tests the generation of an empty dataset.
        /// </summary>
        [TestMethod]
        public void OGDataSetEmpty()
        {
            // Arrange
            this.SetupStandardTypesTest();

            DataSet a = null;
            try
            {
                a = new DataSet();
                a.Locale = CultureInfo.InvariantCulture;
            }
            finally
            {
                a.Dispose();
            }

            // Act
            CodeVariableReferenceExpression var = this.sut.GenerateObject("a", typeof(DataSet), a);

            // Assert
            Assert.AreEqual<string>("a", var.VariableName);

            CompareCode(this.ccu, @"//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.1433
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System.Data;
using System.Globalization;



public class Test
{
    
    public void TestMethod()
    {
        DataSet a = null;
        try
        {
            a = new DataSet();
            a.Locale = CultureInfo.InvariantCulture;
        }
        finally
        {
            a.Dispose();
        }
    }
}
");
        }

        /// <summary>
        /// Tests the generation of an empty dataset in a different locale.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "InDifferent", Justification = "Valid use of two words.")]
        [TestMethod]
        public void OGDataSetEmptyInDifferentLocale()
        {
            // Arrange
            this.SetupStandardTypesTest();

            DataSet a = null;
            try
            {
                a = new DataSet();
                a.Locale = new CultureInfo("en-GB");
            }
            finally
            {
                a.Dispose();
            }

            // Act
            CodeVariableReferenceExpression var = this.sut.GenerateObject("a", typeof(DataSet), a);

            // Assert
            Assert.AreEqual<string>("a", var.VariableName);

            CompareCode(this.ccu, @"//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.1433
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System.Data;
using System.Globalization;



public class Test
{
    
    public void TestMethod()
    {
        DataSet a = null;
        try
        {
            a = new DataSet();
            a.Locale = new CultureInfo(""en-GB"");
        }
        finally
        {
            a.Dispose();
        }
    }
}
");
        }

        /// <summary>
        /// Tests the generation of a dataset inside a class.
        /// </summary>
        [TestMethod]
        public void OGDataSetInsideACompoundType()
        {
            // Arrange
            this.SetupStandardTypesTest();

            CompoundContainingDataSet a = new CompoundContainingDataSet();
            try
            {
                a.Data = new DataSet();
                a.Data.Locale = CultureInfo.InvariantCulture;
            }
            finally
            {
                a.Data.Dispose();
            }

            // Act
            CodeVariableReferenceExpression var = this.sut.GenerateObject("a", typeof(CompoundContainingDataSet), a);

            // Assert
            Assert.AreEqual<string>("a", var.VariableName);

            CompareCode(this.ccu, @"//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.1433
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Microsoft.WcfUnit.Library.Test.TestContracts;
using System.Data;
using System.Globalization;



public class Test
{
    
    public void TestMethod()
    {
        CompoundContainingDataSet a = new CompoundContainingDataSet();
        try
        {
            a.Data = new DataSet();
            a.Data.Locale = CultureInfo.InvariantCulture;
        }
        finally
        {
            a.Data.Dispose();
        }
    }
}
");
        }

        /// <summary>
        /// Tests the generation of two datasets inside a class.
        /// </summary>
        [TestMethod]
        public void OGTwoDataSetsInsideACompoundType()
        {
            // Arrange
            this.SetupStandardTypesTest();

            CompoundContainingDataSets a = new CompoundContainingDataSets();
            try
            {
                a.Data1 = new DataSet();
                a.Data1.Locale = CultureInfo.InvariantCulture;
                try
                {
                    a.Data2 = new DataSet();
                    a.Data2.Locale = CultureInfo.InvariantCulture;
                }
                finally
                {
                    a.Data2.Dispose();
                }
            }
            finally
            {
                a.Data1.Dispose();
            }

            // Act
            CodeVariableReferenceExpression var = this.sut.GenerateObject("a", typeof(CompoundContainingDataSets), a);

            // Assert
            Assert.AreEqual<string>("a", var.VariableName);

            CompareCode(this.ccu, @"//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.1433
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Microsoft.WcfUnit.Library.Test.TestContracts;
using System.Data;
using System.Globalization;



public class Test
{
    
    public void TestMethod()
    {
        CompoundContainingDataSets a = new CompoundContainingDataSets();
        try
        {
            a.Data1 = new DataSet();
            a.Data1.Locale = CultureInfo.InvariantCulture;
            try
            {
                a.Data2 = new DataSet();
                a.Data2.Locale = CultureInfo.InvariantCulture;
            }
            finally
            {
                a.Data2.Dispose();
            }
        }
        finally
        {
            a.Data1.Dispose();
        }
    }
}
");
        }

        /// <summary>
        /// Tests the generation of a dataset with an empty unnamed table
        /// </summary>
        [TestMethod]
        public void OGDataSetOneEmptyUnnamedDataTable()
        {
            // Arrange
            this.SetupStandardTypesTest();

            DataSet a = null;
            try
            {
                a = new DataSet();
                a.Locale = CultureInfo.InvariantCulture;
                a.Tables.Add(new DataTable());
                a.Tables[0].Locale = CultureInfo.InvariantCulture;
            }
            finally
            {
                a.Dispose();
            }

            // Act
            CodeVariableReferenceExpression var = this.sut.GenerateObject("a", typeof(DataSet), a);

            // Assert
            Assert.AreEqual<string>("a", var.VariableName);

            CompareCode(this.ccu, @"//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.1433
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System.Data;
using System.Globalization;



public class Test
{
    
    public void TestMethod()
    {
        DataSet a = null;
        try
        {
            a = new DataSet();
            a.Locale = CultureInfo.InvariantCulture;
            a.Tables.Add(new DataTable());
            a.Tables[0].Locale = CultureInfo.InvariantCulture;
        }
        finally
        {
            a.Dispose();
        }
    }
}
");
        }

        /// <summary>
        /// Tests the generation of a dataset with an unnamed table inside a class.
        /// </summary>
        [TestMethod]
        public void OGDataSetWithUnnamedDataTableInsideACompoundType()
        {
            // Arrange
            this.SetupStandardTypesTest();

            CompoundContainingDataSet a = new CompoundContainingDataSet();
            try
            {
                a.Data = new DataSet();
                a.Data.Locale = CultureInfo.InvariantCulture;
                a.Data.Tables.Add(new DataTable());
                a.Data.Tables[0].Locale = CultureInfo.InvariantCulture;
            }
            finally
            {
                a.Data.Dispose();
            }

            // Act
            CodeVariableReferenceExpression var = this.sut.GenerateObject("a", typeof(CompoundContainingDataSet), a);

            // Assert
            Assert.AreEqual<string>("a", var.VariableName);

            CompareCode(this.ccu, @"//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.1433
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Microsoft.WcfUnit.Library.Test.TestContracts;
using System.Data;
using System.Globalization;



public class Test
{
    
    public void TestMethod()
    {
        CompoundContainingDataSet a = new CompoundContainingDataSet();
        try
        {
            a.Data = new DataSet();
            a.Data.Locale = CultureInfo.InvariantCulture;
            a.Data.Tables.Add(new DataTable());
            a.Data.Tables[0].Locale = CultureInfo.InvariantCulture;
        }
        finally
        {
            a.Data.Dispose();
        }
    }
}
");
        }

        /// <summary>
        /// Tests the generation of a dataset with two empty unnamed tables.
        /// </summary>
        [TestMethod]
        public void OGDataSetTwoEmptyUnnamedDataTables()
        {
            // Arrange
            this.SetupStandardTypesTest();

            DataSet a = null;
            try
            {
                a = new DataSet();
                a.Locale = CultureInfo.InvariantCulture;
                a.Tables.Add(new DataTable());
                a.Tables[0].Locale = CultureInfo.InvariantCulture;
                a.Tables.Add(new DataTable());
                a.Tables[1].Locale = CultureInfo.InvariantCulture;
            }
            finally
            {
                a.Dispose();
            }

            // Act
            CodeVariableReferenceExpression var = this.sut.GenerateObject("a", typeof(DataSet), a);

            // Assert
            Assert.AreEqual<string>("a", var.VariableName);

            CompareCode(this.ccu, @"//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.1433
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System.Data;
using System.Globalization;



public class Test
{
    
    public void TestMethod()
    {
        DataSet a = null;
        try
        {
            a = new DataSet();
            a.Locale = CultureInfo.InvariantCulture;
            a.Tables.Add(new DataTable());
            a.Tables[0].Locale = CultureInfo.InvariantCulture;
            a.Tables.Add(new DataTable());
            a.Tables[1].Locale = CultureInfo.InvariantCulture;
        }
        finally
        {
            a.Dispose();
        }
    }
}
");
        }

        /// <summary>
        /// Tests the generation of a dataset with a named table
        /// </summary>
        [TestMethod]
        public void OGDataSetWithNamedDataTable()
        {
            // Arrange
            this.SetupStandardTypesTest();

            DataSet a = null;
            try
            {
                a = new DataSet();
                a.Locale = CultureInfo.InvariantCulture;
                a.Tables.Add(new DataTable("Name1"));
                a.Tables[0].Locale = CultureInfo.InvariantCulture;
            }
            finally
            {
                a.Dispose();
            }

            // Act
            CodeVariableReferenceExpression var = this.sut.GenerateObject("a", typeof(DataSet), a);

            // Assert
            Assert.AreEqual<string>("a", var.VariableName);

            CompareCode(this.ccu, @"//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.1433
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System.Data;
using System.Globalization;



public class Test
{
    
    public void TestMethod()
    {
        DataSet a = null;
        try
        {
            a = new DataSet();
            a.Locale = CultureInfo.InvariantCulture;
            a.Tables.Add(new DataTable(""Name1""));
            a.Tables[0].Locale = CultureInfo.InvariantCulture;
        }
        finally
        {
            a.Dispose();
        }
    }
}
");
        }

        /// <summary>
        /// Tests the generation of a dataset with a named and namespaced table
        /// </summary>
        [TestMethod]
        public void OGDataSetWithNamedAndNamespacedDataTable()
        {
            // Arrange
            this.SetupStandardTypesTest();

            DataSet a = null;
            try
            {
                a = new DataSet();
                a.Locale = CultureInfo.InvariantCulture;
                a.Tables.Add(new DataTable("Name1", "urn:table1"));
                a.Tables[0].Locale = CultureInfo.InvariantCulture;
            }
            finally
            {
                a.Dispose();
            }

            // Act
            CodeVariableReferenceExpression var = this.sut.GenerateObject("a", typeof(DataSet), a);

            // Assert
            Assert.AreEqual<string>("a", var.VariableName);

            CompareCode(this.ccu, @"//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.1433
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System.Data;
using System.Globalization;



public class Test
{
    
    public void TestMethod()
    {
        DataSet a = null;
        try
        {
            a = new DataSet();
            a.Locale = CultureInfo.InvariantCulture;
            a.Tables.Add(new DataTable(""Name1"", ""urn:table1""));
            a.Tables[0].Locale = CultureInfo.InvariantCulture;
        }
        finally
        {
            a.Dispose();
        }
    }
}
");
        }

        /// <summary>
        /// Tests the generation of a dataset with a table containing some columns and data.
        /// </summary>
        [TestMethod]
        public void OGDataSetWithATableContainingColumnsAndData()
        {
            // Arrange
            this.SetupStandardTypesTest();

            DataSet a = null;
            try
            {
                a = new DataSet();
                a.Locale = CultureInfo.InvariantCulture;
                a.Tables.Add(new DataTable());
                a.Tables[0].Locale = CultureInfo.InvariantCulture;
                a.Tables[0].Columns.Add("Col1", typeof(string));
                a.Tables[0].Columns.Add("Col2", typeof(int));
                a.Tables[0].Rows.Add("hello1", 1);
                a.Tables[0].Rows.Add("hello2", 2);
                a.Tables[0].Rows.Add("hello3", 3);
            }
            finally
            {
                a.Dispose();
            }

            // Act
            CodeVariableReferenceExpression var = this.sut.GenerateObject("a", typeof(DataSet), a);

            // Assert
            Assert.AreEqual<string>("a", var.VariableName);

            CompareCode(this.ccu, @"//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.1433
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Data;
using System.Globalization;



public class Test
{
    
    public void TestMethod()
    {
        DataSet a = null;
        try
        {
            a = new DataSet();
            a.Locale = CultureInfo.InvariantCulture;
            a.Tables.Add(new DataTable());
            a.Tables[0].Locale = CultureInfo.InvariantCulture;
            a.Tables[0].Columns.Add(""Col1"", typeof(String));
            a.Tables[0].Columns.Add(""Col2"", typeof(Int32));
            a.Tables[0].Rows.Add(""hello1"", 1);
            a.Tables[0].Rows.Add(""hello2"", 2);
            a.Tables[0].Rows.Add(""hello3"", 3);
        }
        finally
        {
            a.Dispose();
        }
    }
}
");
        }

        /// <summary>
        /// Tests the generation of a dataset with a table containing some columns and data, with data containing non-primitive types.
        /// </summary>
        [TestMethod]
        public void OGDataSetWithATableContainingColumnsAndNonPrimitiveData()
        {
            // Arrange
            this.SetupStandardTypesTest();

            DataSet a = null;
            try
            {
                a = new DataSet();
                a.Locale = CultureInfo.InvariantCulture;
                a.Tables.Add(new DataTable());
                a.Tables[0].Locale = CultureInfo.InvariantCulture; 
                a.Tables[0].Columns.Add("Col1", typeof(DateTime));
                a.Tables[0].Columns.Add("Col2", typeof(Guid));
                a.Tables[0].Rows.Add(new DateTime(2010, 6, 25), new Guid("A2BF47AC-3431-4f60-B572-94E91D6D8E0E"));
            }
            finally
            {
                a.Dispose();
            }

            // Act
            CodeVariableReferenceExpression var = this.sut.GenerateObject("a", typeof(DataSet), a);

            // Assert
            Assert.AreEqual<string>("a", var.VariableName);

            CompareCode(this.ccu, @"//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.1433
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Data;
using System.Globalization;



public class Test
{
    
    public void TestMethod()
    {
        DataSet a = null;
        try
        {
            a = new DataSet();
            a.Locale = CultureInfo.InvariantCulture;
            a.Tables.Add(new DataTable());
            a.Tables[0].Locale = CultureInfo.InvariantCulture;
            a.Tables[0].Columns.Add(""Col1"", typeof(DateTime));
            a.Tables[0].Columns.Add(""Col2"", typeof(Guid));
            a.Tables[0].Rows.Add(new DateTime(2010, 6, 25, 0, 0, 0, 0), new Guid(""a2bf47ac-3431-4f60-b572-94e91d6d8e0e""));
        }
        finally
        {
            a.Dispose();
        }
    }
}
");
        }

        /// <summary>
        /// Tests the generation of a dataset with a table containing some columns and data that contains nulls.
        /// </summary>
        [TestMethod]
        public void OGDataSetWithATableContainingColumnsAndDataWithNullValues()
        {
            // Arrange
            this.SetupStandardTypesTest();

            DataSet a = null;
            try
            {
                a = new DataSet();
                a.Locale = CultureInfo.InvariantCulture;
                a.Tables.Add(new DataTable());
                a.Tables[0].Locale = CultureInfo.InvariantCulture;
                a.Tables[0].Columns.Add("Col1", typeof(string));
                a.Tables[0].Columns.Add("Col2", typeof(int));
                a.Tables[0].Rows.Add("hello1", null);
                a.Tables[0].Rows.Add("hello2", DBNull.Value);
            }
            finally
            {
                a.Dispose();
            }

            // Act
            CodeVariableReferenceExpression var = this.sut.GenerateObject("a", typeof(DataSet), a);

            // Assert
            Assert.AreEqual<string>("a", var.VariableName);

            CompareCode(this.ccu, @"//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.1433
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Data;
using System.Globalization;



public class Test
{
    
    public void TestMethod()
    {
        DataSet a = null;
        try
        {
            a = new DataSet();
            a.Locale = CultureInfo.InvariantCulture;
            a.Tables.Add(new DataTable());
            a.Tables[0].Locale = CultureInfo.InvariantCulture;
            a.Tables[0].Columns.Add(""Col1"", typeof(String));
            a.Tables[0].Columns.Add(""Col2"", typeof(Int32));
            a.Tables[0].Rows.Add(""hello1"", DBNull.Value);
            a.Tables[0].Rows.Add(""hello2"", DBNull.Value);
        }
        finally
        {
            a.Dispose();
        }
    }
}
");
        }

        /// <summary>
        /// Tests the generation of a typed dataset with a table containing some columns and data.
        /// </summary>
        [TestMethod]
        public void OGDataSetSimpleTypedDataSetWithData()
        {
            // Arrange
            this.SetupStandardTypesTest();

            TypedDataSet a = null;
            try
            {
                a = new TypedDataSet();
                a.Locale = CultureInfo.InvariantCulture;
                a.DataTable1.Locale = CultureInfo.InvariantCulture;
                a.DataTable1.AddDataTable1Row("hello1", 1);
                a.DataTable1.AddDataTable1Row("hello2", 2);
                a.DataTable1.AddDataTable1Row("hello3", 3);
            }
            finally
            {
                a.Dispose();
            }

            // Act
            CodeVariableReferenceExpression var = this.sut.GenerateObject("a", typeof(TypedDataSet), a);

            // Assert
            Assert.AreEqual<string>("a", var.VariableName);

            CompareCode(this.ccu, @"//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.1433
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Microsoft.WcfUnit.Library.Test.TestContracts;
using System.Globalization;



public class Test
{
    
    public void TestMethod()
    {
        TypedDataSet a = null;
        try
        {
            a = new TypedDataSet();
            a.Locale = CultureInfo.InvariantCulture;
            a.Tables[0].Locale = CultureInfo.InvariantCulture;
            a.Tables[0].Rows.Add(""hello1"", 1);
            a.Tables[0].Rows.Add(""hello2"", 2);
            a.Tables[0].Rows.Add(""hello3"", 3);
        }
        finally
        {
            a.Dispose();
        }
    }
}
");
        }

        /// <summary>
        /// Tests the generation of a typed dataset in a compound type.
        /// </summary>
        [TestMethod]
        public void OGDataSetCompoundTypedDataSetWithData()
        {
            // Arrange
            this.SetupStandardTypesTest();

            CompoundContainingTypedDataSet a = new CompoundContainingTypedDataSet();
            try
            {
                a.Data = new TypedDataSet();
                a.Data.Locale = CultureInfo.InvariantCulture;
                a.Data.DataTable1.Locale = CultureInfo.InvariantCulture;
                a.Data.DataTable1.AddDataTable1Row("hello1", 1);
                a.Data.DataTable1.AddDataTable1Row("hello2", 2);
                a.Data.DataTable1.AddDataTable1Row("hello3", 3);
            }
            finally
            {
                a.Data.Dispose();
            }

            // Act
            CodeVariableReferenceExpression var = this.sut.GenerateObject("a", typeof(CompoundContainingTypedDataSet), a);

            // Assert
            Assert.AreEqual<string>("a", var.VariableName);

            CompareCode(this.ccu, @"//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.1433
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Microsoft.WcfUnit.Library.Test.TestContracts;
using System.Globalization;



public class Test
{
    
    public void TestMethod()
    {
        CompoundContainingTypedDataSet a = new CompoundContainingTypedDataSet();
        try
        {
            a.Data = new TypedDataSet();
            a.Data.Locale = CultureInfo.InvariantCulture;
            a.Data.Tables[0].Locale = CultureInfo.InvariantCulture;
            a.Data.Tables[0].Rows.Add(""hello1"", 1);
            a.Data.Tables[0].Rows.Add(""hello2"", 2);
            a.Data.Tables[0].Rows.Add(""hello3"", 3);
        }
        finally
        {
            a.Data.Dispose();
        }
    }
}
");
        }
        
        // TODO: integration test with typed datasets and with compound typed and untyped datasets. Use nulls for some values too. Make sure test asmx, service side too.
        // left it editing asmx service to mirror all the wcf methods, then need to re-gen proxies with host and asmx running, then update clients for all calls.
        #endregion

        private static void AddSimpleType(ISerializationInfo mockSi, Type type)
        {
            Expect
                .Call(mockSi.IsSimpleType(type))
                .Return(true)
                .Repeat
                .Any();
        }

        private static void AddNonSimpleType(ISerializationInfo mockSi, Type type)
        {
            Expect
                .Call(mockSi.IsSimpleType(type))
                .Return(false)
                .Repeat
                .Any();
        }

        private static void AddDerivableCollectionTypeWithNoMembers(ISerializationInfo mockSi, Type type)
        {
            AddCompoundType(mockSi, type); // Derivable collections get checked for other members
        }

        private static void AddCompoundType(ISerializationInfo mockSi, Type type, params string[] memberNames)
        {
            AddNonSimpleType(mockSi, type);

            AddSerializableType(mockSi, type);

            MemberInfo[] members = new MemberInfo[memberNames.Length];
            for (int i = 0; i < memberNames.Length; i++)
            {
                members[i] = type.GetMember(memberNames[i])[0];
            }

            Expect
                .Call(mockSi.SerializableMembers(type))
                .Return(members)
                .Repeat
                .Any();
        }

        private static void AddSerializableType(ISerializationInfo mockSi, Type type)
        {
            Expect
                .Call(mockSi.IsSerializable(type))
                .Return(true)
                .Repeat
                .Any();
        }

        private static void AddNonSerializableType(ISerializationInfo mockSi, Type type)
        {
            Expect
                .Call(mockSi.IsSerializable(type))
                .Return(false)
                .Repeat
                .Any();
        }

        private static void AddNotSerializableErrorMessage(ISerializationInfo mockSi)
        {
            Expect
                .Call(mockSi.NotSerializableError)
                .Return("test error")
                .Repeat
                .Any();
        }

        private static void CompareCode(CodeCompileUnit ccu, string expectedCode)
        {
            using (CodeDomProvider provider = new Microsoft.CSharp.CSharpCodeProvider())
            {
                StringBuilder actualCode = new StringBuilder();
                StringWriter sw = null;
                try
                {
                    sw = new StringWriter(actualCode, CultureInfo.InvariantCulture);
                    using (IndentedTextWriter tw = new IndentedTextWriter(sw, "    "))
                    {
                        sw = null;
                        CodeGeneratorOptions cgo = new CodeGeneratorOptions();
                        cgo.BracingStyle = "C";
                        provider.GenerateCodeFromCompileUnit(ccu, tw, cgo);
                    }
                }
                finally
                {
                    if (sw != null)
                    {
                        sw.Dispose();
                    }
                }

                TestHelper.CompareCode(expectedCode, actualCode.ToString());
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Testing for many types.")]
        private void SetupStandardTypesTest()
        {
            ISerializationInfo mockSi = this.mocks.DynamicMock<ISerializationInfo>();

            AddNotSerializableErrorMessage(mockSi);

            AddSimpleType(mockSi, typeof(TestEnum));
            AddSimpleType(mockSi, typeof(int));
            AddSimpleType(mockSi, typeof(string));
            AddSimpleType(mockSi, typeof(decimal));
            AddSimpleType(mockSi, typeof(DateTime));
            AddSimpleType(mockSi, typeof(TimeSpan));
            AddSimpleType(mockSi, typeof(Guid));
            AddSimpleType(mockSi, typeof(Uri));
            AddSimpleType(mockSi, typeof(XmlQualifiedName));

            AddSimpleType(mockSi, typeof(Nullable<int>));
            AddSimpleType(mockSi, typeof(Nullable<DateTime>));
            AddSimpleType(mockSi, typeof(Nullable<CustomValueType>));

            AddNonSimpleType(mockSi, typeof(XmlNode[]));
            AddNonSimpleType(mockSi, typeof(Stream));
            AddNonSimpleType(mockSi, typeof(MemoryStream));
            AddNonSimpleType(mockSi, typeof(BaseClassContract[]));

            AddDerivableCollectionTypeWithNoMembers(mockSi, typeof(Collection<int>));
            AddDerivableCollectionTypeWithNoMembers(mockSi, typeof(Collection<DateTime>));
            AddDerivableCollectionTypeWithNoMembers(mockSi, typeof(int[]));
            AddDerivableCollectionTypeWithNoMembers(mockSi, typeof(List<int>));
            AddDerivableCollectionTypeWithNoMembers(mockSi, typeof(IList<int>));
            AddDerivableCollectionTypeWithNoMembers(mockSi, typeof(Collection<SimpleCompoundTypeDataContract>));
            AddDerivableCollectionTypeWithNoMembers(mockSi, typeof(SimpleCompoundTypeDataContract[]));
            AddDerivableCollectionTypeWithNoMembers(mockSi, typeof(Dictionary<string, int>));
            AddDerivableCollectionTypeWithNoMembers(mockSi, typeof(Dictionary<Guid, DateTime>));
            AddDerivableCollectionTypeWithNoMembers(mockSi, typeof(Dictionary<string, SimpleCompoundTypeDataContract>));
            AddDerivableCollectionTypeWithNoMembers(mockSi, typeof(Dictionary<SimpleCompoundTypeDataContract, SimpleCompoundTypeDataContract>));
            AddDerivableCollectionTypeWithNoMembers(mockSi, typeof(Dictionary<string, CompoundWithRecursiveDictionary>));
            AddDerivableCollectionTypeWithNoMembers(mockSi, typeof(Hashtable));
            AddDerivableCollectionTypeWithNoMembers(mockSi, typeof(ArrayList));

            AddCompoundType(mockSi, typeof(CustomValueType), "A", "B");
            AddCompoundType(mockSi, typeof(SimpleCompoundTypeDataContract), "Property1", "Property2", "Field1", "Field2");
            AddCompoundType(mockSi, typeof(NestedCompoundType), "Compound1", "Compound2");
            AddCompoundType(mockSi, typeof(CompoundWithArrayType), "Arr");
            AddCompoundType(mockSi, typeof(CompoundTypeWithXmlElement), "Element");
            AddCompoundType(mockSi, typeof(CompoundTypeWithXmlNodeArray), "Nodes");
            AddCompoundType(mockSi, typeof(CompoundWithGenericSimpleCollection), "Arr");
            AddCompoundType(mockSi, typeof(CompoundWithGenericSimpleICollection), "Arr");
            AddCompoundType(mockSi, typeof(CompoundWithGenericCompoundCollection), "Arr");
            AddCompoundType(mockSi, typeof(CompoundWithGenericCompoundICollection), "Arr");
            AddCompoundType(mockSi, typeof(CompoundWithGenericSimpleDictionary), "D");
            AddCompoundType(mockSi, typeof(CompoundWithGenericSimpleIDictionary), "D");
            AddCompoundType(mockSi, typeof(CompoundWithGenericCompoundDictionary), "D");
            AddCompoundType(mockSi, typeof(CompoundWithGenericCompoundBothSidesDictionary), "D");
            AddCompoundType(mockSi, typeof(CompoundWithRecursiveDictionary), "D");
            AddCompoundType(mockSi, typeof(NonGenericEnumerableOnlyCollection));
            AddCompoundType(mockSi, typeof(CompoundContainingNonGenericEnumerableOnlyCollection), "Arr");
            AddCompoundType(mockSi, typeof(ClassWithSerializableMemberOfNonSerializableType), "NonSerializableTypeMember");
            AddCompoundType(mockSi, typeof(CyclicStructure), "BackRef");
            AddCompoundType(mockSi, typeof(CompoundContainingNonGenericDictionary), "Arr");
            AddCompoundType(mockSi, typeof(CompoundContainingArrayList), "Arr");
            AddCompoundType(mockSi, typeof(NestedStreamMessageContract), "Header", "Body");
            AddCompoundType(mockSi, typeof(NestedMemoryStreamMessageContract), "Header", "Body");
            AddCompoundType(mockSi, typeof(GenericListWrapper));
            AddCompoundType(mockSi, typeof(GenericListWrapperWithOtherMembers), "Property");
            AddCompoundType(mockSi, typeof(GenericListWrapperWithNestedWrapperThatHasOtherMembers), "Nested");
            AddCompoundType(mockSi, typeof(GenericDictionaryWrapper));
            AddCompoundType(mockSi, typeof(GenericDictionaryWrapperWithOtherMembers), "Property");
            AddCompoundType(mockSi, typeof(GenericDictionaryWrapperWithNestedWrapperThatHasOtherMembers), "Nested");
            AddCompoundType(mockSi, typeof(BaseClassContract), "A");
            AddCompoundType(mockSi, typeof(DerivedClass1Contract), "A1", "A");
            AddCompoundType(mockSi, typeof(DerivedClass2Contract), "A2", "A");
            AddCompoundType(mockSi, typeof(CompoundClassContainingADerivedClass), "A1");
            AddCompoundType(mockSi, typeof(CompoundContainingDataSet), "Data");
            AddCompoundType(mockSi, typeof(CompoundContainingDataSets), "Data1", "Data2");
            AddCompoundType(mockSi, typeof(CompoundContainingTypedDataSet), "Data");

            AddNonSerializableType(mockSi, typeof(NonDataContractClass));
            AddNonSerializableType(mockSi, typeof(DerivedClass3NoContract));

            this.CreateObjectGenerator(mockSi);
            this.mocks.ReplayAll();
        }

        /// <summary>
        /// Creates the object generator to be tested.
        /// </summary>
        /// <param name="mockSi">The mock serialization information.</param>
        private void CreateObjectGenerator(ISerializationInfo mockSi)
        {
            this.ccu = new CodeCompileUnit();
            CodeNamespace testNamespace = new CodeNamespace();
            this.ccu.Namespaces.Add(testNamespace);
            CodeTypeDeclaration testType = new CodeTypeDeclaration("Test");
            testNamespace.Types.Add(testType);
            testType.IsClass = true;
            testType.TypeAttributes = TypeAttributes.Public;

            CodeMemberMethod testMethod = new CodeMemberMethod();
            testMethod.Name = "TestMethod";
            testMethod.Attributes = MemberAttributes.Public | MemberAttributes.Final;

            testType.Members.Add(testMethod);

            this.imports = testNamespace.Imports;
            this.methodBody = testMethod.Statements;
            this.sut = new ObjectGenerator(this.methodBody, mockSi);
            this.sut.CodeTypeRequestEvent += new EventHandler<CodeTypeReferenceRequestEventArgs>(this.CodeTypeReferenceRequestEventHandler);
            this.useFullQualification = false;
        }

        private void CodeTypeReferenceRequestEventHandler(object sender, CodeTypeReferenceRequestEventArgs o)
        {
            if (this.useFullQualification)
            {
                o.CodeTypeReference = new CodeTypeReference(o.RequestedType);
            }
            else
            {
                bool alreadyImported = false;
                foreach (CodeNamespaceImport import in this.imports)
                {
                    if (import.Namespace == o.RequestedType.Namespace)
                    {
                        alreadyImported = true;
                    }
                }

                if (!alreadyImported)
                {
                    this.imports.Add(new CodeNamespaceImport(o.RequestedType.Namespace));
                }

                Type t;
                if (o.RequestedType.IsArray)
                {
                    t = o.RequestedType.GetElementType();
                }
                else
                {
                    t = o.RequestedType;
                }

                string typeName = string.Empty;
                while (t != null)
                {
                    if (typeName.Length == 0)
                    {
                        typeName = t.Name;
                    }
                    else
                    {
                        typeName = t.Name + "." + typeName;
                    }

                    t = t.DeclaringType;
                }

                if (o.RequestedType.IsArray)
                {
                    typeName += "[]";
                }

                o.CodeTypeReference = new CodeTypeReference(typeName, o.GenericParameters);
            }
        }
    }
}
