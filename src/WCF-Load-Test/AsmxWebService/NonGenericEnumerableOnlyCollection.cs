//---------------------------------------------------------------------
// <copyright file="NonGenericEnumerableOnlyCollection.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
//     THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
//     OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
//     LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR
//     FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
// <summary>The NonGenericEnumerableOnlyCollection type.</summary>
//---------------------------------------------------------------------

namespace AsmxWebService
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    public class NonGenericEnumerableOnlyCollection : IEnumerable
    {
        private ArrayList array = new ArrayList();

        public void Add(object o)
        {
            this.array.Add(o);
        }
        #region IEnumerable Members

        public IEnumerator GetEnumerator()
        {
            return this.array.GetEnumerator();
        }

        #endregion
    }
}
