//---------------------------------------------------------------------
// <copyright file="ConfigurationTests.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
//     THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
//     OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
//     LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR
//     FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
// <summary>The ConfigurationTests type.</summary>
//---------------------------------------------------------------------
namespace Microsoft.WcfUnit.Library.Test
{
    using System;
    using System.IO;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Microsoft.WcfUnit.Library;

    /// <summary>
    /// Tests the <see cref="Configuration"/> class.
    /// </summary>
    [TestClass]
    public class ConfigurationTests
    {
        private string configFileName;

        public ConfigurationTests()
        {
        }

        [TestInitialize]
        public void InitConfigFile()
        {
            this.configFileName = Path.GetTempFileName();
        }

        [TestCleanup]
        public void CleanupConfigFile()
        {
            File.Delete(this.configFileName);
        }

        [TestMethod]
        public void ConfigurationSimple()
        {
            this.CreateConfigFile(@"<?xml version='1.0' encoding='utf-8' ?>
<WcfUnitConfiguration xmlns='http://microsoft.com/wcfunit' testMethodMode='ScenarioMethodOnly' operationTimerMode='IncludeOperationTimers'>
  <assembly fileName='abc'/>
  <soapActions soapActionMode='Include'>
    <soapAction action='http:/abc/def'/>
  </soapActions>
<parser assembly='abc' type='def'/>
</WcfUnitConfiguration>
");
            WcfUnitConfiguration config = ConfigurationReader.Read(this.configFileName);

            Assert.IsNotNull(config);
            Assert.AreEqual<TestMethodMode>(TestMethodMode.ScenarioMethodOnly, config.testMethodMode);
            Assert.AreEqual<OperationTimerMode>(OperationTimerMode.IncludeOperationTimers, config.operationTimerMode);
            Assert.AreEqual<int>(1, config.assembly.Length);
            Assert.AreEqual<string>("abc", config.assembly[0].fileName);
            Assert.AreEqual<SoapActionMode>(SoapActionMode.Include, config.soapActions.soapActionMode);
            Assert.AreEqual<int>(1, config.soapActions.soapAction.Length);
            Assert.AreEqual<string>("http:/abc/def", config.soapActions.soapAction[0].action);
            Assert.AreEqual<string>("abc", config.parser.assembly);
            Assert.AreEqual<string>("def", config.parser.type);
        }

        [TestMethod]
        public void ConfigurationNoListedSoapActions()
        {
            this.CreateConfigFile(@"<?xml version='1.0' encoding='utf-8' ?>
<WcfUnitConfiguration xmlns='http://microsoft.com/wcfunit' testMethodMode='ScenarioMethodOnly' operationTimerMode='IncludeOperationTimers'>
  <assembly fileName='abc'/>
  <soapActions soapActionMode='Include'>
  </soapActions>
</WcfUnitConfiguration>
");
            WcfUnitConfiguration config = ConfigurationReader.Read(this.configFileName);

            Assert.IsNotNull(config);
            Assert.AreEqual<int>(1, config.assembly.Length);
            Assert.AreEqual<string>("abc", config.assembly[0].fileName);
            Assert.IsNull(config.soapActions.soapAction);
        }

        [TestMethod]
        public void ConfigurationNoSoapActions()
        {
            this.CreateConfigFile(@"<?xml version='1.0' encoding='utf-8' ?>
<WcfUnitConfiguration xmlns='http://microsoft.com/wcfunit' testMethodMode='ScenarioMethodOnly' operationTimerMode='IncludeOperationTimers'>
  <assembly fileName='abc'/>
</WcfUnitConfiguration>
");
            TestHelper.TestForUserException(() => ConfigurationReader.Read(this.configFileName));
        }

        [TestMethod]
        public void ConfigurationInvalidNamespace()
        {
            this.CreateConfigFile(@"<?xml version='1.0' encoding='utf-8' ?>
<WcfUnitConfiguration xmlns='http://microsoft.com/wcfunitx'>
  <assembly fileName='abc'/>
  <soapAction action='http:/abc/def'/>
</WcfUnitConfiguration>
");
            TestHelper.TestForUserException(() => ConfigurationReader.Read(this.configFileName));
        }

        [TestMethod]
        public void ConfigurationNoAssemblies()
        {
            this.CreateConfigFile(@"<?xml version='1.0' encoding='utf-8' ?>
<WcfUnitConfiguration xmlns='http://microsoft.com/wcfunit'>
  <soapAction action='http:/abc/def'/>
</WcfUnitConfiguration>
");
            TestHelper.TestForUserException(() => ConfigurationReader.Read(this.configFileName));
        }

        [TestMethod]
        public void ConfigurationBinaryFile()
        {
            TestHelper.TestForUserException(() => ConfigurationReader.Read("Microsoft.WcfUnit.Library.Test.dll"));
        }

        [TestMethod]
        public void ConfigurationFileNotFound()
        {
            TestHelper.TestForUserException(() => ConfigurationReader.Read("NoSuchFile.xml"));
        }

        [TestMethod]
        public void ConfigurationClientTraceSettingDefaultsToTrue()
        {
            this.CreateConfigFile(@"<?xml version='1.0' encoding='utf-8' ?>
<WcfUnitConfiguration xmlns='http://microsoft.com/wcfunit' testMethodMode='ScenarioMethodOnly' operationTimerMode='IncludeOperationTimers'>
  <assembly fileName='abc'/>
  <soapActions soapActionMode='Include'>
    <soapAction action='http:/abc/def'/>
  </soapActions>
</WcfUnitConfiguration>
");
            WcfUnitConfiguration config = ConfigurationReader.Read(this.configFileName);

            Assert.IsTrue(config.clientTrace);
        }

        [TestMethod]
        public void ConfigurationClientTraceSettingValueReadFromConfigurationFile()
        {
            this.CreateConfigFile(@"<?xml version='1.0' encoding='utf-8' ?>
<WcfUnitConfiguration xmlns='http://microsoft.com/wcfunit' testMethodMode='ScenarioMethodOnly' operationTimerMode='IncludeOperationTimers' clientTrace='false'>
  <assembly fileName='abc'/>
  <soapActions soapActionMode='Include'>
    <soapAction action='http:/abc/def'/>
  </soapActions>
</WcfUnitConfiguration>
");
            WcfUnitConfiguration config = ConfigurationReader.Read(this.configFileName);

            Assert.IsFalse(config.clientTrace);
        }

        [TestMethod]
        public void ConfigurationServiceTraceSettingDefaultsToFalse()
        {
            this.CreateConfigFile(@"<?xml version='1.0' encoding='utf-8' ?>
<WcfUnitConfiguration xmlns='http://microsoft.com/wcfunit' testMethodMode='ScenarioMethodOnly' operationTimerMode='IncludeOperationTimers'>
  <assembly fileName='abc'/>
  <soapActions soapActionMode='Include'>
    <soapAction action='http:/abc/def'/>
  </soapActions>
</WcfUnitConfiguration>
");
            WcfUnitConfiguration config = ConfigurationReader.Read(this.configFileName);

            Assert.IsFalse(config.serviceTrace);
        }

        [TestMethod]
        public void ConfigurationServiceTraceSettingValueReadFromConfigurationFile()
        {
            this.CreateConfigFile(@"<?xml version='1.0' encoding='utf-8' ?>
<WcfUnitConfiguration xmlns='http://microsoft.com/wcfunit' testMethodMode='ScenarioMethodOnly' operationTimerMode='IncludeOperationTimers' serviceTrace='true'>
  <assembly fileName='abc'/>
  <soapActions soapActionMode='Include'>
    <soapAction action='http:/abc/def'/>
  </soapActions>
</WcfUnitConfiguration>
");
            WcfUnitConfiguration config = ConfigurationReader.Read(this.configFileName);

            Assert.IsTrue(config.serviceTrace);
        }

        [TestMethod]
        public void ConfigurationNoParserReturnsNullForParser()
        {
            this.CreateConfigFile(@"<?xml version='1.0' encoding='utf-8' ?>
<WcfUnitConfiguration xmlns='http://microsoft.com/wcfunit' testMethodMode='ScenarioMethodOnly' operationTimerMode='IncludeOperationTimers'>
  <assembly fileName='abc'/>
  <soapActions soapActionMode='Include'/>
</WcfUnitConfiguration>
");
            WcfUnitConfiguration config = ConfigurationReader.Read(this.configFileName);

            Assert.IsNull(config.parser);
        }

        [TestMethod]
        public void ConfigurationMissingAssemblyInParserTypeThrowsException()
        {
            this.CreateConfigFile(@"<?xml version='1.0' encoding='utf-8' ?>
<WcfUnitConfiguration xmlns='http://microsoft.com/wcfunit' testMethodMode='ScenarioMethodOnly' operationTimerMode='IncludeOperationTimers'>
  <assembly fileName='abc'/>
  <soapActions soapActionMode='Include'/>
<parser type='def'/>
</WcfUnitConfiguration>
");
            TestHelper.TestForUserException(
                delegate
                {
                    ConfigurationReader.Read(this.configFileName);
                });
        }

        [TestMethod]
        public void ConfigurationMissingTypeInParserTypeThrowsException()
        {
            this.CreateConfigFile(@"<?xml version='1.0' encoding='utf-8' ?>
<WcfUnitConfiguration xmlns='http://microsoft.com/wcfunit' testMethodMode='ScenarioMethodOnly' operationTimerMode='IncludeOperationTimers'>
  <assembly fileName='abc'/>
  <soapActions soapActionMode='Include'/>
<parser assembly='abc'/>
</WcfUnitConfiguration>
");
            TestHelper.TestForUserException(
                delegate
                {
                    ConfigurationReader.Read(this.configFileName);
                });
        }

        [TestMethod]
        public void ConfigurationMoreThanOneParserThrowsException()
        {
            this.CreateConfigFile(@"<?xml version='1.0' encoding='utf-8' ?>
<WcfUnitConfiguration xmlns='http://microsoft.com/wcfunit' testMethodMode='ScenarioMethodOnly' operationTimerMode='IncludeOperationTimers'>
  <assembly fileName='abc'/>
  <soapActions soapActionMode='Include'/>
<parser assembly='abc' type='def'/>
<parser assembly='abc2' type='def2'/>
</WcfUnitConfiguration>
");
            TestHelper.TestForUserException(
                delegate
                {
                    ConfigurationReader.Read(this.configFileName);
                });
        }

        private void CreateConfigFile(string xml)
        {
            using (StreamWriter sw = new StreamWriter(this.configFileName))
            {
                sw.Write(xml);
            }
        }
    }
}
