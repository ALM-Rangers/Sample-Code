// -----------------------------------------------------------------------
// <copyright file="NestedCompoundType.cs" company="Microsoft">
//      Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.WcfUnit.Library.Test
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data;
    using System.IO;
    using System.Runtime.Serialization;
    using System.ServiceModel;
    using System.Xml;
    using System.Xml.Serialization;

    /// <summary>Summary description for <see cref="NestedCompoundType"/> class.</summary>
    [DataContract]
    public class NestedCompoundType
    {
        /// <summary>Summary description for class level field of type <c>Microsoft.WcfUnit.Library.Test.SimpleCompoundTypeNoContract</c>.</summary>
        private SimpleCompoundTypeNoContract ShouldIgnore;

        /// <summary>Summary description for class level field of type <c>Microsoft.WcfUnit.Library.Test.SimpleCompoundTypeDataContract</c>.</summary>
        private SimpleCompoundTypeDataContract _compound1;

        /// <summary>Summary description for class level field of type <c>Microsoft.WcfUnit.Library.Test.SimpleCompoundTypeDataContract</c>.</summary>
        private SimpleCompoundTypeDataContract _compound2;

        /// <summary>Gets or sets the value for Compound1.</summary>
        [DataMember]
        public SimpleCompoundTypeDataContract Compound1
        {
            get { return this._compound1; }
            set { this._compound1 = value; }
        }

        /// <summary>Gets or sets the value for Compound2.</summary>
        [DataMember]
        public SimpleCompoundTypeDataContract Compound2
        {
            get { return this._compound2; }
            set { this._compound2 = value; }
        }
    }
}
