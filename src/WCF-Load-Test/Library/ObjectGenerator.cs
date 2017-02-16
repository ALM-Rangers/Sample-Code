//---------------------------------------------------------------------
// <copyright file="ObjectGenerator.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
//     THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
//     OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
//     LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR
//     FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
// <summary>The ObjectGenerator type.</summary>
//---------------------------------------------------------------------

// TODO: Change ObjectGenerator to accept parameter direction so that it does not even initialise out parameters
namespace Microsoft.WcfUnit.Library
{
    using System;
    using System.CodeDom;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Reflection;
    using System.Xml;

    /// <summary>
    /// Generates code for setting up objects based on in-memory objects.
    /// </summary>
    /// <remarks>
    /// The class is given an instance of an object and it creates code that will create
    /// the same object. The class uses the CodeDOM namespace to generate code. The class
    /// avoids using fully-qualified types because the generated code has to be readable.
    /// </remarks>
    public class ObjectGenerator
    {
        /// <summary>
        /// The maximum level of nesting before generation is stopped because of possible cycles in the object being generated.
        /// </summary>
        private const int MaxNestingLevel = 20;

        /// <summary>
        /// Provides serialization information for the object being generated.
        /// </summary>
        private ISerializationInfo serializationInfo;

        /// <summary>
        /// Nesting level used for detecting possible cycles.
        /// </summary>
        private int level;

        /// <summary>
        /// Stores a sequence number for the generation of temporary variable names.
        /// </summary>
        private int tempVariableSequenceNumber;

        /// <summary>
        /// Initialises a new instance of the <see cref="ObjectGenerator"/> class.
        /// </summary>
        /// <param name="body">The body to add the code to</param>
        /// <param name="serializationInfo">The object which provides serialization information about the object to generated.</param>
        public ObjectGenerator(CodeStatementCollection body, ISerializationInfo serializationInfo)
        {
            this.serializationInfo = serializationInfo;
            this.Body = body;
        }

        /// <summary>
        /// Raised to request a <see cref="CodeTypeReference"/> object for a type.
        /// </summary>
        public event EventHandler<CodeTypeReferenceRequestEventArgs> CodeTypeRequestEvent;

        /// <summary>
        /// Gets the body of statements to which the object generation code is to be added.
        /// </summary>
        public CodeStatementCollection Body { get; private set; } // TODO: unit tests for this

        /// <summary>
        /// Generates the code to create the given object
        /// </summary>
        /// <param name="name">The name of the variable in the generated code to be assigned the value of the object</param>
        /// <param name="objectType">The type of the object to be generated</param>
        /// <param name="o">The object to be generated</param>
        /// <returns>The reference to the generated variable which will contain the generated object</returns>
        /// <remarks>
        /// The type of the object has to be passed in because if the value is null we can't get the
        /// type from the object itself.
        /// </remarks>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "o", Justification = "Happy with this name")]
        public CodeVariableReferenceExpression GenerateObject(string name, Type objectType, object o)
        {
            Trace.WriteLine(string.Format(CultureInfo.InvariantCulture, "Generating assignment to {0}", name));
            Trace.Indent();
            CodeVariableReferenceExpression ans;

            this.EnterLevel();
            try
            {
                Type mappedObjectType = CheckAndMapExpectedType(objectType, o);
                CodeTypeReference objectTypeReference = this.ProcessObjectType(mappedObjectType);

                if (o == null || this.serializationInfo.IsSimpleType(mappedObjectType))
                {
                    this.GenerateSimpleAssignment(name, mappedObjectType, o, objectTypeReference);
                }
                else if (mappedObjectType.IsArray)
                {
                    this.GenerateArrayAssignment(name, o, objectTypeReference);
                }
                else if (IsGenericDictionary(objectType) || IsNonGenericDictionary(objectType))
                {
                    this.GenerateDictionaryAssignment(name, o, objectTypeReference);
                }
                else if (IsGenericCollection(objectType) || IsNonGenericCollection(objectType))
                {
                    this.GenerateCollectionAssignment(name, o, objectTypeReference);
                }
                else if (o is XmlNode)
                {
                    this.GenerateXmlAssignment(name, o);
                }
                else if (o is DataSet)
                {
                    this.GenerateDataSetAssignment(name, o);
                }
                else if (IsStream(objectType))
                {
                    this.GenerateStream(name, objectTypeReference);
                }
                else
                {
                    if (!this.serializationInfo.IsSerializable(mappedObjectType))
                    {
                        throw new UserException(string.Format(CultureInfo.CurrentCulture, this.serializationInfo.NotSerializableError, mappedObjectType.FullName));
                    }

                    this.GenerateCompoundAssignment(name, o, objectTypeReference);
                }

                ans = new CodeVariableReferenceExpression(name);
            }
            finally
            {
                this.ExitLevel();
                Trace.Unindent();
            }

            return ans;
        }

        /// <summary>
        /// Checks to see if a type is a nullable type.
        /// </summary>
        /// <param name="objectType">The type to be checked.</param>
        /// <returns>True if <paramref name="objectType"/> is a nullable type.</returns>
        private static bool IsNullableType(Type objectType)
        {
            return objectType.IsGenericType && objectType.Name == "Nullable`1" && objectType.Namespace == "System";
        }

        /// <summary>
        /// Checks to see if a type is a generic collection.
        /// </summary>
        /// <remarks>
        /// IDictionary is treated separately so it is excluded from the definition of a generic collection
        /// </remarks>
        /// <param name="objectType">The type to be checked.</param>
        /// <returns>True if <paramref name="objectType"/> is a generic collection.</returns>
        private static bool IsGenericCollection(Type objectType)
        {
            // Note that according to the .NET 2.0 docs arrays are provided at runtime with implementations of
            // IEnumerabe<T>,so arrays have to be excluded from the definition of a generic collection for our
            // purposes. See ms-help://MS.MSSDK.1033/MS.NETFX30SDK.1033/cpref7/html/T_System_Array.htm for details.
            bool ans = !objectType.IsArray
                       &&
                       objectType != typeof(string)
                       &&
                       objectType.GetInterface("System.Collections.Generic.IEnumerable`1") != null
                       &&
                       objectType.GetInterface("System.Collections.Generic.IDictionary`2") == null
                       &&
                       objectType.IsGenericType // needed in cases where classes derive from a generic collection
                       &&
                       objectType.GetGenericTypeDefinition() != typeof(IDictionary<,>);
            return ans;
        }

        /// <summary>
        /// Checks to see if a type is a generic dictionary.
        /// </summary>
        /// <param name="objectType">The type to be checked.</param>
        /// <returns>True if <paramref name="objectType"/> is a generic dictionary.</returns>
        private static bool IsGenericDictionary(Type objectType)
        {
            bool ans = false;
            if (objectType.IsGenericType)
            {
                Type generic = objectType.GetGenericTypeDefinition();
                ans = generic == typeof(IDictionary<,>) || generic == typeof(Dictionary<,>);
            }

            return ans;
        }

        /// <summary>
        /// Checks to see if a type is a non generic collection.
        /// </summary>
        /// <param name="objectType">The type to be checked.</param>
        /// <returns>True if <paramref name="objectType"/> is a non-generic collection.</returns>
        private static bool IsNonGenericCollection(Type objectType)
        {
            // String and XmlNode are IEnumerable but we treat these separately so return false for these.
            bool ans = !objectType.IsArray
                       &&
                       objectType != typeof(string)
                       &&
                       !typeof(XmlNode).IsAssignableFrom(objectType)
                       &&
                       objectType.GetInterface("System.Collections.IEnumerable") != null;
            return ans;
        }

        /// <summary>
        /// Checks to see if a type is a non generic dictionary.
        /// </summary>
        /// <param name="objectType">The type to be checked.</param>
        /// <returns>True if <paramref name="objectType"/> is a non-generic dictionary.</returns>
        private static bool IsNonGenericDictionary(Type objectType)
        {
            bool ans = objectType.GetInterface("System.Collections.IDictionary") != null;
            return ans;
        }

        /// <summary>
        /// Checks to see if a type is a stream type.
        /// </summary>
        /// <param name="objectType">The type to be checked.</param>
        /// <returns>True if <paramref name="objectType"/> is derived from a <see cref="Stream"/>.</returns>
        private static bool IsStream(Type objectType)
        {
            bool ans;
            ans = typeof(Stream).IsAssignableFrom(objectType);
            return ans;
        }

        /// <summary>
        /// Checks to see if a type is derived from another type.
        /// </summary>
        /// <param name="derivedType">The type being checked.</param>
        /// <param name="baseType">The potential base type.</param>
        /// <returns>True if <paramref name="derivedType"/> is derived from <paramref name="baseType"/>.</returns>
        private static bool ObjectIsDerivedFromBaseType(Type derivedType, Type baseType)
        {
            bool ans = false;
            if (baseType == derivedType)
            {
                ans = true;
            }
            else if (baseType.IsAssignableFrom(derivedType))
            {
                ans = true;
            }

            return ans;
        }

        /// <summary>
        /// Ensures that the expected type and the actual type are compatible. Maps interface collection types to arrays.
        /// </summary>
        /// <param name="objectType">The expected type of the object.</param>
        /// <param name="o">The object to check.</param>
        /// <returns>The type to be actually generated.</returns>
        private static Type CheckAndMapExpectedType(Type objectType, object o)
        {
            Type ans = objectType;
            if (o != null)
            {
                if (IsNullableType(objectType))
                {
                    Type[] genericParameters = objectType.GetGenericArguments();
                    if (genericParameters.Length != 1 || genericParameters[0] != o.GetType())
                    {
                        throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Messages.ObjectGenerator_ObjectTypeMismatch, objectType.ToString(), o.GetType().ToString()));
                    }
                }
                else if (IsGenericCollection(objectType) && objectType.IsInterface)
                {
                    // For concrete collection like Collection and List the type should match exactly.

                    // For interfaces like IList and ICollection the object supplied should be an array.
                    Type[] genericParameters = objectType.GetGenericArguments();
                    if (genericParameters.Length != 1)
                    {
                        throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Messages.ObjectGenerator_GenericCollectionOneTypeParameterExpected, objectType.ToString()));
                    }
                    else if (!o.GetType().IsArray)
                    {
                        throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Messages.ObjectGenerator_ObjectNotAnArray, objectType.ToString(), o.GetType().ToString()));
                    }
                    else if (genericParameters[0] != o.GetType().GetElementType())
                    {
                        throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Messages.ObjectGenerator_ObjectTypeMismatch, objectType.ToString(), o.GetType().GetElementType().ToString()));
                    }
                    else
                    {
                        ans = typeof(Collection<>).MakeGenericType(genericParameters);
                    }
                }
                else if (IsGenericDictionary(objectType) && objectType.IsInterface)
                {
                    // For concrete dictionaries like Dictionary the type should match exactly.
                    Type[] genericParameters = objectType.GetGenericArguments();
                    Type[] genericParametersOfObject = o.GetType().GetGenericArguments();
                    if (genericParametersOfObject.Length != 2)
                    {
                        throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Messages.ObjectGenerator_DictionaryParamsLength, o.GetType().ToString(), genericParametersOfObject.Length));
                    }
                    else
                    {
                        for (int i = 0; i < genericParameters.Length; i++)
                        {
                            if (genericParameters[i] != genericParametersOfObject[i])
                            {
                                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Messages.ObjectGenerator_ObjectTypeMismatch, genericParameters[i].GetType().ToString(), genericParametersOfObject[i].GetType().ToString()));
                            }
                        }
                    }

                    ans = typeof(Dictionary<,>).MakeGenericType(genericParameters);
                }
                else if (IsStream(objectType))
                {
                    if (objectType != o.GetType())
                    {
                        if (objectType.IsAssignableFrom(o.GetType()))
                        {
                            ans = typeof(Stream);
                        }
                        else
                        {
                            throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Messages.ObjectGenerator_StreamObjectTypeMismatch1, objectType.ToString(), o.GetType().ToString()));
                        }
                    }
                }
                else if (ObjectIsDerivedFromBaseType(o.GetType(), objectType))
                {
                    ans = o.GetType();
                }
                else if (objectType == typeof(object))
                {
                    ans = o.GetType();
                }
                else if (objectType != o.GetType())
                {
                    throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Messages.ObjectGenerator_ObjectTypeMismatch, objectType.ToString(), o.GetType().ToString()));
                }
            }

            Trace.WriteLine(string.Format(CultureInfo.InvariantCulture, "Mapped type {0} to {1}", objectType, ans));

            return ans;
        }

        /// <summary>
        /// Tests if a DataSet is typed or not.
        /// </summary>
        /// <param name="dataSet">The DataSet to test.</param>
        /// <returns>True if the DataSet is typed, false otherwise.</returns>
        private static bool IsTypedDataSet(DataSet dataSet)
        {
            return dataSet.GetType().BaseType == typeof(DataSet);
        }

        /// <summary>
        /// Generates an object assignment.
        /// </summary>
        /// <param name="lhs">The reference to the left hand side.</param>
        /// <param name="objectType">The type of the object.</param>
        /// <param name="o">The object to assign.</param>
        /// <param name="traceName">A name to add to the trace for tracing purposes.</param>
        private void GenerateObject(CodeExpression lhs, Type objectType, object o, string traceName)
        {
            Trace.WriteLine(string.Format(CultureInfo.InvariantCulture, "Generating element assigment to {0}", traceName));
            Trace.Indent();
            this.EnterLevel();
            try
            {
                Type mappedObjectType = CheckAndMapExpectedType(objectType, o);
                CodeTypeReference objectTypeReference = this.ProcessObjectType(mappedObjectType);

                if (this.serializationInfo.IsSimpleType(mappedObjectType) || o == null)
                {
                    this.GenerateSimpleAssignment(lhs, mappedObjectType, o);
                }
                else if (mappedObjectType.IsArray)
                {
                    this.GenerateArrayAssignment(lhs, o, objectTypeReference);
                }
                else if (IsGenericDictionary(objectType) || IsNonGenericDictionary(objectType))
                {
                    this.GenerateDictionaryAssignment(lhs, o, objectTypeReference);
                }
                else if (IsGenericCollection(objectType) || IsNonGenericCollection(objectType))
                {
                    this.GenerateCollectionAssignment(lhs, o, objectTypeReference);
                }
                else if (o is XmlNode)
                {
                    this.GenerateXmlAssignment(lhs, o);
                }
                else if (o is DataSet)
                {
                    this.GenerateDataSetAssignment(lhs, o);
                }
                else if (IsStream(objectType))
                {
                    this.GenerateStream(lhs, objectTypeReference);
                }
                else
                {
                    if (!this.serializationInfo.IsSerializable(mappedObjectType))
                    {
                        throw new UserException(string.Format(CultureInfo.CurrentCulture, this.serializationInfo.NotSerializableError, mappedObjectType.FullName));
                    }

                    this.GenerateCompoundAssignment(lhs, o, objectTypeReference);
                }
            }
            finally
            {
                this.ExitLevel();
                Trace.Unindent();
            }
        }

        /// <summary>
        /// Enters a nesting level.
        /// </summary>
        private void EnterLevel()
        {
            this.level++;
            if (this.level > MaxNestingLevel)
            {
                throw new UserException(string.Format(CultureInfo.CurrentCulture, Messages.ObjectGenerator_NestingLevel, MaxNestingLevel));
            }
        }

        /// <summary>
        /// Exits a nesting level.
        /// </summary>
        private void ExitLevel()
        {
            this.level--;
        }

        /// <summary>
        /// Generates a simple assignment.
        /// </summary>
        /// <param name="name">The name of the variable to be assigned.</param>
        /// <param name="objectType">The type of the object being assigned.</param>
        /// <param name="o">The object to be assigned.</param>
        /// <param name="objectTypeReference">The type reference.</param>
        private void GenerateSimpleAssignment(string name, Type objectType, object o, CodeTypeReference objectTypeReference)
        {
            CodeExpression rhs = this.GenerateSimpleAssignmentRhs(objectType, o);
            CodeVariableDeclarationStatement variableDeclaration = new CodeVariableDeclarationStatement(objectTypeReference, name, rhs);
            this.Body.Add(variableDeclaration);
        }

        /// <summary>
        /// Generates a simple assignment.
        /// </summary>
        /// <param name="lhs">The left hand side of the assignment</param>
        /// <param name="objectType">The type of the object being assigned.</param>
        /// <param name="o">The object to be assigned.</param>
        private void GenerateSimpleAssignment(CodeExpression lhs, Type objectType, object o)
        {
            CodeExpression rhs = this.GenerateSimpleAssignmentRhs(objectType, o);
            CodeAssignStatement assignment = new CodeAssignStatement(lhs, rhs);
            this.Body.Add(assignment);
        }

        /// <summary>
        /// Generates the right hand side of a simple assignment.
        /// </summary>
        /// <param name="objectType">The type of the right hand side.</param>
        /// <param name="o">The value of the right hand side.</param>
        /// <returns>The generated expression for the right hand side.</returns>
        private CodeExpression GenerateSimpleAssignmentRhs(Type objectType, object o)
        {
            CodeExpression rhs = null;
            if (!objectType.IsValueType || o != null || IsNullableType(objectType))
            {
                rhs = this.GenerateSimpleExpression(objectType, o);
            }

            return rhs;
        }

        /// <summary>
        /// Generates the code for an assignment to an array.
        /// </summary>
        /// <param name="name">The name of the variable to be assigned the generated object.</param>
        /// <param name="o">The object to be generated.</param>
        /// <param name="objectTypeReference">The reference to the type of generated array.</param>
        private void GenerateArrayAssignment(string name, object o, CodeTypeReference objectTypeReference)
        {
            if (o.GetType().GetArrayRank() > 1)
            {
                throw new InvalidOperationException(Messages.ObjectGenerator_ArrayRank);
            }

            Array targetArray = (Array)o;
            CodeArrayCreateExpression rhs = new CodeArrayCreateExpression(objectTypeReference, targetArray.Length);
            CodeVariableDeclarationStatement variableDeclaration = new CodeVariableDeclarationStatement(objectTypeReference, name, rhs);
            this.Body.Add(variableDeclaration);
            CodeVariableReferenceExpression targetObject = new CodeVariableReferenceExpression(name);
            this.GenerateArrayAssignmentBody(targetObject, targetArray);
        }

        /// <summary>
        /// Generates the code for an assignment to an array.
        /// </summary>
        /// <param name="targetObject">The variable to be assigned the generated object.</param>
        /// <param name="o">The object to be generated.</param>
        /// <param name="objectTypeReference">The reference to the type of generated array.</param>
        private void GenerateArrayAssignment(CodeExpression targetObject, object o, CodeTypeReference objectTypeReference)
        {
            if (o.GetType().GetArrayRank() > 1)
            {
                throw new InvalidOperationException(Messages.ObjectGenerator_ArrayRank);
            }

            Array targetArray = (Array)o;
            CodeArrayCreateExpression rhs = new CodeArrayCreateExpression(objectTypeReference, targetArray.Length);
            CodeAssignStatement assignment = new CodeAssignStatement(targetObject, rhs);
            this.Body.Add(assignment);
            this.GenerateArrayAssignmentBody(targetObject, targetArray);
        }

        /// <summary>
        /// Generates the main body of an assignment to an array.
        /// </summary>
        /// <param name="targetObject">The reference to the variable to be assigned.</param>
        /// <param name="targetArray">The array to assign.</param>
        private void GenerateArrayAssignmentBody(CodeExpression targetObject, Array targetArray)
        {
            int index = 0;
            Type targetArrayElementType = targetArray.GetType().GetElementType();
            foreach (object element in targetArray)
            {
                Type elementType = element.GetType();
                CodeArrayIndexerExpression indexer = new CodeArrayIndexerExpression(targetObject, new CodePrimitiveExpression(index));
                if (elementType == targetArrayElementType || targetArrayElementType == typeof(XmlNode))
                {
                    this.GenerateObject(indexer, element.GetType(), element, string.Format(CultureInfo.InvariantCulture, "[{0}]", index));
                }
                else
                {
                    CodeExpression rhs = this.GenerateObject(this.NextTemporaryVariableName(), elementType, element);
                    CodeAssignStatement assignment = new CodeAssignStatement(indexer, rhs);
                    this.Body.Add(assignment);
                }

                index++;
            }
        }

        // TODO: refactor to remove repetitive code in these assignement methods

        /// <summary>
        /// Generates the code for an assignment to a dictionary.
        /// </summary>
        /// <param name="name">The name of the variable to be assigned the generated object.</param>
        /// <param name="o">The object to be generated.</param>
        /// <param name="objectTypeReference">The reference to the type of generated dictionary.</param>
        private void GenerateDictionaryAssignment(string name, object o, CodeTypeReference objectTypeReference)
        {
            IDictionary targetDictionary = (IDictionary)o;
            CodeObjectCreateExpression rhs = new CodeObjectCreateExpression(objectTypeReference);
            CodeVariableDeclarationStatement variableDeclaration = new CodeVariableDeclarationStatement(objectTypeReference, name, rhs);
            this.Body.Add(variableDeclaration);
            CodeVariableReferenceExpression targetObject = new CodeVariableReferenceExpression(name);
            this.GenerateDictionaryAssignmentBody(targetObject, targetDictionary);
        }

        /// <summary>
        /// Generates the code for an assignment to a dictionary.
        /// </summary>
        /// <param name="targetObject">The variable to be assigned the generated object.</param>
        /// <param name="o">The object to be generated.</param>
        /// <param name="objectTypeReference">The reference to the type of generated dictionary.</param>
        private void GenerateDictionaryAssignment(CodeExpression targetObject, object o, CodeTypeReference objectTypeReference)
        {
            IDictionary targetDictionary = (IDictionary)o;
            CodeObjectCreateExpression rhs = new CodeObjectCreateExpression(objectTypeReference);
            CodeAssignStatement assignment = new CodeAssignStatement(targetObject, rhs);
            this.Body.Add(assignment);
            this.GenerateDictionaryAssignmentBody(targetObject, targetDictionary);
        }

        /// <summary>
        /// Generates the main body of code for an assignment to a dictionary.
        /// </summary>
        /// <param name="targetObject">The variable to be assigned the generated object.</param>
        /// <param name="targetDictionary">The object to be generated.</param>
        private void GenerateDictionaryAssignmentBody(CodeExpression targetObject, IDictionary targetDictionary)
        {
            foreach (DictionaryEntry entry in targetDictionary)
            {
                CodeExpression key;
                if (this.serializationInfo.IsSimpleType(entry.Key.GetType()))
                {
                    key = this.GenerateSimpleExpression(entry.Key.GetType(), entry.Key);
                }
                else
                {
                    key = this.GenerateObject(this.NextTemporaryVariableName(), entry.Key.GetType(), entry.Key);
                }

                CodeExpression rhs;
                if (this.serializationInfo.IsSimpleType(entry.Value.GetType()))
                {
                    rhs = this.GenerateSimpleExpression(entry.Value.GetType(), entry.Value);
                }
                else
                {
                    rhs = this.GenerateObject(this.NextTemporaryVariableName(), entry.Value.GetType(), entry.Value);
                }

                CodeMethodInvokeExpression callAdd = new CodeMethodInvokeExpression(targetObject, "Add", key, rhs);
                this.Body.Add(callAdd);
            }

            this.GenerateCompoundAssignmentBody(targetObject, targetDictionary); // in case of a derived class that has other members
        }

        /// <summary>
        /// Generates the code for an assignment to a collection.
        /// </summary>
        /// <param name="name">The name of the variable to be assigned the generated object.</param>
        /// <param name="o">The object to be generated.</param>
        /// <param name="objectTypeReference">The reference to the type of generated collection.</param>
        private void GenerateCollectionAssignment(string name, object o, CodeTypeReference objectTypeReference)
        {
            IEnumerable targetArray = (IEnumerable)o;
            CodeObjectCreateExpression rhs = new CodeObjectCreateExpression(objectTypeReference);
            CodeVariableDeclarationStatement variableDeclaration = new CodeVariableDeclarationStatement(objectTypeReference, name, rhs);
            this.Body.Add(variableDeclaration);
            CodeVariableReferenceExpression targetObject = new CodeVariableReferenceExpression(name);
            this.GenerateCollectionAssignmentBody(targetObject, targetArray);
        }

        /// <summary>
        /// Generates the code for an assignment to a collection.
        /// </summary>
        /// <param name="targetObject">The variable to be assigned the generated object.</param>
        /// <param name="o">The object to be generated.</param>
        /// <param name="objectTypeReference">The reference to the type of generated collection.</param>
        private void GenerateCollectionAssignment(CodeExpression targetObject, object o, CodeTypeReference objectTypeReference)
        {
            IEnumerable targetArray = (IEnumerable)o;
            CodeObjectCreateExpression rhs = new CodeObjectCreateExpression(objectTypeReference);
            CodeAssignStatement assignment = new CodeAssignStatement(targetObject, rhs);
            this.Body.Add(assignment);
            this.GenerateCollectionAssignmentBody(targetObject, targetArray);
        }

        /// <summary>
        /// Generates the main body of an assignment to a collection.
        /// </summary>
        /// <param name="targetObject">The variable to be assigned the generated object.</param>
        /// <param name="targetCollection">The object to be generated.</param>
        private void GenerateCollectionAssignmentBody(CodeExpression targetObject, IEnumerable targetCollection)
        {
            foreach (object element in targetCollection)
            {
                CodeExpression rhs;
                if (this.serializationInfo.IsSimpleType(element.GetType()))
                {
                    rhs = this.GenerateSimpleExpression(element.GetType(), element);
                }
                else
                {
                    rhs = this.GenerateObject(this.NextTemporaryVariableName(), element.GetType(), element);
                }

                CodeMethodInvokeExpression callAdd = new CodeMethodInvokeExpression(targetObject, "Add", rhs);
                this.Body.Add(callAdd);
            }

            this.GenerateCompoundAssignmentBody(targetObject, targetCollection); // in case of a derived class that has other members
        }

        /// <summary>
        /// Generates the code for an Xml assignment.
        /// </summary>
        /// <param name="name">The name of the variable to be assigned the generated object.</param>
        /// <param name="o">The object to be generated.</param>
        private void GenerateXmlAssignment(string name, object o)
        {
            CodeVariableReferenceExpression tempVarRef = this.GenerateXmlElementCreation(o);

            CodeVariableDeclarationStatement variableDeclaration = new CodeVariableDeclarationStatement(this.ProcessObjectType(typeof(XmlElement)), name, new CodeFieldReferenceExpression(tempVarRef, "DocumentElement"));
            this.Body.Add(variableDeclaration);
        }

        /// <summary>
        /// Generates the code for an Xml assignment.
        /// </summary>
        /// <param name="targetObject">The variable to be assigned the generated object.</param>
        /// <param name="o">The object to be generated.</param>
        private void GenerateXmlAssignment(CodeExpression targetObject, object o)
        {
            CodeVariableReferenceExpression tempVarRef = this.GenerateXmlElementCreation(o);

            CodeAssignStatement assignment = new CodeAssignStatement(targetObject, new CodeFieldReferenceExpression(tempVarRef, "DocumentElement"));
            this.Body.Add(assignment);
        }

        /// <summary>
        /// Generates an assignment of a dataset to a top-level variable.
        /// </summary>
        /// <param name="name">The name of the variable to assign to.</param>
        /// <param name="o">The object to generate the code for.</param>
        private void GenerateDataSetAssignment(string name, object o)
        {
            CodeTypeReference dataSetTypeReference = this.ProcessObjectType(o.GetType());
            CodeVariableDeclarationStatement variableDeclaration = new CodeVariableDeclarationStatement(dataSetTypeReference, name, new CodePrimitiveExpression(null));
            this.Body.Add(variableDeclaration);
            CodeVariableReferenceExpression variableReference = new CodeVariableReferenceExpression(name);

            CodeTryCatchFinallyStatement exceptionStatement = new CodeTryCatchFinallyStatement();
            exceptionStatement.FinallyStatements.Add(new CodeMethodInvokeExpression(variableReference, "Dispose"));
            this.Body.Add(exceptionStatement);
            this.Body = exceptionStatement.TryStatements;
            this.Body.Add(new CodeAssignStatement(variableReference, new CodeObjectCreateExpression(dataSetTypeReference)));
            this.GenerateDataSetCreation(variableReference, (DataSet)o);
        }

        /// <summary>
        /// Generates an assignment of a dataset to a member of another class.
        /// </summary>
        /// <param name="targetObject">The left hand side of the assignment.</param>
        /// <param name="o">The object to generate the code for.</param>
        private void GenerateDataSetAssignment(CodeExpression targetObject, object o)
        {
            CodeTryCatchFinallyStatement exceptionStatement = new CodeTryCatchFinallyStatement();
            exceptionStatement.FinallyStatements.Add(new CodeMethodInvokeExpression(targetObject, "Dispose"));
            this.Body.Add(exceptionStatement);
            this.Body = exceptionStatement.TryStatements;
                        
            CodeTypeReference dataSetTypeReference = this.ProcessObjectType(o.GetType());
            CodeAssignStatement assignment = new CodeAssignStatement(targetObject, new CodeObjectCreateExpression(dataSetTypeReference));
            this.Body.Add(assignment);
            this.GenerateDataSetCreation(targetObject, (DataSet)o);
        }

        /// <summary>
        /// Generates the code to create a DataSet.
        /// </summary>
        /// <remarks>The DataSet has already been created and assigned to a variable or member.</remarks>
        /// <param name="dataSetVariable">The reference to the DataSet.</param>
        /// <param name="dataSet">The DataSet to generate code for.</param>
        private void GenerateDataSetCreation(CodeExpression dataSetVariable, DataSet dataSet)
        {
            this.GenerateLocaleAssignment(dataSetVariable, dataSet.Locale);

            for (int i = 0; i < dataSet.Tables.Count; i++)
            {
                DataTable dataTable = dataSet.Tables[i];
                CodeExpression tablesCollection = new CodeFieldReferenceExpression(dataSetVariable, "Tables");
                if (!IsTypedDataSet(dataSet))
                {
                    string unnamedTableName = string.Concat("Table", (i + 1).ToString(CultureInfo.InvariantCulture));
                    CodeExpression tableNewExpression;
                    if (string.IsNullOrEmpty(dataTable.TableName) || dataTable.TableName == unnamedTableName)
                    {
                        tableNewExpression = new CodeObjectCreateExpression(this.ProcessObjectType(typeof(DataTable)));
                    }
                    else if (string.IsNullOrEmpty(dataTable.Namespace))
                    {
                        tableNewExpression = new CodeObjectCreateExpression(this.ProcessObjectType(typeof(DataTable)), new CodePrimitiveExpression(dataTable.TableName));
                    }
                    else
                    {
                        tableNewExpression = new CodeObjectCreateExpression(this.ProcessObjectType(typeof(DataTable)), new CodePrimitiveExpression(dataTable.TableName), new CodePrimitiveExpression(dataTable.Namespace));
                    }

                    CodeMethodInvokeExpression tableCreationExpression = new CodeMethodInvokeExpression(tablesCollection, "Add", tableNewExpression);
                    this.Body.Add(new CodeExpressionStatement(tableCreationExpression));
                }

                CodeExpression tableReference = new CodeIndexerExpression(tablesCollection, new CodePrimitiveExpression(i));
                this.GenerateDataTableCreation(tableReference, dataTable);
            }
        }

        /// <summary>
        /// Generates the code to create a DataTable.
        /// </summary>
        /// <remarks>The DataTable has already been created and assigned to a variable or member.</remarks>
        /// <param name="tableReference">The reference to the DataTable.</param>
        /// <param name="dataTable">The DataTable to generate code for.</param>
        private void GenerateDataTableCreation(CodeExpression tableReference, DataTable dataTable)
        {
            this.GenerateLocaleAssignment(tableReference, dataTable.Locale);
            CodeExpression columnsCollection = new CodeFieldReferenceExpression(tableReference, "Columns");
            if (!IsTypedDataSet(dataTable.DataSet))
            {
                foreach (DataColumn dataColumn in dataTable.Columns)
                {
                    CodeMethodInvokeExpression columnCreationExpression = new CodeMethodInvokeExpression(columnsCollection, "Add", new CodePrimitiveExpression(dataColumn.ColumnName), new CodeTypeOfExpression(this.ProcessObjectType(dataColumn.DataType)));
                    this.Body.Add(new CodeExpressionStatement(columnCreationExpression));
                }
            }

            this.GenerateRowCreation(tableReference, dataTable);
        }

        /// <summary>
        /// Assigns the locale property of a variable according to the locale value provided.
        /// </summary>
        /// <param name="variable">The variable with the Locale property to be set.</param>
        /// <param name="locale">The locale to set.</param>
        private void GenerateLocaleAssignment(CodeExpression variable, CultureInfo locale)
        {
            CodeTypeReference cultureInfoTypeReference = this.ProcessObjectType(typeof(CultureInfo));
            CodeExpression cultureInfoExpression;
            if (locale.Name == CultureInfo.InvariantCulture.Name)
            {
                cultureInfoExpression = new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(cultureInfoTypeReference), "InvariantCulture");
            }
            else
            {
                cultureInfoExpression = new CodeObjectCreateExpression(cultureInfoTypeReference, new CodePrimitiveExpression(locale.Name));
            }

            CodeFieldReferenceExpression localeExpression = new CodeFieldReferenceExpression(variable, "Locale");
            CodeAssignStatement localeAssignment = new CodeAssignStatement(localeExpression, cultureInfoExpression);
            this.Body.Add(localeAssignment);
        }

        /// <summary>
        /// Generates the code to create the rows in a DataTable.
        /// </summary>
        /// <remarks>The DataTable has already been created and assigned to a variable or member.</remarks>
        /// <param name="tableReference">The reference to the DataTable.</param>
        /// <param name="dataTable">The DataTable to generate code for.</param>
        private void GenerateRowCreation(CodeExpression tableReference, DataTable dataTable)
        {
            CodeExpression rowsCollection = new CodeFieldReferenceExpression(tableReference, "Rows");
            CodeExpression[] columnValues = new CodeExpression[dataTable.Columns.Count];
            foreach (DataRow dataRow in dataTable.Rows)
            {
                for (int i = 0; i < columnValues.Length; i++)
                {
                    if (dataRow.IsNull(i))
                    {
                        columnValues[i] = new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(this.ProcessObjectType(typeof(DBNull))), "Value");
                    }
                    else
                    {
                        columnValues[i] = this.GenerateSimpleExpression(dataTable.Columns[i].DataType, dataRow[i]);
                    }
                }

                CodeMethodInvokeExpression rowCreationExpression = new CodeMethodInvokeExpression(rowsCollection, "Add", columnValues);
                this.Body.Add(new CodeExpressionStatement(rowCreationExpression));
            }
        }

        /// <summary>
        /// Generates the code to generate an XML element.
        /// </summary>
        /// <param name="o">The XML object to be generated.</param>
        /// <returns>The generated expression.</returns>
        private CodeVariableReferenceExpression GenerateXmlElementCreation(object o)
        {
            XmlNode xo = (XmlNode)o;
            CodeObjectCreateExpression newXmlDoc = new CodeObjectCreateExpression(this.ProcessObjectType(typeof(XmlDocument)));
            CodeVariableDeclarationStatement tempVariableDeclaration = new CodeVariableDeclarationStatement(this.ProcessObjectType(typeof(XmlDocument)), this.NextTemporaryVariableName(), newXmlDoc);
            this.Body.Add(tempVariableDeclaration);

            CodeVariableReferenceExpression tempVarRef = new CodeVariableReferenceExpression(tempVariableDeclaration.Name);
            CodeMethodInvokeExpression loadXml = new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(tempVarRef, "LoadXml"), new CodePrimitiveExpression(xo.OuterXml));
            this.Body.Add(loadXml);
            return tempVarRef;
        }

        /// <summary>
        /// Generates the code for the assignment of an object.
        /// </summary>
        /// <param name="name">The name of the variable being assigned.</param>
        /// <param name="o">The value of the object.</param>
        /// <param name="objectTypeReference">The type of the object being assigned.</param>
        private void GenerateCompoundAssignment(string name, object o, CodeTypeReference objectTypeReference)
        {
            CodeObjectCreateExpression rhs = new CodeObjectCreateExpression(objectTypeReference);
            CodeVariableDeclarationStatement variableDeclaration = new CodeVariableDeclarationStatement(objectTypeReference, name, rhs);
            this.Body.Add(variableDeclaration);
            CodeVariableReferenceExpression targetObject = new CodeVariableReferenceExpression(name);

            this.GenerateCompoundAssignmentBody(targetObject, o);
        }

        /// <summary>
        /// Generates the code for the assignment of an object.
        /// </summary>
        /// <param name="targetObject">The variable being assigned.</param>
        /// <param name="o">The value of the object.</param>
        /// <param name="objectTypeReference">The type of the object being assigned.</param>
        private void GenerateCompoundAssignment(CodeExpression targetObject, object o, CodeTypeReference objectTypeReference)
        {
            CodeObjectCreateExpression rhs = new CodeObjectCreateExpression(objectTypeReference);
            CodeAssignStatement assignment = new CodeAssignStatement(targetObject, rhs);
            this.Body.Add(assignment);
            this.GenerateCompoundAssignmentBody(targetObject, o);
        }

        /// <summary>
        /// Generates the code for the assignment of the members of class or struct.
        /// </summary>
        /// <param name="targetObject">The object being assigned.</param>
        /// <param name="o">The value of the object.</param>
        private void GenerateCompoundAssignmentBody(CodeExpression targetObject, object o)
        {
            foreach (MemberInfo member in this.serializationInfo.SerializableMembers(o.GetType()))
            {
                if (member.MemberType == MemberTypes.Property || member.MemberType == MemberTypes.Field)
                {
                    CodeExpression lhs = null;
                    Type memberType = null;
                    object memberValue = null;
                    if (member.MemberType == MemberTypes.Property)
                    {
                        PropertyInfo pi = (PropertyInfo)member;
                        lhs = new CodePropertyReferenceExpression(targetObject, member.Name);
                        memberType = pi.PropertyType;
                        memberValue = pi.GetValue(o, null);
                    }
                    else if (member.MemberType == MemberTypes.Field)
                    {
                        FieldInfo fi = (FieldInfo)member;
                        lhs = new CodeFieldReferenceExpression(targetObject, member.Name);
                        memberType = fi.FieldType;
                        memberValue = fi.GetValue(o);
                    }

                    Type mappedObjectType = CheckAndMapExpectedType(memberType, memberValue);
                    if (memberValue == null || memberType == memberValue.GetType() || IsGenericCollection(mappedObjectType) || IsGenericDictionary(mappedObjectType) || IsStream(memberType))
                    {
                        this.GenerateObject(lhs, memberType, memberValue, member.Name);
                    }
                    else
                    {
                        CodeExpression rhs = this.GenerateObject(this.NextTemporaryVariableName(), memberType, memberValue);
                        CodeAssignStatement assignment = new CodeAssignStatement(lhs, rhs);
                        this.Body.Add(assignment);
                    }
                }
            }
        }

        /// <summary>
        /// Generates the code for a simple object.
        /// </summary>
        /// <param name="objectType">The type of the object to generate code for.</param>
        /// <param name="o">The value to generate code for.</param>
        /// <returns>The generated expression.</returns>
        private CodeExpression GenerateSimpleExpression(Type objectType, object o)
        {
            CodeExpression e;

            Uri u = o as Uri;
            XmlQualifiedName x = o as XmlQualifiedName;

            if (o is DateTime)
            {
                e = this.GenerateDateTimeExpression((DateTime)o);
            }
            else if (o is TimeSpan)
            {
                e = this.GenerateTimeSpanExpression((TimeSpan)o);
            }
            else if (o is Guid)
            {
                e = this.GenerateGuidExpression((Guid)o);
            }
            else if (u != null)
            {
                e = this.GenerateUriExpression(u);
            }
            else if (x != null)
            {
                e = this.GenerateXmlQualifiedNameExpression(x);
            }
            else if (o != null && o.GetType().IsEnum)
            {
                e = this.GenerateEnumReference((Enum)o);
            }
            else if (o != null && IsNullableType(objectType) && !this.serializationInfo.IsSimpleType(o.GetType()))
            {
                string tempVar = this.NextTemporaryVariableName();
                this.GenerateCompoundAssignment(tempVar, o,  this.ProcessObjectType(o.GetType()));
                e = new CodeVariableReferenceExpression(tempVar);
            }
            else
            {
                e = new CodePrimitiveExpression(o);
            }

            return e;
        }

        /// <summary>
        /// Generates an expression for a <see cref="DateTime"/>
        /// </summary>
        /// <param name="dt">The object to generate an expression for.</param>
        /// <returns>The generated expression.</returns>
        private CodeExpression GenerateDateTimeExpression(DateTime dt)
        {
            CodeExpression e;
            e = new CodeObjectCreateExpression(
                                                this.ProcessObjectType(typeof(DateTime)),
                                                new CodePrimitiveExpression(dt.Year),
                                                new CodePrimitiveExpression(dt.Month),
                                                new CodePrimitiveExpression(dt.Day),
                                                new CodePrimitiveExpression(dt.Hour),
                                                new CodePrimitiveExpression(dt.Minute),
                                                new CodePrimitiveExpression(dt.Second),
                                                new CodePrimitiveExpression(dt.Millisecond));
            return e;
        }

        /// <summary>
        /// Generates an expression for a <see cref="TimeSpan"/>
        /// </summary>
        /// <param name="ts">The object to generate an expression for.</param>
        /// <returns>The generated expression.</returns>
        private CodeExpression GenerateTimeSpanExpression(TimeSpan ts)
        {
            CodeExpression e;
            e = new CodeObjectCreateExpression(
                                                this.ProcessObjectType(typeof(TimeSpan)),
                                                new CodePrimitiveExpression(ts.Days),
                                                new CodePrimitiveExpression(ts.Hours),
                                                new CodePrimitiveExpression(ts.Minutes),
                                                new CodePrimitiveExpression(ts.Seconds),
                                                new CodePrimitiveExpression(ts.Milliseconds));
            return e;
        }

        /// <summary>
        /// Generates an expression for a <see cref="Guid"/>
        /// </summary>
        /// <param name="g">The object to generate an expression for.</param>
        /// <returns>The generated expression.</returns>
        private CodeExpression GenerateGuidExpression(Guid g)
        {
            CodeExpression e;
            e = new CodeObjectCreateExpression(this.ProcessObjectType(typeof(Guid)), new CodePrimitiveExpression(g.ToString()));
            return e;
        }

        /// <summary>
        /// Generates an expression for a <see cref="Uri"/>
        /// </summary>
        /// <param name="u">The object to generate an expression for.</param>
        /// <returns>The generated expression.</returns>
        private CodeExpression GenerateUriExpression(Uri u)
        {
            CodeExpression e;
            if (u.IsAbsoluteUri)
            {
                e = new CodeObjectCreateExpression(this.ProcessObjectType(typeof(Uri)), new CodePrimitiveExpression(u.AbsoluteUri));
            }
            else
            {
                e = new CodeObjectCreateExpression(this.ProcessObjectType(typeof(Uri)), new CodePrimitiveExpression(u.OriginalString), this.GenerateEnumReference(UriKind.Relative));
            }

            return e;
        }

        /// <summary>
        /// Generates an expression for an <see cref="XmlQualifiedName"/>
        /// </summary>
        /// <param name="x">The object to generate an expression for.</param>
        /// <returns>The generated expression.</returns>
        private CodeExpression GenerateXmlQualifiedNameExpression(XmlQualifiedName x)
        {
            CodeExpression e;
            if (x.IsEmpty)
            {
                e = new CodeObjectCreateExpression(this.ProcessObjectType(typeof(XmlQualifiedName)));
            }
            else if (string.IsNullOrEmpty(x.Namespace))
            {
                e = new CodeObjectCreateExpression(this.ProcessObjectType(typeof(XmlQualifiedName)), new CodePrimitiveExpression(x.Name));
            }
            else
            {
                e = new CodeObjectCreateExpression(this.ProcessObjectType(typeof(XmlQualifiedName)), new CodePrimitiveExpression(x.Name), new CodePrimitiveExpression(x.Namespace));
            }

            return e;
        }

        /// <summary>
        /// Generates a reference to an enumeration.
        /// </summary>
        /// <param name="e">The enumeration being referenced.</param>
        /// <returns>The reference to the enumeration.</returns>
        private CodeExpression GenerateEnumReference(Enum e)
        {
            CodeExpression ans;
            CodeTypeReferenceExpression enumType = new CodeTypeReferenceExpression(this.ProcessObjectType(e.GetType()));
            ans = new CodeFieldReferenceExpression(enumType, e.ToString());
            return ans;
        }

        /// <summary>
        /// Generates the code for a stream.
        /// </summary>
        /// <param name="name">The name of the variable to be assigned the generated stream.</param>
        /// <param name="objectTypeReference">The type of the stream to be generated.</param>
        private void GenerateStream(string name, CodeTypeReference objectTypeReference)
        {
            CodeExpression rhs;
            if (objectTypeReference.BaseType == "Stream" || objectTypeReference.BaseType == "System.IO.Stream")
            {
                rhs = new CodePrimitiveExpression(null);
            }
            else
            {
                rhs = new CodeObjectCreateExpression(objectTypeReference);
            }

            CodeVariableDeclarationStatement variableDeclaration = new CodeVariableDeclarationStatement(objectTypeReference, name, rhs);
            this.Body.Add(variableDeclaration);
        }

        /// <summary>
        /// Generates the code for a stream.
        /// </summary>
        /// <param name="lhs">The variable to be assigned the generated stream.</param>
        /// <param name="objectTypeReference">The type of the stream to be generated.</param>
        private void GenerateStream(CodeExpression lhs, CodeTypeReference objectTypeReference)
        {
            CodeExpression rhs;
            if (objectTypeReference.BaseType == "Stream" || objectTypeReference.BaseType == "System.IO.Stream")
            {
                rhs = new CodePrimitiveExpression(null);
            }
            else
            {
                rhs = new CodeObjectCreateExpression(objectTypeReference);
            }

            CodeAssignStatement assignment = new CodeAssignStatement(lhs, rhs);
            this.Body.Add(assignment);
        }

        /// <summary>
        /// Generates a temporary variable name.
        /// </summary>
        /// <returns>The temporary variable name.</returns>
        private string NextTemporaryVariableName()
        {
            string ans = "temp" + this.tempVariableSequenceNumber.ToString(CultureInfo.CurrentCulture);
            this.tempVariableSequenceNumber++;
            return ans;
        }

        /// <summary>
        /// Gets the type reference and adds the namespace to the import collection if it
        /// is not already there.
        /// </summary>
        /// <param name="objectType">Type of the object</param>
        /// <returns>Reference to the object's type</returns>
        private CodeTypeReference ProcessObjectType(Type objectType)
        {
            List<CodeTypeReference> genericParameters = new List<CodeTypeReference>();
            foreach (Type t in objectType.GetGenericArguments())
            {
                genericParameters.Add(this.ProcessObjectType(t));
            }

            CodeTypeReference ans = null;
            CodeTypeReferenceRequestEventArgs request = new CodeTypeReferenceRequestEventArgs(objectType, genericParameters.ToArray());
            if (this.CodeTypeRequestEvent != null)
            {
                this.CodeTypeRequestEvent(this, request);
                ans = request.CodeTypeReference;
            }
            else
            {
                ans = new CodeTypeReference(objectType.Name, genericParameters.ToArray());
            }

            return ans;
        }
    }
}
