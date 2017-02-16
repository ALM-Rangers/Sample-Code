//---------------------------------------------------------------------
// <copyright file="ProxyManagerTests.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
//     THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
//     OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
//     LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR
//     FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
// <summary>The ProxyManagerTests type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.WcfUnit.Library.Test
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.ServiceModel;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Microsoft.WcfUnit.Library;
    using Microsoft.WcfUnit.Library.Test.TestContracts;

    /// <summary>
    /// Tests the <see cref="ProxyManager"/> class.
    /// </summary>
    [TestClass]
    public class ProxyManagerTests
    {
        private string[] assemblies = new string[] { "ClientProxies.dll" };
        private List<Assembly> proxyAssemblies;

        public ProxyManagerTests()
        {
        }

        internal interface ITest1
        {
            void TestMethod1();
        }

        internal interface ITest2
        {
            void TestMethod2();
        }

        internal interface ITest1Message
        {
            void TestMethod1(TestMessage a);
        }

        [TestInitialize]
        public void InitializeTest()
        {
            this.proxyAssemblies = new List<Assembly>();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults", MessageId = "Microsoft.WcfUnit.Library.ProxyManager", Justification = "Constructor test")]
        [TestMethod]
        public void PMConstructorChecksForNullParameter()
        {
            TestHelper.TestForArgumentNullException(() => new ProxyManager(null), "assemblyFileNames");
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults", MessageId = "Microsoft.WcfUnit.Library.ProxyManager", Justification = "Constructor test")]
        [TestMethod]
        public void PMAssemblyFileNotFound()
        {
            TestHelper.TestForUserException(() => new ProxyManager("NoSuchFile.dll"));
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults", MessageId = "Microsoft.WcfUnit.Library.ProxyManager", Justification = "Constructor test")]
        [TestMethod]
        public void PMAssemblyFileNotAnAssembly()
        {
            TestHelper.TestForUserException(() => new ProxyManager("WithMessageBodies.svclog"));
        }

        [TestMethod]
        public void PMFindProxyType()
        {
            ProxyManager pm = new ProxyManager(this.assemblies);
            pm.TypeResolved += new EventHandler<ProxyAssemblyEventArgs>(this.TypeResolvedHandler);
            Type t = pm.GetProxyType("GeneratedContracts.AddRequest");
            Assert.AreEqual<string>("GeneratedContracts.AddRequest", t.FullName);
            Assert.AreEqual<int>(1, this.proxyAssemblies.Count);
            Assert.AreEqual<string>("ClientProxies", this.proxyAssemblies[0].GetName().Name);
        }

        [TestMethod]
        public void PMFailToFindProxyType()
        {
            ProxyManager pm = new ProxyManager(this.assemblies);
            pm.TypeResolved += new EventHandler<ProxyAssemblyEventArgs>(this.TypeResolvedHandler);
            Type t = pm.GetProxyType("contoso.com.data.test.AddRequestX");
            Assert.IsNull(t);
            Assert.AreEqual<int>(0, this.proxyAssemblies.Count);
        }

        [TestMethod]
        public void PMGetContractMethodOnInterface()
        {
            ProxyManager pm = new ProxyManager(this.assemblies);
            pm.TypeResolved += new EventHandler<ProxyAssemblyEventArgs>(this.TypeResolvedHandler);
            MethodInfo mi = pm.GetContractMethod("http://contoso.com/service/test/IArithmetic/Add");
            Assert.AreEqual<string>("Add", mi.Name);
            Assert.AreEqual<string>("GeneratedContracts.IArithmetic", mi.ReflectedType.FullName);
            Assert.AreEqual<int>(1, this.proxyAssemblies.Count);
            Assert.AreEqual<string>("ClientProxies", this.proxyAssemblies[0].GetName().Name);
        }

        [TestMethod]
        public void PMGetContractMethodOnClass()
        {
            ProxyManager pm = new ProxyManager("Microsoft.WcfUnit.Library.Test.dll");
            pm.TypeResolved += new EventHandler<ProxyAssemblyEventArgs>(this.TypeResolvedHandler);
            MethodInfo mi = pm.GetContractMethod("http:/test.test/ServiceContractClass/Operation");
            Assert.AreEqual<string>("Operation", mi.Name);
            Assert.AreEqual<string>("Microsoft.WcfUnit.Library.Test.TestContracts.ServiceContractClass", mi.ReflectedType.FullName);
            Assert.AreEqual<int>(1, this.proxyAssemblies.Count);
            Assert.AreEqual<string>("Microsoft.WcfUnit.Library.Test", this.proxyAssemblies[0].GetName().Name);
        }

        [TestMethod]
        public void PMGetProxyMethodFromInterfaceContractWithProxyDerivedFromICommunicationObject()
        {
            ProxyManager pm = new ProxyManager(this.assemblies);
            EventHandler<ProxyAssemblyEventArgs> handler = new EventHandler<ProxyAssemblyEventArgs>(this.TypeResolvedHandler);

            MethodInfo contractMethod;
            MethodInfo mi;

            contractMethod = pm.GetContractMethod("http://contoso.com/service/test/IArithmetic/Add");
            pm.TypeResolved += handler;
            mi = pm.GetProxyMethod(contractMethod);
            pm.TypeResolved -= handler;
            Assert.AreEqual<string>("Add", mi.Name);
            Assert.AreEqual<string>("GeneratedContracts.ArithmeticClient", mi.ReflectedType.FullName);
            Assert.AreEqual<int>(1, this.proxyAssemblies.Count);
        }

        [TestMethod]
        public void PMGetProxyMethodFromInterfaceContractWithNoProxyDerivedFromICommunicationObject()
        {
            ProxyManager pm = new ProxyManager("Microsoft.WcfUnit.Library.Test.dll");

            MethodInfo contractMethod;
            MethodInfo mi;

            contractMethod = pm.GetContractMethod("http:/test.test/ServiceContractInterface/Operation");
            mi = pm.GetProxyMethod(contractMethod);
            Assert.IsNull(mi);
        }

        [TestMethod]
        public void PMGetProxyMethodFromClassContractWithProxyDerivedFromICommunicationObject()
        {
            ProxyManager pm = new ProxyManager("Microsoft.WcfUnit.Library.Test.dll");
            MethodInfo contractMethod = pm.GetContractMethod("http:/test.test/ServiceContract2Class/Operation");
            pm.TypeResolved += new EventHandler<ProxyAssemblyEventArgs>(this.TypeResolvedHandler);
            MethodInfo mi = pm.GetProxyMethod(contractMethod);
            Assert.AreEqual<string>("Operation", mi.Name);
            Assert.AreEqual<string>("Microsoft.WcfUnit.Library.Test.TestContracts.ServiceContract2ClassCommunicationObjectProxy", mi.ReflectedType.FullName);
            Assert.AreEqual<int>(1, this.proxyAssemblies.Count);
        }

        [TestMethod]
        public void PMGetProxyMethodFromClassContractWithNoProxyDerivedFromICommunicationObject()
        {
            ProxyManager pm = new ProxyManager("Microsoft.WcfUnit.Library.Test.dll");
            MethodInfo contractMethod = pm.GetContractMethod("http:/test.test/ServiceContractClass/Operation");
            MethodInfo mi = pm.GetProxyMethod(contractMethod);
            Assert.IsNull(mi);
        }

        [TestMethod]
        public void PMGetProxyMethodFromClassContractWhichIsSelfProxy()
        {
            ProxyManager pm = new ProxyManager("Microsoft.WcfUnit.Library.Test.dll");
            MethodInfo contractMethod = pm.GetContractMethod("http:/test.test/ServiceContractClassSelfProxy/Operation");
            MethodInfo mi = pm.GetProxyMethod(contractMethod);
            Assert.AreSame(contractMethod, mi);
        }

        [TestMethod]
        public void PMGetProxyMethodFromInterfaceContractSeparateAssemblies()
        {
            ProxyManager pm = new ProxyManager("Contracts.Custom.dll", "ClientProxies.Custom.dll");
            pm.TypeResolved += new EventHandler<ProxyAssemblyEventArgs>(this.TypeResolvedHandler);
            MethodInfo contractMethod;
            MethodInfo mi;

            contractMethod = pm.GetContractMethod("http://tempuri.org/ICustomContracts/Overload");
            mi = pm.GetProxyMethod(contractMethod);
            Assert.AreEqual<string>("Overload", mi.Name);
            Assert.AreEqual<string>("ClientProxies.Custom.CustomContracts", mi.ReflectedType.FullName);
            Assert.AreEqual<int>(2, this.proxyAssemblies.Count);
       }

        [TestMethod]
        public void PMFailToGetContractMethod()
        {
            ProxyManager pm = new ProxyManager(this.assemblies);
            pm.TypeResolved += new EventHandler<ProxyAssemblyEventArgs>(this.TypeResolvedHandler);
            MethodInfo mi = pm.GetContractMethod("http://contoso.com/service/test/IArithmetic/AddXXX");
            Assert.IsNull(mi);
        }

        [TestMethod]
        public void PMFailToFindOperationContractWithoutServiceContract()
        {
            ProxyManager pm = new ProxyManager("Microsoft.WcfUnit.Library.Test.dll");

            MethodInfo mi = pm.GetContractMethod("http:/test.test/OperationContractWithoutServiceContract");
            Assert.IsNull(mi);
        }

        [TestMethod]
        public void PMFindDefaultSoapActionWithContractNamespace()
        {
            ProxyManager pm = new ProxyManager("Microsoft.WcfUnit.Library.Test.dll");

            MethodInfo mi = pm.GetContractMethod("http://test.test/test/IDefaultSoapActionWithContractNamespace/DefaultSoapActionWithContractNamespace");
            Assert.AreEqual<string>("DefaultSoapActionWithContractNamespace", mi.Name);
            Assert.AreEqual<string>("Microsoft.WcfUnit.Library.Test.TestContracts.IDefaultSoapActionWithContractNamespace", mi.ReflectedType.FullName);
        }

        [TestMethod]
        public void PMFindDefaultSoapActionWithContractNamespaceAndOverridingContractName()
        {
            ProxyManager pm = new ProxyManager("Microsoft.WcfUnit.Library.Test.dll");

            MethodInfo mi = pm.GetContractMethod("http://test.test/test/OverriddenName/DefaultSoapActionWithContractNamespace");
            Assert.AreEqual<string>("DefaultSoapActionWithContractNamespace", mi.Name);
            Assert.AreEqual<string>("Microsoft.WcfUnit.Library.Test.TestContracts.IDefaultSoapActionWithContractNamespaceAndNameThatOverridesTypeName", mi.ReflectedType.FullName);
        }

        [TestMethod]
        public void PMFindDefaultSoapActionWithContractNamespaceAndSlash()
        {
            ProxyManager pm = new ProxyManager("Microsoft.WcfUnit.Library.Test.dll");

            MethodInfo mi = pm.GetContractMethod("http://test.test/test/IDefaultSoapActionWithContractNamespaceAndSlash/DefaultSoapActionWithContractNamespaceAndSlash");
            Assert.AreEqual<string>("DefaultSoapActionWithContractNamespaceAndSlash", mi.Name);
            Assert.AreEqual<string>("Microsoft.WcfUnit.Library.Test.TestContracts.IDefaultSoapActionWithContractNamespaceAndSlash", mi.ReflectedType.FullName);
        }

        [TestMethod]
        public void PMFindDefaultSoapActionWithContractNamespaceAndSlashAndOverridingContractName()
        {
            ProxyManager pm = new ProxyManager("Microsoft.WcfUnit.Library.Test.dll");

            MethodInfo mi = pm.GetContractMethod("http://test.test/test/OverriddenName/DefaultSoapActionWithContractNamespaceAndSlash");
            Assert.AreEqual<string>("DefaultSoapActionWithContractNamespaceAndSlash", mi.Name);
            Assert.AreEqual<string>("Microsoft.WcfUnit.Library.Test.TestContracts.IDefaultSoapActionWithContractNamespaceAndSlashAndNameThatOverridesTypeName", mi.ReflectedType.FullName);
        }

        [TestMethod]
        public void PMFindDefaultSoapActionWithEmptyContractNamespace()
        {
            ProxyManager pm = new ProxyManager("Microsoft.WcfUnit.Library.Test.dll");

            MethodInfo mi = pm.GetContractMethod("urn:IDefaultSoapActionWithEmptyContractNamespace/DefaultSoapActionWithEmptyContractNamespace");
            Assert.AreEqual<string>("DefaultSoapActionWithEmptyContractNamespace", mi.Name);
            Assert.AreEqual<string>("Microsoft.WcfUnit.Library.Test.TestContracts.IDefaultSoapActionWithEmptyContractNamespace", mi.ReflectedType.FullName);
        }

        [TestMethod]
        public void PMFindDefaultSoapActionWithEmptyContractNamespaceAndOverridingContractName()
        {
            ProxyManager pm = new ProxyManager("Microsoft.WcfUnit.Library.Test.dll");

            MethodInfo mi = pm.GetContractMethod("urn:OverriddenName/DefaultSoapActionWithEmptyContractNamespace");
            Assert.AreEqual<string>("DefaultSoapActionWithEmptyContractNamespace", mi.Name);
            Assert.AreEqual<string>("Microsoft.WcfUnit.Library.Test.TestContracts.IDefaultSoapActionWithEmptyContractNamespaceAndNameThatOverridesTypeName", mi.ReflectedType.FullName);
        }

        [TestMethod]
        public void PMFindDefaultSoapActionWithDefaultContractNamespace()
        {
            ProxyManager pm = new ProxyManager("Microsoft.WcfUnit.Library.Test.dll");

            MethodInfo mi = pm.GetContractMethod("http://tempuri.org/IDefaultSoapActionWithDefaultContractNamespace/DefaultSoapActionWithDefaultContractNamespace");
            Assert.AreEqual<string>("DefaultSoapActionWithDefaultContractNamespace", mi.Name);
            Assert.AreEqual<string>("Microsoft.WcfUnit.Library.Test.TestContracts.IDefaultSoapActionWithDefaultContractNamespace", mi.ReflectedType.FullName);
        }

        [TestMethod]
        public void PMFindDefaultSoapActionWithDefaultContractNamespaceAndOverridingContractName()
        {
            ProxyManager pm = new ProxyManager("Microsoft.WcfUnit.Library.Test.dll");

            MethodInfo mi = pm.GetContractMethod("http://tempuri.org/OverriddenName/DefaultSoapActionWithDefaultContractNamespace");
            Assert.AreEqual<string>("DefaultSoapActionWithDefaultContractNamespace", mi.Name);
            Assert.AreEqual<string>("Microsoft.WcfUnit.Library.Test.TestContracts.IDefaultSoapActionWithDefaultContractNamespaceAndNameThatOverridesTypeName", mi.ReflectedType.FullName);
        }

        [TestMethod]
        public void PMGetContractMethodFromInterfaceWithOverloadDefaultOverload()
        {
            ProxyManager pm = new ProxyManager("Microsoft.WcfUnit.Library.Test.dll");

            MethodInfo mi = pm.GetContractMethod("http://tempuri.org/IContractWithOverloadedMethods/Overload");
            Assert.AreEqual<string>("Overload", mi.Name);
            Assert.AreEqual<Type>(typeof(int), mi.GetParameters()[0].ParameterType);
            Assert.AreEqual<string>("Microsoft.WcfUnit.Library.Test.TestContracts.IContractWithOverloadedMethods", mi.ReflectedType.FullName);
        }

        [TestMethod]
        public void PMGetContractMethodFromInterfaceWithOverloadSecondOverload()
        {
            ProxyManager pm = new ProxyManager("Microsoft.WcfUnit.Library.Test.dll");

            MethodInfo mi = pm.GetContractMethod("http://tempuri.org/IContractWithOverloadedMethods/OverloadString");
            Assert.AreEqual<string>("Overload", mi.Name);
            Assert.AreEqual<Type>(typeof(string), mi.GetParameters()[0].ParameterType);
            Assert.AreEqual<string>("Microsoft.WcfUnit.Library.Test.TestContracts.IContractWithOverloadedMethods", mi.ReflectedType.FullName);
        }

        [TestMethod]
        public void PMGetProxyMethodFromInterfaceWithOverloadDefaultOverload()
        {
            ProxyManager pm = new ProxyManager("Microsoft.WcfUnit.Library.Test.dll");

            MethodInfo mi = pm.GetContractMethod("http://tempuri.org/IContractWithOverloadedMethods/Overload");
            mi = pm.GetProxyMethod(mi);
            Assert.AreEqual<string>("Overload", mi.Name);
            Assert.AreEqual<Type>(typeof(int), mi.GetParameters()[0].ParameterType);
            Assert.AreEqual<string>("Microsoft.WcfUnit.Library.Test.TestContracts.ProxyClassForOperationContractTestInterfaces", mi.ReflectedType.FullName);
        }

        [TestMethod]
        public void PMGetProxyMethodFromInterfaceWithOverloadSecondOverload()
        {
            ProxyManager pm = new ProxyManager("Microsoft.WcfUnit.Library.Test.dll");

            MethodInfo mi = pm.GetContractMethod("http://tempuri.org/IContractWithOverloadedMethods/OverloadString");
            mi = pm.GetProxyMethod(mi);
            Assert.AreEqual<string>("Overload", mi.Name);
            Assert.AreEqual<Type>(typeof(string), mi.GetParameters()[0].ParameterType);
            Assert.AreEqual<string>("Microsoft.WcfUnit.Library.Test.TestContracts.ProxyClassForOperationContractTestInterfaces", mi.ReflectedType.FullName);
        }

        [TestMethod]
        public void PMGetProxyMethodFromExplicitInterfaceImplementation()
        {
            ProxyManager pm = new ProxyManager("ClientProxies.Custom.dll", "Contracts.Custom.dll");

            MethodInfo mi = pm.GetContractMethod("http://tempuri.org/ICustomContracts/Hidden");
            mi = pm.GetProxyMethod(mi);
            Assert.AreEqual<string>("Contracts.Custom.ICustomContracts.Hidden", mi.Name);
            Assert.AreEqual<Type>(typeof(string), mi.GetParameters()[0].ParameterType);
            Assert.AreEqual<string>("ClientProxies.Custom.CustomContracts", mi.ReflectedType.FullName);
        }

        [TestMethod]
        public void PMNotMessageContractConcrete()
        {
            Assert.IsFalse(ProxyManager.IsMessageContractMethod(typeof(ConcreteTest).GetMethod("TestMethod1")));
        }

        [TestMethod]
        public void PMNotMessageContractInterface()
        {
            Assert.IsFalse(ProxyManager.IsMessageContractMethod(typeof(ITest1).GetMethod("TestMethod1")));
        }

        [TestMethod]
        public void PMIsMessageContractConcreteOnInterface()
        {
            Assert.IsTrue(ProxyManager.IsMessageContractMethod(typeof(ConcreteTestMessage).GetMethod("TestMethod1")));
        }

        [TestMethod]
        public void PMIsMessageContractConcreteOnClass()
        {
            Assert.IsTrue(ProxyManager.IsMessageContractMethod(typeof(ConcreteTestMessage).GetMethod("TestMethod3")));
        }

        [TestMethod]
        public void PMIsMessageContractInterface()
        {
            Assert.IsTrue(ProxyManager.IsMessageContractMethod(typeof(ITest1Message).GetMethod("TestMethod1")));
        }

        [TestMethod]
        public void PMFindProxyMethodRequiringDynamicAssemblyResolution()
        {
            // Exercise the need of the proxy manager to
            // resolve assembly loads, by using assemblies that are not in the normal search path
            // as they are in a completely different folder.
            string tempDir = TestHelper.CopyComplexSampleToTempFolder();

            string path = tempDir + @"\Microsoft.Sample.WCF.Client.exe";
            List<string> assembliesColl = new List<string>();
            ScenarioRunManager.GetReferencedAssemblies(path, assembliesColl);
            this.assemblies = new string[assembliesColl.Count];
            assembliesColl.CopyTo(this.assemblies, 0);
            ProxyManager pm = new ProxyManager(this.assemblies);
            MethodInfo contractMethod = pm.GetContractMethod("http://microsoft.com/sample/wcf/ISampleService/Logon");
            pm.GetProxyMethod(contractMethod);
        }

        [TestMethod]
        public void PMIsXmlSerializerThrowsExceptionForNullArgument()
        {
            try
            {
                ProxyManager.IsXmlSerializerMethod(null);
                Assert.Fail("Expected ArgumentNullException not thrown");
            }
            catch (ArgumentNullException)
            {
            }
        }

        [TestMethod]
        public void PMIsXmlSerializerFalseForClassWithNoXmlFormatAttribute()
        {
            Assert.IsFalse(ProxyManager.IsXmlSerializerMethod(typeof(IServiceContractInterface).GetMethod("Operation")));
        }

        [TestMethod]
        public void PMIsXmlSerializerTrueForAllMethodsWithXmlFormatAttributeAtClassLevel()
        {
            Assert.IsTrue(ProxyManager.IsXmlSerializerMethod(typeof(IXmlSerializerAtClassLevel).GetMethod("Method1")));
            Assert.IsTrue(ProxyManager.IsXmlSerializerMethod(typeof(IXmlSerializerAtClassLevel).GetMethod("Method2")));
        }

        [TestMethod]
        public void PMIsXmlSerializerFalseForOverriddenMethodsWithXmlFormatAttributeAtClassLevel()
        {
            Assert.IsFalse(ProxyManager.IsXmlSerializerMethod(typeof(IXmlSerializerAtClassLevel).GetMethod("Method3")));
        }

        [TestMethod]
        public void PMIsXmlSerializerFalseForUnmarkedMethodsWithXmlFormatAttributeAtMethodLevel()
        {
            Assert.IsFalse(ProxyManager.IsXmlSerializerMethod(typeof(IXmlSerializerAtMethodLevel).GetMethod("Method1")));
        }

        [TestMethod]
        public void PMIsXmlSerializerTrueForMethodsWithXmlFormatAttributeAtMethodLevel()
        {
            Assert.IsTrue(ProxyManager.IsXmlSerializerMethod(typeof(IXmlSerializerAtMethodLevel).GetMethod("Method2")));
        }

        private void TypeResolvedHandler(object sender, ProxyAssemblyEventArgs e)
        {
            bool found = false;
            foreach (Assembly a in this.proxyAssemblies)
            {
                if (a.FullName == e.Assembly.FullName)
                {
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                this.proxyAssemblies.Add(e.Assembly);
            }
        }

        internal class ConcreteTest : ITest1, ITest2
        {
            #region ITest1 Members

            public void TestMethod1()
            {
            }

            #endregion

            #region ITest2 Members

            public void TestMethod2()
            {
            }

            public void TestMethod3()
            {
            }

            #endregion
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses", Justification = "Used indirectly in tests")]
        [MessageContract]
        internal class TestMessage
        {
        }

        internal class ConcreteTestMessage : ITest1Message
        {
            #region ITest1 Members

            public void TestMethod1(TestMessage a)
            {
            }

            #endregion

            public void TestMethod3(TestMessage a)
            {
            }
        }
    }
}
