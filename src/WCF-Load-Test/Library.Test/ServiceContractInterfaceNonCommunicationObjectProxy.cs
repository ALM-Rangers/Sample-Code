// -----------------------------------------------------------------------
// <copyright file="ServiceContractInterfaceNonCommunicationObjectProxy.cs" company="Microsoft">
//      Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.WcfUnit.Library.Test
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Reflection;
    using System.ServiceModel;
    using System.ServiceModel.Description;
    using System.Text;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.WcfUnit.Library;

    /// <summary>Summary description for <see cref="ServiceContractInterfaceNonCommunicationObjectProxy"/> class.</summary>
    public class ServiceContractInterfaceNonCommunicationObjectProxy : IServiceContractInterface
    {
        #region IServiceContractInterface Members

        /// <summary>Summary description for <c>ServiceContractInterfaceNonCommunicationObjectProxy</c> method.</summary>
        public void Operation()
        {
        }

        #endregion
    }
}
