//---------------------------------------------------------------------
// <copyright file="ScenarioRunManagerTests.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
//     THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
//     OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
//     LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR
//     FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
// <summary>The ScenarioRunManagerTests type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.WcfUnit.Library.Test.Integration
{
    using System.Collections.Generic;
    using System.IO;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.WcfUnit.Library;

    [TestClass]
    public class ScenarioRunManagerTests
    {
        /// <summary>
        /// This test requires the test service to be running
        /// </summary>
        [TestMethod]
        public void SRMRunClient()
        {
            Assert.Inconclusive("Disabled because this test requires web service to be running and will otherwise fail");

            // Don't use global variables for exe and config file as those files will get deleted at the end of the test
            string exePath = Path.GetFullPath(@"..\..\..\SampleClientAndService\Client\bin\debug\client.exe");
            string libPath = Path.GetFullPath(@"..\..\..\SampleClientAndService\Client\bin\debug\ClientProxies.dll");
            using (ScenarioRunManager mgr = new ScenarioRunManager())
            {
                mgr.Initialize(exePath);
                string logFileName = null;
                try
                {
                    logFileName = mgr.SetupForTrace();
                    WcfUnitConfiguration config = new WcfUnitConfiguration();
                    mgr.Run(config);
                    Assert.IsTrue(File.Exists(logFileName));
                    Assert.AreEqual<int>(39, config.soapActions.soapAction.Length);
                    Assert.AreEqual<string>("http://contoso.com/service/test/IArithmetic/Add", config.soapActions.soapAction[0].action);
                    Assert.AreEqual<string>("http://contoso.com/service/test/IArithmetic/Add2", config.soapActions.soapAction[1].action);
                    Assert.AreEqual<string>("http://contoso.com/service/test/IArithmetic/Add3", config.soapActions.soapAction[2].action);
                    Assert.AreEqual<string>("http://contoso.com/service/test/IArithmetic/WrappedAdd", config.soapActions.soapAction[3].action);
                    Assert.AreEqual<string>("http://contoso.com/service/test/IArithmetic/WrappednoCustomNamesAdd", config.soapActions.soapAction[4].action);
                    Assert.AreEqual<string>("http://contoso.com/service/test/IArithmetic/WrappedMessageWithHeaderAndBodyNamespaceOverrides", config.soapActions.soapAction[5].action);
                    Assert.AreEqual<string>("http://contoso.com/service/test/IArithmetic/UnwrappedAdd", config.soapActions.soapAction[6].action);
                    Assert.AreEqual<string>("http://contoso.com/service/test/IArithmetic/NoParameters", config.soapActions.soapAction[7].action);
                    Assert.AreEqual<string>("http://contoso.com/service/test/IArithmetic/RefParameter", config.soapActions.soapAction[8].action);
                    Assert.AreEqual<string>("http://contoso.com/service/test/IArithmetic/OutParameter", config.soapActions.soapAction[9].action);
                    Assert.AreEqual<string>("http://contoso.com/service/test/IArithmetic/MixedDirectionParameters", config.soapActions.soapAction[10].action);
                    Assert.AreEqual<string>("http://contoso.com/service/test/ICollections/ProcessCollection", config.soapActions.soapAction[11].action);
                    Assert.AreEqual<string>("http://tempuri.org/ICustomContracts/Overload", config.soapActions.soapAction[12].action);
                    Assert.AreEqual<string>("http://tempuri.org/ICustomContracts/Overload2", config.soapActions.soapAction[13].action);
                    Assert.AreEqual<string>("http://tempuri.org/ICustomContracts/Hidden", config.soapActions.soapAction[14].action);
                    Assert.AreEqual<string>("http://tempuri.org/ICustomContracts2/Contract2Method", config.soapActions.soapAction[15].action);
                    Assert.AreEqual<string>("http://contoso.com/service/test/IBufferedStreamService/BufferedStreamOperation", config.soapActions.soapAction[16].action);
                    Assert.AreEqual<string>("http://contoso.com/service/test/IBufferedStreamService/BufferedMemoryStreamOperation", config.soapActions.soapAction[17].action);
                    Assert.AreEqual<string>("http://contoso.com/service/test/IStreamedStreamService/StreamedStreamOperation", config.soapActions.soapAction[18].action);
                    Assert.AreEqual<string>("http://contoso.com/service/test/IStreamedStreamService/StreamedMemoryStreamOperation", config.soapActions.soapAction[19].action);
                    Assert.AreEqual<string>("http://contoso.com/service/test/IStreamedStreamService/StreamedWithNonStreamParametersOperation", config.soapActions.soapAction[20].action);
                    Assert.AreEqual<string>("http://contoso.com/service/test/IBufferedStreamService/BufferedUnwrappedMessageWithMemoryStream", config.soapActions.soapAction[21].action);
                    Assert.AreEqual<string>("http://contoso.com/service/test/IBufferedStreamService/BufferedUnwrappedMessageWithStream", config.soapActions.soapAction[22].action);
                    Assert.AreEqual<string>("http://contoso.com/service/test/IBufferedStreamService/BufferedWrappedMessageWithMemoryStream", config.soapActions.soapAction[23].action);
                    Assert.AreEqual<string>("http://contoso.com/service/test/IBufferedStreamService/BufferedWrappedMessageWithStream", config.soapActions.soapAction[24].action);
                    Assert.AreEqual<string>("http://contoso.com/service/test/IStreamedStreamService/StreamedUnwrappedMessageWithMemoryStream", config.soapActions.soapAction[25].action);
                    Assert.AreEqual<string>("http://contoso.com/service/test/IStreamedStreamService/StreamedUnwrappedMessageWithStream", config.soapActions.soapAction[26].action);
                    Assert.AreEqual<string>("http://contoso.com/service/test/IStreamedStreamService/StreamedWrappedMessageWithMemoryStream", config.soapActions.soapAction[27].action);
                    Assert.AreEqual<string>("http://contoso.com/service/test/IStreamedStreamService/StreamedWrappedMessageWithStream", config.soapActions.soapAction[28].action);
                    Assert.AreEqual<string>("http://contoso.com/service/test/IShapeService/DoSomething", config.soapActions.soapAction[29].action);
                    Assert.AreEqual<string>("http://contoso.com/asmxservice/test/ProcessSimpleAsmxRequestWrapped", config.soapActions.soapAction[30].action);
                    Assert.AreEqual<string>("http://contoso.com/asmxservice/test/ProcessSimpleAsmxRequestBare", config.soapActions.soapAction[31].action);
                    Assert.AreEqual<string>("http://contoso.com/asmxservice/test/ProcessMultipleParametersWrapped", config.soapActions.soapAction[32].action);
                    Assert.AreEqual<string>("http://contoso.com/asmxservice/test/WithNullableInt", config.soapActions.soapAction[33].action);
                    Assert.AreEqual<string>("http://contoso.com/asmxservice/test/SimpleTypes", config.soapActions.soapAction[34].action);
                    Assert.AreEqual<string>("http://contoso.com/asmxservice/test/ScalarArray", config.soapActions.soapAction[35].action);
                    Assert.AreEqual<string>("http://contoso.com/asmxservice/test/XmlRequestMethod", config.soapActions.soapAction[36].action);
                    Assert.AreEqual<string>("http://contoso.com/asmxservice/test/CollectionMethod", config.soapActions.soapAction[37].action);
                    Assert.AreEqual<string>("http://contoso.com/asmxservice/test/ProcessShape", config.soapActions.soapAction[38].action);

                    foreach (SoapActionType sat in config.soapActions.soapAction)
                    {
                        Assert.IsTrue(sat.Selected);
                    }

                    Assert.Inconclusive("The rest of this test is currently disabled because the functionality to autodiscover the referenced assemblies has been temporarily removed to make sure the user specifies the proxy assembly, because the tool cannot yet cope with proxy-less clients");
                    List<string> assemblies = new List<string>();
                    foreach (AssemblyType a in config.assembly)
                    {
                        assemblies.Add(a.fileName);
                    }

                    Assert.AreEqual<int>(2, assemblies.Count);
                    Assert.IsTrue(assemblies.Contains(exePath));
                    Assert.IsTrue(assemblies.Contains(libPath));
                }
                finally
                {
                    mgr.RestoreOriginalConfiguration();
                    if (logFileName != null)
                    {
                        File.Delete(logFileName);
                    }
                }
            }
        }
    }
}
