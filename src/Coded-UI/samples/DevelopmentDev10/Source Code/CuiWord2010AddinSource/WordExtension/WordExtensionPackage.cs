//-----------------------------------------------------------------------
// <copyright file="WordExtensionPackage.cs" company="Microsoft">(c) Microsoft ALM Rangers This source is subject to the Microsoft Permissive License. See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx. All other rights reserved.</copyright>
//-----------------------------------------------------------------------
// Assembly attribute to denote that this assembly has UITest extensions.
[assembly: Microsoft.VisualStudio.TestTools.UITest.Extension.UITestExtensionPackage("WordExtensionPackage", typeof(Microsoft.ALMRangers.UITest.Extension.Word.WordExtensionPackage))]

namespace Microsoft.ALMRangers.UITest.Extension.Word
{
    using System;
    using Microsoft.VisualStudio.TestTools.UITest.Extension;
    using Microsoft.VisualStudio.TestTools.UITesting;

    /// <summary>
    /// Entry class for Word extension package.
    /// </summary>
    internal class WordExtensionPackage : UITestExtensionPackage
    {
        // Create and cache service.
        private readonly WordTechnologyManager technologyManager = new WordTechnologyManager();
        private readonly WordPropertyProvider propertyProvider = new WordPropertyProvider();

        /// <summary>
        /// Gets the short description of the package.
        /// </summary>
        public override string PackageDescription
        {
            get { return "Add-in for VSTT Record and Playback support on Word"; }
        }

        /// <summary>
        /// Gets the name of the package.
        /// </summary>
        public override string PackageName
        {
            get { return "VSTT Word Add-in"; }
        }

        /// <summary>
        /// Gets the name of the vendor of the package.
        /// </summary>
        public override string PackageVendor
        {
            get { return "Microsoft Corporation"; }
        }

        /// <summary>
        /// Gets the version of the package.
        /// </summary>
        public override Version PackageVersion
        {
            get { return new Version(1, 0); }
        }

        /// <summary>
        /// Gets the version of Visual Studio supported by this package.
        /// </summary>
        public override Version VSVersion
        {
            get { return new Version(10, 0); }
        }

        /// <summary>
        /// Gets the service object of the specified type.
        /// </summary>
        /// <param name="serviceType">An object that specifies the type of service object to get.</param>
        /// <returns>
        /// A service object of type serviceType or null if there is no service object of type serviceType.
        /// </returns>
        public override object GetService(Type serviceType)
        {
            // Return appropriate service.
            if (serviceType == typeof(UITechnologyManager))
            {
                return this.technologyManager;
            }

            if (serviceType == typeof(UITestPropertyProvider))
            {
                return this.propertyProvider;
            }

            // else if (serviceType == typeof(UITestActionFilter))
            // {
            //    return actionFilter;
            // }
            return null;
        }

        /// <summary>
        /// Performs application-defined tasks of cleaning up resources.
        /// </summary>
        public override void Dispose()
        {
            // nothing to cleanup
        }
    }
}
