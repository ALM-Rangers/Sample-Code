//---------------------------------------------------------------------
// <copyright file="Wcf.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
//     THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
//     OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
//     LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR
//     FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
// <summary>The Wcf type.</summary>
//---------------------------------------------------------------------

namespace Client
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Globalization;
    using System.IO;
    using System.ServiceModel;
    using System.Xml;
    using ClientProxies.Custom;
    using GeneratedContracts;
    using GeneratedContractsAsmx;

    /// <summary>
    /// Performs WCF calls to ASMX and WCF services.
    /// </summary>
    public static class Wcf
    {
        /// <summary>
        /// Calls the ASMX service using WCF.
        /// </summary>
        public static void AsmxCallsUsingWcf()
        {
            TestAsmxServiceSoapClient asmxClient = new TestAsmxServiceSoapClient("TestAsmxServiceSoap");
            SimpleAsmxRequest request = new SimpleAsmxRequest();
            request.A = 99;
            request.B = "hello";
            request.OptionalSpecified = false;
            asmxClient.ProcessSimpleAsmxRequestWrapped(request);
            asmxClient.ProcessSimpleAsmxRequestBare(request);
            asmxClient.ProcessMultipleParametersWrapped(99, "hello");

            asmxClient = new TestAsmxServiceSoapClient("TestAsmxServiceSoap12");
            asmxClient.ProcessSimpleAsmxRequestWrapped(request);
            asmxClient.ProcessSimpleAsmxRequestBare(request);
            asmxClient.ProcessMultipleParametersWrapped(99, "hello");

            asmxClient.WithNullableInt(null);
            asmxClient.WithNullableInt(1);

            asmxClient.SimpleTypes(1, GeneratedContractsAsmx.ConsoleColor.Black, "a", 1.1m, DateTime.Now, Guid.NewGuid(), System.Xml.XmlQualifiedName.Empty);

            asmxClient.ScalarArray(new int[] { 0, 1, 2 });

            XmlAsmxRequest data = new XmlAsmxRequest();
            XmlDocument xmle = new XmlDocument();
            xmle.LoadXml("<test xmlns=\"\">Element</test>");
            data.Element = xmle.DocumentElement;
            data.Nodes = new System.Xml.XmlNode[2];
            XmlDocument xmln1 = new XmlDocument();
            xmln1.LoadXml("<test xmlns=\"\"><node>one</node></test>");
            data.Nodes[0] = xmln1.DocumentElement;
            XmlDocument xmln2 = new XmlDocument();
            xmln2.LoadXml("<test xmlns=\"\"><node>two</node></test>");
            data.Nodes[1] = xmln2.DocumentElement;

            asmxClient.XmlRequestMethod(data);

            CollectionsRequest cr = new CollectionsRequest();

            cr.ArrayList = new object[] { 1, "hello" };

            cr.IntList = new int[] { 0, 1, 2 };

            cr.RequestList = new SimpleAsmxRequest[1];
            cr.RequestList[0] = new SimpleAsmxRequest();

            cr.RequestCollection = new SimpleAsmxRequest[1];
            cr.RequestCollection[0] = new SimpleAsmxRequest();

            cr.NonGenericEnumerableOnlyCollection = new object[] { "hello", 1 };

            asmxClient.CollectionMethod(cr);

            AsmxCircle shape = new AsmxCircle();
            asmxClient.ProcessShape(shape);

            DataSet ds = CreateTestDataSet();
            asmxClient.ProcessDataSet(ds);
            asmxClient.ProcessDataSetWithMoreData(ds, 1);
            AsmxCompoundWithDataSet compoundWithDataSet = new AsmxCompoundWithDataSet();
            compoundWithDataSet.Data = ds;
            asmxClient.ProcessCompoundDataSet(compoundWithDataSet);
            asmxClient.ProcessTypedDataSet(CreateTestAsmxTypedDataSet());
            AsmxCompoundWithTypedDataSet compoundWithTypedDataSet = new AsmxCompoundWithTypedDataSet();
            compoundWithTypedDataSet.Data = CreateTestAsmxTypedDataSet();
            asmxClient.ProcessCompoundTypedDataSet(compoundWithTypedDataSet);
        }

        /// <summary>
        /// Calls the WCF service.
        /// </summary>
        public static void WcfCalls()
        {
            ChannelFactory<GeneratedContracts.ICollections> ccf = new ChannelFactory<GeneratedContracts.ICollections>("Collections");
            ChannelFactory<GeneratedContracts.IArithmetic> cf = new ChannelFactory<GeneratedContracts.IArithmetic>("Basic");
            ChannelFactory<GeneratedContracts.IBufferedStreamService> bufferedcf = new ChannelFactory<GeneratedContracts.IBufferedStreamService>("BufferedStreams");
            ChannelFactory<GeneratedContracts.IStreamedStreamService> streamedcf = new ChannelFactory<GeneratedContracts.IStreamedStreamService>("StreamedStreams");
            ChannelFactory<GeneratedContracts.IDataSets> datasetscf = new ChannelFactory<IDataSets>("DataSets");
            GeneratedContracts.IArithmetic ia = cf.CreateChannel();
            GeneratedContracts.ICollections ic = ccf.CreateChannel();
            GeneratedContracts.IBufferedStreamService buffered = bufferedcf.CreateChannel();
            GeneratedContracts.IStreamedStreamService streamed = streamedcf.CreateChannel();
            GeneratedContracts.IDataSets datasets = datasetscf.CreateChannel();

            AddRequest r = new AddRequest();
            r.A = 10;
            r.B = 5;
            Console.WriteLine(ia.Add(r).Answer);
            Console.WriteLine(ia.Add2(r));
            Console.WriteLine(ia.Add3(20, 25));

            AddRequestWrappedMessage wrappedRequest = new AddRequestWrappedMessage();
            wrappedRequest.header = new TestHeader();
            wrappedRequest.header.Header = "hello";
            wrappedRequest.A = 11;
            wrappedRequest.B = 6;
            ia.WrappedAdd(wrappedRequest);

            AddRequestWrappedMessageNoCustomNames wrappedRequestNoCustomNames = new AddRequestWrappedMessageNoCustomNames();
            wrappedRequestNoCustomNames.header = new TestHeader();
            wrappedRequestNoCustomNames.header.Header = "hello";
            wrappedRequestNoCustomNames.A = 11;
            wrappedRequestNoCustomNames.B = 6;
            ia.WrappednoCustomNamesAdd(wrappedRequestNoCustomNames);

            AddRequestWrappedMessageWithHeaderAndBodyNamespaceOverrides wrappedRequestNamespaceOverrides = new AddRequestWrappedMessageWithHeaderAndBodyNamespaceOverrides();
            wrappedRequestNamespaceOverrides.header = new TestHeader();
            wrappedRequestNamespaceOverrides.header.Header = "hello";
            wrappedRequestNamespaceOverrides.A = 11;
            wrappedRequestNamespaceOverrides.B = 6;
            ia.WrappedMessageWithHeaderAndBodyNamespaceOverrides(wrappedRequestNamespaceOverrides);

            AddRequestUnwrappedMessage unwrappedRequest = new AddRequestUnwrappedMessage();
            unwrappedRequest.header = new TestHeader();
            unwrappedRequest.header.Header = "hello";
            unwrappedRequest.A = 11;
            unwrappedRequest.B = 6;
            ia.UnwrappedAdd(unwrappedRequest);

            ia.NoParameters();

            int refparam = 0;
            ia.RefParameter(ref refparam);

            object refobj = null;
            string s = "hello";
            ia.RefObjectParameter(s, ref refobj);

            int outParam1;
            int outParam2;
            outParam1 = ia.OutParameter(out outParam2);

            int refb = 23;
            int refc;
            ia.MixedDirectionParameters(1, ref refb, out refc);

            ia.OneWayOperation(5);

            CollectionsData coll = new CollectionsData();
            coll.ArrayList = new object[] { 1, 2, 3 };

            coll.IntIList = new int[] { 4, 5, 6 };

            coll.IntIList = new int[] { 7, 8, 9 };

            AddRequest addReq1 = new AddRequest();
            addReq1.A = 1;
            addReq1.B = 2;
            AddRequest addReq2 = new AddRequest();
            addReq2.A = 3;
            addReq2.B = 4;

            coll.RequestList = new AddRequest[] { addReq1, addReq2 };
            coll.RequestIList = new AddRequest[] { addReq1, addReq2 };
            coll.RequestCollection = new AddRequest[] { addReq1, addReq2 };
            coll.RequestICollection = new AddRequest[] { addReq1, addReq2 };

            coll.RequestDictionary = new Dictionary<string, AddRequest>();
            coll.RequestDictionary.Add("Key1", addReq1);
            coll.RequestDictionary.Add("Key2", addReq2);

            coll.RequestIDictionary = new Dictionary<string, AddRequest>();
            coll.RequestIDictionary.Add("Key1", addReq1);
            coll.RequestIDictionary.Add("Key2", addReq2);

            coll.NonGenericEnumerableOnlyCollection = new object[] { 0, "hello" };

            coll.Hashtable = new Dictionary<object, object>();
            coll.Hashtable.Add(1, "one");
            coll.Hashtable.Add(2, "two");

            CollectionsData c = ic.ProcessCollection(coll);
            ic.ProcessCollection(c);

            CustomContracts custom = new CustomContracts();
            custom.Overload(23);
            custom.Overload("abc");
            custom.Hidden();

            ChannelFactory<Contracts.Custom.ICustomContracts2> factory2 = new ChannelFactory<Contracts.Custom.ICustomContracts2>("Custom2");
            Contracts.Custom.ICustomContracts2 custom2 = factory2.CreateChannel();
            custom2.Contract2Method();

            // Data contract with streaming
            MemoryStream ms = new MemoryStream();
            for (byte i = 0; i < 127; i++)
            {
                ms.WriteByte(i);
            }

            ms.Position = 0;
            Stream ans = buffered.BufferedStreamOperation(ms);

            ms.Position = 0;
            MemoryStream memoryStreamAns = buffered.BufferedMemoryStreamOperation(ms);

            ms.Position = 0;
            ans = streamed.StreamedStreamOperation(ms);

            memoryStreamAns = streamed.StreamedMemoryStreamOperation(ms);

            streamed.StreamedWithNonStreamParametersOperation("hello", 1);

            // Message contract with streaming
            byte[] buf = new byte[128];
            for (byte i = 0; i < 127; i++)
            {
                buf[i] = i;
            }

            ms.Position = 0;
            buffered.BufferedUnwrappedMessageWithMemoryStream(new BufferedUnwrappedMessageWithMemoryStream(1, ms));

            ms.Position = 0;
            buffered.BufferedUnwrappedMessageWithStream(new BufferedUnwrappedMessageWithStream(1, ms));

            ms.Position = 0;
            buffered.BufferedWrappedMessageWithMemoryStream(new BufferedWrappedMessageWithMemoryStream(1, ms));

            buffered.BufferedWrappedMessageWithStream(new BufferedWrappedMessageWithStream(1, buf));

            ms.Position = 0;
            streamed.StreamedUnwrappedMessageWithMemoryStream(new StreamedUnwrappedMessageWithMemoryStream(1, ms));

            ms.Position = 0;
            streamed.StreamedUnwrappedMessageWithStream(new StreamedUnwrappedMessageWithStream(1, ms));

            ms.Position = 0;
            streamed.StreamedWrappedMessageWithMemoryStream(new StreamedWrappedMessageWithMemoryStream(1, ms));

            ms.Position = 0;
            streamed.StreamedWrappedMessageWithStream(new StreamedWrappedMessageWithStream(1, buf));

            ChannelFactory<IShapeService> shapeServiceFactory = new ChannelFactory<IShapeService>("Shape");
            IShapeService shapeService = shapeServiceFactory.CreateChannel();

            Rectangle rect = (Rectangle)shapeService.DoSomething(new Circle());

            ChannelFactory<IServiceKnownType> serviceKnownTypeFactory = new ChannelFactory<IServiceKnownType>("ServiceKnownType");
            DerivedServiceKnownType derived = new DerivedServiceKnownType();
            derived.BaseProperty = 1;
            derived.DerivedProperty = "abc";
            IServiceKnownType serviceKnownTypeService = serviceKnownTypeFactory.CreateChannel();
            serviceKnownTypeService.DoSomething(derived);

            InstanceContext callbackContext = new InstanceContext(new SharePrice());
            SharePricesClient sharePrices = new SharePricesClient(callbackContext, "SharePrices");
            sharePrices.Open();
            sharePrices.RegisterForNotificationOneWay("MSFT");
            if (!SharePrice.Done.WaitOne(10000))
            {
                Console.WriteLine("Timeout waiting for duplex responses one-way");
            }

            sharePrices.RegisterForNotificationTwoWay("MSFT");
            if (!SharePrice.Done.WaitOne(10000))
            {
                Console.WriteLine("Timeout waiting for duplex responses two-way");
            }

            DataSet ds = CreateTestDataSet();

            datasets.ProcessDataSet(new ProcessDataSetRequest(ds));
            datasets.ProcessDataSetWithMoreData(new ProcessDataSetWithMoreDataRequest(ds, 1));
            CompoundWithDataSet compoundWithDataSet = new CompoundWithDataSet();
            compoundWithDataSet.Data = ds;
            datasets.ProcessCompoundDataSet(new ProcessCompoundDataSetRequest(compoundWithDataSet));
            datasets.ProcessTypedDataSet(new ProcessTypedDataSetRequest(CreateTestTypedDataSet()));
            CompoundWithTypedDataSet compoundWithTypedDataSet = new CompoundWithTypedDataSet();
            compoundWithTypedDataSet.Data = CreateTestTypedDataSet();
            datasets.ProcessCompoundTypedDataSet(new ProcessCompoundTypedDataSetRequest(compoundWithTypedDataSet));
        }

        /// <summary>
        /// Generates a test dataset.
        /// </summary>
        /// <returns>The test dataset.</returns>
        public static GeneratedContractsAsmx.AsmxTypedDataSet CreateTestAsmxTypedDataSet()
        {
            GeneratedContractsAsmx.AsmxTypedDataSet ans = new GeneratedContractsAsmx.AsmxTypedDataSet();

            ans.DataTable1.AddDataTable1Row("hello1", 1, new DateTime(2010, 6, 29));
            ans.DataTable1.AddDataTable1Row("hello2", 2, new DateTime(2010, 5, 29));
            ans.DataTable1.Rows.Add(DBNull.Value, DBNull.Value, DBNull.Value);

            return ans;
        }

        /// <summary>
        /// Generates a test dataset.
        /// </summary>
        /// <returns>The test dataset.</returns>
        public static DataSet CreateTestDataSet()
        {
            DataSet data = new DataSet("test");
            data.Locale = CultureInfo.InvariantCulture;

            DataTable table1 = data.Tables.Add("table1", "urn:table1");
            table1.Columns.Add("col1_1", typeof(int));
            table1.Columns.Add("col1_2", typeof(string));
            table1.Rows.Add(1, "hello1");
            table1.Rows.Add(2, "hello2");
            table1.Rows.Add(DBNull.Value, DBNull.Value);

            DataTable table2 = data.Tables.Add("table2", "urn:table2");
            table2.Columns.Add("col2_1", typeof(bool));
            table2.Columns.Add("col2_2", typeof(decimal));
            table2.Rows.Add(true, 1.0m);
            table2.Rows.Add(false, 99.0m);
            return data;
        }

        /// <summary>
        /// Generates a test dataset.
        /// </summary>
        /// <returns>The test dataset.</returns>
        private static GeneratedContracts.TypedDataSet CreateTestTypedDataSet()
        {
            GeneratedContracts.TypedDataSet ans = new GeneratedContracts.TypedDataSet();

            ans.DataTable1.AddDataTable1Row("hello1", 1, new DateTime(2010, 6, 29));
            ans.DataTable1.AddDataTable1Row("hello2", 2, new DateTime(2010, 5, 29));
            ans.DataTable1.Rows.Add(DBNull.Value, DBNull.Value, DBNull.Value);

            return ans;
        }
    }
}
