//-----------------------------------------------------------------------
// <copyright file="WordExtensionPackage.cs" company="ALM | DevOps Ranger Contributors"> 
// © ALM | DevOps Ranger Contributors 
 
 // MIT License 
 // 
 // Permission is hereby granted, free of charge, to any person obtaining a copy 
 // of this software and associated documentation files (the "Software"), to deal 
 // in the Software without restriction, including without limitation the rights 
 // to use, copy, modify, merge, publish, distribute, sublicense, and/or sell 
 // copies of the Software, and to permit persons to whom the Software is 
 // furnished to do so, subject to the following conditions: 
 // 
 // The above copyright notice and this permission notice shall be included in all 
 // copies or substantial portions of the Software. 
 // 
 // THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
 // IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
 // FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
 // AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
 // LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
 // OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE 
 // SOFTWARE. 
 // </copyright>
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
