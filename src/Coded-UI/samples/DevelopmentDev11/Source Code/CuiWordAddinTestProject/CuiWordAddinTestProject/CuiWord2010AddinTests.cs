//-----------------------------------------------------------------------
// <copyright file="CuiWord2010AddinTests.cs" company="Microsoft">© ALM | DevOps Ranger Contributors This source is subject to the Microsoft Permissive License. See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx. All other rights reserved.</copyright>
//-----------------------------------------------------------------------

using System.IO;

namespace CuiWordAddinTestProject
{
    using Microsoft.VisualStudio.TestTools.UITesting;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// CuiWord2010AddinTests
    /// </summary>
    [CodedUITest]
    public class CuiWord2010AddinTests
    {
        private UIMap map;

        /// <summary>
        /// Gets or sets the test context which provides
        /// information about and functionality for the current test run.
        /// </summary>
        public TestContext TestContext { get; set; }

        public UIMap UIMap
        {
            get { return this.map ?? (this.map = new UIMap()); }
        }

        [TestMethod]
        public void RecordAndSelectText()
        {
            // To generate code for this test, select "Generate Code for Coded UI Test" from the shortcut menu and select one of the menu items.
            EnterSelectTextParams p = new EnterSelectTextParams();
            ApplicationUnderTest.Launch(p.ExePath, p.AlternateExePath);
            this.UIMap.EnterSelectText();
            this.UIMap.AssertText();

            // For more information on generated code, see http://go.microsoft.com/fwlink/?LinkId=179463
        }
    }
}
