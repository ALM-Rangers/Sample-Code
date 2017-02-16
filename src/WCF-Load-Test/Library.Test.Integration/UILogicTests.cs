//---------------------------------------------------------------------
// <copyright file="UILogicTests.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
//     THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
//     OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
//     LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR
//     FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
// <summary>The UILogicTests type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.WcfUnit.Library.Test.Integration
{
    using System.Collections.Generic;
    using System.IO;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.WcfUnit.Library;

    [TestClass]
    public class UILogicTests
    {
        [TestMethod]
        public void UIRunClient()
        {
            Assert.Inconclusive("Disabled because this test requires web service to be running and will otherwise fail");

            string exePath = Path.GetFullPath(@"..\..\..\SampleClientAndService\Client\bin\debug\client.exe");
            string libPath = Path.GetFullPath(@"..\..\..\SampleClientAndService\Client\bin\debug\ClientProxies.dll");
            WizardData data = new WizardData();
            data.Configuration = new WcfUnitConfiguration();
            UILogic uiLogic = new UILogic();
            using (ScenarioRunManager srm = new ScenarioRunManager())
            {
                uiLogic.RunProgramAndGetWizardData(exePath, srm, data);
                Assert.IsTrue(File.Exists(data.TraceFile));
                Assert.AreEqual<int>(39, data.Configuration.soapActions.soapAction.Length);
                Assert.AreEqual<string>("http://contoso.com/service/test/IArithmetic/Add", data.Configuration.soapActions.soapAction[0].action);
                Assert.AreEqual<string>("http://contoso.com/service/test/IArithmetic/Add2", data.Configuration.soapActions.soapAction[1].action);
                Assert.AreEqual<string>("http://contoso.com/service/test/IArithmetic/Add3", data.Configuration.soapActions.soapAction[2].action);
                Assert.AreEqual<string>("http://contoso.com/service/test/IArithmetic/WrappedAdd", data.Configuration.soapActions.soapAction[3].action);
                Assert.AreEqual<string>("http://contoso.com/service/test/IArithmetic/WrappednoCustomNamesAdd", data.Configuration.soapActions.soapAction[4].action);
                Assert.AreEqual<string>("http://contoso.com/service/test/IArithmetic/WrappedMessageWithHeaderAndBodyNamespaceOverrides", data.Configuration.soapActions.soapAction[5].action);
                Assert.AreEqual<string>("http://contoso.com/service/test/IArithmetic/UnwrappedAdd", data.Configuration.soapActions.soapAction[6].action);
                Assert.AreEqual<string>("http://contoso.com/service/test/IArithmetic/NoParameters", data.Configuration.soapActions.soapAction[7].action);
                Assert.AreEqual<string>("http://contoso.com/service/test/IArithmetic/RefParameter", data.Configuration.soapActions.soapAction[8].action);
                Assert.AreEqual<string>("http://contoso.com/service/test/IArithmetic/OutParameter", data.Configuration.soapActions.soapAction[9].action);
                Assert.AreEqual<string>("http://contoso.com/service/test/IArithmetic/MixedDirectionParameters", data.Configuration.soapActions.soapAction[10].action);
                Assert.AreEqual<string>("http://contoso.com/service/test/ICollections/ProcessCollection", data.Configuration.soapActions.soapAction[11].action);
                Assert.AreEqual<string>("http://tempuri.org/ICustomContracts/Overload", data.Configuration.soapActions.soapAction[12].action);
                Assert.AreEqual<string>("http://tempuri.org/ICustomContracts/Overload2", data.Configuration.soapActions.soapAction[13].action);
                Assert.AreEqual<string>("http://tempuri.org/ICustomContracts/Hidden", data.Configuration.soapActions.soapAction[14].action);
                Assert.AreEqual<string>("http://tempuri.org/ICustomContracts2/Contract2Method", data.Configuration.soapActions.soapAction[15].action);
                Assert.AreEqual<string>("http://contoso.com/service/test/IBufferedStreamService/BufferedStreamOperation", data.Configuration.soapActions.soapAction[16].action);
                Assert.AreEqual<string>("http://contoso.com/service/test/IBufferedStreamService/BufferedMemoryStreamOperation", data.Configuration.soapActions.soapAction[17].action);
                Assert.AreEqual<string>("http://contoso.com/service/test/IStreamedStreamService/StreamedStreamOperation", data.Configuration.soapActions.soapAction[18].action);
                Assert.AreEqual<string>("http://contoso.com/service/test/IStreamedStreamService/StreamedMemoryStreamOperation", data.Configuration.soapActions.soapAction[19].action);
                Assert.AreEqual<string>("http://contoso.com/service/test/IStreamedStreamService/StreamedWithNonStreamParametersOperation", data.Configuration.soapActions.soapAction[20].action);
                Assert.AreEqual<string>("http://contoso.com/service/test/IBufferedStreamService/BufferedUnwrappedMessageWithMemoryStream", data.Configuration.soapActions.soapAction[21].action);
                Assert.AreEqual<string>("http://contoso.com/service/test/IBufferedStreamService/BufferedUnwrappedMessageWithStream", data.Configuration.soapActions.soapAction[22].action);
                Assert.AreEqual<string>("http://contoso.com/service/test/IBufferedStreamService/BufferedWrappedMessageWithMemoryStream", data.Configuration.soapActions.soapAction[23].action);
                Assert.AreEqual<string>("http://contoso.com/service/test/IBufferedStreamService/BufferedWrappedMessageWithStream", data.Configuration.soapActions.soapAction[24].action);
                Assert.AreEqual<string>("http://contoso.com/service/test/IStreamedStreamService/StreamedUnwrappedMessageWithMemoryStream", data.Configuration.soapActions.soapAction[25].action);
                Assert.AreEqual<string>("http://contoso.com/service/test/IStreamedStreamService/StreamedUnwrappedMessageWithStream", data.Configuration.soapActions.soapAction[26].action);
                Assert.AreEqual<string>("http://contoso.com/service/test/IStreamedStreamService/StreamedWrappedMessageWithMemoryStream", data.Configuration.soapActions.soapAction[27].action);
                Assert.AreEqual<string>("http://contoso.com/service/test/IStreamedStreamService/StreamedWrappedMessageWithStream", data.Configuration.soapActions.soapAction[28].action);
                Assert.AreEqual<string>("http://contoso.com/service/test/IShapeService/DoSomething", data.Configuration.soapActions.soapAction[29].action);
                Assert.AreEqual<string>("http://contoso.com/asmxservice/test/ProcessSimpleAsmxRequestWrapped", data.Configuration.soapActions.soapAction[30].action);
                Assert.AreEqual<string>("http://contoso.com/asmxservice/test/ProcessSimpleAsmxRequestBare", data.Configuration.soapActions.soapAction[31].action);
                Assert.AreEqual<string>("http://contoso.com/asmxservice/test/ProcessMultipleParametersWrapped", data.Configuration.soapActions.soapAction[32].action);
                Assert.AreEqual<string>("http://contoso.com/asmxservice/test/WithNullableInt", data.Configuration.soapActions.soapAction[33].action);
                Assert.AreEqual<string>("http://contoso.com/asmxservice/test/SimpleTypes", data.Configuration.soapActions.soapAction[34].action);
                Assert.AreEqual<string>("http://contoso.com/asmxservice/test/ScalarArray", data.Configuration.soapActions.soapAction[35].action);
                Assert.AreEqual<string>("http://contoso.com/asmxservice/test/XmlRequestMethod", data.Configuration.soapActions.soapAction[36].action);
                Assert.AreEqual<string>("http://contoso.com/asmxservice/test/CollectionMethod", data.Configuration.soapActions.soapAction[37].action);
                Assert.AreEqual<string>("http://contoso.com/asmxservice/test/ProcessShape", data.Configuration.soapActions.soapAction[38].action);

                Assert.Inconclusive("The rest of this test is currently disabled because the functionality to autodiscover the referenced assemblies has been temporarily removed to make sure the user specifies the proxy assembly, because the tool cannot yet cope with proxy-less clients");
                List<string> assemblies = new List<string>();
                foreach (AssemblyType a in data.Configuration.assembly)
                {
                    assemblies.Add(a.fileName);
                }

                Assert.AreEqual<int>(2, assemblies.Count);
                Assert.IsTrue(assemblies.Contains(exePath));
                Assert.IsTrue(assemblies.Contains(libPath));
            }
        }
    }
}
