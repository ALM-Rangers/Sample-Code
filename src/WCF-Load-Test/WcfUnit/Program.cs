//---------------------------------------------------------------------
// <copyright file="Program.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
//     THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
//     OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
//     LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR
//     FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
// <summary>The main program.</summary>
//---------------------------------------------------------------------

namespace Microsoft.WcfUnit
{
    using System;
    using System.IO;
    using Microsoft.WcfUnit.Library;

    /// <summary>
    /// The main program.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main entry point.
        /// </summary>
        /// <param name="args">Command line parameters.</param>
        public static void Main(string[] args)
        {
            Console.WriteLine(Resources.ProgramTitle, Utility.ReadVersion());
            if (args == null || (args.Length != 3 && args.Length != 4))
            {
                Console.WriteLine(Resources.ProgramUsage);
                Console.WriteLine(Resources.ProgramUsageExample);
            }
            else
            {
                string scenarioName = args[0];
                string traceFile = args[1];
                string configFile = args[2];
                string timingsFile = (args.Length == 4) ? args[3] : null;

                Stream timings = null;

                try
                {
                    if (timingsFile != null)
                    {
                        if (File.Exists(timingsFile))
                        {
                            try
                            {
                                timings = new FileStream(timingsFile, FileMode.Open, FileAccess.Read, FileShare.Read);
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.Message);
                            }
                        }
                        else
                        {
                            Console.WriteLine(Resources.TimingsFileDoesNotExist, timingsFile);
                        }
                    }

                    if (Utility.IsValidIdentifier(scenarioName))
                    {
                        try
                        {
                            WcfUnitConfiguration config = ConfigurationReader.Read(configFile);
                            TraceFileProcessor tfp = new TraceFileProcessor();
                            tfp.ProcessTraceFile(scenarioName, traceFile, timings, config);
                        }
                        catch (UserException ue)
                        {
                            Console.WriteLine(ue.Message);
                            Exception e = ue.InnerException;
                            while (e != null)
                            {
                                Console.WriteLine(e.Message);
                                e = e.InnerException;
                            }

                            Environment.Exit(1);
                        }
                    }
                    else
                    {
                        Console.WriteLine(Resources.InvalidScenarioName);
                        Environment.Exit(1);
                    }
                }
                finally
                {
                    if (timings != null)
                    {
                        timings.Dispose();
                    }
                }
            }
        }
    }
}
