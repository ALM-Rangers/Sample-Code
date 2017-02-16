//---------------------------------------------------------------------
// <copyright file="TestServiceHosting.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
//     THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
//     OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
//     LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR
//     FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
// <summary>The TestServiceHosting type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.WcfUnit.Library.Test
{
    using System;
    using System.ServiceModel;

    using Logic;
    using TestContracts = Contracts;

    /// <summary>
    /// Provides self hosting for test services.
    /// </summary>
    internal static class TestServiceHosting
    {
        /// <summary>
        /// First service host.
        /// </summary>
        private static ServiceHost host1;

        /// <summary>
        /// Second service host.
        /// </summary>
        private static ServiceHost host2;

        /// <summary>
        /// Third service host.
        /// </summary>
        private static ServiceHost host3;

        /// <summary>
        /// Starts the self hosting.
        /// </summary>
        public static void StartHosts()
        {
            Console.WriteLine("Starting self-hosted services");
            for (int i = 0; i < 3; i++)
            {
                try
                {
                    BasicHttpBinding binding = new BasicHttpBinding();
                    BasicHttpBinding bufferedBinding = new BasicHttpBinding();
                    bufferedBinding.TransferMode = TransferMode.Buffered;
                    BasicHttpBinding streamedBinding = new BasicHttpBinding();
                    streamedBinding.TransferMode = TransferMode.Streamed;
                    WSDualHttpBinding duplexBinding = new WSDualHttpBinding();

                    host1 = new ServiceHost(typeof(Arithmetic), new Uri("http://localhost:8081/arithmetic"));

                    ServiceBehaviorAttribute serviceBehaviour = host1.Description.Behaviors.Find<ServiceBehaviorAttribute>();
                    serviceBehaviour.IncludeExceptionDetailInFaults = true;

                    host1.AddServiceEndpoint(typeof(TestContracts.IArithmetic), binding, "basic");
                    host1.AddServiceEndpoint(typeof(TestContracts.ICollections), binding, "collections");
                    host1.AddServiceEndpoint(typeof(TestContracts.IBufferedStreamService), bufferedBinding, "bufferedStreams");
                    host1.AddServiceEndpoint(typeof(TestContracts.IStreamedStreamService), streamedBinding, "streamedStreams");
                    host1.AddServiceEndpoint(typeof(TestContracts.IShapeService), binding, "shape");
                    host1.AddServiceEndpoint(typeof(TestContracts.IServiceKnownType), binding, "serviceKnownType");
                    host1.AddServiceEndpoint(typeof(TestContracts.ISharePrices), duplexBinding, "sharePrices");

                    host1.Open();

                    host2 = new ServiceHost(typeof(Custom), new Uri("http://localhost:8081/custom"));

                    ServiceBehaviorAttribute serviceBehaviour2 = host2.Description.Behaviors.Find<ServiceBehaviorAttribute>();
                    serviceBehaviour2.IncludeExceptionDetailInFaults = true;

                    host2.AddServiceEndpoint(typeof(TestContracts.Custom.ICustomContracts), binding, "custom");
                    host2.AddServiceEndpoint(typeof(TestContracts.Custom.ICustomContracts2), binding, "custom2");

                    host2.Open();

                    host3 = new ServiceHost(typeof(DataSets), new Uri("http://localhost:8081/datasets"));

                    ServiceBehaviorAttribute serviceBehaviour3 = host3.Description.Behaviors.Find<ServiceBehaviorAttribute>();
                    serviceBehaviour3.IncludeExceptionDetailInFaults = true;

                    host3.AddServiceEndpoint(typeof(TestContracts.IDataSets), binding, "DataSets");

                    host3.Open();
                    
                    Console.WriteLine("Started self-hosted service");
                    break;
                }
                catch (InvalidOperationException)
                {
                    if (i == 2)
                    {
                        throw;
                    }

                    Console.WriteLine("Waiting 5 seconds and then retrying start...");
                    System.Threading.Thread.Sleep(5000);
                }
            }

            PrintWCFServiceDescription(host1);
        }

        /// <summary>
        /// Stops the self hosting.
        /// </summary>
        public static void StopHost()
        {
            Console.WriteLine("Closing self-hosted services");
            host1.Close();
            host2.Close();
            host3.Close();
            Console.WriteLine("Closed self-hosted services");
        }

        /// <summary>
        /// Prints a description of the service and its endpoints.
        /// </summary>
        /// <param name="host">The service host to describe.</param>
        private static void PrintWCFServiceDescription(ServiceHost host)
        {
            Console.WriteLine(
                "{0} is running with the following WCF endpoints/listeners...\n",
                host.Description.ServiceType.Name);

            Console.WriteLine("Endpoints");
            Console.WriteLine("*********");
            foreach (System.ServiceModel.Description.ServiceEndpoint se in host.Description.Endpoints)
            {
                Console.WriteLine("Endpoint:");
                Console.WriteLine("name: {0}", se.Name);
                Console.WriteLine("address: {0}", se.Address);
                Console.WriteLine("binding: {0}", se.Binding.Name);
                Console.WriteLine("contract: {0}\n", se.Contract.Name);
            }

            Console.WriteLine("Listeners");
            Console.WriteLine("*********");
            foreach (System.ServiceModel.Dispatcher.ChannelDispatcher dispatcher in host.ChannelDispatchers)
            {
                Console.WriteLine("Listener: {0}", dispatcher.Listener.Uri.AbsoluteUri);
            }

            Console.WriteLine(string.Empty);
        }
    }
}
