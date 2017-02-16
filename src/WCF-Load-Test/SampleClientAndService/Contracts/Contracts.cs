//---------------------------------------------------------------------
// <copyright file="Contracts.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
//     THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
//     OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
//     LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR
//     FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
// <summary>The contract types.</summary>
//---------------------------------------------------------------------

namespace Contracts
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data;
    using System.Data.SqlTypes;
    using System.IO;
    using System.Runtime.Serialization;
    using System.ServiceModel;
    using System.Xml;

    [ServiceContract(Namespace = "http://contoso.com/service/test")]
    public interface IArithmetic
    {
        [OperationContract]
        Response Add(AddRequest request);

        [OperationContract]
        int Add2(AddRequest request);

        [OperationContract]
        int Add3(int a, int b);

        [OperationContract]
        void WrappedAdd(AddRequestWrappedMessage request);

        [OperationContract]
        void WrappednoCustomNamesAdd(AddRequestWrappedMessageNoCustomNames request);

        [OperationContract]
        void WrappedMessageWithHeaderAndBodyNamespaceOverrides(AddRequestWrappedMessageWithHeaderAndBodyNamespaceOverrides request);

        [OperationContract]
        void UnwrappedAdd(AddRequestUnwrappedMessage request);

        [OperationContract]
        void NoParameters();

        [OperationContract]
        void RefParameter(ref int a);

        [OperationContract]
        void RefObjectParameter(object a, ref object b);

        [OperationContract]
        void OutParameter(out int a, out int b);

        [OperationContract]
        void MixedDirectionParameters(int a, ref int b, out int c);

        [OperationContract(IsOneWay = true)]
        void OneWayOperation(int a);
    }

    [ServiceContract(Namespace = "http://contoso.com/service/test")]
    public interface ICollections
    {
        [OperationContract]
        CollectionsData ProcessCollection(CollectionsData data);
    }

    [ServiceContract(Namespace = "http://contoso.com/service/datasettest")]
    public interface IDataSets
    {
        [OperationContract]
        DataSet ProcessDataSet(DataSet data);

        [OperationContract]
        DataSet ProcessDataSetWithMoreData(DataSet data, int somedata);

        [OperationContract]
        void ProcessTypedDataSet(TypedDataSet data);

        [OperationContract]
        void ProcessCompoundDataSet(CompoundWithDataSet data);

        [OperationContract]
        void ProcessCompoundTypedDataSet(CompoundWithTypedDataSet data);
    }

    // The next two interfaces are identical, I have two so that I can filter the trace file for
    // buffered and unbuffered to allow me write separate tests but keep all the trace data together.
    [ServiceContract(Namespace = "http://contoso.com/service/test")]
    public interface IBufferedStreamService
    {
        [OperationContract]
        Stream BufferedStreamOperation(Stream input);

        [OperationContract]
        MemoryStream BufferedMemoryStreamOperation(MemoryStream input);

        [OperationContract]
        void BufferedUnwrappedMessageWithStream(BufferedUnwrappedMessageWithStream input);

        [OperationContract]
        void BufferedUnwrappedMessageWithMemoryStream(BufferedUnwrappedMessageWithMemoryStream input);

        [OperationContract]
        void BufferedWrappedMessageWithStream(BufferedWrappedMessageWithStream input);

        [OperationContract]
        void BufferedWrappedMessageWithMemoryStream(BufferedWrappedMessageWithMemoryStream input);
    }

    [ServiceContract(Namespace = "http://contoso.com/service/test")]
    public interface IStreamedStreamService
    {
        [OperationContract]
        Stream StreamedStreamOperation(Stream input);

        [OperationContract]
        MemoryStream StreamedMemoryStreamOperation(MemoryStream input);

        [OperationContract]
        void StreamedWithNonStreamParametersOperation(string s, int i);

        [OperationContract]
        void StreamedUnwrappedMessageWithStream(StreamedUnwrappedMessageWithStream input);

        [OperationContract]
        void StreamedUnwrappedMessageWithMemoryStream(StreamedUnwrappedMessageWithMemoryStream input);

        [OperationContract]
        void StreamedWrappedMessageWithStream(StreamedWrappedMessageWithStream input);

        [OperationContract]
        void StreamedWrappedMessageWithMemoryStream(StreamedWrappedMessageWithMemoryStream input);
    }

    [ServiceContract]
    [ServiceKnownType(typeof(DerivedServiceKnownType))]
    public interface IServiceKnownType
    {
        [OperationContract]
        void DoSomething(BaseServiceKnownType arg);
    }

    [ServiceContract(Namespace = "http://contoso.com/service/test")]
    public interface IShapeService
    {
        [OperationContract]
        Shape DoSomething(Shape parameter);
    }

    [ServiceContract(Namespace = "http://contoso.com/service/test", CallbackContract = typeof(ISharePriceCallback))]
    public interface ISharePrices
    {
        [OperationContract(IsOneWay = true)]
        void RegisterForNotificationOneWay(string symbol);

        [OperationContract(IsOneWay = false)]
        void RegisterForNotificationTwoWay(string symbol);
    }

    public interface ISharePriceCallback
    {
        [OperationContract(IsOneWay = true)]
        void PriceOneWay(string symbol, float price);

        [OperationContract(IsOneWay = false)]
        void PriceTwoWay(string symbol, float price);
    }

    [DataContract(Namespace = "http://contoso.com/data/test")]
    public class AddRequest
    {
        private int a;
        private int b;

        [DataMember]
        public int A
        {
            get { return this.a; }
            set { this.a = value; }
        }

        [DataMember]
        public int B
        {
            get { return this.b; }
            set { this.b = value; }
        }
    }

    [DataContract(Namespace = "http://contoso.com/service/datasettest")]
    public class CompoundWithDataSet
    {
        [DataMember]
        public DataSet Data { get; set; }
    }

    [DataContract(Namespace = "http://contoso.com/service/datasettest")]
    public class CompoundWithTypedDataSet
    {
        [DataMember]
        public TypedDataSet Data { get; set; }
    }

    [DataContract]
    public class TestHeader
    {
        [DataMember]
        public string Header;
    }

    [MessageContract(IsWrapped = true)]
    public class AddRequestWrappedMessageNoCustomNames
    {
        [MessageHeader]
        public TestHeader header;

        [MessageBodyMember]
        public int A;

        [MessageBodyMember]
        public int B;
    }

    [MessageContract(IsWrapped = true, WrapperName = "MyWrapper", WrapperNamespace = "http://wrapper/")]
    public class AddRequestWrappedMessage
    {
        [MessageHeader]
        public TestHeader header;

        [MessageBodyMember]
        public int A;

        [MessageBodyMember]
        public int B;
    }

    [MessageContract(IsWrapped = true, WrapperName = "MyWrapper", WrapperNamespace = "http://overrideswrapper/")]
    public class AddRequestWrappedMessageWithHeaderAndBodyNamespaceOverrides
    {
        [MessageHeader(Namespace = "http://override1")]
        public TestHeader header;

        [MessageBodyMember(Namespace = "http://override2")]
        public int A;

        [MessageBodyMember(Namespace = "http://override3")]
        public int B;
    }

    [MessageContract(IsWrapped = false, WrapperName = "MyWrapper", WrapperNamespace = "http://wrapper/")]
    public class AddRequestUnwrappedMessage
    {
        [MessageHeader]
        public TestHeader header;

        [MessageBodyMember]
        public int A;

        [MessageBodyMember]
        public int B;
    }

    [DataContract(Namespace = "http://contoso.com/data/test")]
    public class Response
    {
        private int answer;

        [DataMember]
        public int Answer
        {
            get { return this.answer; }
            set { this.answer = value; }
        }
    }

    [DataContract(Namespace = "http://contoso.com/data/test")]
    public class CollectionsData
    {
        [DataMember]
        public ArrayList ArrayList;

        [DataMember]
        public List<int> IntList;

        [DataMember]
        public IList<int> IntIList;

        [DataMember]
        public List<AddRequest> RequestList;

        [DataMember]
        public IList<AddRequest> RequestIList;

        [DataMember]
        public Dictionary<string, AddRequest> RequestDictionary;

        [DataMember]
        public IDictionary<string, AddRequest> RequestIDictionary;

        [DataMember]
        public NonGenericEnumerableOnlyCollection NonGenericEnumerableOnlyCollection;

        [DataMember]
        public Hashtable Hashtable;

        [DataMember]
        public XmlElement Element;

        [DataMember]
        public XmlNode[] Nodes;

        [DataMember]
        public SqlXml SqlData;

        [DataMember]
        private Collection<AddRequest> RequestCollection;

        [DataMember]
        private ICollection<AddRequest> RequestICollection;
    }

    public class NonGenericEnumerableOnlyCollection : IEnumerable
    {
        private ArrayList array = new ArrayList();

        public void Add(object o)
        {
            this.array.Add(o);
        }
        #region IEnumerable Members

        public IEnumerator GetEnumerator()
        {
            return this.array.GetEnumerator();
        }

        #endregion
    }

    [MessageContract(IsWrapped = false, WrapperName = "MyWrapper", WrapperNamespace = "http://wrapper/")]
    public class BufferedUnwrappedMessageWithStream
    {
        [MessageHeader]
        public int Header;

        [MessageBodyMember]
        public Stream Body1;
    }

    [MessageContract(IsWrapped = false, WrapperName = "MyWrapper", WrapperNamespace = "http://wrapper/BufferedUnwrappedMessageWithMemoryStream")]
    public class BufferedUnwrappedMessageWithMemoryStream
    {
        [MessageHeader]
        public int Header;

        [MessageBodyMember]
        public MemoryStream Body2;
    }

    [MessageContract(IsWrapped = true, WrapperName = "MyWrapper", WrapperNamespace = "http://wrapper/BufferedWrappedMessageWithStream")]
    public class BufferedWrappedMessageWithStream
    {
        [MessageHeader]
        public int Header;

        [MessageBodyMember]
        public Stream Body3;
    }

    [MessageContract(IsWrapped = true, WrapperName = "MyWrapper", WrapperNamespace = "http://wrapper/BufferedWrappedMessageWithMemoryStream")]
    public class BufferedWrappedMessageWithMemoryStream
    {
        [MessageHeader]
        public int Header;

        [MessageBodyMember]
        public MemoryStream Body4;
    }

    [MessageContract(IsWrapped = false, WrapperName = "MyWrapper", WrapperNamespace = "http://wrapper/StreamedUnwrappedMessageWithStream")]
    public class StreamedUnwrappedMessageWithStream
    {
        [MessageHeader]
        public int Header;

        [MessageBodyMember]
        public Stream Body1;
    }

    [MessageContract(IsWrapped = false, WrapperName = "MyWrapper", WrapperNamespace = "http://wrapper/StreamedUnwrappedMessageWithMemoryStream")]
    public class StreamedUnwrappedMessageWithMemoryStream
    {
        [MessageHeader]
        public int Header;

        [MessageBodyMember]
        public MemoryStream Body2;
    }

    [MessageContract(IsWrapped = true, WrapperName = "MyWrapper", WrapperNamespace = "http://wrapper/StreamedWrappedMessageWithStream")]
    public class StreamedWrappedMessageWithStream
    {
        [MessageHeader]
        public int Header;

        [MessageBodyMember]
        public Stream Body3;
    }

    [MessageContract(IsWrapped = true, WrapperName = "MyWrapper", WrapperNamespace = "http://wrapper/StreamedWrappedMessageWithMemoryStream")]
    public class StreamedWrappedMessageWithMemoryStream
    {
        [MessageHeader]
        public int Header;

        [MessageBodyMember]
        public MemoryStream Body4;
    }

    [DataContract(Namespace = "http://contoso.com/data/test")]
    [KnownType(typeof(Circle))]
    [KnownType(typeof(Rectangle))]
    public class Shape
    {
        private int colour;

        [DataMember]
        public int Colour
        {
            get { return this.colour; }
            set { this.colour = value; }
        }
    }

    [DataContract(Namespace = "http://contoso.com/data/test")]
    public class Circle : Shape
    {
        private int radius;

        [DataMember]
        public int Radius
        {
            get { return this.radius; }
            set { this.radius = value; }
        }
    }

    [DataContract(Namespace = "http://contoso.com/data/test")]
    public class Rectangle : Shape
    {
        private int verticalSideLength;
        private int horizontalSideLength;

        public int VerticalSideLength
        {
            get { return this.verticalSideLength; }
            set { this.verticalSideLength = value; }
        }

        public int HorizontalSideLength
        {
            get { return this.horizontalSideLength; }
            set { this.horizontalSideLength = value; }
        }
    }

    [DataContract]
    public class BaseServiceKnownType
    {
        [DataMember]
        public int BaseProperty
        {
            get;
            set;
        }
    }

    [DataContract]
    public class DerivedServiceKnownType : BaseServiceKnownType
    {
        [DataMember]
        public string DerivedProperty
        {
            get;
            set;
        }
    }
}
