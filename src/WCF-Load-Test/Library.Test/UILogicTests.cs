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
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.WcfUnit.Library;

    [TestClass]
    public class UILogicTests
    {
        private UILogic uiLogic;

        [TestInitialize]
        public void Initialize()
        {
            this.uiLogic = new UILogic();
        }

        [TestMethod]
        public void UIParseTraceFileChecksConfigParameterForNull()
        {
            TestHelper.TestForArgumentNullException(() => this.uiLogic.ParseTraceFile("abc", null), "config");
        }

        [TestMethod]
        public void UIParseClientTraceFileReturnsASoapActionList()
        {
            WcfUnitConfiguration config = new WcfUnitConfiguration();
            config.clientTrace = true;
            config.serviceTrace = false;
            this.uiLogic.ParseTraceFile("WithMessageBodies.svclog", config);

            Assert.AreEqual<int>(3, config.soapActions.soapAction.Length);
            foreach (SoapActionType sat in config.soapActions.soapAction)
            {
                Assert.IsTrue(!string.IsNullOrEmpty(sat.action));
                Assert.IsTrue(sat.Selected);
            }
        }

        [TestMethod]
        public void UIParseServerTraceFileReturnsASoapActionList()
        {
            WcfUnitConfiguration config = new WcfUnitConfiguration();
            config.clientTrace = false;
            config.serviceTrace = true;
            this.uiLogic.ParseTraceFile("ServiceSideLog.svclog", config);

            Assert.AreEqual<int>(2, config.soapActions.soapAction.Length);
            foreach (SoapActionType sat in config.soapActions.soapAction)
            {
                Assert.IsTrue(!string.IsNullOrEmpty(sat.action));
                Assert.IsTrue(sat.Selected);
            }
        }
    }
}
