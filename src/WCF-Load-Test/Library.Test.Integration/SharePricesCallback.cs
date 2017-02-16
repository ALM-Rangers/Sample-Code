//---------------------------------------------------------------------
// <copyright file="SharePricesCallback.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
//     THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
//     OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
//     LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR
//     FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
// <summary>The SharePricesCallback type.</summary>
//---------------------------------------------------------------------

namespace Library.Test.Integration
{
    using System;
    using System.Threading;

    /// <summary>
    /// Dummy class for share price test callbacks.
    /// </summary>
    public class SharePricesCallback : GeneratedContracts.ISharePricesCallback, Contracts.ISharePriceCallback
    {
        /// <summary>
        /// Wait handle used to indicate when completed receiving updates.
        /// </summary>
        private static AutoResetEvent done = new AutoResetEvent(false);

        /// <summary>
        /// Gets the wait handle used to indicate when completed receiving updates.
        /// </summary>
        public static AutoResetEvent Done
        {
            get
            {
                return done;
            }
        }

        #region ISharePricesCallback Members

        /// <summary>
        /// Notifies a price for a share in a one-way communication.
        /// </summary>
        /// <param name="symbol">The symbol of the share.</param>
        /// <param name="price1">The new price of the share.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1725:ParameterNamesShouldMatchBaseDeclaration", MessageId = "1#", Justification = "May affect recorded log files so not changing this")]
        public void PriceOneWay(string symbol, float price1)
        {
            Console.WriteLine("New price for {0} is {1} (one-way)", symbol, price1);
            if (price1 < 0)
            {
                done.Set();
            }
        }

        /// <summary>
        /// Notifies a price for a share in a two-way communication.
        /// </summary>
        /// <param name="symbol">The symbol of the share.</param>
        /// <param name="price1">The new price of the share.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1725:ParameterNamesShouldMatchBaseDeclaration", MessageId = "1#", Justification = "May affect recorded log files so not changing this")]
        public void PriceTwoWay(string symbol, float price1)
        {
            Console.WriteLine("New price for {0} is {1} (two-way)", symbol, price1);
            if (price1 < 0)
            {
                done.Set();
            }
        }

        #endregion
    }
}
