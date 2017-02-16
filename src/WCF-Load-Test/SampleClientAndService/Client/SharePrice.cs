//---------------------------------------------------------------------
// <copyright file="SharePrice.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
//     THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
//     OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
//     LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR
//     FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
// <summary>The SharePrice type.</summary>
//---------------------------------------------------------------------

namespace Client
{
    using System;
    using System.Threading;
    using GeneratedContracts;

    /// <summary>
    /// Used to receive share price callbacks.
    /// </summary>
    public class SharePrice : ISharePricesCallback
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
