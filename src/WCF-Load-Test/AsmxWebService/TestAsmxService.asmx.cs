//---------------------------------------------------------------------
// <copyright file="TestAsmxService.asmx.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
//     THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
//     OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
//     LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR
//     FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
// <summary>The TestAsmxService type.</summary>
//---------------------------------------------------------------------

namespace AsmxWebService
{
    using System;
    using System.ComponentModel;
    using System.Data;
    using System.Diagnostics;
    using System.Globalization;
    using System.Web.Services;
    using System.Web.Services.Protocols;

    /// <summary>
    /// Test web service.
    /// </summary>
    [WebService(Namespace = "http://contoso.com/asmxservice/test")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ToolboxItem(false)]
    public class TestAsmxService : System.Web.Services.WebService
    {
        /// <summary>
        /// Test method.
        /// </summary>
        /// <param name="r">Test parameter.</param>
        /// <returns>A test value.</returns>
        [WebMethod]
        [SoapDocumentMethod(ParameterStyle = SoapParameterStyle.Wrapped)]
        public string ProcessSimpleAsmxRequestWrapped(SimpleAsmxRequest r)
        {
            return r.B + r.A.ToString();
        }

        /// <summary>
        /// Test method.
        /// </summary>
        /// <param name="a">Test parameter a.</param>
        /// <param name="b">Test parameter b.</param>
        /// <returns>Test value.</returns>
        [WebMethod]
        [SoapDocumentMethod(ParameterStyle = SoapParameterStyle.Wrapped)]
        public string ProcessMultipleParametersWrapped(int a, string b)
        {
            return b + a.ToString();
        }

        /// <summary>
        /// Test method.
        /// </summary>
        /// <param name="r">Test parameter.</param>
        /// <returns>Test value.</returns>
        [WebMethod]
        [SoapDocumentMethod(ParameterStyle = SoapParameterStyle.Bare)]
        public string ProcessSimpleAsmxRequestBare(SimpleAsmxRequest r)
        {
            return r.B + r.A.ToString();
        }

        /// <summary>
        /// Test for simple types.
        /// </summary>
        /// <param name="i">The parameter is not used.</param>
        /// <param name="e">The parameter is not used.</param>
        /// <param name="s">The parameter is not used.</param>
        /// <param name="d">The parameter is not used.</param>
        /// <param name="dt">The parameter is not used.</param>
        /// <param name="g">The parameter is not used.</param>
        /// <param name="xqn">The parameter is not used.</param>
        /// <returns>An empty string.</returns>
        [WebMethod]
        public string SimpleTypes(int i, ConsoleColor e, string s, decimal d, DateTime dt, Guid g, System.Xml.XmlQualifiedName xqn)
        {
            return string.Empty;
        }

        /// <summary>
        /// Test method.
        /// </summary>
        /// <param name="r">Test parameter.</param>
        /// <returns>Test value.</returns>
        [WebMethod]
        public string WithNullableInt(Nullable<int> r)
        {
            string ans = string.Empty;
            if (r.HasValue)
            {
                ans = r.ToString();
            }

            return ans;
        }

        /// <summary>
        /// Test method.
        /// </summary>
        /// <param name="a">Test parameter.</param>
        /// <returns>Test value.</returns>
        [WebMethod]
        public int ScalarArray(int[] a)
        {
            return a.Length;
        }

        /// <summary>
        /// Test for a type containing XML.
        /// </summary>
        /// <param name="request">The object containing XML</param>
        [WebMethod]
        public void XmlRequestMethod(XmlAsmxRequest request)
        {
        }

        /// <summary>
        /// Test for a collection.
        /// </summary>
        /// <param name="data">The collection.</param>
        [WebMethod]
        public void CollectionMethod(CollectionsRequest data)
        {
            Trace.WriteLine(string.Format(CultureInfo.InvariantCulture, "Request Collection = {0}", data.RequestCollection.Count));
        }

        /// <summary>
        /// Test for a base type.
        /// </summary>
        /// <param name="shape">~The base type</param>
        [WebMethod]
        public void ProcessShape(AsmxShape shape)
        {
        }

        /// <summary>
        /// Test to see if the XmlIgnore in the base class is inherited.
        /// </summary>
        /// <param name="circle">The circle.</param>
        [WebMethod]
        public void ProcessCircle(AsmxCircle circle)
        {
        }

        /// <summary>
        /// Test method for datasets.
        /// </summary>
        /// <param name="a">Test parameter.</param>
        /// <returns>Test value.</returns>
        [WebMethod]
        public DataSet ProcessDataSet(DataSet a)
        {
            return a;
        }

        /// <summary>
        /// Test for a DataSet with another parameter.
        /// </summary>
        /// <param name="a">The DataSet.</param>
        /// <param name="i">The other parameter.</param>
        /// <returns>A DataSet.</returns>
        [WebMethod]
        public DataSet ProcessDataSetWithMoreData(DataSet a, int i)
        {
            return a;
        }

        /// <summary>
        /// Test for a typed DataSet.
        /// </summary>
        /// <param name="data">The typed DataSet.</param>
        [WebMethod]
        public void ProcessTypedDataSet(AsmxTypedDataSet data)
        {
        }

        /// <summary>
        /// Test for a compound type with a DataSet.
        /// </summary>
        /// <param name="data">The compound type with a DataSet.</param>
        [WebMethod]
        public void ProcessCompoundDataSet(AsmxCompoundWithDataSet data)
        {
        }

        /// <summary>
        /// Test for a compound type with a typed DataSet.
        /// </summary>
        /// <param name="data">The compound type with a typed DataSet.</param>
        [WebMethod]
        public void ProcessCompoundTypedDataSet(AsmxCompoundWithTypedDataSet data)
        {
        }
    }
}
