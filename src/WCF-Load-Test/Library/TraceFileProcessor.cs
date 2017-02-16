//---------------------------------------------------------------------
// <copyright file="TraceFileProcessor.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
//     THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
//     OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
//     LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR
//     FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
// <summary>The TraceFileProcessor type.</summary>
//---------------------------------------------------------------------

// TODO: Add think times to the scenarios as an option.
// TODO: Other channel shapes (duplex in particular)
// TODO: Add random data generation/injection to the wizard
// TODO: Pass result objects to the stubs as well
// TODO: Start/stop capture when using the wizard (to allow sub-scenarios to be handled)
namespace Microsoft.WcfUnit.Library
{
    using System;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Reflection;

    /// <summary>
    /// Processes trace files and produces the generated code.
    /// </summary>
    public class TraceFileProcessor : MarshalByRefObject
    {
        /// <summary>
        /// The list of proxy assemblies required.
        /// </summary>
        private Collection<Assembly> proxyAssemblies;

        /// <summary>
        /// Processes a trace file and generates the test code. Executes in a separate app domain so that assemblies are not locked after the test.
        /// </summary>
        /// <param name="scenarioName">The name to be given to the scenario, used for namespace and class names.</param>
        /// <param name="traceFileName">The file containing the trace to be processed.</param>
        /// <param name="timedCommentsFile">The stream representing the file of timed comments, <c>null</c> if there is no timed comments file.</param>
        /// <param name="configuration">The configuration guiding the generation process.</param>
        /// <param name="mainFileName">The name of the main unit test file to be generated.</param>
        /// <param name="stubFileName">The name of the file to be generated containing the stubs.</param>
        /// <returns>Collection of paths to assemblies containing the proxies.</returns>
        public static Collection<string> ProcessTraceFileInAppDomain(string scenarioName, string traceFileName, Stream timedCommentsFile, WcfUnitConfiguration configuration, string mainFileName, string stubFileName)
        {
            Collection<string> ans = null;
            AppDomainSetup ads = new AppDomainSetup();
            ads.ApplicationBase = AppDomain.CurrentDomain.BaseDirectory;
            ads.DisallowBindingRedirects = false;
            ads.DisallowCodeDownload = true;
            ads.ConfigurationFile = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;
            ads.LoaderOptimization = LoaderOptimization.SingleDomain;
            AppDomain processingAppDomain = AppDomain.CreateDomain("Processing App Domain", null, ads);
            try
            {
                TraceFileProcessor tfp = (TraceFileProcessor)processingAppDomain.CreateInstanceAndUnwrap(typeof(TraceFileProcessor).Assembly.FullName, typeof(TraceFileProcessor).FullName);
                ans = tfp.ProcessTraceFile(scenarioName, traceFileName, timedCommentsFile, configuration, mainFileName, stubFileName);
            }
            finally
            {
                AppDomain.Unload(processingAppDomain);
            }

            return ans;
        }

        /// <summary>
        /// Processes a trace file and produces code for the scenario in the trace file.
        /// </summary>
        /// <param name="scenarioName">The name to be given to the scenario, the file names are generated from this.</param>
        /// <param name="traceFile">The file containing the trace to be processed.</param>
        /// <param name="timedCommentsFile">The stream representing the file of timed comments, <c>null</c> if there is no timed comments file.</param>
        /// <param name="configuration">The configuration guiding the generation process.</param>
        /// <returns>Collection of paths to assemblies containing the proxies.</returns>
        public Collection<string> ProcessTraceFile(string scenarioName, string traceFile, Stream timedCommentsFile, WcfUnitConfiguration configuration)
        {
            return this.ProcessTraceFile(scenarioName, traceFile, timedCommentsFile, configuration, scenarioName + ".cs", scenarioName + ".stubs");
        }

        /// <summary>
        /// Processes a trace file and generates the test code.
        /// </summary>
        /// <param name="scenarioName">The name to be given to the scenario, used for namespace and class names.</param>
        /// <param name="traceFileName">The file containing the trace to be processed.</param>
        /// <param name="timedCommentsFile">The stream representing the file of timed comments, <c>null</c> if there is no timed comments file.</param>
        /// <param name="configuration">The configuration guiding the generation process.</param>
        /// <param name="mainFileName">The name of the main unit test file to be generated.</param>
        /// <param name="stubFileName">The name of the file to be generated containing the stubs.</param>
        /// <returns>Collection of paths to assemblies containing the proxies.</returns>
        public Collection<string> ProcessTraceFile(string scenarioName, string traceFileName, Stream timedCommentsFile, WcfUnitConfiguration configuration, string mainFileName, string stubFileName)
        {
            Trace.WriteLine(string.Format(CultureInfo.InvariantCulture, "Starting to process the trace file"));
            if (configuration == null)
            {
                throw new ArgumentNullException("configuration");
            }

            this.proxyAssemblies = new Collection<Assembly>();
            string[] assemblies = new string[configuration.assembly.Length];
            for (int i = 0; i < configuration.assembly.Length; i++)
            {
                assemblies[i] = configuration.assembly[i].fileName;
            }

            string[] actions = null;
            if (configuration.soapActions.soapAction != null)
            {
                actions = new string[configuration.soapActions.soapAction.Length];
                for (int i = 0; i < configuration.soapActions.soapAction.Length; i++)
                {
                    actions[i] = configuration.soapActions.soapAction[i].action;
                }
            }

            TimingsFile timings = null;
            if (timedCommentsFile != null)
            {
                timings = new TimingsFile(timedCommentsFile);
            }

            ProxyManager pm = new ProxyManager(assemblies);
            pm.TypeResolved += new EventHandler<ProxyAssemblyEventArgs>(this.TypeResolvedHandler);
            UnitTestGenerator tg = new UnitTestGenerator(scenarioName, scenarioName, scenarioName + "Tests", configuration.testMethodMode, configuration.operationTimerMode);

            Stream traceFile = null;
            try
            {
                traceFile = Parser.OpenTraceFile(traceFileName);
                int count = 0;
                using (Parser p = Parser.CreateParser(configuration.parser, traceFile, traceFileName, configuration.clientTrace, configuration.serviceTrace, configuration.soapActions.soapActionMode, actions))
                {
                    traceFile = null;
                    Deserializer d = new Deserializer();
                    ParsedMessage parsedMessage;
                    TimedComment timedComment = null;
                    DateTime lastMessageTimeStamp = DateTime.MinValue;

                    if (timings != null)
                    {
                        timedComment = timings.NextTimedComment();
                    }

                    while ((parsedMessage = p.ReadNextRequest()) != null)
                    {
                        string comment = parsedMessage.Timestamp.ToString(CultureInfo.CurrentCulture);

                        if (timings != null)
                        {
                            // loop in case more than one timed comment between the same two messages, must get last one.
                            while (timedComment != null && timedComment.Timestamp > lastMessageTimeStamp && timedComment.Timestamp <= parsedMessage.Timestamp.ToUniversalTime())
                            {
                                comment = string.Format(CultureInfo.CurrentCulture, "{0} {1}", timedComment.Timestamp, timedComment.Comment);
                                timedComment = timings.NextTimedComment();
                            }
                        }

                        lastMessageTimeStamp = parsedMessage.Timestamp.ToUniversalTime();

                        MethodInfo contractMethod = pm.GetContractMethod(parsedMessage.SoapAction);
                        if (contractMethod == null)
                        {
                            Console.WriteLine(string.Format(CultureInfo.CurrentCulture, Messages.Processor_MethodNotFound, parsedMessage.SoapAction));
                            continue;
                        }

                        CallParameterInfo[] parameters = d.DeserializeInputParameters(parsedMessage.Message, contractMethod);
                        MethodInfo proxyMethod = pm.GetProxyMethod(contractMethod);
                        tg.GenerateServiceCall(proxyMethod, contractMethod, parameters, comment);
                        count++;
                    }
                }

                if (count <= 0)
                {
                    throw new UserException(Messages.NoDataInTraceFile);
                }

                tg.WriteCode(mainFileName, stubFileName);
            }
            finally
            {
                if (traceFile != null)
                {
                    traceFile.Dispose();
                }
            }

            Collection<string> ans = new Collection<string>();
            foreach (Assembly a in this.proxyAssemblies)
            {
                ans.Add(a.Location);
            }

            return ans;
        }

        /// <summary>
        /// Handles the event when a type is resolved. Used to add the the assembly to the list of assemblies needed for the proxies.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        private void TypeResolvedHandler(object sender, ProxyAssemblyEventArgs e)
        {
            Trace.WriteLine(string.Format(CultureInfo.InvariantCulture, "Resolved a proxy type from the assembly {0}", e.Assembly.FullName));
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
    }
}
