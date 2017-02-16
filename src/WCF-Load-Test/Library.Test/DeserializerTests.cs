//---------------------------------------------------------------------
// <copyright file="DeserializerTests.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
//     THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
//     OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
//     LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR
//     FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
// <summary>The DeserializerTests type.</summary>
//---------------------------------------------------------------------

// TODO: CallParameterInfo needs to include serializer type so object generator can be told what to do, or rather the proxy manager.

namespace Microsoft.WcfUnit.Library.Test
{
    using System.CodeDom;
    using System.IO;
    using System.Reflection;
    using System.Text;
    using GeneratedContracts;
    using GeneratedContractsAsmx;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.WcfUnit.Library;
    using Microsoft.WcfUnit.Library.Test.TestContracts;

    [TestClass]
    public class DeserializerTests
    {
        public DeserializerTests()
        {
        }

        [TestCleanup]
        public void CleanupTest()
        {
            if (TestHelper.LastTraceFileOpened != null)
            {
                TestHelper.LastTraceFileOpened.Close();
            }
        }

        #region Basic data contract tests

        [TestMethod]
        public void DeserializeNoParameters()
        {
            CallParameterInfo[] parameters;
            parameters = RunDeserializer("ClientProxies.dll", "NoParameters.svclog", true, "http://contoso.com/service/test/IArithmetic/NoParameters");
            Assert.AreEqual<int>(0, parameters.Length);
        }

        [TestMethod]
        public void DeserializeSample()
        {
            string[] allSoapActions = new string[] { "http://contoso.com/service/test/IArithmetic/Add", "http://contoso.com/service/test/IArithmetic/Add2", "http://contoso.com/service/test/IArithmetic/Add3" };
            using (Parser p = TestHelper.CreateWcfParserFromFile("SampleWithNamespaces.svclog", true, false, SoapActionMode.Include, allSoapActions))
            {
                ProxyManager pm = new ProxyManager("ClientProxies.dll");
                Deserializer d = new Deserializer();
                ParsedMessage parsedMessage;
                CallParameterInfo[] parameters;
                MethodInfo mi;

                parsedMessage = p.ReadNextRequest();
                mi = pm.GetContractMethod(parsedMessage.SoapAction);
                parameters = d.DeserializeInputParameters(parsedMessage.Message, mi);
                Assert.AreEqual<int>(1, parameters.Length);
                this.ValidateNonNullInputParameter<GeneratedContracts.AddRequest>(parameters[0], "request");

                parsedMessage = p.ReadNextRequest();
                mi = pm.GetContractMethod(parsedMessage.SoapAction);
                parameters = d.DeserializeInputParameters(parsedMessage.Message, mi);
                Assert.AreEqual<int>(1, parameters.Length);
                this.ValidateNonNullInputParameter<GeneratedContracts.AddRequest>(parameters[0], "request");

                parsedMessage = p.ReadNextRequest();
                mi = pm.GetContractMethod(parsedMessage.SoapAction);
                parameters = d.DeserializeInputParameters(parsedMessage.Message, mi);
                Assert.AreEqual<int>(2, parameters.Length);
                this.ValidateNonNullInputParameter<int>(parameters[0], "a");
                this.ValidateNonNullInputParameter<int>(parameters[1], "b");
            }
        }

        [TestMethod]
        public void DeserializeXml()
        {
            string[] allSoapActions = new string[] { "http://contoso.com/service/test/ICollections/ProcessCollection" };
            using (Parser p = TestHelper.CreateWcfParserFromFile("SampleWithNamespaces.svclog", true, false, SoapActionMode.Include, allSoapActions))
            {
                ProxyManager pm = new ProxyManager("ClientProxies.dll");
                Deserializer d = new Deserializer();
                ParsedMessage parsedMessage;
                CallParameterInfo[] parameters;
                MethodInfo mi;

                // The second one has the newlines in it.
                p.ReadNextRequest();
                parsedMessage = p.ReadNextRequest();
                mi = pm.GetContractMethod(parsedMessage.SoapAction);
                parameters = d.DeserializeInputParameters(parsedMessage.Message, mi);
                Assert.AreEqual<int>(1, parameters.Length);
                this.ValidateNonNullInputParameter<GeneratedContracts.CollectionsData>(parameters[0], "data");
                CollectionsData msg = (CollectionsData)parameters[0].Value;
                Assert.AreEqual<string>("<test xmlns=\"\">Element</test>", msg.Element.OuterXml);
                Assert.AreEqual<int>(2, msg.Nodes.Length);
                Assert.AreEqual<string>("<test xmlns=\"\"><node>one</node></test>", msg.Nodes[0].OuterXml);
                Assert.AreEqual<string>("<test xmlns=\"\"><node>two</node></test>", msg.Nodes[1].OuterXml);
            }
        }

        [TestMethod]
        public void DeserializeSampleWithOverloadsAndRenamedParametersOnProxy()
        {
            string[] allSoapActions = new string[] { "http://tempuri.org/ICustomContracts/Overload", "http://tempuri.org/ICustomContracts/Overload2" };
            using (Parser p = TestHelper.CreateWcfParserFromFile("OverloadedContractMethods.svclog", true, false, SoapActionMode.Include, allSoapActions))
            {
                ProxyManager pm = new ProxyManager("ClientProxies.Custom.dll", "Contracts.Custom.dll");
                Deserializer d = new Deserializer();
                ParsedMessage parsedMessage;
                CallParameterInfo[] parameters;
                MethodInfo mi;

                parsedMessage = p.ReadNextRequest();
                mi = pm.GetContractMethod(parsedMessage.SoapAction);
                Assert.AreEqual<string>("Overload", mi.Name);
                parameters = d.DeserializeInputParameters(parsedMessage.Message, mi);
                Assert.AreEqual<int>(1, parameters.Length);
                this.ValidateNonNullInputParameter<int>(parameters[0], "a");

                parsedMessage = p.ReadNextRequest();
                mi = pm.GetContractMethod(parsedMessage.SoapAction);
                Assert.AreEqual<string>("Overload", mi.Name);
                parameters = d.DeserializeInputParameters(parsedMessage.Message, mi);
                Assert.AreEqual<int>(1, parameters.Length);
                this.ValidateNonNullInputParameter<string>(parameters[0], "a");
            }
        }

        [TestMethod]
        public void DeserializeSampleExplicitInterfaceImplementation()
        {
            string[] allSoapActions = new string[] { "http://tempuri.org/ICustomContracts/Hidden" };
            using (Parser p = TestHelper.CreateWcfParserFromFile("ExplicitInterface.svclog", true, false, SoapActionMode.Include, allSoapActions))
            {
                ProxyManager pm = new ProxyManager("ClientProxies.Custom.dll", "Contracts.Custom.dll");
                Deserializer d = new Deserializer();
                ParsedMessage parsedMessage;
                CallParameterInfo[] parameters;
                MethodInfo mi;

                parsedMessage = p.ReadNextRequest();
                mi = pm.GetContractMethod(parsedMessage.SoapAction);
                Assert.AreEqual<string>("Hidden", mi.Name);
                parameters = d.DeserializeInputParameters(parsedMessage.Message, mi);
                Assert.AreEqual<int>(1, parameters.Length);
                this.ValidateNonNullInputParameter<string>(parameters[0], "a");
            }
        }

        #endregion

        #region Basic message contract tests

        [TestMethod]
        public void DeserializeMessageContractWrappedSampleWithGeneratedProxy()
        {
            CallParameterInfo[] parameters;
            parameters = RunDeserializer("ClientProxies.dll", "MessageContract.svclog", true, "http://contoso.com/service/test/IArithmetic/WrappedAdd");
            Assert.AreEqual<int>(1, parameters.Length);
            this.ValidateNonNullInputParameter<GeneratedContracts.AddRequestWrappedMessage>(parameters[0], "request");
            AddRequestWrappedMessage msg = (AddRequestWrappedMessage)parameters[0].Value;
            Assert.AreEqual<string>("hello", msg.header.Header);
            Assert.AreEqual<int>(11, msg.A);
            Assert.AreEqual<int>(6, msg.B);
        }

        [TestMethod]
        public void DeserializeMessageContractWrappedSampleWithOriginalContract()
        {
            CallParameterInfo[] parameters;
            parameters = RunDeserializer("Contracts.dll", "MessageContract.svclog", true, "http://contoso.com/service/test/IArithmetic/WrappedAdd");
            Assert.AreEqual<int>(1, parameters.Length);
            this.ValidateNonNullInputParameter<Contracts.AddRequestWrappedMessage>(parameters[0], "request");
            Contracts.AddRequestWrappedMessage msg = (Contracts.AddRequestWrappedMessage)parameters[0].Value;
            Assert.AreEqual<string>("hello", msg.header.Header);
            Assert.AreEqual<int>(11, msg.A);
            Assert.AreEqual<int>(6, msg.B);
        }

        [TestMethod]
        public void DeserializeMessageContractWrappedSampleNoCustomNamesWithGeneratedProxy()
        {
            CallParameterInfo[] parameters;
            parameters = RunDeserializer("ClientProxies.dll", "MessageContract.svclog", true, "http://contoso.com/service/test/IArithmetic/WrappednoCustomNamesAdd");

            Assert.AreEqual<int>(1, parameters.Length);
            this.ValidateNonNullInputParameter<GeneratedContracts.AddRequestWrappedMessageNoCustomNames>(parameters[0], "request");
            AddRequestWrappedMessageNoCustomNames msg = (AddRequestWrappedMessageNoCustomNames)parameters[0].Value;
            Assert.AreEqual<string>("hello", msg.header.Header);
            Assert.AreEqual<int>(11, msg.A);
            Assert.AreEqual<int>(6, msg.B);
        }

        [TestMethod]
        public void DeserializeMessageContractWrappedSampleNoCustomNamesWithOriginalContract()
        {
            CallParameterInfo[] parameters;
            parameters = RunDeserializer("Contracts.dll", "MessageContract.svclog", true, "http://contoso.com/service/test/IArithmetic/WrappednoCustomNamesAdd");

            Assert.AreEqual<int>(1, parameters.Length);
            this.ValidateNonNullInputParameter<Contracts.AddRequestWrappedMessageNoCustomNames>(parameters[0], "request");
            Contracts.AddRequestWrappedMessageNoCustomNames msg = (Contracts.AddRequestWrappedMessageNoCustomNames)parameters[0].Value;
            Assert.AreEqual<string>("hello", msg.header.Header);
            Assert.AreEqual<int>(11, msg.A);
            Assert.AreEqual<int>(6, msg.B);
        }

        [TestMethod]
        public void DeserializeMessageContractWrappedSampleWithNamespaceOverridesWithGeneratedProxy()
        {
            CallParameterInfo[] parameters;
            parameters = RunDeserializer("ClientProxies.dll", "MessageContract.svclog", true, "http://contoso.com/service/test/IArithmetic/WrappedMessageWithHeaderAndBodyNamespaceOverrides");

            Assert.AreEqual<int>(1, parameters.Length);
            this.ValidateNonNullInputParameter<GeneratedContracts.AddRequestWrappedMessageWithHeaderAndBodyNamespaceOverrides>(parameters[0], "request");
            AddRequestWrappedMessageWithHeaderAndBodyNamespaceOverrides msg = (AddRequestWrappedMessageWithHeaderAndBodyNamespaceOverrides)parameters[0].Value;
            Assert.AreEqual<string>("hello", msg.header.Header);
            Assert.AreEqual<int>(11, msg.A);
            Assert.AreEqual<int>(6, msg.B);
        }

        [TestMethod]
        public void DeserializeMessageContractWrappedSampleWithNamespaceOverridesWithOriginalContract()
        {
            CallParameterInfo[] parameters;
            parameters = RunDeserializer("Contracts.dll", "MessageContract.svclog", true, "http://contoso.com/service/test/IArithmetic/WrappedMessageWithHeaderAndBodyNamespaceOverrides");

            Assert.AreEqual<int>(1, parameters.Length);
            this.ValidateNonNullInputParameter<Contracts.AddRequestWrappedMessageWithHeaderAndBodyNamespaceOverrides>(parameters[0], "request");
            Contracts.AddRequestWrappedMessageWithHeaderAndBodyNamespaceOverrides msg = (Contracts.AddRequestWrappedMessageWithHeaderAndBodyNamespaceOverrides)parameters[0].Value;
            Assert.AreEqual<string>("hello", msg.header.Header);
            Assert.AreEqual<int>(11, msg.A);
            Assert.AreEqual<int>(6, msg.B);
        }

        [TestMethod]
        public void DeserializeMessageContractUnwrappedSampleWithGeneratedProxy()
        {
            CallParameterInfo[] parameters;
            parameters = RunDeserializer("ClientProxies.dll", "MessageContract.svclog", true, "http://contoso.com/service/test/IArithmetic/UnwrappedAdd");

            Assert.AreEqual<int>(1, parameters.Length);
            this.ValidateNonNullInputParameter<GeneratedContracts.AddRequestUnwrappedMessage>(parameters[0], "request");
            AddRequestUnwrappedMessage msg = (AddRequestUnwrappedMessage)parameters[0].Value;
            Assert.AreEqual<string>("hello", msg.header.Header);
            Assert.AreEqual<int>(11, msg.A);
            Assert.AreEqual<int>(6, msg.B);
        }

        [TestMethod]
        public void DeserializeMessageContractUnwrappedSampleWithOriginalContract()
        {
            CallParameterInfo[] parameters;
            parameters = RunDeserializer("Contracts.dll", "MessageContract.svclog", true, "http://contoso.com/service/test/IArithmetic/UnwrappedAdd");

            Assert.AreEqual<int>(1, parameters.Length);
            this.ValidateNonNullInputParameter<Contracts.AddRequestUnwrappedMessage>(parameters[0], "request");
            Contracts.AddRequestUnwrappedMessage msg = (Contracts.AddRequestUnwrappedMessage)parameters[0].Value;
            Assert.AreEqual<string>("hello", msg.header.Header);
            Assert.AreEqual<int>(11, msg.A);
            Assert.AreEqual<int>(6, msg.B);
        }

        #endregion

        #region Out and Ref parameter tests

        [TestMethod]
        public void DeserializeDataContractWithRefParameter()
        {
            CallParameterInfo[] parameters;
            parameters = RunDeserializer("Contracts.dll", "RefParameter.svclog", true, "http://contoso.com/service/test/IArithmetic/RefParameter");
            Assert.AreEqual<int>(1, parameters.Length);
            this.ValidateRefParameter<int>(parameters[0], "a", 99);
        }

        [TestMethod]
        public void DeserializeDataContractWithAnOutParameter()
        {
            CallParameterInfo[] parameters;
            parameters = RunDeserializer("Contracts.dll", "OutParameter.svclog", true, "http://contoso.com/service/test/IArithmetic/OutParameter");
            Assert.AreEqual<int>(2, parameters.Length);
            this.ValidateOutputParameter<int>(parameters[0], "a");
            this.ValidateOutputParameter<int>(parameters[1], "b");
        }

        [TestMethod]
        public void DeserializeDataContractWithMixedDirectionParameters()
        {
            CallParameterInfo[] parameters;
            parameters = RunDeserializer("Contracts.dll", "MixedDirectionParameters.svclog", true, "http://contoso.com/service/test/IArithmetic/MixedDirectionParameters");

            Assert.AreEqual<int>(3, parameters.Length);
            this.ValidateNonNullInputParameter<int>(parameters[0], "a");
            this.ValidateRefParameter<int>(parameters[1], "b", 23);
            this.ValidateOutputParameter<int>(parameters[2], "c");
        }

        [TestMethod]
        public void DeserializeDataContractWithRefObjectParameter()
        {
            CallParameterInfo[] parameters;
            parameters = RunDeserializer("Contracts.dll", "RefObjectParameter.svclog", true, "http://contoso.com/service/test/IArithmetic/RefObjectParameter");

            Assert.AreEqual<int>(2, parameters.Length);
            this.ValidateObjectInputParameter<string>(parameters[0], "a", "hello");
            this.ValidateRefParameter<object>(parameters[1], "b", new object());
        }

        #endregion

        #region ServiceKnownType tests

        [TestMethod]
        public void DeserializeDataContractWithServiceKnownType()
        {
            CallParameterInfo[] parameters;
            parameters = RunDeserializer("Contracts.dll", "ServiceKnownType.svclog", true, "http://tempuri.org/IServiceKnownType/DoSomething");
            Assert.AreEqual<int>(1, parameters.Length);
            Assert.AreEqual<string>("arg", parameters[0].Name);
            Assert.AreEqual<string>("Contracts.BaseServiceKnownType", parameters[0].ParameterType.FullName);
            Assert.AreEqual<FieldDirection>(FieldDirection.In, parameters[0].Direction);
            Assert.AreEqual<string>("Contracts.DerivedServiceKnownType", parameters[0].Value.GetType().FullName);
        }

        #endregion

        #region XmlSerializer tests

        [TestMethod]
        public void DeserializeAsmxWrappedSoap11()
        {
            CallParameterInfo[] parameters;
            parameters = RunDeserializer("ClientProxies.dll", "AsmxWrappedSoap11.svclog", true, "http://contoso.com/asmxservice/test/ProcessSimpleAsmxRequestWrapped");

            Assert.AreEqual<int>(1, parameters.Length);
            this.ValidateNonNullInputParameter<SimpleAsmxRequest>(parameters[0], "r");
            SimpleAsmxRequest msg = (SimpleAsmxRequest)parameters[0].Value;
            Assert.AreEqual<int>(99, msg.A);
            Assert.AreEqual<string>("hello", msg.B);
        }

        [TestMethod]
        public void DeserializeAsmxBareSoap11()
        {
            CallParameterInfo[] parameters;
            parameters = RunDeserializer("ClientProxies.dll", "AsmxBareSoap11.svclog", true, "http://contoso.com/asmxservice/test/ProcessSimpleAsmxRequestBare");

            Assert.AreEqual<int>(1, parameters.Length);
            this.ValidateNonNullInputParameter<ProcessSimpleAsmxRequestBareRequest>(parameters[0], "request");
            ProcessSimpleAsmxRequestBareRequest msg = (ProcessSimpleAsmxRequestBareRequest)parameters[0].Value;
            Assert.AreEqual<int>(99, msg.r.A);
            Assert.AreEqual<string>("hello", msg.r.B);
        }

        [TestMethod]
        public void DeserializeAsmxMultipleParameterWrappedSoap11()
        {
            CallParameterInfo[] parameters;
            parameters = RunDeserializer("ClientProxies.dll", "AsmxWrappedMultiParameterSoap11.svclog", true, "http://contoso.com/asmxservice/test/ProcessMultipleParametersWrapped");

            Assert.AreEqual<int>(2, parameters.Length);
            this.ValidateInputParameter<int>(parameters[0], "a", 99);
            this.ValidateInputParameter<string>(parameters[1], "b", "hello");
        }

        [TestMethod]
        public void DeserializeAsmxWrappedSoap12()
        {
            CallParameterInfo[] parameters;
            parameters = RunDeserializer("ClientProxies.dll", "AsmxWrappedSoap12.svclog", true, "http://contoso.com/asmxservice/test/ProcessSimpleAsmxRequestWrapped");

            Assert.AreEqual<int>(1, parameters.Length);
            this.ValidateNonNullInputParameter<SimpleAsmxRequest>(parameters[0], "r");
            SimpleAsmxRequest msg = (SimpleAsmxRequest)parameters[0].Value;
            Assert.AreEqual<int>(99, msg.A);
            Assert.AreEqual<string>("hello", msg.B);
        }

        [TestMethod]
        public void DeserializeAsmxBareSoap12()
        {
            CallParameterInfo[] parameters;
            parameters = RunDeserializer("ClientProxies.dll", "AsmxBareSoap12.svclog", true, "http://contoso.com/asmxservice/test/ProcessSimpleAsmxRequestBare");

            Assert.AreEqual<int>(1, parameters.Length);
            this.ValidateNonNullInputParameter<ProcessSimpleAsmxRequestBareRequest>(parameters[0], "request");
            ProcessSimpleAsmxRequestBareRequest msg = (ProcessSimpleAsmxRequestBareRequest)parameters[0].Value;
            Assert.AreEqual<int>(99, msg.r.A);
            Assert.AreEqual<string>("hello", msg.r.B);
        }

        [TestMethod]
        public void DeserializeAsmxMultipleParameterWrappedSoap12()
        {
            CallParameterInfo[] parameters;
            parameters = RunDeserializer("ClientProxies.dll", "AsmxWrappedMultiParameterSoap12.svclog", true, "http://contoso.com/asmxservice/test/ProcessMultipleParametersWrapped");

            Assert.AreEqual<int>(2, parameters.Length);
            this.ValidateInputParameter<int>(parameters[0], "a", 99);
            this.ValidateInputParameter<string>(parameters[1], "b", "hello");
        }

        [TestMethod]
        public void DeserializeAsmxXmlSerializerFormatMessage()
        {
            XmlSerializerRequest req = new XmlSerializerRequest();
            req.S = "hello";
            req.I = 1000;

            Deserializer d = new Deserializer();
            ParsedMessage parsedMessage = TestHelper.CreateParsedMessage();
            MethodBase mi = typeof(IXmlSerializerService).GetMethod("SimpleXmlSerializerRequest");

            CallParameterInfo[] parameters = d.DeserializeInputParameters(parsedMessage.Message, mi);

            Assert.AreEqual<int>(1, parameters.Length);
            this.ValidateNonNullInputParameter<XmlSerializerRequest>(parameters[0], "r");
            XmlSerializerRequest p = (XmlSerializerRequest)parameters[0].Value;
            Assert.AreEqual<string>("wcfxml", p.S);
            Assert.AreEqual<int>(1001, p.I);
        }

        #endregion

        #region Streamed data contract tests

        [TestMethod]
        public void DeserializeDataContractWithBufferedStreamInputParameterReturnsStreamWithNullValue()
        {
            CallParameterInfo[] parameters;
            parameters = RunDeserializer("Contracts.dll", "StreamOperations.svclog", true, "http://contoso.com/service/test/IBufferedStreamService/BufferedStreamOperation");

            Assert.AreEqual<int>(1, parameters.Length);
            this.ValidateInputParameter<Stream>(parameters[0], "input", null);
        }

        [TestMethod]
        public void DeserializeDataContractWithBufferedStreamInputParameterReturnsMemoryStreamFromServiceSideTrace()
        {
            CallParameterInfo[] parameters;
            parameters = RunDeserializer("Contracts.dll", "FullSampleTestServiceTrace.svclog", false, "http://contoso.com/service/test/IBufferedStreamService/BufferedStreamOperation");

            Assert.AreEqual<int>(1, parameters.Length);
            this.ValidateInputParameter<Stream>(parameters[0], "input", null);
        }

        [TestMethod]
        public void DeserializeDataContractWithBufferedMemoryStreamInputParameterReturnsMemoryStream()
        {
            CallParameterInfo[] parameters;
            parameters = RunDeserializer("Contracts.dll", "StreamOperations.svclog", true, "http://contoso.com/service/test/IBufferedStreamService/BufferedMemoryStreamOperation");

            Assert.AreEqual<int>(1, parameters.Length);
            this.ValidateInputParameter<MemoryStream>(parameters[0], "input", null);
        }

        [TestMethod]
        public void DeserializeDataContractWithStreamedStreamInputParameterReturnsStreamWithNullValue()
        {
            CallParameterInfo[] parameters;
            parameters = RunDeserializer("Contracts.dll", "StreamOperations.svclog", true, "http://contoso.com/service/test/IStreamedStreamService/StreamedStreamOperation");

            Assert.AreEqual<int>(1, parameters.Length);
            this.ValidateInputParameter<Stream>(parameters[0], "input", null);
        }

        [TestMethod]
        public void DeserializeDataContractWithStreamedMemoryStreamInputParameterReturnsMemoryStream()
        {
            CallParameterInfo[] parameters;
            parameters = RunDeserializer("Contracts.dll", "StreamOperations.svclog", true, "http://contoso.com/service/test/IStreamedStreamService/StreamedMemoryStreamOperation");

            Assert.AreEqual<int>(1, parameters.Length);
            this.ValidateInputParameter<MemoryStream>(parameters[0], "input", null);
        }

        [TestMethod]
        public void DeserializeDataContractWithStreamedMemoryStreamInputParameterReturnsMemoryStreamNullValueFromServiceSideTrace()
        {
            CallParameterInfo[] parameters;
            parameters = RunDeserializer("Contracts.dll", "FullSampleTestServiceTrace.svclog", false, "http://contoso.com/service/test/IStreamedStreamService/StreamedMemoryStreamOperation");

            Assert.AreEqual<int>(1, parameters.Length);
            this.ValidateInputParameter<MemoryStream>(parameters[0], "input", null);
        }

        [TestMethod]
        public void DeserializeDataContractWithStreamedNonStreamParametersGetsParametersAndValues()
        {
            CallParameterInfo[] parameters;
            parameters = RunDeserializer("Contracts.dll", "StreamOperations.svclog", true, "http://contoso.com/service/test/IStreamedStreamService/StreamedWithNonStreamParametersOperation");

            Assert.AreEqual<int>(2, parameters.Length);
            this.ValidateInputParameter<string>(parameters[0], "s", "hello");
            this.ValidateInputParameter<int>(parameters[1], "i", 1);
        }

        [TestMethod]
        public void DeserializeDataContractWithStreamedNonStreamParametersGetsParametersButNoValuesFromServiceSideTrace()
        {
            CallParameterInfo[] parameters;
            parameters = RunDeserializer("Contracts.dll", "FullSampleTestServiceTrace.svclog", false, "http://contoso.com/service/test/IStreamedStreamService/StreamedWithNonStreamParametersOperation");

            Assert.AreEqual<int>(2, parameters.Length);
            this.ValidateInputParameter<string>(parameters[0], "s", null);
            this.ValidateNullInputParameter<int>(parameters[1], "i");
        }

        #endregion

        #region Message contracts with streams tests (client side)

        [TestMethod]
        public void DeserializeMessageContractWithBufferedUnwrappedMessageWithStreamInputParameterReturnsStreamWithNullValue()
        {
            CallParameterInfo[] parameters = RunDeserializer("Contracts.dll", "MessageContractStreamOperationsClientSide.svclog", true, "http://contoso.com/service/test/IBufferedStreamService/BufferedUnwrappedMessageWithStream");
            Assert.AreEqual<int>(1, parameters.Length);
            this.ValidateNonNullInputParameter<Contracts.BufferedUnwrappedMessageWithStream>(parameters[0], "input");
        }

        [TestMethod]
        public void DeserializeMessageContractWithBufferedUnwrappedMessageWithMemoryStreamInputParameterReturnsStreamWithNullValue()
        {
            CallParameterInfo[] parameters = RunDeserializer("Contracts.dll", "MessageContractStreamOperationsClientSide.svclog", true, "http://contoso.com/service/test/IBufferedStreamService/BufferedUnwrappedMessageWithMemoryStream");
            Assert.AreEqual<int>(1, parameters.Length);
            this.ValidateNonNullInputParameter<Contracts.BufferedUnwrappedMessageWithMemoryStream>(parameters[0], "input");
        }

        [TestMethod]
        public void DeserializeMessageContractWithBufferedWrappedMessageWithStreamInputParameterReturnsStreamWithNullValue()
        {
            CallParameterInfo[] parameters = RunDeserializer("Contracts.dll", "MessageContractStreamOperationsClientSide.svclog", true, "http://contoso.com/service/test/IBufferedStreamService/BufferedWrappedMessageWithStream");
            Assert.AreEqual<int>(1, parameters.Length);
            this.ValidateNonNullInputParameter<Contracts.BufferedWrappedMessageWithStream>(parameters[0], "input");
        }

        [TestMethod]
        public void DeserializeMessageContractWithBufferedWrappedMessageWithMemoryStreamInputParameterReturnsStreamWithNullValue()
        {
            CallParameterInfo[] parameters = RunDeserializer("Contracts.dll", "MessageContractStreamOperationsClientSide.svclog", true, "http://contoso.com/service/test/IBufferedStreamService/BufferedWrappedMessageWithMemoryStream");
            Assert.AreEqual<int>(1, parameters.Length);
            this.ValidateNonNullInputParameter<Contracts.BufferedWrappedMessageWithMemoryStream>(parameters[0], "input");
        }

        [TestMethod]
        public void DeserializeMessageContractWithStreamedUnwrappedMessageWithStreamInputParameterReturnsStreamWithNullValue()
        {
            CallParameterInfo[] parameters = RunDeserializer("Contracts.dll", "MessageContractStreamOperationsClientSide.svclog", true, "http://contoso.com/service/test/IStreamedStreamService/StreamedUnwrappedMessageWithStream");
            Assert.AreEqual<int>(1, parameters.Length);
            this.ValidateNonNullInputParameter<Contracts.StreamedUnwrappedMessageWithStream>(parameters[0], "input");
        }

        [TestMethod]
        public void DeserializeMessageContractWithStreamedUnwrappedMessageWithMemoryStreamInputParameterReturnsStreamWithNullValue()
        {
            CallParameterInfo[] parameters = RunDeserializer("Contracts.dll", "MessageContractStreamOperationsClientSide.svclog", true, "http://contoso.com/service/test/IStreamedStreamService/StreamedUnwrappedMessageWithMemoryStream");
            Assert.AreEqual<int>(1, parameters.Length);
            this.ValidateNonNullInputParameter<Contracts.StreamedUnwrappedMessageWithMemoryStream>(parameters[0], "input");
        }

        [TestMethod]
        public void DeserializeMessageContractWithStreamedWrappedMessageWithStreamInputParameterReturnsStreamWithNullValue()
        {
            CallParameterInfo[] parameters = RunDeserializer("Contracts.dll", "MessageContractStreamOperationsClientSide.svclog", true, "http://contoso.com/service/test/IStreamedStreamService/StreamedWrappedMessageWithStream");
            Assert.AreEqual<int>(1, parameters.Length);
            this.ValidateNonNullInputParameter<Contracts.StreamedWrappedMessageWithStream>(parameters[0], "input");
        }

        [TestMethod]
        public void DeserializeMessageContractWithStreamedWrappedMessageWithMemoryStreamInputParameterReturnsStreamWithNullValue()
        {
            CallParameterInfo[] parameters = RunDeserializer("Contracts.dll", "MessageContractStreamOperationsClientSide.svclog", true, "http://contoso.com/service/test/IStreamedStreamService/StreamedWrappedMessageWithMemoryStream");
            Assert.AreEqual<int>(1, parameters.Length);
            this.ValidateNonNullInputParameter<Contracts.StreamedWrappedMessageWithMemoryStream>(parameters[0], "input");
        }

        #endregion

        #region Message contracts with streams tests (service side)

        [TestMethod]
        public void DeserializeMessageContractWithBufferedUnwrappedMessageWithStreamInputParameterReturnsStreamWithNullValueFromServiceTrace()
        {
            CallParameterInfo[] parameters = RunDeserializer("Contracts.dll", "MessageContractStreamOperationsServiceSide.svclog", false, "http://contoso.com/service/test/IBufferedStreamService/BufferedUnwrappedMessageWithStream");
            Assert.AreEqual<int>(1, parameters.Length);
            this.ValidateNonNullInputParameter<Contracts.BufferedUnwrappedMessageWithStream>(parameters[0], "input");
        }

        [TestMethod]
        public void DeserializeMessageContractWithBufferedUnwrappedMessageWithMemoryStreamInputParameterReturnsStreamWithNullValueFromServiceTrace()
        {
            CallParameterInfo[] parameters = RunDeserializer("Contracts.dll", "MessageContractStreamOperationsServiceSide.svclog", false, "http://contoso.com/service/test/IBufferedStreamService/BufferedUnwrappedMessageWithMemoryStream");
            Assert.AreEqual<int>(1, parameters.Length);
            this.ValidateNonNullInputParameter<Contracts.BufferedUnwrappedMessageWithMemoryStream>(parameters[0], "input");
        }

        [TestMethod]
        public void DeserializeMessageContractWithBufferedWrappedMessageWithStreamInputParameterReturnsStreamWithNullValueFromServiceTrace()
        {
            CallParameterInfo[] parameters = RunDeserializer("Contracts.dll", "MessageContractStreamOperationsServiceSide.svclog", false, "http://contoso.com/service/test/IBufferedStreamService/BufferedWrappedMessageWithStream");
            Assert.AreEqual<int>(1, parameters.Length);
            this.ValidateNonNullInputParameter<Contracts.BufferedWrappedMessageWithStream>(parameters[0], "input");
        }

        [TestMethod]
        public void DeserializeMessageContractWithBufferedWrappedMessageWithMemoryStreamInputParameterReturnsStreamWithNullValueFromServiceTrace()
        {
            CallParameterInfo[] parameters = RunDeserializer("Contracts.dll", "MessageContractStreamOperationsServiceSide.svclog", false, "http://contoso.com/service/test/IBufferedStreamService/BufferedWrappedMessageWithMemoryStream");
            Assert.AreEqual<int>(1, parameters.Length);
            this.ValidateNonNullInputParameter<Contracts.BufferedWrappedMessageWithMemoryStream>(parameters[0], "input");
        }

        [TestMethod]
        public void DeserializeMessageContractWithStreamedUnwrappedMessageWithStreamInputParameterReturnsStreamWithNullValueFromServiceTrace()
        {
            CallParameterInfo[] parameters = RunDeserializer("Contracts.dll", "MessageContractStreamOperationsServiceSide.svclog", false, "http://contoso.com/service/test/IStreamedStreamService/StreamedUnwrappedMessageWithStream");
            Assert.AreEqual<int>(1, parameters.Length);
            this.ValidateNonNullInputParameter<Contracts.StreamedUnwrappedMessageWithStream>(parameters[0], "input");
        }

        [TestMethod]
        public void DeserializeMessageContractWithStreamedUnwrappedMessageWithMemoryStreamInputParameterReturnsStreamWithNullValueFromServiceTrace()
        {
            CallParameterInfo[] parameters = RunDeserializer("Contracts.dll", "MessageContractStreamOperationsServiceSide.svclog", false, "http://contoso.com/service/test/IStreamedStreamService/StreamedUnwrappedMessageWithMemoryStream");
            Assert.AreEqual<int>(1, parameters.Length);
            this.ValidateNonNullInputParameter<Contracts.StreamedUnwrappedMessageWithMemoryStream>(parameters[0], "input");
        }

        [TestMethod]
        public void DeserializeMessageContractWithStreamedWrappedMessageWithStreamInputParameterReturnsStreamWithNullValueFromServiceTrace()
        {
            CallParameterInfo[] parameters = RunDeserializer("Contracts.dll", "MessageContractStreamOperationsServiceSide.svclog", false, "http://contoso.com/service/test/IStreamedStreamService/StreamedWrappedMessageWithStream");
            Assert.AreEqual<int>(1, parameters.Length);
            this.ValidateNonNullInputParameter<Contracts.StreamedWrappedMessageWithStream>(parameters[0], "input");
        }

        [TestMethod]
        public void DeserializeMessageContractWithStreamedWrappedMessageWithMemoryStreamInputParameterReturnsStreamWithNullValueFromServiceTrace()
        {
            CallParameterInfo[] parameters = RunDeserializer("Contracts.dll", "MessageContractStreamOperationsServiceSide.svclog", false, "http://contoso.com/service/test/IStreamedStreamService/StreamedWrappedMessageWithMemoryStream");
            Assert.AreEqual<int>(1, parameters.Length);
            this.ValidateNonNullInputParameter<Contracts.StreamedWrappedMessageWithMemoryStream>(parameters[0], "input");
        }

        #endregion

        #region Test helpers

        // TODO: refactor to use this support method throughout deserializer tests
        private static CallParameterInfo[] RunDeserializer(string dll, string trace, bool clientSide, string action)
        {
            using (Parser p = TestHelper.CreateWcfParserFromFile(trace, clientSide, !clientSide, SoapActionMode.Include, action))
            {
                CallParameterInfo[] parameters;
                ProxyManager pm = new ProxyManager(dll);
                Deserializer d = new Deserializer();
                ParsedMessage parsedMessage;
                MethodInfo mi;

                parsedMessage = p.ReadNextRequest();
                mi = pm.GetContractMethod(parsedMessage.SoapAction);
                parameters = d.DeserializeInputParameters(parsedMessage.Message, mi);

                return parameters;
            }
        }

        #endregion

        #region Validation methods

        private void ValidateNonNullInputParameter<ParameterType, ValueType>(CallParameterInfo parameter, string expectedName)
        {
            this.ValidateNullInputParameter<ParameterType>(parameter, expectedName);
            Assert.AreEqual<string>(typeof(ValueType).FullName, parameter.Value.GetType().FullName);
        }

        private void ValidateNonNullInputParameter<T>(CallParameterInfo parameter, string expectedName)
        {
            this.ValidateNonNullInputParameter<T, T>(parameter, expectedName);
        }

        private void ValidateNullInputParameter<T>(CallParameterInfo parameter, string expectedName)
        {
            Assert.AreEqual<string>(expectedName, parameter.Name);
            Assert.AreEqual<string>(typeof(T).FullName, parameter.ParameterType.FullName);
            Assert.AreEqual<FieldDirection>(FieldDirection.In, parameter.Direction);
        }

        private void ValidateInputParameter<T>(CallParameterInfo parameter, string expectedName, T expectedValue)
        {
            if (expectedValue != null)
            {
                this.ValidateNonNullInputParameter<T>(parameter, expectedName);
                Assert.AreEqual<T>(expectedValue, (T)parameter.Value);
            }
            else
            {
                this.ValidateNullInputParameter<T>(parameter, expectedName);
                Assert.IsNull(parameter.Value);
            }
        }

        private void ValidateObjectInputParameter<T>(CallParameterInfo parameter, string expectedName, T expectedValue)
        {
            if (expectedValue != null)
            {
                this.ValidateNonNullInputParameter<object, string>(parameter, expectedName);
                Assert.AreEqual<T>(expectedValue, (T)parameter.Value);
            }
            else
            {
                this.ValidateNullInputParameter<object>(parameter, expectedName);
                Assert.IsNull(parameter.Value);
            }
        }

        private void ValidateOutputParameter<ParameterType, ValueType>(CallParameterInfo parameter, string expectedName)
        {
            Assert.AreEqual<string>(expectedName, parameter.Name);
            Assert.AreEqual<string>(typeof(ParameterType).FullName, parameter.ParameterType.FullName);
            Assert.IsNull(parameter.Value);
            Assert.AreEqual<FieldDirection>(FieldDirection.Out, parameter.Direction);
        }

        private void ValidateRefParameter<T>(CallParameterInfo parameter, string expectedName, T expectedValue)
        {
            Assert.AreEqual<string>(expectedName, parameter.Name);
            Assert.AreEqual<string>(typeof(T).FullName, parameter.ParameterType.FullName);
            if (parameter.Value != null)
            {
                Assert.AreEqual<string>(typeof(T).FullName, parameter.Value.GetType().FullName);
            }

            Assert.AreEqual<FieldDirection>(FieldDirection.Ref, parameter.Direction);

            // Cannot compare two object()s
            if (expectedValue.GetType() != typeof(object))
            {
                Assert.AreEqual<T>(expectedValue, (T)parameter.Value);
            }
        }

        private void ValidateOutputParameter<T>(CallParameterInfo parameter, string expectedName)
        {
            this.ValidateOutputParameter<T, T>(parameter, expectedName);
        }

        #endregion
    }
}
