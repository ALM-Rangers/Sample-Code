// -----------------------------------------------------------------------
// <copyright file="ServiceContractClass.cs" company="Microsoft">
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

    /// <summary>Summary description for <see cref="ServiceContractClass"/> class.</summary>
    [ServiceContract]
    public class ServiceContractClass
    {
        /// <summary>Summary description for <c>ServiceContractClass</c> method.</summary>
        [OperationContract(Action = "http:/test.test/ServiceContractClass/Operation")]
        public virtual void Operation()
        {
        }
    }
}
