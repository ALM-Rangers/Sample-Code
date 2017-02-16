// -----------------------------------------------------------------------
// <copyright file="ServiceContract2Class.cs" company="Microsoft">
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

    /// <summary>Summary description for <see cref="ServiceContract2Class"/> class.</summary>
    [ServiceContract]
    public class ServiceContract2Class
    {
        /// <summary>Summary description for <c>ServiceContract2Class</c> method.</summary>
        [OperationContract(Action = "http:/test.test/ServiceContract2Class/Operation")]
        public virtual void Operation()
        {
        }
    }
}
