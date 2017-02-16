//---------------------------------------------------------------------
// <copyright file="UTGTestContractProxyCommObject.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
//     THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
//     OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
//     LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR
//     FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
// <summary>The UTGTestContractProxyCommObject type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.WcfUnit.Library.Test.TestContracts
{
    using System;
    using System.ServiceModel;
    using Microsoft.WcfUnit.Library.Test.TestContracts;

    public class UTGTestContractProxyCommObject : IUTGTestContract, ICommunicationObject
    {
        public event EventHandler Opened;

        public event EventHandler Opening;

        public event EventHandler Faulted;

        public event EventHandler Closed;

        public event EventHandler Closing;

        public CommunicationState State
        {
            get { return 0; }
        }

        public void Operation()
        {
        }

        public void Abort()
        {
        }

        public IAsyncResult BeginClose(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return null;
        }

        public IAsyncResult BeginClose(AsyncCallback callback, object state)
        {
            return null;
        }

        public IAsyncResult BeginOpen(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return null;
        }

        public IAsyncResult BeginOpen(AsyncCallback callback, object state)
        {
            return null;
        }

        public void Close(TimeSpan timeout)
        {
        }

        public void Close()
        {
        }

        public void EndClose(IAsyncResult result)
        {
        }

        public void EndOpen(IAsyncResult result)
        {
        }

        public void Open(TimeSpan timeout)
        {
        }

        public void Open()
        {
        }
    }
}
