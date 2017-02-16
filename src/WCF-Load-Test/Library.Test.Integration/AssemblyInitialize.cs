//---------------------------------------------------------------------
// <copyright file="AssemblyInitialize.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
//     THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
//     OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
//     LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR
//     FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
// <summary>The AssemblyInitialize type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.WcfUnit.Library.Test
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Starts and stops hosting of actual WCF services.
    /// </summary>
    [TestClass]
    public static class AssemblyInitialize
    {
        [AssemblyInitialize]
        public static void InitializeAssembly(TestContext context)
        {
            Console.WriteLine("Assembly initialize");
            TestServiceHosting.StartHosts();
        }

        [AssemblyCleanup]
        public static void CleanupAssembly()
        {
            Console.WriteLine("Assembly cleanup");
            TestServiceHosting.StopHost();
        }
    }
}
