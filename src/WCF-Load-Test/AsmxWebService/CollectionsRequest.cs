//---------------------------------------------------------------------
// <copyright file="CollectionsRequest.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
//     THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
//     OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
//     LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR
//     FITNESS FOR A PARTICULAR PURPOSE.
// </copyright>
// <summary>The CollectionRequest type.</summary>
//---------------------------------------------------------------------

namespace AsmxWebService
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    public class CollectionsRequest
    {
        public ArrayList ArrayList;

        public List<int> IntList;

        public List<SimpleAsmxRequest> RequestList;

        public Collection<SimpleAsmxRequest> RequestCollection;

        public NonGenericEnumerableOnlyCollection NonGenericEnumerableOnlyCollection;
    }
}
