// -----------------------------------------------------------------------
// <copyright file="CustomContracts2.cs" company="Microsoft">
//      Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace ClientProxies.Custom
{
    using System;
    using System.Collections.Generic;
    using System.ServiceModel;
    using System.Text;
    using Contracts.Custom;

    /// <summary>Summary description for <see cref="CustomContracts2"/> class.</summary>
    public class CustomContracts2 : ICustomContracts2
    {
        #region ICustomContracts2 Members

        /// <summary>Summary description for <c>CustomContracts2</c> method.</summary>
        public void Contract2Method()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion
    }
}
