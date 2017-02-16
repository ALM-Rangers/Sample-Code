//---------------------------------------------------------------------
// <copyright file="UtilityTests.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
//     THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
//     OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
//     LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR
//     FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
// <summary>The UtilityTests type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.WcfUnit.Library.Test
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.WcfUnit.Library;

    [TestClass]
    public class UtilityTests
    {
        [TestMethod]
        public void UTILScenarioNameValidAllLetters()
        {
            Assert.IsTrue(Utility.IsValidIdentifier("abc"));
        }

        [TestMethod]
        public void UTILScenarioNameValidContainsDigits()
        {
            Assert.IsTrue(Utility.IsValidIdentifier("ab2c"));
        }

        [TestMethod]
        public void UTILScenarioNameValidLeadingUnderscore()
        {
            Assert.IsTrue(Utility.IsValidIdentifier("_abc"));
        }

        [TestMethod]
        public void UTILScenarioNameInvalidStartsWithDigit()
        {
            Assert.IsFalse(Utility.IsValidIdentifier("2abc"));
        }

        [TestMethod]
        public void UTILScenarioNameInvalidContainsPeriod()
        {
            Assert.IsFalse(Utility.IsValidIdentifier("abc.def"));
        }

        [TestMethod]
        public void UTILScenarioNameInvalidContainsDash()
        {
            Assert.IsFalse(Utility.IsValidIdentifier("abc-def"));
        }

        [TestMethod]
        public void UTILMakeSafeIdentifierFromValidIdentifier()
        {
            Assert.AreEqual<string>("abc", Utility.MakeSafeIdentifier("abc"));
        }

        [TestMethod]
        public void UTILMakeSafeIdentifierFromIdentifierWithSpaces()
        {
            Assert.AreEqual<string>("abc", Utility.MakeSafeIdentifier("a b c"));
        }

        [TestMethod]
        public void UTILMakeSafeIdentifierFromIdentifierWithLeadingInvalidCharacters()
        {
            Assert.AreEqual<string>("abc", Utility.MakeSafeIdentifier("222abc"));
        }

        [TestMethod]
        public void UTILMakeSafeIdentifierFromIdentifierWithNoValidCharacters()
        {
            Assert.AreEqual<string>(string.Empty, Utility.MakeSafeIdentifier(" .-"));
        }

        [TestMethod]
        public void UTILToCamelCaseThrowsExceptionForNullArgument()
        {
            TestHelper.TestForArgumentNullException(() => Utility.ToCamelCase(null), "identifier");
        }

        [TestMethod]
        public void UTILToCamelCaseEmptyStringThrowsArgumentNullException()
        {
            TestHelper.TestForArgumentNullException(() => Utility.ToCamelCase(string.Empty), "identifier");
        }

        [TestMethod]
        public void UTILToCamelCaseAllLowercaseReturnsAllLowercase()
        {
            Assert.AreEqual<string>("abc", Utility.ToCamelCase("abc"));
        }

        [TestMethod]
        public void UTILToCamelCaseAllUppercaseReturnsAllLowercaseExceptLast()
        {
            Assert.AreEqual<string>("abC", Utility.ToCamelCase("ABC"));
        }

        [TestMethod]
        public void UTILToCamelCaseLeadingCapsTurnedToLowercaseExceptLastOne()
        {
            Assert.AreEqual<string>("abCdef", Utility.ToCamelCase("ABCdef"));
        }

        [TestMethod]
        public void UTILToCamelCaseSingleCapToLower()
        {
            Assert.AreEqual<string>("a", Utility.ToCamelCase("A"));
        }

        [TestMethod]
        public void UTILToCamelCaseInitialCapIsLowered()
        {
            Assert.AreEqual<string>("abc", Utility.ToCamelCase("Abc"));
        }

        [TestMethod]
        public void UTILToCamelCasePreservedAfterFirstLowercase()
        {
            Assert.AreEqual<string>("abCdEf", Utility.ToCamelCase("ABCdEf"));
        }

        [TestMethod]
        public void UTILGetVersion()
        {
            Assert.AreEqual<string>("4.0.0.0", Utility.ReadVersion());
        }

        /// <summary>
        /// Tests that <see cref="Utility.CreateConfiguredType"/> checks argument for null.
        /// </summary>
        [TestMethod]
        public void UTILCreateConfiguredTypeChecksArgumentForNull()
        {
            TestHelper.TestForArgumentNullException(() =>
                {
                    using (Utility.CreateConfiguredType<IFormatParser>(null))
                    {
                    }
                },
                "configuredTypeName");
        }

        /// <summary>
        /// Tests creating a configured type.
        /// </summary>
        [TestMethod]
        public void UTILCreateConfiguredType()
        {
            // Arrange
            typeType t = TestHelper.CreateConfigTypeForWcfFormatParser();

            // Act
            using (IFormatParser dummy = Utility.CreateConfiguredType<IFormatParser>(t))
            {
                // Assert
                Assert.IsNotNull(dummy);
                Assert.IsInstanceOfType(dummy, typeof(WcfParser));
            }
        }

        /// <summary>
        /// Tests creating a configured type when the assembly does not exist.
        /// </summary>
        [TestMethod]
        public void UTILCreateConfiguredTypeThrowsExceptionIfFileNotFound()
        {
            // Arrange
            typeType t = new typeType();
            t.assembly = "NonExistent";
            t.type = "Microsoft.WcfUnit.Library.WcfParser";

            // Act and assert
            TestHelper.TestForUserException(
                delegate
                {
                    using (IFormatParser dummy = Utility.CreateConfiguredType<IFormatParser>(t))
                    {
                    }
                });
        }

        /// <summary>
        /// Tests creating a configured type when the type does not exist in the assembly.
        /// </summary>
        [TestMethod]
        public void UTILCreateConfiguredTypeThrowsExceptionIfTypeNotFound()
        {
            // Arrange
            typeType t = new typeType();
            t.assembly = TestHelper.LibraryAssembly;
            t.type = "NonExistent";

            // Act and assert
            TestHelper.TestForUserException(
                delegate
                {
                    using (IFormatParser dummy = Utility.CreateConfiguredType<IFormatParser>(t))
                    {
                    }
                });
        }

        /// <summary>
        /// Tests creating a configured type when the type is not of the right type.
        /// </summary>
        [TestMethod]
        public void UTILCreateConfiguredTypeThrowsExceptionIfTypeDoesNotMatch()
        {
            // Arrange
            typeType t = new typeType();
            t.assembly = TestHelper.LibraryAssembly;
            t.type = "Microsoft.WcfUnit.Library.FiddlerTextParser";

            // Act and assert
            TestHelper.TestForUserException(
                delegate
                {
                    using (WcfParser dummy = Utility.CreateConfiguredType<WcfParser>(t))
                    {
                    }
                });
        }

        /// <summary>
        /// Tests creating a configured type when the type does not have a parameter-less constructor.
        /// </summary>
        [TestMethod]
        public void UTILCreateConfiguredTypeThrowsExceptionIfTypeDoesNotHaveParameterlessConstructor()
        {
            // Arrange
            typeType t = new typeType();
            t.assembly = TestHelper.LibraryAssembly;
            t.type = "Microsoft.WcfUnit.Library.Parser";

            // Act and assert
            TestHelper.TestForUserException(
                delegate
                {
                    using (Parser dummy = Utility.CreateConfiguredType<Parser>(t))
                    {
                    }
                });
        }
    }
}
