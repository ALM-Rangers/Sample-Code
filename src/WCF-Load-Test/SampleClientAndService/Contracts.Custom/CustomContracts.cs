//---------------------------------------------------------------------
// <copyright file="CustomContracts.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
//     THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
//     OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
//     LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR
//     FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
// <summary>The custom contracts.</summary>
//---------------------------------------------------------------------

namespace Contracts.Custom
{
    using System.ServiceModel;

    [ServiceContract]
    public interface ICustomContracts
    {
        [OperationContract]
        void Overload(int a);

        [OperationContract(Name = "Overload2")]
        void Overload(string a);

        [OperationContract]
        void Hidden(string a);
    }

    // The point of this one is that there is a class derived from it that is NOT a proxy,
    // to make sure that we generate code for the interface only.
    [ServiceContract]
    public interface ICustomContracts2
    {
        [OperationContract]
        void Contract2Method();
    }
}
