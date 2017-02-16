//---------------------------------------------------------------------
// <copyright file="UnitTestGenerator.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
//     THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
//     OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
//     LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR
//     FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
// <summary>The UnitTestGenerator type.</summary>
//---------------------------------------------------------------------

// TODO: Handle namespaces more intelligently, only fully qualify if determine that there is an ambiguity otherwise.
// TODO: Consider supporting flow of data between service operation calls
// TODO: Out parameters. These are problematic because in some cases the generated proxy makes the out parameter the return value, but the contract method may have it as an explicit parameter.
namespace Microsoft.WcfUnit.Library
{
    using System;
    using System.CodeDom;
    using System.CodeDom.Compiler;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Reflection;
    using System.ServiceModel;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Generates a unit test class for a series of calls to various proxies.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "This is possibly a longer term change that would be needed.")]
    public class UnitTestGenerator
    {
        /// <summary>
        /// The string used to indent the generated code.
        /// </summary>
        private const string IndentString = "    ";

        /// <summary>
        /// The name of the scenario the code is being generated for.
        /// </summary>
        private readonly string scenarioName;

        /// <summary>
        /// The mode used to generate test methods.
        /// </summary>
        private readonly TestMethodMode testMethodMode;

        /// <summary>
        /// The mode used to control operation timer generation.
        /// </summary>
        private readonly OperationTimerMode operationTimerMode;

        /// <summary>
        /// The code compile unit for the main code unit.
        /// </summary>
        private CodeCompileUnit mainCcu;

        /// <summary>
        /// The code compile unit for the stubs code unit.
        /// </summary>
        private CodeCompileUnit stubCcu;

        /// <summary>
        /// The object generator used to generate objects.
        /// </summary>
        private ObjectGenerator og;

        /// <summary>
        /// The namespace for the main code unit.
        /// </summary>
        private CodeNamespace mainTestNamespace;

        /// <summary>
        /// The namespace for the stub code unit.
        /// </summary>
        private CodeNamespace stubTestNamespace;

        /// <summary>
        /// The type for the main code unit.
        /// </summary>
        private CodeTypeDeclaration mainTestType;

        /// <summary>
        /// The type for the stub code unit.
        /// </summary>
        private CodeTypeDeclaration stubTestType;

        /// <summary>
        /// The generated method which executes the scenario.
        /// </summary>
        private CodeMemberMethod scenarioMethod;

        /// <summary>
        /// The test initializer in the stub.
        /// </summary>
        private CodeMemberMethod stubTestInitializer;

        /// <summary>
        /// Dictionary used to give sequential numbers to repeated calls of the same method.
        /// </summary>
        private Dictionary<string, int> prototypeTable = new Dictionary<string, int>();

        /// <summary>
        /// Initialises a new instance of the <see cref="UnitTestGenerator"/> class.
        /// </summary>
        /// <param name="scenarioName">The name of the scenario.</param>
        /// <param name="namespaceName">The name of the namespace in which to place the unit test class.</param>
        /// <param name="unitTestClassName">The name to be given to the unit test class.</param>
        /// <param name="testMethodMode">Whether to include the test methods for the individual operations or not.</param>
        /// <param name="operationTimerMode">Whether to include timers for the individual operations or not.</param>
        public UnitTestGenerator(string scenarioName, string namespaceName, string unitTestClassName, TestMethodMode testMethodMode, OperationTimerMode operationTimerMode)
        {
            this.scenarioName = scenarioName;
            this.testMethodMode = testMethodMode;
            this.operationTimerMode = operationTimerMode;

            this.mainCcu = new CodeCompileUnit();
            this.mainTestNamespace = new CodeNamespace(namespaceName);
            this.mainCcu.Namespaces.Add(this.mainTestNamespace);

            this.stubCcu = new CodeCompileUnit();
            this.stubTestNamespace = new CodeNamespace(namespaceName);
            this.stubCcu.Namespaces.Add(this.stubTestNamespace);

            this.mainTestType = this.GenerateMainTestClass(unitTestClassName);
            this.stubTestType = this.GenerateStubTestClass(unitTestClassName);

            this.mainTestNamespace.Imports.Add(new CodeNamespaceImport("Microsoft.VisualStudio.TestTools.UnitTesting"));
            this.mainTestNamespace.Imports.Add(new CodeNamespaceImport("System.Collections.Generic"));
        }

        /// <summary>
        /// Generates the code to add a service call to the test.
        /// </summary>
        /// <param name="proxyMethod">A <see cref="MethodInfo"/> object describing the method on the proxy that is to be called, null if there is no proxy.</param>
        /// <param name="contractMethod">A <see cref="MethodInfo"/> object describing the method on the contract that is to be called.</param>
        /// <param name="parameters">The parameter names and object values that represent each parameter.</param>
        /// <param name="comment">A comment to add to the scenario method call, <c>null</c> if no comment is to be added.</param>
        public void GenerateServiceCall(MethodInfo proxyMethod, MethodInfo contractMethod, CallParameterInfo[] parameters, string comment)
        {
            if (contractMethod == null)
            {
                throw new ArgumentNullException("contractMethod");
            }

            if (contractMethod.Name == this.scenarioName)
            {
                throw new UserException(string.Format(CultureInfo.CurrentCulture, Messages.UTGenerator_ScenarioNameAndMethodNameConflict, this.scenarioName));
            }

            Trace.WriteLine(string.Format(CultureInfo.InvariantCulture, "Generating code for call to {0}", contractMethod.Name));
            string derivedMethodName = this.GetMethodName(contractMethod.Name);
            CodeMemberMethod basicMethod = this.GenerateBasicMethod(proxyMethod, contractMethod, derivedMethodName, parameters);
            this.mainTestType.Members.Add(basicMethod);
            if (this.testMethodMode == TestMethodMode.IncludeIndividualOperations)
            {
                this.mainTestType.Members.Add(GenerateUnitTestMethod(derivedMethodName + "Test", basicMethod));
            }

            GenerateScenarioUnitTestMethodCall(this.scenarioMethod, basicMethod, comment);
            Trace.WriteLine(string.Format(CultureInfo.InvariantCulture, "Completed generating code for call to {0}", contractMethod.Name));
        }

        /// <summary>
        /// Writes the code that has been generated so far to a main unit test file and a stubs file.
        /// </summary>
        /// <param name="mainFileName">The name of the file to write the main code to.</param>
        /// <param name="stubFileName">The name of the file to write stub code to.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times", Justification = "IndentedTextWriter needs to be Closed as well as Disposed")]
        public void WriteCode(string mainFileName, string stubFileName)
        {
            Trace.WriteLine(string.Format(CultureInfo.InvariantCulture, "Writing code to {0} and {1}", mainFileName, stubFileName));
            string mainTempFile = Path.GetTempFileName();
            string stubTempFile = Path.GetTempFileName();

            try
            {
                using (CodeDomProvider provider = new Microsoft.CSharp.CSharpCodeProvider())
                {
                    StreamWriter mainFileWriter = null;
                    try
                    {
                        mainFileWriter = new StreamWriter(mainTempFile);
                        using (IndentedTextWriter mainFileTextWriter = new IndentedTextWriter(mainFileWriter, "    "))
                        {
                            mainFileWriter = null;
                            CodeGeneratorOptions cgo = new CodeGeneratorOptions();
                            cgo.BracingStyle = "C";
                            cgo.IndentString = IndentString;
                            try
                            {
                                provider.GenerateCodeFromCompileUnit(this.mainCcu, mainFileTextWriter, cgo);
                            }
                            catch (ArgumentException ae)
                            {
                                Regex r = new Regex(@"Invalid Primitive Type: (?<DataType>\S+). Consider using CodeObjectCreateExpression.");
                                Match m = r.Match(ae.Message);
                                if (m.Success)
                                {
                                    throw new UserException(string.Format(CultureInfo.CurrentCulture, Messages.UTGenerator_TypeNotSupported, m.Result("${DataType}")));
                                }
                                else
                                {
                                    throw;
                                }
                            }

                            mainFileTextWriter.Close();
                        }
                    }
                    finally
                    {
                        if (mainFileWriter != null)
                        {
                            mainFileWriter.Dispose();
                        }
                    }

                    // Any namespaces in the main class to be copied across to the stub as they will be needed there too.
                    foreach (CodeNamespaceImport n in this.mainTestNamespace.Imports)
                    {
                        this.stubTestNamespace.Imports.Add(n);
                    }

                    StreamWriter stubFileWriter = null;
                    try
                    {
                        stubFileWriter = new StreamWriter(stubTempFile);
                        using (IndentedTextWriter stubTextFileWriter = new IndentedTextWriter(stubFileWriter, "    "))
                        {
                            stubFileWriter = null;
                            CodeGeneratorOptions cgo = new CodeGeneratorOptions();
                            cgo.BracingStyle = "C";
                            provider.GenerateCodeFromCompileUnit(this.stubCcu, stubTextFileWriter, cgo);
                            stubTextFileWriter.Close();
                        }
                    }
                    finally
                    {
                        if (stubFileWriter != null)
                        {
                            stubFileWriter.Dispose();
                        }
                    }

                    File.Copy(mainTempFile, mainFileName, true);
                    File.Copy(stubTempFile, stubFileName, true);
                }
            }
            finally
            {
                File.Delete(mainTempFile);
                File.Delete(stubTempFile);
            }

            Trace.WriteLine(string.Format(CultureInfo.InvariantCulture, "Completed writing code to {0} and {1}", mainFileName, stubFileName));
        }

        /// <summary>
        /// Generates the TestContext property code.
        /// </summary>
        /// <param name="testType">The type to which the TestContext is to be added.</param>
        private static void GenerateTestContextProperty(CodeTypeDeclaration testType)
        {
            CodeMemberField backingVariable = new CodeMemberField("TestContext", "_testContext");
            backingVariable.Attributes = MemberAttributes.Private;
            testType.Members.Add(backingVariable);

            CodeMemberProperty testContextProp = new CodeMemberProperty();
            testContextProp.Type = new CodeTypeReference("TestContext");
            testContextProp.Name = "TestContext";
            testContextProp.Attributes = MemberAttributes.Public | MemberAttributes.Final;
            testContextProp.GetStatements.Add(new CodeMethodReturnStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), backingVariable.Name)));
            testContextProp.SetStatements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), backingVariable.Name), new CodeVariableReferenceExpression("value")));

            testType.Members.Add(testContextProp);
        }

        /// <summary>
        /// Gets a reference to the TestContext field.
        /// </summary>
        /// <returns>A reference to the TestContext field.</returns>
        private static CodeFieldReferenceExpression GetTestContextReference()
        {
            return new CodeFieldReferenceExpression(null, "_testContext");
        }

        /// <summary>
        /// Generates a unit test method for a single WCF service call.
        /// </summary>
        /// <param name="methodName">The name of the method to generate.</param>
        /// <param name="basicMethod">The basic method which contains the actual invocation of the WCF service call.</param>
        /// <returns>The unit test method.</returns>
        private static CodeMemberMethod GenerateUnitTestMethod(string methodName, CodeMemberMethod basicMethod)
        {
            CodeMemberMethod ans = new CodeMemberMethod();
            ans.Name = methodName;
            ans.Attributes = MemberAttributes.Public | MemberAttributes.Final;
            ans.CustomAttributes.Add(new CodeAttributeDeclaration("TestMethod"));

            CodeMethodReferenceExpression method = new CodeMethodReferenceExpression(new CodeThisReferenceExpression(), basicMethod.Name);
            CodeMethodInvokeExpression call = new CodeMethodInvokeExpression(method);
            ans.Statements.Add(call);

            return ans;
        }

        /// <summary>
        /// Gets the appropriate serialization information object for the chosen method.
        /// </summary>
        /// <param name="contractMethod">The contract method.</param>
        /// <returns>The serialization info object to use.</returns>
        private static ISerializationInfo GetSerializationInfo(MethodInfo contractMethod)
        {
            ISerializationInfo ans = null;

            WcfSerializationInfoClass wcf = new WcfSerializationInfoClass();
            if (wcf.Supports(contractMethod))
            {
                Debug.Assert(ans == null, "ContractMethod supports more than one serialization method, which is not allowed");
                ans = wcf;
            }

            AsmxSerializationInfoClass asmx = new AsmxSerializationInfoClass();
            if (asmx.Supports(contractMethod))
            {
                Debug.Assert(ans == null, "ContractMethod supports more than one serialization method, which is not allowed");
                ans = asmx;
            }

            Debug.Assert(ans != null, "Contract method found not to support any known serialization method");
            return ans;
        }

        /// <summary>
        /// Gets the direction of a parameter.
        /// </summary>
        /// <param name="p">The parameter to get the direction for.</param>
        /// <returns>The direction of the parameter.</returns>
        private static FieldDirection GetDirectionForCustomisationCall(CallParameterInfo p)
        {
            Debug.Assert(p.Direction != FieldDirection.Out, "Do not expect to be called for an out parameter");

            FieldDirection ans = FieldDirection.In;
            if (p.ParameterType.IsValueType && p.Value == null)
            {
                // This can occur on service-side traces with non stream parameters, the
                // trace in this case has no data and the deserializer sets its value to null in this case.
                ans = FieldDirection.Out;
            }
            else if (p.ParameterType.IsValueType || p.ParameterType == typeof(string))
            {
                ans = FieldDirection.Ref;
            }
            else if (typeof(Stream).IsAssignableFrom(p.ParameterType))
            {
                ans = FieldDirection.Out;
            }

            return ans;
        }

        /// <summary>
        /// Generates the test method which tests the entire scenario.
        /// </summary>
        /// <remarks>
        /// The body of this method is built up as each service call is processed.
        /// </remarks>
        /// <param name="scenarioName">The name of the scenario.</param>
        /// <returns>The scenario unit test method.</returns>
        private static CodeMemberMethod GenerateScenarioUnitTestMethod(string scenarioName)
        {
            CodeMemberMethod ans = new CodeMemberMethod();
            ans.Name = scenarioName;
            ans.Attributes = MemberAttributes.Public | MemberAttributes.Final;
            ans.CustomAttributes.Add(new CodeAttributeDeclaration("TestMethod"));
            return ans;
        }

        /// <summary>
        /// Generates a call to a unit test method to a scenario unit test method.
        /// </summary>
        /// <param name="scenarioMethod">The scenario unit test method.</param>
        /// <param name="method">The method to add to the scenario.</param>
        /// <param name="comment">A comment to be added to the call.</param>
        private static void GenerateScenarioUnitTestMethodCall(CodeMemberMethod scenarioMethod, CodeMemberMethod method, string comment)
        {
            if (!string.IsNullOrEmpty(comment))
            {
                CodeCommentStatement commentStatement = new CodeCommentStatement(comment, false);
                scenarioMethod.Statements.Add(commentStatement);
            }

            CodeMethodReferenceExpression methodRef = new CodeMethodReferenceExpression(new CodeThisReferenceExpression(), method.Name);
            CodeMethodInvokeExpression call = new CodeMethodInvokeExpression(methodRef);
            scenarioMethod.Statements.Add(call);
        }

        /// <summary>
        /// Generates the prefix for the name of the proxy variable.
        /// </summary>
        /// <remarks>
        /// Rule for the proxy's name prefix is that if it starts with two capitalised letters and the first is an I then
        /// this is an interface and we remove the "I". The resulting name is then camel cased.
        /// </remarks>
        /// <param name="proxyType">The proxy type.</param>
        /// <returns>The name prefix.</returns>
        private static string GenerateProxyVariableNamePrefix(Type proxyType)
        {
            string baseName = proxyType.Name;
            if (baseName.Length >= 2 && baseName[0] == 'I' && char.IsUpper(baseName[1]))
            {
                baseName = baseName.Substring(1, baseName.Length - 1); // remove the leading I
            }

            baseName = Utility.ToCamelCase(baseName);
            return baseName;
        }

        /// <summary>
        /// Gets a reference to a referenced type.
        /// </summary>
        /// <param name="t">The type being referenced.</param>
        /// <param name="genericParameters">Any generic type parameters.</param>
        /// <returns>A reference to a referenced type.</returns>
        private static CodeTypeReference GetReferencedTypeReference(Type t, params Type[] genericParameters)
        {
            CodeTypeReference ans = new CodeTypeReference(t);
            foreach (Type gp in genericParameters)
            {
                ans.TypeArguments.Add(new CodeTypeReference(gp));
            }

            return ans;
        }

        /// <summary>
        /// Gets a reference to a referenced type.
        /// </summary>
        /// <param name="t">The type being referenced.</param>
        /// <param name="genericParameters">Any generic type parameters.</param>
        /// <returns>A reference to a referenced type.</returns>
        private static CodeTypeReference GetReferencedTypeReference(string t, params Type[] genericParameters)
        {
            CodeTypeReference ans = new CodeTypeReference(t);
            foreach (Type gp in genericParameters)
            {
                ans.TypeArguments.Add(new CodeTypeReference(gp));
            }

            return ans;
        }

        /// <summary>
        /// Gets an expression that is a reference to a type.
        /// </summary>
        /// <param name="t">The type being referenced.</param>
        /// <returns>An expression that is a reference to a type.</returns>
        private static CodeTypeReferenceExpression GetReferencedTypeReferenceExpression(Type t)
        {
            return new CodeTypeReferenceExpression(GetReferencedTypeReference(t));
        }

        /// <summary>
        /// Generates the main test class.
        /// </summary>
        /// <param name="unitTestClassName">The name of the unit test class.</param>
        /// <returns>Declaration of the main test class.</returns>
        private CodeTypeDeclaration GenerateMainTestClass(string unitTestClassName)
        {
            CodeTypeDeclaration testType = new CodeTypeDeclaration(unitTestClassName);
            this.mainTestNamespace.Types.Add(testType);
            testType.IsClass = true;
            testType.IsPartial = true;
            testType.TypeAttributes = TypeAttributes.Public;
            testType.CustomAttributes.Add(new CodeAttributeDeclaration("TestClass"));
            testType.CustomAttributes.Add(new CodeAttributeDeclaration(
                                                                       new CodeTypeReference("System.Diagnostics.CodeAnalysis.SuppressMessage"),
                                                                       new CodeAttributeArgument(new CodePrimitiveExpression("Microsoft.Design")),
                                                                       new CodeAttributeArgument(new CodePrimitiveExpression("CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable")),
                                                                       new CodeAttributeArgument("Justification", new CodePrimitiveExpression("proxy should not be disposed"))));

            GenerateTestContextProperty(testType);

            this.scenarioMethod = GenerateScenarioUnitTestMethod(this.scenarioName);
            testType.Members.Add(this.scenarioMethod);

            return testType;
        }

        /// <summary>
        /// Generates the stub test class.
        /// </summary>
        /// <param name="unitTestClassName">The name of the unit test class.</param>
        /// <returns>Declaration of the stub test class.</returns>
        private CodeTypeDeclaration GenerateStubTestClass(string unitTestClassName)
        {
            CodeTypeDeclaration testType = new CodeTypeDeclaration(unitTestClassName);
            this.stubTestNamespace.Types.Add(testType);
            testType.IsClass = true;
            testType.IsPartial = true;
            testType.TypeAttributes = TypeAttributes.Public;

            this.stubTestInitializer = new CodeMemberMethod();
            this.stubTestInitializer.Attributes = MemberAttributes.Public | MemberAttributes.Final;
            this.stubTestInitializer.Name = "InitializeTest";
            this.stubTestInitializer.CustomAttributes.Add(new CodeAttributeDeclaration("TestInitialize"));
            this.stubTestInitializer.CustomAttributes.Add(new CodeAttributeDeclaration(
                                                                                       new CodeTypeReference("System.Diagnostics.CodeAnalysis.SuppressMessage"),
                                                                                       new CodeAttributeArgument(new CodePrimitiveExpression("Microsoft.Reliability")),
                                                                                       new CodeAttributeArgument(new CodePrimitiveExpression("CA2000:Dispose objects before losing scope")),
                                                                                       new CodeAttributeArgument("Justification", new CodePrimitiveExpression("proxy should not be disposed"))));
            testType.Members.Add(this.stubTestInitializer);

            return testType;
        }

        /// <summary>
        /// Generates a basic method to invoke a WCF service call.
        /// </summary>
        /// <param name="proxyMethod">The proxy method to call.</param>
        /// <param name="contractMethod">The method definition on the contract.</param>
        /// <param name="generatedMethodName">The name to give to the generated test method.</param>
        /// <param name="parameters">The parameters to pass to the WCF service call.</param>
        /// <returns>A basic method to invoke a WCF service call.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "This is possibly a longer term change that would be needed.")]
        private CodeMemberMethod GenerateBasicMethod(MethodInfo proxyMethod, MethodInfo contractMethod, string generatedMethodName, CallParameterInfo[] parameters)
        {
            CodeMemberMethod ans = new CodeMemberMethod();
            ans.Name = generatedMethodName;
            ans.Attributes = MemberAttributes.Private | MemberAttributes.Final;
            ans.ReturnType = GetReferencedTypeReference(contractMethod.ReturnType);

            // Generate the parameter variables and assign them
            CodeExpression[] proxyParameterVariables = new CodeExpression[parameters.Length];
            List<CodeDirectionExpression> customisationParametersVariables = new List<CodeDirectionExpression>();
            List<CallParameterInfo> customisationParameters = new List<CallParameterInfo>();
            int i = 0;
            this.og = new ObjectGenerator(ans.Statements, GetSerializationInfo(contractMethod));
            this.og.CodeTypeRequestEvent += new EventHandler<CodeTypeReferenceRequestEventArgs>(this.CodeTypeReferenceRequestEventHandler);
            foreach (CallParameterInfo p in parameters)
            {
                CodeVariableReferenceExpression proxyVariable;
                if (p.Direction != FieldDirection.Out)
                {
                    proxyVariable = this.og.GenerateObject(p.Name, p.ParameterType, p.Value);
                }
                else
                {
                    CodeVariableDeclarationStatement declareOutVariable = new CodeVariableDeclarationStatement(GetReferencedTypeReference(p.ParameterType), p.Name);
                    ans.Statements.Add(declareOutVariable);
                    proxyVariable = new CodeVariableReferenceExpression(p.Name);
                }

                proxyParameterVariables[i] = new CodeDirectionExpression(p.Direction, proxyVariable);

                if (p.Direction != FieldDirection.Out)
                {
                    customisationParameters.Add(p);
                    customisationParametersVariables.Add(new CodeDirectionExpression(GetDirectionForCustomisationCall(p), proxyVariable));
                }

                i++;
            }

            // Code the call to the customisation method which allows values to be modified before the proxy call
            if (customisationParametersVariables.Count > 0)
            {
                CodeMethodReferenceExpression customisationMethod = new CodeMethodReferenceExpression(new CodeThisReferenceExpression(), "Customise" + ans.Name);
                CodeMethodInvokeExpression customisationCall = new CodeMethodInvokeExpression(customisationMethod, customisationParametersVariables.ToArray());
                this.og.Body.Add(customisationCall);

                CodeMemberMethod methodStub = new CodeMemberMethod();
                methodStub.Name = customisationMethod.MethodName;
                methodStub.Attributes = MemberAttributes.Private | MemberAttributes.Final;
                foreach (CallParameterInfo p in customisationParameters)
                {
                    CodeParameterDeclarationExpression parameterDeclaration = new CodeParameterDeclarationExpression();
                    parameterDeclaration.Type = GetReferencedTypeReference(p.ParameterType);
                    parameterDeclaration.Name = p.Name;
                    parameterDeclaration.Direction = GetDirectionForCustomisationCall(p);
                    methodStub.Parameters.Add(parameterDeclaration);

                    // Assign default value to out parameters so that the code will always compile.
                    if (parameterDeclaration.Direction == FieldDirection.Out)
                    {
                        CodeAssignStatement outParameterAssignment = new CodeAssignStatement(new CodeVariableReferenceExpression(parameterDeclaration.Name), new CodeDefaultValueExpression(parameterDeclaration.Type));
                        methodStub.Statements.Add(outParameterAssignment);
                    }
                }

                this.stubTestType.Members.Add(methodStub);
            }

            // Code the call to the proxy.
            CodeStatement invocationStatement;
            CodeMethodReferenceExpression proxyMethodRef = new CodeMethodReferenceExpression(this.GetProxyVariable(proxyMethod, contractMethod), contractMethod.Name);
            CodeMethodInvokeExpression proxyCall = new CodeMethodInvokeExpression(proxyMethodRef, proxyParameterVariables);
            if (contractMethod.ReturnType != typeof(void))
            {
                invocationStatement = new CodeMethodReturnStatement(proxyCall);
            }
            else
            {
                invocationStatement = new CodeExpressionStatement(proxyCall);
            }

            // If timers have been requested then put these in a try-finally.
            if (this.operationTimerMode == OperationTimerMode.NoOperationTimers)
            {
                this.og.Body.Add(invocationStatement);
            }
            else
            {
                // TODO: What to do about timer names when there is an overload, using contract method name means both are recorded as the same txn.
                string timerName = this.scenarioName + "_" + contractMethod.Name;
                CodeMethodInvokeExpression startTimer = new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(GetTestContextReference(), "BeginTimer"), new CodePrimitiveExpression(timerName));
                CodeMethodInvokeExpression endTimer = new CodeMethodInvokeExpression(new CodeMethodReferenceExpression(GetTestContextReference(), "EndTimer"), new CodePrimitiveExpression(timerName));

                this.og.Body.Add(startTimer);
                CodeTryCatchFinallyStatement timerBlock = new CodeTryCatchFinallyStatement();
                timerBlock.TryStatements.Add(invocationStatement);
                timerBlock.FinallyStatements.Add(endTimer);

                this.og.Body.Add(timerBlock);
            }

            return ans;
        }

        /// <summary>
        /// Gets the reference to the proxy, adding a declaration if one is not already present. Initialise it in the stub.
        /// </summary>
        /// <param name="proxyMethod">The method as defined on the proxy.</param>
        /// <param name="contractMethod">The method as defined in the contract.</param>
        /// <returns>The reference to the proxy.</returns>
        private CodeExpression GetProxyVariable(MethodInfo proxyMethod, MethodInfo contractMethod)
        {
            CodeExpression ans = null;

            // Check to see if we already have a declaration for a proxy of this type
            foreach (CodeTypeMember member in this.mainTestType.Members)
            {
                CodeMemberField field = member as CodeMemberField;
                if (field != null)
                {
                    if (field.Type.BaseType == GetReferencedTypeReference(contractMethod.DeclaringType).BaseType)
                    {
                        ans = new CodeFieldReferenceExpression(null, field.Name);
                    }
                }
            }

            // Add declaration if one has not been found and add setup of proxy in stub
            if (ans == null)
            {
                CodeFieldReferenceExpression proxyRef = this.GenerateProxyDeclaration(contractMethod.DeclaringType);
                ans = proxyRef;

                if (proxyMethod == null)
                {
                    this.GenerateStubProxySetup(null, contractMethod.DeclaringType, proxyRef);
                }
                else
                {
                    this.GenerateStubProxySetup(proxyMethod.ReflectedType, contractMethod.DeclaringType, proxyRef);
                }
            }

            return ans;
        }

        /// <summary>
        /// Adds the declaration of the proxy to the main class.
        /// </summary>
        /// <param name="proxyType">The type of the proxy.</param>
        /// <returns>A reference to proxy variable.</returns>
        private CodeFieldReferenceExpression GenerateProxyDeclaration(Type proxyType)
        {
            string proxyName = GenerateProxyVariableNamePrefix(proxyType) + "Client";

            CodeMemberField newField = new CodeMemberField(GetReferencedTypeReference(proxyType), proxyName);
            newField.Attributes = MemberAttributes.Private;
            this.mainTestType.Members.Add(newField);
            CodeFieldReferenceExpression proxyRef = new CodeFieldReferenceExpression(null, newField.Name);
            return proxyRef;
        }

        /// <summary>
        /// Adds the setup of the proxy to the stub file, uses a dictionary to store it for use in load tests.
        /// </summary>
        /// <param name="proxyType">The type of the proxy.</param>
        /// <param name="contractType">The type that defines the contract.</param>
        /// <param name="proxyRef">A reference to the proxy variable.</param>
        private void GenerateStubProxySetup(Type proxyType, Type contractType, CodeFieldReferenceExpression proxyRef)
        {
            // code the static dictionary to store the proxy for each thread
            CodeMemberField proxyTable = new CodeMemberField();
            proxyTable.Attributes = MemberAttributes.Private | MemberAttributes.Static;
            proxyTable.Type = GetReferencedTypeReference("Dictionary", typeof(int), contractType);
            proxyTable.Name = GenerateProxyVariableNamePrefix(contractType) + "ProxyTable";
            proxyTable.InitExpression = new CodeObjectCreateExpression(proxyTable.Type);
            this.stubTestType.Members.Add(proxyTable);

            CodeFieldReferenceExpression proxyTableRef = new CodeFieldReferenceExpression(null, proxyTable.Name);

            // Need to cast the proxy reference to an ICommunicationObject to get the channel state
            CodeExpression channelRef = new CodeCastExpression(typeof(ICommunicationObject), proxyRef);

            // code the setup of the proxy by checking the static dictionary
            CodeMethodReferenceExpression enterMonitor = new CodeMethodReferenceExpression(GetReferencedTypeReferenceExpression(typeof(System.Threading.Monitor)), "Enter");
            this.stubTestInitializer.Statements.Add(new CodeMethodInvokeExpression(enterMonitor, proxyTableRef));

            CodeTryCatchFinallyStatement tryStatement = new CodeTryCatchFinallyStatement();

            CodeFieldReferenceExpression currentThreadRef = new CodeFieldReferenceExpression(GetReferencedTypeReferenceExpression(typeof(System.Threading.Thread)), "CurrentThread");
            CodeFieldReferenceExpression currentThreadIdRef = new CodeFieldReferenceExpression(currentThreadRef, "ManagedThreadId");
            CodeMethodInvokeExpression invokeTryGetValue = new CodeMethodInvokeExpression(proxyTableRef, "TryGetValue", currentThreadIdRef, new CodeDirectionExpression(FieldDirection.Out, proxyRef));
            tryStatement.TryStatements.Add(invokeTryGetValue);

            CodeExpression isProxyNull = new CodeBinaryOperatorExpression(proxyRef, CodeBinaryOperatorType.ValueEquality, new CodePrimitiveExpression(null));
            CodeExpression isProxyFaulted = new CodeBinaryOperatorExpression(new CodeFieldReferenceExpression(channelRef, "State"), CodeBinaryOperatorType.ValueEquality, new CodeFieldReferenceExpression(GetReferencedTypeReferenceExpression(typeof(CommunicationState)), "Faulted"));
            CodeExpression condition = new CodeBinaryOperatorExpression(isProxyNull, CodeBinaryOperatorType.BooleanOr, isProxyFaulted);
            CodeConditionStatement ifStatement = new CodeConditionStatement();
            ifStatement.Condition = condition;
            CodeCommentStatement commentStatement = new CodeCommentStatement(Messages.UTGenerator_ProxyConstructionComment, false);
            ifStatement.TrueStatements.Add(commentStatement);
            if (proxyType == null)
            {
                // Get the proxy from the servicefactory
                ////System.ServiceModel.ChannelFactory<Contracts.IArithmetic> arithmeticFactory = new System.ServiceModel.ChannelFactory<Contracts.IArithmetic>();
                ////arithmeticClient = arithmeticFactory.CreateChannel();
                CodeTypeReference factoryTypeRef = GetReferencedTypeReference(typeof(ChannelFactory), contractType);
                CodeVariableDeclarationStatement factoryVariableDecl = new CodeVariableDeclarationStatement(factoryTypeRef, GenerateProxyVariableNamePrefix(contractType) + "Factory", new CodeObjectCreateExpression(factoryTypeRef));
                ifStatement.TrueStatements.Add(factoryVariableDecl);
                ifStatement.TrueStatements.Add(new CodeAssignStatement(proxyRef, new CodeMethodInvokeExpression(new CodeFieldReferenceExpression(null, factoryVariableDecl.Name), "CreateChannel")));
            }
            else
            {
                // Just new the proxy
                ifStatement.TrueStatements.Add(new CodeAssignStatement(proxyRef, new CodeObjectCreateExpression(GetReferencedTypeReference(proxyType))));
            }

            ifStatement.TrueStatements.Add(new CodeMethodInvokeExpression(channelRef, "Open"));

            CodeAssignStatement assignToTable = new CodeAssignStatement();
            assignToTable.Left = new CodeArrayIndexerExpression(proxyTableRef, currentThreadIdRef);
            assignToTable.Right = proxyRef;
            ifStatement.TrueStatements.Add(assignToTable);

            tryStatement.TryStatements.Add(ifStatement);

            CodeMethodReferenceExpression exitMonitor = new CodeMethodReferenceExpression(GetReferencedTypeReferenceExpression(typeof(System.Threading.Monitor)), "Exit");
            tryStatement.FinallyStatements.Add(new CodeMethodInvokeExpression(exitMonitor, proxyTableRef));

            this.stubTestInitializer.Statements.Add(tryStatement);
        }

        /// <summary>
        /// Computes the name to be used for a method based on the prototype name. If a method already exists with the
        /// prototype name then the existing method is suffixed with a digit and subsequent calls for the same prototype
        /// add further digits.
        /// </summary>
        /// <param name="prototypeName">The name of the service method.</param>
        /// <returns>The method name.</returns>
        private string GetMethodName(string prototypeName)
        {
            string ans = prototypeName;

            if (this.prototypeTable.ContainsKey(prototypeName))
            {
                int currentCount = this.prototypeTable[prototypeName];
                currentCount++;
                this.prototypeTable[prototypeName] = currentCount;
                ans = ans + currentCount.ToString(CultureInfo.InvariantCulture);

                if (currentCount == 2)
                {
                    this.ChangeMethodName(prototypeName);
                }
            }
            else
            {
                this.prototypeTable.Add(prototypeName, 1);
            }

            return ans;
        }

        /// <summary>
        /// Changes the name of a method that has already been generated.
        /// </summary>
        /// <remarks>
        /// Not implemented.
        /// </remarks>
        /// <param name="prototypeName">The prototype method name.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822", Justification = "Intend to implement this again one day")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801", Justification = "Intend to implement this again one day")]
        private void ChangeMethodName(string prototypeName)
        {
            //// TODO: Re-implement method name change, but need to be careful to change all references as well as declarations

            ////// Have to modify all existing methods with this name, including calls to these methods
            ////foreach (CodeTypeMember member in this._mainTestType.Members)
            ////{
            ////    CodeMemberMethod method = member as CodeMemberMethod;
            ////    if (method != null)
            ////    {
            ////        if (method.Name.StartsWith(prototypeName))
            ////        {
            ////            string newName = method.Name.Replace(prototypeName, prototypeName + "1");
            ////            method.Name = newName;
            ////        }
            ////    }
            ////}
        }

        /// <summary>
        /// Event handler for code type reference requests.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="o">The event arguments.</param>
        private void CodeTypeReferenceRequestEventHandler(object sender, CodeTypeReferenceRequestEventArgs o)
        {
            o.CodeTypeReference = GetReferencedTypeReference(o.RequestedType);
        }
    }
}
