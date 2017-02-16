//---------------------------------------------------------------------
// <copyright file="IXmlSerializerService.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
//     THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
//     OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
//     LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR
//     FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
// <summary>The IXmlSerializerService type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.WcfUnit.Library.Test.TestContracts
{
    using System.ServiceModel;

    [ServiceContract]
    public interface IXmlSerializerService
    {
        [System.ServiceModel.OperationContractAttribute(Action = "http://tempuri.org/HelloWorldXmlSerializerRequest", ReplyAction = "*")]
        [System.ServiceModel.XmlSerializerFormatAttribute]
        string SimpleXmlSerializerRequest(XmlSerializerRequest r);
    }
}
