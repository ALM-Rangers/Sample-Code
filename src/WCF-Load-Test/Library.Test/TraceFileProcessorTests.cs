//---------------------------------------------------------------------
// <copyright file="TraceFileProcessorTests.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
//     THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
//     OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
//     LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR
//     FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
// <summary>The TraceFileProcessorTests type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.WcfUnit.Library.Test
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.IO;
    using System.Text;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Microsoft.WcfUnit.Library;

    /// <summary>
    /// Tests the <see cref="TraceFileProcessor"/> class.
    /// </summary>
    [TestClass]
    public class TraceFileProcessorTests
    {
        /// <summary>
        /// The object under test.
        /// </summary>
        private TraceFileProcessor sut;

        /// <summary>
        /// The config data to use when running the trace file processor.
        /// </summary>
        private WcfUnitConfiguration config;

        /// <summary>
        /// Initialises a new instance of the <see cref="TraceFileProcessorTests"/> class.
        /// </summary>
        public TraceFileProcessorTests()
        {
        }

        /// <summary>
        /// Initializes the config data for the tests.
        /// </summary>
        [TestInitialize]
        public void InitConfig()
        {
            this.config = new WcfUnitConfiguration();
            this.config.testMethodMode = TestMethodMode.ScenarioMethodOnly;
            this.config.operationTimerMode = OperationTimerMode.IncludeOperationTimers;
            AssemblyType a = new AssemblyType();
            a.fileName = "ClientProxies.dll";
            this.config.assembly = new AssemblyType[] { a };
            this.config.soapActions = new WcfUnitConfigurationSoapActions();
            this.config.soapActions.soapActionMode = SoapActionMode.Include;
            File.Delete("SampleWithNamespaces.cs");
            File.Delete("SampleWithNamespaces.stubs");
            this.sut = new TraceFileProcessor();
        }

        /// <summary>
        /// This test produces output that is included as part of the tool's test suite,
        /// hence the different prefix to the method name.
        /// </summary>
        [TestMethod]
        public void GenerateFullSampleTestWithProxy()
        {
            this.config = new WcfUnitConfiguration();
            this.config.testMethodMode = TestMethodMode.ScenarioMethodOnly;
            this.config.operationTimerMode = OperationTimerMode.IncludeOperationTimers;
            AssemblyType a1 = new AssemblyType();
            a1.fileName = "ClientProxies.dll";
            AssemblyType a2 = new AssemblyType();
            a2.fileName = "Contracts.Custom.dll";
            AssemblyType a3 = new AssemblyType();
            a3.fileName = "ClientProxies.Custom.dll";
            this.config.assembly = new AssemblyType[] { a1, a2, a3 };
            this.config.soapActions = new WcfUnitConfigurationSoapActions();
            this.config.soapActions.soapActionMode = SoapActionMode.Include;

            Collection<string> proxies = this.sut.ProcessTraceFile("GeneratedSampleTest", "FullSampleTest.svclog", null, this.config);

            Assert.AreEqual<int>(3, proxies.Count);
            Assert.AreEqual<string>("ClientProxies", Path.GetFileNameWithoutExtension(proxies[0]));
            Assert.AreEqual<string>("Contracts.Custom", Path.GetFileNameWithoutExtension(proxies[1]));
            Assert.AreEqual<string>("ClientProxies.Custom", Path.GetFileNameWithoutExtension(proxies[2]));

            File.Copy("GeneratedSampleTest.cs", "..\\..\\..\\Library.Test.Integration\\GeneratedSampleTest.cs", true);
            File.Copy("GeneratedSampleTest.stubs", "..\\..\\..\\Library.Test.Integration\\GeneratedSampleTest.stubs", true);
        }

        /// <summary>
        /// This test produces output that is included as part of the tool's test suite,
        /// hence the different prefix to the method name.
        /// </summary>
        [TestMethod]
        public void GenerateFullSampleTestWithNoProxy()
        {
            this.config = new WcfUnitConfiguration();
            this.config.testMethodMode = TestMethodMode.ScenarioMethodOnly;
            this.config.operationTimerMode = OperationTimerMode.IncludeOperationTimers;
            AssemblyType a1 = new AssemblyType();
            a1.fileName = "Contracts.dll";
            AssemblyType a2 = new AssemblyType();
            a2.fileName = "Contracts.Custom.dll";
            this.config.assembly = new AssemblyType[] { a1, a2 };
            this.config.soapActions = new WcfUnitConfigurationSoapActions();
            this.config.soapActions.soapActionMode = SoapActionMode.Include;

            Collection<string> proxies = this.sut.ProcessTraceFile("GeneratedSampleTestNoProxy", "FullSampleTest.svclog", null, this.config);

            Assert.AreEqual<int>(2, proxies.Count);
            Assert.AreEqual<string>("Contracts", Path.GetFileNameWithoutExtension(proxies[0]));
            Assert.AreEqual<string>("Contracts.Custom", Path.GetFileNameWithoutExtension(proxies[1]));

            File.Copy("GeneratedSampleTestNoProxy.cs", "..\\..\\..\\Library.Test.Integration\\GeneratedSampleTestNoProxy.cs", true);
            File.Copy("GeneratedSampleTestNoProxy.stubs", "..\\..\\..\\Library.Test.Integration\\GeneratedSampleTestNoProxy.stubs", true);
        }

        /// <summary>
        /// This test produces output that is included as part of the tool's test suite,
        /// hence the different prefix to the method name.
        /// </summary>
        [TestMethod]
        public void GenerateFullSampleTestWithProxyFromServerTrace()
        {
            this.config = new WcfUnitConfiguration();
            this.config.clientTrace = false;
            this.config.serviceTrace = true;
            this.config.testMethodMode = TestMethodMode.ScenarioMethodOnly;
            this.config.operationTimerMode = OperationTimerMode.IncludeOperationTimers;
            AssemblyType a1 = new AssemblyType();
            a1.fileName = "Contracts.dll";
            AssemblyType a2 = new AssemblyType();
            a2.fileName = "Contracts.Custom.dll";
            this.config.assembly = new AssemblyType[] { a1, a2 };
            this.config.soapActions = new WcfUnitConfigurationSoapActions();
            this.config.soapActions.soapActionMode = SoapActionMode.Include;

            Collection<string> proxies = this.sut.ProcessTraceFile("GeneratedSampleTestWithProxyFromServerTrace", "FullSampleTestServiceTrace.svclog", null, this.config);

            Assert.AreEqual<int>(2, proxies.Count);
            Assert.AreEqual<string>("Contracts", Path.GetFileNameWithoutExtension(proxies[0]));
            Assert.AreEqual<string>("Contracts.Custom", Path.GetFileNameWithoutExtension(proxies[1]));

            File.Copy("GeneratedSampleTestWithProxyFromServerTrace.cs", "..\\..\\..\\Library.Test.Integration\\GeneratedSampleTestWithProxyFromServerTrace.cs", true);
            File.Copy("GeneratedSampleTestWithProxyFromServerTrace.stubs", "..\\..\\..\\Library.Test.Integration\\GeneratedSampleTestWithProxyFromServerTrace.stubs", true);
        }

        /// <summary>
        /// This test produces output that is included as part of the tool's test suite,
        /// hence the different prefix to the method name.
        /// </summary>
        [TestMethod]
        public void GenerateFullAsmxSampleTestWithProxy()
        {
            this.config = new WcfUnitConfiguration();
            this.config.testMethodMode = TestMethodMode.ScenarioMethodOnly;
            this.config.operationTimerMode = OperationTimerMode.IncludeOperationTimers;
            AssemblyType a1 = new AssemblyType();
            a1.fileName = "ClientProxies.dll";
            this.config.assembly = new AssemblyType[] { a1 };
            this.config.soapActions = new WcfUnitConfigurationSoapActions();
            this.config.soapActions.soapActionMode = SoapActionMode.Include;

            Collection<string> proxies = this.sut.ProcessTraceFile("GeneratedAsmxSampleTest", "AsmxIntegrationTest.svclog", null, this.config);

            Assert.AreEqual<int>(1, proxies.Count);
            Assert.AreEqual<string>("ClientProxies", Path.GetFileNameWithoutExtension(proxies[0]));

            File.Copy("GeneratedAsmxSampleTest.cs", "..\\..\\..\\Library.Test.Integration\\GeneratedAsmxSampleTest.cs", true);
            File.Copy("GeneratedAsmxSampleTest.stubs", "..\\..\\..\\Library.Test.Integration\\GeneratedAsmxSampleTest.stubs", true);
        }

        /// <summary>
        /// This test produces output that is included as part of the tool's test suite,
        /// hence the different prefix to the method name.
        /// </summary>
        [TestMethod]
        public void GenerateFullAsmxSampleTestWithProxyFromFiddlerTrace()
        {
            this.config = new WcfUnitConfiguration();
            this.config.testMethodMode = TestMethodMode.ScenarioMethodOnly;
            this.config.operationTimerMode = OperationTimerMode.IncludeOperationTimers;
            AssemblyType a1 = new AssemblyType();
            a1.fileName = "ClientProxies.dll";
            this.config.assembly = new AssemblyType[] { a1 };
            this.config.soapActions = new WcfUnitConfigurationSoapActions();
            this.config.soapActions.soapActionMode = SoapActionMode.Include;
            this.config.parser = TestHelper.CreateConfigTypeForFiddlerTextFormatParser();

            Collection<string> proxies = this.sut.ProcessTraceFile("GeneratedAsmxFiddlerSampleTest", "AsmxFiddlerIntegrationTest.txt", null, this.config);

            Assert.AreEqual<int>(1, proxies.Count);
            Assert.AreEqual<string>("ClientProxies", Path.GetFileNameWithoutExtension(proxies[0]));

            File.Copy("GeneratedAsmxFiddlerSampleTest.cs", "..\\..\\..\\Library.Test.Integration\\GeneratedAsmxFiddlerSampleTest.cs", true);
            File.Copy("GeneratedAsmxFiddlerSampleTest.stubs", "..\\..\\..\\Library.Test.Integration\\GeneratedAsmxFiddlerSampleTest.stubs", true);
        }

        /// <summary>
        /// Tests that an exception is thrown if the scenario name matches an operation name.
        /// </summary>
        [TestMethod]
        public void GenerateScenarioNameMatchesOperationName()
        {
            this.config = new WcfUnitConfiguration();
            this.config.testMethodMode = TestMethodMode.ScenarioMethodOnly;
            this.config.operationTimerMode = OperationTimerMode.IncludeOperationTimers;
            AssemblyType a = new AssemblyType();
            a.fileName = "ClientProxies.dll";
            this.config.assembly = new AssemblyType[] { a };
            this.config.soapActions = new WcfUnitConfigurationSoapActions();
            this.config.soapActions.soapActionMode = SoapActionMode.Include;

            TestHelper.TestForUserException(() => this.sut.ProcessTraceFile("Add", "FullSampleTest.svclog", null, this.config));

            Assert.IsFalse(System.IO.File.Exists("Add.cs"));
            Assert.IsFalse(System.IO.File.Exists("Add.stubs"));
        }

        /// <summary>
        /// Tests that an exception is thrown if service level logging was not enabled.
        /// </summary>
        [TestMethod]
        public void ProcessorNoServiceLevelLogging()
        {
            TestHelper.TestForUserException(() => this.sut.ProcessTraceFile("NoServiceLevelLogging", "NoServiceLevelLogging.svclog", null, this.config));

            Assert.IsFalse(System.IO.File.Exists("NoServiceLevelLogging.cs"));
            Assert.IsFalse(System.IO.File.Exists("NoServiceLevelLogging.stubs"));
        }

        /// <summary>
        /// Tests that an exception is thrown if there are no message bodies in the log file.
        /// </summary>
        [TestMethod]
        public void ProcessorNoMessageBodies()
        {
            TestHelper.TestForUserException(() => this.sut.ProcessTraceFile("NoMessageBodies", "NoMessageBodies.svclog", null, this.config));
            Assert.IsFalse(System.IO.File.Exists("NoMessageBodies.cs"));
            Assert.IsFalse(System.IO.File.Exists("NoMessageBodies.stubs"));
        }

        /// <summary>
        /// Tests that scenario method only is generated.
        /// </summary>
        [TestMethod]
        public void ProcessorWithMessageBodiesScenarioMethodOnly()
        {
            Collection<string> proxies = this.sut.ProcessTraceFile("SampleWithNamespaces", "SampleWithNamespaces.svclog", null, this.config);
            Assert.AreEqual<int>(1, proxies.Count);
            Assert.AreEqual<string>("ClientProxies", Path.GetFileNameWithoutExtension(proxies[0]));

            Assert.IsTrue(System.IO.File.Exists("SampleWithNamespaces.cs"));
            Assert.IsTrue(System.IO.File.Exists("SampleWithNamespaces.stubs"));
            TestHelper.CheckFileContains("SampleWithNamespaces.cs", @"using Microsoft\.VisualStudio\.TestTools\.UnitTesting;", 1);
            TestHelper.CheckFileContains("SampleWithNamespaces.cs", @"private GeneratedContracts.IArithmetic arithmeticClient", 1);
            TestHelper.CheckFileContains("SampleWithNamespaces.cs", @"\[TestMethod\(\)\]", 1);
            TestHelper.CheckFileContains("SampleWithNamespaces.cs", @"BeginTimer", 9);
            TestHelper.CheckFileContains("SampleWithNamespaces.cs", @"EndTimer", 9);
            TestHelper.CheckFileContains("SampleWithNamespaces.stubs", @"using Microsoft\.VisualStudio\.TestTools\.UnitTesting;", 1);
            TestHelper.CheckFileContains("SampleWithNamespaces.stubs", @"using System\.Collections\.Generic;", 1);
            TestHelper.CheckFileContains("SampleWithNamespaces.stubs", @"private static Dictionary<int, GeneratedContracts\.IArithmetic> arithmeticProxyTable = new Dictionary<int, GeneratedContracts\.IArithmetic>\(\);", 1);
            TestHelper.CheckFileContains("SampleWithNamespaces.stubs", @"System\.Threading\.Monitor\.Enter\(arithmeticProxyTable\);.*\{.*\{.*\}.*\}\s*finally\s*\{\s*System\.Threading\.Monitor\.Exit\(arithmeticProxyTable\);\s*\}", 1);
            TestHelper.CheckFileContains("SampleWithNamespaces.stubs", @"\[TestInitialize\(\)\]\s*\[System\.Diagnostics\.CodeAnalysis\.SuppressMessage\(""Microsoft.Reliability""\, ""CA2000\:Dispose objects before losing scope""\, Justification\=""proxy should not be disposed""\)\]\s*public void InitializeTest\(\).*try\s*\{\s*arithmeticProxyTable\.TryGetValue\(System\.Threading\.Thread\.CurrentThread\.ManagedThreadId, out arithmeticClient\);\s*if \(\(\(arithmeticClient == null\)\s*\|\|\s*\(\(\(System\.ServiceModel\.ICommunicationObject\)\(arithmeticClient\)\)\.State == System\.ServiceModel\.CommunicationState\.Faulted\)\)\)\s*{.*arithmeticClient = new GeneratedContracts.ArithmeticClient\(\);\s*\(\(System\.ServiceModel\.ICommunicationObject\)\(arithmeticClient\)\)\.Open\(\);\s*arithmeticProxyTable\[System\.Threading\.Thread\.CurrentThread\.ManagedThreadId\] = arithmeticClient;\s*\}\s*\}\s*finally", 1);
        }

        /// <summary>
        /// Tests that timers can be excluded.
        /// </summary>
        [TestMethod]
        public void ProcessorWithMessageBodiesNoTimers()
        {
            this.config.operationTimerMode = OperationTimerMode.NoOperationTimers;

            try
            {
                this.sut.ProcessTraceFile("SampleWithNamespaces", "SampleWithNamespaces.svclog", null, this.config, "X.cs", "Y.cs");
                Assert.IsTrue(System.IO.File.Exists("X.cs"));
                Assert.IsTrue(System.IO.File.Exists("Y.cs"));
                TestHelper.CheckFileContains("X.cs", @"\[TestMethod\(\)\]", 1);
                TestHelper.CheckFileContains("X.cs", @"BeginTimer", 0);
                TestHelper.CheckFileContains("X.cs", @"EndTimer", 0);
            }
            finally
            {
                File.Delete("X.cs");
                File.Delete("Y.cs");
            }
        }

        /// <summary>
        /// Tests that individual test methods can be generated.
        /// </summary>
        [TestMethod]
        public void ProcessorWithMessageBodiesIndividualTestMethods()
        {
            this.config.testMethodMode = TestMethodMode.IncludeIndividualOperations;

            this.sut.ProcessTraceFile("SampleWithNamespaces", "SampleWithNamespaces.svclog", null, this.config);
            Assert.IsTrue(System.IO.File.Exists("SampleWithNamespaces.cs"));
            Assert.IsTrue(System.IO.File.Exists("SampleWithNamespaces.stubs"));
            TestHelper.CheckFileContains("SampleWithNamespaces.cs", @"\[TestMethod\(\)\]", 10);
            TestHelper.CheckFileContains("SampleWithNamespaces.cs", @"BeginTimer", 9);
        }

        /// <summary>
        /// Tests that an exception is thrown if the wrong proxy is provided.
        /// </summary>
        [TestMethod]
        public void ProcessorWithMessageBodiesWrongProxy()
        {
            this.config = new WcfUnitConfiguration();
            AssemblyType a = new AssemblyType();
            a.fileName = "Contracts.Custom.dll";
            this.config.assembly = new AssemblyType[] { a };
            this.config.soapActions = new WcfUnitConfigurationSoapActions();
            this.config.soapActions.soapActionMode = SoapActionMode.Include;

            TestHelper.TestForUserException(() => this.sut.ProcessTraceFile("MessageContract", "MessageContract.svclog", null, this.config));
        }

        /// <summary>
        /// Tests that include filtering works.
        /// </summary>
        [TestMethod]
        public void ProcessorWithMessageBodiesFilteredInclude()
        {
            SoapActionType action = new SoapActionType();
            action.action = "http://contoso.com/service/test/IArithmetic/Add3";
            this.config.soapActions.soapAction = new SoapActionType[] { action };

            Collection<string> proxies = this.sut.ProcessTraceFile("SampleWithNamespaces", "SampleWithNamespaces.svclog", null, this.config);

            Assert.AreEqual<int>(1, proxies.Count);
            Assert.AreEqual<string>("ClientProxies", Path.GetFileNameWithoutExtension(proxies[0]));

            TestHelper.CheckFileContains("SampleWithNamespaces.cs", @"\WAdd\(", 0);
            TestHelper.CheckFileContains("SampleWithNamespaces.cs", @"\WAdd2\(", 0);
            TestHelper.CheckFileContains("SampleWithNamespaces.cs", @"\WAdd3\(", 3);
        }

        /// <summary>
        /// Tests that exclude filtering works.
        /// </summary>
        [TestMethod]
        public void ProcessorWithMessageBodiesFilteredExclude()
        {
            this.config.soapActions.soapActionMode = SoapActionMode.Exclude;
            SoapActionType action = new SoapActionType();
            action.action = "http://contoso.com/service/test/IArithmetic/Add3";
            this.config.soapActions.soapAction = new SoapActionType[] { action };

            Collection<string> proxies = this.sut.ProcessTraceFile("SampleWithNamespaces", "SampleWithNamespaces.svclog", null, this.config);

            Assert.AreEqual<int>(1, proxies.Count);
            Assert.AreEqual<string>("ClientProxies", Path.GetFileNameWithoutExtension(proxies[0]));

            TestHelper.CheckFileContains("SampleWithNamespaces.cs", @"\WAdd3\(", 0);
            TestHelper.CheckFileContains("SampleWithNamespaces.cs", @"\WAdd\(", 3);
            TestHelper.CheckFileContains("SampleWithNamespaces.cs", @"\WAdd2\(", 3);
        }

        /// <summary>
        /// Tests that generates correct code for a timer when the service result is not void.
        /// </summary>
        [TestMethod]
        public void ProcessorWithTimerNonVoidResult()
        {
            this.config = new WcfUnitConfiguration();
            this.config.testMethodMode = TestMethodMode.ScenarioMethodOnly;
            this.config.operationTimerMode = OperationTimerMode.IncludeOperationTimers;
            AssemblyType a = new AssemblyType();
            a.fileName = "ClientProxies.dll";
            this.config.assembly = new AssemblyType[] { a };
            this.config.soapActions = new WcfUnitConfigurationSoapActions();
            this.config.soapActions.soapActionMode = SoapActionMode.Include;
            SoapActionType action = new SoapActionType();
            action.action = "http://contoso.com/service/test/IArithmetic/Add";
            this.config.soapActions.soapAction = new SoapActionType[] { action };

            Collection<string> proxies = this.sut.ProcessTraceFile("AddScenario", "FullSampleTest.svclog", null, this.config);

            Assert.AreEqual<int>(1, proxies.Count);
            Assert.AreEqual<string>("ClientProxies", Path.GetFileNameWithoutExtension(proxies[0]));

            TestHelper.CheckFileContains("AddScenario.cs", @"CustomiseAdd.*\n.*BeginTimer.*\n.*try.*\n.*\n.*return.*Add.*\n.*\n.*finally.*\n.*\n.*EndTimer", 1);
        }

        /// <summary>
        /// Tests that generates correct code for a timer when the service result is void.
        /// </summary>
        [TestMethod]
        public void ProcessorWithTimerVoidResult()
        {
            this.config.operationTimerMode = OperationTimerMode.IncludeOperationTimers;
            SoapActionType action = new SoapActionType();
            action.action = "http://contoso.com/service/test/IArithmetic/NoParameters";
            this.config.soapActions.soapAction = new SoapActionType[] { action };

            this.sut.ProcessTraceFile("SampleWithNamespaces", "SampleWithNamespaces.svclog", null, this.config);
            TestHelper.CheckFileContains("SampleWithNamespaces.cs", @"\{.*\n.*BeginTimer.*\n.*try.*\n.*\n\W*arithmeticClient.NoParameters.*\n.*\n.*finally.*\n.*\n.*EndTimer", 1);
        }

        /// <summary>
        /// Tests that generates correct code when the service result is not void and there is no timer.
        /// </summary>
        [TestMethod]
        public void ProcessorNoTimerNonVoidResult()
        {
            this.config = new WcfUnitConfiguration();
            this.config.testMethodMode = TestMethodMode.ScenarioMethodOnly;
            this.config.operationTimerMode = OperationTimerMode.NoOperationTimers;
            AssemblyType a = new AssemblyType();
            a.fileName = "ClientProxies.dll";
            this.config.assembly = new AssemblyType[] { a };
            this.config.soapActions = new WcfUnitConfigurationSoapActions();
            this.config.soapActions.soapActionMode = SoapActionMode.Include;
            SoapActionType action = new SoapActionType();
            action.action = "http://contoso.com/service/test/IArithmetic/Add";
            this.config.soapActions.soapAction = new SoapActionType[] { action };

            this.sut.ProcessTraceFile("AddScenario", "FullSampleTest.svclog", null, this.config);
            TestHelper.CheckFileContains("AddScenario.cs", @"CustomiseAdd.*\n.*return.*Add.*\n.*\}", 1);
        }

        /// <summary>
        /// Tests that generates correct code when the service result is void and there is no timer.
        /// </summary>
        [TestMethod]
        public void ProcessorNoTimerVoidResult()
        {
            this.config.operationTimerMode = OperationTimerMode.NoOperationTimers;
            SoapActionType action = new SoapActionType();
            action.action = "http://contoso.com/service/test/IArithmetic/NoParameters";
            this.config.soapActions.soapAction = new SoapActionType[] { action };

            this.sut.ProcessTraceFile("SampleWithNamespaces", "SampleWithNamespaces.svclog", null, this.config);
            TestHelper.CheckFileContains("SampleWithNamespaces.cs", @"\{.*\n\W*arithmeticClient.NoParameters.*\n.*\}", 1);
        }

        /// <summary>
        /// Tests that generates correct code when there are overloads and parameters renamed on the proxy.
        /// </summary>
        [TestMethod]
        public void ProcessorWithOverloadsAndRenamedParametersOnProxy()
        {
            this.config = new WcfUnitConfiguration();
            this.config.testMethodMode = TestMethodMode.ScenarioMethodOnly;
            this.config.operationTimerMode = OperationTimerMode.NoOperationTimers;
            AssemblyType a1 = new AssemblyType();
            a1.fileName = "ClientProxies.Custom.dll";
            AssemblyType a2 = new AssemblyType();
            a2.fileName = "Contracts.Custom.dll";
            this.config.assembly = new AssemblyType[] { a1, a2 };
            this.config.soapActions = new WcfUnitConfigurationSoapActions();
            this.config.soapActions.soapActionMode = SoapActionMode.Include;
            SoapActionType action1 = new SoapActionType();
            action1.action = "http://tempuri.org/ICustomContracts/Overload";
            SoapActionType action2 = new SoapActionType();
            action2.action = "http://tempuri.org/ICustomContracts/Overload2";
            this.config.soapActions.soapAction = new SoapActionType[] { action1, action2 };
            this.sut.ProcessTraceFile("SampleWithNamespaces", "OverloadedContractMethods.svclog", null, this.config);
            TestHelper.CheckFileContains("SampleWithNamespaces.cs", @"customContractsClient.Overload\(a\);", 2);
        }

        /// <summary>
        /// Tests explicit interface.
        /// </summary>
        [TestMethod]
        public void ProcessorExplicitInterfaceImplementation()
        {
            this.config = new WcfUnitConfiguration();
            this.config.testMethodMode = TestMethodMode.ScenarioMethodOnly;
            this.config.operationTimerMode = OperationTimerMode.NoOperationTimers;
            AssemblyType a1 = new AssemblyType();
            a1.fileName = "ClientProxies.Custom.dll";
            AssemblyType a2 = new AssemblyType();
            a2.fileName = "Contracts.Custom.dll";
            this.config.assembly = new AssemblyType[] { a1, a2 };
            this.config.soapActions = new WcfUnitConfigurationSoapActions();
            this.config.soapActions.soapActionMode = SoapActionMode.Include;
            SoapActionType action1 = new SoapActionType();
            action1.action = "http://tempuri.org/ICustomContracts/Hidden";
            this.config.soapActions.soapAction = new SoapActionType[] { action1 };
            this.sut.ProcessTraceFile("SampleWithNamespaces", "ExplicitInterface.svclog", null, this.config);
            TestHelper.CheckFileContains("SampleWithNamespaces.cs", @"customContractsClient.Hidden\(a\);", 1);
        }

        /// <summary>
        /// Tests interface-only scenario.
        /// </summary>
        [TestMethod]
        public void ProcessorInterfaceOnly()
        {
            this.config = new WcfUnitConfiguration();
            this.config.testMethodMode = TestMethodMode.ScenarioMethodOnly;
            this.config.operationTimerMode = OperationTimerMode.NoOperationTimers;
            AssemblyType a1 = new AssemblyType();
            a1.fileName = "Contracts.Custom.dll";
            this.config.assembly = new AssemblyType[] { a1 };
            this.config.soapActions = new WcfUnitConfigurationSoapActions();
            this.config.soapActions.soapActionMode = SoapActionMode.Include;
            SoapActionType action1 = new SoapActionType();
            action1.action = "http://tempuri.org/ICustomContracts/Overload";
            SoapActionType action2 = new SoapActionType();
            action2.action = "http://tempuri.org/ICustomContracts/Overload2";
            this.config.soapActions.soapAction = new SoapActionType[] { action1, action2 };
            this.sut.ProcessTraceFile("SampleWithNamespaces", "OverloadedContractMethods.svclog", null, this.config);
            TestHelper.CheckFileContains("SampleWithNamespaces.stubs", @"\[TestInitialize\(\)\]\s*\[System\.Diagnostics\.CodeAnalysis\.SuppressMessage\(""Microsoft.Reliability""\, ""CA2000\:Dispose objects before losing scope""\, Justification\=""proxy should not be disposed""\)\]\s*public void InitializeTest\(\).*try\s*\{\s*customContractsProxyTable\.TryGetValue\(System\.Threading\.Thread\.CurrentThread\.ManagedThreadId, out customContractsClient\);\s*if \(\(\(customContractsClient == null\)\s*\|\|\s*\(\(\(System\.ServiceModel\.ICommunicationObject\)\(customContractsClient\)\)\.State == System\.ServiceModel\.CommunicationState\.Faulted\)\)\)\s*{.*System\.ServiceModel\.ChannelFactory<Contracts\.Custom\.ICustomContracts> customContractsFactory = new System\.ServiceModel\.ChannelFactory<Contracts\.Custom\.ICustomContracts>\(\);\s*customContractsClient = customContractsFactory\.CreateChannel\(\);\s*\(\(System\.ServiceModel\.ICommunicationObject\)\(customContractsClient\)\)\.Open\(\);\s*customContractsProxyTable\[System\.Threading\.Thread\.CurrentThread\.ManagedThreadId\] = customContractsClient;\s*\}\s*\}\s*finally", 1);
        }

        /// <summary>
        /// Test temporary collection with proxy.
        /// </summary>
        [TestMethod]
        public void ProcessorTempCollectionWithProxy()
        {
            this.config = new WcfUnitConfiguration();
            this.config.testMethodMode = TestMethodMode.ScenarioMethodOnly;
            this.config.operationTimerMode = OperationTimerMode.IncludeOperationTimers;
            AssemblyType a1 = new AssemblyType();
            a1.fileName = "ClientProxies.dll";
            AssemblyType a2 = new AssemblyType();
            a2.fileName = "Contracts.Custom.dll";
            this.config.assembly = new AssemblyType[] { a1, a2 };
            this.config.soapActions = new WcfUnitConfigurationSoapActions();
            this.config.soapActions.soapActionMode = SoapActionMode.Include;
            SoapActionType action1 = new SoapActionType();
            action1.action = "http://contoso.com/service/test/ICollections/ProcessCollection";
            this.config.soapActions.soapAction = new SoapActionType[] { action1 };

            this.sut.ProcessTraceFile("GeneratedSampleTest", "FullSampleTest.svclog", null, this.config);
        }

        /// <summary>
        /// Test temporary collection without proxy.
        /// </summary>
        [TestMethod]
        public void ProcessorTempCollectionWithNoProxy()
        {
            this.config = new WcfUnitConfiguration();
            this.config.testMethodMode = TestMethodMode.ScenarioMethodOnly;
            this.config.operationTimerMode = OperationTimerMode.IncludeOperationTimers;
            AssemblyType a1 = new AssemblyType();
            a1.fileName = "Contracts.dll";
            this.config.assembly = new AssemblyType[] { a1 };
            this.config.soapActions = new WcfUnitConfigurationSoapActions();
            this.config.soapActions.soapActionMode = SoapActionMode.Include;
            SoapActionType action1 = new SoapActionType();
            action1.action = "http://contoso.com/service/test/ICollections/ProcessCollection";
            this.config.soapActions.soapAction = new SoapActionType[] { action1 };

            this.sut.ProcessTraceFile("GeneratedSampleTest", "FullSampleTest.svclog", null, this.config);
        }

        /// <summary>
        /// Tests ref parameters.
        /// </summary>
        [TestMethod]
        public void ProcessorRefParameters()
        {
            this.config = new WcfUnitConfiguration();
            this.config.testMethodMode = TestMethodMode.ScenarioMethodOnly;
            this.config.operationTimerMode = OperationTimerMode.NoOperationTimers;
            AssemblyType a1 = new AssemblyType();
            a1.fileName = "Contracts.dll";
            this.config.assembly = new AssemblyType[] { a1 };
            this.config.soapActions = new WcfUnitConfigurationSoapActions();
            this.config.soapActions.soapActionMode = SoapActionMode.Include;
            this.sut.ProcessTraceFile("RefParameterScenario", "RefParameter.svclog", null, this.config);
            TestHelper.CheckFileContains("RefParameterScenario.cs", @"int a = 99;\s*this.CustomiseRefParameter\(ref a\);\s*arithmeticClient.RefParameter\(ref a\);", 1);
        }

        /// <summary>
        /// Tests that timestamps are used when there is no timed comments file.
        /// </summary>
        [TestMethod]
        public void ProcessorTimestampsUsedForCommentsWhenNoTimedCommentsFile()
        {
            this.config = new WcfUnitConfiguration();
            this.config.testMethodMode = TestMethodMode.ScenarioMethodOnly;
            this.config.operationTimerMode = OperationTimerMode.IncludeOperationTimers;
            AssemblyType a = new AssemblyType();
            a.fileName = "ClientProxies.dll";
            this.config.assembly = new AssemblyType[] { a };
            this.config.soapActions = new WcfUnitConfigurationSoapActions();
            this.config.soapActions.soapActionMode = SoapActionMode.Include;

            this.sut.ProcessTraceFile("Comment", "FullSampleTest.svclog", null, this.config);

            TestHelper.CheckFileContains("Comment.cs", @"public void Comment\(\)\s*\{\s*// [0-9]{2}/[0-9]{2}/[0-9]{4} [0-9]{2}:[0-9]{2}:[0-9]{2}\s*this", 1);
        }

        /// <summary>
        /// Tests that timed comments are inserted.
        /// </summary>
        [TestMethod]
        public void ProcessorTimedCommentsInserted()
        {
            List<DateTime> messageTimeStamps = new List<DateTime>();

            // Get the timestamps of the messages in the trace file to be used.
            using (Parser parser = TestHelper.CreateWcfParserFromFile("FullSampleTest.svclog", true, false, SoapActionMode.Include))
            {
                ParsedMessage m;
                do
                {
                    m = parser.ReadNextRequest();
                    if (m != null)
                    {
                        messageTimeStamps.Add(m.Timestamp);
                    }
                }
                while (m != null);
            }

            // Build timedComments file to place a message before the first and third messages
            DateTime time1 = messageTimeStamps[0] - new TimeSpan(1);
            DateTime time2 = messageTimeStamps[2] - new TimeSpan(1);
            StringBuilder sb = new StringBuilder();
            sb.Append(time1.ToUniversalTime().ToString(@"yyyy-MM-dd\THH:mm:ss.ffffffK", CultureInfo.InvariantCulture));
            sb.Append("Comment1");
            sb.AppendLine();
            sb.Append(time2.ToUniversalTime().ToString(@"yyyy-MM-dd\THH:mm:ss.ffffffK", CultureInfo.InvariantCulture));
            sb.Append("Comment2");
            sb.AppendLine();

            string comment1 = time1.ToUniversalTime().ToString(CultureInfo.CurrentCulture) + " Comment1";
            string comment2 = time2.ToUniversalTime().ToString(CultureInfo.CurrentCulture) + " Comment2";

            using (Stream timedCommentsFile = TestHelper.BuildFile(sb.ToString()))
            {
                this.config = new WcfUnitConfiguration();
                this.config.testMethodMode = TestMethodMode.ScenarioMethodOnly;
                this.config.operationTimerMode = OperationTimerMode.IncludeOperationTimers;
                AssemblyType a = new AssemblyType();
                a.fileName = "ClientProxies.dll";
                this.config.assembly = new AssemblyType[] { a };
                this.config.soapActions = new WcfUnitConfigurationSoapActions();
                this.config.soapActions.soapActionMode = SoapActionMode.Include;

                this.sut.ProcessTraceFile("Comment", "FullSampleTest.svclog", timedCommentsFile, this.config);

                TestHelper.CheckFileContains("Comment.cs", @"public void Comment\(\)\s*\{\s*// " + comment1 + @"\s*this.*this.*// " + comment2 + @"\s*this", 1);
            }
        }

        /// <summary>
        /// Tests for multiple timed comments.
        /// </summary>
        [TestMethod]
        public void ProcessorWhenMoreThanOneTimedCommentBeforeAMessageLastOneOnlyIsInserted()
        {
            List<DateTime> messageTimeStamps = new List<DateTime>();

            // Get the timestamps of the messages in the trace file to be used.
            using (Parser parser = TestHelper.CreateWcfParserFromFile("FullSampleTest.svclog", true, false, SoapActionMode.Include))
            {
                ParsedMessage m;
                do
                {
                    m = parser.ReadNextRequest();
                    if (m != null)
                    {
                        messageTimeStamps.Add(m.Timestamp);
                    }
                }
                while (m != null);
            }

            // Build timedComments file to place a message before the first and third messages
            DateTime time1 = messageTimeStamps[0] - new TimeSpan(2);
            DateTime time2 = messageTimeStamps[0] - new TimeSpan(1);
            DateTime time3 = messageTimeStamps[2] - new TimeSpan(2);
            StringBuilder sb = new StringBuilder();
            sb.Append(time1.ToUniversalTime().ToString(@"yyyy-MM-dd\THH:mm:ss.ffffffK", CultureInfo.InvariantCulture));
            sb.Append("Comment1");
            sb.AppendLine();
            sb.Append(time2.ToUniversalTime().ToString(@"yyyy-MM-dd\THH:mm:ss.ffffffK", CultureInfo.InvariantCulture));
            sb.Append("Comment2");
            sb.AppendLine();
            sb.Append(time3.ToUniversalTime().ToString(@"yyyy-MM-dd\THH:mm:ss.ffffffK", CultureInfo.InvariantCulture));
            sb.Append("Comment3");
            sb.AppendLine();
            sb.Append(time3.ToUniversalTime().ToString(@"yyyy-MM-dd\THH:mm:ss.ffffffK", CultureInfo.InvariantCulture));
            sb.Append("Comment4");
            sb.AppendLine();

            string comment1 = time2.ToUniversalTime().ToString(CultureInfo.CurrentCulture) + " Comment2";
            string comment2 = time3.ToUniversalTime().ToString(CultureInfo.CurrentCulture) + " Comment4";

            using (Stream timedCommentsFile = TestHelper.BuildFile(sb.ToString()))
            {
                this.config = new WcfUnitConfiguration();
                this.config.testMethodMode = TestMethodMode.ScenarioMethodOnly;
                this.config.operationTimerMode = OperationTimerMode.IncludeOperationTimers;
                AssemblyType a = new AssemblyType();
                a.fileName = "ClientProxies.dll";
                this.config.assembly = new AssemblyType[] { a };
                this.config.soapActions = new WcfUnitConfigurationSoapActions();
                this.config.soapActions.soapActionMode = SoapActionMode.Include;

                this.sut.ProcessTraceFile("Comment", "FullSampleTest.svclog", timedCommentsFile, this.config);

                TestHelper.CheckFileContains("Comment.cs", @"public void Comment\(\)\s*\{\s*// " + comment1 + @"\s*this.*this.*// " + comment2 + @"\s*this", 1);
            }
        }

        /// <summary>
        /// Tests that the processor works when run in a separate app domain.
        /// </summary>
        [TestMethod]
        public void ProcessorInAppDomain()
        {
            this.config.testMethodMode = TestMethodMode.IncludeIndividualOperations;

            TraceFileProcessor.ProcessTraceFileInAppDomain("SampleWithNamespaces", "SampleWithNamespaces.svclog", null, this.config, "SampleWithNamespaces.cs", "SampleWithNamespaces.stubs");
            Assert.IsTrue(System.IO.File.Exists("SampleWithNamespaces.cs"));
            Assert.IsTrue(System.IO.File.Exists("SampleWithNamespaces.stubs"));
            TestHelper.CheckFileContains("SampleWithNamespaces.cs", @"\[TestMethod\(\)\]", 10);
            TestHelper.CheckFileContains("SampleWithNamespaces.cs", @"BeginTimer", 9);
        }
    }
}
