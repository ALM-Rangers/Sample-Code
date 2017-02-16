// -----------------------------------------------------------------------
// <copyright file="XmlSerializerRequest.cs" company="Microsoft">
//      Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace ClientProxies
{
    using System.CodeDom;
    using System.IO;
    using System.Reflection;
    using System.ServiceModel;
    using System.Text;
    using GeneratedContracts;
    using GeneratedContractsAsmx;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.WcfUnit.Library;
    using Microsoft.WcfUnit.Library.Test;

    /// <summary>Summary description for <see cref="XmlSerializerRequest"/> class.</summary>
    [System.SerializableAttribute]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://tempuri.org/")]
    public class XmlSerializerRequest
    {
        /// <summary>Summary description for class level field of type <c>string</c>.</summary>
        private string sField;

        /// <summary>Summary description for class level field of type <c>int</c>.</summary>
        private int iField;

        /// <summary>Gets or sets the value for s.</summary>
        /// <remarks>TODO: Summary description for this remark.</remarks>
        [System.Xml.Serialization.XmlAttributeAttribute]
        public string s
        {
            get
            {
                return this.sField;
            }

            set
            {
                this.sField = value;
            }
        }

        /// <summary>Gets or sets the value for i.</summary>
        /// <remarks>TODO: Summary description for this remark.</remarks>
        [System.Xml.Serialization.XmlAttributeAttribute]
        public int i
        {
            get
            {
                return this.iField;
            }

            set
            {
                this.iField = value;
            }
        }
    }
}
