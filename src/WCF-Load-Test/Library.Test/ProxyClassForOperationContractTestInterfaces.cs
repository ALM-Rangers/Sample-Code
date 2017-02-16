// -----------------------------------------------------------------------
// <copyright file="ProxyClassForOperationContractTestInterfaces.cs" company="Microsoft">
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

    /// <summary>Summary description for <see cref="ProxyClassForOperationContractTestInterfaces"/> class.</summary>
    public class ProxyClassForOperationContractTestInterfaces : IOperationContractWithoutServiceContract, IDefaultSoapActionWithContractNamespace, IDefaultSoapActionWithContractNamespaceAndSlash, IDefaultSoapActionWithDefaultContractNamespace, IContractWithOverloadedMethods, ICommunicationObject
    {
        #region IOperationContractWithoutServiceContract Members

        /// <summary>Summary description for <c>ProxyClassForOperationContractTestInterfaces</c> method.</summary>
        public void OperationContractWithoutServiceContract()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion

        #region IDefaultSoapActionWithContractNamespace Members

        /// <summary>Summary description for <c>ProxyClassForOperationContractTestInterfaces</c> method.</summary>
        public void DefaultSoapActionWithContractNamespace()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion

        #region IDefaultSoapActionWithContractNamespaceAndSlash Members

        /// <summary>Summary description for <c>ProxyClassForOperationContractTestInterfaces</c> method.</summary>
        public void DefaultSoapActionWithContractNamespaceAndSlash()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion

        #region IDefaultSoapActionWithDefaultContractNamespace Members

        /// <summary>Summary description for <c>ProxyClassForOperationContractTestInterfaces</c> method.</summary>
        public void DefaultSoapActionWithDefaultContractNamespace()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion

        #region IContractWithOverloadedMethods Members

        /// <summary>Summary description for <c>ProxyClassForOperationContractTestInterfaces</c> method.</summary>
        /// <param name="a">TODO: Summary description for <c>a</c> parameter of type <c>int</c>.</param>
        public void Overload(int a)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        /// <summary>Summary description for <c>ProxyClassForOperationContractTestInterfaces</c> method.</summary>
        /// <param name="a">TODO: Summary description for <c>a</c> parameter of type <c>string</c>.</param>
        public void Overload(string a)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion

        #region ICommunicationObject Members

        /// <summary>Summary description for <c>ProxyClassForOperationContractTestInterfaces</c> method.</summary>
        public void Abort()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        /// <summary>Summary description for <c>ProxyClassForOperationContractTestInterfaces</c> method.</summary>
        /// <param name="timeout">TODO: Summary description for <c>timeout</c> parameter of type <c>System.TimeSpan</c>.</param>
        /// <param name="callback">TODO: Summary description for <c>callback</c> parameter of type <c>System.AsyncCallback</c>.</param>
        /// <param name="state">TODO: Summary description for <c>state</c> parameter of type <c>object</c>.</param>
        /// <returns>TODO: returns an object of type <c>System.IAsyncResult</c>.</returns>
        public IAsyncResult BeginClose(TimeSpan timeout, AsyncCallback callback, object state)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        /// <summary>Summary description for <c>ProxyClassForOperationContractTestInterfaces</c> method.</summary>
        /// <param name="callback">TODO: Summary description for <c>callback</c> parameter of type <c>System.AsyncCallback</c>.</param>
        /// <param name="state">TODO: Summary description for <c>state</c> parameter of type <c>object</c>.</param>
        /// <returns>TODO: returns an object of type <c>System.IAsyncResult</c>.</returns>
        public IAsyncResult BeginClose(AsyncCallback callback, object state)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        /// <summary>Summary description for <c>ProxyClassForOperationContractTestInterfaces</c> method.</summary>
        /// <param name="timeout">TODO: Summary description for <c>timeout</c> parameter of type <c>System.TimeSpan</c>.</param>
        /// <param name="callback">TODO: Summary description for <c>callback</c> parameter of type <c>System.AsyncCallback</c>.</param>
        /// <param name="state">TODO: Summary description for <c>state</c> parameter of type <c>object</c>.</param>
        /// <returns>TODO: returns an object of type <c>System.IAsyncResult</c>.</returns>
        public IAsyncResult BeginOpen(TimeSpan timeout, AsyncCallback callback, object state)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        /// <summary>Summary description for <c>ProxyClassForOperationContractTestInterfaces</c> method.</summary>
        /// <param name="callback">TODO: Summary description for <c>callback</c> parameter of type <c>System.AsyncCallback</c>.</param>
        /// <param name="state">TODO: Summary description for <c>state</c> parameter of type <c>object</c>.</param>
        /// <returns>TODO: returns an object of type <c>System.IAsyncResult</c>.</returns>
        public IAsyncResult BeginOpen(AsyncCallback callback, object state)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        /// <summary>Summary description for <c>ProxyClassForOperationContractTestInterfaces</c> method.</summary>
        /// <param name="timeout">TODO: Summary description for <c>timeout</c> parameter of type <c>System.TimeSpan</c>.</param>
        public void Close(TimeSpan timeout)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        /// <summary>Summary description for <c>ProxyClassForOperationContractTestInterfaces</c> method.</summary>
        public void Close()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        /// <summary>Summary description for <c>Closed</c> event.</summary>
        public event EventHandler Closed;

        /// <summary>Summary description for <c>Closing</c> event.</summary>
        public event EventHandler Closing;

        /// <summary>Summary description for <c>ProxyClassForOperationContractTestInterfaces</c> method.</summary>
        /// <param name="result">TODO: Summary description for <c>result</c> parameter of type <c>System.IAsyncResult</c>.</param>
        public void EndClose(IAsyncResult result)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        /// <summary>Summary description for <c>ProxyClassForOperationContractTestInterfaces</c> method.</summary>
        /// <param name="result">TODO: Summary description for <c>result</c> parameter of type <c>System.IAsyncResult</c>.</param>
        public void EndOpen(IAsyncResult result)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        /// <summary>Summary description for <c>Faulted</c> event.</summary>
        public event EventHandler Faulted;

        /// <summary>Summary description for <c>ProxyClassForOperationContractTestInterfaces</c> method.</summary>
        /// <param name="timeout">TODO: Summary description for <c>timeout</c> parameter of type <c>System.TimeSpan</c>.</param>
        public void Open(TimeSpan timeout)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        /// <summary>Summary description for <c>ProxyClassForOperationContractTestInterfaces</c> method.</summary>
        public void Open()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        /// <summary>Summary description for <c>Opened</c> event.</summary>
        public event EventHandler Opened;

        /// <summary>Summary description for <c>Opening</c> event.</summary>
        public event EventHandler Opening;

        /// <summary>Gets the value for State.</summary>
        public CommunicationState State
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        #endregion
    }
}
