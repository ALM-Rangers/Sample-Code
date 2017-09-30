//-----------------------------------------------------------------------
// <copyright file="WordCommunicator.cs" company="Microsoft">© ALM | DevOps Ranger Contributors This source is subject to the Microsoft Permissive License. See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx. All other rights reserved.</copyright>
//-----------------------------------------------------------------------
namespace Microsoft.ALMRangers.UITest.Extension.Word
{
    using System;

    /// <summary>
    /// Static class to manage the Word communication interface.
    /// </summary>
    internal static class WordCommunicator
    {
        /// <summary>
        /// Singleton interface used to communicate with the Word via .NET Remoting.
        /// </summary>
        private static IWordUITestCommunication wordCommunicator;

        /// <summary>
        /// Singleton interface used to communicate with the Word via .NET Remoting.
        /// </summary>
        internal static IWordUITestCommunication Instance
        {
            get { return wordCommunicator ?? (wordCommunicator = (IWordUITestCommunication)Activator.GetObject(typeof(IWordUITestCommunication), "ipc://WordUITest/WordUITest")); }
        }
    }
}
