//---------------------------------------------------------------------
// <copyright file="UTGTestContractClassProxyCommObject.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
//     THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
//     OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
//     LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR
//     FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
// <summary>The UTGTestContractClassProxyCommObject type.</summary>
//---------------------------------------------------------------------

namespace Microsoft.WcfUnit.Library.Test.TestContracts
{
    using System;
    using System.ServiceModel;

    [ServiceContract]
    public sealed class UTGTestContractClassProxyCommObject : UTGTestContractClass, ICommunicationObject
    {
        public event EventHandler Opened;

        public event EventHandler Opening;

        public event EventHandler Closed;

        public event EventHandler Closing;

        public event EventHandler Faulted;

        public CommunicationState State { get; private set; }

        void ICommunicationObject.Abort()
        {
            throw new NotImplementedException();
        }

        IAsyncResult ICommunicationObject.BeginClose(TimeSpan timeout, AsyncCallback callback, object state)
        {
            throw new NotImplementedException();
        }

        IAsyncResult ICommunicationObject.BeginClose(AsyncCallback callback, object state)
        {
            throw new NotImplementedException();
        }

        IAsyncResult ICommunicationObject.BeginOpen(TimeSpan timeout, AsyncCallback callback, object state)
        {
            throw new NotImplementedException();
        }

        IAsyncResult ICommunicationObject.BeginOpen(AsyncCallback callback, object state)
        {
            throw new NotImplementedException();
        }

        void ICommunicationObject.Close(TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        void ICommunicationObject.Close()
        {
            throw new NotImplementedException();
        }

        void ICommunicationObject.EndClose(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

        void ICommunicationObject.EndOpen(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

        void ICommunicationObject.Open(TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        void ICommunicationObject.Open()
        {
            throw new NotImplementedException();
        }
    }
}
