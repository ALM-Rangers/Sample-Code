//-----------------------------------------------------------------------
// <copyright file="ThisAddIn.cs" company="Microsoft">(c) Microsoft ALM Rangers This source is subject to the Microsoft Permissive License. See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx. All other rights reserved.</copyright>
//-----------------------------------------------------------------------
namespace Microsoft.ALMRangers.UITest.Extension.Word
{
    using System.Runtime.Remoting;
    using System.Runtime.Remoting.Channels;
    using System.Runtime.Remoting.Channels.Ipc;

    /// <summary>
    /// ThisAddIn
    /// </summary>
    public partial class ThisAddIn
    {
        /// <summary>
        /// The channel for .NET Remoting calls.
        /// </summary>
        private IChannel channel;

        /// <summary>
        /// Singleton instance to this add-in.
        /// </summary>
        internal static ThisAddIn Instance
        {
            get;
            private set;
        }

        /// <summary>
        /// Register for .NET Remoting on startup of this add-in.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        private void ThisAddIn_Startup(object sender, System.EventArgs e)
        {
            Instance = this;
            this.channel = new IpcChannel("WordUITest");
            ChannelServices.RegisterChannel(this.channel, false);
            RemotingConfiguration.RegisterWellKnownServiceType(typeof(UITestCommunicator), "WordUITest", WellKnownObjectMode.Singleton);
        }

        /// <summary>
        /// Unregister for .NET Remoting on shutdown of this add-in.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        private void ThisAddIn_Shutdown(object sender, System.EventArgs e)
        {
            if (this.channel != null)
            {
                ChannelServices.UnregisterChannel(this.channel);
            }
        }

        #region VSTO generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InternalStartup()
        {
            this.Startup += this.ThisAddIn_Startup;
            this.Shutdown += this.ThisAddIn_Shutdown;
        }
        
        #endregion
    }
}
