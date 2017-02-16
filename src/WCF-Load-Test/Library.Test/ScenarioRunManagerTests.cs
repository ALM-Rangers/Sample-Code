//---------------------------------------------------------------------
// <copyright file="ScenarioRunManagerTests.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
//     THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
//     OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
//     LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR
//     FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
// <summary>The ScenarioRunManagerTests type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.WcfUnit.Library.Test
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Security.AccessControl;
    using System.Text;
    using System.Xml;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.WcfUnit.Library;

    /// <summary>
    /// Tests for <see cref="ScenarioRunManager"/>.
    /// </summary>
    [TestClass]
    public class ScenarioRunManagerTests
    {
        private const string ExpectedDiagnosticsSectionTemplate = @"
  <system.diagnostics>
    <sources>
      <source name=""System.ServiceModel.MessageLogging"" switchValue=""Warning, ActivityTracing"">
        <listeners>
          <add type=""System.Diagnostics.DefaultTraceListener"" name=""Default"">
            <filter type="""" />
          </add>
          <add name=""ServiceModelMessageLoggingListener"">
            <filter type="""" />
          </add>
        </listeners>
      </source>
    </sources>
    <sharedListeners>
      <add initializeData=""#LOGFILE#""
        type=""System.Diagnostics.XmlWriterTraceListener, System, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089""
        name=""ServiceModelMessageLoggingListener"" traceOutputOptions=""Timestamp"">
        <filter type="""" />
      </add>
    </sharedListeners>
  </system.diagnostics>
";

        private string expectedServiceModelSection = @"<diagnostics>
      <messageLogging logEntireMessage=""true"" logMalformedMessages=""false""
        logMessagesAtServiceLevel=""true"" logMessagesAtTransportLevel=""false"" />
    </diagnostics>";

        private string configFileName;
        private string exeFileName;

        public ScenarioRunManagerTests()
        {
        }

        [TestInitialize]
        public void InitConfigFile()
        {
            this.exeFileName = Path.GetTempFileName();
            this.configFileName = this.exeFileName + ".config";
        }

        [TestCleanup]
        public void CleanupConfigFile()
        {
            File.Delete(this.exeFileName);
            File.Delete(this.configFileName);
        }

        [TestMethod]
        public void SRMNoConfigFileWithRestore()
        {
            string expectedConfig = @"<?xml version=""1.0"" encoding=""utf-8""?><configuration>"
                                    +
                                    ExpectedDiagnosticsSectionTemplate
                                    +
                                    @"<system.serviceModel>"
                                    +
                                    this.expectedServiceModelSection
                                    +
                                    @"</system.serviceModel>"
                                    +
                                    @"</configuration>";
            using (ScenarioRunManager mgr = new ScenarioRunManager())
            {
                mgr.Initialize(this.exeFileName);
                string logFileName = mgr.SetupForTrace();
                this.CompareConfig(expectedConfig, logFileName);
                mgr.RestoreOriginalConfiguration();
                Assert.IsFalse(File.Exists(this.configFileName));
            }
        }

        [TestMethod]
        public void SRMNoConfigFileWithDispose()
        {
            string expectedConfig = @"<?xml version=""1.0"" encoding=""utf-8""?><configuration>"
                                    +
                                    ExpectedDiagnosticsSectionTemplate
                                    +
                                    @"<system.serviceModel>"
                                    +
                                    this.expectedServiceModelSection
                                    +
                                    @"</system.serviceModel>"
                                    +
                                    @"</configuration>";
            using (ScenarioRunManager mgr = new ScenarioRunManager())
            {
                mgr.Initialize(this.exeFileName);
                string logFileName = mgr.SetupForTrace();
                this.CompareConfig(expectedConfig, logFileName);
            }

            Assert.IsFalse(File.Exists(this.configFileName));
        }

        [TestMethod]
        public void SRMNoConfigFileWithRestoreAndDispose()
        {
            string expectedConfig = @"<?xml version=""1.0"" encoding=""utf-8""?><configuration>"
                                    +
                                    ExpectedDiagnosticsSectionTemplate
                                    +
                                    @"<system.serviceModel>"
                                    +
                                    this.expectedServiceModelSection
                                    +
                                    @"</system.serviceModel>"
                                    +
                                    @"</configuration>";
            using (ScenarioRunManager mgr = new ScenarioRunManager())
            {
                mgr.Initialize(this.exeFileName);
                string logFileName = mgr.SetupForTrace();
                this.CompareConfig(expectedConfig, logFileName);
                mgr.RestoreOriginalConfiguration();
                Assert.IsFalse(File.Exists(this.configFileName));
            }
        }

        [TestMethod]
        public void SRMNoConfigFileWithRestoreAndUsing()
        {
            string expectedConfig = @"<?xml version=""1.0"" encoding=""utf-8""?><configuration>"
                                    +
                                    ExpectedDiagnosticsSectionTemplate
                                    +
                                    @"<system.serviceModel>"
                                    +
                                    this.expectedServiceModelSection
                                    +
                                    @"</system.serviceModel>"
                                    +
                                    @"</configuration>";
            using (ScenarioRunManager mgr = new ScenarioRunManager())
            {
                mgr.Initialize(this.exeFileName);
                string logFileName = mgr.SetupForTrace();
                this.CompareConfig(expectedConfig, logFileName);
                mgr.RestoreOriginalConfiguration();
                Assert.IsFalse(File.Exists(this.configFileName));
            }
        }

        [TestMethod]
        public void SRMNoPermission()
        {
            // Don't use global variables for exe and config file as those files will get deleted at the end of the test
            string exePath = Path.GetFullPath(@"..\..\..\SampleClientAndService\Client\bin\debug\client.exe");
            string configPath = exePath + ".config";
            string dirName = Path.GetDirectoryName(exePath);
            DirectorySecurity acl = Directory.GetAccessControl(dirName);
            FileSystemAccessRule rule = new FileSystemAccessRule("Everyone", FileSystemRights.Modify, AccessControlType.Deny);
            try
            {
                acl.AddAccessRule(rule);
                Directory.SetAccessControl(dirName, acl);
                using (ScenarioRunManager mgr = new ScenarioRunManager())
                {
                    mgr.Initialize(exePath);
                    mgr.SetupForTrace();
                    Assert.Fail("Should have thrown a user exception here");
                }
            }
            catch (UserException)
            {
            }
            finally
            {
                acl.RemoveAccessRule(rule);
                Directory.SetAccessControl(dirName, acl);
            }

            Assert.IsTrue(File.Exists(configPath));
        }

        [TestMethod]
        public void SRMNoSuchExecutable()
        {
            using (ScenarioRunManager mgr = new ScenarioRunManager())
            {
                TestHelper.TestForUserException(() => mgr.Initialize("NoSuch.exe"));
            }
        }

        [TestMethod]
        public void SRMModifyEmptyConfigAndRestore()
        {
            string initialConfig = @"<?xml version=""1.0"" encoding=""utf-8""?><configuration></configuration>";
            string expectedConfig = @"<?xml version=""1.0"" encoding=""utf-8""?><configuration>"
                                    +
                                    ExpectedDiagnosticsSectionTemplate
                                    +
                                    @"<system.serviceModel>"
                                    +
                                    this.expectedServiceModelSection
                                    +
                                    @"</system.serviceModel>"
                                    +
                                    @"</configuration>";
            this.CreateConfigFile(initialConfig);
            using (ScenarioRunManager mgr = new ScenarioRunManager())
            {
                mgr.Initialize(this.exeFileName);
                string logFileName = mgr.SetupForTrace();
                this.CompareConfig(expectedConfig, logFileName);
                mgr.RestoreOriginalConfiguration();
                this.CompareConfig(initialConfig, string.Empty);
            }
        }

        [TestMethod]
        public void SRMModifyEmptyConfigAndDispose()
        {
            string initialConfig = @"<?xml version=""1.0"" encoding=""utf-8""?><configuration></configuration>";
            string expectedConfig = @"<?xml version=""1.0"" encoding=""utf-8""?><configuration>"
                                    +
                                    ExpectedDiagnosticsSectionTemplate
                                    +
                                    @"<system.serviceModel>"
                                    +
                                    this.expectedServiceModelSection
                                    +
                                    @"</system.serviceModel>"
                                    +
                                    @"</configuration>";
            this.CreateConfigFile(initialConfig);
            using (ScenarioRunManager mgr = new ScenarioRunManager())
            {
                mgr.Initialize(this.exeFileName);
                string logFileName = mgr.SetupForTrace();
                this.CompareConfig(expectedConfig, logFileName);
            }

            this.CompareConfig(initialConfig, string.Empty);
        }

        [TestMethod]
        public void SRMModifyEmptyConfigRestoreWithDispose()
        {
            string initialConfig = @"<?xml version=""1.0"" encoding=""utf-8""?><configuration></configuration>";
            string expectedConfig = @"<?xml version=""1.0"" encoding=""utf-8""?><configuration>"
                                    +
                                    ExpectedDiagnosticsSectionTemplate
                                    +
                                    @"<system.serviceModel>"
                                    +
                                    this.expectedServiceModelSection
                                    +
                                    @"</system.serviceModel>"
                                    +
                                    @"</configuration>";
            this.CreateConfigFile(initialConfig);
            using (ScenarioRunManager mgr = new ScenarioRunManager())
            {
                mgr.Initialize(this.exeFileName);
                string logFileName = mgr.SetupForTrace();
                this.CompareConfig(expectedConfig, logFileName);
                mgr.RestoreOriginalConfiguration();
                this.CompareConfig(initialConfig, string.Empty);
            }
        }

        [TestMethod]
        public void SRMModifyEmptyConfigRestoreWithUsing()
        {
            string initialConfig = @"<?xml version=""1.0"" encoding=""utf-8""?><configuration></configuration>";
            string expectedConfig = @"<?xml version=""1.0"" encoding=""utf-8""?><configuration>"
                                    +
                                    ExpectedDiagnosticsSectionTemplate
                                    +
                                    @"<system.serviceModel>"
                                    +
                                    this.expectedServiceModelSection
                                    +
                                    @"</system.serviceModel>"
                                    +
                                    @"</configuration>";
            this.CreateConfigFile(initialConfig);
            using (ScenarioRunManager mgr = new ScenarioRunManager())
            {
                mgr.Initialize(this.exeFileName);
                string logFileName = mgr.SetupForTrace();
                this.CompareConfig(expectedConfig, logFileName);
                mgr.RestoreOriginalConfiguration();
                this.CompareConfig(initialConfig, string.Empty);
            }
        }

        [TestMethod]
        public void SRMModifyExistingSystemDiagnosticsSection()
        {
            string initialConfig = @"<?xml version=""1.0"" encoding=""utf-8""?><configuration><system.diagnostics>stuff</system.diagnostics></configuration>";
            string expectedConfig = @"<?xml version=""1.0"" encoding=""utf-8""?><configuration>"
                                    +
                                    ExpectedDiagnosticsSectionTemplate
                                    +
                                    @"<system.serviceModel>"
                                    +
                                    this.expectedServiceModelSection
                                    +
                                    @"</system.serviceModel>"
                                    +
                                    @"</configuration>";
            this.CreateConfigFile(initialConfig);
            using (ScenarioRunManager mgr = new ScenarioRunManager())
            {
                mgr.Initialize(this.exeFileName);
                string logFileName = mgr.SetupForTrace();
                this.CompareConfig(expectedConfig, logFileName);
                mgr.RestoreOriginalConfiguration();
                this.CompareConfig(initialConfig, string.Empty);
            }
        }

        [TestMethod]
        public void SRMModifyExistingServiceModelDiagnosticsSection()
        {
            string initialConfig = @"<?xml version=""1.0"" encoding=""utf-8""?><configuration><system.serviceModel><diagnostics><messageLogging logEntireMessage=""false"" logMalformedMessages=""true"" logMessagesAtServiceLevel=""false"" logMessagesAtTransportLevel=""true"" /></diagnostics></system.serviceModel></configuration>";
            string expectedConfig = @"<?xml version=""1.0"" encoding=""utf-8""?><configuration>"
                                    +
                                    ExpectedDiagnosticsSectionTemplate
                                    +
                                    @"<system.serviceModel>"
                                    +
                                    this.expectedServiceModelSection
                                    +
                                    @"</system.serviceModel>"
                                    +
                                    @"</configuration>";
            this.CreateConfigFile(initialConfig);
            using (ScenarioRunManager mgr = new ScenarioRunManager())
            {
                mgr.Initialize(this.exeFileName);
                string logFileName = mgr.SetupForTrace();
                this.CompareConfig(expectedConfig, logFileName);
                mgr.RestoreOriginalConfiguration();
                this.CompareConfig(initialConfig, string.Empty);
            }
        }

        [TestMethod]
        public void SRMModifyWithOtherConfig()
        {
            string initialConfig = @"<?xml version=""1.0"" encoding=""utf-8""?>
<configuration>
    <MoreStuff1/>
    <system.diagnostics/>
    <MoreStuff2/>
    <system.serviceModel>
        <MoreStuff3/>
        <diagnostics/>
        <MoreStuff4/>
    </system.serviceModel>
    <MoreStuff5/>
</configuration>";
            string expectedConfig = @"<?xml version=""1.0"" encoding=""utf-8""?><configuration>"
                                    +
                                    "<MoreStuff1/>"
                                    +
                                    ExpectedDiagnosticsSectionTemplate
                                    +
                                    "<MoreStuff2/>"
                                    +
                                    @"<system.serviceModel>"
                                    +
                                    "<MoreStuff3/>"
                                    +
                                    this.expectedServiceModelSection
                                    +
                                    "<MoreStuff4/>"
                                    +
                                    @"</system.serviceModel>"
                                    +
                                    "<MoreStuff5/>"
                                    +
                                    @"</configuration>";
            this.CreateConfigFile(initialConfig);
            using (ScenarioRunManager mgr = new ScenarioRunManager())
            {
                mgr.Initialize(this.exeFileName);
                string logFileName = mgr.SetupForTrace();
                this.CompareConfig(expectedConfig, logFileName);
                mgr.RestoreOriginalConfiguration();
                this.CompareConfig(initialConfig, string.Empty);
            }
        }

        [TestMethod]
        public void SRMReferencedAssemblies()
        {
            string tempDir = TestHelper.CopyComplexSampleToTempFolder();
            string path = tempDir + @"\Microsoft.Sample.WCF.Client.exe";
            List<string> assembliesColl = new List<string>();
            ScenarioRunManager.GetReferencedAssemblies(path, assembliesColl);
            Assert.AreEqual<int>(11, assembliesColl.Count);
        }

        [TestMethod]
        public void SRMThreeCyclesThroughShouldAlwaysCleanupCorrectly()
        {
            string initialConfig = @"<?xml version=""1.0"" encoding=""utf-8""?><configuration></configuration>";
            string expectedConfig = @"<?xml version=""1.0"" encoding=""utf-8""?><configuration>"
                                    +
                                    ExpectedDiagnosticsSectionTemplate
                                    +
                                    @"<system.serviceModel>"
                                    +
                                    this.expectedServiceModelSection
                                    +
                                    @"</system.serviceModel>"
                                    +
                                    @"</configuration>";
            this.CreateConfigFile(initialConfig);
            using (ScenarioRunManager mgr = new ScenarioRunManager())
            {
                mgr.Initialize(this.exeFileName);
                string logFileName = mgr.SetupForTrace();
                this.CompareConfig(expectedConfig, logFileName);
                mgr.RestoreOriginalConfiguration();
                this.CompareConfig(initialConfig, string.Empty);

                mgr.Initialize(this.exeFileName);
                logFileName = mgr.SetupForTrace();
                this.CompareConfig(expectedConfig, logFileName);
                mgr.RestoreOriginalConfiguration();
                this.CompareConfig(initialConfig, string.Empty);

                mgr.Initialize(this.exeFileName);
                logFileName = mgr.SetupForTrace();
                this.CompareConfig(expectedConfig, logFileName);
                mgr.RestoreOriginalConfiguration();
                this.CompareConfig(initialConfig, string.Empty);
            }
        }

        private void CreateConfigFile(string xml)
        {
            using (StreamWriter sw = new StreamWriter(this.configFileName))
            {
                sw.Write(xml);
            }
        }

        private void CompareConfig(string expectedConfig, string logFileName)
        {
            XmlDocument expectedDoc = new XmlDocument();
            expectedDoc.LoadXml(expectedConfig.Replace("#LOGFILE#", logFileName));
            XmlDocument actualDoc = new XmlDocument();
            actualDoc.Load(this.configFileName);

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.CloseOutput = true;
            settings.Encoding = Encoding.UTF8;
            StringBuilder expectedString = new StringBuilder();
            StringBuilder actualString = new StringBuilder();
            using (XmlWriter xw = XmlWriter.Create(expectedString, settings))
            {
                expectedDoc.WriteTo(xw);
            }

            using (XmlWriter xw = XmlWriter.Create(actualString, settings))
            {
                actualDoc.WriteTo(xw);
            }

            Assert.AreEqual<int>(expectedString.Length, actualString.Length, string.Format(CultureInfo.InvariantCulture, "Expected string\n<{0}>\n\n\n\nactual string\n<{1}>", expectedString.ToString(), actualString.ToString()));
            for (int i = 0; i < expectedString.Length; i++)
            {
                Assert.AreEqual<char>(expectedString[i], actualString[i], string.Format(CultureInfo.InvariantCulture, "Mismatch at character {0}. Expected string\n<{1}>\n\n\n\nactual string\n<{2}>", i, expectedString.ToString(), actualString.ToString()));
            }
            ////Assert.AreEqual<string>(expectedString.ToString(), actualString.ToString());
        }
    }
}
