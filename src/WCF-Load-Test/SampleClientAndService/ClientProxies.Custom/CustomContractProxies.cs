//---------------------------------------------------------------------
// <copyright file="CustomContractProxies.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
//     THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
//     OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
//     LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR
//     FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
// <summary>The custom contract types.</summary>
//---------------------------------------------------------------------

namespace ClientProxies.Custom
{
    using System;
    using System.ServiceModel;
    using Contracts.Custom;

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Sample code for test use only")]
    public class CustomContracts : ICustomContracts, ICommunicationObject
    {
        private ChannelFactory<ICustomContracts> factory;
        private ICustomContracts proxy;

        public CustomContracts()
        {
            this.factory = new ChannelFactory<ICustomContracts>("Custom");
            this.proxy = this.factory.CreateChannel();
        }

        public event EventHandler Closed;

        public event EventHandler Closing;

        public event EventHandler Faulted;

        public event EventHandler Opened;

        public event EventHandler Opening;

        public CommunicationState State
        {
            get
            {
                return ((ICommunicationObject)this.proxy).State;
            }
        }

        // Method parameter deliberately named differently to the interface
        public void Overload(int aa)
        {
            this.proxy.Overload(aa);
        }

        // Method parameter deliberately named differently to the interface
        public void Overload(string aa)
        {
            this.proxy.Overload(aa);
        }

        void ICustomContracts.Hidden(string a)
        {
            this.proxy.Hidden(a);
        }

        public void Hidden()
        {
            ((ICustomContracts)this).Hidden("def");
        }
        
        public void Abort()
        {
            ((ICommunicationObject)this.proxy).Abort();
        }

        public IAsyncResult BeginClose(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return ((ICommunicationObject)this.proxy).BeginClose(timeout, callback, state);
        }

        public IAsyncResult BeginClose(AsyncCallback callback, object state)
        {
            return ((ICommunicationObject)this.proxy).BeginClose(callback, state);
        }

        public IAsyncResult BeginOpen(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return ((ICommunicationObject)this.proxy).BeginOpen(timeout, callback, state);
        }

        public IAsyncResult BeginOpen(AsyncCallback callback, object state)
        {
            return ((ICommunicationObject)this.proxy).BeginOpen(callback, state);
        }

        public void Close(TimeSpan timeout)
        {
            ((ICommunicationObject)this.proxy).Close(timeout);
        }

        public void Close()
        {
            ((ICommunicationObject)this.proxy).Close();
        }

        public void EndClose(IAsyncResult result)
        {
            ((ICommunicationObject)this.proxy).EndClose(result);
        }

        public void EndOpen(IAsyncResult result)
        {
            ((ICommunicationObject)this.proxy).EndOpen(result);
        }

        public void Open(TimeSpan timeout)
        {
            ((ICommunicationObject)this.proxy).Open(timeout);
        }

        public void Open()
        {
            ((ICommunicationObject)this.proxy).Open();
        }
    }

    // this is a decoy to test that code generated does not reference this class as it is not
    // a proxy.
    public class CustomContracts2 : ICustomContracts2
    {
        #region ICustomContracts2 Members

        public void Contract2Method()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion
    }
}
