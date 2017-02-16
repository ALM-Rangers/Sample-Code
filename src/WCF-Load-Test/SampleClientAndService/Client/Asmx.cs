//---------------------------------------------------------------------
// <copyright file="Asmx.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
//     THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
//     OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
//     LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR
//     FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
// <summary>The Asmx type.</summary>
//---------------------------------------------------------------------

namespace Client
{
    using System;
    using System.Data;
    using System.Xml;

    /// <summary>
    /// Calls ASMX service using an ASMX proxy generated using the WSDL.EXE tool.
    /// </summary>
    public static class Asmx
    {
        /// <summary>
        /// Tests using the ASMX proxy generated using the WSDL.EXE tool.
        /// </summary>
        /// <remarks>
        /// Use Fiddler2 to get the traces from this test. Configure Fiddler to redirect the host name below to redirect to localhost and the port the service is listening on.
        /// </remarks>
        public static void AsmxCallsUsingAsmxProxy()
        {
            AsmxProxy.TestAsmxService asmxClient = new AsmxProxy.TestAsmxService();
            asmxClient.Url = "http://" + Environment.MachineName + ":8082/TestAsmxService.asmx";
            AsmxProxy.SimpleAsmxRequest request = new AsmxProxy.SimpleAsmxRequest();
            request.A = 99;
            request.B = "hello";
            request.OptionalSpecified = false;
            asmxClient.ProcessSimpleAsmxRequestWrapped(request);
            asmxClient.ProcessSimpleAsmxRequestBare(request);
            asmxClient.ProcessMultipleParametersWrapped(99, "hello");

            asmxClient.WithNullableInt(null);
            asmxClient.WithNullableInt(1);

            asmxClient.SimpleTypes(1, AsmxProxy.ConsoleColor.Black, "a", 1.1m, DateTime.Now, Guid.NewGuid(), System.Xml.XmlQualifiedName.Empty);

            asmxClient.ScalarArray(new int[] { 0, 1, 2 });

            AsmxProxy.XmlAsmxRequest data = new AsmxProxy.XmlAsmxRequest();
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

            AsmxProxy.CollectionsRequest cr = new AsmxProxy.CollectionsRequest();

            cr.ArrayList = new object[] { 1, "hello" };

            cr.IntList = new int[] { 0, 1, 2 };

            cr.RequestList = new AsmxProxy.SimpleAsmxRequest[1];
            cr.RequestList[0] = new AsmxProxy.SimpleAsmxRequest();

            cr.RequestCollection = new AsmxProxy.SimpleAsmxRequest[1];
            cr.RequestCollection[0] = new AsmxProxy.SimpleAsmxRequest();

            cr.NonGenericEnumerableOnlyCollection = new object[] { "hello", 1 };

            asmxClient.CollectionMethod(cr);

            AsmxProxy.AsmxCircle shape = new AsmxProxy.AsmxCircle();
            asmxClient.ProcessShape(shape);

            DataSet ds = Wcf.CreateTestDataSet();
            asmxClient.ProcessDataSet(ds);
            asmxClient.ProcessDataSetWithMoreData(ds, 1);
            AsmxProxy.AsmxCompoundWithDataSet compoundWithDataSet = new AsmxProxy.AsmxCompoundWithDataSet();
            compoundWithDataSet.Data = ds;
            asmxClient.ProcessCompoundDataSet(compoundWithDataSet);
            asmxClient.ProcessTypedDataSet(CreateTestAsmxTypedDataSet());
            AsmxProxy.AsmxCompoundWithTypedDataSet compoundWithTypedDataSet = new AsmxProxy.AsmxCompoundWithTypedDataSet();
            compoundWithTypedDataSet.Data = CreateTestAsmxTypedDataSet();
            asmxClient.ProcessCompoundTypedDataSet(compoundWithTypedDataSet);
        }

        /// <summary>
        /// Creates an <see cref="AsmxTypedDataset"/> for testing.
        /// </summary>
        /// <returns>The dataset.</returns>
        private static AsmxProxy.AsmxTypedDataSet CreateTestAsmxTypedDataSet()
        {
            AsmxProxy.AsmxTypedDataSet ans = new AsmxProxy.AsmxTypedDataSet();

            ans.DataTable1.AddDataTable1Row("hello1", 1, new DateTime(2010, 6, 29));
            ans.DataTable1.AddDataTable1Row("hello2", 2, new DateTime(2010, 5, 29));
            ans.DataTable1.Rows.Add(DBNull.Value, DBNull.Value, DBNull.Value);

            return ans;
        }
    }
}
