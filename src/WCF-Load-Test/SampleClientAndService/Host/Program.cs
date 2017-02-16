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

namespace Host
{
    using System;
    using System.ServiceModel;
    using System.ServiceModel.Description;
    using System.ServiceModel.Dispatcher;

    using Logic;

    /// <summary>
    /// Hosts test services.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// The main method.
        /// </summary>
        /// <param name="args">Command line arguments.</param>
        private static void Main(string[] args)
        {
            using (ServiceHost arithmeticHost = new ServiceHost(typeof(Arithmetic), new Uri("http://localhost:8081/arithmetic")))
            {
                arithmeticHost.Open();
                PrintWCFServiceDescription(arithmeticHost);
                using (ServiceHost dataSetHost = new ServiceHost(typeof(DataSets), new Uri("http://localhost:8081/datasets")))
                {
                    dataSetHost.Open();
                    PrintWCFServiceDescription(dataSetHost);
                    using (ServiceHost customHost = new ServiceHost(typeof(Custom), new Uri("http://localhost:8081/custom")))
                    {
                        customHost.Open();
                        PrintWCFServiceDescription(customHost);
                        Console.WriteLine("Press Enter to terminate...");
                        Console.ReadLine();
                        customHost.Close();
                    }

                    dataSetHost.Close();
                }

                arithmeticHost.Close();
            }
        }

        /// <summary>
        /// Prints the ServiceDescription details to the Console window.
        /// </summary>
        /// <param name="host">The service host to describe.</param>
        private static void PrintWCFServiceDescription(ServiceHost host)
        {
            Console.WriteLine(
                "{0} is running with the following WCF endpoints/listeners...\n",
                host.Description.ServiceType.Name);

            Console.WriteLine("Endpoints");
            Console.WriteLine("*********");
            foreach (ServiceEndpoint se in host.Description.Endpoints)
            {
                Console.WriteLine("Endpoint:");
                Console.WriteLine("name: {0}", se.Name);
                Console.WriteLine("address: {0}", se.Address);
                Console.WriteLine("binding: {0}", se.Binding.Name);
                Console.WriteLine("contract: {0}\n", se.Contract.Name);
            }

            Console.WriteLine("Listeners");
            Console.WriteLine("*********");
            foreach (ChannelDispatcher dispatcher in host.ChannelDispatchers)
            {
                Console.WriteLine("Listener: {0}", dispatcher.Listener.Uri.AbsoluteUri);
            }

            Console.WriteLine();
        }
    }
}
