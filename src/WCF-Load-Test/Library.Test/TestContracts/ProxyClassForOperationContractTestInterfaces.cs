//---------------------------------------------------------------------
// <copyright file="ProxyClassForOperationContractTestInterfaces.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
//     THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
//     OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
//     LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR
//     FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
// <summary>The ProxyClassForOperationContractTestInterfaces type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.WcfUnit.Library.Test.TestContracts
{
    using System;
    using System.ServiceModel;

    public class ProxyClassForOperationContractTestInterfaces : IOperationContractWithoutServiceContract, IDefaultSoapActionWithContractNamespace, IDefaultSoapActionWithContractNamespaceAndSlash, IDefaultSoapActionWithDefaultContractNamespace, IContractWithOverloadedMethods, ICommunicationObject
    {
        public event EventHandler Closed;

        public event EventHandler Closing;

        public event EventHandler Faulted;

        public event EventHandler Opened;

        public event EventHandler Opening;

        public CommunicationState State { get; set; }

        #region IOperationContractWithoutServiceContract Members

        public void OperationContractWithoutServiceContract()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IDefaultSoapActionWithContractNamespace Members

        public void DefaultSoapActionWithContractNamespace()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IDefaultSoapActionWithContractNamespaceAndSlash Members

        public void DefaultSoapActionWithContractNamespaceAndSlash()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IDefaultSoapActionWithDefaultContractNamespace Members

        public void DefaultSoapActionWithDefaultContractNamespace()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IContractWithOverloadedMethods Members

        public void Overload(int a)
        {
            throw new NotImplementedException();
        }

        public void Overload(string a)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region ICommunicationObject Members

        public void Abort()
        {
            throw new NotImplementedException();
        }

        public IAsyncResult BeginClose(TimeSpan timeout, AsyncCallback callback, object state)
        {
            throw new NotImplementedException();
        }

        public IAsyncResult BeginClose(AsyncCallback callback, object state)
        {
            throw new NotImplementedException();
        }

        public IAsyncResult BeginOpen(TimeSpan timeout, AsyncCallback callback, object state)
        {
            throw new NotImplementedException();
        }

        public IAsyncResult BeginOpen(AsyncCallback callback, object state)
        {
            throw new NotImplementedException();
        }

        public void Close(TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public void Close()
        {
            throw new NotImplementedException();
        }

        public void EndClose(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

        public void EndOpen(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

        public void Open(TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public void Open()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
