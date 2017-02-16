//---------------------------------------------------------------------
// <copyright file="IDataContractFormatServiceContract.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
//     THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
//     OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
//     LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR
//     FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
// <summary>The IDataContractFormatServiceContract type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.WcfUnit.Library.Test.TestContracts
{
    using System;
    using System.Collections.Generic;
    using System.ServiceModel;
    using System.Text;

    [DataContractFormat]
    public interface IDataContractFormatServiceContract
    {
        [XmlSerializerFormat]
        void Xml();

        void None();

        [DataContractFormat]
        void DataContract();
    }
}
