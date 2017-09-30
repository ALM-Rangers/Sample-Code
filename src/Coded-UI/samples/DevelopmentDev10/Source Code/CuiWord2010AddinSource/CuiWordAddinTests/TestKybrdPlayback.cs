//-----------------------------------------------------------------------
// <copyright file="TestKybrdPlayback.cs" company="Microsoft">© ALM | DevOps Ranger Contributors This source is subject to the Microsoft Permissive License. See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx. All other rights reserved.</copyright>
//-----------------------------------------------------------------------
namespace CodedUITestProject
{
    using Microsoft.VisualStudio.TestTools.UITesting;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Test a simple playback using the keyboard only (no mouse clicks)
    /// </summary>
    [CodedUITest]
    public class TestKybrdPlayback
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
        public void Test010_PluginKeyboardPlayback()
        {
            // To generate code for this test, select "Generate Code for Coded UI Test" from the shortcut menu and select one of the menu items.
            // For more information on generated code, see http://go.microsoft.com/fwlink/?LinkId=179463
            this.UIMap.PlayKeyboardBack();
            this.UIMap.HighlightKeyboardPlayback();
        }
    }
}
