//---------------------------------------------------------------------
// <copyright file="Logic.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
//     THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
//     OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
//     LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR
//     FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
// <summary>The logic types.</summary>
//---------------------------------------------------------------------

namespace Logic
{
    using System;
    using System.Data;
    using System.IO;
    using System.ServiceModel;
    using System.Threading;
    using System.Xml;
    using Contracts;
    using Contracts.Custom;

    public class Arithmetic : IArithmetic, ICollections, IBufferedStreamService, IStreamedStreamService, IShapeService, IServiceKnownType, ISharePrices
    {
        private ISharePriceCallback callback = null;

        #region IArithmetic Members

        public Response Add(AddRequest request)
        {
            Response r = new Response();
            r.Answer = request.A + request.B;
            Console.WriteLine("Add {0}+{1}={2}", request.A, request.B, r.Answer);
            return r;
        }

        public int Add2(AddRequest request)
        {
            Console.WriteLine("Add2 {0}+{1}={2}", request.A, request.B, request.A + request.B);
            return request.A + request.B;
        }

        public int Add3(int a, int b)
        {
            Console.WriteLine("Add3 {0}+{1}={2}", a, b, a + b);
            return a + b;
        }

        public void WrappedAdd(AddRequestWrappedMessage request)
        {
            Console.WriteLine("Wrapped {0}+{1}={2}", request.A, request.B, request.A + request.B);
        }

        public void UnwrappedAdd(AddRequestUnwrappedMessage request)
        {
            Console.WriteLine("Unwrapped {0}+{1}={2}", request.A, request.B, request.A + request.B);
        }

        public void WrappednoCustomNamesAdd(AddRequestWrappedMessageNoCustomNames request)
        {
            Console.WriteLine("Wrapped no custom names {0}+{1}={2}", request.A, request.B, request.A + request.B);
        }

        public void WrappedMessageWithHeaderAndBodyNamespaceOverrides(AddRequestWrappedMessageWithHeaderAndBodyNamespaceOverrides request)
        {
            Console.WriteLine("Wrapped with custom header and body namesspaces {0}+{1}={2}", request.A, request.B, request.A + request.B);
        }
        
        public void NoParameters()
        {
            Console.WriteLine("No parameters");
        }

        public void RefParameter(ref int a)
        {
            a = a + 1;
            Console.WriteLine("RefParameter returning {0}", a);
        }

        public void RefObjectParameter(object a, ref object b)
        {
            b = a;
            Console.WriteLine("RefObjectParameter returning {0}", a);
        }

        public void OutParameter(out int a, out int b)
        {
            a = 99;
            b = 100;
            Console.WriteLine("OutParameter returning {0} and {1}", a, b);
        }

        public void MixedDirectionParameters(int a, ref int b, out int c)
        {
            b = b + a + 1;
            c = a + 2;
            Console.WriteLine("MixedDirectionParameters returning {0} and {1}", b, c);
        }

        public void OneWayOperation(int a)
        {
            Console.WriteLine("One way call with value {0}", a);
        }

       #endregion

        #region ICollections Members

        public CollectionsData ProcessCollection(CollectionsData data)
        {
            Console.WriteLine("Process Collection called");
            XmlDocument doc1 = new XmlDocument();
            doc1.LoadXml("<test>Element</test>");
            XmlDocument doc2 = new XmlDocument();
            doc2.LoadXml("<test><node>one</node></test>");
            XmlDocument doc3 = new XmlDocument();
            doc3.LoadXml("<test><node>two</node></test>");

            CollectionsData newData = new CollectionsData();
            newData.Element = doc1.DocumentElement;
            newData.Nodes = new XmlNode[2];
            newData.Nodes[0] = doc2;
            newData.Nodes[1] = doc3;

            Console.WriteLine("Process Collection returning");
            return newData;
        }
        #endregion

        #region IBufferedStreamService Members

        public Stream BufferedStreamOperation(Stream input)
        {
            return CopyToNewStream(input);
        }

        public MemoryStream BufferedMemoryStreamOperation(MemoryStream input)
        {
            input.Position = 0;
            return input;
        }

        public void BufferedUnwrappedMessageWithStream(BufferedUnwrappedMessageWithStream input)
        {
        }

        public void BufferedUnwrappedMessageWithMemoryStream(BufferedUnwrappedMessageWithMemoryStream input)
        {
        }

        public void BufferedWrappedMessageWithStream(BufferedWrappedMessageWithStream input)
        {
        }

        public void BufferedWrappedMessageWithMemoryStream(BufferedWrappedMessageWithMemoryStream input)
        {
        }

        #endregion

        #region IStreamedStreamService Members

        public Stream StreamedStreamOperation(Stream input)
        {
            Console.WriteLine("Starting StreamedStreamOperation");
            Stream ans = CopyToNewStream(input);
            Console.WriteLine("Completing StreamedStreamOperation");
            return ans;
        }

        public MemoryStream StreamedMemoryStreamOperation(MemoryStream input)
        {
            input.Position = 0;
            return input;
        }

        public void StreamedWithNonStreamParametersOperation(string s, int i)
        {
        }

        public void StreamedUnwrappedMessageWithStream(StreamedUnwrappedMessageWithStream input)
        {
        }

        public void StreamedUnwrappedMessageWithMemoryStream(StreamedUnwrappedMessageWithMemoryStream input)
        {
        }

        public void StreamedWrappedMessageWithStream(StreamedWrappedMessageWithStream input)
        {
        }

        public void StreamedWrappedMessageWithMemoryStream(StreamedWrappedMessageWithMemoryStream input)
        {
        }

        #endregion

        #region IShapeService Members

        public Shape DoSomething(Shape parameter)
        {
            Shape ans = null;
            if (parameter is Circle)
            {
                ans = new Rectangle();
            }
            else
            {
                ans = new Circle();
            }

            return ans;
        }

        #endregion

        #region IServiceKnownType Members

        public void DoSomething(BaseServiceKnownType arg)
        {
            Console.WriteLine("ServiceKnownType: {0}", arg.GetType().Name);
        }

        #endregion

        #region ISharePrices Members

        public void RegisterForNotificationOneWay(string symbol)
        {
            Console.WriteLine("Received request for one-way updates to {0}", symbol);
            this.callback = OperationContext.Current.GetCallbackChannel<ISharePriceCallback>();
            Thread updaterThread = new Thread(this.SendSharePriceUpdatesOneWay);
            updaterThread.Start();
        }

        public void RegisterForNotificationTwoWay(string symbol)
        {
            Console.WriteLine("Received request for two-way updates to {0}", symbol);
            this.callback = OperationContext.Current.GetCallbackChannel<ISharePriceCallback>();
            Thread updaterThread = new Thread(this.SendSharePriceUpdatesTwoWay);
            updaterThread.Start();
        }

       #endregion

        private static void CopyStream(Stream input, Stream output)
        {
            byte[] buffer = new byte[1024];
            while (true)
            {
                int len = input.Read(buffer, 0, buffer.Length);
                if (len > 0)
                {
                    output.Write(buffer, 0, len);
                }
                else
                {
                    break;
                }
            }
        }

        private static MemoryStream CopyToNewStream(Stream input)
        {
            MemoryStream ms = new MemoryStream();
            CopyStream(input, ms);
            ms.Position = 0;
            return ms;
        }

        private void SendSharePriceUpdatesOneWay()
        {
            try
            {
                this.callback.PriceOneWay("MSFT", 30.0f);
                this.callback.PriceOneWay("MSFT", 29.0f);
                this.callback.PriceOneWay("MSFT", 31.0f);
                this.callback.PriceOneWay("DONE", -1.0f);
            }
            catch (TimeoutException)
            {
            }

            Console.WriteLine("Completed sending share price updates one-way");
        }

        private void SendSharePriceUpdatesTwoWay()
        {
            try
            {
                this.callback.PriceTwoWay("MSFT", 30.0f);
                this.callback.PriceTwoWay("MSFT", 29.0f);
                this.callback.PriceTwoWay("MSFT", 31.0f);
                this.callback.PriceTwoWay("DONE", -1.0f);
            }
            catch (TimeoutException)
            {
            }

            Console.WriteLine("Completed sending share price updates two way");
        }
    }

    public class DataSets : IDataSets
    {
        #region IDataSets Members

        public DataSet ProcessDataSet(DataSet data)
        {
            Console.WriteLine("DataSet");
            return data;
        }

        public DataSet ProcessDataSetWithMoreData(DataSet data, int somedata)
        {
            Console.WriteLine("DataSet with more data");
            return data;
        }

        public void ProcessTypedDataSet(TypedDataSet data)
        {
            Console.WriteLine("ProcessTypedDataSet");
        }

        public void ProcessCompoundDataSet(CompoundWithDataSet data)
        {
            Console.WriteLine("ProcessCompoundDataSet");
        }

        public void ProcessCompoundTypedDataSet(CompoundWithTypedDataSet data)
        {
            Console.WriteLine("ProcessCompoundTypesDataSet");
        }

        #endregion
    }

    public class Custom : ICustomContracts, ICustomContracts2
    {
        #region ICustomContracts Members

        public void Overload(int a)
        {
            Console.WriteLine("{0} called with param={1}", System.Reflection.MethodInfo.GetCurrentMethod().Name, a);
        }

        public void Overload(string a)
        {
            Console.WriteLine("{0} called with param={1}", System.Reflection.MethodInfo.GetCurrentMethod().Name, a);
        }

        public void Hidden(string a)
        {
            Console.WriteLine("{0} called with param={1}", System.Reflection.MethodInfo.GetCurrentMethod().Name, a);
        }

        void ICustomContracts.Overload(int a)
        {
            Console.WriteLine("{0} called with param={1}", System.Reflection.MethodInfo.GetCurrentMethod().Name, a);
        }

        void ICustomContracts.Overload(string a)
        {
            Console.WriteLine("{0} called with param={1}", System.Reflection.MethodInfo.GetCurrentMethod().Name, a);
        }

        #endregion

        #region ICustomContracts2 Members

        public void Contract2Method()
        {
            Console.WriteLine("Contract2Method called");
        }

        #endregion
    }
}
