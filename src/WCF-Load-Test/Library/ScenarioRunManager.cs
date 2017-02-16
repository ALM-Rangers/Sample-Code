//---------------------------------------------------------------------
// <copyright file="ScenarioRunManager.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
//     THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
//     OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
//     LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR
//     FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
// <summary>The ScenarioRunManager type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.WcfUnit.Library
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Reflection;
    using System.Text;
    using System.Xml;
    using System.Xml.XPath;

    /// <summary>
    /// Sets up the configuration file for capturing a suitable trace, runs the application
    /// under test and then restores the configuration at the end.
    /// </summary>
    public class ScenarioRunManager : IScenarioRunManager
    {
        /// <summary>
        /// Template for the diagnostics section.
        /// </summary>
        private const string DiagnosticsSectionTemplate = @"
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

        /// <summary>
        /// Service model section for the configuration file.
        /// </summary>
        private const string ServiceModelSection = @"<diagnostics>
      <messageLogging logEntireMessage=""true"" logMalformedMessages=""false""
        logMessagesAtServiceLevel=""true"" logMessagesAtTransportLevel=""false"" />
    </diagnostics>";

        /// <summary>
        /// Tracks whether Dispose has been called.
        /// </summary>
        private bool disposed;

        /// <summary>
        /// The name of the configuration file.
        /// </summary>
        private string configFileName;

        /// <summary>
        /// The name of the executable.
        /// </summary>
        private string exeFileName;

        /// <summary>
        /// The name given to the temporarily renamed configuration file.
        /// </summary>
        private string renamedConfigFileName;

        /// <summary>
        /// The state of the application configuration file.
        /// </summary>
        private ConfigFileState configFileState;

        /// <summary>
        /// The name of the trace file.
        /// </summary>
        private string traceFileName;

        /// <summary>
        /// Finalises an instance of the <see cref="ScenarioRunManager"/> class.
        /// </summary>
        ~ScenarioRunManager()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            this.Dispose(false);
        }

        /// <summary>
        /// The state of the application configuration file when setting up.
        /// </summary>
        private enum ConfigFileState
        {
            /// <summary>
            /// The application configuration file state is unknown.
            /// </summary>
            Unknown,

            /// <summary>
            /// The application configuration file was missing.
            /// </summary>
            Missing,

            /// <summary>
            /// The application configuration file existed.
            /// </summary>
            Existed,

            /// <summary>
            /// The application configuration file was moved.
            /// </summary>
            Moved
        }

        /// <summary>
        /// Gets the list of assemblies referenced by <paramref name="assemblyPath"/>.
        /// </summary>
        /// <remarks>
        /// The process of getting the references assemblies is recursive and it protects against
        /// cyclic dependencies by checking the accumulated list before recursing again.
        /// </remarks>
        /// <param name="assemblyPath">The assembly to get the references for.</param>
        /// <param name="referencedAssemblies">The collection to which the references are added.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2001:AvoidCallingProblematicMethods", MessageId = "System.Reflection.Assembly.LoadFile", Justification = "The nature of the program requires this call to be used")]
        public static void GetReferencedAssemblies(string assemblyPath, ICollection<string> referencedAssemblies)
        {
            if (referencedAssemblies == null)
            {
                throw new ArgumentNullException("referencedAssemblies");
            }

            Trace.WriteLine(string.Format(CultureInfo.InvariantCulture, "Getting references for assembly {0}", assemblyPath));
            if (referencedAssemblies.Contains(assemblyPath))
            {
                Trace.WriteLine(string.Format(CultureInfo.InvariantCulture, "This assembly has already been found, and will not be processed again"));
            }
            else
            {
                referencedAssemblies.Add(assemblyPath);
                Assembly a = Assembly.LoadFile(assemblyPath);
                AssemblyName[] assemblies = a.GetReferencedAssemblies();
                foreach (AssemblyName an in assemblies)
                {
                    Trace.Indent();
                    try
                    {
                        string probe;
                        probe = string.Format(CultureInfo.InvariantCulture, "{0}\\{1}.dll", Path.GetDirectoryName(assemblyPath), an.Name);
                        if (File.Exists(probe))
                        {
                            GetReferencedAssemblies(probe, referencedAssemblies);
                        }
                        else
                        {
                            probe = string.Format(CultureInfo.InvariantCulture, "{0}\\{1}.exe", Path.GetDirectoryName(assemblyPath), an.Name);
                            if (File.Exists(probe))
                            {
                                GetReferencedAssemblies(probe, referencedAssemblies);
                            }
                        }
                    }
                    finally
                    {
                        Trace.Unindent();
                    }
                }
            }

            Trace.WriteLine(string.Format(CultureInfo.InvariantCulture, "Finished getting references for assembly {0}", assemblyPath));
        }

        /// <summary>
        /// Initialises the object to work with the given executable.
        /// </summary>
        /// <param name="executableFileName">The executable to be managed.</param>
        public void Initialize(string executableFileName)
        {
            if (!File.Exists(executableFileName))
            {
                throw new UserException(string.Format(CultureInfo.CurrentCulture, Messages.SRMExeNotFound, executableFileName));
            }

            this.exeFileName = executableFileName;
            this.configFileName = executableFileName + ".config";
            this.configFileState = ConfigFileState.Unknown;
        }

        /// <summary>
        /// Disposes the object.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Sets up the executable's configuration file.
        /// </summary>
        /// <returns>The path to the trace file that will be generated.</returns>
        public string SetupForTrace()
        {
            this.traceFileName = Path.GetTempFileName();
            Trace.WriteLine(string.Format(CultureInfo.InvariantCulture, "Trace file will be {0}", this.traceFileName));

            try
            {
                this.SetupForTraceImpl();
            }
            catch (Exception)
            {
                Trace.WriteLine(string.Format(CultureInfo.InvariantCulture, "Failed to set up for trace, deleting {0}", this.traceFileName));
                File.Delete(this.traceFileName);
                throw;
            }

            return this.traceFileName;
        }

        /// <summary>
        /// Restores the executable's configuration file to its original state.
        /// </summary>
        public void RestoreOriginalConfiguration()
        {
            Trace.WriteLine(string.Format(CultureInfo.InvariantCulture, "Restoring original application configuration"));
            if (this.configFileState == ConfigFileState.Missing || this.configFileState == ConfigFileState.Moved)
            {
                File.Delete(this.configFileName);
            }

            if (this.configFileState == ConfigFileState.Moved)
            {
                File.Move(this.renamedConfigFileName, this.configFileName);
            }

            this.configFileState = ConfigFileState.Unknown; // so it is safe to do a Dispose even if restore has been called explicitly
            Trace.WriteLine(string.Format(CultureInfo.InvariantCulture, "Finished restoring original application configuration"));
        }

        /// <summary>
        /// Runs the program and gets the configuration information.
        /// </summary>
        /// <remarks>
        /// The assemblies found to add to the list to look for the proxy are limited to non-GAC assemblies.
        /// </remarks>
        /// <param name="configuration">The configuration that is to be updated with information from the trace.</param>
        public void Run(WcfUnitConfiguration configuration)
        {
            ////AppDomainSetup testAppSetup = new AppDomainSetup();
            ////testAppSetup.ApplicationBase = Path.GetDirectoryName(this._exeFileName);
            ////testAppSetup.ConfigurationFile = this._configFileName;
            ////testAppSetup.DisallowBindingRedirects = false;
            ////testAppSetup.DisallowCodeDownload = true;
            ////testAppSetup.LoaderOptimization = LoaderOptimization.MultiDomainHost;
            ////AppDomain testApp = AppDomain.CreateDomain("App Under Test", null, testAppSetup);
            ////try
            ////{
            ////    testApp.AssemblyLoad += new AssemblyLoadEventHandler(testApp_AssemblyLoad);
            ////    testApp.ExecuteAssembly(this._exeFileName);
            ////    Assembly[] assemblies = testApp.GetAssemblies();
            ////    config.assembly = new assemblyType[assemblies.Length];
            ////    for(int i=0; i<assemblies.Length; i++)
            ////    {
            ////        config.assembly[i] = new assemblyType();
            ////        config.assembly[i].fileName = assemblies[i].CodeBase;
            ////    }

            using (Process p = new Process())
            {
                p.StartInfo.FileName = this.exeFileName;
                p.StartInfo.CreateNoWindow = false;
                p.StartInfo.ErrorDialog = true;
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardError = true;
                Trace.WriteLine("Starting application under test");
                if (p.Start())
                {
                    Trace.WriteLine("Application under test started");
                    string error = p.StandardError.ReadToEnd();
                    Trace.WriteLine("Waiting for application under test to end");
                    p.WaitForExit();
                    Trace.WriteLine("Application under test has ended");
                    if (!string.IsNullOrEmpty(error))
                    {
                        throw new UserException(error);
                    }

                    UILogic uiLogic = new UILogic();
                    uiLogic.ParseTraceFile(this.traceFileName, configuration);
                }
                else
                {
                    Trace.WriteLine("Application under test failed to start");
                }
            }

            Trace.WriteLine("Finished running application under test");
        }

        /// <summary>
        /// Disposes the object.
        /// </summary>
        /// <param name="disposing">Indicates whether the dispose is automatic or not.</param>
        private void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!this.disposed)
            {
                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if (disposing)
                {
                    this.RestoreOriginalConfiguration();
                }

                // Note disposing has been done.
                this.disposed = true;
            }
        }

        /// <summary>
        /// Kept separate so get good stack trace in case of an exception
        /// </summary>
        private void SetupForTraceImpl()
        {
            this.configFileState = File.Exists(this.configFileName) ? ConfigFileState.Existed : ConfigFileState.Missing;

            XmlDocument config = new XmlDocument();

            if (this.configFileState == ConfigFileState.Existed)
            {
                config.Load(this.configFileName);
            }

            XPathNavigator configNav = config.CreateNavigator();

            // Add <configuration> element if not already present
            XPathNavigator configurationElementNav = configNav.SelectSingleNode("/configuration");
            if (configurationElementNav == null)
            {
                configNav.AppendChildElement(null, "configuration", null, null);
                configurationElementNav = configNav.SelectSingleNode("/configuration");
            }

            // Add system.diagnostics section, replace any existing section
            XPathNavigator systemDiagnosticsElementNav = configNav.SelectSingleNode("/configuration/system.diagnostics");
            if (systemDiagnosticsElementNav != null)
            {
                systemDiagnosticsElementNav.ReplaceSelf(DiagnosticsSectionTemplate.Replace("#LOGFILE#", this.traceFileName));
            }
            else
            {
                configurationElementNav.PrependChild(DiagnosticsSectionTemplate.Replace("#LOGFILE#", this.traceFileName));
            }

            // Add <system.serviceModel> element if not already present
            XPathNavigator serviceModelNav = configNav.SelectSingleNode("/configuration/system.serviceModel");
            if (serviceModelNav == null)
            {
                configurationElementNav.AppendChildElement(null, "system.serviceModel", null, null);
                serviceModelNav = configNav.SelectSingleNode("/configuration/system.serviceModel");
            }

            // Add service model diagnostics section, replace any existing section
            XPathNavigator serviceModelDiagnosticsElementNav = configNav.SelectSingleNode("/configuration/system.serviceModel/diagnostics");
            if (serviceModelDiagnosticsElementNav != null)
            {
                serviceModelDiagnosticsElementNav.ReplaceSelf(ServiceModelSection);
            }
            else
            {
                serviceModelNav.PrependChild(ServiceModelSection);
            }

            // Write the new config file
            try
            {
                if (this.configFileState == ConfigFileState.Existed)
                {
                    this.renamedConfigFileName = this.configFileName + "._wcfunit";
                    File.Move(this.configFileName, this.renamedConfigFileName);
                    this.configFileState = ConfigFileState.Moved;
                    Trace.WriteLine(string.Format(CultureInfo.InvariantCulture, "Renamed application config file for the program under test to {0}", this.renamedConfigFileName));
                }

                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Indent = true;
                settings.CloseOutput = true;
                settings.Encoding = Encoding.UTF8;
                using (XmlWriter xw = XmlWriter.Create(this.configFileName, settings))
                {
                    config.WriteTo(xw);
                }
            }
            catch (UnauthorizedAccessException uae)
            {
                if (this.configFileState == ConfigFileState.Moved)
                {
                    File.Move(this.renamedConfigFileName, this.configFileName);
                    this.configFileState = ConfigFileState.Unknown;
                }

                throw new UserException(Messages.SRMAccessDenied, uae);
            }
            catch (Exception)
            {
                // Make sure we put things back if we can whatever happens
                if (this.configFileState == ConfigFileState.Moved)
                {
                    File.Move(this.renamedConfigFileName, this.configFileName);
                    this.configFileState = ConfigFileState.Unknown;
                }

                throw;
            }
        }
    }
}
