//---------------------------------------------------------------------
// <copyright file="IDefaultSoapActionWithContractNamespaceAndSlash.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
//     THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
//     OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
//     LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR
//     FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
// <summary>The IDefaultSoapActionWithContractNamespaceAndSlash type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.WcfUnit.Library.Test.TestContracts
{
    using System.ServiceModel;

    [ServiceContract(Namespace = "http://test.test/test/")]
    public interface IDefaultSoapActionWithContractNamespaceAndSlash
    {
        [OperationContract]
        void DefaultSoapActionWithContractNamespaceAndSlash();
    }
}
