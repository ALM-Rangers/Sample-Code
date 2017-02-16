//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.1433
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace GeneratedSampleTestWithProxyFromServerTrace
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Collections.Generic;
    using System.IO;
    using Library.Test.Integration;
    using System.ServiceModel;


    public partial class GeneratedSampleTestWithProxyFromServerTraceTests
    {

        private static Dictionary<int, Contracts.IArithmetic> arithmeticProxyTable = new Dictionary<int, Contracts.IArithmetic>();

        private static Dictionary<int, Contracts.ICollections> collectionsProxyTable = new Dictionary<int, Contracts.ICollections>();

        private static Dictionary<int, Contracts.Custom.ICustomContracts> customContractsProxyTable = new Dictionary<int, Contracts.Custom.ICustomContracts>();

        private static Dictionary<int, Contracts.Custom.ICustomContracts2> customContracts2ProxyTable = new Dictionary<int, Contracts.Custom.ICustomContracts2>();

        private static Dictionary<int, Contracts.IBufferedStreamService> bufferedStreamServiceProxyTable = new Dictionary<int, Contracts.IBufferedStreamService>();

        private static Dictionary<int, Contracts.IStreamedStreamService> streamedStreamServiceProxyTable = new Dictionary<int, Contracts.IStreamedStreamService>();

        private static Dictionary<int, Contracts.IShapeService> shapeServiceProxyTable = new Dictionary<int, Contracts.IShapeService>();

        private static Dictionary<int, Contracts.IServiceKnownType> serviceKnownTypeProxyTable = new Dictionary<int, Contracts.IServiceKnownType>();

        private static Dictionary<int, Contracts.ISharePrices> sharePricesProxyTable = new Dictionary<int, Contracts.ISharePrices>();

        private static Dictionary<int, Contracts.IDataSets> dataSetsProxyTable = new Dictionary<int, Contracts.IDataSets>();

        [TestInitialize()]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "proxy should not be disposed")]
        public void InitializeTest()
        {
            System.Threading.Monitor.Enter(arithmeticProxyTable);
            try
            {
                arithmeticProxyTable.TryGetValue(System.Threading.Thread.CurrentThread.ManagedThreadId, out arithmeticClient);
                if (((arithmeticClient == null)
                            || (((System.ServiceModel.ICommunicationObject)(arithmeticClient)).State == System.ServiceModel.CommunicationState.Faulted)))
                {
                    // The following line may need to be customised to select the appropriate binding from the configuration file
                    System.ServiceModel.ChannelFactory<Contracts.IArithmetic> arithmeticFactory = new System.ServiceModel.ChannelFactory<Contracts.IArithmetic>("Arithmetic");
                    arithmeticClient = arithmeticFactory.CreateChannel();
                    ((System.ServiceModel.ICommunicationObject)(arithmeticClient)).Open();
                    arithmeticProxyTable[System.Threading.Thread.CurrentThread.ManagedThreadId] = arithmeticClient;
                }
            }
            finally
            {
                System.Threading.Monitor.Exit(arithmeticProxyTable);
            }
            System.Threading.Monitor.Enter(collectionsProxyTable);
            try
            {
                collectionsProxyTable.TryGetValue(System.Threading.Thread.CurrentThread.ManagedThreadId, out collectionsClient);
                if (((collectionsClient == null)
                            || (((System.ServiceModel.ICommunicationObject)(collectionsClient)).State == System.ServiceModel.CommunicationState.Faulted)))
                {
                    // The following line may need to be customised to select the appropriate binding from the configuration file
                    System.ServiceModel.ChannelFactory<Contracts.ICollections> collectionsFactory = new System.ServiceModel.ChannelFactory<Contracts.ICollections>("Collections");
                    collectionsClient = collectionsFactory.CreateChannel();
                    ((System.ServiceModel.ICommunicationObject)(collectionsClient)).Open();
                    collectionsProxyTable[System.Threading.Thread.CurrentThread.ManagedThreadId] = collectionsClient;
                }
            }
            finally
            {
                System.Threading.Monitor.Exit(collectionsProxyTable);
            }
            System.Threading.Monitor.Enter(customContractsProxyTable);
            try
            {
                customContractsProxyTable.TryGetValue(System.Threading.Thread.CurrentThread.ManagedThreadId, out customContractsClient);
                if (((customContractsClient == null)
                            || (((System.ServiceModel.ICommunicationObject)(customContractsClient)).State == System.ServiceModel.CommunicationState.Faulted)))
                {
                    // The following line may need to be customised to select the appropriate binding from the configuration file
                    System.ServiceModel.ChannelFactory<Contracts.Custom.ICustomContracts> customContractsFactory = new System.ServiceModel.ChannelFactory<Contracts.Custom.ICustomContracts>("Custom");
                    customContractsClient = customContractsFactory.CreateChannel();
                    ((System.ServiceModel.ICommunicationObject)(customContractsClient)).Open();
                    customContractsProxyTable[System.Threading.Thread.CurrentThread.ManagedThreadId] = customContractsClient;
                }
            }
            finally
            {
                System.Threading.Monitor.Exit(customContractsProxyTable);
            }
            System.Threading.Monitor.Enter(customContracts2ProxyTable);
            try
            {
                customContracts2ProxyTable.TryGetValue(System.Threading.Thread.CurrentThread.ManagedThreadId, out customContracts2Client);
                if (((customContracts2Client == null)
                            || (((System.ServiceModel.ICommunicationObject)(customContracts2Client)).State == System.ServiceModel.CommunicationState.Faulted)))
                {
                    // The following line may need to be customised to select the appropriate binding from the configuration file
                    System.ServiceModel.ChannelFactory<Contracts.Custom.ICustomContracts2> customContracts2Factory = new System.ServiceModel.ChannelFactory<Contracts.Custom.ICustomContracts2>("Custom2");
                    customContracts2Client = customContracts2Factory.CreateChannel();
                    ((System.ServiceModel.ICommunicationObject)(customContracts2Client)).Open();
                    customContracts2ProxyTable[System.Threading.Thread.CurrentThread.ManagedThreadId] = customContracts2Client;
                }
            }
            finally
            {
                System.Threading.Monitor.Exit(customContracts2ProxyTable);
            }
            System.Threading.Monitor.Enter(bufferedStreamServiceProxyTable);
            try
            {
                bufferedStreamServiceProxyTable.TryGetValue(System.Threading.Thread.CurrentThread.ManagedThreadId, out bufferedStreamServiceClient);
                if (((bufferedStreamServiceClient == null)
                            || (((System.ServiceModel.ICommunicationObject)(bufferedStreamServiceClient)).State == System.ServiceModel.CommunicationState.Faulted)))
                {
                    // The following line may need to be customised to select the appropriate binding from the configuration file
                    System.ServiceModel.ChannelFactory<Contracts.IBufferedStreamService> bufferedStreamServiceFactory = new System.ServiceModel.ChannelFactory<Contracts.IBufferedStreamService>("BufferedStreams");
                    bufferedStreamServiceClient = bufferedStreamServiceFactory.CreateChannel();
                    ((System.ServiceModel.ICommunicationObject)(bufferedStreamServiceClient)).Open();
                    bufferedStreamServiceProxyTable[System.Threading.Thread.CurrentThread.ManagedThreadId] = bufferedStreamServiceClient;
                }
            }
            finally
            {
                System.Threading.Monitor.Exit(bufferedStreamServiceProxyTable);
            }
            System.Threading.Monitor.Enter(streamedStreamServiceProxyTable);
            try
            {
                streamedStreamServiceProxyTable.TryGetValue(System.Threading.Thread.CurrentThread.ManagedThreadId, out streamedStreamServiceClient);
                if (((streamedStreamServiceClient == null)
                            || (((System.ServiceModel.ICommunicationObject)(streamedStreamServiceClient)).State == System.ServiceModel.CommunicationState.Faulted)))
                {
                    // The following line may need to be customised to select the appropriate binding from the configuration file
                    System.ServiceModel.ChannelFactory<Contracts.IStreamedStreamService> streamedStreamServiceFactory = new System.ServiceModel.ChannelFactory<Contracts.IStreamedStreamService>("StreamedStreams");
                    streamedStreamServiceClient = streamedStreamServiceFactory.CreateChannel();
                    ((System.ServiceModel.ICommunicationObject)(streamedStreamServiceClient)).Open();
                    streamedStreamServiceProxyTable[System.Threading.Thread.CurrentThread.ManagedThreadId] = streamedStreamServiceClient;
                }
            }
            finally
            {
                System.Threading.Monitor.Exit(streamedStreamServiceProxyTable);
            }
            System.Threading.Monitor.Enter(shapeServiceProxyTable);
            try
            {
                shapeServiceProxyTable.TryGetValue(System.Threading.Thread.CurrentThread.ManagedThreadId, out shapeServiceClient);
                if (((shapeServiceClient == null)
                            || (((System.ServiceModel.ICommunicationObject)(shapeServiceClient)).State == System.ServiceModel.CommunicationState.Faulted)))
                {
                    // The following line may need to be customised to select the appropriate binding from the configuration file
                    System.ServiceModel.ChannelFactory<Contracts.IShapeService> shapeServiceFactory = new System.ServiceModel.ChannelFactory<Contracts.IShapeService>("Shape");
                    shapeServiceClient = shapeServiceFactory.CreateChannel();
                    ((System.ServiceModel.ICommunicationObject)(shapeServiceClient)).Open();
                    shapeServiceProxyTable[System.Threading.Thread.CurrentThread.ManagedThreadId] = shapeServiceClient;
                }
            }
            finally
            {
                System.Threading.Monitor.Exit(shapeServiceProxyTable);
            }
            System.Threading.Monitor.Enter(serviceKnownTypeProxyTable);
            try
            {
                serviceKnownTypeProxyTable.TryGetValue(System.Threading.Thread.CurrentThread.ManagedThreadId, out serviceKnownTypeClient);
                if (((serviceKnownTypeClient == null)
                            || (((System.ServiceModel.ICommunicationObject)(serviceKnownTypeClient)).State == System.ServiceModel.CommunicationState.Faulted)))
                {
                    // The following line may need to be customised to select the appropriate binding from the configuration file
                    System.ServiceModel.ChannelFactory<Contracts.IServiceKnownType> serviceKnownTypeFactory = new System.ServiceModel.ChannelFactory<Contracts.IServiceKnownType>("ServiceKnownType");
                    serviceKnownTypeClient = serviceKnownTypeFactory.CreateChannel();
                    ((System.ServiceModel.ICommunicationObject)(serviceKnownTypeClient)).Open();
                    serviceKnownTypeProxyTable[System.Threading.Thread.CurrentThread.ManagedThreadId] = serviceKnownTypeClient;
                }
            }
            finally
            {
                System.Threading.Monitor.Exit(serviceKnownTypeProxyTable);
            }
            System.Threading.Monitor.Enter(sharePricesProxyTable);
            try
            {
                sharePricesProxyTable.TryGetValue(System.Threading.Thread.CurrentThread.ManagedThreadId, out sharePricesClient);
                if (((sharePricesClient == null)
                            || (((System.ServiceModel.ICommunicationObject)(sharePricesClient)).State == System.ServiceModel.CommunicationState.Faulted)))
                {
                    // The following line may need to be customised to select the appropriate binding from the configuration file
                    System.ServiceModel.DuplexChannelFactory<Contracts.ISharePrices> sharePricesFactory = new System.ServiceModel.DuplexChannelFactory<Contracts.ISharePrices>(new InstanceContext(new SharePricesCallback()),"SharePrices");
                    sharePricesClient = sharePricesFactory.CreateChannel();
                    ((System.ServiceModel.ICommunicationObject)(sharePricesClient)).Open();
                    sharePricesProxyTable[System.Threading.Thread.CurrentThread.ManagedThreadId] = sharePricesClient;
                }
            }
            finally
            {
                System.Threading.Monitor.Exit(sharePricesProxyTable);
            }
            System.Threading.Monitor.Enter(dataSetsProxyTable);
            try
            {
                dataSetsProxyTable.TryGetValue(System.Threading.Thread.CurrentThread.ManagedThreadId, out dataSetsClient);
                if (((dataSetsClient == null)
                            || (((System.ServiceModel.ICommunicationObject)(dataSetsClient)).State == System.ServiceModel.CommunicationState.Faulted)))
                {
                    // The following line may need to be customised to select the appropriate binding from the configuration file
                    System.ServiceModel.ChannelFactory<Contracts.IDataSets> dataSetsFactory = new System.ServiceModel.ChannelFactory<Contracts.IDataSets>("DataSets");
                    dataSetsClient = dataSetsFactory.CreateChannel();
                    ((System.ServiceModel.ICommunicationObject)(dataSetsClient)).Open();
                    dataSetsProxyTable[System.Threading.Thread.CurrentThread.ManagedThreadId] = dataSetsClient;
                }
            }
            finally
            {
                System.Threading.Monitor.Exit(dataSetsProxyTable);
            }
        }

        private void CustomiseAdd(Contracts.AddRequest request)
        {
        }

        private void CustomiseAdd2(Contracts.AddRequest request)
        {
        }

        private void CustomiseAdd3(ref int a, ref int b)
        {
        }

        private void CustomiseWrappedAdd(Contracts.AddRequestWrappedMessage request)
        {
        }

        private void CustomiseWrappednoCustomNamesAdd(Contracts.AddRequestWrappedMessageNoCustomNames request)
        {
        }

        private void CustomiseWrappedMessageWithHeaderAndBodyNamespaceOverrides(Contracts.AddRequestWrappedMessageWithHeaderAndBodyNamespaceOverrides request)
        {
        }

        private void CustomiseUnwrappedAdd(Contracts.AddRequestUnwrappedMessage request)
        {
        }

        private void CustomiseRefParameter(ref int a)
        {
        }

        private void CustomiseRefObjectParameter(object a, object b)
        {
        }

        private void CustomiseMixedDirectionParameters(ref int a, ref int b)
        {
        }

        private void CustomiseOneWayOperation(ref int a)
        {
        }

        private void CustomiseProcessCollection(Contracts.CollectionsData data)
        {
        }

        private void CustomiseProcessCollection2(Contracts.CollectionsData data)
        {
        }

        private void CustomiseOverload(ref int a)
        {
        }

        private void CustomiseOverload2(ref string a)
        {
        }

        private void CustomiseHidden(ref string a)
        {
        }

        private void CustomiseBufferedStreamOperation(out System.IO.Stream input)
        {
            input = new MemoryStream();
        }

        private void CustomiseBufferedMemoryStreamOperation(out System.IO.MemoryStream input)
        {
            input = new MemoryStream();
        }

        private void CustomiseStreamedStreamOperation(out System.IO.Stream input)
        {
            input = new MemoryStream();
        }

        private void CustomiseStreamedMemoryStreamOperation(out System.IO.MemoryStream input)
        {
            input = new MemoryStream();
        }

        private void CustomiseStreamedWithNonStreamParametersOperation(ref string s, out int i)
        {
            s = "hello";
            i = 99;
        }

        private void CustomiseBufferedUnwrappedMessageWithMemoryStream(Contracts.BufferedUnwrappedMessageWithMemoryStream input)
        {
            input.Body2 = new MemoryStream();
        }

        private void CustomiseBufferedUnwrappedMessageWithStream(Contracts.BufferedUnwrappedMessageWithStream input)
        {
            input.Body1 = new MemoryStream();
        }

        private void CustomiseBufferedWrappedMessageWithMemoryStream(Contracts.BufferedWrappedMessageWithMemoryStream input)
        {
            input.Body4 = new MemoryStream();
        }

        private void CustomiseBufferedWrappedMessageWithStream(Contracts.BufferedWrappedMessageWithStream input)
        {
            input.Body3 = new MemoryStream();
        }

        private void CustomiseStreamedUnwrappedMessageWithMemoryStream(Contracts.StreamedUnwrappedMessageWithMemoryStream input)
        {
        }

        private void CustomiseStreamedUnwrappedMessageWithStream(Contracts.StreamedUnwrappedMessageWithStream input)
        {
            input.Body1 = new MemoryStream();
        }

        private void CustomiseStreamedWrappedMessageWithMemoryStream(Contracts.StreamedWrappedMessageWithMemoryStream input)
        {
            input.Body4 = new MemoryStream();
        }

        private void CustomiseStreamedWrappedMessageWithStream(Contracts.StreamedWrappedMessageWithStream input)
        {
            input.Body3 = new MemoryStream();
        }

        private void CustomiseDoSomething(Contracts.Shape parameter)
        {
        }

        private void CustomiseDoSomething2(Contracts.BaseServiceKnownType arg)
        {
        }

        private void CustomiseRegisterForNotificationOneWay(ref string symbol)
        {
        }

        private void CustomiseRegisterForNotificationTwoWay(ref string symbol)
        {
        }

        private void CustomiseProcessDataSet(System.Data.DataSet data)
        {
        }

        private void CustomiseProcessDataSetWithMoreData(System.Data.DataSet data, ref int somedata)
        {
        }

        private void CustomiseProcessCompoundDataSet(Contracts.CompoundWithDataSet data)
        {
        }

        private void CustomiseProcessTypedDataSet(Contracts.TypedDataSet data)
        {
        }

        private void CustomiseProcessCompoundTypedDataSet(Contracts.CompoundWithTypedDataSet data)
        {
        }
    }
}